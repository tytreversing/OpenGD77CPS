using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class GeneralSetForm : DockContent, IDisp
{
	private enum MonitorType
	{
		OpenSquelch,
		Silent
	}

	private enum TalkPermitTone
	{
		None,
		Digital,
		Analog,
		AnalogAndDigital
	}

	private enum ArtsTone
	{
		Disable,
		Once,
		Always
	}

	private enum ScanModeE
	{
		Time,
		Carrier,
		Serach
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class GeneralSet : IVerify<GeneralSet>
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		private byte[] callsign;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		private byte[] radioId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		private byte[] reserve;

		private byte arsInitDly;

		private byte txPreambleDur;

		private byte monitorType;

		private byte voxSense;

		private byte rxLowBatt;

		private byte callAlertDur;

		private byte respTmr;

		private byte reminderTmr;

		private byte grpHang;

		private byte privateHang;

		private byte flag1;

		private byte flag2;

		private byte flag3;

		private byte flag4;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		private byte[] reserver2;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		private byte[] prgPwd;

		public string Callsign
		{
			get
			{
				int num = 0;
				StringBuilder stringBuilder = new StringBuilder(8);
				for (num = 0; num < 8 && callsign[num] != byte.MaxValue; num++)
				{
					stringBuilder.Append(Convert.ToChar(callsign[num]));
				}
				return stringBuilder.ToString();
			}
			set
			{
				int num = 0;
				callsign.Fill(byte.MaxValue);
				for (num = 0; num < value.Length; num++)
				{
					callsign[num] = Convert.ToByte(value[num]);
				}
			}
		}

		public string RadioId
		{
			get
			{
				string s = BitConverter.ToString(radioId).Replace("-", "");
				s = int.Parse(s).ToString();
				if (ContactForm.IsValidId(s))
				{
					return s;
				}
				return "00000001";
			}
			set
			{
				int num = 0;
				string text = value.PadLeft(8, '0');
				for (num = 0; num < 4; num++)
				{
					radioId[num] = Convert.ToByte(text.Substring(num * 2, 2), 16);
				}
			}
		}

		public decimal ArsInitDly
		{
			get
			{
				if (arsInitDly >= 0 && arsInitDly <= 8)
				{
					return arsInitDly * 30;
				}
				return 0m;
			}
			set
			{
				int num = Convert.ToInt32(value / 30m);
				if (num >= 0 && num <= 8)
				{
					arsInitDly = Convert.ToByte(num);
				}
				else
				{
					arsInitDly = 0;
				}
			}
		}

		public decimal TxPreambleDur
		{
			get
			{
				if (txPreambleDur < 0 || txPreambleDur > 144)
				{
					return 180m;
				}
				return txPreambleDur * 60;
			}
			set
			{
				int num = Convert.ToInt32(value / 60m);
				if (num >= 0 && num <= 144)
				{
					txPreambleDur = Convert.ToByte(num);
				}
				else
				{
					txPreambleDur = 3;
				}
			}
		}

		public int CodeplugVersion
		{
			get
			{
				return reserve[0];
			}
			set
			{
				reserve[0] = (byte)value;
			}
		}

		public int MonitorType
		{
			get
			{
				if (Enum.IsDefined(typeof(MonitorType), (int)monitorType))
				{
					return monitorType;
				}
				return 0;
			}
			set
			{
				if (Enum.IsDefined(typeof(MonitorType), value))
				{
					monitorType = Convert.ToByte(value);
				}
				else
				{
					monitorType = 0;
				}
			}
		}

		public int VoxSense
		{
			get
			{
				if (voxSense >= 1 && voxSense <= 10)
				{
					return voxSense;
				}
				return 3;
			}
			set
			{
				if (value >= 1 && value <= 10)
				{
					voxSense = Convert.ToByte(value);
				}
				else
				{
					voxSense = 3;
				}
			}
		}

		public decimal RxLowBatt
		{
			get
			{
				if (rxLowBatt < 0 || rxLowBatt > 127)
				{
					return 120m;
				}
				return rxLowBatt * 5;
			}
			set
			{
				int num = Convert.ToInt32(value / 5m);
				if (num >= 0 && num <= 127)
				{
					rxLowBatt = Convert.ToByte(num);
				}
				else
				{
					rxLowBatt = 24;
				}
			}
		}

		public decimal CallAlertDur
		{
			get
			{
				if (callAlertDur >= 0 && callAlertDur <= 240)
				{
					return callAlertDur * 5;
				}
				return 0m;
			}
			set
			{
				int num = Convert.ToInt32(value / 5m);
				if (num >= 0 && num <= 240)
				{
					callAlertDur = Convert.ToByte(num);
				}
				else
				{
					callAlertDur = 0;
				}
			}
		}

		public decimal RespTmr
		{
			get
			{
				if (respTmr >= 1 && respTmr <= byte.MaxValue)
				{
					return respTmr;
				}
				return 1m;
			}
			set
			{
				int num = Convert.ToInt32(value);
				if (num >= 1 && num <= 255)
				{
					respTmr = Convert.ToByte(num);
				}
				else
				{
					respTmr = 1;
				}
			}
		}

		public decimal ReminderTmr
		{
			get
			{
				if (reminderTmr < 1 || reminderTmr > byte.MaxValue)
				{
					return 10m;
				}
				return reminderTmr;
			}
			set
			{
				int num = Convert.ToInt32(value);
				if (num >= 1 && num <= 255)
				{
					reminderTmr = Convert.ToByte(num);
				}
				else
				{
					reminderTmr = 10;
				}
			}
		}

		public decimal GrpHang
		{
			get
			{
				if (grpHang < 0 || grpHang > 14)
				{
					return 3000m;
				}
				return grpHang * 500;
			}
			set
			{
				int num = Convert.ToInt32(value / 500m);
				if (num >= 0 && num <= 14)
				{
					grpHang = Convert.ToByte(num);
				}
				else
				{
					grpHang = 6;
				}
			}
		}

		public decimal PrivateHang
		{
			get
			{
				if (privateHang < 0 || privateHang > 14)
				{
					return 4000m;
				}
				return privateHang * 500;
			}
			set
			{
				int num = Convert.ToInt32(value / 500m);
				if (num >= 0 && num <= 14)
				{
					privateHang = Convert.ToByte(num);
				}
				else
				{
					privateHang = 8;
				}
			}
		}

		public int ArtsTone
		{
			get
			{
				int num = (flag1 & 0xC0) >> 6;
				if (Enum.IsDefined(typeof(ArtsTone), num))
				{
					return num;
				}
				return 1;
			}
			set
			{
				flag1 &= 63;
				if (!Enum.IsDefined(typeof(ArtsTone), value))
				{
					value = 1;
				}
				flag1 |= Convert.ToByte((value & 3) << 6);
			}
		}

		public bool ChVoice
		{
			get
			{
				return Convert.ToBoolean(flag1 & 0x20);
			}
			set
			{
				if (value)
				{
					flag1 |= 32;
				}
				else
				{
					flag1 &= 223;
				}
			}
		}

		public int VoiceLang
		{
			get
			{
				return (flag1 & 0x10) >> 4;
			}
			set
			{
				flag1 &= 239;
				flag1 |= Convert.ToByte((value << 4) & 0x10);
			}
		}

		public bool UnfamiliarNumber
		{
			get
			{
				return Convert.ToBoolean(flag1 & 8);
			}
			set
			{
				if (value)
				{
					flag1 |= 8;
				}
				else
				{
					flag1 &= 247;
				}
			}
		}

		public bool ResetTone
		{
			get
			{
				return Convert.ToBoolean(flag1 & 4);
			}
			set
			{
				if (value)
				{
					flag1 |= 4;
				}
				else
				{
					flag1 &= 251;
				}
			}
		}

		public int UpChMode
		{
			get
			{
				return (flag1 & 2) >> 1;
			}
			set
			{
				flag1 &= 253;
				flag1 |= Convert.ToByte((value << 1) & 2);
			}
		}

		public int DownChMode
		{
			get
			{
				return flag1 & 1;
			}
			set
			{
				flag1 &= 254;
				flag1 |= Convert.ToByte(value & 1);
			}
		}

		public bool BattPreamble
		{
			get
			{
				return Convert.ToBoolean(flag2 & 0x80);
			}
			set
			{
				if (value)
				{
					flag2 |= 128;
				}
				else
				{
					flag2 &= 127;
				}
			}
		}

		public bool BattRx
		{
			get
			{
				return Convert.ToBoolean(flag2 & 0x40);
			}
			set
			{
				if (value)
				{
					flag2 |= 64;
				}
				else
				{
					flag2 &= 191;
				}
			}
		}

		public bool DisableAllTones
		{
			get
			{
				return Convert.ToBoolean(flag2 & 0x20);
			}
			set
			{
				if (value)
				{
					flag2 |= 32;
				}
				else
				{
					flag2 &= 223;
				}
			}
		}

		public bool CrescendoTone
		{
			get
			{
				return Convert.ToBoolean(flag2 & 0x10);
			}
			set
			{
				if (value)
				{
					flag2 |= 16;
				}
				else
				{
					flag2 &= 239;
				}
			}
		}

		public bool ChFreeTone
		{
			get
			{
				return Convert.ToBoolean(flag2 & 8);
			}
			set
			{
				if (value)
				{
					flag2 |= 8;
				}
				else
				{
					flag2 &= 247;
				}
			}
		}

		public bool SelfTestPassTone
		{
			get
			{
				return Convert.ToBoolean(flag2 & 4);
			}
			set
			{
				if (value)
				{
					flag2 |= 4;
				}
				else
				{
					flag2 &= 251;
				}
			}
		}

		public int TalkPermitTone
		{
			get
			{
				return flag2 & 3;
			}
			set
			{
				flag2 &= 252;
				flag2 |= Convert.ToByte(value & 3);
			}
		}

		public bool PrivateCall
		{
			get
			{
				return Convert.ToBoolean(flag3 & 0x80);
			}
			set
			{
				if (value)
				{
					flag3 |= 128;
				}
				else
				{
					flag3 &= 127;
				}
			}
		}

		public bool TxInhibitQuickOverride
		{
			get
			{
				return Convert.ToBoolean(flag3 & 0x40);
			}
			set
			{
				if (value)
				{
					flag3 |= 64;
				}
				else
				{
					flag3 &= 191;
				}
			}
		}

		public bool DisableAllLeds
		{
			get
			{
				return Convert.ToBoolean(flag3 & 0x20);
			}
			set
			{
				if (value)
				{
					flag3 |= 32;
				}
				else
				{
					flag3 &= 223;
				}
			}
		}

		public bool DataEnableCtrl
		{
			get
			{
				return Convert.ToBoolean(flag3 & 0x10);
			}
			set
			{
				if (value)
				{
					flag3 |= 16;
				}
				else
				{
					flag3 &= 239;
				}
			}
		}

		public bool TestMode
		{
			get
			{
				return Convert.ToBoolean(flag3 & 8);
			}
			set
			{
				if (value)
				{
					flag3 |= 8;
				}
				else
				{
					flag3 &= 247;
				}
			}
		}

		public int KillState
		{
			set
			{
				flag3 &= 251;
			}
		}

		public bool TxExitTone
		{
			get
			{
				return Convert.ToBoolean(flag4 & 8);
			}
			set
			{
				if (value)
				{
					flag4 |= 8;
				}
				else
				{
					flag4 &= 247;
				}
			}
		}

		public int ScanMode
		{
			get
			{
				int num = (flag4 & 0xC0) >> 6;
				if (Enum.IsDefined(typeof(ArtsTone), num))
				{
					return num;
				}
				return 1;
			}
			set
			{
				flag4 &= 63;
				if (!Enum.IsDefined(typeof(ScanModeE), value))
				{
					value = 1;
				}
				flag4 |= Convert.ToByte((value & 3) << 6);
			}
		}

		public string PrgPwd
		{
			get
			{
				int num = 0;
				StringBuilder stringBuilder = new StringBuilder(8);
				for (num = 0; num < 8 && prgPwd[num] != byte.MaxValue; num++)
				{
					stringBuilder.Append(Convert.ToChar(prgPwd[num]));
				}
				return stringBuilder.ToString();
			}
			set
			{
				int num = 0;
				prgPwd.Fill(byte.MaxValue);
				for (num = 0; num < value.Length; num++)
				{
					prgPwd[num] = Convert.ToByte(value[num]);
				}
			}
		}

		public GeneralSet()
		{
			callsign = new byte[8];
			radioId = new byte[4];
			reserver2 = new byte[2];
			reserve = new byte[5];
			prgPwd = new byte[8];
			reserve[0] = 1;
		}

		public void Verify(GeneralSet def)
		{
			Settings.ValidateNumberRangeWithDefault(ref txPreambleDur, 0, 144, def.txPreambleDur);
			if (!Enum.IsDefined(typeof(MonitorType), (int)monitorType))
			{
				monitorType = def.monitorType;
			}
			Settings.ValidateNumberRangeWithDefault(ref voxSense, 1, 10, def.voxSense);
			Settings.ValidateNumberRangeWithDefault(ref rxLowBatt, 0, 127, def.rxLowBatt);
			Settings.ValidateNumberRangeWithDefault(ref callAlertDur, 0, 240, def.callAlertDur);
			Settings.ValidateNumberRangeWithDefault(ref reminderTmr, 1, byte.MaxValue, def.reminderTmr);
			Settings.ValidateNumberRangeWithDefault(ref respTmr, 1, byte.MaxValue, def.respTmr);
			Settings.ValidateNumberRangeWithDefault(ref grpHang, 0, 14, def.grpHang);
			Settings.ValidateNumberRangeWithDefault(ref privateHang, 0, 14, def.privateHang);
			int num = Settings.smethod_14(flag1, 6, 2);
			if (!Enum.IsDefined(typeof(ArtsTone), num))
			{
				ArtsTone = def.ArtsTone;
			}
			reserve[0] = 1;
		}
	}

	private const int LEN_RADIO_NAME = 8;

	private const int SPACE_RADIO_ID = 4;

	private const int MIN_RADIO_ID = 1;

	private const int MAX_RADIO_ID = 16776415;

	private const int INC_RADIO_ID = 1;

	private const int LEN_RADIO_ID = 8;

	private const string SZ_RADIO_ID = "0123456789\b";

	private const int MIN_ARS_INIT_DLY = 0;

	private const int MAX_ARS_INIT_DLY = 8;

	private const int INC_ARS_INIT_DLY = 1;

	private const int SCL_ARS_INIT_DLY = 30;

	private const int LEN_ARS_INIT_DLY = 3;

	private const int MIN_TX_PREAMBLE_DUR = 0;

	private const int MAX_TX_PREAMBLE_DUR = 144;

	private const int INC_TX_PREAMBLE_DUR = 1;

	private const int SCL_TX_PREAMBLE_DUR = 60;

	private const int LEN_TX_PREAMBLE_DUR = 4;

	public const int LEN_PRG_PWD = 8;

	public const string SZ_MONITOR_TYPE_NAME = "MonitorType";

	private const int MIN_VOX_SENSE = 1;

	private const int MAX_VOX_SENSE = 10;

	public const string SZ_TALK_PERMIT_TONE_NAME = "TalkPermitTone";

	private const int MIN_RX_LOW_BATT = 0;

	private const int MAX_RX_LOW_BATT = 127;

	private const int INC_RX_LOW_BATT = 1;

	private const int SCL_RX_LOW_BATT = 5;

	private const int LEN_RX_LOW_BATT = 3;

	private const int MIN_CALL_ALERT_DUR = 0;

	private const int MAX_CALL_ALERT_DUR = 240;

	private const int SCL_CALL_ALERT_DUR = 5;

	private const int INC_CALL_ALERT_DUR = 1;

	private const int LEN_CALL_ALERT_DUR = 4;

	public const string SZ_CH_MODE_NAME = "ChannelMode";

	public const string SZ_ARTS_TONE_NAME = "ArtsToneName";

	public const string SZ_SCAN_MODE_NAME = "ScanMode";

	private const int MIN_RESP_TMR = 1;

	private const int MAX_RESP_TMR = 255;

	private const int INC_RESP_TMR = 1;

	private const int SCL_RESP_TMR = 1;

	private const int LEN_RESP_TIEMR = 3;

	private const int MIN_REMINDER_TMR = 1;

	private const int MAX_REMINDER_TMR = 255;

	private const int INC_REMINDER_TMR = 1;

	private const int SCL_REMINDER_TMR = 1;

	private const int LEN_REMINDER_TMR = 3;

	private const int MIN_GRP_HANG = 0;

	private const int MAX_GRP_HANG = 14;

	private const int INC_GRP_HANG = 1;

	private const int SCL_GRP_HANG = 500;

	private const int LEN_GRP_HANG = 4;

	private const int MIN_PRIVATE_HANG = 0;

	private const int MAX_PRIVATE_HANG = 14;

	private const int INC_PRIVATE_HANG = 1;

	private const int SCL_PRIVATE_HANG = 500;

	private const int LEN_PRIVATE_HANG = 4;

	private Label lblRadioName;

	private SGTextBox txtCallsign;

	private Label label_0;

	private SGTextBox txtRadioId;

	private Label lblTxPreambleDur;

	private ComboBox cmbMonitorType;

	private Label lblMonitorType;

	private CheckBox chkTxInhibit;

	private CheckBox chkDisableAllLeds;

	private CheckBox chkDataEnCtrlStation;

	private CheckBox chkTestMode;

	private CheckBox chkKillState;

	private Label lblVoxSense;

	private ComboBox cmbVoxSense;

	private Label lblProgramPwd;

	private SGTextBox txtProgramPwd;

	private CustomNumericUpDown nudTxPreambleDur;

	private CheckBox chkBatteryPreamble;

	private CheckBox chkBatteryRx;

	private CheckBox chkDisableAllTone;

	private CheckBox chkCrescendoTone;

	private CheckBox chkChFreeTone;

	private CheckBox chkSelfTestPassTone;

	private Label lblTalkPermitTone;

	private ComboBox cmbTalkPermitTone;

	private Label lblCallAlertDur;

	private CustomNumericUpDown nudCallAlertDur;

	private Label lblPrivateHang;

	private CustomNumericUpDown nudPrivateHang;

	private Label lblGrpHold;

	private CustomNumericUpDown nudGrpHang;

	private Label lblReminderTmr;

	private Label lblRespTmr;

	private CustomNumericUpDown nudReminderTmr;

	private CustomNumericUpDown nudRespTmr;

	private Label lblRxLowBatt;

	private CustomNumericUpDown nudRxLowBatt;

	private Label lblArtsTone;

	private ComboBox cmbArtsTone;

	private CheckBox chkChVoice;

	private Label lblVoiceLang;

	private ComboBox cmbVoiceLang;

	private CheckBox chkPrivateCall;

	private Label lblArsInitDly;

	private CustomNumericUpDown nudArsInitDly;

	private GroupBox grpSaveMode;

	private GroupBox grpLoneWork;

	private GroupBox grpTalkAround;

	private CustomPanel pnlFill;

	private GroupBox grpBeep;

	private GroupBox grpVoice;

	private CheckBox chkResetTone;

	private CheckBox chkUnifamiliarNumber;

	private CheckBox chkTxExitTone;

	private GroupBox grpScan;

	private Label lblScanMode;

	private ComboBox cmbScanMode;

	private Label lblDownChMode;

	private Label lblUpChMode;

	private ComboBox cmbDownChMode;

	private ComboBox cmbUpChMode;

	private static readonly string[] SZ_MONITOR_TYPE;

	private static readonly string[] SZ_TALK_PERMIT_TONE;

	private static readonly string[] SZ_CH_MODE;

	private static readonly string[] SZ_ARTS_TONE;

	private static readonly string[] SZ_SCAN_MODE;

	private static readonly string[] SZ_VOICE_LANG;

	public static GeneralSet DefaultGeneralSet;

	private Button btnLookupIDForCallsign;

	public static GeneralSet data;

	public TreeNode Node { get; set; }

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.pnlFill = new CustomPanel();
		this.btnLookupIDForCallsign = new System.Windows.Forms.Button();
		this.lblDownChMode = new System.Windows.Forms.Label();
		this.lblUpChMode = new System.Windows.Forms.Label();
		this.cmbDownChMode = new System.Windows.Forms.ComboBox();
		this.cmbUpChMode = new System.Windows.Forms.ComboBox();
		this.grpScan = new System.Windows.Forms.GroupBox();
		this.lblScanMode = new System.Windows.Forms.Label();
		this.cmbScanMode = new System.Windows.Forms.ComboBox();
		this.chkTxExitTone = new System.Windows.Forms.CheckBox();
		this.grpBeep = new System.Windows.Forms.GroupBox();
		this.chkResetTone = new System.Windows.Forms.CheckBox();
		this.chkUnifamiliarNumber = new System.Windows.Forms.CheckBox();
		this.lblCallAlertDur = new System.Windows.Forms.Label();
		this.nudCallAlertDur = new CustomNumericUpDown();
		this.chkSelfTestPassTone = new System.Windows.Forms.CheckBox();
		this.chkCrescendoTone = new System.Windows.Forms.CheckBox();
		this.chkChFreeTone = new System.Windows.Forms.CheckBox();
		this.chkDisableAllTone = new System.Windows.Forms.CheckBox();
		this.lblTalkPermitTone = new System.Windows.Forms.Label();
		this.cmbTalkPermitTone = new System.Windows.Forms.ComboBox();
		this.lblArtsTone = new System.Windows.Forms.Label();
		this.cmbArtsTone = new System.Windows.Forms.ComboBox();
		this.grpVoice = new System.Windows.Forms.GroupBox();
		this.chkChVoice = new System.Windows.Forms.CheckBox();
		this.cmbVoiceLang = new System.Windows.Forms.ComboBox();
		this.lblVoiceLang = new System.Windows.Forms.Label();
		this.grpSaveMode = new System.Windows.Forms.GroupBox();
		this.chkBatteryRx = new System.Windows.Forms.CheckBox();
		this.chkBatteryPreamble = new System.Windows.Forms.CheckBox();
		this.grpLoneWork = new System.Windows.Forms.GroupBox();
		this.nudRespTmr = new CustomNumericUpDown();
		this.lblReminderTmr = new System.Windows.Forms.Label();
		this.lblRespTmr = new System.Windows.Forms.Label();
		this.nudReminderTmr = new CustomNumericUpDown();
		this.grpTalkAround = new System.Windows.Forms.GroupBox();
		this.nudGrpHang = new CustomNumericUpDown();
		this.nudPrivateHang = new CustomNumericUpDown();
		this.lblPrivateHang = new System.Windows.Forms.Label();
		this.lblGrpHold = new System.Windows.Forms.Label();
		this.chkDisableAllLeds = new System.Windows.Forms.CheckBox();
		this.lblRadioName = new System.Windows.Forms.Label();
		this.txtCallsign = new DMR.SGTextBox();
		this.label_0 = new System.Windows.Forms.Label();
		this.lblTxPreambleDur = new System.Windows.Forms.Label();
		this.nudArsInitDly = new CustomNumericUpDown();
		this.lblProgramPwd = new System.Windows.Forms.Label();
		this.nudRxLowBatt = new CustomNumericUpDown();
		this.txtRadioId = new DMR.SGTextBox();
		this.nudTxPreambleDur = new CustomNumericUpDown();
		this.lblRxLowBatt = new System.Windows.Forms.Label();
		this.chkKillState = new System.Windows.Forms.CheckBox();
		this.chkTestMode = new System.Windows.Forms.CheckBox();
		this.lblArsInitDly = new System.Windows.Forms.Label();
		this.chkDataEnCtrlStation = new System.Windows.Forms.CheckBox();
		this.txtProgramPwd = new DMR.SGTextBox();
		this.lblMonitorType = new System.Windows.Forms.Label();
		this.cmbMonitorType = new System.Windows.Forms.ComboBox();
		this.lblVoxSense = new System.Windows.Forms.Label();
		this.cmbVoxSense = new System.Windows.Forms.ComboBox();
		this.chkPrivateCall = new System.Windows.Forms.CheckBox();
		this.chkTxInhibit = new System.Windows.Forms.CheckBox();
		this.pnlFill.SuspendLayout();
		this.grpScan.SuspendLayout();
		this.grpBeep.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nudCallAlertDur).BeginInit();
		this.grpVoice.SuspendLayout();
		this.grpSaveMode.SuspendLayout();
		this.grpLoneWork.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nudRespTmr).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudReminderTmr).BeginInit();
		this.grpTalkAround.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nudGrpHang).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudPrivateHang).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudArsInitDly).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudRxLowBatt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudTxPreambleDur).BeginInit();
		base.SuspendLayout();
		this.pnlFill.AutoScroll = true;
		this.pnlFill.AutoSize = true;
		this.pnlFill.Controls.Add(this.btnLookupIDForCallsign);
		this.pnlFill.Controls.Add(this.lblDownChMode);
		this.pnlFill.Controls.Add(this.lblUpChMode);
		this.pnlFill.Controls.Add(this.cmbDownChMode);
		this.pnlFill.Controls.Add(this.cmbUpChMode);
		this.pnlFill.Controls.Add(this.grpScan);
		this.pnlFill.Controls.Add(this.chkTxExitTone);
		this.pnlFill.Controls.Add(this.grpBeep);
		this.pnlFill.Controls.Add(this.grpVoice);
		this.pnlFill.Controls.Add(this.grpSaveMode);
		this.pnlFill.Controls.Add(this.grpLoneWork);
		this.pnlFill.Controls.Add(this.grpTalkAround);
		this.pnlFill.Controls.Add(this.chkDisableAllLeds);
		this.pnlFill.Controls.Add(this.lblRadioName);
		this.pnlFill.Controls.Add(this.txtCallsign);
		this.pnlFill.Controls.Add(this.label_0);
		this.pnlFill.Controls.Add(this.lblTxPreambleDur);
		this.pnlFill.Controls.Add(this.nudArsInitDly);
		this.pnlFill.Controls.Add(this.lblProgramPwd);
		this.pnlFill.Controls.Add(this.nudRxLowBatt);
		this.pnlFill.Controls.Add(this.txtRadioId);
		this.pnlFill.Controls.Add(this.nudTxPreambleDur);
		this.pnlFill.Controls.Add(this.lblRxLowBatt);
		this.pnlFill.Controls.Add(this.chkKillState);
		this.pnlFill.Controls.Add(this.chkTestMode);
		this.pnlFill.Controls.Add(this.lblArsInitDly);
		this.pnlFill.Controls.Add(this.chkDataEnCtrlStation);
		this.pnlFill.Controls.Add(this.txtProgramPwd);
		this.pnlFill.Controls.Add(this.lblMonitorType);
		this.pnlFill.Controls.Add(this.cmbMonitorType);
		this.pnlFill.Controls.Add(this.lblVoxSense);
		this.pnlFill.Controls.Add(this.cmbVoxSense);
		this.pnlFill.Controls.Add(this.chkPrivateCall);
		this.pnlFill.Controls.Add(this.chkTxInhibit);
		this.pnlFill.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnlFill.Location = new System.Drawing.Point(0, 0);
		this.pnlFill.Name = "pnlFill";
		this.pnlFill.Size = new System.Drawing.Size(309, 121);
		this.pnlFill.TabIndex = 0;
		this.btnLookupIDForCallsign.Location = new System.Drawing.Point(122, 41);
		this.btnLookupIDForCallsign.Name = "btnLookupIDForCallsign";
		this.btnLookupIDForCallsign.Size = new System.Drawing.Size(158, 23);
		this.btnLookupIDForCallsign.TabIndex = 37;
		this.btnLookupIDForCallsign.Text = "Lookup Id for Callsign";
		this.btnLookupIDForCallsign.UseVisualStyleBackColor = true;
		this.btnLookupIDForCallsign.Click += new System.EventHandler(btnLookupIDForCallsign_Click);
		this.lblDownChMode.Location = new System.Drawing.Point(47, 470);
		this.lblDownChMode.Name = "lblDownChMode";
		this.lblDownChMode.Size = new System.Drawing.Size(186, 24);
		this.lblDownChMode.TabIndex = 35;
		this.lblDownChMode.Text = "Down Channel Mode";
		this.lblDownChMode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblDownChMode.Visible = false;
		this.lblUpChMode.Location = new System.Drawing.Point(47, 429);
		this.lblUpChMode.Name = "lblUpChMode";
		this.lblUpChMode.Size = new System.Drawing.Size(186, 24);
		this.lblUpChMode.TabIndex = 35;
		this.lblUpChMode.Text = "Up Channel Mode";
		this.lblUpChMode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblUpChMode.Visible = false;
		this.cmbDownChMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbDownChMode.ForeColor = System.Drawing.SystemColors.WindowText;
		this.cmbDownChMode.FormattingEnabled = true;
		this.cmbDownChMode.Items.AddRange(new object[2] { "Open Squelch", "Silent" });
		this.cmbDownChMode.Location = new System.Drawing.Point(248, 470);
		this.cmbDownChMode.Name = "cmbDownChMode";
		this.cmbDownChMode.Size = new System.Drawing.Size(120, 24);
		this.cmbDownChMode.TabIndex = 36;
		this.cmbDownChMode.Visible = false;
		this.cmbUpChMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbUpChMode.ForeColor = System.Drawing.SystemColors.WindowText;
		this.cmbUpChMode.FormattingEnabled = true;
		this.cmbUpChMode.Items.AddRange(new object[2] { "Open Squelch", "Silent" });
		this.cmbUpChMode.Location = new System.Drawing.Point(248, 429);
		this.cmbUpChMode.Name = "cmbUpChMode";
		this.cmbUpChMode.Size = new System.Drawing.Size(120, 24);
		this.cmbUpChMode.TabIndex = 36;
		this.cmbUpChMode.Visible = false;
		this.grpScan.Controls.Add(this.lblScanMode);
		this.grpScan.Controls.Add(this.cmbScanMode);
		this.grpScan.Location = new System.Drawing.Point(484, 527);
		this.grpScan.Name = "grpScan";
		this.grpScan.Size = new System.Drawing.Size(460, 67);
		this.grpScan.TabIndex = 34;
		this.grpScan.TabStop = false;
		this.grpScan.Text = "Scan";
		this.grpScan.Visible = false;
		this.lblScanMode.Location = new System.Drawing.Point(29, 24);
		this.lblScanMode.Name = "lblScanMode";
		this.lblScanMode.Size = new System.Drawing.Size(181, 24);
		this.lblScanMode.TabIndex = 28;
		this.lblScanMode.Text = "Scan Mode";
		this.lblScanMode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.cmbScanMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbScanMode.FormattingEnabled = true;
		this.cmbScanMode.Items.AddRange(new object[4] { "None", "Digital", "Analog", "Both" });
		this.cmbScanMode.Location = new System.Drawing.Point(223, 24);
		this.cmbScanMode.Name = "cmbScanMode";
		this.cmbScanMode.Size = new System.Drawing.Size(121, 24);
		this.cmbScanMode.TabIndex = 29;
		this.chkTxExitTone.AutoSize = true;
		this.chkTxExitTone.Location = new System.Drawing.Point(707, 240);
		this.chkTxExitTone.Name = "chkTxExitTone";
		this.chkTxExitTone.Size = new System.Drawing.Size(102, 20);
		this.chkTxExitTone.TabIndex = 33;
		this.chkTxExitTone.Text = "Tx Exit Tone";
		this.chkTxExitTone.UseVisualStyleBackColor = true;
		this.chkTxExitTone.Visible = false;
		this.grpBeep.Controls.Add(this.chkResetTone);
		this.grpBeep.Controls.Add(this.chkUnifamiliarNumber);
		this.grpBeep.Controls.Add(this.lblCallAlertDur);
		this.grpBeep.Controls.Add(this.nudCallAlertDur);
		this.grpBeep.Controls.Add(this.chkSelfTestPassTone);
		this.grpBeep.Controls.Add(this.chkCrescendoTone);
		this.grpBeep.Controls.Add(this.chkChFreeTone);
		this.grpBeep.Controls.Add(this.chkDisableAllTone);
		this.grpBeep.Controls.Add(this.lblTalkPermitTone);
		this.grpBeep.Controls.Add(this.cmbTalkPermitTone);
		this.grpBeep.Controls.Add(this.lblArtsTone);
		this.grpBeep.Controls.Add(this.cmbArtsTone);
		this.grpBeep.Location = new System.Drawing.Point(484, 21);
		this.grpBeep.Name = "grpBeep";
		this.grpBeep.Size = new System.Drawing.Size(465, 248);
		this.grpBeep.TabIndex = 32;
		this.grpBeep.TabStop = false;
		this.grpBeep.Text = "Alert Tone";
		this.grpBeep.Visible = false;
		this.chkResetTone.AutoSize = true;
		this.chkResetTone.Location = new System.Drawing.Point(223, 196);
		this.chkResetTone.Name = "chkResetTone";
		this.chkResetTone.Size = new System.Drawing.Size(98, 20);
		this.chkResetTone.TabIndex = 29;
		this.chkResetTone.Text = "Reset Tone";
		this.chkResetTone.UseVisualStyleBackColor = true;
		this.chkUnifamiliarNumber.AutoSize = true;
		this.chkUnifamiliarNumber.Location = new System.Drawing.Point(223, 174);
		this.chkUnifamiliarNumber.Name = "chkUnifamiliarNumber";
		this.chkUnifamiliarNumber.Size = new System.Drawing.Size(179, 20);
		this.chkUnifamiliarNumber.TabIndex = 28;
		this.chkUnifamiliarNumber.Text = "Unifamiliar Number Tone";
		this.chkUnifamiliarNumber.UseVisualStyleBackColor = true;
		this.lblCallAlertDur.Location = new System.Drawing.Point(29, 117);
		this.lblCallAlertDur.Name = "lblCallAlertDur";
		this.lblCallAlertDur.Size = new System.Drawing.Size(181, 24);
		this.lblCallAlertDur.TabIndex = 4;
		this.lblCallAlertDur.Text = "Call Alert Tone Duration [s]";
		this.lblCallAlertDur.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.nudCallAlertDur.Increment = new decimal(new int[4] { 25, 0, 0, 0 });
		this.nudCallAlertDur.Location = new System.Drawing.Point(223, 117);
		this.nudCallAlertDur.Maximum = new decimal(new int[4] { 6375, 0, 0, 0 });
		this.nudCallAlertDur.Minimum = new decimal(new int[4] { 50, 0, 0, 0 });
		this.nudCallAlertDur.Name = "nudCallAlertDur";
		this.nudCallAlertDur.Size = new System.Drawing.Size(120, 23);
		this.nudCallAlertDur.TabIndex = 17;
		this.nudCallAlertDur.Value = new decimal(new int[4] { 50, 0, 0, 0 });
		this.chkSelfTestPassTone.AutoSize = true;
		this.chkSelfTestPassTone.Location = new System.Drawing.Point(223, 64);
		this.chkSelfTestPassTone.Name = "chkSelfTestPassTone";
		this.chkSelfTestPassTone.Size = new System.Drawing.Size(150, 20);
		this.chkSelfTestPassTone.TabIndex = 25;
		this.chkSelfTestPassTone.Text = "Self Test Pass Tone";
		this.chkSelfTestPassTone.UseVisualStyleBackColor = true;
		this.chkCrescendoTone.AutoSize = true;
		this.chkCrescendoTone.Location = new System.Drawing.Point(59, 20);
		this.chkCrescendoTone.Name = "chkCrescendoTone";
		this.chkCrescendoTone.Size = new System.Drawing.Size(130, 20);
		this.chkCrescendoTone.TabIndex = 23;
		this.chkCrescendoTone.Text = "Enhanced Tone*";
		this.chkCrescendoTone.UseVisualStyleBackColor = true;
		this.chkCrescendoTone.Visible = false;
		this.chkChFreeTone.AutoSize = true;
		this.chkChFreeTone.Location = new System.Drawing.Point(223, 42);
		this.chkChFreeTone.Name = "chkChFreeTone";
		this.chkChFreeTone.Size = new System.Drawing.Size(212, 20);
		this.chkChFreeTone.TabIndex = 24;
		this.chkChFreeTone.Text = "Channel Free Indication Tone";
		this.chkChFreeTone.UseVisualStyleBackColor = true;
		this.chkDisableAllTone.AutoSize = true;
		this.chkDisableAllTone.Location = new System.Drawing.Point(223, 20);
		this.chkDisableAllTone.Name = "chkDisableAllTone";
		this.chkDisableAllTone.Size = new System.Drawing.Size(126, 20);
		this.chkDisableAllTone.TabIndex = 22;
		this.chkDisableAllTone.Text = "Disable All Tone";
		this.chkDisableAllTone.UseVisualStyleBackColor = true;
		this.lblTalkPermitTone.Location = new System.Drawing.Point(29, 88);
		this.lblTalkPermitTone.Name = "lblTalkPermitTone";
		this.lblTalkPermitTone.Size = new System.Drawing.Size(181, 24);
		this.lblTalkPermitTone.TabIndex = 26;
		this.lblTalkPermitTone.Text = "Talk Permit Tone";
		this.lblTalkPermitTone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.cmbTalkPermitTone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbTalkPermitTone.FormattingEnabled = true;
		this.cmbTalkPermitTone.Items.AddRange(new object[4] { "None", "Digital", "Analog", "Both" });
		this.cmbTalkPermitTone.Location = new System.Drawing.Point(223, 88);
		this.cmbTalkPermitTone.Name = "cmbTalkPermitTone";
		this.cmbTalkPermitTone.Size = new System.Drawing.Size(121, 24);
		this.cmbTalkPermitTone.TabIndex = 27;
		this.lblArtsTone.Location = new System.Drawing.Point(29, 146);
		this.lblArtsTone.Name = "lblArtsTone";
		this.lblArtsTone.Size = new System.Drawing.Size(181, 24);
		this.lblArtsTone.TabIndex = 12;
		this.lblArtsTone.Text = "ARTS Tone";
		this.lblArtsTone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.cmbArtsTone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbArtsTone.FormattingEnabled = true;
		this.cmbArtsTone.Items.AddRange(new object[3] { "Diabled", "Once", "Always" });
		this.cmbArtsTone.Location = new System.Drawing.Point(223, 146);
		this.cmbArtsTone.Name = "cmbArtsTone";
		this.cmbArtsTone.Size = new System.Drawing.Size(121, 24);
		this.cmbArtsTone.TabIndex = 13;
		this.grpVoice.Controls.Add(this.chkChVoice);
		this.grpVoice.Controls.Add(this.cmbVoiceLang);
		this.grpVoice.Controls.Add(this.lblVoiceLang);
		this.grpVoice.Location = new System.Drawing.Point(22, 500);
		this.grpVoice.Name = "grpVoice";
		this.grpVoice.Size = new System.Drawing.Size(391, 86);
		this.grpVoice.TabIndex = 31;
		this.grpVoice.TabStop = false;
		this.grpVoice.Text = "Voice";
		this.grpVoice.Visible = false;
		this.chkChVoice.AutoSize = true;
		this.chkChVoice.Location = new System.Drawing.Point(214, 18);
		this.chkChVoice.Name = "chkChVoice";
		this.chkChVoice.Size = new System.Drawing.Size(117, 20);
		this.chkChVoice.TabIndex = 14;
		this.chkChVoice.Text = "Channel Voice";
		this.chkChVoice.UseVisualStyleBackColor = true;
		this.cmbVoiceLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbVoiceLang.FormattingEnabled = true;
		this.cmbVoiceLang.Items.AddRange(new object[2] { "English", "Chinese" });
		this.cmbVoiceLang.Location = new System.Drawing.Point(214, 42);
		this.cmbVoiceLang.Name = "cmbVoiceLang";
		this.cmbVoiceLang.Size = new System.Drawing.Size(121, 24);
		this.cmbVoiceLang.TabIndex = 13;
		this.lblVoiceLang.Location = new System.Drawing.Point(65, 42);
		this.lblVoiceLang.Name = "lblVoiceLang";
		this.lblVoiceLang.Size = new System.Drawing.Size(141, 24);
		this.lblVoiceLang.TabIndex = 12;
		this.lblVoiceLang.Text = "Voice Language";
		this.lblVoiceLang.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.grpSaveMode.Controls.Add(this.chkBatteryRx);
		this.grpSaveMode.Controls.Add(this.chkBatteryPreamble);
		this.grpSaveMode.Location = new System.Drawing.Point(484, 275);
		this.grpSaveMode.Name = "grpSaveMode";
		this.grpSaveMode.Size = new System.Drawing.Size(465, 63);
		this.grpSaveMode.TabIndex = 30;
		this.grpSaveMode.TabStop = false;
		this.grpSaveMode.Text = "Battery Saver";
		this.grpSaveMode.Visible = false;
		this.chkBatteryRx.AutoSize = true;
		this.chkBatteryRx.Location = new System.Drawing.Point(223, 40);
		this.chkBatteryRx.Name = "chkBatteryRx";
		this.chkBatteryRx.Size = new System.Drawing.Size(77, 20);
		this.chkBatteryRx.TabIndex = 21;
		this.chkBatteryRx.Text = "Receive";
		this.chkBatteryRx.UseVisualStyleBackColor = true;
		this.chkBatteryPreamble.AutoSize = true;
		this.chkBatteryPreamble.Location = new System.Drawing.Point(223, 18);
		this.chkBatteryPreamble.Name = "chkBatteryPreamble";
		this.chkBatteryPreamble.Size = new System.Drawing.Size(86, 20);
		this.chkBatteryPreamble.TabIndex = 20;
		this.chkBatteryPreamble.Text = "Preamble";
		this.chkBatteryPreamble.UseVisualStyleBackColor = true;
		this.chkBatteryPreamble.CheckedChanged += new System.EventHandler(chkBatteryPreamble_CheckedChanged);
		this.grpLoneWork.Controls.Add(this.nudRespTmr);
		this.grpLoneWork.Controls.Add(this.lblReminderTmr);
		this.grpLoneWork.Controls.Add(this.lblRespTmr);
		this.grpLoneWork.Controls.Add(this.nudReminderTmr);
		this.grpLoneWork.Location = new System.Drawing.Point(484, 344);
		this.grpLoneWork.Name = "grpLoneWork";
		this.grpLoneWork.Size = new System.Drawing.Size(465, 82);
		this.grpLoneWork.TabIndex = 29;
		this.grpLoneWork.TabStop = false;
		this.grpLoneWork.Text = "Lone Worker";
		this.grpLoneWork.Visible = false;
		this.nudRespTmr.Location = new System.Drawing.Point(223, 22);
		this.nudRespTmr.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudRespTmr.Minimum = new decimal(new int[4] { 1, 0, 0, 0 });
		this.nudRespTmr.Name = "nudRespTmr";
		this.nudRespTmr.Size = new System.Drawing.Size(120, 23);
		this.nudRespTmr.TabIndex = 17;
		this.nudRespTmr.Value = new decimal(new int[4] { 50, 0, 0, 0 });
		this.lblReminderTmr.Location = new System.Drawing.Point(29, 49);
		this.lblReminderTmr.Name = "lblReminderTmr";
		this.lblReminderTmr.Size = new System.Drawing.Size(181, 24);
		this.lblReminderTmr.TabIndex = 4;
		this.lblReminderTmr.Text = "Reminder Timer [s]";
		this.lblReminderTmr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblRespTmr.Location = new System.Drawing.Point(29, 22);
		this.lblRespTmr.Name = "lblRespTmr";
		this.lblRespTmr.Size = new System.Drawing.Size(181, 24);
		this.lblRespTmr.TabIndex = 4;
		this.lblRespTmr.Text = "Response Timer [min]";
		this.lblRespTmr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.nudReminderTmr.Location = new System.Drawing.Point(223, 49);
		this.nudReminderTmr.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudReminderTmr.Minimum = new decimal(new int[4] { 1, 0, 0, 0 });
		this.nudReminderTmr.Name = "nudReminderTmr";
		this.nudReminderTmr.Size = new System.Drawing.Size(120, 23);
		this.nudReminderTmr.TabIndex = 17;
		this.nudReminderTmr.Value = new decimal(new int[4] { 50, 0, 0, 0 });
		this.grpTalkAround.Controls.Add(this.nudGrpHang);
		this.grpTalkAround.Controls.Add(this.nudPrivateHang);
		this.grpTalkAround.Controls.Add(this.lblPrivateHang);
		this.grpTalkAround.Controls.Add(this.lblGrpHold);
		this.grpTalkAround.Location = new System.Drawing.Point(484, 429);
		this.grpTalkAround.Name = "grpTalkAround";
		this.grpTalkAround.Size = new System.Drawing.Size(465, 86);
		this.grpTalkAround.TabIndex = 28;
		this.grpTalkAround.TabStop = false;
		this.grpTalkAround.Text = "Talkaround";
		this.grpTalkAround.Visible = false;
		this.nudGrpHang.Increment = new decimal(new int[4] { 500, 0, 0, 0 });
		this.nudGrpHang.Location = new System.Drawing.Point(223, 20);
		this.nudGrpHang.Maximum = new decimal(new int[4] { 7000, 0, 0, 0 });
		this.nudGrpHang.Name = "nudGrpHang";
		this.nudGrpHang.Size = new System.Drawing.Size(120, 23);
		this.nudGrpHang.TabIndex = 17;
		this.nudGrpHang.Value = new decimal(new int[4] { 50, 0, 0, 0 });
		this.nudPrivateHang.Increment = new decimal(new int[4] { 500, 0, 0, 0 });
		this.nudPrivateHang.Location = new System.Drawing.Point(223, 49);
		this.nudPrivateHang.Maximum = new decimal(new int[4] { 7000, 0, 0, 0 });
		this.nudPrivateHang.Name = "nudPrivateHang";
		this.nudPrivateHang.Size = new System.Drawing.Size(120, 23);
		this.nudPrivateHang.TabIndex = 17;
		this.nudPrivateHang.Value = new decimal(new int[4] { 50, 0, 0, 0 });
		this.lblPrivateHang.Location = new System.Drawing.Point(29, 49);
		this.lblPrivateHang.Name = "lblPrivateHang";
		this.lblPrivateHang.Size = new System.Drawing.Size(181, 24);
		this.lblPrivateHang.TabIndex = 4;
		this.lblPrivateHang.Text = "Group Call Hang Time [ms]";
		this.lblPrivateHang.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblGrpHold.Location = new System.Drawing.Point(29, 20);
		this.lblGrpHold.Name = "lblGrpHold";
		this.lblGrpHold.Size = new System.Drawing.Size(181, 24);
		this.lblGrpHold.TabIndex = 4;
		this.lblGrpHold.Text = "Private Call Hang Time [ms]";
		this.lblGrpHold.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.chkDisableAllLeds.AutoSize = true;
		this.chkDisableAllLeds.Location = new System.Drawing.Point(249, 223);
		this.chkDisableAllLeds.Name = "chkDisableAllLeds";
		this.chkDisableAllLeds.Size = new System.Drawing.Size(122, 20);
		this.chkDisableAllLeds.TabIndex = 12;
		this.chkDisableAllLeds.Text = "Disable All LED";
		this.chkDisableAllLeds.UseVisualStyleBackColor = true;
		this.chkDisableAllLeds.Visible = false;
		this.lblRadioName.Location = new System.Drawing.Point(10, 14);
		this.lblRadioName.Name = "lblRadioName";
		this.lblRadioName.Size = new System.Drawing.Size(134, 24);
		this.lblRadioName.TabIndex = 0;
		this.lblRadioName.Text = "Radio Name";
		this.lblRadioName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.txtCallsign.InputString = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		this.txtCallsign.Location = new System.Drawing.Point(160, 14);
		this.txtCallsign.MaxByteLength = 0;
		this.txtCallsign.Name = "txtCallsign";
		this.txtCallsign.OnlyAllowInputStringAndCapitaliseCharacters = true;
		this.txtCallsign.Size = new System.Drawing.Size(120, 23);
		this.txtCallsign.TabIndex = 1;
		this.label_0.Location = new System.Drawing.Point(13, 70);
		this.label_0.Name = "label_0";
		this.label_0.Size = new System.Drawing.Size(131, 24);
		this.label_0.TabIndex = 2;
		this.label_0.Text = "DMR ID";
		this.label_0.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblTxPreambleDur.Location = new System.Drawing.Point(47, 94);
		this.lblTxPreambleDur.Name = "lblTxPreambleDur";
		this.lblTxPreambleDur.Size = new System.Drawing.Size(186, 24);
		this.lblTxPreambleDur.TabIndex = 6;
		this.lblTxPreambleDur.Text = "Tx Preamble Duration [ms]";
		this.lblTxPreambleDur.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblTxPreambleDur.Visible = false;
		this.nudArsInitDly.Increment = new decimal(new int[4] { 30, 0, 0, 0 });
		this.nudArsInitDly.Location = new System.Drawing.Point(249, 382);
		this.nudArsInitDly.Maximum = new decimal(new int[4] { 240, 0, 0, 0 });
		this.nudArsInitDly.Name = "nudArsInitDly";
		this.nudArsInitDly.Size = new System.Drawing.Size(120, 23);
		this.nudArsInitDly.TabIndex = 5;
		this.nudArsInitDly.Value = new decimal(new int[4] { 50, 0, 0, 0 });
		this.nudArsInitDly.Visible = false;
		this.lblProgramPwd.Location = new System.Drawing.Point(47, 246);
		this.lblProgramPwd.Name = "lblProgramPwd";
		this.lblProgramPwd.Size = new System.Drawing.Size(186, 24);
		this.lblProgramPwd.TabIndex = 16;
		this.lblProgramPwd.Text = "Program Password";
		this.lblProgramPwd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblProgramPwd.Visible = false;
		this.nudRxLowBatt.Increment = new decimal(new int[4] { 5, 0, 0, 0 });
		this.nudRxLowBatt.Location = new System.Drawing.Point(248, 121);
		this.nudRxLowBatt.Maximum = new decimal(new int[4] { 635, 0, 0, 0 });
		this.nudRxLowBatt.Name = "nudRxLowBatt";
		this.nudRxLowBatt.Size = new System.Drawing.Size(120, 23);
		this.nudRxLowBatt.TabIndex = 17;
		this.nudRxLowBatt.Value = new decimal(new int[4] { 50, 0, 0, 0 });
		this.nudRxLowBatt.Visible = false;
		this.txtRadioId.InputString = null;
		this.txtRadioId.Location = new System.Drawing.Point(160, 70);
		this.txtRadioId.MaxByteLength = 0;
		this.txtRadioId.Name = "txtRadioId";
		this.txtRadioId.OnlyAllowInputStringAndCapitaliseCharacters = false;
		this.txtRadioId.Size = new System.Drawing.Size(120, 23);
		this.txtRadioId.TabIndex = 3;
		this.txtRadioId.Leave += new System.EventHandler(txtRadioId_Leave);
		this.txtRadioId.Validating += new System.ComponentModel.CancelEventHandler(txtRadioId_Validating);
		this.nudTxPreambleDur.Increment = new decimal(new int[4] { 60, 0, 0, 0 });
		this.nudTxPreambleDur.Location = new System.Drawing.Point(249, 94);
		this.nudTxPreambleDur.Maximum = new decimal(new int[4] { 8640, 0, 0, 0 });
		this.nudTxPreambleDur.Name = "nudTxPreambleDur";
		this.nudTxPreambleDur.Size = new System.Drawing.Size(120, 23);
		this.nudTxPreambleDur.TabIndex = 7;
		this.nudTxPreambleDur.Value = new decimal(new int[4] { 50, 0, 0, 0 });
		this.nudTxPreambleDur.Visible = false;
		this.lblRxLowBatt.Location = new System.Drawing.Point(47, 121);
		this.lblRxLowBatt.Name = "lblRxLowBatt";
		this.lblRxLowBatt.Size = new System.Drawing.Size(186, 24);
		this.lblRxLowBatt.TabIndex = 4;
		this.lblRxLowBatt.Text = "Rx Low Battery Interval [s]";
		this.lblRxLowBatt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblRxLowBatt.Visible = false;
		this.chkKillState.AutoSize = true;
		this.chkKillState.Location = new System.Drawing.Point(249, 356);
		this.chkKillState.Name = "chkKillState";
		this.chkKillState.Size = new System.Drawing.Size(81, 20);
		this.chkKillState.TabIndex = 15;
		this.chkKillState.Text = "Kill State";
		this.chkKillState.UseVisualStyleBackColor = true;
		this.chkKillState.Visible = false;
		this.chkTestMode.AutoSize = true;
		this.chkTestMode.Location = new System.Drawing.Point(249, 334);
		this.chkTestMode.Name = "chkTestMode";
		this.chkTestMode.Size = new System.Drawing.Size(91, 20);
		this.chkTestMode.TabIndex = 14;
		this.chkTestMode.Text = "Test Mode";
		this.chkTestMode.UseVisualStyleBackColor = true;
		this.chkTestMode.Visible = false;
		this.lblArsInitDly.Location = new System.Drawing.Point(47, 382);
		this.lblArsInitDly.Name = "lblArsInitDly";
		this.lblArsInitDly.Size = new System.Drawing.Size(186, 24);
		this.lblArsInitDly.TabIndex = 4;
		this.lblArsInitDly.Text = "ARS Initialization Delay [min]";
		this.lblArsInitDly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblArsInitDly.Visible = false;
		this.chkDataEnCtrlStation.AutoSize = true;
		this.chkDataEnCtrlStation.Location = new System.Drawing.Point(249, 312);
		this.chkDataEnCtrlStation.Name = "chkDataEnCtrlStation";
		this.chkDataEnCtrlStation.Size = new System.Drawing.Size(210, 20);
		this.chkDataEnCtrlStation.TabIndex = 13;
		this.chkDataEnCtrlStation.Text = "Data Enabled Control Station";
		this.chkDataEnCtrlStation.UseVisualStyleBackColor = true;
		this.chkDataEnCtrlStation.Visible = false;
		this.txtProgramPwd.InputString = null;
		this.txtProgramPwd.Location = new System.Drawing.Point(249, 246);
		this.txtProgramPwd.MaxByteLength = 0;
		this.txtProgramPwd.Name = "txtProgramPwd";
		this.txtProgramPwd.OnlyAllowInputStringAndCapitaliseCharacters = false;
		this.txtProgramPwd.Size = new System.Drawing.Size(121, 23);
		this.txtProgramPwd.TabIndex = 17;
		this.txtProgramPwd.Visible = false;
		this.lblMonitorType.Location = new System.Drawing.Point(47, 151);
		this.lblMonitorType.Name = "lblMonitorType";
		this.lblMonitorType.Size = new System.Drawing.Size(186, 24);
		this.lblMonitorType.TabIndex = 8;
		this.lblMonitorType.Text = "Monitor Type";
		this.lblMonitorType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblMonitorType.Visible = false;
		this.cmbMonitorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbMonitorType.ForeColor = System.Drawing.SystemColors.WindowText;
		this.cmbMonitorType.FormattingEnabled = true;
		this.cmbMonitorType.Items.AddRange(new object[2] { "Open Squelch", "Silent" });
		this.cmbMonitorType.Location = new System.Drawing.Point(249, 151);
		this.cmbMonitorType.Name = "cmbMonitorType";
		this.cmbMonitorType.Size = new System.Drawing.Size(120, 24);
		this.cmbMonitorType.TabIndex = 9;
		this.cmbMonitorType.Visible = false;
		this.lblVoxSense.Location = new System.Drawing.Point(47, 272);
		this.lblVoxSense.Name = "lblVoxSense";
		this.lblVoxSense.Size = new System.Drawing.Size(186, 24);
		this.lblVoxSense.TabIndex = 18;
		this.lblVoxSense.Text = "Vox Sensitivity";
		this.lblVoxSense.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblVoxSense.Visible = false;
		this.cmbVoxSense.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbVoxSense.FormattingEnabled = true;
		this.cmbVoxSense.Items.AddRange(new object[10] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
		this.cmbVoxSense.Location = new System.Drawing.Point(249, 272);
		this.cmbVoxSense.Name = "cmbVoxSense";
		this.cmbVoxSense.Size = new System.Drawing.Size(121, 24);
		this.cmbVoxSense.TabIndex = 19;
		this.cmbVoxSense.Visible = false;
		this.chkPrivateCall.AutoSize = true;
		this.chkPrivateCall.Location = new System.Drawing.Point(249, 179);
		this.chkPrivateCall.Name = "chkPrivateCall";
		this.chkPrivateCall.Size = new System.Drawing.Size(98, 20);
		this.chkPrivateCall.TabIndex = 10;
		this.chkPrivateCall.Text = "Private Call";
		this.chkPrivateCall.UseVisualStyleBackColor = true;
		this.chkPrivateCall.Visible = false;
		this.chkTxInhibit.AutoSize = true;
		this.chkTxInhibit.Location = new System.Drawing.Point(249, 201);
		this.chkTxInhibit.Name = "chkTxInhibit";
		this.chkTxInhibit.Size = new System.Drawing.Size(209, 20);
		this.chkTxInhibit.TabIndex = 11;
		this.chkTxInhibit.Text = "Tx Inhibit Quick Key Override";
		this.chkTxInhibit.UseVisualStyleBackColor = true;
		this.chkTxInhibit.Visible = false;
		base.ClientSize = new System.Drawing.Size(309, 121);
		base.Controls.Add(this.pnlFill);
		this.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "GeneralSetForm";
		this.Text = "General Setting";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(GeneralSetForm_FormClosing);
		base.Load += new System.EventHandler(GeneralSetForm_Load);
		this.pnlFill.ResumeLayout(false);
		this.pnlFill.PerformLayout();
		this.grpScan.ResumeLayout(false);
		this.grpBeep.ResumeLayout(false);
		this.grpBeep.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.nudCallAlertDur).EndInit();
		this.grpVoice.ResumeLayout(false);
		this.grpVoice.PerformLayout();
		this.grpSaveMode.ResumeLayout(false);
		this.grpSaveMode.PerformLayout();
		this.grpLoneWork.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.nudRespTmr).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudReminderTmr).EndInit();
		this.grpTalkAround.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.nudGrpHang).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudPrivateHang).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudArsInitDly).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudRxLowBatt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudTxPreambleDur).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	public void SaveData()
	{
		try
		{
			data.Callsign = txtCallsign.Text.ToUpper();
			data.RadioId = txtRadioId.Text;
			data.ArsInitDly = nudArsInitDly.Value;
			data.TxPreambleDur = nudTxPreambleDur.Value;
			data.MonitorType = cmbMonitorType.SelectedIndex;
			data.VoxSense = cmbVoxSense.SelectedIndex + 1;
			data.RxLowBatt = nudRxLowBatt.Value;
			data.CallAlertDur = nudCallAlertDur.Value;
			data.RespTmr = nudRespTmr.Value;
			data.ReminderTmr = nudReminderTmr.Value;
			data.GrpHang = nudGrpHang.Value;
			data.PrivateHang = nudPrivateHang.Value;
			data.ArtsTone = cmbArtsTone.SelectedIndex;
			data.ChVoice = chkChVoice.Checked;
			data.VoiceLang = cmbVoiceLang.SelectedIndex;
			data.BattPreamble = chkBatteryPreamble.Checked;
			data.BattRx = chkBatteryRx.Checked;
			data.UnfamiliarNumber = chkUnifamiliarNumber.Checked;
			data.ResetTone = chkResetTone.Checked;
			data.UpChMode = cmbUpChMode.SelectedIndex;
			data.DownChMode = cmbDownChMode.SelectedIndex;
			data.DisableAllTones = chkDisableAllTone.Checked;
			data.CrescendoTone = chkCrescendoTone.Checked;
			data.ChFreeTone = chkChFreeTone.Checked;
			data.SelfTestPassTone = chkSelfTestPassTone.Checked;
			data.TalkPermitTone = cmbTalkPermitTone.SelectedIndex;
			data.PrivateCall = chkPrivateCall.Checked;
			data.TxInhibitQuickOverride = chkTxInhibit.Checked;
			data.DisableAllLeds = chkDisableAllLeds.Checked;
			data.DataEnableCtrl = chkDataEnCtrlStation.Checked;
			data.TestMode = chkTestMode.Checked;
			data.TxExitTone = chkTxExitTone.Checked;
			data.ScanMode = cmbScanMode.SelectedIndex;
			data.PrgPwd = txtProgramPwd.Text;
			Settings.objectToByteArray(data, Marshal.SizeOf(data.GetType()));
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void DispData()
	{
		try
		{
			method_1();
			txtCallsign.Text = data.Callsign;
			txtRadioId.Text = data.RadioId;
			nudArsInitDly.Value = data.ArsInitDly;
			nudTxPreambleDur.Value = data.TxPreambleDur;
			cmbMonitorType.SelectedIndex = data.MonitorType;
			cmbVoxSense.SelectedIndex = data.VoxSense - 1;
			nudRxLowBatt.Value = data.RxLowBatt;
			nudCallAlertDur.Value = data.CallAlertDur;
			nudRespTmr.Value = data.RespTmr;
			nudReminderTmr.Value = data.ReminderTmr;
			nudGrpHang.Value = data.GrpHang;
			nudPrivateHang.Value = data.PrivateHang;
			cmbArtsTone.SelectedIndex = data.ArtsTone;
			chkChVoice.Checked = data.ChVoice;
			cmbVoiceLang.SelectedIndex = data.VoiceLang;
			chkUnifamiliarNumber.Checked = data.UnfamiliarNumber;
			chkResetTone.Checked = data.ResetTone;
			cmbUpChMode.SelectedIndex = data.UpChMode;
			cmbDownChMode.SelectedIndex = data.DownChMode;
			chkBatteryPreamble.Checked = data.BattPreamble;
			chkBatteryRx.Checked = data.BattRx;
			chkDisableAllTone.Checked = data.DisableAllTones;
			chkCrescendoTone.Checked = data.CrescendoTone;
			chkChFreeTone.Checked = data.ChFreeTone;
			chkSelfTestPassTone.Checked = data.SelfTestPassTone;
			cmbTalkPermitTone.SelectedIndex = data.TalkPermitTone;
			chkPrivateCall.Checked = data.PrivateCall;
			chkTxInhibit.Checked = data.TxInhibitQuickOverride;
			chkDisableAllLeds.Checked = data.DisableAllLeds;
			chkDataEnCtrlStation.Checked = data.DataEnableCtrl;
			chkTestMode.Checked = data.TestMode;
			chkTxExitTone.Checked = data.TxExitTone;
			cmbScanMode.SelectedIndex = data.ScanMode;
			txtProgramPwd.Text = data.PrgPwd;
			method_2();
			RefreshByUserMode();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void RefreshByUserMode()
	{
		bool flag = Settings.getUserExpertSettings() == Settings.UserMode.Expert;
		lblTxPreambleDur.Enabled &= flag;
		nudTxPreambleDur.Enabled &= flag;
		lblRxLowBatt.Enabled &= flag;
		nudRxLowBatt.Enabled &= flag;
		lblMonitorType.Enabled &= flag;
		cmbMonitorType.Enabled &= flag;
		chkPrivateCall.Enabled &= flag;
		chkTxInhibit.Enabled &= flag;
		chkDisableAllLeds.Enabled &= flag;
		lblProgramPwd.Enabled &= flag;
		txtProgramPwd.Enabled &= flag;
		lblVoxSense.Enabled &= flag;
		cmbVoxSense.Enabled &= flag;
		lblTalkPermitTone.Enabled &= flag;
		cmbTalkPermitTone.Enabled &= flag;
		lblCallAlertDur.Enabled &= flag;
		nudCallAlertDur.Enabled &= flag;
		lblArtsTone.Enabled &= flag;
		cmbArtsTone.Enabled &= flag;
		chkUnifamiliarNumber.Enabled &= flag;
		chkResetTone.Enabled &= flag;
		chkBatteryPreamble.Enabled &= flag;
		chkBatteryRx.Enabled &= flag;
		lblRespTmr.Enabled &= flag;
		nudRespTmr.Enabled &= flag;
		lblReminderTmr.Enabled &= flag;
		nudReminderTmr.Enabled &= flag;
		lblGrpHold.Enabled &= flag;
		nudGrpHang.Enabled &= flag;
		lblPrivateHang.Enabled &= flag;
		nudPrivateHang.Enabled &= flag;
	}

	public void RefreshName()
	{
	}

	public GeneralSetForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	private void method_1()
	{
		txtCallsign.MaxLength = 8;
		txtRadioId.MaxLength = 8;
		txtRadioId.InputString = "0123456789\b";
		Settings.smethod_36(nudArsInitDly, new Class13(0, 8, 1, 30m, 3));
		Settings.smethod_36(nudTxPreambleDur, new Class13(0, 144, 1, 60m, 4));
		Settings.smethod_37(cmbMonitorType, SZ_MONITOR_TYPE);
		Settings.smethod_41(cmbVoxSense, 1, 10);
		Settings.smethod_37(cmbTalkPermitTone, SZ_TALK_PERMIT_TONE);
		Settings.smethod_36(nudRxLowBatt, new Class13(0, 127, 1, 5m, 3));
		Settings.smethod_36(nudCallAlertDur, new Class13(0, 240, 1, 5m, 4));
		nudCallAlertDur.method_4(0m);
		nudCallAlertDur.method_6("∞");
		Settings.smethod_37(cmbUpChMode, SZ_CH_MODE);
		Settings.smethod_37(cmbDownChMode, SZ_CH_MODE);
		Settings.smethod_37(cmbArtsTone, SZ_ARTS_TONE);
		Settings.smethod_36(nudRespTmr, new Class13(1, 255, 1, 1m, 3));
		Settings.smethod_36(nudReminderTmr, new Class13(1, 255, 1, 1m, 3));
		Settings.smethod_36(nudGrpHang, new Class13(0, 14, 1, 500m, 4));
		Settings.smethod_36(nudPrivateHang, new Class13(0, 14, 1, 500m, 4));
		Settings.smethod_37(cmbVoiceLang, SZ_VOICE_LANG);
		Settings.smethod_37(cmbScanMode, SZ_SCAN_MODE);
		txtProgramPwd.MaxByteLength = 8;
		txtProgramPwd.InputString = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz\b";
	}

	public static void RefreshCommonLang()
	{
		string name = typeof(GeneralSetForm).Name;
		Settings.smethod_78("MonitorType", SZ_MONITOR_TYPE, name);
		Settings.smethod_78("TalkPermitTone", SZ_TALK_PERMIT_TONE, name);
		Settings.smethod_78("ArtsToneName", SZ_ARTS_TONE, name);
		Settings.smethod_78("ChannelMode", SZ_CH_MODE, name);
		Settings.smethod_78("ScanMode", SZ_SCAN_MODE, name);
	}

	private void GeneralSetForm_Load(object sender, EventArgs e)
	{
		Settings.smethod_59(base.Controls);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		DispData();
	}

	private void GeneralSetForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveData();
	}

	private void txtRadioId_Validating(object sender, CancelEventArgs e)
	{
		if (string.IsNullOrEmpty(txtRadioId.Text))
		{
			e.Cancel = true;
			MessageBox.Show(Settings.dicCommon["IdNotEmpty"]);
			txtRadioId.Focus();
			txtRadioId.SelectAll();
			return;
		}
		int num = Convert.ToInt32(txtRadioId.Text);
		if (num < 1 || num > 16776415)
		{
			e.Cancel = true;
			MessageBox.Show(Settings.dicCommon["IdOutOfRange"]);
			txtRadioId.Text = 16776415.ToString();
			txtRadioId.Focus();
			txtRadioId.SelectAll();
		}
	}

	private void txtRadioId_Leave(object sender, EventArgs e)
	{
	}

	private void method_2()
	{
		if (!chkBatteryPreamble.Checked)
		{
			chkBatteryRx.Checked = false;
			chkBatteryRx.Enabled = false;
		}
		else
		{
			chkBatteryRx.Enabled = true;
		}
	}

	private void chkBatteryPreamble_CheckedChanged(object sender, EventArgs e)
	{
		method_2();
	}

	static GeneralSetForm()
	{
		SZ_MONITOR_TYPE = new string[2] { "Open Squelch", "Silent" };
		SZ_TALK_PERMIT_TONE = new string[4] { "None", "Digital", "Analog", "Both" };
		SZ_CH_MODE = new string[2] { "Channel", "VFO" };
		SZ_ARTS_TONE = new string[3] { "Disabled", "Once", "Always" };
		SZ_SCAN_MODE = new string[3] { "Time", "Carrier", "Search" };
		SZ_VOICE_LANG = new string[2] { "English", "Chinese" };
		data = new GeneralSet();
	}

	private void btnLookupIDForCallsign_Click(object sender, EventArgs e)
	{
		WebClient webClient = new WebClient();
		try
		{
			Cursor.Current = Cursors.WaitCursor;
			Refresh();
			Application.DoEvents();
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			webClient.DownloadStringCompleted += downloadFromRadioIdCompleteHandler;
			webClient.DownloadStringAsync(new Uri("https://database.radioid.net/api/dmr/user/?callsign=" + txtCallsign.Text));
		}
		catch (Exception)
		{
			Cursor.Current = Cursors.Default;
			MessageBox.Show(Settings.dicCommon["UnableDownloadFromInternet"]);
		}
	}

	private void downloadFromRadioIdCompleteHandler(object sender, DownloadStringCompletedEventArgs e)
	{
		string text = "";
		try
		{
			text = e.Result;
		}
		catch (Exception)
		{
			MessageBox.Show(Settings.dicCommon["UnableDownloadFromInternet"]);
			return;
		}
		try
		{
			RadioIdResults radioIdResults = new JavaScriptSerializer().Deserialize<RadioIdResults>(text);
			switch (radioIdResults.count)
			{
			case 0:
				MessageBox.Show("No ID found for this callsign");
				break;
			case 1:
				txtRadioId.Text = radioIdResults.results[0].id;
				break;
			default:
				MessageBox.Show("This station has more than one ID");
				break;
			}
			Cursor.Current = Cursors.Default;
		}
		catch (Exception)
		{
			MessageBox.Show(Settings.dicCommon["ErrorParsingData"]);
		}
		finally
		{
			Cursor.Current = Cursors.Default;
		}
	}
}
