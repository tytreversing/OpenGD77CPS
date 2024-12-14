using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DMR;

public class FirmwareLoaderUI_MK22 : Form
{
	public static readonly string windowTitle = "Firmware Loader";

	public bool IsLoading;

	private static string tempFile = "";

	private WebClientAsync wc;

	private static bool _saveDownloadedFile = false;

	public static Dictionary<string, string> StringsDict = new Dictionary<string, string>();

	public static RegistryKey regKeyOfficialFirmware = null;

	public static string languageFile = "";

	public static bool donorFileIsValid = false;

	public static readonly byte[] FW311_SHA256_Checksum = new byte[32]
	{
		32, 196, 44, 117, 24, 140, 231, 211, 171, 91,
		122, 17, 220, 46, 79, 199, 121, 58, 75, 170,
		254, 12, 219, 150, 127, 244, 134, 246, 51, 1,
		122, 94
	};

	public static readonly byte[] FW436_SHA256_Checksum = new byte[32]
	{
		216, 189, 252, 196, 197, 251, 181, 225, 245, 142,
		45, 111, 36, 51, 129, 198, 115, 214, 208, 157,
		71, 24, 177, 206, 145, 120, 229, 15, 171, 1,
		112, 111
	};

    public static string officialFirmwareFile = Application.ExecutablePath + "\\SourceFirmware\\source.sgl";

    private IContainer components;

	private ProgressBar progressBarDwnl;

	private ProgressBar progressBar1;

	private Button btnClose;

	private Button btnDetectModel;

	private Button btnSelectOfficialFW;

	private Button btnFWInstructions;

	private Label lblMessage;

	private Button btnUploadFirmware;

	private GroupBox grpboxModel;

	private GroupBox grpboxProgress;

	private RadioButton rbModel0;

	private RadioButton rbModel1;

	private RadioButton rbModel2;

	private RadioButton rbModel3;

	private RadioButton[] rbModels;

	private static byte[] GetSHA256Checksum(string filename)
	{
		using SHA256 sHA = SHA256.Create();
		using FileStream inputStream = File.OpenRead(filename);
		return sHA.ComputeHash(inputStream);
	}

	private bool ValidateOfficialFirmware()
	{
		if (File.Exists(officialFirmwareFile) && GetSHA256Checksum(officialFirmwareFile).SequenceEqual(FW436_SHA256_Checksum))
		{
			if (regKeyOfficialFirmware != null)
			{
				regKeyOfficialFirmware.SetValue("SourceFirmware", officialFirmwareFile);
			}

			donorFileIsValid = true;
			foreach (RadioButton control in grpboxModel.Controls)
			{
				if (control.Checked)
				{
					btnUploadFirmware.Enabled = true;
					break;
				}
			}
			return true;
		}
		if (regKeyOfficialFirmware != null)
		{
			regKeyOfficialFirmware.SetValue("SourceFirmware", "");
		}
		officialFirmwareFile = "";
		donorFileIsValid = false;
		btnUploadFirmware.Enabled = false;
		return false;
	}


    public FirmwareLoaderUI_MK22()
	{
       /* if (DMR.FirmwareLoader_MK22.outputType != DMR.FirmwareLoader_MK22.OutputType.OutputType_UNKNOWN)
        {
            this.rbModels[(int)DMR.FirmwareLoader_MK22.outputType].Checked = true;
            this.btnSelectOfficialFW.Enabled = true;
        }*/

		InitializeComponent();
		lblMessage.Text = "";
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		FirmwareLoader_MK22.outputType = FirmwareLoader_MK22.OutputType.OutputType_GD77;
		ValidateOfficialFirmware();
		string profileStringWithDefault = IniFileUtils.getProfileStringWithDefault("Setup", "LastMK22FirmwareRadio", null);
		if (profileStringWithDefault != "")
		{
			foreach (RadioButton control in grpboxModel.Controls)
			{
				if (control.Name == profileStringWithDefault)
				{
					control.Checked = true;
				}
			}
		}
		Settings.ReadCommonsForSectionIntoDictionary(StringsDict, base.Name);

	}

	public void SetLabel(string txt)
	{
		if (lblMessage.InvokeRequired)
		{
			Invoke((Action)delegate
			{
				SetLabel(txt);
			});
		}
		else
		{
			lblMessage.Text = txt;
		}
	}

	public void SetProgressPercentage(int perc)
	{
		if (progressBar1.InvokeRequired)
		{
			Invoke((Action)delegate
			{
				SetProgressPercentage(perc);
			});
		}
		else
		{
			progressBar1.Value = perc;
		}
	}

	public void SetLoadingState(bool loading)
	{
		if (loading)
		{
			Invoke((MethodInvoker)delegate
			{
				grpboxModel.Enabled = false;
				btnClose.Enabled = false;
				grpboxProgress.Enabled = true;
				btnFWInstructions.Enabled = false;
				btnSelectOfficialFW.Enabled = false;
				btnUploadFirmware.Enabled = false;
			});
			return;
		}
		Invoke((MethodInvoker)delegate
		{
			grpboxModel.Enabled = true;
			btnClose.Enabled = true;
			grpboxProgress.Enabled = false;
			progressBar1.Value = 0;
			btnFWInstructions.Enabled = true;
			btnSelectOfficialFW.Enabled = true;
			btnUploadFirmware.Enabled = donorFileIsValid;
		});
	}

	private void FirmwareLoaderUI_Load(object sender, EventArgs e)
	{
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		if (officialFirmwareFile != "")
		{
			Text = windowTitle + " [ +DMR ]";
		}
	}

	private void btnClose_Click(object sender, EventArgs e)
	{
		if (!IsLoading)
		{
			Close();
		}
		else
		{
			MessageBox.Show(StringsDict["CantInterrupt"]);
		}
	}

	private void rbModel_CheckedChanged(object sender, EventArgs e)
	{
		if (sender is RadioButton { Checked: not false } radioButton)
		{
			FirmwareLoader_MK22.outputType = (FirmwareLoader_MK22.OutputType)radioButton.Tag;
			btnSelectOfficialFW.Enabled = true;
			btnUploadFirmware.Enabled = donorFileIsValid;
		}
	}

	private DialogResult DialogBox(string title, string message, string btn1Label = "&Yes", string btn2Label = "&No", string btn3Label = "&Cancel")
	{
		int num = 10;
		int num2 = 90;
		Form form = new Form();
		Label label = new Label();
		Button button = new Button();
		Button button2 = new Button();
		Button button3 = new Button();
		form.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		form.SuspendLayout();
		if (btn1Label.Length <= 0)
		{
			button.Visible = false;
			button.Enabled = false;
		}
		if (btn2Label.Length <= 0)
		{
			button2.Visible = false;
			button2.Enabled = false;
		}
		if (btn1Label.Length <= 0 || btn2Label.Length <= 0)
		{
			num += 130;
		}
		form.Text = title;
		label.Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
		label.Location = new Point(13, 13);
		label.Name = "LblMessage";
		label.Size = new Size(354, 70);
		label.Text = message;
		label.TextAlign = ContentAlignment.MiddleCenter;
		button.Text = btn1Label ?? string.Empty;
		button.Name = "btnYes";
		button.Location = new Point(num, num2);
		button.Size = new Size(120, 24);
		button.UseVisualStyleBackColor = true;
		if (button.Visible)
		{
			num += 130;
		}
		button2.Text = btn2Label ?? string.Empty;
		button2.Name = "btnNo";
		button2.Location = new Point(num, num2);
		button2.Size = new Size(120, 24);
		button2.UseVisualStyleBackColor = true;
		button3.Text = btn3Label ?? string.Empty;
		button3.Location = new Point(270, num2);
		button3.Name = "btnCancel";
		button3.Size = new Size(100, 24);
		button3.UseVisualStyleBackColor = true;
		button.DialogResult = DialogResult.Yes;
		button2.DialogResult = DialogResult.No;
		button3.DialogResult = DialogResult.Cancel;
		form.ClientSize = new Size(396, 107);
		form.Controls.Add(label);
		if (button.Visible)
		{
			form.Controls.Add(button);
		}
		if (button2.Visible)
		{
			form.Controls.Add(button2);
		}
		form.Controls.Add(button3);
		form.AutoScaleDimensions = new SizeF(6f, 13f);
		form.AutoScaleMode = AutoScaleMode.Font;
		form.ClientSize = new Size(380, 120);
		form.KeyPreview = true;
		form.FormBorderStyle = FormBorderStyle.FixedSingle;
		form.MinimizeBox = false;
		form.MaximizeBox = false;
		form.AcceptButton = ((!button.Visible) ? button2 : button);
		form.CancelButton = button3;
		form.StartPosition = FormStartPosition.CenterParent;
		form.ResumeLayout(performLayout: false);
		return form.ShowDialog();
	}

	private void downloadProgressChangedCallback(object sender, DownloadProgressChangedEventArgs ev)
	{
		progressBarDwnl.Value = ev.ProgressPercentage;
	}

	private void downloadStringCompletedCallback(object sender, DownloadStringCompletedEventArgs ev)
	{
		if (ev.Cancelled)
		{
			MessageBox.Show(StringsDict["DownloadCancelled"], StringsDict["Timeout"], MessageBoxButtons.OK, MessageBoxIcon.Hand);
			SetLoadingState(loading: false);
			progressBarDwnl.Visible = false;
			return;
		}
		if (ev.Error != null)
		{
			MessageBox.Show(ev.Error.Message, StringsDict["Error"], MessageBoxButtons.OK, MessageBoxIcon.Hand);
			SetLoadingState(loading: false);
			progressBarDwnl.Visible = false;
			return;
		}
		string result = ev.Result;
		progressBarDwnl.Visible = false;
		FirmwareLoaderReleasesList_GitHub firmwareLoaderReleasesList_GitHub = new FirmwareLoaderReleasesList_GitHub(result);
		if (DialogResult.Cancel != firmwareLoaderReleasesList_GitHub.ShowDialog())
		{
			if (_saveDownloadedFile)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "firmware files|*.sgl";
				saveFileDialog.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastFirmwareLocation", null);
				saveFileDialog.FileName = FirmwareLoader_MK22.getModelSaveFileString(FirmwareLoader_MK22.outputType) + "_" + firmwareLoaderReleasesList_GitHub.SelectedVersion + ".sgl";
				if (saveFileDialog.ShowDialog() != DialogResult.OK || saveFileDialog.FileName == null)
				{
					MessageBox.Show(StringsDict["No_file_location_specified"]);
					SetLoadingState(loading: false);
					IsLoading = false;
					return;
				}
				tempFile = saveFileDialog.FileName;
			}
			else
			{
				tempFile = Path.GetTempPath() + Guid.NewGuid().ToString() + ".sgl";
			}
			try
			{
				Application.DoEvents();
				progressBarDwnl.Value = 0;
				progressBarDwnl.Visible = true;
				wc.DownloadFileAsync(new Uri(firmwareLoaderReleasesList_GitHub.SelectedURL), tempFile);
				return;
			}
			catch (Exception ex)
			{
				MessageBox.Show(StringsDict["Error"] + ": " + ex.Message, StringsDict["Error"], MessageBoxButtons.OK, MessageBoxIcon.Hand);
				if (File.Exists(tempFile))
				{
					File.Delete(tempFile);
				}
				SetLoadingState(loading: false);
				progressBarDwnl.Visible = false;
				return;
			}
		}
		SetLoadingState(loading: false);
	}

	private void btnDetectModel_Click(object sender, EventArgs e)
	{
		btnDetectModel.Enabled = false;
		FirmwareLoader_MK22.outputType = FirmwareLoader_MK22.OutputType.OutputType_UNKNOWN;
		if (FirmwareLoader_MK22.outputType < FirmwareLoader_MK22.OutputType.OutputType_GD77 || FirmwareLoader_MK22.outputType > FirmwareLoader_MK22.OutputType.OutputType_RD5R)
		{
			MessageBox.Show(StringsDict["Error:_Unable_to_detect_your_radio."], StringsDict["Error"], MessageBoxButtons.OK, MessageBoxIcon.Hand);
			FirmwareLoader_MK22.outputType = FirmwareLoader_MK22.OutputType.OutputType_GD77;
		}
		rbModels[(int)FirmwareLoader_MK22.outputType].Checked = true;
		btnDetectModel.Enabled = true;
	}

	private void btnFWInstructions_Click(object sender, EventArgs e)
	{
		Process.Start("https://www.opengd77.com/static/firmware_installation.php?lang=" + Uri.EscapeUriString(MainForm.Language_Name));
	}

	private void btnSelectOfficialFW_Click(object sender, EventArgs e)
	{
		bool flag = false;
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Title = "Select the official firmware v4.3.6";
		openFileDialog.Filter = "firmware files (GD-77_V4.3.6.sgl)|GD-77_V4.3.6.sgl";
        flag = ValidateOfficialFirmware();
			if (flag || (!flag && officialFirmwareFile != ""))
			{
				Text = windowTitle + " [ +DMR ]";
				MessageBox.Show("The official firmware file has been verified, and Open firmware uploads will now have DMR functionaliy", "Success");
			}
			else
			{
				MessageBox.Show("The file you selected was not the official Radioddity firmware version 4.3.6", "Error");
				Text = windowTitle;
			}
	}

	private void btnUploadFirmware_Click(object sender, EventArgs e)
	{
		if (IsLoading)
		{
			return;
		}

		IniFileUtils.WriteProfileString("Setup", "LastFirmwareLanguage", languageFile);
		foreach (RadioButton control in grpboxModel.Controls)
		{
			if (control.Checked)
			{
				IniFileUtils.WriteProfileString("Setup", "LastMK22FirmwareRadio", control.Name);
				break;
			}
		}
		if (officialFirmwareFile == "")
		{
			MessageBox.Show(Settings.dicCommon["OfficialFWNotSelected"], Settings.dicCommon["Error"], MessageBoxButtons.OK);
			return;
		}
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = Settings.dicCommon["FirmwareFilefilter"];
		openFileDialog.Title = Settings.dicCommon["FirmwareSelectorTitle"];
		openFileDialog.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastFirmwareLocation" + FirmwareLoader_MK22.outputType, null);
		if (openFileDialog.InitialDirectory == "")
		{
			openFileDialog.InitialDirectory = IniFileUtils.getProfileStringWithDefault("Setup", "LastFirmwareLocation", null);
		}
		if (openFileDialog.ShowDialog() != DialogResult.OK || openFileDialog.FileName == null)
		{
			return;
		}
		byte[] firmwareBin = null;
		byte[] languageBuf = null;
		if (Path.GetExtension(openFileDialog.FileName).ToLower() == ".zip")
		{
			ZipArchive val = ZipFile.OpenRead(openFileDialog.FileName);
			try
			{
				using (Stream stream = val.Entries.Where((ZipArchiveEntry fn) => Path.GetExtension(fn.FullName) == ".bin").FirstOrDefault().Open())
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						stream.CopyTo(memoryStream);
						firmwareBin = memoryStream.ToArray();
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
						languageBuf = memoryStream2.ToArray();
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
				string path = Path.Combine(Path.GetDirectoryName(openFileDialog.FileName), languageFile + ".gla");
				if (!File.Exists(path))
				{
					lblMessage.Text = "";
					MessageBox.Show(string.Format(StringsDict["AdditionalLanguageNotFound"], languageFile + ".gla"));
					return;
				}
				languageFile = path;
				languageBuf = File.ReadAllBytes(languageFile);
			}
			firmwareBin = File.ReadAllBytes(openFileDialog.FileName);
		}
		IniFileUtils.WriteProfileString("Setup", "LastFirmwareLocation" + FirmwareLoader_MK22.outputType, Path.GetDirectoryName(openFileDialog.FileName));
		lblMessage.Text = "";
		Action<object> action = delegate
		{
			IsLoading = true;
			SetLoadingState(loading: true);
			FirmwareLoader_MK22.UploadFirmware(firmwareBin, languageBuf, this);
			SetLoadingState(loading: false);
			IsLoading = false;
		};
		try
		{
			new Task(action, "LoaderUSB").Start();
		}
		catch (Exception)
		{
			IsLoading = false;
			SetLoadingState(loading: false);
		}
	}

	private void FirmwareLoaderUI_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (IsLoading)
		{
			MessageBox.Show(StringsDict["CantInterrupt"]);
		}
		e.Cancel = IsLoading;
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
            this.lblMessage = new System.Windows.Forms.Label();
            this.progressBarDwnl = new System.Windows.Forms.ProgressBar();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnDetectModel = new System.Windows.Forms.Button();
            this.btnSelectOfficialFW = new System.Windows.Forms.Button();
            this.btnUploadFirmware = new System.Windows.Forms.Button();
            this.btnFWInstructions = new System.Windows.Forms.Button();
            this.grpboxProgress = new System.Windows.Forms.GroupBox();
            this.grpboxModel = new System.Windows.Forms.GroupBox();
            this.rbModel0 = new System.Windows.Forms.RadioButton();
            this.rbModel1 = new System.Windows.Forms.RadioButton();
            this.rbModel2 = new System.Windows.Forms.RadioButton();
            this.rbModel3 = new System.Windows.Forms.RadioButton();
            this.grpboxProgress.SuspendLayout();
            this.grpboxModel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(20, 18);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(470, 20);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "label1";
            // 
            // progressBarDwnl
            // 
            this.progressBarDwnl.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressBarDwnl.Location = new System.Drawing.Point(333, 35);
            this.progressBarDwnl.Name = "progressBarDwnl";
            this.progressBarDwnl.Size = new System.Drawing.Size(170, 8);
            this.progressBarDwnl.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarDwnl.TabIndex = 4;
            this.progressBarDwnl.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressBar1.Location = new System.Drawing.Point(15, 41);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(577, 10);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(540, 175);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDetectModel
            // 
            this.btnDetectModel.AutoSize = true;
            this.btnDetectModel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDetectModel.Location = new System.Drawing.Point(233, 69);
            this.btnDetectModel.MinimumSize = new System.Drawing.Size(170, 25);
            this.btnDetectModel.Name = "btnDetectModel";
            this.btnDetectModel.Size = new System.Drawing.Size(170, 25);
            this.btnDetectModel.TabIndex = 3;
            this.btnDetectModel.Text = "Detect Radio Type";
            this.btnDetectModel.UseVisualStyleBackColor = true;
            this.btnDetectModel.Visible = false;
            this.btnDetectModel.Click += new System.EventHandler(this.btnDetectModel_Click);
            // 
            // btnSelectOfficialFW
            // 
            this.btnSelectOfficialFW.AutoSize = true;
            this.btnSelectOfficialFW.Enabled = false;
            this.btnSelectOfficialFW.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectOfficialFW.Location = new System.Drawing.Point(5, 175);
            this.btnSelectOfficialFW.MinimumSize = new System.Drawing.Size(240, 25);
            this.btnSelectOfficialFW.Name = "btnSelectOfficialFW";
            this.btnSelectOfficialFW.Size = new System.Drawing.Size(240, 25);
            this.btnSelectOfficialFW.TabIndex = 5;
            this.btnSelectOfficialFW.Text = "Select the official firmware file";
            this.btnSelectOfficialFW.UseVisualStyleBackColor = true;
            this.btnSelectOfficialFW.Visible = false;
            this.btnSelectOfficialFW.Click += new System.EventHandler(this.btnSelectOfficialFW_Click);
            // 
            // btnUploadFirmware
            // 
            this.btnUploadFirmware.AutoSize = true;
            this.btnUploadFirmware.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUploadFirmware.Location = new System.Drawing.Point(351, 50);
            this.btnUploadFirmware.MinimumSize = new System.Drawing.Size(170, 25);
            this.btnUploadFirmware.Name = "btnUploadFirmware";
            this.btnUploadFirmware.Size = new System.Drawing.Size(249, 25);
            this.btnUploadFirmware.TabIndex = 7;
            this.btnUploadFirmware.Text = "Select a &File && Update";
            this.btnUploadFirmware.UseVisualStyleBackColor = true;
            this.btnUploadFirmware.Click += new System.EventHandler(this.btnUploadFirmware_Click);
            // 
            // btnFWInstructions
            // 
            this.btnFWInstructions.AutoSize = true;
            this.btnFWInstructions.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFWInstructions.Location = new System.Drawing.Point(270, 175);
            this.btnFWInstructions.MinimumSize = new System.Drawing.Size(240, 25);
            this.btnFWInstructions.Name = "btnFWInstructions";
            this.btnFWInstructions.Size = new System.Drawing.Size(240, 25);
            this.btnFWInstructions.TabIndex = 5;
            this.btnFWInstructions.Text = "Firmware Installation &Instructions";
            this.btnFWInstructions.UseVisualStyleBackColor = true;
            this.btnFWInstructions.Visible = false;
            this.btnFWInstructions.Click += new System.EventHandler(this.btnFWInstructions_Click);
            // 
            // grpboxProgress
            // 
            this.grpboxProgress.Controls.Add(this.lblMessage);
            this.grpboxProgress.Controls.Add(this.progressBar1);
            this.grpboxProgress.Enabled = false;
            this.grpboxProgress.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpboxProgress.Location = new System.Drawing.Point(5, 105);
            this.grpboxProgress.Name = "grpboxProgress";
            this.grpboxProgress.Size = new System.Drawing.Size(603, 60);
            this.grpboxProgress.TabIndex = 13;
            this.grpboxProgress.TabStop = false;
            this.grpboxProgress.Text = " Progress ";
            // 
            // grpboxModel
            // 
            this.grpboxModel.Controls.Add(this.rbModel0);
            this.grpboxModel.Controls.Add(this.rbModel1);
            this.grpboxModel.Controls.Add(this.rbModel2);
            this.grpboxModel.Controls.Add(this.rbModel3);
            this.grpboxModel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpboxModel.Location = new System.Drawing.Point(5, 5);
            this.grpboxModel.Name = "grpboxModel";
            this.grpboxModel.Size = new System.Drawing.Size(250, 98);
            this.grpboxModel.TabIndex = 12;
            this.grpboxModel.TabStop = false;
            this.grpboxModel.Text = " Select your radio type ";
            // 
            // rbModel0
            // 
            this.rbModel0.AutoSize = true;
            this.rbModel0.Checked = true;
            this.rbModel0.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbModel0.Location = new System.Drawing.Point(5, 15);
            this.rbModel0.Name = "rbModel0";
            this.rbModel0.Size = new System.Drawing.Size(198, 19);
            this.rbModel0.TabIndex = 0;
            this.rbModel0.TabStop = true;
            this.rbModel0.Tag = 0;
            this.rbModel0.Text = "Radioddity GD-&77 / TYT MD-760";
            this.rbModel0.UseVisualStyleBackColor = true;
            this.rbModel0.CheckedChanged += new System.EventHandler(this.rbModel_CheckedChanged);
            // 
            // rbModel1
            // 
            this.rbModel1.AutoSize = true;
            this.rbModel1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbModel1.Location = new System.Drawing.Point(5, 35);
            this.rbModel1.Name = "rbModel1";
            this.rbModel1.Size = new System.Drawing.Size(206, 19);
            this.rbModel1.TabIndex = 1;
            this.rbModel1.Tag = 1;
            this.rbModel1.Text = "Radioddity GD-77&S / TYT MD-730";
            this.rbModel1.UseVisualStyleBackColor = true;
            this.rbModel1.CheckedChanged += new System.EventHandler(this.rbModel_CheckedChanged);
            // 
            // rbModel2
            // 
            this.rbModel2.AutoSize = true;
            this.rbModel2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbModel2.Location = new System.Drawing.Point(5, 55);
            this.rbModel2.Name = "rbModel2";
            this.rbModel2.Size = new System.Drawing.Size(176, 19);
            this.rbModel2.TabIndex = 2;
            this.rbModel2.Tag = 2;
            this.rbModel2.Text = "Baofeng DM-&1801 / DM-860";
            this.rbModel2.UseVisualStyleBackColor = true;
            this.rbModel2.CheckedChanged += new System.EventHandler(this.rbModel_CheckedChanged);
            // 
            // rbModel3
            // 
            this.rbModel3.AutoSize = true;
            this.rbModel3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbModel3.Location = new System.Drawing.Point(5, 75);
            this.rbModel3.Name = "rbModel3";
            this.rbModel3.Size = new System.Drawing.Size(190, 19);
            this.rbModel3.TabIndex = 3;
            this.rbModel3.Tag = 3;
            this.rbModel3.Text = "Baofeng RD-&5R / DM-5R Tier2";
            this.rbModel3.UseVisualStyleBackColor = true;
            this.rbModel3.CheckedChanged += new System.EventHandler(this.rbModel_CheckedChanged);
            // 
            // FirmwareLoaderUI_MK22
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(620, 205);
            this.Controls.Add(this.btnSelectOfficialFW);
            this.Controls.Add(this.btnDetectModel);
            this.Controls.Add(this.progressBarDwnl);
            this.Controls.Add(this.btnUploadFirmware);
            this.Controls.Add(this.btnFWInstructions);
            this.Controls.Add(this.grpboxModel);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.grpboxProgress);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FirmwareLoaderUI_MK22";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Firmware Loader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FirmwareLoaderUI_FormClosing);
            this.Load += new System.EventHandler(this.FirmwareLoaderUI_Load);
            this.grpboxProgress.ResumeLayout(false);
            this.grpboxModel.ResumeLayout(false);
            this.grpboxModel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

	}
}
