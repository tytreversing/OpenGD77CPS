using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace DMR;

public class OpenGD77Comms
{
	private SerialPort _port;

	public ProgressBar _progressBar;

	private Action<int> _progressCallback;

	public OpenGD77Comms(SerialPort port, Action<int> progressCallback)
	{
		_port = port;
		_progressCallback = progressCallback;
	}

	private bool flashWritePrepareSector(int address, ref byte[] sendbuffer, ref byte[] readbuffer, OpenGD77CommsTransferData dataObj)
	{
		dataObj.data_sector = address / 4096;
		sendbuffer[0] = 87;
		sendbuffer[1] = 1;
		sendbuffer[2] = (byte)((dataObj.data_sector >> 16) & 0xFF);
		sendbuffer[3] = (byte)((dataObj.data_sector >> 8) & 0xFF);
		sendbuffer[4] = (byte)(dataObj.data_sector & 0xFF);
		_port.Write(sendbuffer, 0, 5);
		while (_port.BytesToRead == 0)
		{
			Thread.Sleep(0);
		}
		_port.Read(readbuffer, 0, 64);
		if (readbuffer[0] == sendbuffer[0])
		{
			return readbuffer[1] == sendbuffer[1];
		}
		return false;
	}

	private bool flashSendData(int address, int len, ref byte[] sendbuffer, ref byte[] readbuffer)
	{
		sendbuffer[0] = 87;
		sendbuffer[1] = 2;
		sendbuffer[2] = (byte)((address >> 24) & 0xFF);
		sendbuffer[3] = (byte)((address >> 16) & 0xFF);
		sendbuffer[4] = (byte)((address >> 8) & 0xFF);
		sendbuffer[5] = (byte)(address & 0xFF);
		sendbuffer[6] = (byte)((len >> 8) & 0xFF);
		sendbuffer[7] = (byte)(len & 0xFF);
		_port.Write(sendbuffer, 0, len + 8);
		while (_port.BytesToRead == 0)
		{
			Thread.Sleep(0);
		}
		_port.Read(readbuffer, 0, 64);
		if (readbuffer[0] == sendbuffer[0])
		{
			return readbuffer[1] == sendbuffer[1];
		}
		return false;
	}

	private bool flashWriteSector(ref byte[] sendbuffer, ref byte[] readbuffer, OpenGD77CommsTransferData dataObj)
	{
		dataObj.data_sector = -1;
		sendbuffer[0] = 87;
		sendbuffer[1] = 3;
		_port.Write(sendbuffer, 0, 2);
		while (_port.BytesToRead == 0)
		{
			Thread.Sleep(0);
		}
		_port.Read(readbuffer, 0, 64);
		if (readbuffer[0] == sendbuffer[0])
		{
			return readbuffer[1] == sendbuffer[1];
		}
		return false;
	}

	private void close_data_mode()
	{
	}

	public void ReadFlashOrEEPROM(OpenGD77CommsTransferData dataObj)
	{
		int num = 0;
		byte[] array = new byte[512];
		byte[] array2 = new byte[512];
		int num2 = dataObj.startDataAddressInTheRadio;
		int localDataBufferStartPosition = dataObj.localDataBufferStartPosition;
		for (int num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2; num3 > 0; num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2)
		{
			if (num3 > 32)
			{
				num3 = 32;
			}
			array[0] = 82;
			array[1] = (byte)dataObj.mode;
			array[2] = (byte)((num2 >> 24) & 0xFF);
			array[3] = (byte)((num2 >> 16) & 0xFF);
			array[4] = (byte)((num2 >> 8) & 0xFF);
			array[5] = (byte)(num2 & 0xFF);
			array[6] = (byte)((num3 >> 8) & 0xFF);
			array[7] = (byte)(num3 & 0xFF);
			_port.Write(array, 0, 8);
			while (_port.BytesToRead == 0)
			{
				Thread.Sleep(0);
			}
			_port.Read(array2, 0, 64);
			if (array2[0] == 82)
			{
				int num4 = (array2[1] << 8) + array2[2];
				for (int i = 0; i < num4; i++)
				{
					dataObj.dataBuff[localDataBufferStartPosition++] = array2[i + 3];
				}
				int num5 = (num2 - dataObj.startDataAddressInTheRadio) * 100 / dataObj.transferLength;
				if (num != num5)
				{
					_progressCallback(num5);
					num = num5;
				}
				num2 += num4;
			}
			else
			{
				Console.WriteLine($"read stopped (error at {num2:X8})");
				close_data_mode();
			}
		}
		close_data_mode();
	}

	public void WriteFlash(OpenGD77CommsTransferData dataObj)
	{
		int num = 0;
		byte[] sendbuffer = new byte[512];
		byte[] readbuffer = new byte[512];
		int num2 = dataObj.startDataAddressInTheRadio;
		int localDataBufferStartPosition = dataObj.localDataBufferStartPosition;
		dataObj.data_sector = -1;
		for (int num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2; num3 > 0; num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2)
		{
			if (num3 > 32)
			{
				num3 = 32;
			}
			if (dataObj.data_sector == -1 && !flashWritePrepareSector(num2, ref sendbuffer, ref readbuffer, dataObj))
			{
				close_data_mode();
				break;
			}
			if (dataObj.mode != 0)
			{
				int num4 = 0;
				for (int i = 0; i < num3; i++)
				{
					sendbuffer[i + 8] = dataObj.dataBuff[localDataBufferStartPosition++];
					num4++;
					if (dataObj.data_sector != (num2 + num4) / 4096)
					{
						break;
					}
				}
				if (!flashSendData(num2, num4, ref sendbuffer, ref readbuffer))
				{
					close_data_mode();
					break;
				}
				int num5 = (num2 - dataObj.startDataAddressInTheRadio) * 100 / dataObj.transferLength;
				if (num != num5)
				{
					_progressCallback(num5);
					num = num5;
				}
				num2 += num4;
				if (dataObj.data_sector != num2 / 4096 && !flashWriteSector(ref sendbuffer, ref readbuffer, dataObj))
				{
					close_data_mode();
					break;
				}
			}
		}
		if (dataObj.data_sector != -1 && !flashWriteSector(ref sendbuffer, ref readbuffer, dataObj))
		{
			Console.WriteLine($"Error. Write stopped (write sector error at {num2:X8})");
		}
		close_data_mode();
	}

	public void WriteEEPROM(OpenGD77CommsTransferData dataObj)
	{
		int num = 0;
		byte[] array = new byte[512];
		byte[] array2 = new byte[512];
		int num2 = dataObj.startDataAddressInTheRadio;
		int localDataBufferStartPosition = dataObj.localDataBufferStartPosition;
		for (int num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2; num3 > 0; num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2)
		{
			if (num3 > 32)
			{
				num3 = 32;
			}
			if (dataObj.data_sector == -1)
			{
				dataObj.data_sector = num2 / 128;
			}
			int num4 = 0;
			for (int i = 0; i < num3; i++)
			{
				array[i + 8] = dataObj.dataBuff[localDataBufferStartPosition++];
				num4++;
				if (dataObj.data_sector != (num2 + num4) / 128)
				{
					dataObj.data_sector = -1;
					break;
				}
			}
			array[0] = 87;
			array[1] = 4;
			array[2] = (byte)((num2 >> 24) & 0xFF);
			array[3] = (byte)((num2 >> 16) & 0xFF);
			array[4] = (byte)((num2 >> 8) & 0xFF);
			array[5] = (byte)(num2 & 0xFF);
			array[6] = (byte)((num4 >> 8) & 0xFF);
			array[7] = (byte)(num4 & 0xFF);
			_port.Write(array, 0, num4 + 8);
			while (_port.BytesToRead == 0)
			{
				Thread.Sleep(0);
			}
			_port.Read(array2, 0, 64);
			if (array2[0] == array[0] && array2[1] == array[1])
			{
				int num5 = (num2 - dataObj.startDataAddressInTheRadio) * 100 / dataObj.transferLength;
				if (num != num5)
				{
					_progressCallback(num5);
					num = num5;
				}
				num2 += num4;
			}
			else
			{
				Console.WriteLine($"Error. Write stopped (write sector error at {num2:X8})");
				close_data_mode();
			}
		}
		close_data_mode();
	}
}
