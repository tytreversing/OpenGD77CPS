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
            this.SuspendLayout();
            // 
            // cmbPorts
            // 
            this.cmbPorts.FormattingEnabled = true;
            this.cmbPorts.Location = new System.Drawing.Point(12, 12);
            this.cmbPorts.Name = "cmbPorts";
            this.cmbPorts.Size = new System.Drawing.Size(219, 21);
            this.cmbPorts.TabIndex = 0;
            // 
            // btnSelect
            // 
            this.btnSelect.BackColor = System.Drawing.SystemColors.Control;
            this.btnSelect.Location = new System.Drawing.Point(156, 39);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "Select port";
            this.btnSelect.UseVisualStyleBackColor = false;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Location = new System.Drawing.Point(75, 39);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // CommPortSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(242, 73);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.cmbPorts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CommPortSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select OpenGD77 com port";
            this.Load += new System.EventHandler(this.CommPortSelector_Load);
            this.Shown += new System.EventHandler(this.CommPortSelector_Shown);
            this.ResumeLayout(false);

	}
}
