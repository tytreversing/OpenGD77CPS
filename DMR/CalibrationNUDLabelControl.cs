using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DMR;

public class CalibrationNUDLabelControl : UserControl
{
	private IContainer components;

	private Label label1;

	private NumericUpDown numericUpDown1;

	public string bandName
	{
		get
		{
			return label1.Text;
		}
		set
		{
			label1.Text = value;
		}
	}

	public byte bandValue
	{
		get
		{
			return (byte)numericUpDown1.Value;
		}
		set
		{
			numericUpDown1.Value = value;
		}
	}

	public CalibrationNUDLabelControl()
	{
		InitializeComponent();
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
		this.label1 = new System.Windows.Forms.Label();
		this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
		((System.ComponentModel.ISupportInitialize)this.numericUpDown1).BeginInit();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(0, 0);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(35, 13);
		this.label1.TabIndex = 0;
		this.label1.Text = "label1";
		this.numericUpDown1.Location = new System.Drawing.Point(0, 21);
		this.numericUpDown1.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.numericUpDown1.Name = "numericUpDown1";
		this.numericUpDown1.Size = new System.Drawing.Size(48, 20);
		this.numericUpDown1.TabIndex = 1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.numericUpDown1);
		base.Controls.Add(this.label1);
		base.Name = "CalibrationNUDLabelControl";
		base.Size = new System.Drawing.Size(50, 45);
		((System.ComponentModel.ISupportInitialize)this.numericUpDown1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
