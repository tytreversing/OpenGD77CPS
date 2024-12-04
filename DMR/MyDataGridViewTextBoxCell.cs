using System;
using System.Windows.Forms;

namespace DMR;

public class MyDataGridViewTextBoxCell : DataGridViewTextBoxCell
{
	private int maxByteLength;

	public override Type EditType => typeof(MyDataGridViewTextBoxEditingControl);

	public int MaxByteLength
	{
		get
		{
			return maxByteLength;
		}
		set
		{
			maxByteLength = value;
			if (method_1(base.RowIndex))
			{
				method_0().MaxByteLength = value;
			}
		}
	}

	private MyDataGridViewTextBoxEditingControl method_0()
	{
		return base.DataGridView.EditingControl as MyDataGridViewTextBoxEditingControl;
	}

	private bool method_1(int int_0)
	{
		if (int_0 != -1 && base.DataGridView != null)
		{
			if (base.DataGridView.EditingControl is MyDataGridViewTextBoxEditingControl myDataGridViewTextBoxEditingControl)
			{
				return int_0 == ((IDataGridViewEditingControl)myDataGridViewTextBoxEditingControl).EditingControlRowIndex;
			}
			return false;
		}
		return false;
	}

	public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
	{
		base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
		if (base.DataGridView.EditingControl is MyDataGridViewTextBoxEditingControl myDataGridViewTextBoxEditingControl)
		{
			myDataGridViewTextBoxEditingControl.MaxByteLength = MaxByteLength;
		}
	}

	public override object Clone()
	{
		MyDataGridViewTextBoxCell myDataGridViewTextBoxCell = base.Clone() as MyDataGridViewTextBoxCell;
		if (myDataGridViewTextBoxCell != null)
		{
			myDataGridViewTextBoxCell.MaxByteLength = MaxByteLength;
		}
		return myDataGridViewTextBoxCell;
	}
}
