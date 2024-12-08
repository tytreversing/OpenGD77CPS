using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class DtmfContactForm : DockContent, IDisp
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct DtmfContactOne
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		private byte[] name;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		private byte[] code;

		public string Name
		{
			get
			{
				return Settings.bufferToString(name);
			}
			set
			{
				byte[] array = Settings.stringToBuffer(value);
				name.Fill(byte.MaxValue);
				Array.Copy(array, 0, name, 0, Math.Min(array.Length, name.Length));
			}
		}

		public string Code
		{
			get
			{
				int num = 0;
				StringBuilder stringBuilder = new StringBuilder(16);
				for (num = 0; num < 16 && code[num] < 16; num++)
				{
					stringBuilder.Append("0123456789ABCD*#\b"[code[num]]);
				}
				return stringBuilder.ToString();
			}
			set
			{
				int num = 0;
				int num2 = 0;
				for (num = 0; num < 16; num++)
				{
					code[num] = byte.MaxValue;
				}
				for (num = 0; num < value.Length; num++)
				{
					num2 = "0123456789ABCD*#\b".IndexOf(value[num]);
					if (num2 >= 0)
					{
						code[num] = Convert.ToByte(num2);
						continue;
					}
					break;
				}
			}
		}

		public bool Valid => !string.IsNullOrEmpty(Name);

		public DtmfContactOne(int index)
		{
			name = new byte[16];
			code = new byte[16];
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class DtmfContact
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 63)]
		private DtmfContactOne[] contact;

		public DtmfContactOne this[int index]
		{
			get
			{
				if (index >= 63)
				{
					throw new ArgumentOutOfRangeException();
				}
				return contact[index];
			}
			set
			{
				if (index < 63)
				{
					contact[index] = value;
				}
			}
		}

		public string GetName(int index)
		{
			if (index < 63)
			{
				return this[index].Name;
			}
			return "";
		}

		public void SetName(int index, string name)
		{
			if (index < 63)
			{
				contact[index].Name = name;
			}
		}

		public string GetCode(int index)
		{
			if (index < 63)
			{
				return this[index].Code;
			}
			return "";
		}

		public void SetCode(int index, string code)
		{
			if (index < 63)
			{
				contact[index].Code = code;
			}
		}

		public void RemoveAt(int index)
		{
			if (index < 63)
			{
				contact[index].Name = "";
				contact[index].Code = "";
			}
		}

		public void Insert(int index, string name, string code)
		{
			if (index < 63)
			{
				contact[index].Name = name;
				contact[index].Code = code;
			}
		}

		public void Clear()
		{
			int num = 0;
			for (num = 0; num < 63; num++)
			{
				RemoveAt(num);
			}
		}

		public bool Valid(int index)
		{
			if (index < 63)
			{
				return this[index].Valid;
			}
			return false;
		}

		public DtmfContact()
		{
			int num = 0;
			contact = new DtmfContactOne[63];
			for (num = 0; num < 63; num++)
			{
				contact[num] = new DtmfContactOne(num);
			}
		}
	}

	public const int CNT_DTMF_CONTACT = 63;

	private const int LEN_DTMF_NAME = 16;

	private const int LEN_DTMF_CODE = 16;

	public const string SZ_DTMF_CODE = "0123456789ABCD*#\b";

	public static DtmfContact data;

	private DataGridView dgvContact;

	private Button btnDel;

	private Button btnAdd;

	private MyDataGridViewTextBoxColumn txtName;

	private DataGridViewTextBoxColumn txtCode;

	private CustomPanel pnlDtmfContact;

	public TreeNode Node { get; set; }

	public void SaveData()
	{
		try
		{
			int num = 0;
			int num2 = 0;
			dgvContact.EndEdit();
			data.Clear();
			for (num = 0; num < dgvContact.Rows.Count; num++)
			{
				num2 = Convert.ToInt32(dgvContact.Rows[num].Tag);
				data.SetName(num2, dgvContact.Rows[num].Cells[0].Value.ToString());
				data.SetCode(num2, dgvContact.Rows[num].Cells[1].Value.ToString());
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void DispData()
	{
		try
		{
			int i = 0;
			int num = 0;
			dgvContact.Rows.Clear();
			for (; i < 63; i++)
			{
				if (data[i].Valid)
				{
					num = dgvContact.Rows.Add();
					dgvContact.Rows[num].Tag = i;
					dgvContact.Rows[num].Cells[0].Value = data.GetName(i);
					dgvContact.Rows[num].Cells[1].Value = data.GetCode(i);
				}
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void RefreshName()
	{
	}

	public DtmfContactForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	private void method_0()
	{
		txtName.MaxByteLength = 15;
	}

	private void DtmfContactForm_Load(object sender, EventArgs e)
	{
		Settings.smethod_59(base.Controls);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		method_0();
		DispData();
	}

	private void DtmfContactForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveData();
	}

	private void sscOhyqVop(object sender, EventArgs e)
	{
		int num = 0;
		int num2 = 0;
		for (num2 = 0; num2 < dgvContact.Rows.Count && num == (int)dgvContact.Rows[num2].Tag; num2++)
		{
			num++;
		}
		dgvContact.Rows.Insert(num2, 1);
		dgvContact.Rows[num2].Tag = num;
		dgvContact.Rows[num2].Cells[0].Value = "DTMF-" + (num + 1);
		dgvContact.Rows[num2].Cells[1].Value = "12345678";
		method_1();
	}

	private void btnDel_Click(object sender, EventArgs e)
	{
		int index = dgvContact.CurrentRow.Index;
		int contactIndex = (int)dgvContact.Rows[index].Tag;
		dgvContact.Rows.RemoveAt(index);
		ButtonForm.data1.ClearByDtmfContact(contactIndex);
		method_1();
	}

	private void method_1()
	{
		int count = dgvContact.Rows.Count;
		btnAdd.Enabled = count < 63;
		btnDel.Enabled = count != 0;
	}

	private void method_2(object sender, DataGridViewRowPostPaintEventArgs e)
	{
		if (e.RowIndex >= dgvContact.FirstDisplayedScrollingRowIndex)
		{
			using (SolidBrush brush = new SolidBrush(dgvContact.RowHeadersDefaultCellStyle.ForeColor))
			{
				string s = (Convert.ToInt32(dgvContact.Rows[e.RowIndex].Tag) + 1).ToString();
				e.Graphics.DrawString(s, e.InheritedRowStyle.Font, brush, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + 5);
			}
		}
	}

	private void dgvContact_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
	{
		DataGridView dataGridView = (DataGridView)sender;
		if (e.Control is DataGridViewTextBoxEditingControl)
		{
			DataGridViewTextBoxEditingControl dataGridViewTextBoxEditingControl = (DataGridViewTextBoxEditingControl)e.Control;
			if (dataGridView.CurrentCell.ColumnIndex == 1)
			{
				dataGridViewTextBoxEditingControl.KeyPress -= Settings.smethod_57;
				dataGridViewTextBoxEditingControl.KeyPress += Settings.smethod_57;
				dataGridViewTextBoxEditingControl.CharacterCasing = CharacterCasing.Upper;
				dataGridViewTextBoxEditingControl.MaxLength = 16;
			}
			else
			{
				_ = dataGridView.CurrentCell.ColumnIndex;
			}
		}
	}

	private void dgvContact_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
	{
		DataGridView dataGridView = (DataGridView)sender;
		if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
		{
			e.Cancel = true;
			dataGridView.CancelEdit();
		}
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
		this.dgvContact = new System.Windows.Forms.DataGridView();
		this.txtName = new DMR.MyDataGridViewTextBoxColumn();
		this.txtCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.btnDel = new System.Windows.Forms.Button();
		this.btnAdd = new System.Windows.Forms.Button();
		this.pnlDtmfContact = new CustomPanel();
		((System.ComponentModel.ISupportInitialize)this.dgvContact).BeginInit();
		this.pnlDtmfContact.SuspendLayout();
		base.SuspendLayout();
		this.dgvContact.AllowUserToAddRows = false;
		dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
		dataGridViewCellStyle.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
		dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvContact.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
		this.dgvContact.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvContact.Columns.AddRange(this.txtName, this.txtCode);
		this.dgvContact.Location = new System.Drawing.Point(30, 69);
		this.dgvContact.Name = "dgvContact";
		this.dgvContact.RowTemplate.Height = 23;
		this.dgvContact.Size = new System.Drawing.Size(394, 401);
		this.dgvContact.TabIndex = 2;
		this.dgvContact.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(dgvContact_CellValidating);
		this.dgvContact.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(dgvContact_EditingControlShowing);
		this.txtName.HeaderText = "Name";
		this.txtName.MaxByteLength = int.MaxValue;
		this.txtName.Name = "txtName";
		this.txtName.Width = 175;
		this.txtCode.HeaderText = "Number";
		this.txtCode.Name = "txtCode";
		this.txtCode.Width = 175;
		this.btnDel.Location = new System.Drawing.Point(251, 28);
		this.btnDel.Name = "btnDel";
		this.btnDel.Size = new System.Drawing.Size(75, 23);
		this.btnDel.TabIndex = 1;
		this.btnDel.Text = "Delete";
		this.btnDel.UseVisualStyleBackColor = true;
		this.btnDel.Click += new System.EventHandler(btnDel_Click);
		this.btnAdd.Location = new System.Drawing.Point(126, 28);
		this.btnAdd.Name = "btnAdd";
		this.btnAdd.Size = new System.Drawing.Size(75, 23);
		this.btnAdd.TabIndex = 0;
		this.btnAdd.Text = "Add";
		this.btnAdd.UseVisualStyleBackColor = true;
		this.btnAdd.Click += new System.EventHandler(sscOhyqVop);
		this.pnlDtmfContact.AutoScroll = true;
		this.pnlDtmfContact.AutoSize = true;
		this.pnlDtmfContact.Controls.Add(this.dgvContact);
		this.pnlDtmfContact.Controls.Add(this.btnAdd);
		this.pnlDtmfContact.Controls.Add(this.btnDel);
		this.pnlDtmfContact.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnlDtmfContact.Location = new System.Drawing.Point(0, 0);
		this.pnlDtmfContact.Name = "pnlDtmfContact";
		this.pnlDtmfContact.Size = new System.Drawing.Size(454, 498);
		this.pnlDtmfContact.TabIndex = 3;
		base.ClientSize = new System.Drawing.Size(454, 498);
		base.Controls.Add(this.pnlDtmfContact);
		this.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "DtmfContactForm";
		this.Text = "DTMF Contact";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(DtmfContactForm_FormClosing);
		base.Load += new System.EventHandler(DtmfContactForm_Load);
		((System.ComponentModel.ISupportInitialize)this.dgvContact).EndInit();
		this.pnlDtmfContact.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	static DtmfContactForm()
	{
		data = new DtmfContact();
	}
}
