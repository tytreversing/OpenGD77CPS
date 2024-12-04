namespace UsbLibrary;

public class SpecifiedInputReport : InputReport
{
	private byte[] arrData;

	public byte[] Data => arrData;

	public SpecifiedInputReport(HIDDevice oDev)
		: base(oDev)
	{
	}

	public override void ProcessData()
	{
		arrData = base.Buffer;
	}
}
