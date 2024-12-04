using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace DMR;

public class FirmwareLoaderReleasesList_GitHub : Form
{
	public string SelectedURL = "";

	public string SelectedVersion = "";

	private IContainer components;

	private DataGridView releasesGridView;

	private Button btnDown;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;

	private Button btnCancel;

	private DataGridViewTextBoxColumn Date;

	private DataGridViewTextBoxColumn Version;

	private DataGridViewTextBoxColumn Type;

	private DataGridViewTextBoxColumn RelName;

	private DataGridViewTextBoxColumn Downloads;

	public FirmwareLoaderReleasesList_GitHub(string downloadedJsonString)
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		List<GithubRelease> list = JsonConvert.DeserializeObject<List<GithubRelease>>(downloadedJsonString);
		string text = "OpenGD77";
		switch (FirmwareLoader_MK22.outputType)
		{
		case FirmwareLoader_MK22.OutputType.OutputType_GD77:
			text = "OpenGD77";
			break;
		case FirmwareLoader_MK22.OutputType.OutputType_GD77S:
			text = "OpenGD77S";
			break;
		case FirmwareLoader_MK22.OutputType.OutputType_DM1801:
			text = "OpenDM1801";
			break;
		case FirmwareLoader_MK22.OutputType.OutputType_RD5R:
			text = "OpenRD5R";
			break;
		}
		if (Settings.LanguageFile == "Japanese.xml" && FirmwareLoader_MK22.outputType != FirmwareLoader_MK22.OutputType.OutputType_GD77S)
		{
			text += "_JA";
		}
		text += ".sgl";
		foreach (GithubRelease item in list)
		{
			foreach (GithubReleaseAssets asset in item.assets)
			{
				if (asset.browser_download_url.IndexOf(text) != -1)
				{
					int index = releasesGridView.Rows.Add(item.published_at.Replace("T", " "), item.tag_name, (!item.prerelease) ? FirmwareLoaderUI_MK22.StringsDict["Stable"] : FirmwareLoaderUI_MK22.StringsDict["Beta"], item.name, asset.download_count);
					releasesGridView.Rows[index].Tag = new ReleaseAndAsset(item, asset);
				}
			}
		}
		releasesGridView.ReadOnly = true;
		if (releasesGridView.Rows.Count > 0)
		{
			releasesGridView.Rows[0].Selected = true;
		}
	}

	private void btnDown_Click(object sender, EventArgs e)
	{
		DataGridViewSelectedCellCollection selectedCells = releasesGridView.SelectedCells;
		if (selectedCells.Count > 0)
		{
			ReleaseAndAsset releaseAndAsset = releasesGridView.Rows[selectedCells[0].RowIndex].Tag as ReleaseAndAsset;
			SelectedURL = releaseAndAsset.Asset.browser_download_url;
			SelectedVersion = releaseAndAsset.Release.tag_name;
			base.DialogResult = DialogResult.OK;
			Close();
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void FirmwareLoaderReleasesList_Load(object sender, EventArgs e)
	{
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
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
		this.releasesGridView = new System.Windows.Forms.DataGridView();
		this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Version = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.RelName = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Downloads = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.btnDown = new System.Windows.Forms.Button();
		this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.btnCancel = new System.Windows.Forms.Button();
		((System.ComponentModel.ISupportInitialize)this.releasesGridView).BeginInit();
		base.SuspendLayout();
		this.releasesGridView.AllowUserToAddRows = false;
		this.releasesGridView.AllowUserToDeleteRows = false;
		this.releasesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.releasesGridView.Columns.AddRange(this.Date, this.Version, this.Type, this.RelName, this.Downloads);
		this.releasesGridView.Location = new System.Drawing.Point(12, 12);
		this.releasesGridView.Name = "releasesGridView";
		this.releasesGridView.ReadOnly = true;
		this.releasesGridView.Size = new System.Drawing.Size(911, 427);
		this.releasesGridView.TabIndex = 0;
		this.Date.HeaderText = "Date & Time";
		this.Date.Name = "Date";
		this.Date.ReadOnly = true;
		this.Date.Width = 125;
		this.Version.HeaderText = "Version";
		this.Version.Name = "Version";
		this.Version.ReadOnly = true;
		this.Type.HeaderText = "Type";
		this.Type.Name = "Type";
		this.Type.ReadOnly = true;
		this.Type.Width = 50;
		this.RelName.HeaderText = "Release Name";
		this.RelName.Name = "RelName";
		this.RelName.ReadOnly = true;
		this.RelName.Width = 500;
		this.Downloads.HeaderText = "Downloads";
		this.Downloads.Name = "Downloads";
		this.Downloads.ReadOnly = true;
		this.Downloads.Width = 75;
		this.btnDown.Location = new System.Drawing.Point(757, 455);
		this.btnDown.Name = "btnDown";
		this.btnDown.Size = new System.Drawing.Size(167, 23);
		this.btnDown.TabIndex = 1;
		this.btnDown.Text = "Select && continue";
		this.btnDown.UseVisualStyleBackColor = true;
		this.btnDown.Click += new System.EventHandler(btnDown_Click);
		this.dataGridViewTextBoxColumn1.HeaderText = "Date";
		this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
		this.dataGridViewTextBoxColumn2.HeaderText = "Type";
		this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
		this.dataGridViewTextBoxColumn3.HeaderText = "Release Name";
		this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
		this.dataGridViewTextBoxColumn4.HeaderText = "Downloads";
		this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
		this.btnCancel.Location = new System.Drawing.Point(12, 455);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(94, 23);
		this.btnCancel.TabIndex = 1;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.UseVisualStyleBackColor = true;
		this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(936, 487);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnDown);
		base.Controls.Add(this.releasesGridView);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.Name = "FirmwareLoaderReleasesList";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Select release version";
		base.Load += new System.EventHandler(FirmwareLoaderReleasesList_Load);
		((System.ComponentModel.ISupportInitialize)this.releasesGridView).EndInit();
		base.ResumeLayout(false);
	}
}
