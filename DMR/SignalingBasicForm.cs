using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class SignalingBasicForm : DockContent, IDisp
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class SignalingBasic : IVerify<SignalingBasic>
	{
		private byte rmDuration;

		private byte txSyncWakeTot;

		private byte selCallHang;

		private byte autoResetTimer;

		private byte flag1;

		private byte flag2;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		private byte[] reserve;

		public decimal RmDuration
		{
			get
			{
				if (rmDuration < 1 || rmDuration > 12)
				{
					return 10m;
				}
				return rmDuration * 10;
			}
			set
			{
				byte b = Convert.ToByte(value / 10m);
				if (b >= 1 && b <= 12)
				{
					rmDuration = b;
				}
				else
				{
					rmDuration = 1;
				}
			}
		}

		public decimal TxSyncWakeTot
		{
			get
			{
				if (txSyncWakeTot < 5 || txSyncWakeTot > 15)
				{
					return 250m;
				}
				return txSyncWakeTot * 25;
			}
			set
			{
				byte b = Convert.ToByte(value / 25m);
				if (b >= 5 && b <= 15)
				{
					txSyncWakeTot = b;
				}
				else
				{
					txSyncWakeTot = 10;
				}
			}
		}

		public int SelCallHang
		{
			get
			{
				if (selCallHang >= 0 && selCallHang <= 14)
				{
					return selCallHang * 500;
				}
				return 4000;
			}
			set
			{
				value /= 500;
				if (value >= 0 && value <= 14)
				{
					selCallHang = Convert.ToByte(value);
				}
				else
				{
					selCallHang = 8;
				}
			}
		}

		public byte AutoResetTimer
		{
			get
			{
				if (autoResetTimer >= 1 && autoResetTimer <= byte.MaxValue)
				{
					return autoResetTimer;
				}
				return 10;
			}
			set
			{
				if (value >= 1 && value <= byte.MaxValue)
				{
					autoResetTimer = value;
				}
				else
				{
					autoResetTimer = 10;
				}
			}
		}

		public bool RadioDisable
		{
			get
			{
				return Convert.ToBoolean(flag1 & 0x80);
			}
			set
			{
				if (value)
				{
					flag1 |= 128;
				}
				else
				{
					flag1 &= 127;
				}
			}
		}

		public bool RemoteMonitor
		{
			get
			{
				return Convert.ToBoolean(flag1 & 0x40);
			}
			set
			{
				if (value)
				{
					flag1 |= 64;
				}
				else
				{
					flag1 &= 191;
				}
			}
		}

		public bool EmgRm
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

		public decimal TxWakeMsgLimit
		{
			get
			{
				byte b = Convert.ToByte((flag1 & 0x1C) >> 2);
				if (b < 0 || b > 4)
				{
					return 2m;
				}
				return b;
			}
			set
			{
				if (!(value >= 0m) || !(value <= 4m))
				{
					value = 2m;
				}
				value = ((int)value << 2) & 0x1C;
				flag1 &= 227;
				flag1 |= (byte)value;
			}
		}

		public bool CallAlert
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

		public bool SelCallCode
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

		public int SelCallToneId
		{
			get
			{
				return (flag2 & 0x20) >> 5;
			}
			set
			{
				flag2 &= 223;
				flag2 |= Convert.ToByte((value << 5) & 0x20);
			}
		}

		public void Verify(SignalingBasic def)
		{
			Settings.ValidateNumberRangeWithDefault(ref rmDuration, 1, 12, def.rmDuration);
			Settings.ValidateNumberRangeWithDefault(ref txSyncWakeTot, 5, 15, def.txSyncWakeTot);
			Settings.ValidateNumberRangeWithDefault(ref selCallHang, 0, 14, def.selCallHang);
			Settings.ValidateNumberRangeWithDefault(ref autoResetTimer, 1, byte.MaxValue, def.autoResetTimer);
			byte b = Convert.ToByte((flag1 & 0x1C) >> 2);
			if (b < 0 || b > 4)
			{
				flag1 &= 227;
				flag1 |= (byte)(def.flag1 & 0x1C);
			}
		}
	}

	private const byte MIN_RM_DURATION = 1;

	private const byte MAX_RM_DURATION = 12;

	private const byte SCL_RM_DURATION = 10;

	private const byte INC_RM_DURATION = 1;

	private const byte LEN_RM_DURATION = 3;

	private const string SZ_RM_DURATION = "0123456789";

	private const byte MIN_TX_SYNC_WAKE_TOT = 5;

	private const byte MAX_TX_SYNC_WAKE_TOT = 15;

	private const byte INC_TX_SYNC_WAKE_TOT = 1;

	private const byte SCL_TX_SYNC_WAKE_TOT = 25;

	private const byte LEN_TX_SYNC_WAKE_TOT = 3;

	private const string SZ_TX_SYNC_WAKE_TOT = "0123456789";

	private const byte MIN_SEL_CALL_HANG = 0;

	private const byte MAX_SEL_CALL_HANG = 14;

	private const ushort SCL_SEL_CALL_HANG = 500;

	private const byte INC_SEL_CALL_HANG = 1;

	private const byte LEN_SEL_CALL_HANG = 4;

	private const byte MIN_AUTO_RESET_TIMER = 1;

	private const byte MAX_AUTO_RESET_TIMER = byte.MaxValue;

	private const byte INC_AUTO_RESET_TIMER = 1;

	private const byte SCL_AUTO_RESET_TIMER = 1;

	private const byte LEN_AUTO_RESET_TIMER = 3;

	private const byte MIN_TX_WAKE_MSG_LIMIT = 0;

	private const byte MAX_TX_WAKE_MSG_LIMIT = 4;

	private const byte INC_TX_WAKE_MSG_LIMIT = 1;

	private const byte SCL_TX_WAKE_MSG_LIMIT = 1;

	private const byte LEN_TX_WAKE_MSG_LIMIT = 1;

	public static SignalingBasic DefaultSignalingBasic;

	public static SignalingBasic data;

	private CheckBox chkCallAlert;

	private CheckBox chkSelCall;

	private ComboBox cmbSelCallToneId;

	private Label lblSelCallToneId;

	private Label lblSelCallHang;

	private Label lblAutoResetTimer;

	private CheckBox chkRadioDisable;

	private CheckBox chkRemoteMonitor;

	private CheckBox chkEmgRM;

	private Label lblRMDuration;

	private Label lblTxSyncWakeTot;

	private Label lblTxWakeMsgLimit;

	private CustomNumericUpDown nudSelCallHang;

	private CustomNumericUpDown nudAutoResetTimer;

	private CustomNumericUpDown nudRMDuration;

	private CustomNumericUpDown nudTxSyncWakeTot;

	private CustomNumericUpDown nudTxWakeMsgLimit;

	private CustomPanel pnlSignalingBasic;

	public TreeNode Node { get; set; }

	public void SaveData()
	{
		try
		{
			data.RmDuration = nudRMDuration.Value;
			data.TxSyncWakeTot = nudTxSyncWakeTot.Value;
			data.RadioDisable = chkRadioDisable.Checked;
			data.RemoteMonitor = chkRemoteMonitor.Checked;
			data.EmgRm = chkEmgRM.Checked;
			data.TxWakeMsgLimit = nudTxWakeMsgLimit.Value;
			data.CallAlert = chkCallAlert.Checked;
			data.SelCallCode = chkSelCall.Checked;
			data.SelCallToneId = cmbSelCallToneId.SelectedIndex;
			data.SelCallHang = (int)nudSelCallHang.Value;
			data.AutoResetTimer = (byte)nudAutoResetTimer.Value;
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
			method_0();
			chkRadioDisable.Checked = data.RadioDisable;
			chkRemoteMonitor.Checked = data.RemoteMonitor;
			chkEmgRM.Checked = data.EmgRm;
			nudRMDuration.Value = data.RmDuration;
			nudTxWakeMsgLimit.Value = data.TxWakeMsgLimit;
			nudTxSyncWakeTot.Value = data.TxSyncWakeTot;
			chkCallAlert.Checked = data.CallAlert;
			chkSelCall.Checked = data.SelCallCode;
			cmbSelCallToneId.SelectedIndex = data.SelCallToneId;
			nudSelCallHang.Value = data.SelCallHang;
			nudAutoResetTimer.Value = data.AutoResetTimer;
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
		chkRadioDisable.Enabled &= flag;
		chkRemoteMonitor.Enabled &= flag;
		chkEmgRM.Enabled &= flag;
		lblTxWakeMsgLimit.Enabled &= flag;
		nudTxWakeMsgLimit.Enabled &= flag;
		lblRMDuration.Enabled &= flag;
		nudRMDuration.Enabled &= flag;
		lblTxSyncWakeTot.Enabled &= flag;
		nudTxSyncWakeTot.Enabled &= flag;
	}

	public void RefreshName()
	{
	}

	public SignalingBasicForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	private void method_0()
	{
		nudRMDuration.Minimum = 10m;
		nudRMDuration.Maximum = 120m;
		nudRMDuration.Increment = 10m;
		nudRMDuration.method_0(3);
		nudTxSyncWakeTot.Minimum = 125m;
		nudTxSyncWakeTot.Maximum = 375m;
		nudTxSyncWakeTot.Increment = 25m;
		nudTxSyncWakeTot.method_0(3);
		Settings.smethod_36(nudTxWakeMsgLimit, new Class13(0, 4, 1, 1m, 1));
		Settings.smethod_36(nudSelCallHang, new Class13(0, 14, 1, 500m, 4));
		Settings.smethod_36(nudAutoResetTimer, new Class13(1, 255, 1, 1m, 3));
	}

	private void SignalingBasicForm_Load(object sender, EventArgs e)
	{
		Settings.smethod_59(base.Controls);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		DispData();
	}

	private void SignalingBasicForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveData();
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.pnlSignalingBasic = new CustomPanel();
		this.nudTxSyncWakeTot = new CustomNumericUpDown();
		this.chkCallAlert = new System.Windows.Forms.CheckBox();
		this.nudAutoResetTimer = new CustomNumericUpDown();
		this.chkRadioDisable = new System.Windows.Forms.CheckBox();
		this.nudTxWakeMsgLimit = new CustomNumericUpDown();
		this.chkRemoteMonitor = new System.Windows.Forms.CheckBox();
		this.nudRMDuration = new CustomNumericUpDown();
		this.chkEmgRM = new System.Windows.Forms.CheckBox();
		this.lblTxSyncWakeTot = new System.Windows.Forms.Label();
		this.chkSelCall = new System.Windows.Forms.CheckBox();
		this.lblTxWakeMsgLimit = new System.Windows.Forms.Label();
		this.cmbSelCallToneId = new System.Windows.Forms.ComboBox();
		this.nudSelCallHang = new CustomNumericUpDown();
		this.lblSelCallToneId = new System.Windows.Forms.Label();
		this.lblRMDuration = new System.Windows.Forms.Label();
		this.lblSelCallHang = new System.Windows.Forms.Label();
		this.lblAutoResetTimer = new System.Windows.Forms.Label();
		this.pnlSignalingBasic.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nudTxSyncWakeTot).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudAutoResetTimer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudTxWakeMsgLimit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudRMDuration).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudSelCallHang).BeginInit();
		base.SuspendLayout();
		this.pnlSignalingBasic.AutoScroll = true;
		this.pnlSignalingBasic.AutoSize = true;
		this.pnlSignalingBasic.Controls.Add(this.nudTxSyncWakeTot);
		this.pnlSignalingBasic.Controls.Add(this.chkCallAlert);
		this.pnlSignalingBasic.Controls.Add(this.nudAutoResetTimer);
		this.pnlSignalingBasic.Controls.Add(this.chkRadioDisable);
		this.pnlSignalingBasic.Controls.Add(this.nudTxWakeMsgLimit);
		this.pnlSignalingBasic.Controls.Add(this.chkRemoteMonitor);
		this.pnlSignalingBasic.Controls.Add(this.nudRMDuration);
		this.pnlSignalingBasic.Controls.Add(this.chkEmgRM);
		this.pnlSignalingBasic.Controls.Add(this.lblTxSyncWakeTot);
		this.pnlSignalingBasic.Controls.Add(this.chkSelCall);
		this.pnlSignalingBasic.Controls.Add(this.lblTxWakeMsgLimit);
		this.pnlSignalingBasic.Controls.Add(this.cmbSelCallToneId);
		this.pnlSignalingBasic.Controls.Add(this.nudSelCallHang);
		this.pnlSignalingBasic.Controls.Add(this.lblSelCallToneId);
		this.pnlSignalingBasic.Controls.Add(this.lblRMDuration);
		this.pnlSignalingBasic.Controls.Add(this.lblSelCallHang);
		this.pnlSignalingBasic.Controls.Add(this.lblAutoResetTimer);
		this.pnlSignalingBasic.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnlSignalingBasic.Location = new System.Drawing.Point(0, 0);
		this.pnlSignalingBasic.Name = "pnlSignalingBasic";
		this.pnlSignalingBasic.Size = new System.Drawing.Size(381, 224);
		this.pnlSignalingBasic.TabIndex = 0;
		this.nudTxSyncWakeTot.Increment = new decimal(new int[4] { 5, 0, 0, 0 });
		this.nudTxSyncWakeTot.Location = new System.Drawing.Point(245, 158);
		this.nudTxSyncWakeTot.Maximum = new decimal(new int[4] { 375, 0, 0, 0 });
		this.nudTxSyncWakeTot.Minimum = new decimal(new int[4] { 125, 0, 0, 0 });
		this.nudTxSyncWakeTot.Name = "nudTxSyncWakeTot";
		this.nudTxSyncWakeTot.Size = new System.Drawing.Size(120, 23);
		this.nudTxSyncWakeTot.TabIndex = 8;
		this.nudTxSyncWakeTot.Value = new decimal(new int[4] { 125, 0, 0, 0 });
		this.chkCallAlert.AutoSize = true;
		this.chkCallAlert.Location = new System.Drawing.Point(206, 188);
		this.chkCallAlert.Name = "chkCallAlert";
		this.chkCallAlert.Size = new System.Drawing.Size(135, 20);
		this.chkCallAlert.TabIndex = 9;
		this.chkCallAlert.Text = "Call Alert Encode";
		this.chkCallAlert.UseVisualStyleBackColor = true;
		this.chkCallAlert.Visible = false;
		this.nudAutoResetTimer.Location = new System.Drawing.Point(214, 192);
		this.nudAutoResetTimer.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudAutoResetTimer.Minimum = new decimal(new int[4] { 1, 0, 0, 0 });
		this.nudAutoResetTimer.Name = "nudAutoResetTimer";
		this.nudAutoResetTimer.Size = new System.Drawing.Size(120, 23);
		this.nudAutoResetTimer.TabIndex = 16;
		this.nudAutoResetTimer.Value = new decimal(new int[4] { 1, 0, 0, 0 });
		this.nudAutoResetTimer.Visible = false;
		this.chkRadioDisable.AutoSize = true;
		this.chkRadioDisable.Location = new System.Drawing.Point(52, 12);
		this.chkRadioDisable.Name = "chkRadioDisable";
		this.chkRadioDisable.Size = new System.Drawing.Size(168, 20);
		this.chkRadioDisable.TabIndex = 0;
		this.chkRadioDisable.Text = "Radio Disable Decode";
		this.chkRadioDisable.UseVisualStyleBackColor = true;
		this.nudTxWakeMsgLimit.Location = new System.Drawing.Point(245, 98);
		this.nudTxWakeMsgLimit.Maximum = new decimal(new int[4] { 4, 0, 0, 0 });
		this.nudTxWakeMsgLimit.Minimum = new decimal(new int[4] { 1, 0, 0, 0 });
		this.nudTxWakeMsgLimit.Name = "nudTxWakeMsgLimit";
		this.nudTxWakeMsgLimit.Size = new System.Drawing.Size(120, 23);
		this.nudTxWakeMsgLimit.TabIndex = 4;
		this.nudTxWakeMsgLimit.Value = new decimal(new int[4] { 4, 0, 0, 0 });
		this.chkRemoteMonitor.AutoSize = true;
		this.chkRemoteMonitor.Location = new System.Drawing.Point(52, 36);
		this.chkRemoteMonitor.Name = "chkRemoteMonitor";
		this.chkRemoteMonitor.Size = new System.Drawing.Size(180, 20);
		this.chkRemoteMonitor.TabIndex = 1;
		this.chkRemoteMonitor.Text = "Remote Monitor Decode";
		this.chkRemoteMonitor.UseVisualStyleBackColor = true;
		this.nudRMDuration.Increment = new decimal(new int[4] { 10, 0, 0, 0 });
		this.nudRMDuration.Location = new System.Drawing.Point(245, 128);
		this.nudRMDuration.Maximum = new decimal(new int[4] { 120, 0, 0, 0 });
		this.nudRMDuration.Minimum = new decimal(new int[4] { 10, 0, 0, 0 });
		this.nudRMDuration.Name = "nudRMDuration";
		this.nudRMDuration.Size = new System.Drawing.Size(120, 23);
		this.nudRMDuration.TabIndex = 6;
		this.nudRMDuration.Value = new decimal(new int[4] { 10, 0, 0, 0 });
		this.chkEmgRM.AutoSize = true;
		this.chkEmgRM.Location = new System.Drawing.Point(52, 62);
		this.chkEmgRM.Name = "chkEmgRM";
		this.chkEmgRM.Size = new System.Drawing.Size(255, 20);
		this.chkEmgRM.TabIndex = 2;
		this.chkEmgRM.Text = "Emergency Romote Monitor Decode";
		this.chkEmgRM.UseVisualStyleBackColor = true;
		this.lblTxSyncWakeTot.Location = new System.Drawing.Point(49, 158);
		this.lblTxSyncWakeTot.Name = "lblTxSyncWakeTot";
		this.lblTxSyncWakeTot.Size = new System.Drawing.Size(185, 24);
		this.lblTxSyncWakeTot.TabIndex = 7;
		this.lblTxSyncWakeTot.Text = "Tx Sync Wakeup TOT [ms]";
		this.lblTxSyncWakeTot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.chkSelCall.AutoSize = true;
		this.chkSelCall.Location = new System.Drawing.Point(208, 188);
		this.chkSelCall.Name = "chkSelCall";
		this.chkSelCall.Size = new System.Drawing.Size(131, 20);
		this.chkSelCall.TabIndex = 10;
		this.chkSelCall.Text = "Self Call Encode";
		this.chkSelCall.UseVisualStyleBackColor = true;
		this.chkSelCall.Visible = false;
		this.lblTxWakeMsgLimit.Location = new System.Drawing.Point(49, 98);
		this.lblTxWakeMsgLimit.Name = "lblTxWakeMsgLimit";
		this.lblTxWakeMsgLimit.Size = new System.Drawing.Size(185, 24);
		this.lblTxWakeMsgLimit.TabIndex = 3;
		this.lblTxWakeMsgLimit.Text = "Tx Wakeup Message Limit";
		this.lblTxWakeMsgLimit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.cmbSelCallToneId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbSelCallToneId.FormattingEnabled = true;
		this.cmbSelCallToneId.Items.AddRange(new object[2] { "前置", "始终" });
		this.cmbSelCallToneId.Location = new System.Drawing.Point(209, 188);
		this.cmbSelCallToneId.Name = "cmbSelCallToneId";
		this.cmbSelCallToneId.Size = new System.Drawing.Size(121, 24);
		this.cmbSelCallToneId.TabIndex = 12;
		this.cmbSelCallToneId.Visible = false;
		this.nudSelCallHang.Increment = new decimal(new int[4] { 500, 0, 0, 0 });
		this.nudSelCallHang.Location = new System.Drawing.Point(219, 190);
		this.nudSelCallHang.Maximum = new decimal(new int[4] { 7000, 0, 0, 0 });
		this.nudSelCallHang.Name = "nudSelCallHang";
		this.nudSelCallHang.Size = new System.Drawing.Size(120, 23);
		this.nudSelCallHang.TabIndex = 14;
		this.nudSelCallHang.Visible = false;
		this.lblSelCallToneId.Location = new System.Drawing.Point(13, 188);
		this.lblSelCallToneId.Name = "lblSelCallToneId";
		this.lblSelCallToneId.Size = new System.Drawing.Size(185, 24);
		this.lblSelCallToneId.TabIndex = 11;
		this.lblSelCallToneId.Text = "Self Call Tone/ID";
		this.lblSelCallToneId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblSelCallToneId.Visible = false;
		this.lblRMDuration.Location = new System.Drawing.Point(49, 128);
		this.lblRMDuration.Name = "lblRMDuration";
		this.lblRMDuration.Size = new System.Drawing.Size(185, 24);
		this.lblRMDuration.TabIndex = 5;
		this.lblRMDuration.Text = "Remote Monitor Duration [s]";
		this.lblRMDuration.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblSelCallHang.Location = new System.Drawing.Point(23, 190);
		this.lblSelCallHang.Name = "lblSelCallHang";
		this.lblSelCallHang.Size = new System.Drawing.Size(185, 24);
		this.lblSelCallHang.TabIndex = 13;
		this.lblSelCallHang.Text = "Self Call Hang Time [ms]";
		this.lblSelCallHang.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblSelCallHang.Visible = false;
		this.lblAutoResetTimer.Location = new System.Drawing.Point(18, 192);
		this.lblAutoResetTimer.Name = "lblAutoResetTimer";
		this.lblAutoResetTimer.Size = new System.Drawing.Size(185, 24);
		this.lblAutoResetTimer.TabIndex = 15;
		this.lblAutoResetTimer.Text = "Auto Reset Timer [s]";
		this.lblAutoResetTimer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblAutoResetTimer.Visible = false;
		base.ClientSize = new System.Drawing.Size(381, 224);
		base.Controls.Add(this.pnlSignalingBasic);
		this.Font = new System.Drawing.Font("Arial", 10f);
		base.Name = "SignalingBasicForm";
		this.Text = "Signaling System";
		this.pnlSignalingBasic.ResumeLayout(false);
		this.pnlSignalingBasic.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.nudTxSyncWakeTot).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudAutoResetTimer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudTxWakeMsgLimit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudRMDuration).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudSelCallHang).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	static SignalingBasicForm()
	{
		DefaultSignalingBasic = new SignalingBasic();
		data = new SignalingBasic();
	}
}
