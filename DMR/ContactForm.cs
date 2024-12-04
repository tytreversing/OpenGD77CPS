using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class ContactForm : DockContent, IDisp
{
	public enum CallType
	{
		GroupCall,
		PrivateCall,
		AllCall
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ContactOne : IVerify<ContactOne>
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		private byte[] name;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		private byte[] callId;

		private byte callType;

		private byte callRxTone;

		private byte ringStyle;

		private byte reserve1;

		public string Name
		{
			get
			{
				return Settings.smethod_25(name);
			}
			set
			{
				byte[] array = Settings.smethod_23(value);
				name.Fill(byte.MaxValue);
				Array.Copy(array, 0, name, 0, Math.Min(array.Length, name.Length));
			}
		}

		public string CallId
		{
			get
			{
				int num = 0;
				string value = BitConverter.ToString(callId).Replace("-", "");
				try
				{
					num = Convert.ToInt32(value);
					if ((num < 1 || num > 16776415) && num != 16777215)
					{
						return "1";
					}
					return num.ToString();
				}
				catch
				{
					return "";
				}
			}
			set
			{
				int num = 0;
				int num2 = Convert.ToInt32(value);
				if ((num2 >= 1 && num2 <= 16776415) || num2 == 16777215)
				{
					string text = value.PadLeft(8, '0');
					for (num = 0; num < 4; num++)
					{
						callId[num] = Convert.ToByte(text.Substring(num * 2, 2), 16);
					}
				}
			}
		}

		public int CallType
		{
			get
			{
				if (Enum.IsDefined(typeof(CallTypeE), callType))
				{
					return callType;
				}
				return 0;
			}
			set
			{
				if (Enum.IsDefined(typeof(CallTypeE), (byte)value))
				{
					callType = (byte)value;
				}
				else
				{
					callType = 0;
				}
			}
		}

		public string CallTypeS
		{
			get
			{
				if (callType < SZ_CALL_TYPE.Length)
				{
					return SZ_CALL_TYPE[callType];
				}
				return "";
			}
			set
			{
				int num = Array.IndexOf(SZ_CALL_TYPE, value);
				if (num < 0)
				{
					num = 0;
				}
				callType = (byte)num;
			}
		}

		public bool CallRxTone
		{
			get
			{
				return Convert.ToBoolean(callRxTone);
			}
			set
			{
				callRxTone = Convert.ToByte(value);
			}
		}

		public string CallRxToneS
		{
			get
			{
				if (callRxTone < SZ_CALL_RX_TONE.Length)
				{
					return SZ_CALL_RX_TONE[callRxTone];
				}
				return "";
			}
			set
			{
				int num = Array.IndexOf(SZ_CALL_RX_TONE, value);
				if (num < 0)
				{
					num = 0;
				}
				callRxTone = (byte)num;
			}
		}

		public int RepeaterSlot
		{
			get
			{
				if ((reserve1 & 1) == 1)
				{
					return 0;
				}
				return ((reserve1 & 2) >> 1) + 1;
			}
			set
			{
				if (value == 0)
				{
					reserve1 = 1;
				}
				else
				{
					reserve1 = (byte)(value - 1 << 1);
				}
			}
		}

		public string RepeaterSlotS
		{
			get
			{
				if (RepeaterSlot == 0)
				{
					return Settings.SZ_NONE;
				}
				return RepeaterSlot.ToString();
			}
			set
			{
				try
				{
					if (value == Settings.SZ_NONE)
					{
						RepeaterSlot = 0;
					}
					else
					{
						RepeaterSlot = Convert.ToByte(value);
					}
				}
				catch
				{
					RepeaterSlot = 0;
				}
			}
		}

		public int RingStyle
		{
			get
			{
				if (ringStyle >= 0 && ringStyle <= 10)
				{
					return ringStyle;
				}
				return 0;
			}
			set
			{
				if (value >= 0 && value <= 10)
				{
					ringStyle = (byte)value;
				}
			}
		}

		public string RingStyleS
		{
			get
			{
				if (ringStyle == 0)
				{
					return Settings.SZ_NONE;
				}
				return ringStyle.ToString();
			}
			set
			{
				try
				{
					if (value == Settings.SZ_NONE)
					{
						ringStyle = 0;
					}
					else
					{
						ringStyle = Convert.ToByte(value);
					}
				}
				catch
				{
					ringStyle = 0;
				}
			}
		}

		public ContactOne(int index)
		{
			this = default(ContactOne);
			name = new byte[16];
			callId = new byte[4];
			callType = byte.MaxValue;
			callRxTone = 0;
			ringStyle = 0;
			reserve1 = byte.MaxValue;
		}

		public ContactOne Clone()
		{
			return Settings.cloneObject(this);
		}

		public void Default()
		{
			CallRxTone = DefaultContact.CallRxTone;
			RingStyle = DefaultContact.RingStyle;
		}

		public bool DataIsValid()
		{
			if (!string.IsNullOrEmpty(Name))
			{
				return true;
			}
			return false;
		}

		public void Verify(ContactOne def)
		{
			if (!Enum.IsDefined(typeof(CallTypeE), callType))
			{
				callType = def.callType;
			}
			Settings.ValidateNumberRangeWithDefault(ref callRxTone, 0, 1, def.callRxTone);
			Settings.ValidateNumberRangeWithDefault(ref ringStyle, 0, 10, def.ringStyle);
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class Contact : IData
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
		private ContactOne[] contactList;

		public ContactOne[] ContactList => contactList;

		public ContactOne this[int index]
		{
			get
			{
				if (index >= 1024)
				{
					throw new ArgumentOutOfRangeException();
				}
				return contactList[index];
			}
			set
			{
				contactList[index] = value;
			}
		}

		public int Count => 1024;

		public int ValidCount
		{
			get
			{
				int num = 0;
				for (num = 0; num < Count && !string.IsNullOrEmpty(this[num].Name); num++)
				{
				}
				return num;
			}
		}

		public string Format => "Contact{0}";

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

		public Contact()
		{
			int num = 0;
			contactList = new ContactOne[1024];
			for (num = 0; num < contactList.Length; num++)
			{
				contactList[num] = new ContactOne(num);
				contactList[num].Name = "";
				contactList[num].CallId = "00000001";
			}
		}

		public bool HaveAll()
		{
			int num = 0;
			num = 0;
			while (true)
			{
				if (num < Count)
				{
					if (DataIsValid(num) && this[num].CallType == 2)
					{
						break;
					}
					num++;
					continue;
				}
				return false;
			}
			return true;
		}

		public int AllCallIndex()
		{
			return Array.FindIndex(contactList, (ContactOne contactOne_0) => contactOne_0.CallType == 2);
		}

		public int FindNextValidIndex(int index)
		{
			int num = -1;
			num = Array.FindIndex(contactList, index, (ContactOne contactOne_0) => contactOne_0.Name != "");
			if (num == -1)
			{
				while (--index >= 0)
				{
					if (DataIsValid(index))
					{
						num = index;
						break;
					}
				}
			}
			return num;
		}

		public bool DataIsValid(int index)
		{
			if (index < 1024 && index >= 0 && !string.IsNullOrEmpty(this[index].Name))
			{
				return true;
			}
			return false;
		}

		public void SetIndex(int index, int value)
		{
			contactList[index].CallType = value;
			if (value == 0)
			{
				SetName(index, "");
			}
		}

		public void ClearIndex(int index)
		{
			contactList[index].CallType = 255;
			SetName(index, "");
			RxGroupListForm.data.ClearByData(index);
			ChannelForm.data.ClearByContact(index);
			ButtonForm.data1.ClearByContact(index);
		}

		public int GetMinIndex()
		{
			int num = 0;
			num = 0;
			while (true)
			{
				if (num < 1024)
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

		public int GetRepeaterSlot(int index)
		{
			if (DataIsValid(index))
			{
				return contactList[index].RepeaterSlot;
			}
			return 0;
		}

		public string GetCallID(int index)
		{
			if (DataIsValid(index))
			{
				return contactList[index].CallId;
			}
			return string.Empty;
		}

		public void SetCallID(int index, string callID)
		{
			contactList[index].CallId = callID;
		}

		public bool CallIdExist(int index, string callId)
		{
			int num = 0;
			int callType = contactList[index].CallType;
			num = 0;
			while (true)
			{
				if (num < Count)
				{
					if (data.GetCallType(num) == callType && num != index && data.GetCallID(num) == callId)
					{
						break;
					}
					num++;
					continue;
				}
				return false;
			}
			return true;
		}

		public int GetCallIndexFromIdString(int callId)
		{
			int i = 0;
			string text = callId.ToString();
			for (; i < Count; i++)
			{
				if (data.GetCallID(i) == text)
				{
					return i + 1;
				}
			}
			return -1;
		}

		public bool CallIdExist(int index, int callType, string callId, int repeaterSlot)
		{
			int num = 0;
			num = 0;
			while (true)
			{
				if (num < Count)
				{
					if (data.GetCallType(num) == callType && num != index && data.GetCallID(num) == callId && data.GetRepeaterSlot(num) == repeaterSlot)
					{
						break;
					}
					num++;
					continue;
				}
				return false;
			}
			return true;
		}

		public string GetMinCallID()
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			string text = "";
			int validCount = ValidCount;
			num = 0;
			while (true)
			{
				if (num < validCount)
				{
					flag = false;
					text = $"{num + 1:d8}";
					for (num2 = 0; num2 < validCount; num2++)
					{
						if (data.GetCallID(num2) == text)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						break;
					}
					num++;
					continue;
				}
				return string.Empty;
			}
			return text;
		}

		public string GetMinCallID(int type)
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			string text = "";
			int validCount = ValidCount;
			num = 0;
			while (true)
			{
				if (num < validCount)
				{
					flag = false;
					text = $"{num + 1:d8}";
					for (num2 = 0; num2 < validCount; num2++)
					{
						if (data.GetCallType(num2) == type && data.GetCallID(num2) == text)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						break;
					}
					num++;
					continue;
				}
				return string.Empty;
			}
			return text;
		}

		public string GetMinCallID(int type, int index)
		{
			string text = $"{index + 1:d8}";
			bool flag = false;
			int validCount = ValidCount;
			for (int i = 0; i < validCount; i++)
			{
				if (contactList[i].CallType == type && contactList[i].CallId == text)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				return GetMinCallID(type);
			}
			return text;
		}

		public bool CallIdValid(string callId)
		{
			int num = Convert.ToInt32(callId);
			if (num >= 1 && num <= 16776415)
			{
				return true;
			}
			return false;
		}

		public int GetCallType(int index)
		{
			if (DataIsValid(index))
			{
				return contactList[index].CallType;
			}
			return 2;
		}

		public void SetCallType(int index, string callType)
		{
			contactList[index].CallTypeS = callType;
		}

		public void SetCallType(int index, int callType)
		{
			contactList[index].CallType = callType;
		}

		public bool IsGroupCall(int index)
		{
			if (index < 1024)
			{
				return contactList[index].CallType == 0;
			}
			return false;
		}

		public bool IsPrivateCall(int index)
		{
			if (index < 1024)
			{
				return contactList[index].CallType == 1;
			}
			return false;
		}

		public bool IsAllCall(int index)
		{
			if (index < 1024)
			{
				return contactList[index].CallType == 2;
			}
			return false;
		}

		public void SetCallRxTone(int index, string callRxTone)
		{
			contactList[index].CallRxToneS = callRxTone;
		}

		public void SetCallRxTone(int index, bool check)
		{
			contactList[index].CallRxTone = check;
		}

		public void SetRingStyle(int index, string ringStyle)
		{
			contactList[index].RingStyleS = ringStyle;
		}

		public void SetRingStyle(int index, int ringStyle)
		{
			contactList[index].RingStyle = ringStyle;
		}

		public void SetRepeaterSlot(int index, string repeaterSlot)
		{
			contactList[index].RepeaterSlotS = repeaterSlot;
		}

		public void SetRepeaterSlot(int index, int repeaterSlot)
		{
			contactList[index].RepeaterSlot = repeaterSlot;
		}

		public void SetName(int index, string text)
		{
			contactList[index].Name = text;
		}

		public string GetName(int index)
		{
			return contactList[index].Name;
		}

		public string GetMinName(TreeNode node)
		{
			int num = 0;
			int num2 = 0;
			string text = "";
			num2 = data.GetMinIndex();
			text = string.Format(Format, num2 + 1);
			if (!Settings.smethod_51(node, text))
			{
				return text;
			}
			num = 0;
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

		public bool NameExist(string name)
		{
			return contactList.Any((ContactOne x) => x.Name == name);
		}

		public int GetIndexForName(string name)
		{
			return Array.FindIndex(contactList, (ContactOne item) => item.Name == name);
		}

		public void Default(int index)
		{
			contactList[index].Default();
		}

		public void Paste(int from, int to)
		{
			contactList[to].CallRxTone = contactList[from].CallRxTone;
			contactList[to].RingStyle = contactList[from].RingStyle;
		}

		public void Verify()
		{
			int num = 0;
			for (num = 0; num < Count; num++)
			{
				if (DataIsValid(num))
				{
					contactList[num].Verify(DefaultContact);
				}
			}
		}

		[CompilerGenerated]
		private static bool smethod_0(ContactOne contactOne_0)
		{
			return contactOne_0.CallType == 2;
		}

		[CompilerGenerated]
		private static bool smethod_1(ContactOne contactOne_0)
		{
			return contactOne_0.Name != "";
		}
	}

	public const int CNT_CONTACT = 1024;

	public const int LEN_CONTACT_NAME = 16;

	public const int MIN_CALL_ID = 1;

	public const int MAX_CALL_ID = 16776415;

	public const int INC_CALL_ID = 1;

	public const int SCL_CALL_ID = 1;

	public const int LEN_CALL_ID = 8;

	public const int SPC_CALL_ID = 4;

	public const int MIN_RING_STYLE = 0;

	public const int MAX_RING_STYLE = 10;

	public const int MIN_CALL_RX_TONE = 0;

	public const int MAX_CALL_RX_TONE = 1;

	public const string SZ_PRIVATE_ID = "0123456789\b";

	public const string SZ_GROUP_ID = "0123456789*\b";

	public const string SZ_ALL_ID = "*******";

	public const int ALL_CODE = 16777215;

	public const string SZ_CALL_TYPE_NAME = "CallType";

	public const string SZ_CALL_RX_TONE_NAME = "CallRxTone";

	private CheckBox chkCallRxTone;

	private SGTextBox txtCallsign;

	private Label lblName;

	private Label lblCallId;

	private Label lblCallType;

	private CustomCombo cmbCallType;

	private SGTextBox txtCallId;

	private CustomCombo cmbRingStyle;

	private Label lblRingStyle;

	private CustomPanel pnlContact;

	public static readonly string[] SZ_CALL_TYPE;

	public static readonly string[] SZ_CALL_RX_TONE;

	public static ContactOne DefaultContact;

	private CustomCombo cmbRepeaterSlot;

	private Label lblRepeaterSlot;

	private GroupBox openGD77groupbox;

	private Button btnLookupIDForCallsign;

	public static Contact data;

	private string _PreCallId;

	public TreeNode Node { get; set; }

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.pnlContact = new CustomPanel();
		this.btnLookupIDForCallsign = new System.Windows.Forms.Button();
		this.openGD77groupbox = new System.Windows.Forms.GroupBox();
		this.cmbRepeaterSlot = new CustomCombo();
		this.lblRepeaterSlot = new System.Windows.Forms.Label();
		this.txtCallId = new DMR.SGTextBox();
		this.chkCallRxTone = new System.Windows.Forms.CheckBox();
		this.cmbRingStyle = new CustomCombo();
		this.cmbCallType = new CustomCombo();
		this.lblRingStyle = new System.Windows.Forms.Label();
		this.txtCallsign = new DMR.SGTextBox();
		this.lblCallType = new System.Windows.Forms.Label();
		this.lblName = new System.Windows.Forms.Label();
		this.lblCallId = new System.Windows.Forms.Label();
		this.pnlContact.SuspendLayout();
		this.openGD77groupbox.SuspendLayout();
		base.SuspendLayout();
		this.pnlContact.AutoScroll = true;
		this.pnlContact.AutoSize = true;
		this.pnlContact.Controls.Add(this.btnLookupIDForCallsign);
		this.pnlContact.Controls.Add(this.openGD77groupbox);
		this.pnlContact.Controls.Add(this.txtCallId);
		this.pnlContact.Controls.Add(this.chkCallRxTone);
		this.pnlContact.Controls.Add(this.cmbRingStyle);
		this.pnlContact.Controls.Add(this.cmbCallType);
		this.pnlContact.Controls.Add(this.lblRingStyle);
		this.pnlContact.Controls.Add(this.txtCallsign);
		this.pnlContact.Controls.Add(this.lblCallType);
		this.pnlContact.Controls.Add(this.lblName);
		this.pnlContact.Controls.Add(this.lblCallId);
		this.pnlContact.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnlContact.Location = new System.Drawing.Point(0, 0);
		this.pnlContact.Name = "pnlContact";
		this.pnlContact.Size = new System.Drawing.Size(476, 175);
		this.pnlContact.TabIndex = 7;
		this.btnLookupIDForCallsign.Location = new System.Drawing.Point(281, 15);
		this.btnLookupIDForCallsign.Name = "btnLookupIDForCallsign";
		this.btnLookupIDForCallsign.Size = new System.Drawing.Size(158, 23);
		this.btnLookupIDForCallsign.TabIndex = 8;
		this.btnLookupIDForCallsign.Text = "Lookup Id for Callsign";
		this.btnLookupIDForCallsign.UseVisualStyleBackColor = true;
		this.btnLookupIDForCallsign.Click += new System.EventHandler(btnLookupIDForCallsign_Click);
		this.openGD77groupbox.Controls.Add(this.cmbRepeaterSlot);
		this.openGD77groupbox.Controls.Add(this.lblRepeaterSlot);
		this.openGD77groupbox.Location = new System.Drawing.Point(11, 103);
		this.openGD77groupbox.Name = "openGD77groupbox";
		this.openGD77groupbox.Size = new System.Drawing.Size(283, 62);
		this.openGD77groupbox.TabIndex = 7;
		this.openGD77groupbox.TabStop = false;
		this.openGD77groupbox.Text = "OpenGD77 - Channel TS override";
		this.cmbRepeaterSlot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbRepeaterSlot.FormattingEnabled = true;
		this.cmbRepeaterSlot.Items.AddRange(new object[3] { "Disabled", "TS 1", "TS 2" });
		this.cmbRepeaterSlot.Location = new System.Drawing.Point(129, 32);
		this.cmbRepeaterSlot.Name = "cmbRepeaterSlot";
		this.cmbRepeaterSlot.Size = new System.Drawing.Size(120, 24);
		this.cmbRepeaterSlot.TabIndex = 5;
		this.cmbRepeaterSlot.SelectedIndexChanged += new System.EventHandler(cmbRepeaterSlot_SelectedIndexChanged);
		this.lblRepeaterSlot.Location = new System.Drawing.Point(12, 32);
		this.lblRepeaterSlot.Name = "lblRepeaterSlot";
		this.lblRepeaterSlot.Size = new System.Drawing.Size(111, 24);
		this.lblRepeaterSlot.TabIndex = 4;
		this.lblRepeaterSlot.Text = "Repeater Slot";
		this.lblRepeaterSlot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.txtCallId.InputString = null;
		this.txtCallId.Location = new System.Drawing.Point(137, 43);
		this.txtCallId.MaxByteLength = 0;
		this.txtCallId.Name = "txtCallId";
		this.txtCallId.Size = new System.Drawing.Size(120, 23);
		this.txtCallId.TabIndex = 3;
		this.txtCallId.Enter += new System.EventHandler(txtCallId_Enter);
		this.txtCallId.Leave += new System.EventHandler(txtCallId_Leave);
		this.txtCallId.Validating += new System.ComponentModel.CancelEventHandler(txtCallId_Validating);
		this.chkCallRxTone.AutoSize = true;
		this.chkCallRxTone.Location = new System.Drawing.Point(125, 282);
		this.chkCallRxTone.Name = "chkCallRxTone";
		this.chkCallRxTone.Size = new System.Drawing.Size(141, 20);
		this.chkCallRxTone.TabIndex = 6;
		this.chkCallRxTone.Text = "Call Receive Tone";
		this.chkCallRxTone.UseVisualStyleBackColor = true;
		this.chkCallRxTone.Visible = false;
		this.cmbRingStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbRingStyle.FormattingEnabled = true;
		this.cmbRingStyle.Location = new System.Drawing.Point(125, 249);
		this.cmbRingStyle.Name = "cmbRingStyle";
		this.cmbRingStyle.Size = new System.Drawing.Size(120, 24);
		this.cmbRingStyle.TabIndex = 5;
		this.cmbRingStyle.Visible = false;
		this.cmbRingStyle.SelectedIndexChanged += new System.EventHandler(cmbRingStyle_SelectedIndexChanged);
		this.cmbCallType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbCallType.FormattingEnabled = true;
		this.cmbCallType.Location = new System.Drawing.Point(137, 73);
		this.cmbCallType.Name = "cmbCallType";
		this.cmbCallType.Size = new System.Drawing.Size(120, 24);
		this.cmbCallType.TabIndex = 5;
		this.cmbCallType.SelectedIndexChanged += new System.EventHandler(cmbCallType_SelectedIndexChanged);
		this.lblRingStyle.Location = new System.Drawing.Point(8, 249);
		this.lblRingStyle.Name = "lblRingStyle";
		this.lblRingStyle.Size = new System.Drawing.Size(111, 24);
		this.lblRingStyle.TabIndex = 4;
		this.lblRingStyle.Text = "Ring Style";
		this.lblRingStyle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblRingStyle.Visible = false;
		this.txtCallsign.InputString = null;
		this.txtCallsign.Location = new System.Drawing.Point(137, 14);
		this.txtCallsign.MaxByteLength = 0;
		this.txtCallsign.Name = "txtCallsign";
		this.txtCallsign.Size = new System.Drawing.Size(120, 23);
		this.txtCallsign.TabIndex = 1;
		this.txtCallsign.Leave += new System.EventHandler(txtName_Leave);
		this.lblCallType.Enabled = false;
		this.lblCallType.Location = new System.Drawing.Point(23, 73);
		this.lblCallType.Name = "lblCallType";
		this.lblCallType.Size = new System.Drawing.Size(108, 24);
		this.lblCallType.TabIndex = 4;
		this.lblCallType.Text = "Call Type";
		this.lblCallType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblName.Location = new System.Drawing.Point(26, 14);
		this.lblName.Name = "lblName";
		this.lblName.Size = new System.Drawing.Size(105, 24);
		this.lblName.TabIndex = 0;
		this.lblName.Text = "Name";
		this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblCallId.Location = new System.Drawing.Point(26, 43);
		this.lblCallId.Name = "lblCallId";
		this.lblCallId.Size = new System.Drawing.Size(105, 24);
		this.lblCallId.TabIndex = 2;
		this.lblCallId.Text = "Call ID";
		this.lblCallId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		base.ClientSize = new System.Drawing.Size(476, 175);
		base.Controls.Add(this.pnlContact);
		this.Font = new System.Drawing.Font("Arial", 10f);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ContactForm";
		this.Text = "Digital Contact";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ContactForm_FormClosing);
		base.Load += new System.EventHandler(ContactForm_Load);
		base.Shown += new System.EventHandler(ContactForm_Shown);
		this.pnlContact.ResumeLayout(false);
		this.pnlContact.PerformLayout();
		this.openGD77groupbox.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	[CompilerGenerated]
	private string method_1()
	{
		return _PreCallId;
	}

	[CompilerGenerated]
	private void method_2(string value)
	{
		_PreCallId = value;
	}

	public void SaveData()
	{
		if (Settings.smethod_50(Node, txtCallsign.Text))
		{
			return;
		}
		try
		{
			int num = Convert.ToInt32(base.Tag);
			if (num != -1)
			{
				if (txtCallsign.Focused)
				{
					txtName_Leave(txtCallsign, null);
				}
				ContactOne value = new ContactOne(num);
				value.Name = txtCallsign.Text;
				value.CallId = txtCallId.Text;
				value.CallType = cmbCallType.method_3();
				value.CallRxTone = chkCallRxTone.Checked;
				value.RingStyle = cmbRingStyle.SelectedIndex;
				value.RepeaterSlot = cmbRepeaterSlot.SelectedIndex;
				data[num] = value;
				((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
			}
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
			int num = Convert.ToInt32(base.Tag);
			if (num == -1)
			{
				Close();
				return;
			}
			if (!data.DataIsValid(num))
			{
				num = data.FindNextValidIndex(num);
				Node = ((MainForm)base.MdiParent).GetTreeNodeByType(typeof(ContactsForm), num);
				base.Tag = num;
			}
			chkCallRxTone.CheckedChanged -= chkCallRxTone_CheckedChanged;
			cmbRingStyle.SelectedIndexChanged -= cmbRingStyle_SelectedIndexChanged;
			txtCallsign.Text = data[num].Name;
			txtCallId.Text = data[num].CallId;
			cmbCallType.method_2(data[num].CallType);
			chkCallRxTone.Checked = data[num].CallRxTone;
			cmbRingStyle.SelectedIndex = data[num].RingStyle;
			cmbRepeaterSlot.SelectedIndex = data[num].RepeaterSlot;
			chkCallRxTone.CheckedChanged += chkCallRxTone_CheckedChanged;
			cmbRingStyle.SelectedIndexChanged += cmbRingStyle_SelectedIndexChanged;
			method_3();
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
		lblRingStyle.Enabled &= flag;
		cmbRingStyle.Enabled &= flag;
		chkCallRxTone.Enabled &= flag;
	}

	public void RefreshName()
	{
		int index = Convert.ToInt32(base.Tag);
		txtCallsign.Text = data[index].Name;
	}

	public ContactForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	public void InitData()
	{
		int num = 0;
		txtCallsign.MaxLength = 16;
		txtCallsign.KeyPress += Settings.smethod_54;
		txtCallId.MaxLength = 8;
		num = 0;
		cmbCallType.Items.Clear();
		foreach (byte value in Enum.GetValues(typeof(CallTypeE)))
		{
			cmbCallType.method_1(SZ_CALL_TYPE[num++], value);
		}
		cmbRingStyle.Items.Clear();
		cmbRingStyle.Items.Add(Settings.SZ_NONE);
		for (num = 1; num <= 10; num++)
		{
			cmbRingStyle.Items.Add(num.ToString());
		}
	}

	private void ContactForm_Shown(object sender, EventArgs e)
	{
		Focus();
	}

	private void ContactForm_Load(object sender, EventArgs e)
	{
		try
		{
			Settings.smethod_59(base.Controls);
			Settings.UpdateComponentTextsFromLanguageXmlData(this);
			InitData();
			DispData();
			method_3();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	private void ContactForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveData();
	}

	public static void RefreshCommonLang()
	{
		string name = typeof(ContactForm).Name;
		Settings.smethod_78("CallType", SZ_CALL_TYPE, name);
		Settings.smethod_78("CallRxTone", SZ_CALL_RX_TONE, name);
	}

	private void method_3()
	{
		int num = cmbCallType.method_3();
		txtCallId.Enabled = true;
		switch (num)
		{
		case 2:
			txtCallId.Enabled = false;
			txtCallId.Text = 16777215.ToString();
			cmbRingStyle.Enabled = false;
			break;
		case 1:
			txtCallId.InputString = "0123456789\b";
			cmbRingStyle.Enabled = true;
			break;
		case 0:
			txtCallId.InputString = "0123456789*\b";
			cmbRingStyle.Enabled = false;
			break;
		}
	}

	public static bool IsValidId(string strId)
	{
		int num = Convert.ToInt32(strId.Replace('*', '0'));
		if (num != 0 && num % 1000000 != 0)
		{
			return true;
		}
		return false;
	}

	private void cmbCallType_SelectedIndexChanged(object sender, EventArgs e)
	{
		method_3();
	}

	private void cmbRepeaterSlot_SelectedIndexChanged(object sender, EventArgs e)
	{
	}

	private void txtName_Leave(object sender, EventArgs e)
	{
		txtCallsign.Text = txtCallsign.Text.Trim();
		if (Node.Text != txtCallsign.Text)
		{
			if (Settings.smethod_50(Node, txtCallsign.Text))
			{
				MessageBox.Show(Settings.dicCommon["ContactNameDuplicate"]);
				return;
			}
			Node.Text = txtCallsign.Text;
			SaveData();
		}
	}

	private void txtCallId_Validating(object sender, CancelEventArgs e)
	{
		int index = Convert.ToInt32(base.Tag);
		int selectedIndex = cmbCallType.SelectedIndex;
		string callId = method_1();
		int num = Convert.ToInt32(method_1());
		if (selectedIndex != 0 && selectedIndex != 1)
		{
			return;
		}
		if (string.IsNullOrEmpty(txtCallId.Text))
		{
			e.Cancel = true;
			MessageBox.Show(Settings.dicCommon["IdNotEmpty"]);
			txtCallId.Text = data.GetCallID(index);
			txtCallId.Focus();
			txtCallId.SelectAll();
			Activate();
			return;
		}
		int num2 = Convert.ToInt32(txtCallId.Text);
		if (num2 < 1 || num2 > 16776415)
		{
			e.Cancel = true;
			MessageBox.Show(Settings.dicCommon["IdOutOfRange"]);
			txtCallId.Focus();
			txtCallId.SelectAll();
			Activate();
		}
		string callId2 = txtCallId.Text;
		int selectedIndex2 = cmbRepeaterSlot.SelectedIndex;
		if (data.CallIdExist(index, selectedIndex, callId2, selectedIndex2))
		{
			e.Cancel = true;
			MessageBox.Show(Settings.dicCommon["IdAlreadyExists"]);
			txtCallId.Focus();
			txtCallId.SelectAll();
			Activate();
		}
		if (e.Cancel)
		{
			if (num >= 1 && num <= 16776415 && !data.CallIdExist(index, selectedIndex, callId, selectedIndex2))
			{
				txtCallId.Text = callId;
			}
			else
			{
				txtCallId.Text = data.GetMinCallID(selectedIndex);
			}
		}
		else
		{
			txtCallId.Text = callId2;
		}
		data.SetCallID(index, txtCallId.Text);
		((MainForm)base.MdiParent).RefreshRelatedForm(GetType(), index);
	}

	private void txtCallId_Leave(object sender, EventArgs e)
	{
		if (txtCallId.Text.Length < 8)
		{
			string text = txtCallId.Text;
			txtCallId.Text = text;
		}
	}

	private void txtCallId_Enter(object sender, EventArgs e)
	{
		method_2(txtCallId.Text);
	}

	private void chkCallRxTone_CheckedChanged(object sender, EventArgs e)
	{
		int index = Convert.ToInt32(base.Tag);
		data.SetCallRxTone(index, chkCallRxTone.Checked);
		((MainForm)base.MdiParent).RefreshRelatedForm(GetType(), index);
	}

	private void cmbRingStyle_SelectedIndexChanged(object sender, EventArgs e)
	{
		int index = Convert.ToInt32(base.Tag);
		data.SetRingStyle(index, cmbRingStyle.SelectedIndex);
		((MainForm)base.MdiParent).RefreshRelatedForm(GetType(), index);
	}

	static ContactForm()
	{
		SZ_CALL_TYPE = new string[3] { "Group Call", "Private Call", "All Call" };
		SZ_CALL_RX_TONE = new string[2] { "Off", "On" };
		data = new Contact();
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
				txtCallId.Text = radioIdResults.results[0].id;
				cmbCallType.SelectedIndex = 1;
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
