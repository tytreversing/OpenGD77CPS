using System;

namespace UsbLibrary;

public class DataRecievedEventArgs : EventArgs
{
	public readonly byte[] data;

	public DataRecievedEventArgs(byte[] data)
	{
		this.data = data;
	}
}
