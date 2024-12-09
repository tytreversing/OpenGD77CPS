using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class ChannelsForm : DockContent, IDisp, ISingleRow
{
	public const string SZ_HEADER_TEXT_NAME = "HeaderText";

	private const int SCL_FREQ = 100000;

	private static readonly string[] SZ_DISPLAY_HEADER_TEXT;

	private static readonly string[] SZ_EXPORT_HEADER_TEXT;

	private Panel pnlChannel;

	private DataGridView dgvChannels;

	private Button btnAdd;

	private CustomCombo cmbAddChMode;

	private SGTextBox txtRxFreq;

	private SGTextBox txtName;

	private ComboBox cmbPower;

	private ComboBox cmbChMode;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;

	private SGTextBox txtTxFreq;

	private Button btnDeleteSelect;

	public TreeNode Node { get; set; }

	public void SaveData()
	{
	}

	public void DispData()
	{
		try
		{
			dgvChannels.Rows.Clear();
			for (int i = 0; i < ChannelForm.data.Count; i++)
			{
				if (ChannelForm.data.DataIsValid(i))
				{
					ChannelForm.ChannelOne channelOne = ChannelForm.data[i];
					int index = dgvChannels.Rows.Add((i + 1).ToString(), channelOne.Name, channelOne.ChModeS, channelOne.RxFreq, channelOne.TxFreq, channelOne.TxColor.ToString(), channelOne.RepeaterSlotS, channelOne.ContactString, channelOne.RxGroupListString, channelOne.LibreDMR_PowerStr, channelOne.RxTone, channelOne.TxTone);
					dgvChannels.Rows[index].Tag = i;
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		dgvChannels.CurrentCell = null;
	}

	public void RefreshName()
	{
	}

	public ChannelsForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	public static void RefreshCommonLang()
	{
		string name = typeof(ChannelsForm).Name;
		Settings.smethod_78("DisplayHeaderText", SZ_DISPLAY_HEADER_TEXT, name);
		Settings.smethod_78("ExportHeaderText", SZ_EXPORT_HEADER_TEXT, name);
		SZ_EXPORT_HEADER_TEXT[SZ_EXPORT_HEADER_TEXT.Length - 1] = "Optional DMRID";
	}

	private void ChannelsForm_Load(object sender, EventArgs e)
	{
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		method_1();
		DispData();
		cmbAddChMode.SelectedIndex = 0;
	}

	private void btnAdd_Click(object sender, EventArgs e)
	{
		handleInsertClick();
	}

	private int AddDigitalContact(string contactName, int contactType, string contactID)
	{
		int num = -1;
		if (contactName != Settings.SZ_NONE)
		{
			num = ContactForm.data.GetMinIndex();
			if (num != -1)
			{
				ContactForm.data.SetIndex(num, 0);
				ContactForm.data.Default(num);
				ContactForm.data.SetName(num, contactName);
				ContactForm.data.SetCallType(num, contactType);
				ContactForm.data.SetCallID(num, contactID);
			}
		}
		return num + 1;
	}

	private void btnDelete_Click(object sender, EventArgs e)
	{
		int index = dgvChannels.CurrentRow.Index;
		int index2 = (int)dgvChannels.CurrentRow.Tag;
		if (index == 0)
		{
			MessageBox.Show(Settings.dicCommon["FirstNotDelete"]);
			return;
		}
		dgvChannels.Rows.Remove(dgvChannels.CurrentRow);
		ChannelForm.data.ClearIndexAndReset(index2);
		updateAddAndDeleteButtons();
		MainForm obj = base.MdiParent as MainForm;
		obj.DeleteTreeViewNode(Node, index);
		obj.RefreshRelatedForm(GetType());
	}

	private void btnClear_Click(object sender, EventArgs e)
	{
		int index = 1;
		int num = 0;
		MainForm mainForm = base.MdiParent as MainForm;
		while (dgvChannels.RowCount > 1)
		{
			num = (int)dgvChannels.Rows[1].Tag;
			dgvChannels.Rows.RemoveAt(1);
			Node.Nodes.RemoveAt(index);
			ChannelForm.data.ClearIndexAndReset(num);
		}
		updateAddAndDeleteButtons();
		mainForm.RefreshRelatedForm(GetType());
	}

	private void btnDeleteSelected_Click(object sender, EventArgs e)
	{
		handleDeleteClick();
	}

	private void updateAddAndDeleteButtons()
	{
		btnAdd.Enabled = dgvChannels.RowCount < ChannelForm.data.Count;
	}

	private void method_1()
	{
		int num = 0;
		int[] array = new int[12]
		{
			80, 100, 80, 80, 80, 80, 80, 80, 80, 100,
			100, 100
		};
		dgvChannels.ReadOnly = true;
		dgvChannels.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		dgvChannels.AllowUserToAddRows = false;
		dgvChannels.AllowUserToDeleteRows = false;
		dgvChannels.AllowUserToResizeRows = false;
		dgvChannels.AllowUserToOrderColumns = false;
		dgvChannels.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
		DataGridViewTextBoxColumn dataGridViewTextBoxColumn = null;
		string[] sZ_DISPLAY_HEADER_TEXT = SZ_DISPLAY_HEADER_TEXT;
		for (int i = 0; i < array.Length; i++)
		{
			dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
			dataGridViewTextBoxColumn.HeaderText = sZ_DISPLAY_HEADER_TEXT[i];
			dataGridViewTextBoxColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewTextBoxColumn.ReadOnly = true;
			dataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
			dataGridViewTextBoxColumn.Width = array[num++];
			dgvChannels.Columns.Add(dataGridViewTextBoxColumn);
		}
		Settings.fillComboBox(cmbAddChMode, ChannelForm.SZ_CH_MODE);
		Settings.fillComboBox(cmbChMode, ChannelForm.SZ_CH_MODE);
		Settings.fillComboBox(cmbPower, ChannelForm.SZ_POWER);
		txtName.MaxLength = 16;
		txtRxFreq.MaxLength = 9;
		txtTxFreq.MaxLength = 9;
	}

	private void NligzloMrR(object sender, DataGridViewRowPostPaintEventArgs e)
	{
		try
		{
			DataGridView dataGridView = sender as DataGridView;
			if (e.RowIndex >= dataGridView.FirstDisplayedScrollingRowIndex)
			{
				using (SolidBrush brush = new SolidBrush(dataGridView.RowHeadersDefaultCellStyle.ForeColor))
				{
					string s = (e.RowIndex + 1).ToString();
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

	private void dgvChannels_SelectionChanged(object sender, EventArgs e)
	{
		updateAddAndDeleteButtons();
	}

	private void dgvChannels_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
	{
		if (base.MdiParent is MainForm mainForm)
		{
			int index = (int)(sender as DataGridView).Rows[e.RowIndex].Tag;
			mainForm.DispChildForm(typeof(ChannelForm), index);
		}
	}

	private void cmbChMode_Leave(object sender, EventArgs e)
	{
		int index = dgvChannels.CurrentRow.Index;
		int index2 = (int)dgvChannels.CurrentRow.Tag;
		int selectedIndex = cmbChMode.SelectedIndex;
		string text = cmbChMode.Text;
		cmbChMode.Visible = false;
		if (!(cmbChMode.Text == ChannelForm.data[index2].ChModeS))
		{
			int[] array = new int[3] { 2, 6, 54 };
			dgvChannels.CurrentCell.Value = text;
			ChannelForm.data.SetChMode(index2, text);
			Node.Nodes[index].ImageIndex = array[selectedIndex];
			Node.Nodes[index].SelectedImageIndex = array[selectedIndex];
			((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
		}
	}

	private void txtName_Leave(object sender, EventArgs e)
	{
		int index = (int)dgvChannels.CurrentRow.Tag;
		int index2 = dgvChannels.CurrentRow.Index;
		txtName.Visible = false;
		string text = txtName.Text;
		if (!(text == ChannelForm.data[index].Name) && !ChannelForm.data.NameExist(text))
		{
			dgvChannels.CurrentCell.Value = text;
			ChannelForm.data.SetName(index, text);
			Node.Nodes[index2].Text = text;
			((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
		}
	}

	private void CaeqgYciuW(object sender, EventArgs e)
	{
		int index = (int)dgvChannels.CurrentRow.Tag;
		txtRxFreq.Visible = false;
		double num = 0.0;
		string s = txtRxFreq.Text;
		try
		{
			uint uint_ = 0u;
			num = double.Parse(s);
			if (Settings.checkFrequecyIsInValidRange(num, ref uint_) < 0)
			{
				return;
			}
			num = Settings.convert10HzStepFreqToDecimalText(Settings.convertDecimalFreqTo10HzStepValue(num, 100000.0), 100000);
			s = $"{num:f5}";
		}
		catch
		{
			return;
		}
		dgvChannels.CurrentCell.Value = s;
		ChannelForm.data.SetRxFreq(index, s);
		if (ChannelForm.data.FreqIsSameRange(index) < 0)
		{
			ChannelForm.data.SetTxFreq(index, s);
			dgvChannels.CurrentRow.Cells[3].Value = s;
		}
		((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
	}

	private void txtTxFreq_Leave(object sender, EventArgs e)
	{
		int index = (int)dgvChannels.CurrentRow.Tag;
		txtTxFreq.Visible = false;
		double num = 0.0;
		string s = txtTxFreq.Text;
		try
		{
			uint uint_ = 0u;
			num = double.Parse(s);
			if (Settings.checkFrequecyIsInValidRange(num, ref uint_) < 0)
			{
				return;
			}
			num = Settings.convert10HzStepFreqToDecimalText(Settings.convertDecimalFreqTo10HzStepValue(num, 100000.0), 100000);
			s = $"{num:f5}";
		}
		catch
		{
			return;
		}
		dgvChannels.CurrentCell.Value = s;
		ChannelForm.data.SetTxFreq(index, s);
		if (ChannelForm.data.FreqIsSameRange(index) < 0)
		{
			ChannelForm.data.SetRxFreq(index, s);
			dgvChannels.CurrentRow.Cells[2].Value = s;
		}
		((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
	}

	private void cmbPower_Leave(object sender, EventArgs e)
	{
		int index = (int)dgvChannels.CurrentRow.Tag;
		cmbPower.Visible = false;
		dgvChannels.CurrentCell.Value = cmbPower.Text;
		ChannelForm.data.SetPower(index, cmbPower.Text);
		((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
	}

	public void RefreshSingleRow(int index)
	{
		ChannelForm.ChannelOne channelOne = ChannelForm.data[index];
		int index2 = 0;
		foreach (DataGridViewRow item in (IEnumerable)dgvChannels.Rows)
		{
			if (Convert.ToInt32(item.Tag) == index)
			{
				index2 = item.Index;
				break;
			}
		}
		dgvChannels.Rows[index2].Cells[1].Value = channelOne.Name;
		dgvChannels.Rows[index2].Cells[2].Value = channelOne.ChModeS;
		dgvChannels.Rows[index2].Cells[3].Value = channelOne.RxFreq;
		dgvChannels.Rows[index2].Cells[4].Value = channelOne.TxFreq;
		dgvChannels.Rows[index2].Cells[5].Value = channelOne.TxColor.ToString();
		dgvChannels.Rows[index2].Cells[6].Value = channelOne.RepeaterSlotS;
		dgvChannels.Rows[index2].Cells[7].Value = channelOne.ContactString;
		dgvChannels.Rows[index2].Cells[8].Value = channelOne.RxGroupListString;
		dgvChannels.Rows[index2].Cells[9].Value = channelOne.LibreDMR_PowerStr;
		dgvChannels.Rows[index2].Cells[10].Value = channelOne.RxTone;
		dgvChannels.Rows[index2].Cells[11].Value = channelOne.TxTone;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
            this.pnlChannel = new System.Windows.Forms.Panel();
            this.btnDeleteSelect = new System.Windows.Forms.Button();
            this.txtTxFreq = new DMR.SGTextBox();
            this.txtRxFreq = new DMR.SGTextBox();
            this.txtName = new DMR.SGTextBox();
            this.cmbPower = new System.Windows.Forms.ComboBox();
            this.cmbChMode = new System.Windows.Forms.ComboBox();
            this.cmbAddChMode = new CustomCombo();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dgvChannels = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlChannel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChannels)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlChannel
            // 
            this.pnlChannel.AutoScroll = true;
            this.pnlChannel.AutoSize = true;
            this.pnlChannel.Controls.Add(this.btnDeleteSelect);
            this.pnlChannel.Controls.Add(this.txtTxFreq);
            this.pnlChannel.Controls.Add(this.txtRxFreq);
            this.pnlChannel.Controls.Add(this.txtName);
            this.pnlChannel.Controls.Add(this.cmbPower);
            this.pnlChannel.Controls.Add(this.cmbChMode);
            this.pnlChannel.Controls.Add(this.cmbAddChMode);
            this.pnlChannel.Controls.Add(this.btnAdd);
            this.pnlChannel.Controls.Add(this.dgvChannels);
            this.pnlChannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChannel.Location = new System.Drawing.Point(0, 0);
            this.pnlChannel.Name = "pnlChannel";
            this.pnlChannel.Size = new System.Drawing.Size(1136, 531);
            this.pnlChannel.TabIndex = 0;
            // 
            // btnDeleteSelect
            // 
            this.btnDeleteSelect.BackColor = System.Drawing.SystemColors.Control;
            this.btnDeleteSelect.Location = new System.Drawing.Point(269, 10);
            this.btnDeleteSelect.Name = "btnDeleteSelect";
            this.btnDeleteSelect.Size = new System.Drawing.Size(167, 23);
            this.btnDeleteSelect.TabIndex = 12;
            this.btnDeleteSelect.Text = "Delete Selected";
            this.btnDeleteSelect.UseVisualStyleBackColor = false;
            this.btnDeleteSelect.Click += new System.EventHandler(this.btnDeleteSelected_Click);
            // 
            // txtTxFreq
            // 
            this.txtTxFreq.InputString = null;
            this.txtTxFreq.Location = new System.Drawing.Point(927, 13);
            this.txtTxFreq.MaxByteLength = 0;
            this.txtTxFreq.Name = "txtTxFreq";
            this.txtTxFreq.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtTxFreq.Size = new System.Drawing.Size(61, 23);
            this.txtTxFreq.TabIndex = 6;
            this.txtTxFreq.Visible = false;
            this.txtTxFreq.Leave += new System.EventHandler(this.txtTxFreq_Leave);
            // 
            // txtRxFreq
            // 
            this.txtRxFreq.InputString = null;
            this.txtRxFreq.Location = new System.Drawing.Point(860, 12);
            this.txtRxFreq.MaxByteLength = 0;
            this.txtRxFreq.Name = "txtRxFreq";
            this.txtRxFreq.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtRxFreq.Size = new System.Drawing.Size(61, 23);
            this.txtRxFreq.TabIndex = 6;
            this.txtRxFreq.Visible = false;
            this.txtRxFreq.Leave += new System.EventHandler(this.CaeqgYciuW);
            // 
            // txtName
            // 
            this.txtName.InputString = null;
            this.txtName.Location = new System.Drawing.Point(793, 12);
            this.txtName.MaxByteLength = 0;
            this.txtName.Name = "txtName";
            this.txtName.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtName.Size = new System.Drawing.Size(61, 23);
            this.txtName.TabIndex = 5;
            this.txtName.Visible = false;
            this.txtName.Leave += new System.EventHandler(this.txtName_Leave);
            // 
            // cmbPower
            // 
            this.cmbPower.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPower.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbPower.FormattingEnabled = true;
            this.cmbPower.Location = new System.Drawing.Point(1061, 12);
            this.cmbPower.Name = "cmbPower";
            this.cmbPower.Size = new System.Drawing.Size(61, 24);
            this.cmbPower.TabIndex = 8;
            this.cmbPower.Visible = false;
            this.cmbPower.Leave += new System.EventHandler(this.cmbPower_Leave);
            // 
            // cmbChMode
            // 
            this.cmbChMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbChMode.FormattingEnabled = true;
            this.cmbChMode.Location = new System.Drawing.Point(994, 12);
            this.cmbChMode.Name = "cmbChMode";
            this.cmbChMode.Size = new System.Drawing.Size(61, 24);
            this.cmbChMode.TabIndex = 7;
            this.cmbChMode.Visible = false;
            this.cmbChMode.Leave += new System.EventHandler(this.cmbChMode_Leave);
            // 
            // cmbAddChMode
            // 
            this.cmbAddChMode.BackColor = System.Drawing.Color.White;
            this.cmbAddChMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAddChMode.FormattingEnabled = true;
            this.cmbAddChMode.Location = new System.Drawing.Point(12, 10);
            this.cmbAddChMode.Name = "cmbAddChMode";
            this.cmbAddChMode.Size = new System.Drawing.Size(109, 24);
            this.cmbAddChMode.TabIndex = 0;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.Control;
            this.btnAdd.Location = new System.Drawing.Point(139, 11);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(124, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // dgvChannels
            // 
            this.dgvChannels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvChannels.BackgroundColor = System.Drawing.Color.White;
            this.dgvChannels.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChannels.Location = new System.Drawing.Point(12, 42);
            this.dgvChannels.Name = "dgvChannels";
            this.dgvChannels.ReadOnly = true;
            this.dgvChannels.RowHeadersWidth = 50;
            this.dgvChannels.RowTemplate.Height = 23;
            this.dgvChannels.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChannels.Size = new System.Drawing.Size(1110, 457);
            this.dgvChannels.TabIndex = 9;
            this.dgvChannels.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvChannels_RowHeaderMouseDoubleClick);
            this.dgvChannels.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.NligzloMrR);
            this.dgvChannels.SelectionChanged += new System.EventHandler(this.dgvChannels_SelectionChanged);
            this.dgvChannels.Enter += new System.EventHandler(this.dgvChannels_Enter);
            this.dgvChannels.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvChannels_KeyPress);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Column2";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Column3";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Column4";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Column5";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // ChannelsForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1136, 531);
            this.Controls.Add(this.pnlChannel);
            this.Font = new System.Drawing.Font("Arial", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChannelsForm";
            this.Text = "Channels";
            this.Load += new System.EventHandler(this.ChannelsForm_Load);
            this.pnlChannel.ResumeLayout(false);
            this.pnlChannel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChannels)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

	}

	[CompilerGenerated]
	private static bool smethod_0(string string_0)
	{
		return true;
	}

	static ChannelsForm()
	{
		SZ_DISPLAY_HEADER_TEXT = new string[35];
		SZ_EXPORT_HEADER_TEXT = new string[37];
	}

	private void dgvChannels_Enter(object sender, EventArgs e)
	{
	}

	private void dgvChannels_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar != ' ')
		{
			return;
		}
		MainForm mainForm = base.MdiParent as MainForm;
		IEnumerator enumerator = (sender as DataGridView).SelectedRows.GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				int index = (int)((DataGridViewRow)enumerator.Current).Tag;
				mainForm.DispChildForm(typeof(ChannelForm), index);
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}

	private void handleDeleteClick()
	{
		int num = 0;
		int num2 = 0;
		int count = dgvChannels.SelectedRows.Count;
		MainForm mainForm = base.MdiParent as MainForm;
		while (dgvChannels.SelectedRows.Count > 0)
		{
			_ = dgvChannels.SelectedRows[0].Index;
			num = (int)dgvChannels.SelectedRows[0].Tag;
			dgvChannels.Rows.Remove(dgvChannels.SelectedRows[0]);
			ChannelForm.data.ClearIndexAndReset(num);
			num2++;
			if (num2 == count)
			{
				break;
			}
		}
		updateAddAndDeleteButtons();
		mainForm.RefreshRelatedForm(GetType());
		mainForm.InitTree();
		OpenGD77Form.ClearLastUsedChannelsData();
	}

	private void handleInsertClick()
	{
		int selectedIndex = cmbAddChMode.SelectedIndex;
		int minIndex = ChannelForm.data.GetMinIndex();
		MainForm mainForm = base.MdiParent as MainForm;
		string minName = ChannelForm.data.GetMinName(Node);
		string chMode = cmbAddChMode.Text;
		ChannelForm.data.SetIndex(minIndex, 1);
		ChannelForm.data.SetChName(minIndex, minName);
		ChannelForm.data.SetDefaultFreq(minIndex);
		ChannelForm.data.Default(minIndex);
		ChannelForm.data.SetChMode(minIndex, chMode);
		ChannelForm.ChannelOne channelOne = ChannelForm.data[minIndex];
		dgvChannels.Rows.Insert(minIndex, (minIndex + 1).ToString(), channelOne.Name, channelOne.RxFreq, channelOne.TxFreq, channelOne.ChModeS, channelOne.LibreDMR_PowerStr, channelOne.RxTone, channelOne.TxTone, channelOne.TxColor.ToString(), channelOne.RxGroupListString, channelOne.ContactString, channelOne.RepeaterSlotS);
		dgvChannels.Rows[minIndex].Tag = minIndex;
		updateAddAndDeleteButtons();
		int[] array = new int[3] { 2, 6, 54 };
		mainForm.InsertTreeViewNode(Node, minIndex, typeof(ChannelForm), array[selectedIndex], ChannelForm.data);
		mainForm.RefreshRelatedForm(GetType());
		mainForm.DispChildForm(typeof(ChannelForm), minIndex);
		OpenGD77Form.ClearLastUsedChannelsData();
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Insert | Keys.Control:
		case Keys.I | Keys.Control:
			handleInsertClick();
			return true;
		case Keys.Delete | Keys.Control:
			handleDeleteClick();
			return true;
		case Keys.D | Keys.Control:
		{
			DataGridViewRow dataGridViewRow3 = dgvChannels.SelectedRows[0];
			if (dataGridViewRow3.Index == dgvChannels.Rows.Count - 1)
			{
				return true;
			}
			DataGridViewRow dataGridViewRow4 = dgvChannels.Rows[dataGridViewRow3.Index + 1];
			int index2 = dataGridViewRow4.Index;
			ChannelForm.MoveChannelDown((int)dataGridViewRow3.Tag, (int)dataGridViewRow4.Tag);
			MainForm obj2 = base.MdiParent as MainForm;
			obj2.RefreshRelatedForm(GetType());
			obj2.InitTree();
			dgvChannels.Rows.Clear();
			DispData();
			dgvChannels.Rows[index2].Selected = true;
			dgvChannels.FirstDisplayedScrollingRowIndex = index2;
			dgvChannels.CurrentCell = dgvChannels[1, index2];
			return true;
		}
		case Keys.U | Keys.Control:
		{
			DataGridViewRow dataGridViewRow = dgvChannels.SelectedRows[0];
			if (dataGridViewRow.Index == 0)
			{
				return true;
			}
			DataGridViewRow dataGridViewRow2 = dgvChannels.Rows[dataGridViewRow.Index - 1];
			int index = dataGridViewRow2.Index;
			ChannelForm.MoveChannelUp((int)dataGridViewRow.Tag, (int)dataGridViewRow2.Tag);
			MainForm obj = base.MdiParent as MainForm;
			obj.RefreshRelatedForm(GetType());
			obj.InitTree();
			dgvChannels.Rows.Clear();
			DispData();
			dgvChannels.Rows[index].Selected = true;
			dgvChannels.FirstDisplayedScrollingRowIndex = index;
			dgvChannels.CurrentCell = dgvChannels[1, index];
			return true;
		}
		default:
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
