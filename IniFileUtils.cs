using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

internal class IniFileUtils
{
	private static string iniPath;

	private static string keyName;

	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	private static extern long WritePrivateProfileString(string string_0, string string_1, string string_2, string string_3);

	[DllImport("kernel32.DLL ", CharSet = CharSet.Auto)]
	private static extern int GetPrivateProfileInt(string string_0, string string_1, int int_0, string string_2);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	private static extern int GetPrivateProfileString(string string_0, string string_1, string string_2, StringBuilder stringBuilder_0, int int_0, string string_3);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	public static extern int GetPrivateProfileSectionNames(IntPtr intptr_0, int int_0, string string_0);

	[DllImport("kernel32.DLL ", CharSet = CharSet.Auto)]
	private static extern int GetPrivateProfileSection(string string_0, byte[] byte_0, int int_0, string string_1);

	public static string getIniFilePath()
	{
		return iniPath;
	}

	public static void setIniFilePath(string string_0)
	{
		iniPath = string_0;
	}

	private IniFileUtils(string string_0)
	{
		setIniFilePath(string_0);
	}

	public static string getProfileStringWithDefault(string string_0, string string_1, string string_2)
	{
		if (iniPath != null)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			GetPrivateProfileString(string_0, string_1, string_2, stringBuilder, 1024, iniPath);
			return stringBuilder.ToString();
		}
		object value = Registry.GetValue(keyName, string_1, string_2);
		if (value != null)
		{
			return (string)value;
		}
		return string_2;
	}

	public static int getProfileIntWithDefault(string string_0, string string_1, int int_1)
	{
		if (iniPath != null)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			GetPrivateProfileString(string_0, string_1, int_1.ToString(), stringBuilder, 1024, iniPath);
			try
			{
				return int.Parse(stringBuilder.ToString());
			}
			catch (Exception)
			{
				return int_1;
			}
		}
		object value = Registry.GetValue(keyName, string_1, int_1);
		if (value != null)
		{
			return (int)value;
		}
		return int_1;
	}

	public static void WriteProfileString(string string_0, string string_1, string string_2)
	{
		if (iniPath != null)
		{
			WritePrivateProfileString(string_0, string_1, string_2, iniPath);
		}
		else
		{
			Registry.SetValue(keyName, string_1, string_2, RegistryValueKind.String);
		}
	}

	public static void WriteProfileInt(string string_0, string string_1, int int_1)
	{
		if (iniPath != null)
		{
			WritePrivateProfileString(string_0, string_1, int_1.ToString(), iniPath);
		}
		else
		{
			Registry.SetValue(keyName, string_1, int_1, RegistryValueKind.DWord);
		}
	}

	static IniFileUtils()
	{
		keyName = "HKEY_CURRENT_USER\\Software\\OpenGD77CPS_RUS";
		iniPath = null;
		if (File.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "Setup.ini"))
		{
			iniPath = Application.StartupPath + Path.DirectorySeparatorChar + "Setup.ini";
		}
	}
}
