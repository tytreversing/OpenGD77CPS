using System;
using System.Windows.Forms;

namespace DMR;

public class MyDataGridViewTextBoxColumn : DataGridViewTextBoxColumn
{
	public int MaxByteLength
	{
		get
		{
			return method_0().MaxByteLength;
		}
		set
		{
			method_0().MaxByteLength = value;
			if (base.DataGridView == null)
			{
				return;
			}
			DataGridViewRowCollection rows = base.DataGridView.Rows;
			int count = rows.Count;
			for (int i = 0; i < count; i++)
			{
				if (rows.SharedRow(i).Cells[base.Index] is MyDataGridViewTextBoxCell myDataGridViewTextBoxCell)
				{
					myDataGridViewTextBoxCell.MaxByteLength = value;
				}
			}
			base.DataGridView.InvalidateColumn(base.Index);
		}
	}

	public MyDataGridViewTextBoxColumn()
	{
		CellTemplate = new MyDataGridViewTextBoxCell();
		MaxByteLength = int.MaxValue;
	}

	private MyDataGridViewTextBoxCell method_0()
	{
		return (CellTemplate as MyDataGridViewTextBoxCell) ?? throw new InvalidOperationException("Invalid CellTemplate.");
	}
}
