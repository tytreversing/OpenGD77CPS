using System;

namespace UsbLibrary;

public class DataSendEventArgs : EventArgs
{
	public readonly byte[] data;

	public DataSendEventArgs(byte[] data)
	{
		this.data = data;
	}
}
