using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DMR;

public class WaitForm : Form
{
	private IContainer components;

	private Button btnClose;

	private Label lblInfo;

	private Timer tmrClose;

	public string Title { get; set; }

	public string Info { get; set; }

	public int Timeout { get; set; }

	public int Interval { get; set; }

	public int UseTime { get; set; }

	public WaitForm()
	{
	}

	public WaitForm(string info, int timeout)
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
		Info = info;
		Interval = 1000;
		Timeout = timeout;
	}

	private void WaitForm_Load(object sender, EventArgs e)
	{
		UseTime = 0;
		Text = $" {(Timeout - UseTime) / 1000}s";
		lblInfo.Text = Info;
		lblInfo.Left = (base.Width - lblInfo.Width) / 2;
		tmrClose.Interval = Interval;
		tmrClose.Start();
	}

	private void btnClose_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void tmrClose_Tick(object sender, EventArgs e)
	{
		UseTime += Interval;
		if (UseTime >= Timeout)
		{
			Close();
		}
		else
		{
			Text = $" {(Timeout - UseTime) / 1000}s";
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.btnClose = new System.Windows.Forms.Button();
		this.lblInfo = new System.Windows.Forms.Label();
		this.tmrClose = new System.Windows.Forms.Timer(this.components);
		base.SuspendLayout();
		this.btnClose.Location = new System.Drawing.Point(149, 122);
		this.btnClose.Name = "btnClose";
		this.btnClose.Size = new System.Drawing.Size(75, 23);
		this.btnClose.TabIndex = 0;
		this.btnClose.Text = "OK";
		this.btnClose.UseVisualStyleBackColor = true;
		this.btnClose.Click += new System.EventHandler(btnClose_Click);
		this.lblInfo.AutoSize = true;
		this.lblInfo.Location = new System.Drawing.Point(38, 50);
		this.lblInfo.Name = "lblInfo";
		this.lblInfo.Size = new System.Drawing.Size(41, 12);
		this.lblInfo.TabIndex = 1;
		this.lblInfo.Text = "Prompt";
		this.tmrClose.Tick += new System.EventHandler(tmrClose_Tick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.ClientSize = new System.Drawing.Size(373, 192);
		base.Controls.Add(this.lblInfo);
		base.Controls.Add(this.btnClose);
		this.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular);
		base.Name = "WaitForm";
		this.Text = "WaitForm";
		base.Load += new System.EventHandler(WaitForm_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
