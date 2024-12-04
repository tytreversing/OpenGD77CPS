namespace UsbLibrary;

public abstract class Report
{
	private byte[] m_arrBuffer;

	private int m_nLength;

	public byte[] Buffer
	{
		get
		{
			return m_arrBuffer;
		}
		set
		{
			m_arrBuffer = value;
		}
	}

	public int BufferLength => m_nLength;

	public Report(HIDDevice oDev)
	{
	}

	protected void SetBuffer(byte[] arrBytes)
	{
		m_arrBuffer = arrBytes;
		m_nLength = m_arrBuffer.Length;
	}
}
