using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class ScanBasicForm : DockContent, IDisp
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class ScanBasic
	{
		private byte digitHang;

		private byte analogHang;

		private byte voteHang;

		private byte fastVoteRssi;

		private byte startVoteRssi;

		private byte flag1;

		private byte scanTime;

		private byte reserve;

		public decimal DigitHang
		{
			get
			{
				if (digitHang < 1 || digitHang > 20)
				{
					return 1000m;
				}
				return digitHang * 500;
			}
			set
			{
				digitHang = Convert.ToByte(value / 500m);
			}
		}

		public decimal AnalogHang
		{
			get
			{
				if (analogHang < 0 || analogHang > 20)
				{
					return 1000m;
				}
				return analogHang * 500;
			}
			set
			{
				analogHang = Convert.ToByte(value / 500m);
			}
		}

		public decimal VoteHang
		{
			get
			{
				if (voteHang < 0 || voteHang > byte.MaxValue)
				{
					return 3.00m;
				}
				return (decimal)voteHang * 0.25m;
			}
			set
			{
				voteHang = Convert.ToByte(value / 0.25m);
			}
		}

		public decimal FastVoteRssi
		{
			get
			{
				if (fastVoteRssi < 70 || fastVoteRssi > 120)
				{
					return -70m;
				}
				return fastVoteRssi * -1;
			}
			set
			{
				fastVoteRssi = Convert.ToByte(value * -1m);
			}
		}

		public decimal StartVoteRssi
		{
			get
			{
				if (startVoteRssi < 70 || startVoteRssi > 120)
				{
					return -100m;
				}
				return startVoteRssi * -1;
			}
			set
			{
				startVoteRssi = Convert.ToByte(value * -1m);
			}
		}

		public bool PriorityAlert
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

		public decimal ScanTime
		{
			get
			{
				if (scanTime < 1 || scanTime > 13)
				{
					return 5m;
				}
				return scanTime * 5;
			}
			set
			{
				scanTime = Convert.ToByte(value / 5m);
			}
		}

		public ScanBasic()
		{
			reserve = byte.MaxValue;
		}

		public void Verify(ScanBasic def)
		{
			Settings.ValidateNumberRangeWithDefault(ref digitHang, 1, 20, def.digitHang);
			Settings.ValidateNumberRangeWithDefault(ref analogHang, 0, 20, def.analogHang);
		}
	}

	private const byte INC_DIGIT_HANG = 1;

	private const byte MIN_DIGIT_HANG = 1;

	private const byte MAX_DIGIT_HANG = 20;

	private const ushort SCL_DIGIT_HANG = 500;

	private const byte LEN_DIGIT_HANG = 4;

	private const string SZ_DIGIT_HANG = "0123456789\b";

	private const byte INC_ANALOG_HANG = 1;

	private const byte MIN_ANALOG_HANG = 0;

	private const byte MAX_ANALOG_HANG = 20;

	private const ushort SCL_ANALOG_HANG = 500;

	private const byte LEN_ANALOG_HANG = 4;

	private const string SZ_ANALOG_HANG = "0123456789\b";

	private const byte INC_VOTE_HANG = 1;

	private const byte MIN_VOTE_HANG = 0;

	private const byte MAX_VOTE_HANG = byte.MaxValue;

	public static decimal SCL_VOTE_HANG;

	private const byte LEN_VOTE_HANG = 5;

	private const byte INC_FAST_VOTE_RSSI = 1;

	private const byte MIN_FAST_VOTE_RSSI = 70;

	private const byte MAX_FAST_VOTE_RSSI = 120;

	private const sbyte SCL_FAST_VOTE_RSSI = -1;

	private const byte LEN_FAST_VOTE_RSSI = 4;

	private const byte INC_START_VOTE_RSSI = 1;

	private const byte MIN_START_VOTE_RSSI = 70;

	private const byte MAX_START_VOTE_RSSI = 120;

	private const sbyte SCL_START_VOTE_RSSI = -1;

	private const byte LEN_START_VOTE_RSSI = 4;

	private const byte INC_SCAN_TIME = 1;

	private const byte MIN_SCAN_TIME = 1;

	private const byte MAX_SCAN_TIME = 12;

	private const ushort SCL_SCAN_TIME = 5;

	private const byte LEN_SCAN_TIME = 2;

	private const string SZ_SCAN_TIME = "0123456789\b";

	public static ScanBasic DefaultScanBasic;

	public static ScanBasic data;

	private Label lblDigitHang;

	private Label lblAnalogHang;

	private Label lblVoteHang;

	private Label lblFastVoteRssi;

	private Label lblStartVoteRssi;

	private Label lblScanTime;

	private CheckBox chkPriorityAlert;

	private CustomNumericUpDown nudDigitHang;

	private CustomNumericUpDown nudAnalogHang;

	private CustomNumericUpDown nudScanTime;

	private CustomNumericUpDown nudVoteHang;

	private CustomNumericUpDown nudFastVoteRssi;

	private CustomNumericUpDown nudStartVoteRssi;

	private CustomPanel pnlScanBasic;

	public TreeNode Node { get; set; }

	public ScanBasicForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	public void SaveData()
	{
		try
		{
			data.DigitHang = nudDigitHang.Value;
			data.AnalogHang = nudAnalogHang.Value;
			data.ScanTime = nudScanTime.Value;
			data.VoteHang = nudVoteHang.Value;
			data.FastVoteRssi = nudFastVoteRssi.Value;
			data.StartVoteRssi = nudStartVoteRssi.Value;
			data.PriorityAlert = chkPriorityAlert.Checked;
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
			nudDigitHang.Value = data.DigitHang;
			nudAnalogHang.Value = data.AnalogHang;
			nudScanTime.Value = data.ScanTime;
			nudVoteHang.Value = data.VoteHang;
			nudFastVoteRssi.Value = data.FastVoteRssi;
			nudStartVoteRssi.Value = data.StartVoteRssi;
			chkPriorityAlert.Checked = data.PriorityAlert;
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
		lblDigitHang.Enabled &= flag;
		nudDigitHang.Enabled &= flag;
		lblAnalogHang.Enabled &= flag;
		nudAnalogHang.Enabled &= flag;
		nudScanTime.Enabled &= flag;
		chkPriorityAlert.Enabled &= flag;
	}

	public void RefreshName()
	{
	}

	private void method_0()
	{
		nudDigitHang.Increment = 500m;
		nudDigitHang.Minimum = 500m;
		nudDigitHang.Maximum = 10000m;
		nudDigitHang.method_0(4);
		nudDigitHang.method_2("0123456789\b");
		nudAnalogHang.Increment = 500m;
		nudAnalogHang.Minimum = 0m;
		nudAnalogHang.Maximum = 10000m;
		nudAnalogHang.method_0(4);
		nudAnalogHang.method_2("0123456789\b");
		nudScanTime.Increment = 5m;
		nudScanTime.Minimum = 5m;
		nudScanTime.Maximum = 60m;
		nudScanTime.method_0(2);
		nudScanTime.method_2("0123456789\b");
		nudVoteHang.Increment = 0.25m;
		nudVoteHang.Minimum = 0.00m;
		nudVoteHang.Maximum = 63.75m;
		nudVoteHang.method_0(5);
		nudVoteHang.method_2("0123456789.\b");
		nudFastVoteRssi.Increment = 1m;
		nudFastVoteRssi.Minimum = -120m;
		nudFastVoteRssi.Maximum = -70m;
		nudFastVoteRssi.method_0(4);
		nudFastVoteRssi.method_2("-0123456789\b");
		nudStartVoteRssi.Increment = 1m;
		nudStartVoteRssi.Minimum = -120m;
		nudStartVoteRssi.Maximum = -70m;
		nudStartVoteRssi.method_0(4);
		nudStartVoteRssi.method_2("-0123456789\b");
	}

	private void ScanBasicForm_Load(object sender, EventArgs e)
	{
		Settings.smethod_59(base.Controls);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		method_0();
		DispData();
	}

	private void ScanBasicForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveData();
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.pnlScanBasic = new CustomPanel();
		this.nudAnalogHang = new CustomNumericUpDown();
		this.nudScanTime = new CustomNumericUpDown();
		this.lblScanTime = new System.Windows.Forms.Label();
		this.nudStartVoteRssi = new CustomNumericUpDown();
		this.lblDigitHang = new System.Windows.Forms.Label();
		this.nudFastVoteRssi = new CustomNumericUpDown();
		this.lblAnalogHang = new System.Windows.Forms.Label();
		this.nudVoteHang = new CustomNumericUpDown();
		this.lblVoteHang = new System.Windows.Forms.Label();
		this.lblFastVoteRssi = new System.Windows.Forms.Label();
		this.nudDigitHang = new CustomNumericUpDown();
		this.lblStartVoteRssi = new System.Windows.Forms.Label();
		this.chkPriorityAlert = new System.Windows.Forms.CheckBox();
		this.pnlScanBasic.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogHang).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudStartVoteRssi).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudFastVoteRssi).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudVoteHang).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudDigitHang).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudScanTime).BeginInit();
		base.SuspendLayout();
		this.pnlScanBasic.AutoScroll = true;
		this.pnlScanBasic.AutoSize = true;
		this.pnlScanBasic.Controls.Add(this.nudAnalogHang);
		this.pnlScanBasic.Controls.Add(this.nudScanTime);
		this.pnlScanBasic.Controls.Add(this.nudStartVoteRssi);
		this.pnlScanBasic.Controls.Add(this.lblDigitHang);
		this.pnlScanBasic.Controls.Add(this.nudFastVoteRssi);
		this.pnlScanBasic.Controls.Add(this.lblAnalogHang);
		this.pnlScanBasic.Controls.Add(this.lblScanTime);
		this.pnlScanBasic.Controls.Add(this.nudVoteHang);
		this.pnlScanBasic.Controls.Add(this.lblVoteHang);
		this.pnlScanBasic.Controls.Add(this.lblFastVoteRssi);
		this.pnlScanBasic.Controls.Add(this.nudDigitHang);
		this.pnlScanBasic.Controls.Add(this.lblStartVoteRssi);
		this.pnlScanBasic.Controls.Add(this.chkPriorityAlert);
		this.pnlScanBasic.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnlScanBasic.Location = new System.Drawing.Point(0, 0);
		this.pnlScanBasic.Name = "pnlScanBasic";
		this.pnlScanBasic.Size = new System.Drawing.Size(465, 328);
		this.pnlScanBasic.TabIndex = 0;
		this.nudAnalogHang.Increment = new decimal(new int[4] { 500, 0, 0, 0 });
		this.nudAnalogHang.method_2(null);
		this.nudAnalogHang.Location = new System.Drawing.Point(261, 104);
		this.nudAnalogHang.Maximum = new decimal(new int[4] { 10000, 0, 0, 0 });
		this.nudAnalogHang.Minimum = new decimal(new int[4] { 500, 0, 0, 0 });
		this.nudAnalogHang.Name = "nudAnalogHang";
		this.nudAnalogHang.method_6(null);
		int[] bits = new int[4];
		this.nudAnalogHang.method_4(new decimal(bits));
		this.nudAnalogHang.Size = new System.Drawing.Size(140, 23);
		this.nudAnalogHang.TabIndex = 3;
		this.nudAnalogHang.Value = new decimal(new int[4] { 500, 0, 0, 0 });
		this.nudScanTime.Increment = new decimal(new int[4] { 5, 0, 0, 0 });
		this.nudScanTime.method_2(null);
		this.nudScanTime.Location = new System.Drawing.Point(261, 40);
		this.nudScanTime.Maximum = new decimal(new int[4] { 60, 0, 0, 0 });
		this.nudScanTime.Minimum = new decimal(new int[4] { 5, 0, 0, 0 });
		this.nudScanTime.Name = "nudScanTime";
		this.nudScanTime.method_6(null);
		int[] bits2 = new int[4];
		this.nudScanTime.method_4(new decimal(bits2));
		this.nudScanTime.Size = new System.Drawing.Size(140, 23);
		this.nudScanTime.TabIndex = 6;
		this.nudScanTime.Value = new decimal(new int[4] { 5, 0, 0, 0 });
		this.nudScanTime.Visible = true;
		this.lblScanTime.Visible = true;
		this.nudStartVoteRssi.method_2(null);
		this.nudStartVoteRssi.Location = new System.Drawing.Point(261, 234);
		this.nudStartVoteRssi.Maximum = new decimal(new int[4] { 70, 0, 0, -2147483648 });
		this.nudStartVoteRssi.Minimum = new decimal(new int[4] { 120, 0, 0, -2147483648 });
		this.nudStartVoteRssi.Name = "nudStartVoteRssi";
		this.nudStartVoteRssi.method_6(null);
		int[] bits3 = new int[4];
		this.nudStartVoteRssi.method_4(new decimal(bits3));
		this.nudStartVoteRssi.Size = new System.Drawing.Size(140, 23);
		this.nudStartVoteRssi.TabIndex = 10;
		this.nudStartVoteRssi.Value = new decimal(new int[4] { 70, 0, 0, -2147483648 });
		this.nudStartVoteRssi.Visible = false;
		this.lblDigitHang.Location = new System.Drawing.Point(50, 72);
		this.lblDigitHang.Name = "lblDigitHang";
		this.lblDigitHang.Size = new System.Drawing.Size(198, 23);
		this.lblDigitHang.TabIndex = 0;
		this.lblDigitHang.Text = "Digital Hang Time [ms]";
		this.lblDigitHang.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.nudFastVoteRssi.method_2(null);
		this.nudFastVoteRssi.Location = new System.Drawing.Point(261, 202);
		this.nudFastVoteRssi.Maximum = new decimal(new int[4] { 70, 0, 0, -2147483648 });
		this.nudFastVoteRssi.Minimum = new decimal(new int[4] { 120, 0, 0, -2147483648 });
		this.nudFastVoteRssi.Name = "nudFastVoteRssi";
		this.nudFastVoteRssi.method_6(null);
		int[] bits4 = new int[4];
		this.nudFastVoteRssi.method_4(new decimal(bits4));
		this.nudFastVoteRssi.Size = new System.Drawing.Size(140, 23);
		this.nudFastVoteRssi.TabIndex = 8;
		this.nudFastVoteRssi.Value = new decimal(new int[4] { 70, 0, 0, -2147483648 });
		this.nudFastVoteRssi.Visible = false;
		this.lblAnalogHang.Location = new System.Drawing.Point(50, 104);
		this.lblAnalogHang.Name = "lblAnalogHang";
		this.lblAnalogHang.Size = new System.Drawing.Size(198, 23);
		this.lblAnalogHang.TabIndex = 2;
		this.lblAnalogHang.Text = "Analog Hang Time [ms]";
		this.lblAnalogHang.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblScanTime.Location = new System.Drawing.Point(50, 40);
		this.lblScanTime.Name = "lblScanTime";
		this.lblScanTime.Size = new System.Drawing.Size(198, 23);
		this.lblScanTime.TabIndex = 10;
		this.lblScanTime.Text = "Scan Time [s]";
		this.lblScanTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.nudVoteHang.DecimalPlaces = 2;
		this.nudVoteHang.Increment = new decimal(new int[4] { 25, 0, 0, 131072 });
		this.nudVoteHang.method_2(null);
		this.nudVoteHang.Location = new System.Drawing.Point(261, 169);
		this.nudVoteHang.Maximum = new decimal(new int[4] { 6375, 0, 0, 131072 });
		this.nudVoteHang.Name = "nudVoteHang";
		this.nudVoteHang.method_6(null);
		int[] bits5 = new int[4];
		this.nudVoteHang.method_4(new decimal(bits5));
		this.nudVoteHang.Size = new System.Drawing.Size(140, 23);
		this.nudVoteHang.TabIndex = 6;
		this.nudVoteHang.Visible = false;
		this.lblVoteHang.Location = new System.Drawing.Point(50, 169);
		this.lblVoteHang.Name = "lblVoteHang";
		this.lblVoteHang.Size = new System.Drawing.Size(198, 23);
		this.lblVoteHang.TabIndex = 5;
		this.lblVoteHang.Text = "Vote Scan Hang Time [s]";
		this.lblVoteHang.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblVoteHang.Visible = false;
		this.lblFastVoteRssi.Location = new System.Drawing.Point(50, 202);
		this.lblFastVoteRssi.Name = "lblFastVoteRssi";
		this.lblFastVoteRssi.Size = new System.Drawing.Size(198, 23);
		this.lblFastVoteRssi.TabIndex = 7;
		this.lblFastVoteRssi.Text = "Fast Vote Rssi Threshold [dB]";
		this.lblFastVoteRssi.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblFastVoteRssi.Visible = false;
		this.nudDigitHang.Increment = new decimal(new int[4] { 500, 0, 0, 0 });
		this.nudDigitHang.method_2(null);
		this.nudDigitHang.Location = new System.Drawing.Point(261, 72);
		this.nudDigitHang.Maximum = new decimal(new int[4] { 10000, 0, 0, 0 });
		this.nudDigitHang.Minimum = new decimal(new int[4] { 500, 0, 0, 0 });
		this.nudDigitHang.Name = "nudDigitHang";
		this.nudDigitHang.method_6(null);
		int[] bits6 = new int[4];
		this.nudDigitHang.method_4(new decimal(bits6));
		this.nudDigitHang.Size = new System.Drawing.Size(140, 23);
		this.nudDigitHang.TabIndex = 1;
		this.nudDigitHang.Value = new decimal(new int[4] { 500, 0, 0, 0 });
		this.lblStartVoteRssi.Location = new System.Drawing.Point(50, 234);
		this.lblStartVoteRssi.Name = "lblStartVoteRssi";
		this.lblStartVoteRssi.Size = new System.Drawing.Size(198, 23);
		this.lblStartVoteRssi.TabIndex = 9;
		this.lblStartVoteRssi.Text = "Start Vote Rssi Threshold [dB]";
		this.lblStartVoteRssi.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblStartVoteRssi.Visible = false;
		this.chkPriorityAlert.AutoSize = true;
		this.chkPriorityAlert.Location = new System.Drawing.Point(261, 137);
		this.chkPriorityAlert.Name = "chkPriorityAlert";
		this.chkPriorityAlert.Size = new System.Drawing.Size(103, 20);
		this.chkPriorityAlert.TabIndex = 4;
		this.chkPriorityAlert.Text = "Priority Alert";
		this.chkPriorityAlert.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.ClientSize = new System.Drawing.Size(465, 328);
		base.Controls.Add(this.pnlScanBasic);
		this.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular);
		base.Name = "ScanBasicForm";
		this.Text = "Scan";
		base.Load += new System.EventHandler(ScanBasicForm_Load);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ScanBasicForm_FormClosing);
		this.pnlScanBasic.ResumeLayout(false);
		this.pnlScanBasic.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogHang).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudScanTime).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudStartVoteRssi).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudFastVoteRssi).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudVoteHang).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudDigitHang).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	static ScanBasicForm()
	{
		SCL_VOTE_HANG = 0.25m;
		SCL_VOTE_HANG = 0.25m;
		DefaultScanBasic = new ScanBasic();
		data = new ScanBasic();
	}
}
