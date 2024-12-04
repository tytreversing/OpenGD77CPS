using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
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

	public static string officialFirmwareFile = "";

	public string officialFirmwareFilePath = "";

	public static string languageFile = "";

	public string windowTitle;

	private const string DONOR_FIRMWARE_PATH_KEY_NAME = "SourceFirmware-D2645";

	private bool donorFirmwareLoaded;

	private STM_DFU_FwUpdate fwUpdate;

	public static Dictionary<string, string> StringsDict = new Dictionary<string, string>();

	private IContainer components;

	private Label lblLanguage;

	private ComboBox cmbLanguage;

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
		regKeyOfficialFirmware = Registry.CurrentUser.CreateSubKey("SOFTWARE\\MD9600FirmwareLoader");
		officialFirmwareFilePath = ((regKeyOfficialFirmware != null && regKeyOfficialFirmware.GetValueNames().Contains("SourceFirmware-D2645")) ? regKeyOfficialFirmware.GetValue("SourceFirmware-D2645").ToString() : "");
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
		for (int i = 0; i < MainForm.FirmwareLanguageFiles.Length; i++)
		{
			array[i + 1] = MainForm.FirmwareLanguageFiles[i];
		}
		cmbLanguage.Items.AddRange(array);
		string profileStringWithDefault2 = IniFileUtils.getProfileStringWithDefault("Setup", "LastFirmwareLanguage", "");
		cmbLanguage.SelectedIndex = 0;
		if (!(profileStringWithDefault2 != ""))
		{
			return;
		}
		foreach (object item in cmbLanguage.Items)
		{
			if ((string)item == profileStringWithDefault2)
			{
				cmbLanguage.SelectedItem = item;
				break;
			}
		}
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
			if (cmbLanguage.SelectedIndex >= 1)
			{
				languageFile = cmbLanguage.SelectedItem.ToString();
			}
			else
			{
				languageFile = "";
			}
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
			ComboBox comboBox = cmbLanguage;
			Button button = btnProgram;
			Button button2 = btnSelectDonorFW;
			bool flag2 = (grpRadioType.Enabled = false);
			bool flag4 = (button2.Enabled = flag2);
			bool enabled = (button.Enabled = flag4);
			comboBox.Enabled = enabled;
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
						ComboBox comboBox2 = cmbLanguage;
						Button button3 = btnProgram;
						Button button4 = btnSelectDonorFW;
						flag2 = (grpRadioType.Enabled = true);
						flag4 = (button4.Enabled = flag2);
						enabled = (button3.Enabled = flag4);
						comboBox2.Enabled = enabled;
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
			ComboBox comboBox3 = cmbLanguage;
			Button button5 = btnProgram;
			Button button6 = btnSelectDonorFW;
			bool flag2 = (grpRadioType.Enabled = true);
			bool flag4 = (button6.Enabled = flag2);
			bool enabled = (button5.Enabled = flag4);
			comboBox3.Enabled = enabled;
		}
	}

	private void UploadCompleted(object sender, FirmwareUpdateMessageEventArgs e)
	{
		if (Progress.InvokeRequired)
		{
			Invoke(new EventHandler<FirmwareUpdateMessageEventArgs>(UploadCompleted), sender, e);
			return;
		}
		ComboBox comboBox = cmbLanguage;
		Button button = btnProgram;
		Button button2 = btnSelectDonorFW;
		bool flag2 = (grpRadioType.Enabled = true);
		bool flag4 = (button2.Enabled = flag2);
		bool enabled = (button.Enabled = flag4);
		comboBox.Enabled = enabled;
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
		if (filename.Length > 0 && File.Exists(filename) && GetSHA256Checksum(filename).SequenceEqual(FW_D2645_SHA256_Checksum))
		{
			if (regKeyOfficialFirmware != null)
			{
				regKeyOfficialFirmware.SetValue("SourceFirmware-D2645", filename);
			}
			officialFirmwareFile = filename;
			donorFirmwareLoaded = true;
			btnProgram.Enabled = true;
			return true;
		}
		if (regKeyOfficialFirmware != null)
		{
			regKeyOfficialFirmware.SetValue("SourceFirmware-D2645", "");
		}
		officialFirmwareFile = "";
		btnProgram.Enabled = false;
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
		this.lblLanguage = new System.Windows.Forms.Label();
		this.cmbLanguage = new System.Windows.Forms.ComboBox();
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
		this.grpRadioType.SuspendLayout();
		base.SuspendLayout();
		this.lblLanguage.Font = new System.Drawing.Font("Arial", 9f);
		this.lblLanguage.Location = new System.Drawing.Point(238, 34);
		this.lblLanguage.Name = "lblLanguage";
		this.lblLanguage.Size = new System.Drawing.Size(123, 24);
		this.lblLanguage.TabIndex = 11;
		this.lblLanguage.Text = "Additional language";
		this.lblLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbLanguage.Font = new System.Drawing.Font("Arial", 9f);
		this.cmbLanguage.Location = new System.Drawing.Point(369, 35);
		this.cmbLanguage.Name = "cmbLanguage";
		this.cmbLanguage.Size = new System.Drawing.Size(113, 23);
		this.cmbLanguage.TabIndex = 6;
		this.btnProgram.Enabled = false;
		this.btnProgram.Font = new System.Drawing.Font("Arial", 9f);
		this.btnProgram.Location = new System.Drawing.Point(241, 75);
		this.btnProgram.Name = "btnProgram";
		this.btnProgram.Size = new System.Drawing.Size(240, 26);
		this.btnProgram.TabIndex = 7;
		this.btnProgram.Text = "Select OpenMD9600 file && update";
		this.btnProgram.UseVisualStyleBackColor = true;
		this.btnProgram.Click += new System.EventHandler(btnProgram_Click);
		this.Progress.Location = new System.Drawing.Point(11, 173);
		this.Progress.Name = "Progress";
		this.Progress.Size = new System.Drawing.Size(474, 13);
		this.Progress.TabIndex = 9;
		this.lblMessage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.lblMessage.Font = new System.Drawing.Font("Arial", 9f);
		this.lblMessage.Location = new System.Drawing.Point(11, 198);
		this.lblMessage.Name = "lblMessage";
		this.lblMessage.Size = new System.Drawing.Size(474, 24);
		this.lblMessage.TabIndex = 10;
		this.btnSelectDonorFW.Font = new System.Drawing.Font("Arial", 9f);
		this.btnSelectDonorFW.Location = new System.Drawing.Point(241, 108);
		this.btnSelectDonorFW.Name = "btnSelectDonorFW";
		this.btnSelectDonorFW.Size = new System.Drawing.Size(240, 26);
		this.btnSelectDonorFW.TabIndex = 8;
		this.btnSelectDonorFW.Text = "Select official firmware (donor) file";
		this.btnSelectDonorFW.UseVisualStyleBackColor = true;
		this.btnSelectDonorFW.Click += new System.EventHandler(btnSelectDonorFW_Click);
		this.rbMD9600.AccessibleDescription = "MD-9600";
		this.rbMD9600.AccessibleName = "MD-9600 / RT-90";
		this.rbMD9600.AutoSize = true;
		this.rbMD9600.Checked = true;
		this.rbMD9600.Font = new System.Drawing.Font("Arial", 9f);
		this.rbMD9600.Location = new System.Drawing.Point(6, 19);
		this.rbMD9600.Name = "rbMD9600";
		this.rbMD9600.Size = new System.Drawing.Size(117, 19);
		this.rbMD9600.TabIndex = 1;
		this.rbMD9600.TabStop = true;
		this.rbMD9600.Tag = DMR.FirmwareLoaderUI_STM32.OutputType.OutputType_MD9600;
		this.rbMD9600.Text = "MD-9600 / RT-90";
		this.rbMD9600.UseVisualStyleBackColor = true;
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
		this.grpRadioType.AccessibleDescription = "Radio type";
		this.grpRadioType.AccessibleName = "Radio type";
		this.grpRadioType.Controls.Add(this.rbMD9600);
		this.grpRadioType.Controls.Add(this.rbMD380);
		this.grpRadioType.Controls.Add(this.rbMDUV380);
		this.grpRadioType.Controls.Add(this.rbMD2017);
		this.grpRadioType.Controls.Add(this.rbDM1701);
		this.grpRadioType.Font = new System.Drawing.Font("Arial", 9f);
		this.grpRadioType.Location = new System.Drawing.Point(12, 15);
		this.grpRadioType.Name = "grpRadioType";
		this.grpRadioType.Size = new System.Drawing.Size(200, 136);
		this.grpRadioType.TabIndex = 0;
		this.grpRadioType.TabStop = false;
		this.grpRadioType.Text = "Radio type";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(493, 232);
		base.Controls.Add(this.grpRadioType);
		base.Controls.Add(this.lblMessage);
		base.Controls.Add(this.Progress);
		base.Controls.Add(this.btnSelectDonorFW);
		base.Controls.Add(this.btnProgram);
		base.Controls.Add(this.lblLanguage);
		base.Controls.Add(this.cmbLanguage);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "FirmwareLoaderUI_STM32";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "STM32 firmware loader";
		base.Load += new System.EventHandler(FirmwareLoaderUI_MD9600_Load);
		this.grpRadioType.ResumeLayout(false);
		this.grpRadioType.PerformLayout();
		base.ResumeLayout(false);
	}
}
