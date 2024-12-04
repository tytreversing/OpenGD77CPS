using System;
using System.Drawing;
using System.Windows.Forms;

namespace DMR;

public class CommPrgForm : Form
{
	private Label lblPrompt;

	private ProgressBar prgComm;

	private Button btnCancel;

	private Button btnOK;

	private CodeplugComms hidComm;

	private bool _closeWhenFinished;

	public bool IsSucess { get; set; }

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.lblPrompt = new System.Windows.Forms.Label();
		this.prgComm = new System.Windows.Forms.ProgressBar();
		this.btnCancel = new System.Windows.Forms.Button();
		this.btnOK = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.lblPrompt.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.lblPrompt.Location = new System.Drawing.Point(43, 118);
		this.lblPrompt.Name = "lblPrompt";
		this.lblPrompt.Size = new System.Drawing.Size(380, 26);
		this.lblPrompt.TabIndex = 0;
		this.lblPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.prgComm.Location = new System.Drawing.Point(43, 70);
		this.prgComm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.prgComm.Name = "prgComm";
		this.prgComm.Size = new System.Drawing.Size(380, 31);
		this.prgComm.TabIndex = 1;
		this.btnCancel.Location = new System.Drawing.Point(184, 161);
		this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(87, 31);
		this.btnCancel.TabIndex = 2;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.UseVisualStyleBackColor = true;
		this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
		this.btnOK.Location = new System.Drawing.Point(336, 161);
		this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnOK.Name = "btnOK";
		this.btnOK.Size = new System.Drawing.Size(87, 31);
		this.btnOK.TabIndex = 3;
		this.btnOK.Text = "OK";
		this.btnOK.UseVisualStyleBackColor = true;
		this.btnOK.Click += new System.EventHandler(btnOK_Click);
		this.btnOK.Visible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.ClientSize = new System.Drawing.Size(468, 214);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.prgComm);
		base.Controls.Add(this.lblPrompt);
		this.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular);
		base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		base.Name = "CommPrgForm";
		base.ShowInTaskbar = false;
		base.Load += new System.EventHandler(CommPrgForm_Load);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(CommPrgForm_FormClosing);
		base.ResumeLayout(false);
	}

	public CommPrgForm(bool closeWhenFinished = false)
	{
		_closeWhenFinished = closeWhenFinished;
		hidComm = new CodeplugComms();
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	private void CommPrgForm_Load(object sender, EventArgs e)
	{
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		prgComm.Minimum = 0;
		prgComm.Maximum = 100;
		switch (CodeplugComms.CommunicationMode)
		{
		case CodeplugComms.CommunicationType.codeplugRead:
			Text = Settings.dicCommon["CodeplugRead"];
			break;
		case CodeplugComms.CommunicationType.DMRIDRead:
			Text = Settings.dicCommon["DMRIDRead"];
			break;
		case CodeplugComms.CommunicationType.calibrationRead:
			Text = Settings.dicCommon["CalibrationRead"];
			break;
		case CodeplugComms.CommunicationType.codeplugWrite:
			Text = Settings.dicCommon["CodeplugWrite"];
			break;
		case CodeplugComms.CommunicationType.DMRIDWrite:
			Text = Settings.dicCommon["DMRIDWrite"];
			break;
		case CodeplugComms.CommunicationType.calibrationWrite:
			Text = Settings.dicCommon["CalibrationWrite"];
			break;
		case CodeplugComms.CommunicationType.dataRead:
			Text = Settings.dicCommon["dataRead"];
			break;
		case CodeplugComms.CommunicationType.dataWrite:
			Text = Settings.dicCommon["dataWrite"];
			break;
		}
		switch (CodeplugComms.CommunicationMode)
		{
		case CodeplugComms.CommunicationType.codeplugRead:
		case CodeplugComms.CommunicationType.DMRIDRead:
		case CodeplugComms.CommunicationType.calibrationRead:
			hidComm.START_ADDR = new int[7] { 128, 304, 21392, 29976, 32768, 44816, 95776 };
			hidComm.END_ADDR = new int[7] { 297, 14208, 22056, 30208, 32784, 45488, 126624 };
			break;
		case CodeplugComms.CommunicationType.codeplugWrite:
		case CodeplugComms.CommunicationType.DMRIDWrite:
		case CodeplugComms.CommunicationType.calibrationWrite:
			hidComm.START_ADDR = new int[7] { 128, 304, 21392, 29976, 32768, 44816, 95776 };
			hidComm.END_ADDR = new int[7] { 297, 14208, 22056, 30208, 32784, 45488, 126624 };
			break;
		}
		hidComm.SetProgressCallback(progressCallback);
		hidComm.startCodeplugReadOrWriteInNewThread();
	}

	private void CommPrgForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (hidComm.isThreadAlive())
		{
			hidComm.SetCancelComm(bool_0: true);
			hidComm.JoinThreadIfAlive();
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void btnOK_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void progressCallback(object sender, FirmwareUpdateProgressEventArgs e)
	{
		if (prgComm.InvokeRequired)
		{
			BeginInvoke(new EventHandler<FirmwareUpdateProgressEventArgs>(progressCallback), sender, e);
		}
		else if (e.Failed)
		{
			if (!string.IsNullOrEmpty(e.Message))
			{
				MessageBox.Show(e.Message, Settings.SZ_PROMPT);
			}
			Close();
		}
		else
		{
			if (e.Closed)
			{
				return;
			}
			prgComm.Value = (int)e.Percentage;
			if (e.Percentage == (float)prgComm.Maximum)
			{
				IsSucess = true;
				if (_closeWhenFinished)
				{
					base.DialogResult = DialogResult.OK;
					Close();
					return;
				}
				switch (CodeplugComms.CommunicationMode)
				{
				case CodeplugComms.CommunicationType.codeplugRead:
				case CodeplugComms.CommunicationType.DMRIDRead:
				case CodeplugComms.CommunicationType.calibrationRead:
					lblPrompt.Text = Settings.dicCommon["ReadComplete"];
					break;
				case CodeplugComms.CommunicationType.codeplugWrite:
				case CodeplugComms.CommunicationType.DMRIDWrite:
				case CodeplugComms.CommunicationType.calibrationWrite:
					lblPrompt.Text = Settings.dicCommon["WriteComplete"];
					break;
				}
				btnOK.Visible = true;
				btnCancel.Visible = false;
			}
			else
			{
				lblPrompt.Text = $"{prgComm.Value}%";
			}
		}
	}
}
