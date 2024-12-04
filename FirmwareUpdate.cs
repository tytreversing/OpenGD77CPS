using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DMR;
using Microsoft.Win32.SafeHandles;
using UsbLibrary;

internal class FirmwareUpdate : Win32Usb, IFirmwareUpdate
{
	public class Class18
	{
		public enum SectorType
		{
			InternalFLASH,
			OptionBytes,
			OTP,
			DeviceFeature,
			Other
		}

		public SectorType Type;

		public string Name;

		public uint dwStartAddress;

		public uint dwSectorIndex;

		public uint dwSectorSize;

		public Class18(string string_0, SectorType sectorType_0, uint uint_0, uint uint_1, ushort ushort_0)
		{
			Name = string_0;
			Type = sectorType_0;
			dwStartAddress = uint_0;
			dwSectorSize = uint_1;
			dwSectorIndex = ushort_0;
		}
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct Struct0
	{
		public byte bLength;

		public byte bDescriptorType;

		public ushort bcdUSB;

		public byte bDeviceClass;

		public byte bDeviceSubClass;

		public byte bDeviceProtocol;

		public byte bMaxPacketSize0;

		public ushort idVendor;

		public ushort idProduct;

		public ushort bcdDevice;

		public byte iManufacturer;

		public byte iProduct;

		public byte iSerialNumber;

		public byte bNumConfigurations;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
	public struct Struct1
	{
		public byte bLength;

		public byte bDescriptorType;

		public byte bmAttributes;

		public ushort wDetachTimeOut;

		public ushort wTransfertSize;

		public ushort bcdDFUVersion;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct Struct2
	{
		public byte bStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public byte[] bwPollTimeout;

		public byte bState;

		public byte iString;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct Struct3
	{
		public byte bLength;

		public byte bDescriptorType;

		public byte bInterfaceNumber;

		public byte bAlternateSetting;

		public byte bNumEndpoints;

		public byte bInterfaceClass;

		public byte bInterfaceSubClass;

		public byte bInterfaceProtocol;

		public byte iInterface;
	}

	private const byte CST_CMD = 162;

	private const byte SUB_CMD_PRG = 80;

	private const byte SUB_CMD_READ_INFO = 81;

	private const byte SUB_CMD_WRITE = 82;

	private const byte SUB_CMD_READ = 83;

	private const byte SUB_CMD_END = 84;

	private const byte DATA_ACK = 65;

	private const byte DATA_NACK = 78;

	private const ushort HID_VID = 4660;

	private const ushort HID_PID = 22136;

	private const byte HID_DETACH_REPORT_ID = 128;

	private const byte USAGE_DETACH = 85;

	private const uint STDFU_ERROR_OFFSET = 305397760u;

	private const uint STDFU_NOERROR = 305397760u;

	private const byte STATE_IDLE = 0;

	private const byte STATE_DETACH = 1;

	private const byte STATE_DFU_IDLE = 2;

	private const byte STATE_DFU_DOWNLOAD_SYNC = 3;

	private const byte STATE_DFU_DOWNLOAD_BUS = 4;

	private const byte STATE_DFU_DOWNLOAD_IDLE = 5;

	private const byte STATE_DFU_MANIFEST_SYNC = 6;

	private const byte STATE_DFU_MANIFEST = 7;

	private const byte STATE_DFU_MANIFEST_WAIT_RESET = 8;

	private const byte STATE_DFU_UPLOAD_IDLE = 9;

	private const byte STATE_DFU_ERROR = 10;

	private const byte STATE_DFU_UPLOAD_SYNC = 145;

	private const byte STATE_DFU_UPLOAD_BUSY = 146;

	private static readonly byte[] PACK_PRG;

	private static readonly byte[] PACK_READ_INFO;

	private static readonly byte[] PACK_READ;

	private static readonly byte[] PACK_WRITE;

	private static readonly byte[] PACK_END;

	private static readonly byte[] PACK_ACK;

	private static readonly byte[] PACK_NACK;

	private static readonly byte[] PACK_LOW_VOLT;

	private IntPtr INVALID_HANDLE_VALUE;

	private Guid GUID_DFU;

	private string DFU_FilePath;

	private string DFU_DevicePath;

	private SafeFileHandle _ParentHandle;

	private ushort MaxWriteBlockSize;

	private uint[] CrcTable;

	private bool CCancelComm;

	public event FirmwareUpdateProgressEventHandler OnFirmwareUpdateProgress;

	[CompilerGenerated]
	public bool method_0()
	{
		return CCancelComm;
	}

	[CompilerGenerated]
	public void method_1(bool bool_0)
	{
		CCancelComm = bool_0;
	}

	public bool getIsThreadAlive()
	{
		return false;
	}

	public void method_5()
	{
	}

	public void UpdateFirmware()
	{
		Console.WriteLine("Here");
	}

	public void MassErase()
	{
		Console.WriteLine("MassErase");
		IntPtr zero = IntPtr.Zero;
		try
		{
			if (this.OnFirmwareUpdateProgress != null)
			{
				this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, "检测设备", Failed: false, Closed: false));
			}
			method_8();
			if (this.OnFirmwareUpdateProgress != null)
			{
				this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(10f, "正在打开设备", Failed: false, Closed: false));
			}
			zero = method_13(out MaxWriteBlockSize);
			if (this.OnFirmwareUpdateProgress != null)
			{
				this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(20f, "擦除数据", Failed: false, Closed: false));
			}
			method_14(zero);
			if (this.OnFirmwareUpdateProgress != null)
			{
				this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100f, "擦除完成", Failed: false, Closed: false));
			}
		}
		catch (Exception ex)
		{
			if (this.OnFirmwareUpdateProgress != null)
			{
				this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100f, ex.Message, Failed: true, Closed: true));
			}
		}
	}

	private void method_6()
	{
		int num = 0;
		uint eEROM_SPACE = Settings.EEROM_SPACE;
		IntPtr intptr_ = IntPtr.Zero;
		ushort ushort_ = 0;
		ushort num2 = 0;
		byte[] array = new byte[eEROM_SPACE];
		byte[] array2 = new byte[MaxWriteBlockSize];
		try
		{
			if (this.OnFirmwareUpdateProgress != null)
			{
				this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, "检测设备", Failed: false, Closed: false));
			}
			method_8();
			if (this.OnFirmwareUpdateProgress != null)
			{
				this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(3f, "正在打开设备", Failed: false, Closed: false));
			}
			intptr_ = method_13(out MaxWriteBlockSize);
			Thread.Sleep(500);
			switch (method_21(intptr_))
			{
			case 1:
				this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100f, "对讲机还未完成初始化", Failed: true, Closed: true));
				break;
			case 0:
			case 2:
				if (!method_22(intptr_))
				{
					this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(10f, "机型不匹配", Failed: true, Closed: true));
					break;
				}
				method_23(intptr_);
				method_26(intptr_, 0u);
				while (true)
				{
					method_18(intptr_, array2, ushort_);
					if (Settings.CUR_PWD == "DT8168")
					{
						break;
					}
					string text = "";
					for (num = 0; num < 8; num++)
					{
						char value = Convert.ToChar(array2[Settings.ADDR_PWD + num]);
						if ("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz\b".IndexOf(value) < 0)
						{
							break;
						}
						text += value;
					}
					if (string.IsNullOrEmpty(text) || !(text != Settings.CUR_PWD))
					{
						break;
					}
					Settings.CUR_PWD = "";
					if (new PasswordForm().ShowDialog() != DialogResult.OK)
					{
						this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(10f, "密码错误", Failed: false, Closed: true));
						return;
					}
				}
				num2 = (ushort)(eEROM_SPACE / MaxWriteBlockSize);
				for (ushort_ = 0; ushort_ < eEROM_SPACE / MaxWriteBlockSize; ushort_++)
				{
					if (method_0())
					{
						return;
					}
					method_18(intptr_, array2, ushort_);
					Array.Copy(array2, 0, array, ushort_ * MaxWriteBlockSize, MaxWriteBlockSize);
					if (this.OnFirmwareUpdateProgress != null)
					{
						this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(10 + (ushort_ + 1) * 90 / num2, ushort_.ToString(), Failed: false, Closed: false));
					}
				}
				Thread.Sleep(500);
				if (this.OnFirmwareUpdateProgress != null)
				{
					this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100f, ushort_.ToString(), Failed: false, Closed: true));
				}
				MainForm.ByteToData(array);
				break;
			}
		}
		catch (Exception ex)
		{
			if (this.OnFirmwareUpdateProgress != null)
			{
				this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100f, ex.Message, Failed: true, Closed: true));
			}
		}
		finally
		{
			method_25(intptr_);
			STDFU_Close(ref intptr_);
		}
	}

	private void method_7()
	{
		int num = 0;
		IntPtr intptr_ = IntPtr.Zero;
		ushort ushort_ = 0;
		ushort num2 = 0;
		byte[] array = new byte[MaxWriteBlockSize];
		byte[] array2 = MainForm.DataToByte();
		int num3 = array2.Length;
		try
		{
			if (this.OnFirmwareUpdateProgress != null)
			{
				this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(0f, "检测设备", Failed: false, Closed: false));
			}
			method_8();
			if (this.OnFirmwareUpdateProgress != null)
			{
				this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(10f, "正在打开设备", Failed: false, Closed: false));
			}
			intptr_ = method_13(out MaxWriteBlockSize);
			Thread.Sleep(500);
			switch (method_21(intptr_))
			{
			case 0:
			{
				if (!method_22(intptr_))
				{
					this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(10f, "机型不匹配", Failed: true, Closed: true));
					break;
				}
				method_24(intptr_);
				method_26(intptr_, 0u);
				while (true)
				{
					method_18(intptr_, array, ushort_);
					if (Settings.CUR_MODE <= 0)
					{
						int aDDR_DEVICE_INFO = Settings.ADDR_DEVICE_INFO;
						if (array[aDDR_DEVICE_INFO] == array2[aDDR_DEVICE_INFO] && array[aDDR_DEVICE_INFO + 1] == array2[aDDR_DEVICE_INFO + 1])
						{
							_ = array[aDDR_DEVICE_INFO + 2];
							_ = array2[aDDR_DEVICE_INFO + 2];
						}
						Array.Copy(array, Settings.ADDR_DEVICE_INFO + Settings.OFS_MODEL, array2, Settings.ADDR_DEVICE_INFO + Settings.OFS_MODEL, Settings.SPACE_DEVICE_INFO - Settings.OFS_MODEL);
					}
					else
					{
						Array.Copy(array, Settings.ADDR_DEVICE_INFO + Settings.OFS_CPS_SW_VER, array2, Settings.ADDR_DEVICE_INFO + Settings.OFS_CPS_SW_VER, Settings.SPACE_DEVICE_INFO - Settings.OFS_CPS_SW_VER);
					}
					if (Settings.CUR_PWD == "DT8168")
					{
						break;
					}
					string text = "";
					for (num = 0; num < 8; num++)
					{
						char value = Convert.ToChar(array[Settings.ADDR_PWD + num]);
						if ("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz\b".IndexOf(value) < 0)
						{
							break;
						}
						text += value;
					}
					if (string.IsNullOrEmpty(text) || !(text != Settings.CUR_PWD))
					{
						break;
					}
					Settings.CUR_PWD = "";
					if (new PasswordForm().ShowDialog() != DialogResult.OK)
					{
						return;
					}
				}
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
				num2 = (ushort)(num3 / MaxWriteBlockSize);
				for (ushort_ = 0; ushort_ < num3 / MaxWriteBlockSize; ushort_++)
				{
					if (method_0())
					{
						return;
					}
					if (this.OnFirmwareUpdateProgress != null)
					{
						this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(10 + (ushort_ + 1) * 90 / num2, ushort_.ToString(), Failed: false, Closed: false));
					}
					try
					{
						Array.Copy(array2, ushort_ * MaxWriteBlockSize, array, 0, MaxWriteBlockSize);
						method_19(intptr_, array, ushort_);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				if (this.OnFirmwareUpdateProgress != null)
				{
					this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100f, "完成", Failed: false, Closed: true));
				}
				break;
			}
			case 2:
				if (this.OnFirmwareUpdateProgress != null)
				{
					this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100f, "设备电池电平太低，不能执行操作，请给设备充电，然后重试。", Failed: true, Closed: true));
				}
				break;
			case 1:
				this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100f, "对讲机还未完成初始化", Failed: true, Closed: true));
				break;
			}
		}
		catch (Exception ex2)
		{
			STDFU_Close(ref intptr_);
			if (this.OnFirmwareUpdateProgress != null)
			{
				this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(100f, ex2.Message, Failed: true, Closed: true));
			}
		}
		finally
		{
			method_25(intptr_);
			STDFU_Close(ref intptr_);
		}
	}

	private void method_8()
	{
		if (!method_9(4660, 22136))
		{
			return;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(65);
		Marshal.WriteByte(intPtr, 0, 128);
		Marshal.WriteByte(intPtr, 1, 85);
		for (int i = 2; i < 65; i++)
		{
			Marshal.WriteByte(intPtr, i, 0);
		}
		if (Win32Usb.HidD_SetFeature(_ParentHandle, intPtr, 65u))
		{
			if (this.OnFirmwareUpdateProgress != null)
			{
				this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(1f, "HID 分离成功", Failed: false, Closed: false));
			}
		}
		else if (this.OnFirmwareUpdateProgress != null)
		{
			this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(1f, "HID 分离错误 = " + Marshal.GetLastWin32Error(), Failed: false, Closed: false));
		}
		Marshal.FreeHGlobal(intPtr);
		Thread.Sleep(5000);
	}

	public bool method_9(ushort ushort_0, ushort ushort_1)
	{
		string value = $"vid_{ushort_0:x4}&pid_{ushort_1:x4}";
		Guid gHid = default(Guid);
		Win32Usb.HidD_GetHidGuid(out gHid);
		IntPtr intPtr = Win32Usb.SetupDiGetClassDevs(ref gHid, null, IntPtr.Zero, 18u);
		try
		{
			DeviceInterfaceData oInterfaceData = default(DeviceInterfaceData);
			oInterfaceData.Size = Marshal.SizeOf(oInterfaceData);
			for (int i = 0; Win32Usb.SetupDiEnumDeviceInterfaces(intPtr, 0u, ref gHid, (uint)i, ref oInterfaceData); i++)
			{
				if (method_10(intPtr, ref oInterfaceData).IndexOf(value) >= 0)
				{
					return true;
				}
			}
			if (Marshal.GetLastWin32Error() != 0 && 259L != Marshal.GetLastWin32Error())
			{
				if (1784L == Marshal.GetLastWin32Error())
				{
					throw new Exception("Size member of hInfoSet is not set correctly (5 for 32bit or 8 for 64bit)");
				}
				throw new Exception("SetupDiEnumDeviceInterfaces returned error " + Marshal.GetLastWin32Error());
			}
		}
		finally
		{
			Win32Usb.SetupDiDestroyDeviceInfoList(intPtr);
		}
		return false;
	}

	private string method_10(IntPtr intptr_0, ref DeviceInterfaceData deviceInterfaceData_0)
	{
		uint nRequiredSize = 0u;
		if (!Win32Usb.SetupDiGetDeviceInterfaceDetail(intptr_0, ref deviceInterfaceData_0, IntPtr.Zero, 0u, ref nRequiredSize, IntPtr.Zero))
		{
			DeviceInterfaceDetailData oDetailData = default(DeviceInterfaceDetailData);
			if (IntPtr.Size == 8)
			{
				oDetailData.Size = 8;
			}
			else
			{
				if (IntPtr.Size != 4)
				{
					throw new Exception("Operating system is neither 32 nor 64 bits!");
				}
				oDetailData.Size = 5;
			}
			if (Win32Usb.SetupDiGetDeviceInterfaceDetail(intptr_0, ref deviceInterfaceData_0, ref oDetailData, nRequiredSize, ref nRequiredSize, IntPtr.Zero))
			{
				return oDetailData.DevicePath;
			}
		}
		return null;
	}

	public bool ParseDFU_File(string Filepath, out ushort VID, out ushort PID, out ushort Version)
	{
		bool result = true;
		try
		{
			byte[] array = File.ReadAllBytes(Filepath);
			if (Encoding.UTF8.GetString(array, 0, 5) != "DfuSe")
			{
				throw new Exception("File signature error");
			}
			if (array[5] != 1)
			{
				throw new Exception("DFU file version must be 1");
			}
			if (!(Encoding.UTF8.GetString(array, array.Length - 8, 3) != "UFD") && array[array.Length - 5] == 16 && array[array.Length - 10] == 26 && array[array.Length - 9] == 1)
			{
				if (BitConverter.ToUInt32(array, array.Length - 4) != calculateCRC(array))
				{
					throw new Exception("File CRC error");
				}
				VID = BitConverter.ToUInt16(array, array.Length - 12);
				PID = BitConverter.ToUInt16(array, array.Length - 14);
				Version = BitConverter.ToUInt16(array, array.Length - 16);
				return result;
			}
			throw new Exception("File suffix error");
		}
		catch
		{
			VID = 0;
			PID = 0;
			Version = 0;
			return false;
		}
	}

	private void method_11(byte[] byte_0, out byte[] byte_1, out uint uint_0, out uint uint_1)
	{
		byte_1 = Array.Empty<byte>();
		uint_0 = 0u;
		uint_1 = 0u;
		try
		{
			byte_0 = File.ReadAllBytes(DFU_FilePath);
			if (Encoding.UTF8.GetString(byte_0, 0, 5) != "DfuSe")
			{
				throw new Exception("File signature error");
			}
			if (byte_0[5] != 1)
			{
				throw new Exception("DFU file version must be 1");
			}
			if (byte_0[10] != 1)
			{
				throw new Exception("There should be exactly one target in the DFU file");
			}
			if (!(Encoding.UTF8.GetString(byte_0, byte_0.Length - 8, 3) != "UFD") && byte_0[byte_0.Length - 5] == 16 && byte_0[byte_0.Length - 10] == 26 && byte_0[byte_0.Length - 9] == 1)
			{
				if (BitConverter.ToUInt32(byte_0, byte_0.Length - 4) != calculateCRC(byte_0))
				{
					throw new Exception("File CRC error");
				}
				if (Encoding.UTF8.GetString(byte_0, 11, 6) != "Target")
				{
					throw new Exception("Target signature error");
				}
				BitConverter.ToUInt32(byte_0, 277);
				_ = byte_0[18];
				if (BitConverter.ToUInt32(byte_0, 281) != 1)
				{
					throw new Exception("We only expect one element in the target");
				}
				uint_0 = BitConverter.ToUInt32(byte_0, 285);
				uint_1 = BitConverter.ToUInt32(byte_0, 289);
				byte_1 = byte_0.Skip(293).Take((int)uint_1).ToArray();
				if (this.OnFirmwareUpdateProgress != null)
				{
					this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(5f, "DFU 文件解析" + uint_1 + uint_0, Failed: false, Closed: false));
				}
				return;
			}
			throw new Exception("File suffix error");
		}
		catch (Exception ex)
		{
			throw new Exception("DFU file read failed. " + ex.Message);
		}
	}

	private uint calculateCRC(byte[] byte_0)
	{
		uint num = uint.MaxValue;
		for (int i = 0; i < byte_0.Length - 4; i++)
		{
			num = CrcTable[(num ^ byte_0[i]) & 0xFF] ^ (num >> 8);
		}
		return num;
	}

	private IntPtr method_13(out ushort ushort_0)
	{
		int num = 10;
		uint num2 = 0u;
		Guid gClass = GUID_DFU;
		DeviceInterfaceData oInterfaceData = default(DeviceInterfaceData);
		oInterfaceData.Size = Marshal.SizeOf(oInterfaceData);
		DeviceInterfaceDetailData oDetailData = default(DeviceInterfaceDetailData);
		uint nRequiredSize = 0u;
		IntPtr intPtr = Win32Usb.SetupDiGetClassDevs(ref gClass, null, IntPtr.Zero, 18u);
		IntPtr intptr_ = IntPtr.Zero;
		ushort_0 = 1024;
		try
		{
			if (intPtr == INVALID_HANDLE_VALUE)
			{
				throw new Exception("SetupDiGetClassDevs error code = " + Marshal.GetLastWin32Error());
			}
			while (num-- > 0)
			{
				for (num2 = 0u; Win32Usb.SetupDiEnumDeviceInterfaces(intPtr, 0u, ref gClass, num2, ref oInterfaceData); num2++)
				{
				}
				if (num2 != 0)
				{
					break;
				}
				Thread.Sleep(500);
			}
			if (1 == num2)
			{
				Win32Usb.SetupDiEnumDeviceInterfaces(intPtr, 0u, ref gClass, 0u, ref oInterfaceData);
				Win32Usb.SetupDiGetDeviceInterfaceDetail(intPtr, ref oInterfaceData, IntPtr.Zero, 0u, ref nRequiredSize, IntPtr.Zero);
				if (IntPtr.Size == 8)
				{
					oDetailData.Size = 8;
				}
				else
				{
					if (IntPtr.Size != 4)
					{
						throw new Exception("Operating system version");
					}
					oDetailData.Size = 5;
				}
				if (Marshal.SizeOf(oDetailData) < nRequiredSize)
				{
					throw new Exception("DeviceInterfaceDetailData Too small ");
				}
				if (Win32Usb.SetupDiGetDeviceInterfaceDetail(intPtr, ref oInterfaceData, ref oDetailData, nRequiredSize, ref nRequiredSize, IntPtr.Zero))
				{
					DFU_DevicePath = oDetailData.DevicePath.ToUpper();
					if (305397760 == STDFU_Open(DFU_DevicePath, out intptr_))
					{
						if (this.OnFirmwareUpdateProgress != null)
						{
							this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(10f, "DFU The device opens successfully", Failed: false, Closed: false));
						}
						Struct0 struct0_ = default(Struct0);
						if (305397760 == STDFU_GetDeviceDescriptor(ref intptr_, ref struct0_))
						{
							switch (struct0_.bcdDevice)
							{
							default:
								throw new Exception("未支持的驱动版本  Unsupported driver version = " + struct0_.bcdDevice.ToString("X4"));
							case 528:
								ushort_0 = 2048;
								break;
							case 282:
							case 512:
								ushort_0 = 1024;
								break;
							}
							uint uint_ = 0u;
							uint uint_2 = 0u;
							Struct1 struct1_ = default(Struct1);
							if (305397760 == STDFU_GetDFUDescriptor(ref intptr_, ref uint_, ref uint_2, ref struct1_))
							{
								if (this.OnFirmwareUpdateProgress != null)
								{
									this.OnFirmwareUpdateProgress(this, new FirmwareUpdateProgressEventArgs(14f, "获取DFU版本 " + struct1_.bcdDFUVersion.ToString("X"), Failed: false, Closed: false));
								}
								return intptr_;
							}
							throw new Exception("STDFU_GetDFUDescriptor 失败, 错误码 = " + Marshal.GetLastWin32Error());
						}
						throw new Exception("STDFU_GetDeviceDescriptor 失败, 错误码 = " + Marshal.GetLastWin32Error());
					}
					throw new Exception("STDFU_Open 失败, 错误码 = " + Marshal.GetLastWin32Error());
				}
				return intptr_;
			}
			throw new Exception("DFU device not found");
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
		finally
		{
			Win32Usb.SetupDiDestroyDeviceInfoList(intPtr);
		}
	}

	private bool method_14(IntPtr intptr_0)
	{
		uint num = 0u;
		byte[] byte_ = new byte[16]
		{
			65, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255
		};
		if (305397760 == (num = STDFU_SelectCurrentConfiguration(ref intptr_0, 0u, 0u, 1u)))
		{
			if (!method_20(ref intptr_0))
			{
				return false;
			}
			if (305397760 == (num = STDFU_Dnload(ref intptr_0, byte_, 1u, 0)))
			{
				if (!method_20(ref intptr_0))
				{
					return false;
				}
				return true;
			}
			throw new Exception("STDFU_Dnload returned (返回) " + num.ToString("X8"));
		}
		throw new Exception("STDFU_SelectCurrentConfiguration returned (返回) " + num.ToString("X8"));
	}

	private void method_15(IntPtr intptr_0, uint uint_0, uint uint_1, List<Class18> cLJ3sEY9awr3Obv1TP)
	{
		foreach (Class18 item in cLJ3sEY9awr3Obv1TP)
		{
			if (uint_0 < item.dwStartAddress + item.dwSectorSize && uint_0 + uint_1 > item.dwStartAddress)
			{
				method_16(intptr_0, item.dwStartAddress);
			}
		}
	}

	private bool method_16(IntPtr intptr_0, uint uint_0)
	{
		uint num = 0u;
		byte[] array = new byte[16]
		{
			65, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255
		};
		array[1] = (byte)(uint_0 & 0xFF);
		array[2] = (byte)((uint_0 >> 8) & 0xFF);
		array[3] = (byte)((uint_0 >> 16) & 0xFF);
		array[4] = (byte)((uint_0 >> 24) & 0xFF);
		if (305397760 == (num = STDFU_SelectCurrentConfiguration(ref intptr_0, 0u, 0u, 0u)))
		{
			if (!method_20(ref intptr_0))
			{
				return false;
			}
			if (305397760 == (num = STDFU_Dnload(ref intptr_0, array, 5u, 0)))
			{
				if (!method_20(ref intptr_0))
				{
					return false;
				}
				return true;
			}
			throw new Exception("STDFU_Dnload returned " + num.ToString("X8"));
		}
		throw new Exception("STDFU_SelectCurrentConfiguration returned " + num.ToString("X8"));
	}

	private bool method_17(IntPtr intptr_0, uint uint_0, byte[] byte_0, uint uint_1)
	{
		if (byte_0.Length > MaxWriteBlockSize)
		{
			throw new Exception("Block size too big (" + byte_0.Length + ")");
		}
		if (uint_1 == 0)
		{
			method_26(intptr_0, uint_0);
		}
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		STDFU_Dnload(ref intptr_0, byte_0, (uint)byte_0.Length, (ushort)(uint_1 + 2));
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		return true;
	}

	private bool method_18(IntPtr intptr_0, byte[] byte_0, ushort ushort_0)
	{
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		STDFU_Upload(ref intptr_0, byte_0, MaxWriteBlockSize, (ushort)(ushort_0 + 2));
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		return true;
	}

	private bool method_19(IntPtr intptr_0, byte[] byte_0, ushort ushort_0)
	{
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		STDFU_Dnload(ref intptr_0, byte_0, MaxWriteBlockSize, (ushort)(ushort_0 + 2));
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		return true;
	}

	private bool method_20(ref IntPtr intptr_0)
	{
		long num = 50000000L;
		long ticks = DateTime.Now.Ticks;
		Struct2 struct2_ = default(Struct2);
		STDFU_Getstatus(ref intptr_0, ref struct2_);
		while (true)
		{
			if (struct2_.bState != 2)
			{
				if (ticks + num <= DateTime.Now.Ticks)
				{
					break;
				}
				Thread.Sleep(100);
				Application.DoEvents();
				STDFU_Clrstatus(ref intptr_0);
				STDFU_Getstatus(ref intptr_0, ref struct2_);
				continue;
			}
			return true;
		}
		STDFU_Close(ref intptr_0);
		return false;
	}

	private int method_21(IntPtr intptr_0)
	{
		int result = -1;
		byte[] byte_ = new byte[MaxWriteBlockSize];
		if (!method_20(ref intptr_0))
		{
			return result;
		}
		STDFU_Dnload(ref intptr_0, PACK_PRG, (uint)PACK_PRG.Length, 0);
		if (!method_20(ref intptr_0))
		{
			return result;
		}
		STDFU_Upload(ref intptr_0, byte_, MaxWriteBlockSize, 0);
		if (!method_20(ref intptr_0))
		{
			return result;
		}
		if (byte_.smethod_4(PACK_ACK))
		{
			return 0;
		}
		if (byte_.smethod_4(PACK_NACK))
		{
			return 1;
		}
		if (byte_.smethod_4(PACK_LOW_VOLT))
		{
			return 2;
		}
		return -1;
	}

	private bool method_22(IntPtr intptr_0)
	{
		byte[] array = new byte[MaxWriteBlockSize];
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		STDFU_Dnload(ref intptr_0, PACK_READ_INFO, (uint)PACK_READ_INFO.Length, 0);
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		STDFU_Upload(ref intptr_0, array, MaxWriteBlockSize, 0);
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		byte[] array2 = new byte[8];
		Buffer.BlockCopy(array, 1, array2, 0, 8);
		if (!array2.smethod_4(Settings.CUR_MODEL))
		{
			return false;
		}
		return true;
	}

	private bool method_23(IntPtr intptr_0)
	{
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		STDFU_Dnload(ref intptr_0, PACK_READ, (uint)PACK_READ.Length, 0);
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		return true;
	}

	private bool method_24(IntPtr intptr_0)
	{
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		STDFU_Dnload(ref intptr_0, PACK_WRITE, (uint)PACK_WRITE.Length, 0);
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		return true;
	}

	private bool method_25(IntPtr intptr_0)
	{
		byte[] byte_ = new byte[MaxWriteBlockSize];
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		STDFU_Dnload(ref intptr_0, PACK_END, (uint)PACK_END.Length, 0);
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		STDFU_Upload(ref intptr_0, byte_, 2u, 0);
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		return true;
	}

	private bool method_26(IntPtr intptr_0, uint uint_0)
	{
		byte[] byte_ = new byte[5]
		{
			33,
			(byte)(uint_0 & 0xFF),
			(byte)((uint_0 >> 8) & 0xFF),
			(byte)((uint_0 >> 16) & 0xFF),
			(byte)((uint_0 >> 24) & 0xFF)
		};
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		STDFU_Dnload(ref intptr_0, byte_, 5u, 0);
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		return true;
	}

	private bool method_27(IntPtr intptr_0, uint uint_0)
	{
		byte[] byte_ = new byte[5]
		{
			33,
			(byte)(uint_0 & 0xFF),
			(byte)((uint_0 >> 8) & 0xFF),
			(byte)((uint_0 >> 16) & 0xFF),
			(byte)((uint_0 >> 24) & 0xFF)
		};
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		STDFU_Dnload(ref intptr_0, byte_, 5u, 0);
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		STDFU_Dnload(ref intptr_0, byte_, 0u, 0);
		if (!method_20(ref intptr_0))
		{
			return false;
		}
		return true;
	}

	private List<Class18> method_28(IntPtr intptr_0)
	{
		List<Class18> list = new List<Class18>();
		uint num = 0u;
		uint uint_ = 0u;
		uint uint_2 = 0u;
		Struct1 struct1_ = default(Struct1);
		Struct3 struct3_ = default(Struct3);
		uint num2 = 0u;
		IntPtr intPtr = Marshal.AllocHGlobal(512);
		if (305397760 == (num = STDFU_GetDFUDescriptor(ref intptr_0, ref uint_, ref uint_2, ref struct1_)))
		{
			for (num2 = 0u; num2 < uint_2; num2++)
			{
				if (305397760 == (num = STDFU_GetInterfaceDescriptor(ref intptr_0, 0u, uint_, num2, ref struct3_)))
				{
					if (struct3_.iInterface != 0)
					{
						if (305397760 != (num = STDFU_GetStringDescriptor(ref intptr_0, struct3_.iInterface, intPtr, 512u)))
						{
							break;
						}
						ushort num3 = 0;
						uint num4 = 0u;
						ushort num5 = 0;
						Class18.SectorType sectorType = Class18.SectorType.Other;
						string text = Marshal.PtrToStringAnsi(intPtr);
						if ('@' == text[0])
						{
							string text2 = text.Substring(1, text.IndexOf('/') - 1);
							text2 = text2.TrimEnd(' ');
							sectorType = ((!text2.Equals("Internal Flash")) ? (text2.Equals("Option Bytes") ? Class18.SectorType.OptionBytes : (text2.Equals("OTP Memory") ? Class18.SectorType.OTP : ((!text2.Equals("Device Feature")) ? Class18.SectorType.Other : Class18.SectorType.DeviceFeature))) : Class18.SectorType.InternalFLASH);
							uint num6 = uint.Parse(text.Substring(text.IndexOf('/') + 3, 8), NumberStyles.HexNumber);
							string text3 = text;
							while (text3.IndexOf('*') >= 0)
							{
								string text4 = text3.Substring(text3.IndexOf('*') - 3, 3);
								num3 = ((!char.IsDigit(text4[0])) ? ushort.Parse(text4.Substring(1)) : ushort.Parse(text4));
								num4 = ushort.Parse(text3.Substring(text3.IndexOf('*') + 1, 3));
								if ('k' == char.ToLower(text3[text3.IndexOf('*') + 4]))
								{
									num4 *= 1024;
								}
								for (num5 = 0; num5 < num3; num5++)
								{
									list.Add(new Class18(text2, sectorType, num6, num4, num5));
									num6 += num4;
								}
								text3 = text3.Substring(text3.IndexOf('*') + 1);
							}
							continue;
						}
						throw new Exception("STDFU_GetStringDescriptor bad value in MapDesc, i=" + num2);
					}
					throw new Exception("STDFU_GetInterfaceDescriptor bad value in iInterface");
				}
				throw new Exception("STDFU_GetInterfaceDescriptor returned " + num);
			}
			return list;
		}
		throw new Exception("STDFU_GetDFUDescriptor returned " + num);
	}

	[DllImport("STDFU.dll", CharSet = CharSet.Ansi)]
	public static extern uint STDFU_Open([MarshalAs(UnmanagedType.LPStr)] string string_0, out IntPtr intptr_0);

	[DllImport("STDFU.dll", CharSet = CharSet.Ansi)]
	public static extern uint STDFU_SelectCurrentConfiguration(ref IntPtr intptr_0, uint uint_0, uint uint_1, uint uint_2);

	[DllImport("STDFU.dll", CharSet = CharSet.Auto)]
	public static extern uint STDFU_GetDeviceDescriptor(ref IntPtr intptr_0, ref Struct0 struct0_0);

	[DllImport("STDFU.dll", CharSet = CharSet.Auto)]
	public static extern uint STDFU_GetDFUDescriptor(ref IntPtr intptr_0, ref uint uint_0, ref uint uint_1, ref Struct1 struct1_0);

	[DllImport("STDFU.dll", CharSet = CharSet.Auto)]
	public static extern uint STDFU_GetInterfaceDescriptor(ref IntPtr intptr_0, uint uint_0, uint uint_1, uint uint_2, ref Struct3 struct3_0);

	[DllImport("STDFU.dll", CharSet = CharSet.Auto)]
	public static extern uint STDFU_GetStringDescriptor(ref IntPtr intptr_0, uint uint_0, IntPtr intptr_1, uint uint_1);

	[DllImport("STDFU.dll", CharSet = CharSet.Ansi)]
	public static extern uint STDFU_Dnload(ref IntPtr intptr_0, [MarshalAs(UnmanagedType.LPArray)] byte[] byte_0, uint uint_0, ushort ushort_0);

	[DllImport("STDFU.dll", CharSet = CharSet.Ansi)]
	public static extern uint STDFU_Upload(ref IntPtr intptr_0, [MarshalAs(UnmanagedType.LPArray)] byte[] byte_0, uint uint_0, ushort ushort_0);

	[DllImport("STDFU.dll", CharSet = CharSet.Ansi)]
	public static extern uint STDFU_Getstatus(ref IntPtr intptr_0, ref Struct2 struct2_0);

	[DllImport("STDFU.dll", CharSet = CharSet.Ansi)]
	public static extern uint STDFU_Clrstatus(ref IntPtr intptr_0);

	[DllImport("STDFU.dll", CharSet = CharSet.Ansi)]
	public static extern uint STDFU_Close(ref IntPtr intptr_0);

	[DllImport("STDFU.dll", CharSet = CharSet.Ansi)]
	public static extern uint STDFU_Abort(ref IntPtr intptr_0);

	public FirmwareUpdate()
	{
		INVALID_HANDLE_VALUE = (IntPtr)(-1);
		GUID_DFU = new Guid(1072171435u, 64401, 19637, 166, 67, 105, 103, 13, 82, 54, 110);
		DFU_FilePath = "";
		DFU_DevicePath = "";
		MaxWriteBlockSize = 1024;
		CrcTable = new uint[256]
		{
			0u, 1996959894u, 3993919788u, 2567524794u, 124634137u, 1886057615u, 3915621685u, 2657392035u, 249268274u, 2044508324u,
			3772115230u, 2547177864u, 162941995u, 2125561021u, 3887607047u, 2428444049u, 498536548u, 1789927666u, 4089016648u, 2227061214u,
			450548861u, 1843258603u, 4107580753u, 2211677639u, 325883990u, 1684777152u, 4251122042u, 2321926636u, 335633487u, 1661365465u,
			4195302755u, 2366115317u, 997073096u, 1281953886u, 3579855332u, 2724688242u, 1006888145u, 1258607687u, 3524101629u, 2768942443u,
			901097722u, 1119000684u, 3686517206u, 2898065728u, 853044451u, 1172266101u, 3705015759u, 2882616665u, 651767980u, 1373503546u,
			3369554304u, 3218104598u, 565507253u, 1454621731u, 3485111705u, 3099436303u, 671266974u, 1594198024u, 3322730930u, 2970347812u,
			795835527u, 1483230225u, 3244367275u, 3060149565u, 1994146192u, 31158534u, 2563907772u, 4023717930u, 1907459465u, 112637215u,
			2680153253u, 3904427059u, 2013776290u, 251722036u, 2517215374u, 3775830040u, 2137656763u, 141376813u, 2439277719u, 3865271297u,
			1802195444u, 476864866u, 2238001368u, 4066508878u, 1812370925u, 453092731u, 2181625025u, 4111451223u, 1706088902u, 314042704u,
			2344532202u, 4240017532u, 1658658271u, 366619977u, 2362670323u, 4224994405u, 1303535960u, 984961486u, 2747007092u, 3569037538u,
			1256170817u, 1037604311u, 2765210733u, 3554079995u, 1131014506u, 879679996u, 2909243462u, 3663771856u, 1141124467u, 855842277u,
			2852801631u, 3708648649u, 1342533948u, 654459306u, 3188396048u, 3373015174u, 1466479909u, 544179635u, 3110523913u, 3462522015u,
			1591671054u, 702138776u, 2966460450u, 3352799412u, 1504918807u, 783551873u, 3082640443u, 3233442989u, 3988292384u, 2596254646u,
			62317068u, 1957810842u, 3939845945u, 2647816111u, 81470997u, 1943803523u, 3814918930u, 2489596804u, 225274430u, 2053790376u,
			3826175755u, 2466906013u, 167816743u, 2097651377u, 4027552580u, 2265490386u, 503444072u, 1762050814u, 4150417245u, 2154129355u,
			426522225u, 1852507879u, 4275313526u, 2312317920u, 282753626u, 1742555852u, 4189708143u, 2394877945u, 397917763u, 1622183637u,
			3604390888u, 2714866558u, 953729732u, 1340076626u, 3518719985u, 2797360999u, 1068828381u, 1219638859u, 3624741850u, 2936675148u,
			906185462u, 1090812512u, 3747672003u, 2825379669u, 829329135u, 1181335161u, 3412177804u, 3160834842u, 628085408u, 1382605366u,
			3423369109u, 3138078467u, 570562233u, 1426400815u, 3317316542u, 2998733608u, 733239954u, 1555261956u, 3268935591u, 3050360625u,
			752459403u, 1541320221u, 2607071920u, 3965973030u, 1969922972u, 40735498u, 2617837225u, 3943577151u, 1913087877u, 83908371u,
			2512341634u, 3803740692u, 2075208622u, 213261112u, 2463272603u, 3855990285u, 2094854071u, 198958881u, 2262029012u, 4057260610u,
			1759359992u, 534414190u, 2176718541u, 4139329115u, 1873836001u, 414664567u, 2282248934u, 4279200368u, 1711684554u, 285281116u,
			2405801727u, 4167216745u, 1634467795u, 376229701u, 2685067896u, 3608007406u, 1308918612u, 956543938u, 2808555105u, 3495958263u,
			1231636301u, 1047427035u, 2932959818u, 3654703836u, 1088359270u, 936918000u, 2847714899u, 3736837829u, 1202900863u, 817233897u,
			3183342108u, 3401237130u, 1404277552u, 615818150u, 3134207493u, 3453421203u, 1423857449u, 601450431u, 3009837614u, 3294710456u,
			1567103746u, 711928724u, 3020668471u, 3272380065u, 1510334235u, 755167117u
		};
	}

	[CompilerGenerated]
	private static bool smethod_0(byte byte_0)
	{
		return byte_0 == 0;
	}

	static FirmwareUpdate()
	{
		PACK_PRG = new byte[9] { 162, 80, 80, 82, 79, 71, 82, 65, 77 };
		PACK_READ_INFO = new byte[2] { 162, 81 };
		PACK_READ = new byte[2] { 162, 83 };
		PACK_WRITE = new byte[2] { 162, 82 };
		PACK_END = new byte[5] { 162, 84, 69, 78, 68 };
		PACK_ACK = new byte[2] { 162, 65 };
		PACK_NACK = new byte[2] { 162, 78 };
		PACK_LOW_VOLT = new byte[2] { 162, 79 };
	}
}
