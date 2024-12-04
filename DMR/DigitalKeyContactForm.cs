using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class DigitalKeyContactForm : DockContent, IDisp
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class NumKeyContact
	{
		private ushort index;

		private ushort reserve;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		private ushort[] contact;

		public ushort this[int index]
		{
			get
			{
				return contact[index];
			}
			set
			{
				method_0(index, value);
				contact[index] = value;
			}
		}

		private void method_0(int int_0, int int_1)
		{
			ushort num = (ushort)(~(1 << int_0));
			index &= num;
			if (int_1 != 0)
			{
				index |= (ushort)(1 << int_0);
			}
		}

		private bool method_1(int int_0)
		{
			try
			{
				return new BitArray(BitConverter.GetBytes(index))[int_0];
			}
			catch
			{
				return false;
			}
		}

		public NumKeyContact()
		{
			contact = new ushort[10];
		}

		public void Verify()
		{
			int num = 0;
			int num2 = 0;
			for (num = 0; num < contact.Length; num++)
			{
				if (method_1(num))
				{
					num2 = contact[num] - 1;
					if (!ContactForm.data.DataIsValid(num2))
					{
						contact[num] = 0;
						Settings.smethod_17(ref index, num, 1);
					}
				}
			}
		}
	}

	private const int CNT_NUM_KEY_CONTACT = 10;

	public const string SZ_DIGIT_KEY_NAME = "DigitKey";

	private static string SZ_DIGIT_KEY_TEXT;

	public static NumKeyContact data;

	private DataGridViewComboBoxColumn cmbContact;

	private DataGridView dgvContact;

	public TreeNode Node { get; set; }

	public void SaveData()
	{
		try
		{
			int num = 0;
			dgvContact.EndEdit();
			for (num = 0; num < dgvContact.Rows.Count; num++)
			{
				if (dgvContact.Rows[num].Cells[0].Value != null)
				{
					data[num] = (ushort)(int)dgvContact.Rows[num].Cells[0].Value;
				}
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void DispData()
	{
		method_0();
		try
		{
			int num = 0;
			for (num = 0; num < dgvContact.Rows.Count; num++)
			{
				dgvContact.Rows[num].Cells[0].Value = (int)data[num];
			}
			dgvContact.EndEdit();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public void RefreshName()
	{
	}

	public DigitalKeyContactForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	private void method_0()
	{
		int num = 0;
		string text = "";
		dgvContact.RowCount = 10;
		cmbContact.Items.Clear();
		cmbContact.Items.Add(new NameValuePair(Settings.SZ_NONE, 0));
		for (num = 0; num < 1024; num++)
		{
			if (ContactForm.data.DataIsValid(num))
			{
				text = ContactForm.data[num].Name;
				cmbContact.Items.Add(new NameValuePair(text, num + 1));
			}
		}
		cmbContact.DisplayMember = "Text";
		cmbContact.ValueMember = "Value";
		for (num = 0; num < dgvContact.Rows.Count; num++)
		{
			dgvContact.Rows[num].HeaderCell.Value = SZ_DIGIT_KEY_TEXT + num;
		}
	}

	public static void RefreshCommonLang()
	{
		string name = typeof(DigitalKeyContactForm).Name;
		Settings.smethod_77("DigitKey", ref SZ_DIGIT_KEY_TEXT, name);
	}

	private void DigitalKeyContactForm_Load(object sender, EventArgs e)
	{
		Settings.smethod_59(base.Controls);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		DispData();
	}

	private void DigitalKeyContactForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveData();
	}

	private void method_1(object sender, DataGridViewRowPostPaintEventArgs e)
	{
		try
		{
			DataGridView dataGridView = sender as DataGridView;
			if (e.RowIndex >= dataGridView.FirstDisplayedScrollingRowIndex)
			{
				using (SolidBrush brush = new SolidBrush(dataGridView.RowHeadersDefaultCellStyle.ForeColor))
				{
					string s = SZ_DIGIT_KEY_TEXT + e.RowIndex;
					e.Graphics.DrawString(s, e.InheritedRowStyle.Font, brush, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + 5);
					return;
				}
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	private void dgvContact_DataError(object sender, DataGridViewDataErrorEventArgs e)
	{
		DataGridView dataGridView = sender as DataGridView;
		if (e.Context == DataGridViewDataErrorContexts.Formatting && dataGridView != null)
		{
			dataGridView[e.ColumnIndex, e.RowIndex].Value = 0;
			e.Cancel = true;
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
		this.cmbContact = new System.Windows.Forms.DataGridViewComboBoxColumn();
		((System.ComponentModel.ISupportInitialize)this.dgvContact).BeginInit();
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
		this.dgvContact.Columns.AddRange(this.cmbContact);
		this.dgvContact.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
		this.dgvContact.Location = new System.Drawing.Point(38, 12);
		this.dgvContact.Name = "dgvContact";
		this.dgvContact.RowHeadersWidth = 150;
		this.dgvContact.RowTemplate.Height = 23;
		this.dgvContact.Size = new System.Drawing.Size(456, 289);
		this.dgvContact.TabIndex = 16;
		this.cmbContact.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
		this.cmbContact.HeaderText = "Menu";
		this.cmbContact.Name = "cmbContact";
		this.cmbContact.Width = 300;
		base.ClientSize = new System.Drawing.Size(570, 345);
		base.Controls.Add(this.dgvContact);
		this.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.Name = "DigitalKeyContactForm";
		this.Text = "Number Key Quick Contact Access";
		((System.ComponentModel.ISupportInitialize)this.dgvContact).EndInit();
		base.ResumeLayout(false);
	}

	static DigitalKeyContactForm()
	{
		SZ_DIGIT_KEY_TEXT = "Number Key";
		data = new NumKeyContact();
	}
}
