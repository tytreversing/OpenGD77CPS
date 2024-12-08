using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Properties;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class APRSForm : DockContent, IDisp
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct APRS_One : IVerify<APRS_One>
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		private byte[] name;

		private byte senderSSID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		private byte[] latitude;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		private byte[] longitude;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		private byte[] via1;

		private byte via1SSID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		private byte[] via2;

		private byte via2SSID;

		private byte iconTable;

		private byte iconIndex;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
		private byte[] comment;

		private uint txFreq;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		private byte[] reserved;

		private byte flags;

		private ushort magicVer;

		public string Name
		{
			get
			{
				return Settings.bufferToString(name);
			}
			set
			{
				byte[] array = Settings.stringToBuffer(value);
				name.Fill(byte.MaxValue);
				Array.Copy(array, 0, name, 0, Math.Min(array.Length, name.Length));
			}
		}

		public string Frequency
		{
			get
			{
				try
				{
					uint uint_ = 0u;
					if (txFreq != 0)
					{
						double double_ = Settings.convert10HzStepFreqToDecimalText((int)txFreq, 100000);
						if (Settings.checkFrequecyIsInValidRange(double_, ref uint_) == -1)
						{
							double_ = uint_;
						}
						return double_.ToString("f5");
					}
					return "";
				}
				catch
				{
					return "";
				}
			}
			set
			{
				try
				{
					if (value.Trim().Length > 0)
					{
						decimal value2 = decimal.Parse(value) * 100000m;
						txFreq = Convert.ToUInt32(value2);
					}
				}
				catch
				{
					txFreq = 0u;
				}
			}
		}

		public string Longitude
		{
			get
			{
				uint num = (uint)((longitude[2] << 16) + (longitude[1] << 8) + longitude[0]);
				if ((num & 0x7FFFFF) > 5898240)
				{
					return "0";
				}
				return ChannelForm.ChannelOne.LatLonBin24ToString(num);
			}
			set
			{
				try
				{
					NumberFormatInfo numberFormat = CultureInfo.CurrentCulture.NumberFormat;
					string s = value.Replace(".", numberFormat.NumberDecimalSeparator);
					double num = 0.0;
					try
					{
						num = double.Parse(s);
					}
					catch (Exception)
					{
					}
					if (num >= -180.0 || num <= 180.0)
					{
						uint num2 = ChannelForm.ChannelOne.LatLonStringToBin24(value);
						longitude[0] = (byte)(num2 & 0xFF);
						longitude[1] = (byte)((num2 >> 8) & 0xFF);
						longitude[2] = (byte)((num2 >> 16) & 0xFF);
						return;
					}
					throw new Exception();
				}
				catch
				{
					longitude[0] = (longitude[1] = (longitude[2] = 0));
				}
			}
		}

		public string Latitude
		{
			get
			{
				uint num = (uint)((latitude[2] << 16) + (latitude[1] << 8) + latitude[0]);
				if ((num & 0x7FFFFF) > 2949120)
				{
					return "0";
				}
				return ChannelForm.ChannelOne.LatLonBin24ToString(num);
			}
			set
			{
				try
				{
					double num = double.Parse(value);
					if (num >= -90.0 || num <= 90.0)
					{
						uint num2 = ChannelForm.ChannelOne.LatLonStringToBin24(value);
						latitude[0] = (byte)(num2 & 0xFF);
						latitude[1] = (byte)((num2 >> 8) & 0xFF);
						latitude[2] = (byte)((num2 >> 16) & 0xFF);
						return;
					}
					throw new Exception();
				}
				catch
				{
					latitude[0] = (latitude[1] = (latitude[2] = 0));
				}
			}
		}

		public string Comment
		{
			get
			{
				return Settings.bufferToString(comment);
			}
			set
			{
				byte[] array = Settings.stringToBuffer(value);
				comment.Fill((byte)0);
				Array.Copy(array, 0, comment, 0, Math.Min(array.Length, comment.Length));
			}
		}

		public string Via1
		{
			get
			{
				return Settings.bufferToString(via1);
			}
			set
			{
				byte[] array = Settings.stringToBuffer(value);
				via1.Fill((byte)0);
				Array.Copy(array, 0, via1, 0, Math.Min(array.Length, via1.Length));
			}
		}

		public string Via2
		{
			get
			{
				return Settings.bufferToString(via2);
			}
			set
			{
				byte[] array = Settings.stringToBuffer(value);
				via2.Fill((byte)0);
				Array.Copy(array, 0, via2, 0, Math.Min(array.Length, via2.Length));
			}
		}

		public int SSID
		{
			get
			{
				if (senderSSID >= 0 && senderSSID <= 15)
				{
					return senderSSID;
				}
				return 7;
			}
			set
			{
				if (value >= 0 && value <= 15)
				{
					senderSSID = Convert.ToByte(value);
				}
				else
				{
					senderSSID = 7;
				}
			}
		}

		public int Via1SSID
		{
			get
			{
				if (via1SSID >= 0 && via1SSID <= 15)
				{
					return via1SSID;
				}
				return 1;
			}
			set
			{
				if (value >= 0 && via1SSID <= 15)
				{
					via1SSID = Convert.ToByte(value);
				}
				else
				{
					via1SSID = 1;
				}
			}
		}

		public int Via2SSID
		{
			get
			{
				if (via2SSID >= 0 && via2SSID <= 15)
				{
					return via2SSID;
				}
				return 0;
			}
			set
			{
				if ((decimal)value >= 0m && via2SSID <= 15)
				{
					via2SSID = Convert.ToByte(value);
				}
				else
				{
					via2SSID = 0;
				}
			}
		}

		public int IconTableNumber
		{
			get
			{
				if (iconTable >= 0 && iconTable <= 1)
				{
					return iconTable;
				}
				return 0;
			}
			set
			{
				iconTable = Convert.ToByte(value);
			}
		}

		public int IconIndex
		{
			get
			{
				if (iconIndex >= 0 && iconIndex < 94)
				{
					return iconIndex;
				}
				return 0;
			}
			set
			{
				iconIndex = Convert.ToByte(value);
			}
		}

		public int BaudRate
		{
			get
			{
				return flags & 1;
			}
			set
			{
				flags = (byte)(flags & -2);
				if (value >= 0 && value <= 1 && value == 1)
				{
					flags |= 1;
				}
			}
		}

		public bool UsePosition
		{
			get
			{
				return (flags & 2) != 0;
			}
			set
			{
				flags = (byte)(flags & -3);
				if (value)
				{
					flags |= 2;
				}
			}
		}

		public int PositionMasking
		{
			get
			{
				return (flags & 0xF0) >> 4;
			}
			set
			{
				if (value >= 0 || value < 15)
				{
					flags = (byte)(flags & -241);
					flags |= (byte)(value << 4);
				}
			}
		}

		public ushort Magic
		{
			get
			{
				return magicVer;
			}
			set
			{
				magicVer = value;
			}
		}

		public APRS_One(int index)
		{
			this = default(APRS_One);
			name = new byte[8];
			latitude = new byte[3];
			longitude = new byte[3];
			via1 = new byte[6];
			via2 = new byte[6];
			comment = new byte[24];
			magicVer = 16722;
		}

		public APRS_One(APRS_One config)
		{
			this = default(APRS_One);
			name = new byte[8];
			latitude = new byte[3];
			longitude = new byte[3];
			via1 = new byte[6];
			via2 = new byte[6];
			comment = new byte[24];
			Array.Copy(config.name, name, 8);
			Array.Copy(config.latitude, latitude, 3);
			Array.Copy(config.longitude, longitude, 3);
			Array.Copy(config.via1, via1, 6);
			Array.Copy(config.via2, via2, 6);
			Array.Copy(config.comment, comment, 24);
			senderSSID = config.senderSSID;
			via1SSID = config.via1SSID;
			via2SSID = config.via2SSID;
			IconTableNumber = config.IconTableNumber;
			IconIndex = config.IconIndex;
			flags = config.flags;
			magicVer = 16722;
			txFreq = 0u;
		}

		public void Default()
		{
			Name = "";
			SSID = 7;
			Via1 = "WIDE1";
			Via1SSID = 1;
			Via2 = "WIDE2";
			Via2SSID = 1;
			IconTableNumber = 0;
			IconIndex = 15;
			Comment = "";
			PositionMasking = 0;
			UsePosition = false;
			Latitude = "0";
			Longitude = "0";
			BaudRate = 0;
			Frequency = "";
			Magic = 16722;
		}

		public APRS_One Clone()
		{
			return Settings.cloneObject(this);
		}

		public void Verify(APRS_One def)
		{
			if (name[0] != byte.MaxValue)
			{
				Settings.ValidateNumberRangeWithDefault(ref senderSSID, 0, 15, def.senderSSID);
				Settings.ValidateNumberRangeWithDefault(ref via2SSID, 0, 15, def.via2SSID);
				Settings.ValidateNumberRangeWithDefault(ref via1SSID, 0, 15, def.via1SSID);
				Settings.ValidateNumberRangeWithDefault(ref iconTable, 0, 1, def.iconTable);
				Settings.ValidateNumberRangeWithDefault(ref iconIndex, 0, 93, def.iconIndex);
				if ((uint)(((longitude[2] << 16) + (longitude[1] << 8) + longitude[0]) & 0x7FFFFF) > 5898240u)
				{
					longitude[0] = (longitude[0] = (longitude[0] = 0));
				}
				if ((uint)(((latitude[2] << 16) + (latitude[1] << 8) + latitude[0]) & 0x7FFFFF) > 2949120u)
				{
					latitude[0] = (latitude[0] = (latitude[0] = 0));
				}
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class APRS_Config : IData
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public APRS_One[] APRS_list;

		public APRS_One this[int index]
		{
			get
			{
				if (index >= Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				return APRS_list[index];
			}
			set
			{
				APRS_list[index] = value;
			}
		}

		public int Count => 8;

		public string Format => "APRS{0}";

		public bool ListIsEmpty
		{
			get
			{
				int num = 0;
				while (true)
				{
					if (num < Count)
					{
						if (DataIsValid(num))
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
		}

		public APRS_Config()
		{
			APRS_list = new APRS_One[8];
			for (int i = 0; i < APRS_list.Length; i++)
			{
				APRS_list[i] = new APRS_One(i);
				APRS_list[i].Name = "";
			}
		}

		public int GetMinIndex()
		{
			int num = 0;
			while (true)
			{
				if (num < Count)
				{
					if (string.IsNullOrEmpty(this[num].Name))
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

		public bool DataIsValid(int index)
		{
			try
			{
				return !string.IsNullOrEmpty(this[index].Name);
			}
			catch
			{
				return false;
			}
		}

		public void SetIndex(int index, int value)
		{
			if (value == 0)
			{
				SetName(index, "");
			}
		}

		public void ClearIndex(int index)
		{
			if (singleton != null)
			{
				singleton.Close();
				singleton = null;
			}
			SetName(index, "");
			ChannelForm.data.ClearByAPRS(index);
			VfoForm.data.ClearByAPRS(index);
		}

		public string GetMinName(TreeNode node)
		{
			int minIndex = GetMinIndex();
			string text = string.Format(Format, minIndex + 1);
			if (!Settings.smethod_51(node, text))
			{
				return text;
			}
			int num = 0;
			while (true)
			{
				if (num < Count)
				{
					text = string.Format(Format, num + 1);
					if (!Settings.smethod_51(node, text))
					{
						break;
					}
					num++;
					continue;
				}
				return "";
			}
			return text;
		}

		public void SetName(int index, string text)
		{
			APRS_list[index].Name = text;
		}

		public string GetName(int index)
		{
			return APRS_list[index].Name;
		}

		public void Default(int index)
		{
			APRS_list[index].Default();
		}

		public void Paste(int from, int to)
		{
			APRS_list[to].Latitude = APRS_list[from].Latitude;
			APRS_list[to].Longitude = APRS_list[from].Longitude;
			APRS_list[to].SSID = APRS_list[from].SSID;
			APRS_list[to].Via1 = APRS_list[from].Via1;
			APRS_list[to].Via2 = APRS_list[from].Via2;
			APRS_list[to].Via2SSID = APRS_list[from].Via2SSID;
			APRS_list[to].Via1SSID = APRS_list[from].Via1SSID;
			APRS_list[to].IconTableNumber = APRS_list[from].IconTableNumber;
			APRS_list[to].IconIndex = APRS_list[from].IconIndex;
			APRS_list[to].Comment = APRS_list[from].Comment;
			APRS_list[to].BaudRate = APRS_list[from].BaudRate;
			APRS_list[to].UsePosition = APRS_list[from].UsePosition;
			APRS_list[to].PositionMasking = APRS_list[from].PositionMasking;
			APRS_list[to].Frequency = APRS_list[from].Frequency;
			APRS_list[to].Magic = 16722;
		}

		public void Verify()
		{
			for (int i = 0; i < APRS_list.Length; i++)
			{
				APRS_list[i].Verify(DefaultAPRS_Config);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class EmergencyEx
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		private ushort[] revertCh;

		public EmergencyEx()
		{
			revertCh = new ushort[32];
		}
	}

	private static APRSForm singleton;

	private const ushort MAGIC_VER_NUMBER = 16722;

	private const byte APRS_ICONS_MAX = 94;

	private Label lblName;

	private Label lblSenderSSID;

	private Label lblVia1SSID;

	private Label lblIcon;

	private ComboBox cmbIconTable;

	private ComboBox cmbIconIndex;

	private ComboBox cmbTxSSID;

	private ComboBox cmbVia1SSID;

	private CustomPanel pnlEmergency;

	public static APRS_One DefaultAPRS_Config;

	public static APRS_Config data;

	private Label lblLatitude;

	private Label lblVia1;

	private ComboBox cmbVia2SSID;

	private Label lblVia2;

	private SGTextBox txtVia2;

	private SGTextBox txtVia1;

	private SGTextBox txtLatitude;

	private SGTextBox txtName;

	private Label lblComment;

	private SGTextBox txtMessage;

	private Label label1;

	private Label lblCallsign;

	private Label label2;

	private Label lblBaudRate;

	private ComboBox cmbBaudRate;

	private Label lblLongitude;

	private SGTextBox txtLongitude;

	private GroupBox grpFixedPosition;

	private CheckBox chkUsePosition;

	private Label lblPositionMasking;

	private ComboBox cbPositionMasking;

	private ImageList iconsList;

	private Label lblTxFrequency;

	private SGTextBox txtTxFrequency;

	public static EmergencyEx dataEx;

	public TreeNode Node { get; set; }

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.pnlEmergency = new CustomPanel();
		this.grpFixedPosition = new System.Windows.Forms.GroupBox();
		this.chkUsePosition = new System.Windows.Forms.CheckBox();
		this.lblLatitude = new System.Windows.Forms.Label();
		this.txtLatitude = new DMR.SGTextBox();
		this.txtLongitude = new DMR.SGTextBox();
		this.lblLongitude = new System.Windows.Forms.Label();
		this.cmbVia2SSID = new System.Windows.Forms.ComboBox();
		this.lblVia2 = new System.Windows.Forms.Label();
		this.cmbVia1SSID = new System.Windows.Forms.ComboBox();
		this.lblVia1 = new System.Windows.Forms.Label();
		this.lblBaudRate = new System.Windows.Forms.Label();
		this.lblPositionMasking = new System.Windows.Forms.Label();
		this.lblComment = new System.Windows.Forms.Label();
		this.lblTxFrequency = new System.Windows.Forms.Label();
		this.lblName = new System.Windows.Forms.Label();
		this.cmbTxSSID = new System.Windows.Forms.ComboBox();
		this.cbPositionMasking = new System.Windows.Forms.ComboBox();
		this.cmbBaudRate = new System.Windows.Forms.ComboBox();
		this.cmbIconIndex = new System.Windows.Forms.ComboBox();
		this.cmbIconTable = new System.Windows.Forms.ComboBox();
		this.lblCallsign = new System.Windows.Forms.Label();
		this.lblSenderSSID = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.lblVia1SSID = new System.Windows.Forms.Label();
		this.lblIcon = new System.Windows.Forms.Label();
		this.txtVia2 = new DMR.SGTextBox();
		this.txtVia1 = new DMR.SGTextBox();
		this.txtMessage = new DMR.SGTextBox();
		this.txtTxFrequency = new DMR.SGTextBox();
		this.txtName = new DMR.SGTextBox();
		this.pnlEmergency.SuspendLayout();
		this.grpFixedPosition.SuspendLayout();
		base.SuspendLayout();
		this.pnlEmergency.AutoScroll = true;
		this.pnlEmergency.AutoSize = true;
		this.pnlEmergency.Controls.Add(this.grpFixedPosition);
		this.pnlEmergency.Controls.Add(this.cmbVia2SSID);
		this.pnlEmergency.Controls.Add(this.lblVia2);
		this.pnlEmergency.Controls.Add(this.cmbVia1SSID);
		this.pnlEmergency.Controls.Add(this.lblVia1);
		this.pnlEmergency.Controls.Add(this.lblBaudRate);
		this.pnlEmergency.Controls.Add(this.lblPositionMasking);
		this.pnlEmergency.Controls.Add(this.lblComment);
		this.pnlEmergency.Controls.Add(this.lblTxFrequency);
		this.pnlEmergency.Controls.Add(this.lblName);
		this.pnlEmergency.Controls.Add(this.cmbTxSSID);
		this.pnlEmergency.Controls.Add(this.cbPositionMasking);
		this.pnlEmergency.Controls.Add(this.cmbBaudRate);
		this.pnlEmergency.Controls.Add(this.cmbIconIndex);
		this.pnlEmergency.Controls.Add(this.cmbIconTable);
		this.pnlEmergency.Controls.Add(this.lblCallsign);
		this.pnlEmergency.Controls.Add(this.lblSenderSSID);
		this.pnlEmergency.Controls.Add(this.label1);
		this.pnlEmergency.Controls.Add(this.label2);
		this.pnlEmergency.Controls.Add(this.lblVia1SSID);
		this.pnlEmergency.Controls.Add(this.lblIcon);
		this.pnlEmergency.Controls.Add(this.txtVia2);
		this.pnlEmergency.Controls.Add(this.txtVia1);
		this.pnlEmergency.Controls.Add(this.txtMessage);
		this.pnlEmergency.Controls.Add(this.txtTxFrequency);
		this.pnlEmergency.Controls.Add(this.txtName);
		this.pnlEmergency.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnlEmergency.Location = new System.Drawing.Point(0, 0);
		this.pnlEmergency.Name = "pnlEmergency";
		this.pnlEmergency.Size = new System.Drawing.Size(387, 403);
		this.pnlEmergency.TabIndex = 0;
		this.grpFixedPosition.Controls.Add(this.chkUsePosition);
		this.grpFixedPosition.Controls.Add(this.lblLatitude);
		this.grpFixedPosition.Controls.Add(this.txtLatitude);
		this.grpFixedPosition.Controls.Add(this.txtLongitude);
		this.grpFixedPosition.Controls.Add(this.lblLongitude);
		this.grpFixedPosition.Location = new System.Drawing.Point(108, 245);
		this.grpFixedPosition.Name = "grpFixedPosition";
		this.grpFixedPosition.Size = new System.Drawing.Size(241, 100);
		this.grpFixedPosition.TabIndex = 13;
		this.grpFixedPosition.TabStop = false;
		this.grpFixedPosition.Text = "Fixed position";
		this.chkUsePosition.Location = new System.Drawing.Point(11, 18);
		this.chkUsePosition.Name = "chkUsePosition";
		this.chkUsePosition.Size = new System.Drawing.Size(164, 20);
		this.chkUsePosition.TabIndex = 4;
		this.chkUsePosition.Text = "Use position";
		this.chkUsePosition.UseVisualStyleBackColor = true;
		this.lblLatitude.Location = new System.Drawing.Point(14, 41);
		this.lblLatitude.Name = "lblLatitude";
		this.lblLatitude.Size = new System.Drawing.Size(123, 24);
		this.lblLatitude.TabIndex = 0;
		this.lblLatitude.Text = "Latitude";
		this.lblLatitude.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.txtLatitude.AccessibleDescription = "Latitude";
		this.txtLatitude.AccessibleName = "Latitude";
		this.txtLatitude.InputString = null;
		this.txtLatitude.Location = new System.Drawing.Point(148, 42);
		this.txtLatitude.MaxByteLength = 0;
		this.txtLatitude.MaxLength = 8;
		this.txtLatitude.Name = "txtLatitude";
		this.txtLatitude.OnlyAllowInputStringAndCapitaliseCharacters = true;
		this.txtLatitude.Size = new System.Drawing.Size(84, 23);
		this.txtLatitude.TabIndex = 3;
		this.txtLatitude.Leave += new System.EventHandler(txtLatLong_Leave);
		this.txtLongitude.AccessibleDescription = "Longitude";
		this.txtLongitude.AccessibleName = "Longitude";
		this.txtLongitude.InputString = null;
		this.txtLongitude.Location = new System.Drawing.Point(148, 71);
		this.txtLongitude.MaxByteLength = 0;
		this.txtLongitude.MaxLength = 9;
		this.txtLongitude.Name = "txtLongitude";
		this.txtLongitude.OnlyAllowInputStringAndCapitaliseCharacters = true;
		this.txtLongitude.Size = new System.Drawing.Size(84, 23);
		this.txtLongitude.TabIndex = 3;
		this.txtLongitude.Leave += new System.EventHandler(txtLatLong_Leave);
		this.lblLongitude.Location = new System.Drawing.Point(17, 70);
		this.lblLongitude.Name = "lblLongitude";
		this.lblLongitude.Size = new System.Drawing.Size(120, 24);
		this.lblLongitude.TabIndex = 0;
		this.lblLongitude.Text = "Longitude";
		this.lblLongitude.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.cmbVia2SSID.AccessibleDescription = "Via2 SSID";
		this.cmbVia2SSID.AccessibleName = "Via2 SSID";
		this.cmbVia2SSID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbVia2SSID.Location = new System.Drawing.Point(312, 102);
		this.cmbVia2SSID.Name = "cmbVia2SSID";
		this.cmbVia2SSID.Size = new System.Drawing.Size(37, 24);
		this.cmbVia2SSID.TabIndex = 7;
		this.lblVia2.Location = new System.Drawing.Point(10, 103);
		this.lblVia2.Name = "lblVia2";
		this.lblVia2.Size = new System.Drawing.Size(131, 24);
		this.lblVia2.TabIndex = 0;
		this.lblVia2.Text = "Via 2";
		this.lblVia2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.cmbVia1SSID.AccessibleDescription = "Via1 SSID";
		this.cmbVia1SSID.AccessibleName = "Via1 SSID";
		this.cmbVia1SSID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbVia1SSID.Location = new System.Drawing.Point(312, 73);
		this.cmbVia1SSID.Name = "cmbVia1SSID";
		this.cmbVia1SSID.Size = new System.Drawing.Size(37, 24);
		this.cmbVia1SSID.TabIndex = 5;
		this.lblVia1.Location = new System.Drawing.Point(10, 74);
		this.lblVia1.Name = "lblVia1";
		this.lblVia1.Size = new System.Drawing.Size(131, 24);
		this.lblVia1.TabIndex = 0;
		this.lblVia1.Text = "Via 1";
		this.lblVia1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblBaudRate.Location = new System.Drawing.Point(35, 359);
		this.lblBaudRate.Name = "lblBaudRate";
		this.lblBaudRate.Size = new System.Drawing.Size(172, 24);
		this.lblBaudRate.TabIndex = 0;
		this.lblBaudRate.Text = "Baud rate";
		this.lblBaudRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblPositionMasking.Location = new System.Drawing.Point(13, 204);
		this.lblPositionMasking.Name = "lblPositionMasking";
		this.lblPositionMasking.Size = new System.Drawing.Size(234, 24);
		this.lblPositionMasking.TabIndex = 0;
		this.lblPositionMasking.Text = "Position masking";
		this.lblPositionMasking.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblComment.Location = new System.Drawing.Point(10, 161);
		this.lblComment.Name = "lblComment";
		this.lblComment.Size = new System.Drawing.Size(131, 24);
		this.lblComment.TabIndex = 0;
		this.lblComment.Text = "Comment";
		this.lblComment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblTxFrequency.Location = new System.Drawing.Point(76, 421);
		this.lblTxFrequency.Name = "lblTxFrequency";
		this.lblTxFrequency.Size = new System.Drawing.Size(131, 24);
		this.lblTxFrequency.TabIndex = 0;
		this.lblTxFrequency.Text = "Tx Frequency";
		this.lblTxFrequency.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblTxFrequency.Visible = false;
		this.lblName.Location = new System.Drawing.Point(10, 11);
		this.lblName.Name = "lblName";
		this.lblName.Size = new System.Drawing.Size(131, 24);
		this.lblName.TabIndex = 0;
		this.lblName.Text = "Name";
		this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.cmbTxSSID.AccessibleDescription = "Tx SSID";
		this.cmbTxSSID.AccessibleName = "Tx SSID";
		this.cmbTxSSID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbTxSSID.Location = new System.Drawing.Point(312, 40);
		this.cmbTxSSID.Name = "cmbTxSSID";
		this.cmbTxSSID.Size = new System.Drawing.Size(37, 24);
		this.cmbTxSSID.TabIndex = 2;
		this.cbPositionMasking.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbPositionMasking.Items.AddRange(new object[8] { "None", "0.0005 deg", "0.001 deg", "0.005 deg", "0.01 deg", "0.05 deg", "0.1 deg", "0.5 deg" });
		this.cbPositionMasking.Location = new System.Drawing.Point(253, 205);
		this.cbPositionMasking.Name = "cbPositionMasking";
		this.cbPositionMasking.Size = new System.Drawing.Size(96, 24);
		this.cbPositionMasking.TabIndex = 9;
		this.cmbBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbBaudRate.Items.AddRange(new object[2] { "1200 (VHF/UHF)", "300 (HF)" });
		this.cmbBaudRate.Location = new System.Drawing.Point(218, 362);
		this.cmbBaudRate.Name = "cmbBaudRate";
		this.cmbBaudRate.Size = new System.Drawing.Size(131, 24);
		this.cmbBaudRate.TabIndex = 9;
		this.cmbIconIndex.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
		this.cmbIconIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbIconIndex.Location = new System.Drawing.Point(280, 132);
		this.cmbIconIndex.Name = "cmbIconIndex";
		this.cmbIconIndex.Size = new System.Drawing.Size(69, 24);
		this.cmbIconIndex.TabIndex = 9;
		this.cmbIconIndex.DrawItem += new System.Windows.Forms.DrawItemEventHandler(comboBoxIcons_DrawItemEvent);
		this.cmbIconTable.AccessibleDescription = "Icon table number";
		this.cmbIconTable.AccessibleName = "Icon table number";
		this.cmbIconTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbIconTable.Items.AddRange(new object[2] { "Primary (/)", "Alternate (\\)" });
		this.cmbIconTable.Location = new System.Drawing.Point(152, 132);
		this.cmbIconTable.Name = "cmbIconTable";
		this.cmbIconTable.Size = new System.Drawing.Size(100, 24);
		this.cmbIconTable.TabIndex = 8;
		this.cmbIconTable.SelectedIndexChanged += new System.EventHandler(cmbIconTable_SelectedIndexChanged);
		this.lblCallsign.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.lblCallsign.Location = new System.Drawing.Point(152, 40);
		this.lblCallsign.Name = "lblCallsign";
		this.lblCallsign.Size = new System.Drawing.Size(132, 24);
		this.lblCallsign.TabIndex = 8;
		this.lblCallsign.Text = "VK3KYY";
		this.lblCallsign.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblSenderSSID.Location = new System.Drawing.Point(10, 40);
		this.lblSenderSSID.Name = "lblSenderSSID";
		this.lblSenderSSID.Size = new System.Drawing.Size(131, 24);
		this.lblSenderSSID.TabIndex = 8;
		this.lblSenderSSID.Text = "Tx SSID";
		this.lblSenderSSID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.label1.Location = new System.Drawing.Point(290, 102);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(16, 24);
		this.label1.TabIndex = 10;
		this.label1.Text = "-";
		this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.label2.Location = new System.Drawing.Point(290, 40);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(16, 24);
		this.label2.TabIndex = 10;
		this.label2.Text = "-";
		this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.lblVia1SSID.Location = new System.Drawing.Point(290, 73);
		this.lblVia1SSID.Name = "lblVia1SSID";
		this.lblVia1SSID.Size = new System.Drawing.Size(16, 24);
		this.lblVia1SSID.TabIndex = 10;
		this.lblVia1SSID.Text = "-";
		this.lblVia1SSID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.lblIcon.Location = new System.Drawing.Point(10, 132);
		this.lblIcon.Name = "lblIcon";
		this.lblIcon.Size = new System.Drawing.Size(131, 24);
		this.lblIcon.TabIndex = 12;
		this.lblIcon.Text = "Icon";
		this.lblIcon.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.txtVia2.AccessibleDescription = "Via2";
		this.txtVia2.AccessibleName = "Via2";
		this.txtVia2.InputString = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		this.txtVia2.Location = new System.Drawing.Point(218, 103);
		this.txtVia2.MaxByteLength = 0;
		this.txtVia2.MaxLength = 6;
		this.txtVia2.Name = "txtVia2";
		this.txtVia2.OnlyAllowInputStringAndCapitaliseCharacters = true;
		this.txtVia2.Size = new System.Drawing.Size(66, 23);
		this.txtVia2.TabIndex = 6;
		this.txtVia1.AccessibleDescription = "Via1";
		this.txtVia1.AccessibleName = "Via1";
		this.txtVia1.InputString = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		this.txtVia1.Location = new System.Drawing.Point(218, 74);
		this.txtVia1.MaxByteLength = 0;
		this.txtVia1.MaxLength = 6;
		this.txtVia1.Name = "txtVia1";
		this.txtVia1.OnlyAllowInputStringAndCapitaliseCharacters = true;
		this.txtVia1.Size = new System.Drawing.Size(66, 23);
		this.txtVia1.TabIndex = 4;
		this.txtMessage.AccessibleDescription = "Message";
		this.txtMessage.AccessibleName = "Message";
		this.txtMessage.InputString = null;
		this.txtMessage.Location = new System.Drawing.Point(152, 162);
		this.txtMessage.MaxByteLength = 0;
		this.txtMessage.MaxLength = 24;
		this.txtMessage.Name = "txtMessage";
		this.txtMessage.OnlyAllowInputStringAndCapitaliseCharacters = false;
		this.txtMessage.Size = new System.Drawing.Size(197, 23);
		this.txtMessage.TabIndex = 10;
		this.txtTxFrequency.AccessibleDescription = "APRS Configuation name";
		this.txtTxFrequency.AccessibleName = "APRSConfiguationName";
		this.txtTxFrequency.InputString = null;
		this.txtTxFrequency.Location = new System.Drawing.Point(217, 422);
		this.txtTxFrequency.MaxByteLength = 0;
		this.txtTxFrequency.MaxLength = 8;
		this.txtTxFrequency.Name = "txtTxFrequency";
		this.txtTxFrequency.OnlyAllowInputStringAndCapitaliseCharacters = false;
		this.txtTxFrequency.Size = new System.Drawing.Size(132, 23);
		this.txtTxFrequency.TabIndex = 1;
		this.txtTxFrequency.Visible = false;
		this.txtTxFrequency.Leave += new System.EventHandler(txtName_Leave);
		this.txtName.AccessibleDescription = "APRS Configuation name";
		this.txtName.AccessibleName = "APRSConfiguationName";
		this.txtName.InputString = null;
		this.txtName.Location = new System.Drawing.Point(152, 11);
		this.txtName.MaxByteLength = 0;
		this.txtName.MaxLength = 8;
		this.txtName.Name = "txtName";
		this.txtName.OnlyAllowInputStringAndCapitaliseCharacters = false;
		this.txtName.Size = new System.Drawing.Size(132, 23);
		this.txtName.TabIndex = 1;
		this.txtName.Leave += new System.EventHandler(txtName_Leave);
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(387, 403);
		base.Controls.Add(this.pnlEmergency);
		this.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.Name = "APRSForm";
		this.Text = "APRS";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(EmergencyForm_FormClosing);
		base.Load += new System.EventHandler(APRSForm_Load);
		this.pnlEmergency.ResumeLayout(false);
		this.pnlEmergency.PerformLayout();
		this.grpFixedPosition.ResumeLayout(false);
		this.grpFixedPosition.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	public void SaveData()
	{
		int index = Convert.ToInt32(base.Tag);
		if (txtName.Focused)
		{
			txtName_Leave(txtName, null);
		}
		if (txtLongitude.Focused)
		{
			txtLatLong_Leave(txtLongitude, null);
		}
		if (txtLatitude.Focused)
		{
			txtLatLong_Leave(txtLatitude, null);
		}
		APRS_One value = new APRS_One(index);
		value.Name = txtName.Text.Trim();
		value.SSID = cmbTxSSID.SelectedIndex;
		value.Frequency = txtTxFrequency.Text.Trim();
		value.Latitude = txtLatitude.Text.Trim().ToUpper();
		value.Longitude = txtLongitude.Text.Trim().ToUpper();
		value.Via1 = txtVia1.Text.Trim().ToUpper();
		if (value.Via1 == "")
		{
			value.Via1 = "WIDE2";
			value.Via1SSID = 1;
		}
		else
		{
			value.Via1SSID = cmbVia1SSID.SelectedIndex;
		}
		value.Via2 = txtVia2.Text.Trim().ToUpper();
		value.Via2SSID = cmbVia2SSID.SelectedIndex;
		value.IconTableNumber = cmbIconTable.SelectedIndex;
		value.IconIndex = cmbIconIndex.SelectedIndex;
		value.Comment = txtMessage.Text.Trim();
		value.BaudRate = cmbBaudRate.SelectedIndex;
		value.UsePosition = chkUsePosition.Checked;
		value.PositionMasking = cbPositionMasking.SelectedIndex;
		data[index] = value;
	}

	public void DispData()
	{
		int num = Convert.ToInt32(base.Tag);
		if (num == -1 || !data.DataIsValid(num))
		{
			Close();
			return;
		}
		txtName.Text = data[num].Name;
		cmbTxSSID.SelectedIndex = data[num].SSID;
		txtLatitude.Text = data[num].Latitude;
		txtLongitude.Text = data[num].Longitude;
		lblCallsign.Text = GeneralSetForm.data.Callsign;
		txtVia1.Text = data[num].Via1;
		cmbVia1SSID.SelectedIndex = data[num].Via1SSID;
		txtVia2.Text = data[num].Via2;
		cmbVia2SSID.SelectedIndex = data[num].Via2SSID;
		cmbIconTable.SelectedIndex = data[num].IconTableNumber;
		cmbIconIndex.SelectedIndex = data[num].IconIndex;
		txtMessage.Text = data[num].Comment;
		cmbBaudRate.SelectedIndex = data[num].BaudRate;
		chkUsePosition.Checked = data[num].UsePosition;
		cbPositionMasking.SelectedIndex = data[num].PositionMasking;
		txtTxFrequency.Text = data[num].Frequency;
		if (GeneralSetForm.data.Callsign == "")
		{
			MessageBox.Show("CALLSIGN ERROR");
			new GeneralSetForm().ShowDialog();
		}
	}

	public void RefreshName()
	{
		int index = Convert.ToInt32(base.Tag);
		txtName.Text = data[index].Name;
	}

	public APRSForm()
	{
		InitializeComponent();
		Bitmap bitmap = new Bitmap(Resources.APRSIconsPrimary);
		Bitmap bitmap2 = new Bitmap(Resources.APRSIconsAlternate);
		iconsList = new ImageList();
		iconsList.ImageSize = new Size(20, 20);
		iconsList.TransparentColor = Color.White;
		for (int i = 0; i < 94; i++)
		{
			Rectangle rect = new Rectangle(20 * i, 0, 20, 20);
			PixelFormat pixelFormat = bitmap.PixelFormat;
			char c = (char)(33 + i);
			iconsList.Images.Add(bitmap.Clone(rect, pixelFormat));
			cmbIconIndex.Items.Add((c == '&') ? "&&" : c.ToString());
		}
		for (int j = 0; j < 94; j++)
		{
			Rectangle rect2 = new Rectangle(20 * j, 0, 20, 20);
			PixelFormat pixelFormat2 = bitmap2.PixelFormat;
			iconsList.Images.Add(bitmap2.Clone(rect2, pixelFormat2));
		}
		object[] array = new object[16];
		for (int k = 0; k <= 15; k++)
		{
			array[k] = k.ToString();
		}
		cmbVia1SSID.Items.AddRange(array);
		cmbVia2SSID.Items.AddRange(array);
		cmbTxSSID.Items.AddRange(array);
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
		singleton = this;
	}

	public static void RefreshCommonLang()
	{
		_ = typeof(APRSForm).Name;
	}

	private void comboBoxIcons_DrawItemEvent(object sender, DrawItemEventArgs e)
	{
		if (e.Index < 0)
		{
			return;
		}
		ComboBox comboBox = sender as ComboBox;
		Color foreColor = e.ForeColor;
		if (e.State.HasFlag(DrawItemState.Selected) && !e.State.HasFlag(DrawItemState.ComboBoxEdit))
		{
			e.DrawBackground();
			e.DrawFocusRectangle();
		}
		else
		{
			using (SolidBrush brush = new SolidBrush(comboBox.BackColor))
			{
				Rectangle bounds = e.Bounds;
				bounds.Inflate(1, 1);
				e.Graphics.FillRectangle(brush, bounds);
			}
			foreColor = comboBox.ForeColor;
		}
		TextRenderer.DrawText(e.Graphics, comboBox.GetItemText(comboBox.Items[e.Index]), e.Font, new Point(e.Bounds.Height + 10, e.Bounds.Y), foreColor);
		e.Graphics.SmoothingMode = SmoothingMode.None;
		e.Graphics.DrawImage(iconsList.Images[cmbIconTable.SelectedIndex * 94 + e.Index], new Rectangle(new Point(e.Bounds.Location.X + 4, e.Bounds.Location.Y), new Size(e.Bounds.Height, e.Bounds.Height)));
	}

	private void APRSForm_Load(object sender, EventArgs e)
	{
		Settings.smethod_59(base.Controls);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
	}

	private void EmergencyForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveData();
	}

	private void txtName_Leave(object sender, EventArgs e)
	{
		txtName.Text = txtName.Text.Trim();
		if (Node.Text != txtName.Text)
		{
			if (Settings.smethod_50(Node, txtName.Text))
			{
				txtName.Text = Node.Text;
			}
			else
			{
				Node.Text = txtName.Text;
			}
		}
	}

	private void txtLatLong_Leave(object sender, EventArgs e)
	{
		SGTextBox obj = sender as SGTextBox;
		NumberFormatInfo numberFormat = CultureInfo.CurrentCulture.NumberFormat;
		uint value = ChannelForm.ChannelOne.LatLonStringToBin24(obj.Text.Replace(".", numberFormat.NumberDecimalSeparator));
		obj.Text = ChannelForm.ChannelOne.LatLonBin24ToString(value);
	}

	static APRSForm()
	{
		data = new APRS_Config();
		dataEx = new EmergencyEx();
	}

	private void cmbIconTable_SelectedIndexChanged(object sender, EventArgs e)
	{
		cmbIconIndex.Invalidate();
	}

	public static void copyConfigsDownwards(int sourceNum, int destNum)
	{
		for (int i = sourceNum; i < data.APRS_list.Length; i++)
		{
			data.APRS_list[destNum] = new APRS_One(data.APRS_list[i]);
			data.APRS_list[i].Name = "";
			ChannelForm.data.ChangeAPRS_Index(sourceNum, destNum);
			VfoForm.data.ChangeAPRS_Index(sourceNum, destNum);
			destNum++;
		}
	}

	public static void ValidateMagicAndCompact()
	{
		for (int i = 0; i < data.APRS_list.Length; i++)
		{
			if (data[i].Magic != 16722)
			{
				data.APRS_list[i].Name = "";
			}
		}
		Compact();
	}

	public static void Compact()
	{
		int num = -1;
		for (int i = 0; i < data.APRS_list.Length; i++)
		{
			if (num == -1 && !data.DataIsValid(i))
			{
				num = i;
			}
			else if (num != -1 && data.DataIsValid(i))
			{
				copyConfigsDownwards(i, num);
				num = -1;
			}
		}
	}
}
