using System;
using System.Drawing;
using System.Windows.Forms;

namespace DMR;

public class AboutForm : Form
{
	private Label lblVersion;

	private Label lblTranslationCredit;

	private Button btnClose;

	public AboutForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	private void AboutForm_Load(object sender, EventArgs e)
	{
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		lblVersion.Text = "OpenGD77 CPS";
	}

	private void btnClose_Click(object sender, EventArgs e)
	{
		Close();
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.lblVersion = new System.Windows.Forms.Label();
		this.btnClose = new System.Windows.Forms.Button();
		this.lblTranslationCredit = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.lblVersion.Location = new System.Drawing.Point(31, 20);
		this.lblVersion.Name = "lblVersion";
		this.lblVersion.Size = new System.Drawing.Size(351, 20);
		this.lblVersion.TabIndex = 0;
		this.lblVersion.Text = "v1.0.0";
		this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.btnClose.Location = new System.Drawing.Point(175, 60);
		this.btnClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnClose.Name = "btnClose";
		this.btnClose.Size = new System.Drawing.Size(64, 27);
		this.btnClose.TabIndex = 1;
		this.btnClose.Text = "OK";
		this.btnClose.UseVisualStyleBackColor = true;
		this.btnClose.Click += new System.EventHandler(btnClose_Click);
		this.lblTranslationCredit.Location = new System.Drawing.Point(31, 204);
		this.lblTranslationCredit.Name = "lblTranslationCredit";
		this.lblTranslationCredit.Size = new System.Drawing.Size(351, 20);
		this.lblTranslationCredit.TabIndex = 0;
		this.lblTranslationCredit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		base.ClientSize = new System.Drawing.Size(409, 104);
		base.Controls.Add(this.btnClose);
		base.Controls.Add(this.lblTranslationCredit);
		base.Controls.Add(this.lblVersion);
		this.Font = new System.Drawing.Font("Arial", 10f);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		base.MinimizeBox = false;
		base.Name = "AboutForm";
		this.Text = "About";
		base.Load += new System.EventHandler(AboutForm_Load);
		base.ResumeLayout(false);
	}
}
