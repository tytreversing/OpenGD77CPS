using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DMR;

public class CalibrationPowerControl : UserControl
{
	private List<CalibrationNUDLabelControl> _controlsList = new List<CalibrationNUDLabelControl>();

	private int _rows = 1;

	private int _cols = 1;

	private IContainer components;

	private GroupBox groupBox1;

	public string[] Names
	{
		get
		{
			string[] array = new string[_controlsList.Count];
			for (int i = 0; i < _controlsList.Count; i++)
			{
				array[i] = _controlsList[i].bandName;
			}
			return array;
		}
		set
		{
			if (value.Length <= _rows * _cols)
			{
				for (int i = 0; i < value.Length; i++)
				{
					_controlsList[i].bandName = value[i];
				}
			}
		}
	}

	public int[] Values
	{
		get
		{
			int[] array = new int[_controlsList.Count];
			for (int i = 0; i < _controlsList.Count; i++)
			{
				array[i] = _controlsList[i].bandValue;
			}
			return array;
		}
		set
		{
			if (value.Length <= _rows * _cols)
			{
				for (int i = 0; i < value.Length; i++)
				{
					_controlsList[i].bandValue = (byte)value[i];
				}
			}
		}
	}

	public int Rows
	{
		get
		{
			return _rows;
		}
		set
		{
			_rows = value;
			updateComponents();
		}
	}

	public int Cols
	{
		get
		{
			return _cols;
		}
		set
		{
			_cols = value;
			updateComponents();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Bindable(true)]
	public override string Text
	{
		get
		{
			return groupBox1.Text;
		}
		set
		{
			groupBox1.Text = value;
		}
	}

	public string CtrlText
	{
		get
		{
			return groupBox1.Text;
		}
		set
		{
			groupBox1.Text = value;
		}
	}

	public CalibrationPowerControl()
	{
		InitializeComponent();
		updateComponents();
	}

	private void updateComponents()
	{
		if (_controlsList.Count != 0)
		{
			for (int i = 0; i < _controlsList.Count; i++)
			{
				groupBox1.Controls.Remove(_controlsList[i]);
			}
			_controlsList.Clear();
		}
		CalibrationNUDLabelControl calibrationNUDLabelControl = null;
		int num = groupBox1.Left + 10;
		int num2 = groupBox1.Top + 15;
		for (int j = 0; j < _rows; j++)
		{
			num = 5;
			for (int k = 0; k < _cols; k++)
			{
				calibrationNUDLabelControl = new CalibrationNUDLabelControl();
				calibrationNUDLabelControl.bandName = "label_" + j + "_" + k;
				calibrationNUDLabelControl.bandValue = 0;
				calibrationNUDLabelControl.Location = new Point(num, num2);
				calibrationNUDLabelControl.Name = "calibrationNUDLabelControl1";
				num += calibrationNUDLabelControl.Width;
				calibrationNUDLabelControl.TabIndex = j;
				_controlsList.Add(calibrationNUDLabelControl);
				groupBox1.Controls.Add(calibrationNUDLabelControl);
			}
			num2 += calibrationNUDLabelControl.Height;
		}
		groupBox1.Width = num + 5;
		groupBox1.Height = num2 + 15;
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
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		base.SuspendLayout();
		this.groupBox1.Location = new System.Drawing.Point(0, 4);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(219, 125);
		this.groupBox1.TabIndex = 0;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "groupBox1";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.groupBox1);
		base.Name = "CalibrationPowerControl";
		base.Size = new System.Drawing.Size(235, 136);
		base.ResumeLayout(false);
	}
}
