using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Media;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Extras.OpenGD77;

namespace DMR;

public class OpenGD77Form : Form
{
	public enum CustomDataType
	{
		UNINITIALISED_TYPE = 255,
		IMAGE_TYPE = 1,
		MELODY_TYPE = 2,
		SATELLITE_KEPS_TYPE = 3,
		THEME_DAY_TYPE = 4,
		THEME_NIGHT_TYPE = 5
	}

	public enum RadioInfoFeatures : ushort
	{
		SCREEN_INVERTED = 1,
		DMRID_USES_VOICE_PROMPTS = 2,
		VOICE_PROMPTS_AVAILABLE = 4,
		SUPPORT_SETTINGS_ACCESS = 8
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 46)]
	public struct RadioInfo
	{
		[FieldOffset(0)]
		[MarshalAs(UnmanagedType.U4)]
		public uint structVersion;

		[FieldOffset(4)]
		[MarshalAs(UnmanagedType.U4)]
		public uint radioType;

		[FieldOffset(8)]
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		public string identifier;

		[FieldOffset(24)]
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		public string buildDateTime;

		[FieldOffset(40)]
		[MarshalAs(UnmanagedType.U4)]
		public uint flashId;

		[FieldOffset(44)]
		[MarshalAs(UnmanagedType.U2)]
		public ushort features;
	}



    public static byte[] CustomData;

	public static byte[] LastUsedChannelsData;

	public static byte[] SatelliteKepsData = null;

	private SerialPort commPort;

	private const string IDENTIFIER = "OpenGD77";

	private WebClient _wc;

	private string _kepsTxt;

	private string satsAdded = "";

	private BackgroundWorker worker;

	public const int MAX_TRANSFER_BUFFER_SIZE = 1032;

	public const int MAX_TRANSFER_READ_SIZE = 1024;

	public const int MAX_TRANSFER_WRITE_SIZE = 1024;

	private OpenGD77CommsTransferData.CommsAction _initialAction;

	public static Dictionary<string, string> StringsDict = new Dictionary<string, string>();

	private const int VOICE_PROMPTS_ADDRESS_IN_FLASH = 586752;

	public const int VOICE_PROMPTS_SIZE_MAX = 166912;

	private const string LAST_USED_CHANNEL_IN_ZONE_IDENTIFIER = "LUCZ";

	private List<KepAndSatData> satConstData;

	private char writeCommandCharacter = 'W';

	public static readonly int FLASH_MEMORY_EEPROM_EMU_SIZE = 131072;

	public static readonly int STM32_FLASH_ADDRESS_OFFSET = FLASH_MEMORY_EEPROM_EMU_SIZE;

	public static readonly int CUSTOM_DATA_HEADER_SIZE = "OpenGD77".Length + 4;

	public static bool oldUSBBufferSize = true;

	private const string TLE_COMPESSION_LUT = "0123456789. +-*";

	private IContainer components;

	private Button btnBackupEEPROM;

	private ProgressBar progressBar1;

	private Button btnBackupFlash;

	private Button btnRestoreEEPROM;

	private Button btnRestoreFlash;

	private Button btnReadCodeplug;

	private Button btnWriteCodeplug;

	private Label txtMessage;

	private Button btnBackupCalibration;

	private Button btnRestoreCalibration;

	private Button btnBackupMCUROM;

	private Button btnDownloadScreenGrab;

	private Button btnOpenFile;

	private PictureBox pictureBox1;

	private TextBox txtBootTune;

	private GroupBox grpFunThings;

	private Label lblBootTune;

	private Button btnPlayTune;

	private Button btnCompressAudio;

	private Button btnWriteVoicePrompts;

	private Button btnClearVoicePrompts;

	private Button btnDownloadSatelliteKeps;

	private TextBox txtKepsServer;

	private Button btnBackupSettings;

	private Button btnRestoreSettings;

	private Button btnSaveNMEA;
    private Button btnResetSettings;
    private Button btnReadSecureRegisters;

	public OpenGD77Form(OpenGD77CommsTransferData.CommsAction initAction)
	{
		_initialAction = initAction;
		InitializeComponent();
		if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
		{
			btnBackupCalibration.Visible = false;
			btnRestoreCalibration.Visible = false;
			btnRestoreEEPROM.Visible = false;
			btnBackupEEPROM.Visible = false;
			btnBackupSettings.Visible = false;
			btnRestoreSettings.Visible = false;
			btnReadSecureRegisters.Visible = true;
		}
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		int returnedOffsetPos = 0;
		if (FindCustomDataBlock(CustomData, CustomDataType.IMAGE_TYPE, ref returnedOffsetPos))
		{
			loadImageFromCodeplug(returnedOffsetPos + 8);
		}
		if (FindCustomDataBlock(CustomData, CustomDataType.MELODY_TYPE, ref returnedOffsetPos))
		{
			loadMelodyFromCodeplug(returnedOffsetPos + 8);
		}
		txtKepsServer.Text = IniFileUtils.getProfileStringWithDefault("Setup", "SatelliteKepsURL", "https://celestrak.org/NORAD/elements/gp.php?GROUP=amateur&FORMAT=tle");
	}

	private void loadSatellitesTxtData(string satelliteDataFile)
	{
		try
		{
			string[] array = File.ReadAllText(satelliteDataFile).Split('\n');
			satConstData = new List<KepAndSatData>();
			for (int i = 1; i < Math.Min(array.Length, 26); i++)
			{
				if (array[i].Trim().Length > 0)
				{
					satConstData.Add(new KepAndSatData(array[i].Trim()));
				}
			}
		}
		catch (Exception)
		{
			txtKepsServer.Enabled = false;
			btnDownloadSatelliteKeps.Enabled = false;
			IniFileUtils.WriteProfileString("Setup", "SatelliteTxtFile" + MainForm.PRODUCT_VERSION, "");
		}
	}

	public static Form getMainForm()
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

	public static bool RadioInfoIsFeatureSet(RadioInfoFeatures feature)
	{
		return ((uint)MainForm.RadioInfo.features & (uint)feature) != 0;
	}




    public bool probeRadioModel(bool stealth = false, bool startup = false)
    {
		if (!setupCommPort(startup))
		{
			if (!stealth)
			{
				SystemSounds.Hand.Play();
				MessageBox.Show(StringsDict["No_com_port"]);
			}
			return false;
		}
		sendCommand(commPort, 254);
		commPort.DiscardInBuffer();
		if (!sendCommand(commPort, stealth ? 254 : 0))
		{
			displayMessage(StringsDict["Error_connecting_to_the_radio"]);
			commPort = null;
			return false;
		}
		MainForm.RadioInfo = readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort, stealth);
		if (MainForm.RadioInfo.identifier == "RUSSIAN")
		{
			if (MainForm.RadioInfo.radioType == 5 || MainForm.RadioInfo.radioType == 6 || MainForm.RadioInfo.radioType == 8 || MainForm.RadioInfo.radioType == 10 || MainForm.RadioInfo.radioType == 9 || MainForm.RadioInfo.radioType == 7 || MainForm.RadioInfo.radioType == 106)
			{
				writeCommandCharacter = 'X';
				btnBackupCalibration.Visible = false;
				btnRestoreCalibration.Visible = false;
				btnRestoreEEPROM.Visible = false;
				btnBackupEEPROM.Visible = false;
				btnBackupSettings.Visible = false;
				btnRestoreSettings.Visible = false;
				btnReadSecureRegisters.Visible = true;
				((MainForm)getMainForm())?.changeRadioType(MainForm.RadioTypeEnum.RadioTypeSTM32);
			}
			else
			{
				writeCommandCharacter = 'W';
				btnBackupCalibration.Visible = true;
				btnRestoreCalibration.Visible = true;
				btnRestoreEEPROM.Visible = true;
				btnBackupEEPROM.Visible = true;
				btnBackupSettings.Visible = true;
				btnRestoreSettings.Visible = true;
				btnReadSecureRegisters.Visible = false;
				((MainForm)getMainForm())?.changeRadioType(MainForm.RadioTypeEnum.RadioTypeMK22);
			}
			if (!stealth)
			{
				sendCommand(commPort, 5);
			}
			commPort.Close();
			commPort = null;
			return true;
		}
		else
		{
            sendCommand(commPort, 6);
            commPort.Close();
            commPort = null;
            MessageBox.Show(StringsDict["Incorrect_firmware"], StringsDict["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}
	}

	public static int LittleEndianToInt(byte[] dataBytes, int offset)
	{
		return dataBytes[offset] + dataBytes[offset + 1] * 256 + dataBytes[offset + 2] * 256 * 256 + dataBytes[offset + 3] * 256 * 256 * 256;
	}

	public static RadioInfo readOpenGD77RadioInfoAndUpdateUSBBufferSize(SerialPort port, bool stealth = false)
	{
		RadioInfo result = readOpenGD77RadioInfo(port, stealth);
		string text = MainForm.RadioInfo.buildDateTime.Substring(0, 8);
		oldUSBBufferSize = true;
		try
		{
			if (int.Parse(text) > 20211002)
			{
				oldUSBBufferSize = false;
			}
		}
		catch (FormatException)
		{
			MessageBox.Show("Unable to parse '" + text + "'");
		}
		return result;
	}

	public static int getUSBReadBufferSize()
	{
		if (!oldUSBBufferSize)
		{
			return 1024;
		}
		return 32;
	}

	public static int getUSBWriteBufferSize()
	{
		if (!oldUSBBufferSize)
		{
			return 1024;
		}
		return 32;
	}

	public static bool FindCustomDataBlock(byte[] customData, CustomDataType typeToFind, ref int returnedOffsetPos, int uninitLength = 1)
	{
		returnedOffsetPos = CUSTOM_DATA_HEADER_SIZE;
		do
		{
			if (customData[returnedOffsetPos] == (byte)typeToFind)
			{
				bool num;
				if (typeToFind == CustomDataType.UNINITIALISED_TYPE)
				{
					num = returnedOffsetPos + 8 + uninitLength <= customData.Length - 8;
				}
				else
				{
					if (returnedOffsetPos + 8 >= customData.Length - 8)
					{
						goto IL_0054;
					}
					num = returnedOffsetPos + 8 + LittleEndianToInt(customData, returnedOffsetPos + 4) <= customData.Length - 8;
				}
				if (num)
				{
					return true;
				}
			}
			goto IL_0054;
			IL_0054:
			returnedOffsetPos += 8 + LittleEndianToInt(customData, returnedOffsetPos + 4);
		}
		while (returnedOffsetPos < customData.Length - 8);
		return false;
	}

	private void loadMelodyFromCodeplug(int position)
	{
		string text = "";
		for (int i = 0; i < 512 && (CustomData[position + i] != 0 || CustomData[position + i + 1] != 0); i++)
		{
			text = text + CustomData[position + i] + ",";
		}
		if (text == "")
		{
			text = "0,0,";
		}
		txtBootTune.Text = text.Substring(0, text.Length - 1);
	}

	private void loadSatelliteKepsFromCodeplug(int position)
	{
		SatelliteKepsData = new byte[2580];
		Array.Copy(CustomData, 0, SatelliteKepsData, position + 8, 1024);
	}

	private void loadImageFromCodeplug(int position)
	{
		Bitmap bitmap = new Bitmap(128, 64);
		Graphics.FromImage(bitmap).Clear(Color.White);
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 128; j++)
			{
				for (int k = 0; k < 8; k++)
				{
					if (((CustomData[position + i * 128 + j] >> k) & 1) != 0)
					{
						bitmap.SetPixel(j, i * 8 + k, Color.Black);
					}
				}
			}
		}
		pictureBox1.Image = bitmap;
	}

	public static byte[] LoadCustomData(byte[] eeprom)
	{
		CustomData = new byte[Settings.ADDR_OPENGD77_CUSTOM_DATA_END - Settings.ADDR_OPENGD77_CUSTOM_DATA_START];
		Array.Copy(eeprom, Settings.ADDR_OPENGD77_CUSTOM_DATA_START, CustomData, 0, CustomData.Length);
		bool flag = true;
		for (int i = 0; i < "OpenGD77".Length; i++)
		{
			if (CustomData[i] != "OpenGD77"[i])
			{
				flag = false;
				break;
			}
		}
		if (!flag)
		{
			CustomData.Fill(byte.MaxValue);
			Array.Copy(Encoding.ASCII.GetBytes("OpenGD77"), CustomData, "OpenGD77".Length);
			CustomData[8] = 1;
			CustomData[9] = 0;
			CustomData[10] = 0;
			CustomData[11] = 0;
		}
		return CustomData;
	}

	public static void ClearLastUsedChannelsData()
	{
		LastUsedChannelsData.Fill((byte)0);
		Array.Copy(Encoding.ASCII.GetBytes("LUCZ"), LastUsedChannelsData, "LUCZ".Length);
	}

	public static byte[] LoadLastUsedChannelsData(byte[] eeprom)
	{
		LastUsedChannelsData = new byte[Settings.ADDR_OPENGD77_LAST_USED_CHANNELS_DATA_END - Settings.ADDR_OPENGD77_LAST_USED_CHANNELS_DATA_START];
		Array.Copy(eeprom, Settings.ADDR_OPENGD77_LAST_USED_CHANNELS_DATA_START, LastUsedChannelsData, 0, LastUsedChannelsData.Length);
		bool flag = true;
		for (int i = 0; i < "LUCZ".Length; i++)
		{
			if (LastUsedChannelsData[i] != "LUCZ"[i])
			{
				flag = false;
				break;
			}
		}
		if (!flag)
		{
			ClearLastUsedChannelsData();
		}
		return LastUsedChannelsData;
	}

	private void downloadKepsCompleteHandler(object sender, DownloadStringCompletedEventArgs e)
	{
		bool flag = false;
		try
		{
			_kepsTxt = e.Result;
		}
		catch (Exception)
		{
			MessageBox.Show("Не удалось загрузить данные спутников из Интернета!");
			flag = true;
			enableDisableAllButtons(show: true);
		}
		if (!flag)
		{
			if (importKeps(sender, e))
			{
				IniFileUtils.WriteProfileString("Setup", "SatelliteKepsURL", txtKepsServer.Text);
			}
			else
			{
				enableDisableAllButtons(show: true);
			}
		}
		_wc = null;
		Cursor.Current = Cursors.Default;
		progressBar1.Value = 0;
	}

	private byte[] compressTLEText(string s)
	{
		List<byte> list = new List<byte>();
		int length = s.Length;
		for (int i = 0; i < length; i += 2)
		{
			int num = "0123456789. +-*".IndexOf(s[i]) << 4;
			int num2 = "0123456789. +-*".IndexOf(s[i + 1]);
			byte item = (byte)(num + num2);
			list.Add(item);
		}
		return list.ToArray();
	}

	private bool importKeps(object sender, EventArgs e)
	{
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
		satsAdded = "";
		if (_kepsTxt == null)
		{
			return false;
		}
		List<string> list = new List<string>();
		StringReader stringReader = new StringReader(_kepsTxt);
		string text;
		do
		{
			text = stringReader.ReadLine();
			if (text != null)
			{
				list.Add(text);
			}
		}
		while (text != null);
		string[] array = list.ToArray();
		byte[] array2 = new byte[2520];
		int num = 0;
		int num2 = 0;
		foreach (KepAndSatData item in satConstData.OrderBy((KepAndSatData x) => x.DisplayName).ToList())
		{
			for (int i = 0; i < array.Length / 3; i++)
			{
				try
				{
					if (num2 < 25 && item.CatalogueNumber == array[i * 3 + 1].Substring(2, 6))
					{
						string text2 = item.DisplayName.Trim();
						if (text2.Length > 8)
						{
							text2 = text2.Substring(0, 8);
						}
						satsAdded = satsAdded + text2 + ", ";
						byte[] bytes = Encoding.ASCII.GetBytes(text2);
						Array.Copy(bytes, 0, array2, num, bytes.Length);
						num += 8;
						string text3 = array[i * 3 + 1].PadRight(70);
						string s = text3.Substring(18, 2) + text3.Substring(20, 12) + text3.Substring(33, 10);
						byte[] array3 = compressTLEText(s);
						Array.Copy(array3, 0, array2, num, array3.Length);
						num += array3.Length;
						string text4 = array[i * 3 + 2].PadRight(70);
						string s2 = text4.Substring(8, 8) + text4.Substring(17, 8) + text4.Substring(26, 7) + text4.Substring(34, 8) + text4.Substring(43, 8) + text4.Substring(52, 11) + text4.Substring(63, 5) + " ";
						array3 = compressTLEText(s2);
						Array.Copy(array3, 0, array2, num, array3.Length);
						num += array3.Length;
						bytes = BitConverter.GetBytes(item.Rx1);
						Array.Copy(bytes, 0, array2, num, 4);
						num += 4;
						bytes = BitConverter.GetBytes(item.Tx1);
						Array.Copy(bytes, 0, array2, num, 4);
						num += 4;
						bytes = BitConverter.GetBytes(item.TxCTCSS1);
						Array.Copy(bytes, 0, array2, num, 2);
						num += 2;
						bytes = BitConverter.GetBytes(item.ArmCTCSS1);
						Array.Copy(bytes, 0, array2, num, 2);
						num += 2;
						bytes = BitConverter.GetBytes(item.Rx2);
						Array.Copy(bytes, 0, array2, num, 4);
						num += 4;
						bytes = BitConverter.GetBytes(item.Tx2);
						Array.Copy(bytes, 0, array2, num, 4);
						num += 4;
						bytes = BitConverter.GetBytes(item.Rx3);
						Array.Copy(bytes, 0, array2, num, 4);
						num += 4;
						bytes = BitConverter.GetBytes(item.Tx3);
						Array.Copy(bytes, 0, array2, num, 4);
						num += 4;
						string text5 = item.Reserved.Trim();
						if (text5.Length > 24)
						{
							text5 = text5.Substring(0, 24);
						}
						bytes = Encoding.ASCII.GetBytes(text5);
						Array.Copy(bytes, 0, array2, num, bytes.Length);
						num += 24;
						num2++;
					}
				}
				catch (Exception)
				{
				}
			}
		}
		if (num2 == 0)
		{
			MessageBox.Show("Данные не получены", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return false;
		}
		int returnedOffsetPos = 0;
		bool flag = false;
		if (FindCustomDataBlock(CustomData, CustomDataType.SATELLITE_KEPS_TYPE, ref returnedOffsetPos))
		{
			flag = true;
		}
		else if (FindCustomDataBlock(CustomData, CustomDataType.UNINITIALISED_TYPE, ref returnedOffsetPos, 2520))
		{
			flag = true;
		}
		if (flag)
		{
			CustomData[returnedOffsetPos] = 3;
			CustomData[returnedOffsetPos + 1] = 0;
			CustomData[returnedOffsetPos + 2] = 0;
			CustomData[returnedOffsetPos + 3] = 0;
			byte[] bytes = BitConverter.GetBytes(num);
			CustomData[returnedOffsetPos + 4] = 216;
			CustomData[returnedOffsetPos + 5] = 9;
			CustomData[returnedOffsetPos + 6] = 0;
			CustomData[returnedOffsetPos + 7] = 0;
			Array.Copy(array2, 0, CustomData, returnedOffsetPos + 8, array2.Length);
			satsAdded = satsAdded.Remove(satsAdded.Length - 2);
			OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.WRITE_SATELLITE_KEPS);
			openGD77CommsTransferData.dataBuff = new byte[Settings.ADDR_OPENGD77_CUSTOM_DATA_END - Settings.ADDR_OPENGD77_CUSTOM_DATA_START];
			Array.Copy(CustomData, openGD77CommsTransferData.dataBuff, openGD77CommsTransferData.dataBuff.Length);
			enableDisableAllButtons(show: false);
			perFormCommsTask(openGD77CommsTransferData);
			return true;
		}
		MessageBox.Show("ERROR :-( Kep update failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		return false;
	}

	public static bool convertThemeColours565(ref byte[] dataBytes, int offset, bool checkForCustomDataHeader)
	{
		int num = offset;
		bool flag = true;
		if (checkForCustomDataHeader)
		{
			for (int i = 0; i < "OpenGD77".Length; i++)
			{
				if (dataBytes[num + i] != "OpenGD77"[i])
				{
					flag = false;
					break;
				}
			}
			num += CUSTOM_DATA_HEADER_SIZE;
		}
		if (flag)
		{
			int num2 = 0;
			do
			{
				if (dataBytes[num] == 4 || dataBytes[num] == 5)
				{
					int num3 = LittleEndianToInt(dataBytes, num + 4);
					num += 8;
					if (num3 == ThemeForm.THEME_SIZE)
					{
						for (int j = 0; j < num3; j += 2)
						{
							ushort num4 = (ushort)((dataBytes[num + j] << 8) + dataBytes[num + j + 1]);
							ushort num5 = (ushort)(((num4 & 0xF800) >> 11) | (num4 & 0x7E0) | ((num4 & 0x1F) << 11));
							dataBytes[num + j] = (byte)((num5 & 0xFF00) >> 8);
							dataBytes[num + j + 1] = (byte)(num5 & 0xFF);
						}
						num2++;
					}
					num += num3;
				}
				else
				{
					num += 8 + LittleEndianToInt(dataBytes, num + 4);
				}
			}
			while (num < dataBytes.Length - 8);
			return num2 > 0;
		}
		return false;
	}

	public static bool sendCommand(SerialPort port, int commandNumber, int x_or_command_option_number = 0, int y = 0, int iSize = 0, int alignment = 0, int isInverted = 0, string message = "")
	{
		Encoding transcoder = Encoding.GetEncoding("windows-1251");
        int num = 100;
		byte[] array = new byte[1032];
		int num2 = 2;
		array[0] = 67;
		array[1] = (byte)commandNumber;
		switch (commandNumber)
		{
		case 2:
			array[3] = (byte)y;
			array[4] = (byte)iSize;
			array[5] = (byte)alignment;
			array[6] = (byte)isInverted;
			num2 += 5 + Math.Min(message.Length, 16);
			Buffer.BlockCopy(transcoder.GetBytes(message), 0, array, 7, num2 - 7);
			break;
		case 6:
			array[2] = (byte)x_or_command_option_number;
			num2++;
			break;
		case 77:
            array[2] = (byte)x_or_command_option_number;
            num2++;
            break;
        }
		port.Write(array, 0, num2);
		while (port.BytesToWrite > 0)
		{
			Thread.Sleep(1);
		}
		Thread.Sleep(50);
		while (port.BytesToRead == 0 && num-- > 0)
		{
			Thread.Sleep(5);
		}
		if (num != -1)
		{
			port.Read(array, 0, port.BytesToRead);
		}
		if (array[1] == commandNumber)
		{
			return num != -1;
		}
        return false;
	}

	public static bool flashWritePrepareSector(SerialPort port, char writeCharacter, int address, ref byte[] sendbuffer, ref byte[] readbuffer, OpenGD77CommsTransferData dataObj)
	{
		int num = 100;
		dataObj.data_sector = address / 4096;
		sendbuffer[0] = (byte)writeCharacter;
		sendbuffer[1] = 1;
		sendbuffer[2] = (byte)((dataObj.data_sector >> 16) & 0xFF);
		sendbuffer[3] = (byte)((dataObj.data_sector >> 8) & 0xFF);
		sendbuffer[4] = (byte)(dataObj.data_sector & 0xFF);
		port.Write(sendbuffer, 0, 5);
		while (port.BytesToWrite > 0)
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

	public static bool flashSendData(SerialPort port, char writeCharacter, int address, int len, ref byte[] sendbuffer, ref byte[] readbuffer)
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
		while (port.BytesToWrite > 0)
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

	public static bool flashWriteSector(SerialPort port, char writeCharacter, ref byte[] sendbuffer, ref byte[] readbuffer, OpenGD77CommsTransferData dataObj)
	{
		int num = 100;
		dataObj.data_sector = -1;
		sendbuffer[0] = (byte)writeCharacter;
		sendbuffer[1] = 3;
		port.Write(sendbuffer, 0, 2);
		while (port.BytesToWrite > 0)
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
			if (num3 > getUSBReadBufferSize())
			{
				num3 = getUSBReadBufferSize();
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
			Console.WriteLine(string.Format(StringsDict["read_stopped"], num2));
			return false;
		}
		return true;
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
			if (num3 > getUSBWriteBufferSize())
			{
				num3 = getUSBWriteBufferSize();
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
			Console.WriteLine(string.Format(StringsDict["write_stopped"], num2));
			return false;
		}
		return true;
	}

	public bool WriteEEPROM(SerialPort port, OpenGD77CommsTransferData dataObj)
	{
		int num = 0;
		byte[] array = new byte[1032];
		byte[] array2 = new byte[1032];
		int num2 = dataObj.startDataAddressInTheRadio;
		int localDataBufferStartPosition = dataObj.localDataBufferStartPosition;
		int num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2;
		while (num3 > 0)
		{
			if (num3 > getUSBWriteBufferSize())
			{
				num3 = getUSBWriteBufferSize();
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
			array[0] = (byte)writeCommandCharacter;
			array[1] = 4;
			array[2] = (byte)((num2 >> 24) & 0xFF);
			array[3] = (byte)((num2 >> 16) & 0xFF);
			array[4] = (byte)((num2 >> 8) & 0xFF);
			array[5] = (byte)(num2 & 0xFF);
			array[6] = (byte)((num4 >> 8) & 0xFF);
			array[7] = (byte)(num4 & 0xFF);
			port.Write(array, 0, num4 + 8);
			while (port.BytesToWrite > 0)
			{
				Thread.Sleep(1);
			}
			Thread.Sleep(50);
			while (port.BytesToRead == 0)
			{
				Thread.Sleep(5);
			}
			port.Read(array2, 0, port.BytesToRead);
			if (array2[0] == array[0] && array2[1] == array[1])
			{
				int num5 = (num2 - dataObj.startDataAddressInTheRadio) * 100 / dataObj.transferLength;
				if (num != num5)
				{
					updateProgess(num5);
					num = num5;
				}
				num2 += num4;
				num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2;
				continue;
			}
			//Console.WriteLine(string.Format(StringsDict["write_stopped"], num2));
			return false;
		}
		return true;
	}

	private bool WriteWAV(OpenGD77CommsTransferData dataObj)
	{
		int num = 0;
		byte[] array = new byte[1032];
		byte[] array2 = new byte[1032];
		int num2 = dataObj.startDataAddressInTheRadio;
		int localDataBufferStartPosition = dataObj.localDataBufferStartPosition;
		int num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2;
		while (num3 > 0)
		{
			if (num3 > getUSBWriteBufferSize())
			{
				num3 = getUSBWriteBufferSize();
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
			array[0] = (byte)writeCommandCharacter;
			array[1] = 7;
			array[2] = (byte)((num2 >> 24) & 0xFF);
			array[3] = (byte)((num2 >> 16) & 0xFF);
			array[4] = (byte)((num2 >> 8) & 0xFF);
			array[5] = (byte)(num2 & 0xFF);
			array[6] = (byte)((num4 >> 8) & 0xFF);
			array[7] = (byte)(num4 & 0xFF);
			commPort.Write(array, 0, num4 + 8);
			while (commPort.BytesToWrite > 0)
			{
				Thread.Sleep(1);
			}
			Thread.Sleep(50);
			while (commPort.BytesToRead == 0)
			{
				Thread.Sleep(5);
			}
			commPort.Read(array2, 0, commPort.BytesToRead);
			if (array2[0] == array[0] && array2[1] == array[1])
			{
				int num5 = (num2 - dataObj.startDataAddressInTheRadio) * 100 / dataObj.transferLength;
				if (num != num5)
				{
					updateProgess(num5);
					num = num5;
				}
				num2 += num4;
				num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2;
				continue;
			}
			Console.WriteLine(string.Format(StringsDict["write_stopped"], num2));
			return false;
		}
		return true;
	}

	private bool CompressWAV(SerialPort port, OpenGD77CommsTransferData dataObj)
	{
		dataObj.localDataBufferStartPosition = 0;
		OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData();
		OpenGD77CommsTransferData openGD77CommsTransferData2 = new OpenGD77CommsTransferData();
		openGD77CommsTransferData2.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadAMBE;
		openGD77CommsTransferData2.dataBuff = new byte[32];
		openGD77CommsTransferData2.localDataBufferStartPosition = 0;
		openGD77CommsTransferData2.startDataAddressInTheRadio = 0;
		openGD77CommsTransferData2.transferLength = 32;
		byte[] array = new byte[16384];
		int num = 0;
		sendCommand(port, 6, 5);
		while (dataObj.localDataBufferStartPosition < dataObj.dataBuff.Length)
		{
			openGD77CommsTransferData.dataBuff = new byte[960];
			openGD77CommsTransferData.localDataBufferStartPosition = 0;
			openGD77CommsTransferData.startDataAddressInTheRadio = 0;
			openGD77CommsTransferData.transferLength = openGD77CommsTransferData.dataBuff.Length;
			Array.Copy(dataObj.dataBuff, dataObj.localDataBufferStartPosition, openGD77CommsTransferData.dataBuff, 0, 960);
			sendCommand(port, 6, 6);
			WriteWAV(openGD77CommsTransferData);
			ReadFlashOrEEPROMOrROMOrScreengrab(port, openGD77CommsTransferData2);
			Array.Copy(openGD77CommsTransferData2.dataBuff, 0, array, num, 27);
			num += 27;
			dataObj.localDataBufferStartPosition += Math.Min(960, dataObj.dataBuff.Length - dataObj.localDataBufferStartPosition);
		}
		Array.Resize(ref array, num);
		dataObj.dataBuff = array;
		return true;
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
				case OpenGD77CommsTransferData.CommsAction.BACKUP_EEPROM:
				{
					SaveFileDialog saveFileDialog4 = new SaveFileDialog();
					saveFileDialog4.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastSaveEEPROMLocation", null);
					saveFileDialog4.Filter = StringsDict["EEPROM_files"] + " (*.bin)|*.bin";
					saveFileDialog4.FilterIndex = 1;
					if (saveFileDialog4.ShowDialog() == DialogResult.OK)
					{
						File.WriteAllBytes(saveFileDialog4.FileName, openGD77CommsTransferData.dataBuff);
						IniFileUtils.WriteProfileString("Setup", "LastSaveEEPROMLocation", Path.GetDirectoryName(saveFileDialog4.FileName));
					}
					enableDisableAllButtons(show: true);
					openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
					break;
				}
				case OpenGD77CommsTransferData.CommsAction.BACKUP_FLASH:
				{
					SaveFileDialog saveFileDialog8 = new SaveFileDialog();
					saveFileDialog8.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastSaveFlashLocation", null);
					saveFileDialog8.Filter = "Flash files (*.bin)|*.bin";
					saveFileDialog8.FilterIndex = 1;
					if (saveFileDialog8.ShowDialog() == DialogResult.OK)
					{
						File.WriteAllBytes(saveFileDialog8.FileName, openGD77CommsTransferData.dataBuff);
					}
					IniFileUtils.WriteProfileString("Setup", "LastSaveFlashLocation", Path.GetDirectoryName(saveFileDialog8.FileName));
					enableDisableAllButtons(show: true);
					openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
					break;
				}
				case OpenGD77CommsTransferData.CommsAction.SAVE_NMEA_LOG:
				{
					FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
					folderBrowserDialog.SelectedPath = IniFileUtils.getProfileStringWithDefault("Setup", "LastSaveNMEALocation", null);
					if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
					{
						File.WriteAllBytes(folderBrowserDialog.SelectedPath + Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyyMMddHHmm") + "_NMEA-data.bin", openGD77CommsTransferData.dataBuff);
						IniFileUtils.WriteProfileString("Setup", "LastSaveNMEALocation", folderBrowserDialog.SelectedPath);
						List<string> list = Encoding.UTF8.GetString(openGD77CommsTransferData.dataBuff).Split(new string[1] { "AAAA" }, StringSplitOptions.None).ToList();
						if (list != null)
						{
							if (list[list.Count - 1].IndexOf("ZZ") == -1)
							{
								list[0] = list[list.Count - 1] + list[0];
								list.RemoveAt(list.Count - 1);
							}
							for (int j = 0; j < list.Count; j++)
							{
								if (list[j].IndexOf("$") == 0)
								{
									string text = list[j].Split(new string[1] { "ZZ" }, StringSplitOptions.None)[0];
									string[] array = text.Substring(text.IndexOf("RMC,")).Split(',');
									string text2 = "20" + array[9][4] + array[9][5] + array[9][2] + array[9][3] + array[9][0] + array[9][1];
									string text3 = array[1].Split('.')[0];
									string[] value = text.Split('$');
									string contents = string.Join("\r\n$", value).Substring(2);
									File.WriteAllText(folderBrowserDialog.SelectedPath + Path.DirectorySeparatorChar + "NMEA_" + text2 + "_" + text3 + ".txt", contents);
								}
							}
						}
					}
					enableDisableAllButtons(show: true);
					openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
					break;
				}
				case OpenGD77CommsTransferData.CommsAction.BACKUP_CALIBRATION:
				{
					for (int i = 0; i < CalibrationForm.CALIBRATION_HEADER_SIZE; i++)
					{
						if (openGD77CommsTransferData.dataBuff[i] != CalibrationForm.CALIBRATION_HEADER[i])
						{
							SystemSounds.Hand.Play();
							MessageBox.Show(StringsDict["Calibration_data_could_not_be_found._Please_update_your_firmware"]);
							return;
						}
					}
					SaveFileDialog saveFileDialog2 = new SaveFileDialog();
					saveFileDialog2.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastSaveCalibrationLocation", null);
					saveFileDialog2.Filter = StringsDict["Flash_files"] + " (*.bin)|*.bin";
					saveFileDialog2.FilterIndex = 1;
					if (saveFileDialog2.ShowDialog() == DialogResult.OK)
					{
						File.WriteAllBytes(saveFileDialog2.FileName, openGD77CommsTransferData.dataBuff);
						IniFileUtils.WriteProfileString("Setup", "LastSaveCalibrationLocation", Path.GetDirectoryName(saveFileDialog2.FileName));
					}
					enableDisableAllButtons(show: true);
					openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
					break;
				}
				case OpenGD77CommsTransferData.CommsAction.RESTORE_EEPROM:
				case OpenGD77CommsTransferData.CommsAction.RESTORE_FLASH:
				case OpenGD77CommsTransferData.CommsAction.RESTORE_CALIBRATION:
				case OpenGD77CommsTransferData.CommsAction.RESTORE_SETTINGS:
					SystemSounds.Asterisk.Play();
					MessageBox.Show(StringsDict["Restore_complete"]);
					enableDisableAllButtons(show: true);
					openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
					break;
				case OpenGD77CommsTransferData.CommsAction.READ_CODEPLUG:
				{
					SystemSounds.Asterisk.Play();
					MessageBox.Show(StringsDict["Read_Codeplug_complete"]);
					if (!MainForm.checkZonesFor80Channels(openGD77CommsTransferData.dataBuff))
					{
						MainForm.convertTo80ChannelZoneCodeplug(openGD77CommsTransferData.dataBuff);
					}
					MainForm.ByteToData(openGD77CommsTransferData.dataBuff);
					enableDisableAllButtons(show: true);
					int returnedOffsetPos = 0;
					if (FindCustomDataBlock(CustomData, CustomDataType.IMAGE_TYPE, ref returnedOffsetPos))
					{
						loadImageFromCodeplug(returnedOffsetPos + 8);
					}
					if (FindCustomDataBlock(CustomData, CustomDataType.MELODY_TYPE, ref returnedOffsetPos))
					{
						loadMelodyFromCodeplug(returnedOffsetPos + 8);
					}
					if (_initialAction == OpenGD77CommsTransferData.CommsAction.READ_CODEPLUG)
					{
						Close();
					}
					break;
				}
				case OpenGD77CommsTransferData.CommsAction.WRITE_CODEPLUG:
					SystemSounds.Asterisk.Play();
					enableDisableAllButtons(show: true);
					if (_initialAction == OpenGD77CommsTransferData.CommsAction.WRITE_CODEPLUG)
					{
						Close();
					}
					break;
				case OpenGD77CommsTransferData.CommsAction.BACKUP_MCU_ROM:
				{
					SaveFileDialog saveFileDialog5 = new SaveFileDialog();
					saveFileDialog5.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastSaveMCUROMLocation", null);
					saveFileDialog5.Filter = StringsDict["MCU_ROM_files"] + " (*.bin)|*.bin";
					saveFileDialog5.FilterIndex = 1;
					if (saveFileDialog5.ShowDialog() == DialogResult.OK)
					{
						File.WriteAllBytes(saveFileDialog5.FileName, openGD77CommsTransferData.dataBuff);
						IniFileUtils.WriteProfileString("Setup", "LastSaveMCUROMLocation", Path.GetDirectoryName(saveFileDialog5.FileName));
					}
					enableDisableAllButtons(show: true);
					openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
					break;
				}
				case OpenGD77CommsTransferData.CommsAction.DOWNLOAD_SCREENGRAB:
				{
					Bitmap bitmap = ((MainForm.RadioInfo.radioType != 6 && MainForm.RadioInfo.radioType != 8 && MainForm.RadioInfo.radioType != 10 && MainForm.RadioInfo.radioType != 9 && MainForm.RadioInfo.radioType != 7) ? new Bitmap(128, 64) : new Bitmap(160, 128));
					Graphics graphics = Graphics.FromImage(bitmap);
					Color color = ColorTranslator.FromHtml("#99d9ea");
					graphics.Clear(color);
					if (MainForm.RadioInfo.radioType == 6 || MainForm.RadioInfo.radioType == 8 || MainForm.RadioInfo.radioType == 10 || MainForm.RadioInfo.radioType == 9 || MainForm.RadioInfo.radioType == 7)
					{
						int num = 0;
						for (int k = 0; k < 128; k++)
						{
							for (int l = 0; l < 160; l++)
							{
								int num2 = (openGD77CommsTransferData.dataBuff[num] << 8) + openGD77CommsTransferData.dataBuff[num + 1];
								int num3;
								int num4;
								int num5;
								if (MainForm.RadioInfo.radioType == 8)
								{
									num3 = (num2 & 0x1F) << 3;
									num4 = (num2 & 0x7E0) >> 3;
									num5 = (num2 & 0xF800) >> 8;
								}
								else
								{
									num3 = (num2 & 0xF800) >> 8;
									num4 = (num2 & 0x7E0) >> 3;
									num5 = (num2 & 0x1F) << 3;
								}
								if (RadioInfoIsFeatureSet(RadioInfoFeatures.SCREEN_INVERTED))
								{
									num3 = 255 - num3;
									num4 = 255 - num4;
									num5 = 255 - num5;
								}
								bitmap.SetPixel(l, k, Color.FromArgb(num3, num4, num5));
								num += 2;
							}
						}
					}
					else
					{
						if (RadioInfoIsFeatureSet(RadioInfoFeatures.SCREEN_INVERTED))
						{
							graphics.Clear(Color.Black);
						}
						else
						{
							color = Color.Black;
						}
						for (int m = 0; m < 8; m++)
						{
							for (int n = 0; n < 128; n++)
							{
								for (int num6 = 0; num6 < 8; num6++)
								{
									if (((openGD77CommsTransferData.dataBuff[m * 128 + n] >> num6) & 1) != 0)
									{
										bitmap.SetPixel(n, m * 8 + num6, color);
									}
								}
							}
						}
					}
					Bitmap bitmap2 = ResizeImage(bitmap, bitmap.Width * 2, bitmap.Height * 2);
					Clipboard.SetImage(bitmap2);
					if (Control.ModifierKeys != Keys.Shift)
					{
						SaveFileDialog saveFileDialog7 = new SaveFileDialog();
						saveFileDialog7.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastSaveScreengrabLocation", null);
						saveFileDialog7.Filter = StringsDict["Screengrab_files"] + " (*.png)|*.png";
						saveFileDialog7.FilterIndex = 1;
						if (saveFileDialog7.ShowDialog() == DialogResult.OK)
						{
							bitmap2.Save(saveFileDialog7.FileName, ImageFormat.Png);
							IniFileUtils.WriteProfileString("Setup", "LastSaveScreengrabLocation", Path.GetDirectoryName(saveFileDialog7.FileName));
						}
					}
					enableDisableAllButtons(show: true);
					openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
					break;
				}
				case OpenGD77CommsTransferData.CommsAction.COMPRESS_AUDIO:
				{
					SaveFileDialog saveFileDialog6 = new SaveFileDialog();
					saveFileDialog6.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastSaveAMBEFileLocation", null);
					saveFileDialog6.Filter = StringsDict["AMB_files"] + " (*.amb)|*.amb";
					saveFileDialog6.FilterIndex = 1;
					if (saveFileDialog6.ShowDialog() == DialogResult.OK)
					{
						File.WriteAllBytes(saveFileDialog6.FileName, openGD77CommsTransferData.dataBuff);
						IniFileUtils.WriteProfileString("Setup", "LastSaveAMBEFileLocation", Path.GetDirectoryName(saveFileDialog6.FileName));
					}
					enableDisableAllButtons(show: true);
					openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
					break;
				}
				case OpenGD77CommsTransferData.CommsAction.WRITE_VOICE_PROMPTS:
					SystemSounds.Asterisk.Play();
					MessageBox.Show(StringsDict["Upload_complete"]);
					enableDisableAllButtons(show: true);
					openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
					break;
				case OpenGD77CommsTransferData.CommsAction.WRITE_SATELLITE_KEPS:
					SystemSounds.Asterisk.Play();
					MessageBox.Show(satsAdded, StringsDict["Upload_complete"]);
					enableDisableAllButtons(show: true);
					openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
					break;
				case OpenGD77CommsTransferData.CommsAction.BACKUP_SETTINGS:
				{
					SaveFileDialog saveFileDialog3 = new SaveFileDialog();
					saveFileDialog3.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastSaveSettingsLocation", null);
					saveFileDialog3.Filter = "Settings (*.dat)|*.dat";
					saveFileDialog3.FilterIndex = 1;
					if (saveFileDialog3.ShowDialog() == DialogResult.OK)
					{
						File.WriteAllBytes(saveFileDialog3.FileName, openGD77CommsTransferData.dataBuff);
						IniFileUtils.WriteProfileString("Setup", "LastSaveSettingsLocation", Path.GetDirectoryName(saveFileDialog3.FileName));
					}
					enableDisableAllButtons(show: true);
					openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
					break;
				}
				case OpenGD77CommsTransferData.CommsAction.READ_SECURE_REGISTERS:
				{
					SaveFileDialog saveFileDialog = new SaveFileDialog();
					saveFileDialog.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastSaveSettingsLocation", null);
					saveFileDialog.Filter = "Secure Registers (*.bin)|*.bin";
					saveFileDialog.FilterIndex = 1;
					if (saveFileDialog.ShowDialog() == DialogResult.OK)
					{
						File.WriteAllBytes(saveFileDialog.FileName, openGD77CommsTransferData.dataBuff);
						IniFileUtils.WriteProfileString("Setup", "LastSaveSettingsLocation", Path.GetDirectoryName(saveFileDialog.FileName));
					}
					enableDisableAllButtons(show: true);
					openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
					break;
				}
				}
			}
			else
			{
				SystemSounds.Hand.Play();
				MessageBox.Show(StringsDict["There_has_been_an_error._Refer_to_the_last_status_message_that_was_displayed"], StringsDict["Oops"]);
				enableDisableAllButtons(show: true);
			}
		}
		progressBar1.Value = 0;
	}

	private void worker_DoWork(object sender, DoWorkEventArgs e)
	{
		OpenGD77CommsTransferData openGD77CommsTransferData = e.Argument as OpenGD77CommsTransferData;
		int ADDR_OPENGD77_CUSTOM_DATA_START = Settings.ADDR_OPENGD77_CUSTOM_DATA_START;
		if (commPort == null)
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(StringsDict["No_com_port"]);
			enableDisableAllButtons(show: true);
			return;
		}
		try
		{
			switch (openGD77CommsTransferData.action)
			{
			case OpenGD77CommsTransferData.CommsAction.SAVE_NMEA_LOG:
				if (!sendCommand(commPort, 0))
				{
					displayMessage(StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				MainForm.RadioInfo = readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				sendCommand(commPort, 1);
				sendCommand(commPort, 2, 0, 0, 3, 1, 0, StringsDict["RADIO_DISPLAY_CPS"]);
				sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Reading"]);
				sendCommand(commPort, 2, 0, 32, 3, 1, 0, "NMEA");
				sendCommand(commPort, 3);
				sendCommand(commPort, 6, 3);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadFlash;
				switch (MainForm.RadioInfo.flashId)
				{
				case 16405u:
				case 16407u:
					openGD77CommsTransferData.dataBuff = new byte[1048576];
					openGD77CommsTransferData.startDataAddressInTheRadio = 1048576;
					break;
				case 16408u:
				case 28696u:
					openGD77CommsTransferData.dataBuff = new byte[2097152];
					openGD77CommsTransferData.startDataAddressInTheRadio = 14680064;
					break;
				default:
					if (RadioInfoIsFeatureSet(RadioInfoFeatures.VOICE_PROMPTS_AVAILABLE))
					{
						openGD77CommsTransferData.dataBuff = new byte[294912];
						openGD77CommsTransferData.startDataAddressInTheRadio = 753664;
					}
					else
					{
						openGD77CommsTransferData.dataBuff = new byte[851968];
						openGD77CommsTransferData.startDataAddressInTheRadio = 196608;
					}
					break;
				}
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				openGD77CommsTransferData.transferLength = openGD77CommsTransferData.dataBuff.Length;
				displayMessage(StringsDict["Reading_NMEA"]);
				if (!ReadFlashOrEEPROMOrROMOrScreengrab(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Error_while_reading_flash"]);
					openGD77CommsTransferData.responseCode = 1;
				}
				else
				{
					displayMessage("");
				}
				sendCommand(commPort, 5);
				sendCommand(commPort, 7);
				commPort.Close();
				commPort = null;
				break;
			case OpenGD77CommsTransferData.CommsAction.BACKUP_FLASH:
				if (!sendCommand(commPort, 0))
				{
					displayMessage(StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				MainForm.RadioInfo = readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				sendCommand(commPort, 1);
				sendCommand(commPort, 2, 0, 0, 3, 1, 0, StringsDict["RADIO_DISPLAY_CPS"]);
				sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Backup"]);
				sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_Flash"]);
				sendCommand(commPort, 3);
				sendCommand(commPort, 6, 3);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadFlash;
				switch (MainForm.RadioInfo.flashId)
				{
				case 16405u:
					openGD77CommsTransferData.dataBuff = new byte[2097152];
					break;
				case 16407u:
					openGD77CommsTransferData.dataBuff = new byte[8388608];
					break;
				case 16408u:
				case 28696u:
					openGD77CommsTransferData.dataBuff = new byte[16777216];
					break;
				default:
					openGD77CommsTransferData.dataBuff = new byte[1048576];
					break;
				}
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				openGD77CommsTransferData.startDataAddressInTheRadio = 0;
				openGD77CommsTransferData.transferLength = openGD77CommsTransferData.dataBuff.Length;
				displayMessage(StringsDict["Reading_Flash"]);
				if (!ReadFlashOrEEPROMOrROMOrScreengrab(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Error_while_reading_flash"]);
					openGD77CommsTransferData.responseCode = 1;
				}
				else
				{
					displayMessage("");
				}
				sendCommand(commPort, 5);
				sendCommand(commPort, 7);
				commPort.Close();
				commPort = null;
				break;
			case OpenGD77CommsTransferData.CommsAction.BACKUP_CALIBRATION:
				if (!sendCommand(commPort, 0))
				{
					displayMessage(StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				sendCommand(commPort, 1);
				sendCommand(commPort, 2, 0, 0, 3, 1, 0, StringsDict["RADIO_DISPLAY_CPS"]);
				sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Backup"]);
				sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_Calibration"]);
				sendCommand(commPort, 3);
				sendCommand(commPort, 6, 3);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadFlash;
				openGD77CommsTransferData.dataBuff = new byte[CalibrationForm.CALIBRATION_DATA_SIZE];
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				openGD77CommsTransferData.startDataAddressInTheRadio = CalibrationForm.MEMORY_LOCATION;
				openGD77CommsTransferData.transferLength = CalibrationForm.CALIBRATION_DATA_SIZE;
				displayMessage(StringsDict["Reading_Calibration"]);
				if (!ReadFlashOrEEPROMOrROMOrScreengrab(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Error_while_reading_calibration"]);
					openGD77CommsTransferData.responseCode = 1;
				}
				else
				{
					displayMessage("");
				}
				sendCommand(commPort, 5);
				sendCommand(commPort, 7);
				commPort.Close();
				commPort = null;
				break;
			case OpenGD77CommsTransferData.CommsAction.BACKUP_EEPROM:
				if (!sendCommand(commPort, 0))
				{
					displayMessage(StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				sendCommand(commPort, 1);
				sendCommand(commPort, 2, 0, 0, 3, 1, 0, StringsDict["RADIO_DISPLAY_CPS"]);
				sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Backup"]);
				sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_EEPROM"]);
				sendCommand(commPort, 3);
				sendCommand(commPort, 6, 3);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadEEPROM;
				openGD77CommsTransferData.dataBuff = new byte[65536];
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				openGD77CommsTransferData.startDataAddressInTheRadio = 0;
				openGD77CommsTransferData.transferLength = 65536;
				displayMessage(StringsDict["Reading_EEPROM"]);
				if (!ReadFlashOrEEPROMOrROMOrScreengrab(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Error_while_reading_EEPROM"]);
					openGD77CommsTransferData.responseCode = 1;
				}
				else
				{
					displayMessage("");
				}
				sendCommand(commPort, 5);
				sendCommand(commPort, 7);
				commPort.Close();
				commPort = null;
				break;
			case OpenGD77CommsTransferData.CommsAction.RESTORE_FLASH:
				if (!sendCommand(commPort, 0))
				{
					displayMessage(StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				sendCommand(commPort, 1);
				sendCommand(commPort, 2, 0, 0, 3, 1, 0, StringsDict["RADIO_DISPLAY_CPS"]);
				sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Restoring"]);
				sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_Flash"]);
				sendCommand(commPort, 3);
				sendCommand(commPort, 6, 4);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteFlash;
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				openGD77CommsTransferData.startDataAddressInTheRadio = 0;
				openGD77CommsTransferData.transferLength = openGD77CommsTransferData.dataBuff.Length;
				displayMessage(StringsDict["Restoring_Flash"]);
				if (WriteFlash(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Restore_complete"]);
				}
				else
				{
					SystemSounds.Hand.Play();
					MessageBox.Show(StringsDict["Error_while_restoring"]);
					displayMessage(StringsDict["Error_while_restoring"]);
					openGD77CommsTransferData.responseCode = 1;
				}
				sendCommand(commPort, 6, 2);
				sendCommand(commPort, 6, 1);
				commPort.Close();
				commPort = null;
				break;
			case OpenGD77CommsTransferData.CommsAction.RESTORE_CALIBRATION:
				if (!sendCommand(commPort, 0))
				{
					displayMessage(StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				sendCommand(commPort, 1);
				sendCommand(commPort, 2, 0, 0, 3, 1, 0, StringsDict["RADIO_DISPLAY_CPS"]);
				sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Restoring"]);
				sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_Calibration"]);
				sendCommand(commPort, 3);
				sendCommand(commPort, 6, 4);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteFlash;
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				openGD77CommsTransferData.startDataAddressInTheRadio = CalibrationForm.MEMORY_LOCATION;
				openGD77CommsTransferData.transferLength = CalibrationForm.CALIBRATION_DATA_SIZE;
				displayMessage(StringsDict["Restoring_Calibration"]);
				if (WriteFlash(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Restore_complete"]);
				}
				else
				{
					SystemSounds.Hand.Play();
					MessageBox.Show(StringsDict["Error_while_restoring_Calibration"]);
					displayMessage(StringsDict["Error_while_restoring_Calibration"]);
					openGD77CommsTransferData.responseCode = 1;
				}
				sendCommand(commPort, 6, 2);
				sendCommand(commPort, 6, 1);
				commPort.Close();
				commPort = null;
				break;
			case OpenGD77CommsTransferData.CommsAction.RESTORE_EEPROM:
				if (!sendCommand(commPort, 0))
				{
					displayMessage(StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				sendCommand(commPort, 1);
				sendCommand(commPort, 2, 0, 0, 3, 1, 0, StringsDict["RADIO_DISPLAY_CPS"]);
				sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Restoring"]);
				sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_EEPROM"]);
				sendCommand(commPort, 3);
				sendCommand(commPort, 6, 4);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteEEPROM;
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				openGD77CommsTransferData.startDataAddressInTheRadio = 0;
				openGD77CommsTransferData.transferLength = 65536;
				displayMessage(StringsDict["Restoring_EEPROM"]);
				if (WriteEEPROM(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Restore_complete"]);
				}
				else
				{
					SystemSounds.Hand.Play();
					MessageBox.Show(StringsDict["Error_while_restoring"]);
					displayMessage(StringsDict["Error_while_restoring"]);
					openGD77CommsTransferData.responseCode = 1;
				}
				sendCommand(commPort, 6);
				commPort.Close();
				commPort = null;
				break;
			case OpenGD77CommsTransferData.CommsAction.READ_CODEPLUG:
				if (!sendCommand(commPort, 0))
				{
					displayMessage(StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				sendCommand(commPort, 1);
				sendCommand(commPort, 2, 0, 0, 3, 1, 0, StringsDict["RADIO_DISPLAY_CPS"]);
				sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Reading"]);
				sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_Codeplug"]);
				sendCommand(commPort, 3);
				sendCommand(commPort, 6, 3);
				sendCommand(commPort, 6, 2);
				openGD77CommsTransferData.localDataBufferStartPosition = 128;
				openGD77CommsTransferData.startDataAddressInTheRadio = openGD77CommsTransferData.localDataBufferStartPosition;
				if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
				{
					openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadFlash;
				}
				else
				{
					openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadEEPROM;
				}
				openGD77CommsTransferData.transferLength = 24576 - openGD77CommsTransferData.localDataBufferStartPosition;
				displayMessage(string.Format(StringsDict["Reading_EEPROM"] + " 0x{0:X6} - 0x{1:X6}", openGD77CommsTransferData.localDataBufferStartPosition, openGD77CommsTransferData.localDataBufferStartPosition + openGD77CommsTransferData.transferLength));
				if (!ReadFlashOrEEPROMOrROMOrScreengrab(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Error_while_reading"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				openGD77CommsTransferData.localDataBufferStartPosition = Settings.ADDR_OPENGD77_LAST_USED_CHANNELS_DATA_START;
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadEEPROM;
				openGD77CommsTransferData.startDataAddressInTheRadio = openGD77CommsTransferData.localDataBufferStartPosition;
				openGD77CommsTransferData.transferLength = Settings.ADDR_OPENGD77_LAST_USED_CHANNELS_DATA_END - openGD77CommsTransferData.localDataBufferStartPosition;
				displayMessage(string.Format(StringsDict["Reading_EEPROM"] + " 0x{0:X6} - 0x{1:X6}", openGD77CommsTransferData.localDataBufferStartPosition, openGD77CommsTransferData.localDataBufferStartPosition + openGD77CommsTransferData.transferLength));
				if (!ReadFlashOrEEPROMOrROMOrScreengrab(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Error_while_reading"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				openGD77CommsTransferData.localDataBufferStartPosition = 29952;
				openGD77CommsTransferData.startDataAddressInTheRadio = openGD77CommsTransferData.localDataBufferStartPosition;
				if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
				{
					openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadFlash;
				}
				else
				{
					openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadEEPROM;
				}
				openGD77CommsTransferData.transferLength = 45056 - openGD77CommsTransferData.localDataBufferStartPosition;
				displayMessage(string.Format(StringsDict["Reading_EEPROM"] + " 0x{0:X6} - 0x{1:X6}", openGD77CommsTransferData.localDataBufferStartPosition, openGD77CommsTransferData.localDataBufferStartPosition + openGD77CommsTransferData.transferLength));
				if (!ReadFlashOrEEPROMOrROMOrScreengrab(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Error_while_reading"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadFlash;
				openGD77CommsTransferData.localDataBufferStartPosition = 45056;
				if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
				{
					openGD77CommsTransferData.startDataAddressInTheRadio = 503808 + STM32_FLASH_ADDRESS_OFFSET;
				}
				else
				{
					openGD77CommsTransferData.startDataAddressInTheRadio = 503808;
				}
				openGD77CommsTransferData.transferLength = ADDR_OPENGD77_CUSTOM_DATA_START - openGD77CommsTransferData.localDataBufferStartPosition;
				displayMessage(string.Format(StringsDict["Reading_Flash"] + " 0x{0:X6} - 0x{1:X6}", openGD77CommsTransferData.localDataBufferStartPosition, openGD77CommsTransferData.localDataBufferStartPosition + openGD77CommsTransferData.transferLength));
				if (!ReadFlashOrEEPROMOrROMOrScreengrab(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Error_while_reading"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadFlash;
				openGD77CommsTransferData.localDataBufferStartPosition = Settings.ADDR_OPENGD77_CUSTOM_DATA_START;
				openGD77CommsTransferData.startDataAddressInTheRadio = 0;
				if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
				{
					openGD77CommsTransferData.startDataAddressInTheRadio = STM32_FLASH_ADDRESS_OFFSET;
				}
				else
				{
					openGD77CommsTransferData.startDataAddressInTheRadio = 0;
				}
				openGD77CommsTransferData.transferLength = 131072 - Settings.ADDR_OPENGD77_CUSTOM_DATA_START;
				displayMessage(string.Format(StringsDict["Reading_Flash"] + " 0x{0:X6} - 0x{1:X6}", openGD77CommsTransferData.localDataBufferStartPosition, openGD77CommsTransferData.localDataBufferStartPosition + openGD77CommsTransferData.transferLength));
				if (!ReadFlashOrEEPROMOrROMOrScreengrab(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Error_while_reading"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				displayMessage(StringsDict["Codeplug_read_complete"]);
				sendCommand(commPort, 5);
				sendCommand(commPort, 7);
				commPort.Close();
				commPort = null;
				if (MainForm.RadioInfo.radioType == 8)
				{
					convertThemeColours565(ref openGD77CommsTransferData.dataBuff, Settings.ADDR_OPENGD77_CUSTOM_DATA_START, checkForCustomDataHeader: true);
				}
				break;
			case OpenGD77CommsTransferData.CommsAction.WRITE_CODEPLUG:
				if (!sendCommand(commPort, 0))
				{
					displayMessage(StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				sendCommand(commPort, 1);
				sendCommand(commPort, 2, 0, 0, 3, 1, 0, StringsDict["RADIO_DISPLAY_CPS"]);
				sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Writing"]);
				sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_Codeplug"]);
				sendCommand(commPort, 3);
				sendCommand(commPort, 6, 4);
				sendCommand(commPort, 6, 2);
				openGD77CommsTransferData.dataBuff = MainForm.DataToByte();
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteEEPROM;
				openGD77CommsTransferData.localDataBufferStartPosition = 128;
				if (MainForm.RadioInfo.radioType == 8)
				{
					convertThemeColours565(ref openGD77CommsTransferData.dataBuff, Settings.ADDR_OPENGD77_CUSTOM_DATA_START + CUSTOM_DATA_HEADER_SIZE, checkForCustomDataHeader: false);
				}
				openGD77CommsTransferData.transferLength = 24576 - openGD77CommsTransferData.localDataBufferStartPosition;
				openGD77CommsTransferData.startDataAddressInTheRadio = openGD77CommsTransferData.localDataBufferStartPosition;
				displayMessage(string.Format(StringsDict["Writing_EEPROM"] + " 0x{0:X6} - 0x{1:X6}", openGD77CommsTransferData.localDataBufferStartPosition, openGD77CommsTransferData.localDataBufferStartPosition + openGD77CommsTransferData.transferLength));
				if (!((MainForm.RadioType != MainForm.RadioTypeEnum.RadioTypeSTM32) ? WriteEEPROM(commPort, openGD77CommsTransferData) : WriteFlash(commPort, openGD77CommsTransferData)))
				{
					displayMessage(StringsDict["Error_while_writing"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteEEPROM;
				openGD77CommsTransferData.localDataBufferStartPosition = Settings.ADDR_OPENGD77_LAST_USED_CHANNELS_DATA_START;
				openGD77CommsTransferData.transferLength = Settings.ADDR_OPENGD77_LAST_USED_CHANNELS_DATA_END - openGD77CommsTransferData.localDataBufferStartPosition;
				openGD77CommsTransferData.startDataAddressInTheRadio = openGD77CommsTransferData.localDataBufferStartPosition;
				displayMessage(string.Format(StringsDict["Writing_EEPROM"] + " 0x{0:X6} - 0x{1:X6}", openGD77CommsTransferData.localDataBufferStartPosition, openGD77CommsTransferData.localDataBufferStartPosition + openGD77CommsTransferData.transferLength));
				if (!((MainForm.RadioType != MainForm.RadioTypeEnum.RadioTypeSTM32) ? WriteEEPROM(commPort, openGD77CommsTransferData) : WriteFlash(commPort, openGD77CommsTransferData)))
				{
					displayMessage(StringsDict["Error_while_writing"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteEEPROM;
				openGD77CommsTransferData.localDataBufferStartPosition = 29952;
				openGD77CommsTransferData.startDataAddressInTheRadio = openGD77CommsTransferData.localDataBufferStartPosition;
				openGD77CommsTransferData.transferLength = 45056 - openGD77CommsTransferData.localDataBufferStartPosition;
				displayMessage(string.Format(StringsDict["Writing_EEPROM"] + " 0x{0:X6} - 0x{1:X6}", openGD77CommsTransferData.localDataBufferStartPosition, openGD77CommsTransferData.localDataBufferStartPosition + openGD77CommsTransferData.transferLength));
				if (!((MainForm.RadioType != MainForm.RadioTypeEnum.RadioTypeSTM32) ? WriteEEPROM(commPort, openGD77CommsTransferData) : WriteFlash(commPort, openGD77CommsTransferData)))
				{
					displayMessage(StringsDict["Error_while_writing"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteFlash;
				openGD77CommsTransferData.localDataBufferStartPosition = 45056;
				if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
				{
					openGD77CommsTransferData.startDataAddressInTheRadio = 503808 + STM32_FLASH_ADDRESS_OFFSET;
				}
				else
				{
					openGD77CommsTransferData.startDataAddressInTheRadio = 503808;
				}
				openGD77CommsTransferData.transferLength = ADDR_OPENGD77_CUSTOM_DATA_START - openGD77CommsTransferData.localDataBufferStartPosition;
				displayMessage(string.Format(StringsDict["Writing_Flash"] + " 0x{0:X6} - 0x{1:X6}", openGD77CommsTransferData.localDataBufferStartPosition, openGD77CommsTransferData.localDataBufferStartPosition + openGD77CommsTransferData.transferLength));
				if (!WriteFlash(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Error_while_writing"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteFlash;
				openGD77CommsTransferData.localDataBufferStartPosition = Settings.ADDR_OPENGD77_CUSTOM_DATA_START;
				if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
				{
					openGD77CommsTransferData.startDataAddressInTheRadio = STM32_FLASH_ADDRESS_OFFSET;
				}
				else
				{
					openGD77CommsTransferData.startDataAddressInTheRadio = 0;
				}
				openGD77CommsTransferData.transferLength = 131072 - openGD77CommsTransferData.localDataBufferStartPosition;
				displayMessage(string.Format(StringsDict["Writing_Flash"] + " 0x{0:X6} - 0x{1:X6}", openGD77CommsTransferData.localDataBufferStartPosition, openGD77CommsTransferData.localDataBufferStartPosition + openGD77CommsTransferData.transferLength));
				if (!WriteFlash(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Error_while_writing"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
				}
				else
				{
					displayMessage(StringsDict["Codeplug_write_complete"]);
					sendCommand(commPort, 6);
					commPort.Close();
					commPort = null;
				}
				break;
			case OpenGD77CommsTransferData.CommsAction.BACKUP_MCU_ROM:
				if (!sendCommand(commPort, 0))
				{
					displayMessage(StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				sendCommand(commPort, 1);
				sendCommand(commPort, 2, 0, 0, 3, 1, 0, StringsDict["RADIO_DISPLAY_CPS"]);
				sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Backup"]);
				sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_MCU_ROM"]);
				sendCommand(commPort, 3);
				sendCommand(commPort, 6, 3);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadMCUROM;
				if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
				{
					openGD77CommsTransferData.dataBuff = new byte[1048576];
				}
				else
				{
					openGD77CommsTransferData.dataBuff = new byte[524288];
				}
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				openGD77CommsTransferData.startDataAddressInTheRadio = 0;
				openGD77CommsTransferData.transferLength = openGD77CommsTransferData.dataBuff.Length;
				displayMessage(StringsDict["Reading_MCU_ROM"]);
				if (!ReadFlashOrEEPROMOrROMOrScreengrab(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Error_while_Reading_MCU_ROM"]);
					openGD77CommsTransferData.responseCode = 1;
				}
				else
				{
					displayMessage("");
				}
				sendCommand(commPort, 5);
				sendCommand(commPort, 7);
				commPort.Close();
				commPort = null;
				break;
			case OpenGD77CommsTransferData.CommsAction.DOWNLOAD_SCREENGRAB:
				readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort, stealth: true);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadScreenGrab;
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				openGD77CommsTransferData.startDataAddressInTheRadio = 0;
				if (MainForm.RadioInfo.radioType == 6 || MainForm.RadioInfo.radioType == 8 || MainForm.RadioInfo.radioType == 10 || MainForm.RadioInfo.radioType == 9 || MainForm.RadioInfo.radioType == 7)
				{
					openGD77CommsTransferData.transferLength = 40960;
				}
				else
				{
					openGD77CommsTransferData.transferLength = 1024;
				}
				openGD77CommsTransferData.dataBuff = new byte[openGD77CommsTransferData.transferLength];
				displayMessage(StringsDict["Downloading_Screengrab"]);
				if (!ReadFlashOrEEPROMOrROMOrScreengrab(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Error_while_downloading_Screengrab"]);
					openGD77CommsTransferData.responseCode = 1;
				}
				else
				{
					displayMessage("");
				}
				sendCommand(commPort, 7);
				commPort.Close();
				commPort = null;
				break;
			case OpenGD77CommsTransferData.CommsAction.COMPRESS_AUDIO:
				sendCommand(commPort, 0);
				readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				CompressWAV(commPort, openGD77CommsTransferData);
				sendCommand(commPort, 5);
				sendCommand(commPort, 7);
				commPort.Close();
				commPort = null;
				break;
			case OpenGD77CommsTransferData.CommsAction.WRITE_VOICE_PROMPTS:
				if (!sendCommand(commPort, 0))
				{
					displayMessage(StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				sendCommand(commPort, 1);
				sendCommand(commPort, 2, 0, 0, 3, 1, 0, StringsDict["RADIO_DISPLAY_CPS"]);
				sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Writing"]);
				sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_Voice_prompts"]);
				sendCommand(commPort, 3);
				sendCommand(commPort, 6, 4);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteFlash;
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
				{
					openGD77CommsTransferData.startDataAddressInTheRadio = 586752 + STM32_FLASH_ADDRESS_OFFSET;
				}
				else
				{
					openGD77CommsTransferData.startDataAddressInTheRadio = 586752;
				}
				openGD77CommsTransferData.transferLength = openGD77CommsTransferData.dataBuff.Length;
				displayMessage(StringsDict["Writing_Voice_prompts"]);
				if (WriteFlash(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Upload_complete"]);
				}
				else
				{
					SystemSounds.Hand.Play();
					MessageBox.Show(StringsDict["Error_while_writing"]);
					displayMessage(StringsDict["Error_while_writing"]);
					openGD77CommsTransferData.responseCode = 1;
				}
				sendCommand(commPort, 6, 2);
				sendCommand(commPort, 6, 1);
				commPort.Close();
				commPort = null;
				break;
			case OpenGD77CommsTransferData.CommsAction.WRITE_SATELLITE_KEPS:
			{
				if (!sendCommand(commPort, 0))
				{
					displayMessage(StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				sendCommand(commPort, 1);
				sendCommand(commPort, 2, 0, 0, 3, 1, 0, StringsDict["RADIO_DISPLAY_CPS"]);
				sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Writing"]);
				sendCommand(commPort, 2, 0, 32, 3, 1, 0, "спутников");
				sendCommand(commPort, 3);
				sendCommand(commPort, 6, 4);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteFlash;
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				openGD77CommsTransferData.transferLength = openGD77CommsTransferData.dataBuff.Length;
				displayMessage("Запись данных спутников");
				if (MainForm.RadioInfo.radioType == 8)
				{
					convertThemeColours565(ref openGD77CommsTransferData.dataBuff, CUSTOM_DATA_HEADER_SIZE, checkForCustomDataHeader: false);
				}
				if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
				{
					openGD77CommsTransferData.startDataAddressInTheRadio = STM32_FLASH_ADDRESS_OFFSET;
				}
				else
				{
					openGD77CommsTransferData.startDataAddressInTheRadio = 0;
				}
				if (WriteFlash(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Upload_complete"]);
					sendCommand(commPort, 6, 3);
				}
				else
				{
					SystemSounds.Hand.Play();
					MessageBox.Show(StringsDict["Error_while_writing"]);
					displayMessage(StringsDict["Error_while_writing"]);
					openGD77CommsTransferData.responseCode = 1;
				}
				int num = 100;
				byte[] array = new byte[1032];
				array[0] = 67;
				array[1] = 6;
				array[2] = 7;
				Array.Copy(BitConverter.GetBytes((uint)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds), 0, array, 3, 4);
				commPort.Write(array, 0, 32);
				while (commPort.BytesToWrite > 0)
				{
					Thread.Sleep(1);
				}
				while (commPort.BytesToRead == 0 && num-- > 0)
				{
					Thread.Sleep(5);
				}
				if (num != -1)
				{
					commPort.Read(array, 0, commPort.BytesToRead);
				}
				sendCommand(commPort, 5);
				sendCommand(commPort, 7);
				commPort.Close();
				commPort = null;
				break;
			}
			case OpenGD77CommsTransferData.CommsAction.BACKUP_SETTINGS:
				if (!sendCommand(commPort, 0))
				{
					displayMessage(StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				sendCommand(commPort, 1);
				sendCommand(commPort, 2, 0, 0, 3, 1, 0, StringsDict["RADIO_DISPLAY_CPS"]);
				sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Backup"]);
				sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_EEPROM"]);
				sendCommand(commPort, 3);
				sendCommand(commPort, 6, 2);
				sendCommand(commPort, 6, 10);
				sendCommand(commPort, 6, 3);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadEEPROM;
				openGD77CommsTransferData.dataBuff = new byte[128];
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				openGD77CommsTransferData.startDataAddressInTheRadio = Settings.ADDR_OPENGD77_SETTINGS_START;
				openGD77CommsTransferData.transferLength = openGD77CommsTransferData.dataBuff.Length;
				displayMessage("Reading settings");
				if (!ReadFlashOrEEPROMOrROMOrScreengrab(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Error_while_reading_EEPROM"]);
					openGD77CommsTransferData.responseCode = 1;
				}
				else
				{
					displayMessage("");
				}
				sendCommand(commPort, 5);
				sendCommand(commPort, 7);
				commPort.Close();
				commPort = null;
				break;
			case OpenGD77CommsTransferData.CommsAction.RESTORE_SETTINGS:
				if (!sendCommand(commPort, 0))
				{
					displayMessage(StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				sendCommand(commPort, 1);
				sendCommand(commPort, 2, 0, 0, 3, 1, 0, StringsDict["RADIO_DISPLAY_CPS"]);
				sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Restoring"]);
				sendCommand(commPort, 2, 0, 32, 3, 1, 0, "Settings");
				sendCommand(commPort, 3);
				sendCommand(commPort, 6, 4);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteEEPROM;
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				openGD77CommsTransferData.startDataAddressInTheRadio = Settings.ADDR_OPENGD77_SETTINGS_START;
				openGD77CommsTransferData.transferLength = 128;
				displayMessage("Restoring settings");
				if (WriteEEPROM(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Restore_complete"]);
				}
				else
				{
					SystemSounds.Hand.Play();
					MessageBox.Show(StringsDict["Error_while_restoring"]);
					displayMessage(StringsDict["Error_while_restoring"]);
					openGD77CommsTransferData.responseCode = 1;
				}
				sendCommand(commPort, 6, 10);
				sendCommand(commPort, 6, 1);
				commPort.Close();
				commPort = null;
				break;
			case OpenGD77CommsTransferData.CommsAction.READ_SECURE_REGISTERS:
				if (!sendCommand(commPort, 0))
				{
					displayMessage(StringsDict["Error_connecting_to_the_radio"]);
					openGD77CommsTransferData.responseCode = 1;
					commPort = null;
					break;
				}
				readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
				sendCommand(commPort, 1);
				sendCommand(commPort, 2, 0, 0, 3, 1, 0, StringsDict["RADIO_DISPLAY_CPS"]);
				sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Backup"]);
				sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_Calibration"]);
				sendCommand(commPort, 3);
				sendCommand(commPort, 6, 3);
				openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadSecureRegisters;
				openGD77CommsTransferData.dataBuff = new byte[768];
				openGD77CommsTransferData.localDataBufferStartPosition = 0;
				openGD77CommsTransferData.startDataAddressInTheRadio = 0;
				openGD77CommsTransferData.transferLength = openGD77CommsTransferData.dataBuff.Length;
				displayMessage(StringsDict["Reading_Calibration"]);
				if (!ReadFlashOrEEPROMOrROMOrScreengrab(commPort, openGD77CommsTransferData))
				{
					displayMessage(StringsDict["Error_while_reading_calibration"]);
					openGD77CommsTransferData.responseCode = 1;
				}
				else
				{
					displayMessage("");
				}
				sendCommand(commPort, 5);
				sendCommand(commPort, 7);
				commPort.Close();
				commPort = null;
				break;
			case OpenGD77CommsTransferData.CommsAction.READ_SETTINGS:
			case OpenGD77CommsTransferData.CommsAction.READ_THEME:
			case OpenGD77CommsTransferData.CommsAction.WRITE_THEME:
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

	private bool setupCommPort(bool startup = false)
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
			if (text == null && !startup)
			{
				CommPortSelector commPortSelector = new CommPortSelector();
				if (DialogResult.OK != commPortSelector.ShowDialog())
				{
					return false;
				}
				text = SetupDiWrap.ComPortNameFromFriendlyNamePrefix(commPortSelector.SelectedPort);
				IniFileUtils.WriteProfileString("Setup", "LastCommPort", text);
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
			if (!startup)
			{
                SystemSounds.Hand.Play();
                MessageBox.Show(StringsDict["Failed_to_open_comm_port"], StringsDict["Error"]);
			}
			IniFileUtils.WriteProfileString("Setup", "LastCommPort", "");
			return false;
		}
		try
		{
			commPort.Open();
		}
		catch (Exception)
		{
			if (!startup)
			{
				SystemSounds.Hand.Play();
				MessageBox.Show(StringsDict["Comm_port_not_available"]);
			}
			commPort = null;
			return false;
		}
		return true;
	}

	private void btnBackupEEPROM_Click(object sender, EventArgs e)
	{
		if (probeRadioModel() && MainForm.RadioType != MainForm.RadioTypeEnum.RadioTypeSTM32)
		{
			if (!setupCommPort())
			{
				SystemSounds.Hand.Play();
				MessageBox.Show(StringsDict["No_com_port"]);
			}
			else
			{
				OpenGD77CommsTransferData dataObj = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.BACKUP_EEPROM);
				enableDisableAllButtons(show: false);
				perFormCommsTask(dataObj);
			}
		}
	}

	private void btnBackupFlash_Click(object sender, EventArgs e)
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
				OpenGD77CommsTransferData dataObj = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.BACKUP_FLASH);
				enableDisableAllButtons(show: false);
				perFormCommsTask(dataObj);
			}
		}
	}

	private void btnSaveNMEA_Click(object sender, EventArgs e)
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
				OpenGD77CommsTransferData dataObj = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.SAVE_NMEA_LOG);
				enableDisableAllButtons(show: false);
				perFormCommsTask(dataObj);
			}
		}
	}

	private void btnBackupCalibration_Click(object sender, EventArgs e)
	{
		if (probeRadioModel() && MainForm.RadioType != MainForm.RadioTypeEnum.RadioTypeSTM32)
		{
			if (!setupCommPort())
			{
				SystemSounds.Hand.Play();
				MessageBox.Show(StringsDict["No_com_port"]);
			}
			else
			{
				OpenGD77CommsTransferData dataObj = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.BACKUP_CALIBRATION);
				enableDisableAllButtons(show: false);
				perFormCommsTask(dataObj);
			}
		}
	}

	private bool arrayCompare(byte[] buf1, byte[] buf2)
	{
		int num = Math.Min(buf1.Length, buf2.Length);
		for (int i = 0; i < num; i++)
		{
			if (buf1[i] != buf2[i])
			{
				return false;
			}
		}
		return true;
	}

	private void btnRestoreEEPROM_Click(object sender, EventArgs e)
	{
		if (!probeRadioModel() || MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
		{
			return;
		}
		if (!setupCommPort())
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(StringsDict["No_com_port"]);
		}
		else
		{
			if (DialogResult.Yes != MessageBox.Show(StringsDict["Are_you_sure_you_want_to_restore_the_EEPROM_from_a_previously_saved_file"], StringsDict["Warning"], MessageBoxButtons.YesNo))
			{
				return;
			}
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastSaveEEPROMLocation", null);
			if (DialogResult.OK == openFileDialog.ShowDialog())
			{
				OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.RESTORE_EEPROM);
				openGD77CommsTransferData.dataBuff = File.ReadAllBytes(openFileDialog.FileName);
				IniFileUtils.WriteProfileString("Setup", "LastSaveEEPROMLocation", Path.GetDirectoryName(openFileDialog.FileName));
				if (openGD77CommsTransferData.dataBuff.Length == 65536)
				{
					enableDisableAllButtons(show: false);
					perFormCommsTask(openGD77CommsTransferData);
				}
				else
				{
					SystemSounds.Hand.Play();
					MessageBox.Show(StringsDict["The_file_is_not_the_correct_size."], StringsDict["Error"]);
				}
			}
		}
	}

	private void btnRestoreCalibration_Click(object sender, EventArgs e)
	{
		if (!probeRadioModel() || MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
		{
			return;
		}
		if (!setupCommPort())
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(StringsDict["No_com_port"]);
		}
		else
		{
			if (DialogResult.Yes != MessageBox.Show(StringsDict["Are_you_sure_you_want_to_restore_the_Calibartion_from_a_previously_saved_file"], StringsDict["Warning"], MessageBoxButtons.YesNo))
			{
				return;
			}
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastSaveCalibrationLocation", null);
			openFileDialog.Filter = StringsDict["Calibration_files"] + " (*.bin)|*.bin";
			if (DialogResult.OK != openFileDialog.ShowDialog())
			{
				return;
			}
			OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.RESTORE_CALIBRATION);
			openGD77CommsTransferData.dataBuff = File.ReadAllBytes(openFileDialog.FileName);
			IniFileUtils.WriteProfileString("Setup", "LastSaveCalibrationLocation", Path.GetDirectoryName(openFileDialog.FileName));
			if (openGD77CommsTransferData.dataBuff.Length == CalibrationForm.CALIBRATION_DATA_SIZE)
			{
				for (int i = 0; i < CalibrationForm.CALIBRATION_HEADER_SIZE; i++)
				{
					if (openGD77CommsTransferData.dataBuff[i] != CalibrationForm.CALIBRATION_HEADER[i])
					{
						SystemSounds.Hand.Play();
						MessageBox.Show(StringsDict["Invalid_Calibration_data"]);
						return;
					}
				}
				enableDisableAllButtons(show: false);
				perFormCommsTask(openGD77CommsTransferData);
			}
			else
			{
				SystemSounds.Hand.Play();
				MessageBox.Show(StringsDict["The_file_is_not_the_correct_size."], StringsDict["Error"]);
			}
		}
	}

	private void btnRestoreFlash_Click(object sender, EventArgs e)
	{
		if (!probeRadioModel())
		{
			return;
		}
		if (!setupCommPort())
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(StringsDict["No_com_port"]);
		}
		else
		{
			if (DialogResult.Yes != MessageBox.Show(StringsDict["Are_you_sure_you_want_to_restore_the_Flash_memory_from_a_previously_saved_file"], StringsDict["Warning"], MessageBoxButtons.YesNo))
			{
				return;
			}
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastSaveFlashLocation", null);
			openFileDialog.Filter = StringsDict["Flash_backup_files"] + " (*.bin)|*.bin";
			if (DialogResult.OK == openFileDialog.ShowDialog())
			{
				OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.RESTORE_FLASH);
				openGD77CommsTransferData.dataBuff = File.ReadAllBytes(openFileDialog.FileName);
				IniFileUtils.WriteProfileString("Setup", "LastSaveFlashLocation", Path.GetDirectoryName(openFileDialog.FileName));
				if (openGD77CommsTransferData.dataBuff.Length >= 1048576 || DialogResult.No != MessageBox.Show(StringsDict["This_file_is_not_a_full_backup"], StringsDict["Warning"], MessageBoxButtons.YesNo))
				{
					enableDisableAllButtons(show: false);
					perFormCommsTask(openGD77CommsTransferData);
				}
			}
		}
	}

	private void btnReadSecureRegisters_Click(object sender, EventArgs e)
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
				OpenGD77CommsTransferData dataObj = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.READ_SECURE_REGISTERS);
				enableDisableAllButtons(show: false);
				perFormCommsTask(dataObj);
			}
		}
	}

	private void enableDisableAllButtons(bool show)
	{
		btnBackupEEPROM.Enabled = show;
		btnBackupFlash.Enabled = show;
		btnRestoreEEPROM.Enabled = show;
		btnRestoreFlash.Enabled = show;
		btnResetSettings.Enabled = show;
		btnReadCodeplug.Enabled = show;
		btnWriteCodeplug.Enabled = show;
		btnBackupCalibration.Enabled = show;
		btnRestoreCalibration.Enabled = show;
		btnOpenFile.Enabled = show;
		btnBackupMCUROM.Enabled = show;
		btnDownloadScreenGrab.Enabled = show;
		btnPlayTune.Enabled = show;
		txtBootTune.Enabled = show;
		btnWriteVoicePrompts.Enabled = show;
		btnClearVoicePrompts.Enabled = show;
		txtKepsServer.Enabled = show;
		btnDownloadSatelliteKeps.Enabled = show;
		btnBackupSettings.Enabled = show;
		btnRestoreSettings.Enabled = show;
		btnSaveNMEA.Enabled = show;
		btnReadSecureRegisters.Enabled = show;
	}

	private void OpenGD77Form_Load(object sender, EventArgs e)
	{
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		Settings.ReadCommonsForSectionIntoDictionary(StringsDict, "OpenGD77Form");
		switch (_initialAction)
		{
		case OpenGD77CommsTransferData.CommsAction.READ_CODEPLUG:
			if (probeRadioModel())
			{
				readCodeplug();
			}
			break;
		case OpenGD77CommsTransferData.CommsAction.WRITE_CODEPLUG:
			if (probeRadioModel())
			{
				writeCodeplug();
			}
			break;
		}
	}

	private void OpenGD77Form_FormClosed(object sender, FormClosedEventArgs e)
	{
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

	private void readCodeplug()
	{
		if (!setupCommPort())
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(StringsDict["No_com_port"]);
			return;
		}
		OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.READ_CODEPLUG);
		openGD77CommsTransferData.dataBuff = MainForm.DataToByte();
		enableDisableAllButtons(show: false);
		perFormCommsTask(openGD77CommsTransferData);
	}

	private void btnReadCodeplug_Click(object sender, EventArgs e)
	{
		if (probeRadioModel())
		{
			readCodeplug();
		}
	}

	private void writeCodeplug()
	{
		if (!setupCommPort())
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(StringsDict["No_com_port"]);
			return;
		}
		melodyToBytes(playTune: false);
		OpenGD77CommsTransferData dataObj = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.WRITE_CODEPLUG);
		enableDisableAllButtons(show: false);
		perFormCommsTask(dataObj);
	}

	private void btnWriteCodeplug_Click(object sender, EventArgs e)
	{
		if (probeRadioModel())
		{
			writeCodeplug();
		}
	}

    private void btnResetSettings_Click(object sender, EventArgs e)
    {
        if (probeRadioModel())
        {
            if (!setupCommPort())
            {
                SystemSounds.Hand.Play();
                return;
            }
			if (int.Parse(MainForm.RadioInfo.buildDateTime.Substring(0, 4)) < 2025)
				MessageBox.Show("Сброс настроек через CPS не поддерживается на этой версии прошивки!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			else
			{
                sendCommand(commPort, 77);
				sendCommand(commPort, 6);
                MessageBox.Show("Сброс настроек выполнен!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            commPort.Close();
            commPort = null;
        }
    }

    private void btnBackupMCUROM_Click(object sender, EventArgs e)
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
				OpenGD77CommsTransferData dataObj = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.BACKUP_MCU_ROM);
				enableDisableAllButtons(show: false);
				perFormCommsTask(dataObj);
			}
		}
	}

	private void btnDownloadScreenGrab_Click(object sender, EventArgs e)
	{
		if (probeRadioModel(stealth: true))
		{
			if (!setupCommPort())
			{
				SystemSounds.Hand.Play();
				MessageBox.Show(StringsDict["No_com_port"]);
			}
			else
			{
				OpenGD77CommsTransferData dataObj = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.DOWNLOAD_SCREENGRAB);
				enableDisableAllButtons(show: false);
				perFormCommsTask(dataObj);
			}
		}
	}

	private Bitmap ResizeImage(Image image, int width, int height)
	{
		Bitmap bitmap = new Bitmap(width, height);
		bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
		using Graphics graphics = Graphics.FromImage(bitmap);
		graphics.CompositingMode = CompositingMode.SourceCopy;
		graphics.CompositingQuality = CompositingQuality.AssumeLinear;
		graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
		graphics.SmoothingMode = SmoothingMode.None;
		graphics.PixelOffsetMode = PixelOffsetMode.Half;
		graphics.DrawImage(image, 0, 0, width, height);
		return bitmap;
	}

	private void btnOpenFile_Click(object sender, EventArgs e)
	{
		byte[] array = new byte[1024];
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = StringsDict["PNG_image_files"] + " (*.png)|*.png";
		if (DialogResult.OK != openFileDialog.ShowDialog())
		{
			return;
		}
		BootItemForm.data.BootScreenType = 0;
		Bitmap image = new Bitmap(openFileDialog.FileName);
		Bitmap bitmap = ResizeImage(image, 128, 64);
		Bitmap bitmap2 = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format1bppIndexed);
		pictureBox1.Image = bitmap2;
		for (int i = 0; i < 64; i++)
		{
			int num = i / 8 * 128;
			int num2 = i % 8;
			for (int j = 0; j < 128; j++)
			{
				if (bitmap2.GetPixel(j, i).R == 0)
				{
					array[num + j] += (byte)(1 << num2);
				}
			}
		}
		int returnedOffsetPos = 0;
		bool flag = false;
		if (FindCustomDataBlock(CustomData, CustomDataType.IMAGE_TYPE, ref returnedOffsetPos))
		{
			flag = true;
		}
		else if (FindCustomDataBlock(CustomData, CustomDataType.UNINITIALISED_TYPE, ref returnedOffsetPos, 1024))
		{
			flag = true;
		}
		if (flag)
		{
			CustomData[returnedOffsetPos] = 1;
			CustomData[returnedOffsetPos + 1] = 0;
			CustomData[returnedOffsetPos + 2] = 0;
			CustomData[returnedOffsetPos + 3] = 0;
			CustomData[returnedOffsetPos + 4] = 0;
			CustomData[returnedOffsetPos + 5] = 4;
			CustomData[returnedOffsetPos + 6] = 0;
			CustomData[returnedOffsetPos + 7] = 0;
			Array.Copy(array, 0, CustomData, returnedOffsetPos + 8, 1024);
		}
	}

	private void melodyToBytes(bool playTune)
	{
		byte[] array = new byte[520];
		array[0] = 2;
		array[1] = 0;
		array[2] = 0;
		array[3] = 0;
		array[4] = 0;
		array[5] = 2;
		array[6] = 0;
		array[7] = 0;
		string[] array2 = txtBootTune.Text.Replace(" ", "").Replace("\r\n", "").Split(',');
		if (array2.Length % 2 != 0)
		{
			return;
		}
		int i = 0;
		try
		{
			for (; i < array2.Length - 1; i += 2)
			{
				int num = (int)Math.Round(98.0 * Math.Pow(2.0, float.Parse(array2[i]) / 12f));
				if (num == 98)
				{
					num = 37;
				}
				int num2 = int.Parse(array2[i + 1]);
				if (playTune)
				{
					Console.Beep(num, num2 * 35);
				}
				array[i + 8] = (byte)int.Parse(array2[i]);
				array[i + 8 + 1] = (byte)num2;
			}
			array[i + 8] = 0;
			array[i + 8 + 1] = 0;
			int returnedOffsetPos = 0;
			if (FindCustomDataBlock(CustomData, CustomDataType.MELODY_TYPE, ref returnedOffsetPos))
			{
				Array.Copy(array, 0, CustomData, returnedOffsetPos, array.Length);
			}
			else if (FindCustomDataBlock(CustomData, CustomDataType.UNINITIALISED_TYPE, ref returnedOffsetPos, 512))
			{
				Array.Copy(array, 0, CustomData, returnedOffsetPos, array.Length);
			}
		}
		catch (Exception)
		{
		}
	}

	private void btnPlayTune_Click(object sender, EventArgs e)
	{
		melodyToBytes(playTune: true);
	}

	private void btnCompressAudio_Click(object sender, EventArgs e)
	{
		if (!setupCommPort())
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(StringsDict["No_com_port"]);
			return;
		}
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastSaveAMBELocation", null);
		if (DialogResult.OK == openFileDialog.ShowDialog())
		{
			OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.COMPRESS_AUDIO);
			openGD77CommsTransferData.dataBuff = File.ReadAllBytes(openFileDialog.FileName);
			enableDisableAllButtons(show: false);
			perFormCommsTask(openGD77CommsTransferData);
		}
	}

	private void btnWriteVoicePrompts_Click(object sender, EventArgs e)
	{
		if (!probeRadioModel())
		{
			return;
		}
		if (!setupCommPort())
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(StringsDict["No_com_port"]);
			return;
		}
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = StringsDict["Voice_prompt_files"] + " (*.vpr)|*.vpr";
		openFileDialog.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastVoicePromptLocation", null);
		if (DialogResult.OK == openFileDialog.ShowDialog())
		{
			OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.WRITE_VOICE_PROMPTS);
            openGD77CommsTransferData.dataBuff = File.ReadAllBytes(openFileDialog.FileName);
			IniFileUtils.WriteProfileString("Setup", "LastVoicePromptLocation", Path.GetDirectoryName(openFileDialog.FileName));
			if (openGD77CommsTransferData.dataBuff.Length > 166912)
				{
					MessageBox.Show(StringsDict["Voice_prompts_file_is_too_large"], StringsDict["Error"], MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
			else
			{
                byte[] dataBuff = new byte[166912];
				for (int i = 0; i < 166912; i++)
					if (i< openGD77CommsTransferData.dataBuff.Length)
						dataBuff[i] = openGD77CommsTransferData.dataBuff[i];
				else
                        dataBuff[i] = byte.MaxValue;
				openGD77CommsTransferData.dataBuff = dataBuff;
            }
			perFormCommsTask(openGD77CommsTransferData);
            enableDisableAllButtons(show: false);
        }
	}

	private void btnClearVoicePrompts_Click(object sender, EventArgs e)
	{
		if (probeRadioModel())
		{
			if (!setupCommPort())
			{
				SystemSounds.Hand.Play();
				MessageBox.Show(StringsDict["No_com_port"]);
				return;
			}
			OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.WRITE_VOICE_PROMPTS);
			byte[] dataBuff = Enumerable.Repeat(byte.MaxValue, 4).ToArray();
			openGD77CommsTransferData.dataBuff = dataBuff;
			enableDisableAllButtons(show: false);
			perFormCommsTask(openGD77CommsTransferData);
		}
	}

	private void OpenGD77Form_FormClosing(object sender, FormClosingEventArgs e)
	{
		melodyToBytes(playTune: false);
    }

	private static bool ReadRadioInfo(SerialPort port, OpenGD77CommsTransferData dataObj)
	{
		byte[] array = new byte[1032];
		byte[] array2 = new byte[1032];
		int num = 0;
		array[0] = 82;
		array[1] = (byte)dataObj.mode;
		array[2] = 0;
		array[3] = 0;
		array[4] = 0;
		array[5] = 0;
		array[6] = 0;
		array[7] = 0;
		port.Write(array, 0, 8);
		while (port.BytesToWrite > 0)
		{
			Thread.Sleep(1);
		}
		while (port.BytesToRead == 0)
		{
			Thread.Sleep(5);
		}
		port.Read(array2, 0, port.BytesToRead);
		if (array2[0] == 82)
		{
			int num2 = (array2[1] << 8) + array2[2];
			for (int i = 0; i < num2; i++)
			{
				dataObj.dataBuff[num++] = array2[i + 3];
			}
			return true;
		}
		//Console.WriteLine($"read stopped (error at {0:X8})");
		return false;
	}

	public static RadioInfo readOpenGD77RadioInfo(SerialPort port, bool stealth = false)
	{
		if (!stealth)
		{
            var currentYear = DateTime.Now.Year;
            sendCommand(port, 0);
			sendCommand(port, 1);
			sendCommand(port, 2, 0, 0, 3, 1, 0, "CPS");
			sendCommand(port, 2, 0, 16, 3, 1, 0, "OpenGD77 RUS");
			sendCommand(port, 2, 0, 32, 3, 1, 0, "(c) 2024 - " + currentYear.ToString());
			sendCommand(port, 2, 0, 48, 3, 1, 0, "");
			sendCommand(port, 3);
			sendCommand(port, 6, 4);
		}
		OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData();
		openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadRadioInfo;
		openGD77CommsTransferData.localDataBufferStartPosition = 0;
		openGD77CommsTransferData.transferLength = 0;
		openGD77CommsTransferData.dataBuff = new byte[128];
		MainForm.RadioInfo = default(RadioInfo);
		if (ReadRadioInfo(port, openGD77CommsTransferData))
		{
			MainForm.RadioInfo = ByteArrayToRadioInfo(openGD77CommsTransferData.dataBuff);
			if (MainForm.RadioInfo.structVersion < 3)
			{
				if (MainForm.RadioInfo.structVersion == 1)
				{
					MainForm.RadioInfo.features = 0;
				}
				else
				{
					_ = MainForm.RadioInfo.structVersion;
					_ = 2;
				}
				if (MainForm.RadioInfo.structVersion < 3)
				{
					MainForm.RadioInfo.features |= 4;
				}
			}
		}
		if (!stealth)
		{
			sendCommand(port, 5);
		}
		return MainForm.RadioInfo;
	}

	private static RadioInfo ByteArrayToRadioInfo(byte[] bytes)
	{
		GCHandle gCHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
		try
		{
			return (RadioInfo)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(RadioInfo));
		}
		finally
		{
			gCHandle.Free();
		}
	}

	private void btnDownloadSatelliteKeps_Click(object sender, EventArgs e)
	{
		string text = Application.StartupPath + Path.DirectorySeparatorChar + "satellites.txt";
		string text2 = IniFileUtils.getProfileStringWithDefault("Setup", "SatelliteTxtFile" + MainForm.PRODUCT_VERSION, text);
		if (!File.Exists(text2))
		{
			text2 = text;
		}
		Keys modifierKeys = Control.ModifierKeys;
		if ((modifierKeys & Keys.Control) == Keys.Control)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Satellite txt files (satellites.txt)|satellites.txt";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				text2 = openFileDialog.FileName;
				IniFileUtils.WriteProfileString("Setup", "SatelliteTxtFile" + MainForm.PRODUCT_VERSION, text2);
			}
			return;
		}
		if (!File.Exists(text2))
		{
			MessageBox.Show("ERROR :-( Satellites.txt missing", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		loadSatellitesTxtData(text2);
		if (satConstData.Count == 0)
		{
			MessageBox.Show("ERROR :-( No data in Satellites.txt", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		if ((modifierKeys & Keys.Shift) == Keys.Shift)
		{
			OpenFileDialog openFileDialog2 = new OpenFileDialog();
			openFileDialog2.Filter = "Text files (*.txt)|*.txt";
			if (DialogResult.OK == openFileDialog2.ShowDialog())
			{
				_kepsTxt = File.ReadAllText(openFileDialog2.FileName);
				importKeps(null, null);
				return;
			}
		}
		enableDisableAllButtons(show: false);
		_wc = new WebClient();
		try
		{
			Cursor.Current = Cursors.WaitCursor;
			Refresh();
			Application.DoEvents();
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			_wc.DownloadStringCompleted += downloadKepsCompleteHandler;
			_wc.DownloadStringAsync(new Uri(txtKepsServer.Text));
			Cursor.Current = Cursors.WaitCursor;
		}
		catch (Exception)
		{
			Cursor.Current = Cursors.Default;
			MessageBox.Show(Settings.dicCommon["UnableDownloadFromInternet"]);
			enableDisableAllButtons(show: true);
		}
	}

	private void btnBackupSettings_Click(object sender, EventArgs e)
	{
		if (probeRadioModel() && MainForm.RadioType != MainForm.RadioTypeEnum.RadioTypeSTM32)
		{
			if (!setupCommPort())
			{
				SystemSounds.Hand.Play();
				MessageBox.Show(StringsDict["No_com_port"]);
			}
			else
			{
				OpenGD77CommsTransferData dataObj = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.BACKUP_SETTINGS);
				enableDisableAllButtons(show: false);
				perFormCommsTask(dataObj);
			}
		}
	}

	private void btnRestoreSettings_Click(object sender, EventArgs e)
	{
		if (!probeRadioModel() || MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
		{
			return;
		}
		if (!setupCommPort())
		{
			SystemSounds.Hand.Play();
			MessageBox.Show(StringsDict["No_com_port"]);
		}
		else
		{
			if (DialogResult.Yes != MessageBox.Show(StringsDict["Are_you_sure_you_want_to_restore_the_EEPROM_from_a_previously_saved_file"], StringsDict["Warning"], MessageBoxButtons.YesNo))
			{
				return;
			}
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastSaveEEPROMLocation", null);
			if (DialogResult.OK == openFileDialog.ShowDialog())
			{
				OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.RESTORE_SETTINGS);
				openGD77CommsTransferData.dataBuff = File.ReadAllBytes(openFileDialog.FileName);
				IniFileUtils.WriteProfileString("Setup", "LastSaveEEPROMLocation", Path.GetDirectoryName(openFileDialog.FileName));
				if (openGD77CommsTransferData.dataBuff.Length == 128)
				{
					enableDisableAllButtons(show: false);
					perFormCommsTask(openGD77CommsTransferData);
				}
				else
				{
					SystemSounds.Hand.Play();
					MessageBox.Show(StringsDict["The_file_is_not_the_correct_size."], StringsDict["Error"]);
				}
			}
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
            this.btnBackupEEPROM = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnBackupFlash = new System.Windows.Forms.Button();
            this.btnRestoreEEPROM = new System.Windows.Forms.Button();
            this.btnRestoreFlash = new System.Windows.Forms.Button();
            this.btnReadCodeplug = new System.Windows.Forms.Button();
            this.btnWriteCodeplug = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.Label();
            this.btnBackupCalibration = new System.Windows.Forms.Button();
            this.btnRestoreCalibration = new System.Windows.Forms.Button();
            this.btnBackupMCUROM = new System.Windows.Forms.Button();
            this.btnDownloadScreenGrab = new System.Windows.Forms.Button();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtBootTune = new System.Windows.Forms.TextBox();
            this.grpFunThings = new System.Windows.Forms.GroupBox();
            this.btnPlayTune = new System.Windows.Forms.Button();
            this.lblBootTune = new System.Windows.Forms.Label();
            this.btnCompressAudio = new System.Windows.Forms.Button();
            this.btnWriteVoicePrompts = new System.Windows.Forms.Button();
            this.btnClearVoicePrompts = new System.Windows.Forms.Button();
            this.btnDownloadSatelliteKeps = new System.Windows.Forms.Button();
            this.txtKepsServer = new System.Windows.Forms.TextBox();
            this.btnBackupSettings = new System.Windows.Forms.Button();
            this.btnRestoreSettings = new System.Windows.Forms.Button();
            this.btnSaveNMEA = new System.Windows.Forms.Button();
            this.btnReadSecureRegisters = new System.Windows.Forms.Button();
            this.btnResetSettings = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.grpFunThings.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBackupEEPROM
            // 
            this.btnBackupEEPROM.BackColor = System.Drawing.SystemColors.Control;
            this.btnBackupEEPROM.Font = new System.Drawing.Font("Arial", 9F);
            this.btnBackupEEPROM.Location = new System.Drawing.Point(19, 14);
            this.btnBackupEEPROM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnBackupEEPROM.Name = "btnBackupEEPROM";
            this.btnBackupEEPROM.Size = new System.Drawing.Size(216, 27);
            this.btnBackupEEPROM.TabIndex = 0;
            this.btnBackupEEPROM.Text = "Backup EEPROM";
            this.btnBackupEEPROM.UseVisualStyleBackColor = false;
            this.btnBackupEEPROM.Click += new System.EventHandler(this.btnBackupEEPROM_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(22, 452);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(718, 13);
            this.progressBar1.TabIndex = 44;
            // 
            // btnBackupFlash
            // 
            this.btnBackupFlash.BackColor = System.Drawing.SystemColors.Control;
            this.btnBackupFlash.Font = new System.Drawing.Font("Arial", 9F);
            this.btnBackupFlash.Location = new System.Drawing.Point(19, 93);
            this.btnBackupFlash.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnBackupFlash.Name = "btnBackupFlash";
            this.btnBackupFlash.Size = new System.Drawing.Size(216, 27);
            this.btnBackupFlash.TabIndex = 2;
            this.btnBackupFlash.Text = "Backup Flash";
            this.btnBackupFlash.UseVisualStyleBackColor = false;
            this.btnBackupFlash.Click += new System.EventHandler(this.btnBackupFlash_Click);
            // 
            // btnRestoreEEPROM
            // 
            this.btnRestoreEEPROM.BackColor = System.Drawing.SystemColors.Control;
            this.btnRestoreEEPROM.Font = new System.Drawing.Font("Arial", 9F);
            this.btnRestoreEEPROM.Location = new System.Drawing.Point(19, 47);
            this.btnRestoreEEPROM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRestoreEEPROM.Name = "btnRestoreEEPROM";
            this.btnRestoreEEPROM.Size = new System.Drawing.Size(216, 27);
            this.btnRestoreEEPROM.TabIndex = 1;
            this.btnRestoreEEPROM.Text = "Restore EEPROM";
            this.btnRestoreEEPROM.UseVisualStyleBackColor = false;
            this.btnRestoreEEPROM.Click += new System.EventHandler(this.btnRestoreEEPROM_Click);
            // 
            // btnRestoreFlash
            // 
            this.btnRestoreFlash.BackColor = System.Drawing.SystemColors.Control;
            this.btnRestoreFlash.Font = new System.Drawing.Font("Arial", 9F);
            this.btnRestoreFlash.Location = new System.Drawing.Point(19, 126);
            this.btnRestoreFlash.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRestoreFlash.Name = "btnRestoreFlash";
            this.btnRestoreFlash.Size = new System.Drawing.Size(216, 27);
            this.btnRestoreFlash.TabIndex = 3;
            this.btnRestoreFlash.Text = "Restore Flash";
            this.btnRestoreFlash.UseVisualStyleBackColor = false;
            this.btnRestoreFlash.Click += new System.EventHandler(this.btnRestoreFlash_Click);
            // 
            // btnReadCodeplug
            // 
            this.btnReadCodeplug.BackColor = System.Drawing.SystemColors.Control;
            this.btnReadCodeplug.Font = new System.Drawing.Font("Arial", 9F);
            this.btnReadCodeplug.Location = new System.Drawing.Point(525, 14);
            this.btnReadCodeplug.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnReadCodeplug.Name = "btnReadCodeplug";
            this.btnReadCodeplug.Size = new System.Drawing.Size(216, 27);
            this.btnReadCodeplug.TabIndex = 12;
            this.btnReadCodeplug.Text = "Read codeplug";
            this.btnReadCodeplug.UseVisualStyleBackColor = false;
            this.btnReadCodeplug.Click += new System.EventHandler(this.btnReadCodeplug_Click);
            // 
            // btnWriteCodeplug
            // 
            this.btnWriteCodeplug.BackColor = System.Drawing.SystemColors.Control;
            this.btnWriteCodeplug.Font = new System.Drawing.Font("Arial", 9F);
            this.btnWriteCodeplug.Location = new System.Drawing.Point(525, 47);
            this.btnWriteCodeplug.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnWriteCodeplug.Name = "btnWriteCodeplug";
            this.btnWriteCodeplug.Size = new System.Drawing.Size(216, 27);
            this.btnWriteCodeplug.TabIndex = 13;
            this.btnWriteCodeplug.Text = "Write codeplug";
            this.btnWriteCodeplug.UseVisualStyleBackColor = false;
            this.btnWriteCodeplug.Click += new System.EventHandler(this.btnWriteCodeplug_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(19, 468);
            this.txtMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(556, 29);
            this.txtMessage.TabIndex = 43;
            this.txtMessage.Text = ".";
            // 
            // btnBackupCalibration
            // 
            this.btnBackupCalibration.BackColor = System.Drawing.SystemColors.Control;
            this.btnBackupCalibration.Font = new System.Drawing.Font("Arial", 9F);
            this.btnBackupCalibration.Location = new System.Drawing.Point(19, 198);
            this.btnBackupCalibration.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnBackupCalibration.Name = "btnBackupCalibration";
            this.btnBackupCalibration.Size = new System.Drawing.Size(216, 27);
            this.btnBackupCalibration.TabIndex = 4;
            this.btnBackupCalibration.Text = "Backup Calibration";
            this.btnBackupCalibration.UseVisualStyleBackColor = false;
            this.btnBackupCalibration.Click += new System.EventHandler(this.btnBackupCalibration_Click);
            // 
            // btnRestoreCalibration
            // 
            this.btnRestoreCalibration.BackColor = System.Drawing.SystemColors.Control;
            this.btnRestoreCalibration.Font = new System.Drawing.Font("Arial", 9F);
            this.btnRestoreCalibration.Location = new System.Drawing.Point(19, 231);
            this.btnRestoreCalibration.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRestoreCalibration.Name = "btnRestoreCalibration";
            this.btnRestoreCalibration.Size = new System.Drawing.Size(216, 27);
            this.btnRestoreCalibration.TabIndex = 5;
            this.btnRestoreCalibration.Text = "Restore Calibration";
            this.btnRestoreCalibration.UseVisualStyleBackColor = false;
            this.btnRestoreCalibration.Click += new System.EventHandler(this.btnRestoreCalibration_Click);
            // 
            // btnBackupMCUROM
            // 
            this.btnBackupMCUROM.BackColor = System.Drawing.SystemColors.Control;
            this.btnBackupMCUROM.Font = new System.Drawing.Font("Arial", 9F);
            this.btnBackupMCUROM.Location = new System.Drawing.Point(278, 14);
            this.btnBackupMCUROM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnBackupMCUROM.Name = "btnBackupMCUROM";
            this.btnBackupMCUROM.Size = new System.Drawing.Size(216, 27);
            this.btnBackupMCUROM.TabIndex = 6;
            this.btnBackupMCUROM.Text = "Backup MCU ROM";
            this.btnBackupMCUROM.UseVisualStyleBackColor = false;
            this.btnBackupMCUROM.Click += new System.EventHandler(this.btnBackupMCUROM_Click);
            // 
            // btnDownloadScreenGrab
            // 
            this.btnDownloadScreenGrab.BackColor = System.Drawing.SystemColors.Control;
            this.btnDownloadScreenGrab.Font = new System.Drawing.Font("Arial", 9F);
            this.btnDownloadScreenGrab.Location = new System.Drawing.Point(278, 47);
            this.btnDownloadScreenGrab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDownloadScreenGrab.Name = "btnDownloadScreenGrab";
            this.btnDownloadScreenGrab.Size = new System.Drawing.Size(216, 27);
            this.btnDownloadScreenGrab.TabIndex = 7;
            this.btnDownloadScreenGrab.Text = "Download screengrab";
            this.btnDownloadScreenGrab.UseVisualStyleBackColor = false;
            this.btnDownloadScreenGrab.Click += new System.EventHandler(this.btnDownloadScreenGrab_Click);
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.BackColor = System.Drawing.SystemColors.Control;
            this.btnOpenFile.Font = new System.Drawing.Font("Arial", 9F);
            this.btnOpenFile.Location = new System.Drawing.Point(7, 29);
            this.btnOpenFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(153, 27);
            this.btnOpenFile.TabIndex = 17;
            this.btnOpenFile.Text = "Select boot image";
            this.btnOpenFile.UseVisualStyleBackColor = false;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(7, 62);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(149, 74);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // txtBootTune
            // 
            this.txtBootTune.Font = new System.Drawing.Font("Arial", 9F);
            this.txtBootTune.Location = new System.Drawing.Point(178, 49);
            this.txtBootTune.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtBootTune.Multiline = true;
            this.txtBootTune.Name = "txtBootTune";
            this.txtBootTune.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBootTune.Size = new System.Drawing.Size(415, 85);
            this.txtBootTune.TabIndex = 18;
            this.txtBootTune.Text = "38,6,0,2,38,2,0,2,38,6,0,2,38,2,0,2,38,6";
            // 
            // grpFunThings
            // 
            this.grpFunThings.Controls.Add(this.btnPlayTune);
            this.grpFunThings.Controls.Add(this.lblBootTune);
            this.grpFunThings.Controls.Add(this.txtBootTune);
            this.grpFunThings.Controls.Add(this.pictureBox1);
            this.grpFunThings.Controls.Add(this.btnOpenFile);
            this.grpFunThings.Location = new System.Drawing.Point(22, 273);
            this.grpFunThings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpFunThings.Name = "grpFunThings";
            this.grpFunThings.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpFunThings.Size = new System.Drawing.Size(718, 158);
            this.grpFunThings.TabIndex = 19;
            this.grpFunThings.TabStop = false;
            this.grpFunThings.Text = "Fun things";
            // 
            // btnPlayTune
            // 
            this.btnPlayTune.BackColor = System.Drawing.SystemColors.Control;
            this.btnPlayTune.Font = new System.Drawing.Font("Arial", 9F);
            this.btnPlayTune.Location = new System.Drawing.Point(615, 109);
            this.btnPlayTune.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnPlayTune.Name = "btnPlayTune";
            this.btnPlayTune.Size = new System.Drawing.Size(88, 27);
            this.btnPlayTune.TabIndex = 20;
            this.btnPlayTune.Text = "Play tune";
            this.btnPlayTune.UseVisualStyleBackColor = false;
            this.btnPlayTune.Click += new System.EventHandler(this.btnPlayTune_Click);
            // 
            // lblBootTune
            // 
            this.lblBootTune.AutoSize = true;
            this.lblBootTune.Location = new System.Drawing.Point(178, 24);
            this.lblBootTune.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBootTune.Name = "lblBootTune";
            this.lblBootTune.Size = new System.Drawing.Size(59, 15);
            this.lblBootTune.TabIndex = 9;
            this.lblBootTune.Text = "Boot tune";
            // 
            // btnCompressAudio
            // 
            this.btnCompressAudio.BackColor = System.Drawing.SystemColors.Control;
            this.btnCompressAudio.Font = new System.Drawing.Font("Arial", 9F);
            this.btnCompressAudio.Location = new System.Drawing.Point(278, 231);
            this.btnCompressAudio.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCompressAudio.Name = "btnCompressAudio";
            this.btnCompressAudio.Size = new System.Drawing.Size(216, 27);
            this.btnCompressAudio.TabIndex = 5;
            this.btnCompressAudio.Text = "Compress audio";
            this.btnCompressAudio.UseVisualStyleBackColor = false;
            this.btnCompressAudio.Visible = false;
            this.btnCompressAudio.Click += new System.EventHandler(this.btnCompressAudio_Click);
            // 
            // btnWriteVoicePrompts
            // 
            this.btnWriteVoicePrompts.BackColor = System.Drawing.SystemColors.Control;
            this.btnWriteVoicePrompts.Font = new System.Drawing.Font("Arial", 9F);
            this.btnWriteVoicePrompts.Location = new System.Drawing.Point(278, 82);
            this.btnWriteVoicePrompts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnWriteVoicePrompts.Name = "btnWriteVoicePrompts";
            this.btnWriteVoicePrompts.Size = new System.Drawing.Size(216, 27);
            this.btnWriteVoicePrompts.TabIndex = 8;
            this.btnWriteVoicePrompts.Text = "Write voice prompts";
            this.btnWriteVoicePrompts.UseVisualStyleBackColor = false;
            this.btnWriteVoicePrompts.Click += new System.EventHandler(this.btnWriteVoicePrompts_Click);
            // 
            // btnClearVoicePrompts
            // 
            this.btnClearVoicePrompts.BackColor = System.Drawing.SystemColors.Control;
            this.btnClearVoicePrompts.Font = new System.Drawing.Font("Arial", 9F);
            this.btnClearVoicePrompts.Location = new System.Drawing.Point(278, 115);
            this.btnClearVoicePrompts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnClearVoicePrompts.Name = "btnClearVoicePrompts";
            this.btnClearVoicePrompts.Size = new System.Drawing.Size(216, 27);
            this.btnClearVoicePrompts.TabIndex = 9;
            this.btnClearVoicePrompts.Text = "Clear voice prompts";
            this.btnClearVoicePrompts.UseVisualStyleBackColor = false;
            this.btnClearVoicePrompts.Click += new System.EventHandler(this.btnClearVoicePrompts_Click);
            // 
            // btnDownloadSatelliteKeps
            // 
            this.btnDownloadSatelliteKeps.BackColor = System.Drawing.SystemColors.Control;
            this.btnDownloadSatelliteKeps.Font = new System.Drawing.Font("Arial", 9F);
            this.btnDownloadSatelliteKeps.Location = new System.Drawing.Point(525, 198);
            this.btnDownloadSatelliteKeps.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDownloadSatelliteKeps.Name = "btnDownloadSatelliteKeps";
            this.btnDownloadSatelliteKeps.Size = new System.Drawing.Size(215, 27);
            this.btnDownloadSatelliteKeps.TabIndex = 15;
            this.btnDownloadSatelliteKeps.Text = "Install satellite Keps";
            this.btnDownloadSatelliteKeps.UseVisualStyleBackColor = false;
            this.btnDownloadSatelliteKeps.Click += new System.EventHandler(this.btnDownloadSatelliteKeps_Click);
            // 
            // txtKepsServer
            // 
            this.txtKepsServer.Font = new System.Drawing.Font("Arial", 9F);
            this.txtKepsServer.Location = new System.Drawing.Point(278, 201);
            this.txtKepsServer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtKepsServer.Name = "txtKepsServer";
            this.txtKepsServer.Size = new System.Drawing.Size(216, 21);
            this.txtKepsServer.TabIndex = 14;
            // 
            // btnBackupSettings
            // 
            this.btnBackupSettings.BackColor = System.Drawing.SystemColors.Control;
            this.btnBackupSettings.Font = new System.Drawing.Font("Arial", 9F);
            this.btnBackupSettings.Location = new System.Drawing.Point(525, 128);
            this.btnBackupSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnBackupSettings.Name = "btnBackupSettings";
            this.btnBackupSettings.Size = new System.Drawing.Size(216, 27);
            this.btnBackupSettings.TabIndex = 11;
            this.btnBackupSettings.Text = "Backup Settings";
            this.btnBackupSettings.UseVisualStyleBackColor = false;
            this.btnBackupSettings.Click += new System.EventHandler(this.btnBackupSettings_Click);
            // 
            // btnRestoreSettings
            // 
            this.btnRestoreSettings.BackColor = System.Drawing.SystemColors.Control;
            this.btnRestoreSettings.Font = new System.Drawing.Font("Arial", 9F);
            this.btnRestoreSettings.Location = new System.Drawing.Point(525, 161);
            this.btnRestoreSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRestoreSettings.Name = "btnRestoreSettings";
            this.btnRestoreSettings.Size = new System.Drawing.Size(216, 27);
            this.btnRestoreSettings.TabIndex = 12;
            this.btnRestoreSettings.Text = "Restore Settings";
            this.btnRestoreSettings.UseVisualStyleBackColor = false;
            this.btnRestoreSettings.Click += new System.EventHandler(this.btnRestoreSettings_Click);
            // 
            // btnSaveNMEA
            // 
            this.btnSaveNMEA.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveNMEA.Enabled = false;
            this.btnSaveNMEA.Font = new System.Drawing.Font("Arial", 9F);
            this.btnSaveNMEA.Location = new System.Drawing.Point(278, 161);
            this.btnSaveNMEA.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSaveNMEA.Name = "btnSaveNMEA";
            this.btnSaveNMEA.Size = new System.Drawing.Size(216, 27);
            this.btnSaveNMEA.TabIndex = 10;
            this.btnSaveNMEA.Text = "Save NMEA Log";
            this.btnSaveNMEA.UseVisualStyleBackColor = false;
            this.btnSaveNMEA.Visible = false;
            this.btnSaveNMEA.Click += new System.EventHandler(this.btnSaveNMEA_Click);
            // 
            // btnReadSecureRegisters
            // 
            this.btnReadSecureRegisters.BackColor = System.Drawing.SystemColors.Control;
            this.btnReadSecureRegisters.Font = new System.Drawing.Font("Arial", 9F);
            this.btnReadSecureRegisters.Location = new System.Drawing.Point(19, 161);
            this.btnReadSecureRegisters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnReadSecureRegisters.Name = "btnReadSecureRegisters";
            this.btnReadSecureRegisters.Size = new System.Drawing.Size(216, 27);
            this.btnReadSecureRegisters.TabIndex = 4;
            this.btnReadSecureRegisters.Text = "Backup Registers";
            this.btnReadSecureRegisters.UseVisualStyleBackColor = false;
            this.btnReadSecureRegisters.Click += new System.EventHandler(this.btnReadSecureRegisters_Click);
            // 
            // btnResetSettings
            // 
            this.btnResetSettings.BackColor = System.Drawing.SystemColors.Control;
            this.btnResetSettings.Location = new System.Drawing.Point(525, 82);
            this.btnResetSettings.Name = "btnResetSettings";
            this.btnResetSettings.Size = new System.Drawing.Size(216, 27);
            this.btnResetSettings.TabIndex = 45;
            this.btnResetSettings.Text = "Restore settings";
            this.btnResetSettings.UseVisualStyleBackColor = false;
            this.btnResetSettings.Click += new System.EventHandler(this.btnResetSettings_Click);
            // 
            // OpenGD77Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(759, 527);
            this.Controls.Add(this.btnResetSettings);
            this.Controls.Add(this.txtKepsServer);
            this.Controls.Add(this.grpFunThings);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.btnRestoreCalibration);
            this.Controls.Add(this.btnDownloadSatelliteKeps);
            this.Controls.Add(this.btnWriteVoicePrompts);
            this.Controls.Add(this.btnClearVoicePrompts);
            this.Controls.Add(this.btnCompressAudio);
            this.Controls.Add(this.btnDownloadScreenGrab);
            this.Controls.Add(this.btnBackupMCUROM);
            this.Controls.Add(this.btnBackupCalibration);
            this.Controls.Add(this.btnRestoreSettings);
            this.Controls.Add(this.btnRestoreFlash);
            this.Controls.Add(this.btnBackupSettings);
            this.Controls.Add(this.btnSaveNMEA);
            this.Controls.Add(this.btnBackupFlash);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnRestoreEEPROM);
            this.Controls.Add(this.btnWriteCodeplug);
            this.Controls.Add(this.btnReadCodeplug);
            this.Controls.Add(this.btnBackupEEPROM);
            this.Controls.Add(this.btnReadSecureRegisters);
            this.Font = new System.Drawing.Font("Arial", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OpenGD77Form";
            this.Text = "OpenGD77 Support";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OpenGD77Form_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OpenGD77Form_FormClosed);
            this.Load += new System.EventHandler(this.OpenGD77Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.grpFunThings.ResumeLayout(false);
            this.grpFunThings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

	}


}
