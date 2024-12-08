using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class TextMsgForm : DockContent, IDisp
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class TextMsg
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		private byte[] reserve;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		private byte[] msgLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		private byte[] reserve1;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4608)]
		private byte[] text;

		public byte[] Reserve => reserve;

		public int this[int index]
		{
			get
			{
				if (index >= 32)
				{
					throw new ArgumentOutOfRangeException();
				}
				return msgLen[index];
			}
			set
			{
				if (index < 32)
				{
					msgLen[index] = (byte)value;
				}
			}
		}

		public TextMsg()
		{
			msgLen = new byte[32];
			reserve = new byte[8];
			reserve1 = new byte[32];
			text = new byte[4608];
		}

		public void RemoveAt(int index)
		{
			if (index < 32)
			{
				int i = 0;
				this[index] = 0;
				for (; i < 144; i++)
				{
					text[i + index * 144] = byte.MaxValue;
				}
			}
		}

		public void Insert(int index)
		{
			if (index < 32)
			{
				int i = 0;
				this[index] = 1;
				for (; i < 144; i++)
				{
					text[i + index * 144] = byte.MaxValue;
				}
			}
		}

		public void Clear()
		{
			for (int i = 0; i < 32; i++)
			{
				this[i] = 0;
			}
			for (int j = 0; j < text.Length; j++)
			{
				text[j] = byte.MaxValue;
			}
		}

		public string GetText(int index)
		{
			if (index >= 32)
			{
				return "";
			}
			int num = 0;
			if (this[index] == 1)
			{
				return "";
			}
			byte[] array = new byte[this[index] - 1];
			Array.Copy(text, num + index * 144, array, 0, array.Length);
			return Settings.bufferToString(array);
		}

		public void SetText(int index, string msg)
		{
			if (index < 32)
			{
				RemoveAt(index);
				this[index] = msg.Length + 1;
				byte[] array = Settings.stringToBuffer(msg);
				this[index] = array.Length + 1;
				Array.Copy(array, 0, text, index * 144, Math.Min(array.Length, 144));
			}
		}
	}

	public const int CNT_MSG = 32;

	private const int LEN_MSG = 144;

	private Button btnAdd;

	private Button btnDel;

	private DataGridView dgvMsg;

	private DataGridViewTextBoxColumn txtMessage;

	private SGTextBox txtContent;

	private CustomPanel pnlTextMsg;

	private TextMsgFormCustomPasteWndProc txt;

	public static TextMsg data;

	public TreeNode Node { get; set; }

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
		this.btnAdd = new System.Windows.Forms.Button();
		this.btnDel = new System.Windows.Forms.Button();
		this.dgvMsg = new System.Windows.Forms.DataGridView();
		this.txtMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.pnlTextMsg = new CustomPanel();
		this.txtContent = new DMR.SGTextBox();
		((System.ComponentModel.ISupportInitialize)this.dgvMsg).BeginInit();
		this.pnlTextMsg.SuspendLayout();
		base.SuspendLayout();
		this.btnAdd.Location = new System.Drawing.Point(66, 41);
		this.btnAdd.Name = "btnAdd";
		this.btnAdd.Size = new System.Drawing.Size(75, 23);
		this.btnAdd.TabIndex = 0;
		this.btnAdd.Text = "Add";
		this.btnAdd.UseVisualStyleBackColor = true;
		this.btnAdd.Click += new System.EventHandler(btnAdd_Click);
		this.btnDel.Location = new System.Drawing.Point(191, 41);
		this.btnDel.Name = "btnDel";
		this.btnDel.Size = new System.Drawing.Size(75, 23);
		this.btnDel.TabIndex = 1;
		this.btnDel.Text = "Delete";
		this.btnDel.UseVisualStyleBackColor = true;
		this.btnDel.Click += new System.EventHandler(zesOxyCmfw);
		this.dgvMsg.AllowUserToAddRows = false;
		dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
		dataGridViewCellStyle.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular);
		dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
		dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dgvMsg.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
		this.dgvMsg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvMsg.Columns.AddRange(this.txtMessage);
		this.dgvMsg.Location = new System.Drawing.Point(48, 85);
		this.dgvMsg.Name = "dgvMsg";
		this.dgvMsg.RowTemplate.Height = 23;
		this.dgvMsg.Size = new System.Drawing.Size(700, 447);
		this.dgvMsg.TabIndex = 2;
		this.dgvMsg.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(dgvMsg_RowPostPaint);
		this.dgvMsg.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(dgvMsg_CellEndEdit);
		this.dgvMsg.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(dgvMsg_EditingControlShowing);
		this.txtMessage.HeaderText = "Text Message";
		this.txtMessage.MaxInputLength = 50;
		this.txtMessage.Name = "txtMessage";
		this.txtMessage.Width = 650;
		this.pnlTextMsg.AutoScroll = true;
		this.pnlTextMsg.AutoSize = true;
		this.pnlTextMsg.Controls.Add(this.txtContent);
		this.pnlTextMsg.Controls.Add(this.dgvMsg);
		this.pnlTextMsg.Controls.Add(this.btnAdd);
		this.pnlTextMsg.Controls.Add(this.btnDel);
		this.pnlTextMsg.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnlTextMsg.Location = new System.Drawing.Point(0, 0);
		this.pnlTextMsg.Name = "pnlTextMsg";
		this.pnlTextMsg.Size = new System.Drawing.Size(796, 573);
		this.pnlTextMsg.TabIndex = 3;
		this.txtContent.InputString = null;
		this.txtContent.Location = new System.Drawing.Point(349, 41);
		this.txtContent.MaxByteLength = 0;
		this.txtContent.Name = "txtContent";
		this.txtContent.Size = new System.Drawing.Size(100, 21);
		this.txtContent.TabIndex = 3;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.ClientSize = new System.Drawing.Size(796, 573);
		base.Controls.Add(this.pnlTextMsg);
		this.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular);
		base.Name = "TextMsgForm";
		this.Text = "Text Message";
		base.Load += new System.EventHandler(TextMsgForm_Load);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(TextMsgForm_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.dgvMsg).EndInit();
		this.pnlTextMsg.ResumeLayout(false);
		this.pnlTextMsg.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	public void SaveData()
	{
		try
		{
			int num = 0;
			int num2 = 0;
			string text = null;
			dgvMsg.EndEdit();
			for (num = 0; num < dgvMsg.Rows.Count; num++)
			{
				num2 = (int)dgvMsg.Rows[num].Tag;
				if (dgvMsg.Rows[num].Cells[0].Value != null)
				{
					text = dgvMsg.Rows[num].Cells[0].Value.ToString();
					if (string.IsNullOrEmpty(text))
					{
						data[num2] = 1;
					}
					else
					{
						data.SetText(num2, text);
					}
				}
				else
				{
					data[num2] = 1;
				}
			}
			data.Reserve[0] = (byte)dgvMsg.Rows.Count;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Source);
		}
	}

	public void DispData()
	{
		try
		{
			int num = 0;
			int num2 = 0;
			method_1();
			dgvMsg.Rows.Clear();
			for (num = 0; num < 32; num++)
			{
				if (data[num] == 1)
				{
					num2 = dgvMsg.Rows.Add();
					dgvMsg.Rows[num2].Tag = num;
					dgvMsg.Rows[num2].Cells[0].Value = "";
				}
				else if (data[num] > 1 && data[num] <= 145)
				{
					num2 = dgvMsg.Rows.Add();
					dgvMsg.Rows[num2].Tag = num;
					dgvMsg.Rows[num2].Cells[0].Value = data.GetText(num);
				}
			}
			method_2();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Source);
		}
	}

	public void RefreshName()
	{
	}

	public TextMsgForm()
	{
		txt = new TextMsgFormCustomPasteWndProc();
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	private void method_1()
	{
		txtMessage.MaxInputLength = 144;
		txtMessage.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
		dgvMsg.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
		dgvMsg.AllowUserToDeleteRows = false;
		dgvMsg.AllowUserToAddRows = false;
	}

	private void TextMsgForm_Load(object sender, EventArgs e)
	{
		Settings.smethod_59(base.Controls);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		txtContent.MaxByteLength = 10;
		txtContent.Visible = false;
		dgvMsg.Controls.Add(txtContent);
		DispData();
	}

	private void TextMsgForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveData();
	}

	private void btnAdd_Click(object sender, EventArgs e)
	{
		int num = 0;
		int num2 = 0;
		for (num = 0; num < dgvMsg.Rows.Count && num2 == (int)dgvMsg.Rows[num].Tag; num++)
		{
			num2++;
		}
		dgvMsg.Rows.Insert(num, 1);
		dgvMsg.Rows[num].Tag = num2;
		dgvMsg.Rows[num].Cells[0].Value = "";
		data[num2] = 0;
		data.SetText(num2, "");
		method_2();
		((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
	}

	private void zesOxyCmfw(object sender, EventArgs e)
	{
		int index = dgvMsg.CurrentRow.Index;
		int num = (int)dgvMsg.Rows[index].Tag;
		dgvMsg.Rows.RemoveAt(index);
		data[num] = 0;
		ButtonForm.data1.ClearByMessage(num);
		method_2();
		((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
	}

	private void method_2()
	{
		int count = dgvMsg.Rows.Count;
		btnAdd.Enabled = true;
		btnDel.Enabled = true;
		if (count == 32)
		{
			btnAdd.Enabled = false;
		}
		if (count == 0)
		{
			btnDel.Enabled = false;
		}
	}

	private void dgvMsg_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
	{
		if (e.RowIndex >= dgvMsg.FirstDisplayedScrollingRowIndex)
		{
			using (SolidBrush brush = new SolidBrush(dgvMsg.RowHeadersDefaultCellStyle.ForeColor))
			{
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				string s = ((int)dgvMsg.Rows[e.RowIndex].Tag + 1).ToString();
				Rectangle rectangle = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, dgvMsg.RowHeadersWidth, e.RowBounds.Height);
				e.Graphics.DrawString(s, e.InheritedRowStyle.Font, brush, rectangle, stringFormat);
			}
		}
	}

	private void dgvMsg_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
	{
		if (dgvMsg.CurrentCell.ColumnIndex == 0)
		{
			e.Control.KeyPress += Content_KeyPress;
			txt.AssignHandle(e.Control.Handle);
		}
	}

	private void dgvMsg_CellEndEdit(object sender, DataGridViewCellEventArgs e)
	{
		if (e.ColumnIndex == 0)
		{
			txt.ReleaseHandle();
		}
	}

	public static void Content_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (sender is TextBox textBox && !char.IsControl(e.KeyChar))
		{
			int byteCount = Encoding.GetEncoding(936).GetByteCount(textBox.Text + e.KeyChar);
			int byteCount2 = Encoding.GetEncoding(936).GetByteCount(textBox.SelectedText);
			if (byteCount - byteCount2 > 144)
			{
				e.Handled = true;
			}
		}
	}

	static TextMsgForm()
	{
		data = new TextMsg();
	}
}
