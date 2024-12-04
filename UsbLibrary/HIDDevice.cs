using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace UsbLibrary;

public abstract class HIDDevice : Win32Usb, IDisposable
{
	private FileStream m_oFile;

	private int m_nInputReportLength;

	private int m_nOutputReportLength;

	protected IntPtr m_hHandle;

	public int OutputReportLength => m_nOutputReportLength;

	public int InputReportLength => m_nInputReportLength;

	public event EventHandler OnDeviceRemoved;

	public void Dispose()
	{
		Dispose(bDisposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool bDisposing)
	{
		try
		{
			if (bDisposing && m_oFile != null)
			{
				m_oFile.Close();
				m_oFile = null;
			}
			if (m_hHandle != IntPtr.Zero)
			{
				Win32Usb.CloseHandle(m_hHandle);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}

	private void method_0(string string_0)
	{
		m_hHandle = Win32Usb.CreateFile(string_0, 3221225472u, 3u, IntPtr.Zero, 3u, 1073741824u, IntPtr.Zero);
		if (m_hHandle != Win32Usb.InvalidHandleValue)
		{
			IntPtr lpData = default(IntPtr);
			if (Win32Usb.HidD_GetPreparsedData(m_hHandle, out lpData))
			{
				try
				{
					HidCaps oCaps = default(HidCaps);
					Win32Usb.HidP_GetCaps(lpData, out oCaps);
					m_nInputReportLength = oCaps.InputReportByteLength;
					m_nOutputReportLength = oCaps.OutputReportByteLength;
					m_oFile = new FileStream(new SafeFileHandle(m_hHandle, ownsHandle: false), FileAccess.ReadWrite, m_nInputReportLength, isAsync: true);
					return;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					throw GException0.GenerateWithWinError("Failed to get the detailed data from the hid.");
				}
				finally
				{
					Win32Usb.HidD_FreePreparsedData(ref lpData);
				}
			}
			throw GException0.GenerateWithWinError("GetPreparsedData failed");
		}
		m_hHandle = IntPtr.Zero;
		throw GException0.GenerateWithWinError("Failed to create device file");
	}

	protected bool BeginAsyncRead(byte[] data)
	{
		try
		{
			byte[] array = new byte[m_nInputReportLength];
			IAsyncResult asyncResult = m_oFile.BeginRead(array, 0, m_nInputReportLength, null, null);
			asyncResult.AsyncWaitHandle.WaitOne(3000);
			if (asyncResult.IsCompleted)
			{
				m_oFile.EndRead(asyncResult);
				Array.Copy(array, 4, data, 0, m_nInputReportLength - 4);
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
	}

	protected void BeginAsyncRead()
	{
		byte[] array = new byte[m_nInputReportLength];
		array[0] = 3;
		m_oFile.BeginRead(array, 0, m_nInputReportLength, ReadCompleted, array);
	}

	protected void ReadCompleted(IAsyncResult iResult)
	{
		byte[] data = (byte[])iResult.AsyncState;
		try
		{
			m_oFile.EndRead(iResult);
			try
			{
				InputReport inputReport = CreateInputReport();
				inputReport.SetData(data);
				HandleDataReceived(inputReport);
			}
			finally
			{
				BeginAsyncRead();
			}
		}
		catch (IOException ex)
		{
			Console.WriteLine(ex.Message);
			HandleDeviceRemoved();
			if (this.OnDeviceRemoved != null)
			{
				this.OnDeviceRemoved(this, new EventArgs());
			}
			Dispose();
		}
	}

	protected void Write(OutputReport oOutRep)
	{
		try
		{
			_ = oOutRep.BufferLength;
			m_oFile.BeginWrite(oOutRep.Buffer, 0, oOutRep.BufferLength, null, null);
		}
		catch (IOException ex)
		{
			Console.WriteLine(ex.ToString());
			throw new GException0("Probaly the device was removed");
		}
		catch (Exception ex2)
		{
			Console.WriteLine(ex2.ToString());
		}
	}

	protected virtual void HandleDataReceived(InputReport oInRep)
	{
	}

	protected virtual void HandleDeviceRemoved()
	{
	}

	private static string smethod_0(IntPtr intptr_0, ref DeviceInterfaceData deviceInterfaceData_0)
	{
		uint nRequiredSize = 0u;
		if (!Win32Usb.SetupDiGetDeviceInterfaceDetail(intptr_0, ref deviceInterfaceData_0, IntPtr.Zero, 0u, ref nRequiredSize, IntPtr.Zero))
		{
			DeviceInterfaceDetailData oDetailData = default(DeviceInterfaceDetailData);
			if (IntPtr.Size == 4)
			{
				oDetailData.Size = 5;
			}
			else
			{
				oDetailData.Size = 8;
			}
			if (Win32Usb.SetupDiGetDeviceInterfaceDetail(intptr_0, ref deviceInterfaceData_0, ref oDetailData, nRequiredSize, ref nRequiredSize, IntPtr.Zero))
			{
				return oDetailData.DevicePath;
			}
		}
		return null;
	}

	public static HIDDevice FindDevice(int nVid, int nPid, Type oType)
	{
		string value = $"vid_{nVid:x4}&pid_{nPid:x4}";
		Guid gClass = Win32Usb.HIDGuid;
		IntPtr intPtr = Win32Usb.SetupDiGetClassDevs(ref gClass, null, IntPtr.Zero, 18u);
		try
		{
			DeviceInterfaceData oInterfaceData = default(DeviceInterfaceData);
			oInterfaceData.Size = Marshal.SizeOf(oInterfaceData);
			for (int i = 0; Win32Usb.SetupDiEnumDeviceInterfaces(intPtr, 0u, ref gClass, (uint)i, ref oInterfaceData); i++)
			{
				string text = smethod_0(intPtr, ref oInterfaceData);
				if (text.IndexOf(value) >= 0)
				{
					HIDDevice obj = (HIDDevice)Activator.CreateInstance(oType);
					obj.method_0(text);
					return obj;
				}
			}
		}
		catch (Exception ex)
		{
			throw GException0.GenerateError(ex.ToString());
		}
		finally
		{
			Marshal.GetLastWin32Error();
			Win32Usb.SetupDiDestroyDeviceInfoList(intPtr);
		}
		return null;
	}

	public virtual InputReport CreateInputReport()
	{
		return null;
	}
}
