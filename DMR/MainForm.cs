using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Shapes;
using System.Xml;
using DMR.Properties;
using Extras.OpenGD77;
using WeifenLuo.WinFormsUI.Docking;
using Path = System.IO.Path;



namespace DMR;

public class MainForm : Form
{
    public enum RadioTypeEnum
	{
		RadioTypeMK22,
		RadioTypeSTM32
	}

	public enum RadioInfoPlatformEnum
	{
		PLATFORM_GD77,
		PLATFORM_GD77S,
		PLATFORM_DM1801,
		PLATFORM_RD5R,
		PLATFORM_DM1801A,
		PLATFORM_MD9600,
		PLATFORM_MDUV380,
		PLATFORM_MD380,
		PLATFORM_DM1701_BGR,
		PLATFORM_MD2017,
		PLATFORM_DM1701_RGB
	}

	public static string Language_Name;

	private const string DEFAULT_DATA_FILE_NAME = "OpenGD77RUS.ogd";

	public const int LIBREDMR_CODEPLUG_VERSION = 1;

	public static byte[] CommsBuffer;

	private static string PRODUCT_NAME;

	public static string PRODUCT_VERSION;

	public static string[] FirmwareLanguageFiles;

	public static RadioTypeEnum RadioType;

	public static OpenGD77Form.RadioInfo RadioInfo;

	private IContainer components;

	private MenuStrip mnsMain;

	private ToolStripMenuItem tsmiFile;

	private ToolStripMenuItem tsmiImportExport;

	private ToolStripMenuItem tsmiSetting;

	private ToolStripMenuItem tsmiDtmf;

	private ToolStripMenuItem tsmiEmgSystem;

	private ToolStripMenuItem tsmiContact;

	private ToolStripMenuItem tsmiAPRSConfigs;

	private ToolStripMenuItem tsmiEncrypt;

	private ToolStripMenuItem tsmiTextMsg;

	private ToolStripMenuItem tsmiGerneralSet;

	private ToolStripMenuItem tsmiGrpRxList;

	private ToolStripMenuItem tsmiChannels;

	private ToolStripMenuItem tsmiZone;

	private ToolStripMenuItem tsmiButton;

	private ToolStripMenuItem tsmiVfos;

	private ToolStripMenuItem tsmiVfoA;

	private ToolStripMenuItem tsmiVfoB;

	private ToolStripMenuItem tsmiCodeplugSettings;

	private ToolStripMenuItem tsmiAbout;

	private ContextMenuStrip cmsGroup;

	private ToolStripMenuItem tsmiAdd;

	private ContextMenuStrip cmsSub;

	private ToolStripMenuItem tsmiDel;

	private ToolStripMenuItem tsmiRename;

	private ToolStripMenuItem tsmiWindow;

	private ToolStripMenuItem tsmiCascade;

	private ToolStripMenuItem tsmiTileHor;

	private ToolStripMenuItem tsmiTileVer;

	private ToolStripMenuItem tsmiExit;

	private ToolStripMenuItem tsmiProgram;

	private ToolStripMenuItem tsmiRead;

	private ToolStripMenuItem tsmiWrite;

	private ToolStripMenuItem tsmiNew;

	private ToolStripMenuItem tsmiSave;

	private ToolStripMenuItem tsmiOpen;

    private ToolStripMenuItem tsmiImport;

    private ToolStripMenuItem tsmiExportCSV;

	private ToolStripMenuItem tsmiImportCSV;

	private ToolStripMenuItem tsmiAppendCSV;

	private ToolStripMenuItem tsmiUpdateLocationCSV;

	private ToolStripSeparator toolStripSeparator1;

	private ContextMenuStrip cmsGroupContact;

	private ToolStripMenuItem tsmiAddContact;

	private ToolStripMenuItem tsmiGroupCall;

	private ToolStripMenuItem tsmiPrivateCall;

	private ToolStripMenuItem tsmiAllCall;

	private ImageList imgMain;

	private OpenFileDialog ofdMain;

	private SaveFileDialog sfdMain;

	private ToolStripMenuItem tsmiDtmfContact;

	private ToolStripMenuItem tsmiDmrContacts;

	private ToolStripMenuItem tsmiZoneBasic;

	private ToolStripMenuItem tsmiZoneList;

	private ToolStripMenuItem tsmiCloseAll;

	private ToolStripMenuItem tsmiDeviceInfo;

	private ContextMenuStrip cmsTree;

	private ToolStripMenuItem tsmiExpandAll;

	private ToolStripMenuItem tsmiCollapseAll;

    private DockPanel dockPanel;

	private Panel pnlTvw;

	private TreeView tvwMain;

	private ToolStripMenuItem tsmiView;

	private ToolStripMenuItem tsmiTree;

	private ToolStripMenuItem tsmiHelp;

	private ToolStripMenuItem tsmiMenu;

	private ToolStripMenuItem tsmiBootItem;

	private ToolStripMenuItem tsmiNumKeyContact;

	private StatusStrip ssrMain;

	private ToolStripStatusLabel slblCompany;

	private ToolStrip tsrMain;

	private ToolStripButton tsbtnNew;

	private ToolStripButton tsbtnOpen;

	private ToolStripButton tsbtnSave;

	private ToolStripSeparator toolStripSeparator2;

	private ToolStripButton tsbtnRead;

	private ToolStripButton tsbtnWrite;

	private ToolStripSeparator toolStripSeparator3;

	private ToolStripButton tsbtnAbout;

	private ToolStripMenuItem tsmiClear;

	private ToolStripSeparator toolStripSeparator4;

	private ToolStripMenuItem tsmiCopy;

	private ToolStripMenuItem tsmiMoveUp;

	private ToolStripMenuItem tsmiMoveDown;

	private ToolStripMenuItem tsmiPaste;

	private ToolStripMenuItem tsmiToolBar;

	private ToolStripMenuItem tsmiStatusBar;

	private ToolStripMenuItem tsmiBasic;

	private ToolStripMenuItem tsmiLanguage;

	private ToolStripMenuItem tsmiExtras;

	private ToolStripMenuItem tsmiContactsDownload;

	private ToolStripMenuItem tsmiDMRID;

	private ToolStripMenuItem tsmiCalibrationMK22;

	private ToolStripMenuItem tsmiOpenGD77;

	private ToolStripMenuItem tsmiFirmwareLoader;

	private ToolStripMenuItem tsmiRadioType;

	private ToolStripMenuItem tsmiRadioTypeItem_MK22;

	private ToolStripMenuItem tsmiRadioTypeItem_STM32;

	private ToolStripMenuItem tsmiTheme;

	private DeserializeDockContent m_deserializeDockContent;

	private static IDisp PreActiveMdiChild;

	private HelpForm frmHelp;

	private TreeForm frmTree;

	private TreeNodeItem CopyItem;

	private List<TreeNode> lstFixedNode;

	private TextBox _TextBox;

	private static Dictionary<string, string> dicHelp;

	private static Dictionary<string, string> dicTree;

	private List<TreeNodeItem> lstTreeNodeItem;

	private string _lastCodeplugFileName = string.Empty;

	public static string[] StartupArgs;
    private ToolStripMenuItem tsmiImportG77;
    private ToolStripSeparator toolStripSeparator5;
    private OpenFileDialog importFileDialog;
    public Label radioInformation;
    private Timer pingTimer;
    private ToolStripSeparator toolStripSeparator6;
    private ToolStripMenuItem tsmiSetup;
    private ToolStripButton tsbtnDMR;
    private ToolStripSeparator toolStripSeparator7;
    private ToolStripButton tsbtnSettings;
    private ToolStripButton tsbtnPalette;
    private ToolStripButton toolStripButton1;
    private ToolStripButton tsbtnOpenGD;
    public static bool EnableHiddenFeatures;
    public SaveFileDialog saveFileDialog;
    public static bool messageShown = false;

	public static string CurFileName { get; set; }

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
            this.components = new System.ComponentModel.Container();
            this.imgMain = new System.Windows.Forms.ImageList(this.components);
            this.mnsMain = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNew = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImportG77 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiImportExport = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImportCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAppendCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUpdateLocationCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiProgram = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRead = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiWrite = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTree = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiToolBar = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiStatusBar = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRadioType = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRadioTypeItem_MK22 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRadioTypeItem_STM32 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExtras = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDMRID = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenGD77 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFirmwareLoader = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCalibrationMK22 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTheme = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSetup = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCascade = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTileHor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTileVer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCloseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImport = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeviceInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBootItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNumKeyContact = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiGerneralSet = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiButton = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTextMsg = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEncrypt = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDtmf = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEmgSystem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiContact = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDtmfContact = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDmrContacts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAPRSConfigs = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiGrpRxList = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiZone = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiZoneBasic = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiZoneList = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiChannels = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiVfos = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiVfoA = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiVfoB = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCodeplugSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBasic = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiContactsDownload = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsGroup = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClear = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsSub = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiDel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRename = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsGroupContact = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddContact = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiGroupCall = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPrivateCall = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAllCall = new System.Windows.Forms.ToolStripMenuItem();
            this.ofdMain = new System.Windows.Forms.OpenFileDialog();
            this.sfdMain = new System.Windows.Forms.SaveFileDialog();
            this.cmsTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.pnlTvw = new System.Windows.Forms.Panel();
            this.tvwMain = new System.Windows.Forms.TreeView();
            this.ssrMain = new System.Windows.Forms.StatusStrip();
            this.slblCompany = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsrMain = new System.Windows.Forms.ToolStrip();
            this.tsbtnNew = new System.Windows.Forms.ToolStripButton();
            this.tsbtnOpen = new System.Windows.Forms.ToolStripButton();
            this.tsbtnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnRead = new System.Windows.Forms.ToolStripButton();
            this.tsbtnWrite = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnDMR = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.tsbtnOpenGD = new System.Windows.Forms.ToolStripButton();
            this.tsbtnPalette = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnSettings = new System.Windows.Forms.ToolStripButton();
            this.tsbtnAbout = new System.Windows.Forms.ToolStripButton();
            this.importFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.radioInformation = new System.Windows.Forms.Label();
            this.pingTimer = new System.Windows.Forms.Timer(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.mnsMain.SuspendLayout();
            this.cmsGroup.SuspendLayout();
            this.cmsSub.SuspendLayout();
            this.cmsGroupContact.SuspendLayout();
            this.cmsTree.SuspendLayout();
            this.pnlTvw.SuspendLayout();
            this.ssrMain.SuspendLayout();
            this.tsrMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgMain
            // 
            this.imgMain.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imgMain.ImageSize = new System.Drawing.Size(16, 16);
            this.imgMain.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // mnsMain
            // 
            this.mnsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile,
            this.tsmiSetting,
            this.tsmiProgram,
            this.tsmiView,
            this.tsmiRadioType,
            this.tsmiExtras,
            this.tsmiLanguage,
            this.tsmiWindow,
            this.tsmiAbout});
            this.mnsMain.Location = new System.Drawing.Point(234, 0);
            this.mnsMain.MdiWindowListItem = this.tsmiWindow;
            this.mnsMain.Name = "mnsMain";
            this.mnsMain.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.mnsMain.Size = new System.Drawing.Size(865, 29);
            this.mnsMain.TabIndex = 4;
            // 
            // tsmiFile
            // 
            this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiNew,
            this.tsmiSave,
            this.tsmiOpen,
            this.tsmiImportG77,
            this.toolStripSeparator5,
            this.tsmiImportExport,
            this.toolStripSeparator1,
            this.tsmiExit});
            this.tsmiFile.Name = "tsmiFile";
            this.tsmiFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.tsmiFile.Size = new System.Drawing.Size(41, 23);
            this.tsmiFile.Text = "File";
            // 
            // tsmiNew
            // 
            this.tsmiNew.Name = "tsmiNew";
            this.tsmiNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.tsmiNew.Size = new System.Drawing.Size(281, 24);
            this.tsmiNew.Text = "New";
            this.tsmiNew.Click += new System.EventHandler(this.tsbtnNew_Click);
            // 
            // tsmiSave
            // 
            this.tsmiSave.Name = "tsmiSave";
            this.tsmiSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.tsmiSave.Size = new System.Drawing.Size(281, 24);
            this.tsmiSave.Text = "Save";
            this.tsmiSave.Click += new System.EventHandler(this.tsbtnSave_Click);
            // 
            // tsmiOpen
            // 
            this.tsmiOpen.Name = "tsmiOpen";
            this.tsmiOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.tsmiOpen.Size = new System.Drawing.Size(281, 24);
            this.tsmiOpen.Text = "Open";
            this.tsmiOpen.Click += new System.EventHandler(this.tsbtnOpen_Click);
            // 
            // tsmiImportG77
            // 
            this.tsmiImportG77.Name = "tsmiImportG77";
            this.tsmiImportG77.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.tsmiImportG77.Size = new System.Drawing.Size(281, 24);
            this.tsmiImportG77.Text = "Import from OpenGD77";
            this.tsmiImportG77.Click += new System.EventHandler(this.tsmiImportG77_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(278, 6);
            // 
            // tsmiImportExport
            // 
            this.tsmiImportExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiExportCSV,
            this.tsmiImportCSV,
            this.tsmiAppendCSV,
            this.tsmiUpdateLocationCSV});
            this.tsmiImportExport.Name = "tsmiImportExport";
            this.tsmiImportExport.Size = new System.Drawing.Size(281, 24);
            this.tsmiImportExport.Text = "CSV";
            // 
            // tsmiExportCSV
            // 
            this.tsmiExportCSV.Name = "tsmiExportCSV";
            this.tsmiExportCSV.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.E)));
            this.tsmiExportCSV.Size = new System.Drawing.Size(339, 24);
            this.tsmiExportCSV.Text = "Export to CSV";
            this.tsmiExportCSV.Click += new System.EventHandler(this.tsbtnExportCSV_Click);
            // 
            // tsmiImportCSV
            // 
            this.tsmiImportCSV.Name = "tsmiImportCSV";
            this.tsmiImportCSV.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.I)));
            this.tsmiImportCSV.Size = new System.Drawing.Size(339, 24);
            this.tsmiImportCSV.Text = "Import from CSV";
            this.tsmiImportCSV.Click += new System.EventHandler(this.tsbtnImportCSV_Click);
            // 
            // tsmiAppendCSV
            // 
            this.tsmiAppendCSV.Name = "tsmiAppendCSV";
            this.tsmiAppendCSV.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.A)));
            this.tsmiAppendCSV.Size = new System.Drawing.Size(339, 24);
            this.tsmiAppendCSV.Text = "Append from CSV";
            this.tsmiAppendCSV.Click += new System.EventHandler(this.tsbtnAppendCSV_Click);
            // 
            // tsmiUpdateLocationCSV
            // 
            this.tsmiUpdateLocationCSV.Name = "tsmiUpdateLocationCSV";
            this.tsmiUpdateLocationCSV.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.L)));
            this.tsmiUpdateLocationCSV.Size = new System.Drawing.Size(339, 24);
            this.tsmiUpdateLocationCSV.Text = "Update location from CSV";
            this.tsmiUpdateLocationCSV.Click += new System.EventHandler(this.tsbtnUpdateLocationCSV_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(278, 6);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.tsmiExit.Size = new System.Drawing.Size(281, 24);
            this.tsmiExit.Text = "Exit";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // tsmiSetting
            // 
            this.tsmiSetting.Name = "tsmiSetting";
            this.tsmiSetting.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Y)));
            this.tsmiSetting.Size = new System.Drawing.Size(64, 23);
            this.tsmiSetting.Text = "Setting";
            this.tsmiSetting.DropDownOpening += new System.EventHandler(this.tsmiSetting_DropDownOpening);
            // 
            // tsmiProgram
            // 
            this.tsmiProgram.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiRead,
            this.tsmiWrite});
            this.tsmiProgram.Name = "tsmiProgram";
            this.tsmiProgram.Size = new System.Drawing.Size(74, 23);
            this.tsmiProgram.Text = "Program";
            // 
            // tsmiRead
            // 
            this.tsmiRead.Name = "tsmiRead";
            this.tsmiRead.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.tsmiRead.Size = new System.Drawing.Size(174, 24);
            this.tsmiRead.Text = "Read";
            this.tsmiRead.Click += new System.EventHandler(this.tsbtnRead_Click);
            // 
            // tsmiWrite
            // 
            this.tsmiWrite.Name = "tsmiWrite";
            this.tsmiWrite.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.tsmiWrite.Size = new System.Drawing.Size(174, 24);
            this.tsmiWrite.Text = "Write";
            this.tsmiWrite.Click += new System.EventHandler(this.tsbtnWrite_Click);
            // 
            // tsmiView
            // 
            this.tsmiView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTree,
            this.tsmiHelp,
            this.tsmiToolBar,
            this.tsmiStatusBar});
            this.tsmiView.Name = "tsmiView";
            this.tsmiView.Size = new System.Drawing.Size(50, 23);
            this.tsmiView.Text = "View";
            // 
            // tsmiTree
            // 
            this.tsmiTree.Checked = true;
            this.tsmiTree.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiTree.Name = "tsmiTree";
            this.tsmiTree.Size = new System.Drawing.Size(140, 24);
            this.tsmiTree.Text = "TreeView";
            this.tsmiTree.Click += new System.EventHandler(this.tsmiTree_Click);
            // 
            // tsmiHelp
            // 
            this.tsmiHelp.Name = "tsmiHelp";
            this.tsmiHelp.Size = new System.Drawing.Size(140, 24);
            this.tsmiHelp.Text = "HelpView";
            this.tsmiHelp.Click += new System.EventHandler(this.tsmiHelp_Click);
            // 
            // tsmiToolBar
            // 
            this.tsmiToolBar.Checked = true;
            this.tsmiToolBar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiToolBar.Name = "tsmiToolBar";
            this.tsmiToolBar.Size = new System.Drawing.Size(140, 24);
            this.tsmiToolBar.Text = "Toolbar";
            this.tsmiToolBar.Click += new System.EventHandler(this.tsmiToolBar_Click);
            // 
            // tsmiStatusBar
            // 
            this.tsmiStatusBar.Checked = true;
            this.tsmiStatusBar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiStatusBar.Name = "tsmiStatusBar";
            this.tsmiStatusBar.Size = new System.Drawing.Size(140, 24);
            this.tsmiStatusBar.Text = "Status Bar";
            this.tsmiStatusBar.Click += new System.EventHandler(this.tsmiStatusBar_Click);
            // 
            // tsmiRadioType
            // 
            this.tsmiRadioType.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiRadioTypeItem_MK22,
            this.tsmiRadioTypeItem_STM32});
            this.tsmiRadioType.Name = "tsmiRadioType";
            this.tsmiRadioType.Size = new System.Drawing.Size(87, 23);
            this.tsmiRadioType.Text = "Radio Type";
            this.tsmiRadioType.Visible = false;
            // 
            // tsmiRadioTypeItem_MK22
            // 
            this.tsmiRadioTypeItem_MK22.Name = "tsmiRadioTypeItem_MK22";
            this.tsmiRadioTypeItem_MK22.Size = new System.Drawing.Size(295, 24);
            this.tsmiRadioTypeItem_MK22.Tag = DMR.MainForm.RadioTypeEnum.RadioTypeMK22;
            this.tsmiRadioTypeItem_MK22.Text = "GD77/GD77S/DM1801/RD5R";
            this.tsmiRadioTypeItem_MK22.Click += new System.EventHandler(this.tsmiRadioTypeClickHandler);
            // 
            // tsmiRadioTypeItem_STM32
            // 
            this.tsmiRadioTypeItem_STM32.Checked = true;
            this.tsmiRadioTypeItem_STM32.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiRadioTypeItem_STM32.Name = "tsmiRadioTypeItem_STM32";
            this.tsmiRadioTypeItem_STM32.Size = new System.Drawing.Size(295, 24);
            this.tsmiRadioTypeItem_STM32.Tag = DMR.MainForm.RadioTypeEnum.RadioTypeSTM32;
            this.tsmiRadioTypeItem_STM32.Text = "MD-9600/RT-90,MD-UV380/RT-3S";
            this.tsmiRadioTypeItem_STM32.Click += new System.EventHandler(this.tsmiRadioTypeClickHandler);
            // 
            // tsmiExtras
            // 
            this.tsmiExtras.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDMRID,
            this.tsmiOpenGD77,
            this.tsmiFirmwareLoader,
            this.tsmiCalibrationMK22,
            this.tsmiTheme,
            this.toolStripSeparator6,
            this.tsmiSetup});
            this.tsmiExtras.Name = "tsmiExtras";
            this.tsmiExtras.Size = new System.Drawing.Size(57, 23);
            this.tsmiExtras.Text = "Extras";
            // 
            // tsmiDMRID
            // 
            this.tsmiDMRID.Name = "tsmiDMRID";
            this.tsmiDMRID.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.tsmiDMRID.Size = new System.Drawing.Size(257, 24);
            this.tsmiDMRID.Text = "DMR ID";
            this.tsmiDMRID.ToolTipText = "Загрузка и редактирование базы DMR ID";
            this.tsmiDMRID.Click += new System.EventHandler(this.tsbtnDMRID_Click);
            // 
            // tsmiOpenGD77
            // 
            this.tsmiOpenGD77.Name = "tsmiOpenGD77";
            this.tsmiOpenGD77.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.tsmiOpenGD77.Size = new System.Drawing.Size(257, 24);
            this.tsmiOpenGD77.Text = "OpenGD77 support";
            this.tsmiOpenGD77.ToolTipText = "Общий обмен данными с рацией";
            this.tsmiOpenGD77.Click += new System.EventHandler(this.tsmiOpenGD77_Click);
            // 
            // tsmiFirmwareLoader
            // 
            this.tsmiFirmwareLoader.Name = "tsmiFirmwareLoader";
            this.tsmiFirmwareLoader.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.tsmiFirmwareLoader.Size = new System.Drawing.Size(257, 24);
            this.tsmiFirmwareLoader.Text = "Firmware loader";
            this.tsmiFirmwareLoader.ToolTipText = "Загрузчик прошивки OpenGD77 RUS";
            this.tsmiFirmwareLoader.Click += new System.EventHandler(this.tsmiFirmwareLoader_Click);
            // 
            // tsmiCalibrationMK22
            // 
            this.tsmiCalibrationMK22.Name = "tsmiCalibrationMK22";
            this.tsmiCalibrationMK22.Size = new System.Drawing.Size(257, 24);
            this.tsmiCalibrationMK22.Text = "Calibration editor";
            this.tsmiCalibrationMK22.ToolTipText = "Сохранение и запись калибровочных данных";
            this.tsmiCalibrationMK22.Click += new System.EventHandler(this.tsbtnCalibration_Click);
            // 
            // tsmiTheme
            // 
            this.tsmiTheme.Name = "tsmiTheme";
            this.tsmiTheme.Size = new System.Drawing.Size(257, 24);
            this.tsmiTheme.Text = "Theme editor";
            this.tsmiTheme.ToolTipText = "Редактирование цветовых тем рации";
            this.tsmiTheme.Click += new System.EventHandler(this.tsbtnTheme_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(254, 6);
            // 
            // tsmiSetup
            // 
            this.tsmiSetup.Name = "tsmiSetup";
            this.tsmiSetup.Size = new System.Drawing.Size(257, 24);
            this.tsmiSetup.Text = "Settings";
            this.tsmiSetup.ToolTipText = "Настройки программы";
            this.tsmiSetup.Click += new System.EventHandler(this.tsmiSetup_Click);
            // 
            // tsmiLanguage
            // 
            this.tsmiLanguage.Name = "tsmiLanguage";
            this.tsmiLanguage.Size = new System.Drawing.Size(81, 23);
            this.tsmiLanguage.Text = "Language";
            this.tsmiLanguage.Visible = false;
            // 
            // tsmiWindow
            // 
            this.tsmiWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCascade,
            this.tsmiTileHor,
            this.tsmiTileVer,
            this.tsmiCloseAll});
            this.tsmiWindow.Name = "tsmiWindow";
            this.tsmiWindow.Size = new System.Drawing.Size(71, 23);
            this.tsmiWindow.Text = "Window";
            // 
            // tsmiCascade
            // 
            this.tsmiCascade.Name = "tsmiCascade";
            this.tsmiCascade.Size = new System.Drawing.Size(162, 24);
            this.tsmiCascade.Text = "Cascade";
            this.tsmiCascade.Click += new System.EventHandler(this.tsmiCascade_Click);
            // 
            // tsmiTileHor
            // 
            this.tsmiTileHor.Name = "tsmiTileHor";
            this.tsmiTileHor.Size = new System.Drawing.Size(162, 24);
            this.tsmiTileHor.Text = "Tile Horzontal";
            this.tsmiTileHor.Click += new System.EventHandler(this.tsmiTileHor_Click);
            // 
            // tsmiTileVer
            // 
            this.tsmiTileVer.Name = "tsmiTileVer";
            this.tsmiTileVer.Size = new System.Drawing.Size(162, 24);
            this.tsmiTileVer.Text = "Tile Vertical";
            this.tsmiTileVer.Click += new System.EventHandler(this.tsmiTileVer_Click);
            // 
            // tsmiCloseAll
            // 
            this.tsmiCloseAll.Name = "tsmiCloseAll";
            this.tsmiCloseAll.Size = new System.Drawing.Size(162, 24);
            this.tsmiCloseAll.Text = "Close All";
            this.tsmiCloseAll.Click += new System.EventHandler(this.tsmiCloseAll_Click);
            // 
            // tsmiAbout
            // 
            this.tsmiAbout.Name = "tsmiAbout";
            this.tsmiAbout.Size = new System.Drawing.Size(59, 23);
            this.tsmiAbout.Text = "About";
            this.tsmiAbout.Click += new System.EventHandler(this.tsbtnAbout_Click);
            // 
            // tsmiImport
            // 
            this.tsmiImport.Name = "tsmiImport";
            this.tsmiImport.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.tsmiImport.Size = new System.Drawing.Size(108, 22);
            this.tsmiImport.Text = "Import from OpenGD77";
            this.tsmiImport.Click += new System.EventHandler(this.tsbtnOpen_Click);
            // 
            // tsmiDeviceInfo
            // 
            this.tsmiDeviceInfo.Name = "tsmiDeviceInfo";
            this.tsmiDeviceInfo.Size = new System.Drawing.Size(191, 22);
            this.tsmiDeviceInfo.Text = "Basic Information";
            this.tsmiDeviceInfo.Click += new System.EventHandler(this.tsmiDeviceInfo_Click);
            // 
            // tsmiBootItem
            // 
            this.tsmiBootItem.Name = "tsmiBootItem";
            this.tsmiBootItem.Size = new System.Drawing.Size(191, 22);
            this.tsmiBootItem.Text = "Boot screen";
            this.tsmiBootItem.Click += new System.EventHandler(this.tsmiBootItem_Click);
            // 
            // tsmiMenu
            // 
            this.tsmiMenu.Name = "tsmiMenu";
            this.tsmiMenu.Size = new System.Drawing.Size(191, 22);
            this.tsmiMenu.Text = "Menu";
            this.tsmiMenu.Click += new System.EventHandler(this.tsmiMenu_Click);
            // 
            // tsmiNumKeyContact
            // 
            this.tsmiNumKeyContact.Name = "tsmiNumKeyContact";
            this.tsmiNumKeyContact.Size = new System.Drawing.Size(191, 22);
            this.tsmiNumKeyContact.Text = "Number Key Assign";
            this.tsmiNumKeyContact.Click += new System.EventHandler(this.tsmiNumKeyContact_Click);
            // 
            // tsmiGerneralSet
            // 
            this.tsmiGerneralSet.Name = "tsmiGerneralSet";
            this.tsmiGerneralSet.Size = new System.Drawing.Size(191, 22);
            this.tsmiGerneralSet.Text = "General Setting";
            this.tsmiGerneralSet.Click += new System.EventHandler(this.tsmiGerneralSet_Click);
            // 
            // tsmiButton
            // 
            this.tsmiButton.Name = "tsmiButton";
            this.tsmiButton.Size = new System.Drawing.Size(191, 22);
            this.tsmiButton.Text = "Buttons";
            this.tsmiButton.Click += new System.EventHandler(this.tsmiButton_Click);
            // 
            // tsmiTextMsg
            // 
            this.tsmiTextMsg.Name = "tsmiTextMsg";
            this.tsmiTextMsg.Size = new System.Drawing.Size(191, 22);
            this.tsmiTextMsg.Text = "Text Message";
            this.tsmiTextMsg.Click += new System.EventHandler(this.tsmiTextMsg_Click);
            // 
            // tsmiEncrypt
            // 
            this.tsmiEncrypt.Name = "tsmiEncrypt";
            this.tsmiEncrypt.Size = new System.Drawing.Size(191, 22);
            this.tsmiEncrypt.Text = "Privacy";
            this.tsmiEncrypt.Click += new System.EventHandler(this.tsmiEncrypt_Click);
            // 
            // tsmiDtmf
            // 
            this.tsmiDtmf.Name = "tsmiDtmf";
            this.tsmiDtmf.Size = new System.Drawing.Size(185, 22);
            this.tsmiDtmf.Text = "DTMF Settings";
            this.tsmiDtmf.Click += new System.EventHandler(this.tsmiDtmf_Click);
            // 
            // tsmiEmgSystem
            // 
            this.tsmiEmgSystem.Name = "tsmiEmgSystem";
            this.tsmiEmgSystem.Size = new System.Drawing.Size(185, 22);
            this.tsmiEmgSystem.Text = "APRS";
            this.tsmiEmgSystem.Click += new System.EventHandler(this.tsmiEmgSystem_Click);
            // 
            // tsmiContact
            // 
            this.tsmiContact.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDtmfContact,
            this.tsmiDmrContacts});
            this.tsmiContact.Name = "tsmiContact";
            this.tsmiContact.Size = new System.Drawing.Size(191, 22);
            this.tsmiContact.Text = "Contacts";
            // 
            // tsmiDtmfContact
            // 
            this.tsmiDtmfContact.Name = "tsmiDtmfContact";
            this.tsmiDtmfContact.Size = new System.Drawing.Size(175, 24);
            this.tsmiDtmfContact.Text = "DTMF";
            this.tsmiDtmfContact.Click += new System.EventHandler(this.tsmiDtmfContact_Click);
            // 
            // tsmiDmrContacts
            // 
            this.tsmiDmrContacts.Name = "tsmiDmrContacts";
            this.tsmiDmrContacts.Size = new System.Drawing.Size(175, 24);
            this.tsmiDmrContacts.Text = "Digital Contacts";
            this.tsmiDmrContacts.Click += new System.EventHandler(this.tsmiDmrContacts_Click);
            // 
            // tsmiAPRSConfigs
            // 
            this.tsmiAPRSConfigs.Name = "tsmiAPRSConfigs";
            this.tsmiAPRSConfigs.Size = new System.Drawing.Size(191, 22);
            this.tsmiAPRSConfigs.Text = "APRS Configuations";
            this.tsmiAPRSConfigs.Click += new System.EventHandler(this.tsmiAPRS_Configs_Click);
            // 
            // tsmiGrpRxList
            // 
            this.tsmiGrpRxList.Name = "tsmiGrpRxList";
            this.tsmiGrpRxList.Size = new System.Drawing.Size(191, 22);
            this.tsmiGrpRxList.Text = "Rx Group List";
            this.tsmiGrpRxList.Click += new System.EventHandler(this.tsmiGrpRxList_Click);
            // 
            // tsmiZone
            // 
            this.tsmiZone.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiZoneBasic,
            this.tsmiZoneList});
            this.tsmiZone.Name = "tsmiZone";
            this.tsmiZone.Size = new System.Drawing.Size(191, 22);
            this.tsmiZone.Text = "Zone";
            // 
            // tsmiZoneBasic
            // 
            this.tsmiZoneBasic.Name = "tsmiZoneBasic";
            this.tsmiZoneBasic.Size = new System.Drawing.Size(143, 24);
            this.tsmiZoneBasic.Text = "Zone Basic";
            this.tsmiZoneBasic.Click += new System.EventHandler(this.tsmiZoneBasic_Click);
            // 
            // tsmiZoneList
            // 
            this.tsmiZoneList.Name = "tsmiZoneList";
            this.tsmiZoneList.Size = new System.Drawing.Size(143, 24);
            this.tsmiZoneList.Text = "ZoneList";
            this.tsmiZoneList.Click += new System.EventHandler(this.tsmiZoneList_Click);
            // 
            // tsmiChannels
            // 
            this.tsmiChannels.Name = "tsmiChannels";
            this.tsmiChannels.Size = new System.Drawing.Size(191, 22);
            this.tsmiChannels.Text = "Channels";
            this.tsmiChannels.Click += new System.EventHandler(this.tsmiChannels_Click);
            // 
            // tsmiVfos
            // 
            this.tsmiVfos.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiVfoA,
            this.tsmiVfoB});
            this.tsmiVfos.Name = "tsmiVfos";
            this.tsmiVfos.Size = new System.Drawing.Size(191, 22);
            this.tsmiVfos.Text = "VFOs";
            // 
            // tsmiCodeplugSettings
            // 
            this.tsmiCodeplugSettings.Name = "tsmiCodeplugSettings";
            this.tsmiCodeplugSettings.Size = new System.Drawing.Size(191, 22);
            this.tsmiCodeplugSettings.Text = "Settings";
	    	this.tsmiCodeplugSettings.Click += new System.EventHandler(this.tsmiCodeplugSettings_Click);
            // 
            // tsmiVfoA
            // 
            this.tsmiVfoA.Name = "tsmiVfoA";
            this.tsmiVfoA.Size = new System.Drawing.Size(118, 24);
            this.tsmiVfoA.Text = "VFO A";
            this.tsmiVfoA.Click += new System.EventHandler(this.tsmiVfoA_Click);
            // 
            // tsmiVfoB
            // 
            this.tsmiVfoB.Name = "tsmiVfoB";
            this.tsmiVfoB.Size = new System.Drawing.Size(118, 24);
            this.tsmiVfoB.Text = "VFO B";
            this.tsmiVfoB.Click += new System.EventHandler(this.tsmiVfoB_Click);
            // 
            // tsmiBasic
            // 
            this.tsmiBasic.Name = "tsmiBasic";
            this.tsmiBasic.Size = new System.Drawing.Size(32, 19);
            // 
            // tsmiContactsDownload
            // 
            this.tsmiContactsDownload.Name = "tsmiContactsDownload";
            this.tsmiContactsDownload.Size = new System.Drawing.Size(156, 22);
            this.tsmiContactsDownload.Text = "Download contacts";
            this.tsmiContactsDownload.Click += new System.EventHandler(this.tsbtnContactsDownload_Click);
            // 
            // cmsGroup
            // 
            this.cmsGroup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAdd,
            this.tsmiClear});
            this.cmsGroup.Name = "cmsGroup";
            this.cmsGroup.Size = new System.Drawing.Size(145, 52);
            this.cmsGroup.Opening += new System.ComponentModel.CancelEventHandler(this.cmsGroup_Opening);
            // 
            // tsmiAdd
            // 
            this.tsmiAdd.Name = "tsmiAdd";
            this.tsmiAdd.ShortcutKeyDisplayString = "Enter";
            this.tsmiAdd.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Return)));
            this.tsmiAdd.Size = new System.Drawing.Size(144, 24);
            this.tsmiAdd.Text = "Add";
            this.tsmiAdd.Click += new System.EventHandler(this.tsmiAdd_Click);
            // 
            // tsmiClear
            // 
            this.tsmiClear.Name = "tsmiClear";
            this.tsmiClear.Size = new System.Drawing.Size(144, 24);
            this.tsmiClear.Text = "Clear";
            this.tsmiClear.Click += new System.EventHandler(this.tsmiClear_Click);
            // 
            // cmsSub
            // 
            this.cmsSub.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDel,
            this.tsmiRename,
            this.tsmiCopy,
            this.tsmiPaste,
            this.toolStripSeparator4,
            this.tsmiMoveUp,
            this.tsmiMoveDown});
            this.cmsSub.Name = "cmsSub";
            this.cmsSub.Size = new System.Drawing.Size(212, 154);
            this.cmsSub.Opening += new System.ComponentModel.CancelEventHandler(this.cmsSub_Opening);
            // 
            // tsmiDel
            // 
            this.tsmiDel.Name = "tsmiDel";
            this.tsmiDel.ShortcutKeyDisplayString = "";
            this.tsmiDel.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.tsmiDel.Size = new System.Drawing.Size(211, 24);
            this.tsmiDel.Text = "Delete";
            this.tsmiDel.Click += new System.EventHandler(this.tsmiDel_Click);
            // 
            // tsmiRename
            // 
            this.tsmiRename.Name = "tsmiRename";
            this.tsmiRename.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.tsmiRename.Size = new System.Drawing.Size(211, 24);
            this.tsmiRename.Text = "Rename";
            this.tsmiRename.Click += new System.EventHandler(this.tsmiRename_Click);
            // 
            // tsmiCopy
            // 
            this.tsmiCopy.Name = "tsmiCopy";
            this.tsmiCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.tsmiCopy.Size = new System.Drawing.Size(211, 24);
            this.tsmiCopy.Text = "Copy";
            this.tsmiCopy.Click += new System.EventHandler(this.tsmiCopy_Click);
            // 
            // tsmiPaste
            // 
            this.tsmiPaste.Name = "tsmiPaste";
            this.tsmiPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.tsmiPaste.Size = new System.Drawing.Size(211, 24);
            this.tsmiPaste.Text = "Paste";
            this.tsmiPaste.Click += new System.EventHandler(this.tsmiPaste_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiMoveUp
            // 
            this.tsmiMoveUp.Name = "tsmiMoveUp";
            this.tsmiMoveUp.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.tsmiMoveUp.Size = new System.Drawing.Size(211, 24);
            this.tsmiMoveUp.Text = "Move up";
            this.tsmiMoveUp.Click += new System.EventHandler(this.tsmiMoveUp_Click);
            // 
            // tsmiMoveDown
            // 
            this.tsmiMoveDown.Name = "tsmiMoveDown";
            this.tsmiMoveDown.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.tsmiMoveDown.Size = new System.Drawing.Size(211, 24);
            this.tsmiMoveDown.Text = "Move down";
            this.tsmiMoveDown.Click += new System.EventHandler(this.tsmiMoveDown_Click);
            // 
            // cmsGroupContact
            // 
            this.cmsGroupContact.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddContact});
            this.cmsGroupContact.Name = "cmsGroup";
            this.cmsGroupContact.Size = new System.Drawing.Size(104, 28);
            this.cmsGroupContact.Opening += new System.ComponentModel.CancelEventHandler(this.cmsGroupContact_Opening);
            // 
            // tsmiAddContact
            // 
            this.tsmiAddContact.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiGroupCall,
            this.tsmiPrivateCall,
            this.tsmiAllCall});
            this.tsmiAddContact.Name = "tsmiAddContact";
            this.tsmiAddContact.Size = new System.Drawing.Size(103, 24);
            this.tsmiAddContact.Text = "Add";
            // 
            // tsmiGroupCall
            // 
            this.tsmiGroupCall.Name = "tsmiGroupCall";
            this.tsmiGroupCall.ShortcutKeyDisplayString = "Enter";
            this.tsmiGroupCall.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Return)));
            this.tsmiGroupCall.Size = new System.Drawing.Size(246, 24);
            this.tsmiGroupCall.Text = "Group Call";
            this.tsmiGroupCall.Click += new System.EventHandler(this.tsmiGroupCall_Click);
            // 
            // tsmiPrivateCall
            // 
            this.tsmiPrivateCall.Name = "tsmiPrivateCall";
            this.tsmiPrivateCall.ShortcutKeyDisplayString = "Ctrl+Alt+Enter";
            this.tsmiPrivateCall.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.Return)));
            this.tsmiPrivateCall.Size = new System.Drawing.Size(246, 24);
            this.tsmiPrivateCall.Text = "Private Call";
            this.tsmiPrivateCall.Click += new System.EventHandler(this.tsmiPrivateCall_Click);
            // 
            // tsmiAllCall
            // 
            this.tsmiAllCall.Name = "tsmiAllCall";
            this.tsmiAllCall.Size = new System.Drawing.Size(246, 24);
            this.tsmiAllCall.Text = "All Call";
            this.tsmiAllCall.Click += new System.EventHandler(this.tsmiAllCall_Click);
            // 
            // ofdMain
            // 
            this.ofdMain.Filter = "OpenGD77 RUS (*.ogd)|*.ogd";
            // 
            // sfdMain
            // 
            this.sfdMain.Filter = "OpenGD77 RUS (*.ogd)|*.ogd";
            // 
            // cmsTree
            // 
            this.cmsTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCollapseAll,
            this.tsmiExpandAll});
            this.cmsTree.Name = "cmsTree";
            this.cmsTree.Size = new System.Drawing.Size(149, 52);
            // 
            // tsmiCollapseAll
            // 
            this.tsmiCollapseAll.Name = "tsmiCollapseAll";
            this.tsmiCollapseAll.Size = new System.Drawing.Size(148, 24);
            this.tsmiCollapseAll.Text = "Collapse All";
            this.tsmiCollapseAll.Click += new System.EventHandler(this.tsmiCollapseAll_Click);
            // 
            // tsmiExpandAll
            // 
            this.tsmiExpandAll.Name = "tsmiExpandAll";
            this.tsmiExpandAll.Size = new System.Drawing.Size(148, 24);
            this.tsmiExpandAll.Text = "Expand All";
            this.tsmiExpandAll.Click += new System.EventHandler(this.tsmiExpandAll_Click);
            // 
            // dockPanel
            // 
            this.dockPanel.AllowEndUserDocking = false;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.SystemMdi;
            this.dockPanel.Location = new System.Drawing.Point(234, 71);
            this.dockPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(865, 614);
            this.dockPanel.TabIndex = 6;
            // 
            // pnlTvw
            // 
            this.pnlTvw.Controls.Add(this.tvwMain);
            this.pnlTvw.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlTvw.Location = new System.Drawing.Point(0, 0);
            this.pnlTvw.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlTvw.Name = "pnlTvw";
            this.pnlTvw.Size = new System.Drawing.Size(234, 709);
            this.pnlTvw.TabIndex = 9;
            // 
            // tvwMain
            // 
            this.tvwMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwMain.ImageIndex = 0;
            this.tvwMain.ImageList = this.imgMain;
            this.tvwMain.Location = new System.Drawing.Point(0, 0);
            this.tvwMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tvwMain.Name = "tvwMain";
            this.tvwMain.SelectedImageIndex = 0;
            this.tvwMain.Size = new System.Drawing.Size(234, 709);
            this.tvwMain.TabIndex = 0;
            this.tvwMain.TabStop = false;
            this.tvwMain.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tvwMain_BeforeLabelEdit);
            this.tvwMain.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tvwMain_AfterLabelEdit);
            this.tvwMain.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvwMain_NodeMouseClick);
            this.tvwMain.DoubleClick += new System.EventHandler(this.tvwMain_DoubleClick);
            this.tvwMain.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tvwMain_KeyDown);
            // 
            // ssrMain
            // 
            this.ssrMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slblCompany});
            this.ssrMain.Location = new System.Drawing.Point(234, 685);
            this.ssrMain.Name = "ssrMain";
            this.ssrMain.Padding = new System.Windows.Forms.Padding(1, 0, 17, 0);
            this.ssrMain.Size = new System.Drawing.Size(865, 24);
            this.ssrMain.TabIndex = 12;
            this.ssrMain.Text = "statusStrip1";
            // 
            // slblCompany
            // 
            this.slblCompany.Name = "slblCompany";
            this.slblCompany.Size = new System.Drawing.Size(62, 19);
            this.slblCompany.Text = "Prompt：";
            // 
            // tsrMain
            // 
            this.tsrMain.ImageScalingSize = new System.Drawing.Size(35, 35);
            this.tsrMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnNew,
            this.tsbtnOpen,
            this.tsbtnSave,
            this.toolStripSeparator2,
            this.tsbtnRead,
            this.tsbtnWrite,
            this.toolStripSeparator3,
            this.tsbtnDMR,
            this.toolStripButton1,
            this.tsbtnOpenGD,
            this.tsbtnPalette,
            this.toolStripSeparator7,
            this.tsbtnSettings,
            this.tsbtnAbout});
            this.tsrMain.Location = new System.Drawing.Point(234, 29);
            this.tsrMain.Name = "tsrMain";
            this.tsrMain.Size = new System.Drawing.Size(865, 42);
            this.tsrMain.TabIndex = 13;
            this.tsrMain.Text = "toolStrip1";
            // 
            // tsbtnNew
            // 
            this.tsbtnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnNew.Image = global::DMR_MainForm.file;
            this.tsbtnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnNew.Name = "tsbtnNew";
            this.tsbtnNew.Size = new System.Drawing.Size(39, 39);
            this.tsbtnNew.Text = "Новый кодплаг";
            this.tsbtnNew.Click += new System.EventHandler(this.tsbtnNew_Click);
            // 
            // tsbtnOpen
            // 
            this.tsbtnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnOpen.Image = global::DMR_MainForm.open;
            this.tsbtnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnOpen.Name = "tsbtnOpen";
            this.tsbtnOpen.Size = new System.Drawing.Size(39, 39);
            this.tsbtnOpen.Text = "Открыть кодплаг";
            this.tsbtnOpen.Click += new System.EventHandler(this.tsbtnOpen_Click);
            // 
            // tsbtnSave
            // 
            this.tsbtnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnSave.Image = global::DMR_MainForm.save;
            this.tsbtnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnSave.Name = "tsbtnSave";
            this.tsbtnSave.Size = new System.Drawing.Size(39, 39);
            this.tsbtnSave.Text = "Сохранить кодплаг";
            this.tsbtnSave.ToolTipText = "Save";
            this.tsbtnSave.Click += new System.EventHandler(this.tsbtnSave_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 42);
            // 
            // tsbtnRead
            // 
            this.tsbtnRead.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnRead.Image = global::DMR_MainForm.read;
            this.tsbtnRead.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnRead.Name = "tsbtnRead";
            this.tsbtnRead.Size = new System.Drawing.Size(39, 39);
            this.tsbtnRead.Text = "Читать из рации";
            this.tsbtnRead.Click += new System.EventHandler(this.tsbtnRead_Click);
            // 
            // tsbtnWrite
            // 
            this.tsbtnWrite.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnWrite.Image = global::DMR_MainForm.write;
            this.tsbtnWrite.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnWrite.Name = "tsbtnWrite";
            this.tsbtnWrite.Size = new System.Drawing.Size(39, 39);
            this.tsbtnWrite.Text = "Записать в рацию";
            this.tsbtnWrite.Click += new System.EventHandler(this.tsbtnWrite_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 42);
            // 
            // tsbtnDMR
            // 
            this.tsbtnDMR.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnDMR.Image = global::DMR_MainForm.users;
            this.tsbtnDMR.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnDMR.Name = "tsbtnDMR";
            this.tsbtnDMR.Size = new System.Drawing.Size(39, 39);
            this.tsbtnDMR.Text = "Загрузка базы DMR ID";
            this.tsbtnDMR.Click += new System.EventHandler(this.tsbtnDMRID_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::DMR_MainForm.firmware;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(39, 39);
            this.toolStripButton1.Text = "Загрузчик прошивки";
            this.toolStripButton1.Click += new System.EventHandler(this.tsmiFirmwareLoader_Click);
            // 
            // tsbtnOpenGD
            // 
            this.tsbtnOpenGD.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnOpenGD.Image = global::DMR_MainForm.toolbox;
            this.tsbtnOpenGD.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnOpenGD.Name = "tsbtnOpenGD";
            this.tsbtnOpenGD.Size = new System.Drawing.Size(39, 39);
            this.tsbtnOpenGD.Text = "Технический центр";
            this.tsbtnOpenGD.Click += new System.EventHandler(this.tsmiOpenGD77_Click);
            // 
            // tsbtnPalette
            // 
            this.tsbtnPalette.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnPalette.Image = global::DMR_MainForm.palette;
            this.tsbtnPalette.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnPalette.Name = "tsbtnPalette";
            this.tsbtnPalette.Size = new System.Drawing.Size(39, 39);
            this.tsbtnPalette.Text = "Редактор цветовых тем";
            this.tsbtnPalette.Click += new System.EventHandler(this.tsbtnTheme_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 42);
            // 
            // tsbtnSettings
            // 
            this.tsbtnSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnSettings.Image = global::DMR_MainForm.settings;
            this.tsbtnSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnSettings.Name = "tsbtnSettings";
            this.tsbtnSettings.Size = new System.Drawing.Size(39, 39);
            this.tsbtnSettings.Text = "Настройки программы";
            this.tsbtnSettings.Click += new System.EventHandler(this.tsmiSetup_Click);
            // 
            // tsbtnAbout
            // 
            this.tsbtnAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnAbout.Image = global::DMR_MainForm.about;
            this.tsbtnAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnAbout.Name = "tsbtnAbout";
            this.tsbtnAbout.Size = new System.Drawing.Size(39, 39);
            this.tsbtnAbout.Text = "О программе";
            this.tsbtnAbout.Click += new System.EventHandler(this.tsbtnAbout_Click);
            // 
            // importFileDialog
            // 
            this.importFileDialog.DefaultExt = "g77";
            this.importFileDialog.Filter = "Кодплаги OpenGD77 (*.g77)|*.g77|Все файлы (*.*)|*.*";
            // 
            // radioInformation
            // 
            this.radioInformation.BackColor = System.Drawing.Color.White;
            this.radioInformation.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.radioInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.radioInformation.Location = new System.Drawing.Point(234, 595);
            this.radioInformation.Name = "radioInformation";
            this.radioInformation.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radioInformation.Size = new System.Drawing.Size(865, 90);
            this.radioInformation.TabIndex = 15;
            this.radioInformation.Text = "label1";
            // 
            // pingTimer
            // 
            this.pingTimer.Interval = 500;
            this.pingTimer.Tick += new System.EventHandler(this.pingTimer_Tick);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Архивы|*.zip";
            this.saveFileDialog.Title = "Сохранение прошивки";
            // 
            // MainForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1099, 709);
            this.Controls.Add(this.radioInformation);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.tsrMain);
            this.Controls.Add(this.ssrMain);
            this.Controls.Add(this.mnsMain);
            this.Controls.Add(this.pnlTvw);
            this.DoubleBuffered = true;
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.mnsMain;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MdiChildActivate += new System.EventHandler(this.MainForm_MdiChildActivate);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.mnsMain.ResumeLayout(false);
            this.mnsMain.PerformLayout();
            this.cmsGroup.ResumeLayout(false);
            this.cmsSub.ResumeLayout(false);
            this.cmsGroupContact.ResumeLayout(false);
            this.cmsTree.ResumeLayout(false);
            this.pnlTvw.ResumeLayout(false);
            this.ssrMain.ResumeLayout(false);
            this.ssrMain.PerformLayout();
            this.tsrMain.ResumeLayout(false);
            this.tsrMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

	}

	private IDockContent method_0(string string_0)
	{
		if (string_0 == typeof(TreeForm).ToString())
		{
			return frmTree;
		}
		if (string_0 == typeof(HelpForm).ToString())
		{
			return frmHelp;
		}
		return null;
	}

	public MainForm(string[] args)
	{
		StartupArgs = args;
		if (Control.ModifierKeys.ToString() == "Shift, Control")
		{
			EnableHiddenFeatures = true;
		}
		frmHelp = new HelpForm();
		frmTree = new TreeForm();
		lstTreeNodeItem = new List<TreeNodeItem>();
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		_TextBox = new TextBox();
		_TextBox.Visible = false;
		_TextBox.LostFocus += _TextBox_LostFocus;
		_TextBox.Validating += _TextBox_Validating;
		_TextBox.KeyPress += Settings.applyDTMFFilter;
		base.Controls.Add(_TextBox);
		m_deserializeDockContent = method_0;
		initialiseTree();
		method_20(method_19());
		frmHelp.VisibleChanged += FormPanel_VisibleChanged;
		frmTree.VisibleChanged += FormPanel_VisibleChanged;
		radioInformation.Text = "";
        pingTimer.Interval = IniFileUtils.getProfileIntWithDefault("Setup", "PollingInterval", 500);
    }

	public bool shownInfo = false;
	private void pingRadio()
	{
		bool result;
		OpenGD77Form temp = new OpenGD77Form(OpenGD77CommsTransferData.CommsAction.NONE);
		result = temp.probeRadioModel(true, true);
		if (result && !shownInfo)
		{
			showRadioInfo();
		}
		else
		{
			shownInfo = false;
		}
		temp.Dispose();
		return;
	}

	private string firmwareName = "";
    private string remoteVersion = "";
    public void showRadioInfo()
	{
		string firmwareVersion = "";
		if (!shownInfo)
        {
            shownInfo = true;
            radioInformation.Text = "Рация подключена\r\n";
            if (RadioInfo.identifier != "RUSSIAN")
            {
                radioInformation.Text += "На рации не установлена прошивка OpenGD77 RUS!";
            }
            else
            {
                radioInformation.Text += "Установлена корректная прошивка OpenGD77 RUS";
            }
            radioInformation.Text += "\r\nСборка прошивки: ";
            firmwareVersion = RadioInfo.buildDateTime.Substring(0, 8);
            radioInformation.Text += firmwareVersion;
            radioInformation.Text += "\r\nМодель рации: ";
            switch (RadioInfo.radioType)
            {
                case 0:
                    radioInformation.Text += "TYT MD-760/Radioddity GD-77";
                    firmwareName = "";
                    break;
                case 1:
                    radioInformation.Text += "Radioddity GD-77S";
                    firmwareName = "";
                    break;
                case 2:
                    radioInformation.Text += "Baofeng DM-1801";
                    firmwareName = "";
                    break;
                case 3:
                    radioInformation.Text += "Baofeng RD-5R";
                    firmwareName = "";
                    break;
                case 4:
                    radioInformation.Text += "Baofeng DM-1801A";
                    firmwareName = "";
                    break;
                case 5:
                    radioInformation.Text += "TYT MD-9600/Retevis RT-90";
                    firmwareName = "OpenMD9600RUS";
                    break;
                case 6:
                    radioInformation.Text += "TYT MD-UV380/TYT MD-UV390 (5W)/Retevis RT-3S";
                    firmwareName = "OpenMDUV380_RUS";
                    break;
                case 8:
                case 10:
                    radioInformation.Text += "Baofeng DM-1701/Retevis RT-84";
                    firmwareName = "OpenDM1701-RT84_RUS";
                    break;
                case 9:
                    radioInformation.Text += "TYT MD-2017/Retevis RT-82";
                    firmwareName = "OpenMD2017RUS";
                    break;
                case 106:
                    radioInformation.Text += "TYT MD-UV390 (10W)";
                    firmwareName = "OpenMDUV380_10W_PLUS_RUS";
                    break;
                default:
                    firmwareName = "";
                    break;

            }
			switch(RadioInfo.structVersion) //версия унаследована от OpenGD77
			{
				case 0x03:
					radioInformation.Text += "\r\nЧип флеш-памяти: ";
				    radioInformation.Text += RadioInfo.flashId.ToString();
					break;
				case 0x04:
                    radioInformation.Text += "\r\nВерсия блока настроек: 0x";
					radioInformation.Text += RadioInfo.flashId.ToString("X4");
                    break;
				default:
                    radioInformation.Text += "\r\nИдентификатор не распознан: ";
                    radioInformation.Text += RadioInfo.structVersion.ToString();
					break;
            }
			/*
 	public enum RadioInfoFeatures : ushort
	{
		SCREEN_INVERTED = 1,
		DMRID_USES_VOICE_PROMPTS = 2,
		VOICE_PROMPTS_AVAILABLE = 4,
		SUPPORT_SETTINGS_ACCESS = 8
	}
			*/
            if (OpenGD77Form.RadioInfoIsFeatureSet(OpenGD77Form.RadioInfoFeatures.SCREEN_INVERTED))
            {
                radioInformation.Text += "\r\nИзображение инвертировано";
            }
            if (OpenGD77Form.RadioInfoIsFeatureSet(OpenGD77Form.RadioInfoFeatures.DMRID_USES_VOICE_PROMPTS))
            {
                radioInformation.Text += "\r\nБаза ID расширена за счет памяти голосовых сообщений";
            }
            if (OpenGD77Form.RadioInfoIsFeatureSet(OpenGD77Form.RadioInfoFeatures.VOICE_PROMPTS_AVAILABLE))
            {
                radioInformation.Text += "\r\nГолосовые оповещения загружены";
            }
            if (OpenGD77Form.RadioInfoIsFeatureSet(OpenGD77Form.RadioInfoFeatures.SUPPORT_SETTINGS_ACCESS) && (RadioInfo.flashId >= 0x477D))
			{
                radioInformation.Text += "\r\nДоступно чтение настроек";
            }
			else
			{
                radioInformation.Text += "\r\nЧтение блока настроек не поддерживается прошивкой";
            }
            if (IniFileUtils.getProfileStringWithDefault("Setup", "CheckFirmware", "yes") == "yes" && !messageShown && RadioInfo.identifier == "RUSSIAN")
            {
                messageShown = true;
                string remoteURL = IniFileUtils.getProfileStringWithDefault("Setup", "ServerURI", "https://opengd77rus.ru/data/") + "Firmware.num";
                Uri remoteUri = new Uri(remoteURL);
                WebClient checker = new WebClient();
                string localName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Firmware.ver";
                bool fail = false;
                try
                {
                    checker.DownloadFile(remoteUri, localName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    fail = true;
                }
                if (!fail)
                {
                    try
                    {
                        StreamReader sr = new StreamReader(localName);
                        remoteVersion = sr.ReadLine();
                        sr.Close();
                    }
                    catch
                    {

                    }
                    if (int.Parse(remoteVersion) > int.Parse(RadioInfo.buildDateTime))
                    {
                        DialogResult decision = MessageBox.Show("На сайте проекта доступна новая версия прошивки.\r\nТекущая версия: " + RadioInfo.buildDateTime + "\r\nВерсия на сервере: " + remoteVersion +
                            "\r\nОткрыть страницу загрузок?", "Обновление прошивки", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (decision == DialogResult.Yes)
                        {
                            string saveFileName = firmwareName + "_" + remoteVersion + ".zip";
                            this.saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                            this.saveFileDialog.FileName = saveFileName;
                            if (saveFileDialog.ShowDialog() != DialogResult.Cancel)
                            {
                                Uri firmwareURI = new Uri("https://opengd77rus.ru/firmwares/" + firmwareName + ".zip");
                                WebClient downloader = new WebClient();
                                downloader.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadFileCallback);
                                try
                                {
                                    downloader.DownloadFileAsync(firmwareURI, saveFileDialog.FileName);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    fail = true;
                                }
                            }
                        }
                        File.Delete(localName);
                    }


                }
            }
        }
    }

    private static void downloadFileCallback(object sender, AsyncCompletedEventArgs e)
    {
		if (!e.Cancelled && e.Error==null)
		{
			MessageBox.Show("Файл с прошивкой успешно загружен!", "Загрузка прошивки", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
    }



    public static bool connectionAwailable = false;

    private void MainForm_Load(object sender, EventArgs e)
	{
        bool isElevated;
		string thisFilePath = Application.StartupPath;
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);

        if (isElevated == false && thisFilePath.Contains("Program Files"))
        {
			MessageBox.Show("Программа установлена в папку " + thisFilePath + ", но не запущена от имени администратора. Автообновление файла локализации и ручное его скачивание через Загрузчик прошивки будет недоступно из-за ограничений Windows. Удалите программу и переустановите ее в другую папку, либо установите для исполняемого файла программы галочку запуска от имени администратора.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        };

		

        string _profileStringWithDefault = IniFileUtils.getProfileStringWithDefault("Setup", "RadioType", "MD9600");
        if (!(_profileStringWithDefault == "MD9600"))
        {
            RadioType = RadioTypeEnum.RadioTypeMK22;
        }
        else
        {
            RadioType = RadioTypeEnum.RadioTypeSTM32;
        }
		this.tsmiRadioTypeItem_MK22.Checked = (RadioType == RadioTypeEnum.RadioTypeMK22);
		this.tsmiRadioTypeItem_STM32.Checked = (RadioType == RadioTypeEnum.RadioTypeSTM32);
        if (EnableHiddenFeatures)
        {
            this.tsmiSetting.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[11]
            {
                this.tsmiBootItem, this.tsmiGerneralSet, this.tsmiDeviceInfo, this.tsmiTextMsg, this.tsmiDtmf, this.tsmiAPRSConfigs, this.tsmiContact, this.tsmiGrpRxList, this.tsmiZone, this.tsmiChannels,
                this.tsmiVfos
            });
        }
        else
        {
            this.tsmiSetting.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[11] { this.tsmiBootItem, this.tsmiGerneralSet, this.tsmiDeviceInfo, this.tsmiDtmf, this.tsmiAPRSConfigs, this.tsmiContact, this.tsmiGrpRxList, this.tsmiZone, this.tsmiChannels, this.tsmiVfos, this.tsmiCodeplugSettings });
        }
        Settings.dicCommon.Add("None", Settings.SZ_NONE);
		Settings.dicCommon.Add("Selected", Settings.SZ_SELECTED);
		Settings.dicCommon.Add("Read", Settings.SZ_READ);
		Settings.dicCommon.Add("Write", Settings.SZ_WRITE);
		Settings.dicCommon.Add("ReadComplete", Settings.SZ_READ_COMPLETE);
		Settings.dicCommon.Add("WriteComplete", Settings.SZ_WRITE_COMPLETE);
		Settings.dicCommon.Add(Settings.SZ_NAME_EXIST_NAME, "Имя существует!");
		Settings.dicCommon.Add("FirstChNotDelete", Settings.SZ_FIRST_CH_NOT_DELETE);
		Settings.dicCommon.Add("IdNotEmpty", Settings.SZ_ID_NOT_EMPTY);
		Settings.dicCommon.Add("IdOutOfRange", Settings.SZ_ID_OUT_OF_RANGE);
		Settings.dicCommon.Add("OpenSuccessfully", Settings.SZ_OPEN_SUCCESSFULLY);
		Settings.dicCommon.Add("SaveSuccessfully", Settings.SZ_SAVE_SUCCESSFULLY);
		Settings.dicCommon.Add("TypeNotMatch", Settings.SZ_TYPE_NOT_MATCH);
		Settings.dicCommon.Add("NotSelectItemNotCopyItem", Settings.SZ_NOT_SELECT_ITEM_NOT_COPYITEM);
		Settings.dicCommon.Add("PromptKey1", Settings.SZ_PROMPT_KEY1);
		Settings.dicCommon.Add("PromptKey2", Settings.SZ_PROMPT_KEY2);
		Settings.dicCommon.Add("DataFormatError", "Data format error");
		Settings.dicCommon.Add("FirstNotDelete", "The first row cannot be deleted");
		Settings.dicCommon.Add("KeyPressDtmf", "");
		Settings.dicCommon.Add("KeyPressDigit", "");
		Settings.dicCommon.Add("KeyPressPrint", "");
		Settings.dicCommon.Add("DeviceNotFound", "");
		Settings.dicCommon.Add("CommError", "");
		Settings.dicCommon.Add("codePlugReadConfirm", Settings.SZ_CODEPLUG_READ_CONFIRM);
		Settings.dicCommon.Add("codePlugWriteConfirm", Settings.SZ_CODEPLUG_WRITE_CONFIRM);
		Settings.dicCommon.Add("pleaseConfirm", Settings.SZ_PLEASE_CONFIRM);
		Settings.dicCommon.Add("userAgreement", Settings.SZ_USER_AGREEMENT);
		Settings.dicCommon.Add("DownloadContactsMessageAdded", Settings.SZ_DOWNLOADCONTACTS_REGION_EMPTY);
		Settings.dicCommon.Add("DownloadContactsRegionEmpty", Settings.SZ_DOWNLOADCONTACTS_MESSAGE_ADDED);
		Settings.dicCommon.Add("DownloadContactsDownloading", Settings.SZ_DOWNLOADCONTACTS_DOWNLOADING);
		Settings.dicCommon.Add("DownloadContactsSelectContactsToImport", Settings.SZ_DOWNLOADCONTACTS_SELECT_CONTACTS_TO_IMPORT);
		Settings.dicCommon.Add("DownloadContactsTooMany", Settings.SZ_DOWNLOADCONTACTS_TOO_MANY);
		Settings.dicCommon.Add("Warning", Settings.SZ_WARNING);
		Settings.dicCommon.Add("UnableDownloadFromInternet", Settings.SZ_UNABLEDOWNLOADFROMINTERNET);
		Settings.dicCommon.Add("DownloadContactsImportComplete", Settings.SZ_IMPORT_COMPLETE);
		Settings.dicCommon.Add("CodeplugUpgradeNotice", Settings.SZ_CODEPLUG_UPGRADE_NOTICE);
		Settings.dicCommon.Add("CodeplugUpgradeWarningToManyRxGroups", Settings.SZ_CODEPLUG_UPGRADE_WARNING_TO_MANY_RX_GROUPS);
		Settings.dicCommon.Add("CodeplugRead", Settings.SZ_CODEPLUG_READ);
		Settings.dicCommon.Add("CodeplugWrite", Settings.SZ_CODEPLUG_WRITE);
		Settings.dicCommon.Add("DMRIDRead", Settings.SZ_DMRID_READ);
		Settings.dicCommon.Add("DMRIDWrite", Settings.SZ_DMRID_WRITE);
		Settings.dicCommon.Add("CalibrationRead", Settings.SZ_CALIBRATION_READ);
		Settings.dicCommon.Add("CalibrationWrite", Settings.SZ_CALIBRATION_WRITE);
		Settings.dicCommon.Add("IdAlreadyExists", Settings.SZ_ID_ALREADY_EXISTS);
		Settings.dicCommon.Add("ContactNameDuplicate", Settings.SZ_CONTACT_DUPLICATE_NAME);
		Settings.dicCommon.Add("EnableMemoryAccessMode", Settings.SZ_EnableMemoryAccessMode);
		Settings.dicCommon.Add("dataRead", Settings.SZ_dataRead);
		Settings.dicCommon.Add("dataWrite", Settings.SZ_dataWrite);
		Settings.dicCommon.Add("DMRIdContcatsTotal", Settings.SZ_DMRIdContcatsTotal);
		Settings.dicCommon.Add("ErrorParsingData", Settings.SZ_ErrorParsingData);
		Settings.dicCommon.Add("DMRIdIntroMessage", Settings.SZ_DMRIdIntroMessage);
		Settings.dicCommon.Add("DMRIdTooManyIDs", "Too many ID's for the connected radio, the list will be truncated to the maximum capacity.");
		Settings.dicCommon.Add("OfficialFWNotSelected", "You must first select the location of the official / donor firmware file");
		Settings.dicCommon.Add("Error", "Error");
		Settings.dicCommon.Add("FirmwareFilefilter", "Файлы прошивки|*.bin");
		Settings.dicCommon.Add("FirmwareSelectorTitle", "Select Firmware file");
		Settings.dicCommon.Add("Processing", "Processing...");
		using (Graphics graphics = CreateGraphics())
		{
			Settings.smethod_7(new SizeF(graphics.DpiX / 96f, graphics.DpiY / 96f));
		}
		tsmiBasic.Visible = true;
		Settings.setPassword("TYT380");
		Settings.CUR_MODE = 2;
		ChannelForm.CurCntCh = 1024;
		bool flag = false;
		if (StartupArgs.Length != 0)
		{
			if (File.Exists(StartupArgs[0]))
			{
				switch (System.IO.Path.GetExtension(StartupArgs[0]))
				{
                case ".ogd":
                    openCodeplugFile(StartupArgs[0]);
                    _lastCodeplugFileName = StartupArgs[0];
                    break;
                case ".g77":
					openCodeplugFile(StartupArgs[0]);
					_lastCodeplugFileName = StartupArgs[0];
					break;
				case ".bin":
				case ".zip":
					StartupArgs[0].IndexOf("MD9600");
					StartupArgs[0].IndexOf("MD2017");
					StartupArgs[0].IndexOf("UV380");
					StartupArgs[0].IndexOf("DM1701");
					StartupArgs[0].IndexOf("RT84");
					StartupArgs[0].IndexOf("GD77");
					StartupArgs[0].IndexOf("GD77");
					StartupArgs[0].IndexOf("RD5R");
					break;
				}
			}
			else
			{
				loadDefaultOrInitialFile();
				_lastCodeplugFileName = "";
				IniFileUtils.WriteProfileString("Setup", "LastFilePath", "");
				flag = true;
			}
		}
		if (!flag)
		{
			string profileStringWithDefault = IniFileUtils.getProfileStringWithDefault("Setup", "LastFilePath", "");
			if ("" == profileStringWithDefault)
			{
				loadDefaultOrInitialFile();
			}
			else
			{
				_lastCodeplugFileName = IniFileUtils.getProfileStringWithDefault("Setup", "LastFilePath", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
				if (_lastCodeplugFileName != null && _lastCodeplugFileName != "" && File.Exists(_lastCodeplugFileName))
				{
					openCodeplugFile(_lastCodeplugFileName);
				}
				else
				{
					loadDefaultOrInitialFile();
					_lastCodeplugFileName = "";
					IniFileUtils.WriteProfileString("Setup", "LastFilePath", "");
				}
			}
		}
		frmHelp.Hide();
		frmTree.Show(dockPanel);
		pnlTvw.Dock = DockStyle.Fill;
		frmTree.Controls.Add(pnlTvw);
		ChannelForm.DefaultCh = ChannelForm.data[0].Clone();
		NormalScanForm.DefaultScan = NormalScanForm.data[0].Clone();
		ContactForm.DefaultContact = ContactForm.data[0].Clone();
		APRSForm.DefaultAPRS_Config = APRSForm.data[0].Clone();
		BootItemForm.DefaultBootItem = Settings.cloneObject(BootItemForm.data);
		ButtonForm.DefaultSideKey = Settings.cloneObject(ButtonForm.data);
		ScanBasicForm.DefaultScanBasic = Settings.cloneObject(ScanBasicForm.data);
		SignalingBasicForm.DefaultSignalingBasic = Settings.cloneObject(SignalingBasicForm.data);
		DtmfForm.DefaultDtmf = Settings.cloneObject(DtmfForm.data);
		EncryptForm.DefaultEncrypt = Settings.cloneObject(EncryptForm.data);
		GeneralSetForm.DefaultGeneralSet = Settings.cloneObject(GeneralSetForm.data);
		AttachmentForm.DefaultAttachment = Settings.cloneObject(AttachmentForm.data);
		VfoForm.DefaultCh = VfoForm.data[0].Clone();
		MenuForm.DefaultMenu = Settings.cloneObject(MenuForm.data);
		imgMain.Images.Clear();
		imgMain.Images.AddStrip(Resources.smethod_0());
		base.AutoScaleMode = AutoScaleMode.Font;
		Font = new Font("Arial", 10f, FontStyle.Regular);
        GetAllLang();
		string profileStringWithDefault2 = IniFileUtils.getProfileStringWithDefault("Setup", "Language", "Russian.xml");
		foreach (ToolStripMenuItem dropDownItem in tsmiLanguage.DropDownItems)
		{
			if (System.IO.Path.GetFileName(dropDownItem.Tag.ToString()) == profileStringWithDefault2)
			{
				dropDownItem.PerformClick();
				break;
			}
		}
		Text = getMainTitleStub() + "       " + _lastCodeplugFileName;
		if (IniFileUtils.getProfileStringWithDefault("Setup", "agreedToTerms", "no") == "no")
		{
			if (DialogResult.Yes != MessageBox.Show(Settings.dicCommon["userAgreement"], Settings.dicCommon["pleaseConfirm"], MessageBoxButtons.YesNo))
			{
				if (Application.MessageLoop)
				{
					Application.Exit();
				}
				else
				{
					Environment.Exit(1);
				}
			}
			else
			{
				IniFileUtils.WriteProfileString("Setup", "agreedToTerms", "yes");
				base.FormClosing += MainForm_FormClosing;
			}
		}
		else
		{
			base.FormClosing += MainForm_FormClosing;
		}
		CSVEML.InitCSVs();
		connectionAwailable = true;// hasInternet();
        pingTimer.Enabled = true;
		bool checkUpdate = IniFileUtils.getProfileStringWithDefault("Setup", "CheckVersion", "yes") == "yes";
		if (checkUpdate && connectionAwailable)
		{
            string remoteUri = IniFileUtils.getProfileStringWithDefault("Setup", "ServerURI", "https://opengd77rus.ru/data/") + "Version.num";
            WebClient checker = new WebClient();
			string localName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\OpenGD77RUS.ver";
			bool fail = false;
			string remoteVersion = "";
            try
            {
                checker.DownloadFile(remoteUri, localName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
				fail = true;
            }
            if (!fail)
            {
				try
				{
                    StreamReader sr = new StreamReader(localName);
                    remoteVersion = sr.ReadLine();
                    sr.Close();
                }
				catch
				{

				}                
				string currVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
				if (remoteVersion != currVersion)
				{
					DialogResult decision = MessageBox.Show("На сайте проекта опубликована другая версия OpenGD77 CPS " + remoteVersion + ".\r\nОткрыть страницу загрузок?", "Обновление", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (decision == DialogResult.Yes)
					{
                        System.Diagnostics.Process.Start("https://opengd77rus.ru/download/opengd77-rus-cps/");
                    }
					File.Delete(localName);
				}


            }
        }
	}

	private string getMainTitleStub()
	{
		return "OpenGD77 RUS  [Версия " + PRODUCT_VERSION + "]";
	}

	private void MainForm_MdiChildActivate(object sender, EventArgs e)
	{
		if (base.ActiveMdiChild != null)
		{
			Type type = base.ActiveMdiChild.GetType();
			if (PreActiveMdiChild != null)
			{
				PreActiveMdiChild.SaveData();
			}
			PreActiveMdiChild = base.ActiveMdiChild as IDisp;
			RefreshForm(type);
		}
		else
		{
			PreActiveMdiChild = null;
		}
	}

	private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		switch (MessageBox.Show(Settings.dicCommon["PromptKey1"], "", MessageBoxButtons.YesNoCancel))
		{
		case DialogResult.Cancel:
			e.Cancel = true;
			break;
		case DialogResult.Yes:
			tsmiSave.PerformClick();
			break;
		}
	}

	private void method_1(Form form_0)
	{
		if (form_0 is IDisp disp)
		{
			disp.SaveData();
		}
	}

	private void method_2()
	{
		method_1(base.ActiveMdiChild);
	}

	private void method_3()
	{
		method_2();
		Form[] mdiChildren = base.MdiChildren;
		foreach (Form form in mdiChildren)
		{
			if (form != base.ActiveMdiChild)
			{
				method_1(form);
			}
		}
	}

	private void closeAllForms()
	{
		Form[] mdiChildren = base.MdiChildren;
		for (int i = 0; i < mdiChildren.Length; i++)
		{
			mdiChildren[i].Close();
		}
	}

	private void method_5(Type type_0)
	{
		Form[] mdiChildren = base.MdiChildren;
		foreach (Form form in mdiChildren)
		{
			if (form.GetType() == type_0)
			{
				form.Close();
			}
		}
	}

	private Form method_6(TreeNode treeNode_0)
	{
		if (treeNode_0 == null)
		{
			return null;
		}
		if (treeNode_0.Tag is TreeNodeItem treeNodeItem)
		{
			Form[] mdiChildren = base.MdiChildren;
			foreach (Form form in mdiChildren)
			{
				if (form.GetType() == treeNodeItem.Type)
				{
					return form;
				}
			}
		}
		return null;
	}

	private Form treeviewDoubleClickHandler(TreeNode treeNode_0, bool bool_0)
	{
		if (treeNode_0.Tag is TreeNodeItem treeNodeItem)
		{
			Form[] mdiChildren = base.MdiChildren;
			foreach (Form form in mdiChildren)
			{
				if (!(form.GetType() != treeNodeItem.Type))
				{
					form.Activate();
					form.BringToFront();
					if (!(form is IDisp disp))
					{
						return form;
					}
					disp.SaveData();
					disp.Node = treeNode_0;
					if (bool_0)
					{
						form.Tag = treeNodeItem.Index;
					}
					disp.DispData();
					return form;
				}
			}
			if (treeNodeItem.Type != null)
			{
				if (treeNodeItem.Type == typeof(ButtonForm) || treeNodeItem.Type == typeof(DigitalKeyContactForm) || treeNodeItem.Type == typeof(MenuForm) || treeNodeItem.Type == typeof(ScanBasicForm) || treeNodeItem.Type == typeof(EncryptForm) || treeNodeItem.Type == typeof(NormalScanForm))
				{
					MessageBox.Show("This feature is not supported in the OpenGD77 firmware");
				}
				_ = treeNodeItem.Type == typeof(ZoneBasicForm);
				Form form2 = (Form)Activator.CreateInstance(treeNodeItem.Type);
				form2.MdiParent = this;
				if (form2 is IDisp disp2)
				{
					disp2.Node = treeNode_0;
					if (bool_0)
					{
						form2.Tag = treeNodeItem.Index;
					}
				}
				form2.Show();
				form2.Focus();
				form2.BringToFront();
				return form2;
			}
		}
		return null;
	}

	public void RefreshForm(Type type)
	{
		Form[] mdiChildren = base.MdiChildren;
		foreach (Form form in mdiChildren)
		{
			if (form.GetType() == type && form is IDisp disp)
			{
				disp.DispData();
			}
		}
	}

	public void SaveForm(Type type)
	{
		Form[] mdiChildren = base.MdiChildren;
		foreach (Form form in mdiChildren)
		{
			if (form.GetType() == type)
			{
				method_1(form);
			}
		}
	}

	public ISingleRow GetForm(Type type)
	{
		Form[] mdiChildren = base.MdiChildren;
		int num = 0;
		ISingleRow singleRow;
		while (true)
		{
			if (num < mdiChildren.Length)
			{
				Form form = mdiChildren[num];
				if (form.GetType() == type)
				{
					singleRow = form as ISingleRow;
					if (singleRow != null)
					{
						break;
					}
				}
				num++;
				continue;
			}
			return null;
		}
		return singleRow;
	}

	public void VerifyRelatedForm(Type type)
	{
		if (type == typeof(ContactsForm))
		{
			ButtonForm.data.Verify(ButtonForm.DefaultSideKey);
			ChannelForm.data.Verify();
			RxGroupListForm.data.Verify();
			DigitalKeyContactForm.data.Verify();
		}
		else if (type == typeof(ChannelsForm))
		{
			NormalScanForm.data.Verify();
			ZoneForm.data.Verify();
			APRSForm.data.Verify();
		}
	}

	public void RefreshRelatedForm(Type type, int index)
	{
		if (type == typeof(ContactForm))
		{
			GetForm(typeof(ContactsForm))?.RefreshSingleRow(index);
		}
		else if (type == typeof(ChannelForm))
		{
			GetForm(typeof(ChannelsForm))?.RefreshSingleRow(index);
		}
	}

	public void RefreshRelatedForm(Type type)
	{
		RefreshRelatedForm(type, self: true);
	}

	public void RefreshRelatedForm(Type type, bool self)
	{
		if (type == typeof(DeviceInfoForm))
		{
			ChannelForm.data.Verify();
			RefreshForm(typeof(ChannelForm));
			VfoForm.data.Verify();
			RefreshForm(typeof(VfoForm));
		}
		else if (type == typeof(TextMsgForm))
		{
			RefreshForm(typeof(ButtonForm));
		}
		else if (type == typeof(EncryptForm))
		{
			RefreshForm(typeof(ChannelForm));
		}
		else if (type == typeof(ContactForm))
		{
			RefreshForm(typeof(ButtonForm));
			RefreshForm(typeof(ChannelForm));
			RefreshForm(typeof(RxGroupListForm));
			RefreshForm(typeof(DigitalKeyContactForm));
			RefreshForm(typeof(ContactsForm));
		}
		else if (type == typeof(ContactsForm))
		{
			RefreshForm(typeof(ButtonForm));
			RefreshForm(typeof(ChannelForm));
			RefreshForm(typeof(RxGroupListForm));
			RefreshForm(typeof(DigitalKeyContactForm));
			RefreshForm(typeof(ContactForm));
		}
		else if (type == typeof(NormalScanForm))
		{
			RefreshForm(typeof(ChannelForm));
		}
		else if (type == typeof(ChannelForm))
		{
			if (self)
			{
				RefreshForm(typeof(ChannelForm));
			}
			RefreshForm(typeof(NormalScanForm));
			RefreshForm(typeof(ZoneForm));
			RefreshForm(typeof(ZoneBasicForm));
			RefreshForm(typeof(AttachmentForm));
			RefreshForm(typeof(APRSForm));
			RefreshForm(typeof(ChannelsForm));
		}
		else if (type == typeof(ChannelsForm))
		{
			RefreshForm(typeof(ChannelForm));
			RefreshForm(typeof(NormalScanForm));
			RefreshForm(typeof(ZoneForm));
			RefreshForm(typeof(ZoneBasicForm));
			RefreshForm(typeof(AttachmentForm));
			RefreshForm(typeof(APRSForm));
		}
		else if (type == typeof(DtmfContactForm))
		{
			RefreshForm(typeof(ButtonForm));
		}
		else if (type == typeof(RxGroupListForm))
		{
			RefreshForm(typeof(ChannelForm));
		}
		else if (type == typeof(ZoneForm))
		{
			RefreshForm(typeof(ZoneBasicForm));
			RefreshForm(typeof(AttachmentForm));
			if (self)
			{
				RefreshForm(typeof(ZoneForm));
			}
		}
		else if (type == typeof(ZoneBasicForm))
		{
			RefreshForm(typeof(NormalScanForm));
		}
		else if (type == typeof(APRSForm))
		{
			RefreshForm(typeof(ChannelForm));
			RefreshForm(typeof(VfoForm));
			RefreshForm(typeof(APRSConfigsForm));
		}
		else if (type == typeof(APRSConfigsForm))
		{
			RefreshForm(typeof(ChannelForm));
			RefreshForm(typeof(VfoForm));
			RefreshForm(typeof(APRSForm));
		}
	}

	public void InitTree()
	{
		tvwMain.Nodes.Clear();
		initialiseTree();
		method_20(method_19());
		lstFixedNode = tvwMain.smethod_5();
		lstFixedNode.ForEach(delegate(TreeNode treeNode_0)
		{
			if (dicTree.ContainsKey(treeNode_0.Name))
			{
				if (treeNode_0.Name == "Model")
				{
					treeNode_0.Text = GeneralSetForm.data.Callsign;
				}
				else
				{
					treeNode_0.Text = dicTree[treeNode_0.Name];
				}
			}
		});
		InitDynamicNode();
		Settings.smethod_49(tvwMain, 0);
	}

	public void InitRxGroupLists(TreeNode parentNode)
	{
		int num = 0;
		for (num = 0; num < 76; num++)
		{
			if (RxGroupListForm.data.DataIsValid(num))
			{
				AddTreeViewNode(parentNode.Nodes, RxGroupListForm.data[num].Name, new TreeNodeItem(cmsSub, typeof(RxGroupListForm), null, 76, num, 19, RxGroupListForm.data));
			}
		}
	}

	public void InitAPRSConfigurations(TreeNode parentNode)
	{
		for (int i = 0; i < 8; i++)
		{
			if (APRSForm.data.DataIsValid(i))
			{
				AddTreeViewNode(parentNode.Nodes, APRSForm.data[i].Name, new TreeNodeItem(cmsSub, typeof(APRSForm), null, 32, i, 11, APRSForm.data));
			}
		}
	}

	public void InitZones(TreeNode parentNode)
	{
		int num = 0;
		try
		{
			for (num = 0; num < 68; num++)
			{
				if (ZoneForm.data.DataIsValid(num))
				{
					AddTreeViewNode(parentNode.Nodes, ZoneForm.data.GetName(num), new TreeNodeItem(cmsSub, typeof(ZoneForm), null, 68, num, 25, ZoneForm.data));
				}
			}
		}
		catch (Exception)
		{
			throw;
		}
	}

	public void InitDigitContacts(TreeNode parentNode)
	{
		int num = 0;
		method_5(typeof(ContactForm));
		parentNode.Nodes.Clear();
		for (num = 0; num < 1024; num++)
		{
			if (ContactForm.data.DataIsValid(num))
			{
				if (ContactForm.data.GetCallType(num) == 0)
				{
					AddTreeViewNode(parentNode.Nodes, ContactForm.data[num].Name, new TreeNodeItem(cmsSub, typeof(ContactForm), null, 1024, num, 8, ContactForm.data));
				}
				else if (ContactForm.data.GetCallType(num) == 1)
				{
					AddTreeViewNode(parentNode.Nodes, ContactForm.data[num].Name, new TreeNodeItem(cmsSub, typeof(ContactForm), null, 1024, num, 10, ContactForm.data));
				}
				else if (ContactForm.data.GetCallType(num) == 2)
				{
					AddTreeViewNode(parentNode.Nodes, ContactForm.data[num].Name, new TreeNodeItem(cmsSub, typeof(ContactForm), null, 1024, num, 7, ContactForm.data));
				}
			}
		}
	}

	public void InitChannels(TreeNode parentNode)
	{
		int num = 0;
		int num2 = 0;
		method_5(typeof(ChannelForm));
		parentNode.Nodes.Clear();
		for (num = 0; num < ChannelForm.CurCntCh; num++)
		{
			num2 = num;
			if (ChannelForm.data.DataIsValid(num2))
			{
				int chMode = ChannelForm.data.GetChMode(num2);
				AddTreeViewNode(parentNode.Nodes, ChannelForm.data.GetName(num2), new TreeNodeItem(cmsSub, typeof(ChannelForm), null, ChannelForm.CurCntCh, num, chMode switch
				{
					0 => 2, 
					1 => 6, 
					_ => 54, 
				}, ChannelForm.data));
			}
		}
	}

	public void InitScans(TreeNode parentNode)
	{
		int num = 0;
		for (num = 0; num < 64; num++)
		{
			if (NormalScanForm.data.DataIsValid(num))
			{
				AddTreeViewNode(parentNode.Nodes, NormalScanForm.data[num].Name, new TreeNodeItem(cmsSub, typeof(NormalScanForm), null, 64, num, 26, NormalScanForm.data));
			}
		}
	}

	public void InsertTreeViewNode(TreeNode parentNode, int index, Type formType, int imageIndex, IData data)
	{
		string name = data.GetName(index);
		AddTreeViewNode(parentNode.Nodes, name, new TreeNodeItem(cmsSub, formType, null, data.Count, index, imageIndex, data));
	}

	public void DeleteTreeViewNode(TreeNode parentNode, int index)
	{
		parentNode.Nodes.RemoveAt(index);
	}

	public void RefreshTreeNodeText(TreeNode parentNode, int rowIndex, int index)
	{
		parentNode.Nodes[rowIndex].Text = ContactForm.data[index].Name;
	}

	public void RefreshTreeNodeImage(TreeNode parentNode, int rowIndex, int index)
	{
		TreeNode treeNode = parentNode.Nodes[rowIndex];
		TreeNodeItem treeNodeItem = treeNode.Tag as TreeNodeItem;
		treeNode.ImageIndex = treeNodeItem.ImageIndex;
		treeNode.SelectedImageIndex = treeNodeItem.ImageIndex;
	}

	public TreeNode AddTreeViewNode(TreeNodeCollection parentNode, string text, object tag)
	{
		TreeNode treeNode = null;
		if (!(tag is TreeNodeItem treeNodeItem))
		{
			treeNode.Name = text;
			treeNode = parentNode.Add(text);
			treeNode.ImageIndex = 2;
			treeNode.SelectedImageIndex = 2;
			treeNode.Tag = null;
		}
		else
		{
			treeNode = ((treeNodeItem.Index >= 0) ? parentNode.Insert(treeNodeItem.Index, text) : parentNode.Add(text));
			treeNode.Name = text;
			treeNode.ImageIndex = treeNodeItem.ImageIndex;
			treeNode.SelectedImageIndex = treeNodeItem.ImageIndex;
			treeNode.Tag = tag;
		}
		return treeNode;
	}

	public TreeNode GetTreeNodeByType(Type type)
	{
		return method_9(type, tvwMain.Nodes);
	}

	public TreeNode GetTreeNodeByType(Type type, int index)
	{
		foreach (TreeNode node in GetTreeNodeByType(type).Nodes)
		{
			if (node.Tag is TreeNodeItem treeNodeItem && treeNodeItem.Index == index)
			{
				return node;
			}
		}
		return null;
	}

	private TreeNode method_8(Type type_0, TreeNodeCollection treeNodeCollection_0)
	{
		foreach (TreeNode item in treeNodeCollection_0)
		{
			if (item.Tag is TreeNodeItem treeNodeItem && treeNodeItem.SubType == type_0)
			{
				return item;
			}
			TreeNode treeNode2 = method_8(type_0, item.Nodes);
			if (treeNode2 != null)
			{
				return treeNode2;
			}
		}
		return null;
	}

	private TreeNode method_9(Type type_0, TreeNodeCollection treeNodeCollection_0)
	{
		foreach (TreeNode item in treeNodeCollection_0)
		{
			if (item.Tag is TreeNodeItem treeNodeItem && treeNodeItem.Type == type_0)
			{
				return item;
			}
			TreeNode treeNode2 = method_9(type_0, item.Nodes);
			if (treeNode2 != null)
			{
				return treeNode2;
			}
		}
		return null;
	}

	public TreeNode GetTreeNodeByTypeAndIndex(Type type, int index, TreeNodeCollection nodes)
	{
		foreach (TreeNode node in nodes)
		{
			if (node.Tag is TreeNodeItem treeNodeItem && treeNodeItem.Type == type && treeNodeItem.Index == index)
			{
				return node;
			}
			TreeNode treeNodeByTypeAndIndex = GetTreeNodeByTypeAndIndex(type, index, node.Nodes);
			if (treeNodeByTypeAndIndex != null)
			{
				return treeNodeByTypeAndIndex;
			}
		}
		return null;
	}

	private void _TextBox_LostFocus(object sender, EventArgs e)
	{
		_TextBox.Visible = false;
	}

	private void _TextBox_Validating(object sender, CancelEventArgs e)
	{
		tvwMain.SelectedNode.Text = _TextBox.Text;
	}

	[DllImport("user32.dll")]
	private static extern IntPtr SendMessage(IntPtr intptr_0, uint uint_0, IntPtr intptr_1, IntPtr intptr_2);

	private void tvwMain_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
	{
		if (e.Button != MouseButtons.Right)
		{
			return;
		}
		if (e.Node.Parent != null && e.Node != null)
		{
			tvwMain.SelectedNode = e.Node;
			if (e.Node.Tag is TreeNodeItem { Cms: not null } treeNodeItem)
			{
				method_2();
				treeNodeItem.Cms.Show(tvwMain, e.X, e.Y);
			}
		}
		else
		{
			cmsTree.Show(tvwMain, e.X, e.Y);
		}
	}

	private void tvwMain_DoubleClick(object sender, EventArgs e)
	{
		TreeNode treeNode = null;
		treeNode = tvwMain.SelectedNode;
		if (treeNode != null && treeNode.Tag is TreeNodeItem)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

	private void tvwMain_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
	{
		_ = e.Node;
		IntPtr intPtr = SendMessage(tvwMain.Handle, 4367u, IntPtr.Zero, IntPtr.Zero);
		if (!(intPtr != IntPtr.Zero))
		{
			return;
		}
		int value = 0;
		if (e.Node.Tag is TreeNodeItem treeNodeItem)
		{
			if (treeNodeItem.Type == typeof(ChannelForm))
			{
				value = 16;
			}
			else if (treeNodeItem.Type == typeof(ContactForm))
			{
				value = 16;
			}
			else if (treeNodeItem.Type == typeof(APRSForm))
			{
				value = 8;
			}
			else if (treeNodeItem.Type == typeof(NormalScanForm))
			{
				value = 15;
			}
			else if (treeNodeItem.Type == typeof(ZoneForm))
			{
				value = 16;
			}
			else if (treeNodeItem.Type == typeof(RxGroupListForm))
			{
				value = 16;
			}
		}
		SendMessage(intPtr, 197u, new IntPtr(value), IntPtr.Zero);
	}

	private void tvwMain_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
	{
		try
		{
			if (e.Label == null || e.Node.Text == e.Label)
			{
				return;
			}
			if (Settings.nodeNameExistsOrEmpty(e.Node, e.Label))
			{
				MessageBox.Show("Имя существует!");
				e.CancelEdit = true;
			}
			else if (e.Node.Tag is TreeNodeItem { Data: var data } treeNodeItem)
			{
				data.SetName(treeNodeItem.Index, e.Label);
				RefreshRelatedForm(treeNodeItem.Type);
				e.Node.Text = data.GetName(treeNodeItem.Index);
				e.CancelEdit = true;
				Form form = method_6(e.Node);
				if (form != null && form is IDisp disp && (int)form.Tag == treeNodeItem.Index)
				{
					disp.RefreshName();
				}
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
		tvwMain.LabelEdit = false;
	}

	private void tvwMain_KeyDown(object sender, KeyEventArgs e)
	{
		if (!(sender is TreeView { SelectedNode: { Tag: TreeNodeItem { Cms: not null } tag } selectedNode }))
		{
			return;
		}
		Keys keys = e.KeyData;
		if (e.KeyData == Keys.Return)
		{
			keys |= Keys.Control;
		}
		ToolStripMenuItem toolStripMenuItem = method_10(tag.Cms.Items, keys);
		if (toolStripMenuItem == null)
		{
			return;
		}
		if (tag.Type != null)
		{
			string name = tag.Type.Name;
			if (name == "ZoneForm" || name == "ChannelForm")
			{
				if (selectedNode.Index == selectedNode.Parent.Nodes.Count - 1)
				{
					tsmiMoveDown.Visible = false;
				}
				else
				{
					tsmiMoveDown.Visible = true;
				}
				if (selectedNode.Index == 0)
				{
					tsmiMoveUp.Visible = false;
				}
				else
				{
					tsmiMoveUp.Visible = true;
				}
			}
			else
			{
				tsmiMoveDown.Visible = false;
				tsmiMoveUp.Visible = false;
			}
		}
		toolStripMenuItem.PerformClick();
	}

	private ToolStripMenuItem method_10(ToolStripItemCollection toolStripItemCollection_0, Keys keys_0)
	{
		foreach (ToolStripItem item in toolStripItemCollection_0)
		{
			if (item is ToolStripMenuItem toolStripMenuItem)
			{
				if (toolStripMenuItem.ShortcutKeys == keys_0)
				{
					return toolStripMenuItem;
				}
				ToolStripMenuItem toolStripMenuItem2 = method_10(toolStripMenuItem.DropDownItems, keys_0);
				if (toolStripMenuItem2 != null)
				{
					return toolStripMenuItem2;
				}
			}
		}
		return null;
	}

	private void tsmiCollapseAll_Click(object sender, EventArgs e)
	{
		foreach (TreeNode node in tvwMain.Nodes)
		{
			foreach (TreeNode node2 in node.Nodes)
			{
				if (node2.IsExpanded)
				{
					node2.Collapse(ignoreChildren: false);
				}
			}
		}
	}

	private void tsmiExpandAll_Click(object sender, EventArgs e)
	{
		tvwMain.ExpandAll();
	}

	private void cmsGroup_Opening(object sender, CancelEventArgs e)
	{
		TreeNode selectedNode = tvwMain.SelectedNode;
		if (selectedNode != null && selectedNode.Tag is TreeNodeItem treeNodeItem)
		{
			tsmiAdd.Visible = selectedNode.Nodes.Count < treeNodeItem.Count;
			tsmiClear.Visible = selectedNode.Nodes.Count > 1;
		}
	}

	private void tsmiAdd_Click(object sender, EventArgs e)
	{
		int num = -1;
		string text = "";
		TreeNode selectedNode = tvwMain.SelectedNode;
		if (selectedNode != null && selectedNode.Tag is TreeNodeItem treeNodeItem && selectedNode.Nodes.Count < treeNodeItem.Count && treeNodeItem.SubType != null)
		{
			num = treeNodeItem.Data.GetMinIndex();
			text = treeNodeItem.Data.GetMinName(selectedNode);
			treeNodeItem.Data.SetIndex(num, 1);
			treeNodeItem.Data.Default(num);
			bool flag = false;
			if (treeNodeItem.SubType == typeof(NormalScanForm))
			{
				AddTreeViewNode(selectedNode.Nodes, text, new TreeNodeItem(cmsSub, treeNodeItem.SubType, null, 0, num, 26, treeNodeItem.Data));
			}
			else if (treeNodeItem.SubType == typeof(ZoneForm))
			{
				AddTreeViewNode(selectedNode.Nodes, text, new TreeNodeItem(cmsSub, treeNodeItem.SubType, null, 0, num, 25, treeNodeItem.Data));
			}
			else if (treeNodeItem.SubType == typeof(ChannelForm))
			{
				AddTreeViewNode(selectedNode.Nodes, text, new TreeNodeItem(cmsSub, treeNodeItem.SubType, null, 0, num, 2, treeNodeItem.Data));
				ChannelForm.Channel obj = (ChannelForm.Channel)treeNodeItem.Data;
				obj.SetChMode(num, ChannelForm.ChModeE.Digital);
				obj.SetDefaultFreq(num);
				flag = true;
			}
			else if (treeNodeItem.SubType == typeof(RxGroupListForm))
			{
				AddTreeViewNode(selectedNode.Nodes, text, new TreeNodeItem(cmsSub, treeNodeItem.SubType, null, 0, num, 19, treeNodeItem.Data));
				DispChildForm(typeof(RxGroupListForm), num);
			}
			else if (treeNodeItem.SubType == typeof(APRSForm))
			{
				AddTreeViewNode(selectedNode.Nodes, text, new TreeNodeItem(cmsSub, treeNodeItem.SubType, null, 0, num, 11, treeNodeItem.Data));
			}
			treeNodeItem.Data.SetName(num, text);
			slblCompany.Text = string.Format(Settings.SZ_ADD + text);
			if (!selectedNode.IsExpanded)
			{
				selectedNode.Expand();
			}
			if (base.ActiveMdiChild is IDisp disp)
			{
				disp.SaveData();
			}
			RefreshRelatedForm(treeNodeItem.SubType);
			if (flag)
			{
				DispChildForm(typeof(ChannelForm), num);
			}
		}
	}

	private void tsmiClear_Click(object sender, EventArgs e)
	{
		int num = 0;
		TreeNode selectedNode = tvwMain.SelectedNode;
		if (selectedNode != null && selectedNode.Tag is TreeNodeItem treeNodeItem && selectedNode.Nodes.Count > 1 && treeNodeItem.SubType != null)
		{
			string text = string.Format(treeNodeItem.Data.Format, num + 1);
			treeNodeItem.Data.SetName(0, text);
			selectedNode.Nodes[0].Text = text;
			treeNodeItem.Data.Default(0);
			for (num = 1; num < treeNodeItem.Data.Count; num++)
			{
				treeNodeItem.Data.SetIndex(num, 0);
			}
			while (selectedNode.Nodes.Count > 1)
			{
				selectedNode.Nodes.RemoveAt(1);
			}
			if (!selectedNode.IsExpanded)
			{
				selectedNode.Expand();
			}
			Form form = method_6(selectedNode.Nodes[0]);
			if (form != null && form is IDisp disp)
			{
				disp.Node = selectedNode.Nodes[0];
				form.Tag = 0;
				disp.DispData();
			}
			RefreshRelatedForm(treeNodeItem.SubType);
		}
	}

	private void cmsSub_Opening(object sender, CancelEventArgs e)
	{
		TreeNode selectedNode = tvwMain.SelectedNode;
		if (selectedNode == null || !(selectedNode.Tag is TreeNodeItem treeNodeItem))
		{
			return;
		}
		string name = treeNodeItem.Type.Name;
		if (name == "ZoneForm" || name == "ChannelForm")
		{
			if (selectedNode.Index == selectedNode.Parent.Nodes.Count - 1)
			{
				tsmiMoveDown.Visible = false;
			}
			else
			{
				tsmiMoveDown.Visible = true;
			}
			if (selectedNode.Index == 0)
			{
				tsmiMoveUp.Visible = false;
			}
			else
			{
				tsmiMoveUp.Visible = true;
			}
		}
		else
		{
			tsmiMoveDown.Visible = false;
			tsmiMoveUp.Visible = false;
		}
		if (treeNodeItem.Type == typeof(ChannelForm))
		{
			if (treeNodeItem.Index + 1 == ZoneForm.data.FstZoneFstCh)
			{
				tsmiDel.Visible = true;
			}
			else
			{
				tsmiDel.Visible = true;
			}
		}
		else
		{
			tsmiDel.Visible = true;
		}
		tsmiPaste.Visible = CopyItem != null && CopyItem != treeNodeItem && CopyItem.Type == treeNodeItem.Type;
	}

	private void tsmiRename_Click(object sender, EventArgs e)
	{
		method_2();
		tvwMain.LabelEdit = true;
		tvwMain.SelectedNode.BeginEdit();
	}

	private void tsmiDel_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = null;
		TreeNode selectedNode = tvwMain.SelectedNode;
		TreeNodeItem treeNodeItem = null;
		if (selectedNode == null)
		{
			return;
		}
		treeNode = selectedNode.Parent;
		treeNodeItem = selectedNode.Tag as TreeNodeItem;
		method_2();
		if (treeNodeItem == null || treeNode.Nodes.Count <= 0 || treeNodeItem.Cms != cmsSub)
		{
			return;
		}
		Form form = method_6(selectedNode);
		if (form != null && form is IDisp disp && selectedNode == disp.Node)
		{
			if (selectedNode.NextNode != null)
			{
				tvwMain.SelectedNode = selectedNode.NextNode;
			}
			else if (selectedNode.PrevNode != null)
			{
				tvwMain.SelectedNode = selectedNode.PrevNode;
			}
			else
			{
				tvwMain.SelectedNode = treeNode;
			}
			disp.Node = tvwMain.SelectedNode;
			TreeNodeItem treeNodeItem2 = disp.Node.Tag as TreeNodeItem;
			form.Tag = treeNodeItem2.Index;
			disp.DispData();
		}
		TreeNode treeNode2 = selectedNode.Parent;
		treeNode.Nodes.Remove(selectedNode);
		if (treeNodeItem != null)
		{
			if (base.ActiveMdiChild is IDisp disp2)
			{
				disp2.SaveData();
			}
			treeNodeItem.Data.ClearIndex(treeNodeItem.Index);
			treeNodeItem.Data.Default(treeNodeItem.Index);
			RefreshRelatedForm(treeNodeItem.Type);
		}
		if (treeNodeItem == CopyItem)
		{
			CopyItem = null;
		}
		if (treeNodeItem.Type == typeof(ZoneForm))
		{
			ZoneForm.CompactZones();
			treeNode2.Nodes.Clear();
			InitZones(treeNode2);
		}
		if (treeNodeItem.Type == typeof(APRSForm))
		{
			APRSForm.Compact();
			treeNode2.Nodes.Clear();
			InitAPRSConfigurations(treeNode2);
		}
	}

	private void tsmiCopy_Click(object sender, EventArgs e)
	{
		TreeNode selectedNode = tvwMain.SelectedNode;
		if (selectedNode != null)
		{
			CopyItem = selectedNode.Tag as TreeNodeItem;
		}
	}

	private void tsmiPaste_Click(object sender, EventArgs e)
	{
		TreeNode selectedNode = tvwMain.SelectedNode;
		if (selectedNode == null)
		{
			return;
		}
		_ = selectedNode.Parent;
		if (selectedNode.Tag is TreeNodeItem treeNodeItem && CopyItem != null)
		{
			if (CopyItem.Type != treeNodeItem.Type)
			{
				MessageBox.Show(Settings.dicCommon["TypeNotMatch"]);
				return;
			}
			if (treeNodeItem != null && base.ActiveMdiChild is IDisp disp)
			{
				disp.SaveData();
			}
			treeNodeItem.Paste(CopyItem);
			Form form = method_6(selectedNode);
			if (form != null && form is IDisp disp2 && disp2.Node == selectedNode)
			{
				disp2.DispData();
			}
			RefreshRelatedForm(treeNodeItem.Type);
		}
		else
		{
			MessageBox.Show(Settings.dicCommon["NotSelectItemNotCopyItem"]);
		}
	}

	private void tsmiMoveDown_Click(object sender, EventArgs e)
	{
		TreeNode selectedNode = tvwMain.SelectedNode;
		if (selectedNode == null)
		{
			return;
		}
		TreeNodeItem treeNodeItem = selectedNode.Tag as TreeNodeItem;
		string name = treeNodeItem.Type.Name;
		if (!(name == "ZoneForm") && !(name == "ChannelForm"))
		{
			return;
		}
		closeForm(treeNodeItem);
		tvwMain.Focus();
		string name2 = treeNodeItem.Type.Name;
		if (!(name2 == "ZoneForm"))
		{
			if (name2 == "ChannelForm")
			{
				closeAllForms();
				ChannelForm.MoveChannelDown((selectedNode.Tag as TreeNodeItem).Index, (selectedNode.NextNode.Tag as TreeNodeItem).Index);
			}
		}
		else
		{
			ZoneForm.MoveZoneDown(selectedNode.Index);
			RefreshForm(typeof(ZoneBasicForm));
		}
		TreeNode treeNode = selectedNode.Parent;
		treeNode.Nodes.Clear();
		name2 = treeNodeItem.Type.Name;
		if (!(name2 == "ZoneForm"))
		{
			if (name2 == "ChannelForm")
			{
				InitChannels(treeNode);
			}
		}
		else
		{
			InitZones(treeNode);
		}
		if (selectedNode.Index < treeNode.Nodes.Count - 1)
		{
			tvwMain.SelectedNode = treeNode.Nodes[selectedNode.Index + 1];
		}
		else
		{
			tvwMain.SelectedNode = treeNode.Nodes[selectedNode.Index];
		}
	}

	private void tsmiMoveUp_Click(object sender, EventArgs e)
	{
		TreeNode selectedNode = tvwMain.SelectedNode;
		if (selectedNode == null)
		{
			return;
		}
		TreeNodeItem treeNodeItem = selectedNode.Tag as TreeNodeItem;
		string name = treeNodeItem.Type.Name;
		if (!(name == "ZoneForm") && !(name == "ChannelForm"))
		{
			return;
		}
		closeForm(treeNodeItem);
		tvwMain.Focus();
		string name2 = treeNodeItem.Type.Name;
		if (!(name2 == "ZoneForm"))
		{
			if (name2 == "ChannelForm")
			{
				closeAllForms();
				ChannelForm.MoveChannelUp((selectedNode.Tag as TreeNodeItem).Index, (selectedNode.PrevNode.Tag as TreeNodeItem).Index);
			}
		}
		else
		{
			ZoneForm.MoveZoneUp(selectedNode.Index);
			RefreshForm(typeof(ZoneBasicForm));
		}
		TreeNode treeNode = selectedNode.Parent;
		treeNode.Nodes.Clear();
		name2 = treeNodeItem.Type.Name;
		if (!(name2 == "ZoneForm"))
		{
			if (name2 == "ChannelForm")
			{
				InitChannels(treeNode);
			}
		}
		else
		{
			InitZones(treeNode);
		}
		if (selectedNode.Index > 0)
		{
			tvwMain.SelectedNode = treeNode.Nodes[selectedNode.Index - 1];
		}
		else
		{
			tvwMain.SelectedNode = treeNode.Nodes[selectedNode.Index];
		}
	}

	private void closeForm(TreeNodeItem treenodeitem)
	{
		Form[] mdiChildren = base.MdiChildren;
		foreach (Form form in mdiChildren)
		{
			if (form.GetType() == treenodeitem.Type)
			{
				form.Close();
			}
		}
	}

	private void closeAllForms(Type formType)
	{
		Form[] mdiChildren = base.MdiChildren;
		foreach (Form form in mdiChildren)
		{
			if (form.GetType() == formType)
			{
				form.Close();
			}
		}
	}

	private void cmsGroupContact_Opening(object sender, CancelEventArgs e)
	{
		TreeNode selectedNode = tvwMain.SelectedNode;
		if (selectedNode != null && selectedNode.Tag is TreeNodeItem treeNodeItem)
		{
			if (selectedNode.Nodes.Count >= treeNodeItem.Count)
			{
				tsmiAddContact.Enabled = false;
			}
			else
			{
				tsmiAddContact.Enabled = true;
			}
		}
		if (ContactForm.data.HaveAll())
		{
			tsmiAllCall.Enabled = false;
		}
		else
		{
			tsmiAllCall.Enabled = true;
		}
	}

	private void tsmiGroupCall_Click(object sender, EventArgs e)
	{
		int num = -1;
		string text = "";
		TreeNode selectedNode = tvwMain.SelectedNode;
		if (base.ActiveMdiChild is IDisp disp)
		{
			disp.SaveData();
		}
		if (selectedNode != null && selectedNode.Tag is TreeNodeItem treeNodeItem && selectedNode.Nodes.Count < treeNodeItem.Count)
		{
			num = treeNodeItem.Data.GetMinIndex();
			text = treeNodeItem.Data.GetMinName(selectedNode);
			treeNodeItem.Data.SetIndex(num, 0);
			AddTreeViewNode(selectedNode.Nodes, text, new TreeNodeItem(cmsSub, typeof(ContactForm), null, 0, num, 8, treeNodeItem.Data));
			ContactForm.Contact contact = (ContactForm.Contact)treeNodeItem.Data;
			treeNodeItem.Data.Default(num);
			contact.SetCallID(num, contact.GetMinCallID(0, num));
			treeNodeItem.Data.SetName(num, text);
			contact.SetRepeaterSlot(num, 0);
			RefreshRelatedForm(treeNodeItem.SubType);
			if (!selectedNode.IsExpanded)
			{
				selectedNode.Expand();
			}
			DispChildForm(typeof(ContactForm), num);
		}
	}

	private void tsmiPrivateCall_Click(object sender, EventArgs e)
	{
		int num = -1;
		string text = "";
		TreeNode selectedNode = tvwMain.SelectedNode;
		if (selectedNode != null && selectedNode.Tag is TreeNodeItem treeNodeItem && selectedNode.Nodes.Count < treeNodeItem.Count)
		{
			num = treeNodeItem.Data.GetMinIndex();
			text = treeNodeItem.Data.GetMinName(selectedNode);
			treeNodeItem.Data.SetIndex(num, 1);
			AddTreeViewNode(selectedNode.Nodes, text, new TreeNodeItem(cmsSub, typeof(ContactForm), null, 0, num, 10, treeNodeItem.Data));
			ContactForm.Contact contact = (ContactForm.Contact)treeNodeItem.Data;
			treeNodeItem.Data.Default(num);
			contact.SetCallID(num, contact.GetMinCallID(1, num));
			treeNodeItem.Data.SetName(num, text);
			contact.SetRepeaterSlot(num, 0);
			RefreshRelatedForm(treeNodeItem.SubType);
			if (!selectedNode.IsExpanded)
			{
				selectedNode.Expand();
			}
			DispChildForm(typeof(ContactForm), num);
		}
	}

	private void tsmiAllCall_Click(object sender, EventArgs e)
	{
		int num = -1;
		string text = "";
		TreeNode selectedNode = tvwMain.SelectedNode;
		if (selectedNode != null && selectedNode.Tag is TreeNodeItem treeNodeItem && selectedNode.Nodes.Count < treeNodeItem.Count)
		{
			num = treeNodeItem.Data.GetMinIndex();
			text = treeNodeItem.Data.GetMinName(selectedNode);
			treeNodeItem.Data.SetIndex(num, 2);
			ContactForm.Contact obj = (ContactForm.Contact)treeNodeItem.Data;
			AddTreeViewNode(selectedNode.Nodes, text, new TreeNodeItem(cmsSub, typeof(ContactForm), null, 0, num, 7, treeNodeItem.Data));
			treeNodeItem.Data.Default(num);
			treeNodeItem.Data.SetName(num, text);
			obj.SetCallID(num, 16777215.ToString());
			obj.SetRepeaterSlot(num, 0);
			RefreshRelatedForm(treeNodeItem.SubType);
			if (!selectedNode.IsExpanded)
			{
				selectedNode.Expand();
			}
			DispChildForm(typeof(ContactForm), num);
		}
	}

	private void tsmiSetting_DropDownOpening(object sender, EventArgs e)
	{
		tsmiDmrContacts.Visible = !ContactForm.data.ListIsEmpty;
		tsmiZone.Visible = !ZoneForm.data.ListIsEmpty;
		tsmiGrpRxList.Visible = !RxGroupListForm.data.ListIsEmpty;
		tsmiChannels.Visible = !ChannelForm.data.ListIsEmpty;
	}

	private void loadDefaultOrInitialFile(string overRideWithFile = null)
	{
		string text = Application.StartupPath + Path.DirectorySeparatorChar + DEFAULT_DATA_FILE_NAME;
		if (overRideWithFile != null)
		{
			text = overRideWithFile;
		}
		if (!string.IsNullOrEmpty(text) && File.Exists(text))
		{
			byte[] eerom = File.ReadAllBytes(text);
			closeAllForms();
			ByteToData(eerom);
			InitTree();
			Text = getMainTitleStub();
		}
	}

	private void tsbtnNew_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show(Settings.dicCommon["PromptKey2"], "", MessageBoxButtons.OKCancel) == DialogResult.OK)
		{
			loadDefaultOrInitialFile();
			CurFileName = "";
			IniFileUtils.WriteProfileString("Setup", "LastFilePath", "");
		}
	}

	private void tsbtnSave_Click(object sender, EventArgs e)
	{
		string profileStringWithDefault = IniFileUtils.getProfileStringWithDefault("Setup", "LastFilePath", "");
		string initialDirectory;
		try
		{
			initialDirectory = ((!(profileStringWithDefault == "")) ? Path.GetDirectoryName(profileStringWithDefault) : Environment.GetFolderPath(Environment.SpecialFolder.Personal));
		}
		catch (Exception)
		{
			initialDirectory = "";
		}
		try
		{
			if (string.IsNullOrEmpty(CurFileName))
			{
				sfdMain.FileName = GeneralSetForm.data.Callsign + "_" + DateTime.Now.ToString("MMdd_HHmmss") + ".ogd";
				sfdMain.InitialDirectory = initialDirectory;
			}
			else
			{
				sfdMain.InitialDirectory = Path.GetDirectoryName(CurFileName);
				sfdMain.FileName = Path.GetFileNameWithoutExtension(CurFileName) + ".ogd";
			}
			method_3();
			if (sfdMain.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(sfdMain.FileName))
			{
				byte[] array = DataToByte();
				Buffer.BlockCopy(Settings.CUR_MODEL, 0, array, 0, 8);
				File.WriteAllBytes(sfdMain.FileName, array);
				CurFileName = sfdMain.FileName;
				MessageBox.Show(Settings.dicCommon["SaveSuccessfully"]);
				IniFileUtils.WriteProfileString("Setup", "LastFilePath", sfdMain.FileName);
				Text = getMainTitleStub() + " " + CurFileName;
			}
		}
		catch (Exception ex2)
		{
			MessageBox.Show(ex2.Message);
		}
	}

	private void openCodeplugFile(string fileName)
	{
		int index = 0;
		byte[] array = File.ReadAllBytes(fileName);
		bool num = !array.Take(8).All((byte byte_0) => byte_0 == byte.MaxValue);
		bool flag = !array.Take(8).All((byte x) => x == Settings.CUR_MODEL[index++]);
		if (num && flag)
		{
			MessageBox.Show("Кодплаг в файле не является кодплагом поддерживаемых прошивок OpenGD77 RUS!\r\nЕсли он создан в стандартной CPS OpenGD77 или считан из рации под управлением стандартной прошивки OpenGD77, используйте пункт меню \"Файл->Импорт из OpenGD77\".", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			IniFileUtils.WriteProfileString("Setup", "LastFilePath", "");
			return;
		}
		else
		{
			CurFileName = fileName;
			IniFileUtils.WriteProfileString("Setup", "LastFilePath", fileName);
			closeAllForms();
			if (!checkZonesFor80Channels(array))
			{
				convertTo80ChannelZoneCodeplug(array);
			}
			ByteToData(array, isFromFile: true);
			InitTree();
			Text = getMainTitleStub() + " " + fileName;
		}
	}

    private void importCodeplugFile(string fileName)
    {
        int index = 0;
        byte[] OLD_CODEPLUG = new byte[8] { 77, 68, 45, 55, 54, 48, 80, 255 };
        byte[] array = File.ReadAllBytes(fileName);
        bool num = !array.Take(8).All((byte byte_0) => byte_0 == byte.MaxValue);
        bool flag = !array.Take(8).All((byte x) => x == OLD_CODEPLUG[index++]);
        if (num && flag)
        {
            MessageBox.Show("Кодплаг в файле не является кодплагом OpenGD77!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            IniFileUtils.WriteProfileString("Setup", "LastFilePath", "");
            return;
        }
        else
        {
            CurFileName = fileName;
            IniFileUtils.WriteProfileString("Setup", "LastFilePath", fileName);
            closeAllForms();
            if (!checkZonesFor80Channels(array))
            {
                convertTo80ChannelZoneCodeplug(array);
            }
            ByteToData(array, isFromFile: true);
            InitTree();
            Text = getMainTitleStub() + " " + fileName;
        }
    }

    private void tsbtnOpen_Click(object sender, EventArgs e)
	{
		try
		{
			string profileStringWithDefault = IniFileUtils.getProfileStringWithDefault("Setup", "LastFilePath", "");
			try
			{
				if (profileStringWithDefault == null || "" == profileStringWithDefault)
				{
					ofdMain.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				}
				else
				{
					ofdMain.InitialDirectory = Path.GetDirectoryName(profileStringWithDefault);
				}
			}
			catch (Exception)
			{
				ofdMain.InitialDirectory = "";
			}
			if (ofdMain.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(ofdMain.FileName))
			{
				openCodeplugFile(ofdMain.FileName);
			}
		}
		catch (Exception ex2)
		{
			MessageBox.Show(ex2.Message);
		}
	}

	private void tsbtnExportCSV_Click(object sender, EventArgs e)
	{
		closeAllForms();
		CSVEML.ExportCSV();
	}

	private void tsbtnImportCSV_Click(object sender, EventArgs e)
	{
		closeAllForms();
		CSVEML.ImportCSV();
		InitTree();
		OpenGD77Form.ClearLastUsedChannelsData();
	}

	private void tsbtnAppendCSV_Click(object sender, EventArgs e)
	{
		closeAllForms();
		CSVEML.AppendCSV();
		InitTree();
		OpenGD77Form.ClearLastUsedChannelsData();
	}

	private void tsbtnUpdateLocationCSV_Click(object sender, EventArgs e)
	{
		closeAllForms();
		CSVEML.UpdateLocationCSV();
		InitTree();
		OpenGD77Form.ClearLastUsedChannelsData();
	}

	private void tsmiExit_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void tsmiDeviceInfo_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(DeviceInfoForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

	private void tsmiGerneralSet_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(GeneralSetForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

	private void tsmiButton_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(ButtonForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

	private void tsmiMenu_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(MenuForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

	private void tsmiBootItem_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(BootItemForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

	private void tsmiNumKeyContact_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(DigitalKeyContactForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

	private void tsmiTextMsg_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(TextMsgForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: false);
		}
	}

	private void tsmiEncrypt_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(EncryptForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: false);
		}
	}

	private void tsmiSignalingSystem_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(SignalingBasicForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: false);
		}
	}

	private void tsmiDtmf_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(DtmfForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: false);
		}
	}

	private void tsmiEmgSystem_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(APRSForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: false);
		}
	}

	private void tsmiAPRS_Configs_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(APRSConfigsForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

	private void tsmiDtmfContact_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(DtmfContactForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: false);
		}
	}

	private void method_12(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(ContactsForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

	private void tsmiDmrContacts_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(ContactsForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

	private void tsmiGrpRxList_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(RxGroupListForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

	private void tsmiZoneBasic_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(ZoneBasicForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

	private void tsmiZoneList_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(ZoneForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

	private void tsmiChannels_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(ChannelsForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

    private void tsmiCodeplugSettings_Click(object sender, EventArgs e)
    {
        TreeNode treeNode = method_9(typeof(CodeplugSettingsForm), tvwMain.Nodes);
        if (treeNode != null)
        {
            treeviewDoubleClickHandler(treeNode, bool_0: true);
        }
    }

    private void tsmiScanBasic_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(ScanBasicForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

	private void tsmiScanList_Click(object sender, EventArgs e)
	{
		TreeNode treeNode = method_9(typeof(NormalScanForm), tvwMain.Nodes);
		if (treeNode != null)
		{
			treeviewDoubleClickHandler(treeNode, bool_0: true);
		}
	}

	private void tsmiVfoA_Click(object sender, EventArgs e)
	{
		TreeNode treeNodeByTypeAndIndex = GetTreeNodeByTypeAndIndex(typeof(VfoForm), 0, tvwMain.Nodes);
		if (treeNodeByTypeAndIndex != null)
		{
			treeviewDoubleClickHandler(treeNodeByTypeAndIndex, bool_0: true);
		}
	}

	private void tsmiVfoB_Click(object sender, EventArgs e)
	{
		TreeNode treeNodeByTypeAndIndex = GetTreeNodeByTypeAndIndex(typeof(VfoForm), 1, tvwMain.Nodes);
		if (treeNodeByTypeAndIndex != null)
		{
			treeviewDoubleClickHandler(treeNodeByTypeAndIndex, bool_0: true);
		}
	}

	private void tsbtnContactsDownload_Click(object sender, EventArgs e)
	{
        closeAllForms();
		DownloadContactsForm downloadContactsForm = new DownloadContactsForm();
		downloadContactsForm.mainForm = this;
		TreeNode treeNode = method_9(typeof(ContactsForm), tvwMain.Nodes);
		downloadContactsForm.treeNode = treeNode;
		try
		{
			downloadContactsForm.ShowDialog();
		}
		catch (Exception)
		{
			Cursor.Current = Cursors.Default;
			MessageBox.Show(Settings.dicCommon["UnableDownloadFromInternet"]);
		}
	}

	private void tsbtnDMRID_Click(object sender, EventArgs e)
	{
        closeAllForms();
		DMRIDForm dMRIDForm = new DMRIDForm();
		dMRIDForm.StartPosition = FormStartPosition.CenterParent;
		dMRIDForm.ShowDialog();
	}

	private void tsbtnCalibration_Click(object sender, EventArgs e)
	{
        closeAllForms();
        if (MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32)
        {
            CalibrationFormMDUV380 calibrationForm = new CalibrationFormMDUV380();
            calibrationForm.StartPosition = FormStartPosition.CenterParent;
            calibrationForm.ShowDialog();
        }
        else
        {
            CalibrationForm calibrationForm = new CalibrationForm();
            calibrationForm.StartPosition = FormStartPosition.CenterParent;
            calibrationForm.ShowDialog();
        }

	}

	private void tsbtnTheme_Click(object sender, EventArgs e)
	{
        closeAllForms();
		ThemeForm themeForm = new ThemeForm();
		themeForm.StartPosition = FormStartPosition.CenterParent;
		themeForm.ShowDialog();
	}

	private void openGD77Form(OpenGD77CommsTransferData.CommsAction buttonAction)
	{
        closeAllForms();
		OpenGD77Form obj = new OpenGD77Form(buttonAction);
		obj.StartPosition = FormStartPosition.CenterParent;
		obj.ShowDialog();
		InitTree();
	}

	private void tsmiOpenGD77_Click(object sender, EventArgs e)
	{
        openGD77Form(OpenGD77CommsTransferData.CommsAction.NONE);
	}

	private void tsmiFirmwareLoader_Click(object sender, EventArgs e)
	{
        RadioTypeEnum radioType = RadioType;
		if (radioType != 0 && radioType == RadioTypeEnum.RadioTypeSTM32)
		{
			new FirmwareLoaderUI_STM32().ShowDialog();
		}
		else
		{
			new FirmwareLoaderUI_MK22().ShowDialog();
		}
	}

	private void tsmiFirmwareLoaderMD9600_Click(object sender, EventArgs e)
	{
        new FirmwareLoaderUI_STM32().ShowDialog();
	}

	private void tsmiRadioTypeClickHandler(object sender, EventArgs e)
	{
		ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
		foreach (ToolStripMenuItem dropDownItem in tsmiRadioType.DropDownItems)
		{
			dropDownItem.Checked = dropDownItem == toolStripMenuItem;
		}
		setRadioType((RadioTypeEnum)toolStripMenuItem.Tag);
	}

	private void setRadioType(RadioTypeEnum radio)
	{
		RadioType = radio;
		tsmiCalibrationMK22.Enabled = true;
		RadioTypeEnum radioType = RadioType;
		string string_;
		if (radioType != 0 && radioType == RadioTypeEnum.RadioTypeSTM32)
		{
			string_ = "MD9600";
			tsmiTheme.Enabled = true;
		}
		else
		{
			string_ = "MK22";
			tsmiTheme.Enabled = false;
		}
		IniFileUtils.WriteProfileString("Setup", "RadioType", string_);
	}

	public void changeRadioType(RadioTypeEnum radio)
	{
		foreach (ToolStripMenuItem dropDownItem in tsmiRadioType.DropDownItems)
		{
			dropDownItem.Checked = (RadioTypeEnum)dropDownItem.Tag == radio;
		}
		setRadioType(radio);
	}

	private void tsbtnRead_Click(object sender, EventArgs e)
	{
		openGD77Form(OpenGD77CommsTransferData.CommsAction.READ_CODEPLUG);
	}

	private void tsbtnWrite_Click(object sender, EventArgs e)
	{
        openGD77Form(OpenGD77CommsTransferData.CommsAction.WRITE_CODEPLUG);
	}

	private void tsmiTree_Click(object sender, EventArgs e)
	{
		if (tsmiTree.Checked = !tsmiTree.Checked)
		{
			frmTree.Show(dockPanel);
		}
		else
		{
			frmTree.Hide();
		}
	}

	private void tsmiHelp_Click(object sender, EventArgs e)
	{
		if (tsmiHelp.Checked = !tsmiHelp.Checked)
		{
			frmHelp.Show(dockPanel);
		}
		else
		{
			frmHelp.Hide();
		}
	}

	private void tsmiToolBar_Click(object sender, EventArgs e)
	{
		ToolStrip toolStrip = tsrMain;
		bool flag2 = (tsmiToolBar.Checked = !tsmiToolBar.Checked);
		bool visible = flag2;
		toolStrip.Visible = visible;
	}

	private void tsmiStatusBar_Click(object sender, EventArgs e)
	{
		StatusStrip statusStrip = ssrMain;
		bool flag2 = (tsmiStatusBar.Checked = !tsmiStatusBar.Checked);
		bool visible = flag2;
		statusStrip.Visible = visible;
	}

	private void tsmiCascade_Click(object sender, EventArgs e)
	{
		LayoutMdi(MdiLayout.Cascade);
	}

	private void tsmiTileHor_Click(object sender, EventArgs e)
	{
		LayoutMdi(MdiLayout.TileHorizontal);
	}

	private void tsmiTileVer_Click(object sender, EventArgs e)
	{
		LayoutMdi(MdiLayout.TileVertical);
	}

	private void tsmiCloseAll_Click(object sender, EventArgs e)
	{
		closeAllForms();
	}

	private void tsbtnAbout_Click(object sender, EventArgs e)
	{
		AboutForm aboutForm = new AboutForm();
		aboutForm.StartPosition = FormStartPosition.CenterParent;
		aboutForm.ShowDialog();
	}

	private void FormPanel_VisibleChanged(object sender, EventArgs e)
	{
		if (sender == frmTree)
		{
			tsmiTree.Checked = !frmTree.IsHidden;
		}
		else if (sender == frmHelp)
		{
			tsmiHelp.Checked = !frmHelp.IsHidden;
		}
	}

	private void method_13()
	{
		ButtonForm.RefreshCommonLang();
		BootItemForm.RefreshCommonLang();
		ChannelForm.RefreshCommonLang();
		ChannelsForm.RefreshCommonLang();
		ContactForm.RefreshCommonLang();
		ContactsForm.RefreshCommonLang();
		APRSConfigsForm.RefreshCommonLang();
		DigitalKeyContactForm.RefreshCommonLang();
		DtmfForm.RefreshCommonLang();
		APRSForm.RefreshCommonLang();
		EncryptForm.RefreshCommonLang();
		GeneralSetForm.RefreshCommonLang();
		MenuForm.RefreshCommonLang();
		NormalScanForm.RefreshCommonLang();
		VfoForm.RefreshCommonLang();
		Settings.smethod_10();
	}

	private void languageChangeHandler(object sender, EventArgs e)
	{
		slblCompany.Text = "";
		closeAllForms();
		frmHelp.ShowHelp(null);
		ToolStripMenuItem obj = sender as ToolStripMenuItem;
		Language_Name = obj.Name;
		string text = obj.Tag.ToString();
		IniFileUtils.WriteProfileString("Setup", "Language", Path.GetFileName(text));
		Settings.setLanguageXMLFile(text);
		Settings.SetHelpFilename(Path.ChangeExtension(text, "chm"));
		string helpFilename = Settings.GetHelpFilename();
		if (!File.Exists(helpFilename))
		{
			Settings.SetHelpFilename(Path.GetDirectoryName(helpFilename) + Path.DirectorySeparatorChar + "English.chm");
		}
		Settings.smethod_76("Read", ref Settings.SZ_READ);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		Settings.UpdateComponentTextsFromLanguageXmlData(frmHelp);
		Settings.UpdateComponentTextsFromLanguageXmlData(frmTree);
		Settings.UpdateContextMenuStripFromLanguageXmlData(cmsGroup.smethod_9(), base.Name);
		Settings.UpdateContextMenuStripFromLanguageXmlData(cmsGroupContact.smethod_9(), base.Name);
		Settings.UpdateContextMenuStripFromLanguageXmlData(cmsTree.smethod_9(), base.Name);
		Settings.UpdateContextMenuStripFromLanguageXmlData(cmsSub.smethod_9(), base.Name);
		Settings.ReadCommonsTextIntoDictionary(Settings.dicCommon);
		method_13();
		Settings.smethod_75(new List<string> { Settings.SZ_READ }, new List<string> { "Read" });
		dicTree.Clear();
		string xpath = "/Resource/MainForm/TreeView/TreeNode";
		foreach (XmlNode item in Settings.languageXML.SelectNodes(xpath))
		{
			string value = item.Attributes["Id"].Value;
			string value2 = item.Attributes["Text"].Value;
			dicTree.Add(value, value2);
		}
		lstFixedNode.ForEach(delegate(TreeNode treeNode_0)
		{
			if (dicTree.ContainsKey(treeNode_0.Name))
			{
				if (treeNode_0.Name == "Model")
				{
					treeNode_0.Text = GeneralSetForm.data.Callsign;
				}
				else
				{
					treeNode_0.Text = dicTree[treeNode_0.Name];
				}
			}
		});
		List<ToolStripMenuItem> list = mnsMain.smethod_7();
		Dictionary<string, string> dicMenuItem = new Dictionary<string, string>();
		xpath = "/Resource/MainForm/MenuStrip/MenuItem";
		foreach (XmlNode item2 in Settings.languageXML.SelectNodes(xpath))
		{
			string value3 = item2.Attributes["Id"].Value;
			string value4 = item2.Attributes["Text"].Value;
			dicMenuItem.Add(value3, value4);
		}
		list.ForEach(delegate(ToolStripMenuItem x)
		{
			if (dicMenuItem.ContainsKey(x.Name))
			{
				x.Text = dicMenuItem[x.Name];
			}
		});
		List<ToolStripItem> list2 = tsrMain.smethod_10();
		Dictionary<string, string> dicToolItem = new Dictionary<string, string>();
		xpath = "/Resource/MainForm/ToolStrip/ToolItem";
		foreach (XmlNode item3 in Settings.languageXML.SelectNodes(xpath))
		{
			string value5 = item3.Attributes["Id"].Value;
			string value6 = item3.Attributes["Text"].Value;
			dicToolItem.Add(value5, value6);
		}
		list2.ForEach(delegate(ToolStripItem x)
		{
			if (dicToolItem.ContainsKey(x.Name))
			{
				string text3 = (x.ToolTipText = dicToolItem[x.Name]);
				x.Text = text3;
			}
		});
		Text = getMainTitleStub() + " " + _lastCodeplugFileName;
	}

	public static byte[] DataToByte()
	{
		byte[] array = new byte[Settings.EEROM_SPACE];
		array.Fill(byte.MaxValue);
		DataVerify();
		byte[] array2 = Settings.objectToByteArray(GeneralSetForm.data, Marshal.SizeOf(GeneralSetForm.data.GetType()));
		Array.Copy(array2, 0, array, Settings.ADDR_GENERAL_SET, array2.Length);
		array2 = Settings.objectToByteArray(DeviceInfoForm.data, Marshal.SizeOf(DeviceInfoForm.data.GetType()));
		Array.Copy(array2, 0, array, Settings.ADDR_DEVICE_INFO, array2.Length);
		array2 = Settings.objectToByteArray(ButtonForm.data, Marshal.SizeOf(ButtonForm.data.GetType()));
		Array.Copy(array2, 0, array, Settings.ADDR_BUTTON, array2.Length);
		array2 = Settings.objectToByteArray(ButtonForm.data1, Marshal.SizeOf(ButtonForm.data1.GetType()));
		Array.Copy(array2, 0, array, Settings.ADDR_ONE_TOUCH, array2.Length);
		array2 = Settings.objectToByteArray(TextMsgForm.data, Marshal.SizeOf(TextMsgForm.data.GetType()));
		Array.Copy(array2, 0, array, Settings.ADDR_TEXT_MSG, array2.Length);
		array2 = Settings.objectToByteArray(EncryptForm.data, Marshal.SizeOf(EncryptForm.data.GetType()));
		Array.Copy(array2, 0, array, Settings.ADDR_ENCRYPT, array2.Length);
		array2 = Settings.objectToByteArray(SignalingBasicForm.data, Marshal.SizeOf(SignalingBasicForm.data.GetType()));
		Array.Copy(array2, 0, array, Settings.ADDR_SIGNALING_BASIC, array2.Length);
		array2 = Settings.objectToByteArray(DtmfForm.data, Marshal.SizeOf(DtmfForm.data.GetType()));
		Array.Copy(array2, 0, array, Settings.ADDR_DTMF_BASIC, array2.Length);
		array2 = Settings.objectToByteArray(APRSForm.data, Marshal.SizeOf(APRSForm.data.GetType()));
		Array.Copy(array2, 0, array, Settings.ADDR_APRS_SYSTEM, array2.Length);
		array2 = Settings.objectToByteArray(ContactForm.data, Marshal.SizeOf(ContactForm.data.GetType()));
		Array.Copy(array2, 0, array, Settings.ADDR_DMR_CONTACT_EX, array2.Length);
		array2 = Settings.objectToByteArray(DtmfContactForm.data, Marshal.SizeOf(DtmfContactForm.data.GetType()));
		Array.Copy(array2, 0, array, Settings.ADDR_DTMF_CONTACT, array2.Length);
		array2 = Settings.objectToByteArray(RxGroupListForm.data, Marshal.SizeOf(RxGroupListForm.data.GetType()));
		Array.Copy(array2, 0, array, Settings.ADDR_RX_GRP_LIST_EX, array2.Length);
		array2 = ChannelForm.data.ToEerom(0);
		Array.Copy(array2, 0, array, Settings.ADDR_CHANNEL, array2.Length);
		array2 = Settings.objectToByteArray(ScanBasicForm.data, Marshal.SizeOf(ScanBasicForm.data.GetType()));
		Array.Copy(array2, 0, array, Settings.ADDR_SCAN, array2.Length);
		array2 = Settings.objectToByteArray(NormalScanForm.data, Marshal.SizeOf(NormalScanForm.data.GetType()));
		Array.Copy(array2, 0, array, Settings.ADDR_SCAN_LIST, array2.Length);
		array2 = Settings.objectToByteArray(BootItemForm.data, Settings.SPACE_BOOT_ITEM);
		Array.Copy(array2, 0, array, Settings.ADDR_BOOT_ITEM, array2.Length);
		array2 = Settings.objectToByteArray(DigitalKeyContactForm.data, Settings.SPACE_DIGITAL_KEY_CONTACT);
		Array.Copy(array2, 0, array, Settings.ADDR_DIGITAL_KEY_CONTACT, Settings.SPACE_DIGITAL_KEY_CONTACT);
		array2 = Settings.objectToByteArray(MenuForm.data, Settings.SPACE_MENU_CONFIG);
		Array.Copy(array2, 0, array, Settings.ADDR_MENU_CONFIG, Settings.SPACE_MENU_CONFIG);
		array2 = Settings.objectToByteArray(BootItemForm.dataContent, Settings.SPACE_BOOT_CONTENT);
		Array.Copy(array2, 0, array, Settings.ADDR_BOOT_CONTENT, Settings.SPACE_BOOT_CONTENT);
		array2 = Settings.objectToByteArray(AttachmentForm.data, Settings.SPACE_ATTACHMENT);
		Array.Copy(array2, 0, array, Settings.ADDR_ATTACHMENT, Settings.SPACE_ATTACHMENT);
		array2 = VfoForm.data.ToEerom();
		Array.Copy(array2, 0, array, Settings.ADDR_VFO, array2.Length);
		if (ChannelForm.CurCntCh > 128)
		{
			array2 = Settings.objectToByteArray(ZoneForm.data, Settings.SPACE_EX_ZONE);
			Array.Copy(array2, 0, array, Settings.ADDR_EX_ZONE_LIST, array2.Length);
			array2 = Settings.objectToByteArray(APRSForm.dataEx, Marshal.SizeOf(APRSForm.dataEx));
			Array.Copy(array2, 0, array, Settings.ADDR_EX_EMERGENCY, array2.Length);
			for (int i = 1; i < 8; i++)
			{
				array2 = ChannelForm.data.ToEerom(i);
				Array.Copy(array2, 0, array, Settings.ADDR_EX_CH + (i - 1) * ChannelForm.SPACE_CH_GROUP, ChannelForm.SPACE_CH_GROUP);
			}
		}
		Array.Copy(OpenGD77Form.CustomData, 0, array, Settings.ADDR_OPENGD77_CUSTOM_DATA_START, OpenGD77Form.CustomData.Length);
		Array.Copy(OpenGD77Form.LastUsedChannelsData, 0, array, Settings.ADDR_OPENGD77_LAST_USED_CHANNELS_DATA_START, OpenGD77Form.LastUsedChannelsData.Length);
		return array;
	}

	public static bool checkCodeplugVersion311(byte[] cplg)
	{
		for (byte b = 0; b < 76; b++)
		{
			if (cplg[Settings.ADDR_RX_GRP_LIST_EX + b] > 16)
			{
				return true;
			}
		}
		if (cplg[Settings.ADDR_RX_GRP_LIST_EX + 1] > 0)
		{
			byte b = cplg[Settings.ADDR_RX_GRP_LIST_EX + 128 + 48 + 1];
			if (b > 3)
			{
				return false;
			}
		}
		return true;
	}

	public static bool checkZonesFor80Channels(byte[] codeplug)
	{
		if (codeplug[Settings.ADDR_EX_ZONE_LIST + 81] <= 4)
		{
			return true;
		}
		return false;
	}

	public static void convertTo80ChannelZoneCodeplug(byte[] cplg)
	{
		MessageBox.Show("Your codeplug uses 16 channel zones.\n\nIt will be automatically updated to 80 channel zones.\n\nPlease check the Zones to ensure the update worked correctly before saving or uploading the codeplug", "Codeplug update warning");
		byte[,] array = new byte[68, 48];
		int num = Settings.ADDR_EX_ZONE_LIST + 32;
		for (int i = 0; i < 68; i++)
		{
			for (int j = 0; j < 48; j++)
			{
				array[i, j] = cplg[num + i * 48 + j];
			}
		}
		for (int k = 0; k < 68; k++)
		{
			for (int l = 0; l < 48; l++)
			{
				cplg[num + k * 176 + l] = array[k, l];
			}
			for (int m = 48; m < 176; m++)
			{
				cplg[num + k * 176 + m] = 0;
			}
		}
	}

	public static void convertCodeplug(byte[] cplg)
	{
		MessageBox.Show(Settings.dicCommon["CodeplugUpgradeNotice"]);
		byte[,] array = new byte[128, 48];
		int j;
		for (int i = 0; i < 128; i++)
		{
			for (j = 0; j < 48; j++)
			{
				array[i, j] = cplg[(long)Settings.ADDR_RX_GRP_LIST_EX + 128L + i * 48 + j];
			}
		}
		for (int i = 0; i < 76; i++)
		{
			for (j = 0; j < 48; j++)
			{
				cplg[(long)Settings.ADDR_RX_GRP_LIST_EX + 128L + i * 80 + j] = array[i, j];
			}
			for (j = 48; j < 80; j++)
			{
				cplg[(long)Settings.ADDR_RX_GRP_LIST_EX + 128L + i * 80 + j] = 0;
			}
		}
		j = 0;
		for (int i = 76; i < 128; i++)
		{
			if (cplg[Settings.ADDR_RX_GRP_LIST_EX + i] > 0)
			{
				j++;
			}
			cplg[Settings.ADDR_RX_GRP_LIST_EX + i] = 0;
		}
		if (j > 0)
		{
			MessageBox.Show(Settings.dicCommon["CodeplugUpgradeWarningToManyRxGroups"]);
		}
	}

	public static void ByteToData(byte[] eerom, bool isFromFile = false)
	{
		byte[] array = new byte[Settings.SPACE_DEVICE_INFO];
		Array.Copy(eerom, Settings.ADDR_DEVICE_INFO, array, 0, array.Length);
		DeviceInfoForm.data = (DeviceInfoForm.DeviceInfo)Settings.byteArrayToObject(array, DeviceInfoForm.data.GetType());
		Settings.MIN_FREQ[0] = 220u;
		Settings.MAX_FREQ[0] = 564u;
		Settings.MIN_FREQ[1] = 127u;
		Settings.MAX_FREQ[1] = 174u;
		array = new byte[Settings.SPACE_GENERAL_SET];
		Array.Copy(eerom, Settings.ADDR_GENERAL_SET, array, 0, array.Length);
		GeneralSetForm.data = (GeneralSetForm.GeneralSet)Settings.byteArrayToObject(array, GeneralSetForm.data.GetType());
		GeneralSetForm.data.Callsign = GeneralSetForm.data.Callsign.ToUpper();
		Regex regex = new Regex("[^a-zA-Z0-9]");
		GeneralSetForm.data.Callsign = regex.Replace(GeneralSetForm.data.Callsign, "");
		array = new byte[Settings.SPACE_BUTTON];
		Array.Copy(eerom, Settings.ADDR_BUTTON, array, 0, array.Length);
		ButtonForm.data = (ButtonForm.SideKey)Settings.byteArrayToObject(array, ButtonForm.data.GetType());
		array = new byte[Settings.SPACE_ONE_TOUCH];
		Array.Copy(eerom, Settings.ADDR_ONE_TOUCH, array, 0, array.Length);
		ButtonForm.data1 = (ButtonForm.OneTouch)Settings.byteArrayToObject(array, ButtonForm.data1.GetType());
		array = new byte[Settings.SPACE_TEXT_MSG];
		Array.Copy(eerom, Settings.ADDR_TEXT_MSG, array, 0, array.Length);
		TextMsgForm.data = (TextMsgForm.TextMsg)Settings.byteArrayToObject(array, TextMsgForm.data.GetType());
		array = new byte[Settings.SPACE_ENCRYPT];
		Array.Copy(eerom, Settings.ADDR_ENCRYPT, array, 0, array.Length);
		EncryptForm.data = (EncryptForm.Encrypt)Settings.byteArrayToObject(array, EncryptForm.data.GetType());
		array = new byte[Settings.SPACE_SIGNALING_BASIC];
		Array.Copy(eerom, Settings.ADDR_SIGNALING_BASIC, array, 0, array.Length);
		SignalingBasicForm.data = (SignalingBasicForm.SignalingBasic)Settings.byteArrayToObject(array, SignalingBasicForm.data.GetType());
		array = new byte[Settings.SPACE_DTMF_BASIC];
		Array.Copy(eerom, Settings.ADDR_DTMF_BASIC, array, 0, array.Length);
		DtmfForm.data = (DtmfForm.Dtmf)Settings.byteArrayToObject(array, DtmfForm.data.GetType());
		array = new byte[Settings.SPACE_APRS_SYSTEM];
		Array.Copy(eerom, Settings.ADDR_APRS_SYSTEM, array, 0, array.Length);
		APRSForm.data = (APRSForm.APRS_Config)Settings.byteArrayToObject(array, APRSForm.data.GetType());
		array = new byte[Settings.SPACE_DMR_CONTACT_EX];
		Array.Copy(eerom, Settings.ADDR_DMR_CONTACT_EX, array, 0, array.Length);
		ContactForm.data = (ContactForm.Contact)Settings.byteArrayToObject(array, ContactForm.data.GetType());
		array = new byte[Settings.SPACE_DTMF_CONTACT];
		Array.Copy(eerom, Settings.ADDR_DTMF_CONTACT, array, 0, array.Length);
		DtmfContactForm.data = (DtmfContactForm.DtmfContact)Settings.byteArrayToObject(array, DtmfContactForm.data.GetType());
		array = new byte[Settings.SPACE_RX_GRP_LIST];
		Array.Copy(eerom, Settings.ADDR_RX_GRP_LIST_EX, array, 0, array.Length);
		RxGroupListForm.data = (RxListData)Settings.byteArrayToObject(array, RxGroupListForm.data.GetType());
		ZoneForm.data.ZoneIndex[0] = eerom[Settings.ADDR_ZONE_BASIC];
		APRSForm.ValidateMagicAndCompact();
		array = new byte[ChannelForm.SPACE_CH_GROUP];
		Array.Copy(eerom, Settings.ADDR_CHANNEL, array, 0, array.Length);
		ChannelForm.data.FromEerom(0, array);
		array = new byte[Settings.SPACE_SCAN_BASIC];
		Array.Copy(eerom, Settings.ADDR_SCAN, array, 0, array.Length);
		ScanBasicForm.data = (ScanBasicForm.ScanBasic)Settings.byteArrayToObject(array, ScanBasicForm.data.GetType());
		array = new byte[Settings.SPACE_SCAN_LIST];
		Array.Copy(eerom, Settings.ADDR_SCAN_LIST, array, 0, array.Length);
		NormalScanForm.data = (NormalScanForm.NormalScan)Settings.byteArrayToObject(array, NormalScanForm.data.GetType());
		array = new byte[Settings.SPACE_BOOT_ITEM];
		Array.Copy(eerom, Settings.ADDR_BOOT_ITEM, array, 0, array.Length);
		BootItemForm.data = (BootItemForm.BootItem)Settings.byteArrayToObject(array, BootItemForm.data.GetType());
		array = new byte[Settings.SPACE_DIGITAL_KEY_CONTACT];
		Array.Copy(eerom, Settings.ADDR_DIGITAL_KEY_CONTACT, array, 0, Settings.SPACE_DIGITAL_KEY_CONTACT);
		DigitalKeyContactForm.data = (DigitalKeyContactForm.NumKeyContact)Settings.byteArrayToObject(array, DigitalKeyContactForm.data.GetType());
		array = new byte[Settings.SPACE_MENU_CONFIG];
		Array.Copy(eerom, Settings.ADDR_MENU_CONFIG, array, 0, Settings.SPACE_MENU_CONFIG);
		MenuForm.data = (MenuForm.MenuSet)Settings.byteArrayToObject(array, MenuForm.data.GetType());
		array = new byte[Settings.SPACE_BOOT_CONTENT];
		Array.Copy(eerom, Settings.ADDR_BOOT_CONTENT, array, 0, array.Length);
		BootItemForm.dataContent = (BootItemForm.BootContent)Settings.byteArrayToObject(array, typeof(BootItemForm.BootContent));
		array = new byte[Settings.SPACE_ATTACHMENT];
		Array.Copy(eerom, Settings.ADDR_ATTACHMENT, array, 0, array.Length);
		AttachmentForm.data = (AttachmentForm.Attachment)Settings.byteArrayToObject(array, typeof(AttachmentForm.Attachment));
		array = new byte[Settings.SPACE_VFO];
		Array.Copy(eerom, Settings.ADDR_VFO, array, 0, array.Length);
		VfoForm.data.FromEerom(array);
		try
		{
			if (ChannelForm.CurCntCh > 128)
			{
				array = new byte[ZoneForm.SPACE_BASIC_ZONE];
				Array.Copy(eerom, Settings.ADDR_EX_ZONE_BASIC, array, 0, array.Length);
				array = new byte[Settings.SPACE_EX_ZONE];
				Array.Copy(eerom, Settings.ADDR_EX_ZONE_LIST, array, 0, Settings.SPACE_EX_ZONE);
				ZoneForm.data = (ZoneForm.Zone)Settings.byteArrayToObject(array, ZoneForm.data.GetType());
				ZoneForm.CompactZones();
				array = new byte[Settings.SPACE_EX_EMERGENCY];
				Array.Copy(eerom, Settings.ADDR_EX_EMERGENCY, array, 0, array.Length);
				APRSForm.dataEx = (APRSForm.EmergencyEx)Settings.byteArrayToObject(array, APRSForm.dataEx.GetType());
				for (int i = 1; i < 8; i++)
				{
					array = new byte[ChannelForm.SPACE_CH_GROUP];
					Array.Copy(eerom, Settings.ADDR_EX_CH + (i - 1) * array.Length, array, 0, array.Length);
					ChannelForm.data.FromEerom(i, array);
				}
			}
			GeneralSetForm.data.CodeplugVersion = 1;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
		OpenGD77Form.LoadCustomData(eerom);
		OpenGD77Form.LoadLastUsedChannelsData(eerom);
	}

	public static void DataVerify()
	{
		BootItemForm.data.Verify(BootItemForm.DefaultBootItem);
		ButtonForm.data.Verify(ButtonForm.DefaultSideKey);
		ChannelForm.data.Verify();
		ContactForm.data.Verify();
		DigitalKeyContactForm.data.Verify();
		DtmfForm.data.Verify(DtmfForm.DefaultDtmf);
		APRSForm.data.Verify();
		EncryptForm.data.Verify(EncryptForm.DefaultEncrypt);
		GeneralSetForm.data.Verify(GeneralSetForm.DefaultGeneralSet);
		AttachmentForm.data.Verify(AttachmentForm.DefaultAttachment);
		VfoForm.data.Verify();
		MenuForm.data.Verify(MenuForm.DefaultMenu);
		NormalScanForm.data.Verify();
		RxGroupListForm.data.Verify();
		ScanBasicForm.data.Verify(ScanBasicForm.DefaultScanBasic);
		ZoneForm.data.Verify();
	}

	public void ShowHelp(string helpId)
	{
		string helpFilename = Settings.GetHelpFilename();
		if (dicHelp.ContainsKey(helpId) && !string.IsNullOrEmpty(dicHelp[helpId].Trim()))
		{
			string text = "mk:@MSITStore:" + helpFilename + dicHelp[helpId];
			frmHelp.ShowHelp(text.Replace("#", "%23"));
		}
		else
		{
			frmHelp.ShowHelp("");
		}
	}



	private void method_16()
	{
		XmlTextWriter xmlTextWriter = new XmlTextWriter(Application.StartupPath + "/help.xml", Encoding.UTF8);
		xmlTextWriter.Formatting = Formatting.Indented;
		xmlTextWriter.WriteStartDocument();
		xmlTextWriter.WriteStartElement("Controls");
		Form[] mdiChildren = base.MdiChildren;
		foreach (Form form in mdiChildren)
		{
			method_17(form.Controls, xmlTextWriter);
		}
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndDocument();
		xmlTextWriter.Flush();
		xmlTextWriter.Close();
	}

	private void method_17(Control.ControlCollection controlCollection_0, XmlTextWriter xmlTextWriter_0)
	{
		foreach (Control item in controlCollection_0)
		{
			if (item is TextBox || item is CheckBox || item is ListBox || item is NumericUpDown || item is ComboBox)
			{
				if (item.Parent is NumericUpDown)
				{
					continue;
				}
				xmlTextWriter_0.WriteStartElement("Control");
				xmlTextWriter_0.WriteAttributeString("id", item.FindForm().Name + "_" + item.Name);
				xmlTextWriter_0.WriteAttributeString("html", " ");
				xmlTextWriter_0.WriteEndElement();
			}
			if (item.Controls.Count > 0)
			{
				method_17(item.Controls, xmlTextWriter_0);
			}
		}
	}

	public void DispChildForm(Type type, int index)
	{
		TreeNode treeNodeByTypeAndIndex = GetTreeNodeByTypeAndIndex(type, index, tvwMain.Nodes);
		if (treeNodeByTypeAndIndex != null)
		{
            treeviewDoubleClickHandler(treeNodeByTypeAndIndex, bool_0: true);
		}
	}

	private void MainForm_KeyDown(object sender, KeyEventArgs e)
	{
	}

	public void GetAllLang()
	{
		tsmiLanguage.DropDownItems.Clear();
		string startupPath = Application.StartupPath;
		string[] array = (from f in Directory.GetFiles(startupPath + Path.DirectorySeparatorChar + "Language" + Path.DirectorySeparatorChar + "CPS", "*.xml")
			orderby f
			select f).ToArray();
		if (array.Length == 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			try
			{
				string text = array[i];
				ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
				toolStripMenuItem.Text = GetLangName(text);
				toolStripMenuItem.Name = GetLangName(text);
				toolStripMenuItem.Tag = text;
				tsmiLanguage.DropDownItems.Add(toolStripMenuItem);
				toolStripMenuItem.Click += languageChangeHandler;
			}
			catch
			{
			}
		}
	}



	public string GetLangName(string xmlFile)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(xmlFile);
		string xpath = "/Resource/Language";
		XmlNode xmlNode = xmlDocument.SelectSingleNode(xpath);
		if (xmlNode != null)
		{
			_ = xmlNode.Attributes["Id"].Value;
			return xmlNode.Attributes["Text"].Value;
		}
		return "";
	}

	private void initialiseTree()
	{
		lstTreeNodeItem.Clear();
		lstTreeNodeItem.Add(new TreeNodeItem(null, null, null, 0, -1, 18, null));
        lstTreeNodeItem.Add(new TreeNodeItem(null, typeof(CodeplugSettingsForm), null, 0, -1, 1, null));
        lstTreeNodeItem.Add(new TreeNodeItem(null, typeof(GeneralSetForm), null, 0, -1, 5, null));
		lstTreeNodeItem.Add(new TreeNodeItem(null, typeof(BootItemForm), null, 0, -1, 30, null));
		lstTreeNodeItem.Add(new TreeNodeItem(null, typeof(DeviceInfoForm), null, 0, -1, 20, null));
		if (EnableHiddenFeatures)
		{
			lstTreeNodeItem.Add(new TreeNodeItem(null, typeof(TextMsgForm), null, 0, -1, 22, null));
		}
		lstTreeNodeItem.Add(new TreeNodeItem(null, typeof(DtmfForm), null, 0, -1, 39, null));
		lstTreeNodeItem.Add(new TreeNodeItem(cmsGroup, typeof(APRSConfigsForm), typeof(APRSForm), 8, -1, 17, APRSForm.data));
		lstTreeNodeItem.Add(new TreeNodeItem(null, null, null, 0, -1, 17, null));
		lstTreeNodeItem.Add(new TreeNodeItem(null, typeof(DtmfContactForm), null, 0, -1, 49, null));
		lstTreeNodeItem.Add(new TreeNodeItem(cmsGroupContact, typeof(ContactsForm), typeof(ContactForm), 1024, -1, 17, ContactForm.data));
		lstTreeNodeItem.Add(new TreeNodeItem(cmsGroup, null, typeof(RxGroupListForm), 76, -1, 17, RxGroupListForm.data));
		lstTreeNodeItem.Add(new TreeNodeItem(cmsGroup, typeof(ZoneBasicForm), typeof(ZoneForm), 68, -1, 16, ZoneForm.data));
		lstTreeNodeItem.Add(new TreeNodeItem(cmsGroup, typeof(ChannelsForm), typeof(ChannelForm), ChannelForm.CurCntCh, -1, 17, ChannelForm.data));
		lstTreeNodeItem.Add(new TreeNodeItem(null, null, null, 0, -1, 17, null));
		int num = 0;
		for (int i = 0; i < 2; i++)
		{
			int chMode = VfoForm.data.GetChMode(i);
			lstTreeNodeItem.Add(new TreeNodeItem(null, typeof(VfoForm), null, 2, i, chMode switch
			{
				0 => 2, 
				1 => 6, 
				_ => 54, 
			}, VfoForm.data));
		}
        
    }

	private DataTable method_19()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Id");
		dataTable.Columns.Add("Name");
		dataTable.Columns.Add("ParentId");
		dataTable.Rows.Add("00", "Model", "-1");
        dataTable.Rows.Add("0017", "Settings", "00");
        dataTable.Rows.Add("0001", "GeneralSetting", "00");
		dataTable.Rows.Add("0002", "BootItem", "00");
		dataTable.Rows.Add("0005", "BasicInfo", "00");
		if (EnableHiddenFeatures)
		{
			dataTable.Rows.Add("0007", "TextMsg", "00");
		}
		dataTable.Rows.Add("0008", "DtmfSignal", "00");
		dataTable.Rows.Add("0010", "APRS", "00");
		dataTable.Rows.Add("0011", "Contact", "00");
		dataTable.Rows.Add("001100", "DtmfContact", "0011");
		dataTable.Rows.Add("001101", "DigitalContact", "0011");
		dataTable.Rows.Add("0012", "RxGroupList", "00");
		dataTable.Rows.Add("0013", "Zone", "00");
		dataTable.Rows.Add("0014", "Channel", "00");
		dataTable.Rows.Add("0016", "VFO", "00");
		dataTable.Rows.Add("001600", "VFOA", "0016");
		dataTable.Rows.Add("001601", "VFOB", "0016");
        
        return dataTable;
	}

	private void method_20(DataTable dataTable_0)
	{
		DataRow[] array = (from dataRow_0 in dataTable_0.Select("ParentId='-1'")
			orderby dataRow_0[0]
			select dataRow_0).ToArray();
		if (array != null && array.Length != 0)
		{
			TreeNode treeNode_ = AddTreeViewNode(tvwMain.Nodes, array[0]["Name"].ToString(), lstTreeNodeItem[dataTable_0.Rows.IndexOf(array[0])]);
			method_21(array[0], treeNode_, dataTable_0);
		}
	}

	private void method_21(DataRow dataRow_0, TreeNode treeNode_0, DataTable dataTable_0)
	{
		if (dataRow_0 == null || treeNode_0 == null)
		{
			return;
		}
		try
		{
			DataRow[] array = (from dataRow_00 in dataTable_0.Select("ParentId='" + dataRow_0["Id"]?.ToString() + "'")
				orderby dataRow_0[0]
				select dataRow_00).ToArray();
			if (array != null || array.Length != 0)
			{
				DataRow[] array2 = array;
				foreach (DataRow dataRow in array2)
				{
					int index = dataTable_0.Rows.IndexOf(dataRow);
					TreeNode treeNode_1 = AddTreeViewNode(treeNode_0.Nodes, dataRow["Name"].ToString(), lstTreeNodeItem[index]);
					method_21(dataRow, treeNode_1, dataTable_0);
				}
			}
		}
		catch (Exception)
		{
			throw;
		}
	}

	public void InitDynamicNode()
	{
		TreeNode parentNode = method_8(typeof(APRSForm), tvwMain.Nodes);
		InitAPRSConfigurations(parentNode);
		parentNode = method_9(typeof(ContactsForm), tvwMain.Nodes);
		InitDigitContacts(parentNode);
		parentNode = method_8(typeof(RxGroupListForm), tvwMain.Nodes);
		InitRxGroupLists(parentNode);
		parentNode = method_9(typeof(ZoneBasicForm), tvwMain.Nodes);
		InitZones(parentNode);
		parentNode = method_9(typeof(ChannelsForm), tvwMain.Nodes);
		InitChannels(parentNode);
		parentNode = method_9(typeof(ScanBasicForm), tvwMain.Nodes);
	}

	public void InitChannelsImportNodes()
	{
		TreeNode parentNode = method_9(typeof(ContactsForm), tvwMain.Nodes);
		InitDigitContacts(parentNode);
		parentNode = method_8(typeof(RxGroupListForm), tvwMain.Nodes);
		InitRxGroupLists(parentNode);
		parentNode = method_9(typeof(ChannelsForm), tvwMain.Nodes);
		InitChannels(parentNode);
		parentNode = method_9(typeof(ScanBasicForm), tvwMain.Nodes);
		InitScans(parentNode);
		parentNode = method_9(typeof(ZoneBasicForm), tvwMain.Nodes);
		InitZones(parentNode);
	}

	public void WriteXml(List<ToolStripItem> lstMenuItem)
	{
		XmlTextWriter xmlTextWriter = new XmlTextWriter(Application.StartupPath + "/test.xml", Encoding.UTF8);
		xmlTextWriter.Formatting = Formatting.Indented;
		xmlTextWriter.WriteStartDocument();
		xmlTextWriter.WriteStartElement("MenuStrip");
		foreach (ToolStripItem item in lstMenuItem)
		{
			xmlTextWriter.WriteStartElement("MenuItem");
			xmlTextWriter.WriteAttributeString("Id", item.Name);
			xmlTextWriter.WriteAttributeString("Text", item.Text);
			xmlTextWriter.WriteEndElement();
		}
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndDocument();
		xmlTextWriter.Flush();
		xmlTextWriter.Close();
	}

	public void WriteMenuXml(List<ToolStripMenuItem> lstMenuItem, XmlTextWriter xml)
	{
		xml.WriteStartElement("ContextMenuStrip");
		lstMenuItem.ForEach(delegate(ToolStripMenuItem x)
		{
			xml.WriteStartElement("MenuItem");
			xml.WriteAttributeString("Id", x.Name);
			xml.WriteAttributeString("Text", x.Text);
			xml.WriteEndElement();
		});
		xml.WriteEndElement();
	}

	public void WriteToolStripXml(List<ToolStripItem> lstControls, XmlTextWriter xml)
	{
		lstControls.ForEach(delegate(ToolStripItem x)
		{
			xml.WriteStartElement("ToolStripItem");
			xml.WriteAttributeString("Id", x.Name);
			xml.WriteAttributeString("Text", x.Text);
			xml.WriteEndElement();
		});
	}

	private void method_22()
	{
		XmlTextWriter xmlTextWriter = new XmlTextWriter(Application.StartupPath + "/test.xml", Encoding.UTF8);
		xmlTextWriter.Formatting = Formatting.Indented;
		xmlTextWriter.WriteStartDocument();
		xmlTextWriter.WriteStartElement("Resource");
		Form[] mdiChildren = base.MdiChildren;
		foreach (Form form in mdiChildren)
		{
			xmlTextWriter.WriteStartElement(form.Name);
			WriteControlXml(form.smethod_12(), xmlTextWriter);
			xmlTextWriter.WriteEndElement();
		}
		foreach (Form openForm in Application.OpenForms)
		{
			xmlTextWriter.WriteStartElement(openForm.Name);
			WriteControlXml(openForm.smethod_12(), xmlTextWriter);
			xmlTextWriter.WriteEndElement();
		}
		WriteMenuXml(cmsGroup.smethod_9(), xmlTextWriter);
		WriteMenuXml(cmsSub.smethod_9(), xmlTextWriter);
		WriteMenuXml(cmsTree.smethod_9(), xmlTextWriter);
		WriteMenuXml(cmsGroupContact.smethod_9(), xmlTextWriter);
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndDocument();
		xmlTextWriter.Flush();
		xmlTextWriter.Close();
	}

	public void WriteControlXml(List<Control> lstControls, XmlTextWriter xml)
	{
		xml.WriteStartElement("Controls");
		foreach (Control lstControl in lstControls)
		{
			xml.WriteStartElement("Control");
			xml.WriteAttributeString("id", lstControl.Name);
			if (lstControl is DataGridView)
			{
				List<string> list = new List<string>();
				DataGridView dataGridView = lstControl as DataGridView;
				foreach (DataGridViewColumn column in dataGridView.Columns)
				{
					list.Add(column.HeaderText);
				}
				xml.WriteAttributeString("Text", string.Join(",", list.ToArray()));
				foreach (DataGridViewColumn column2 in dataGridView.Columns)
				{
					xml.WriteStartElement("ControlItem");
					xml.WriteAttributeString("Id", column2.Name);
					xml.WriteAttributeString("Text", column2.HeaderText);
					xml.WriteEndElement();
				}
			}
			else if (lstControl is ToolStrip)
			{
				xml.WriteAttributeString("Text", lstControl.Text);
				ToolStrip toolStrip_ = lstControl as ToolStrip;
				WriteToolStripXml(toolStrip_.smethod_10(), xml);
			}
			else
			{
				xml.WriteAttributeString("Text", lstControl.Text);
			}
			xml.WriteEndElement();
		}
		xml.WriteEndElement();
	}

	[CompilerGenerated]
	private static void smethod_0(TreeNode treeNode_0)
	{
		if (dicTree.ContainsKey(treeNode_0.Name))
		{
			if (treeNode_0.Name == "Model")
			{
				treeNode_0.Text = GeneralSetForm.data.Callsign;
			}
			else
			{
				treeNode_0.Text = dicTree[treeNode_0.Name];
			}
		}
	}

	[CompilerGenerated]
	private static bool smethod_1(byte byte_0)
	{
		return byte_0 == byte.MaxValue;
	}

	[CompilerGenerated]
	private static void smethod_2(TreeNode treeNode_0)
	{
		if (dicTree.ContainsKey(treeNode_0.Name))
		{
			if (treeNode_0.Name == "Model")
			{
				treeNode_0.Text = GeneralSetForm.data.Callsign;
			}
			else
			{
				treeNode_0.Text = dicTree[treeNode_0.Name];
			}
		}
	}

	[CompilerGenerated]
	private static object smethod_4(DataRow dataRow_0)
	{
		return dataRow_0[0];
	}

	[CompilerGenerated]
	private static object smethod_5(DataRow dataRow_0)
	{
		return dataRow_0[0];
	}

	static MainForm()
	{
		CommsBuffer = null;
		PRODUCT_NAME = ((AssemblyProductAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyProductAttribute), inherit: false)).Product;
		PRODUCT_VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        RadioType = RadioTypeEnum.RadioTypeSTM32;
		PreActiveMdiChild = null;
		dicHelp = new Dictionary<string, string>();
		dicTree = new Dictionary<string, string>();
	}

    private void tsmiImportG77_Click(object sender, EventArgs e)
    {
        try
        {
            string profileStringWithDefault = IniFileUtils.getProfileStringWithDefault("Setup", "LastFilePath", "");
            try
            {
                if (profileStringWithDefault == null || "" == profileStringWithDefault)
                {
                    importFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                }
                else
                {
                    importFileDialog.InitialDirectory = Path.GetDirectoryName(profileStringWithDefault);
                }
            }
            catch (Exception)
            {
                importFileDialog.InitialDirectory = "";
            }
            if (importFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(importFileDialog.FileName))
            {
                importCodeplugFile(importFileDialog.FileName);
            }
        }
        catch (Exception ex2)
        {
            MessageBox.Show(ex2.Message);
        }
    }

    private void dummyClick(object sender, EventArgs e)
    {
		MessageBox.Show("На данный момент функция неактивна!");
    }

    private void pingTimer_Tick(object sender, EventArgs e)
    {
		FormCollection forms = Application.OpenForms;
		if (forms.Count > 2) return;
		try
		{
			pingRadio();
		}
		catch 
		{ 
			radioInformation.Text = "";
		}
    }

    private void tsmiSetup_Click(object sender, EventArgs e)
    {
		AppSettings setupForm = new AppSettings();
        setupForm.ShowDialog();
    }


}
