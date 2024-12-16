using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace DMR
{
    public partial class AppSettings : Form
    {
        private int oldInterval = 0;
        private string oldURI = "";
        public AppSettings()
        {
            InitializeComponent();
            oldInterval = IniFileUtils.getProfileIntWithDefault("Setuo", "PollingInterval", 500);
            numPolling.Value = oldInterval;
            tbURI.Text = IniFileUtils.getProfileStringWithDefault("Setup", "ServerURI", "https://opengd77rus.ru/data/");
            oldURI = tbURI.Text;
        }

        private void AppSettings_Load(object sender, EventArgs e)
        {
            try
            {
                Settings.UpdateComponentTextsFromLanguageXmlData(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            btnRestart.Visible = true;
            IniFileUtils.WriteProfileInt("Setup", "PollingInterval", (int)numPolling.Value);
            IniFileUtils.WriteProfileString("Setup", "ServerURI", tbURI.Text);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            IniFileUtils.WriteProfileInt("Setup", "PollingInterval", oldInterval);
            IniFileUtils.WriteProfileString("Setup", "ServerURI", oldURI);
            Close();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Restart();
        }
    }


}
