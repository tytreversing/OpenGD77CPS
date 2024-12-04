using System;
using System.Drawing;
using System.Windows.Forms;

namespace DMR;

public class PasswordForm : Form
{
	private Label lblPwd;

	private SGTextBox txtPwd;

	private Button btnOk;

	private Button btnCancel;

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.lblPwd = new System.Windows.Forms.Label();
		this.btnOk = new System.Windows.Forms.Button();
		this.btnCancel = new System.Windows.Forms.Button();
		this.txtPwd = new DMR.SGTextBox();
		base.SuspendLayout();
		this.lblPwd.AutoSize = true;
		this.lblPwd.Location = new System.Drawing.Point(39, 50);
		this.lblPwd.Name = "lblPwd";
		this.lblPwd.Size = new System.Drawing.Size(69, 16);
		this.lblPwd.TabIndex = 0;
		this.lblPwd.Text = "Password";
		this.btnOk.Location = new System.Drawing.Point(45, 102);
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 2;
		this.btnOk.Text = "OK";
		this.btnOk.UseVisualStyleBackColor = true;
		this.btnOk.Click += new System.EventHandler(btnOk_Click);
		this.btnCancel.Location = new System.Drawing.Point(159, 102);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 3;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.UseVisualStyleBackColor = true;
		this.txtPwd.InputString = null;
		this.txtPwd.Location = new System.Drawing.Point(113, 47);
		this.txtPwd.MaxByteLength = 0;
		this.txtPwd.Name = "txtPwd";
		this.txtPwd.PasswordChar = '*';
		this.txtPwd.Size = new System.Drawing.Size(129, 23);
		this.txtPwd.TabIndex = 1;
		base.AcceptButton = this.btnOk;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(268, 153);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.txtPwd);
		base.Controls.Add(this.lblPwd);
		this.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular);
		base.Name = "PasswordForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Password";
		base.Load += new System.EventHandler(PasswordForm_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	public PasswordForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	private void PasswordForm_Load(object sender, EventArgs e)
	{
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		txtPwd.MaxByteLength = 8;
		txtPwd.InputString = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz\b";
		txtPwd.Text = Settings.CUR_PWD;
	}

	private void btnOk_Click(object sender, EventArgs e)
	{
		Settings.CUR_PWD = txtPwd.Text;
	}
}
