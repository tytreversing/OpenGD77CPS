using System;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class ZoneBasicForm : DockContent, IDisp
{
	private CustomPanel pnlZoneBasic;

	private const int ZONE_NAME_LENGTH = 16;

	private const int ZONES_IN_USE_DATA_LENGTH = 32;

	private const int NUM_CHANNELS_PER_ZONE = 80;

	private const int NUM_ZONES = 68;

	private GroupBox grpSub;

	private CustomCombo cmbSubCh;

	private Label lblSubCh;

	private CustomCombo cmbSubZone;

	private Label lblSubZone;

	private GroupBox grpMain;

	private CustomCombo cmbMainCh;

	private Label lblMainCh;

	private CustomCombo cmbMainZone;

	private Label lblCurZone;

	private Button btnDown;

	private Button btnUp;

	private ListBox lstZones;

	private const int UNKNOWN_VAR_OF_32 = 96;

	private int _selectedIndex;

	public int MainZoneLastSelectedIndex { get; set; }

	public int SubZoneLastSelectedIndex { get; set; }

	public TreeNode Node { get; set; }

	public void SaveData()
	{
		if (ChannelForm.CurCntCh > 128)
		{
			ZoneForm.basicData.CurZone = cmbMainZone.method_3();
			ZoneForm.basicData.MainCh = cmbMainCh.method_3();
			ZoneForm.basicData.SubCh = cmbSubCh.method_3();
			ZoneForm.basicData.SubZone = cmbSubZone.method_3();
		}
		else
		{
			ZoneForm.basicData.MainCh = cmbMainCh.method_3();
			ZoneForm.basicData.SubCh = cmbSubCh.method_3();
		}
		((MainForm)base.MdiParent).RefreshRelatedForm(GetType());
	}

	public void DispData()
	{
		method_2(cmbMainZone);
		int curZone = ZoneForm.basicData.CurZone;
		cmbMainZone.method_2(ZoneForm.basicData.CurZone);
		method_3(curZone);
		cmbMainCh.method_2(ZoneForm.basicData.MainCh);
		method_2(cmbSubZone);
		int subZone = ZoneForm.basicData.SubZone;
		cmbSubZone.method_2(ZoneForm.basicData.SubZone);
		method_4(subZone);
		cmbSubCh.method_2(ZoneForm.basicData.SubCh);
		lstZones.Items.Clear();
		for (curZone = 0; curZone < 68; curZone++)
		{
			if (ZoneForm.data.DataIsValid(curZone))
			{
				lstZones.Items.Add(new ListItem
				{
					Name = ZoneForm.data[curZone].Name,
					Value = curZone
				});
			}
		}
		lstZones.SelectedIndex = _selectedIndex;
	}

	public void RefreshName()
	{
	}

	public ZoneBasicForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	private void method_1()
	{
		method_2(cmbMainZone);
		method_2(cmbSubZone);
	}

	private void method_2(CustomCombo class4_0)
	{
		int num = 0;
		class4_0.method_0();
		for (num = 0; num <= 250; num++)
		{
			if (ZoneForm.data.ZoneChIsValid(num))
			{
				class4_0.method_1(ZoneForm.data.GetName(num), num);
			}
		}
	}

	private void method_3(int int_0)
	{
		method_5(int_0, cmbMainCh);
	}

	private void method_4(int int_0)
	{
		method_5(int_0, cmbSubCh);
	}

	private void method_5(int int_0, CustomCombo class4_0)
	{
		class4_0.method_0();
		for (int i = 0; i < 80; i++)
		{
			int num = ZoneForm.data[int_0].ChList[i] - 1;
			if (num >= 0 && num < ChannelForm.CurCntCh && ChannelForm.data.DataIsValid(num))
			{
				class4_0.method_1(ChannelForm.data.GetName(num), i);
			}
		}
	}

	private void method_6()
	{
		cmbSubCh.method_0();
		for (int i = 0; i < ChannelForm.CurCntCh; i++)
		{
			if (ChannelForm.data.DataIsValid(i))
			{
				cmbSubCh.method_1(ChannelForm.data.GetName(i), i + 1);
			}
		}
	}

	private void ZoneBasicForm_Load(object sender, EventArgs e)
	{
		Settings.smethod_59(base.Controls);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		DispData();
	}

	private void ZoneBasicForm_FormClosing(object sender, FormClosingEventArgs e)
	{
	}

	private void cmbMainZone_SelectedIndexChanged(object sender, EventArgs e)
	{
		int num = cmbMainZone.method_3();
		if (num != MainZoneLastSelectedIndex)
		{
			method_3(num);
			if (cmbMainCh.Items.Count > 0)
			{
				cmbMainCh.SelectedIndex = 0;
			}
		}
	}

	private void cmbSubZone_SelectedIndexChanged(object sender, EventArgs e)
	{
		int num = cmbSubZone.method_3();
		if (num != SubZoneLastSelectedIndex)
		{
			method_4(num);
			if (cmbSubCh.Items.Count > 0)
			{
				cmbSubCh.SelectedIndex = 0;
			}
		}
	}

	private void cmbMainZone_DropDown(object sender, EventArgs e)
	{
		MainZoneLastSelectedIndex = cmbMainZone.method_3();
	}

	private void cmbSubZone_DropDown(object sender, EventArgs e)
	{
		SubZoneLastSelectedIndex = cmbSubZone.method_3();
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
            this.pnlZoneBasic = new CustomPanel();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.lstZones = new System.Windows.Forms.ListBox();
            this.grpSub = new System.Windows.Forms.GroupBox();
            this.cmbSubCh = new CustomCombo();
            this.lblSubCh = new System.Windows.Forms.Label();
            this.cmbSubZone = new CustomCombo();
            this.lblSubZone = new System.Windows.Forms.Label();
            this.grpMain = new System.Windows.Forms.GroupBox();
            this.cmbMainCh = new CustomCombo();
            this.lblMainCh = new System.Windows.Forms.Label();
            this.cmbMainZone = new CustomCombo();
            this.lblCurZone = new System.Windows.Forms.Label();
            this.pnlZoneBasic.SuspendLayout();
            this.grpSub.SuspendLayout();
            this.grpMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlZoneBasic
            // 
            this.pnlZoneBasic.AutoScroll = true;
            this.pnlZoneBasic.AutoSize = true;
            this.pnlZoneBasic.Controls.Add(this.btnDown);
            this.pnlZoneBasic.Controls.Add(this.btnUp);
            this.pnlZoneBasic.Controls.Add(this.lstZones);
            this.pnlZoneBasic.Controls.Add(this.grpSub);
            this.pnlZoneBasic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlZoneBasic.Location = new System.Drawing.Point(0, 0);
            this.pnlZoneBasic.Name = "pnlZoneBasic";
            this.pnlZoneBasic.Size = new System.Drawing.Size(341, 458);
            this.pnlZoneBasic.TabIndex = 0;
            // 
            // btnDown
            // 
            this.btnDown.AccessibleDescription = "Down";
            this.btnDown.AccessibleName = "Down";
            this.btnDown.BackColor = System.Drawing.SystemColors.Control;
            this.btnDown.Location = new System.Drawing.Point(211, 70);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(75, 23);
            this.btnDown.TabIndex = 14;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = false;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.AccessibleDescription = "Up";
            this.btnUp.AccessibleName = "Up";
            this.btnUp.BackColor = System.Drawing.SystemColors.Control;
            this.btnUp.Location = new System.Drawing.Point(211, 30);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(75, 23);
            this.btnUp.TabIndex = 13;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = false;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // lstZones
            // 
            this.lstZones.AccessibleDescription = "Zones";
            this.lstZones.AccessibleName = "Zones";
            this.lstZones.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstZones.FormattingEnabled = true;
            this.lstZones.ItemHeight = 16;
            this.lstZones.Location = new System.Drawing.Point(28, 12);
            this.lstZones.Name = "lstZones";
            this.lstZones.Size = new System.Drawing.Size(160, 388);
            this.lstZones.TabIndex = 12;
            // 
            // grpSub
            // 
            this.grpSub.Controls.Add(this.cmbSubCh);
            this.grpSub.Controls.Add(this.lblSubCh);
            this.grpSub.Controls.Add(this.cmbSubZone);
            this.grpSub.Controls.Add(this.lblSubZone);
            this.grpSub.Controls.Add(this.grpMain);
            this.grpSub.Location = new System.Drawing.Point(194, 271);
            this.grpSub.Name = "grpSub";
            this.grpSub.Size = new System.Drawing.Size(142, 129);
            this.grpSub.TabIndex = 7;
            this.grpSub.TabStop = false;
            this.grpSub.Text = "Down";
            this.grpSub.Visible = false;
            // 
            // cmbSubCh
            // 
            this.cmbSubCh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSubCh.FormattingEnabled = true;
            this.cmbSubCh.Location = new System.Drawing.Point(6, 72);
            this.cmbSubCh.Name = "cmbSubCh";
            this.cmbSubCh.Size = new System.Drawing.Size(120, 24);
            this.cmbSubCh.TabIndex = 5;
            // 
            // lblSubCh
            // 
            this.lblSubCh.Location = new System.Drawing.Point(14, 72);
            this.lblSubCh.Name = "lblSubCh";
            this.lblSubCh.Size = new System.Drawing.Size(70, 24);
            this.lblSubCh.TabIndex = 4;
            this.lblSubCh.Text = "Channel";
            this.lblSubCh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbSubZone
            // 
            this.cmbSubZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSubZone.FormattingEnabled = true;
            this.cmbSubZone.Location = new System.Drawing.Point(6, 37);
            this.cmbSubZone.Name = "cmbSubZone";
            this.cmbSubZone.Size = new System.Drawing.Size(120, 24);
            this.cmbSubZone.TabIndex = 1;
            this.cmbSubZone.DropDown += new System.EventHandler(this.cmbSubZone_DropDown);
            this.cmbSubZone.SelectedIndexChanged += new System.EventHandler(this.cmbSubZone_SelectedIndexChanged);
            // 
            // lblSubZone
            // 
            this.lblSubZone.Location = new System.Drawing.Point(14, 37);
            this.lblSubZone.Name = "lblSubZone";
            this.lblSubZone.Size = new System.Drawing.Size(70, 24);
            this.lblSubZone.TabIndex = 0;
            this.lblSubZone.Text = "Zone";
            this.lblSubZone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // grpMain
            // 
            this.grpMain.Controls.Add(this.cmbMainCh);
            this.grpMain.Controls.Add(this.lblMainCh);
            this.grpMain.Controls.Add(this.cmbMainZone);
            this.grpMain.Controls.Add(this.lblCurZone);
            this.grpMain.Location = new System.Drawing.Point(0, 0);
            this.grpMain.Name = "grpMain";
            this.grpMain.Size = new System.Drawing.Size(92, 129);
            this.grpMain.TabIndex = 6;
            this.grpMain.TabStop = false;
            this.grpMain.Text = "Up";
            this.grpMain.Visible = false;
            // 
            // cmbMainCh
            // 
            this.cmbMainCh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMainCh.FormattingEnabled = true;
            this.cmbMainCh.Location = new System.Drawing.Point(17, 71);
            this.cmbMainCh.Name = "cmbMainCh";
            this.cmbMainCh.Size = new System.Drawing.Size(120, 24);
            this.cmbMainCh.TabIndex = 3;
            // 
            // lblMainCh
            // 
            this.lblMainCh.Location = new System.Drawing.Point(22, 70);
            this.lblMainCh.Name = "lblMainCh";
            this.lblMainCh.Size = new System.Drawing.Size(70, 24);
            this.lblMainCh.TabIndex = 2;
            this.lblMainCh.Text = "Channel";
            this.lblMainCh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbMainZone
            // 
            this.cmbMainZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMainZone.FormattingEnabled = true;
            this.cmbMainZone.Location = new System.Drawing.Point(17, 37);
            this.cmbMainZone.Name = "cmbMainZone";
            this.cmbMainZone.Size = new System.Drawing.Size(120, 24);
            this.cmbMainZone.TabIndex = 1;
            this.cmbMainZone.DropDown += new System.EventHandler(this.cmbMainZone_DropDown);
            this.cmbMainZone.SelectedIndexChanged += new System.EventHandler(this.cmbMainZone_SelectedIndexChanged);
            // 
            // lblCurZone
            // 
            this.lblCurZone.Location = new System.Drawing.Point(22, 37);
            this.lblCurZone.Name = "lblCurZone";
            this.lblCurZone.Size = new System.Drawing.Size(70, 24);
            this.lblCurZone.TabIndex = 0;
            this.lblCurZone.Text = "Zone";
            this.lblCurZone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ZoneBasicForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(341, 458);
            this.Controls.Add(this.pnlZoneBasic);
            this.Font = new System.Drawing.Font("Arial", 10F);
            this.Name = "ZoneBasicForm";
            this.Text = "Zone";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ZoneBasicForm_FormClosing);
            this.Load += new System.EventHandler(this.ZoneBasicForm_Load);
            this.pnlZoneBasic.ResumeLayout(false);
            this.grpSub.ResumeLayout(false);
            this.grpMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

	}

	private void btnUp_Click(object sender, EventArgs e)
	{
		handleUp();
	}

	private void btnDown_Click(object sender, EventArgs e)
	{
		handleDown();
	}

	private void handleDown()
	{
		int selectedIndex = lstZones.SelectedIndex;
		if (selectedIndex != -1 && selectedIndex != lstZones.Items.Count - 1)
		{
			ZoneForm.MoveZoneDown(selectedIndex);
			_selectedIndex = selectedIndex + 1;
			DispData();
			(base.MdiParent as MainForm).InitTree();
		}
	}

	private void handleUp()
	{
		int selectedIndex = lstZones.SelectedIndex;
		if (selectedIndex > 0)
		{
			ZoneForm.MoveZoneUp(selectedIndex);
			_selectedIndex = selectedIndex - 1;
			DispData();
			(base.MdiParent as MainForm).InitTree();
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.D | Keys.Control:
			handleDown();
			return true;
		case Keys.U | Keys.Control:
			handleUp();
			return true;
		default:
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
