using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DMR;

public class DownloadContactsForm : Form
{
	public ContactsForm parentForm;

	public MainForm mainForm;

	public TreeNode treeNode;

	private bool _isDownloading;

	private WebClient _wc;

	private IContainer components;

	private DataGridView dgvDownloadeContacts;

	private Button btnImport;

	private Button btnDownloadLastHeard;

	private TextBox txtIDStart;

	private Label lblIDStart;

	private Label lblMessage;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;

	private DataGridViewTextBoxColumn id;

	private DataGridViewTextBoxColumn callsign;

	private DataGridViewTextBoxColumn name;

	private DataGridViewTextBoxColumn lastheard;

	private Button btnSelectAll;

	private Button button1;

	private TextBox txtDownloadURL;

	private TextBox textBox1;

	private Label label1;

	private TextBox txtAgeMaxDays;

	private Label lblInactivityFilter;

	public DownloadContactsForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		if (int.Parse(GeneralSetForm.data.RadioId) / 10000 > 0)
		{
			txtIDStart.Text = (int.Parse(GeneralSetForm.data.RadioId) / 10000).ToString();
		}
		string text = IniFileUtils.getProfileStringWithDefault("Setup", "DownloadContactsURL", "");
		if (text == "")
		{
			text = "http://ham-digital.org/user_by_lh.php";
		}
		txtDownloadURL.Text = text;
	}

	private bool addPrivateContact(string id, string callsignAndName)
	{
		int minIndex = ContactForm.data.GetMinIndex();
		if (minIndex < 0)
		{
			return false;
		}
		ContactForm.data.SetIndex(minIndex, 1);
		ContactForm.ContactOne value = new ContactForm.ContactOne(minIndex);
		value.Name = callsignAndName;
		value.CallId = $"{int.Parse(id):d8}";
		value.CallTypeS = ContactForm.SZ_CALL_TYPE[1];
		value.RingStyleS = ContactForm.DefaultContact.RingStyleS;
		value.CallRxToneS = ContactForm.SZ_CALL_RX_TONE[0];
		ContactForm.data[minIndex] = value;
		int[] array = new int[3] { 8, 10, 7 };
		if (parentForm != null)
		{
			mainForm.InsertTreeViewNode(parentForm.Node, minIndex, typeof(ContactForm), array[1], ContactForm.data);
		}
		else
		{
			mainForm.InsertTreeViewNode(treeNode, minIndex, typeof(ContactForm), array[1], ContactForm.data);
		}
		return true;
	}

	private void DMRMARCDownloadCompleteHandler(object sender = null, DownloadStringCompletedEventArgs e = null)
	{
		string text = GeneralSetForm.data.RadioId;
		if (text.Substring(0, 1) == "0")
		{
			text = text.Substring(1);
		}
		string result = e.Result;
		try
		{
			string[] array = result.Split('\n');
			text = "\"" + text + "\"";
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = array2[i].Split(',');
				if (!(array3[0] != "") || !(txtIDStart.Text == array3[0].Substring(1, txtIDStart.Text.Length)))
				{
					continue;
				}
				bool flag = false;
				if (text == array3[0])
				{
					flag = true;
				}
				else
				{
					int num = int.Parse(array3[0].Trim('"'));
					for (int j = 0; j < ContactForm.data.Count; j++)
					{
						if (ContactForm.data.DataIsValid(j) && int.Parse(ContactForm.data[j].CallId) == num)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					dgvDownloadeContacts.Rows.Insert(0, array3[0].Trim('"'), array3[1].Trim('"'), array3[2].Trim('"'), "");
				}
			}
			lblMessage.Text = string.Format(Settings.dicCommon["DownloadContactsMessageAdded"], dgvDownloadeContacts.RowCount);
		}
		catch (Exception)
		{
			MessageBox.Show(Settings.dicCommon["UnableDownloadFromInternet"]);
		}
		finally
		{
			_wc = null;
			_isDownloading = false;
			Cursor.Current = Cursors.Default;
		}
	}

	private void downloadProgressHandler(object sender, DownloadProgressChangedEventArgs e)
	{
		try
		{
			BeginInvoke((Action)delegate
			{
				lblMessage.Text = Settings.dicCommon["DownloadContactsDownloading"] + e.ProgressPercentage + "%";
			});
		}
		catch (Exception)
		{
		}
	}

	private void btnDownloadDMRMARC_Click(object sender, EventArgs e)
	{
		if (Cursor.Current == Cursors.WaitCursor || _isDownloading)
		{
			MessageBox.Show("Already downloading");
			return;
		}
		if (txtIDStart.Text == "" || int.Parse(txtIDStart.Text) == 0)
		{
			MessageBox.Show(Settings.dicCommon["DownloadContactsRegionEmpty"]);
			return;
		}
		lblMessage.Text = Settings.dicCommon["DownloadContactsDownloading"];
		Refresh();
		_wc = new WebClient();
		try
		{
			Application.DoEvents();
			_wc.DownloadStringCompleted += DMRMARCDownloadCompleteHandler;
			_wc.DownloadProgressChanged += downloadProgressHandler;
			_wc.DownloadStringAsync(new Uri("http://radioid.net/static/users_quoted.csv"));
		}
		catch (Exception)
		{
			MessageBox.Show(Settings.dicCommon["UnableDownloadFromInternet"]);
			return;
		}
		_isDownloading = true;
		Cursor.Current = Cursors.WaitCursor;
	}

	private void btnDownloadLastHeard_Click(object sender, EventArgs e)
	{
		if (Cursor.Current == Cursors.WaitCursor || _isDownloading)
		{
			MessageBox.Show("Already downloading");
			return;
		}
		if (txtIDStart.Text == "" || int.Parse(txtIDStart.Text) == 0)
		{
			MessageBox.Show(Settings.dicCommon["DownloadContactsRegionEmpty"]);
			return;
		}
		lblMessage.Text = Settings.dicCommon["DownloadContactsDownloading"];
		Refresh();
		_wc = new WebClient();
		string text;
		try
		{
			_isDownloading = true;
			Cursor.Current = Cursors.WaitCursor;
			Application.DoEvents();
			text = _wc.DownloadString(txtDownloadURL.Text + "?id=" + txtIDStart.Text + "&cnt=1024");
		}
		catch (Exception)
		{
			_isDownloading = false;
			Cursor.Current = Cursors.Default;
			MessageBox.Show(Settings.dicCommon["UnableDownloadFromInternet"]);
			return;
		}
		finally
		{
			_wc = null;
		}
		dgvDownloadeContacts.SuspendLayout();
		string[] array = text.Split('\n');
		int num = int.Parse(GeneralSetForm.data.RadioId);
		int num2 = int.MaxValue;
		try
		{
			num2 = int.Parse(txtAgeMaxDays.Text);
		}
		catch (Exception)
		{
		}
		for (int num3 = array.Length - 2; num3 > 1; num3--)
		{
			bool flag = false;
			string[] array2 = array[num3].Split(';');
			if (num == int.Parse(array2[2]) || int.Parse(array2[4]) > num2)
			{
				flag = true;
			}
			else
			{
				int num4 = int.Parse(array2[2]);
				for (int i = 0; i < ContactForm.data.Count; i++)
				{
					if (ContactForm.data.DataIsValid(i) && int.Parse(ContactForm.data[i].CallId) == num4)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				string text2 = ((array2[3].IndexOf(" ") == -1) ? array2[3] : array2[3].Substring(0, array2[3].IndexOf(" ")));
				dgvDownloadeContacts.Rows.Insert(0, array2[2], array2[1], text2, Regex.Replace(array2[4], "\\n|\\r", ""));
			}
		}
		updateTotalNumberMessage();
		Cursor.Current = Cursors.Default;
		_isDownloading = false;
	}

	private void updateTotalNumberMessage()
	{
		lblMessage.Text = string.Format(Settings.dicCommon["DownloadContactsMessageAdded"], dgvDownloadeContacts.RowCount);
	}

	private void btnImport_Click(object sender, EventArgs e)
	{
		if (dgvDownloadeContacts.SelectedRows.Count == 0)
		{
			MessageBox.Show(Settings.dicCommon["DownloadContactsSelectContactsToImport"]);
			return;
		}
		List<DataGridViewRow> list = new List<DataGridViewRow>();
		foreach (DataGridViewRow selectedRow in dgvDownloadeContacts.SelectedRows)
		{
			list.Add(selectedRow);
		}
		list.Reverse();
		foreach (DataGridViewRow item in list)
		{
			if (!addPrivateContact(item.Cells[0].Value?.ToString() ?? "", item.Cells[1].Value?.ToString() + " " + item.Cells[2].Value))
			{
				MessageBox.Show(Settings.dicCommon["DownloadContactsTooMany"], Settings.dicCommon["Warning"]);
				break;
			}
			dgvDownloadeContacts.Rows.Remove(item);
		}
		if (parentForm != null)
		{
			parentForm.DispData();
			mainForm.RefreshRelatedForm(GetType());
		}
		MessageBox.Show(Settings.dicCommon["DownloadContactsImportComplete"]);
	}

	private void btnSelectAll_Click(object sender, EventArgs e)
	{
		dgvDownloadeContacts.SelectAll();
	}

	private void DownloadContacts_Load(object sender, EventArgs e)
	{
		Settings.smethod_59(base.Controls);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
	}

	private void btnClose_Click(object sender, EventArgs e)
	{
		IniFileUtils.WriteProfileString("Setup", "DownloadContactsURL", txtDownloadURL.Text);
		Close();
	}

	private void onFormClosing(object sender, FormClosingEventArgs e)
	{
		if (_wc != null)
		{
			_wc.CancelAsync();
		}
	}

	private void dgvDownloadeContacts_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
	{
		updateTotalNumberMessage();
	}

	private void dgvDownloadeContacts_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
	{
		if (e.Column.Index != 3)
		{
			return;
		}
		try
		{
			if (int.Parse(e.CellValue1.ToString()) < int.Parse(e.CellValue2.ToString()))
			{
				e.SortResult = -1;
			}
			else
			{
				e.SortResult = 1;
			}
			e.Handled = true;
		}
		catch (Exception)
		{
		}
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
		this.dgvDownloadeContacts = new System.Windows.Forms.DataGridView();
		this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.callsign = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.lastheard = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.btnImport = new System.Windows.Forms.Button();
		this.btnDownloadLastHeard = new System.Windows.Forms.Button();
		this.txtIDStart = new System.Windows.Forms.TextBox();
		this.lblIDStart = new System.Windows.Forms.Label();
		this.lblMessage = new System.Windows.Forms.Label();
		this.btnSelectAll = new System.Windows.Forms.Button();
		this.button1 = new System.Windows.Forms.Button();
		this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.txtDownloadURL = new System.Windows.Forms.TextBox();
		this.textBox1 = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.txtAgeMaxDays = new System.Windows.Forms.TextBox();
		this.lblInactivityFilter = new System.Windows.Forms.Label();
		((System.ComponentModel.ISupportInitialize)this.dgvDownloadeContacts).BeginInit();
		base.SuspendLayout();
		this.dgvDownloadeContacts.AllowUserToAddRows = false;
		this.dgvDownloadeContacts.AllowUserToOrderColumns = true;
		this.dgvDownloadeContacts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvDownloadeContacts.Columns.AddRange(this.id, this.callsign, this.name, this.lastheard);
		this.dgvDownloadeContacts.Location = new System.Drawing.Point(21, 42);
		this.dgvDownloadeContacts.Name = "dgvDownloadeContacts";
		this.dgvDownloadeContacts.ReadOnly = true;
		this.dgvDownloadeContacts.Size = new System.Drawing.Size(551, 416);
		this.dgvDownloadeContacts.TabIndex = 0;
		this.dgvDownloadeContacts.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(dgvDownloadeContacts_SortCompare);
		this.dgvDownloadeContacts.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(dgvDownloadeContacts_UserDeletedRow);
		this.id.HeaderText = "ID";
		this.id.Name = "id";
		this.id.ReadOnly = true;
		this.callsign.HeaderText = "Callsign";
		this.callsign.Name = "callsign";
		this.callsign.ReadOnly = true;
		this.name.HeaderText = "Name";
		this.name.Name = "name";
		this.name.ReadOnly = true;
		this.lastheard.HeaderText = "Last heard (days ago)";
		this.lastheard.Name = "lastheard";
		this.lastheard.ReadOnly = true;
		this.lastheard.Width = 175;
		this.btnImport.Location = new System.Drawing.Point(643, 381);
		this.btnImport.Name = "btnImport";
		this.btnImport.Size = new System.Drawing.Size(129, 28);
		this.btnImport.TabIndex = 1;
		this.btnImport.Text = "Import selected";
		this.btnImport.UseVisualStyleBackColor = true;
		this.btnImport.Click += new System.EventHandler(btnImport_Click);
		this.btnDownloadLastHeard.Location = new System.Drawing.Point(582, 99);
		this.btnDownloadLastHeard.Name = "btnDownloadLastHeard";
		this.btnDownloadLastHeard.Size = new System.Drawing.Size(197, 28);
		this.btnDownloadLastHeard.TabIndex = 2;
		this.btnDownloadLastHeard.Text = "Download from URL";
		this.btnDownloadLastHeard.UseVisualStyleBackColor = true;
		this.btnDownloadLastHeard.Click += new System.EventHandler(btnDownloadLastHeard_Click);
		this.txtIDStart.Location = new System.Drawing.Point(735, 12);
		this.txtIDStart.Name = "txtIDStart";
		this.txtIDStart.Size = new System.Drawing.Size(37, 23);
		this.txtIDStart.TabIndex = 3;
		this.txtIDStart.Text = "505";
		this.lblIDStart.Location = new System.Drawing.Point(585, 15);
		this.lblIDStart.Name = "lblIDStart";
		this.lblIDStart.Size = new System.Drawing.Size(144, 16);
		this.lblIDStart.TabIndex = 4;
		this.lblIDStart.Text = "Region Prefix code";
		this.lblIDStart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblMessage.AutoSize = true;
		this.lblMessage.Location = new System.Drawing.Point(18, 9);
		this.lblMessage.Name = "lblMessage";
		this.lblMessage.Size = new System.Drawing.Size(386, 16);
		this.lblMessage.TabIndex = 5;
		this.lblMessage.Text = "Enter the ID prefix code for your region and press Download";
		this.btnSelectAll.Location = new System.Drawing.Point(581, 271);
		this.btnSelectAll.Name = "btnSelectAll";
		this.btnSelectAll.Size = new System.Drawing.Size(124, 28);
		this.btnSelectAll.TabIndex = 1;
		this.btnSelectAll.Text = "Select all";
		this.btnSelectAll.UseVisualStyleBackColor = true;
		this.btnSelectAll.Click += new System.EventHandler(btnSelectAll_Click);
		this.button1.Location = new System.Drawing.Point(643, 426);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(129, 28);
		this.button1.TabIndex = 1;
		this.button1.Text = "Close";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(btnClose_Click);
		this.dataGridViewTextBoxColumn1.HeaderText = "ID";
		this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
		this.dataGridViewTextBoxColumn1.ReadOnly = true;
		this.dataGridViewTextBoxColumn2.HeaderText = "Callsign";
		this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
		this.dataGridViewTextBoxColumn2.ReadOnly = true;
		this.dataGridViewTextBoxColumn3.HeaderText = "Name";
		this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
		this.dataGridViewTextBoxColumn3.ReadOnly = true;
		this.dataGridViewTextBoxColumn4.HeaderText = "Last heard\n(Days ago)";
		this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
		this.dataGridViewTextBoxColumn4.ReadOnly = true;
		this.dataGridViewTextBoxColumn4.Width = 150;
		this.txtDownloadURL.Location = new System.Drawing.Point(125, 469);
		this.txtDownloadURL.Name = "txtDownloadURL";
		this.txtDownloadURL.Size = new System.Drawing.Size(447, 23);
		this.txtDownloadURL.TabIndex = 7;
		this.textBox1.Enabled = false;
		this.textBox1.Location = new System.Drawing.Point(578, 469);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size(201, 23);
		this.textBox1.TabIndex = 8;
		this.textBox1.Text = "?id=REGION_PREFIX&cnt=1024";
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(18, 472);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(101, 16);
		this.label1.TabIndex = 9;
		this.label1.Text = "Download URL";
		this.txtAgeMaxDays.Location = new System.Drawing.Point(735, 42);
		this.txtAgeMaxDays.Name = "txtAgeMaxDays";
		this.txtAgeMaxDays.Size = new System.Drawing.Size(37, 23);
		this.txtAgeMaxDays.TabIndex = 3;
		this.txtAgeMaxDays.Text = "180";
		this.lblInactivityFilter.Location = new System.Drawing.Point(585, 45);
		this.lblInactivityFilter.Name = "lblInactivityFilter";
		this.lblInactivityFilter.Size = new System.Drawing.Size(147, 16);
		this.lblInactivityFilter.TabIndex = 4;
		this.lblInactivityFilter.Text = "Inactivity filter (days)";
		this.lblInactivityFilter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(784, 507);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.textBox1);
		base.Controls.Add(this.txtDownloadURL);
		base.Controls.Add(this.lblMessage);
		base.Controls.Add(this.lblInactivityFilter);
		base.Controls.Add(this.txtAgeMaxDays);
		base.Controls.Add(this.lblIDStart);
		base.Controls.Add(this.txtIDStart);
		base.Controls.Add(this.btnDownloadLastHeard);
		base.Controls.Add(this.btnSelectAll);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.btnImport);
		base.Controls.Add(this.dgvDownloadeContacts);
		this.Font = new System.Drawing.Font("Arial", 10f);
		base.MinimizeBox = false;
		base.Name = "DownloadContactsForm";
		this.Text = "Download contacts";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(onFormClosing);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.Load += new System.EventHandler(DownloadContacts_Load);
		((System.ComponentModel.ISupportInitialize)this.dgvDownloadeContacts).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
