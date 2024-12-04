using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DMR;
using UsbLibrary;

internal class CodeplugComms
{
	public delegate void Delegate1(object sender, FirmwareUpdateProgressEventArgs e);

	public enum CommunicationType
	{
		codeplugRead,
		codeplugWrite,
		DMRIDRead,
		DMRIDWrite,
		calibrationRead,
		calibrationWrite,
		dataRead,
		dataWrite
	}

	private const int HEAD_LEN = 4;

	private const int MAX_COMM_LEN = 32;

	private const byte CMD_WRITE = 87;

	private const byte CMD_READ = 82;

	private const byte CMD_CMD = 67;

	private const byte CMD_BASE = 66;

	private const int MaxReadTimeout = 5000;

	private const int MaxWriteTimeout = 1000;

	private const int MaxBuf = 160;

	private const float IndexListPercent = 5f;

	private const int HID_VID = 5538;

	private const int HID_PID = 115;

	private static readonly byte[] CMD_ENDR = Encoding.ASCII.GetBytes("ENDR");

	private static readonly byte[] CMD_ENDW = Encoding.ASCII.GetBytes("ENDW");

	private static readonly byte[] CMD_ACK = new byte[1] { 65 };

	private static readonly byte[] CMD_PRG = new byte[7] { 2, 80, 82, 79, 71, 82, 65 };

	private static readonly byte[] CMD_PRG2 = new byte[2] { 77, 2 };

	public int[] START_ADDR;

	public int[] END_ADDR;

	private Thread thread;

	private Delegate1 OnFirmwareUpdateProgress;

	private bool _CancelComm;

	public static int transferLength;

	public static int startAddress;

	public static CommunicationType CommunicationMode { get; set; }

	[CompilerGenerated]
	public bool method_0()
	{
		return _CancelComm;
	}

	[CompilerGenerated]
	public void SetCancelComm(bool bool_0)
	{
		_CancelComm = bool_0;
	}

	public bool isThreadAlive()
	{
		if (thread != null)
		{
			return thread.IsAlive;
		}
		return false;
	}

	public void JoinThreadIfAlive()
	{
		if (isThreadAlive())
		{
			thread.Join();
		}
	}

	public void startCodeplugReadOrWriteInNewThread()
	{
		thread = null;
		switch (CommunicationMode)
		{
		case CommunicationType.codeplugRead:
			thread = new Thread(readCodeplug);
			break;
		case CommunicationType.codeplugWrite:
			thread = new Thread(writeCodeplug);
			break;
		case CommunicationType.DMRIDRead:
			startAddress = 196608;
			transferLength = 131072;
			thread = new Thread(readData);
			break;
		case CommunicationType.DMRIDWrite:
			startAddress = 196608;
			transferLength = 131072;
			thread = new Thread(writeData);
			break;
		case CommunicationType.calibrationRead:
			startAddress = CalibrationForm.CALIBRATION_MEMORY_LOCATION_OFFICIAL_USB_PROTOCOL;
			transferLength = 4096;
			thread = new Thread(readData);
			break;
		case CommunicationType.calibrationWrite:
			startAddress = CalibrationForm.CALIBRATION_MEMORY_LOCATION_OFFICIAL_USB_PROTOCOL;
			transferLength = 4096;
			thread = new Thread(writeData);
			break;
		case CommunicationType.dataWrite:
			thread = new Thread(writeData);
			break;
		case CommunicationType.dataRead:
			thread = new Thread(readData);
			break;
		}
		if (thread != null)
		{
			thread.Start();
		}
	}

	public static string ByteArrayToString(byte[] ba)
	{
		return BitConverter.ToString(ba).Replace("-", "");
	}

	public void readData()
	{
		byte[] array = new byte[160];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		SpecifiedDevice specifiedDevice = null;
		try
		{
			specifiedDevice = SpecifiedDevice.FindSpecifiedDevice(5538, 115);
			if (specifiedDevice == null)
			{
				if (OnFirmwareUpdateProgress != null)
				{
					OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, "Device not found", Failed: true, Closed: true));
				}
			}
			else
			{
				Array.Clear(array, 0, array.Length);
				specifiedDevice.SendData(CMD_PRG);
				specifiedDevice.ReceiveData(array);
				if (array[0] == CMD_ACK[0])
				{
					specifiedDevice.SendData(CMD_PRG2);
					Array.Clear(array, 0, array.Length);
					specifiedDevice.ReceiveData(array);
					byte[] dst = new byte[8];
					Buffer.BlockCopy(array, 0, dst, 0, 8);
					specifiedDevice.SendData(CMD_ACK);
					Array.Clear(array, 0, array.Length);
					specifiedDevice.ReceiveData(array);
					if (array[0] == CMD_ACK[0])
					{
						num = 32;
						int num5 = 0;
						int num6 = 65536;
						int num7 = transferLength / num;
						int num8 = startAddress / num;
						int num9 = num8;
						while (true)
						{
							if (num9 < num8 + num7)
							{
								if (num5 != num9 * num / num6)
								{
									num5 = num9 * num / num6;
									num2 = num * num9;
									byte[] array2 = new byte[8] { 67, 87, 66, 4, 0, 0, 0, 0 };
									num4 = num2 >> 16 << 16;
									array2[4] = (byte)(num4 >> 24);
									array2[5] = (byte)(num4 >> 16);
									array2[6] = (byte)(num4 >> 8);
									array2[7] = (byte)num4;
									Console.WriteLine(SpecifiedDevice.ByteArrayToString(array2));
									Array.Clear(array, 0, array.Length);
									specifiedDevice.SendData(array2, 0, array2.Length);
									specifiedDevice.ReceiveData(array);
									if (array[0] != CMD_ACK[0])
									{
										break;
									}
								}
								num3 = (num9 * num) & 0xFFFF;
								byte[] data = new byte[4]
								{
									82,
									(byte)(num3 >> 8),
									(byte)num3,
									(byte)num
								};
								Array.Clear(array, 0, array.Length);
								specifiedDevice.SendData(data, 0, 4);
								if (specifiedDevice.ReceiveData(array))
								{
									Buffer.BlockCopy(array, 4, MainForm.CommsBuffer, num9 * num, num);
									if (OnFirmwareUpdateProgress != null)
									{
										OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs((float)(num9 + 1 - num8) * 100f / (float)num7, "", Failed: false, Closed: false));
									}
									num9++;
									continue;
								}
								break;
							}
							specifiedDevice.SendData(CMD_ENDR);
							specifiedDevice.ReceiveData(array);
							break;
						}
					}
				}
			}
		}
		catch (TimeoutException ex)
		{
			Console.WriteLine(ex.Message);
			if (OnFirmwareUpdateProgress != null)
			{
				OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, "Comms error", Failed: false, Closed: false));
			}
		}
		finally
		{
			specifiedDevice?.Dispose();
		}
		if (OnFirmwareUpdateProgress != null)
		{
			OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100f, "", Failed: false, Closed: true));
		}
	}

	public void writeData()
	{
		byte[] array = new byte[160];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		SpecifiedDevice specifiedDevice = null;
		try
		{
			specifiedDevice = SpecifiedDevice.FindSpecifiedDevice(5538, 115);
			if (specifiedDevice == null)
			{
				if (OnFirmwareUpdateProgress != null)
				{
					OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, "Device not found", Failed: true, Closed: true));
				}
			}
			else
			{
				Array.Clear(array, 0, array.Length);
				specifiedDevice.SendData(CMD_PRG);
				specifiedDevice.ReceiveData(array);
				if (array[0] == CMD_ACK[0])
				{
					specifiedDevice.SendData(CMD_PRG2);
					Array.Clear(array, 0, array.Length);
					specifiedDevice.ReceiveData(array);
					byte[] dst = new byte[8];
					Buffer.BlockCopy(array, 0, dst, 0, 8);
					specifiedDevice.SendData(CMD_ACK);
					Array.Clear(array, 0, array.Length);
					specifiedDevice.ReceiveData(array);
					if (array[0] == CMD_ACK[0])
					{
						num = 32;
						int num5 = 0;
						int num6 = 65536;
						int num7 = transferLength / num;
						int num8 = startAddress / num;
						int num9 = num8;
						while (true)
						{
							if (num9 < num8 + num7)
							{
								if (num5 != num9 * num / num6)
								{
									num5 = num9 * num / num6;
									num2 = num * num9;
									byte[] array2 = new byte[8] { 67, 87, 66, 4, 0, 0, 0, 0 };
									num4 = num2 >> 16 << 16;
									array2[4] = (byte)(num4 >> 24);
									array2[5] = (byte)(num4 >> 16);
									array2[6] = (byte)(num4 >> 8);
									array2[7] = (byte)num4;
									Array.Clear(array, 0, array.Length);
									specifiedDevice.SendData(array2, 0, array2.Length);
									specifiedDevice.ReceiveData(array);
									if (array[0] != CMD_ACK[0])
									{
										goto IL_025d;
									}
								}
								num3 = (num9 * num) & 0xFFFF;
								byte[] array3 = new byte[36];
								array3[0] = 87;
								array3[1] = (byte)(num3 >> 8);
								array3[2] = (byte)num3;
								array3[3] = (byte)num;
								byte[] array4 = array3;
								Array.Clear(array, 0, array.Length);
								Buffer.BlockCopy(MainForm.CommsBuffer, num9 * num, array4, 4, num);
								specifiedDevice.SendData(array4, 0, 36);
								specifiedDevice.ReceiveData(array);
								if (array[0] == CMD_ACK[0])
								{
									if (OnFirmwareUpdateProgress != null)
									{
										OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs((float)(num9 + 1 - num8) * 100f / (float)num7, "", Failed: false, Closed: false));
									}
									num9++;
									continue;
								}
								goto IL_025d;
							}
							specifiedDevice.SendData(CMD_ENDW);
							specifiedDevice.ReceiveData(array);
							break;
							IL_025d:
							specifiedDevice.SendData(CMD_ENDW);
							specifiedDevice.ReceiveData(array);
							break;
						}
					}
				}
			}
		}
		catch (TimeoutException ex)
		{
			Console.WriteLine(ex.Message);
			if (OnFirmwareUpdateProgress != null)
			{
				OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, "Error", Failed: false, Closed: false));
			}
		}
		finally
		{
			specifiedDevice?.Dispose();
		}
		if (OnFirmwareUpdateProgress != null)
		{
			OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100f, "", Failed: false, Closed: true));
		}
	}

	public void readCodeplug()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		byte[] array = new byte[Settings.EEROM_SPACE];
		byte[] array2 = new byte[160];
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		int num10 = 0;
		int num11 = 0;
		bool flag = false;
		float num12 = 0f;
		int num13 = 0;
		SpecifiedDevice specifiedDevice = null;
		try
		{
			specifiedDevice = SpecifiedDevice.FindSpecifiedDevice(5538, 115);
			if (specifiedDevice == null)
			{
				if (OnFirmwareUpdateProgress != null)
				{
					OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, Settings.SZ_DEVICE_NOT_FOUND, Failed: true, Closed: true));
				}
				return;
			}
			while (true)
			{
				Array.Clear(array2, 0, array2.Length);
				specifiedDevice.SendData(CMD_PRG);
				specifiedDevice.ReceiveData(array2);
				if (array2[0] != CMD_ACK[0])
				{
					break;
				}
				specifiedDevice.SendData(CMD_PRG2);
				Array.Clear(array2, 0, array2.Length);
				specifiedDevice.ReceiveData(array2);
				byte[] array3 = new byte[8];
				Buffer.BlockCopy(array2, 0, array3, 0, 8);
				if (array3.smethod_4(Settings.CUR_MODEL))
				{
					specifiedDevice.SendData(CMD_ACK);
					Array.Clear(array2, 0, array2.Length);
					specifiedDevice.ReceiveData(array2);
					if (array2[0] != CMD_ACK[0])
					{
						break;
					}
					if (!flag && Settings.CUR_PWD != "DT8168")
					{
						num7 = Settings.ADDR_PWD;
						num5 = 8;
						byte[] data = new byte[4]
						{
							82,
							(byte)(num7 >> 8),
							(byte)num7,
							8
						};
						Array.Clear(array2, 0, array2.Length);
						specifiedDevice.SendData(data, 0, 4);
						specifiedDevice.ReceiveData(array2);
						string text = "";
						for (num = 0; num < 8; num++)
						{
							char value = Convert.ToChar(array2[num + 4]);
							if ("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz\b".IndexOf(value) < 0)
							{
								break;
							}
							text += value;
						}
						if (string.IsNullOrEmpty(text))
						{
							Array.Clear(array2, 0, array2.Length);
							specifiedDevice.SendData(CMD_ENDR);
							specifiedDevice.ReceiveData(array2);
							if (array2[0] != CMD_ACK[0])
							{
								break;
							}
							flag = true;
						}
						else if (text != Settings.CUR_PWD)
						{
							Array.Clear(array2, 0, array2.Length);
							specifiedDevice.SendData(CMD_ENDR);
							specifiedDevice.ReceiveData(array2);
							if (array2[0] != CMD_ACK[0])
							{
								break;
							}
							Settings.CUR_PWD = "";
							if (new PasswordForm().ShowDialog() != DialogResult.OK)
							{
								OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, "", Failed: true, Closed: true));
								return;
							}
							num13++;
							if (num13 >= 3)
							{
								OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, "Password error more than three times, quit communication！", Failed: true, Closed: true));
								return;
							}
						}
						else
						{
							Array.Clear(array2, 0, array2.Length);
							specifiedDevice.SendData(CMD_ENDR);
							specifiedDevice.ReceiveData(array2);
							if (array2[0] != CMD_ACK[0])
							{
								break;
							}
							flag = true;
						}
						continue;
					}
					List<int> list3 = new List<int>();
					List<int> list4 = new List<int>();
					list3.Add(Settings.ADDR_CHANNEL);
					list4.Add(Settings.ADDR_CHANNEL + 16);
					for (num2 = 1; num2 < 8; num2++)
					{
						num9 = Settings.ADDR_EX_CH + (num2 - 1) * ChannelForm.SPACE_CH_GROUP;
						list3.Add(num9);
						list4.Add(num9 + 16);
					}
					num9 = Settings.ADDR_EX_ZONE_LIST;
					list3.Add(num9);
					list4.Add(num9 + 32);
					num3 = 0;
					num4 = 0;
					for (num = 0; num < list3.Count; num++)
					{
						num10 = list3[num];
						num11 = list4[num];
						for (num7 = num10; num7 < num11; num7 += num5)
						{
							num6 = num7 % 32;
							num5 = ((num7 + 32 <= num11) ? (32 - num6) : (num11 - num7));
							num4++;
						}
					}
					num9 = 0;
					num = 0;
					while (true)
					{
						if (num < list3.Count)
						{
							num10 = list3[num];
							num11 = list4[num];
							for (num7 = num10; num7 < num11; num7 += num5)
							{
								if (!method_0())
								{
									if (num9 >> 16 != num7 >> 16)
									{
										byte[] array4 = new byte[8] { 67, 87, 66, 4, 0, 0, 0, 0 };
										num9 = num7 >> 16 << 16;
										array4[4] = (byte)(num9 >> 24);
										array4[5] = (byte)(num9 >> 16);
										array4[6] = (byte)(num9 >> 8);
										array4[7] = (byte)num9;
										Array.Clear(array2, 0, array2.Length);
										specifiedDevice.SendData(array4, 0, array4.Length);
										specifiedDevice.ReceiveData(array2);
										if (array2[0] != CMD_ACK[0])
										{
											goto end_IL_0092;
										}
									}
									num6 = num7 % 32;
									num5 = ((num7 + 32 <= num11) ? (32 - num6) : (num11 - num7));
									num8 = num7 - num9;
									byte[] data2 = new byte[4]
									{
										82,
										(byte)(num8 >> 8),
										(byte)num8,
										(byte)num5
									};
									Array.Clear(array2, 0, array2.Length);
									specifiedDevice.SendData(data2, 0, 4);
									if (!specifiedDevice.ReceiveData(array2))
									{
										goto end_IL_0092;
									}
									Array.Copy(array2, 4, array, num7, num5);
									if (OnFirmwareUpdateProgress != null)
									{
										OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs((float)(++num3) * 5f / (float)num4, num7.ToString(), Failed: false, Closed: false));
									}
									continue;
								}
								specifiedDevice.SendData(CMD_ENDR);
								specifiedDevice.ReceiveData(array2);
								return;
							}
							num++;
							continue;
						}
						byte[] array5 = new byte[16];
						Array.Copy(array, Settings.ADDR_CHANNEL, array5, 0, array5.Length);
						BitArray bitArray = new BitArray(array5);
						list.Add(Settings.ADDR_CHANNEL);
						list2.Add(Settings.ADDR_CHANNEL + 16);
						for (num = 0; num < 128; num++)
						{
							if (bitArray[num])
							{
								num10 = Settings.ADDR_CHANNEL + 16 + num * ChannelForm.SPACE_CH;
								num11 = num10 + ChannelForm.SPACE_CH;
								list.Add(num10);
								list2.Add(num11);
							}
						}
						for (num2 = 1; num2 < 8; num2++)
						{
							num9 = Settings.ADDR_EX_CH + (num2 - 1) * ChannelForm.SPACE_CH_GROUP;
							Array.Copy(array, num9, array5, 0, array5.Length);
							bitArray = new BitArray(array5);
							list.Add(num9);
							list2.Add(num9 + 16);
							for (num = 0; num < 128; num++)
							{
								if (bitArray[num])
								{
									num10 = num9 + 16 + num * ChannelForm.SPACE_CH;
									num11 = num10 + ChannelForm.SPACE_CH;
									list.Add(num10);
									list2.Add(num11);
								}
							}
						}
						byte[] array6 = new byte[32];
						num9 = Settings.ADDR_EX_ZONE_LIST;
						Array.Copy(array, num9, array6, 0, array6.Length);
						bitArray = new BitArray(array6);
						list.Add(num9);
						list2.Add(num9 + 32);
						for (num = 0; num < 250; num++)
						{
							num9 = Settings.ADDR_EX_ZONE_LIST + 32;
							if (bitArray[num])
							{
								num10 = num9 + num * ZoneForm.SPACE_ZONE;
								num11 = num10 + ZoneForm.SPACE_ZONE;
								list.Add(num10);
								list2.Add(num11);
							}
						}
						for (num = 0; num < START_ADDR.Length; num++)
						{
							list.Add(START_ADDR[num]);
							list2.Add(END_ADDR[num]);
						}
						num3 = 0;
						num4 = 0;
						for (num = 0; num < list.Count; num++)
						{
							num10 = list[num];
							num11 = list2[num];
							for (num7 = num10; num7 < num11; num7 += num5)
							{
								num6 = num7 % 32;
								num5 = ((num7 + 32 <= num11) ? (32 - num6) : (num11 - num7));
								num4++;
							}
						}
						num9 = 0;
						num = 0;
						while (true)
						{
							if (num < list.Count)
							{
								num10 = list[num];
								num11 = list2[num];
								num7 = num10;
								while (num7 < num11)
								{
									if (!method_0())
									{
										if (num9 >> 16 != num7 >> 16)
										{
											byte[] array7 = new byte[8] { 67, 87, 66, 4, 0, 0, 0, 0 };
											num9 = num7 >> 16 << 16;
											array7[4] = (byte)(num9 >> 24);
											array7[5] = (byte)(num9 >> 16);
											array7[6] = (byte)(num9 >> 8);
											array7[7] = (byte)num9;
											Array.Clear(array2, 0, array2.Length);
											specifiedDevice.SendData(array7, 0, array7.Length);
											specifiedDevice.ReceiveData(array2);
											if (array2[0] != CMD_ACK[0])
											{
												goto IL_0a0b;
											}
										}
										num6 = num7 % 32;
										num5 = ((num7 + 32 <= num11) ? (32 - num6) : (num11 - num7));
										num8 = num7 - num9;
										byte[] array8 = new byte[4]
										{
											82,
											(byte)(num8 >> 8),
											(byte)num8,
											(byte)num5
										};
										Array.Clear(array2, 0, array2.Length);
										specifiedDevice.SendData(array8, 0, 4);
										if (specifiedDevice.ReceiveData(array2))
										{
											if (Settings.smethod_18(array8, array2, 4))
											{
												Array.Copy(array2, 4, array, num7, num5);
												if (OnFirmwareUpdateProgress != null)
												{
													num12 = 5f + (float)(++num3) * 95f / (float)num4;
													OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(num12, num7.ToString(), Failed: false, Closed: false));
												}
												num7 += num5;
												continue;
											}
											goto IL_09d9;
										}
										goto IL_09f2;
									}
									specifiedDevice.SendData(CMD_ENDR);
									specifiedDevice.ReceiveData(array2);
									return;
								}
								num++;
								continue;
							}
							Array.Clear(array2, 0, array2.Length);
							specifiedDevice.SendData(CMD_ENDR);
							specifiedDevice.ReceiveData(array2);
							if (array2[0] != CMD_ACK[0])
							{
								break;
							}
							MainForm.ByteToData(array);
							if (OnFirmwareUpdateProgress != null)
							{
								OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100f, "", Failed: false, Closed: true));
							}
							return;
							IL_0a0b:
							specifiedDevice.SendData(CMD_ENDR);
							specifiedDevice.ReceiveData(array2);
							break;
							IL_09f2:
							specifiedDevice.SendData(CMD_ENDR);
							specifiedDevice.ReceiveData(array2);
							break;
							IL_09d9:
							specifiedDevice.SendData(CMD_ENDR);
							specifiedDevice.ReceiveData(array2);
							break;
						}
						break;
					}
					break;
				}
				if (OnFirmwareUpdateProgress != null)
				{
					OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, Settings.SZ_MODEL_NOT_MATCH, Failed: true, Closed: true));
				}
				return;
				continue;
				end_IL_0092:
				break;
			}
			if (OnFirmwareUpdateProgress != null)
			{
				OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, Settings.SZ_COMM_ERROR, Failed: true, Closed: true));
			}
		}
		catch (TimeoutException ex)
		{
			Console.WriteLine(ex.Message);
			if (OnFirmwareUpdateProgress != null)
			{
				OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, Settings.SZ_COMM_ERROR, Failed: false, Closed: false));
			}
		}
		finally
		{
			specifiedDevice?.Dispose();
		}
	}

	public void writeCodeplug()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		byte[] array = new byte[160];
		byte[] array2 = MainForm.DataToByte();
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		int num10 = 0;
		int num11 = 0;
		bool flag = false;
		byte[] array3 = new byte[6];
		int year = DateTime.Now.Year;
		int month = DateTime.Now.Month;
		int day = DateTime.Now.Day;
		int hour = DateTime.Now.Hour;
		int minute = DateTime.Now.Minute;
		array3[0] = (byte)((year / 1000 << 4) | (year / 100 % 10));
		array3[1] = (byte)((year % 100 / 10 << 4) | (year % 10));
		array3[2] = (byte)((month / 10 << 4) | (month % 10));
		array3[3] = (byte)((day / 10 << 4) | (day % 10));
		array3[4] = (byte)((hour / 10 << 4) | (hour % 10));
		array3[5] = (byte)((minute / 10 << 4) | (minute % 10));
		Array.Copy(array3, 0, array2, Settings.ADDR_DEVICE_INFO + Settings.OFS_LAST_PRG_TIME, 6);
		SpecifiedDevice specifiedDevice = SpecifiedDevice.FindSpecifiedDevice(5538, 115);
		if (specifiedDevice == null)
		{
			if (OnFirmwareUpdateProgress != null)
			{
				OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, Settings.SZ_DEVICE_NOT_FOUND, Failed: true, Closed: true));
			}
			return;
		}
		try
		{
			while (true)
			{
				specifiedDevice.SendData(CMD_PRG);
				Array.Clear(array, 0, array.Length);
				specifiedDevice.ReceiveData(array);
				if (array[0] != CMD_ACK[0])
				{
					break;
				}
				specifiedDevice.SendData(CMD_PRG2);
				Array.Clear(array, 0, array.Length);
				specifiedDevice.ReceiveData(array);
				byte[] array4 = new byte[8];
				Buffer.BlockCopy(array, 0, array4, 0, 8);
				if (array4.smethod_4(Settings.CUR_MODEL))
				{
					specifiedDevice.SendData(CMD_ACK);
					Array.Clear(array, 0, array.Length);
					specifiedDevice.ReceiveData(array);
					if (array[0] != CMD_ACK[0])
					{
						break;
					}
					if (!flag && Settings.CUR_PWD != "DT8168")
					{
						num7 = Settings.ADDR_PWD;
						num5 = 8;
						byte[] data = new byte[4]
						{
							82,
							(byte)(num7 >> 8),
							(byte)num7,
							8
						};
						Array.Clear(array, 0, array.Length);
						specifiedDevice.SendData(data, 0, 4);
						specifiedDevice.ReceiveData(array);
						string text = "";
						for (num = 0; num < 8; num++)
						{
							char value = Convert.ToChar(array[num + 4]);
							if ("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz\b".IndexOf(value) < 0)
							{
								break;
							}
							text += value;
						}
						if (string.IsNullOrEmpty(text))
						{
							Array.Clear(array, 0, array.Length);
							specifiedDevice.SendData(CMD_ENDW);
							specifiedDevice.ReceiveData(array);
							if (array[0] != CMD_ACK[0])
							{
								break;
							}
							flag = true;
						}
						else if (text != Settings.CUR_PWD)
						{
							Settings.CUR_PWD = "";
							if (new PasswordForm().ShowDialog() != DialogResult.OK)
							{
								OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, "", Failed: true, Closed: true));
								return;
							}
							Array.Clear(array, 0, array.Length);
							specifiedDevice.SendData(CMD_ENDW);
							specifiedDevice.ReceiveData(array);
							if (array[0] != CMD_ACK[0])
							{
								break;
							}
							flag = true;
						}
						else
						{
							Array.Clear(array, 0, array.Length);
							specifiedDevice.SendData(CMD_ENDW);
							specifiedDevice.ReceiveData(array);
							if (array[0] != CMD_ACK[0])
							{
								break;
							}
							flag = true;
						}
						continue;
					}
					byte[] array5 = new byte[16];
					Array.Copy(array2, Settings.ADDR_CHANNEL, array5, 0, array5.Length);
					BitArray bitArray = new BitArray(array5);
					list.Add(Settings.ADDR_CHANNEL);
					list2.Add(Settings.ADDR_CHANNEL + 16);
					for (num = 0; num < 128; num++)
					{
						if (bitArray[num])
						{
							num10 = Settings.ADDR_CHANNEL + 16 + num * ChannelForm.SPACE_CH;
							num11 = num10 + ChannelForm.SPACE_CH;
							list.Add(num10);
							list2.Add(num11);
						}
					}
					for (num2 = 1; num2 < 8; num2++)
					{
						num8 = Settings.ADDR_EX_CH + (num2 - 1) * ChannelForm.SPACE_CH_GROUP;
						Array.Copy(array2, num8, array5, 0, array5.Length);
						bitArray = new BitArray(array5);
						list.Add(num8);
						list2.Add(num8 + 16);
						for (num = 0; num < 128; num++)
						{
							if (bitArray[num])
							{
								num10 = num8 + 16 + num * ChannelForm.SPACE_CH;
								num11 = num10 + ChannelForm.SPACE_CH;
								list.Add(num10);
								list2.Add(num11);
							}
						}
					}
					byte[] array6 = new byte[32];
					num8 = Settings.ADDR_EX_ZONE_LIST;
					Array.Copy(array2, num8, array6, 0, array6.Length);
					bitArray = new BitArray(array6);
					list.Add(num8);
					list2.Add(num8 + 32);
					for (num = 0; num < 250; num++)
					{
						num8 = Settings.ADDR_EX_ZONE_LIST + 32;
						if (bitArray[num])
						{
							num10 = num8 + num * ZoneForm.SPACE_ZONE;
							num11 = num10 + ZoneForm.SPACE_ZONE;
							list.Add(num10);
							list2.Add(num11);
						}
					}
					for (num = 0; num < START_ADDR.Length; num++)
					{
						list.Add(START_ADDR[num]);
						list2.Add(END_ADDR[num]);
					}
					for (num = 0; num < list.Count; num++)
					{
						num10 = list[num];
						num11 = list2[num];
						for (num7 = num10; num7 < num11; num7 += num5)
						{
							num6 = num7 % 32;
							num5 = ((num7 + 32 <= num11) ? (32 - num6) : (num11 - num7));
							num4++;
						}
					}
					num8 = 0;
					num = 0;
					while (true)
					{
						if (num < list.Count)
						{
							num10 = list[num];
							num11 = list2[num];
							num7 = num10;
							while (num7 < num11)
							{
								if (!method_0())
								{
									if (num8 >> 16 != num7 >> 16)
									{
										byte[] array7 = new byte[8] { 67, 87, 66, 4, 0, 0, 0, 0 };
										num8 = num7 >> 16 << 16;
										array7[4] = (byte)(num8 >> 24);
										array7[5] = (byte)(num8 >> 16);
										array7[6] = (byte)(num8 >> 8);
										array7[7] = (byte)num8;
										Array.Clear(array, 0, array.Length);
										specifiedDevice.SendData(array7, 0, array7.Length);
										specifiedDevice.ReceiveData(array);
										if (array[0] != CMD_ACK[0])
										{
											goto IL_080b;
										}
									}
									num6 = num7 % 32;
									num5 = ((num7 + 32 <= num11) ? (32 - num6) : (num11 - num7));
									num9 = num7 - num8;
									byte[] array8 = new byte[num5 + 4];
									array8[0] = 87;
									array8[1] = (byte)(num9 >> 8);
									array8[2] = (byte)num9;
									array8[3] = (byte)num5;
									Array.Clear(array, 0, array.Length);
									Array.Copy(array2, num7, array8, 4, num5);
									specifiedDevice.SendData(array8, 0, 4 + num5);
									specifiedDevice.ReceiveData(array);
									if (array[0] == CMD_ACK[0])
									{
										if (OnFirmwareUpdateProgress != null)
										{
											OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs((float)(++num3) * 100f / (float)num4, num7.ToString(), Failed: false, Closed: false));
										}
										num7 += num5;
										continue;
									}
									goto IL_07f2;
								}
								specifiedDevice.SendData(CMD_ENDR);
								specifiedDevice.ReceiveData(array);
								return;
							}
							num++;
							continue;
						}
						Array.Clear(array, 0, array.Length);
						specifiedDevice.SendData(CMD_ENDW);
						specifiedDevice.ReceiveData(array);
						if (array[0] != CMD_ACK[0])
						{
							break;
						}
						if (OnFirmwareUpdateProgress != null)
						{
							OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100f, "", Failed: false, Closed: true));
						}
						return;
						IL_080b:
						specifiedDevice.SendData(CMD_ENDW);
						specifiedDevice.ReceiveData(array);
						break;
						IL_07f2:
						specifiedDevice.SendData(CMD_ENDW);
						specifiedDevice.ReceiveData(array);
						break;
					}
					break;
				}
				if (OnFirmwareUpdateProgress != null)
				{
					OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, Settings.SZ_MODEL_NOT_MATCH, Failed: true, Closed: true));
				}
				return;
			}
			if (OnFirmwareUpdateProgress != null)
			{
				OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, Settings.SZ_COMM_ERROR, Failed: true, Closed: true));
			}
		}
		catch (TimeoutException ex)
		{
			Console.WriteLine(ex.Message);
			if (OnFirmwareUpdateProgress != null)
			{
				OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, Settings.SZ_COMM_ERROR, Failed: false, Closed: false));
			}
		}
		finally
		{
			specifiedDevice?.Dispose();
		}
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void SetProgressCallback(Delegate1 delegate1_0)
	{
		OnFirmwareUpdateProgress = (Delegate1)Delegate.Combine(OnFirmwareUpdateProgress, delegate1_0);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void method_10(Delegate1 delegate1_0)
	{
		OnFirmwareUpdateProgress = (Delegate1)Delegate.Remove(OnFirmwareUpdateProgress, delegate1_0);
	}

	public CodeplugComms()
	{
		START_ADDR = new int[0];
		END_ADDR = new int[0];
	}
}
