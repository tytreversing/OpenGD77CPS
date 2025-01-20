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
            this.rbCPS = new System.Windows.Forms.RadioButton();
            this.rbHam = new System.Windows.Forms.RadioButton();
            this.tbDMRFilter = new System.Windows.Forms.TextBox();
            this.tbScanPause = new System.Windows.Forms.TextBox();
            this.tbScanTime = new System.Windows.Forms.TextBox();
            this.nmPriority = new System.Windows.Forms.NumericUpDown();
            this.lblPriority = new System.Windows.Forms.Label();
            this.lblScanTime = new System.Windows.Forms.Label();
            this.lblScanPause = new System.Windows.Forms.Label();
            this.lblDMRFilter = new System.Windows.Forms.Label();
            this.lblBandlimit = new System.Windows.Forms.Label();
            this.tips = new System.Windows.Forms.ToolTip(this.components);
            this.lblScanMode = new System.Windows.Forms.Label();
            this.chAutoScan = new System.Windows.Forms.CheckBox();
            this.lblSQLVHF = new System.Windows.Forms.Label();
            this.lblSQLUHF = new System.Windows.Forms.Label();
            this.lblSQL220 = new System.Windows.Forms.Label();
            this.cbPTTLatch = new System.Windows.Forms.CheckBox();
            this.lblPower = new System.Windows.Forms.Label();
            this.lblPrivate = new System.Windows.Forms.Label();
            this.cbDMRCRC = new System.Windows.Forms.CheckBox();
            this.cb10WMode = new System.Windows.Forms.CheckBox();
            this.cbTrackball = new System.Windows.Forms.CheckBox();
            this.cbFastTrackball = new System.Windows.Forms.CheckBox();
            this.lblDayBacklight = new System.Windows.Forms.Label();
            this.lblNightBacklight = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReadSettings = new System.Windows.Forms.Button();
            this.btnWriteSettings = new System.Windows.Forms.Button();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.btnLoadSettings = new System.Windows.Forms.Button();
            this.rbBeiDou = new System.Windows.Forms.RadioButton();
            this.rbGlonass = new System.Windows.Forms.RadioButton();
            this.rbHold = new System.Windows.Forms.RadioButton();
            this.pnBandlimit = new System.Windows.Forms.Panel();
            this.pnScanMode = new System.Windows.Forms.Panel();
            this.rbStop = new System.Windows.Forms.RadioButton();
            this.rbPause = new System.Windows.Forms.RadioButton();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tpCommons = new System.Windows.Forms.TabPage();
            this.tpRadio = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbPrivateByPTT = new System.Windows.Forms.RadioButton();
            this.rbPrivateOn = new System.Windows.Forms.RadioButton();
            this.rbPrivateOff = new System.Windows.Forms.RadioButton();
            this.cbPowerLevel = new System.Windows.Forms.ComboBox();
            this.nm220Squelch = new System.Windows.Forms.NumericUpDown();
            this.nmUHFSquelch = new System.Windows.Forms.NumericUpDown();
            this.nmVHFSquelch = new System.Windows.Forms.NumericUpDown();
            this.tpDisplay = new System.Windows.Forms.TabPage();
            this.nmMinBacklight = new System.Windows.Forms.NumericUpDown();
            this.nmNightBacklight = new System.Windows.Forms.NumericUpDown();
            this.nmDayBacklight = new System.Windows.Forms.NumericUpDown();
            this.lblEco = new System.Windows.Forms.Label();
            this.cmbEco = new System.Windows.Forms.ComboBox();
            this.cbSafeOn = new System.Windows.Forms.CheckBox();
            this.cmbSK1 = new System.Windows.Forms.ComboBox();
            this.cmbSK1Long = new System.Windows.Forms.ComboBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblWarning = new System.Windows.Forms.Label();
            this.cmbHotspot = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.nmPriority)).BeginInit();
            this.pnBandlimit.SuspendLayout();
            this.pnScanMode.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tpCommons.SuspendLayout();
            this.tpRadio.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nm220Squelch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmUHFSquelch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmVHFSquelch)).BeginInit();
            this.tpDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmMinBacklight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmNightBacklight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmDayBacklight)).BeginInit();
            this.SuspendLayout();
            // 
            // lblGPSMode
            // 
            this.lblGPSMode.AutoSize = true;
            this.lblGPSMode.Location = new System.Drawing.Point(17, 225);
            this.lblGPSMode.Name = "lblGPSMode";
            this.lblGPSMode.Size = new System.Drawing.Size(158, 13);
            this.lblGPSMode.TabIndex = 10;
            this.lblGPSMode.Text = "Режим геопозиционирования";
            this.tips.SetToolTip(this.lblGPSMode, resources.GetString("lblGPSMode.ToolTip"));
            // 
            // chAutoSat
            // 
            this.chAutoSat.AutoSize = true;
            this.chAutoSat.Location = new System.Drawing.Point(20, 206);
            this.chAutoSat.Name = "chAutoSat";
            this.chAutoSat.Size = new System.Drawing.Size(131, 17);
            this.chAutoSat.TabIndex = 9;
            this.chAutoSat.Text = "Автовыбор спутника";
            this.tips.SetToolTip(this.chAutoSat, "Автоматическое переключение на спутник с ближайшим временем\r\nрасчетного прохожден" +
        "ия. Если не выбрано, рация сохранит настройку\r\nна выбранный вручную спутник ради" +
        "олюбительской связи.");
            this.chAutoSat.UseVisualStyleBackColor = true;
            // 
            // chAPOReset
            // 
            this.chAPOReset.AutoSize = true;
            this.chAPOReset.Location = new System.Drawing.Point(20, 191);
            this.chAPOReset.Name = "chAPOReset";
            this.chAPOReset.Size = new System.Drawing.Size(146, 17);
            this.chAPOReset.TabIndex = 8;
            this.chAPOReset.Text = "Сброс автовыключения";
            this.tips.SetToolTip(this.chAPOReset, "Если задан таймер автоматического отключения рации, он будет \r\nсбрасываться кажды" +
        "й раз при передаче или приеме сигнала.");
            this.chAPOReset.UseVisualStyleBackColor = true;
            // 
            // lblAPO
            // 
            this.lblAPO.AutoSize = true;
            this.lblAPO.Location = new System.Drawing.Point(17, 174);
            this.lblAPO.Name = "lblAPO";
            this.lblAPO.Size = new System.Drawing.Size(120, 13);
            this.lblAPO.TabIndex = 7;
            this.lblAPO.Text = "Автовыключение, мин";
            this.tips.SetToolTip(this.lblAPO, "Автоматическое отключение рации по истечению указанного времени");
            // 
            // lblBattery
            // 
            this.lblBattery.AutoSize = true;
            this.lblBattery.Location = new System.Drawing.Point(17, 157);
            this.lblBattery.Name = "lblBattery";
            this.lblBattery.Size = new System.Drawing.Size(133, 13);
            this.lblBattery.TabIndex = 6;
            this.lblBattery.Text = "Калибровка напряжения";
            this.tips.SetToolTip(this.lblBattery, "Коррекция показаний вольтметра на дисплее. Например, при показаниях 7,2 В \r\nпри и" +
        "змеренном напряжении на аккумуляторе портативной рации 7,4 В\r\nнеобходимо установ" +
        "ить коррекцию +0,2.");
            // 
            // lblHotspot
            // 
            this.lblHotspot.AutoSize = true;
            this.lblHotspot.Location = new System.Drawing.Point(17, 127);
            this.lblHotspot.Name = "lblHotspot";
            this.lblHotspot.Size = new System.Drawing.Size(90, 13);
            this.lblHotspot.TabIndex = 5;
            this.lblHotspot.Text = "Режим хотспота";
            this.tips.SetToolTip(this.lblHotspot, resources.GetString("lblHotspot.ToolTip"));
            // 
            // lblP3Long
            // 
            this.lblP3Long.AutoSize = true;
            this.lblP3Long.Location = new System.Drawing.Point(17, 104);
            this.lblP3Long.Name = "lblP3Long";
            this.lblP3Long.Size = new System.Drawing.Size(156, 13);
            this.lblP3Long.TabIndex = 4;
            this.lblP3Long.Text = "Длительное нажатие SK1/P3";
            this.tips.SetToolTip(this.lblP3Long, resources.GetString("lblP3Long.ToolTip"));
            // 
            // lblP3
            // 
            this.lblP3.AutoSize = true;
            this.lblP3.Location = new System.Drawing.Point(17, 81);
            this.lblP3.Name = "lblP3";
            this.lblP3.Size = new System.Drawing.Size(94, 13);
            this.lblP3.TabIndex = 3;
            this.lblP3.Text = "Функция SK1/P3";
            this.tips.SetToolTip(this.lblP3, resources.GetString("lblP3.ToolTip"));
            // 
            // lblAutoBlock
            // 
            this.lblAutoBlock.AutoSize = true;
            this.lblAutoBlock.Location = new System.Drawing.Point(17, 46);
            this.lblAutoBlock.Name = "lblAutoBlock";
            this.lblAutoBlock.Size = new System.Drawing.Size(91, 13);
            this.lblAutoBlock.TabIndex = 2;
            this.lblAutoBlock.Text = "Автоблокировка";
            this.tips.SetToolTip(this.lblAutoBlock, "Автоматическая блокировка клавиатуры по истечению указанного времени");
            // 
            // lblRepeat
            // 
            this.lblRepeat.AutoSize = true;
            this.lblRepeat.Location = new System.Drawing.Point(17, 29);
            this.lblRepeat.Name = "lblRepeat";
            this.lblRepeat.Size = new System.Drawing.Size(90, 13);
            this.lblRepeat.TabIndex = 1;
            this.lblRepeat.Text = "Повтор нажатия";
            this.tips.SetToolTip(this.lblRepeat, "Частота генерации повторных нажатий при удержании кнопки");
            // 
            // lblLongPress
            // 
            this.lblLongPress.AutoSize = true;
            this.lblLongPress.Location = new System.Drawing.Point(17, 12);
            this.lblLongPress.Name = "lblLongPress";
            this.lblLongPress.Size = new System.Drawing.Size(127, 13);
            this.lblLongPress.TabIndex = 0;
            this.lblLongPress.Text = "Длительное нажатие, с";
            this.tips.SetToolTip(this.lblLongPress, "Минимальное время, при котором событие нажатия кнопки обрабатывается как длительн" +
        "ое");
            // 
            // rbCPS
            // 
            this.rbCPS.AutoSize = true;
            this.rbCPS.Location = new System.Drawing.Point(3, 20);
            this.rbCPS.Name = "rbCPS";
            this.rbCPS.Size = new System.Drawing.Size(61, 17);
            this.rbCPS.TabIndex = 10;
            this.rbCPS.Text = "по CPS";
            this.rbCPS.UseVisualStyleBackColor = true;
            // 
            // rbHam
            // 
            this.rbHam.AutoSize = true;
            this.rbHam.Checked = true;
            this.rbHam.Location = new System.Drawing.Point(3, 3);
            this.rbHam.Name = "rbHam";
            this.rbHam.Size = new System.Drawing.Size(128, 17);
            this.rbHam.TabIndex = 9;
            this.rbHam.TabStop = true;
            this.rbHam.Text = "радиолюбительские";
            this.rbHam.UseVisualStyleBackColor = true;
            // 
            // tbDMRFilter
            // 
            this.tbDMRFilter.Location = new System.Drawing.Point(183, 59);
            this.tbDMRFilter.Name = "tbDMRFilter";
            this.tbDMRFilter.Size = new System.Drawing.Size(120, 20);
            this.tbDMRFilter.TabIndex = 8;
            this.tbDMRFilter.Text = "10";
            this.tbDMRFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filterNumerics);
            this.tbDMRFilter.Leave += new System.EventHandler(this.tbDMRFilter_Leave);
            // 
            // tbScanPause
            // 
            this.tbScanPause.Location = new System.Drawing.Point(183, 85);
            this.tbScanPause.Name = "tbScanPause";
            this.tbScanPause.Size = new System.Drawing.Size(120, 20);
            this.tbScanPause.TabIndex = 7;
            this.tbScanPause.Text = "5";
            this.tbScanPause.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filterNumerics);
            this.tbScanPause.Leave += new System.EventHandler(this.tbScanPause_Leave);
            // 
            // tbScanTime
            // 
            this.tbScanTime.Location = new System.Drawing.Point(183, 111);
            this.tbScanTime.Name = "tbScanTime";
            this.tbScanTime.Size = new System.Drawing.Size(120, 20);
            this.tbScanTime.TabIndex = 6;
            this.tbScanTime.Text = "30";
            this.tbScanTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filterNumerics);
            this.tbScanTime.Leave += new System.EventHandler(this.tbScanTime_Leave);
            // 
            // nmPriority
            // 
            this.nmPriority.Location = new System.Drawing.Point(183, 137);
            this.nmPriority.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nmPriority.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nmPriority.Name = "nmPriority";
            this.nmPriority.Size = new System.Drawing.Size(120, 20);
            this.nmPriority.TabIndex = 5;
            this.nmPriority.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nmPriority.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filterNumerics);
            this.nmPriority.Leave += new System.EventHandler(this.nmPriority_Leave);
            // 
            // lblPriority
            // 
            this.lblPriority.AutoSize = true;
            this.lblPriority.Location = new System.Drawing.Point(17, 139);
            this.lblPriority.Name = "lblPriority";
            this.lblPriority.Size = new System.Drawing.Size(126, 13);
            this.lblPriority.TabIndex = 4;
            this.lblPriority.Text = "Множитель приоритета";
            this.tips.SetToolTip(this.lblPriority, resources.GetString("lblPriority.ToolTip"));
            // 
            // lblScanTime
            // 
            this.lblScanTime.AutoSize = true;
            this.lblScanTime.Location = new System.Drawing.Point(17, 114);
            this.lblScanTime.Name = "lblScanTime";
            this.lblScanTime.Size = new System.Drawing.Size(135, 13);
            this.lblScanTime.TabIndex = 3;
            this.lblScanTime.Text = "Время сканирования, мс";
            this.tips.SetToolTip(this.lblScanTime, "Время, в течение которого рация прослушивает очередной канал или частоту.\r\nДейств" +
        "ует для всех каналов, кроме приоритетных.");
            // 
            // lblScanPause
            // 
            this.lblScanPause.AutoSize = true;
            this.lblScanPause.Location = new System.Drawing.Point(17, 88);
            this.lblScanPause.Name = "lblScanPause";
            this.lblScanPause.Size = new System.Drawing.Size(125, 13);
            this.lblScanPause.TabIndex = 2;
            this.lblScanPause.Text = "Пауза сканирования, с";
            this.tips.SetToolTip(this.lblScanPause, "Если выбран режим сканирования \"Пауза\", рация принимает сигнал\r\nв течение заданно" +
        "го времени, затем продолжает сканирование.");
            // 
            // lblDMRFilter
            // 
            this.lblDMRFilter.AutoSize = true;
            this.lblDMRFilter.Location = new System.Drawing.Point(17, 62);
            this.lblDMRFilter.Name = "lblDMRFilter";
            this.lblDMRFilter.Size = new System.Drawing.Size(132, 13);
            this.lblDMRFilter.TabIndex = 1;
            this.lblDMRFilter.Text = "Фильтр таймслота DMR";
            this.tips.SetToolTip(this.lblDMRFilter, "При выключенном фильтре таймслотов в режиме DMR\r\nрация переключается на прослушив" +
        "ание обоих таймслотов\r\nтолько по истечению указанного времени после последнего \r" +
        "\nприема.");
            // 
            // lblBandlimit
            // 
            this.lblBandlimit.AutoSize = true;
            this.lblBandlimit.Location = new System.Drawing.Point(17, 19);
            this.lblBandlimit.Name = "lblBandlimit";
            this.lblBandlimit.Size = new System.Drawing.Size(109, 13);
            this.lblBandlimit.TabIndex = 0;
            this.lblBandlimit.Text = "Ограничения частот";
            this.tips.SetToolTip(this.lblBandlimit, "Режим частотных ограничений для каналов и VFO");
            // 
            // tips
            // 
            this.tips.IsBalloon = true;
            // 
            // lblScanMode
            // 
            this.lblScanMode.AutoSize = true;
            this.lblScanMode.Location = new System.Drawing.Point(17, 168);
            this.lblScanMode.Name = "lblScanMode";
            this.lblScanMode.Size = new System.Drawing.Size(117, 13);
            this.lblScanMode.TabIndex = 11;
            this.lblScanMode.Text = "Режим сканирования";
            this.tips.SetToolTip(this.lblScanMode, resources.GetString("lblScanMode.ToolTip"));
            // 
            // chAutoScan
            // 
            this.chAutoScan.AutoSize = true;
            this.chAutoScan.Location = new System.Drawing.Point(20, 235);
            this.chAutoScan.Name = "chAutoScan";
            this.chAutoScan.Size = new System.Drawing.Size(185, 17);
            this.chAutoScan.TabIndex = 12;
            this.chAutoScan.Text = "Автоматическое сканирование";
            this.tips.SetToolTip(this.chAutoScan, "Если выбрано, рация автоматически переходит в режим\r\nсканирования после включения" +
        " питания.");
            this.chAutoScan.UseVisualStyleBackColor = true;
            // 
            // lblSQLVHF
            // 
            this.lblSQLVHF.AutoSize = true;
            this.lblSQLVHF.Location = new System.Drawing.Point(17, 259);
            this.lblSQLVHF.Name = "lblSQLVHF";
            this.lblSQLVHF.Size = new System.Drawing.Size(121, 13);
            this.lblSQLVHF.TabIndex = 13;
            this.lblSQLVHF.Text = "Шумоподавление (2 м)";
            this.tips.SetToolTip(this.lblSQLVHF, "Настройка шумоподавления в диапазоне двухметровых волн (144 МГц)\r\n");
            // 
            // lblSQLUHF
            // 
            this.lblSQLUHF.AutoSize = true;
            this.lblSQLUHF.Location = new System.Drawing.Point(17, 286);
            this.lblSQLUHF.Name = "lblSQLUHF";
            this.lblSQLUHF.Size = new System.Drawing.Size(133, 13);
            this.lblSQLUHF.TabIndex = 15;
            this.lblSQLUHF.Text = "Шумоподавление (70 см)";
            this.tips.SetToolTip(this.lblSQLUHF, "Настройка шумоподавления в диапазоне 70 см (433 МГц)");
            // 
            // lblSQL220
            // 
            this.lblSQL220.AutoSize = true;
            this.lblSQL220.Location = new System.Drawing.Point(17, 314);
            this.lblSQL220.Name = "lblSQL220";
            this.lblSQL220.Size = new System.Drawing.Size(146, 13);
            this.lblSQL220.TabIndex = 17;
            this.lblSQL220.Text = "Шумоподавление (220 МГц)";
            this.tips.SetToolTip(this.lblSQL220, "Шумоподавление для диапазона 220 МГц\r\nработоспособно только на TYT MD-UV380/390,\r" +
        "\nRetevis RT-3S");
            // 
            // cbPTTLatch
            // 
            this.cbPTTLatch.AutoSize = true;
            this.cbPTTLatch.Location = new System.Drawing.Point(20, 342);
            this.cbPTTLatch.Name = "cbPTTLatch";
            this.cbPTTLatch.Size = new System.Drawing.Size(133, 17);
            this.cbPTTLatch.TabIndex = 19;
            this.cbPTTLatch.Text = "Режим защелки PTT";
            this.tips.SetToolTip(this.cbPTTLatch, "Если выбрано, нажатие PTT активирует передачу\r\nбез удержания кнопки. Повторное на" +
        "жатие прерывает\r\nпередачу.\r\nВНИМАНИЕ!\r\nФункция работает только при включенном ог" +
        "раничении\r\nвремени передачи!");
            this.cbPTTLatch.UseVisualStyleBackColor = true;
            // 
            // lblPower
            // 
            this.lblPower.AutoSize = true;
            this.lblPower.Location = new System.Drawing.Point(17, 389);
            this.lblPower.Name = "lblPower";
            this.lblPower.Size = new System.Drawing.Size(97, 13);
            this.lblPower.TabIndex = 20;
            this.lblPower.Text = "Общая мощность";
            this.tips.SetToolTip(this.lblPower, resources.GetString("lblPower.ToolTip"));
            // 
            // lblPrivate
            // 
            this.lblPrivate.AutoSize = true;
            this.lblPrivate.Location = new System.Drawing.Point(17, 421);
            this.lblPrivate.Name = "lblPrivate";
            this.lblPrivate.Size = new System.Drawing.Size(107, 13);
            this.lblPrivate.TabIndex = 22;
            this.lblPrivate.Text = "Приватные вызовы";
            this.tips.SetToolTip(this.lblPrivate, "В зависимости от настройки приватные вызовы либо\r\nзапрещены, либо принимаются, ка" +
        "к обычная передача,\r\nлибо требуют подтверждения приема.");
            // 
            // cbDMRCRC
            // 
            this.cbDMRCRC.AutoSize = true;
            this.cbDMRCRC.Checked = true;
            this.cbDMRCRC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDMRCRC.Location = new System.Drawing.Point(20, 482);
            this.cbDMRCRC.Name = "cbDMRCRC";
            this.cbDMRCRC.Size = new System.Drawing.Size(228, 17);
            this.cbDMRCRC.TabIndex = 24;
            this.cbDMRCRC.Text = "Не проверять контрольную сумму DMR";
            this.tips.SetToolTip(this.cbDMRCRC, "Отключение контроля CRC может быть полезно\r\nпри работе с некоторыми репитерами и " +
        "хотспотами DMR.");
            this.cbDMRCRC.UseVisualStyleBackColor = true;
            // 
            // cb10WMode
            // 
            this.cb10WMode.AutoSize = true;
            this.cb10WMode.Location = new System.Drawing.Point(20, 366);
            this.cb10WMode.Name = "cb10WMode";
            this.cb10WMode.Size = new System.Drawing.Size(91, 17);
            this.cb10WMode.TabIndex = 25;
            this.cb10WMode.Text = "Режим 10 Вт";
            this.tips.SetToolTip(this.cb10WMode, resources.GetString("cb10WMode.ToolTip"));
            this.cb10WMode.UseVisualStyleBackColor = true;
            // 
            // cbTrackball
            // 
            this.cbTrackball.AutoSize = true;
            this.cbTrackball.Location = new System.Drawing.Point(20, 259);
            this.cbTrackball.Name = "cbTrackball";
            this.cbTrackball.Size = new System.Drawing.Size(143, 17);
            this.cbTrackball.TabIndex = 13;
            this.cbTrackball.Text = "Использовать трекбол";
            this.tips.SetToolTip(this.cbTrackball, "Настройка только для TYT MD-2017");
            this.cbTrackball.UseVisualStyleBackColor = true;
            // 
            // cbFastTrackball
            // 
            this.cbFastTrackball.AutoSize = true;
            this.cbFastTrackball.Location = new System.Drawing.Point(20, 274);
            this.cbFastTrackball.Name = "cbFastTrackball";
            this.cbFastTrackball.Size = new System.Drawing.Size(192, 17);
            this.cbFastTrackball.TabIndex = 14;
            this.cbFastTrackball.Text = "Повышенная скорость трекбола";
            this.tips.SetToolTip(this.cbFastTrackball, "Настройка только для TYT MD-2017");
            this.cbFastTrackball.UseVisualStyleBackColor = true;
            // 
            // lblDayBacklight
            // 
            this.lblDayBacklight.AutoSize = true;
            this.lblDayBacklight.Location = new System.Drawing.Point(17, 19);
            this.lblDayBacklight.Name = "lblDayBacklight";
            this.lblDayBacklight.Size = new System.Drawing.Size(96, 13);
            this.lblDayBacklight.TabIndex = 0;
            this.lblDayBacklight.Text = "Дневная яркость";
            this.tips.SetToolTip(this.lblDayBacklight, "Яркость подсветки при активной дневной теме");
            // 
            // lblNightBacklight
            // 
            this.lblNightBacklight.AutoSize = true;
            this.lblNightBacklight.Location = new System.Drawing.Point(17, 47);
            this.lblNightBacklight.Name = "lblNightBacklight";
            this.lblNightBacklight.Size = new System.Drawing.Size(88, 13);
            this.lblNightBacklight.TabIndex = 2;
            this.lblNightBacklight.Text = "Ночная яркость";
            this.tips.SetToolTip(this.lblNightBacklight, "Яркость подсветки при активной ночной теме");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Минимальная яркость";
            this.tips.SetToolTip(this.label1, "На этот режим подсветка переходит по истечению таймера.\r\nПри установке 0% подсвет" +
        "ка выключается, при установке в\r\nнесколько процентов реализуется тлеющий режим.");
            // 
            // btnReadSettings
            // 
            this.btnReadSettings.BackColor = System.Drawing.SystemColors.Control;
            this.btnReadSettings.Location = new System.Drawing.Point(363, 35);
            this.btnReadSettings.Name = "btnReadSettings";
            this.btnReadSettings.Size = new System.Drawing.Size(223, 25);
            this.btnReadSettings.TabIndex = 2;
            this.btnReadSettings.Text = "read";
            this.btnReadSettings.UseVisualStyleBackColor = false;
            this.btnReadSettings.Click += new System.EventHandler(this.btnReadSettings_Click);
            // 
            // btnWriteSettings
            // 
            this.btnWriteSettings.BackColor = System.Drawing.SystemColors.Control;
            this.btnWriteSettings.Location = new System.Drawing.Point(363, 63);
            this.btnWriteSettings.Name = "btnWriteSettings";
            this.btnWriteSettings.Size = new System.Drawing.Size(223, 25);
            this.btnWriteSettings.TabIndex = 3;
            this.btnWriteSettings.Text = "write";
            this.btnWriteSettings.UseVisualStyleBackColor = false;
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveSettings.Location = new System.Drawing.Point(363, 91);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(223, 25);
            this.btnSaveSettings.TabIndex = 4;
            this.btnSaveSettings.Text = "save";
            this.btnSaveSettings.UseVisualStyleBackColor = false;
            // 
            // btnLoadSettings
            // 
            this.btnLoadSettings.BackColor = System.Drawing.SystemColors.Control;
            this.btnLoadSettings.Location = new System.Drawing.Point(363, 119);
            this.btnLoadSettings.Name = "btnLoadSettings";
            this.btnLoadSettings.Size = new System.Drawing.Size(223, 25);
            this.btnLoadSettings.TabIndex = 5;
            this.btnLoadSettings.Text = "load";
            this.btnLoadSettings.UseVisualStyleBackColor = false;
            // 
            // rbBeiDou
            // 
            this.rbBeiDou.AutoSize = true;
            this.rbBeiDou.Checked = true;
            this.rbBeiDou.Location = new System.Drawing.Point(194, 225);
            this.rbBeiDou.Name = "rbBeiDou";
            this.rbBeiDou.Size = new System.Drawing.Size(88, 17);
            this.rbBeiDou.TabIndex = 11;
            this.rbBeiDou.TabStop = true;
            this.rbBeiDou.Text = "GPS+BeiDou";
            this.rbBeiDou.UseVisualStyleBackColor = true;
            // 
            // rbGlonass
            // 
            this.rbGlonass.AutoSize = true;
            this.rbGlonass.Location = new System.Drawing.Point(194, 242);
            this.rbGlonass.Name = "rbGlonass";
            this.rbGlonass.Size = new System.Drawing.Size(104, 17);
            this.rbGlonass.TabIndex = 12;
            this.rbGlonass.TabStop = true;
            this.rbGlonass.Text = "GPS+ГЛОНАСС";
            this.rbGlonass.UseVisualStyleBackColor = true;
            // 
            // rbHold
            // 
            this.rbHold.AutoSize = true;
            this.rbHold.Checked = true;
            this.rbHold.Location = new System.Drawing.Point(3, 5);
            this.rbHold.Name = "rbHold";
            this.rbHold.Size = new System.Drawing.Size(92, 17);
            this.rbHold.TabIndex = 12;
            this.rbHold.TabStop = true;
            this.rbHold.Text = "удерживание";
            this.rbHold.UseVisualStyleBackColor = true;
            // 
            // pnBandlimit
            // 
            this.pnBandlimit.Controls.Add(this.rbHam);
            this.pnBandlimit.Controls.Add(this.rbCPS);
            this.pnBandlimit.Location = new System.Drawing.Point(183, 15);
            this.pnBandlimit.Name = "pnBandlimit";
            this.pnBandlimit.Size = new System.Drawing.Size(139, 43);
            this.pnBandlimit.TabIndex = 6;
            // 
            // pnScanMode
            // 
            this.pnScanMode.Controls.Add(this.rbStop);
            this.pnScanMode.Controls.Add(this.rbPause);
            this.pnScanMode.Controls.Add(this.rbHold);
            this.pnScanMode.Location = new System.Drawing.Point(183, 162);
            this.pnScanMode.Name = "pnScanMode";
            this.pnScanMode.Size = new System.Drawing.Size(139, 67);
            this.pnScanMode.TabIndex = 6;
            // 
            // rbStop
            // 
            this.rbStop.AutoSize = true;
            this.rbStop.Location = new System.Drawing.Point(3, 39);
            this.rbStop.Name = "rbStop";
            this.rbStop.Size = new System.Drawing.Size(48, 17);
            this.rbStop.TabIndex = 14;
            this.rbStop.TabStop = true;
            this.rbStop.Text = "стоп";
            this.rbStop.UseVisualStyleBackColor = true;
            // 
            // rbPause
            // 
            this.rbPause.AutoSize = true;
            this.rbPause.Location = new System.Drawing.Point(3, 22);
            this.rbPause.Name = "rbPause";
            this.rbPause.Size = new System.Drawing.Size(54, 17);
            this.rbPause.TabIndex = 13;
            this.rbPause.TabStop = true;
            this.rbPause.Text = "пауза";
            this.rbPause.UseVisualStyleBackColor = true;
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tpCommons);
            this.tabs.Controls.Add(this.tpRadio);
            this.tabs.Controls.Add(this.tpDisplay);
            this.tabs.HotTrack = true;
            this.tabs.Location = new System.Drawing.Point(3, 13);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.ShowToolTips = true;
            this.tabs.Size = new System.Drawing.Size(354, 541);
            this.tabs.TabIndex = 6;
            // 
            // tpCommons
            // 
            this.tpCommons.Controls.Add(this.cmbHotspot);
            this.tpCommons.Controls.Add(this.cmbSK1Long);
            this.tpCommons.Controls.Add(this.cmbSK1);
            this.tpCommons.Controls.Add(this.cbSafeOn);
            this.tpCommons.Controls.Add(this.cmbEco);
            this.tpCommons.Controls.Add(this.lblEco);
            this.tpCommons.Controls.Add(this.cbFastTrackball);
            this.tpCommons.Controls.Add(this.cbTrackball);
            this.tpCommons.Controls.Add(this.rbGlonass);
            this.tpCommons.Controls.Add(this.lblLongPress);
            this.tpCommons.Controls.Add(this.rbBeiDou);
            this.tpCommons.Controls.Add(this.lblRepeat);
            this.tpCommons.Controls.Add(this.lblGPSMode);
            this.tpCommons.Controls.Add(this.lblAutoBlock);
            this.tpCommons.Controls.Add(this.chAutoSat);
            this.tpCommons.Controls.Add(this.lblP3);
            this.tpCommons.Controls.Add(this.chAPOReset);
            this.tpCommons.Controls.Add(this.lblP3Long);
            this.tpCommons.Controls.Add(this.lblAPO);
            this.tpCommons.Controls.Add(this.lblHotspot);
            this.tpCommons.Controls.Add(this.lblBattery);
            this.tpCommons.Location = new System.Drawing.Point(4, 22);
            this.tpCommons.Name = "tpCommons";
            this.tpCommons.Padding = new System.Windows.Forms.Padding(3);
            this.tpCommons.Size = new System.Drawing.Size(346, 515);
            this.tpCommons.TabIndex = 0;
            this.tpCommons.Text = "Общие";
            this.tpCommons.UseVisualStyleBackColor = true;
            // 
            // tpRadio
            // 
            this.tpRadio.Controls.Add(this.cb10WMode);
            this.tpRadio.Controls.Add(this.cbDMRCRC);
            this.tpRadio.Controls.Add(this.panel1);
            this.tpRadio.Controls.Add(this.lblPrivate);
            this.tpRadio.Controls.Add(this.cbPowerLevel);
            this.tpRadio.Controls.Add(this.lblPower);
            this.tpRadio.Controls.Add(this.cbPTTLatch);
            this.tpRadio.Controls.Add(this.nm220Squelch);
            this.tpRadio.Controls.Add(this.lblSQL220);
            this.tpRadio.Controls.Add(this.nmUHFSquelch);
            this.tpRadio.Controls.Add(this.lblSQLUHF);
            this.tpRadio.Controls.Add(this.nmVHFSquelch);
            this.tpRadio.Controls.Add(this.lblSQLVHF);
            this.tpRadio.Controls.Add(this.chAutoScan);
            this.tpRadio.Controls.Add(this.pnScanMode);
            this.tpRadio.Controls.Add(this.lblDMRFilter);
            this.tpRadio.Controls.Add(this.pnBandlimit);
            this.tpRadio.Controls.Add(this.lblBandlimit);
            this.tpRadio.Controls.Add(this.lblScanMode);
            this.tpRadio.Controls.Add(this.lblScanPause);
            this.tpRadio.Controls.Add(this.tbDMRFilter);
            this.tpRadio.Controls.Add(this.lblScanTime);
            this.tpRadio.Controls.Add(this.tbScanPause);
            this.tpRadio.Controls.Add(this.lblPriority);
            this.tpRadio.Controls.Add(this.tbScanTime);
            this.tpRadio.Controls.Add(this.nmPriority);
            this.tpRadio.Location = new System.Drawing.Point(4, 22);
            this.tpRadio.Name = "tpRadio";
            this.tpRadio.Padding = new System.Windows.Forms.Padding(3);
            this.tpRadio.Size = new System.Drawing.Size(346, 515);
            this.tpRadio.TabIndex = 1;
            this.tpRadio.Text = "Радиосвязь";
            this.tpRadio.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbPrivateByPTT);
            this.panel1.Controls.Add(this.rbPrivateOn);
            this.panel1.Controls.Add(this.rbPrivateOff);
            this.panel1.Location = new System.Drawing.Point(183, 415);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(139, 62);
            this.panel1.TabIndex = 23;
            // 
            // rbPrivateByPTT
            // 
            this.rbPrivateByPTT.AutoSize = true;
            this.rbPrivateByPTT.Location = new System.Drawing.Point(3, 38);
            this.rbPrivateByPTT.Name = "rbPrivateByPTT";
            this.rbPrivateByPTT.Size = new System.Drawing.Size(96, 17);
            this.rbPrivateByPTT.TabIndex = 2;
            this.rbPrivateByPTT.TabStop = true;
            this.rbPrivateByPTT.Text = "прием по PTT";
            this.rbPrivateByPTT.UseVisualStyleBackColor = true;
            // 
            // rbPrivateOn
            // 
            this.rbPrivateOn.AutoSize = true;
            this.rbPrivateOn.Checked = true;
            this.rbPrivateOn.Location = new System.Drawing.Point(3, 21);
            this.rbPrivateOn.Name = "rbPrivateOn";
            this.rbPrivateOn.Size = new System.Drawing.Size(83, 17);
            this.rbPrivateOn.TabIndex = 1;
            this.rbPrivateOn.TabStop = true;
            this.rbPrivateOn.Text = "разрешены";
            this.rbPrivateOn.UseVisualStyleBackColor = true;
            // 
            // rbPrivateOff
            // 
            this.rbPrivateOff.AutoSize = true;
            this.rbPrivateOff.Location = new System.Drawing.Point(3, 4);
            this.rbPrivateOff.Name = "rbPrivateOff";
            this.rbPrivateOff.Size = new System.Drawing.Size(84, 17);
            this.rbPrivateOff.TabIndex = 0;
            this.rbPrivateOff.Text = "запрещены";
            this.rbPrivateOff.UseVisualStyleBackColor = true;
            // 
            // cbPowerLevel
            // 
            this.cbPowerLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPowerLevel.FormattingEnabled = true;
            this.cbPowerLevel.Items.AddRange(new object[] {
            "50/100 мВт",
            "250 мВт",
            "500 мВт",
            "750 мВт",
            "1 Вт",
            "2/5 Вт",
            "3/10 Вт",
            "4/5/25 Вт",
            "5/10/45 Вт"});
            this.cbPowerLevel.Location = new System.Drawing.Point(183, 387);
            this.cbPowerLevel.Name = "cbPowerLevel";
            this.cbPowerLevel.Size = new System.Drawing.Size(121, 21);
            this.cbPowerLevel.TabIndex = 21;
            // 
            // nm220Squelch
            // 
            this.nm220Squelch.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nm220Squelch.Location = new System.Drawing.Point(183, 311);
            this.nm220Squelch.Name = "nm220Squelch";
            this.nm220Squelch.Size = new System.Drawing.Size(120, 20);
            this.nm220Squelch.TabIndex = 18;
            this.nm220Squelch.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nm220Squelch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filterNumerics);
            this.nm220Squelch.Leave += new System.EventHandler(this.nm220Squelch_Leave);
            // 
            // nmUHFSquelch
            // 
            this.nmUHFSquelch.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nmUHFSquelch.Location = new System.Drawing.Point(183, 284);
            this.nmUHFSquelch.Name = "nmUHFSquelch";
            this.nmUHFSquelch.Size = new System.Drawing.Size(120, 20);
            this.nmUHFSquelch.TabIndex = 16;
            this.nmUHFSquelch.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nmUHFSquelch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filterNumerics);
            this.nmUHFSquelch.Leave += new System.EventHandler(this.nmUHFSquelch_Leave);
            // 
            // nmVHFSquelch
            // 
            this.nmVHFSquelch.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nmVHFSquelch.Location = new System.Drawing.Point(183, 257);
            this.nmVHFSquelch.Name = "nmVHFSquelch";
            this.nmVHFSquelch.Size = new System.Drawing.Size(120, 20);
            this.nmVHFSquelch.TabIndex = 14;
            this.nmVHFSquelch.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nmVHFSquelch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filterNumerics);
            this.nmVHFSquelch.Leave += new System.EventHandler(this.nmVHFSquelch_Leave);
            // 
            // tpDisplay
            // 
            this.tpDisplay.Controls.Add(this.nmMinBacklight);
            this.tpDisplay.Controls.Add(this.label1);
            this.tpDisplay.Controls.Add(this.nmNightBacklight);
            this.tpDisplay.Controls.Add(this.lblNightBacklight);
            this.tpDisplay.Controls.Add(this.nmDayBacklight);
            this.tpDisplay.Controls.Add(this.lblDayBacklight);
            this.tpDisplay.Location = new System.Drawing.Point(4, 22);
            this.tpDisplay.Name = "tpDisplay";
            this.tpDisplay.Size = new System.Drawing.Size(346, 515);
            this.tpDisplay.TabIndex = 2;
            this.tpDisplay.Text = "Экран";
            this.tpDisplay.UseVisualStyleBackColor = true;
            // 
            // nmMinBacklight
            // 
            this.nmMinBacklight.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nmMinBacklight.Location = new System.Drawing.Point(183, 70);
            this.nmMinBacklight.Name = "nmMinBacklight";
            this.nmMinBacklight.Size = new System.Drawing.Size(120, 20);
            this.nmMinBacklight.TabIndex = 5;
            this.nmMinBacklight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filterNumerics);
            this.nmMinBacklight.Leave += new System.EventHandler(this.nmMinBacklight_Leave);
            // 
            // nmNightBacklight
            // 
            this.nmNightBacklight.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nmNightBacklight.Location = new System.Drawing.Point(183, 45);
            this.nmNightBacklight.Name = "nmNightBacklight";
            this.nmNightBacklight.Size = new System.Drawing.Size(120, 20);
            this.nmNightBacklight.TabIndex = 3;
            this.nmNightBacklight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nmNightBacklight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filterNumerics);
            this.nmNightBacklight.Leave += new System.EventHandler(this.nmNightBacklight_Leave);
            // 
            // nmDayBacklight
            // 
            this.nmDayBacklight.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nmDayBacklight.Location = new System.Drawing.Point(183, 19);
            this.nmDayBacklight.Name = "nmDayBacklight";
            this.nmDayBacklight.Size = new System.Drawing.Size(120, 20);
            this.nmDayBacklight.TabIndex = 1;
            this.nmDayBacklight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nmDayBacklight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filterNumerics);
            this.nmDayBacklight.Leave += new System.EventHandler(this.nmDayBacklight_Leave);
            // 
            // lblEco
            // 
            this.lblEco.AutoSize = true;
            this.lblEco.Location = new System.Drawing.Point(17, 298);
            this.lblEco.Name = "lblEco";
            this.lblEco.Size = new System.Drawing.Size(76, 13);
            this.lblEco.TabIndex = 16;
            this.lblEco.Text = "Экономайзер";
            this.tips.SetToolTip(this.lblEco, "Работает только на портативных рациях. Задает соотношение \r\nвремени активности и " +
        "неактивности приемника.");
            // 
            // cmbEco
            // 
            this.cmbEco.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEco.FormattingEnabled = true;
            this.cmbEco.Items.AddRange(new object[] {
            "выкл.",
            "1:1",
            "1:2",
            "1:3",
            "1:4",
            "1:5"});
            this.cmbEco.Location = new System.Drawing.Point(194, 296);
            this.cmbEco.Name = "cmbEco";
            this.cmbEco.Size = new System.Drawing.Size(121, 21);
            this.cmbEco.TabIndex = 17;
            // 
            // cbSafeOn
            // 
            this.cbSafeOn.AutoSize = true;
            this.cbSafeOn.Location = new System.Drawing.Point(20, 324);
            this.cbSafeOn.Name = "cbSafeOn";
            this.cbSafeOn.Size = new System.Drawing.Size(145, 17);
            this.cbSafeOn.TabIndex = 18;
            this.cbSafeOn.Text = "Безопасное включение";
            this.tips.SetToolTip(this.cbSafeOn, resources.GetString("cbSafeOn.ToolTip"));
            this.cbSafeOn.UseVisualStyleBackColor = true;
            // 
            // cmbSK1
            // 
            this.cmbSK1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSK1.FormattingEnabled = true;
            this.cmbSK1.Items.AddRange(new object[] {
            "информация/выкл",
            "реверс частот",
            "прямая связь",
            "быстрый канал",
            "отключение фильтров"});
            this.cmbSK1.Location = new System.Drawing.Point(194, 78);
            this.cmbSK1.Name = "cmbSK1";
            this.cmbSK1.Size = new System.Drawing.Size(121, 21);
            this.cmbSK1.TabIndex = 19;
            // 
            // cmbSK1Long
            // 
            this.cmbSK1Long.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSK1Long.FormattingEnabled = true;
            this.cmbSK1Long.Items.AddRange(new object[] {
            "информация/выкл",
            "реверс частот",
            "прямая связь",
            "быстрый канал",
            "отключение фильтров"});
            this.cmbSK1Long.Location = new System.Drawing.Point(194, 101);
            this.cmbSK1Long.Name = "cmbSK1Long";
            this.cmbSK1Long.Size = new System.Drawing.Size(121, 21);
            this.cmbSK1Long.TabIndex = 20;
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.SystemColors.Control;
            this.btnReset.Location = new System.Drawing.Point(363, 164);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(223, 25);
            this.btnReset.TabIndex = 7;
            this.btnReset.Text = "button1";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // lblWarning
            // 
            this.lblWarning.Font = new System.Drawing.Font("Microsoft Tai Le", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblWarning.ForeColor = System.Drawing.Color.Red;
            this.lblWarning.Location = new System.Drawing.Point(365, 199);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(220, 163);
            this.lblWarning.TabIndex = 8;
            this.lblWarning.Text = "Настройки рации при нажатии этой кнопки будут сброшены на значения по умолчанию, " +
    "жестко заданные прошивкой.";
            // 
            // cmbHotspot
            // 
            this.cmbHotspot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHotspot.FormattingEnabled = true;
            this.cmbHotspot.Items.AddRange(new object[] {
            "выкл",
            "MMDVM",
            "BlueDV"});
            this.cmbHotspot.Location = new System.Drawing.Point(194, 124);
            this.cmbHotspot.Name = "cmbHotspot";
            this.cmbHotspot.Size = new System.Drawing.Size(121, 21);
            this.cmbHotspot.TabIndex = 21;
            // 
            // CodeplugSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(607, 565);
            this.Controls.Add(this.lblWarning);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.btnLoadSettings);
            this.Controls.Add(this.btnSaveSettings);
            this.Controls.Add(this.btnWriteSettings);
            this.Controls.Add(this.btnReadSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "CodeplugSettingsForm";
            this.Text = "CodeplugSettingsForm";
            this.Load += new System.EventHandler(this.CodeplugSettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nmPriority)).EndInit();
            this.pnBandlimit.ResumeLayout(false);
            this.pnBandlimit.PerformLayout();
            this.pnScanMode.ResumeLayout(false);
            this.pnScanMode.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.tpCommons.ResumeLayout(false);
            this.tpCommons.PerformLayout();
            this.tpRadio.ResumeLayout(false);
            this.tpRadio.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nm220Squelch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmUHFSquelch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmVHFSquelch)).EndInit();
            this.tpDisplay.ResumeLayout(false);
            this.tpDisplay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmMinBacklight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmNightBacklight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmDayBacklight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
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
        private System.Windows.Forms.NumericUpDown nmPriority;
        private System.Windows.Forms.TextBox tbScanTime;
        private System.Windows.Forms.TextBox tbScanPause;
        private System.Windows.Forms.TextBox tbDMRFilter;
        private System.Windows.Forms.RadioButton rbHam;
        private System.Windows.Forms.RadioButton rbCPS;
        private System.Windows.Forms.RadioButton rbGlonass;
        private System.Windows.Forms.RadioButton rbBeiDou;
        private System.Windows.Forms.Label lblScanMode;
        private System.Windows.Forms.RadioButton rbHold;
        private System.Windows.Forms.Panel pnBandlimit;
        private System.Windows.Forms.RadioButton rbPause;
        private System.Windows.Forms.Panel pnScanMode;
        private System.Windows.Forms.RadioButton rbStop;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tpCommons;
        private System.Windows.Forms.TabPage tpRadio;
        private System.Windows.Forms.CheckBox chAutoScan;
        private System.Windows.Forms.NumericUpDown nmVHFSquelch;
        private System.Windows.Forms.Label lblSQLVHF;
        private System.Windows.Forms.NumericUpDown nmUHFSquelch;
        private System.Windows.Forms.Label lblSQLUHF;
        private System.Windows.Forms.NumericUpDown nm220Squelch;
        private System.Windows.Forms.Label lblSQL220;
        private System.Windows.Forms.CheckBox cbPTTLatch;
        private System.Windows.Forms.ComboBox cbPowerLevel;
        private System.Windows.Forms.Label lblPower;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbPrivateOn;
        private System.Windows.Forms.RadioButton rbPrivateOff;
        private System.Windows.Forms.Label lblPrivate;
        private System.Windows.Forms.RadioButton rbPrivateByPTT;
        private System.Windows.Forms.CheckBox cbDMRCRC;
        private System.Windows.Forms.CheckBox cb10WMode;
        private System.Windows.Forms.CheckBox cbTrackball;
        private System.Windows.Forms.CheckBox cbFastTrackball;
        private System.Windows.Forms.TabPage tpDisplay;
        private System.Windows.Forms.Label lblDayBacklight;
        private System.Windows.Forms.NumericUpDown nmDayBacklight;
        private System.Windows.Forms.NumericUpDown nmNightBacklight;
        private System.Windows.Forms.Label lblNightBacklight;
        private System.Windows.Forms.NumericUpDown nmMinBacklight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblEco;
        private System.Windows.Forms.ComboBox cmbEco;
        private System.Windows.Forms.CheckBox cbSafeOn;
        private System.Windows.Forms.ComboBox cmbSK1;
        private System.Windows.Forms.ComboBox cmbSK1Long;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.ComboBox cmbHotspot;
    }
}