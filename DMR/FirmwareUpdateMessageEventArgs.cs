using System;

namespace DMR;

public class FirmwareUpdateMessageEventArgs : EventArgs
{
	public float Percentage;

	public string Message;

	public bool IsError;

	public FirmwareUpdateMessageEventArgs(float percentage, string message, bool isError)
	{
		Percentage = percentage;
		Message = message;
		IsError = isError;
	}
}
