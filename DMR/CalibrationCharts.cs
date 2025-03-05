using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMR
{
    public partial class CalibrationCharts: Form
    {
        public CalibrationCharts(CalibrationDataSTM32 calData)
        {
            InitializeComponent();
            base.Icon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            chPowers.Series["srVHFPow0"].LegendText = "Уровень мощности 1";
            chPowers.Series["srVHFPow1"].LegendText = "Уровень мощности 2";
            chPowers.Series["srVHFPow2"].LegendText = "Уровень мощности 3";
            chPowers.Series["srVHFPow3"].LegendText = "Уровень мощности 4";
            chPowers.Series["srVHFPow4"].LegendText = "Уровень мощности 5";
            chPowers.Series["srVHFPow5"].LegendText = "Уровень мощности 6";
            chPowers.Series["srVHFPow6"].LegendText = "Уровень мощности 7";
            chPowers.Series["srVHFPow7"].LegendText = "Уровень мощности 8";
            chPowers.Series["srVHFPow8"].LegendText = "Уровень мощности 9";
            for (int i = 0; i < 5; i++)
            {
                chPowers.Series["srVHFPow0"].Points.AddXY(i, calData.VHFCalPower0[i]);
                chPowers.Series["srVHFPow1"].Points.AddXY(i, calData.VHFCalPower1[i]);
                chPowers.Series["srVHFPow2"].Points.AddXY(i, calData.VHFCalPower2[i]);
                chPowers.Series["srVHFPow3"].Points.AddXY(i, calData.VHFCalPower3[i]);
                chPowers.Series["srVHFPow4"].Points.AddXY(i, calData.VHFCalPower4[i]);
                chPowers.Series["srVHFPow5"].Points.AddXY(i, (byte)calData.VHFLowPowerCal[i] * 16);
                chPowers.Series["srVHFPow6"].Points.AddXY(i, (byte)calData.VHFMidLowPowerCal[i] * 16);
                chPowers.Series["srVHFPow7"].Points.AddXY(i, (byte)calData.VHFMidPowerCal[i] * 16);
                chPowers.Series["srVHFPow8"].Points.AddXY(i, (byte)calData.VHFHighPowerCal[i] * 16);
            }
            chPowers.Series["srUHFPow0"].LegendText = "Уровень мощности 1";
            chPowers.Series["srUHFPow1"].LegendText = "Уровень мощности 2";
            chPowers.Series["srUHFPow2"].LegendText = "Уровень мощности 3";
            chPowers.Series["srUHFPow3"].LegendText = "Уровень мощности 4";
            chPowers.Series["srUHFPow4"].LegendText = "Уровень мощности 5";
            chPowers.Series["srUHFPow5"].LegendText = "Уровень мощности 6";
            chPowers.Series["srUHFPow6"].LegendText = "Уровень мощности 7";
            chPowers.Series["srUHFPow7"].LegendText = "Уровень мощности 8";
            chPowers.Series["srUHFPow8"].LegendText = "Уровень мощности 9";
            for (int i = 0; i < 9; i++)
            {
                chPowers.Series["srUHFPow0"].Points.AddXY(i, calData.UHFCalPower0[i]);
                chPowers.Series["srUHFPow1"].Points.AddXY(i, calData.UHFCalPower1[i]);
                chPowers.Series["srUHFPow2"].Points.AddXY(i, calData.UHFCalPower2[i]);
                chPowers.Series["srUHFPow3"].Points.AddXY(i, calData.UHFCalPower3[i]);
                chPowers.Series["srUHFPow4"].Points.AddXY(i, calData.UHFCalPower4[i]);
                chPowers.Series["srUHFPow5"].Points.AddXY(i, (byte)calData.UHFLowPowerCal[i] * 16);
                chPowers.Series["srUHFPow6"].Points.AddXY(i, (byte)calData.UHFMidLowPowerCal[i] * 16);
                chPowers.Series["srUHFPow7"].Points.AddXY(i, (byte)calData.UHFMidPowerCal[i] * 16);
                chPowers.Series["srUHFPow8"].Points.AddXY(i, (byte)calData.UHFHighPowerCal[i] * 16);
            }
            chPowers.Show();
            
        }
    }
}
