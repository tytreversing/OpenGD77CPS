using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using DMR;

namespace Extras.OpenGD77;

public class ThemeForm : Form
{
	private const int NUM_THEME_COLOURS = 32;

	public static readonly int THEME_SIZE = 64;

	private const int NUM_COLUMNS = 3;

	private const int NUM_ROWS = 12;

	private const string THEME_EXTENSION_FILTER = " (*.gtm)|*.gtm|(*.*)|*.*";

	private ColorDialog colourDlg = new ColorDialog();

	private BackgroundWorker worker;

	private SerialPort commPort;

	private char writeCommandCharacter = 'W';

	private Label[] colourLabels = new Label[32];

	private Label[] textLabels = new Label[32];

	private byte[] themeData;

	public static Dictionary<string, string> ThemeStringsDict = new Dictionary<string, string>();

	public static Dictionary<string, string> OpenGD77StringsDict = new Dictionary<string, string>();

	private IContainer components;

	private Button btnOpen;

	private Button btnSave;

	private Button btnWrite;

	private Button btnRead;

	private Label txtMessage;

	private ProgressBar progressBar1;

	private GroupBox gbColours;

	private Button btnOK;

	private RadioButton rbDaytimeDay;

	private RadioButton rbDaytimeNight;

	private GroupBox grpDaytimeTheme;

	public ThemeForm()
	{
		InitializeComponent();
        base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		colourDlg.FullOpen = true;
		themeData = new byte[THEME_SIZE];
		SetupColoursAndLabels();
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		Settings.ReadCommonsForSectionIntoDictionary(ThemeStringsDict, "ThemeForm");
		Settings.ReadCommonsForSectionIntoDictionary(OpenGD77StringsDict, "OpenGD77Form");
		readThemeFromCodeplug(ref OpenGD77Form.CustomData);
	}

	private void SetupColoursAndLabels()
	{
		int num = themeData.Length / 2 - 1;
		SuspendLayout();
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 12; j++)
			{
				int num2 = i * 12 + j;
				if (num2 > num)
				{
					return;
				}
				Label label = (colourLabels[num2] = new Label());
				label.AutoSize = false;
				label.Size = new Size(20, 20);
				label.BackColor = Color.White;
				label.BorderStyle = BorderStyle.FixedSingle;
				label.Location = new Point(10 + i * 250, 20 + j * 30);
				label.TabIndex = num2;
				label.Text = "";
				label.Click += backgroundColour_Click;
				label.Cursor = Cursors.Hand;
				gbColours.Controls.Add(label);
				label = (textLabels[num2] = new Label());
				label.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
				label.AutoSize = true;
				label.Location = new Point(35 + i * 250, 20 + j * 30 + 4);
				label.TabIndex = num2 * 2 + 1;
				label.Name = "lblColour_" + num2;
				gbColours.Controls.Add(label);
			}
		}
		ResumeLayout();
	}

	private void updateLabelColours()
	{
		int num = themeData.Length / 2 - 1;
		SuspendLayout();
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 12; j++)
			{
				int num2 = i * 12 + j;
				if (num2 > num)
				{
					return;
				}
				ushort rgb = (ushort)((themeData[num2 * 2] << 8) + themeData[num2 * 2 + 1]);
				colourLabels[num2].BackColor = RGB565ToColor(rgb);
			}
		}
		ResumeLayout();
	}

	private void readLabelColours()
	{
		int num = themeData.Length / 2 - 1;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 12; j++)
			{
				int num2 = i * 12 + j;
				if (num2 > num)
				{
					return;
				}
				ushort num3 = ColorToRGB565(colourLabels[num2].BackColor);
				themeData[num2 * 2] = (byte)(num3 >> 8);
				themeData[num2 * 2 + 1] = (byte)(num3 & 0xFF);
			}
		}
	}

	private void readThemeFromCodeplug(ref byte[] customData)
	{
		int returnedOffsetPos = 0;
		if (OpenGD77Form.FindCustomDataBlock(customData, rbDaytimeDay.Checked ? OpenGD77Form.CustomDataType.THEME_DAY_TYPE : OpenGD77Form.CustomDataType.THEME_NIGHT_TYPE, ref returnedOffsetPos))
		{
			Array.Copy(customData, returnedOffsetPos + 8, themeData, 0, THEME_SIZE);
			updateLabelColours();
		}
		else if (OpenGD77Form.FindCustomDataBlock(customData, OpenGD77Form.CustomDataType.UNINITIALISED_TYPE, ref returnedOffsetPos, THEME_SIZE))
		{
			OpenGD77Form.CustomData[returnedOffsetPos] = (byte)(rbDaytimeDay.Checked ? 4u : 5u);
			OpenGD77Form.CustomData[returnedOffsetPos + 1] = 0;
			OpenGD77Form.CustomData[returnedOffsetPos + 2] = 0;
			OpenGD77Form.CustomData[returnedOffsetPos + 3] = 0;
			OpenGD77Form.CustomData[returnedOffsetPos + 4] = (byte)THEME_SIZE;
			OpenGD77Form.CustomData[returnedOffsetPos + 5] = 0;
			OpenGD77Form.CustomData[returnedOffsetPos + 6] = 0;
			OpenGD77Form.CustomData[returnedOffsetPos + 7] = 0;
			byte[] array = File.ReadAllBytes(Application.StartupPath + Path.DirectorySeparatorChar + "DefaultTheme.gtm");
			Array.Copy(array, 0, themeData, 0, array.Length);
			Array.Copy(themeData, 0, customData, returnedOffsetPos + 8, themeData.Length);
			updateLabelColours();
		}
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
				MessageBox.Show(OpenGD77StringsDict["Please_connect_the_radio,_and_try_again."], OpenGD77StringsDict["Radio_not_detected."]);
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
			MessageBox.Show(OpenGD77StringsDict["Failed_to_open_comm_port"], OpenGD77StringsDict["Error"]);
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
			MessageBox.Show(OpenGD77StringsDict["Comm_port_not_available"]);
			commPort = null;
			return false;
		}
		return true;
	}

	private void displayMessage(string message)
	{
		if (txtMessage.InvokeRequired)
		{
			txtMessage.Invoke((MethodInvoker)delegate
			{
				txtMessage.Text = message;
			});
		}
		else
		{
			txtMessage.Text = message;
		}
	}

	private bool probeRadioModel(bool stealth = false)
	{
		if (!setupCommPort())
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(OpenGD77StringsDict["No_com_port"]);
			return false;
		}
		OpenGD77Form.sendCommand(commPort, 254);
		commPort.DiscardInBuffer();
		if (!OpenGD77Form.sendCommand(commPort, stealth ? 254 : 0))
		{
			displayMessage(OpenGD77StringsDict["Error_connecting_to_the_radio"]);
			commPort = null;
			return false;
		}
		MainForm.RadioInfo = OpenGD77Form.readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort, stealth);
		if (MainForm.RadioInfo.identifier == "RUSSIAN")
		{
			if (MainForm.RadioInfo.radioType == 5 || MainForm.RadioInfo.radioType == 6 || MainForm.RadioInfo.radioType == 8 || MainForm.RadioInfo.radioType == 10 || MainForm.RadioInfo.radioType == 9 || MainForm.RadioInfo.radioType == 7)
			{
				writeCommandCharacter = 'X';
				((MainForm)OpenGD77Form.getMainForm())?.changeRadioType(MainForm.RadioTypeEnum.RadioTypeSTM32);
			}
			else
			{
				writeCommandCharacter = 'W';
				((MainForm)OpenGD77Form.getMainForm())?.changeRadioType(MainForm.RadioTypeEnum.RadioTypeMK22);
			}
			if (!stealth)
			{
				OpenGD77Form.sendCommand(commPort, 5);
			}
			commPort.Close();
			commPort = null;
			return true;
		}
		else
		{
            OpenGD77Form.sendCommand(commPort, 6);
            commPort.Close();
            commPort = null;
            MessageBox.Show(OpenGD77Form.StringsDict["Incorrect_firmware"], OpenGD77Form.StringsDict["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
	}

	private bool SaveToCodeplug(OpenGD77Form.CustomDataType daytimeOverride = OpenGD77Form.CustomDataType.UNINITIALISED_TYPE)
	{
		int returnedOffsetPos = 0;
		bool flag = false;
		OpenGD77Form.CustomDataType customDataType = ((daytimeOverride != OpenGD77Form.CustomDataType.UNINITIALISED_TYPE) ? daytimeOverride : (rbDaytimeDay.Checked ? OpenGD77Form.CustomDataType.THEME_DAY_TYPE : OpenGD77Form.CustomDataType.THEME_NIGHT_TYPE));
		readLabelColours();
		if (OpenGD77Form.FindCustomDataBlock(OpenGD77Form.CustomData, customDataType, ref returnedOffsetPos))
		{
			flag = true;
		}
		else if (OpenGD77Form.FindCustomDataBlock(OpenGD77Form.CustomData, OpenGD77Form.CustomDataType.UNINITIALISED_TYPE, ref returnedOffsetPos, THEME_SIZE))
		{
			flag = true;
		}
		if (flag)
		{
			OpenGD77Form.CustomData[returnedOffsetPos] = (byte)customDataType;
			OpenGD77Form.CustomData[returnedOffsetPos + 1] = 0;
			OpenGD77Form.CustomData[returnedOffsetPos + 2] = 0;
			OpenGD77Form.CustomData[returnedOffsetPos + 3] = 0;
			OpenGD77Form.CustomData[returnedOffsetPos + 4] = (byte)THEME_SIZE;
			OpenGD77Form.CustomData[returnedOffsetPos + 5] = 0;
			OpenGD77Form.CustomData[returnedOffsetPos + 6] = 0;
			OpenGD77Form.CustomData[returnedOffsetPos + 7] = 0;
			Array.Copy(themeData, 0, OpenGD77Form.CustomData, returnedOffsetPos + 8, themeData.Length);
			return true;
		}
		return false;
	}

	private void updateProgess(int progressPercentage)
	{
		if (progressBar1.InvokeRequired)
		{
			progressBar1.Invoke((MethodInvoker)delegate
			{
				progressBar1.Value = progressPercentage;
			});
		}
		else
		{
			progressBar1.Value = progressPercentage;
		}
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
			if (dataObj.data_sector == -1 && !OpenGD77Form.flashWritePrepareSector(port, writeCommandCharacter, num2, ref sendbuffer, ref readbuffer, dataObj))
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
				if (!OpenGD77Form.flashSendData(port, writeCommandCharacter, num2, num4, ref sendbuffer, ref readbuffer))
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
				if (dataObj.data_sector != num2 / 4096 && !OpenGD77Form.flashWriteSector(port, writeCommandCharacter, ref sendbuffer, ref readbuffer, dataObj))
				{
					return false;
				}
			}
		}
		if (dataObj.data_sector != -1 && !OpenGD77Form.flashWriteSector(port, writeCommandCharacter, ref sendbuffer, ref readbuffer, dataObj))
		{
			Console.WriteLine(string.Format(OpenGD77StringsDict["write_stopped"], num2));
			return false;
		}
		return true;
	}

	private void worker_DoWork(object sender, DoWorkEventArgs e)
	{
		OpenGD77CommsTransferData openGD77CommsTransferData = e.Argument as OpenGD77CommsTransferData;
		if (commPort == null)
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(OpenGD77StringsDict["No_com_port"]);
			enableDisableAllButtons(show: true);
			return;
		}
		try
		{
			switch (openGD77CommsTransferData.action)
			{
			case OpenGD77CommsTransferData.CommsAction.READ_THEME:
				if (!OpenGD77Form.sendCommand(commPort, 0))
				{
					displayMessage(OpenGD77StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				MainForm.RadioInfo = OpenGD77Form.readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				OpenGD77Form.sendCommand(commPort, 1);
				OpenGD77Form.sendCommand(commPort, 2, 0, 0, 3, 1, 0, OpenGD77StringsDict["RADIO_DISPLAY_CPS"]);
				OpenGD77Form.sendCommand(commPort, 2, 0, 16, 3, 1, 0, OpenGD77StringsDict["RADIO_DISPLAY_Reading"]);
				OpenGD77Form.sendCommand(commPort, 2, 0, 32, 3, 1, 0, OpenGD77StringsDict["Theme"]);
				OpenGD77Form.sendCommand(commPort, 3);
				OpenGD77Form.sendCommand(commPort, 6, 3);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadFlash;
				openGD77CommsTransferData.dataBuff = new byte[Settings.ADDR_OPENGD77_CUSTOM_DATA_END - Settings.ADDR_OPENGD77_CUSTOM_DATA_START];
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				openGD77CommsTransferData.startDataAddressInTheRadio = OpenGD77Form.STM32_FLASH_ADDRESS_OFFSET;
				openGD77CommsTransferData.transferLength = openGD77CommsTransferData.dataBuff.Length;
				if (!ReadFlashOrEEPROMOrROMOrScreengrab(commPort, openGD77CommsTransferData))
				{
					displayMessage(OpenGD77StringsDict["Error_while_reading"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
				}
				else
				{
					OpenGD77Form.sendCommand(commPort, 5);
					OpenGD77Form.sendCommand(commPort, 7);
					commPort.Close();
					commPort = null;
				}
				break;
			case OpenGD77CommsTransferData.CommsAction.WRITE_THEME:
				if (!OpenGD77Form.sendCommand(commPort, 0))
				{
					displayMessage(OpenGD77StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				OpenGD77Form.readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				OpenGD77Form.sendCommand(commPort, 1);
				OpenGD77Form.sendCommand(commPort, 2, 0, 0, 3, 1, 0, OpenGD77StringsDict["RADIO_DISPLAY_CPS"]);
				OpenGD77Form.sendCommand(commPort, 2, 0, 16, 3, 1, 0, OpenGD77StringsDict["RADIO_DISPLAY_Writing"]);
				OpenGD77Form.sendCommand(commPort, 2, 0, 32, 3, 1, 0, OpenGD77StringsDict["Theme"]);
				OpenGD77Form.sendCommand(commPort, 3);
				OpenGD77Form.sendCommand(commPort, 6, 4);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteFlash;
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				openGD77CommsTransferData.transferLength = openGD77CommsTransferData.dataBuff.Length;
				openGD77CommsTransferData.startDataAddressInTheRadio = OpenGD77Form.STM32_FLASH_ADDRESS_OFFSET;
				if (MainForm.RadioInfo.radioType == 8)
				{
					lock (sender)
					{
						if (!OpenGD77Form.convertThemeColours565(ref openGD77CommsTransferData.dataBuff, OpenGD77Form.CUSTOM_DATA_HEADER_SIZE, checkForCustomDataHeader: false))
						{
							MessageBox.Show(OpenGD77StringsDict["Colour_Conversion_Error"]);
						}
					}
				}
				if (WriteFlash(commPort, openGD77CommsTransferData))
				{
					displayMessage(OpenGD77StringsDict["Upload_complete"]);
					OpenGD77Form.sendCommand(commPort, 6, 3);
				}
				else
				{
					SystemSounds.Hand.Play();
					MessageBox.Show(OpenGD77StringsDict["Error_while_writing"]);
					openGD77CommsTransferData.responseCode = 1;
				}
				OpenGD77Form.sendCommand(commPort, 6, 1);
				commPort.Close();
				commPort = null;
				break;
			}
		}
		catch (Exception ex)
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(ex.Message);
		}
		e.Result = openGD77CommsTransferData;
	}

	private bool ReadFlashOrEEPROMOrROMOrScreengrab(SerialPort port, OpenGD77CommsTransferData dataObj)
	{
		int num = 0;
		byte[] array = new byte[1032];
		byte[] array2 = new byte[1032];
		int num2 = dataObj.startDataAddressInTheRadio;
		int localDataBufferStartPosition = dataObj.localDataBufferStartPosition;
		int num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2;
		while (num3 > 0)
		{
			int num4 = 100;
			if (num3 > OpenGD77Form.getUSBReadBufferSize())
			{
				num3 = OpenGD77Form.getUSBReadBufferSize();
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
			while (port.BytesToWrite > 0)
			{
				Thread.Sleep(1);
			}
			while (port.BytesToRead == 0 && num4-- > 0)
			{
				Thread.Sleep(5);
			}
			if (num4 == -1)
			{
				return false;
			}
			port.Read(array2, 0, port.BytesToRead);
			if (array2[0] == 82)
			{
				int num5 = (array2[1] << 8) + array2[2];
				for (int i = 0; i < num5; i++)
				{
					dataObj.dataBuff[localDataBufferStartPosition++] = array2[i + 3];
				}
				int num6 = (num2 - dataObj.startDataAddressInTheRadio) * 100 / dataObj.transferLength;
				if (num != num6)
				{
					updateProgess(num6);
					num = num6;
				}
				num2 += num5;
				num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2;
				continue;
			}
			Console.WriteLine(string.Format(OpenGD77Form.StringsDict["read_stopped"], num2));
			return false;
		}
		return true;
	}

	private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (!(e.Result is OpenGD77CommsTransferData openGD77CommsTransferData))
		{
			return;
		}
		if (openGD77CommsTransferData.action != 0)
		{
			if (openGD77CommsTransferData.responseCode == 0)
			{
				switch (openGD77CommsTransferData.action)
				{
				case OpenGD77CommsTransferData.CommsAction.READ_THEME:
					SystemSounds.Asterisk.Play();
					enableDisableAllButtons(show: true);
					openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
					if (MainForm.RadioInfo.radioType == 8)
					{
						int returnedOffsetPos = 0;
						if (OpenGD77Form.FindCustomDataBlock(openGD77CommsTransferData.dataBuff, rbDaytimeDay.Checked ? OpenGD77Form.CustomDataType.THEME_DAY_TYPE : OpenGD77Form.CustomDataType.THEME_NIGHT_TYPE, ref returnedOffsetPos))
						{
							lock (sender)
							{
								OpenGD77Form.convertThemeColours565(ref openGD77CommsTransferData.dataBuff, returnedOffsetPos, checkForCustomDataHeader: false);
							}
						}
					}
					readThemeFromCodeplug(ref openGD77CommsTransferData.dataBuff);
					break;
				case OpenGD77CommsTransferData.CommsAction.WRITE_THEME:
					SystemSounds.Asterisk.Play();
					enableDisableAllButtons(show: true);
					openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
					break;
				}
			}
			else
			{
				SystemSounds.Hand.Play();
				MessageBox.Show(OpenGD77StringsDict["There_has_been_an_error._Refer_to_the_last_status_message_that_was_displayed"], OpenGD77StringsDict["Oops"]);
				enableDisableAllButtons(show: true);
			}
		}
		progressBar1.Value = 0;
	}

	private void perFormCommsTask(OpenGD77CommsTransferData dataObj)
	{
		try
		{
			worker = new BackgroundWorker();
			worker.DoWork += worker_DoWork;
			worker.RunWorkerCompleted += worker_RunWorkerCompleted;
			worker.RunWorkerAsync(dataObj);
		}
		catch (Exception ex)
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(ex.Message);
		}
	}

	private void enableDisableAllButtons(bool show)
	{
		btnOpen.Enabled = show;
		btnSave.Enabled = show;
		grpDaytimeTheme.Enabled = show;
		btnRead.Enabled = show;
		btnWrite.Enabled = show;
		btnOK.Enabled = show;
	}

	private void btnOpen_Click(object sender, EventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = ThemeStringsDict["Theme_files"] + " (*.gtm)|*.gtm|(*.*)|*.*";
		openFileDialog.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastThemeLocation", null);
		if (openFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
		{
			IniFileUtils.WriteProfileString("ThemeEditor", "LastThemeLocation", Path.GetDirectoryName(openFileDialog.FileName));
			themeData = File.ReadAllBytes(openFileDialog.FileName);
			if (themeData.Length == THEME_SIZE)
			{
				updateLabelColours();
			}
			else
			{
				MessageBox.Show("File is the wrong size");
			}
		}
	}

	private void btnSave_Click(object sender, EventArgs e)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = ThemeStringsDict["Theme_files"] + " (*.gtm)|*.gtm|(*.*)|*.*";
		saveFileDialog.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastThemeLocation", null);
		if (saveFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(saveFileDialog.FileName))
		{
			IniFileUtils.WriteProfileString("ThemeEditor", "LastThemeLocation", Path.GetDirectoryName(saveFileDialog.FileName));
			readLabelColours();
			File.WriteAllBytes(saveFileDialog.FileName, themeData);
		}
	}

	private void Daytime_CheckedChanged(object sender, EventArgs e)
	{
		RadioButton radioButton = (RadioButton)sender;
		if (radioButton.Checked)
		{
			IniFileUtils.WriteProfileString("ThemeEditor", "LastDaytimeDaySelected", rbDaytimeDay.Checked ? "1" : "0");
			readThemeFromCodeplug(ref OpenGD77Form.CustomData);
		}
		else
		{
			SaveToCodeplug((OpenGD77Form.CustomDataType)radioButton.Tag);
		}
	}

	private void backgroundColour_Click(object sender, EventArgs e)
	{
		Label label = sender as Label;
		colourDlg.Color = label.BackColor;
		if (colourDlg.ShowDialog() == DialogResult.OK)
		{
			label.BackColor = RGB565ToColor(ColorToRGB565(colourDlg.Color));
		}
	}

	private void btnReadTheme_Click(object sender, EventArgs e)
	{
		if (probeRadioModel())
		{
			if (!setupCommPort())
			{
				SystemSounds.Hand.Play();
				MessageBox.Show(OpenGD77StringsDict["No_com_port"]);
				return;
			}
			if (MainForm.RadioInfo.radioType != 6 && MainForm.RadioInfo.radioType != 8 && MainForm.RadioInfo.radioType != 10 && MainForm.RadioInfo.radioType != 9)
			{
				MessageBox.Show(ThemeStringsDict["colour_not_supported"], OpenGD77StringsDict["Error"]);
				return;
			}
			OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.READ_THEME);
			openGD77CommsTransferData.dataBuff = new byte[Settings.ADDR_OPENGD77_CUSTOM_DATA_END - Settings.ADDR_OPENGD77_CUSTOM_DATA_START];
			enableDisableAllButtons(show: false);
			perFormCommsTask(openGD77CommsTransferData);
		}
	}

	private void btnWriteTheme_Click(object sender, EventArgs e)
	{
		if (probeRadioModel())
		{
			if (!setupCommPort())
			{
				SystemSounds.Hand.Play();
				MessageBox.Show(OpenGD77StringsDict["No_com_port"]);
			}
			else if (MainForm.RadioInfo.radioType != 6 && MainForm.RadioInfo.radioType != 8 && MainForm.RadioInfo.radioType != 10 && MainForm.RadioInfo.radioType != 9)
			{
				MessageBox.Show(ThemeStringsDict["colour_not_supported"], OpenGD77StringsDict["Error"]);
			}
			else if (SaveToCodeplug())
			{
				OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.WRITE_THEME);
				openGD77CommsTransferData.dataBuff = new byte[Settings.ADDR_OPENGD77_CUSTOM_DATA_END - Settings.ADDR_OPENGD77_CUSTOM_DATA_START];
				Array.Copy(OpenGD77Form.CustomData, openGD77CommsTransferData.dataBuff, openGD77CommsTransferData.dataBuff.Length);
				enableDisableAllButtons(show: false);
				perFormCommsTask(openGD77CommsTransferData);
			}
			else
			{
				MessageBox.Show(ThemeStringsDict["Theme_write_failed"], OpenGD77StringsDict["Error"], MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
	}

	private Color RGB565ToColor(ushort rgb565)
	{
		float num = (float)(rgb565 >> 11) / 31f * 255f;
		float num2 = (float)((rgb565 >> 5) & 0x3F) / 63f * 255f;
		float num3 = (float)(rgb565 & 0x1F) / 31f * 255f;
		return Color.FromArgb(255, (int)Math.Round(num), (int)Math.Round(num2), (int)Math.Round(num3));
	}

	private ushort ColorToRGB565(Color c)
	{
		int num = (int)Math.Round((float)(int)c.R / 255f * 31f);
		int num2 = (int)Math.Round((float)(int)c.G / 255f * 63f);
		int num3 = (int)Math.Round((float)(int)c.B / 255f * 31f);
		return (ushort)((num << 11) | (num2 << 5) | num3);
	}

	private void btnOK_Click(object sender, EventArgs e)
	{
		SaveToCodeplug();
		Close();
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
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnRead = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.gbColours = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.rbDaytimeDay = new System.Windows.Forms.RadioButton();
            this.rbDaytimeNight = new System.Windows.Forms.RadioButton();
            this.grpDaytimeTheme = new System.Windows.Forms.GroupBox();
            this.grpDaytimeTheme.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOpen
            // 
            this.btnOpen.BackColor = System.Drawing.SystemColors.Control;
            this.btnOpen.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpen.Location = new System.Drawing.Point(12, 12);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(180, 23);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "Open theme file";
            this.btnOpen.UseVisualStyleBackColor = false;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.Control;
            this.btnSave.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(198, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(201, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save theme file";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnWrite
            // 
            this.btnWrite.BackColor = System.Drawing.SystemColors.Control;
            this.btnWrite.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWrite.Location = new System.Drawing.Point(198, 46);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(201, 23);
            this.btnWrite.TabIndex = 7;
            this.btnWrite.Text = "Write to radio";
            this.btnWrite.UseVisualStyleBackColor = false;
            this.btnWrite.Click += new System.EventHandler(this.btnWriteTheme_Click);
            // 
            // btnRead
            // 
            this.btnRead.BackColor = System.Drawing.SystemColors.Control;
            this.btnRead.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRead.Location = new System.Drawing.Point(12, 46);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(180, 23);
            this.btnRead.TabIndex = 6;
            this.btnRead.Text = "Read from radio";
            this.btnRead.UseVisualStyleBackColor = false;
            this.btnRead.Click += new System.EventHandler(this.btnReadTheme_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessage.Location = new System.Drawing.Point(12, 529);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(754, 18);
            this.txtMessage.TabIndex = 8;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(15, 75);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(751, 12);
            this.progressBar1.TabIndex = 9;
            // 
            // gbColours
            // 
            this.gbColours.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbColours.Location = new System.Drawing.Point(15, 95);
            this.gbColours.Name = "gbColours";
            this.gbColours.Size = new System.Drawing.Size(751, 420);
            this.gbColours.TabIndex = 10;
            this.gbColours.TabStop = false;
            this.gbColours.Text = "Colours";
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.SystemColors.Control;
            this.btnOK.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(694, 525);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // rbDaytimeDay
            // 
            this.rbDaytimeDay.AccessibleDescription = "DayTheme";
            this.rbDaytimeDay.AccessibleName = "Day Theme";
            this.rbDaytimeDay.AutoSize = true;
            this.rbDaytimeDay.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbDaytimeDay.Location = new System.Drawing.Point(10, 19);
            this.rbDaytimeDay.Name = "rbDaytimeDay";
            this.rbDaytimeDay.Size = new System.Drawing.Size(88, 19);
            this.rbDaytimeDay.TabIndex = 4;
            this.rbDaytimeDay.TabStop = true;
            this.rbDaytimeDay.Tag = DMR.OpenGD77Form.CustomDataType.THEME_DAY_TYPE;
            this.rbDaytimeDay.Text = "Day Theme";
            this.rbDaytimeDay.UseVisualStyleBackColor = true;
            this.rbDaytimeDay.CheckedChanged += new System.EventHandler(this.Daytime_CheckedChanged);
            // 
            // rbDaytimeNight
            // 
            this.rbDaytimeNight.AccessibleDescription = "DayTheme";
            this.rbDaytimeNight.AccessibleName = "Night Theme";
            this.rbDaytimeNight.AutoSize = true;
            this.rbDaytimeNight.Location = new System.Drawing.Point(120, 19);
            this.rbDaytimeNight.Name = "rbDaytimeNight";
            this.rbDaytimeNight.Size = new System.Drawing.Size(96, 19);
            this.rbDaytimeNight.TabIndex = 5;
            this.rbDaytimeNight.TabStop = true;
            this.rbDaytimeNight.Tag = DMR.OpenGD77Form.CustomDataType.THEME_NIGHT_TYPE;
            this.rbDaytimeNight.Text = "Night Theme";
            this.rbDaytimeNight.UseVisualStyleBackColor = true;
            this.rbDaytimeNight.CheckedChanged += new System.EventHandler(this.Daytime_CheckedChanged);
            // 
            // grpDaytimeTheme
            // 
            this.grpDaytimeTheme.AccessibleDescription = "Daytime";
            this.grpDaytimeTheme.AccessibleName = "Daytime";
            this.grpDaytimeTheme.Controls.Add(this.rbDaytimeDay);
            this.grpDaytimeTheme.Controls.Add(this.rbDaytimeNight);
            this.grpDaytimeTheme.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpDaytimeTheme.Location = new System.Drawing.Point(540, 12);
            this.grpDaytimeTheme.Name = "grpDaytimeTheme";
            this.grpDaytimeTheme.Size = new System.Drawing.Size(226, 45);
            this.grpDaytimeTheme.TabIndex = 3;
            this.grpDaytimeTheme.TabStop = false;
            this.grpDaytimeTheme.Text = "Daytime";
            // 
            // ThemeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(778, 560);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gbColours);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.btnRead);
            this.Controls.Add(this.btnWrite);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.grpDaytimeTheme);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ThemeForm";
            this.Text = "Theme editor";
            this.Load += new System.EventHandler(this.ThemeForm_Load);
            this.grpDaytimeTheme.ResumeLayout(false);
            this.grpDaytimeTheme.PerformLayout();
            this.ResumeLayout(false);

	}

    private void ThemeForm_Load(object sender, EventArgs e)
    {
        Settings.UpdateComponentTextsFromLanguageXmlData(this);
        Settings.ReadCommonsForSectionIntoDictionary(OpenGD77Form.StringsDict, "OpenGD77Form");
        int num = System.Convert.ToInt32(IniFileUtils.getProfileStringWithDefault("ThemeEditor", "LastDaytimeDaySelected", "1"));
        rbDaytimeDay.Checked = num == 1;
        rbDaytimeNight.Checked = num != 1;
    }
}
