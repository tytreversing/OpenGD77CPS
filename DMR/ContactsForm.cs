using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using ReadWriteCsv;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class ContactsForm : DockContent, IDisp, ISingleRow
{
	private const string SZ_HEADER_TEXT_NAME = "HeaderText";

	private Panel pnlContact;

	private DataGridView dgvContacts;

	private Button btnClear;

	private Button btnDelete;

	private Button btnAdd;

	private CustomCombo cmbAddType;

	private SGTextBox txtCallId;

	private SGTextBox txtName;

	private ComboBox cmbCallRxTone;

	private ComboBox cmbRingStyle;

	private ComboBox cmbType;

	private ComboBox cmbRepeaterSlot;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;

	private Button btnDeleteSelect;

	private Button btnImport;

	private Button btnExport;

	private Button btnInternetDownload;
    private Button btnSortContacts;
    private static readonly string[] SZ_HEADER_TEXT;

	public TreeNode Node { get; set; }

	public DataGridView getDataGridView()
	{
		return dgvContacts;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
            this.pnlContact = new System.Windows.Forms.Panel();
            this.btnSortContacts = new System.Windows.Forms.Button();
            this.btnInternetDownload = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnDeleteSelect = new System.Windows.Forms.Button();
            this.txtCallId = new DMR.SGTextBox();
            this.txtName = new DMR.SGTextBox();
            this.cmbCallRxTone = new System.Windows.Forms.ComboBox();
            this.cmbRingStyle = new System.Windows.Forms.ComboBox();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.cmbAddType = new CustomCombo();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dgvContacts = new System.Windows.Forms.DataGridView();
            this.cmbRepeaterSlot = new System.Windows.Forms.ComboBox();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlContact.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvContacts)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlContact
            // 
            this.pnlContact.AutoScroll = true;
            this.pnlContact.AutoSize = true;
            this.pnlContact.Controls.Add(this.btnSortContacts);
            this.pnlContact.Controls.Add(this.btnInternetDownload);
            this.pnlContact.Controls.Add(this.btnImport);
            this.pnlContact.Controls.Add(this.btnExport);
            this.pnlContact.Controls.Add(this.btnDeleteSelect);
            this.pnlContact.Controls.Add(this.txtCallId);
            this.pnlContact.Controls.Add(this.txtName);
            this.pnlContact.Controls.Add(this.cmbCallRxTone);
            this.pnlContact.Controls.Add(this.cmbRingStyle);
            this.pnlContact.Controls.Add(this.cmbType);
            this.pnlContact.Controls.Add(this.cmbAddType);
            this.pnlContact.Controls.Add(this.btnClear);
            this.pnlContact.Controls.Add(this.btnDelete);
            this.pnlContact.Controls.Add(this.btnAdd);
            this.pnlContact.Controls.Add(this.dgvContacts);
            this.pnlContact.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContact.Location = new System.Drawing.Point(0, 0);
            this.pnlContact.Name = "pnlContact";
            this.pnlContact.Size = new System.Drawing.Size(765, 524);
            this.pnlContact.TabIndex = 0;
            // 
            // btnSortContacts
            // 
            this.btnSortContacts.BackColor = System.Drawing.SystemColors.Control;
            this.btnSortContacts.Location = new System.Drawing.Point(264, 40);
            this.btnSortContacts.Name = "btnSortContacts";
            this.btnSortContacts.Size = new System.Drawing.Size(236, 23);
            this.btnSortContacts.TabIndex = 13;
            this.btnSortContacts.Text = "Sort by name";
            this.btnSortContacts.UseVisualStyleBackColor = false;
            // 
            // btnInternetDownload
            // 
            this.btnInternetDownload.Location = new System.Drawing.Point(375, 488);
            this.btnInternetDownload.Name = "btnInternetDownload";
            this.btnInternetDownload.Size = new System.Drawing.Size(156, 23);
            this.btnInternetDownload.TabIndex = 12;
            this.btnInternetDownload.Text = "Internet Download";
            this.btnInternetDownload.UseVisualStyleBackColor = true;
            this.btnInternetDownload.Visible = false;
            this.btnInternetDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(618, 488);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 11;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Visible = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(537, 488);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 11;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Visible = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnDeleteSelect
            // 
            this.btnDeleteSelect.BackColor = System.Drawing.SystemColors.Control;
            this.btnDeleteSelect.Location = new System.Drawing.Point(22, 40);
            this.btnDeleteSelect.Name = "btnDeleteSelect";
            this.btnDeleteSelect.Size = new System.Drawing.Size(236, 23);
            this.btnDeleteSelect.TabIndex = 10;
            this.btnDeleteSelect.Text = "Delete Selected";
            this.btnDeleteSelect.UseVisualStyleBackColor = false;
            this.btnDeleteSelect.Click += new System.EventHandler(this.btnDeleteSelect_Click);
            // 
            // txtCallId
            // 
            this.txtCallId.InputString = null;
            this.txtCallId.Location = new System.Drawing.Point(156, 486);
            this.txtCallId.MaxByteLength = 0;
            this.txtCallId.Name = "txtCallId";
            this.txtCallId.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtCallId.Size = new System.Drawing.Size(61, 23);
            this.txtCallId.TabIndex = 6;
            this.txtCallId.Visible = false;
            // 
            // txtName
            // 
            this.txtName.InputString = null;
            this.txtName.Location = new System.Drawing.Point(89, 487);
            this.txtName.MaxByteLength = 0;
            this.txtName.Name = "txtName";
            this.txtName.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtName.Size = new System.Drawing.Size(61, 23);
            this.txtName.TabIndex = 5;
            this.txtName.Visible = false;
            // 
            // cmbCallRxTone
            // 
            this.cmbCallRxTone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCallRxTone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCallRxTone.FormattingEnabled = true;
            this.cmbCallRxTone.Location = new System.Drawing.Point(308, 485);
            this.cmbCallRxTone.Name = "cmbCallRxTone";
            this.cmbCallRxTone.Size = new System.Drawing.Size(61, 24);
            this.cmbCallRxTone.TabIndex = 8;
            this.cmbCallRxTone.Visible = false;
            // 
            // cmbRingStyle
            // 
            this.cmbRingStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRingStyle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbRingStyle.FormattingEnabled = true;
            this.cmbRingStyle.Location = new System.Drawing.Point(223, 487);
            this.cmbRingStyle.Name = "cmbRingStyle";
            this.cmbRingStyle.Size = new System.Drawing.Size(61, 24);
            this.cmbRingStyle.TabIndex = 7;
            this.cmbRingStyle.Visible = false;
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(22, 486);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(61, 24);
            this.cmbType.TabIndex = 4;
            this.cmbType.Visible = false;
            // 
            // cmbAddType
            // 
            this.cmbAddType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAddType.FormattingEnabled = true;
            this.cmbAddType.Location = new System.Drawing.Point(22, 9);
            this.cmbAddType.Name = "cmbAddType";
            this.cmbAddType.Size = new System.Drawing.Size(112, 24);
            this.cmbAddType.TabIndex = 0;
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.SystemColors.Control;
            this.btnClear.Location = new System.Drawing.Point(471, 8);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(155, 23);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.Control;
            this.btnDelete.Location = new System.Drawing.Point(310, 8);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(155, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.Control;
            this.btnAdd.Location = new System.Drawing.Point(149, 8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(155, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // dgvContacts
            // 
            this.dgvContacts.BackgroundColor = System.Drawing.Color.White;
            this.dgvContacts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvContacts.Location = new System.Drawing.Point(22, 69);
            this.dgvContacts.Name = "dgvContacts";
            this.dgvContacts.ReadOnly = true;
            this.dgvContacts.RowHeadersWidth = 50;
            this.dgvContacts.RowTemplate.Height = 23;
            this.dgvContacts.Size = new System.Drawing.Size(713, 440);
            this.dgvContacts.TabIndex = 9;
            this.dgvContacts.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvContacts_RowHeaderMouseDoubleClick);
            this.dgvContacts.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvContacts_RowPostPaint);
            this.dgvContacts.SelectionChanged += new System.EventHandler(this.dgvContacts_SelectionChanged);
            this.dgvContacts.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvContacts_KeyPress);
            // 
            // cmbRepeaterSlot
            // 
            this.cmbRepeaterSlot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRepeaterSlot.FormattingEnabled = true;
            this.cmbRepeaterSlot.Location = new System.Drawing.Point(22, 43);
            this.cmbRepeaterSlot.Name = "cmbRepeaterSlot";
            this.cmbRepeaterSlot.Size = new System.Drawing.Size(112, 21);
            this.cmbRepeaterSlot.TabIndex = 0;
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
            // ContactsForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(765, 524);
            this.Controls.Add(this.pnlContact);
            this.Font = new System.Drawing.Font("Arial", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ContactsForm";
            this.Text = "Contacts";
            this.pnlContact.ResumeLayout(false);
            this.pnlContact.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvContacts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

	}

	public void SaveData()
	{
	}

	public void DispData()
	{
		try
		{
			dgvContacts.Rows.Clear();
			for (int i = 0; i < ContactForm.data.Count; i++)
			{
				if (ContactForm.data.DataIsValid(i))
				{
					string callTypeS = ContactForm.data[i].CallTypeS;
					string callId = ContactForm.data[i].CallId;
					string name = ContactForm.data[i].Name;
					string callRxToneS = ContactForm.data[i].CallRxToneS;
					string ringStyleS = ContactForm.data[i].RingStyleS;
					string repeaterSlotS = ContactForm.data[i].RepeaterSlotS;
					int index = dgvContacts.Rows.Add((i + 1).ToString(), name, callId, callTypeS, ringStyleS, callRxToneS, repeaterSlotS);
					dgvContacts.Rows[index].Tag = i;
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public void RefreshName()
	{
	}

	public void RefreshSingleRow(int index)
	{
		ContactForm.ContactOne contactOne = ContactForm.data[index];
		int index2 = 0;
		foreach (DataGridViewRow item in (IEnumerable)dgvContacts.Rows)
		{
			if (Convert.ToInt32(item.Tag) == index)
			{
				index2 = item.Index;
				break;
			}
		}
		dgvContacts.Rows[index2].Cells[1].Value = contactOne.Name;
		dgvContacts.Rows[index2].Cells[2].Value = contactOne.CallId;
		dgvContacts.Rows[index2].Cells[3].Value = contactOne.CallTypeS;
		dgvContacts.Rows[index2].Cells[4].Value = contactOne.RingStyleS;
		dgvContacts.Rows[index2].Cells[5].Value = contactOne.CallRxToneS;
		dgvContacts.Rows[index2].Cells[6].Value = contactOne.RepeaterSlotS;
	}

	public ContactsForm()
	{
		base.Load += ContactsForm_Load;
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	public static void RefreshCommonLang()
	{
		string name = typeof(ContactsForm).Name;
		Settings.smethod_78("HeaderText", SZ_HEADER_TEXT, name);
	}

	private void ContactsForm_Load(object sender, EventArgs e)
	{
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		method_2();
		DispData();
		cmbAddType.SelectedIndex = 0;
	}

	private void btnAdd_Click(object sender, EventArgs e)
	{
		handleInsertClick();
	}

	private void btnDelete_Click(object sender, EventArgs e)
	{
		handleDeleteClick();
	}

	private void btnClear_Click(object sender, EventArgs e)
	{
		int num = 0;
		MainForm mainForm = base.MdiParent as MainForm;
		while (dgvContacts.RowCount > 1)
		{
			num = (int)dgvContacts.Rows[1].Tag;
			dgvContacts.Rows.RemoveAt(1);
			Node.Nodes.RemoveAt(1);
			ContactForm.data.ClearIndex(num);
		}
		method_1();
		mainForm.RefreshRelatedForm(GetType());
	}

	private void btnDeleteSelect_Click(object sender, EventArgs e)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int count = dgvContacts.SelectedRows.Count;
		MainForm mainForm = base.MdiParent as MainForm;
		while (dgvContacts.SelectedRows.Count > 0)
		{
			num = dgvContacts.SelectedRows[0].Index;
			num2 = (int)dgvContacts.SelectedRows[0].Tag;
			dgvContacts.Rows.Remove(dgvContacts.SelectedRows[0]);
			ContactForm.data.ClearIndex(num2);
			mainForm.DeleteTreeViewNode(Node, num);
			num3++;
			if (num3 == count)
			{
				break;
			}
		}
		method_1();
		mainForm.RefreshRelatedForm(GetType());
	}

	private void btnExport_Click(object sender, EventArgs e)
	{
		int num = 0;
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.DefaultExt = "*.csv";
		saveFileDialog.AddExtension = true;
		saveFileDialog.Filter = "csv files|*.csv";
		saveFileDialog.OverwritePrompt = true;
		saveFileDialog.CheckPathExists = true;
		saveFileDialog.FileName = "Contact_" + DateTime.Now.ToString("MMdd_HHmmss");
		if (saveFileDialog.ShowDialog() != DialogResult.OK || saveFileDialog.FileName == null)
		{
			return;
		}
		using CsvFileWriter csvFileWriter = new CsvFileWriter(new FileStream(saveFileDialog.FileName, FileMode.Create), Encoding.Default);
		CsvRow csvRow = new CsvRow();
		csvRow.AddRange(SZ_HEADER_TEXT);
		csvFileWriter.WriteRow(csvRow);
		for (num = 0; num < ContactForm.data.Count; num++)
		{
			if (ContactForm.data.DataIsValid(num))
			{
				csvRow.RemoveAll((string string_0) => true);
				csvRow.Add(num.ToString());
				csvRow.Add(ContactForm.data[num].Name);
				csvRow.Add(ContactForm.data[num].CallId);
				csvRow.Add(ContactForm.data[num].CallTypeS);
				csvRow.Add(ContactForm.data[num].CallRxToneS);
				csvRow.Add(ContactForm.data[num].RingStyleS);
				csvRow.Add(ContactForm.data[num].RepeaterSlotS);
				csvFileWriter.WriteRow(csvRow);
			}
		}
	}

	private void btnImport_Click(object sender, EventArgs e)
	{
		int num = 0;
		int num2 = 0;
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "csv files|*.csv";
		if (openFileDialog.ShowDialog() != DialogResult.OK || openFileDialog.FileName == null)
		{
			return;
		}
		using CsvFileReader csvFileReader = new CsvFileReader(openFileDialog.FileName, Encoding.Default);
		CsvRow csvRow = new CsvRow();
		csvFileReader.ReadRow(csvRow);
		if (csvRow.Count == 7 && csvRow.SequenceEqual(SZ_HEADER_TEXT))
		{
			for (num = 0; num < ContactForm.data.Count; num++)
			{
				ContactForm.data.SetName(num, "");
			}
			while (csvFileReader.ReadRow(csvRow))
			{
				if (csvRow.Count >= 6)
				{
					num = 0;
					num = 1;
					num2 = Convert.ToInt32(csvRow[0]);
					if (num2 < ContactForm.data.Count)
					{
						ContactForm.data.SetName(num2, csvRow[num++]);
						ContactForm.data.SetCallID(num2, csvRow[num++]);
						ContactForm.data.SetCallType(num2, csvRow[num++]);
						ContactForm.data.SetCallRxTone(num2, csvRow[num++]);
						ContactForm.data.SetRingStyle(num2, csvRow[num++]);
						ContactForm.data.SetRepeaterSlot(num2, csvRow[num++]);
					}
				}
			}
			DispData();
			MainForm obj = base.MdiParent as MainForm;
			obj.InitDigitContacts(Node);
			obj.VerifyRelatedForm(GetType());
			obj.RefreshRelatedForm(GetType());
		}
		else
		{
			MessageBox.Show(Settings.SZ_DATA_FORMAT_ERROR);
		}
	}

	private void method_1()
	{
		if (dgvContacts.Rows.Count > 0)
		{
			btnDelete.Enabled = !dgvContacts.SelectedRows.Contains(dgvContacts.Rows[0]);
		}
		else
		{
			btnDelete.Enabled = false;
		}
		btnAdd.Enabled = dgvContacts.RowCount < ContactForm.data.Count;
	}

	private void method_2()
	{
		int num = 0;
		int[] array = new int[7] { 80, 200, 100, 100, 100, 100, 150 };
		dgvContacts.ReadOnly = true;
		dgvContacts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
		dgvContacts.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		dgvContacts.AllowUserToAddRows = false;
		dgvContacts.AllowUserToDeleteRows = false;
		dgvContacts.AllowUserToResizeRows = false;
		dgvContacts.AllowUserToOrderColumns = false;
		dgvContacts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
		DataGridViewTextBoxColumn dataGridViewTextBoxColumn = null;
		string[] sZ_HEADER_TEXT = SZ_HEADER_TEXT;
		foreach (string headerText in sZ_HEADER_TEXT)
		{
			dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
			dataGridViewTextBoxColumn.Width = array[num++];
			dataGridViewTextBoxColumn.HeaderText = headerText;
			dataGridViewTextBoxColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewTextBoxColumn.ReadOnly = true;
			dataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
			dgvContacts.Columns.Add(dataGridViewTextBoxColumn);
		}
		Settings.smethod_40(cmbAddType, ContactForm.SZ_CALL_TYPE, new int[2] { 0, 1 });
		Settings.fillComboBox(cmbType, ContactForm.SZ_CALL_TYPE);
		Settings.fillComboBox(cmbCallRxTone, ContactForm.SZ_CALL_RX_TONE);
		Settings.smethod_43(cmbRingStyle, 0, 10, 0, Settings.SZ_NONE);
		Settings.smethod_43(cmbRepeaterSlot, 0, 4, 0, Settings.SZ_NONE);
		txtName.MaxLength = 16;
		txtCallId.MaxLength = 8;
		dgvContacts.Columns[4].Visible = false;
		dgvContacts.Columns[5].Visible = false;
	}

	private void method_3(object sender, DataGridViewCellMouseEventArgs e)
	{
		if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.ColumnIndex == 5)
		{
			return;
		}
		Control control = (new Control[5] { txtName, txtCallId, cmbType, cmbRingStyle, cmbCallRxTone })[e.ColumnIndex];
		if (dgvContacts.CurrentRow.Tag != null)
		{
			int index = (int)dgvContacts.CurrentRow.Tag;
			if ((!ContactForm.data.IsAllCall(index) || e.ColumnIndex != 2) && (e.RowIndex != 0 || e.ColumnIndex != 3) && ((e.ColumnIndex != 4 && e.ColumnIndex != 5) || Settings.getUserExpertSettings() == Settings.UserMode.Expert))
			{
				Rectangle cellDisplayRectangle = dgvContacts.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, cutOverflow: false);
				Point location = cellDisplayRectangle.Location;
				location.Offset(dgvContacts.Location);
				location.Offset(pnlContact.Location);
				control.Location = location;
				control.Size = cellDisplayRectangle.Size;
				control.Text = ((DataGridView)sender).CurrentCell.Value.ToString();
				control.Visible = true;
				control.Focus();
				control.BringToFront();
			}
		}
	}

	private void dgvContacts_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
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

	private void dgvContacts_SelectionChanged(object sender, EventArgs e)
	{
		method_1();
	}

	private void dgvContacts_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
	{
		if (base.MdiParent is MainForm mainForm)
		{
			int index = (int)(sender as DataGridView).Rows[e.RowIndex].Tag;
			mainForm.DispChildForm(typeof(ContactForm), index);
		}
	}

	private void cmbType_Leave(object sender, EventArgs e)
	{
		int index = dgvContacts.CurrentRow.Index;
		int index2 = (int)dgvContacts.CurrentRow.Tag;
		int selectedIndex = cmbType.SelectedIndex;
		string text = cmbType.Text;
		cmbType.Visible = false;
		if (cmbType.Text == ContactForm.data[index2].CallTypeS)
		{
			return;
		}
		int[] array = new int[3] { 8, 10, 7 };
		if (text == ContactForm.SZ_CALL_TYPE[2])
		{
			if (ContactForm.data.HaveAll())
			{
				int num = ContactForm.data.AllCallIndex();
				int index3 = method_4(num);
				dgvContacts.Rows[index3].Cells[1].Value = dgvContacts.CurrentRow.Cells[1].Value;
				dgvContacts.Rows[index3].Cells[2].Value = dgvContacts.CurrentRow.Cells[2].Value;
				ContactForm.data.SetCallID(num, dgvContacts.CurrentRow.Cells[1].Value.ToString());
				ContactForm.data.SetCallType(num, dgvContacts.CurrentRow.Cells[2].Value.ToString());
				Node.Nodes[index3].ImageIndex = Node.Nodes[index].ImageIndex;
				Node.Nodes[index3].SelectedImageIndex = Node.Nodes[index].SelectedImageIndex;
			}
			string text2 = 16777215.ToString();
			ContactForm.data.SetCallID(index2, text2);
			dgvContacts.CurrentRow.Cells[1].Value = text2;
		}
		else if (text == ContactForm.SZ_CALL_TYPE[0])
		{
			string callId = ContactForm.data[index2].CallId;
			int repeaterSlot = ContactForm.data[index2].RepeaterSlot;
			if (ContactForm.data.IsAllCall(index2) || ContactForm.data.CallIdExist(index2, 0, callId, repeaterSlot))
			{
				callId = ContactForm.data.GetMinCallID(0);
				dgvContacts.CurrentRow.Cells[1].Value = callId;
				ContactForm.data.SetCallID(index2, callId);
			}
		}
		else if (text == ContactForm.SZ_CALL_TYPE[1])
		{
			string callId2 = ContactForm.data[index2].CallId;
			int repeaterSlot2 = ContactForm.data[index2].RepeaterSlot;
			if (ContactForm.data.IsAllCall(index2) || ContactForm.data.CallIdExist(index2, 1, callId2, repeaterSlot2))
			{
				callId2 = ContactForm.data.GetMinCallID(1);
				dgvContacts.CurrentRow.Cells[1].Value = callId2;
				ContactForm.data.SetCallID(index2, callId2);
			}
		}
		dgvContacts.CurrentCell.Value = text;
		ContactForm.data.SetCallType(index2, text);
		Node.Nodes[index].ImageIndex = array[selectedIndex];
		Node.Nodes[index].SelectedImageIndex = array[selectedIndex];
		((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
	}

	private int method_4(int int_0)
	{
		int num = 0;
		while (true)
		{
			if (num < dgvContacts.Rows.Count)
			{
				if ((int)dgvContacts.Rows[num].Tag == int_0)
				{
					break;
				}
				num++;
				continue;
			}
			return -1;
		}
		return num;
	}

	private void txtName_Leave(object sender, EventArgs e)
	{
		int index = (int)dgvContacts.CurrentRow.Tag;
		int index2 = dgvContacts.CurrentRow.Index;
		txtName.Visible = false;
		string text = txtName.Text;
		if (!(text == ContactForm.data[index].Name) && !ContactForm.data.NameExist(text))
		{
			dgvContacts.CurrentCell.Value = text;
			ContactForm.data.SetName(index, text);
			Node.Nodes[index2].Text = text;
			((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
		}
	}

	private void txtCallId_Leave(object sender, EventArgs e)
	{
		int index = (int)dgvContacts.CurrentRow.Tag;
		txtCallId.Visible = false;
		string text = txtCallId.Text.PadLeft(8, '0');
		if (!(text == ContactForm.data[index].CallId) && !ContactForm.data.CallIdExist(index, text) && ContactForm.data.CallIdValid(text))
		{
			dgvContacts.CurrentCell.Value = text;
			ContactForm.data.SetCallID(index, text);
			((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
		}
	}

	private void cmbRingStyle_Leave(object sender, EventArgs e)
	{
		int index = (int)dgvContacts.CurrentRow.Tag;
		cmbRingStyle.Visible = false;
		dgvContacts.CurrentCell.Value = cmbRingStyle.Text;
		ContactForm.data.SetRingStyle(index, cmbRingStyle.Text);
		((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
	}

	private void cmbCallRxTone_Leave(object sender, EventArgs e)
	{
		int index = (int)dgvContacts.CurrentRow.Tag;
		cmbCallRxTone.Visible = false;
		dgvContacts.CurrentCell.Value = cmbCallRxTone.Text;
		ContactForm.data.SetCallRxTone(index, cmbCallRxTone.Text);
		((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
	}

	[CompilerGenerated]
	private static bool smethod_0(string string_0)
	{
		return true;
	}

	static ContactsForm()
	{
		SZ_HEADER_TEXT = new string[7] { "", "", "", "", "", "", "" };
	}

	private void btnDownload_Click(object sender, EventArgs e)
	{
		DownloadContactsForm downloadContactsForm = new DownloadContactsForm();
		downloadContactsForm.StartPosition = FormStartPosition.CenterParent;
		downloadContactsForm.parentForm = this;
		downloadContactsForm.mainForm = base.MdiParent as MainForm;
		try
		{
			downloadContactsForm.ShowDialog();
		}
		catch (Exception)
		{
			MessageBox.Show(Settings.dicCommon["UnableDownloadFromInternet"]);
		}
	}

	private void dgvContacts_KeyPress(object sender, KeyPressEventArgs e)
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
				mainForm.DispChildForm(typeof(ContactForm), index);
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

	private void handleInsertClick()
	{
		int selectedIndex = cmbAddType.SelectedIndex;
		int minIndex = ContactForm.data.GetMinIndex();
		MainForm mainForm = base.MdiParent as MainForm;
		string minCallID = ContactForm.data.GetMinCallID(selectedIndex, minIndex);
		string minName = ContactForm.data.GetMinName(Node);
		string callRxToneS = ContactForm.DefaultContact.CallRxToneS;
		string ringStyleS = ContactForm.DefaultContact.RingStyleS;
		string text = cmbAddType.Text;
		ContactForm.data.SetIndex(minIndex, 1);
		ContactForm.ContactOne value = new ContactForm.ContactOne(minIndex);
		value.Name = minName;
		value.CallId = minCallID;
		value.CallTypeS = text;
		value.RingStyleS = ringStyleS;
		value.CallRxToneS = callRxToneS;
		ContactForm.data[minIndex] = value;
		dgvContacts.Rows.Insert(minIndex, (minIndex + 1).ToString(), minName, minCallID, text, ringStyleS, callRxToneS);
		dgvContacts.Rows[minIndex].Tag = minIndex;
		method_1();
		int[] array = new int[3] { 8, 10, 7 };
		mainForm.InsertTreeViewNode(Node, minIndex, typeof(ContactForm), array[selectedIndex], ContactForm.data);
		mainForm.RefreshRelatedForm(GetType());
		mainForm.DispChildForm(typeof(ContactForm), minIndex);
	}

	private void handleDeleteClick()
	{
		if (dgvContacts.CurrentRow != null && dgvContacts.CurrentRow.Tag != null)
		{
			int index = dgvContacts.CurrentRow.Index;
			int index2 = (int)dgvContacts.CurrentRow.Tag;
			if (index == 0)
			{
				MessageBox.Show(Settings.dicCommon["FirstNotDelete"]);
				return;
			}
			dgvContacts.Rows.Remove(dgvContacts.CurrentRow);
			ContactForm.data.ClearIndex(index2);
			method_1();
			MainForm obj = base.MdiParent as MainForm;
			obj.DeleteTreeViewNode(Node, index);
			obj.RefreshRelatedForm(GetType());
		}
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
		default:
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
