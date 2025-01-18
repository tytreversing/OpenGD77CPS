using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml.Linq;
using ReadWriteCsv;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR
{
    public partial class CodeplugSettingsForm : DockContent//, IDisp, ISingleRow
    {
        public CodeplugSettingsForm()
        {
            InitializeComponent();
            base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Scale(Settings.smethod_6());
        }

        private void CodeplugSettingsForm_Load(object sender, EventArgs e)
        {
            try
            {
                Settings.smethod_59(base.Controls);
                Settings.UpdateComponentTextsFromLanguageXmlData(this);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
