namespace UsbLibrary;

public abstract class InputReport : Report
{
	public InputReport(HIDDevice oDev)
		: base(oDev)
	{
	}

	public void SetData(byte[] arrData)
	{
		SetBuffer(arrData);
		ProcessData();
	}

	public abstract void ProcessData();
}
