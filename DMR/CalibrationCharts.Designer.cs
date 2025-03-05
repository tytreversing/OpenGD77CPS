namespace DMR
{
    partial class CalibrationCharts
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series11 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series13 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series14 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series15 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series16 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series17 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series18 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chPowers = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chPowers)).BeginInit();
            this.SuspendLayout();
            // 
            // chPowers
            // 
            chartArea1.AxisX.MajorGrid.Interval = 0D;
            chartArea1.AxisX.Maximum = 4D;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisX.Title = "Настройки мощности VHF";
            chartArea1.AxisX.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            chartArea1.AxisY.Minimum = 0D;
            chartArea1.Name = "caVHF";
            chartArea2.AxisX.Maximum = 8D;
            chartArea2.AxisX.Minimum = 0D;
            chartArea2.AxisX.Title = "Настройки мощности UHF";
            chartArea2.AxisX.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            chartArea2.AxisY.Minimum = 0D;
            chartArea2.Name = "caUHF";
            this.chPowers.ChartAreas.Add(chartArea1);
            this.chPowers.ChartAreas.Add(chartArea2);
            this.chPowers.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.DockedToChartArea = "caVHF";
            legend1.IsDockedInsideChartArea = false;
            legend1.LegendItemOrder = System.Windows.Forms.DataVisualization.Charting.LegendItemOrder.ReversedSeriesOrder;
            legend1.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Column;
            legend1.Name = "Legend1";
            legend1.TableStyle = System.Windows.Forms.DataVisualization.Charting.LegendTableStyle.Wide;
            legend2.DockedToChartArea = "caUHF";
            legend2.IsDockedInsideChartArea = false;
            legend2.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Column;
            legend2.Name = "Legend2";
            legend2.TableStyle = System.Windows.Forms.DataVisualization.Charting.LegendTableStyle.Wide;
            this.chPowers.Legends.Add(legend1);
            this.chPowers.Legends.Add(legend2);
            this.chPowers.Location = new System.Drawing.Point(0, 0);
            this.chPowers.Name = "chPowers";
            this.chPowers.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series1.BorderWidth = 4;
            series1.ChartArea = "caVHF";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "srVHFPow0";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.UInt32;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.UInt32;
            series2.BorderWidth = 4;
            series2.ChartArea = "caVHF";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Legend = "Legend1";
            series2.Name = "srVHFPow1";
            series3.BorderWidth = 4;
            series3.ChartArea = "caVHF";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "srVHFPow2";
            series4.BorderWidth = 4;
            series4.ChartArea = "caVHF";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Legend = "Legend1";
            series4.Name = "srVHFPow3";
            series5.BorderWidth = 4;
            series5.ChartArea = "caVHF";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Legend = "Legend1";
            series5.Name = "srVHFPow4";
            series6.BorderWidth = 4;
            series6.ChartArea = "caVHF";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series6.Legend = "Legend1";
            series6.Name = "srVHFPow5";
            series7.BorderWidth = 4;
            series7.ChartArea = "caVHF";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series7.Legend = "Legend1";
            series7.Name = "srVHFPow6";
            series8.BorderWidth = 4;
            series8.ChartArea = "caVHF";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series8.Legend = "Legend1";
            series8.Name = "srVHFPow7";
            series9.BorderWidth = 4;
            series9.ChartArea = "caVHF";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series9.Legend = "Legend1";
            series9.Name = "srVHFPow8";
            series10.BorderWidth = 4;
            series10.ChartArea = "caUHF";
            series10.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series10.Legend = "Legend2";
            series10.Name = "srUHFPow0";
            series11.BorderWidth = 4;
            series11.ChartArea = "caUHF";
            series11.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series11.Legend = "Legend2";
            series11.Name = "srUHFPow1";
            series12.BorderWidth = 4;
            series12.ChartArea = "caUHF";
            series12.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series12.Legend = "Legend2";
            series12.Name = "srUHFPow2";
            series13.BorderWidth = 4;
            series13.ChartArea = "caUHF";
            series13.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series13.Legend = "Legend2";
            series13.Name = "srUHFPow3";
            series14.BorderWidth = 4;
            series14.ChartArea = "caUHF";
            series14.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series14.Legend = "Legend2";
            series14.Name = "srUHFPow4";
            series15.BorderWidth = 4;
            series15.ChartArea = "caUHF";
            series15.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series15.Legend = "Legend2";
            series15.Name = "srUHFPow5";
            series16.BorderWidth = 4;
            series16.ChartArea = "caUHF";
            series16.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series16.Legend = "Legend2";
            series16.Name = "srUHFPow6";
            series17.BorderWidth = 4;
            series17.ChartArea = "caUHF";
            series17.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series17.Legend = "Legend2";
            series17.Name = "srUHFPow7";
            series18.BorderWidth = 4;
            series18.ChartArea = "caUHF";
            series18.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series18.Legend = "Legend2";
            series18.Name = "srUHFPow8";
            this.chPowers.Series.Add(series1);
            this.chPowers.Series.Add(series2);
            this.chPowers.Series.Add(series3);
            this.chPowers.Series.Add(series4);
            this.chPowers.Series.Add(series5);
            this.chPowers.Series.Add(series6);
            this.chPowers.Series.Add(series7);
            this.chPowers.Series.Add(series8);
            this.chPowers.Series.Add(series9);
            this.chPowers.Series.Add(series10);
            this.chPowers.Series.Add(series11);
            this.chPowers.Series.Add(series12);
            this.chPowers.Series.Add(series13);
            this.chPowers.Series.Add(series14);
            this.chPowers.Series.Add(series15);
            this.chPowers.Series.Add(series16);
            this.chPowers.Series.Add(series17);
            this.chPowers.Series.Add(series18);
            this.chPowers.Size = new System.Drawing.Size(1002, 782);
            this.chPowers.TabIndex = 0;
            // 
            // CalibrationCharts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1002, 782);
            this.Controls.Add(this.chPowers);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalibrationCharts";
            this.Text = "Кривые мощности";
            ((System.ComponentModel.ISupportInitialize)(this.chPowers)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chPowers;
    }
}