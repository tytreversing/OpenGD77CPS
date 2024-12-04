using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class HelpForm : ToolWindow
{
	private Panel pnlHelp;

	private WebBrowser wbHelp;

	public HelpForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
		base.AllowEndUserDocking = true;
	}

	public void ShowHelp(string help)
	{
		wbHelp.Navigate(help);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.pnlHelp = new System.Windows.Forms.Panel();
		this.wbHelp = new System.Windows.Forms.WebBrowser();
		this.pnlHelp.SuspendLayout();
		base.SuspendLayout();
		this.pnlHelp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.pnlHelp.Controls.Add(this.wbHelp);
		this.pnlHelp.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnlHelp.Location = new System.Drawing.Point(0, 0);
		this.pnlHelp.Name = "pnlHelp";
		this.pnlHelp.Size = new System.Drawing.Size(284, 262);
		this.pnlHelp.TabIndex = 7;
		this.wbHelp.Dock = System.Windows.Forms.DockStyle.Fill;
		this.wbHelp.IsWebBrowserContextMenuEnabled = false;
		this.wbHelp.Location = new System.Drawing.Point(0, 0);
		this.wbHelp.MinimumSize = new System.Drawing.Size(20, 20);
		this.wbHelp.Name = "wbHelp";
		this.wbHelp.Size = new System.Drawing.Size(280, 258);
		this.wbHelp.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(284, 262);
		base.Controls.Add(this.pnlHelp);
		this.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.Name = "HelpForm";
		base.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide;
		base.TabText = "HelpView";
		this.Text = "HelpView";
		this.pnlHelp.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
