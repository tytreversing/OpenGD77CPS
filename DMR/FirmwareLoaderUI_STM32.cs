using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DMR;

public class FirmwareLoaderUI_STM32 : Form
{
	public enum OutputType
	{
		OutputType_MD9600,
		OutputType_MDUV380,
		OutputType_MD2017,
		OutputType_MD380,
		OutputType_DM1701
	}

	private static readonly byte[] FW_D2645_SHA256_Checksum = new byte[32]
	{
		216, 166, 83, 48, 114, 34, 229, 118, 238, 65,
		106, 182, 212, 112, 77, 20, 117, 143, 167, 28,
		27, 14, 130, 111, 253, 132, 15, 38, 38, 108,
		241, 31
	};

	public static RegistryKey regKeyOfficialFirmware = null;

	public static string officialFirmwareFile = Environment.CurrentDirectory + "\\SourceFirmware\\source.bin";

	public string officialFirmwareFilePath = "";

	public static string languageFile = "";

	public string windowTitle;

	private bool donorFirmwareLoaded;

	private STM_DFU_FwUpdate fwUpdate;

	public static Dictionary<string, string> StringsDict = new Dictionary<string, string>();

	private IContainer components;

	private Button btnProgram;

	private ProgressBar Progress;

	private Label lblMessage;

	private OpenFileDialog dlgOpenFile;

	private Button btnSelectDonorFW;

	private RadioButton rbMD9600;

	private RadioButton rbMDUV380;

	private RadioButton rbMD2017;

	private RadioButton rbDM1701;

	private RadioButton rbMD380;
    private LinkLabel findFirmwareLink;
    private LinkLabel flashInstruction;
    private Label warning;
    private Label doNotUse;
    private Button downloadRussian;
    private GroupBox grpRadioType;

	public FirmwareLoaderUI_STM32()
	{
		InitializeComponent();
		if (!Control.ModifierKeys.HasFlag(Keys.Control) || !Control.ModifierKeys.HasFlag(Keys.Shift))
		{
			rbMD380.Visible = false;
			rbDM1701.AccessibleDescription = "DM-1701 / RT-84";
			rbDM1701.AccessibleName = "DM-1701 / RT-84";
			rbDM1701.AutoSize = true;
			rbDM1701.Text = "DM-1701 / RT-84";
		}
		fwUpdate = new STM_DFU_FwUpdate();
		fwUpdate.DisplayMessage += DisplayMessage;
		fwUpdate.UploadCompleted += UploadCompleted;
		officialFirmwareFilePath = Environment.CurrentDirectory + "\\SourceFirmware\\source.bin"; //путь к исходнику фиксированный
		string profileStringWithDefault = IniFileUtils.getProfileStringWithDefault("Setup", "LastSTM32FirmwareRadio", null);
		if (profileStringWithDefault != "")
		{
			foreach (RadioButton control in grpRadioType.Controls)
			{
				if (control.Name == profileStringWithDefault)
				{
					control.Checked = true;
				}
			}
		}
		Settings.ReadCommonsForSectionIntoDictionary(StringsDict, base.Name);
		object[] array = new object[MainForm.FirmwareLanguageFiles.Length + 1];
		array[0] = StringsDict["AdditionalLanguageNone"];
		findFirmwareLink.Text = StringsDict["FindFirmware"];
		flashInstruction.Text = StringsDict["FlashFirmware"];
		warning.Text = StringsDict["Description"];
		doNotUse.Text = StringsDict["DoNotUse"];
        downloadRussian.Text = StringsDict["DownloadRussian"];

    }

	private void btnProgram_Click(object sender, EventArgs e)
	{
		try
		{
			lblMessage.Text = "";
			Progress.Value = 0;
			if (!donorFirmwareLoaded)
			{
				MessageBox.Show(Settings.dicCommon["OfficialFWNotSelected"], Settings.dicCommon["Error"], MessageBoxButtons.OK);
				return;
			}
			OutputType outputType = OutputType.OutputType_MD9600;
			foreach (RadioButton control in grpRadioType.Controls)
			{
				if (control.Checked)
				{
					outputType = (OutputType)control.Tag;
					IniFileUtils.WriteProfileString("Setup", "LastSTM32FirmwareRadio", control.Name);
					break;
				}
			}
			
		    languageFile = Environment.CurrentDirectory + "\\Language\\Firmware\\Russian";
			
			IniFileUtils.WriteProfileString("Setup", "LastFirmwareLanguage", languageFile);
			dlgOpenFile.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastFirmwareLocation" + outputType, null);
			dlgOpenFile.Filter = Settings.dicCommon["FirmwareFilefilter"];
			dlgOpenFile.Title = Settings.dicCommon["FirmwareSelectorTitle"];
			if (DialogResult.OK != dlgOpenFile.ShowDialog())
			{
				return;
			}
			IniFileUtils.WriteProfileString("Setup", "LastFirmwareLocation" + outputType, Path.GetDirectoryName(dlgOpenFile.FileName));
			lblMessage.Text = "";
			Progress.Value = 0;
			Button button = btnProgram;
			Button button2 = btnSelectDonorFW;
			bool flag2 = (grpRadioType.Enabled = false);
			bool flag4 = (button2.Enabled = flag2);
			bool enabled = (button.Enabled = flag4);
			byte[] openFirmwareBuf = null;
			byte[] userLanguageBuf = null;
			if (Path.GetExtension(dlgOpenFile.FileName).ToLower() == ".zip")
			{
				ZipArchive val = ZipFile.OpenRead(dlgOpenFile.FileName);
				try
				{
					using (Stream stream = val.Entries.Where((ZipArchiveEntry fn) => Path.GetExtension(fn.FullName) == ".bin").FirstOrDefault().Open())
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							stream.CopyTo(memoryStream);
							openFirmwareBuf = memoryStream.ToArray();
						}
						stream.Close();
					}
					if (languageFile != "")
					{
						string langauageFileFullName = languageFile + ".gla";
						using Stream stream2 = val.Entries.Where((ZipArchiveEntry fn) => fn.FullName == langauageFileFullName).FirstOrDefault().Open();
						using (MemoryStream memoryStream2 = new MemoryStream())
						{
							stream2.CopyTo(memoryStream2);
							userLanguageBuf = memoryStream2.ToArray();
						}
						stream2.Close();
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			else
			{
				if (languageFile != "")
				{
					string path = Path.Combine(Path.GetDirectoryName(dlgOpenFile.FileName), languageFile + ".gla");
					if (!File.Exists(path))
					{
						lblMessage.Text = "";
						MessageBox.Show(string.Format(StringsDict["AdditionalLanguageNotFound"], languageFile + ".gla"));
						Button button3 = btnProgram;
						Button button4 = btnSelectDonorFW;
						flag2 = (grpRadioType.Enabled = true);
						flag4 = (button4.Enabled = flag2);
						enabled = (button3.Enabled = flag4);
						return;
					}
					languageFile = path;
					userLanguageBuf = File.ReadAllBytes(languageFile);
				}
				openFirmwareBuf = File.ReadAllBytes(dlgOpenFile.FileName);
			}
			fwUpdate.UpdateRadioFirmware(this, openFirmwareBuf, officialFirmwareFile, userLanguageBuf, outputType);
		}
		catch (Exception ex)
		{
			lblMessage.Text = "";
			Progress.Value = 0;
			MessageBox.Show(ex.Message);
			Button button5 = btnProgram;
			Button button6 = btnSelectDonorFW;
			bool flag2 = (grpRadioType.Enabled = true);
			bool flag4 = (button6.Enabled = flag2);
			bool enabled = (button5.Enabled = flag4);
		}
	}

	private void UploadCompleted(object sender, FirmwareUpdateMessageEventArgs e)
	{
		if (Progress.InvokeRequired)
		{
			Invoke(new EventHandler<FirmwareUpdateMessageEventArgs>(UploadCompleted), sender, e);
			return;
		}
		Button button = btnProgram;
		Button button2 = btnSelectDonorFW;
		bool flag2 = (grpRadioType.Enabled = true);
		bool flag4 = (button2.Enabled = flag2);
		bool enabled = (button.Enabled = flag4);
	}

	private void DisplayMessage(object sender, FirmwareUpdateMessageEventArgs e)
	{
		if (Progress.InvokeRequired)
		{
			Invoke(new EventHandler<FirmwareUpdateMessageEventArgs>(DisplayMessage), sender, e);
			return;
		}
		lblMessage.Text = e.Message;
		if (e.IsError)
		{
			MessageBox.Show(e.Message, StringsDict["Error"]);
			Progress.Value = 0;
		}
		else
		{
			Progress.Value = (int)e.Percentage;
		}
	}

	private void btnSelectDonorFW_Click(object sender, EventArgs e)
	{
		bool flag = false;
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Title = StringsDict["SelectOfficialFirmware"] + " MD9600-CSV(2571V5)-V26.45.bin";
		openFileDialog.Filter = StringsDict["FirmwareFiles"] + " (MD9600-CSV(2571V5)-V26.45.bin)|MD9600-CSV(2571V5)-V26.45.bin";
		if (File.Exists(officialFirmwareFilePath))
		{
			openFileDialog.InitialDirectory = Path.GetDirectoryName(officialFirmwareFilePath);
		}
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			flag = ValidateOfficialFirmware(openFileDialog.FileName);
			if (flag || (!flag && officialFirmwareFile != ""))
			{
				Text = windowTitle + " [ +DMR ]";
				MessageBox.Show(StringsDict["OfficialFWVerified"], StringsDict["Success"]);
			}
			else
			{
				MessageBox.Show(StringsDict["OfficialFWNotCorrect"] + " MD9600-CSV(2571V5)-V26.45.bin", StringsDict["Error"]);
				Text = windowTitle;
			}
		}
	}

	private byte[] GetSHA256Checksum(string filename)
	{
		using SHA256 sHA = SHA256.Create();
		using FileStream inputStream = File.OpenRead(filename);
		return sHA.ComputeHash(inputStream);
	}

	private bool ValidateOfficialFirmware(string filename)
	{
		lblMessage.Text = "";
		if (filename.Length > 0 && File.Exists(filename) && GetSHA256Checksum(filename).SequenceEqual(FW_D2645_SHA256_Checksum))
		{
			officialFirmwareFile = filename;
			donorFirmwareLoaded = true;
			btnProgram.Enabled = true;
			return true;
		}
		return false;
	}

	private void FirmwareLoaderUI_MD9600_Load(object sender, EventArgs e)
	{
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		windowTitle = Text;
		if (ValidateOfficialFirmware(officialFirmwareFilePath))
		{
			Text = windowTitle + " [ +DMR ]";
		}
		else
		{
			Text = windowTitle + " [ " + StringsDict["FmOnly"] + " ]";
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
            this.btnProgram = new System.Windows.Forms.Button();
            this.Progress = new System.Windows.Forms.ProgressBar();
            this.lblMessage = new System.Windows.Forms.Label();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.btnSelectDonorFW = new System.Windows.Forms.Button();
            this.rbMD9600 = new System.Windows.Forms.RadioButton();
            this.rbMDUV380 = new System.Windows.Forms.RadioButton();
            this.rbMD2017 = new System.Windows.Forms.RadioButton();
            this.rbDM1701 = new System.Windows.Forms.RadioButton();
            this.rbMD380 = new System.Windows.Forms.RadioButton();
            this.grpRadioType = new System.Windows.Forms.GroupBox();
            this.findFirmwareLink = new System.Windows.Forms.LinkLabel();
            this.flashInstruction = new System.Windows.Forms.LinkLabel();
            this.warning = new System.Windows.Forms.Label();
            this.doNotUse = new System.Windows.Forms.Label();
            this.downloadRussian = new System.Windows.Forms.Button();
            this.grpRadioType.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnProgram
            // 
            this.btnProgram.BackColor = System.Drawing.Color.White;
            this.btnProgram.Font = new System.Drawing.Font("Arial", 9F);
            this.btnProgram.Location = new System.Drawing.Point(140, 259);
            this.btnProgram.Name = "btnProgram";
            this.btnProgram.Size = new System.Drawing.Size(263, 26);
            this.btnProgram.TabIndex = 7;
            this.btnProgram.Text = "Select OpenMD9600 file && update";
            this.btnProgram.UseVisualStyleBackColor = false;
            this.btnProgram.Click += new System.EventHandler(this.btnProgram_Click);
            // 
            // Progress
            // 
            this.Progress.Location = new System.Drawing.Point(12, 306);
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(523, 21);
            this.Progress.TabIndex = 9;
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("Arial", 9F);
            this.lblMessage.Location = new System.Drawing.Point(13, 330);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(522, 24);
            this.lblMessage.TabIndex = 10;
            // 
            // btnSelectDonorFW
            // 
            this.btnSelectDonorFW.Font = new System.Drawing.Font("Arial", 9F);
            this.btnSelectDonorFW.Location = new System.Drawing.Point(256, 325);
            this.btnSelectDonorFW.Name = "btnSelectDonorFW";
            this.btnSelectDonorFW.Size = new System.Drawing.Size(240, 26);
            this.btnSelectDonorFW.TabIndex = 8;
            this.btnSelectDonorFW.Text = "Select official firmware (donor) file";
            this.btnSelectDonorFW.UseVisualStyleBackColor = true;
            this.btnSelectDonorFW.Visible = false;
            this.btnSelectDonorFW.Click += new System.EventHandler(this.btnSelectDonorFW_Click);
            // 
            // rbMD9600
            // 
            this.rbMD9600.AccessibleDescription = "MD-9600";
            this.rbMD9600.AccessibleName = "MD-9600 / RT-90";
            this.rbMD9600.AutoSize = true;
            this.rbMD9600.Checked = true;
            this.rbMD9600.Font = new System.Drawing.Font("Arial", 9F);
            this.rbMD9600.Location = new System.Drawing.Point(6, 19);
            this.rbMD9600.Name = "rbMD9600";
            this.rbMD9600.Size = new System.Drawing.Size(117, 19);
            this.rbMD9600.TabIndex = 1;
            this.rbMD9600.TabStop = true;
            this.rbMD9600.Tag = DMR.FirmwareLoaderUI_STM32.OutputType.OutputType_MD9600;
            this.rbMD9600.Text = "MD-9600 / RT-90";
            this.rbMD9600.UseVisualStyleBackColor = true;
            // 
            // rbMDUV380
            // 
            this.rbMDUV380.AccessibleDescription = "MD-UV380 / RT-3S";
            this.rbMDUV380.AccessibleName = "MD-UV380 / RT-3S";
            this.rbMDUV380.AutoSize = true;
            this.rbMDUV380.Location = new System.Drawing.Point(6, 42);
            this.rbMDUV380.Name = "rbMDUV380";
            this.rbMDUV380.Size = new System.Drawing.Size(127, 19);
            this.rbMDUV380.TabIndex = 2;
            this.rbMDUV380.Tag = DMR.FirmwareLoaderUI_STM32.OutputType.OutputType_MDUV380;
            this.rbMDUV380.Text = "MD-UV380 / RT-3S";
            this.rbMDUV380.UseVisualStyleBackColor = true;
            // 
            // rbMD2017
            // 
            this.rbMD2017.AccessibleDescription = "MD-2017 / RT-82";
            this.rbMD2017.AccessibleName = "MD-2017 / RT-82";
            this.rbMD2017.AutoSize = true;
            this.rbMD2017.Location = new System.Drawing.Point(6, 88);
            this.rbMD2017.Name = "rbMD2017";
            this.rbMD2017.Size = new System.Drawing.Size(117, 19);
            this.rbMD2017.TabIndex = 3;
            this.rbMD2017.Tag = DMR.FirmwareLoaderUI_STM32.OutputType.OutputType_MD2017;
            this.rbMD2017.Text = "MD-2017 / RT-82";
            this.rbMD2017.UseVisualStyleBackColor = true;
            // 
            // rbDM1701
            // 
            this.rbDM1701.AccessibleDescription = "DM-1701 / DM-1701B / RT-84";
            this.rbDM1701.AccessibleName = "DM-1701 / DM-1701B / RT-84";
            this.rbDM1701.AutoSize = true;
            this.rbDM1701.Location = new System.Drawing.Point(6, 65);
            this.rbDM1701.Name = "rbDM1701";
            this.rbDM1701.Size = new System.Drawing.Size(184, 19);
            this.rbDM1701.TabIndex = 4;
            this.rbDM1701.Tag = DMR.FirmwareLoaderUI_STM32.OutputType.OutputType_DM1701;
            this.rbDM1701.Text = "DM-1701 / DM-1701B / RT-84";
            this.rbDM1701.UseVisualStyleBackColor = true;
            // 
            // rbMD380
            // 
            this.rbMD380.AccessibleDescription = "MD-380 / RT-3";
            this.rbMD380.AccessibleName = "MD-380 / RT-3";
            this.rbMD380.AutoSize = true;
            this.rbMD380.Location = new System.Drawing.Point(6, 111);
            this.rbMD380.Name = "rbMD380";
            this.rbMD380.Size = new System.Drawing.Size(103, 19);
            this.rbMD380.TabIndex = 5;
            this.rbMD380.Tag = DMR.FirmwareLoaderUI_STM32.OutputType.OutputType_MD380;
            this.rbMD380.Text = "MD-380 / RT-3";
            this.rbMD380.UseVisualStyleBackColor = true;
            // 
            // grpRadioType
            // 
            this.grpRadioType.AccessibleDescription = "Radio type";
            this.grpRadioType.AccessibleName = "Radio type";
            this.grpRadioType.Controls.Add(this.rbMD9600);
            this.grpRadioType.Controls.Add(this.rbMD380);
            this.grpRadioType.Controls.Add(this.rbMDUV380);
            this.grpRadioType.Controls.Add(this.rbMD2017);
            this.grpRadioType.Controls.Add(this.rbDM1701);
            this.grpRadioType.Font = new System.Drawing.Font("Arial", 9F);
            this.grpRadioType.Location = new System.Drawing.Point(12, 15);
            this.grpRadioType.Name = "grpRadioType";
            this.grpRadioType.Size = new System.Drawing.Size(200, 136);
            this.grpRadioType.TabIndex = 0;
            this.grpRadioType.TabStop = false;
            this.grpRadioType.Text = "Radio Type";
            // 
            // findFirmwareLink
            // 
            this.findFirmwareLink.AccessibleDescription = "FindFirmware";
            this.findFirmwareLink.AccessibleName = "FindFirmware";
            this.findFirmwareLink.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.findFirmwareLink.AutoSize = true;
            this.findFirmwareLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.findFirmwareLink.Location = new System.Drawing.Point(236, 22);
            this.findFirmwareLink.Name = "findFirmwareLink";
            this.findFirmwareLink.Size = new System.Drawing.Size(299, 16);
            this.findFirmwareLink.TabIndex = 11;
            this.findFirmwareLink.TabStop = true;
            this.findFirmwareLink.Text = "Найти актуальную прошивку OpenGD77 RUS";
            this.findFirmwareLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.findFirmwareLink_LinkClicked);
            // 
            // flashInstruction
            // 
            this.flashInstruction.AutoSize = true;
            this.flashInstruction.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.flashInstruction.Location = new System.Drawing.Point(236, 40);
            this.flashInstruction.Name = "flashInstruction";
            this.flashInstruction.Size = new System.Drawing.Size(214, 16);
            this.flashInstruction.TabIndex = 12;
            this.flashInstruction.TabStop = true;
            this.flashInstruction.Text = "Инструкция по прошивке рации";
            this.flashInstruction.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.flashInstruction_LinkClicked);
            // 
            // warning
            // 
            this.warning.Location = new System.Drawing.Point(264, 70);
            this.warning.Name = "warning";
            this.warning.Size = new System.Drawing.Size(249, 39);
            this.warning.TabIndex = 13;
            this.warning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // doNotUse
            // 
            this.doNotUse.AutoEllipsis = true;
            this.doNotUse.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.doNotUse.ForeColor = System.Drawing.Color.Red;
            this.doNotUse.Location = new System.Drawing.Point(277, 126);
            this.doNotUse.Name = "doNotUse";
            this.doNotUse.Size = new System.Drawing.Size(219, 100);
            this.doNotUse.TabIndex = 14;
            this.doNotUse.Text = "НЕ ИСПОЛЬЗУЙТЕ\r\nСТАНДАРТНЫЕ\r\nПРОШИВКИ\r\nOPENGD77!";
            this.doNotUse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // downloadRussian
            // 
            this.downloadRussian.AutoSize = true;
            this.downloadRussian.BackColor = System.Drawing.Color.White;
            this.downloadRussian.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.downloadRussian.Location = new System.Drawing.Point(12, 160);
            this.downloadRussian.Name = "downloadRussian";
            this.downloadRussian.Size = new System.Drawing.Size(200, 66);
            this.downloadRussian.TabIndex = 15;
            this.downloadRussian.Text = "button1";
            this.downloadRussian.UseVisualStyleBackColor = false;
            this.downloadRussian.Click += new System.EventHandler(this.downloadRussian_Click);
            // 
            // FirmwareLoaderUI_STM32
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(549, 363);
            this.Controls.Add(this.downloadRussian);
            this.Controls.Add(this.doNotUse);
            this.Controls.Add(this.warning);
            this.Controls.Add(this.flashInstruction);
            this.Controls.Add(this.findFirmwareLink);
            this.Controls.Add(this.grpRadioType);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.Progress);
            this.Controls.Add(this.btnSelectDonorFW);
            this.Controls.Add(this.btnProgram);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FirmwareLoaderUI_STM32";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FirmwareLoaderUI_STM32";
            this.Load += new System.EventHandler(this.FirmwareLoaderUI_MD9600_Load);
            this.grpRadioType.ResumeLayout(false);
            this.grpRadioType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

	}

    private void flashInstruction_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        try
        {
            System.Diagnostics.Process.Start("https://opengd77rus.ru/%d0%b8%d0%bd%d1%81%d1%82%d1%80%d1%83%d0%ba%d1%86%d0%b8%d0%b8/");
        }
        catch (System.ComponentModel.Win32Exception noBrowser)
        {
            if (noBrowser.ErrorCode == -2147467259)
                MessageBox.Show(noBrowser.Message);
        }
        catch (System.Exception other)
        {
            MessageBox.Show(other.Message);
        }
    }

    private void findFirmwareLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        try
        {
            System.Diagnostics.Process.Start("https://opengd77rus.ru/%d0%bf%d1%80%d0%be%d1%88%d0%b8%d0%b2%d0%ba%d0%b0/");
        }
        catch (System.ComponentModel.Win32Exception noBrowser)
        {
            if (noBrowser.ErrorCode == -2147467259)
                MessageBox.Show(noBrowser.Message);
        }
        catch (System.Exception other)
        {
            MessageBox.Show(other.Message);
        }
    }

    private void downloadRussian_Click(object sender, EventArgs e)
    {
        string remoteUri = "https://opengd77rus.ru/data/";
        string fileName = "Russian.gla", fullURI = null;
        lblMessage.Text = "";
        DialogResult dialogResult = MessageBox.Show(StringsDict["WillBeReplaced"], StringsDict["DownloadRussian"], MessageBoxButtons.YesNo);
        if (dialogResult == DialogResult.Yes)
        {
            WebClient glaDownloader = new WebClient();
            fullURI = remoteUri + fileName;
            try
            {
                glaDownloader.DownloadFile(fullURI, "Language\\Firmware\\" + fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StringsDict["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            lblMessage.Text = StringsDict["FileUpdated"];
        }
        
    }
}
