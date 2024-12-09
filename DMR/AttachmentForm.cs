using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class AttachmentForm : DockContent, IDisp
{
	public enum Pk1Key
	{
		Default,
		P1Key,
		FM
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class Attachment : IVerify<Attachment>
	{
		private byte p1Key;

		private byte gps;

		private byte fm;

		private byte callZone;

		private byte callCh;

		private byte recording;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		private byte[] reserve;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		private ushort[] chFreq;

		public int P1Key
		{
			get
			{
				if (Enum.IsDefined(typeof(Pk1Key), (int)p1Key))
				{
					return p1Key;
				}
				return 0;
			}
			set
			{
				p1Key = (byte)value;
			}
		}

		public bool GpsSwitch
		{
			get
			{
				return Settings.smethod_14(gps, 6, 2) == 0;
			}
			set
			{
				if (value)
				{
					gps &= 63;
				}
				else
				{
					gps |= 192;
				}
			}
		}

		public int TimeZone
		{
			get
			{
				int num = Settings.smethod_14(gps, 1, 5);
				if (num >= 0 && num <= 24)
				{
					return num;
				}
				return 0;
			}
			set
			{
				Settings.smethod_16(ref gps, 1, 5, value);
			}
		}

		public bool FmSwitch
		{
			get
			{
				return Settings.smethod_14(fm, 6, 2) == 0;
			}
			set
			{
				if (value)
				{
					Settings.smethod_15(ref fm, 6, 2);
				}
				else
				{
					Settings.smethod_16(ref fm, 6, 2, 3);
				}
			}
		}

		public int FmBootCh
		{
			get
			{
				int num = Settings.smethod_14(fm, 1, 5);
				if (num < 0 || num >= 20)
				{
					num = 0;
				}
				return num;
			}
			set
			{
				Settings.smethod_16(ref fm, 1, 5, value);
			}
		}

		public int CallZone
		{
			get
			{
				if (callZone >= 250 && callZone != byte.MaxValue)
				{
					return 0;
				}
				return callZone;
			}
			set
			{
				if (value < 250 || value == 255)
				{
					callZone = (byte)value;
				}
			}
		}

		public int CallCh
		{
			get
			{
				if (callCh >= ChannelForm.CurCntCh && callCh != byte.MaxValue)
				{
					return 0;
				}
				return callCh;
			}
			set
			{
				if (value < ChannelForm.CurCntCh || value == 255)
				{
					callCh = (byte)value;
				}
			}
		}

		public bool RecordingSwitch
		{
			get
			{
				return Settings.smethod_14(recording, 7, 1) == 0;
			}
			set
			{
				if (value)
				{
					Settings.smethod_15(ref recording, 7, 1);
				}
				else
				{
					Settings.smethod_16(ref recording, 7, 1, 1);
				}
			}
		}

		public int RecordingInterval
		{
			get
			{
				int num = Settings.smethod_14(recording, 1, 5);
				if (num < 0 || num >= 31)
				{
					num = 0;
				}
				return num;
			}
			set
			{
				Settings.smethod_16(ref recording, 1, 5, value);
			}
		}

		public byte[] Reserve => reserve;

		public string this[int index]
		{
			get
			{
				if (index < 20)
				{
					if (chFreq[index] >= 6500 && chFreq[index] <= 10800)
					{
						return ((double)(int)chFreq[index] / 100.0).ToString("f2");
					}
					return "";
				}
				throw new ArgumentOutOfRangeException();
			}
			set
			{
				if (index < 20)
				{
					if (string.IsNullOrEmpty(value))
					{
						chFreq[index] = ushort.MaxValue;
						return;
					}
					double num = Convert.ToDouble(value);
					chFreq[index] = Convert.ToUInt16(num * 100.0);
				}
			}
		}

		public Attachment()
		{
			reserve = new byte[2];
			chFreq = new ushort[20];
		}

		public void RefreshP1Key()
		{
			if (p1Key == 2)
			{
				p1Key = 0;
			}
		}

		public void Verify(Attachment def)
		{
			if (TimeZone < 0 || TimeZone > 24)
			{
				TimeZone = def.TimeZone;
			}
			if (FmBootCh < 0 || FmBootCh >= 20)
			{
				FmBootCh = 0;
			}
		}
	}

	public const int CNT_FM = 20;

	private const int LEN_FM_FREQ = 6;

	public const string SZ_PK1_KEY_NAME = "Pk1KeyName";

	private const int MIN_TIME_ZONE = 0;

	private const int MAX_TIME_ZONE = 24;

	private const int MIN_FM_FREQ = 6500;

	private const int MAX_FM_FREQ = 10800;

	private const int MIN_RECORDING_INTERVAL = 0;

	private const int MAX_RECORDING_INTERVAL = 31;

	private DataGridView dgvAttachment;

	private CustomPanel pnlAttachment;

	private Label lblTimeZone;

	private CheckBox chkGpsSwitch;

	private CustomCombo cmbTimeZone;

	private Label lblP1Key;

	private CustomCombo cmbP1Key;

	private CheckBox chkFmSwitch;

	private GroupBox grpGps;

	private GroupBox grpP1;

	private GroupBox grpFm;

	private ComboBox cmbBootCh;

	private Label lblBootCh;

	private GroupBox grpRecording;

	private CheckBox chkRecordingSwitch;

	private CustomCombo cmbRecordingInterval;

	private Label lblRecordingInterval;

	private CustomCombo cmbCallCh;

	private Label lblCallCh;

	private CustomCombo cmbCallZone;

	private Label lblCallZone;

	private GroupBox grpCall;

	private DataGridViewTextBoxColumn txtMessage;

	private static readonly string[] SZ_PK1_KEY;

	public static Attachment DefaultAttachment;

	public static Attachment data;

	public TreeNode Node { get; set; }

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		this.pnlAttachment = new CustomPanel();
		this.grpCall = new System.Windows.Forms.GroupBox();
		this.cmbCallCh = new CustomCombo();
		this.lblCallCh = new System.Windows.Forms.Label();
		this.cmbCallZone = new CustomCombo();
		this.lblCallZone = new System.Windows.Forms.Label();
		this.grpRecording = new System.Windows.Forms.GroupBox();
		this.chkRecordingSwitch = new System.Windows.Forms.CheckBox();
		this.cmbRecordingInterval = new CustomCombo();
		this.lblRecordingInterval = new System.Windows.Forms.Label();
		this.grpGps = new System.Windows.Forms.GroupBox();
		this.chkGpsSwitch = new System.Windows.Forms.CheckBox();
		this.cmbTimeZone = new CustomCombo();
		this.lblTimeZone = new System.Windows.Forms.Label();
		this.grpP1 = new System.Windows.Forms.GroupBox();
		this.cmbP1Key = new CustomCombo();
		this.lblP1Key = new System.Windows.Forms.Label();
		this.grpFm = new System.Windows.Forms.GroupBox();
		this.cmbBootCh = new System.Windows.Forms.ComboBox();
		this.lblBootCh = new System.Windows.Forms.Label();
		this.chkFmSwitch = new System.Windows.Forms.CheckBox();
		this.dgvAttachment = new System.Windows.Forms.DataGridView();
		this.txtMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.pnlAttachment.SuspendLayout();
		this.grpCall.SuspendLayout();
		this.grpRecording.SuspendLayout();
		this.grpGps.SuspendLayout();
		this.grpP1.SuspendLayout();
		this.grpFm.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvAttachment).BeginInit();
		base.SuspendLayout();
		this.pnlAttachment.AutoScroll = true;
		this.pnlAttachment.AutoSize = true;
		this.pnlAttachment.Controls.Add(this.grpCall);
		this.pnlAttachment.Controls.Add(this.grpRecording);
		this.pnlAttachment.Controls.Add(this.grpGps);
		this.pnlAttachment.Controls.Add(this.grpP1);
		this.pnlAttachment.Controls.Add(this.grpFm);
		this.pnlAttachment.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnlAttachment.Location = new System.Drawing.Point(0, 0);
		this.pnlAttachment.Name = "pnlAttachment";
		this.pnlAttachment.Size = new System.Drawing.Size(759, 553);
		this.pnlAttachment.TabIndex = 3;
		this.grpCall.Controls.Add(this.cmbCallCh);
		this.grpCall.Controls.Add(this.lblCallCh);
		this.grpCall.Controls.Add(this.cmbCallZone);
		this.grpCall.Controls.Add(this.lblCallZone);
		this.grpCall.Location = new System.Drawing.Point(43, 369);
		this.grpCall.Name = "grpCall";
		this.grpCall.Size = new System.Drawing.Size(291, 100);
		this.grpCall.TabIndex = 18;
		this.grpCall.TabStop = false;
		this.grpCall.Text = "Call Channel";
		this.grpCall.Visible = false;
		this.cmbCallCh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbCallCh.FormattingEnabled = true;
		this.cmbCallCh.Location = new System.Drawing.Point(148, 57);
		this.cmbCallCh.Name = "cmbCallCh";
		this.cmbCallCh.Size = new System.Drawing.Size(87, 20);
		this.cmbCallCh.TabIndex = 17;
		this.lblCallCh.Location = new System.Drawing.Point(22, 57);
		this.lblCallCh.Name = "lblCallCh";
		this.lblCallCh.Size = new System.Drawing.Size(119, 20);
		this.lblCallCh.TabIndex = 16;
		this.lblCallCh.Text = "Channel";
		this.lblCallCh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.cmbCallZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbCallZone.FormattingEnabled = true;
		this.cmbCallZone.Location = new System.Drawing.Point(148, 23);
		this.cmbCallZone.Name = "cmbCallZone";
		this.cmbCallZone.Size = new System.Drawing.Size(87, 20);
		this.cmbCallZone.TabIndex = 15;
		this.cmbCallZone.SelectedIndexChanged += new System.EventHandler(cmbCallZone_SelectedIndexChanged);
		this.lblCallZone.Location = new System.Drawing.Point(22, 23);
		this.lblCallZone.Name = "lblCallZone";
		this.lblCallZone.Size = new System.Drawing.Size(119, 20);
		this.lblCallZone.TabIndex = 14;
		this.lblCallZone.Text = "Zone";
		this.lblCallZone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.grpRecording.Controls.Add(this.chkRecordingSwitch);
		this.grpRecording.Controls.Add(this.cmbRecordingInterval);
		this.grpRecording.Controls.Add(this.lblRecordingInterval);
		this.grpRecording.Location = new System.Drawing.Point(43, 251);
		this.grpRecording.Name = "grpRecording";
		this.grpRecording.Size = new System.Drawing.Size(291, 100);
		this.grpRecording.TabIndex = 13;
		this.grpRecording.TabStop = false;
		this.grpRecording.Text = "Recording";
		this.grpRecording.Visible = false;
		this.chkRecordingSwitch.AutoSize = true;
		this.chkRecordingSwitch.Location = new System.Drawing.Point(148, 23);
		this.chkRecordingSwitch.Name = "chkRecordingSwitch";
		this.chkRecordingSwitch.Size = new System.Drawing.Size(120, 16);
		this.chkRecordingSwitch.TabIndex = 6;
		this.chkRecordingSwitch.Text = "Recording Swicth";
		this.chkRecordingSwitch.UseVisualStyleBackColor = true;
		this.cmbRecordingInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbRecordingInterval.FormattingEnabled = true;
		this.cmbRecordingInterval.Location = new System.Drawing.Point(148, 51);
		this.cmbRecordingInterval.Name = "cmbRecordingInterval";
		this.cmbRecordingInterval.Size = new System.Drawing.Size(84, 20);
		this.cmbRecordingInterval.TabIndex = 8;
		this.lblRecordingInterval.Location = new System.Drawing.Point(22, 51);
		this.lblRecordingInterval.Name = "lblRecordingInterval";
		this.lblRecordingInterval.Size = new System.Drawing.Size(119, 20);
		this.lblRecordingInterval.TabIndex = 7;
		this.lblRecordingInterval.Text = "Recording Interval";
		this.lblRecordingInterval.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.grpGps.Controls.Add(this.chkGpsSwitch);
		this.grpGps.Controls.Add(this.cmbTimeZone);
		this.grpGps.Controls.Add(this.lblTimeZone);
		this.grpGps.Location = new System.Drawing.Point(43, 128);
		this.grpGps.Name = "grpGps";
		this.grpGps.Size = new System.Drawing.Size(291, 100);
		this.grpGps.TabIndex = 12;
		this.grpGps.TabStop = false;
		this.grpGps.Text = "Gps";
		this.chkGpsSwitch.AutoSize = true;
		this.chkGpsSwitch.Location = new System.Drawing.Point(148, 23);
		this.chkGpsSwitch.Name = "chkGpsSwitch";
		this.chkGpsSwitch.Size = new System.Drawing.Size(84, 16);
		this.chkGpsSwitch.TabIndex = 6;
		this.chkGpsSwitch.Text = "Gps Switch";
		this.chkGpsSwitch.UseVisualStyleBackColor = true;
		this.cmbTimeZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbTimeZone.FormattingEnabled = true;
		this.cmbTimeZone.Location = new System.Drawing.Point(148, 51);
		this.cmbTimeZone.Name = "cmbTimeZone";
		this.cmbTimeZone.Size = new System.Drawing.Size(84, 20);
		this.cmbTimeZone.TabIndex = 8;
		this.lblTimeZone.Location = new System.Drawing.Point(22, 51);
		this.lblTimeZone.Name = "lblTimeZone";
		this.lblTimeZone.Size = new System.Drawing.Size(119, 20);
		this.lblTimeZone.TabIndex = 7;
		this.lblTimeZone.Text = "TimeZone";
		this.lblTimeZone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.grpP1.Controls.Add(this.cmbP1Key);
		this.grpP1.Controls.Add(this.lblP1Key);
		this.grpP1.Location = new System.Drawing.Point(43, 22);
		this.grpP1.Name = "grpP1";
		this.grpP1.Size = new System.Drawing.Size(291, 88);
		this.grpP1.TabIndex = 12;
		this.grpP1.TabStop = false;
		this.grpP1.Text = "P1 ";
		this.cmbP1Key.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbP1Key.FormattingEnabled = true;
		this.cmbP1Key.Location = new System.Drawing.Point(148, 36);
		this.cmbP1Key.Name = "cmbP1Key";
		this.cmbP1Key.Size = new System.Drawing.Size(84, 20);
		this.cmbP1Key.TabIndex = 10;
		this.lblP1Key.Location = new System.Drawing.Point(22, 36);
		this.lblP1Key.Name = "lblP1Key";
		this.lblP1Key.Size = new System.Drawing.Size(119, 20);
		this.lblP1Key.TabIndex = 9;
		this.lblP1Key.Text = "P1 Key";
		this.lblP1Key.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.grpFm.Controls.Add(this.cmbBootCh);
		this.grpFm.Controls.Add(this.lblBootCh);
		this.grpFm.Controls.Add(this.chkFmSwitch);
		this.grpFm.Controls.Add(this.dgvAttachment);
		this.grpFm.Location = new System.Drawing.Point(362, 22);
		this.grpFm.Name = "grpFm";
		this.grpFm.Size = new System.Drawing.Size(323, 496);
		this.grpFm.TabIndex = 11;
		this.grpFm.TabStop = false;
		this.grpFm.Text = "FM";
		this.cmbBootCh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbBootCh.FormattingEnabled = true;
		this.cmbBootCh.Location = new System.Drawing.Point(131, 56);
		this.cmbBootCh.Name = "cmbBootCh";
		this.cmbBootCh.Size = new System.Drawing.Size(121, 20);
		this.cmbBootCh.TabIndex = 11;
		this.lblBootCh.Location = new System.Drawing.Point(34, 56);
		this.lblBootCh.Name = "lblBootCh";
		this.lblBootCh.Size = new System.Drawing.Size(90, 20);
		this.lblBootCh.TabIndex = 10;
		this.lblBootCh.Text = "Boot Channel";
		this.lblBootCh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.chkFmSwitch.AutoSize = true;
		this.chkFmSwitch.Location = new System.Drawing.Point(131, 29);
		this.chkFmSwitch.Name = "chkFmSwitch";
		this.chkFmSwitch.Size = new System.Drawing.Size(102, 16);
		this.chkFmSwitch.TabIndex = 6;
		this.chkFmSwitch.Text = "Boot Enter Fm";
		this.chkFmSwitch.UseVisualStyleBackColor = true;
		this.dgvAttachment.AllowUserToAddRows = false;
		dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
		dataGridViewCellStyle.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular);
		dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
		dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvAttachment.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
		this.dgvAttachment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvAttachment.Columns.AddRange(this.txtMessage);
		this.dgvAttachment.Location = new System.Drawing.Point(29, 88);
		this.dgvAttachment.Name = "dgvAttachment";
		this.dgvAttachment.RowTemplate.Height = 23;
		this.dgvAttachment.Size = new System.Drawing.Size(264, 393);
		this.dgvAttachment.TabIndex = 2;
		this.dgvAttachment.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(dgvAttachment_RowPostPaint);
		this.dgvAttachment.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(dgvAttachment_CellEndEdit);
		this.dgvAttachment.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(dgvAttachment_EditingControlShowing);
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		this.txtMessage.DefaultCellStyle = dataGridViewCellStyle2;
		this.txtMessage.HeaderText = "Frequency";
		this.txtMessage.MaxInputLength = 50;
		this.txtMessage.Name = "txtMessage";
		this.txtMessage.Width = 200;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.ClientSize = new System.Drawing.Size(759, 553);
		base.Controls.Add(this.pnlAttachment);
		this.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular);
		base.Name = "AttachmentForm";
		this.Text = "Attachment";
		base.Load += new System.EventHandler(AttachmentForm_Load);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(AttachmentForm_FormClosing);
		this.pnlAttachment.ResumeLayout(false);
		this.grpCall.ResumeLayout(false);
		this.grpRecording.ResumeLayout(false);
		this.grpRecording.PerformLayout();
		this.grpGps.ResumeLayout(false);
		this.grpGps.PerformLayout();
		this.grpP1.ResumeLayout(false);
		this.grpFm.ResumeLayout(false);
		this.grpFm.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.dgvAttachment).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	public void SaveData()
	{
		try
		{
			int num = 0;
			int num2 = -1;
			data.P1Key = cmbP1Key.SelectedIndex;
			data.GpsSwitch = chkGpsSwitch.Checked;
			data.TimeZone = cmbTimeZone.SelectedIndex;
			data.FmSwitch = chkFmSwitch.Checked;
			data.FmBootCh = cmbBootCh.SelectedIndex;
			dgvAttachment.EndEdit();
			string text = "";
			for (num = 0; num < dgvAttachment.RowCount; num++)
			{
				text = ((dgvAttachment.Rows[num].Cells[0].Value == null) ? "" : dgvAttachment.Rows[num].Cells[0].Value.ToString());
				if (!string.IsNullOrEmpty(text) && num2 == -1)
				{
					num2 = num;
				}
				data[num] = text;
			}
			data.RecordingSwitch = chkRecordingSwitch.Checked;
			data.RecordingInterval = cmbRecordingInterval.SelectedIndex;
			data.CallZone = cmbCallZone.method_3();
			data.CallCh = cmbCallCh.method_3();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Source);
		}
	}

	public void DispData()
	{
		try
		{
			int num = 0;
			method_1();
			cmbP1Key.SelectedIndex = Math.Min(cmbP1Key.Items.Count - 1, data.P1Key);
			chkGpsSwitch.Checked = data.GpsSwitch;
			cmbTimeZone.SelectedIndex = data.TimeZone;
			chkFmSwitch.Checked = data.FmSwitch;
			cmbBootCh.SelectedIndex = data.FmBootCh;
			for (num = 0; num < dgvAttachment.RowCount; num++)
			{
				dgvAttachment.Rows[num].Cells[0].Value = data[num];
			}
			chkRecordingSwitch.Checked = data.RecordingSwitch;
			cmbRecordingInterval.SelectedIndex = data.RecordingInterval;
			int num2 = 0;
			method_3(cmbCallZone);
			num2 = data.CallZone;
			cmbCallZone.method_2(data.CallZone);
			method_4(num2, cmbCallCh);
			cmbCallCh.method_2(data.CallCh);
			method_2();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Source);
		}
	}

	public void RefreshName()
	{
	}

	public AttachmentForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	private void method_1()
	{
		if (MenuForm.data.Fm)
		{
			Settings.fillComboBox(cmbP1Key, SZ_PK1_KEY);
		}
		else
		{
			Settings.smethod_38(cmbP1Key, SZ_PK1_KEY, 2);
		}
		string text = "";
		cmbTimeZone.Items.Clear();
		for (int i = 0; i <= 24; i++)
		{
			text = string.Format("GMT{0}", (i - 12).ToString("+0;-0"));
			cmbTimeZone.Items.Add(text);
		}
		Settings.smethod_41(cmbBootCh, 1, 20);
		dgvAttachment.RowCount = 20;
		dgvAttachment.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
		dgvAttachment.AllowUserToDeleteRows = false;
		dgvAttachment.AllowUserToAddRows = false;
		Settings.smethod_41(cmbRecordingInterval, 1, 32);
	}

	private void AttachmentForm_Load(object sender, EventArgs e)
	{
		Settings.smethod_59(base.Controls);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		DispData();
	}

	private void AttachmentForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveData();
	}

	private void method_2()
	{
	}

	private void dgvAttachment_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
	{
		if (e.RowIndex >= dgvAttachment.FirstDisplayedScrollingRowIndex)
		{
			using (SolidBrush brush = new SolidBrush(dgvAttachment.RowHeadersDefaultCellStyle.ForeColor))
			{
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				string s = (e.RowIndex + 1).ToString();
				Rectangle rectangle = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, dgvAttachment.RowHeadersWidth, e.RowBounds.Height);
				e.Graphics.DrawString(s, e.InheritedRowStyle.Font, brush, rectangle, stringFormat);
			}
		}
	}

	private void dgvAttachment_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
	{
		if (dgvAttachment.CurrentCell.ColumnIndex == 0)
		{
			e.Control.KeyPress += Content_KeyPress;
		}
	}

	private void dgvAttachment_CellEndEdit(object sender, DataGridViewCellEventArgs e)
	{
		if (e.ColumnIndex != 0)
		{
			return;
		}
		try
		{
			if (dgvAttachment[e.ColumnIndex, e.RowIndex].Value == null)
			{
				return;
			}
			string value = dgvAttachment[e.ColumnIndex, e.RowIndex].Value.ToString();
			if (!string.IsNullOrEmpty(value))
			{
				ushort num = Convert.ToUInt16(Convert.ToDouble(value) * 100.0);
				if (num < 6500 || num > 10800)
				{
					num = 6500;
				}
				dgvAttachment[e.ColumnIndex, e.RowIndex].Value = ((double)(int)num / 100.0).ToString("f2");
			}
			else
			{
				dgvAttachment[e.ColumnIndex, e.RowIndex].Value = "";
			}
		}
		catch (Exception)
		{
			dgvAttachment[e.ColumnIndex, e.RowIndex].Value = 65.0.ToString("f2");
		}
	}

	public static void Content_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (sender is TextBox && !char.IsControl(e.KeyChar) && (e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '.' && e.KeyChar != '.')
		{
			e.Handled = true;
		}
	}

	private void method_3(CustomCombo class4_0)
	{
		int num = 0;
		class4_0.method_0();
		class4_0.method_1(Settings.SZ_NONE, 255);
		for (num = 0; num <= 250; num++)
		{
			if (ZoneForm.data.ZoneChIsValid(num))
			{
				class4_0.method_1(ZoneForm.data.GetName(num), num);
			}
		}
	}

	private void method_4(int int_0, CustomCombo class4_0)
	{
		int num = 0;
		int num2 = 0;
		class4_0.method_0();
		if (int_0 == 255)
		{
			class4_0.method_1(Settings.SZ_NONE, 255);
			return;
		}
		for (num = 0; num < 16; num++)
		{
			num2 = ZoneForm.data[int_0].ChList[num] - 1;
			if (num2 >= 0 && num2 < ChannelForm.CurCntCh && ChannelForm.data.DataIsValid(num2))
			{
				class4_0.method_1(ChannelForm.data.GetName(num2), num);
			}
		}
	}

	private void cmbCallZone_SelectedIndexChanged(object sender, EventArgs e)
	{
		int int_ = cmbCallZone.method_3();
		method_4(int_, cmbCallCh);
		if (cmbCallCh.Items.Count > 0)
		{
			cmbCallCh.SelectedIndex = 0;
		}
	}

	static AttachmentForm()
	{
		SZ_PK1_KEY = new string[3] { "缺省", "P1侧键", "收音机" };
		data = new Attachment();
	}
}
