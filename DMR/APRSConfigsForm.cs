using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class APRSConfigsForm : DockContent, IDisp, ISingleRow
{
	private Panel pnlContact;

	private DataGridView dgvAPRSConfigs;

	private Button btnClear;

	private Button btnDelete;

	private Button btnAdd;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;

	private static readonly string[] SZ_HEADER_TEXT;

	public TreeNode Node { get; set; }

	public DataGridView getDataGridView()
	{
		return dgvAPRSConfigs;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
            this.pnlContact = new System.Windows.Forms.Panel();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dgvAPRSConfigs = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlContact.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAPRSConfigs)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlContact
            // 
            this.pnlContact.AutoScroll = true;
            this.pnlContact.AutoSize = true;
            this.pnlContact.Controls.Add(this.btnClear);
            this.pnlContact.Controls.Add(this.btnDelete);
            this.pnlContact.Controls.Add(this.btnAdd);
            this.pnlContact.Controls.Add(this.dgvAPRSConfigs);
            this.pnlContact.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContact.Location = new System.Drawing.Point(0, 0);
            this.pnlContact.Name = "pnlContact";
            this.pnlContact.Size = new System.Drawing.Size(965, 381);
            this.pnlContact.TabIndex = 0;
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.SystemColors.Control;
            this.btnClear.Location = new System.Drawing.Point(292, 12);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(129, 23);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.Control;
            this.btnDelete.Location = new System.Drawing.Point(157, 12);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(129, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.Control;
            this.btnAdd.Location = new System.Drawing.Point(22, 12);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(129, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // dgvAPRSConfigs
            // 
            this.dgvAPRSConfigs.BackgroundColor = System.Drawing.Color.White;
            this.dgvAPRSConfigs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAPRSConfigs.Location = new System.Drawing.Point(22, 41);
            this.dgvAPRSConfigs.Name = "dgvAPRSConfigs";
            this.dgvAPRSConfigs.ReadOnly = true;
            this.dgvAPRSConfigs.RowHeadersWidth = 50;
            this.dgvAPRSConfigs.RowTemplate.Height = 23;
            this.dgvAPRSConfigs.Size = new System.Drawing.Size(913, 328);
            this.dgvAPRSConfigs.TabIndex = 9;
            this.dgvAPRSConfigs.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvAPRS_Configs_RowHeaderMouseDoubleClick);
            this.dgvAPRSConfigs.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvAPRS_Configs_RowPostPaint);
            this.dgvAPRSConfigs.SelectionChanged += new System.EventHandler(this.dgvAPRS_Configs_SelectionChanged);
            this.dgvAPRSConfigs.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvAPRS_Configs_KeyPress);
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
            // APRSConfigsForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(965, 381);
            this.Controls.Add(this.pnlContact);
            this.Font = new System.Drawing.Font("Arial", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "APRSConfigsForm";
            this.Text = "Contacts";
            this.pnlContact.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAPRSConfigs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

	}

	public void SaveData()
	{
	}

	public void DispData()
	{
		dgvAPRSConfigs.Rows.Clear();
		for (int i = 0; i < APRSForm.data.Count; i++)
		{
			if (APRSForm.data.DataIsValid(i))
			{
				int index = dgvAPRSConfigs.Rows.Add(APRSForm.data[i].Name, APRSForm.data[i].SSID, APRSForm.data[i].Via1, APRSForm.data[i].Via1SSID, APRSForm.data[i].Via2, APRSForm.data[i].Via2SSID);
				dgvAPRSConfigs.Rows[index].Tag = i;
			}
		}
	}

	public void RefreshName()
	{
	}

	public void RefreshSingleRow(int index)
	{
		APRSForm.APRS_One aPRS_One = APRSForm.data[index];
		int index2 = 0;
		foreach (DataGridViewRow item in (IEnumerable)dgvAPRSConfigs.Rows)
		{
			if (Convert.ToInt32(item.Tag) == index)
			{
				index2 = item.Index;
				break;
			}
		}
		dgvAPRSConfigs.Rows[index2].Cells[1].Value = aPRS_One.Name;
	}

	public APRSConfigsForm()
	{
		base.Load += APRSConfigsForm_Load;
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	public static void RefreshCommonLang()
	{
		string name = typeof(APRSConfigsForm).Name;
		Settings.smethod_78("HeaderText", SZ_HEADER_TEXT, name);
	}

	private void APRSConfigsForm_Load(object sender, EventArgs e)
	{
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		method_2();
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
		while (dgvAPRSConfigs.RowCount > 1)
		{
			num = (int)dgvAPRSConfigs.Rows[1].Tag;
			dgvAPRSConfigs.Rows.RemoveAt(1);
			Node.Nodes.RemoveAt(1);
			APRSForm.data.ClearIndex(num);
		}
		updateButtonEnabledDisabled();
		mainForm.RefreshRelatedForm(GetType());
	}

	private void updateButtonEnabledDisabled()
	{
		btnDelete.Enabled = dgvAPRSConfigs.Rows.Count > 0;
		btnAdd.Enabled = dgvAPRSConfigs.RowCount < APRSForm.data.Count;
	}

	private void method_2()
	{
		dgvAPRSConfigs.ReadOnly = true;
		dgvAPRSConfigs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
		dgvAPRSConfigs.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		dgvAPRSConfigs.AllowUserToAddRows = false;
		dgvAPRSConfigs.AllowUserToDeleteRows = false;
		dgvAPRSConfigs.AllowUserToResizeRows = false;
		dgvAPRSConfigs.AllowUserToOrderColumns = false;
		dgvAPRSConfigs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
		DataGridViewTextBoxColumn dataGridViewTextBoxColumn = null;
		string[] sZ_HEADER_TEXT = SZ_HEADER_TEXT;
		foreach (string headerText in sZ_HEADER_TEXT)
		{
			dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
			dataGridViewTextBoxColumn.HeaderText = headerText;
			dataGridViewTextBoxColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewTextBoxColumn.ReadOnly = true;
			dataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
			dgvAPRSConfigs.Columns.Add(dataGridViewTextBoxColumn);
		}
	}

	private void dgvAPRS_Configs_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
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

	private void dgvAPRS_Configs_SelectionChanged(object sender, EventArgs e)
	{
	}

	private Form getMainForm()
	{
		foreach (Form openForm in Application.OpenForms)
		{
			if (openForm is MainForm)
			{
				return openForm;
			}
		}
		return null;
	}

	private void dgvAPRS_Configs_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
	{
		MainForm mainForm = (MainForm)getMainForm();
		if (mainForm != null)
		{
			int index = (int)(sender as DataGridView).Rows[e.RowIndex].Tag;
			mainForm.DispChildForm(typeof(APRSForm), index);
		}
	}

	[CompilerGenerated]
	private static bool smethod_0(string string_0)
	{
		return true;
	}

	static APRSConfigsForm()
	{
		SZ_HEADER_TEXT = new string[6] { "", "", "", "", "", "" };
	}

	private void dgvAPRS_Configs_KeyPress(object sender, KeyPressEventArgs e)
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
				mainForm.DispChildForm(typeof(APRSForm), index);
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
		MainForm mainForm = base.MdiParent as MainForm;
		int minIndex = APRSForm.data.GetMinIndex();
		if (minIndex >= 0)
		{
			string minName = APRSForm.data.GetMinName(Node);
			APRSForm.data.SetIndex(minIndex, 1);
			APRSForm.data.Default(minIndex);
			APRSForm.data.SetName(minIndex, minName);
			int index = minIndex;
			dgvAPRSConfigs.Rows.Add(APRSForm.data[index].Name, APRSForm.data[index].SSID, APRSForm.data[index].Via1, APRSForm.data[index].Via1SSID, APRSForm.data[index].Via2, APRSForm.data[index].Via2SSID);
			dgvAPRSConfigs.Rows[minIndex].Tag = minIndex;
			updateButtonEnabledDisabled();
			mainForm.InsertTreeViewNode(Node, minIndex, typeof(APRSForm), 11, APRSForm.data);
			mainForm.RefreshRelatedForm(GetType());
			mainForm.DispChildForm(typeof(APRSForm), minIndex);
		}
	}

	private void handleDeleteClick()
	{
		if (dgvAPRSConfigs.CurrentRow != null && dgvAPRSConfigs.CurrentRow.Tag != null)
		{
			int index = dgvAPRSConfigs.CurrentRow.Index;
			int index2 = (int)dgvAPRSConfigs.CurrentRow.Tag;
			dgvAPRSConfigs.Rows.Remove(dgvAPRSConfigs.CurrentRow);
			APRSForm.data.ClearIndex(index2);
			APRSForm.Compact();
			updateButtonEnabledDisabled();
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
