using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace DMR;

public class CalibrationForm : Form
{
	public const int MAX_TRANSFER_BUFFER_SIZE = 1032;

	public static int MEMORY_LOCATION = 61440;

	public static int MEMORY_LOCATION_STM32 = 65536;

	public static int CALIBRATION_MEMORY_LOCATION_OFFICIAL_USB_PROTOCOL = 31744;

	private const int VHF_OFFSET = 112;

	public static int CALIBRATION_DATA_SIZE = 224;

	public static int CALIBRATION_DATA_SIZE_STM32 = 512;

	public static int CALIBRATION_HEADER_SIZE = 2;

	public static byte[] CALIBRATION_HEADER = new byte[8] { 160, 15, 192, 18, 160, 15, 192, 18 };

	private char writeCommandCharacter = 'W';

	private const int MAX_TRANSFER_SIZE = 32;

	private SerialPort commPort;

	public static Dictionary<string, string> StringsDict = new Dictionary<string, string>();

	public static byte[] calibrationDataSTM32;

	private IContainer components;

	private TabControl tabCtlBands;

	private TabPage tabVHF;

	private TabPage tabUHF;

	private Button btnWrite;

	private CalibrationBandControl calibrationBandControlUHF;

	private CalibrationBandControl calibrationBandControlVHF;

	private Button btnReadFile;

	private Button btnReadFromRadio;

	private Label lblMessage;

	private Button btnSaveCalibration;

    XmlSerializer xmlSerializer = new XmlSerializer(typeof(CalibrationStruct));

    public CalibrationForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Settings.ReadCommonsForSectionIntoDictionary(StringsDict, "OpenGD77Form");
		calibrationDataSTM32 = new byte[CALIBRATION_DATA_SIZE_STM32];
	}

	private Form getMainForm()
	{
		foreach (Form openForm in Application.OpenForms)
		{
			if (openForm is MainForm)
			{
				return openForm;
			}
		}
		return null;
	}

	private bool probeRadioModel()
	{
		if (!setupCommPort())
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(StringsDict["No_com_port"]);
			return false;
		}
		sendCommand(commPort, 254);
		commPort.DiscardInBuffer();
		if (!sendCommand(commPort, 0))
		{
			MessageBox.Show(StringsDict["Error_connecting_to_the_radio"]);
			commPort = null;
			return false;
		}
		MainForm.RadioInfo = OpenGD77Form.readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
		if (MainForm.RadioInfo.radioType == 5 || MainForm.RadioInfo.radioType == 6 || MainForm.RadioInfo.radioType == 8 || MainForm.RadioInfo.radioType == 10 || MainForm.RadioInfo.radioType == 9 || MainForm.RadioInfo.radioType == 7)
		{
			writeCommandCharacter = 'X';
			((MainForm)getMainForm())?.changeRadioType(MainForm.RadioTypeEnum.RadioTypeSTM32);
		}
		else
		{
			writeCommandCharacter = 'W';
			((MainForm)getMainForm())?.changeRadioType(MainForm.RadioTypeEnum.RadioTypeMK22);
		}
		sendCommand(commPort, 5);
		commPort.Close();
		commPort = null;
		return true;
	}

	private bool sendCommand(SerialPort port, int commandNumber, int x_or_command_option_number = 0, int y = 0, int iSize = 0, int alignment = 0, int isInverted = 0, string message = "")
	{
		byte[] array = new byte[64];
		array[0] = 67;
		array[1] = (byte)commandNumber;
		switch (commandNumber)
		{
		case 2:
			array[3] = (byte)y;
			array[4] = (byte)iSize;
			array[5] = (byte)alignment;
			array[6] = (byte)isInverted;
			Buffer.BlockCopy(Encoding.ASCII.GetBytes(message), 0, array, 7, Math.Min(message.Length, 16));
			break;
		case 6:
			array[2] = (byte)x_or_command_option_number;
			break;
		}
		commPort.Write(array, 0, 32);
		while (commPort.BytesToRead == 0)
		{
			Thread.Sleep(0);
		}
		commPort.Read(array, 0, 64);
		return array[1] == commandNumber;
	}

	private void updateProgess(int progressPercentage)
	{
	}

	private bool ReadFlashOrEEPROM(SerialPort port, OpenGD77CommsTransferData dataObj)
	{
		int num = 0;
		byte[] array = new byte[512];
		byte[] array2 = new byte[512];
		bool result = true;
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
			port.Write(array, 0, 8);
			while (port.BytesToRead == 0)
			{
				Thread.Sleep(0);
			}
			port.Read(array2, 0, 64);
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
					updateProgess(num5);
					num = num5;
				}
				num2 += num4;
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	public bool readDataFromRadio()
	{
		bool result = true;
		lblMessage.Text = "";
		int num = Marshal.SizeOf(typeof(CalibrationData));
		byte[] array = new byte[num];
		if (!probeRadioModel())
		{
			return false;
		}
		if (!setupCommPort())
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(StringsDict["No_com_port"]);
			return false;
		}
		OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData();
		sendCommand(commPort, 0);
		sendCommand(commPort, 1);
        sendCommand(commPort, 2, 0, 0, 3, 1, 0, "CPS");
        sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Reading"]);
        sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_Calibrations"]);
        sendCommand(commPort, 3);
		sendCommand(commPort, 6, 3);
		openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadFlash;
		openGD77CommsTransferData.dataBuff = new byte[(MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32) ? CALIBRATION_DATA_SIZE_STM32 : CALIBRATION_DATA_SIZE];
		openGD77CommsTransferData.localDataBufferStartPosition = 0;
		openGD77CommsTransferData.startDataAddressInTheRadio = ((MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32) ? MEMORY_LOCATION_STM32 : MEMORY_LOCATION);
		openGD77CommsTransferData.transferLength = ((MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32) ? CALIBRATION_DATA_SIZE_STM32 : CALIBRATION_DATA_SIZE);
		if (!ReadFlashOrEEPROM(commPort, openGD77CommsTransferData))
		{
			result = false;
			openGD77CommsTransferData.responseCode = 1;
		}
		sendCommand(commPort, 5);
		sendCommand(commPort, 7);
		commPort.Close();
		commPort = null;
		if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeMK22)
		{
			for (int i = 0; i < CALIBRATION_HEADER_SIZE; i++)
			{
				if (openGD77CommsTransferData.dataBuff[i] != CALIBRATION_HEADER[i])
				{
					MessageBox.Show(StringsDict["Calibration_data_could_not_be_found"]);
					return false;
				}
			}
		}
		if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
		{
			Array.Copy(openGD77CommsTransferData.dataBuff, 0, calibrationDataSTM32, 0, calibrationDataSTM32.Length);
		}
		else
		{
			Array.Copy(openGD77CommsTransferData.dataBuff, 0, array, 0, num);
			calibrationBandControlUHF.data = ByteToData(array);
			array = new byte[num];
			Array.Copy(openGD77CommsTransferData.dataBuff, 112, array, 0, num);
			calibrationBandControlVHF.data = ByteToData(array);
		}
		return result;
	}

	private bool setupCommPort()
	{
		if (commPort != null)
		{
			try
			{
				if (commPort.IsOpen)
				{
					commPort.Close();
				}
			}
			catch (Exception)
			{
			}
			commPort = null;
		}
		try
		{
			string text = null;
			text = SetupDiWrap.ComPortNameFromFriendlyNamePrefix("OpenGD77");
			if (text == null)
			{
				CommPortSelector commPortSelector = new CommPortSelector();
				if (DialogResult.OK != commPortSelector.ShowDialog())
				{
					return false;
				}
				text = SetupDiWrap.ComPortNameFromFriendlyNamePrefix(commPortSelector.SelectedPort);
				IniFileUtils.WriteProfileString("Setup", "LastCommPort", text);
			}
			if (text == null)
			{
				MessageBox.Show(StringsDict["Please_connect_the_radio,_and_try_again."], StringsDict["Radio_not_detected."]);
			}
			else
			{
				commPort = new SerialPort(text, 115200, Parity.None, 8, StopBits.One);
				commPort.ReadTimeout = 1000;
			}
		}
		catch (Exception)
		{
			commPort = null;
			SystemSounds.Hand.Play();
			MessageBox.Show(StringsDict["Failed_to_open_comm_port"], StringsDict["Error"]);
			IniFileUtils.WriteProfileString("Setup", "LastCommPort", "");
			return false;
		}
		try
		{
			commPort.Open();
		}
		catch (Exception)
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(StringsDict["Comm_port_not_available"]);
			commPort = null;
			return false;
		}
		return true;
	}

	private void btnWrite_Click(object sender, EventArgs e)
	{
		MainForm.RadioTypeEnum radioType = MainForm.RadioType;
		if (!probeRadioModel() || radioType != MainForm.RadioType)
		{
			return;
		}
		if (!setupCommPort())
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(StringsDict["No_com_port"]);
			return;
		}
		OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData();
		if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
		{
			openGD77CommsTransferData.dataBuff = new byte[CALIBRATION_DATA_SIZE_STM32];
			Array.Copy(calibrationDataSTM32, 0, openGD77CommsTransferData.dataBuff, 0, CALIBRATION_DATA_SIZE_STM32);
		}
		else
		{
			openGD77CommsTransferData.dataBuff = new byte[CALIBRATION_DATA_SIZE];
			int length = Marshal.SizeOf(typeof(CalibrationData));
			Array.Copy(DataToByte(calibrationBandControlUHF.data), 0, openGD77CommsTransferData.dataBuff, 0, length);
			Array.Copy(DataToByte(calibrationBandControlVHF.data), 0, openGD77CommsTransferData.dataBuff, 112, length);
		}
		sendCommand(commPort, 0);
		sendCommand(commPort, 1);
        sendCommand(commPort, 2, 0, 0, 3, 1, 0, "CPS");
        sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Restoring"]);
        sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_Calibrations"]);
        sendCommand(commPort, 3);
		sendCommand(commPort, 6, 4);
		openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteFlash;
		openGD77CommsTransferData.localDataBufferStartPosition = 0;
		openGD77CommsTransferData.startDataAddressInTheRadio = ((MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32) ? MEMORY_LOCATION_STM32 : MEMORY_LOCATION);
		openGD77CommsTransferData.transferLength = ((MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32) ? CALIBRATION_DATA_SIZE_STM32 : CALIBRATION_DATA_SIZE);
		if (!WriteFlash(commPort, openGD77CommsTransferData))
		{
			MessageBox.Show("Ошибка при восстановлении!");
			openGD77CommsTransferData.responseCode = 1;
		}
		sendCommand(commPort, 6, 2);
		sendCommand(commPort, 6, 1);
		commPort.Close();
		commPort = null;
		lblMessage.Text = "Обновление калибровок завершено";
	}

	private bool flashWriteSector(SerialPort port, char writeCharacter, ref byte[] sendbuffer, ref byte[] readbuffer, OpenGD77CommsTransferData dataObj)
	{
		int num = 100;
		dataObj.data_sector = -1;
		sendbuffer[0] = (byte)writeCharacter;
		sendbuffer[1] = 3;
		port.Write(sendbuffer, 0, 2);
		while (port.BytesToRead == 0)
		{
			Thread.Sleep(1);
		}
		Thread.Sleep(100);
		while (port.BytesToRead == 0 && num-- > 0)
		{
			Thread.Sleep(5);
		}
		port.Read(readbuffer, 0, port.BytesToRead);
		if (readbuffer[0] == sendbuffer[0] && readbuffer[1] == sendbuffer[1])
		{
			return num != -1;
		}
		return false;
	}

	private bool flashWritePrepareSector(SerialPort port, char writeCharacter, int address, ref byte[] sendbuffer, ref byte[] readbuffer, OpenGD77CommsTransferData dataObj)
	{
		int num = 100;
		dataObj.data_sector = address / 4096;
		sendbuffer[0] = (byte)writeCharacter;
		sendbuffer[1] = 1;
		sendbuffer[2] = (byte)((dataObj.data_sector >> 16) & 0xFF);
		sendbuffer[3] = (byte)((dataObj.data_sector >> 8) & 0xFF);
		sendbuffer[4] = (byte)(dataObj.data_sector & 0xFF);
		port.Write(sendbuffer, 0, 5);
		while (port.BytesToRead == 0)
		{
			Thread.Sleep(1);
		}
		Thread.Sleep(50);
		while (port.BytesToRead == 0 && num-- > 0)
		{
			Thread.Sleep(1);
		}
		if (num != -1)
		{
			port.Read(readbuffer, 0, port.BytesToRead);
		}
		if (readbuffer[0] == sendbuffer[0] && readbuffer[1] == sendbuffer[1])
		{
			return num != -1;
		}
		return false;
	}

	private bool flashSendData(SerialPort port, char writeCharacter, int address, int len, ref byte[] sendbuffer, ref byte[] readbuffer)
	{
		int num = 100;
		sendbuffer[0] = (byte)writeCharacter;
		sendbuffer[1] = 2;
		sendbuffer[2] = (byte)((address >> 24) & 0xFF);
		sendbuffer[3] = (byte)((address >> 16) & 0xFF);
		sendbuffer[4] = (byte)((address >> 8) & 0xFF);
		sendbuffer[5] = (byte)(address & 0xFF);
		sendbuffer[6] = (byte)((len >> 8) & 0xFF);
		sendbuffer[7] = (byte)(len & 0xFF);
		port.Write(sendbuffer, 0, len + 8);
		while (port.BytesToRead == 0)
		{
			Thread.Sleep(1);
		}
		Thread.Sleep(20);
		while (port.BytesToRead == 0 && num-- > 0)
		{
			Thread.Sleep(1);
		}
		if (num != -1)
		{
			port.Read(readbuffer, 0, port.BytesToRead);
		}
		if (readbuffer[0] == sendbuffer[0] && readbuffer[1] == sendbuffer[1])
		{
			return num != -1;
		}
		return false;
	}

	private bool WriteFlash(SerialPort port, OpenGD77CommsTransferData dataObj)
	{
		int num = 0;
		byte[] sendbuffer = new byte[1032];
		byte[] readbuffer = new byte[1032];
		int num2 = dataObj.startDataAddressInTheRadio;
		int localDataBufferStartPosition = dataObj.localDataBufferStartPosition;
		dataObj.data_sector = -1;
		for (int num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2; num3 > 0; num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2)
		{
			if (num3 > OpenGD77Form.getUSBWriteBufferSize())
			{
				num3 = OpenGD77Form.getUSBWriteBufferSize();
			}
			if (dataObj.data_sector == -1 && !flashWritePrepareSector(port, writeCommandCharacter, num2, ref sendbuffer, ref readbuffer, dataObj))
			{
				return false;
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
				if (!flashSendData(port, writeCommandCharacter, num2, num4, ref sendbuffer, ref readbuffer))
				{
					return false;
				}
				int num5 = (num2 - dataObj.startDataAddressInTheRadio) * 100 / dataObj.transferLength;
				if (num != num5)
				{
					updateProgess(num5);
					num = num5;
				}
				num2 += num4;
				if (dataObj.data_sector != num2 / 4096 && !flashWriteSector(port, writeCommandCharacter, ref sendbuffer, ref readbuffer, dataObj))
				{
					return false;
				}
			}
		}
		if (dataObj.data_sector != -1 && !flashWriteSector(port, writeCommandCharacter, ref sendbuffer, ref readbuffer, dataObj))
		{
			Console.WriteLine($"Error. Write stopped (write sector error at {num2:X8})");
			return false;
		}
		return true;
	}

	private CalibrationData ByteToData(byte[] byte_0)
	{
		int num = Marshal.SizeOf(typeof(CalibrationData));
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.Copy(byte_0, 0, intPtr, num);
		object obj = Marshal.PtrToStructure(intPtr, typeof(CalibrationData));
		Marshal.FreeHGlobal(intPtr);
		CalibrationData calibrationData = (CalibrationData)obj;
		if (calibrationData.DACOscRefTune > 511)
		{
			calibrationData.DACOscRefTune -= 1024;
		}
		return calibrationData;
	}

	public static byte[] DataToByte(CalibrationData calData)
	{
		if (calData.DACOscRefTune < 0)
		{
			calData.DACOscRefTune = (short)(1024 + calData.DACOscRefTune);
		}
		calData.Q_MOD2_OFFSET = (sbyte)calData.DACOscRefTune;
		int num = Marshal.SizeOf(typeof(CalibrationData));
		byte[] array = new byte[num];
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.StructureToPtr(calData, intPtr, fDeleteOld: false);
		Marshal.Copy(intPtr, array, 0, num);
		Marshal.FreeHGlobal(intPtr);
		return array;
	}

	private void onFormLoad(object sender, EventArgs e)
	{
		try
		{
			Settings.UpdateComponentTextsFromLanguageXmlData(this);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	private void onFormShown(object sender, EventArgs e)
	{
		MessageBox.Show(StringsDict["This_feature_is_highly_experimental"], StringsDict["Warning"]);
	}

	private void btnReadFile_Click(object sender, EventArgs e)
	{
		byte[] array = null;
		OpenFileDialog openFileDialog = new OpenFileDialog();
		lblMessage.Text = "";
		if (openFileDialog.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(openFileDialog.FileName))
		{
			return;
		}
		int num = Marshal.SizeOf(typeof(CalibrationData));
		byte[] array2 = new byte[num];
		array = File.ReadAllBytes(openFileDialog.FileName);
		if (array.Length == CALIBRATION_DATA_SIZE)
		{
			if (array[0] == 160 && array[1] == 15)
			{
				((MainForm)getMainForm())?.changeRadioType(MainForm.RadioTypeEnum.RadioTypeMK22);
				Array.Copy(array, 0, array2, 0, num);
				calibrationBandControlUHF.data = ByteToData(array2);
				array2 = new byte[num];
				Array.Copy(array, 112, array2, 0, num);
				calibrationBandControlVHF.data = ByteToData(array2);
				showButtons();
			}
			else
			{
				MessageBox.Show("Неверный заголовок файла для процессора МК22!");
			}
		}
		else if (array.Length == CALIBRATION_DATA_SIZE_STM32)
		{
			int num2 = 432;
			if (array[num2] == 0 && array[num2 + 1] == 37 && array[num2 + 2] == 0 && array[num2 + 3] == 64 && array[num2 + 4] == 0 && array[num2 + 5] == 69 && array[num2 + 6] == 1 && array[num2 + 7] == 64)
			{
				((MainForm)getMainForm())?.changeRadioType(MainForm.RadioTypeEnum.RadioTypeSTM32);
				Array.Copy(array, 0, calibrationDataSTM32, 0, CALIBRATION_DATA_SIZE_STM32);
				showButtons();
			}
			else
			{
                MessageBox.Show("Неверный заголовок файла для процессора STM32!");
            }
		}
		else
		{
			MessageBox.Show(StringsDict["File_is_the_wrong_size"]);
		}
	}

	private void btnReadFromRadio_Click(object sender, EventArgs e)
	{
		if (readDataFromRadio())
		{
			showButtons();
		}
	}

	private void showButtons()
	{
		tabCtlBands.Visible = MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeMK22;
		btnWrite.Visible = true;
		btnSaveCalibration.Visible = true;
	}

	

	private void btnSaveCalibration_Click(object sender, EventArgs e)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		lblMessage.Text = "";
		if (saveFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(saveFileDialog.FileName))
		{
			if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
			{
				File.WriteAllBytes(saveFileDialog.FileName, calibrationDataSTM32);
            }
			else
			{
				byte[] array = new byte[CALIBRATION_DATA_SIZE];
				int length = Marshal.SizeOf(typeof(CalibrationData));
				Array.Copy(DataToByte(calibrationBandControlUHF.data), 0, array, 0, length);
				Array.Copy(DataToByte(calibrationBandControlVHF.data), 0, array, 112, length);
				File.WriteAllBytes(saveFileDialog.FileName, array);
			}
			lblMessage.Text = StringsDict["File_saved"];
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
            this.tabCtlBands = new System.Windows.Forms.TabControl();
            this.tabVHF = new System.Windows.Forms.TabPage();
            this.tabUHF = new System.Windows.Forms.TabPage();
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnReadFile = new System.Windows.Forms.Button();
            this.btnReadFromRadio = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnSaveCalibration = new System.Windows.Forms.Button();
            this.calibrationBandControlVHF = new DMR.CalibrationBandControl();
            this.calibrationBandControlUHF = new DMR.CalibrationBandControl();
            this.tabCtlBands.SuspendLayout();
            this.tabVHF.SuspendLayout();
            this.tabUHF.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabCtlBands
            // 
            this.tabCtlBands.Controls.Add(this.tabVHF);
            this.tabCtlBands.Controls.Add(this.tabUHF);
            this.tabCtlBands.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabCtlBands.Location = new System.Drawing.Point(12, 85);
            this.tabCtlBands.Name = "tabCtlBands";
            this.tabCtlBands.SelectedIndex = 0;
            this.tabCtlBands.Size = new System.Drawing.Size(921, 549);
            this.tabCtlBands.TabIndex = 0;
            // 
            // tabVHF
            // 
            this.tabVHF.Controls.Add(this.calibrationBandControlVHF);
            this.tabVHF.Location = new System.Drawing.Point(4, 23);
            this.tabVHF.Name = "tabVHF";
            this.tabVHF.Padding = new System.Windows.Forms.Padding(3);
            this.tabVHF.Size = new System.Drawing.Size(913, 522);
            this.tabVHF.TabIndex = 0;
            this.tabVHF.Text = "VHF";
            this.tabVHF.UseVisualStyleBackColor = true;
            // 
            // tabUHF
            // 
            this.tabUHF.Controls.Add(this.calibrationBandControlUHF);
            this.tabUHF.Location = new System.Drawing.Point(4, 23);
            this.tabUHF.Name = "tabUHF";
            this.tabUHF.Padding = new System.Windows.Forms.Padding(3);
            this.tabUHF.Size = new System.Drawing.Size(913, 497);
            this.tabUHF.TabIndex = 1;
            this.tabUHF.Text = "UHF";
            this.tabUHF.UseVisualStyleBackColor = true;
            // 
            // btnWrite
            // 
            this.btnWrite.BackColor = System.Drawing.SystemColors.Control;
            this.btnWrite.Font = new System.Drawing.Font("Arial", 8F);
            this.btnWrite.Location = new System.Drawing.Point(12, 41);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(239, 23);
            this.btnWrite.TabIndex = 1;
            this.btnWrite.Text = "Write to radio";
            this.btnWrite.UseVisualStyleBackColor = false;
            this.btnWrite.Visible = false;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // btnReadFile
            // 
            this.btnReadFile.BackColor = System.Drawing.SystemColors.Control;
            this.btnReadFile.Font = new System.Drawing.Font("Arial", 8F);
            this.btnReadFile.Location = new System.Drawing.Point(722, 12);
            this.btnReadFile.Name = "btnReadFile";
            this.btnReadFile.Size = new System.Drawing.Size(216, 23);
            this.btnReadFile.TabIndex = 1;
            this.btnReadFile.Text = "Open Calibration file";
            this.btnReadFile.UseVisualStyleBackColor = false;
            this.btnReadFile.Click += new System.EventHandler(this.btnReadFile_Click);
            // 
            // btnReadFromRadio
            // 
            this.btnReadFromRadio.BackColor = System.Drawing.SystemColors.Control;
            this.btnReadFromRadio.Font = new System.Drawing.Font("Arial", 8F);
            this.btnReadFromRadio.Location = new System.Drawing.Point(12, 12);
            this.btnReadFromRadio.Name = "btnReadFromRadio";
            this.btnReadFromRadio.Size = new System.Drawing.Size(239, 23);
            this.btnReadFromRadio.TabIndex = 1;
            this.btnReadFromRadio.Text = "Read calibration from radio";
            this.btnReadFromRadio.UseVisualStyleBackColor = false;
            this.btnReadFromRadio.Click += new System.EventHandler(this.btnReadFromRadio_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.lblMessage.Location = new System.Drawing.Point(257, 12);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(459, 35);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "Please read the calibration data from the radio or open a calibration file";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSaveCalibration
            // 
            this.btnSaveCalibration.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveCalibration.Font = new System.Drawing.Font("Arial", 8F);
            this.btnSaveCalibration.Location = new System.Drawing.Point(722, 41);
            this.btnSaveCalibration.Name = "btnSaveCalibration";
            this.btnSaveCalibration.Size = new System.Drawing.Size(216, 23);
            this.btnSaveCalibration.TabIndex = 1;
            this.btnSaveCalibration.Text = "Save Calibration file";
            this.btnSaveCalibration.UseVisualStyleBackColor = false;
            this.btnSaveCalibration.Visible = false;
            this.btnSaveCalibration.Click += new System.EventHandler(this.btnSaveCalibration_Click);
            // 
            // calibrationBandControlVHF
            // 
            this.calibrationBandControlVHF.Font = new System.Drawing.Font("Arial", 8F);
            this.calibrationBandControlVHF.Location = new System.Drawing.Point(6, 7);
            this.calibrationBandControlVHF.Name = "calibrationBandControlVHF";
            this.calibrationBandControlVHF.Size = new System.Drawing.Size(880, 509);
            this.calibrationBandControlVHF.TabIndex = 0;
            this.calibrationBandControlVHF.Type = "VHF";
            // 
            // calibrationBandControlUHF
            // 
            this.calibrationBandControlUHF.Location = new System.Drawing.Point(5, 5);
            this.calibrationBandControlUHF.Name = "calibrationBandControlUHF";
            this.calibrationBandControlUHF.Size = new System.Drawing.Size(882, 487);
            this.calibrationBandControlUHF.TabIndex = 0;
            this.calibrationBandControlUHF.Type = "UHF";
            // 
            // CalibrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(945, 661);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnReadFromRadio);
            this.Controls.Add(this.btnSaveCalibration);
            this.Controls.Add(this.btnReadFile);
            this.Controls.Add(this.btnWrite);
            this.Controls.Add(this.tabCtlBands);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalibrationForm";
            this.Text = "Calibration";
            this.Load += new System.EventHandler(this.onFormLoad);
            this.Shown += new System.EventHandler(this.onFormShown);
            this.tabCtlBands.ResumeLayout(false);
            this.tabVHF.ResumeLayout(false);
            this.tabUHF.ResumeLayout(false);
            this.ResumeLayout(false);

	}
}

/*typedef struct
{
//0x00
	uint8_t VoxLevel1;				//calibration for Vox Setting 1
	uint8_t VoxLevel10;				//calibration for Vox Setting 10
	uint8_t RxLowVoltage;			//Exact use unknown
	uint8_t RxHighVoltage;			//Exact use unknown
	uint8_t RSSI120;			    //RSSI Calibration for -120dBm
	uint8_t RSSI70;					//RSSI Calibration for -70dBm
	uint8_t UnknownBlock1[2];		//Unknown
//0x08
	uint8_t Unknown1;				//Unknown
	uint8_t UHFOscRefTune;			//UHF reference tuning
	uint8_t UnknownBlock2[2];		//Unknown
	uint8_t VHFOscRefTune;			//UHF reference tuning
	uint8_t UnknownBlock3[3];		//Unknown
//0x10
	uint8_t UHFHighPowerCal[9];	//UHF High Power Calibration 9 frequencies
//0x19
	uint8_t VHFHighPowerCal[5];	//VHF High Power Calibration 5 frequencies
//0x1E
	uint8_t UnknownBlock4[2];		//Unknown
//0x20
	uint8_t UHFLowPowerCal[9];		//UHF Low Power Calibration 9 frequencies
//0x29
	uint8_t VHFLowPowerCal[5];		//VHF Low Power Calibration 5 frequencies
//0x2E
	uint8_t UnknownBlock5[2];		//Unknown
//0x30
	uint8_t UHFRxTuning[9];		//UHF Rx Front End Tuning 9 frequencies
//0x39
	uint8_t VHFRxTuning[5];		//VHF Rx Front End Tuning 5 frequencies
//0x3E
	uint8_t UnknownBlock6[2];		//Unknown
//0x40
	uint8_t UHFOpenSquelch9[9];   //UHF Squelch Level 9 Opening  9 frequencies
//0x49
	uint8_t UnknownBlock7[7];		//Unknown
//0x50
	uint8_t UHFCloseSquelch9[9];   //UHF Squelch Level 9 Closing 9 frequencies
//0x59
	uint8_t UnknownBlock8[7];		//Unknown
//0x60
	uint8_t UHFOpenSquelch1[9];   //UHF Squelch Level 1 Opening  9 frequencies
//0x69
	uint8_t UnknownBlock9[7];		//Unknown
//0x70
	uint8_t UHFCloseSquelch1[9];   //UHF Squelch Level 1 Closing 9 frequencies
//0x79
	uint8_t UnknownBlock10[7];		//Unknown
//0x80
	uint8_t UnknownBlock11[16];		//Unknown
//0x90
	uint8_t UHFCTC67[9];			//UHF CTCSS Deviation for 67Hz Tone 9 frequencies
//0x99
	uint8_t UnknownBlock12[2];		//Unknown
//0x9B
	uint8_t VHFCTC67[5];			//VHF CTCSS Deviation for 67Hz Tone 5 frequencies
//0xA0
	uint8_t UHFCTC151[9];			//UHF CTCSS Deviation for 151.4Hz Tone 9 frequencies
//0xA9
	uint8_t UnknownBlock13[2];		//Unknown
//0x9B
	uint8_t VHFCTC151[5];			//VHF CTCSS Deviation for 151.4Hz Tone 5 frequencies
//0xB0
	uint8_t UHFCTC254[9];			//UHF CTCSS Deviation for 254.1Hz Tone 9 frequencies
//0xB9
	uint8_t UnknownBlock14[2];		//Unknown
//0xBB
	uint8_t VHFCTC254[5];			//VHF CTCSS Deviation for 254.1Hz Tone 5 frequencies
//0xC0
	uint8_t UnknownBlock15[16];		//Unknown
//0xD0
	uint8_t UHFDCS[9];				//UHF DCS Deviation 9 frequencies
//0xD9
	uint8_t UnknownBlock16[2];		//Unknown
//0xDB
	uint8_t VHFDCS[5];				//VHF DCS Deviation 5 frequencies
//0xE0
	uint8_t VHFOpenSquelch9[5];     //VHF Squelch Level 9 Opening  5 frequencies
	uint8_t VHFCloseSquelch9[5];     //VHF Squelch Level 9 Closing  5 frequencies
	uint8_t VHFOpenSquelch1[5];     //VHF Squelch Level 1 Opening  5 frequencies
	uint8_t VHFCloseSquelch1[5];     //VHF Squelch Level 1 Closing  5 frequencies
//0xF4
	uint8_t UnknownBlock17[12];		//Unknown
//0x100
	uint8_t VHFCalFreqs[10][4];		// VHF Calibration Frequencies 4 BCD bytes per freq, 5 pairs of freqs Rx and Tx
//0x128
	uint8_t UnknownBlock18[8];		//Unknown
//0x130
	uint8_t UHFDMRIGain[9];			//UHF I Gain for DMR	9 Frequencies
//0x139
	uint8_t VHFDMRIGain[5];			//VHF I Gain for DMR	5 Frequencies
	uint8_t UnknownBlock19[2];		//Unknown
//0x140
	uint8_t UHFDMRQGain[9];			//UHF Q Gain for DMR	9 Frequencies
//0x149
	uint8_t VHFDMRQGain[5];			//VHF Q Gain for DMR	5 Frequencies
	uint8_t UnknownBlock20[2];		//Unknown
//0x150
	uint8_t UnknownBlock21[32];		//Unknown
//0x170
	uint8_t UHFFMIGain[9];			//UHF I Gain for FM	9 Frequencies
//0x179
	uint8_t VHFFMIGain[5];			//VHF I Gain for FM	5 Frequencies
	uint8_t UnknownBlock22[2];		//Unknown
//0x180
	uint8_t UHFFMQGain[9];			//UHF Q Gain for FM	9 Frequencies
//0x189
	uint8_t VHFFMQGain[5];			//VHF Q Gain for FM	5 Frequencies
	uint8_t UnknownBlock23[2];		//Unknown
//0x190
	uint8_t UHFMidPowerCal[9];		//UHF Mid Power Calibration 9 frequencies
//0x199
	uint8_t VHFMidPowerCal[5];		//VHF Mid Power Calibration 5 frequencies
//0x19E
	uint8_t UnknownBlock24[2];		//Unknown
//0x1A0
	uint8_t UHFMidLowPowerCal[9];		//UHF MidLow Power Calibration 9 frequencies
//0x1A9
	uint8_t VHFMidLowPowerCal[5];		//VHF MidLow Power Calibration 5 frequencies
//0x1AE
	uint8_t UnknownBlock25[2];		//Unknown
//0x1B0
	uint8_t UHFCalFreqs[18][4];		// UHF Calibration Frequencies 4 BCD bytes per freq, 9 pairs of freqs Rx and Tx
	uint8_t UnknownBlock26[8];		//Unknown
//0x200
} CalibrationData_t;*/

