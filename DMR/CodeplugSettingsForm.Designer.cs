namespace DMR
{
    partial class CodeplugSettingsForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodeplugSettingsForm));
            this.gbGeneral = new System.Windows.Forms.GroupBox();
            this.lblGPSMode = new System.Windows.Forms.Label();
            this.chAutoSat = new System.Windows.Forms.CheckBox();
            this.chAPOReset = new System.Windows.Forms.CheckBox();
            this.lblAPO = new System.Windows.Forms.Label();
            this.lblBattery = new System.Windows.Forms.Label();
            this.lblHotspot = new System.Windows.Forms.Label();
            this.lblP3Long = new System.Windows.Forms.Label();
            this.lblP3 = new System.Windows.Forms.Label();
            this.lblAutoBlock = new System.Windows.Forms.Label();
            this.lblRepeat = new System.Windows.Forms.Label();
            this.lblLongPress = new System.Windows.Forms.Label();
            this.gbRadio = new System.Windows.Forms.GroupBox();
            this.lblPriority = new System.Windows.Forms.Label();
            this.lblScanTime = new System.Windows.Forms.Label();
            this.lblScanPause = new System.Windows.Forms.Label();
            this.lblDMRFilter = new System.Windows.Forms.Label();
            this.lblBandlimit = new System.Windows.Forms.Label();
            this.tips = new System.Windows.Forms.ToolTip(this.components);
            this.btnReadSettings = new System.Windows.Forms.Button();
            this.btnWriteSettings = new System.Windows.Forms.Button();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.btnLoadSettings = new System.Windows.Forms.Button();
            this.gbGeneral.SuspendLayout();
            this.gbRadio.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbGeneral
            // 
            this.gbGeneral.Controls.Add(this.lblGPSMode);
            this.gbGeneral.Controls.Add(this.chAutoSat);
            this.gbGeneral.Controls.Add(this.chAPOReset);
            this.gbGeneral.Controls.Add(this.lblAPO);
            this.gbGeneral.Controls.Add(this.lblBattery);
            this.gbGeneral.Controls.Add(this.lblHotspot);
            this.gbGeneral.Controls.Add(this.lblP3Long);
            this.gbGeneral.Controls.Add(this.lblP3);
            this.gbGeneral.Controls.Add(this.lblAutoBlock);
            this.gbGeneral.Controls.Add(this.lblRepeat);
            this.gbGeneral.Controls.Add(this.lblLongPress);
            this.gbGeneral.Location = new System.Drawing.Point(12, 10);
            this.gbGeneral.Name = "gbGeneral";
            this.gbGeneral.Size = new System.Drawing.Size(403, 226);
            this.gbGeneral.TabIndex = 0;
            this.gbGeneral.TabStop = false;
            this.gbGeneral.Text = "groupBox1";
            // 
            // lblGPSMode
            // 
            this.lblGPSMode.AutoSize = true;
            this.lblGPSMode.Location = new System.Drawing.Point(13, 195);
            this.lblGPSMode.Name = "lblGPSMode";
            this.lblGPSMode.Size = new System.Drawing.Size(29, 13);
            this.lblGPSMode.TabIndex = 10;
            this.lblGPSMode.Text = "GPS";
            this.tips.SetToolTip(this.lblGPSMode, resources.GetString("lblGPSMode.ToolTip"));
            // 
            // chAutoSat
            // 
            this.chAutoSat.AutoSize = true;
            this.chAutoSat.Location = new System.Drawing.Point(13, 176);
            this.chAutoSat.Name = "chAutoSat";
            this.chAutoSat.Size = new System.Drawing.Size(80, 17);
            this.chAutoSat.TabIndex = 9;
            this.chAutoSat.Text = "checkBox1";
            this.tips.SetToolTip(this.chAutoSat, "Автоматическое переключение на спутник с ближайшим временем\r\nрасчетного прохожден" +
        "ия. Если не выбрано, рация сохранит настройку\r\nна выбранный вручную спутник ради" +
        "олюбительской связи.");
            this.chAutoSat.UseVisualStyleBackColor = true;
            // 
            // chAPOReset
            // 
            this.chAPOReset.AutoSize = true;
            this.chAPOReset.Location = new System.Drawing.Point(13, 161);
            this.chAPOReset.Name = "chAPOReset";
            this.chAPOReset.Size = new System.Drawing.Size(80, 17);
            this.chAPOReset.TabIndex = 8;
            this.chAPOReset.Text = "checkBox1";
            this.tips.SetToolTip(this.chAPOReset, "Если задан таймер автоматического отключения рации, он будет \r\nсбрасываться кажды" +
        "й раз при передаче или приеме сигнала.");
            this.chAPOReset.UseVisualStyleBackColor = true;
            // 
            // lblAPO
            // 
            this.lblAPO.AutoSize = true;
            this.lblAPO.Location = new System.Drawing.Point(13, 144);
            this.lblAPO.Name = "lblAPO";
            this.lblAPO.Size = new System.Drawing.Size(48, 13);
            this.lblAPO.TabIndex = 7;
            this.lblAPO.Text = "poweroff";
            this.tips.SetToolTip(this.lblAPO, "Автоматическое отключение рации по истечению указанного времени");
            // 
            // lblBattery
            // 
            this.lblBattery.AutoSize = true;
            this.lblBattery.Location = new System.Drawing.Point(13, 127);
            this.lblBattery.Name = "lblBattery";
            this.lblBattery.Size = new System.Drawing.Size(40, 13);
            this.lblBattery.TabIndex = 6;
            this.lblBattery.Text = "Battery";
            this.tips.SetToolTip(this.lblBattery, "Коррекция показаний вольтметра на дисплее. Например, при показаниях 7,2 В \r\nпри и" +
        "змеренном напряжении на аккумуляторе портативной рации 7,4 В\r\nнеобходимо установ" +
        "ить коррекцию +0,2.");
            // 
            // lblHotspot
            // 
            this.lblHotspot.AutoSize = true;
            this.lblHotspot.Location = new System.Drawing.Point(13, 110);
            this.lblHotspot.Name = "lblHotspot";
            this.lblHotspot.Size = new System.Drawing.Size(44, 13);
            this.lblHotspot.TabIndex = 5;
            this.lblHotspot.Text = "Hotspot";
            this.tips.SetToolTip(this.lblHotspot, resources.GetString("lblHotspot.ToolTip"));
            // 
            // lblP3Long
            // 
            this.lblP3Long.AutoSize = true;
            this.lblP3Long.Location = new System.Drawing.Point(13, 93);
            this.lblP3Long.Name = "lblP3Long";
            this.lblP3Long.Size = new System.Drawing.Size(47, 13);
            this.lblP3Long.TabIndex = 4;
            this.lblP3Long.Text = "P3 Long";
            this.tips.SetToolTip(this.lblP3Long, "Действие при длительном нажатии кнопки SK1 на портативных рациях и P3 - на мобиль" +
        "ных");
            // 
            // lblP3
            // 
            this.lblP3.AutoSize = true;
            this.lblP3.Location = new System.Drawing.Point(13, 76);
            this.lblP3.Name = "lblP3";
            this.lblP3.Size = new System.Drawing.Size(54, 13);
            this.lblP3.TabIndex = 3;
            this.lblP3.Text = "P3 Button";
            this.tips.SetToolTip(this.lblP3, "Действие при кратком нажатии кнопки SK1 на портативных рациях и P3 - на мобильных" +
        "");
            // 
            // lblAutoBlock
            // 
            this.lblAutoBlock.AutoSize = true;
            this.lblAutoBlock.Location = new System.Drawing.Point(13, 59);
            this.lblAutoBlock.Name = "lblAutoBlock";
            this.lblAutoBlock.Size = new System.Drawing.Size(55, 13);
            this.lblAutoBlock.TabIndex = 2;
            this.lblAutoBlock.Text = "Autoblock";
            this.tips.SetToolTip(this.lblAutoBlock, "Автоматическая блокировка клавиатуры по истечению указанного времени");
            // 
            // lblRepeat
            // 
            this.lblRepeat.AutoSize = true;
            this.lblRepeat.Location = new System.Drawing.Point(13, 42);
            this.lblRepeat.Name = "lblRepeat";
            this.lblRepeat.Size = new System.Drawing.Size(42, 13);
            this.lblRepeat.TabIndex = 1;
            this.lblRepeat.Text = "Repeat";
            this.tips.SetToolTip(this.lblRepeat, "Частота генерации повторных нажатий при удержании кнопки");
            // 
            // lblLongPress
            // 
            this.lblLongPress.AutoSize = true;
            this.lblLongPress.Location = new System.Drawing.Point(13, 25);
            this.lblLongPress.Name = "lblLongPress";
            this.lblLongPress.Size = new System.Drawing.Size(52, 13);
            this.lblLongPress.TabIndex = 0;
            this.lblLongPress.Text = "longpress";
            this.tips.SetToolTip(this.lblLongPress, "Минимальное время, при котором событие нажатия кнопки обрабатывается как длительн" +
        "ое");
            // 
            // gbRadio
            // 
            this.gbRadio.Controls.Add(this.lblPriority);
            this.gbRadio.Controls.Add(this.lblScanTime);
            this.gbRadio.Controls.Add(this.lblScanPause);
            this.gbRadio.Controls.Add(this.lblDMRFilter);
            this.gbRadio.Controls.Add(this.lblBandlimit);
            this.gbRadio.Location = new System.Drawing.Point(422, 10);
            this.gbRadio.Name = "gbRadio";
            this.gbRadio.Size = new System.Drawing.Size(318, 226);
            this.gbRadio.TabIndex = 1;
            this.gbRadio.TabStop = false;
            this.gbRadio.Text = "groupBox1";
            // 
            // lblPriority
            // 
            this.lblPriority.AutoSize = true;
            this.lblPriority.Location = new System.Drawing.Point(13, 93);
            this.lblPriority.Name = "lblPriority";
            this.lblPriority.Size = new System.Drawing.Size(38, 13);
            this.lblPriority.TabIndex = 4;
            this.lblPriority.Text = "Priority";
            this.tips.SetToolTip(this.lblPriority, resources.GetString("lblPriority.ToolTip"));
            // 
            // lblScanTime
            // 
            this.lblScanTime.AutoSize = true;
            this.lblScanTime.Location = new System.Drawing.Point(13, 76);
            this.lblScanTime.Name = "lblScanTime";
            this.lblScanTime.Size = new System.Drawing.Size(52, 13);
            this.lblScanTime.TabIndex = 3;
            this.lblScanTime.Text = "scan time";
            this.tips.SetToolTip(this.lblScanTime, "Время, в течение которого рация прослушивает очередной канал или частоту.\r\nДейств" +
        "ует для всех каналов, кроме приоритетных.");
            // 
            // lblScanPause
            // 
            this.lblScanPause.AutoSize = true;
            this.lblScanPause.Location = new System.Drawing.Point(13, 59);
            this.lblScanPause.Name = "lblScanPause";
            this.lblScanPause.Size = new System.Drawing.Size(62, 13);
            this.lblScanPause.TabIndex = 2;
            this.lblScanPause.Text = "scan pause";
            this.tips.SetToolTip(this.lblScanPause, "Если выбран режим сканирования \"Пауза\", рация принимает сигнал\r\nв течение заданно" +
        "го времени, затем продолжает сканирование.");
            // 
            // lblDMRFilter
            // 
            this.lblDMRFilter.AutoSize = true;
            this.lblDMRFilter.Location = new System.Drawing.Point(13, 42);
            this.lblDMRFilter.Name = "lblDMRFilter";
            this.lblDMRFilter.Size = new System.Drawing.Size(57, 13);
            this.lblDMRFilter.TabIndex = 1;
            this.lblDMRFilter.Text = "DMR Filter";
            this.tips.SetToolTip(this.lblDMRFilter, "При выключенном фильтре таймслотов в режиме DMR\r\nрация переключается на прослушив" +
        "ание обоих таймслотов\r\nтолько по истечению указанного времени после последнего \r" +
        "\nприема.");
            // 
            // lblBandlimit
            // 
            this.lblBandlimit.AutoSize = true;
            this.lblBandlimit.Location = new System.Drawing.Point(13, 25);
            this.lblBandlimit.Name = "lblBandlimit";
            this.lblBandlimit.Size = new System.Drawing.Size(48, 13);
            this.lblBandlimit.TabIndex = 0;
            this.lblBandlimit.Text = "bandlimit";
            this.tips.SetToolTip(this.lblBandlimit, "Режим частотных ограничений для каналов и VFO");
            // 
            // tips
            // 
            this.tips.IsBalloon = true;
            // 
            // btnReadSettings
            // 
            this.btnReadSettings.BackColor = System.Drawing.SystemColors.Control;
            this.btnReadSettings.Location = new System.Drawing.Point(746, 13);
            this.btnReadSettings.Name = "btnReadSettings";
            this.btnReadSettings.Size = new System.Drawing.Size(335, 25);
            this.btnReadSettings.TabIndex = 2;
            this.btnReadSettings.Text = "read";
            this.btnReadSettings.UseVisualStyleBackColor = false;
            this.btnReadSettings.Click += new System.EventHandler(this.btnReadSettings_Click);
            // 
            // btnWriteSettings
            // 
            this.btnWriteSettings.BackColor = System.Drawing.SystemColors.Control;
            this.btnWriteSettings.Location = new System.Drawing.Point(746, 41);
            this.btnWriteSettings.Name = "btnWriteSettings";
            this.btnWriteSettings.Size = new System.Drawing.Size(335, 25);
            this.btnWriteSettings.TabIndex = 3;
            this.btnWriteSettings.Text = "write";
            this.btnWriteSettings.UseVisualStyleBackColor = false;
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveSettings.Location = new System.Drawing.Point(746, 69);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(335, 25);
            this.btnSaveSettings.TabIndex = 4;
            this.btnSaveSettings.Text = "save";
            this.btnSaveSettings.UseVisualStyleBackColor = false;
            // 
            // btnLoadSettings
            // 
            this.btnLoadSettings.BackColor = System.Drawing.SystemColors.Control;
            this.btnLoadSettings.Location = new System.Drawing.Point(746, 97);
            this.btnLoadSettings.Name = "btnLoadSettings";
            this.btnLoadSettings.Size = new System.Drawing.Size(335, 25);
            this.btnLoadSettings.TabIndex = 5;
            this.btnLoadSettings.Text = "load";
            this.btnLoadSettings.UseVisualStyleBackColor = false;
            // 
            // CodeplugSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1093, 551);
            this.Controls.Add(this.btnLoadSettings);
            this.Controls.Add(this.btnSaveSettings);
            this.Controls.Add(this.btnWriteSettings);
            this.Controls.Add(this.btnReadSettings);
            this.Controls.Add(this.gbRadio);
            this.Controls.Add(this.gbGeneral);
            this.Name = "CodeplugSettingsForm";
            this.Text = "CodeplugSettingsForm";
            this.Load += new System.EventHandler(this.CodeplugSettingsForm_Load);
            this.gbGeneral.ResumeLayout(false);
            this.gbGeneral.PerformLayout();
            this.gbRadio.ResumeLayout(false);
            this.gbRadio.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbGeneral;
        private System.Windows.Forms.Label lblLongPress;
        private System.Windows.Forms.Label lblRepeat;
        private System.Windows.Forms.Label lblAutoBlock;
        private System.Windows.Forms.Label lblP3;
        private System.Windows.Forms.Label lblP3Long;
        private System.Windows.Forms.Label lblBattery;
        private System.Windows.Forms.Label lblHotspot;
        private System.Windows.Forms.Label lblAPO;
        private System.Windows.Forms.CheckBox chAPOReset;
        private System.Windows.Forms.CheckBox chAutoSat;
        private System.Windows.Forms.Label lblGPSMode;
        private System.Windows.Forms.GroupBox gbRadio;
        private System.Windows.Forms.Label lblBandlimit;
        private System.Windows.Forms.ToolTip tips;
        private System.Windows.Forms.Label lblDMRFilter;
        private System.Windows.Forms.Label lblScanPause;
        private System.Windows.Forms.Label lblScanTime;
        private System.Windows.Forms.Label lblPriority;
        private System.Windows.Forms.Button btnReadSettings;
        private System.Windows.Forms.Button btnWriteSettings;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.Button btnLoadSettings;
    }
}