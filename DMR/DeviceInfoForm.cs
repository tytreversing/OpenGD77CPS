using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class DeviceInfoForm : DockContent, IDisp
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class DeviceInfo
	{
		private ushort minFreq;

		private ushort maxFreq;

		private ushort minFreq2;

		private ushort maxFreq2;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		private byte[] lastPrgTime;

		private ushort reserve2;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		private byte[] model;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		private byte[] sn;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		private byte[] cpsSwVer;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		private byte[] hardwareVer;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		private byte[] firmwareVer;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
		private byte[] dspFwVer;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		private byte[] reserve3;

		public string MinFreq
		{
			get
			{
				return minFreq.ToString("X");
			}
			set
			{
				minFreq = ushort.Parse(value, NumberStyles.HexNumber);
			}
		}

		public string MaxFreq
		{
			get
			{
				return maxFreq.ToString("X");
			}
			set
			{
				maxFreq = ushort.Parse(value, NumberStyles.HexNumber);
			}
		}

		public string MinFreq2
		{
			get
			{
				return minFreq2.ToString("X");
			}
			set
			{
				minFreq2 = ushort.Parse(value, NumberStyles.HexNumber);
			}
		}

		public string MaxFreq2
		{
			get
			{
				return maxFreq2.ToString("X");
			}
			set
			{
				maxFreq2 = ushort.Parse(value, NumberStyles.HexNumber);
			}
		}

		public string LastPrgTime
		{
			get
			{
				string text = $"{lastPrgTime[0]:X02}{lastPrgTime[1]:X02}/{lastPrgTime[2]:X}/{lastPrgTime[3]:X} {lastPrgTime[4]:X}:{lastPrgTime[5]:X02}";
				DateTime result = default(DateTime);
				if (DateTime.TryParse(text, out result))
				{
					return text;
				}
				return "";
			}
		}

		public string Model
		{
			get
			{
				int num = 0;
				StringBuilder stringBuilder = new StringBuilder(8);
				for (num = 0; num < 8 && model[num] != byte.MaxValue; num++)
				{
					stringBuilder.Append(Convert.ToChar(model[num]));
				}
				return stringBuilder.ToString();
			}
			set
			{
				int num = 0;
				model.Fill(byte.MaxValue);
				for (num = 0; num < value.Length; num++)
				{
					model[num] = Convert.ToByte(value[num]);
				}
			}
		}

		public string Sn
		{
			get
			{
				int num = 0;
				StringBuilder stringBuilder = new StringBuilder(16);
				for (num = 0; num < 16 && sn[num] != byte.MaxValue; num++)
				{
					stringBuilder.Append(Convert.ToChar(sn[num]));
				}
				return stringBuilder.ToString();
			}
			set
			{
				int num = 0;
				sn.Fill(byte.MaxValue);
				for (num = 0; num < value.Length; num++)
				{
					sn[num] = Convert.ToByte(value[num]);
				}
			}
		}

		public string CpsSwVer
		{
			get
			{
				int num = 0;
				StringBuilder stringBuilder = new StringBuilder(8);
				for (num = 0; num < 8 && cpsSwVer[num] != byte.MaxValue; num++)
				{
					stringBuilder.Append(Convert.ToChar(cpsSwVer[num]));
				}
				return stringBuilder.ToString();
			}
			set
			{
				int num = 0;
				cpsSwVer.Fill(byte.MaxValue);
				for (num = 0; num < value.Length; num++)
				{
					cpsSwVer[num] = Convert.ToByte(value[num]);
				}
			}
		}

		public string HardwareVer
		{
			get
			{
				int num = 0;
				StringBuilder stringBuilder = new StringBuilder(8);
				for (num = 0; num < 8 && hardwareVer[num] != byte.MaxValue; num++)
				{
					stringBuilder.Append(Convert.ToChar(hardwareVer[num]));
				}
				return stringBuilder.ToString();
			}
			set
			{
				int num = 0;
				hardwareVer.Fill(byte.MaxValue);
				for (num = 0; num < value.Length; num++)
				{
					hardwareVer[num] = Convert.ToByte(value[num]);
				}
			}
		}

		public string FirmwareVer
		{
			get
			{
				int num = 0;
				StringBuilder stringBuilder = new StringBuilder(8);
				for (num = 0; num < 8 && firmwareVer[num] != byte.MaxValue; num++)
				{
					stringBuilder.Append(Convert.ToChar(firmwareVer[num]));
				}
				return stringBuilder.ToString();
			}
		}

		public string DspFwVer
		{
			get
			{
				int num = 0;
				StringBuilder stringBuilder = new StringBuilder(24);
				for (num = 0; num < 24 && dspFwVer[num] != byte.MaxValue; num++)
				{
					stringBuilder.Append(Convert.ToChar(dspFwVer[num]));
				}
				return stringBuilder.ToString();
			}
		}

		public DeviceInfo()
		{
			lastPrgTime = new byte[6];
			model = new byte[8];
			sn = new byte[16];
			cpsSwVer = new byte[8];
			hardwareVer = new byte[8];
			firmwareVer = new byte[8];
			dspFwVer = new byte[24];
			reserve3 = new byte[8];
		}
	}

	public const int LEN_MIN_FREQ = 3;

	public const int LEN_MAX_FREQ = 3;

	public const string SZ_MHZ_FREQ = "0123456789\b";

	public const int LEN_LAST_PRG_TIME = 6;

	public const int LEN_MODEL = 8;

	public const int LEN_SN = 16;

	public const int LEN_CSP_SW_VER = 8;

	public const int LEN_HARDWARE_VER = 8;

	public const int LEN_FIRMWARE_VER = 8;

	public const int LEN_DSP_FW_VER = 24;

	private Label lblModel;

	private SGTextBox txtModel;

	private Label lblSn;

	private SGTextBox txtSn;

	private SGTextBox sgtextBox_0;

	private Label label_0;

	private SGTextBox txtHardwareVer;

	private Label lblHardwareVer;

	private SGTextBox txtFirmwareVer;

	private Label lblFirmwareVer;

	private SGTextBox sgtextBox_1;

	private Label label_1;

	private TextBox txtLastPrgTime;

	private Label lblLastPrgTime;

	private SGTextBox txtMaxFreq;

	private Label lblSection2;

	private SGTextBox txtMinFreq;

	private Label lblSection1;

	private SGTextBox txtMaxFreq2;

	private SGTextBox txtMinFreq2;

	private Label lblTo2;

	private Label lblTo1;

	private CustomPanel pnlDeviceInfo;

	public static DeviceInfo data;

	public TreeNode Node { get; set; }

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
            this.pnlDeviceInfo = new CustomPanel();
            this.lblTo1 = new System.Windows.Forms.Label();
            this.lblTo2 = new System.Windows.Forms.Label();
            this.lblSection1 = new System.Windows.Forms.Label();
            this.label_1 = new System.Windows.Forms.Label();
            this.txtModel = new DMR.SGTextBox();
            this.lblFirmwareVer = new System.Windows.Forms.Label();
            this.txtLastPrgTime = new System.Windows.Forms.TextBox();
            this.lblHardwareVer = new System.Windows.Forms.Label();
            this.txtMaxFreq2 = new DMR.SGTextBox();
            this.txtMaxFreq = new DMR.SGTextBox();
            this.label_0 = new System.Windows.Forms.Label();
            this.txtMinFreq2 = new DMR.SGTextBox();
            this.txtMinFreq = new DMR.SGTextBox();
            this.lblSn = new System.Windows.Forms.Label();
            this.txtSn = new DMR.SGTextBox();
            this.sgtextBox_0 = new DMR.SGTextBox();
            this.lblSection2 = new System.Windows.Forms.Label();
            this.txtHardwareVer = new DMR.SGTextBox();
            this.lblLastPrgTime = new System.Windows.Forms.Label();
            this.txtFirmwareVer = new DMR.SGTextBox();
            this.lblModel = new System.Windows.Forms.Label();
            this.sgtextBox_1 = new DMR.SGTextBox();
            this.pnlDeviceInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlDeviceInfo
            // 
            this.pnlDeviceInfo.AutoScroll = true;
            this.pnlDeviceInfo.AutoSize = true;
            this.pnlDeviceInfo.Controls.Add(this.lblTo1);
            this.pnlDeviceInfo.Controls.Add(this.lblTo2);
            this.pnlDeviceInfo.Controls.Add(this.lblSection1);
            this.pnlDeviceInfo.Controls.Add(this.label_1);
            this.pnlDeviceInfo.Controls.Add(this.txtModel);
            this.pnlDeviceInfo.Controls.Add(this.lblFirmwareVer);
            this.pnlDeviceInfo.Controls.Add(this.txtLastPrgTime);
            this.pnlDeviceInfo.Controls.Add(this.lblHardwareVer);
            this.pnlDeviceInfo.Controls.Add(this.txtMaxFreq2);
            this.pnlDeviceInfo.Controls.Add(this.txtMaxFreq);
            this.pnlDeviceInfo.Controls.Add(this.label_0);
            this.pnlDeviceInfo.Controls.Add(this.txtMinFreq2);
            this.pnlDeviceInfo.Controls.Add(this.txtMinFreq);
            this.pnlDeviceInfo.Controls.Add(this.lblSn);
            this.pnlDeviceInfo.Controls.Add(this.txtSn);
            this.pnlDeviceInfo.Controls.Add(this.sgtextBox_0);
            this.pnlDeviceInfo.Controls.Add(this.lblSection2);
            this.pnlDeviceInfo.Controls.Add(this.txtHardwareVer);
            this.pnlDeviceInfo.Controls.Add(this.lblLastPrgTime);
            this.pnlDeviceInfo.Controls.Add(this.txtFirmwareVer);
            this.pnlDeviceInfo.Controls.Add(this.lblModel);
            this.pnlDeviceInfo.Controls.Add(this.sgtextBox_1);
            this.pnlDeviceInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDeviceInfo.Location = new System.Drawing.Point(0, 0);
            this.pnlDeviceInfo.Name = "pnlDeviceInfo";
            this.pnlDeviceInfo.Size = new System.Drawing.Size(397, 95);
            this.pnlDeviceInfo.TabIndex = 0;
            // 
            // lblTo1
            // 
            this.lblTo1.AutoSize = true;
            this.lblTo1.Location = new System.Drawing.Point(289, 50);
            this.lblTo1.Name = "lblTo1";
            this.lblTo1.Size = new System.Drawing.Size(12, 16);
            this.lblTo1.TabIndex = 2;
            this.lblTo1.Text = "-";
            // 
            // lblTo2
            // 
            this.lblTo2.AutoSize = true;
            this.lblTo2.Location = new System.Drawing.Point(288, 14);
            this.lblTo2.Name = "lblTo2";
            this.lblTo2.Size = new System.Drawing.Size(12, 16);
            this.lblTo2.TabIndex = 6;
            this.lblTo2.Text = "-";
            // 
            // lblSection1
            // 
            this.lblSection1.Location = new System.Drawing.Point(12, 47);
            this.lblSection1.Name = "lblSection1";
            this.lblSection1.Size = new System.Drawing.Size(205, 23);
            this.lblSection1.TabIndex = 0;
            this.lblSection1.Text = "Frequency Range 1 [MHz]";
            this.lblSection1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_1
            // 
            this.label_1.Location = new System.Drawing.Point(28, 309);
            this.label_1.Name = "label_1";
            this.label_1.Size = new System.Drawing.Size(172, 23);
            this.label_1.TabIndex = 20;
            this.label_1.Text = "DSP Version";
            this.label_1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label_1.Visible = false;
            // 
            // txtModel
            // 
            this.txtModel.InputString = null;
            this.txtModel.Location = new System.Drawing.Point(210, 135);
            this.txtModel.MaxByteLength = 0;
            this.txtModel.Name = "txtModel";
            this.txtModel.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtModel.ReadOnly = true;
            this.txtModel.Size = new System.Drawing.Size(139, 23);
            this.txtModel.TabIndex = 11;
            this.txtModel.Visible = false;
            // 
            // lblFirmwareVer
            // 
            this.lblFirmwareVer.Location = new System.Drawing.Point(28, 274);
            this.lblFirmwareVer.Name = "lblFirmwareVer";
            this.lblFirmwareVer.Size = new System.Drawing.Size(172, 23);
            this.lblFirmwareVer.TabIndex = 18;
            this.lblFirmwareVer.Text = "Firmware Version";
            this.lblFirmwareVer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblFirmwareVer.Visible = false;
            // 
            // txtLastPrgTime
            // 
            this.txtLastPrgTime.Location = new System.Drawing.Point(210, 101);
            this.txtLastPrgTime.Name = "txtLastPrgTime";
            this.txtLastPrgTime.ReadOnly = true;
            this.txtLastPrgTime.Size = new System.Drawing.Size(139, 23);
            this.txtLastPrgTime.TabIndex = 9;
            this.txtLastPrgTime.Visible = false;
            // 
            // lblHardwareVer
            // 
            this.lblHardwareVer.Location = new System.Drawing.Point(28, 239);
            this.lblHardwareVer.Name = "lblHardwareVer";
            this.lblHardwareVer.Size = new System.Drawing.Size(172, 23);
            this.lblHardwareVer.TabIndex = 16;
            this.lblHardwareVer.Text = "Hardware Version";
            this.lblHardwareVer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblHardwareVer.Visible = false;
            // 
            // txtMaxFreq2
            // 
            this.txtMaxFreq2.BackColor = System.Drawing.Color.White;
            this.txtMaxFreq2.InputString = null;
            this.txtMaxFreq2.Location = new System.Drawing.Point(306, 9);
            this.txtMaxFreq2.MaxByteLength = 0;
            this.txtMaxFreq2.Name = "txtMaxFreq2";
            this.txtMaxFreq2.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtMaxFreq2.ReadOnly = true;
            this.txtMaxFreq2.Size = new System.Drawing.Size(61, 23);
            this.txtMaxFreq2.TabIndex = 7;
            this.txtMaxFreq2.Validating += new System.ComponentModel.CancelEventHandler(this.txtMaxFreq2_Validating);
            // 
            // txtMaxFreq
            // 
            this.txtMaxFreq.BackColor = System.Drawing.Color.White;
            this.txtMaxFreq.InputString = null;
            this.txtMaxFreq.Location = new System.Drawing.Point(306, 47);
            this.txtMaxFreq.MaxByteLength = 0;
            this.txtMaxFreq.Name = "txtMaxFreq";
            this.txtMaxFreq.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtMaxFreq.ReadOnly = true;
            this.txtMaxFreq.Size = new System.Drawing.Size(61, 23);
            this.txtMaxFreq.TabIndex = 3;
            this.txtMaxFreq.Validating += new System.ComponentModel.CancelEventHandler(this.txtMaxFreq_Validating);
            // 
            // label_0
            // 
            this.label_0.Location = new System.Drawing.Point(28, 205);
            this.label_0.Name = "label_0";
            this.label_0.Size = new System.Drawing.Size(172, 23);
            this.label_0.TabIndex = 14;
            this.label_0.Text = "CPS Version";
            this.label_0.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label_0.Visible = false;
            // 
            // txtMinFreq2
            // 
            this.txtMinFreq2.BackColor = System.Drawing.Color.White;
            this.txtMinFreq2.InputString = null;
            this.txtMinFreq2.Location = new System.Drawing.Point(223, 9);
            this.txtMinFreq2.MaxByteLength = 0;
            this.txtMinFreq2.Name = "txtMinFreq2";
            this.txtMinFreq2.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtMinFreq2.ReadOnly = true;
            this.txtMinFreq2.Size = new System.Drawing.Size(61, 23);
            this.txtMinFreq2.TabIndex = 5;
            this.txtMinFreq2.Validating += new System.ComponentModel.CancelEventHandler(this.txtMinFreq2_Validating);
            // 
            // txtMinFreq
            // 
            this.txtMinFreq.BackColor = System.Drawing.Color.White;
            this.txtMinFreq.InputString = null;
            this.txtMinFreq.Location = new System.Drawing.Point(223, 47);
            this.txtMinFreq.MaxByteLength = 0;
            this.txtMinFreq.Name = "txtMinFreq";
            this.txtMinFreq.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtMinFreq.ReadOnly = true;
            this.txtMinFreq.Size = new System.Drawing.Size(61, 23);
            this.txtMinFreq.TabIndex = 1;
            this.txtMinFreq.Validating += new System.ComponentModel.CancelEventHandler(this.txtMinFreq_Validating);
            // 
            // lblSn
            // 
            this.lblSn.Location = new System.Drawing.Point(28, 170);
            this.lblSn.Name = "lblSn";
            this.lblSn.Size = new System.Drawing.Size(172, 23);
            this.lblSn.TabIndex = 12;
            this.lblSn.Text = "Serial Number";
            this.lblSn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblSn.Visible = false;
            // 
            // txtSn
            // 
            this.txtSn.InputString = null;
            this.txtSn.Location = new System.Drawing.Point(210, 170);
            this.txtSn.MaxByteLength = 0;
            this.txtSn.Name = "txtSn";
            this.txtSn.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtSn.ReadOnly = true;
            this.txtSn.Size = new System.Drawing.Size(139, 23);
            this.txtSn.TabIndex = 13;
            this.txtSn.Visible = false;
            // 
            // sgtextBox_0
            // 
            this.sgtextBox_0.InputString = null;
            this.sgtextBox_0.Location = new System.Drawing.Point(210, 205);
            this.sgtextBox_0.MaxByteLength = 0;
            this.sgtextBox_0.Name = "sgtextBox_0";
            this.sgtextBox_0.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.sgtextBox_0.ReadOnly = true;
            this.sgtextBox_0.Size = new System.Drawing.Size(139, 23);
            this.sgtextBox_0.TabIndex = 15;
            this.sgtextBox_0.Visible = false;
            // 
            // lblSection2
            // 
            this.lblSection2.Location = new System.Drawing.Point(12, 9);
            this.lblSection2.Name = "lblSection2";
            this.lblSection2.Size = new System.Drawing.Size(205, 23);
            this.lblSection2.TabIndex = 4;
            this.lblSection2.Text = "Frequency Range 2 [MHz]";
            this.lblSection2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtHardwareVer
            // 
            this.txtHardwareVer.InputString = null;
            this.txtHardwareVer.Location = new System.Drawing.Point(210, 239);
            this.txtHardwareVer.MaxByteLength = 0;
            this.txtHardwareVer.Name = "txtHardwareVer";
            this.txtHardwareVer.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtHardwareVer.ReadOnly = true;
            this.txtHardwareVer.Size = new System.Drawing.Size(139, 23);
            this.txtHardwareVer.TabIndex = 17;
            this.txtHardwareVer.Visible = false;
            // 
            // lblLastPrgTime
            // 
            this.lblLastPrgTime.Location = new System.Drawing.Point(28, 101);
            this.lblLastPrgTime.Name = "lblLastPrgTime";
            this.lblLastPrgTime.Size = new System.Drawing.Size(172, 23);
            this.lblLastPrgTime.TabIndex = 8;
            this.lblLastPrgTime.Text = "Last Programed Date";
            this.lblLastPrgTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblLastPrgTime.Visible = false;
            // 
            // txtFirmwareVer
            // 
            this.txtFirmwareVer.InputString = null;
            this.txtFirmwareVer.Location = new System.Drawing.Point(210, 274);
            this.txtFirmwareVer.MaxByteLength = 0;
            this.txtFirmwareVer.Name = "txtFirmwareVer";
            this.txtFirmwareVer.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtFirmwareVer.ReadOnly = true;
            this.txtFirmwareVer.Size = new System.Drawing.Size(139, 23);
            this.txtFirmwareVer.TabIndex = 19;
            this.txtFirmwareVer.Visible = false;
            // 
            // lblModel
            // 
            this.lblModel.Location = new System.Drawing.Point(28, 135);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(172, 23);
            this.lblModel.TabIndex = 10;
            this.lblModel.Text = "Model Name";
            this.lblModel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblModel.Visible = false;
            // 
            // sgtextBox_1
            // 
            this.sgtextBox_1.InputString = null;
            this.sgtextBox_1.Location = new System.Drawing.Point(210, 309);
            this.sgtextBox_1.MaxByteLength = 0;
            this.sgtextBox_1.Name = "sgtextBox_1";
            this.sgtextBox_1.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.sgtextBox_1.ReadOnly = true;
            this.sgtextBox_1.Size = new System.Drawing.Size(139, 23);
            this.sgtextBox_1.TabIndex = 21;
            this.sgtextBox_1.Visible = false;
            // 
            // DeviceInfoForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(397, 95);
            this.Controls.Add(this.pnlDeviceInfo);
            this.Font = new System.Drawing.Font("Arial", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeviceInfoForm";
            this.Text = "Basic Information";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DeviceInfoForm_FormClosing);
            this.Load += new System.EventHandler(this.DeviceInfoForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DeviceInfoForm_KeyDown);
            this.pnlDeviceInfo.ResumeLayout(false);
            this.pnlDeviceInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

	}

	public void SaveData()
	{
		data.MinFreq = txtMinFreq.Text;
		data.MaxFreq = txtMaxFreq.Text;
		data.MinFreq2 = txtMinFreq2.Text;
		data.MaxFreq2 = txtMaxFreq2.Text;
		if (Settings.CUR_MODE > 0)
		{
			data.Sn = txtSn.Text;
			data.Model = txtModel.Text;
			data.CpsSwVer = sgtextBox_0.Text;
			data.HardwareVer = txtHardwareVer.Text;
		}
		((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
	}

	public void DispData()
	{
		txtMinFreq.Text = data.MinFreq;
		txtMaxFreq.Text = data.MaxFreq;
		txtMinFreq2.Text = data.MinFreq2;
		txtMaxFreq2.Text = data.MaxFreq2;
		txtLastPrgTime.Text = data.LastPrgTime;
		txtModel.Text = data.Model;
		txtSn.Text = data.Sn;
		sgtextBox_0.Text = data.CpsSwVer;
		txtHardwareVer.Text = data.HardwareVer;
		txtFirmwareVer.Text = data.FirmwareVer;
		sgtextBox_1.Text = data.DspFwVer;
		RefreshByUserMode();
	}

	public void RefreshByUserMode()
	{
		Settings.getUserExpertSettings();
		bool flag = Settings.CUR_MODE > 0;
		bool flag2 = Settings.CUR_MODE > 1;
		txtMinFreq.ReadOnly = !flag;
		txtMaxFreq.ReadOnly = !flag;
		txtMinFreq2.ReadOnly = !flag;
		txtMaxFreq2.ReadOnly = !flag;
		txtModel.ReadOnly = !flag2;
		txtSn.ReadOnly = !flag2;
		txtHardwareVer.ReadOnly = !flag2;
		sgtextBox_0.ReadOnly = !flag2;
	}

	public void RefreshName()
	{
	}

	public DeviceInfoForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	private void method_1()
	{
		if (Settings.CUR_MODE > 0)
		{
			txtModel.ReadOnly = true;
			txtSn.ReadOnly = true;
		}
		txtMinFreq.MaxLength = 3;
		txtMinFreq.InputString = "0123456789\b";
		txtMaxFreq.MaxLength = 3;
		txtMaxFreq.InputString = "0123456789\b";
		txtMinFreq2.MaxLength = 3;
		txtMinFreq2.InputString = "0123456789\b";
		txtMaxFreq2.MaxLength = 3;
		txtMaxFreq2.InputString = "0123456789\b";
		txtModel.MaxByteLength = 8;
		txtSn.MaxByteLength = 16;
		sgtextBox_0.MaxByteLength = 8;
		txtHardwareVer.MaxByteLength = 8;
		txtFirmwareVer.MaxByteLength = 8;
		sgtextBox_1.MaxByteLength = 24;
	}

	private void DeviceInfoForm_Load(object sender, EventArgs e)
	{
		Settings.smethod_59(base.Controls);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		method_1();
		DispData();
	}

	private void DeviceInfoForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveData();
	}

	private void DeviceInfoForm_KeyDown(object sender, KeyEventArgs e)
	{
	}

	private void txtMinFreq_Validating(object sender, CancelEventArgs e)
	{
		int num;
		try
		{
			num = int.Parse(txtMinFreq.Text);
		}
		catch (Exception)
		{
			num = (int)Settings.VALID_MIN_FREQ[0];
			txtMinFreq.Text = Settings.VALID_MIN_FREQ[0].ToString();
		}
		int num2;
		try
		{
			num2 = int.Parse(txtMaxFreq.Text);
		}
		catch (Exception)
		{
			num2 = (int)Settings.VALID_MAX_FREQ[0];
			txtMinFreq.Text = Settings.VALID_MAX_FREQ[0].ToString();
		}
		if (num > num2)
		{
			txtMinFreq.Text = Settings.VALID_MIN_FREQ[0].ToString();
		}
		else if (num < Settings.VALID_MIN_FREQ[0])
		{
			txtMinFreq.Text = Settings.VALID_MIN_FREQ[0].ToString();
		}
		else if (num >= Settings.VALID_MAX_FREQ[0])
		{
			txtMinFreq.Text = Settings.VALID_MIN_FREQ[0].ToString();
		}
	}

	private void txtMaxFreq_Validating(object sender, CancelEventArgs e)
	{
		int num;
		try
		{
			num = int.Parse(txtMinFreq.Text);
		}
		catch (Exception)
		{
			num = (int)Settings.VALID_MIN_FREQ[0];
			txtMinFreq.Text = Settings.VALID_MIN_FREQ[0].ToString();
		}
		int num2;
		try
		{
			num2 = int.Parse(txtMaxFreq.Text);
		}
		catch (Exception)
		{
			num2 = (int)Settings.VALID_MAX_FREQ[0];
			txtMaxFreq.Text = Settings.VALID_MAX_FREQ[0].ToString();
		}
		if (num2 <= num)
		{
			txtMaxFreq.Text = Settings.VALID_MAX_FREQ[0].ToString();
		}
		else if (num2 < Settings.VALID_MIN_FREQ[0])
		{
			txtMaxFreq.Text = Settings.VALID_MAX_FREQ[0].ToString();
		}
		else if (num2 > Settings.VALID_MAX_FREQ[0])
		{
			txtMaxFreq.Text = Settings.VALID_MAX_FREQ[0].ToString();
		}
	}

	private void txtMinFreq2_Validating(object sender, CancelEventArgs e)
	{
		int num;
		try
		{
			num = int.Parse(txtMinFreq2.Text);
		}
		catch (Exception)
		{
			num = (int)Settings.VALID_MIN_FREQ[1];
			txtMinFreq2.Text = Settings.VALID_MIN_FREQ[1].ToString();
		}
		int num2;
		try
		{
			num2 = int.Parse(txtMaxFreq2.Text);
		}
		catch (Exception)
		{
			num2 = (int)Settings.VALID_MAX_FREQ[1];
			txtMaxFreq2.Text = Settings.VALID_MAX_FREQ[1].ToString();
		}
		if (num > num2)
		{
			txtMinFreq2.Text = Settings.VALID_MIN_FREQ[1].ToString();
		}
		else if (num < Settings.VALID_MIN_FREQ[1])
		{
			txtMinFreq2.Text = Settings.VALID_MIN_FREQ[1].ToString();
		}
		else if (num >= Settings.VALID_MAX_FREQ[1])
		{
			txtMinFreq2.Text = Settings.VALID_MIN_FREQ[1].ToString();
		}
	}

	private void txtMaxFreq2_Validating(object sender, CancelEventArgs e)
	{
		int num;
		try
		{
			num = int.Parse(txtMinFreq2.Text);
		}
		catch (Exception)
		{
			num = (int)Settings.VALID_MIN_FREQ[1];
			txtMinFreq2.Text = Settings.VALID_MIN_FREQ[1].ToString();
		}
		int num2;
		try
		{
			num2 = int.Parse(txtMaxFreq2.Text);
		}
		catch (Exception)
		{
			num2 = (int)Settings.VALID_MAX_FREQ[1];
			txtMaxFreq2.Text = Settings.VALID_MAX_FREQ[1].ToString();
		}
		if (num2 <= num)
		{
			txtMaxFreq2.Text = Settings.VALID_MAX_FREQ[1].ToString();
		}
		else if (num2 < Settings.VALID_MIN_FREQ[1])
		{
			txtMaxFreq2.Text = Settings.VALID_MAX_FREQ[1].ToString();
		}
		else if (num2 > Settings.VALID_MAX_FREQ[1])
		{
			txtMaxFreq2.Text = Settings.VALID_MAX_FREQ[1].ToString();
		}
	}

	static DeviceInfoForm()
	{
		data = new DeviceInfo();
	}
}
