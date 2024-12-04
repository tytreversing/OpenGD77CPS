using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UsbLibrary;

namespace DMR;

internal class FirmwareLoader_MK22
{
	public enum OutputType
	{
		OutputType_GD77,
		OutputType_GD77S,
		OutputType_DM1801,
		OutputType_RD5R,
		OutputType_UNKNOWN
	}

	private class StringAndOutputType
	{
		public byte[] Model { get; set; }

		public OutputType Type { get; set; }
	}

	private static readonly byte[] responseOK = new byte[1] { 65 };

	private static readonly int VENDOR_ID = 5538;

	private static readonly int PRODUCT_ID = 115;

	private static SpecifiedDevice _specifiedDevice = null;

	private static FirmwareLoaderUI_MK22 _progessForm;

	private static byte[] _languageBuf;

	public static OutputType outputType = OutputType.OutputType_GD77;

	public static string getModelString(OutputType type)
	{
		return type switch
		{
			OutputType.OutputType_GD77 => "GD-77", 
			OutputType.OutputType_GD77S => "GD-77S", 
			OutputType.OutputType_DM1801 => "DM-1801", 
			OutputType.OutputType_RD5R => "RD-5R", 
			_ => "Unknown", 
		};
	}

	public static string getModelSaveFileString(OutputType type)
	{
		return type switch
		{
			OutputType.OutputType_GD77 => "OpenGD77", 
			OutputType.OutputType_GD77S => "OpenGD77S", 
			OutputType.OutputType_DM1801 => "OpenDM1801", 
			OutputType.OutputType_RD5R => "OpenRD5R", 
			_ => "Unknown", 
		};
	}

	public static string getModelName()
	{
		return getModelString(outputType);
	}

	public static int UploadFirmware(byte[] fileBuf, byte[] languageBuf, FirmwareLoaderUI_MK22 progessForm = null)
	{
		byte[] encodeKey = null;
		_languageBuf = languageBuf;
		_progessForm = progessForm;
		switch (outputType)
		{
		case OutputType.OutputType_GD77:
			encodeKey = new byte[4] { 97, 109, 110, 98 };
			break;
		case OutputType.OutputType_GD77S:
			encodeKey = new byte[4] { 109, 64, 125, 99 };
			break;
		case OutputType.OutputType_DM1801:
			encodeKey = new byte[4] { 116, 33, 68, 57 };
			break;
		case OutputType.OutputType_RD5R:
			encodeKey = new byte[4] { 83, 54, 55, 98 };
			break;
		case OutputType.OutputType_UNKNOWN:
			return -99;
		}
		_specifiedDevice = SpecifiedDevice.FindSpecifiedDevice(VENDOR_ID, PRODUCT_ID);
		if (_specifiedDevice == null)
		{
			_progessForm.SetLabel(string.Format(FirmwareLoaderUI_MK22.StringsDict["Error._Cant_connect_to_the"] + " {0}", getModelName()));
			return -1;
		}
		if (fileBuf.Length > 505856)
		{
			_progessForm.SetLabel(FirmwareLoaderUI_MK22.StringsDict["Error._Firmware_file_too_large"]);
			_specifiedDevice.Dispose();
			_specifiedDevice = null;
			return -100;
		}
		if (FirmwareLoaderUI_MK22.officialFirmwareFile != "")
		{
			_progessForm.SetLabel(FirmwareLoaderUI_MK22.StringsDict["Patching_the_official_firmware"]);
			byte[] array = File.ReadAllBytes(FirmwareLoaderUI_MK22.officialFirmwareFile);
			int num = 493569;
			byte[] array2 = new byte[Math.Max(fileBuf.Length, 493569)];
			Buffer.BlockCopy(array, array.Length - num, array2, 0, num);
			byte[] array3 = encrypt(array2, doEncrypt: false);
			Buffer.BlockCopy(fileBuf, 0, array3, 0, 1024);
			Buffer.BlockCopy(fileBuf, 4864, array3, 4864, 322816);
			Buffer.BlockCopy(fileBuf, 491664, array3, 491664, fileBuf.Length - 491664);
			fileBuf = array3;
		}
		_progessForm.SetLabel(FirmwareLoaderUI_MK22.StringsDict["Firmware_file_is_unencrypted_binary"]);
		if (_languageBuf != null)
		{
			new Utils().MergeLanguageFile(_progessForm, ref fileBuf, _languageBuf);
		}
		fileBuf = encrypt(fileBuf);
		if (sendInitialCommands(encodeKey))
		{
			int num2 = sendFileData(fileBuf);
			switch (num2)
			{
			case 0:
				_progessForm.SetLabel(FirmwareLoaderUI_MK22.StringsDict["Success"]);
				MessageBox.Show(_progessForm, string.Format(FirmwareLoaderUI_MK22.StringsDict["Firmware_update_complete"] + " {0}", getModelName()), FirmwareLoaderUI_MK22.StringsDict["Success"], MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				_specifiedDevice.Dispose();
				_specifiedDevice = null;
				return 0;
			case -1:
				_progessForm.SetLabel(FirmwareLoaderUI_MK22.StringsDict["Error._Firmware_file_too_large"]);
				break;
			case -5:
			case -4:
			case -3:
			case -2:
				_progessForm.SetLabel(FirmwareLoaderUI_MK22.StringsDict["Error"] + " " + num2 + " " + FirmwareLoaderUI_MK22.StringsDict["While_sending_data_file"]);
				break;
			}
			_specifiedDevice.Dispose();
			_specifiedDevice = null;
			return -3;
		}
		_progessForm.SetLabel(string.Format(FirmwareLoaderUI_MK22.StringsDict["Error_while_sending_initial_commands._Is_the_{0}_in_firmware_update_mode?"], getModelName()));
		_specifiedDevice.Dispose();
		_specifiedDevice = null;
		return -4;
	}

	private static byte[] sendAndGetResponse(byte[] cmd)
	{
		byte[] array = new byte[38];
		if (_specifiedDevice != null)
		{
			_specifiedDevice.SendData(cmd);
			_specifiedDevice.ReceiveData(array);
		}
		return array;
	}

	private static bool sendAndCheckResponse(byte[] cmd, byte[] resp)
	{
		byte[] array = new byte[38];
		byte[] array2 = new byte[38];
		if (resp.Length < 38)
		{
			Buffer.BlockCopy(resp, 0, array, 0, resp.Length);
		}
		_specifiedDevice.SendData(cmd);
		_specifiedDevice.ReceiveData(array2);
		if (array2.SequenceEqual(array))
		{
			return true;
		}
		_progessForm.SetLabel(FirmwareLoaderUI_MK22.StringsDict["Error_read_returned"]);
		return false;
	}

	private static byte[] createChecksumData(byte[] buf, int startAddress, int endAddress)
	{
		byte[] array = new byte[8] { 69, 78, 68, 255, 222, 173, 190, 239 };
		int num = 0;
		for (int i = startAddress; i < endAddress; i++)
		{
			num += buf[i];
		}
		array[4] = (byte)(num % 256);
		array[5] = (byte)((num >> 8) % 256);
		array[6] = (byte)((num >> 16) % 256);
		array[7] = (byte)((num >> 24) % 256);
		return array;
	}

	private static void updateBlockAddressAndLength(byte[] buf, int address, int length)
	{
		buf[5] = (byte)(length % 256);
		buf[4] = (byte)((length >> 8) % 256);
		buf[3] = (byte)(address % 256);
		buf[2] = (byte)((address >> 8) % 256);
		buf[1] = (byte)((address >> 16) % 256);
		buf[0] = (byte)((address >> 24) % 256);
	}

	private static int sendFileData(byte[] fileBuf)
	{
		byte[] array = new byte[38];
		int num = 32;
		int startAddress = 0;
		int num2 = 0;
		if (_progessForm != null)
		{
			_progessForm.SetLabel(string.Format(FirmwareLoaderUI_MK22.StringsDict["Uploading_firmware_to"] + "{0}", getModelName()));
		}
		int num3 = fileBuf.Length;
		int num4 = num3 / 1024 + 1;
		while (num2 < num3)
		{
			if (num2 % 1024 == 0)
			{
				startAddress = num2;
			}
			updateBlockAddressAndLength(array, num2, num);
			if (num2 + num < num3)
			{
				Buffer.BlockCopy(fileBuf, num2, array, 6, 32);
				if (!sendAndCheckResponse(array, responseOK))
				{
					_progessForm.SetLabel(FirmwareLoaderUI_MK22.StringsDict["Error_sending_data"]);
					return -2;
				}
				num2 += num;
				if (num2 % 1024 == 0)
				{
					if (_progessForm != null)
					{
						_progessForm.SetProgressPercentage(num2 * 100 / 1024 / num4);
					}
					if (!sendAndCheckResponse(createChecksumData(fileBuf, startAddress, num2), responseOK))
					{
						_progessForm.SetLabel(FirmwareLoaderUI_MK22.StringsDict["Error_sending_checksum"]);
						return -3;
					}
				}
			}
			else
			{
				num = num3 - num2;
				updateBlockAddressAndLength(array, num2, num);
				Buffer.BlockCopy(fileBuf, num2, array, 6, num);
				if (!sendAndCheckResponse(array, responseOK))
				{
					_progessForm.SetLabel(FirmwareLoaderUI_MK22.StringsDict["Error_sending_data"]);
					return -4;
				}
				num2 += num;
				if (!sendAndCheckResponse(createChecksumData(fileBuf, startAddress, num2), responseOK))
				{
					_progessForm.SetLabel(FirmwareLoaderUI_MK22.StringsDict["Error_sending_checksum"]);
					return -5;
				}
			}
		}
		return 0;
	}

	public static OutputType probeModel()
	{
		byte[] array = new byte[1] { 65 };
		byte[][] array2 = new byte[2][]
		{
			new byte[8] { 68, 79, 87, 78, 76, 79, 65, 68 },
			new byte[8] { 35, 85, 80, 68, 65, 84, 69, 63 }
		};
		byte[][] array3 = new byte[2][] { array, responseOK };
		byte[] cmd = new byte[8] { 255, 255, 255, 255, 255, 255, 255, 255 };
		byte[][][] array4 = new byte[2][][] { array2, array3 };
		StringAndOutputType[] array5 = new StringAndOutputType[4]
		{
			new StringAndOutputType
			{
				Model = Encoding.ASCII.GetBytes("DV01"),
				Type = OutputType.OutputType_GD77
			},
			new StringAndOutputType
			{
				Model = Encoding.ASCII.GetBytes("DV02"),
				Type = OutputType.OutputType_GD77S
			},
			new StringAndOutputType
			{
				Model = Encoding.ASCII.GetBytes("DV03"),
				Type = OutputType.OutputType_DM1801
			},
			new StringAndOutputType
			{
				Model = Encoding.ASCII.GetBytes("DV02"),
				Type = OutputType.OutputType_RD5R
			}
		};
		int i = 0;
		_specifiedDevice = SpecifiedDevice.FindSpecifiedDevice(VENDOR_ID, PRODUCT_ID);
		if (_specifiedDevice == null)
		{
			return OutputType.OutputType_UNKNOWN;
		}
		for (; i < array4.Length; i++)
		{
			if (!sendAndCheckResponse(array4[i][0], array4[i][1]))
			{
				_specifiedDevice.Dispose();
				_specifiedDevice = null;
				return OutputType.OutputType_UNKNOWN;
			}
		}
		byte[] array6 = sendAndGetResponse(cmd);
		if (array6.Length >= 4)
		{
			StringAndOutputType[] array7 = array5;
			foreach (StringAndOutputType stringAndOutputType in array7)
			{
				if (stringAndOutputType.Model.SequenceEqual(array6.ToList().GetRange(0, 4).ToArray()))
				{
					_specifiedDevice.Dispose();
					_specifiedDevice = null;
					return stringAndOutputType.Type;
				}
			}
		}
		_specifiedDevice.Dispose();
		_specifiedDevice = null;
		return OutputType.OutputType_UNKNOWN;
	}

	private static bool sendInitialCommands(byte[] encodeKey)
	{
		byte[] array = new byte[1] { 65 };
		byte[][] array2 = new byte[2][]
		{
			new byte[8] { 68, 79, 87, 78, 76, 79, 65, 68 },
			new byte[8] { 35, 85, 80, 68, 65, 84, 69, 63 }
		};
		byte[][] array3 = new byte[2][] { array, responseOK };
		byte[][] array4 = null;
		byte[][] array5 = new byte[2][]
		{
			new byte[8] { 70, 45, 80, 82, 79, 71, 255, 255 },
			responseOK
		};
		byte[][] array6 = null;
		byte[][] array7 = null;
		byte[][] array8 = new byte[2][]
		{
			new byte[8] { 86, 49, 46, 48, 48, 46, 48, 49 },
			responseOK
		};
		switch (outputType)
		{
		case OutputType.OutputType_GD77:
			array4 = new byte[2][]
			{
				new byte[8] { 68, 86, 48, 49, 97, 109, 110, 98 },
				new byte[4] { 68, 86, 48, 49 }
			};
			array6 = new byte[2][]
			{
				new byte[16]
				{
					83, 71, 45, 77, 68, 45, 55, 54, 48, 255,
					255, 255, 255, 255, 255, 255
				},
				responseOK
			};
			array7 = new byte[2][]
			{
				new byte[8] { 77, 68, 45, 55, 54, 48, 255, 255 },
				responseOK
			};
			break;
		case OutputType.OutputType_GD77S:
			array4 = new byte[2][]
			{
				new byte[8] { 68, 86, 48, 50, 109, 64, 125, 99 },
				new byte[4] { 68, 86, 48, 50 }
			};
			array6 = new byte[2][]
			{
				new byte[16]
				{
					83, 71, 45, 77, 68, 45, 55, 51, 48, 255,
					255, 255, 255, 255, 255, 255
				},
				responseOK
			};
			array7 = new byte[2][]
			{
				new byte[8] { 77, 68, 45, 55, 51, 48, 255, 255 },
				responseOK
			};
			break;
		case OutputType.OutputType_DM1801:
			array4 = new byte[2][]
			{
				new byte[8] { 68, 86, 48, 51, 116, 33, 68, 57 },
				new byte[4] { 68, 86, 48, 51 }
			};
			array6 = new byte[2][]
			{
				new byte[16]
				{
					66, 70, 45, 68, 77, 82, 255, 255, 255, 255,
					255, 255, 255, 255, 255, 255
				},
				responseOK
			};
			array7 = new byte[2][]
			{
				new byte[8] { 49, 56, 48, 49, 255, 255, 255, 255 },
				responseOK
			};
			break;
		case OutputType.OutputType_RD5R:
			array4 = new byte[2][]
			{
				new byte[8] { 68, 86, 48, 50, 83, 54, 55, 98 },
				new byte[4] { 68, 86, 48, 50 }
			};
			array6 = new byte[2][]
			{
				new byte[16]
				{
					66, 70, 45, 53, 82, 255, 255, 255, 255, 255,
					255, 255, 255, 255, 255, 255
				},
				responseOK
			};
			array7 = new byte[2][]
			{
				new byte[8] { 66, 70, 45, 53, 82, 255, 255, 255 },
				responseOK
			};
			break;
		}
		byte[][] array9 = new byte[2][]
		{
			new byte[8] { 70, 45, 69, 82, 65, 83, 69, 255 },
			responseOK
		};
		byte[][] array10 = new byte[2][] { array, responseOK };
		byte[][] array11 = new byte[2][]
		{
			new byte[8] { 80, 82, 79, 71, 82, 65, 77, 15 },
			responseOK
		};
		byte[][][] array12 = new byte[10][][] { array2, array3, array4, array5, array6, array7, array8, array9, array10, array11 };
		string[] array13 = new string[10]
		{
			FirmwareLoaderUI_MK22.StringsDict["Sending_Download_command"],
			FirmwareLoaderUI_MK22.StringsDict["Sending_ACK"],
			FirmwareLoaderUI_MK22.StringsDict["Sending_encryption_key"],
			FirmwareLoaderUI_MK22.StringsDict["Sending_F-PROG_command"],
			FirmwareLoaderUI_MK22.StringsDict["Sending_radio_modem_number"],
			FirmwareLoaderUI_MK22.StringsDict["Sending_radio_modem_number_2"],
			FirmwareLoaderUI_MK22.StringsDict["Sending_version"],
			FirmwareLoaderUI_MK22.StringsDict["Sending_erase_command"],
			FirmwareLoaderUI_MK22.StringsDict["Send_post_erase_command"],
			FirmwareLoaderUI_MK22.StringsDict["Sending_Program_command"]
		};
		int i = 0;
		Buffer.BlockCopy(encodeKey, 0, array4[0], 4, 4);
		for (; i < array12.Length; i++)
		{
			if (_progessForm != null)
			{
				_progessForm.SetLabel(array13[i]);
			}
			if (!sendAndCheckResponse(array12[i][0], array12[i][1]))
			{
				_progessForm.SetLabel(FirmwareLoaderUI_MK22.StringsDict["Error_sending_command"]);
				return false;
			}
		}
		return true;
	}

	private static byte[] encrypt(byte[] unencrypted, bool doEncrypt = true)
	{
		int num = 17611;
		int num2 = unencrypted.Length;
		if (doEncrypt)
		{
			switch (outputType)
			{
			case OutputType.OutputType_GD77:
				num = 2055;
				break;
			case OutputType.OutputType_GD77S:
				num = 10894;
				break;
			case OutputType.OutputType_DM1801:
				num = 11388;
				break;
			case OutputType.OutputType_RD5R:
				num = 12398;
				break;
			}
		}
		byte[] array = new byte[num2];
		for (int i = 0; i < num2; i++)
		{
			int num3;
			try
			{
				num3 = unencrypted[i];
			}
			catch (IndexOutOfRangeException)
			{
				num3 = 255;
			}
			if (doEncrypt)
			{
				num3 = (byte)num3 ^ FirmwareDataTable_MK22.EncryptionTable[num++];
				num3 = ~(((num3 >> 3) & 0x1F) | ((num3 << 5) & 0xE0));
			}
			else
			{
				num3 = ~(((num3 << 3) & 0xF8) | ((num3 >> 5) & 7));
				num3 = (byte)num3 ^ FirmwareDataTable_MK22.EncryptionTable[num++];
			}
			array[i] = (byte)num3;
			if (num >= 32767)
			{
				num = 0;
			}
		}
		return array;
	}

	private static byte[] checkForSGLAndReturnEncryptedData(byte[] fileBuf, byte[] encodeKey, ref byte headerModel)
	{
		byte[] array = new byte[4] { 83, 71, 76, 33 };
		byte[] array2 = new byte[4];
		Buffer.BlockCopy(fileBuf, 0, array2, 0, array2.Length);
		headerModel = Buffer.GetByte(fileBuf, 11);
		if (array2.SequenceEqual(array))
		{
			Buffer.BlockCopy(fileBuf, 12, array2, 0, array2.Length);
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] ^= array[i];
			}
			int num = array2[0] + 256 * array2[1];
			byte[] array3 = new byte[2]
			{
				array2[2],
				array2[3]
			};
			byte[] array4 = new byte[512];
			Buffer.BlockCopy(fileBuf, num + 6, array4, 0, array4.Length);
			int num2 = 0;
			for (int j = 0; j < array4.Length; j++)
			{
				array4[j] ^= array3[num2];
				num2++;
				if (num2 == 2)
				{
					num2 = 0;
				}
			}
			Buffer.BlockCopy(array4, 93, encodeKey, 0, 4);
			byte b = array4[0];
			byte b2 = array4[1];
			byte b3 = array4[2];
			int num3 = (array4[3] << 24) + (b3 << 16) + (b2 << 8) + b;
			byte[] array5 = new byte[num3];
			Buffer.BlockCopy(fileBuf, fileBuf.Length - num3, array5, 0, array5.Length);
			return array5;
		}
		_progessForm.SetLabel(FirmwareLoaderUI_MK22.StringsDict["ERROR:_SGL!_header_missing."]);
		return null;
	}
}
