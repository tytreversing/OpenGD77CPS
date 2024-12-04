using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace DMR;

public class SetupDiWrap
{
	private struct SP_DEVINFO_DATA
	{
		public uint cbSize;

		public Guid ClassGuid;

		public uint DevInst;

		public uint Reserved;
	}

	private struct SP_DEVICE_INTERFACE_DATA
	{
		public int cbSize;

		public Guid interfaceClassGuid;

		public int flags;

		private UIntPtr reserved;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	private struct SP_DEVICE_INTERFACE_DETAIL_DATA
	{
		public int cbSize;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string DevicePath;
	}

	private enum SPDRP
	{
		DEVICEDESC,
		HARDWAREID,
		COMPATIBLEIDS,
		NTDEVICEPATHS,
		SERVICE,
		CONFIGURATION,
		CONFIGURATIONVECTOR,
		CLASS,
		CLASSGUID,
		DRIVER,
		CONFIGFLAGS,
		MFG,
		FRIENDLYNAME,
		LOCATION_INFORMATION,
		PHYSICAL_DEVICE_OBJECT_NAME,
		CAPABILITIES,
		UI_NUMBER,
		UPPERFILTERS,
		LOWERFILTERS,
		MAXIMUM_PROPERTY
	}

	private const int BUFFER_SIZE = 1024;

	private const int DIGCF_DEFAULT = 1;

	private const int DIGCF_PRESENT = 2;

	private const int DIGCF_ALLCLASSES = 4;

	private const int DIGCF_PROFILE = 8;

	private const int DIGCF_DEVICEINTERFACE = 16;

	private const int INVALID_HANDLE_VALUE = -1;

	public static string ComPortNameFromFriendlyNamePrefix(string friendlyNamePrefix)
	{
		Guid[] classGUIDs = GetClassGUIDs("Ports");
		Regex regex = new Regex(".? \\((COM\\d+)\\)\\s?$");
		Guid[] array = classGUIDs;
		for (int i = 0; i < array.Length; i++)
		{
			Guid ClassGuid = array[i];
			IntPtr deviceInfoSet = SetupDiGetClassDevs(ref ClassGuid, IntPtr.Zero, IntPtr.Zero, 10);
			if (deviceInfoSet.ToInt32() == -1)
			{
				continue;
			}
			int num = 0;
			while (true)
			{
				SP_DEVINFO_DATA DeviceInterfaceData = default(SP_DEVINFO_DATA);
				DeviceInterfaceData.cbSize = (uint)Marshal.SizeOf(DeviceInterfaceData);
				if (SetupDiEnumDeviceInfo(deviceInfoSet, num++, ref DeviceInterfaceData) == 0)
				{
					break;
				}
				byte[] array2 = new byte[1024];
				if (SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref DeviceInterfaceData, 12u, out var _, array2, 1024u, out var RequiredSize))
				{
					string @string = Encoding.Unicode.GetString(array2, 0, (int)(RequiredSize - 2));
					if (@string.StartsWith(friendlyNamePrefix) && regex.IsMatch(@string))
					{
						return regex.Match(@string).Groups[1].Value;
					}
				}
			}
			SetupDiDestroyDeviceInfoList(deviceInfoSet);
		}
		return null;
	}

	public static List<string> GetComPortNames()
	{
		Guid[] classGUIDs = GetClassGUIDs("Ports");
		List<string> list = new List<string>();
		Guid[] array = classGUIDs;
		for (int i = 0; i < array.Length; i++)
		{
			Guid ClassGuid = array[i];
			IntPtr deviceInfoSet = SetupDiGetClassDevs(ref ClassGuid, IntPtr.Zero, IntPtr.Zero, 10);
			if (deviceInfoSet.ToInt32() == -1)
			{
				continue;
			}
			int num = 0;
			while (true)
			{
				SP_DEVINFO_DATA DeviceInterfaceData = default(SP_DEVINFO_DATA);
				DeviceInterfaceData.cbSize = (uint)Marshal.SizeOf(DeviceInterfaceData);
				if (SetupDiEnumDeviceInfo(deviceInfoSet, num++, ref DeviceInterfaceData) == 0)
				{
					break;
				}
				byte[] array2 = new byte[1024];
				if (SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref DeviceInterfaceData, 12u, out var _, array2, 1024u, out var RequiredSize))
				{
					string @string = Encoding.Unicode.GetString(array2, 0, (int)(RequiredSize - 2));
					list.Add(@string);
				}
			}
			SetupDiDestroyDeviceInfoList(deviceInfoSet);
		}
		return list;
	}

	[DllImport("setupapi.dll", SetLastError = true)]
	private static extern bool SetupDiClassGuidsFromName(string ClassName, ref Guid ClassGuidArray1stItem, uint ClassGuidArraySize, out uint RequiredSize);

	[DllImport("setupapi.dll")]
	internal static extern IntPtr SetupDiGetClassDevsEx(IntPtr ClassGuid, [MarshalAs(UnmanagedType.LPStr)] string enumerator, IntPtr hwndParent, int Flags, IntPtr DeviceInfoSet, [MarshalAs(UnmanagedType.LPStr)] string MachineName, IntPtr Reserved);

	[DllImport("setupapi.dll")]
	internal static extern int SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern bool SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr optionalCrap, ref Guid interfaceClassGuid, uint memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

	[DllImport("setupapi.dll")]
	private static extern int SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, int MemberIndex, ref SP_DEVINFO_DATA DeviceInterfaceData);

	[DllImport("setupapi.dll")]
	private static extern int SetupDiClassNameFromGuid(ref Guid ClassGuid, StringBuilder className, int ClassNameSize, ref int RequiredSize);

	[DllImport("setupapi.dll")]
	private static extern int SetupDiGetClassDescription(ref Guid ClassGuid, StringBuilder classDescription, int ClassDescriptionSize, ref int RequiredSize);

	[DllImport("setupapi.dll")]
	private static extern int SetupDiGetDeviceInstanceId(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, StringBuilder DeviceInstanceId, int DeviceInstanceIdSize, ref int RequiredSize);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, int Flags);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize, out uint requiredSize, ref SP_DEVINFO_DATA deviceInfoData);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern bool SetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, uint Property, out uint PropertyRegDataType, byte[] PropertyBuffer, uint PropertyBufferSize, out uint RequiredSize);

	private static Guid[] GetClassGUIDs(string className)
	{
		uint RequiredSize = 0u;
		Guid[] array = new Guid[1];
		if (SetupDiClassGuidsFromName(className, ref array[0], 1u, out RequiredSize))
		{
			if (1 < RequiredSize)
			{
				array = new Guid[RequiredSize];
				SetupDiClassGuidsFromName(className, ref array[0], RequiredSize, out RequiredSize);
			}
			return array;
		}
		throw new Win32Exception();
	}
}
