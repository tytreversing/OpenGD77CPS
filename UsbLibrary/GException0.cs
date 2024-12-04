using System;
using System.Runtime.InteropServices;

namespace UsbLibrary;

public class GException0 : ApplicationException
{
	public GException0(string strMessage)
	{
	}

	public static GException0 GenerateWithWinError(string strMessage)
	{
		return new GException0($"Msg:{strMessage} WinEr:{Marshal.GetLastWin32Error():X8}");
	}

	public static GException0 GenerateError(string strMessage)
	{
		return new GException0($"Msg:{strMessage}");
	}
}
