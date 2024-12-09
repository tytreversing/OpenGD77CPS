using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using DMR;
using WeifenLuo.WinFormsUI.Docking;

internal class Settings
{
	public enum UserMode
	{
		Basic,
		Expert
	}

	public const string SZ_PWD = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz\b";

	public const string SZ_HEX = "0123456789ABCDEF";

	public const string SZ_ATTRIBUTE_NAME = "Id";

	public const string SZ_ATTRIBUTE_VALUE = "Text";

	public const string SZ_NONE_NAME = "None";

	public const string SZ_SELECTED_NAME = "Selected";

	public const string SZ_ADD_NAME = "Add";

	public const string SZ_OFF_NAME = "Off";

	public const string SZ_DEVICE_NOT_FOUND_N = "DeviceNotFound";

	public const string SZ_OPEN_PORT_FAIL_NAME = "OpenPortFail";

	public const string SZ_COMM_ERROR_N = "CommError";

	public const string SZ_MODEL_NOT_MATCH_N = "Model does not match";

	public const string SZ_READ_NAME = "Read";

	public const string SZ_WRITE_NAME = "Write";

	public const string SZ_READ_COMPLETE_NAME = "ReadComplete";

	public const string SZ_WRITE_COMPLETE_NAME = "WriteComplete";

	public const string SZ_KEYPRESS_DTMF_NAME = "KeyPressDtmf";

	public const string SZ_KEYPRESS_HEX_NAME = "KeyPressHex";

	public const string SZ_KEYPRESS_DIGIT_N = "KeyPressDigit";

	public const string SZ_KEYPRESS_PRINT_N = "KeyPressPrint";

	public const string SZ_DATA_FORMAT_ERROR_N = "DataFormatError";

	public const string SZ_FIRST_CH_NOT_DELETE_NAME = "FirstChNotDelete";

	public const string SZ_FIRST_NOT_DELETE_N = "FirstNotDelete";

	public const string SZ_NAME_EXIST = "Name exists";

	public const string SZ_FILE_FORMAT_ERROR_N = "FileFormatError";

	public const string SZ_OPEN_SUCCESSFULLY_N = "OpenSuccessfully";

	public const string SZ_SAVE_SUCCESSFULLY_N = "SaveSuccessfully";

	public const string SZ_TYPE_NOT_MATCH_N = "TypeNotMatch";

	public const string SZ_EXPORT_SUCCESS_N = "ExportSuccess";

	public const string SZ_IMPORT_SUCCESS_N = "ImportSuccess";

	public const string SZ_ID_NOT_EMPTY_N = "IdNotEmpty";

	public const string SZ_ID_OUT_OF_RANGE_N = "IdOutOfRange";

	public const string SZ_ID_ALREADY_EXISTS_N = "IdAlreadyExists";

	public const string SZ_NOT_SELECT_ITEM_NOT_COPYITEM_NAME = "NotSelectItemNotCopyItem";

	public const string SZ_PROMPT_KEY1_NAME = "PromptKey1";

	public const string SZ_PROMPT_KEY2_NAME = "PromptKey2";

	public const string SZ_APP_SCAN_SELECTED_NAME = "ScanSelected";

	public const string SZ_UNABLE = "Unable to operate selected";

	public const string SZ_PROMPT_NAME = "Prompt";

	public const string SZ_ERROR_NAME = "Error";

	public const string SZ_WARNING_NAME = "Warning";

	public const int FREQ_STEP_1 = 250;

	public const int FREQ_STEP_2 = 625;

	public const string SUPER_PWD = "DT8168";

	public const int LEN_NAME_MCU = 15;

	public static readonly string[] TONES_LIST;

	public static readonly byte[] CUR_MODEL;

	public static string SZ_NONE;

	public static string SZ_NA;

	public static string SZ_SELECTED;

	public static string SZ_ADD;

	public static string SZ_OFF;

	public static string SZ_DEVICE_NOT_FOUND;

	public static string SZ_OPEN_PORT_FAIL;

	public static string SZ_COMM_ERROR;

	public static string SZ_MODEL_NOT_MATCH;

	public static string SZ_READ;

	public static string SZ_WRITE;

	public static string SZ_READ_COMPLETE;

	public static string SZ_WRITE_COMPLETE;

	public static string SZ_CODEPLUG_READ_CONFIRM;

	public static string SZ_CODEPLUG_WRITE_CONFIRM;

	public static string SZ_PLEASE_CONFIRM;

	public static string SZ_USER_AGREEMENT;

	public static string SZ_KEYPRESS_DTMF;

	public static string SZ_KEYPRESS_HEX;

	public static string SZ_KEYPRESS_DIGIT;

	public static string SZ_KEYPRESS_PRINT;

	public static string SZ_DATA_FORMAT_ERROR;

	public static string SZ_FIRST_CH_NOT_DELETE;

	public static string SZ_FIRST_NOT_DELETE;

	public static string SZ_NAME_EXIST_NAME;

	public static string SZ_FILE_FORMAT_ERROR;

	public static string SZ_OPEN_SUCCESSFULLY;

	public static string SZ_SAVE_SUCCESSFULLY;

	public static string SZ_TYPE_NOT_MATCH;

	public static string SZ_EXPORT_SUCCESS;

	public static string SZ_IMPORT_SUCCESS;

	public static string SZ_ID_NOT_EMPTY;

	public static string SZ_ID_OUT_OF_RANGE;

	public static string SZ_ID_ALREADY_EXISTS;

	public static string SZ_NOT_SELECT_ITEM_NOT_COPYITEM;

	public static string SZ_PROMPT_KEY1;

	public static string SZ_PROMPT_KEY2;

	public static string SZ_PROMPT;

	public static string SZ_ERROR;

	public static string SZ_WARNING;

	public static string SZ_DOWNLOADCONTACTS_REGION_EMPTY;

	public static string SZ_DOWNLOADCONTACTS_MESSAGE_ADDED;

	public static string SZ_DOWNLOADCONTACTS_DOWNLOADING;

	public static string SZ_DOWNLOADCONTACTS_SELECT_CONTACTS_TO_IMPORT;

	public static string SZ_DOWNLOADCONTACTS_TOO_MANY;

	public static string SZ_UNABLEDOWNLOADFROMINTERNET;

	public static string SZ_IMPORT_COMPLETE;

	public static string SZ_CODEPLUG_UPGRADE_NOTICE;

	public static string SZ_CODEPLUG_UPGRADE_WARNING_TO_MANY_RX_GROUPS;

	public static string SZ_CODEPLUG_READ;

	public static string SZ_CODEPLUG_WRITE;

	public static string SZ_DMRID_READ;

	public static string SZ_DMRID_WRITE;

	public static string SZ_CALIBRATION_READ;

	public static string SZ_CALIBRATION_WRITE;

	public static string SZ_CONTACT_DUPLICATE_NAME;

	public static string SZ_EnableMemoryAccessMode;

	public static string SZ_dataRead;

	public static string SZ_dataWrite;

	public static string SZ_DMRIdContcatsTotal;

	public static string SZ_ErrorParsingData;

	public static string SZ_DMRIdIntroMessage;

	public static int CUR_MODE;

	public static readonly uint[] MIN_FREQ;

	public static readonly uint[] MAX_FREQ;

	public static readonly uint[] VALID_MIN_FREQ;

	public static readonly uint[] VALID_MAX_FREQ;

	public static int CUR_CH_GROUP;

	public static int CUR_ZONE_GROUP;

	public static int CUR_ZONE;

	public static string CUR_PWD;

	public static readonly uint EEROM_SPACE;

	public static readonly int SPACE_DEVICE_INFO;

	public static readonly int ADDR_DEVICE_INFO;

	public static readonly int OFS_LAST_PRG_TIME;

	public static readonly int OFS_CPS_SW_VER;

	public static readonly int OFS_MODEL;

	public static readonly int SPACE_GENERAL_SET;

	public static readonly int ADDR_GENERAL_SET;

	public static readonly int ADDR_PWD;

	public static readonly int SPACE_BUTTON;

	public static readonly int ADDR_BUTTON;

	public static readonly int SPACE_ONE_TOUCH;

	public static readonly int ADDR_ONE_TOUCH;

	public static readonly int SPACE_TEXT_MSG;

	public static readonly int ADDR_TEXT_MSG;

	public static readonly int SPACE_ENCRYPT;

	public static readonly int ADDR_ENCRYPT;

	public static readonly int SPACE_SIGNALING_BASIC;

	public static readonly int ADDR_SIGNALING_BASIC;

	public static readonly int SPACE_DTMF_BASIC;

	public static readonly int ADDR_DTMF_BASIC;

	public static readonly int SPACE_APRS_SYSTEM;

	public static readonly int ADDR_APRS_SYSTEM;

	public static readonly int SPACE_DMR_CONTACT;

	public static readonly int ADDR_DMR_CONTACT;

	public static readonly int SPACE_DMR_CONTACT_EX;

	public static readonly int ADDR_DMR_CONTACT_EX;

	public static readonly int SPACE_DTMF_CONTACT;

	public static readonly int ADDR_DTMF_CONTACT;

	public static readonly int SPACE_RX_GRP_LIST;

	public static readonly int ADDR_RX_GRP_LIST_EX;

	public static readonly int ADDR_ZONE_BASIC;

	public static readonly int ADDR_ZONE_LIST;

	public static readonly int ADDR_CHANNEL;

	public static readonly int SPACE_SCAN_BASIC;

	public static readonly int ADDR_SCAN;

	public static readonly int SPACE_SCAN_LIST;

	public static readonly int ADDR_SCAN_LIST;

	public static readonly int SPACE_BOOT_ITEM;

	public static readonly int ADDR_BOOT_ITEM;

	public static readonly int SPACE_DIGITAL_KEY_CONTACT;

	public static readonly int ADDR_DIGITAL_KEY_CONTACT;

	public static readonly int SPACE_MENU_CONFIG;

	public static readonly int ADDR_MENU_CONFIG;

	public static readonly int SPACE_BOOT_CONTENT;

	public static readonly int ADDR_BOOT_CONTENT;

	public static readonly int SPACE_ATTACHMENT;

	public static readonly int ADDR_ATTACHMENT;

	public static readonly int SPACE_VFO;

	public static readonly int ADDR_VFO;

	public static readonly int SPACE_EX_ZONE;

	public static readonly int ADDR_EX_ZONE;

	public static readonly int ADDR_EX_ZONE_BASIC;

	public static readonly int ADDR_EX_ZONE_LIST;

	public static readonly int SPACE_EX_SCAN;

	public static readonly int ADDR_EX_SCAN;

	public static readonly int ADDR_EX_SCAN_PRI_CH1;

	public static readonly int ADDR_EX_SCAN_PRI_CH2;

	public static readonly int ADDR_EX_SCAN_SPECIFY_CH;

	public static readonly int ADDR_EX_SCAN_CH_LIST;

	public static readonly int SPACE_EX_EMERGENCY;

	public static readonly int ADDR_EX_EMERGENCY;

	public static readonly int SPACE_EX_CH;

	public static readonly int ADDR_EX_CH;

	public static readonly int ADDR_OPENGD77_CUSTOM_DATA_START;

	public static readonly int ADDR_OPENGD77_CUSTOM_DATA_END;

	public static readonly int ADDR_OPENGD77_LAST_USED_CHANNELS_DATA_START;

	public static readonly int LAST_USED_CHANNEL_IN_ZONE_SIZE;

	public static readonly int LAST_USED_CHANNEL_IN_ZONE_BLOCK_SIZE;

	public static readonly int ADDR_OPENGD77_LAST_USED_CHANNELS_DATA_END;

	public static readonly int ADDR_OPENGD77_SETTINGS_START;

	public static Dictionary<string, string> dicCommon;

	private static XmlDocument _languageXML;

	public static string LanguageFile;

	private static string HelpFilename;

	private static SizeF _003CFactor_003Ek__BackingField;

	private static string _003CCurUserPwd_003Ek__BackingField;

	public static XmlDocument languageXML => _languageXML;

	public static void setLanguageXMLFile(string xmlFile)
	{
		LanguageFile = Path.GetFileName(xmlFile);
		_languageXML = new XmlDocument();
		_languageXML.Load(xmlFile);
	}

	[CompilerGenerated]
	public static string GetHelpFilename()
	{
		return HelpFilename;
	}

	[CompilerGenerated]
	public static void SetHelpFilename(string string_0)
	{
		HelpFilename = string_0;
	}

	public static UserMode getUserExpertSettings()
	{
		return UserMode.Expert;
	}

	public static void smethod_5(UserMode userMode_0)
	{
	}

	[CompilerGenerated]
	public static SizeF smethod_6()
	{
		return _003CFactor_003Ek__BackingField;
	}

	[CompilerGenerated]
	public static void smethod_7(SizeF sizeF_0)
	{
		_003CFactor_003Ek__BackingField = sizeF_0;
	}

	[CompilerGenerated]
	public static string smethod_8()
	{
		return _003CCurUserPwd_003Ek__BackingField;
	}

	[CompilerGenerated]
	public static void smethod_9(string string_0)
	{
		_003CCurUserPwd_003Ek__BackingField = string_0;
	}

	public static void smethod_10()
	{
		smethod_76("None", ref SZ_NONE);
		smethod_76("Selected", ref SZ_SELECTED);
		smethod_76("Add", ref SZ_ADD);
		smethod_76("Off", ref SZ_OFF);
		smethod_76("DeviceNotFound", ref SZ_DEVICE_NOT_FOUND);
		smethod_76("CommError", ref SZ_COMM_ERROR);
		smethod_76("Model does not match", ref SZ_MODEL_NOT_MATCH);
		smethod_76("DataFormatError", ref SZ_DATA_FORMAT_ERROR);
		smethod_76("N/A", ref SZ_NA);
	}

	public static void ValidateNumberRangeWithDefault(ref byte byte_0, byte byte_1, byte byte_2, byte byte_3)
	{
		if (!checkInRange(byte_0, byte_1, byte_2))
		{
			byte_0 = byte_3;
		}
	}

	public static bool checkInRange(byte byte_0, byte byte_1, byte byte_2)
	{
		if (byte_0 >= byte_1 && byte_0 <= byte_2)
		{
			return true;
		}
		return false;
	}

	public static bool checkIntInRange(int int_0, int int_1, int int_2)
	{
		if (int_0 >= int_1 && int_0 <= int_2)
		{
			return true;
		}
		return false;
	}

	public static int smethod_14(byte byte_0, int int_0, int int_1)
	{
		int num = (int)Math.Pow(2.0, int_1) - 1;
		return (byte_0 >> int_0) & num;
	}

	public static void smethod_15(ref byte byte_0, int int_0, int int_1)
	{
		int num = (int)Math.Pow(2.0, int_1) - 1;
		byte_0 &= (byte)(~(num << int_0));
	}

	public static void smethod_16(ref byte byte_0, int int_0, int int_1, int int_2)
	{
		int num = (int)Math.Pow(2.0, int_1) - 1;
		byte_0 &= (byte)(~(num << int_0));
		byte_0 |= (byte)((int_2 & num) << int_0);
	}

	public static void smethod_17(ref ushort ushort_0, int int_0, int int_1)
	{
		int num = (int)Math.Pow(2.0, int_1) - 1;
		ushort_0 &= (ushort)(~(num << int_0));
	}

	public static bool smethod_18(byte[] byte_0, byte[] byte_1, int int_0)
	{
		if (byte_0.Length < int_0)
		{
			return false;
		}
		if (byte_1.Length < int_0)
		{
			return false;
		}
		int num = 0;
		while (true)
		{
			if (num < int_0)
			{
				if (byte_0[num] != byte_1[num])
				{
					break;
				}
				num++;
				continue;
			}
			return true;
		}
		return false;
	}

	public static int checkFrequecyIsInValidRange(double double_0, ref uint uint_0)
	{
		int num = 0;
		uint_0 = 0u;
		num = 0;
		while (true)
		{
			if (num < MIN_FREQ.Length)
			{
				if (MIN_FREQ[num] < MAX_FREQ[num] && uint_0 == 0)
				{
					if (double_0 < (double)MIN_FREQ[1])
					{
						uint_0 = MIN_FREQ[1];
					}
					if (double_0 > (double)MAX_FREQ[1] && double_0 < (double)MIN_FREQ[0])
					{
						uint num2 = (MIN_FREQ[0] - MAX_FREQ[1]) / 2 + MAX_FREQ[1];
						if (double_0 >= (double)num2)
						{
							uint_0 = MIN_FREQ[0];
						}
						else
						{
							uint_0 = MAX_FREQ[1];
						}
					}
					if (double_0 > (double)MAX_FREQ[0])
					{
						uint_0 = MAX_FREQ[0];
					}
				}
				if (double_0 >= (double)MIN_FREQ[num] && !(double_0 > (double)MAX_FREQ[num]))
				{
					break;
				}
				num++;
				continue;
			}
			return -1;
		}
		return num;
	}

	public static int smethod_20(double double_0, double double_1)
	{
		int num = 0;
		int num2 = -1;
		int num3 = -1;
		for (num = 0; num < MIN_FREQ.Length; num++)
		{
			if (double_0 >= (double)MIN_FREQ[num] && !(double_0 > (double)MAX_FREQ[num]))
			{
				num2 = num;
				break;
			}
		}
		for (num = 0; num < MAX_FREQ.Length; num++)
		{
			if (double_1 >= (double)MIN_FREQ[num] && !(double_1 > (double)MAX_FREQ[num]))
			{
				num3 = num;
				break;
			}
		}
		if (num2 == num3 && num2 != -1)
		{
			return num2;
		}
		return -1;
	}

	public static int smethod_21(uint uint_0, ref int int_0)
	{
		int num = 0;
		int_0 = -1;
		num = 0;
		while (true)
		{
			if (num < MIN_FREQ.Length)
			{
				if (VALID_MIN_FREQ[num] < VALID_MAX_FREQ[num] && int_0 < 0)
				{
					int_0 = num;
				}
				if (uint_0 >= VALID_MIN_FREQ[num] && uint_0 <= VALID_MAX_FREQ[num])
				{
					break;
				}
				num++;
				continue;
			}
			return -1;
		}
		return num;
	}

	public static int smethod_22(double double_0, double double_1)
	{
		int num = 0;
		int num2 = -1;
		int num3 = -1;
		for (num = 0; num < VALID_MIN_FREQ.Length; num++)
		{
			if (double_0 >= (double)VALID_MIN_FREQ[num] && !(double_0 > (double)VALID_MAX_FREQ[num]))
			{
				num2 = num;
				break;
			}
		}
		for (num = 0; num < VALID_MAX_FREQ.Length; num++)
		{
			if (double_1 >= (double)VALID_MIN_FREQ[num] && !(double_1 > (double)VALID_MAX_FREQ[num]))
			{
				num3 = num;
				break;
			}
		}
		if (num2 == num3 && num2 != -1)
		{
			return num2;
		}
		return -1;
	}



	public static byte[] stringToBuffer(string name)
	{		
		Encoding encoding = Encoding.GetEncoding("windows-1251");
		if (encoding == null)
		{
			encoding = Encoding.Default;
		}
		byte[] buffer = encoding.GetBytes(name);
        for (int c = 0; c < buffer.Length; c++)
        {
			if (buffer[c] == 0xff)
				buffer[c] = 0x7f; // замена "я" на подменный символ
        }
        return buffer;
	}


    public static string bufferToString(byte[] codeplugData)
	{
		Encoding encoding = Encoding.GetEncoding("windows-1251");
        if (encoding == null)
		{
			encoding = Encoding.Default;
		}
		int num = Array.IndexOf(codeplugData, byte.MaxValue);
		if (num == -1)
		{
			num = Array.IndexOf(codeplugData, (byte)0);
			if (num == -1)
			{
				num = codeplugData.Length;
			}
		}
        byte[] buffer = new byte[num];
        for (int c = 0; c < num; c++)
        {
            buffer[c] = codeplugData[c];
            if (buffer[c] == 0x7f) // превращение подменного символа в корректное "я"
                buffer[c] = byte.MaxValue;
        }

        return encoding.GetString(buffer, 0, num);
	}

	public static int convertDecimalFreqTo10HzStepValue(double double_0, double double_1)
	{
		decimal num = Convert.ToDecimal(double_0);
		decimal num2 = Convert.ToDecimal(double_1);
		return Convert.ToInt32(num * num2);
	}

	public static double convert10HzStepFreqToDecimalText(int int_0, int int_1)
	{
		decimal num = Convert.ToDecimal(int_0);
		decimal num2 = Convert.ToDecimal(int_1);
		return Convert.ToDouble(num / num2);
	}

	public static void AdjustFrequencyToNearestStep(ref int int_0, int int_1, int int_2)
	{
		int num = int_0 % int_1;
		int num2 = int_0 % int_2;
		if (num != 0 && num2 != 0)
		{
			int num3 = 250 - num;
			int num4 = 625 - num2;
			if (num3 < num4)
			{
				int_0 += num3;
			}
			else
			{
				int_0 += num4;
			}
		}
	}

	public static void smethod_30(ref int int_0, int int_1, int int_2)
	{
		int num = 1;
		int num2 = 0;
		int num3 = 0;
		int[] array = new int[4]
		{
			int_0 % int_1,
			0,
			0,
			0
		};
		array[1] = int_1 - array[0];
		array[2] = int_0 % int_2;
		array[3] = int_2 - array[2];
		num2 = array[0];
		for (num = 1; num < array.Length; num++)
		{
			if (array[num] < num2)
			{
				num2 = array[num];
				num3 = num;
			}
		}
		if (num3 % 2 == 0)
		{
			int_0 -= num2;
		}
		else
		{
			int_0 += num2;
		}
	}

	public static void smethod_31(ref int int_0)
	{
		int num = int_0 % 625;
		if (num == 0)
		{
			return;
		}
		int i;
		for (i = 0; i < 3; i++)
		{
			num += 25;
			if (num % 625 == 0)
			{
				int_0 += (i + 1) * 25;
				break;
			}
		}
		if (i == 3)
		{
			int_0 = (int_0 + 250) / 250 * 250;
		}
	}

	public static ushort smethod_32(ushort ushort_0)
	{
		int num = 0;
		int num2 = 0;
		ushort num3 = 0;
		for (num = 0; num < 4; num++)
		{
			num2 = ushort_0 & 0xF;
			ushort_0 >>= 4;
			num3 = (ushort)((double)(int)num3 + (double)num2 * Math.Pow(10.0, num));
		}
		return num3;
	}

	public static ushort smethod_33(ushort ushort_0)
	{
		int num = 0;
		int num2 = 0;
		ushort num3 = 0;
		for (num = 0; num < 4; num++)
		{
			num2 = ushort_0 % 10;
			ushort_0 /= 10;
			num3 = (ushort)((double)(int)num3 + (double)num2 * Math.Pow(16.0, num));
		}
		return num3;
	}

	public static uint smethod_34(uint uint_0)
	{
		int num = 0;
		uint num2 = 0u;
		uint num3 = 0u;
		for (num = 0; num < 8; num++)
		{
			num2 = uint_0 & 0xF;
			uint_0 >>= 4;
			num3 += num2 * (uint)Math.Pow(10.0, num);
		}
		return num3;
	}

	public static uint smethod_35(uint uint_0)
	{
		int num = 0;
		uint num2 = 0u;
		uint num3 = 0u;
		for (num = 0; num < 8; num++)
		{
			num2 = uint_0 % 10;
			uint_0 /= 10;
			num3 += num2 * (uint)Math.Pow(16.0, num);
		}
		return num3;
	}

	public static void smethod_36(CustomNumericUpDown class12_0, Class13 class13_0)
	{
		if (class13_0.method_6() < 0m)
		{
			class12_0.Minimum = (decimal)class13_0.method_2() * class13_0.method_6();
			class12_0.Maximum = (decimal)class13_0.method_0() * class13_0.method_6();
			class12_0.Increment = Math.Abs((decimal)class13_0.method_4() * class13_0.method_6());
		}
		else
		{
			class12_0.Minimum = (decimal)class13_0.method_0() * class13_0.method_6();
			class12_0.Maximum = (decimal)class13_0.method_2() * class13_0.method_6();
			class12_0.Increment = (decimal)class13_0.method_4() * class13_0.method_6();
		}
		class12_0.method_0(class13_0.method_8());
	}

	public static void fillComboBox(ComboBox target, string[] data)
	{
		int num = 0;
		target.Items.Clear();
		for (num = 0; num < data.Length; num++)
		{
			target.Items.Add(data[num]);
		}
	}

	public static void smethod_38(ComboBox comboBox_0, string[] string_0, int int_0)
	{
		int num = 0;
		int num2 = Math.Min(string_0.Length, int_0);
		comboBox_0.Items.Clear();
		for (num = 0; num < num2; num++)
		{
			comboBox_0.Items.Add(string_0[num]);
		}
	}

	public static void smethod_39(CustomCombo class4_0, string[] string_0)
	{
		int num = 0;
		class4_0.method_0();
		foreach (string string_1 in string_0)
		{
			class4_0.method_1(string_1, num++);
		}
	}

	public static void smethod_40(CustomCombo class4_0, string[] string_0, int[] int_0)
	{
		class4_0.method_0();
		foreach (int num in int_0)
		{
			class4_0.method_1(string_0[num], num);
		}
	}

	public static void smethod_41(ComboBox comboBox_0, int int_0, int int_1)
	{
		int num = 0;
		comboBox_0.Items.Clear();
		for (num = int_0; num <= int_1; num++)
		{
			comboBox_0.Items.Add(num);
		}
	}

	public static void smethod_42(ComboBox comboBox_0, string string_0, int int_0, int int_1)
	{
		int num = 0;
		comboBox_0.Items.Clear();
		comboBox_0.Items.Add(string_0);
		for (num = int_0; num <= int_1; num++)
		{
			comboBox_0.Items.Add(num);
		}
	}

	public static void smethod_43(ComboBox comboBox_0, int int_0, int int_1, int int_2, string string_0)
	{
		int num = 0;
		comboBox_0.Items.Clear();
		for (num = int_0; num <= int_1; num++)
		{
			if (int_2 == num)
			{
				comboBox_0.Items.Add(string_0);
			}
			else
			{
				comboBox_0.Items.Add(num.ToString());
			}
		}
	}

	public static void smethod_44(CustomCombo class4_0, IData idata_0, string noneText = null)
	{
		int num = 0;
		string text = "";
		class4_0.method_0();
		if (noneText == null)
		{
			class4_0.method_1(SZ_NONE, 0);
		}
		else
		{
			class4_0.method_1(noneText, 0);
		}
		for (num = 0; num < idata_0.Count; num++)
		{
			if (idata_0.DataIsValid(num))
			{
				text = idata_0.GetName(num);
				class4_0.method_1(text, num + 1);
			}
		}
	}

	public static void smethod_45(CustomCombo class4_0, string[] string_0, IData idata_0)
	{
		int num = 0;
		string text = "";
		class4_0.method_0();
		for (num = 0; num < string_0.Length; num++)
		{
			class4_0.method_1(string_0[num], num);
		}
		for (num = 0; num < idata_0.Count; num++)
		{
			if (idata_0.DataIsValid(num))
			{
				text = $"{num + 1:d3}:{idata_0.GetName(num)}";
				class4_0.method_1(text, num + string_0.Length);
			}
		}
	}

	public static void smethod_46(CustomCombo class4_0, string[] string_0, ListBox listBox_0)
	{
		int num = 0;
		string text = "";
		class4_0.method_0();
		for (num = 0; num < string_0.Length; num++)
		{
			class4_0.method_1(string_0[num], num);
		}
		for (num = 0; num < listBox_0.Items.Count; num++)
		{
			text = listBox_0.Items[num].ToString();
			if (listBox_0.Items[num] is SelectedItemUtils selectedItemUtils)
			{
				class4_0.method_1(text, selectedItemUtils.Value);
			}
		}
	}

	public static int smethod_47(ListBox listBox_0, SelectedItemUtils class14_0)
	{
		int num = 0;
		num = 0;
		while (true)
		{
			if (num < listBox_0.Items.Count)
			{
				SelectedItemUtils selectedItemUtils = (SelectedItemUtils)listBox_0.Items[num];
				if (class14_0.Value < selectedItemUtils.Value)
				{
					break;
				}
				num++;
				continue;
			}
			return num;
		}
		return num;
	}

	public static void smethod_48(TreeNode treeNode_0, int int_0)
	{
		if (int_0 < treeNode_0.Level)
		{
			return;
		}
		treeNode_0.Expand();
		foreach (TreeNode node in treeNode_0.Nodes)
		{
			smethod_48(node, int_0);
		}
	}

	public static void smethod_49(TreeView treeView_0, int int_0)
	{
		foreach (TreeNode node in treeView_0.Nodes)
		{
			smethod_48(node, int_0);
		}
	}

	public static bool smethod_50(TreeNode treeNode_0, string string_0)
	{
		if (string.IsNullOrEmpty(string_0))
		{
			return true;
		}
		foreach (TreeNode node in treeNode_0.Parent.Nodes)
		{
			if (node != treeNode_0 && node.Text.Trim() == string_0.Trim())
			{
				return true;
			}
		}
		return false;
	}

	public static bool smethod_51(TreeNode treeNode_0, string string_0)
	{
		if (string.IsNullOrEmpty(string_0))
		{
			return true;
		}
		foreach (TreeNode node in treeNode_0.Nodes)
		{
			if (node.Text.Trim() == string_0.Trim())
			{
				return true;
			}
		}
		return false;
	}

	public static void smethod_52(object sender, DataGridViewCellEventArgs e)
	{
		DataGridView dataGridView = sender as DataGridView;
		string helpId = dataGridView.FindForm().Name + "_" + dataGridView.Columns[e.ColumnIndex].Name;
		if (Application.OpenForms[0] is MainForm mainForm)
		{
			mainForm.ShowHelp(helpId);
		}
	}

	public static void smethod_53(object sender, KeyPressEventArgs e)
	{
		if ((e.KeyChar < '\0' || e.KeyChar > '\u007f') && e.KeyChar != '\b' && e.KeyChar != '.')
		{
			MessageBox.Show(dicCommon["KeyPressPrint"]);
			e.Handled = true;
		}
	}

	public static void smethod_54(object sender, KeyPressEventArgs e)
	{
	}

	public static void smethod_55(object sender, KeyPressEventArgs e)
	{
		if (!char.IsControl(e.KeyChar))
		{
			NumberFormatInfo numberFormat = CultureInfo.CurrentCulture.NumberFormat;
			if ((e.KeyChar < '0' || e.KeyChar > '9') && !numberFormat.NumberDecimalSeparator.Contains(e.KeyChar))
			{
				MessageBox.Show(string.Format(dicCommon["KeyPressDigit"], numberFormat.NumberDecimalSeparator));
				e.Handled = true;
			}
		}
	}

	public static bool smethod_56(string string_0)
	{
		char[] array = string_0.ToCharArray();
		int num = 0;
		while (true)
		{
			if (num < array.Length)
			{
				char c = array[num];
				if (c < '\0' || c > '\u007f')
				{
					break;
				}
				num++;
				continue;
			}
			return true;
		}
		return false;
	}

	public static void smethod_57(object sender, KeyPressEventArgs e)
	{
		if ("0123456789ABCD*#\b".IndexOf(char.ToUpper(e.KeyChar)) < 0 && e.KeyChar != '\b' && e.KeyChar != '.')
		{
			string text = Regex.Replace("0123456789ABCD*#\b", "[^\\dA-D\\*#]*", "");
			MessageBox.Show(dicCommon["KeyPressDtmf"] + text);
			e.Handled = true;
		}
	}

	public static void smethod_58(object sender, KeyPressEventArgs e)
	{
		if ("0123456789ABCDEF".IndexOf(char.ToUpper(e.KeyChar)) < 0 && e.KeyChar != '\b' && e.KeyChar != '.')
		{
			MessageBox.Show("KeyPressHex0123456789ABCDEF");
			e.Handled = true;
		}
	}

	public static void smethod_59(Control.ControlCollection controlCollection_0)
	{
		foreach (Control item in controlCollection_0)
		{
			if (!(item is Button) && !(item is TextBox) && !(item is ListBox) && !(item is NumericUpDown) && !(item is ComboBox) && !(item is CheckBox))
			{
				if (item is DataGridView)
				{
					(item as DataGridView).CellEnter += smethod_52;
				}
				else if (item.Controls.Count > 0)
				{
					smethod_59(item.Controls);
				}
			}
			else
			{
				item.Enter += smethod_60;
			}
		}
	}

	public static void smethod_60(object sender, EventArgs e)
	{
		Control control = sender as Control;
		string helpId = control.FindForm().Name + "_" + control.Name;
		if (Application.OpenForms[0] is MainForm mainForm)
		{
			mainForm.ShowHelp(helpId);
		}
	}

	public static byte[] objectToByteArray(object object_0, int int_0)
	{
		byte[] array = new byte[int_0];
		IntPtr intPtr = Marshal.AllocHGlobal(int_0);
		Marshal.StructureToPtr(object_0, intPtr, fDeleteOld: false);
		Marshal.Copy(intPtr, array, 0, int_0);
		Marshal.FreeHGlobal(intPtr);
		return array;
	}

	public static object byteArrayToObject(byte[] byte_0, Type type_0)
	{
		int num = Marshal.SizeOf(type_0);
		if (num > byte_0.Length)
		{
			throw new ArgumentException();
		}
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.Copy(byte_0, 0, intPtr, num);
		object result = Marshal.PtrToStructure(intPtr, type_0);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public static void smethod_63(string string_0)
	{
		FileStream fileStream = new FileStream(string_0, FileMode.Create);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		binaryFormatter.Serialize(fileStream, GeneralSetForm.data);
		binaryFormatter.Serialize(fileStream, ButtonForm.data);
		binaryFormatter.Serialize(fileStream, ButtonForm.data1);
		binaryFormatter.Serialize(fileStream, TextMsgForm.data);
		binaryFormatter.Serialize(fileStream, EncryptForm.data);
		binaryFormatter.Serialize(fileStream, SignalingBasicForm.data);
		binaryFormatter.Serialize(fileStream, DtmfForm.data);
		binaryFormatter.Serialize(fileStream, APRSForm.data);
		binaryFormatter.Serialize(fileStream, DtmfContactForm.data);
		binaryFormatter.Serialize(fileStream, ContactForm.data);
		binaryFormatter.Serialize(fileStream, RxGroupListForm.data);
		binaryFormatter.Serialize(fileStream, ZoneForm.data);
		binaryFormatter.Serialize(fileStream, ChannelForm.data);
		binaryFormatter.Serialize(fileStream, ScanBasicForm.data);
		binaryFormatter.Serialize(fileStream, NormalScanForm.data);
		fileStream.Close();
	}

	public static void smethod_64(string string_0)
	{
		FileStream fileStream = new FileStream(string_0, FileMode.Open, FileAccess.Read, FileShare.Read);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		GeneralSetForm.data = (GeneralSetForm.GeneralSet)binaryFormatter.Deserialize(fileStream);
		ButtonForm.data = (ButtonForm.SideKey)binaryFormatter.Deserialize(fileStream);
		ButtonForm.data1 = (ButtonForm.OneTouch)binaryFormatter.Deserialize(fileStream);
		TextMsgForm.data = (TextMsgForm.TextMsg)binaryFormatter.Deserialize(fileStream);
		EncryptForm.data = (EncryptForm.Encrypt)binaryFormatter.Deserialize(fileStream);
		SignalingBasicForm.data = (SignalingBasicForm.SignalingBasic)binaryFormatter.Deserialize(fileStream);
		DtmfForm.data = (DtmfForm.Dtmf)binaryFormatter.Deserialize(fileStream);
		APRSForm.data = (APRSForm.APRS_Config)binaryFormatter.Deserialize(fileStream);
		DtmfContactForm.data = (DtmfContactForm.DtmfContact)binaryFormatter.Deserialize(fileStream);
		ContactForm.data = (ContactForm.Contact)binaryFormatter.Deserialize(fileStream);
		RxGroupListForm.data = (RxListData)binaryFormatter.Deserialize(fileStream);
		ZoneForm.data = (ZoneForm.Zone)binaryFormatter.Deserialize(fileStream);
		ChannelForm.data = (ChannelForm.Channel)binaryFormatter.Deserialize(fileStream);
		ScanBasicForm.data = (ScanBasicForm.ScanBasic)binaryFormatter.Deserialize(fileStream);
		NormalScanForm.data = (NormalScanForm.NormalScan)binaryFormatter.Deserialize(fileStream);
		fileStream.Close();
	}

	public static QkGVc1MQ9NxKRGCTdE cloneObject<QkGVc1MQ9NxKRGCTdE>(QkGVc1MQ9NxKRGCTdE CeqCQcoTiZ0ZwY0OmE)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		binaryFormatter.Serialize(memoryStream, CeqCQcoTiZ0ZwY0OmE);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		QkGVc1MQ9NxKRGCTdE result = (QkGVc1MQ9NxKRGCTdE)binaryFormatter.Deserialize(memoryStream);
		memoryStream.Close();
		return result;
	}

	public static string smethod_66(string string_0)
	{
		if (dicCommon.ContainsKey(string_0))
		{
			return dicCommon[string_0];
		}
		return "";
	}

	public static string smethod_67(string string_0)
	{
		new Dictionary<string, string>();
		string xpath = $"/Resource/Settings/Item[@Id='{string_0}']";
		try
		{
			XmlNode xmlNode = _languageXML.SelectSingleNode(xpath);
			if (xmlNode != null && xmlNode.Attributes["Text"] != null)
			{
				return xmlNode.Attributes["Text"].Value;
			}
		}
		catch
		{
			return "";
		}
		return "";
	}

	public static void UpdateComponentTextsFromLanguageXmlData(Form form_0)
	{
		new Dictionary<string, string>();
		string xpath = $"/Resource/{form_0.Name}";
		XmlNode xmlNode = _languageXML.SelectSingleNode(xpath);
		try
		{
			form_0.Text = xmlNode.Attributes["Text"].Value;
			if (form_0 is DockContent dockContent)
			{
				dockContent.TabText = xmlNode.Attributes["Text"].Value;
			}
			setContolTextFromXmlData(form_0.smethod_12(), form_0.Name);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public static void setContolTextFromXmlData(List<Control> list_0, string string_0)
	{
		Dictionary<string, string> dic = new Dictionary<string, string>();
		string xpath = $"/Resource/{string_0}/Controls/Control";
		foreach (XmlNode item in _languageXML.SelectNodes(xpath))
		{
			string value = item.Attributes["Id"].Value;
			string value2 = item.Attributes["Text"].Value;
			dic.Add(value, value2);
		}
		list_0.ForEach(delegate(Control x)
		{
			if (dic.ContainsKey(x.Name))
			{
				if (x is DataGridView)
				{
					foreach (DataGridViewColumn column in (x as DataGridView).Columns)
					{
						column.HeaderText = dic[x.Name].Split(',')[column.Index];
					}
					return;
				}
				x.Text = dic[x.Name];
			}
		});
	}

	public static void UpdateContextMenuStripFromLanguageXmlData(List<ToolStripMenuItem> gku9yQXy4fa3WZdpnA, string string_0)
	{
		Dictionary<string, string> dic = new Dictionary<string, string>();
		string xpath = $"/Resource/{string_0}/ContextMenuStrip/MenuItem";
		foreach (XmlNode item in _languageXML.SelectNodes(xpath))
		{
			string value = item.Attributes["Id"].Value;
			string value2 = item.Attributes["Text"].Value;
			dic.Add(value, value2);
		}
		gku9yQXy4fa3WZdpnA.ForEach(delegate(ToolStripMenuItem x)
		{
			if (dic.ContainsKey(x.Name))
			{
				string text2 = (x.ToolTipText = dic[x.Name]);
				x.Text = text2;
			}
		});
	}

	public static void UpdateToolStripFromLanguageXmlData(List<ToolStripItem> LE9oY1wrram2m8Ao56, string string_0)
	{
		Dictionary<string, string> dic = new Dictionary<string, string>();
		string xpath = $"/Resource/{string_0}/Controls/Control/ToolStripItem";
		foreach (XmlNode item in _languageXML.SelectNodes(xpath))
		{
			string value = item.Attributes["Id"].Value;
			string value2 = item.Attributes["Text"].Value;
			dic.Add(value, value2);
		}
		LE9oY1wrram2m8Ao56.ForEach(delegate(ToolStripItem x)
		{
			if (dic.ContainsKey(x.Name))
			{
				string text2 = (x.ToolTipText = dic[x.Name]);
				x.Text = text2;
			}
		});
	}

	public static void ReadCommonsTextIntoDictionary(Dictionary<string, string> dictionary)
	{
		string xpath = $"/Resource/Commons/Item";
		foreach (XmlNode item in _languageXML.SelectNodes(xpath))
		{
			string value = item.Attributes["Id"].Value;
			string value2 = item.Attributes["Text"].Value;
			if (dictionary.ContainsKey(value))
			{
				dictionary[value] = value2;
			}
		}
	}

	public static void ReadCommonsForSectionIntoDictionary(Dictionary<string, string> dictionary, string sectionName)
	{
		string xpath = $"/Resource/{sectionName}/Commons/Item";
		foreach (XmlNode item in _languageXML.SelectNodes(xpath))
		{
			string value = item.Attributes["Id"].Value;
			string value2 = item.Attributes["Text"].Value;
			dictionary[value] = value2;
		}
	}

	public static void smethod_74(List<string[]> n2SR3VmEodXx385mq9, List<string> AMMonO7JcQ5lQDAuEr, string string_0)
	{
		for (int i = 0; i < n2SR3VmEodXx385mq9.Count; i++)
		{
			string xpath = $"/Resource/{string_0}/Commons/Item[@Id='{AMMonO7JcQ5lQDAuEr[i]}']";
			XmlNode xmlNode = _languageXML.SelectSingleNode(xpath);
			if (xmlNode != null)
			{
				string[] array = xmlNode.Attributes["Text"].Value.Split(',');
				for (int j = 0; j < n2SR3VmEodXx385mq9[i].Length && j < array.Length; j++)
				{
					n2SR3VmEodXx385mq9[i][j] = array[j];
				}
			}
		}
	}

	public static void smethod_75(List<string> mTBilSHhIiS5P1HoGl, List<string> QaKAVsVaOpyU5FW5pp)
	{
		for (int i = 0; i < mTBilSHhIiS5P1HoGl.Count; i++)
		{
			string xpath = $"/Resource/Commons/Item[@Id='{QaKAVsVaOpyU5FW5pp[i]}']";
			XmlNode xmlNode = _languageXML.SelectSingleNode(xpath);
			if (xmlNode != null)
			{
				mTBilSHhIiS5P1HoGl[i] = xmlNode.Attributes["Text"].Value;
			}
		}
	}

	public static void smethod_76(string string_0, ref string string_1)
	{
		string xpath = $"/Resource/Commons/Item[@Id='{string_0}' and @Text]";
		XmlNode xmlNode = _languageXML.SelectSingleNode(xpath);
		if (xmlNode != null)
		{
			string_1 = xmlNode.Attributes["Text"].Value;
		}
	}

	public static void smethod_77(string string_0, ref string string_1, string string_2)
	{
		string xpath = $"/Resource/{string_2}/Commons/Item[@Id='{string_0}' and @Text]";
		XmlNode xmlNode = _languageXML.SelectSingleNode(xpath);
		if (xmlNode != null)
		{
			string_1 = xmlNode.Attributes["Text"].Value;
		}
	}

	public static void smethod_78(string string_0, string[] string_1, string string_2)
	{
		string xpath = $"/Resource/{string_2}/Commons/Item[@Id='{string_0}' and @Text]";
		XmlNode xmlNode = _languageXML.SelectSingleNode(xpath);
		if (xmlNode != null)
		{
			string[] array = xmlNode.Attributes["Text"].Value.Split(',');
			for (int i = 0; i < string_1.Length && i < array.Length; i++)
			{
				string_1[i] = array[i];
			}
		}
	}

	static Settings()
	{
		TONES_LIST = new string[216]
		{
			"67.0", "69.3", "71.9", "74.4", "77.0", "79.7", "82.5", "85.4", "88.5", "91.5",
			"94.8", "97.4", "100.0", "103.5", "107.2", "110.9", "114.8", "118.8", "123.0", "127.3",
			"131.8", "136.5", "141.3", "146.2", "151.4", "156.7", "159.8", "162.2", "165.5", "167.9",
			"171.3", "173.8", "177.3", "179.9", "183.5", "186.2", "189.9", "192.8", "196.6", "199.5",
			"203.5", "206.5", "210.7", "218.1", "225.7", "229.1", "233.6", "241.8", "250.3", "254.1",
			"D023N", "D025N", "D026N", "D031N", "D032N", "D043N", "D047N", "D051N", "D054N", "D065N",
			"D071N", "D072N", "D073N", "D074N", "D114N", "D115N", "D116N", "D125N", "D131N", "D132N",
			"D134N", "D143N", "D152N", "D155N", "D156N", "D162N", "D165N", "D172N", "D174N", "D205N",
			"D223N", "D226N", "D243N", "D244N", "D245N", "D251N", "D261N", "D263N", "D265N", "D271N",
			"D306N", "D311N", "D315N", "D331N", "D343N", "D346N", "D351N", "D364N", "D365N", "D371N",
			"D411N", "D412N", "D413N", "D423N", "D431N", "D432N", "D445N", "D464N", "D465N", "D466N",
			"D503N", "D506N", "D516N", "D532N", "D546N", "D565N", "D606N", "D612N", "D624N", "D627N",
			"D631N", "D632N", "D654N", "D662N", "D664N", "D703N", "D712N", "D723N", "D731N", "D732N",
			"D734N", "D743N", "D754N", "D023I", "D025I", "D026I", "D031I", "D032I", "D043I", "D047I",
			"D051I", "D054I", "D065I", "D071I", "D072I", "D073I", "D074I", "D114I", "D115I", "D116I",
			"D125I", "D131I", "D132I", "D134I", "D143I", "D152I", "D155I", "D156I", "D162I", "D165I",
			"D172I", "D174I", "D205I", "D223I", "D226I", "D243I", "D244I", "D245I", "D251I", "D261I",
			"D263I", "D265I", "D271I", "D306I", "D311I", "D315I", "D331I", "D343I", "D346I", "D351I",
			"D364I", "D365I", "D371I", "D411I", "D412I", "D413I", "D423I", "D431I", "D432I", "D445I",
			"D464I", "D465I", "D466I", "D503I", "D506I", "D516I", "D532I", "D546I", "D565I", "D606I",
			"D612I", "D624I", "D627I", "D631I", "D632I", "D654I", "D662I", "D664I", "D703I", "D712I",
			"D723I", "D731I", "D732I", "D734I", "D743I", "D754I"
		};
		SZ_USER_AGREEMENT = "This software is supplied 'as is' with no warranties. You use it at your own risk to both your PC and to your DMR Radio. By pressing the  Yes   button you agree and understand.";
		SZ_DOWNLOADCONTACTS_REGION_EMPTY = "Please enter the 3 digit Region previx code. e.g. 505 for Australia.";
		SZ_DOWNLOADCONTACTS_MESSAGE_ADDED = "There are {0} new ID's which are not already in your contacts";
		SZ_DOWNLOADCONTACTS_DOWNLOADING = "Downloading...";
		SZ_DOWNLOADCONTACTS_SELECT_CONTACTS_TO_IMPORT = "Please select the contacts you would like to import";
		SZ_DOWNLOADCONTACTS_TOO_MANY = "Not all contacts could be imported because the maximum number of Digital Contacts has been reached";
		SZ_UNABLEDOWNLOADFROMINTERNET = "Unable to download data. Please check your Internet connection";
		SZ_IMPORT_COMPLETE = "Import complete";
		SZ_CODEPLUG_UPGRADE_NOTICE = "This appears to be a V3.0.6 Codeplug. It will be converted to V3.1.x";
		SZ_CODEPLUG_UPGRADE_WARNING_TO_MANY_RX_GROUPS = "Version 3.1.x can only have 76 Rx Groups. Additional Rx Groups have been ignored";
		SZ_CODEPLUG_READ = "Reading codeplug from GD-77";
		SZ_CODEPLUG_WRITE = "Writing codeplug to GD-77";
		SZ_DMRID_READ = "Reading DMR ID database from GD-77";
		SZ_DMRID_WRITE = "Writing DMR ID database to GD-77";
		SZ_CALIBRATION_READ = "Reading calibration data from GD-77";
		SZ_CALIBRATION_WRITE = "Writing calibration data to GD-77";
		SZ_CONTACT_DUPLICATE_NAME = "Warning. Duplicate contact name.";
		SZ_EnableMemoryAccessMode = "The GD-77 does not seem to be in Memory Access mode\nHold keys SK2 (Blue side key), Green Menu and * when turning on the transceiver.\nand try again";
		SZ_dataRead = "Reading data from GD-77";
		SZ_dataWrite = "Writing data to GD-77";
		SZ_DMRIdContcatsTotal = "Total number of IDs = {0}. Max of 10920 can be uploaded";
		SZ_ErrorParsingData = "Error while parsing data";
		SZ_DMRIdIntroMessage = "Data is downloaded from Ham-digital.org and appended any existing data";
		EEROM_SPACE = 131072u;
		ADDR_OPENGD77_CUSTOM_DATA_START = 126560;
		ADDR_OPENGD77_CUSTOM_DATA_END = 131072;
		ADDR_OPENGD77_LAST_USED_CHANNELS_DATA_START = 24576;
		LAST_USED_CHANNEL_IN_ZONE_SIZE = 70;
		LAST_USED_CHANNEL_IN_ZONE_BLOCK_SIZE = LAST_USED_CHANNEL_IN_ZONE_SIZE + 4;
		ADDR_OPENGD77_LAST_USED_CHANNELS_DATA_END = ADDR_OPENGD77_LAST_USED_CHANNELS_DATA_START + LAST_USED_CHANNEL_IN_ZONE_BLOCK_SIZE;
		ADDR_OPENGD77_SETTINGS_START = ADDR_OPENGD77_LAST_USED_CHANNELS_DATA_END + 1;
		dicCommon = new Dictionary<string, string>();
		_languageXML = null;
		LanguageFile = "";
		CUR_MODEL = new byte[8] { 77, 68, 45, 55, 54, 48, 80, 255 };
		SZ_NONE = "None";
		SZ_SELECTED = "Selected";
		SZ_ADD = "Add";
		SZ_OFF = "Off";
		SZ_DEVICE_NOT_FOUND = "Device not found";
		SZ_OPEN_PORT_FAIL = "";
		SZ_COMM_ERROR = "Communication error";
		SZ_MODEL_NOT_MATCH = "";
		SZ_READ = "Read";
		SZ_WRITE = "Write";
		SZ_READ_COMPLETE = "Read Complete";
		SZ_WRITE_COMPLETE = "Write Complete";
		SZ_CODEPLUG_READ_CONFIRM = "Are you sure you want to read the codeplug from the GD-77?\nThis will overwrite the current codeplug.";
		SZ_CODEPLUG_WRITE_CONFIRM = "Are you sure you want to write this codeplug to the GD-77?\nThis will overwrite codeplug currently in the GD-77";
		SZ_PLEASE_CONFIRM = "Please confirm";
		SZ_KEYPRESS_DTMF = "";
		SZ_KEYPRESS_HEX = "Please Input: ";
		SZ_KEYPRESS_DIGIT = "Please input: Digit and {0}";
		SZ_KEYPRESS_PRINT = "Please input Alphanumeric symbol";
		SZ_DATA_FORMAT_ERROR = "";
		SZ_FIRST_CH_NOT_DELETE = "The first channel in the first zone cannot be deleted";
		SZ_FIRST_NOT_DELETE = "The first cannot be deleted";
		SZ_NAME_EXIST_NAME = "NameExist";
		SZ_FILE_FORMAT_ERROR = "File format error!";
		SZ_OPEN_SUCCESSFULLY = "Open Successfully!";
		SZ_SAVE_SUCCESSFULLY = "Save Successfully";
		SZ_TYPE_NOT_MATCH = "Type does not match";
		SZ_EXPORT_SUCCESS = "Export Success";
		SZ_IMPORT_SUCCESS = "Import Success";
		SZ_ID_NOT_EMPTY = "ID can not be empty!";
		SZ_ID_OUT_OF_RANGE = "ID out of range!";
		SZ_ID_ALREADY_EXISTS = "ID already exists！";
		SZ_NOT_SELECT_ITEM_NOT_COPYITEM = "Not select item or Not copyitem";
		SZ_PROMPT_KEY1 = "Does the software exit and save the file?";
		SZ_PROMPT_KEY2 = "Whether the new, will be restored to the initial state!";
		SZ_PROMPT = "Prompt";
		SZ_ERROR = "Error";
		SZ_WARNING = "Warning";
		SZ_NA = "N/A";
		CUR_MODE = 2;
		MIN_FREQ = new uint[2] { 380u, 127u };
		MAX_FREQ = new uint[2] { 564u, 282u };
		VALID_MIN_FREQ = new uint[2] { 380u, 127u };
		VALID_MAX_FREQ = new uint[2] { 564u, 178u };
		CUR_CH_GROUP = 0;
		CUR_ZONE_GROUP = 0;
		CUR_ZONE = 0;
		CUR_PWD = "";
		SPACE_DEVICE_INFO = Marshal.SizeOf(typeof(DeviceInfoForm.DeviceInfo));
		ADDR_DEVICE_INFO = 128;
		OFS_LAST_PRG_TIME = Marshal.OffsetOf(typeof(DeviceInfoForm.DeviceInfo), "lastPrgTime").ToInt32();
		OFS_CPS_SW_VER = Marshal.OffsetOf(typeof(DeviceInfoForm.DeviceInfo), "cpsSwVer").ToInt32();
		OFS_MODEL = Marshal.OffsetOf(typeof(DeviceInfoForm.DeviceInfo), "model").ToInt32();
		SPACE_GENERAL_SET = Marshal.SizeOf(typeof(GeneralSetForm.GeneralSet));
		ADDR_GENERAL_SET = 224;
		ADDR_PWD = ADDR_GENERAL_SET + Marshal.OffsetOf(typeof(GeneralSetForm.GeneralSet), "prgPwd").ToInt32();
		SPACE_BUTTON = Marshal.SizeOf(typeof(ButtonForm.SideKey));
		ADDR_BUTTON = 264;
		SPACE_ONE_TOUCH = Marshal.SizeOf(typeof(ButtonForm.OneTouch));
		ADDR_ONE_TOUCH = 272;
		SPACE_TEXT_MSG = Marshal.SizeOf(typeof(TextMsgForm.TextMsg));
		ADDR_TEXT_MSG = 296;
		SPACE_ENCRYPT = Marshal.SizeOf(typeof(EncryptForm.Encrypt));
		ADDR_ENCRYPT = 4976;
		SPACE_SIGNALING_BASIC = Marshal.SizeOf(typeof(SignalingBasicForm.SignalingBasic));
		ADDR_SIGNALING_BASIC = 5112;
		SPACE_DTMF_BASIC = Marshal.SizeOf(typeof(DtmfForm.Dtmf));
		ADDR_DTMF_BASIC = 5120;
		SPACE_APRS_SYSTEM = Marshal.SizeOf(typeof(APRSForm.APRS_Config));
		ADDR_APRS_SYSTEM = 5512;
		SPACE_DMR_CONTACT = Marshal.SizeOf(typeof(ContactForm.Contact));
		ADDR_DMR_CONTACT = 6024;
		SPACE_DMR_CONTACT_EX = Marshal.SizeOf(typeof(ContactForm.Contact));
		ADDR_DMR_CONTACT_EX = 95776;
		SPACE_DTMF_CONTACT = Marshal.SizeOf(typeof(DtmfContactForm.DtmfContact));
		ADDR_DTMF_CONTACT = 12168;
		SPACE_RX_GRP_LIST = Marshal.SizeOf(typeof(RxListData));
		ADDR_RX_GRP_LIST_EX = 120352;
		ADDR_ZONE_BASIC = 14136;
		ADDR_ZONE_LIST = 14144;
		ADDR_CHANNEL = 14208;
		SPACE_SCAN_BASIC = Marshal.SizeOf(typeof(ScanBasicForm.ScanBasic));
		ADDR_SCAN = 6024;
		SPACE_SCAN_LIST = Marshal.SizeOf(typeof(NormalScanForm.NormalScan));
		ADDR_SCAN_LIST = ADDR_SCAN + SPACE_SCAN_BASIC;
		SPACE_BOOT_ITEM = Marshal.SizeOf(typeof(BootItemForm.BootItem));
		ADDR_BOOT_ITEM = 29976;
		SPACE_DIGITAL_KEY_CONTACT = Marshal.SizeOf(typeof(DigitalKeyContactForm.NumKeyContact));
		ADDR_DIGITAL_KEY_CONTACT = 29984;
		SPACE_MENU_CONFIG = Marshal.SizeOf(typeof(MenuForm.MenuSet));
		ADDR_MENU_CONFIG = 30008;
		SPACE_BOOT_CONTENT = Marshal.SizeOf(typeof(BootItemForm.BootContent));
		ADDR_BOOT_CONTENT = 30016;
		SPACE_ATTACHMENT = Marshal.SizeOf(typeof(AttachmentForm.Attachment));
		ADDR_ATTACHMENT = 30048;
		SPACE_VFO = Marshal.SizeOf(typeof(VfoForm.Vfo));
		ADDR_VFO = 30096;
		SPACE_EX_ZONE = Marshal.SizeOf(typeof(ZoneForm.Zone));
		ADDR_EX_ZONE = 32768;
		ADDR_EX_ZONE_BASIC = ADDR_EX_ZONE;
		ADDR_EX_ZONE_LIST = ADDR_EX_ZONE + 16;
		SPACE_EX_SCAN = Marshal.SizeOf(typeof(NormalScanForm.NormalScanEx));
		ADDR_EX_SCAN = 44816;
		ADDR_EX_SCAN_PRI_CH1 = 44816;
		ADDR_EX_SCAN_PRI_CH2 = 44848;
		ADDR_EX_SCAN_SPECIFY_CH = 44880;
		ADDR_EX_SCAN_CH_LIST = 44912;
		SPACE_EX_EMERGENCY = Marshal.SizeOf(typeof(APRSForm.EmergencyEx));
		ADDR_EX_EMERGENCY = 45424;
		SPACE_EX_CH = ChannelForm.SPACE_CH_GROUP * 7;
		ADDR_EX_CH = 45488;
	}
}
