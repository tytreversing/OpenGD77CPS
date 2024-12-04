using System;

namespace UsbLibrary;

public class SpecifiedDevice : HIDDevice
{
	public event DataRecievedEventHandler DataRecieved;

	public event DataSendEventHandler DataSend;

	public override InputReport CreateInputReport()
	{
		return new SpecifiedInputReport(this);
	}

	public static SpecifiedDevice FindSpecifiedDevice(int vendor_id, int product_id)
	{
		return (SpecifiedDevice)HIDDevice.FindDevice(vendor_id, product_id, typeof(SpecifiedDevice));
	}

	protected override void HandleDataReceived(InputReport oInRep)
	{
		if (this.DataRecieved != null)
		{
			SpecifiedInputReport specifiedInputReport = (SpecifiedInputReport)oInRep;
			this.DataRecieved(this, new DataRecievedEventArgs(specifiedInputReport.Data));
		}
	}

	public static string ByteArrayToString(byte[] ba)
	{
		return BitConverter.ToString(ba).Replace("-", "");
	}

	public bool SendData(byte[] data)
	{
		SpecifiedOutputReport specifiedOutputReport = new SpecifiedOutputReport(this);
		specifiedOutputReport.SendData(data);
		try
		{
			Write(specifiedOutputReport);
			if (this.DataSend != null)
			{
				this.DataSend(this, new DataSendEventArgs(data));
			}
		}
		catch (GException0 gException)
		{
			Console.WriteLine(gException.Message);
			return false;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	public bool SendData(byte[] data, int index, int length)
	{
		SpecifiedOutputReport specifiedOutputReport = new SpecifiedOutputReport(this);
		specifiedOutputReport.SendData(data, index, length);
		try
		{
			Write(specifiedOutputReport);
			if (this.DataSend != null)
			{
				this.DataSend(this, new DataSendEventArgs(data));
			}
		}
		catch (GException0 gException)
		{
			Console.WriteLine(gException.Message);
			return false;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	public bool ReceiveData(byte[] data)
	{
		return BeginAsyncRead(data);
	}

	protected override void Dispose(bool bDisposing)
	{
		base.Dispose(bDisposing);
	}
}
