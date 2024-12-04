using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DMR;

public class CommPortSelector : Form
{
	public string SelectedPort;

	private IContainer components;

	private ComboBox cmbPorts;

	private Button btnSelect;

	private Button btnCancel;

	private void CommPortSelector_Load(object sender, EventArgs e)
	{
		try
		{
			Settings.UpdateComponentTextsFromLanguageXmlData(this);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	private void CommPortSelector_Shown(object sender, EventArgs e)
	{
		Focus();
	}

	public CommPortSelector()
	{
		InitializeComponent();
		foreach (string comPortName in SetupDiWrap.GetComPortNames())
		{
			cmbPorts.Items.Add(comPortName);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void btnSelect_Click(object sender, EventArgs e)
	{
		if (cmbPorts.SelectedItem != null)
		{
			SelectedPort = cmbPorts.SelectedItem.ToString();
			base.DialogResult = DialogResult.OK;
		}
		Close();
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
		this.cmbPorts = new System.Windows.Forms.ComboBox();
		this.btnSelect = new System.Windows.Forms.Button();
		this.btnCancel = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.cmbPorts.FormattingEnabled = true;
		this.cmbPorts.Location = new System.Drawing.Point(12, 12);
		this.cmbPorts.Name = "cmbPorts";
		this.cmbPorts.Size = new System.Drawing.Size(219, 21);
		this.cmbPorts.TabIndex = 0;
		this.btnSelect.Location = new System.Drawing.Point(156, 39);
		this.btnSelect.Name = "btnSelect";
		this.btnSelect.Size = new System.Drawing.Size(75, 23);
		this.btnSelect.TabIndex = 1;
		this.btnSelect.Text = "Select port";
		this.btnSelect.UseVisualStyleBackColor = true;
		this.btnSelect.Click += new System.EventHandler(btnSelect_Click);
		this.btnCancel.Location = new System.Drawing.Point(75, 39);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 1;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.UseVisualStyleBackColor = true;
		this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(242, 73);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnSelect);
		base.Controls.Add(this.cmbPorts);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "CommPortSelector";
		this.Text = "Select OpenGD77 com port";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		base.Load += new System.EventHandler(CommPortSelector_Load);
		base.Shown += new System.EventHandler(CommPortSelector_Shown);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
