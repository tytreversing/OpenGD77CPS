using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class EncryptForm : DockContent, IDisp
{
	private enum EncryptType
	{
		None,
		Basic,
		Enhanced
	}

	private enum KeyLen
	{
		Length32,
		Length64,
		Length40
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class Encrypt : IVerify<Encrypt>, IData
	{
		private byte type;

		private byte keyLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		private byte[] keyIndex;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		private byte[] reserve;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		private byte[] keyList;

		public int Type
		{
			get
			{
				return type;
			}
			set
			{
				type = Convert.ToByte(value);
			}
		}

		public int KeyLen
		{
			get
			{
				return keyLen;
			}
			set
			{
				keyLen = Convert.ToByte(value);
			}
		}

		public bool this[int index]
		{
			get
			{
				if (index < 16)
				{
					return new BitArray(keyIndex)[index];
				}
				return false;
			}
			set
			{
				if (index < 16)
				{
					BitArray bitArray = new BitArray(keyIndex);
					bitArray[index] = value;
					bitArray.CopyTo(keyIndex, 0);
				}
			}
		}

		public int Count => 16;

		public string Format => "";

		public bool ListIsEmpty
		{
			get
			{
				int num = 0;
				while (true)
				{
					if (num < Count)
					{
						if (DataIsValid(num))
						{
							break;
						}
						num++;
						continue;
					}
					return true;
				}
				return false;
			}
		}

		public Encrypt()
		{
			keyIndex = new byte[2];
			reserve = new byte[4];
			keyList = new byte[128];
		}

		public void RemoveAt(int index)
		{
			int i = 0;
			this[index] = false;
			for (; i < 8; i++)
			{
				keyList[i + index * 8] = byte.MaxValue;
			}
		}

		public void Insert(int index)
		{
			int i = 0;
			this[index] = true;
			for (; i < 8; i++)
			{
				keyList[i + index * 8] = Convert.ToByte("53474C3953474C39".Substring(i * 2, 2));
			}
		}

		public void Clear()
		{
			for (int i = 0; i < keyIndex.Length; i++)
			{
				keyIndex[i] = 0;
			}
			for (int j = 0; j < keyList.Length; j++)
			{
				keyList[j] = byte.MaxValue;
			}
		}

		public string GetKey(int index)
		{
			if (index >= 16)
			{
				return "";
			}
			int i = 0;
			StringBuilder stringBuilder = new StringBuilder(16);
			for (; i < 8; i++)
			{
				byte b = keyList[i + index * 8];
				stringBuilder.Append(b.ToString("X2"));
			}
			if (data.KeyLen == 0)
			{
				return stringBuilder.ToString().Substring(0, 8);
			}
			return stringBuilder.ToString();
		}

		public void SetKey(int index, string key)
		{
			if (index < 16)
			{
				for (int i = 0; i < key.Length / 2; i++)
				{
					keyList[i + index * 8] = Convert.ToByte(key.Substring(i * 2, 2), 16);
				}
				if (key.Length == 8)
				{
					Array.Copy(keyList, index * 8, keyList, index * 8 + 4, 4);
				}
			}
		}

		public int GetMinIndex()
		{
			int num = 0;
			for (num = 0; num < Count; num++)
			{
			}
			return -1;
		}

		public bool DataIsValid(int index)
		{
			if (index < 16)
			{
				return new BitArray(keyIndex)[index];
			}
			return false;
		}

		public void SetIndex(int index, int value)
		{
			if (value == 0)
			{
				SetName(index, "");
			}
		}

		public void ClearIndex(int index)
		{
			SetName(index, "");
		}

		public string GetMinName(TreeNode node)
		{
			return "";
		}

		public void SetName(int index, string text)
		{
		}

		public string GetName(int index)
		{
			return GetKey(index);
		}

		public void Default(int index)
		{
		}

		public void Paste(int from, int to)
		{
		}

		public void Verify(Encrypt def)
		{
			if (!Enum.IsDefined(typeof(EncryptType), (int)type))
			{
				type = def.type;
			}
			if (!Enum.IsDefined(typeof(KeyLen), (int)keyLen))
			{
				keyLen = def.keyLen;
			}
		}
	}

	private const int CNT_KEY = 16;

	private const int CNT_KEY_INDEX = 2;

	private const int SPACE_PER_KEY = 8;

	private const int LEN_KEY_32BIT = 8;

	private const int LEN_KEY_64BIT = 16;

	public const string SZ_ENCRYPT_TYPE_NAME = "EncryptType";

	private const string DEF_KEY = "53474C3953474C39";

	private static readonly string[] SZ_ENCRYPT_TYPE;

	public static Encrypt DefaultEncrypt;

	public static Encrypt data;

	private Label lblType;

	private ComboBox cmbType;

	private Label lblKeyLen;

	private ComboBox cmbKeyLen;

	private DataGridView dgvKey;

	private Button btnDel;

	private Button btnAdd;

	private DataGridViewTextBoxColumn txtKey;

	private CustomPanel pnlEncrypt;

	private int _003CPreKeyLen_003Ek__BackingField;

	public TreeNode Node { get; set; }

	public EncryptForm()
	{
		InitializeComponent();
		Scale(Settings.smethod_6());
	}

	[CompilerGenerated]
	private int method_0()
	{
		return _003CPreKeyLen_003Ek__BackingField;
	}

	[CompilerGenerated]
	private void method_1(int value)
	{
		_003CPreKeyLen_003Ek__BackingField = value;
	}

	public void SaveData()
	{
		int i = 0;
		int num = 0;
		string text = null;
		data.Type = (byte)cmbType.SelectedIndex;
		data.KeyLen = (byte)cmbKeyLen.SelectedIndex;
		dgvKey.EndEdit();
		data.Clear();
		for (; i < dgvKey.Rows.Count; i++)
		{
			num = (int)dgvKey.Rows[i].Tag;
			data[num] = true;
			text = dgvKey.Rows[i].Cells[0].Value.ToString();
			data.SetKey(num, text);
		}
	}

	public void DispData()
	{
		int num = 0;
		int num2 = 0;
		cmbType.SelectedIndex = data.Type;
		cmbKeyLen.SelectedIndex = data.KeyLen;
		num2 = ((cmbKeyLen.SelectedIndex != 1) ? 8 : 16);
		txtKey.MaxInputLength = num2;
		dgvKey.Rows.Clear();
		for (num = 0; num < 16; num++)
		{
			if (data[num])
			{
				int index = dgvKey.Rows.Add();
				dgvKey.Rows[index].Tag = num;
				dgvKey.Rows[index].Cells[0].Value = data.GetKey(num).Substring(0, num2);
			}
		}
		method_3();
		RefreshByUserMode();
	}

	public void RefreshByUserMode()
	{
		bool flag = Settings.getUserExpertSettings() == Settings.UserMode.Expert;
		lblType.Enabled &= flag;
		cmbType.Enabled &= flag;
		lblKeyLen.Enabled &= flag;
		cmbKeyLen.Enabled &= flag;
		btnAdd.Enabled &= flag;
		btnDel.Enabled &= flag;
		dgvKey.Enabled &= flag;
	}

	public void RefreshName()
	{
	}

	private void method_2()
	{
		DataGridViewTextBoxColumn obj = dgvKey.Columns[0] as DataGridViewTextBoxColumn;
		obj.MaxInputLength = 8;
		obj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		obj.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		Settings.fillComboBox(cmbType, SZ_ENCRYPT_TYPE);
	}

	public static void RefreshCommonLang()
	{
		string name = typeof(EncryptForm).Name;
		Settings.smethod_78("EncryptType", SZ_ENCRYPT_TYPE, name);
	}

	private void EncryptForm_Load(object sender, EventArgs e)
	{
		try
		{
			Settings.smethod_59(base.Controls);
			Settings.UpdateComponentTextsFromLanguageXmlData(this);
			method_2();
			DispData();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	private void EncryptForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			SaveData();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	private void btnAdd_Click(object sender, EventArgs e)
	{
		int num = 0;
		int num2 = 0;
		string text = null;
		for (num = 0; num < dgvKey.Rows.Count && num2 == (int)dgvKey.Rows[num].Tag; num++)
		{
			num2++;
		}
		dgvKey.Rows.Insert(num, 1);
		dgvKey.Rows[num].Tag = num2;
		text = ((cmbKeyLen.SelectedIndex != 0) ? "53474C3953474C39" : "53474C3953474C39".Substring(8));
		dgvKey.Rows[num].Cells[0].Value = text;
		SaveData();
		method_3();
		((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
	}

	private void btnDel_Click(object sender, EventArgs e)
	{
		int index = dgvKey.CurrentRow.Index;
		int keyIndex = (int)dgvKey.Rows[index].Tag;
		dgvKey.Rows.RemoveAt(index);
		ChannelForm.data.ClearByEncrypt(keyIndex);
		method_3();
		((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
	}

	private void method_3()
	{
		int selectedIndex = cmbType.SelectedIndex;
		int count = dgvKey.Rows.Count;
		btnAdd.Enabled = true;
		btnDel.Enabled = true;
		if (count == 16 || selectedIndex == 0)
		{
			btnAdd.Enabled = false;
		}
		if (count == 0 || selectedIndex == 0)
		{
			btnDel.Enabled = false;
		}
	}

	private void cmbKeyLen_SelectedIndexChanged(object sender, EventArgs e)
	{
		int num = 0;
		int selectedIndex = cmbKeyLen.SelectedIndex;
		if (method_0() == selectedIndex)
		{
			return;
		}
		method_1(selectedIndex);
		if (selectedIndex == 0)
		{
			txtKey.MaxInputLength = 8;
			for (num = 0; num < dgvKey.Rows.Count; num++)
			{
				string text = dgvKey.Rows[num].Cells[0].Value as string;
				text = text.Substring(0, 8);
				dgvKey.Rows[num].Cells[0].Value = text;
			}
		}
		else
		{
			txtKey.MaxInputLength = 16;
			for (num = 0; num < dgvKey.Rows.Count; num++)
			{
				string text2 = dgvKey.Rows[num].Cells[0].Value as string;
				text2 += text2;
				dgvKey.Rows[num].Cells[0].Value = text2;
			}
		}
	}

	private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (cmbType.SelectedIndex == 0)
		{
			cmbKeyLen.Enabled = false;
			dgvKey.Enabled = false;
			btnAdd.Enabled = false;
			btnDel.Enabled = false;
		}
		else
		{
			cmbKeyLen.Enabled = true;
			dgvKey.Enabled = true;
			btnAdd.Enabled = true;
			btnDel.Enabled = true;
			method_3();
		}
	}

	private void dgvKey_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
	{
		if (e.RowIndex >= dgvKey.FirstDisplayedScrollingRowIndex)
		{
			using (SolidBrush brush = new SolidBrush(dgvKey.RowHeadersDefaultCellStyle.ForeColor))
			{
				string s = (Convert.ToInt32(dgvKey.Rows[e.RowIndex].Tag) + 1).ToString();
				e.Graphics.DrawString(s, e.InheritedRowStyle.Font, brush, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + 5);
			}
		}
	}

	private void cmbKeyLen_Enter(object sender, EventArgs e)
	{
		method_1(cmbKeyLen.SelectedIndex);
	}

	private void dgvKey_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
	{
		if (e.Control is DataGridViewTextBoxEditingControl)
		{
			DataGridViewTextBoxEditingControl obj = (DataGridViewTextBoxEditingControl)e.Control;
			obj.KeyPress -= Settings.smethod_58;
			obj.KeyPress += Settings.smethod_58;
			obj.CharacterCasing = CharacterCasing.Upper;
		}
	}

	private void dgvKey_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
	{
		DataGridView dataGridView = (DataGridView)sender;
		if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
		{
			e.Cancel = true;
			dataGridView.CancelEdit();
			dataGridView.EndEdit();
		}
	}

	private void dgvKey_CellValidated(object sender, DataGridViewCellEventArgs e)
	{
		DataGridView dataGridView = (DataGridView)sender;
		int maxInputLength = ((DataGridViewTextBoxColumn)dgvKey.Columns[e.ColumnIndex]).MaxInputLength;
		object value = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
		if (value != null)
		{
			string text = value.ToString();
			if (text.Length < maxInputLength)
			{
				text = text.PadRight(maxInputLength, 'F');
				dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = text;
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.lblType = new System.Windows.Forms.Label();
		this.cmbType = new System.Windows.Forms.ComboBox();
		this.lblKeyLen = new System.Windows.Forms.Label();
		this.cmbKeyLen = new System.Windows.Forms.ComboBox();
		this.dgvKey = new System.Windows.Forms.DataGridView();
		this.txtKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.btnDel = new System.Windows.Forms.Button();
		this.btnAdd = new System.Windows.Forms.Button();
		this.pnlEncrypt = new CustomPanel();
		((System.ComponentModel.ISupportInitialize)this.dgvKey).BeginInit();
		this.pnlEncrypt.SuspendLayout();
		base.SuspendLayout();
		this.lblType.Location = new System.Drawing.Point(45, 41);
		this.lblType.Name = "lblType";
		this.lblType.Size = new System.Drawing.Size(109, 24);
		this.lblType.TabIndex = 0;
		this.lblType.Text = "Privacy Type";
		this.lblType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbType.FormattingEnabled = true;
		this.cmbType.Items.AddRange(new object[3] { "None", "Basic", "Enhanced" });
		this.cmbType.Location = new System.Drawing.Point(168, 41);
		this.cmbType.Name = "cmbType";
		this.cmbType.Size = new System.Drawing.Size(121, 24);
		this.cmbType.TabIndex = 1;
		this.cmbType.SelectedIndexChanged += new System.EventHandler(cmbType_SelectedIndexChanged);
		this.lblKeyLen.Location = new System.Drawing.Point(45, 71);
		this.lblKeyLen.Name = "lblKeyLen";
		this.lblKeyLen.Size = new System.Drawing.Size(109, 24);
		this.lblKeyLen.TabIndex = 2;
		this.lblKeyLen.Text = "Key Length";
		this.lblKeyLen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.cmbKeyLen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbKeyLen.FormattingEnabled = true;
		this.cmbKeyLen.Items.AddRange(new object[3] { "32", "64", "40" });
		this.cmbKeyLen.Location = new System.Drawing.Point(168, 71);
		this.cmbKeyLen.Name = "cmbKeyLen";
		this.cmbKeyLen.Size = new System.Drawing.Size(121, 24);
		this.cmbKeyLen.TabIndex = 3;
		this.cmbKeyLen.SelectedIndexChanged += new System.EventHandler(cmbKeyLen_SelectedIndexChanged);
		this.cmbKeyLen.Enter += new System.EventHandler(cmbKeyLen_Enter);
		this.dgvKey.AllowUserToAddRows = false;
		this.dgvKey.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvKey.Columns.AddRange(this.txtKey);
		this.dgvKey.Location = new System.Drawing.Point(75, 145);
		this.dgvKey.Name = "dgvKey";
		this.dgvKey.RowTemplate.Height = 23;
		this.dgvKey.Size = new System.Drawing.Size(240, 353);
		this.dgvKey.TabIndex = 6;
		this.dgvKey.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(dgvKey_CellValidated);
		this.dgvKey.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(dgvKey_RowPostPaint);
		this.dgvKey.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(dgvKey_CellValidating);
		this.dgvKey.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(dgvKey_EditingControlShowing);
		this.txtKey.HeaderText = "Key";
		this.txtKey.Name = "txtKey";
		this.btnDel.Location = new System.Drawing.Point(220, 107);
		this.btnDel.Name = "btnDel";
		this.btnDel.Size = new System.Drawing.Size(75, 23);
		this.btnDel.TabIndex = 5;
		this.btnDel.Text = "Delete";
		this.btnDel.UseVisualStyleBackColor = true;
		this.btnDel.Click += new System.EventHandler(btnDel_Click);
		this.btnAdd.Location = new System.Drawing.Point(95, 107);
		this.btnAdd.Name = "btnAdd";
		this.btnAdd.Size = new System.Drawing.Size(75, 23);
		this.btnAdd.TabIndex = 4;
		this.btnAdd.Text = "Add";
		this.btnAdd.UseVisualStyleBackColor = true;
		this.btnAdd.Click += new System.EventHandler(btnAdd_Click);
		this.pnlEncrypt.AutoScroll = true;
		this.pnlEncrypt.AutoSize = true;
		this.pnlEncrypt.Controls.Add(this.cmbType);
		this.pnlEncrypt.Controls.Add(this.dgvKey);
		this.pnlEncrypt.Controls.Add(this.lblType);
		this.pnlEncrypt.Controls.Add(this.btnDel);
		this.pnlEncrypt.Controls.Add(this.lblKeyLen);
		this.pnlEncrypt.Controls.Add(this.btnAdd);
		this.pnlEncrypt.Controls.Add(this.cmbKeyLen);
		this.pnlEncrypt.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnlEncrypt.Location = new System.Drawing.Point(0, 0);
		this.pnlEncrypt.Name = "pnlEncrypt";
		this.pnlEncrypt.Size = new System.Drawing.Size(390, 539);
		this.pnlEncrypt.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.ClientSize = new System.Drawing.Size(390, 539);
		base.Controls.Add(this.pnlEncrypt);
		this.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.Name = "EncryptForm";
		this.Text = "Privacy Setting";
		base.Load += new System.EventHandler(EncryptForm_Load);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(EncryptForm_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.dgvKey).EndInit();
		this.pnlEncrypt.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	static EncryptForm()
	{
		SZ_ENCRYPT_TYPE = new string[3] { "None", "Basic", "Ehnanced" };
		data = new Encrypt();
	}
}
