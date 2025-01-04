using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.VisualBasic.FileIO;

namespace DMR;

internal class CSVEML
{
	private const int ContactLength = 24;

	private const int ContactNameLength = 16;

	private const int NumberOfTGLists = 76;

	private const int NumberOfTGListMembers = 32;

	private const int TGListLength = 80;

	private const int TGListNameLength = 16;

	private const int ZoneNameLength = 16;

	private const int NumberofZones = 68;

	private const int ZoneLength = 176;

	private const int NumberofZoneMembers = 80;

	private const int NumberOfChannelsPerBank = 128;

	private const int NumberOfChannelBanks = 8;

	private const int ChannelLength = 56;

	private const int ChannelNamelength = 16;

	private const int NumberOfDTMF = 63;

	private const int DTMFLength = 32;

	private const int DTMFNameLength = 16;

	private static string[] POWER_LEVELS_LOOKUP = new string[11]
	{
		"Master", "P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "P9",
		"-W+"
	};

	private const string DTMF_LOOKUP = "0123456789ABCD*#";

	private static string[] TA_TX_LOOKUP = new string[4] { "Off", "APRS", "Text", "APRS+Text" };

	private static string N_A_VALUE = "";

	public static int ContactCount;

	public static int TGListCount;

	public static int ChannelCount;

	public static int Zonecount;

	public static int DTMFCount;

	public static int APRSCount;

	public static string CSVName;

	public static bool append = false;

	public static string decimalSeparator = ".";

	public static string altDecimalSeperator = ",";

	public static string writeSeparator = ",";

	public static int[] ChannelStartBank = new int[8] { 14208, 45488, 52672, 59856, 67040, 74224, 81408, 88592 };

	public static string[] ContactName = new string[1025];

	public static int[] ContactID = new int[1025];

	public static byte[] ContactType = new byte[1025];

	public static byte[] ContactTS = new byte[1025];

	public static string[] TGListName = new string[77];

	public static int[,] TGListMember = new int[77, 33];

	public static string[] ChannelName = new string[1025];

	public static byte[,] ChannelByte = new byte[1025, 41];

	public static string[] ZoneName = new string[69];

	public static int[,] ZoneMember = new int[69, 81];

	public static string[] DTMFName = new string[65];

	public static string[] DTMFCode = new string[65];

	public static string[] APRSName = new string[65];

	public static APRSForm.APRS_Config aprsConfigs = new APRSForm.APRS_Config();

	public static byte[] Codeplug;

	public static Dictionary<string, string> StringsDict = new Dictionary<string, string>();

	public static void InitCSVs()
	{
		decimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
		if (decimalSeparator == ",")
		{
			altDecimalSeperator = ".";
			writeSeparator = ";";
		}
		Settings.ReadCommonsForSectionIntoDictionary(StringsDict, "CSVEML");
	}

	public static string GetCSVFolder(string prompt)
	{
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		folderBrowserDialog.Description = prompt;
		folderBrowserDialog.SelectedPath = IniFileUtils.getProfileStringWithDefault("Setup", "LastSaveCSVLocation", null);
		if (DialogResult.OK == folderBrowserDialog.ShowDialog())
		{
			IniFileUtils.WriteProfileString("Setup", "LastSaveCSVLocation", folderBrowserDialog.SelectedPath);
			return folderBrowserDialog.SelectedPath;
		}
		return "";
	}

	public static void ExportCSV()
	{
		Codeplug = MainForm.DataToByte();
		string cSVFolder = GetCSVFolder(StringsDict["Please_select_a_folder_to_contain_the_.csv_files"]);
		if (cSVFolder != "")
		{
			CreateCSVs(cSVFolder);
		}
	}

	public static void ImportCSV()
	{
		if (MessageBox.Show(StringsDict["Are_You_Sure_CSV"], "", MessageBoxButtons.OKCancel) == DialogResult.OK)
		{
			Codeplug = MainForm.DataToByte();
			string cSVFolder = GetCSVFolder(StringsDict["Please_select_a_folder_containing_the_.csv_files"]);
			if (cSVFolder != "")
			{
				ImportCSVs(cSVFolder);
				MainForm.ByteToData(Codeplug, isFromFile: true);
			}
		}
	}

	public static void AppendCSV()
	{
		Codeplug = MainForm.DataToByte();
		string cSVFolder = GetCSVFolder(StringsDict["Please_select_a_folder_containing_the_.csv_files"]);
		if (cSVFolder != "")
		{
			AppendCSVs(cSVFolder);
			MainForm.ByteToData(Codeplug, isFromFile: true);
		}
	}

	public static void UpdateLocationCSV()
	{
		Codeplug = MainForm.DataToByte();
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "csv (*.csv)|*.csv";
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			UpdateLocationCSVs(openFileDialog.FileName);
		}
	}

	public static void CreateCSVs(string CSVDirectory)
	{
		DecodeCodeplug();
		if (WriteCSVs(CSVDirectory + "/"))
		{
			Filestats(StringsDict["CSVs_Created_in"] + " " + CSVDirectory, StringsDict["Codeplug_contains"]);
		}
	}

	public static void ImportCSVs(string CSVDirectory)
	{
		DecodeCodeplug();
		append = false;
		if (ReadCSVs(CSVDirectory + "/"))
		{
			Filestats(CSVDirectory + " " + StringsDict["Imported"], StringsDict["Codeplug_now_contains"]);
		}
	}

	public static void AppendCSVs(string CSVDirectory)
	{
		DecodeCodeplug();
		aprsConfigs = APRSForm.data;
		append = true;
		if (ReadCSVs(CSVDirectory + "/"))
		{
			Filestats(CSVDirectory + " " + StringsDict["Appended"], StringsDict["Codeplug_now_contains"]);
		}
	}

	public static void UpdateLocationCSVs(string filename)
	{
		OpenLocationsCSV(filename);
	}

	private static void DecodeCodeplug()
	{
		ExtractContacts();
		ExtractTGLists();
		ExtractChannels();
		ExtractZones();
		ExtractDTMFs();
	}

	private static string ExtractString(int offset, short maxlen)
	{
		string text = "";
        Encoding decoder = Encoding.GetEncoding(1251);
        Encoding encoder = Encoding.UTF8;
		byte[] inBuffer = new byte[maxlen];
        byte[] outBuffer = new byte[maxlen];
		int stop = 0;
        for (int i = 0; i < maxlen; i++)
		{
			int num = Codeplug[offset + i];
			if (num == 0 || num == 255)
			{
				break;
			}
			stop++;
			if (num == 0x7f)
				num = 0xff;
			inBuffer[i] = (byte)num;
		}
		outBuffer = Encoding.Convert(decoder, encoder, inBuffer, 0, stop);
		text = encoder.GetString(outBuffer);
		return text;
	}

	private static void InsertString(int offset, string st, byte maxlen, byte filler)
	{
		int num = 0;
		int length = st.Length;
        Encoding decoder = Encoding.GetEncoding(1251);
		byte[] inBuffer = new byte[maxlen];
		inBuffer = decoder.GetBytes(st);
        for (int i = 0; i < inBuffer.Length; i++)
		{
			if (inBuffer[i] == 0xff)
				inBuffer[i] = 0x7f;
		}
        for (int i = 0; i < maxlen; i++)
		{
			if (i<length)
			{
				num = (int)inBuffer[i];
			}
			else
			{
				num = filler;
			}
			Codeplug[offset + i] = (byte)num;
		}
	}

	private static void InsertDTMFCode(int offset, string st)
	{
		for (int i = 0; i < 16; i++)
		{
			Codeplug[offset + i] = byte.MaxValue;
		}
		for (int j = 0; j < st.Length; j++)
		{
			Codeplug[offset + j] = (byte)"0123456789ABCD*#".IndexOf(st[j]);
		}
	}

	private static string ExtractDTMFCode(int offset)
	{
		string text = "";
		for (int i = 0; i < 16; i++)
		{
			int num = Codeplug[offset + i];
			if (num < "0123456789ABCD*#".Length)
			{
				text += "0123456789ABCD*#"[num];
			}
		}
		return text;
	}

	private static int ExtractID(int offset)
	{
		int num = 0;
		for (int i = 0; i <= 3; i++)
		{
			int num2 = BCDtoByte(Codeplug[offset + i]);
			num *= 100;
			num += num2;
		}
		return num;
	}

	private static void InsertID(int offset, int id)
	{
		int num = id;
		for (int num2 = 3; num2 >= 0; num2--)
		{
			byte val = (byte)(num % 100);
			num /= 100;
			Codeplug[offset + num2] = BytetoBCD(val);
		}
	}

	private static byte ExtractByte(int offset)
	{
		return Codeplug[offset];
	}

	private static void InsertByte(int offset, byte value)
	{
		Codeplug[offset] = value;
	}

	private static byte BCDtoByte(byte bcd)
	{
		int num = bcd >> 4;
		int num2 = bcd & 0xF;
		return (byte)(num * 10 + num2);
	}

	private static byte BytetoBCD(byte val)
	{
		int num = (int)((double)(int)val / 10.0);
		int num2 = val % 10;
		return (byte)(num * 16 + num2);
	}

	private static string DecodeFrequency(byte a, byte b, byte c, byte d)
	{
		string text = d.ToString("X") + (c >> 4).ToString("X") + decimalSeparator + (c & 0xF).ToString("X");
		if (b < 10)
		{
			text += "0";
		}
		text += b.ToString("X");
		if (a < 10)
		{
			text += "0";
		}
		text += a.ToString("X");
		return "\t" + text;
	}

	private static void EncodeFrequency(double f, ref byte a, ref byte b, ref byte c, ref byte d)
	{
		f += 1E-06;
		d = BytetoBCD((byte)(int)(f / 10.0));
		c = BytetoBCD((byte)((int)(f * 10.0) % 100));
		b = BytetoBCD((byte)((int)(f * 1000.0) % 100));
		a = BytetoBCD((byte)((int)(f * 100000.0) % 100));
	}

	private static string DecodeSquelch(byte a)
	{
		if (a == 0)
		{
			return "Disabled";
		}
		if (a == 1)
		{
			return "Open";
		}
		if (a > 20)
		{
			return "Closed";
		}
		return (byte)((a - 1) * 5) + "%";
	}

	private static byte EncodeSquelch(string s)
	{
		byte result = 0;
		switch (s)
		{
		case "Disabled":
			result = 0;
			break;
		case "Open":
			result = 1;
			break;
		case "Closed":
			result = 21;
			break;
		default:
			if (s.IndexOf("%") != -1)
			{
				s = s.TrimEnd('%');
				result = (byte)(int.Parse(s) / 5 + 1);
			}
			break;
		}
		return result;
	}

	private static string DecodePower(byte a)
	{
		string result = "Master";
		if (a < POWER_LEVELS_LOOKUP.Length)
		{
			result = POWER_LEVELS_LOOKUP[a];
		}
		return result;
	}

	private static byte EncodePower(string s)
	{
		for (int i = 0; i < POWER_LEVELS_LOOKUP.Length; i++)
		{
			if (POWER_LEVELS_LOOKUP[i] == s)
			{
				return (byte)i;
			}
		}
		return 0;
	}

	private static string DecodeTATx(int a)
	{
		string result = TA_TX_LOOKUP[0];
		if (a < TA_TX_LOOKUP.Length)
		{
			result = TA_TX_LOOKUP[a];
		}
		return result;
	}

	private static int EncodeTATx(string s)
	{
		for (int i = 0; i < TA_TX_LOOKUP.Length; i++)
		{
			if (TA_TX_LOOKUP[i] == s)
			{
				return i;
			}
		}
		return 0;
	}

	private static string DecodeTot(byte a)
	{
		return (a * 15).ToString();
	}

	private static byte EncodeTOT(string s)
	{
		return (byte)(int.Parse(s) / 15);
	}

	private static string DecodeLatLon(byte b2, byte b1, byte b0)
	{
		return ChannelForm.ChannelOne.LatLonBin24ToString((uint)((b2 << 16) + (b1 << 8) + b0));
	}

	private static uint EncodeLatLon(string value)
	{
		return ChannelForm.ChannelOne.LatLonStringToBin24(value);
	}

	private static string DecodeTone(byte a, byte b)
	{
		if ((a == byte.MaxValue && b == byte.MaxValue) || (a == 0 && b == 0))
		{
			return "None";
		}
		if ((b & 0x80) != 0)
		{
			string text = "D" + (b & 7).ToString("X") + (a >> 4).ToString("X") + (a & 0xF).ToString("X");
			if ((b & 0x40) != 0)
			{
				return text + "I";
			}
			return text + "N";
		}
		return (b & 0x3F).ToString("X") + (a >> 4).ToString("X") + decimalSeparator + (a & 0xF).ToString("X");
	}

	private static void EncodeTone(string s, ref byte a, ref byte b)
	{
		if (s == "None" || s == N_A_VALUE)
		{
			a = byte.MaxValue;
			b = byte.MaxValue;
		}
		else if (s.IndexOf("D") != -1)
		{
			float num = int.Parse(s.Substring(1, 3));
			b = BytetoBCD((byte)(int)((double)num / 100.0));
			a = BytetoBCD((byte)(int)(num % 100f));
			b |= 128;
			if (s.IndexOf("I") != -1)
			{
				b |= 64;
			}
		}
		else
		{
			s = s.Replace(altDecimalSeperator, decimalSeparator);
			decimal num2 = decimal.Parse(s);
			int num3 = (int)(num2 / 10m);
			int num4 = (int)(num2 * 10m) % 100;
			b = BytetoBCD((byte)num3);
			a = BytetoBCD((byte)num4);
		}
	}

	private static void ExtractContacts()
	{
		ContactCount = 0;
		for (int i = 0; i < ContactForm.data.Count; i++)
		{
			int num = Settings.ADDR_DMR_CONTACT_EX + i * 24;
			ContactName[i] = ExtractString(num, 16);
			ContactID[i] = ExtractID(num + 16);
			ContactType[i] = ExtractByte(num + 20);
			ContactTS[i] = ExtractByte(num + 23);
			if (ContactName[i] != "")
			{
				ContactCount++;
			}
		}
	}

	private static void InsertContacts()
	{
		for (int i = 0; i < ContactForm.data.Count; i++)
		{
			int num = Settings.ADDR_DMR_CONTACT_EX + i * 24;
			InsertString(num, ContactName[i], 16, byte.MaxValue);
			InsertID(num + 16, ContactID[i]);
			InsertByte(num + 20, ContactType[i]);
			InsertByte(num + 23, ContactTS[i]);
		}
	}

	private static void ExtractDTMFs()
	{
		DTMFCount = 0;
		for (int i = 0; i < 63; i++)
		{
			int num = Settings.ADDR_DTMF_CONTACT + i * 32;
			DTMFName[i] = ExtractString(num, 16);
			DTMFCode[i] = ExtractDTMFCode(num + 16);
			if (DTMFName[i] != "")
			{
				DTMFCount++;
			}
		}
	}

	private static void InsertDTMFs()
	{
		for (int i = 0; i < 63; i++)
		{
			int num = Settings.ADDR_DTMF_CONTACT + i * 32;
			InsertString(num, DTMFName[i], 16, byte.MaxValue);
			InsertDTMFCode(num + 16, DTMFCode[i]);
		}
	}

	private static byte GetTGListSize(byte offset)
	{
		return Codeplug[Settings.ADDR_RX_GRP_LIST_EX + offset];
	}

	private static void setTGListsize(byte offset, byte v)
	{
		Codeplug[Settings.ADDR_RX_GRP_LIST_EX + offset] = v;
	}

	private static void ExtractTGLists()
	{
		TGListCount = 0;
		for (int i = 0; i < 76; i++)
		{
			int num = Settings.ADDR_RX_GRP_LIST_EX + 128 + i * 80;
			byte tGListSize = GetTGListSize((byte)i);
			if (tGListSize > 0)
			{
				tGListSize--;
				TGListName[i] = ExtractString(num, 16);
				for (int j = 0; j < 32; j++)
				{
					if (j < tGListSize)
					{
						TGListMember[i, j] = (short)(ExtractByte(num + 16 + j * 2) + ExtractByte(num + 17 + j * 2) * 256);
					}
					else
					{
						TGListMember[i, j] = 0;
					}
				}
			}
			else
			{
				TGListName[i] = "";
				for (int k = 0; k < 32; k++)
				{
					TGListMember[i, k] = 0;
				}
			}
			if (TGListName[i] != "")
			{
				TGListCount++;
			}
		}
	}

	private static void InsertTGLists()
	{
		for (int i = 0; i < 76; i++)
		{
			int num = 0;
			int num2 = Settings.ADDR_RX_GRP_LIST_EX + 128 + i * 80;
			InsertString(num2, TGListName[i], 16, 0);
			if (TGListName[i] != "")
			{
				num = 1;
			}
			for (int j = 0; j < 32; j++)
			{
				InsertByte(num2 + 16 + j * 2, (byte)(TGListMember[i, j] & 0xFF));
				InsertByte(num2 + 17 + j * 2, (byte)(TGListMember[i, j] >> 8));
				if (TGListMember[i, j] > 0)
				{
					num++;
				}
			}
			setTGListsize((byte)i, (byte)num);
		}
	}

	public static int TestBitArray(int p, byte b)
	{
		byte b2 = (byte)(b % 8);
		byte b3 = (byte)(b >> 3);
		byte b4 = (byte)(1 << (int)b2);
		if ((byte)(ExtractByte(p + b3) & b4) > 0)
		{
			return 1;
		}
		return 0;
	}

	public static void SetBitArray(int p, byte b, byte v)
	{
		byte b2 = (byte)(b % 8);
		byte b3 = (byte)(b >> 3);
		byte b4 = (byte)(1 << (int)b2);
		if (v == 1)
		{
			Codeplug[p + b3] = (byte)(Codeplug[p + b3] | b4);
		}
		else
		{
			Codeplug[p + b3] = (byte)(Codeplug[p + b3] & ~b4);
		}
	}

	public static string Strip(string s)
	{
		if (s.Length == 0)
		{
			return s;
		}
		if (s[0] == '\'')
		{
			return s.Substring(1);
		}
		return s;
	}

	public static string NumberFormatting(string s)
	{
		s = s.Replace(altDecimalSeperator, decimalSeparator);
		if (float.TryParse(s, out var _))
		{
			return "\t" + s;
		}
		return s;
	}

	private static void ExtractChannels()
	{
		ChannelCount = 0;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 128; j++)
			{
				int num = 128 * i + j;
				int num2 = ChannelStartBank[i] + 16 + j * 56;
				if (TestBitArray(ChannelStartBank[i], (byte)j) == 1)
				{
					ChannelName[num] = ExtractString(num2, 16);
					for (int k = 0; k < 40; k++)
					{
						ChannelByte[num, k] = ExtractByte(num2 + 16 + k);
					}
				}
				else
				{
					ChannelName[num] = "";
					for (int k = 0; k < 40; k++)
					{
						ChannelByte[num, k] = 0;
					}
				}
				if (ChannelName[num] != "")
				{
					ChannelCount++;
				}
			}
		}
	}

	private static void InsertChannels()
	{
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 128; j++)
			{
				int num = 128 * i + j;
				int num2 = ChannelStartBank[i] + 16 + j * 56;
				if (ChannelName[num] == "")
				{
					InsertString(num2, ChannelName[num], 16, 0);
					SetBitArray(ChannelStartBank[i], (byte)j, 0);
				}
				else
				{
					InsertString(num2, ChannelName[num], 16, byte.MaxValue);
					SetBitArray(ChannelStartBank[i], (byte)j, 1);
				}
				for (int k = 0; k < 40; k++)
				{
					InsertByte(num2 + 16 + k, ChannelByte[num, k]);
				}
			}
		}
	}

	private static void ExtractZones()
	{
		Zonecount = 0;
		for (int i = 0; i < 68; i++)
		{
			int num = Settings.ADDR_EX_ZONE_LIST + 32 + i * 176;
			if (TestBitArray(Settings.ADDR_EX_ZONE_LIST, (byte)i) == 1)
			{
				ZoneName[i] = ExtractString(num, 16);
				for (int j = 0; j < 80; j++)
				{
					ZoneMember[i, j] = (short)(ExtractByte(num + 16 + j * 2) + ExtractByte(num + 17 + j * 2) * 256);
				}
			}
			else
			{
				ZoneName[i] = "";
				for (int j = 0; j < 80; j++)
				{
					ZoneMember[i, j] = 0;
				}
			}
			if (ZoneName[i] != "")
			{
				Zonecount++;
			}
		}
	}

	private static void InsertZones()
	{
		for (int i = 0; i < 68; i++)
		{
			int num = Settings.ADDR_EX_ZONE_LIST + 32 + i * 176;
			if (ZoneName[i] != "")
			{
				InsertString(num, ZoneName[i], 16, byte.MaxValue);
				SetBitArray(Settings.ADDR_EX_ZONE_LIST, (byte)i, 1);
			}
			else
			{
				InsertString(num, ZoneName[i], 16, 0);
				SetBitArray(Settings.ADDR_EX_ZONE_LIST, (byte)i, 0);
			}
			for (int j = 0; j < 80; j++)
			{
				InsertByte(num + 16 + j * 2, (byte)(ZoneMember[i, j] & 0xFF));
				InsertByte(num + 17 + j * 2, (byte)(ZoneMember[i, j] >> 8));
			}
		}
	}

	private static bool SaveContactsCSV()
	{
		string text = "Contact Name" + writeSeparator + "ID" + writeSeparator + "ID Type" + writeSeparator + "TS Override" + Environment.NewLine;
		ContactCount = 0;
		for (int i = 0; i < ContactForm.data.Count; i++)
		{
			if (ContactName[i] != "")
			{
				ContactCount++;
				string text2 = "Group";
				if (ContactType[i] == 0)
				{
					text2 = "Group";
				}
				else if (ContactType[i] == 1)
				{
					text2 = "Private";
				}
				else if (ContactType[i] == 2)
				{
					text2 = "AllCall";
				}
				string text3 = (((ContactTS[i] & 1) != 0) ? "Disabled" : (((ContactTS[i] & 2) == 0) ? "1" : "2"));
				text = text + NumberFormatting((ContactName[i])) + writeSeparator + ContactID[i] + writeSeparator + text2 + writeSeparator + text3 + Environment.NewLine;
			}
		}
		try
		{
			File.WriteAllText(CSVName + "Contacts.csv", text);
		}
		catch
		{
			MessageBox.Show(StringsDict["ERROR:-_Could_not_open"] + " Contacts.csv. " + StringsDict["May_be_in_use_by_another_program"]);
			return false;
		}
		return true;
	}

	private static bool isFileLocked(string fileName)
	{
		try
		{
			File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None).Close();
		}
		catch (IOException)
		{
			return true;
		}
		return false;
	}

	private static bool OpenContactsCSV()
	{
		int num = (append ? ContactCount : 0);
		string text = CSVName + "Contacts.csv";
		if (File.Exists(text))
		{
			if (isFileLocked(text))
			{
				MessageBox.Show(StringsDict["ERROR:-_Could_not_open"] + " Contacts.csv. " + StringsDict["May_be_in_use_by_another_program"]);
				return false;
			}
			string text2 = CheckDelimiter(text);
			if (text2 == "")
			{
				return true;
			}
			TextFieldParser textFieldParser = new TextFieldParser(text);
			textFieldParser.TextFieldType = FieldType.Delimited;
			textFieldParser.SetDelimiters(text2);
			textFieldParser.ReadFields();
			while (!textFieldParser.EndOfData && num < ContactForm.data.Count)
			{
				try
				{
					string[] array = textFieldParser.ReadFields();
					if (array.Length < 4)
					{
						MessageBox.Show(StringsDict["Format_error_in"] + " Contacts.csv. " + StringsDict["Not_enough_columns_in_line"] + (textFieldParser.LineNumber - 1));
						textFieldParser.Close();
						return false;
					}
					int num2 = num;
					if (append)
					{
						int num3 = LookupContact(Strip(array[0]));
						if (num3 > 0)
						{
							num2 = num3 - 1;
						}
					}
					ContactName[num2] = Strip(array[0]);
					ContactID[num2] = int.Parse(array[1]);
					ContactType[num2] = 0;
					if (array[2] == "Private")
					{
						ContactType[num2] = 1;
					}
					if (array[2] == "AllCall")
					{
						ContactType[num2] = 2;
					}
					ContactTS[num2] = 1;
					if (array[3] == "2")
					{
						ContactTS[num2] = 2;
					}
					if (array[3] == "1")
					{
						ContactTS[num2] = 0;
					}
					if (num2 == num && ContactName[num] != "")
					{
						num++;
					}
				}
				catch
				{
					MessageBox.Show(StringsDict["Line"] + " " + (textFieldParser.LineNumber - 1) + " " + StringsDict["is_not_valid_in"] + " Contacts.csv " + StringsDict["and_will_be_skipped"]);
					textFieldParser.Close();
					return false;
				}
			}
			ContactCount = num;
			if (num < ContactForm.data.Count)
			{
				for (int i = num; i < ContactForm.data.Count; i++)
				{
					ContactName[i] = "";
					ContactID[i] = 0;
					ContactType[i] = 0;
					ContactTS[i] = 0;
				}
			}
			textFieldParser.Close();
		}
		return true;
	}

	private static bool SaveDTMFCSV()
	{
		DTMFCount = 0;
		string text = "Contact Name" + writeSeparator + "Code" + Environment.NewLine;
		for (int i = 0; i < 63; i++)
		{
			if (DTMFName[i] != "")
			{
				DTMFCount++;
				text = text + NumberFormatting(DTMFName[i]) + writeSeparator + NumberFormatting(DTMFCode[i]) + Environment.NewLine;
			}
		}
		try
		{
			File.WriteAllText(CSVName + "DTMF.csv", text);
		}
		catch
		{
			MessageBox.Show(StringsDict["ERROR:-_Could_not_open"] + " DTMF.csv. " + StringsDict["May_be_in_use_by_another_program"]);
			return false;
		}
		return true;
	}

	private static bool SaveAPRSCSV()
	{
		APRSCount = 0;
		string text = "APRS config Name" + writeSeparator + "SSID" + writeSeparator + "Via1" + writeSeparator + "Via1 SSID" + writeSeparator + "Via2" + writeSeparator + "Via2 SSID" + writeSeparator + "Icon table" + writeSeparator + "Icon" + writeSeparator + "Comment text" + writeSeparator + "Ambiguity" + writeSeparator + "Use position" + writeSeparator + "Latitude" + writeSeparator + "Longitude" + writeSeparator + "Baud rate setting" + Environment.NewLine;
		for (int i = 0; i < APRSForm.data.Count; i++)
		{
			if (APRSForm.data.DataIsValid(i))
			{
				text = text + APRSForm.data[i].Name + writeSeparator + APRSForm.data[i].SSID + writeSeparator + APRSForm.data[i].Via1 + writeSeparator + APRSForm.data[i].Via1SSID + writeSeparator + APRSForm.data[i].Via2 + writeSeparator + APRSForm.data[i].Via2SSID + writeSeparator + APRSForm.data[i].IconTableNumber + writeSeparator + APRSForm.data[i].IconIndex + writeSeparator + APRSForm.data[i].Comment + writeSeparator + APRSForm.data[i].PositionMasking + writeSeparator + APRSForm.data[i].UsePosition + writeSeparator + APRSForm.data[i].Latitude + writeSeparator + APRSForm.data[i].Longitude + writeSeparator + APRSForm.data[i].BaudRate + writeSeparator + Environment.NewLine;
				APRSCount++;
			}
		}
		try
		{
			File.WriteAllText(CSVName + "APRS.csv", text);
		}
		catch
		{
			MessageBox.Show(StringsDict["ERROR:-_Could_not_open"] + " APRS.csv. " + StringsDict["May_be_in_use_by_another_program"]);
			return false;
		}
		return true;
	}

	private static bool OpenDTMFCSV()
	{
		string text = CSVName + "DTMF.csv";
		int num = (append ? DTMFCount : 0);
		if (File.Exists(text))
		{
			if (isFileLocked(text))
			{
				MessageBox.Show(StringsDict["ERROR:-_Could_not_open"] + " DTMF.csv. " + StringsDict["May_be_in_use_by_another_program"]);
				return false;
			}
			string text2 = CheckDelimiter(text);
			if (text2 == "")
			{
				return true;
			}
			TextFieldParser textFieldParser = new TextFieldParser(text);
			textFieldParser.TextFieldType = FieldType.Delimited;
			textFieldParser.SetDelimiters(text2);
			textFieldParser.ReadFields();
			while (!textFieldParser.EndOfData && num < 63)
			{
				try
				{
					string[] array = textFieldParser.ReadFields();
					if (array.Length < 2)
					{
						MessageBox.Show(StringsDict["Format_error_in"] + " DTMF.csv. " + StringsDict["Not_enough_columns_in_line"] + (textFieldParser.LineNumber - 1));
						textFieldParser.Close();
						return false;
					}
					int num2 = num;
					if (append)
					{
						int num3 = LookupDTMF(Strip(array[0]));
						if (num3 > 0)
						{
							num2 = num3 - 1;
						}
					}
					DTMFName[num2] = Strip(array[0]);
					DTMFCode[num2] = Strip(array[1]);
					if (num2 == num && DTMFName[num] != "")
					{
						num++;
					}
				}
				catch
				{
					MessageBox.Show(StringsDict["Line"] + " " + (textFieldParser.LineNumber - 1) + " " + StringsDict["is_not_valid_in"] + " DTMF.csv " + StringsDict["and_will_be_skipped"]);
					textFieldParser.Close();
					return false;
				}
			}
			DTMFCount = num;
			if (num < 63)
			{
				for (int i = num; i < 63; i++)
				{
					DTMFName[i] = "";
					DTMFCode[i] = "";
				}
			}
			textFieldParser.Close();
		}
		return true;
	}

	private static bool OpenAPRSCSV()
	{
		string text = CSVName + "APRS.csv";
		int num = (append ? APRSCount : 0);
		if (File.Exists(text))
		{
			if (isFileLocked(text))
			{
				MessageBox.Show(StringsDict["ERROR:-_Could_not_open"] + " APRS.csv. " + StringsDict["May_be_in_use_by_another_program"]);
				return false;
			}
			string text2 = CheckDelimiter(text);
			if (text2 == "")
			{
				return true;
			}
			TextFieldParser textFieldParser = new TextFieldParser(text);
			textFieldParser.TextFieldType = FieldType.Delimited;
			textFieldParser.SetDelimiters(text2);
			textFieldParser.ReadFields();
			while (!textFieldParser.EndOfData && num < APRSForm.data.Count)
			{
				try
				{
					string[] array = textFieldParser.ReadFields();
					if (array.Length < 10)
					{
						MessageBox.Show(StringsDict["Format_error_in"] + " APRS.csv. " + StringsDict["Not_enough_columns_in_line"] + (textFieldParser.LineNumber - 1));
						textFieldParser.Close();
						return false;
					}
					int num2 = num;
					if (append)
					{
						int num3 = LookupAPRS(Strip(array[0]));
						if (num3 > 0)
						{
							num2 = num3 - 1;
						}
					}
					aprsConfigs.APRS_list[num2] = new APRSForm.APRS_One(num2);
					aprsConfigs.APRS_list[num2].Name = Strip(array[0]);
					aprsConfigs.APRS_list[num2].SSID = int.Parse(Strip(array[1]));
					aprsConfigs.APRS_list[num2].Via1 = Strip(array[2]);
					aprsConfigs.APRS_list[num2].Via1SSID = int.Parse(Strip(array[3]));
					aprsConfigs.APRS_list[num2].Via2 = Strip(array[4]);
					aprsConfigs.APRS_list[num2].Via2SSID = int.Parse(Strip(array[5]));
					aprsConfigs.APRS_list[num2].IconTableNumber = int.Parse(Strip(array[6]));
					aprsConfigs.APRS_list[num2].IconIndex = int.Parse(Strip(array[7]));
					aprsConfigs.APRS_list[num2].Comment = Strip(array[8]);
					aprsConfigs.APRS_list[num2].PositionMasking = int.Parse(Strip(array[9]));
					aprsConfigs.APRS_list[num2].UsePosition = bool.Parse(Strip(array[10]));
					aprsConfigs.APRS_list[num2].Latitude = Strip(array[11]);
					aprsConfigs.APRS_list[num2].Longitude = Strip(array[12]);
					aprsConfigs.APRS_list[num2].BaudRate = int.Parse(Strip(array[13]));
					if (num2 == num && aprsConfigs.APRS_list[num].Name != "")
					{
						num++;
					}
				}
				catch
				{
					MessageBox.Show(StringsDict["Line"] + " " + (textFieldParser.LineNumber - 1) + " " + StringsDict["is_not_valid_in"] + " APRS.csv " + StringsDict["and_will_be_skipped"]);
					textFieldParser.Close();
					return false;
				}
			}
			APRSCount = num;
			if (num < APRSForm.data.Count)
			{
				for (int i = num; i < APRSForm.data.Count; i++)
				{
					aprsConfigs.APRS_list[i] = new APRSForm.APRS_One(i);
					aprsConfigs.APRS_list[i].Default();
				}
			}
			textFieldParser.Close();
			byte[] array2 = Settings.objectToByteArray(aprsConfigs, Marshal.SizeOf(APRSForm.data.GetType()));
			Array.Copy(array2, 0, Codeplug, Settings.ADDR_APRS_SYSTEM, array2.Length);
		}
		return true;
	}

	private static bool SaveTGListsCSV()
	{
		string path = CSVName + "TG_Lists.csv";
		string text = "TG List Name";
		TGListCount = 0;
		for (int i = 1; i <= 32; i++)
		{
			text = text + writeSeparator + "Contact" + i;
		}
		text += Environment.NewLine;
		for (int j = 0; j < 76; j++)
		{
			if (TGListName[j] != "")
			{
				TGListCount++;
				string outListName = TGListName[j];
				string outContactName = "";
				text = text + NumberFormatting(outListName) + writeSeparator;
				for (int k = 0; k < 32; k++)
				{
					if (TGListMember[j, k] > 0) 
						outContactName = convert1251toUTF8(ContactName[TGListMember[j, k] - 1]);

                    text = ((TGListMember[j, k] <= 0) ? (text + writeSeparator) : (text + NumberFormatting(outContactName) + writeSeparator));
				}
				text = text.Substring(0, text.Length - 1);
				text += Environment.NewLine;
			}
		}
		try
		{
			File.WriteAllText(path, text);
		}
		catch
		{
			MessageBox.Show(StringsDict["ERROR:-_Could_not_open"] + " TG_Lists.csv. " + StringsDict["May_be_in_use_by_another_program"]);
			return false;
		}
		return true;
	}

	private static bool OpenTGListsCSV()
	{
		string text = CSVName + "TG_Lists.csv";
		int num = (append ? TGListCount : 0);
		if (File.Exists(text))
		{
			if (isFileLocked(text))
			{
				MessageBox.Show(StringsDict["ERROR:-_Could_not_open"] + " TG_Lists.csv. " + StringsDict["May_be_in_use_by_another_program"]);
				return false;
			}
			string text2 = CheckDelimiter(text);
			if (text2 == "")
			{
				return true;
			}
			TextFieldParser textFieldParser = new TextFieldParser(text);
			textFieldParser.TextFieldType = FieldType.Delimited;
			textFieldParser.SetDelimiters(text2);
			string[] array = textFieldParser.ReadFields();
			int num2 = array.Length - 1;
			int num3 = ((num2 <= 32) ? num2 : 32);
			while (!textFieldParser.EndOfData && num < 76)
			{
				try
				{
					array = textFieldParser.ReadFields();
					if (array.Length < 33)
					{
						MessageBox.Show(StringsDict["Format_error_in"] + " TG_Lists.csv. " + StringsDict["Not_enough_columns_in_line"] + (textFieldParser.LineNumber - 1));
						textFieldParser.Close();
						return false;
					}
					int num4 = num;
					if (append)
					{
						int num5 = LookupTGList(Strip(array[0]));
						if (num5 > 0)
						{
							num4 = num5 - 1;
						}
					}
					TGListName[num4] = Strip(array[0]);
					for (int i = 0; i < num3; i++)
					{
						int num6 = LookupContact(Strip(array[i + 1]));
						TGListMember[num4, i] = num6;
					}
					if (num3 < 32)
					{
						for (int j = num2; j < 32; j++)
						{
							TGListMember[num4, j] = 0;
						}
					}
					if (num4 == num && TGListName[num] != "")
					{
						num++;
					}
				}
				catch (Exception)
				{
					MessageBox.Show(StringsDict["Line"] + " " + (textFieldParser.LineNumber - 1) + " " + StringsDict["is_not_valid_in"] + " TG-Lists.csv " + StringsDict["and_will_be_skipped"]);
					textFieldParser.Close();
					return false;
				}
			}
			TGListCount = num;
			if (num < 76)
			{
				for (int k = num; k < 76; k++)
				{
					TGListName[k] = "";
					for (int l = 0; l < 32; l++)
					{
						TGListMember[k, l] = 0;
					}
				}
			}
			textFieldParser.Close();
		}
		return true;
	}

	private static bool SaveZonesCSV()
	{
		string path = CSVName + "Zones.csv";
		string text = "Zone Name";
		Zonecount = 0;
		for (int i = 1; i <= 80; i++)
		{
			text = text + writeSeparator + "Channel" + i;
		}
		text += Environment.NewLine;
		for (int j = 0; j < 68; j++)
		{
			if (ZoneName[j] != "")
			{
				Zonecount++;
				string zone = ZoneName[j];
                text = text + NumberFormatting(zone) + writeSeparator;
				for (int k = 0; k < 80; k++)
				{
					if (ZoneMember[j, k] <= 0)
					{ 
						text += writeSeparator;
					}
					else
					{
						string name = ChannelName[ZoneMember[j, k] - 1];
                        text += NumberFormatting(name) + writeSeparator;
					};
				}
				text = text.Substring(0, text.Length - 1);
				text += Environment.NewLine;
			}
		}
		try
		{
			File.WriteAllText(path, text);
		}
		catch
		{
			MessageBox.Show(StringsDict["ERROR:-_Could_not_open"] + " Zones.csv. " + StringsDict["May_be_in_use_by_another_program"]);
			return false;
		}
		return true;
	}


    private static bool OpenZonesCSV()
	{
		string text = CSVName + "Zones.csv";
		int num = (append ? Zonecount : 0);
		if (File.Exists(text))
		{
			if (isFileLocked(text))
			{
				MessageBox.Show(StringsDict["ERROR:-_Could_not_open"] + " Zones.csv. " + StringsDict["May_be_in_use_by_another_program"]);
				return false;
			}
			string text2 = CheckDelimiter(text);
			if (text2 == "")
			{
				return true;
			}
			TextFieldParser textFieldParser = new TextFieldParser(text);
			textFieldParser.TextFieldType = FieldType.Delimited;
			textFieldParser.SetDelimiters(text2);
			string[] array = textFieldParser.ReadFields();
			while (!textFieldParser.EndOfData && num < 68)
			{
				try
				{
					array = textFieldParser.ReadFields();
					if (array.Length < 81)
					{
						MessageBox.Show(StringsDict["Format_error_in"] + " Zones.csv. " + StringsDict["Not_enough_columns_in_line"] + (textFieldParser.LineNumber - 1));
						textFieldParser.Close();
						return false;
					}
					int num2 = num;
					if (append)
					{
						int num3 = LookupZone(Strip(array[0]));
						if (num3 > 0)
						{
							num2 = num3 - 1;
						}
					}
					ZoneName[num2] = Strip(array[0]);
					for (int i = 0; i < 80; i++)
					{
						ZoneMember[num2, i] = Lookupchannel(Strip(array[i + 1]));
					}
					if (num2 == num && ZoneName[num] != "")
					{
						num++;
					}
				}
				catch
				{
					MessageBox.Show(StringsDict["Line"] + " " + (textFieldParser.LineNumber - 1) + " " + StringsDict["is_not_valid_in"] + " Zones.csv " + StringsDict["and_will_be_skipped"]);
					textFieldParser.Close();
					return false;
				}
			}
			Zonecount = num;
			if (num < 68)
			{
				for (int j = num; j < 68; j++)
				{
					ZoneName[j] = "";
					for (int i = 0; i < 80; i++)
					{
						ZoneMember[j, i] = 0;
					}
				}
			}
			textFieldParser.Close();
		}
		return true;
	}



	private static bool SaveChannelsCSV()
	{
		
        string path = CSVName + "Channels.csv";
		string text = "Channel Number" + writeSeparator + "Channel Name" + writeSeparator + "Channel Type" + writeSeparator + "Rx Frequency" + writeSeparator + "Tx Frequency" + writeSeparator + "Bandwidth (kHz)" + writeSeparator + "Colour Code" + writeSeparator + "Timeslot" + writeSeparator + "Contact" + writeSeparator + "TG List" + writeSeparator + "DMR ID" + writeSeparator + "TS1_TA_Tx" + writeSeparator + "TS2_TA_Tx ID" + writeSeparator + "RX Tone" + writeSeparator + "TX Tone" + writeSeparator + "Squelch" + writeSeparator + "Power" + writeSeparator + "Rx Only" + writeSeparator + "Zone Skip" + writeSeparator + "All Skip" + writeSeparator + "TOT" + writeSeparator + "VOX" + writeSeparator + "No Beep" + writeSeparator + "No Eco" + writeSeparator + "APRS" + writeSeparator + "Latitude" + writeSeparator + "Longitude" + writeSeparator + "Use Location" + writeSeparator + "Fastcall" + Environment.NewLine;
		ChannelCount = 0;
		for (int i = 0; i < 1024; i++)
		{
/*			if (ChannelForm.data.DataIsValid(i) && ChannelName[i] != ChannelForm.data[i].Name)
			{
                MessageBox.Show(StringsDict["mismatch"] + ChannelForm.data[i].Name);
			}*/
			if (ChannelForm.data.DataIsValid(i))
			{
				ChannelCount++;
                
                text = text + (i + 1) + writeSeparator;
				text = text + NumberFormatting(convert1251toUTF8(ChannelForm.data[i].Name)) + writeSeparator;
				text = ((ChannelByte[i, 8] != 1) ? (text + "Analogue") : (text + "Digital"));
				text += writeSeparator;
				text = text + DecodeFrequency(ChannelByte[i, 0], ChannelByte[i, 1], ChannelByte[i, 2], ChannelByte[i, 3]) + writeSeparator;
				text = text + DecodeFrequency(ChannelByte[i, 4], ChannelByte[i, 5], ChannelByte[i, 6], ChannelByte[i, 7]) + writeSeparator;
				if (ChannelByte[i, 8] == 1)
				{
					text = text + N_A_VALUE + writeSeparator;
					text = text + ChannelByte[i, 28] + writeSeparator;
					text = (((ChannelByte[i, 33] & 0x40) == 0) ? (text + "1") : (text + "2"));
					text += writeSeparator;
					int num = ChannelByte[i, 30] + ChannelByte[i, 31] * 256;
					text = ((num <= 0 || num >= ContactName.Length) ? (text + "None") : (text + NumberFormatting(ContactName[num - 1])));
					text += writeSeparator;
					text = ((ChannelByte[i, 27] <= 0) ? (text + "None") : (text + NumberFormatting(TGListName[ChannelByte[i, 27] - 1])));
					text += writeSeparator;
					long num2 = ChannelByte[i, 25] + ChannelByte[i, 24] * 256 + ChannelByte[i, 23] * 65536;
					text = ((num2 <= 0 || (ChannelByte[i, 22] & 0x80) == 0) ? (text + "None") : (text + num2));
					text += writeSeparator;
					string text2 = DecodeTATx(ChannelByte[i, 32] & 3);
					string text3 = DecodeTATx((ChannelByte[i, 32] & 0xC) >> 2);
					text = text + text2 + writeSeparator;
					text = text + text3 + writeSeparator;
					text = text + N_A_VALUE + writeSeparator;
					text = text + N_A_VALUE + writeSeparator;
					text = text + N_A_VALUE + writeSeparator;
				}
				else
				{
					text = (((ChannelByte[i, 35] & 2) == 0) ? (text + "12" + decimalSeparator + "5") : (text + "25"));
					text += writeSeparator;
					text = text + N_A_VALUE + writeSeparator;
					text = text + N_A_VALUE + writeSeparator;
					text = text + N_A_VALUE + writeSeparator;
					text = text + N_A_VALUE + writeSeparator;
					text = text + N_A_VALUE + writeSeparator;
					text = text + N_A_VALUE + writeSeparator;
					text = text + N_A_VALUE + writeSeparator;
					text = text + DecodeTone(ChannelByte[i, 16], ChannelByte[i, 17]) + writeSeparator;
					text = text + DecodeTone(ChannelByte[i, 18], ChannelByte[i, 19]) + writeSeparator;
					text = text + DecodeSquelch(ChannelByte[i, 39]) + writeSeparator;
				}
				text = text + DecodePower(ChannelByte[i, 9]) + writeSeparator;
				text = (((ChannelByte[i, 35] & 4) == 0) ? (text + "No") : (text + "Yes"));
				text += writeSeparator;
				text = (((ChannelByte[i, 35] & 0x20) == 0) ? (text + "No") : (text + "Yes"));
				text += writeSeparator;
				text = (((ChannelByte[i, 35] & 0x10) == 0) ? (text + "No") : (text + "Yes"));
				text += writeSeparator;
				text = text + DecodeTot(ChannelByte[i, 11]) + writeSeparator;
				text = (((ChannelByte[i, 35] & 0x40) == 0) ? (text + "Off") : (text + "On"));
				text += writeSeparator;
				text = (((ChannelByte[i, 22] & 0x40) == 0) ? (text + "No") : (text + "Yes"));
				text += writeSeparator;
				text = (((ChannelByte[i, 22] & 0x20) == 0) ? (text + "No") : (text + "Yes"));
				text += writeSeparator;
				text = ((ChannelForm.data[i].APRS_System == 0) ? (text + "None") : (text + APRSForm.data[ChannelForm.data[i].APRS_System - 1].Name));
				text += writeSeparator;
				text = text + DecodeLatLon(ChannelByte[i, 13], ChannelByte[i, 12], ChannelByte[i, 10]) + writeSeparator;
				text = text + DecodeLatLon(ChannelByte[i, 20], ChannelByte[i, 15], ChannelByte[i, 14]) + writeSeparator;
				text = (((ChannelByte[i, 22] & 8) == 0) ? (text + "No") : (text + "Yes")) + writeSeparator;
                text = (((ChannelByte[i, 21] & 0x80) == 0) ? (text + "No") : (text + "Yes"));
                text += writeSeparator;
                text += Environment.NewLine;
			}
		}
		try
		{
			File.WriteAllText(path, text);
		}
		catch
		{
			MessageBox.Show(StringsDict["ERROR:-_Could_not_open"] + " Channels.csv. " + StringsDict["May_be_in_use_by_another_program"]);
			return false;
		}
		return true;
	}

	private static bool OpenChannelsCSV()
	{
		string text = CSVName + "Channels.csv";
		int num = 0;
		int num2;
		if (append)
		{
			CompactChannels();
			num2 = ChannelCount;
			ChannelCount = 0;
		}
		else
		{
			num2 = 0;
			ChannelCount = 0;
		}
		if (File.Exists(text))
		{
			if (isFileLocked(text))
			{
				MessageBox.Show(StringsDict["ERROR:-_Could_not_open"] + " Channels.csv. " + StringsDict["May_be_in_use_by_another_program"]);
				return false;
			}
			string text2 = CheckDelimiter(text);
			if (text2 == "")
			{
				return true;
			}
			TextFieldParser textFieldParser = new TextFieldParser(text);
			textFieldParser.TextFieldType = FieldType.Delimited;
			textFieldParser.SetDelimiters(text2);
			textFieldParser.ReadFields();
			if (!append)
			{
				for (int i = 0; i < ChannelForm.data.Count; i++)
				{
					ChannelName[i] = "";
					for (int j = 0; j < 40; j++)
					{
						ChannelByte[i, j] = 0;

					}
				}
			}
			while (!textFieldParser.EndOfData && num2 < ChannelForm.data.Count)
			{
				try
				{
					string[] array = textFieldParser.ReadFields();
					if (array.Length < 22)
					{
						MessageBox.Show(StringsDict["Format_error_in"] + " Channels.csv. " + StringsDict["Not_enough_columns_in_line"] + (textFieldParser.LineNumber - 1));
						textFieldParser.Close();
						return false;
					}
					num = num2;
					if (append)
					{
						int num3 = Lookupchannel(Strip(array[1]));
						if (num3 > 0)
						{
							num = num3 - 1;
						}
					}
					if (!append)
					{
						num = int.Parse(array[0]) - 1;
						if (num >= ChannelForm.data.Count)
						{
							throw new Exception();
						}
					}
					ChannelName[num] = Strip(array[1]);
					if (array[2] == "Digital")
					{
						ChannelByte[num, 8] = 1;
					}
					else
					{
						ChannelByte[num, 8] = 0;
					}
					array[3] = array[3].Replace(altDecimalSeperator, decimalSeparator);
					EncodeFrequency(double.Parse(array[3]), ref ChannelByte[num, 0], ref ChannelByte[num, 1], ref ChannelByte[num, 2], ref ChannelByte[num, 3]);
					array[4] = array[4].Replace(altDecimalSeperator, decimalSeparator);
					EncodeFrequency(double.Parse(array[4]), ref ChannelByte[num, 4], ref ChannelByte[num, 5], ref ChannelByte[num, 6], ref ChannelByte[num, 7]);
					if (array[6] != N_A_VALUE)
					{
						ChannelByte[num, 28] = (byte)int.Parse(array[6]);
					}
					else
					{
						ChannelByte[num, 28] = 0;
					}
					int num4 = LookupContact(Strip(array[8]));
					ChannelByte[num, 30] = (byte)(num4 % 256);
					ChannelByte[num, 31] = (byte)((double)num4 / 256.0);
					ChannelByte[num, 27] = (byte)LookupTGList(Strip(array[9]));
					int num5 = ((!(array[10] == "None") && !(array[10] == N_A_VALUE)) ? int.Parse(array[10]) : 0);
					ChannelByte[num, 23] = (byte)((num5 >> 16) & 0xFF);
					ChannelByte[num, 24] = (byte)((num5 >> 8) & 0xFF);
					ChannelByte[num, 25] = (byte)(num5 & 0xFF);
					if (array[11] != N_A_VALUE)
					{
						ChannelByte[num, 32] |= (byte)EncodeTATx(array[11]);
					}
					if (array[12] != N_A_VALUE)
					{
						ChannelByte[num, 32] |= (byte)(EncodeTATx(array[12]) << 2);
					}
					EncodeTone(array[13], ref ChannelByte[num, 16], ref ChannelByte[num, 17]);
					EncodeTone(array[14], ref ChannelByte[num, 18], ref ChannelByte[num, 19]);
					ChannelByte[num, 39] = EncodeSquelch(array[15]);
					ChannelByte[num, 9] = EncodePower(array[16]);
					ChannelByte[num, 11] = EncodeTOT(array[20]);
					ChannelByte[num, 33] &= 191;
					if (array[7] == "2")
					{
						ChannelByte[num, 33] |= 64;
					}
					ChannelByte[num, 35] &= 137;
					if (array[21] == "On")
					{
						ChannelByte[num, 35] |= 64;
					}
					if (array[5] == "25")
					{
						ChannelByte[num, 35] |= 2;
					}
					if (array[17] == "Yes")
					{
						ChannelByte[num, 35] |= 4;
					}
					if (array[18] == "Yes")
					{
						ChannelByte[num, 35] |= 32;
					}
					if (array[19] == "Yes")
					{
						ChannelByte[num, 35] |= 16;
					}
					ChannelByte[num, 22] &= 31;
					if (num5 > 0)
					{
						ChannelByte[num, 22] |= 128;
					}
					if (array[22] == "Yes")
					{
						ChannelByte[num, 22] |= 64;
					}
					if (array[23] == "Yes")
					{
						ChannelByte[num, 22] |= 32;
					}
					if (array[24] != "None")
					{
						ChannelByte[num, 29] = (byte)LookupAPRS(Strip(array[24]));
					}
					else
					{
						ChannelByte[num, 29] = 0;
					}
					if (Strip(array[25]) != "" && Strip(array[26]) != "")
					{
						uint num6 = EncodeLatLon(Strip(array[25]));
						ChannelByte[num, 10] = (byte)(num6 & 0xFF);
						ChannelByte[num, 12] = (byte)((num6 >> 8) & 0xFF);
						ChannelByte[num, 13] = (byte)((num6 >> 16) & 0xFF);
						num6 = EncodeLatLon(Strip(array[26]));
						ChannelByte[num, 14] = (byte)(num6 & 0xFF);
						ChannelByte[num, 15] = (byte)((num6 >> 8) & 0xFF);
						ChannelByte[num, 20] = (byte)((num6 >> 16) & 0xFF);
					}
					if (array[27] == "Yes")
					{
						ChannelByte[num, 22] |= 8;
					}
					try
					{
						if (array[28] == "Yes")
						{
							ChannelByte[num, 21] |= 128;
						}
					}
					catch (Exception)
					{ }
                }
				catch (Exception)
				{
					MessageBox.Show(StringsDict["Line"] + " " + (textFieldParser.LineNumber - 1) + " " + StringsDict["is_not_valid_in"] + " Channels.csv " + StringsDict["and_will_be_skipped"]);
					textFieldParser.Close();
					return false;
				}
				if (num == num2 && ChannelName[num2] != "")
				{
					num2++;
				}
				if (append && num2 == num && ChannelName[num2] != "")
				{
					num2++;
				}
			}
			textFieldParser.Close();
		}
		ChannelCount = countchannels();
		return true;
	}

	private static bool OpenLocationsCSV(string filename)
	{
		if (File.Exists(filename))
		{
			if (isFileLocked(filename))
			{
				MessageBox.Show(StringsDict["ERROR:-_Could_not_open"] + filename + StringsDict["May_be_in_use_by_another_program"]);
				return false;
			}
			string text = CheckDelimiter(filename);
			if (text == "")
			{
				return true;
			}
			TextFieldParser textFieldParser = new TextFieldParser(filename);
			textFieldParser.TextFieldType = FieldType.Delimited;
			textFieldParser.SetDelimiters(text);
			textFieldParser.ReadFields();
			while (!textFieldParser.EndOfData)
			{
				try
				{
					string[] array = textFieldParser.ReadFields();
					if (array.Length < 2)
					{
						MessageBox.Show(StringsDict["Format_error_in"] + " csv. " + StringsDict["Not_enough_columns_in_line"] + (textFieldParser.LineNumber - 1));
						textFieldParser.Close();
						return false;
					}
					string text2 = Strip(array[0]).ToUpper();
					if (!(text2 != ""))
					{
						continue;
					}
					for (int i = 0; i < 1024; i++)
					{
						if (!ChannelForm.data.DataIsValid(i) || ChannelForm.data[i].Name.ToUpper().IndexOf(text2) == -1)
						{
							continue;
						}
						try
						{
							float num = float.Parse(array[1].Trim());
							float num2 = float.Parse(array[2].Trim());
							if (num != 0f || num2 != 0f)
							{
								ChannelForm.data.chList[i].Latitude = array[1];
								ChannelForm.data.chList[i].Longitude = array[2];
							}
						}
						catch
						{
						}
					}
				}
				catch (Exception)
				{
					MessageBox.Show(StringsDict["Line"] + " " + (textFieldParser.LineNumber - 1) + " " + StringsDict["is_not_valid_in"] + " csv " + StringsDict["and_will_be_skipped"]);
					textFieldParser.Close();
					return false;
				}
			}
			textFieldParser.Close();
		}
		return true;
	}

	private static string CheckDelimiter(string fname)
	{
		int num = 0;
		int num2 = 0;
		string[] array = File.ReadAllLines(fname);
		if (array.Length < 2)
		{
			return "";
		}
		for (int i = 0; i < 2; i++)
		{
			string text;
			try
			{
				text = array[i];
			}
			catch
			{
				text = ";;;;;;";
			}
			num2 += text.Split(',').Length - 1;
			num += text.Split(';').Length - 1;
		}
		if (num2 > num)
		{
			return ",";
		}
		return ";";
	}

	private static int countchannels()
	{
		int num = 0;
		for (int i = 0; i < 1024; i++)
		{
			if (ChannelName[i] != "")
			{
				num++;
			}
		}
		return num;
	}

	private static void CompactChannels()
	{
		int num = 1024;
		int num2 = 0;
		int num3 = 1;
		while (num3 < num)
		{
			num2 = FindNextChannel(num2, 0);
			num3 = FindNextChannel(num2, 1);
			if (num2 < num - 1 && num3 < num)
			{
				ChannelName[num2] = ChannelName[num3];
				ChannelName[num3] = "";
				for (int i = 0; i < 40; i++)
				{
					ChannelByte[num2, i] = ChannelByte[num3, i];
					ChannelByte[num3, i] = 0;
				}
				num2++;
				num3++;
			}
		}
		ChannelCount = FindNextChannel(0, 0);
	}

	private static int FindNextChannel(int p, byte occupied)
	{
		int i;
		for (i = p; i < 1024 && (occupied != 0 || !(ChannelName[i] == "")) && (occupied != 1 || !(ChannelName[i] != "")); i++)
		{
		}
		return i;
	}

	private static int LookupContact(string name)
	{
		if (name == "" || name == N_A_VALUE)
		{
			return 0;
		}
		for (int i = 0; i <= ContactForm.data.Count; i++)
		{
			if (ContactName[i] == name)
			{
				return i + 1;
			}
		}
		return 0;
	}

	private static int LookupDTMF(string name)
	{
		int result = 0;
		for (int i = 0; i <= 63; i++)
		{
			if (DTMFName[i] == name)
			{
				result = i + 1;
			}
		}
		if (name == "")
		{
			result = 0;
		}
		return result;
	}

	private static int LookupAPRS(string name)
	{
		name = name.ToUpper();
		for (int i = 0; i < APRSForm.data.Count; i++)
		{
			if (aprsConfigs.DataIsValid(i) && aprsConfigs.APRS_list[i].Name.ToUpper() == name)
			{
				return i + 1;
			}
		}
		return 0;
	}

	private static int LookupTGList(string name)
	{
		int result = 0;
		if (name == "")
		{
			return 0;
		}
		for (int i = 0; i <= 76; i++)
		{
			if (TGListName[i] == name)
			{
				result = i + 1;
			}
		}
		return result;
	}

	private static int Lookupchannel(string name, bool acceptPartial = false)
	{
		int result = 0;
		for (int i = 0; i < 1024; i++)
		{
			if (acceptPartial)
			{
				if (ChannelName[i].IndexOf(name) != -1)
				{
					result = i + 1;
				}
			}
			else if (ChannelName[i] == name)
			{
				result = i + 1;
			}
		}
		if (name == "")
		{
			result = 0;
		}
		return result;
	}

	private static int LookupZone(string name)
	{
		int result = 0;
		for (int i = 0; i <= 68; i++)
		{
			if (ZoneName[i] == name)
			{
				result = i + 1;
			}
		}
		if (name == "")
		{
			result = 0;
		}
		return result;
	}

	private static bool ReadCSVs(string PathName)
	{
		CSVName = PathName;
		if (!OpenContactsCSV())
		{
			return false;
		}
		if (!OpenTGListsCSV())
		{
			return false;
		}
		if (!OpenAPRSCSV())
		{
			return false;
		}
		if (!OpenChannelsCSV())
		{
			return false;
		}
		if (!OpenZonesCSV())
		{
			return false;
		}
		if (!OpenDTMFCSV())
		{
			return false;
		}
		InsertContacts();
		InsertTGLists();
		InsertChannels();
		InsertZones();
		InsertDTMFs();
		return true;
	}

	private static bool WriteCSVs(string PathName)
	{
		CSVName = PathName;
		if (!SaveContactsCSV())
		{
			return false;
		}
		if (!SaveTGListsCSV())
		{
			return false;
		}
		if (!SaveChannelsCSV())
		{
			return false;
		}
		if (!SaveZonesCSV())
		{
			return false;
		}
		if (!SaveDTMFCSV())
		{
			return false;
		}
		if (!SaveAPRSCSV())
		{
			return false;
		}
		return true;
	}

	private static void Filestats(string header, string comment)
	{
		MessageBox.Show(header + Environment.NewLine + Environment.NewLine + comment + Environment.NewLine + Environment.NewLine + StringsDict["Contacts"] + ": " + ContactCount + Environment.NewLine + StringsDict["TG_Lists"] + ": " + TGListCount + Environment.NewLine + StringsDict["Channels"] + ": " + ChannelCount + Environment.NewLine + StringsDict["Zones"] + ": " + Zonecount + Environment.NewLine + StringsDict["DTMF_Contacts"] + ": " + DTMFCount + Environment.NewLine + "APRS" + ": " + APRSCount  + Environment.NewLine);
	}

    private static string convert1251toUTF8(string input)
    {
        Encoding encodingIn = Encoding.GetEncoding(1251);
		Encoding encodingOut = Encoding.Unicode;
        byte[] sourceBuffer = encodingIn.GetBytes(input);
        byte[] destBuffer = Encoding.Convert(encodingIn, encodingOut, sourceBuffer);
        string temp = encodingOut.GetString(destBuffer);
        return temp;
    }

    private static string convertUTF8to1251(string input)
    {
        Encoding encodingIn = Encoding.GetEncoding("utf-8");
        Encoding encodingOut = Encoding.GetEncoding("windows-1251");
        byte[] sourceBuffer = encodingIn.GetBytes(input);
        byte[] destBuffer = Encoding.Convert(encodingIn, encodingOut, sourceBuffer);
        string temp = encodingOut.GetString(destBuffer);
        return temp;
    }
}
