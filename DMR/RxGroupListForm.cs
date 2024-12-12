using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class RxGroupListForm : DockContent, IDisp
{
	private ToolStrip tsrGrpList;

	private ToolStripButton tsbtnFirst;

	private ToolStripButton tsbtnPrev;

	private ToolStripButton tsbtnNext;

	private ToolStripButton tsbtnLast;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripButton tsbtnAdd;

	private ToolStripButton tsbtnDel;

	private ToolStripMenuItem tsmiCh;

	private ToolStripMenuItem tsmiFirst;

	private ToolStripMenuItem tsmiPrev;

	private ToolStripMenuItem tsmiNext;

	private ToolStripMenuItem tsmiLast;

	private ToolStripMenuItem tsmiAdd;

	private ToolStripMenuItem tsmiDel;

	private ToolStripSeparator toolStripSeparator2;

	private ToolStripLabel tslblInfo;

	private Button btnDel;

	private Button btnAdd;

	private ListBox lstSelected;

	private ListBox lstUnselected;

	private SGTextBox txtName;

	private Label lblName;

	private GroupBox grpUnselected;

	private GroupBox grpSelected;

	private Button btnUp;

	private Button btnDown;

	private CustomPanel pnlRxGrpList;

	public static RxListData data;
    private Label lblWarning;
    private ComponentResourceManager componentResourceManager;

	public TreeNode Node { get; set; }

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
            this.tsmiCh = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFirst = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPrev = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNext = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLast = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDel = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlRxGrpList = new CustomPanel();
            this.lblWarning = new System.Windows.Forms.Label();
            this.tsrGrpList = new System.Windows.Forms.ToolStrip();
            this.tslblInfo = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnFirst = new System.Windows.Forms.ToolStripButton();
            this.tsbtnPrev = new System.Windows.Forms.ToolStripButton();
            this.tsbtnNext = new System.Windows.Forms.ToolStripButton();
            this.tsbtnLast = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnAdd = new System.Windows.Forms.ToolStripButton();
            this.tsbtnDel = new System.Windows.Forms.ToolStripButton();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.txtName = new DMR.SGTextBox();
            this.grpSelected = new System.Windows.Forms.GroupBox();
            this.lstSelected = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.grpUnselected = new System.Windows.Forms.GroupBox();
            this.lstUnselected = new System.Windows.Forms.ListBox();
            this.btnDel = new System.Windows.Forms.Button();
            this.lblName = new System.Windows.Forms.Label();
            this.pnlRxGrpList.SuspendLayout();
            this.tsrGrpList.SuspendLayout();
            this.grpSelected.SuspendLayout();
            this.grpUnselected.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsmiCh
            // 
            this.tsmiCh.Name = "tsmiCh";
            this.tsmiCh.Size = new System.Drawing.Size(32, 19);
            // 
            // tsmiFirst
            // 
            this.tsmiFirst.Name = "tsmiFirst";
            this.tsmiFirst.Size = new System.Drawing.Size(32, 19);
            // 
            // tsmiPrev
            // 
            this.tsmiPrev.Name = "tsmiPrev";
            this.tsmiPrev.Size = new System.Drawing.Size(32, 19);
            // 
            // tsmiNext
            // 
            this.tsmiNext.Name = "tsmiNext";
            this.tsmiNext.Size = new System.Drawing.Size(32, 19);
            // 
            // tsmiLast
            // 
            this.tsmiLast.Name = "tsmiLast";
            this.tsmiLast.Size = new System.Drawing.Size(32, 19);
            // 
            // tsmiAdd
            // 
            this.tsmiAdd.Name = "tsmiAdd";
            this.tsmiAdd.Size = new System.Drawing.Size(32, 19);
            // 
            // tsmiDel
            // 
            this.tsmiDel.Name = "tsmiDel";
            this.tsmiDel.Size = new System.Drawing.Size(32, 19);
            // 
            // pnlRxGrpList
            // 
            this.pnlRxGrpList.AutoScroll = true;
            this.pnlRxGrpList.AutoSize = true;
            this.pnlRxGrpList.BackColor = System.Drawing.Color.White;
            this.pnlRxGrpList.Controls.Add(this.lblWarning);
            this.pnlRxGrpList.Controls.Add(this.tsrGrpList);
            this.pnlRxGrpList.Controls.Add(this.btnDown);
            this.pnlRxGrpList.Controls.Add(this.btnUp);
            this.pnlRxGrpList.Controls.Add(this.txtName);
            this.pnlRxGrpList.Controls.Add(this.grpSelected);
            this.pnlRxGrpList.Controls.Add(this.btnAdd);
            this.pnlRxGrpList.Controls.Add(this.grpUnselected);
            this.pnlRxGrpList.Controls.Add(this.btnDel);
            this.pnlRxGrpList.Controls.Add(this.lblName);
            this.pnlRxGrpList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRxGrpList.Location = new System.Drawing.Point(0, 0);
            this.pnlRxGrpList.Name = "pnlRxGrpList";
            this.pnlRxGrpList.Size = new System.Drawing.Size(693, 567);
            this.pnlRxGrpList.TabIndex = 8;
            // 
            // lblWarning
            // 
            this.lblWarning.AutoSize = true;
            this.lblWarning.Location = new System.Drawing.Point(249, 42);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(45, 16);
            this.lblWarning.TabIndex = 10;
            this.lblWarning.Text = "label1";
            // 
            // tsrGrpList
            // 
            this.tsrGrpList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslblInfo,
            this.toolStripSeparator2,
            this.tsbtnFirst,
            this.tsbtnPrev,
            this.tsbtnNext,
            this.tsbtnLast,
            this.toolStripSeparator1,
            this.tsbtnAdd,
            this.tsbtnDel});
            this.tsrGrpList.Location = new System.Drawing.Point(0, 0);
            this.tsrGrpList.Name = "tsrGrpList";
            this.tsrGrpList.Size = new System.Drawing.Size(693, 25);
            this.tsrGrpList.TabIndex = 0;
            // 
            // tslblInfo
            // 
            this.tslblInfo.Name = "tslblInfo";
            this.tslblInfo.Size = new System.Drawing.Size(0, 22);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbtnFirst
            // 
            this.tsbtnFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnFirst.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnFirst.Name = "tsbtnFirst";
            this.tsbtnFirst.Size = new System.Drawing.Size(23, 22);
            this.tsbtnFirst.Text = "First";
            this.tsbtnFirst.Click += new System.EventHandler(this.tsmiFirst_Click);
            // 
            // tsbtnPrev
            // 
            this.tsbtnPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnPrev.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnPrev.Name = "tsbtnPrev";
            this.tsbtnPrev.Size = new System.Drawing.Size(23, 22);
            this.tsbtnPrev.Text = "Previous";
            this.tsbtnPrev.Click += new System.EventHandler(this.tsmiPrev_Click);
            // 
            // tsbtnNext
            // 
            this.tsbtnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnNext.Name = "tsbtnNext";
            this.tsbtnNext.Size = new System.Drawing.Size(23, 22);
            this.tsbtnNext.Text = "Next";
            this.tsbtnNext.Click += new System.EventHandler(this.tsmiNext_Click);
            // 
            // tsbtnLast
            // 
            this.tsbtnLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnLast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnLast.Name = "tsbtnLast";
            this.tsbtnLast.Size = new System.Drawing.Size(23, 22);
            this.tsbtnLast.Text = "Last";
            this.tsbtnLast.Click += new System.EventHandler(this.tsmiLast_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbtnAdd
            // 
            this.tsbtnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnAdd.Name = "tsbtnAdd";
            this.tsbtnAdd.Size = new System.Drawing.Size(23, 22);
            this.tsbtnAdd.Text = "Add..";
            this.tsbtnAdd.Click += new System.EventHandler(this.tsmiAdd_Click);
            // 
            // tsbtnDel
            // 
            this.tsbtnDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnDel.Name = "tsbtnDel";
            this.tsbtnDel.Size = new System.Drawing.Size(23, 22);
            this.tsbtnDel.Text = "Delete";
            this.tsbtnDel.Click += new System.EventHandler(this.tsmiDel_Click);
            // 
            // btnDown
            // 
            this.btnDown.BackColor = System.Drawing.SystemColors.Control;
            this.btnDown.Location = new System.Drawing.Point(598, 276);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(75, 23);
            this.btnDown.TabIndex = 9;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = false;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.BackColor = System.Drawing.SystemColors.Control;
            this.btnUp.Location = new System.Drawing.Point(598, 224);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(75, 23);
            this.btnUp.TabIndex = 8;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = false;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // txtName
            // 
            this.txtName.CausesValidation = false;
            this.txtName.InputString = null;
            this.txtName.Location = new System.Drawing.Point(100, 39);
            this.txtName.MaxByteLength = 0;
            this.txtName.MaxLength = 16;
            this.txtName.Name = "txtName";
            this.txtName.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtName.ShortcutsEnabled = false;
            this.txtName.Size = new System.Drawing.Size(140, 23);
            this.txtName.TabIndex = 1;
            this.txtName.Leave += new System.EventHandler(this.txtName_Leave);
            // 
            // grpSelected
            // 
            this.grpSelected.Controls.Add(this.lstSelected);
            this.grpSelected.Location = new System.Drawing.Point(353, 97);
            this.grpSelected.Name = "grpSelected";
            this.grpSelected.Size = new System.Drawing.Size(230, 446);
            this.grpSelected.TabIndex = 7;
            this.grpSelected.TabStop = false;
            this.grpSelected.Text = "Member";
            // 
            // lstSelected
            // 
            this.lstSelected.FormattingEnabled = true;
            this.lstSelected.ItemHeight = 16;
            this.lstSelected.Location = new System.Drawing.Point(25, 25);
            this.lstSelected.Name = "lstSelected";
            this.lstSelected.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstSelected.Size = new System.Drawing.Size(180, 388);
            this.lstSelected.TabIndex = 5;
            this.lstSelected.SelectedIndexChanged += new System.EventHandler(this.lstSelected_SelectedIndexChanged);
            this.lstSelected.DoubleClick += new System.EventHandler(this.lstSelected_DoubleClick);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.Control;
            this.btnAdd.Location = new System.Drawing.Point(252, 224);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(89, 23);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // grpUnselected
            // 
            this.grpUnselected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpUnselected.Controls.Add(this.lstUnselected);
            this.grpUnselected.Location = new System.Drawing.Point(10, 97);
            this.grpUnselected.Name = "grpUnselected";
            this.grpUnselected.Size = new System.Drawing.Size(230, 446);
            this.grpUnselected.TabIndex = 6;
            this.grpUnselected.TabStop = false;
            this.grpUnselected.Text = "Available";
            // 
            // lstUnselected
            // 
            this.lstUnselected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstUnselected.FormattingEnabled = true;
            this.lstUnselected.ItemHeight = 16;
            this.lstUnselected.Location = new System.Drawing.Point(25, 25);
            this.lstUnselected.Name = "lstUnselected";
            this.lstUnselected.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstUnselected.Size = new System.Drawing.Size(180, 388);
            this.lstUnselected.TabIndex = 2;
            // 
            // btnDel
            // 
            this.btnDel.BackColor = System.Drawing.SystemColors.Control;
            this.btnDel.Location = new System.Drawing.Point(252, 276);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(89, 23);
            this.btnDel.TabIndex = 4;
            this.btnDel.Text = "Delete";
            this.btnDel.UseVisualStyleBackColor = false;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // lblName
            // 
            this.lblName.Location = new System.Drawing.Point(21, 39);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(86, 23);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RxGroupListForm
            // 
            this.ClientSize = new System.Drawing.Size(693, 567);
            this.Controls.Add(this.pnlRxGrpList);
            this.Font = new System.Drawing.Font("Arial", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RxGroupListForm";
            this.Text = "Rx Group List";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RxGroupListForm_FormClosing);
            this.Load += new System.EventHandler(this.RxGroupListForm_Load);
            this.pnlRxGrpList.ResumeLayout(false);
            this.pnlRxGrpList.PerformLayout();
            this.tsrGrpList.ResumeLayout(false);
            this.tsrGrpList.PerformLayout();
            this.grpSelected.ResumeLayout(false);
            this.grpUnselected.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

	}

	public void SaveData()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		try
		{
			num3 = Convert.ToInt32(base.Tag);
			if (num3 == -1)
			{
				return;
			}
			if (txtName.Focused)
			{
				txtName_Leave(txtName, null);
			}
			RxListOneData value = new RxListOneData(num3);
			value.Name = txtName.Text;
			num2 = lstSelected.Items.Count;
			ushort[] array = new ushort[num2];
			data.SetIndex(num3, num2 + 1);
			foreach (SelectedItemUtils item in lstSelected.Items)
			{
				array[num++] = (ushort)item.Value;
			}
			value.ContactList = array;
			data[num3] = value;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void DispData()
	{
		int num = 0;
		int num2 = 0;
		ushort num3 = 0;
		string text = "";
		int num4 = 0;
		int num5 = 0;
		try
		{
			num2 = Convert.ToInt32(base.Tag);
			if (num2 == -1)
			{
				Close();
				return;
			}
			txtName.Text = data[num2].Name;
			lstSelected.Items.Clear();
			num4 = data.GetContactCntByIndex(num2);
			for (num = 0; num < num4; num++)
			{
				num3 = data[num2].ContactList[num];
				if (ContactForm.data.DataIsValid(num3 - 1))
				{
					if (ContactForm.data.IsGroupCall(num3 - 1) || ContactForm.data.IsPrivateCall(num3 - 1) || ContactForm.data.IsAllCall(num3 - 1))
					{
						text = ContactForm.data[num3 - 1].Name;
						lstSelected.Items.Add(new SelectedItemUtils(num, num3, text));
					}
					num5++;
				}
			}
			if (num4 != num5 && num5 > 0)
			{
				data.SetIndex(num2, num5 + 1);
			}
			if (lstSelected.Items.Count > 0)
			{
				lstSelected.SelectedIndex = 0;
			}
			lstUnselected.Items.Clear();
			for (num = 0; num < 1024; num++)
			{
				if (!data[num2].ContactList.Contains((ushort)(num + 1)) && ContactForm.data.DataIsValid(num) && (ContactForm.data[num].CallType == 0 || ContactForm.data[num].CallType == 1 || ContactForm.data[num].CallType == 2))
				{
					lstUnselected.Items.Add(new SelectedItemUtils(-1, num + 1, ContactForm.data[num].Name));
				}
			}
			if (lstUnselected.Items.Count > 0)
			{
				lstUnselected.SelectedIndex = 0;
			}
			method_4();
			method_6();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void RefreshName()
	{
		int index = Convert.ToInt32(base.Tag);
		txtName.Text = data[index].Name;
	}

	public RxGroupListForm()
	{
		componentResourceManager = new ComponentResourceManager(typeof(RxGroupListForm));
		InitializeComponent();
		tsbtnDel.Image = (Image)componentResourceManager.GetObject("tsbtnDel.Image");
		tsbtnFirst.Image = (Image)componentResourceManager.GetObject("tsbtnFirst.Image");
		tsbtnLast.Image = (Image)componentResourceManager.GetObject("tsbtnLast.Image");
		tsbtnPrev.Image = (Image)componentResourceManager.GetObject("tsbtnPrev.Image");
		tsbtnAdd.Image = (Image)componentResourceManager.GetObject("tsbtnAdd.Image");
		tsbtnNext.Image = (Image)componentResourceManager.GetObject("tsbtnNext.Image");
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	private void method_1()
	{
		txtName.MaxByteLength = 15;
		txtName.KeyPress += Settings.smethod_54;
	}

	private void RxGroupListForm_Load(object sender, EventArgs e)
	{
		Settings.smethod_59(base.Controls);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		method_1();
		DispData();
	}

	private void RxGroupListForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveData();
	}

	private void btnAdd_Click(object sender, EventArgs e)
	{
		int num = 0;
		int count = lstUnselected.SelectedIndices.Count;
		int num2 = lstUnselected.SelectedIndices[count - 1];
		int num3 = 0;
		lstSelected.SelectedItems.Clear();
		while (lstUnselected.SelectedItems.Count > 0 && lstSelected.Items.Count < 32)
		{
			num = lstSelected.Items.Count;
			SelectedItemUtils selectedItemUtils = (SelectedItemUtils)lstUnselected.SelectedItems[0];
			selectedItemUtils.method_1(num);
			num3 = lstSelected.Items.Add(selectedItemUtils);
			lstSelected.SetSelected(num3, value: true);
			lstUnselected.Items.RemoveAt(lstUnselected.SelectedIndices[0]);
		}
		if (lstUnselected.SelectedItems.Count == 0)
		{
			int num4 = num2 - count + 1;
			if (num4 >= lstUnselected.Items.Count)
			{
				num4 = lstUnselected.Items.Count - 1;
			}
			lstUnselected.SelectedIndex = num4;
		}
		method_3();
		method_4();
		if (!btnAdd.Enabled)
		{
			lstSelected.Focus();
		}
	}

	private void btnDel_Click(object sender, EventArgs e)
	{
		int num = 0;
		int count = lstSelected.SelectedIndices.Count;
		int num2 = lstSelected.SelectedIndices[count - 1];
		lstUnselected.SelectedItems.Clear();
		while (lstSelected.SelectedItems.Count > 0)
		{
			SelectedItemUtils selectedItemUtils = (SelectedItemUtils)lstSelected.SelectedItems[0];
			num = method_2(selectedItemUtils);
			selectedItemUtils.method_1(-1);
			lstUnselected.Items.Insert(num, selectedItemUtils);
			lstUnselected.SetSelected(num, value: true);
			lstSelected.Items.RemoveAt(lstSelected.SelectedIndices[0]);
		}
		int num3 = num2 - count + 1;
		if (num3 >= lstSelected.Items.Count)
		{
			num3 = lstSelected.Items.Count - 1;
		}
		lstSelected.SelectedIndex = num3;
		method_3();
		method_4();
	}

	private void btnUp_Click(object sender = null, EventArgs e = null)
	{
		int num = 0;
		int num2 = 0;
		int count = lstSelected.SelectedIndices.Count;
		_ = lstSelected.SelectedIndices[count - 1];
		for (num = 0; num < count; num++)
		{
			num2 = lstSelected.SelectedIndices[num];
			if (num != num2)
			{
				object value = lstSelected.Items[num2];
				lstSelected.Items[num2] = lstSelected.Items[num2 - 1];
				lstSelected.Items[num2 - 1] = value;
				lstSelected.SetSelected(num2, value: false);
				lstSelected.SetSelected(num2 - 1, value: true);
			}
		}
		method_3();
	}

	private void btnDown_Click(object sender = null, EventArgs e = null)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int count = lstSelected.Items.Count;
		int count2 = lstSelected.SelectedIndices.Count;
		_ = lstSelected.SelectedIndices[count2 - 1];
		num = count2 - 1;
		while (num >= 0)
		{
			num3 = lstSelected.SelectedIndices[num];
			if (count - 1 - num2 != num3)
			{
				object value = lstSelected.Items[num3];
				lstSelected.Items[num3] = lstSelected.Items[num3 + 1];
				lstSelected.Items[num3 + 1] = value;
				lstSelected.SetSelected(num3, value: false);
				lstSelected.SetSelected(num3 + 1, value: true);
			}
			num--;
			num2++;
		}
		method_3();
	}

	private int method_2(SelectedItemUtils class14_0)
	{
		int num = 0;
		num = 0;
		while (true)
		{
			if (num < lstUnselected.Items.Count)
			{
				SelectedItemUtils selectedItemUtils = (SelectedItemUtils)lstUnselected.Items[num];
				if (class14_0.Value < selectedItemUtils.Value)
				{
					break;
				}
				num++;
				continue;
			}
			return num;
		}
		return num;
	}

	private void method_3()
	{
		int num = 0;
		lstSelected.BeginUpdate();
		for (num = 0; num < lstSelected.Items.Count; num++)
		{
			SelectedItemUtils selectedItemUtils = (SelectedItemUtils)lstSelected.Items[num];
			if (selectedItemUtils.method_0() != num)
			{
				selectedItemUtils.method_1(num);
				bool selected = lstSelected.GetSelected(num);
				lstSelected.Items[num] = selectedItemUtils;
				if (selected)
				{
					lstSelected.SetSelected(num, value: true);
				}
			}
		}
		lstSelected.EndUpdate();
	}

	private void method_4()
	{
		btnAdd.Enabled = lstUnselected.Items.Count > 0 && lstSelected.Items.Count < 32;
		btnDel.Enabled = lstSelected.Items.Count > 0;
		int count = lstSelected.Items.Count;
		int count2 = lstSelected.SelectedIndices.Count;
		btnUp.Enabled = lstSelected.SelectedItems.Count > 0 && lstSelected.Items.Count > 0 && lstSelected.SelectedIndices[count2 - 1] != count2 - 1;
		btnDown.Enabled = lstSelected.Items.Count > 0 && lstSelected.SelectedItems.Count > 0 && lstSelected.SelectedIndices[0] != count - count2;
	}

	private void txtName_Leave(object sender, EventArgs e)
	{
		txtName.Text = txtName.Text.Trim();
		if (Node.Text != txtName.Text)
		{
			if (Settings.nodeNameExistsOrEmpty(Node, txtName.Text))
			{
				txtName.Text = Node.Text;
			}
			else
			{
				Node.Text = txtName.Text;
			}
		}
	}

	private void lstSelected_SelectedIndexChanged(object sender, EventArgs e)
	{
		method_4();
	}

	private void lstSelected_DoubleClick(object sender, EventArgs e)
	{
		if (lstSelected.SelectedItem != null)
		{
			SelectedItemUtils selectedItemUtils = lstSelected.SelectedItem as SelectedItemUtils;
			if (base.MdiParent is MainForm mainForm)
			{
				mainForm.DispChildForm(typeof(ContactForm), selectedItemUtils.Value - 1);
			}
		}
	}

	static RxGroupListForm()
	{
		data = new RxListData();
	}

	private void tsmiFirst_Click(object sender, EventArgs e)
	{
		SaveData();
		Node = Node.Parent.FirstNode;
		TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
		base.Tag = treeNodeItem.Index;
		DispData();
	}

	private void handlePreviousClick()
	{
		SaveData();
		if (Node.PrevNode != null)
		{
			Node = Node.PrevNode;
			TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
			base.Tag = treeNodeItem.Index;
			DispData();
		}
	}

	private void tsmiPrev_Click(object sender, EventArgs e)
	{
		handlePreviousClick();
	}

	private void handleNextClick()
	{
		SaveData();
		if (Node.NextNode != null)
		{
			Node = Node.NextNode;
			TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
			base.Tag = treeNodeItem.Index;
			DispData();
		}
	}

	private void tsmiNext_Click(object sender, EventArgs e)
	{
		handleNextClick();
	}

	private void tsmiLast_Click(object sender, EventArgs e)
	{
		SaveData();
		Node = Node.Parent.LastNode;
		TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
		base.Tag = treeNodeItem.Index;
		DispData();
	}

	private void handleInsertClick()
	{
		if (Node.Parent.Nodes.Count < 76)
		{
			SaveData();
			TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
			int minIndex = data.GetMinIndex();
			string minName = data.GetMinName(Node);
			data.SetIndex(minIndex, 1);
			TreeNodeItem tag = new TreeNodeItem(treeNodeItem.Cms, treeNodeItem.Type, null, 0, minIndex, treeNodeItem.ImageIndex, treeNodeItem.Data);
			TreeNode treeNode = new TreeNode(minName);
			treeNode.Tag = tag;
			treeNode.ImageIndex = 19;
			treeNode.SelectedImageIndex = 19;
			Node.Parent.Nodes.Insert(minIndex, treeNode);
			data.SetName(minIndex, minName);
			Node = treeNode;
			base.Tag = minIndex;
			DispData();
			method_7();
		}
	}

	private void tsmiAdd_Click(object sender, EventArgs e)
	{
		handleInsertClick();
	}

	private void handleDeleteClick()
	{
		if (Node.Parent.Nodes.Count > 1 && Node.Index != 0)
		{
			SaveData();
			TreeNode node = Node.NextNode ?? Node.PrevNode;
			TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
			data.ClearIndex(treeNodeItem.Index);
			Node.Remove();
			Node = node;
			TreeNodeItem treeNodeItem2 = Node.Tag as TreeNodeItem;
			base.Tag = treeNodeItem2.Index;
			DispData();
			method_7();
		}
		else
		{
			MessageBox.Show(Settings.dicCommon["FirstNotDelete"]);
		}
	}

	private void tsmiDel_Click(object sender, EventArgs e)
	{
		handleDeleteClick();
	}

	private void method_6()
	{
		tsbtnAdd.Enabled = Node.Parent.Nodes.Count != 76;
		tsbtnDel.Enabled = Node.Parent.Nodes.Count != 1 && Node.Index != 0;
		tsbtnFirst.Enabled = Node != Node.Parent.FirstNode;
		tsbtnPrev.Enabled = Node != Node.Parent.FirstNode;
		tsbtnNext.Enabled = Node != Node.Parent.LastNode;
		tsbtnLast.Enabled = Node != Node.Parent.LastNode;
	}

	private void method_7()
	{
		if (base.MdiParent is MainForm mainForm)
		{
			mainForm.RefreshRelatedForm(typeof(RxGroupListForm));
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Right | Keys.Control:
			handleNextClick();
			return true;
		case Keys.Left | Keys.Control:
			handlePreviousClick();
			return true;
		case Keys.Insert | Keys.Control:
		case Keys.I | Keys.Control:
			handleInsertClick();
			return true;
		case Keys.Delete | Keys.Control:
			handleDeleteClick();
			return true;
		case Keys.Up | Keys.Control:
			btnUp_Click();
			return true;
		case Keys.Down | Keys.Control:
			btnDown_Click();
			return true;
		default:
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}


}
