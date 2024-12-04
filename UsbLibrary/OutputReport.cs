namespace UsbLibrary;

public abstract class OutputReport : Report
{
	public OutputReport(HIDDevice oDev)
		: base(oDev)
	{
		SetBuffer(new byte[oDev.OutputReportLength]);
	}
}
