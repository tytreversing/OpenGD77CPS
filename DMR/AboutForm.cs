using System;
using System.Drawing;
using System.Windows.Forms;

namespace DMR;

public class AboutForm : Form
{
	private Label lblVersion;

	private Label lblTranslationCredit;
    private Button telegramLink;
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
        lblVersion.Text = "OpenGD77 CPS - русская версия\r\n\r\nПредназначена для работы ТОЛЬКО\r\nс модифицированной прошивкой OpenGD77 RUS\r\n\r\n" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + "\r\n\r\n" + "(с) Aufwiegler, 2024";

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.lblVersion = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblTranslationCredit = new System.Windows.Forms.Label();
            this.telegramLink = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblVersion
            // 
            this.lblVersion.Location = new System.Drawing.Point(31, 20);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(351, 201);
            this.lblVersion.TabIndex = 0;
            this.lblVersion.Text = "1.0.0";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(175, 360);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(64, 27);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "OK";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblTranslationCredit
            // 
            this.lblTranslationCredit.Location = new System.Drawing.Point(31, 321);
            this.lblTranslationCredit.Name = "lblTranslationCredit";
            this.lblTranslationCredit.Size = new System.Drawing.Size(351, 20);
            this.lblTranslationCredit.TabIndex = 0;
            this.lblTranslationCredit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // telegramLink
            // 
            this.telegramLink.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.telegramLink.Cursor = System.Windows.Forms.Cursors.Hand;
            this.telegramLink.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
            this.telegramLink.FlatAppearance.BorderSize = 0;
            this.telegramLink.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.telegramLink.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.telegramLink.Image = ((System.Drawing.Image)(resources.GetObject("telegramLink.Image")));
            this.telegramLink.Location = new System.Drawing.Point(156, 240);
            this.telegramLink.Name = "telegramLink";
            this.telegramLink.Size = new System.Drawing.Size(105, 105);
            this.telegramLink.TabIndex = 3;
            this.telegramLink.UseVisualStyleBackColor = true;
            this.telegramLink.Click += new System.EventHandler(this.telegramLink_Click);
            // 
            // AboutForm
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(409, 404);
            this.Controls.Add(this.telegramLink);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblTranslationCredit);
            this.Controls.Add(this.lblVersion);
            this.Font = new System.Drawing.Font("Arial", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.Text = "About";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.ResumeLayout(false);

	}

    private void telegramLink_Click(object sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Process.Start("https://t.me/Opengd77_Russian");
        }
        catch (System.ComponentModel.Win32Exception noBrowser)
        {
            if (noBrowser.ErrorCode == -2147467259)
                MessageBox.Show(noBrowser.Message);
        }
        catch (System.Exception other)
        {
            MessageBox.Show(other.Message);
        }
    }
}
