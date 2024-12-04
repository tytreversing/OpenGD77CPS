using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Media;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMR;

public class DMRIDForm : Form
{
	public enum CommsDataMode
	{
		DataModeNone,
		DataModeReadFlash,
		DataModeReadEEPROM,
		DataModeWriteFlash,
		DataModeWriteEEPROM
	}

	public enum CommsAction
	{
		NONE,
		BACKUP_EEPROM,
		BACKUP_FLASH,
		RESTORE_EEPROM,
		RESTORE_FLASH,
		READ_CODEPLUG,
		WRITE_CODEPLUG
	}

	private static List<DMRDataItem> DataList = null;

	public static Dictionary<string, string> StringsDict = new Dictionary<string, string>();

	private static byte[] SIG_PATTERN_BYTES;

	private WebClient _wc;

	private bool _isDownloading;

	private int _stringLength = 8;

	private const int HEADER_LENGTH = 12;

	private int ID_NUMBER_SIZE = 3;

	private SerialPort commPort;

	private string _radioIdCSV;

	private char writeCommandCharacter = 'W';

	private CheckBox[] _filterCheckBoxes;

	private string _separator;

	private string[] SEPARATORS_ARRAY = new string[2] { " ", "." };

	private IContainer components;

	private Button btnWriteToGD77;

	private TextBox txtRegionId;

	private Button btnClear;

	private Label lblMessage;

	private DataGridView dataGridView1;

	private TextBox txtAgeMaxDays;

	private Label lblRegionId;

	private Label lblInactivityFilter;

	private ComboBox cmbStringLen;

	private Label lblEnhancedLength;

	private ProgressBar progressBar1;

	private Button btnDownload;

	private GroupBox groupBox1;

	private Button button1;

	private ComboBox cmbRadioType;

	private Label lblRadioType;

	private CheckBox chkUseVPMemory;

	private ComboBox cmbDownloadLocation;

	private TextBox txtDownloadURL;

	private GroupBox uploadParameters;

	private GroupBox grpColumnFilter;

	private CheckBox chkCol_5;

	private CheckBox chkCol_4;

	private CheckBox chkCol_3;

	private CheckBox chkCol_2;

	private CheckBox chkCol_1;

	private Label lblSeparator;

	private ComboBox cmbSeparator;

	public DMRIDForm()
	{
		SIG_PATTERN_BYTES = new byte[8] { 73, 100, 45, 86, 48, 48, 49, 0 };
		if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
		{
			writeCommandCharacter = 'X';
		}
		InitializeComponent();
		_filterCheckBoxes = new CheckBox[5];
		_filterCheckBoxes[0] = chkCol_1;
		_filterCheckBoxes[1] = chkCol_2;
		_filterCheckBoxes[2] = chkCol_3;
		_filterCheckBoxes[3] = chkCol_4;
		_filterCheckBoxes[4] = chkCol_5;
		txtDownloadURL.Text = IniFileUtils.getProfileStringWithDefault("Setup", "DMRID-DownloadURL-20240921", "https://raw.githubusercontent.com/ContactLists/NORMAL-ContactLists/master/RADIODDITY/GD77/GD77_WW_default.csv");
		cmbDownloadLocation.SelectedIndex = int.Parse(IniFileUtils.getProfileStringWithDefault("Setup", "DMRID-DownloadTypeSelection", "0"));
		try
		{
			cmbSeparator.SelectedIndex = int.Parse(IniFileUtils.getProfileStringWithDefault("Setup", "DMRID-DownloadSeparator", "0"));
			if (cmbSeparator.SelectedIndex < 0 || cmbSeparator.SelectedIndex > cmbSeparator.Items.Count - 1)
			{
				cmbSeparator.SelectedIndex = 0;
			}
		}
		catch (Exception)
		{
			cmbSeparator.SelectedIndex = 0;
		}
		_separator = SEPARATORS_ARRAY[cmbSeparator.SelectedIndex];
		byte b = (byte)int.Parse(IniFileUtils.getProfileStringWithDefault("Setup", "DMRID-DownloadColumnsFilter", "31"));
		for (int i = 0; i < 5; i++)
		{
			_filterCheckBoxes[i].Checked = (b & (1 << i)) != 0;
		}
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		cmbStringLen.Visible = false;
		lblEnhancedLength.Visible = false;
		string string_;
		try
		{
			string_ = GeneralSetForm.data.RadioId.Substring(0, 3);
		}
		catch (Exception)
		{
			string_ = "";
		}
		txtRegionId.Text = IniFileUtils.getProfileStringWithDefault("Setup", "DMRID-RegionFilter", string_);
		DataList = new List<DMRDataItem>();
		dataGridView1.AutoGenerateColumns = false;
		DataGridViewCell cellTemplate = new DataGridViewTextBoxCell();
		DataGridViewTextBoxColumn dataGridViewColumn = new DataGridViewTextBoxColumn
		{
			CellTemplate = cellTemplate,
			Name = "Id",
			HeaderText = "ID",
			DataPropertyName = "DMRID",
			Width = 75
		};
		dataGridView1.Columns.Add(dataGridViewColumn);
		cellTemplate = new DataGridViewTextBoxCell();
		dataGridViewColumn = new DataGridViewTextBoxColumn
		{
			CellTemplate = cellTemplate,
			Name = "Call",
			HeaderText = "Callsign",
			DataPropertyName = "Callsign",
			Width = 75
		};
		dataGridView1.Columns.Add(dataGridViewColumn);
		cellTemplate = new DataGridViewTextBoxCell();
		dataGridViewColumn = new DataGridViewTextBoxColumn
		{
			CellTemplate = cellTemplate,
			Name = "Details",
			HeaderText = "Details",
			DataPropertyName = "Details",
			Width = 460
		};
		dataGridView1.Columns.Add(dataGridViewColumn);
		dataGridView1.UserDeletedRow += dataGridRowDeleted;
		rebindData();
		int num = 10;
		try
		{
			num = IniFileUtils.getProfileIntWithDefault("Setup", "DmrIdCmbStringLenIndex", 10);
		}
		catch (Exception)
		{
		}
		if (num < 0 || num > cmbStringLen.Items.Count - 1)
		{
			cmbStringLen.SelectedIndex = 10;
		}
		else
		{
			cmbStringLen.SelectedIndex = num;
		}
		cmbStringLen.Visible = true;
		lblEnhancedLength.Visible = true;
		if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
		{
			cmbRadioType.SelectedIndex = 3;
			cmbRadioType.Visible = false;
			lblRadioType.Visible = false;
			chkUseVPMemory.Visible = false;
		}
		else
		{
			cmbRadioType.SelectedIndex = 0;
		}
		updateTotalNumberMessage();
		base.FormBorderStyle = FormBorderStyle.FixedSingle;
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
		OpenGD77Form.sendCommand(commPort, 254);
		commPort.DiscardInBuffer();
		if (!OpenGD77Form.sendCommand(commPort, 0))
		{
			MessageBox.Show(StringsDict["Error_connecting_to_the_radio"]);
			commPort = null;
			return false;
		}
		MainForm.RadioInfo = OpenGD77Form.readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
		if (MainForm.RadioInfo.radioType == 5 || MainForm.RadioInfo.radioType == 6 || MainForm.RadioInfo.radioType == 8 || MainForm.RadioInfo.radioType == 10 || MainForm.RadioInfo.radioType == 9 || MainForm.RadioInfo.radioType == 7)
		{
			writeCommandCharacter = 'X';
			cmbRadioType.SelectedIndex = 3;
			cmbRadioType.Visible = false;
			lblRadioType.Visible = false;
			chkUseVPMemory.Visible = false;
			chkUseVPMemory.Checked = false;
			updateTotalNumberMessage();
			((MainForm)getMainForm())?.changeRadioType(MainForm.RadioTypeEnum.RadioTypeSTM32);
		}
		else
		{
			writeCommandCharacter = 'W';
			cmbRadioType.Visible = true;
			lblRadioType.Visible = true;
			chkUseVPMemory.Visible = true;
			updateTotalNumberMessage();
			((MainForm)getMainForm())?.changeRadioType(MainForm.RadioTypeEnum.RadioTypeMK22);
		}
		OpenGD77Form.sendCommand(commPort, 5);
		commPort.Close();
		commPort = null;
		return true;
	}

	private void enableUI(bool state)
	{
		btnDownload.Enabled = state;
		button1.Enabled = state;
		cmbStringLen.Enabled = state;
		txtRegionId.Enabled = state;
		chkUseVPMemory.Enabled = state;
		cmbRadioType.Enabled = state;
		dataGridView1.Enabled = state;
		btnClear.Enabled = state;
		btnWriteToGD77.Enabled = state;
		cmbDownloadLocation.Enabled = state;
		if (state)
		{
			bool enabled = false;
			switch (cmbDownloadLocation.SelectedIndex)
			{
			case 0:
				enabled = true;
				cmbSeparator.Enabled = true;
				break;
			case 1:
				txtAgeMaxDays.Enabled = true;
				break;
			case 2:
				enabled = true;
				txtDownloadURL.Enabled = true;
				cmbSeparator.Enabled = true;
				break;
			}
			for (int i = 0; i < 5; i++)
			{
				_filterCheckBoxes[i].Enabled = enabled;
			}
		}
		else
		{
			txtDownloadURL.Enabled = false;
			txtAgeMaxDays.Enabled = false;
			cmbSeparator.Enabled = false;
			for (int j = 0; j < 5; j++)
			{
				_filterCheckBoxes[j].Enabled = state;
			}
		}
	}

	private void dataGridRowDeleted(object sender, DataGridViewRowEventArgs e)
	{
		updateTotalNumberMessage();
	}

	private void rebindData()
	{
		BindingSource dataSource = new BindingSource(new BindingList<DMRDataItem>(DataList), null);
		dataGridView1.DataSource = dataSource;
		updateTotalNumberMessage();
	}

	private void downloadFromHamDigital()
	{
		if (DataList == null || _isDownloading)
		{
			return;
		}
		_wc = new WebClient();
		try
		{
			lblMessage.Text = Settings.dicCommon["DownloadContactsDownloading"];
			Cursor.Current = Cursors.WaitCursor;
			Refresh();
			Application.DoEvents();
			string text = "";
			try
			{
				text = txtRegionId.Text.Split(',')[0];
			}
			catch (Exception)
			{
			}
			_wc.DownloadStringCompleted += downloadFromHamDigitalCompleteHandler;
			_wc.DownloadStringAsync(new Uri("http://ham-digital.org/user_by_lh.php?id=" + text));
		}
		catch (Exception)
		{
			Cursor.Current = Cursors.Default;
			MessageBox.Show(Settings.dicCommon["UnableDownloadFromInternet"]);
			return;
		}
		_isDownloading = true;
	}

	private void downloadFromHamDigitalCompleteHandler(object sender, DownloadStringCompletedEventArgs e)
	{
		_ = GeneralSetForm.data.RadioId;
		int num = int.MaxValue;
		string result;
		try
		{
			result = e.Result;
		}
		catch (Exception)
		{
			MessageBox.Show(Settings.dicCommon["UnableDownloadFromInternet"]);
			return;
		}
		try
		{
			num = int.Parse(txtAgeMaxDays.Text);
		}
		catch (Exception)
		{
		}
		try
		{
			bool flag = true;
			string[] array = result.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string cSVLine in array)
			{
				if (flag)
				{
					flag = false;
					continue;
				}
				DMRDataItem dMRDataItem = new DMRDataItem().FromHamDigital(cSVLine);
				if (dMRDataItem.AgeAsInt <= num)
				{
					DataList.Add(dMRDataItem);
				}
			}
			DataList = DataList.Distinct().ToList();
			rebindData();
			Cursor.Current = Cursors.Default;
		}
		catch (Exception)
		{
			MessageBox.Show(Settings.dicCommon["ErrorParsingData"]);
		}
		finally
		{
			_wc = null;
			_isDownloading = false;
			Cursor.Current = Cursors.Default;
		}
	}

	private void downloadFromURLCompleteHandler(object sender, DownloadStringCompletedEventArgs e)
	{
		bool flag = false;
		progressBar1.Value = 0;
		enableUI(state: true);
		try
		{
			_radioIdCSV = e.Result;
		}
		catch (Exception)
		{
			MessageBox.Show(Settings.dicCommon["UnableDownloadFromInternet"]);
			flag = true;
		}
		if (!flag)
		{
			importFromRadioIdCSV();
		}
		_wc = null;
		_isDownloading = false;
		Cursor.Current = Cursors.Default;
	}

	private void processCsvLinesList(List<string> csvLinesList)
	{
		bool flag = true;
		List<string> list = txtRegionId.Text.Split(',').ToList();
		foreach (string csvLines in csvLinesList)
		{
			if (flag)
			{
				string[] colNames = csvLines.Split(',');
				int i;
				for (i = 0; i < Math.Min(5, colNames.Length - 2); i++)
				{
					Invoke((Action)delegate
					{
						_filterCheckBoxes[i].Text = colNames[i + 2];
					});
				}
				flag = false;
			}
			else
			{
				if (csvLines.Length <= 0)
				{
					continue;
				}
				string text = csvLines.Split(',')[0];
				bool flag2 = false;
				foreach (string item in list)
				{
					if (text.IndexOf(item) == 0)
					{
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					DMRDataItem dMRDataItem = new DMRDataItem().FromURLOrCSV(csvLines, _filterCheckBoxes, _separator);
					if (dMRDataItem != null)
					{
						DataList.Add(dMRDataItem);
					}
				}
			}
		}
		DataList = DataList.Distinct().ToList();
	}

	private Task<bool> processCSVDownloadedDataImport()
	{
		try
		{
			List<string> csvLinesList = _radioIdCSV.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
			processCsvLinesList(csvLinesList);
		}
		catch (Exception)
		{
			MessageBox.Show(Settings.dicCommon["ErrorParsingData"]);
			return Task.FromResult(result: false);
		}
		return Task.FromResult(result: true);
	}

	private async void importFromRadioIdCSV()
	{
		if (_radioIdCSV != null)
		{
			lblMessage.Text = Settings.dicCommon["Processing"];
			enableUI(state: false);
			progressBar1.Style = ProgressBarStyle.Marquee;
			progressBar1.Value = 100;
			progressBar1.MarqueeAnimationSpeed = 100;
			Task task = Task.Factory.StartNew(() => processCSVDownloadedDataImport());
			await task;
			task.Dispose();
			rebindData();
			Cursor.Current = Cursors.Default;
			progressBar1.Style = ProgressBarStyle.Continuous;
			progressBar1.Value = 0;
			enableUI(state: true);
		}
	}

	private void updateTotalNumberMessage()
	{
		string format = Settings.dicCommon["DMRIdContcatsTotal"];
		lblMessage.Text = string.Format(format, DataList.Count, getMaxRecords(getSelectedRadioMemorySize()));
	}

	private void downloadProgressHandler(object sender, DownloadProgressChangedEventArgs e)
	{
		try
		{
			BeginInvoke((Action)delegate
			{
				lblMessage.Text = Settings.dicCommon["DownloadContactsDownloading"] + e.ProgressPercentage + "%";
			});
		}
		catch (Exception)
		{
		}
	}

	private int getSelectedRadioMemorySize()
	{
		int[] obj = new int[6] { 557056, 1605632, 557056, 0, 7897088, 14188544 };
		obj[3] = 14188544 - OpenGD77Form.FLASH_MEMORY_EEPROM_EMU_SIZE;
		return obj[Math.Max(0, cmbRadioType.SelectedIndex)] + (chkUseVPMemory.Checked ? 166912 : 0);
	}

	private int getMaxRecords(int memorysize)
	{
		int num = DMRDataItem.compressSize(_stringLength) + ID_NUMBER_SIZE;
		int num2 = 262132 / num;
		int num3 = (memorysize - 262144) / num;
		return num2 + num3;
	}

	private byte[] GenerateUploadData(int numRecords)
	{
		int num = DMRDataItem.compressSize(_stringLength) + ID_NUMBER_SIZE;
		byte[] array = new byte[numRecords * num + 12];
		Array.Copy(SIG_PATTERN_BYTES, array, SIG_PATTERN_BYTES.Length);
		Array.Copy(BitConverter.GetBytes(numRecords), 0, array, 8, 4);
		if (DataList == null)
		{
			return array;
		}
		List<DMRDataItem> list = new List<DMRDataItem>(DataList);
		list.Sort();
		for (int i = 0; i < numRecords; i++)
		{
			Array.Copy(list[i].getRadioData(_stringLength), 0, array, 12 + i * num, num);
		}
		return array;
	}

	private void btnClear_Click(object sender, EventArgs e)
	{
		DataList = new List<DMRDataItem>();
		rebindData();
	}

	private void btnWriteToGD77_Click(object sender, EventArgs e)
	{
		if (probeRadioModel())
		{
			if (!setupCommPort())
			{
				SystemSounds.Hand.Play();
				MessageBox.Show(StringsDict["No_com_port"]);
			}
			else
			{
				writeToOpenGD77();
			}
		}
	}

	private void DMRIDFormNew_FormClosing(object sender, FormClosingEventArgs e)
	{
		IniFileUtils.WriteProfileString("Setup", "DMRID-DownloadSeparator", cmbSeparator.SelectedIndex.ToString());
		int num = 0;
		for (int i = 0; i < 5; i++)
		{
			num += (int)((_filterCheckBoxes[i].Checked ? 1u : 0u) << i);
		}
		IniFileUtils.WriteProfileString("Setup", "DMRID-DownloadColumnsFilter", num.ToString());
		IniFileUtils.WriteProfileString("Setup", "DMRID-RegionFilter", txtRegionId.Text);
		if (commPort == null)
		{
			return;
		}
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

	private void DMRIDForm_Load(object sender, EventArgs e)
	{
		Settings.smethod_59(base.Controls);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		Settings.ReadCommonsForSectionIntoDictionary(StringsDict, "OpenGD77Form");
	}

	private void cmbStringLen_SelectedIndexChanged(object sender, EventArgs e)
	{
		_stringLength = cmbStringLen.SelectedIndex + 6;
		updateTotalNumberMessage();
		IniFileUtils.WriteProfileInt("Setup", "DmrIdCmbStringLenIndex", cmbStringLen.SelectedIndex);
	}

	private void WriteFlash(SerialPort port, OpenGD77CommsTransferData dataObj)
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
				if (!OpenGD77Form.flashSendData(port, writeCommandCharacter, num2, num4, ref sendbuffer, ref readbuffer))
				{
					break;
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
					break;
				}
			}
		}
		if (dataObj.data_sector != -1 && !OpenGD77Form.flashWriteSector(port, writeCommandCharacter, ref sendbuffer, ref readbuffer, dataObj))
		{
			Console.WriteLine($"Error. Write stopped (write sector error at {num2:X8})");
		}
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

	private int getRadioInfoMemorySize(OpenGD77Form.RadioInfo radioInfo)
	{
		int num;
		switch (radioInfo.flashId)
		{
		case 16405u:
			num = 1605632;
			break;
		case 16407u:
			num = 7897088;
			break;
		case 16408u:
		case 28696u:
			num = 14188544;
			if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
			{
				num -= OpenGD77Form.FLASH_MEMORY_EEPROM_EMU_SIZE;
			}
			break;
		default:
			num = 557056;
			break;
		}
		return num;
	}

	private int getRecordLength()
	{
		return DMRDataItem.compressSize(_stringLength) + ID_NUMBER_SIZE;
	}

	private void writeToOpenGD77()
	{
		int num = 196608;
		enableUI(state: false);
		MainForm.RadioInfo = OpenGD77Form.readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
		if (MainForm.RadioInfo.buildDateTime != null)
		{
			int radioInfoMemorySize = getRadioInfoMemorySize(MainForm.RadioInfo);
			radioInfoMemorySize += (chkUseVPMemory.Checked ? 166912 : 0);
			int maxRecords = getMaxRecords(radioInfoMemorySize);
			if (DataList.Count > maxRecords && MessageBox.Show(Settings.dicCommon["DMRIdTooManyIDs"], "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) != DialogResult.OK)
			{
				enableUI(state: true);
				return;
			}
			OpenGD77Form.sendCommand(commPort, 0);
			OpenGD77Form.sendCommand(commPort, 1);
			OpenGD77Form.sendCommand(commPort, 2, 0, 0, 3, 1, 0, "CPS");
			OpenGD77Form.sendCommand(commPort, 2, 0, 16, 3, 1, 0, "Writing");
			OpenGD77Form.sendCommand(commPort, 2, 0, 32, 3, 1, 0, "DMRID");
			OpenGD77Form.sendCommand(commPort, 2, 0, 48, 3, 1, 0, "Database");
			OpenGD77Form.sendCommand(commPort, 3);
			OpenGD77Form.sendCommand(commPort, 6, 4);
			OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData();
			openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteFlash;
			if (ID_NUMBER_SIZE == 3)
			{
				if (chkUseVPMemory.Checked)
				{
					SIG_PATTERN_BYTES[2] = 110;
				}
				else
				{
					SIG_PATTERN_BYTES[2] = 78;
				}
			}
			int recordLength = getRecordLength();
			SIG_PATTERN_BYTES[3] = (byte)(74 + recordLength);
			openGD77CommsTransferData.dataBuff = GenerateUploadData(Math.Min(DataList.Count, maxRecords));
			int num2 = (openGD77CommsTransferData.localDataBufferStartPosition = 0);
			if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
			{
				openGD77CommsTransferData.startDataAddressInTheRadio = num + OpenGD77Form.STM32_FLASH_ADDRESS_OFFSET;
			}
			else
			{
				openGD77CommsTransferData.startDataAddressInTheRadio = num;
			}
			int num3 = openGD77CommsTransferData.dataBuff.Length;
			int num4 = 12 + recordLength * (262132 / recordLength);
			if (num3 > num4)
			{
				openGD77CommsTransferData.transferLength = num4;
			}
			else
			{
				openGD77CommsTransferData.transferLength = num3;
			}
			WriteFlash(commPort, openGD77CommsTransferData);
			num3 -= openGD77CommsTransferData.transferLength;
			num2 += openGD77CommsTransferData.transferLength;
			if (num3 > 0)
			{
				openGD77CommsTransferData.startDataAddressInTheRadio = (chkUseVPMemory.Checked ? 586752 : 753664);
				if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
				{
					openGD77CommsTransferData.startDataAddressInTheRadio += OpenGD77Form.STM32_FLASH_ADDRESS_OFFSET;
				}
				openGD77CommsTransferData.localDataBufferStartPosition = num2;
				openGD77CommsTransferData.transferLength = num3;
				WriteFlash(commPort, openGD77CommsTransferData);
			}
			progressBar1.Value = 0;
		}
		OpenGD77Form.sendCommand(commPort, 6, 2);
		OpenGD77Form.sendCommand(commPort, 6, 1);
		commPort.Close();
		commPort = null;
		if (MainForm.RadioInfo.buildDateTime == null)
		{
			MessageBox.Show("Incompatible firmware. Please update the firmware in your radio", "Error");
		}
		enableUI(state: true);
	}

	private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
	{
		if (e.Column.Index != 3)
		{
			return;
		}
		try
		{
			if (int.Parse(e.CellValue1.ToString()) < int.Parse(e.CellValue2.ToString()))
			{
				e.SortResult = -1;
			}
			else
			{
				e.SortResult = 1;
			}
			e.Handled = true;
		}
		catch (Exception)
		{
		}
	}

	private Task<bool> processCSVFileImport(string filename)
	{
		try
		{
			_ = txtRegionId.Text;
			txtRegionId.Text.Split(',').ToList();
			List<string> list = new List<string>();
			using StreamReader streamReader = new StreamReader(filename);
			while (!streamReader.EndOfStream)
			{
				string item = streamReader.ReadLine().Trim();
				list.Add(item);
			}
			processCsvLinesList(list);
		}
		catch (Exception)
		{
			MessageBox.Show("The CSV file could not be opened");
			return Task.FromResult(result: false);
		}
		return Task.FromResult(result: true);
	}

	private async void btnImportCSV_Click(object sender, EventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "firmware files|*.csv";
		if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileName != null)
		{
			lblMessage.Text = "Processing...";
			enableUI(state: false);
			progressBar1.Style = ProgressBarStyle.Marquee;
			progressBar1.Value = 100;
			progressBar1.MarqueeAnimationSpeed = 100;
			Task task = Task.Factory.StartNew(() => processCSVFileImport(openFileDialog.FileName));
			await task;
			task.Dispose();
			rebindData();
			Cursor.Current = Cursors.Default;
			progressBar1.Style = ProgressBarStyle.Continuous;
			progressBar1.Value = 0;
			enableUI(state: true);
		}
	}

	private void downloadProgressChangedCallback(object sender, DownloadProgressChangedEventArgs ev)
	{
		progressBar1.Value = ev.ProgressPercentage;
	}

	private void downloadFromRadioURL(string URL)
	{
		if (DataList == null || _isDownloading)
		{
			return;
		}
		progressBar1.Value = 0;
		if (_radioIdCSV != null)
		{
			importFromRadioIdCSV();
			return;
		}
		enableUI(state: false);
		_wc = new WebClient();
		try
		{
			lblMessage.Text = Settings.dicCommon["DownloadContactsDownloading"];
			Cursor.Current = Cursors.WaitCursor;
			Refresh();
			Application.DoEvents();
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			_wc.DownloadStringCompleted += downloadFromURLCompleteHandler;
			_wc.DownloadProgressChanged += downloadProgressChangedCallback;
			_wc.DownloadStringAsync(new Uri(URL));
		}
		catch (Exception)
		{
			Cursor.Current = Cursors.Default;
			MessageBox.Show(Settings.dicCommon["UnableDownloadFromInternet"]);
			enableUI(state: true);
			return;
		}
		_isDownloading = true;
	}

	private void cmbRadioType_SelectedIndexChanged(object sender, EventArgs e)
	{
		updateTotalNumberMessage();
	}

	private void chkUseVPMemory_CheckedChanged(object sender, EventArgs e)
	{
		updateTotalNumberMessage();
	}

	private void cmbDownloadLocation_SelectedIndexChanged(object sender, EventArgs e)
	{
		bool enabled = true;
		switch (cmbDownloadLocation.SelectedIndex)
		{
		case 0:
			txtAgeMaxDays.Enabled = false;
			txtDownloadURL.Enabled = false;
			lblInactivityFilter.Enabled = false;
			enabled = true;
			cmbSeparator.Enabled = true;
			break;
		case 1:
			txtAgeMaxDays.Enabled = true;
			txtDownloadURL.Enabled = false;
			lblInactivityFilter.Enabled = true;
			enabled = false;
			cmbSeparator.Enabled = false;
			break;
		case 2:
			txtAgeMaxDays.Enabled = false;
			txtDownloadURL.Enabled = true;
			lblInactivityFilter.Enabled = false;
			enabled = true;
			cmbSeparator.Enabled = true;
			break;
		}
		for (int i = 0; i < 5; i++)
		{
			_filterCheckBoxes[i].Enabled = enabled;
		}
		IniFileUtils.WriteProfileString("Setup", "DMRID-DownloadTypeSelection", cmbDownloadLocation.SelectedIndex.ToString());
		_radioIdCSV = null;
	}

	private void btnDownloadFrom_Click(object sender, EventArgs e)
	{
		switch (cmbDownloadLocation.SelectedIndex)
		{
		case 0:
			downloadFromRadioURL("https://database.radioid.net/static/user.csv");
			break;
		case 1:
			downloadFromHamDigital();
			break;
		case 2:
			downloadFromRadioURL(txtDownloadURL.Text);
			IniFileUtils.WriteProfileString("Setup", "DMRID-DownloadURL-20240921", txtDownloadURL.Text);
			break;
		}
	}

	private void cmbSeparator_SelectedIndexChanged(object sender, EventArgs e)
	{
		_separator = SEPARATORS_ARRAY[cmbSeparator.SelectedIndex];
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DMR.DMRIDForm));
		this.btnWriteToGD77 = new System.Windows.Forms.Button();
		this.txtRegionId = new System.Windows.Forms.TextBox();
		this.btnClear = new System.Windows.Forms.Button();
		this.lblMessage = new System.Windows.Forms.Label();
		this.dataGridView1 = new System.Windows.Forms.DataGridView();
		this.txtAgeMaxDays = new System.Windows.Forms.TextBox();
		this.lblRegionId = new System.Windows.Forms.Label();
		this.lblInactivityFilter = new System.Windows.Forms.Label();
		this.cmbStringLen = new System.Windows.Forms.ComboBox();
		this.lblEnhancedLength = new System.Windows.Forms.Label();
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		this.btnDownload = new System.Windows.Forms.Button();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.button1 = new System.Windows.Forms.Button();
		this.cmbRadioType = new System.Windows.Forms.ComboBox();
		this.lblRadioType = new System.Windows.Forms.Label();
		this.chkUseVPMemory = new System.Windows.Forms.CheckBox();
		this.cmbDownloadLocation = new System.Windows.Forms.ComboBox();
		this.txtDownloadURL = new System.Windows.Forms.TextBox();
		this.uploadParameters = new System.Windows.Forms.GroupBox();
		this.grpColumnFilter = new System.Windows.Forms.GroupBox();
		this.cmbSeparator = new System.Windows.Forms.ComboBox();
		this.lblSeparator = new System.Windows.Forms.Label();
		this.chkCol_5 = new System.Windows.Forms.CheckBox();
		this.chkCol_4 = new System.Windows.Forms.CheckBox();
		this.chkCol_3 = new System.Windows.Forms.CheckBox();
		this.chkCol_2 = new System.Windows.Forms.CheckBox();
		this.chkCol_1 = new System.Windows.Forms.CheckBox();
		((System.ComponentModel.ISupportInitialize)this.dataGridView1).BeginInit();
		this.groupBox1.SuspendLayout();
		this.uploadParameters.SuspendLayout();
		this.grpColumnFilter.SuspendLayout();
		base.SuspendLayout();
		this.btnWriteToGD77.Location = new System.Drawing.Point(570, 541);
		this.btnWriteToGD77.Name = "btnWriteToGD77";
		this.btnWriteToGD77.Size = new System.Drawing.Size(123, 28);
		this.btnWriteToGD77.TabIndex = 29;
		this.btnWriteToGD77.Text = "Write to GD-77";
		this.btnWriteToGD77.UseVisualStyleBackColor = true;
		this.btnWriteToGD77.Click += new System.EventHandler(btnWriteToGD77_Click);
		this.btnWriteToGD77.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtRegionId.Location = new System.Drawing.Point(106, 73);
		this.txtRegionId.Name = "txtRegionId";
		this.txtRegionId.Size = new System.Drawing.Size(300, 20);
		this.txtRegionId.TabIndex = 3;
		this.txtRegionId.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.txtRegionId.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnClear.Location = new System.Drawing.Point(12, 541);
		this.btnClear.Name = "btnClear";
		this.btnClear.Size = new System.Drawing.Size(71, 28);
		this.btnClear.TabIndex = 28;
		this.btnClear.Text = "Clear list";
		this.btnClear.UseVisualStyleBackColor = true;
		this.btnClear.Click += new System.EventHandler(btnClear_Click);
		this.btnClear.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblMessage.Location = new System.Drawing.Point(12, 9);
		this.lblMessage.Name = "lblMessage";
		this.lblMessage.Size = new System.Drawing.Size(405, 23);
		this.lblMessage.TabIndex = 0;
		this.lblMessage.Text = "lblMessage";
		this.lblMessage.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.dataGridView1.AllowUserToResizeColumns = false;
		this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridView1.Location = new System.Drawing.Point(12, 228);
		this.dataGridView1.Name = "dataGridView1";
		this.dataGridView1.Size = new System.Drawing.Size(680, 174);
		this.dataGridView1.TabIndex = 20;
		this.dataGridView1.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(dataGridView1_SortCompare);
		this.dataGridView1.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtAgeMaxDays.Location = new System.Drawing.Point(650, 69);
		this.txtAgeMaxDays.Name = "txtAgeMaxDays";
		this.txtAgeMaxDays.Size = new System.Drawing.Size(42, 20);
		this.txtAgeMaxDays.TabIndex = 5;
		this.txtAgeMaxDays.Text = "365";
		this.txtAgeMaxDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
		this.txtAgeMaxDays.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblRegionId.Location = new System.Drawing.Point(12, 72);
		this.lblRegionId.Name = "lblRegionId";
		this.lblRegionId.Size = new System.Drawing.Size(88, 20);
		this.lblRegionId.TabIndex = 2;
		this.lblRegionId.Text = "Region";
		this.lblRegionId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.lblRegionId.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblInactivityFilter.Location = new System.Drawing.Point(459, 72);
		this.lblInactivityFilter.Name = "lblInactivityFilter";
		this.lblInactivityFilter.Size = new System.Drawing.Size(185, 19);
		this.lblInactivityFilter.TabIndex = 4;
		this.lblInactivityFilter.Text = "HamDigital - Inactivity filter (days)";
		this.lblInactivityFilter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblInactivityFilter.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cmbStringLen.FormattingEnabled = true;
		this.cmbStringLen.Items.AddRange(new object[45]
		{
			"6", "7", "8", "9", "10", "11", "12", "13", "14", "15",
			"16", "17", "18", "19", "20", "21", "22", "23", "24", "25",
			"26", "27", "28", "29", "30", "31", "32", "33", "34", "35",
			"36", "37", "38", "39", "40", "41", "42", "43", "44", "45",
			"46", "47", "48", "49", "50"
		});
		this.cmbStringLen.Location = new System.Drawing.Point(15, 19);
		this.cmbStringLen.Name = "cmbStringLen";
		this.cmbStringLen.Size = new System.Drawing.Size(56, 21);
		this.cmbStringLen.TabIndex = 24;
		this.cmbStringLen.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cmbStringLen.SelectedIndexChanged += new System.EventHandler(cmbStringLen_SelectedIndexChanged);
		this.lblEnhancedLength.AutoSize = true;
		this.lblEnhancedLength.Location = new System.Drawing.Point(85, 27);
		this.lblEnhancedLength.Name = "lblEnhancedLength";
		this.lblEnhancedLength.Size = new System.Drawing.Size(109, 13);
		this.lblEnhancedLength.TabIndex = 23;
		this.lblEnhancedLength.Text = "Number of characters";
		this.lblEnhancedLength.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.progressBar1.Location = new System.Drawing.Point(15, 35);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(677, 17);
		this.progressBar1.TabIndex = 1;
		this.btnDownload.Location = new System.Drawing.Point(15, 109);
		this.btnDownload.Name = "btnDownload";
		this.btnDownload.Size = new System.Drawing.Size(153, 23);
		this.btnDownload.TabIndex = 6;
		this.btnDownload.Text = "Download from ...";
		this.btnDownload.UseVisualStyleBackColor = true;
		this.btnDownload.Click += new System.EventHandler(btnDownloadFrom_Click);
		this.btnDownload.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.groupBox1.Controls.Add(this.cmbStringLen);
		this.groupBox1.Controls.Add(this.lblEnhancedLength);
		this.groupBox1.Location = new System.Drawing.Point(6, 28);
		this.groupBox1.Name = "grpRecordLength";
		this.groupBox1.Size = new System.Drawing.Size(230, 56);
		this.groupBox1.TabIndex = 22;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "Data record length";
		this.groupBox1.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.button1.Location = new System.Drawing.Point(539, 109);
		this.button1.Name = "btnCSVImport";
		this.button1.Size = new System.Drawing.Size(153, 23);
		this.button1.TabIndex = 8;
		this.button1.Text = "Import CSV";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(btnImportCSV_Click);
		this.button1.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cmbRadioType.FormattingEnabled = true;
		this.cmbRadioType.Items.AddRange(new object[6] { "GD-77 / GD-77S / MD-760", "DM-1801", "RD-5R", "MD-9600 / MD-UV3x0 / DM-1701", "Custom 8Mb", "Custom 16Mb" });
		this.cmbRadioType.Location = new System.Drawing.Point(500, 55);
		this.cmbRadioType.Name = "cmbRadioType";
		this.cmbRadioType.Size = new System.Drawing.Size(158, 21);
		this.cmbRadioType.TabIndex = 27;
		this.cmbRadioType.SelectedIndexChanged += new System.EventHandler(cmbRadioType_SelectedIndexChanged);
		this.cmbRadioType.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblRadioType.Location = new System.Drawing.Point(445, 58);
		this.lblRadioType.Name = "lblRadioType";
		this.lblRadioType.Size = new System.Drawing.Size(49, 19);
		this.lblRadioType.TabIndex = 26;
		this.lblRadioType.Text = "Radio";
		this.lblRadioType.TextAlign = System.Drawing.ContentAlignment.TopRight;
		this.lblRadioType.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.chkUseVPMemory.AutoSize = true;
		this.chkUseVPMemory.Location = new System.Drawing.Point(508, 19);
		this.chkUseVPMemory.Name = "chkUseVPMemory";
		this.chkUseVPMemory.Size = new System.Drawing.Size(150, 17);
		this.chkUseVPMemory.TabIndex = 25;
		this.chkUseVPMemory.Text = "Use Voice Prompt memory";
		this.chkUseVPMemory.UseVisualStyleBackColor = true;
		this.chkUseVPMemory.CheckedChanged += new System.EventHandler(chkUseVPMemory_CheckedChanged);
		this.chkUseVPMemory.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cmbDownloadLocation.FormattingEnabled = true;
		this.cmbDownloadLocation.Items.AddRange(new object[3] { "RadioId.net", "HamDigital", "URL" });
		this.cmbDownloadLocation.Location = new System.Drawing.Point(174, 111);
		this.cmbDownloadLocation.Name = "cmbDownloadLocation";
		this.cmbDownloadLocation.Size = new System.Drawing.Size(121, 21);
		this.cmbDownloadLocation.TabIndex = 7;
		this.cmbDownloadLocation.SelectedIndexChanged += new System.EventHandler(cmbDownloadLocation_SelectedIndexChanged);
		this.cmbDownloadLocation.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDownloadURL.Location = new System.Drawing.Point(18, 141);
		this.txtDownloadURL.Name = "txtDownloadURL";
		this.txtDownloadURL.Size = new System.Drawing.Size(674, 20);
		this.txtDownloadURL.TabIndex = 9;
		this.txtDownloadURL.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.uploadParameters.Controls.Add(this.groupBox1);
		this.uploadParameters.Controls.Add(this.chkUseVPMemory);
		this.uploadParameters.Controls.Add(this.cmbRadioType);
		this.uploadParameters.Controls.Add(this.lblRadioType);
		this.uploadParameters.Location = new System.Drawing.Point(12, 424);
		this.uploadParameters.Name = "uploadParameters";
		this.uploadParameters.Size = new System.Drawing.Size(680, 100);
		this.uploadParameters.TabIndex = 21;
		this.uploadParameters.TabStop = false;
		this.uploadParameters.Text = "Write parameters";
		this.uploadParameters.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.grpColumnFilter.Controls.Add(this.cmbSeparator);
		this.grpColumnFilter.Controls.Add(this.lblSeparator);
		this.grpColumnFilter.Controls.Add(this.chkCol_5);
		this.grpColumnFilter.Controls.Add(this.chkCol_4);
		this.grpColumnFilter.Controls.Add(this.chkCol_3);
		this.grpColumnFilter.Controls.Add(this.chkCol_2);
		this.grpColumnFilter.Controls.Add(this.chkCol_1);
		this.grpColumnFilter.Location = new System.Drawing.Point(18, 168);
		this.grpColumnFilter.Name = "grpColumnFilter";
		this.grpColumnFilter.Size = new System.Drawing.Size(674, 54);
		this.grpColumnFilter.TabIndex = 10;
		this.grpColumnFilter.TabStop = false;
		this.grpColumnFilter.Text = "Data columns";
		this.grpColumnFilter.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.cmbSeparator.FormattingEnabled = true;
		this.cmbSeparator.ItemHeight = 13;
		this.cmbSeparator.Items.AddRange(new object[2] { "SPACE", "DOT" });
		this.cmbSeparator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbSeparator.Location = new System.Drawing.Point(603, 18);
		this.cmbSeparator.Name = "cmbSeparator";
		this.cmbSeparator.Size = new System.Drawing.Size(65, 21);
		this.cmbSeparator.TabIndex = 17;
		this.cmbSeparator.SelectedIndexChanged += new System.EventHandler(cmbSeparator_SelectedIndexChanged);
		this.cmbSeparator.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblSeparator.Location = new System.Drawing.Point(511, 20);
		this.lblSeparator.Name = "lblSeparator";
		this.lblSeparator.Size = new System.Drawing.Size(86, 19);
		this.lblSeparator.TabIndex = 16;
		this.lblSeparator.Text = "Separator";
		this.lblSeparator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblSeparator.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.chkCol_5.AutoSize = true;
		this.chkCol_5.Location = new System.Drawing.Point(407, 19);
		this.chkCol_5.Name = "chkCol_5";
		this.chkCol_5.Size = new System.Drawing.Size(32, 17);
		this.chkCol_5.TabIndex = 15;
		this.chkCol_5.Text = "5";
		this.chkCol_5.UseVisualStyleBackColor = true;
		this.chkCol_5.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.chkCol_4.AutoSize = true;
		this.chkCol_4.Location = new System.Drawing.Point(307, 19);
		this.chkCol_4.Name = "chkCol_4";
		this.chkCol_4.Size = new System.Drawing.Size(32, 17);
		this.chkCol_4.TabIndex = 14;
		this.chkCol_4.Text = "4";
		this.chkCol_4.UseVisualStyleBackColor = true;
		this.chkCol_4.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.chkCol_3.AutoSize = true;
		this.chkCol_3.Location = new System.Drawing.Point(203, 19);
		this.chkCol_3.Name = "chkCol_3";
		this.chkCol_3.Size = new System.Drawing.Size(32, 17);
		this.chkCol_3.TabIndex = 13;
		this.chkCol_3.Text = "3";
		this.chkCol_3.UseVisualStyleBackColor = true;
		this.chkCol_3.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.chkCol_2.AutoSize = true;
		this.chkCol_2.Location = new System.Drawing.Point(113, 19);
		this.chkCol_2.Name = "chkCol_2";
		this.chkCol_2.Size = new System.Drawing.Size(32, 17);
		this.chkCol_2.TabIndex = 12;
		this.chkCol_2.Text = "2";
		this.chkCol_2.UseVisualStyleBackColor = true;
		this.chkCol_2.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.chkCol_1.AutoSize = true;
		this.chkCol_1.Location = new System.Drawing.Point(15, 20);
		this.chkCol_1.Name = "chkCol_1";
		this.chkCol_1.Size = new System.Drawing.Size(32, 17);
		this.chkCol_1.TabIndex = 11;
		this.chkCol_1.Text = "1";
		this.chkCol_1.UseVisualStyleBackColor = true;
		this.chkCol_1.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(731, 588);
		base.Controls.Add(this.grpColumnFilter);
		base.Controls.Add(this.uploadParameters);
		base.Controls.Add(this.cmbDownloadLocation);
		base.Controls.Add(this.lblRegionId);
		base.Controls.Add(this.lblInactivityFilter);
		base.Controls.Add(this.txtDownloadURL);
		base.Controls.Add(this.txtAgeMaxDays);
		base.Controls.Add(this.txtRegionId);
		base.Controls.Add(this.progressBar1);
		base.Controls.Add(this.dataGridView1);
		base.Controls.Add(this.lblMessage);
		base.Controls.Add(this.btnClear);
		base.Controls.Add(this.btnWriteToGD77);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.btnDownload);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "DMRIDForm";
		this.Text = "DMR ID";
		this.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(DMRIDFormNew_FormClosing);
		base.Load += new System.EventHandler(DMRIDForm_Load);
		((System.ComponentModel.ISupportInitialize)this.dataGridView1).EndInit();
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.uploadParameters.ResumeLayout(false);
		this.uploadParameters.PerformLayout();
		this.grpColumnFilter.ResumeLayout(false);
		this.grpColumnFilter.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
