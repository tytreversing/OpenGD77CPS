using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DMR;

public class VoteScanForm : Form
{
	private byte[] data;

	private CheckBox chkTalkback;

	private CheckBox chkChMark;

	private CheckBox chkEarlyUnmute;

	private Label lblTxDesignatedCh;

	private Label lblPretimeDelay;

	private ComboBox comTxDesignatedCh;

	private ListBox lstUnselected;

	private ListBox lstSelected;

	private Button btnAdd;

	private Button button2;

	private NumericUpDown nudPretimeDelay;

	public VoteScanForm()
	{
		data = new byte[40];
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	private void VoteScanForm_Load(object sender, EventArgs e)
	{
		try
		{
			Settings.smethod_59(base.Controls);
			Settings.UpdateComponentTextsFromLanguageXmlData(this);
			chkChMark.Checked = Convert.ToBoolean(data[37] & 0x10);
			chkTalkback.Checked = Convert.ToBoolean(data[37] & 0x80);
			chkEarlyUnmute.Checked = Convert.ToBoolean(data[37] & 8);
			nudPretimeDelay.Value = data[38] * 25;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	private void VoteScanForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (chkChMark.Checked)
			{
				data[37] |= 16;
			}
			else
			{
				data[37] &= 239;
			}
			if (chkTalkback.Checked)
			{
				data[37] |= 128;
			}
			else
			{
				data[37] &= 127;
			}
			if (chkEarlyUnmute.Checked)
			{
				data[37] |= 8;
			}
			else
			{
				data[37] &= 247;
			}
			data[38] = (byte)(nudPretimeDelay.Value / 25m);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.chkTalkback = new System.Windows.Forms.CheckBox();
		this.chkChMark = new System.Windows.Forms.CheckBox();
		this.chkEarlyUnmute = new System.Windows.Forms.CheckBox();
		this.lblTxDesignatedCh = new System.Windows.Forms.Label();
		this.lblPretimeDelay = new System.Windows.Forms.Label();
		this.comTxDesignatedCh = new System.Windows.Forms.ComboBox();
		this.lstUnselected = new System.Windows.Forms.ListBox();
		this.lstSelected = new System.Windows.Forms.ListBox();
		this.btnAdd = new System.Windows.Forms.Button();
		this.button2 = new System.Windows.Forms.Button();
		this.nudPretimeDelay = new System.Windows.Forms.NumericUpDown();
		((System.ComponentModel.ISupportInitialize)this.nudPretimeDelay).BeginInit();
		base.SuspendLayout();
		this.chkTalkback.AutoSize = true;
		this.chkTalkback.Location = new System.Drawing.Point(249, 322);
		this.chkTalkback.Name = "chkTalkback";
		this.chkTalkback.Size = new System.Drawing.Size(82, 20);
		this.chkTalkback.TabIndex = 0;
		this.chkTalkback.Text = "Talkback";
		this.chkTalkback.UseVisualStyleBackColor = true;
		this.chkChMark.AutoSize = true;
		this.chkChMark.Location = new System.Drawing.Point(249, 352);
		this.chkChMark.Name = "chkChMark";
		this.chkChMark.Size = new System.Drawing.Size(128, 20);
		this.chkChMark.TabIndex = 1;
		this.chkChMark.Text = "Channel Marker";
		this.chkChMark.UseVisualStyleBackColor = true;
		this.chkEarlyUnmute.AutoSize = true;
		this.chkEarlyUnmute.Location = new System.Drawing.Point(249, 442);
		this.chkEarlyUnmute.Name = "chkEarlyUnmute";
		this.chkEarlyUnmute.Size = new System.Drawing.Size(111, 20);
		this.chkEarlyUnmute.TabIndex = 1;
		this.chkEarlyUnmute.Text = "Early Unmute";
		this.chkEarlyUnmute.UseVisualStyleBackColor = true;
		this.lblTxDesignatedCh.Location = new System.Drawing.Point(83, 382);
		this.lblTxDesignatedCh.Name = "lblTxDesignatedCh";
		this.lblTxDesignatedCh.Size = new System.Drawing.Size(156, 24);
		this.lblTxDesignatedCh.TabIndex = 2;
		this.lblTxDesignatedCh.Text = "Tx Designated Channel";
		this.lblTxDesignatedCh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblPretimeDelay.Location = new System.Drawing.Point(83, 412);
		this.lblPretimeDelay.Name = "lblPretimeDelay";
		this.lblPretimeDelay.Size = new System.Drawing.Size(156, 24);
		this.lblPretimeDelay.TabIndex = 2;
		this.lblPretimeDelay.Text = "Pretime Delay [ms]";
		this.lblPretimeDelay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.comTxDesignatedCh.FormattingEnabled = true;
		this.comTxDesignatedCh.Location = new System.Drawing.Point(249, 382);
		this.comTxDesignatedCh.Name = "comTxDesignatedCh";
		this.comTxDesignatedCh.Size = new System.Drawing.Size(121, 24);
		this.comTxDesignatedCh.TabIndex = 3;
		this.lstUnselected.FormattingEnabled = true;
		this.lstUnselected.ItemHeight = 16;
		this.lstUnselected.Location = new System.Drawing.Point(63, 49);
		this.lstUnselected.Name = "lstUnselected";
		this.lstUnselected.Size = new System.Drawing.Size(120, 244);
		this.lstUnselected.TabIndex = 4;
		this.lstSelected.FormattingEnabled = true;
		this.lstSelected.ItemHeight = 16;
		this.lstSelected.Location = new System.Drawing.Point(348, 49);
		this.lstSelected.Name = "lstSelected";
		this.lstSelected.Size = new System.Drawing.Size(120, 244);
		this.lstSelected.TabIndex = 4;
		this.btnAdd.Location = new System.Drawing.Point(224, 92);
		this.btnAdd.Name = "btnAdd";
		this.btnAdd.Size = new System.Drawing.Size(75, 23);
		this.btnAdd.TabIndex = 5;
		this.btnAdd.Text = "Add";
		this.btnAdd.UseVisualStyleBackColor = true;
		this.button2.Location = new System.Drawing.Point(224, 122);
		this.button2.Name = "button2";
		this.button2.Size = new System.Drawing.Size(75, 23);
		this.button2.TabIndex = 5;
		this.button2.Text = "Delete";
		this.button2.UseVisualStyleBackColor = true;
		this.nudPretimeDelay.Increment = new decimal(new int[4] { 25, 0, 0, 0 });
		this.nudPretimeDelay.Location = new System.Drawing.Point(249, 412);
		this.nudPretimeDelay.Maximum = new decimal(new int[4] { 1500, 0, 0, 0 });
		this.nudPretimeDelay.Name = "nudPretimeDelay";
		this.nudPretimeDelay.Size = new System.Drawing.Size(120, 23);
		this.nudPretimeDelay.TabIndex = 6;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.ClientSize = new System.Drawing.Size(531, 507);
		base.Controls.Add(this.nudPretimeDelay);
		base.Controls.Add(this.button2);
		base.Controls.Add(this.btnAdd);
		base.Controls.Add(this.lstSelected);
		base.Controls.Add(this.lstUnselected);
		base.Controls.Add(this.comTxDesignatedCh);
		base.Controls.Add(this.lblPretimeDelay);
		base.Controls.Add(this.lblTxDesignatedCh);
		base.Controls.Add(this.chkEarlyUnmute);
		base.Controls.Add(this.chkChMark);
		base.Controls.Add(this.chkTalkback);
		this.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular);
		base.Name = "VoteScanForm";
		this.Text = "Vote Scan";
		base.Load += new System.EventHandler(VoteScanForm_Load);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(VoteScanForm_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.nudPretimeDelay).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
