using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml.Linq;
using WeifenLuo.WinFormsUI.Docking;
using static DMR.OpenGD77Form;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DMR
{
    public partial class CodeplugSettingsForm : DockContent//, IDisp, ISingleRow
    {
        private SerialPort commPort;
        public static Dictionary<string, string> OpenGD77StringsDict = new Dictionary<string, string>();
        private BackgroundWorker worker;


        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 64)]
        public struct RadioSettings
        {
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.U4)]
            public uint magicNumber;

            [FieldOffset(4)]
            [MarshalAs(UnmanagedType.U1)]
            public byte timezone;

            [FieldOffset(5)]
            [MarshalAs(UnmanagedType.U1)]
            public byte beepOptions;

            [FieldOffset(6)]
            [MarshalAs(UnmanagedType.U4)]
            public uint bitfieldOptions;

            [FieldOffset(10)]
            [MarshalAs(UnmanagedType.U4)]
            public uint aprsBeaconingSettingsPart1;

            [FieldOffset(14)]
            [MarshalAs(UnmanagedType.U4)]
            public uint aprsBeaconingSettingsPart2;

            [FieldOffset(18)]
            [MarshalAs(UnmanagedType.U2)]
            public ushort aprsBeaconingSettingsPart3;

            [FieldOffset(20)]
            [MarshalAs(UnmanagedType.U1)]
            public byte txPowerLevel;

            [FieldOffset(21)]
            [MarshalAs(UnmanagedType.U1)]
            public byte txTimeoutBeepX5Secs;

            [FieldOffset(22)]
            [MarshalAs(UnmanagedType.U1)]
            public byte beepVolumeDivider;

            [FieldOffset(23)]
            [MarshalAs(UnmanagedType.U1)]
            public byte micGainDMR;

            [FieldOffset(24)]
            [MarshalAs(UnmanagedType.U1)]
            public byte micGainFM;

            [FieldOffset(25)]
            [MarshalAs(UnmanagedType.U1)]
            public byte backlightMode;

            [FieldOffset(26)]
            [MarshalAs(UnmanagedType.U1)]
            public byte backLightTimeout;

            [FieldOffset(27)]
            [MarshalAs(UnmanagedType.U1)]
            public byte displayContrast;

            [FieldOffset(28)]
            [MarshalAs(UnmanagedType.U1)]
            public byte displayBacklightPercentageDay;

            [FieldOffset(29)]
            [MarshalAs(UnmanagedType.U1)]
            public byte displayBacklightPercentageNight;

            [FieldOffset(30)]
            [MarshalAs(UnmanagedType.U1)]
            public byte displayBacklightPercentageOff;

            [FieldOffset(31)]
            [MarshalAs(UnmanagedType.U1)]
            public byte extendedInfosOnScreen;

            [FieldOffset(32)]
            [MarshalAs(UnmanagedType.U1)]
            public byte scanModePause;

            [FieldOffset(33)]
            [MarshalAs(UnmanagedType.U1)]
            public byte scanDelay;

            [FieldOffset(34)]
            [MarshalAs(UnmanagedType.U1)]
            public byte DMR_RxAGC;

            [FieldOffset(35)]
            [MarshalAs(UnmanagedType.U1)]
            public byte hotspotType;

            [FieldOffset(36)]
            [MarshalAs(UnmanagedType.U1)]
            public byte scanStepTime;

            [FieldOffset(37)]
            [MarshalAs(UnmanagedType.U1)]
            public byte dmrCaptureTimeout;

            [FieldOffset(38)]
            [MarshalAs(UnmanagedType.U1)]
            public byte privateCalls;

            [FieldOffset(39)]
            [MarshalAs(UnmanagedType.U1)]
            public byte contactDisplayPriority;

            [FieldOffset(40)]
            [MarshalAs(UnmanagedType.U1)]
            public byte splitContact;

            [FieldOffset(41)]
            [MarshalAs(UnmanagedType.U1)]
            public byte voxThreshold; // 0: disabled

            [FieldOffset(42)]
            [MarshalAs(UnmanagedType.U1)]
            public byte voxTailUnits; // 500ms units

            [FieldOffset(43)]
            [MarshalAs(UnmanagedType.U1)]
            public byte audioPromptMode;

            [FieldOffset(44)]
            [MarshalAs(UnmanagedType.U1)]
            public byte batteryCalibration; // Units of 0.01V (NOTE: only the 4 lower bits are used)

            [FieldOffset(45)]
            [MarshalAs(UnmanagedType.U1)]
            public byte squelchDefaultVHF; // VHF, 200 and UHF

            [FieldOffset(46)]
            [MarshalAs(UnmanagedType.U1)]
            public byte squelchDefaultUHF; // VHF, 200 and UHF

            [FieldOffset(47)]
            [MarshalAs(UnmanagedType.U1)]
            public byte squelchDefault220; // VHF, 200 and UHF

            [FieldOffset(48)]
            [MarshalAs(UnmanagedType.U1)]
            public byte ecoLevel;// Power saving / economy level

            [FieldOffset(49)]
            [MarshalAs(UnmanagedType.U1)]
            public byte apo; // unit: 30 minutes (5 is skipped, as we want 0, 30, 60, 90, 120 and 180)

            [FieldOffset(50)]
            [MarshalAs(UnmanagedType.U1)]
            public byte keypadTimerLong;

            [FieldOffset(51)]
            [MarshalAs(UnmanagedType.U1)]
            public byte keypadTimerRepeat;

            [FieldOffset(52)]
            [MarshalAs(UnmanagedType.U1)]
            public byte autolockTimer; // in minutes

            [FieldOffset(53)]
            [MarshalAs(UnmanagedType.U1)]
            public byte buttonP3; //Режим работы кнопки P3

            [FieldOffset(54)]
            [MarshalAs(UnmanagedType.U1)]
            public byte buttonP3Long;

            [FieldOffset(55)]
            [MarshalAs(UnmanagedType.U1)]
            public byte scanPriority;

            [FieldOffset(56)]
            [MarshalAs(UnmanagedType.U1)]
            public byte txFreqLimited;

            [FieldOffset(57)]
            [MarshalAs(UnmanagedType.U1)]
            public byte reserve1;

            [FieldOffset(58)]
            [MarshalAs(UnmanagedType.U2)]
            public ushort reserve2;

            [FieldOffset(60)]
            [MarshalAs(UnmanagedType.U4)]
            public uint reserve3;
        }

        public enum SettingBits
        {
            BIT_INVERSE_VIDEO,
            BIT_PTT_LATCH,
            BIT_UNUSED_2,
            BIT_BATTERY_VOLTAGE_IN_HEADER,
            BIT_SETTINGS_UPDATED,
            BIT_TX_RX_FREQ_LOCK,
            BIT_ALL_LEDS_DISABLED,
            BIT_SCAN_ON_BOOT_ENABLED,
            BIT_POWEROFF_SUSPEND,
            BIT_SATELLITE_MANUAL_AUTO,
            BIT_UNUSED_1,
	        BIT_SPEAKER_CLICK_SUPPRESS,
            BIT_DMR_CRC_IGNORED,
            BIT_APO_WITH_RF,
            BIT_SAFE_POWER_ON,
            BIT_AUTO_NIGHT,
            BIT_AUTO_NIGHT_OVERRIDE,
            BIT_AUTO_NIGHT_DAYTIME,
	        BIT_VISUAL_VOLUME,
            BIT_SECONDARY_LANGUAGE,
            BIT_SORT_CHANNEL_DISTANCE,
            BIT_DISPLAY_CHANNEL_DISTANCE,
	        BIT_TRACKBALL_ENABLED,
	        BIT_TRACKBALL_FAST_MOTION,
	        BIT_FORCE_10W_RADIO,
            BIT_GPS_MODULE_CUSTOM
        }

        public bool checkOptionBit(uint settings, byte bit)
        {
            return (((settings >> bit) & 1) != 0);
        }
        
            public CodeplugSettingsForm()
        {
            InitializeComponent();
            base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Scale(Settings.smethod_6());
            Settings.ReadCommonsForSectionIntoDictionary(OpenGD77StringsDict, "OpenGD77Form");
            cbPowerLevel.SelectedIndex = 4;
            cmbHotspot.SelectedIndex = 0;
            cmbSK1.SelectedIndex = 0;
            cmbSK1Long.SelectedIndex = 0;
            cmbEco.SelectedIndex = 0;
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
        private bool setupCommPort()
        {
            if (commPort != null)
            {
                try
                {
                    if (commPort.IsOpen)
                    {
                        commPort.Close();
                    }
                }
                catch (Exception)
                {
                }
                commPort = null;
            }
            try
            {
                string text = null;
                text = SetupDiWrap.ComPortNameFromFriendlyNamePrefix("OpenGD77");
                if (text == null)
                {
                    CommPortSelector commPortSelector = new CommPortSelector();
                    if (DialogResult.OK != commPortSelector.ShowDialog())
                    {
                        return false;
                    }
                    text = SetupDiWrap.ComPortNameFromFriendlyNamePrefix(commPortSelector.SelectedPort);
                    IniFileUtils.WriteProfileString("Setup", "LastCommPort", text);
                }
                if (text == null)
                {
                    MessageBox.Show(OpenGD77StringsDict["Please_connect_the_radio,_and_try_again."], OpenGD77StringsDict["Radio_not_detected."]);
                }
                else
                {
                    commPort = new SerialPort(text, 115200, Parity.None, 8, StopBits.One);
                    commPort.ReadTimeout = 1000;
                }
            }
            catch (Exception)
            {
                commPort = null;
                SystemSounds.Hand.Play();
                MessageBox.Show(OpenGD77StringsDict["Failed_to_open_comm_port"], OpenGD77StringsDict["Error"]);
                IniFileUtils.WriteProfileString("Setup", "LastCommPort", "");
                return false;
            }
            try
            {
                commPort.Open();
            }
            catch (Exception)
            {
                SystemSounds.Hand.Play();
                MessageBox.Show(OpenGD77StringsDict["Comm_port_not_available"]);
                commPort = null;
                return false;
            }
            return true;
        }

        private bool probeRadioModel(bool stealth = false)
        {
            if (!setupCommPort())
            {
                SystemSounds.Hand.Play();
                MessageBox.Show(OpenGD77StringsDict["No_com_port"]);
                return false;
            }
            OpenGD77Form.sendCommand(commPort, 254);
            commPort.DiscardInBuffer();
            if (!OpenGD77Form.sendCommand(commPort, stealth ? 254 : 0))
            {
                MessageBox.Show(OpenGD77StringsDict["Error_connecting_to_the_radio"]);
                commPort = null;
                return false;
            }
            MainForm.RadioInfo = OpenGD77Form.readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort, stealth);
            if (MainForm.RadioInfo.identifier == "RUSSIAN")
            {
                
                if (!stealth)
                {
                    OpenGD77Form.sendCommand(commPort, 5);
                }
                commPort.Close();
                commPort = null;
                return true;
            }
            else
            {
                commPort.Close();
                commPort = null;
                MessageBox.Show(OpenGD77Form.StringsDict["Incorrect_firmware"], OpenGD77Form.StringsDict["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool ReadData(SerialPort port, OpenGD77CommsTransferData dataObj)
        {
            byte[] array = new byte[1032];
            byte[] array2 = new byte[1032];
            int num = 0;
            array[0] = 82;
            array[1] = (byte)dataObj.mode;
            array[2] = 0;
            array[3] = 0;
            array[4] = 0;
            array[5] = 0;
            array[6] = 0;
            array[7] = 0;
            port.Write(array, 0, 8);
            while (port.BytesToWrite > 0)
            {
                Thread.Sleep(1);
            }
            while (port.BytesToRead == 0)
            {
                Thread.Sleep(5);
            }
            port.Read(array2, 0, port.BytesToRead);
            if (array2[0] == 82)
            {
                int num2 = (array2[1] << 8) + array2[2];
                for (int i = 0; i < num2; i++)
                {
                    dataObj.dataBuff[num++] = array2[i + 3];
                }
                return true;
            }
            //Console.WriteLine($"read stopped (error at {0:X8})");
            return false;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            OpenGD77CommsTransferData openGD77CommsTransferData = e.Argument as OpenGD77CommsTransferData;
            if (commPort == null)
            {
                SystemSounds.Hand.Play();
                MessageBox.Show(OpenGD77StringsDict["No_com_port"]);
                return;
            }
            try
            {
                switch (openGD77CommsTransferData.action)
                {
                    case OpenGD77CommsTransferData.CommsAction.READ_SETTINGS:
                        if (!OpenGD77Form.sendCommand(commPort, 0))
                        {
                            MessageBox.Show(OpenGD77StringsDict["Error_connecting_to_the_radio"]);
                            openGD77CommsTransferData.responseCode = 1;
                            commPort = null;
                            break;
                        }
                        MainForm.RadioInfo = OpenGD77Form.readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
                        OpenGD77Form.sendCommand(commPort, 1);
                        OpenGD77Form.sendCommand(commPort, 2, 0, 0, 3, 1, 0, OpenGD77StringsDict["RADIO_DISPLAY_CPS"]);
                        OpenGD77Form.sendCommand(commPort, 2, 0, 16, 3, 1, 0, OpenGD77StringsDict["RADIO_DISPLAY_Reading"]);
                        OpenGD77Form.sendCommand(commPort, 2, 0, 32, 3, 1, 0, OpenGD77StringsDict["RADIO_DISPLAY_Settings"]);
                        OpenGD77Form.sendCommand(commPort, 3);
                        OpenGD77Form.sendCommand(commPort, 6, 3);
                        openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadSettings;
                        openGD77CommsTransferData.dataBuff = new byte[64];
                        openGD77CommsTransferData.localDataBufferStartPosition = 0;
                        openGD77CommsTransferData.transferLength = openGD77CommsTransferData.dataBuff.Length;
                        if (!ReadData(commPort, openGD77CommsTransferData))
                        {
                            MessageBox.Show(OpenGD77StringsDict["Error_while_reading"]);
                            openGD77CommsTransferData.responseCode = 1;
                            commPort = null;
                        }
                        else
                        {
                            OpenGD77Form.sendCommand(commPort, 5);
                            OpenGD77Form.sendCommand(commPort, 7);
                            commPort.Close();
                            commPort = null;
                            File.WriteAllBytes("test.bin", openGD77CommsTransferData.dataBuff);
                        }
                        break;
                    case OpenGD77CommsTransferData.CommsAction.WRITE_SETTINGS:
                        if (!OpenGD77Form.sendCommand(commPort, 0))
                        {
                            MessageBox.Show(OpenGD77StringsDict["Error_connecting_to_the_radio"]);
                            openGD77CommsTransferData.responseCode = 1;
                            commPort = null;
                            break;
                        }
                        OpenGD77Form.readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
                        OpenGD77Form.sendCommand(commPort, 1);
                        OpenGD77Form.sendCommand(commPort, 2, 0, 0, 3, 1, 0, OpenGD77StringsDict["RADIO_DISPLAY_CPS"]);
                        OpenGD77Form.sendCommand(commPort, 2, 0, 16, 3, 1, 0, OpenGD77StringsDict["RADIO_DISPLAY_Writing"]);
                        OpenGD77Form.sendCommand(commPort, 2, 0, 32, 3, 1, 0, OpenGD77StringsDict["RADIO_DISPLAY_Settings"]);
                        OpenGD77Form.sendCommand(commPort, 3);
                        OpenGD77Form.sendCommand(commPort, 6, 4);
                        openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteFlash;
                        openGD77CommsTransferData.localDataBufferStartPosition = 0;
                        openGD77CommsTransferData.transferLength = openGD77CommsTransferData.dataBuff.Length;
                        openGD77CommsTransferData.startDataAddressInTheRadio = OpenGD77Form.STM32_FLASH_ADDRESS_OFFSET;
                        if (MainForm.RadioInfo.radioType == 8)
                        {
                            lock (sender)
                            {
                                if (!OpenGD77Form.convertThemeColours565(ref openGD77CommsTransferData.dataBuff, OpenGD77Form.CUSTOM_DATA_HEADER_SIZE, checkForCustomDataHeader: false))
                                {
                                    MessageBox.Show(OpenGD77StringsDict["Colour_Conversion_Error"]);
                                }
                            }
                        }

                            SystemSounds.Hand.Play();
                            MessageBox.Show(OpenGD77StringsDict["Error_while_writing"]);
                            openGD77CommsTransferData.responseCode = 1;

                        OpenGD77Form.sendCommand(commPort, 6, 1);
                        commPort.Close();
                        commPort = null;
                        break;
                }
            }
            catch (Exception ex)
            {
                SystemSounds.Hand.Play();
                MessageBox.Show(ex.Message);
            }
            e.Result = openGD77CommsTransferData;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(e.Result is OpenGD77CommsTransferData openGD77CommsTransferData))
            {
                return;
            }
            if (openGD77CommsTransferData.action != 0)
            {
                if (openGD77CommsTransferData.responseCode == 0)
                {
                    switch (openGD77CommsTransferData.action)
                    {
                        case OpenGD77CommsTransferData.CommsAction.READ_SETTINGS:
                            openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
                            RadioSettings radioSettings = ByteArrayToRadioSettings(openGD77CommsTransferData.dataBuff);
                            cmbHotspot.SelectedIndex = radioSettings.hotspotType;
                            cmbSK1.SelectedIndex = radioSettings.buttonP3;
                            cmbSK1Long.SelectedIndex = radioSettings.buttonP3Long;
                            cbTrackball.Checked = checkOptionBit(radioSettings.bitfieldOptions, (byte)SettingBits.BIT_TRACKBALL_ENABLED);
                            cbFastTrackball.Checked = checkOptionBit(radioSettings.bitfieldOptions, (byte)SettingBits.BIT_TRACKBALL_FAST_MOTION);
                            chAPOReset.Checked = checkOptionBit(radioSettings.bitfieldOptions, (byte)SettingBits.BIT_APO_WITH_RF);
                            chAutoSat.Checked = checkOptionBit(radioSettings.bitfieldOptions, (byte)SettingBits.BIT_SATELLITE_MANUAL_AUTO);
                            rbGlonass.Checked = checkOptionBit(radioSettings.bitfieldOptions, (byte)SettingBits.BIT_GPS_MODULE_CUSTOM);
                            rbBeiDou.Checked = !rbGlonass.Checked;
                            cmbEco.SelectedIndex = radioSettings.ecoLevel;
                            cbSafeOn.Checked = checkOptionBit(radioSettings.bitfieldOptions, (byte)SettingBits.BIT_SAFE_POWER_ON);
                            nmPriority.Value = radioSettings.scanPriority;
                            tbDMRFilter.Text = radioSettings.dmrCaptureTimeout.ToString();
                            tbScanTime.Text = (radioSettings.scanStepTime * 30 + 30).ToString();
                            tbScanPause.Text = radioSettings.scanDelay.ToString();
                            rbCPS.Checked = (radioSettings.txFreqLimited != 0);
                            rbHam.Checked = !rbCPS.Checked;
                            switch (radioSettings.scanModePause)
                            {
                                case 0:
                                    rbHold.Checked = true;
                                    rbPause.Checked = false;
                                    rbStop.Checked = false;
                                    break;
                                case 1:
                                    rbHold.Checked = false;
                                    rbPause.Checked = true;
                                    rbStop.Checked = false;
                                    break;
                                case 2:
                                    rbHold.Checked = false;
                                    rbPause.Checked = false;
                                    rbStop.Checked = true;
                                    break;
                                default:
                                    radioSettings.scanModePause = 0;
                                    rbHold.Checked = true;
                                    rbPause.Checked = false;
                                    rbStop.Checked = false;
                                    break;
                            }
                            chAutoScan.Checked = checkOptionBit(radioSettings.bitfieldOptions, (byte)SettingBits.BIT_SCAN_ON_BOOT_ENABLED);
                            nmVHFSquelch.Value = (radioSettings.squelchDefaultVHF - 1) * 5;
                            nmUHFSquelch.Value = (radioSettings.squelchDefaultUHF - 1) * 5;
                            nm220Squelch.Value = (radioSettings.squelchDefault220 - 1) * 5;
                            cbPTTLatch.Checked = checkOptionBit(radioSettings.bitfieldOptions, (byte)SettingBits.BIT_PTT_LATCH);
                            cbPowerLevel.SelectedIndex = radioSettings.txPowerLevel;
                            switch (radioSettings.privateCalls)
                            {
                                case 0:
                                    rbPrivateOff.Checked = true;
                                    rbPrivateOn.Checked = false;
                                    rbPrivateByPTT.Checked = false;
                                    break;
                                case 1:
                                    rbPrivateOff.Checked = false;
                                    rbPrivateOn.Checked = true;
                                    rbPrivateByPTT.Checked = false;
                                    break;
                                case 2:
                                    rbPrivateOff.Checked = false;
                                    rbPrivateOn.Checked = false;
                                    rbPrivateByPTT.Checked = true;
                                    break;
                                default:
                                    radioSettings.privateCalls = 1;
                                    rbPrivateOff.Checked = false;
                                    rbPrivateOn.Checked = true;
                                    rbPrivateByPTT.Checked = false;
                                    break;
                            }
                            cbDMRCRC.Checked = checkOptionBit(radioSettings.bitfieldOptions, (byte)SettingBits.BIT_DMR_CRC_IGNORED);
                            cb10WMode.Checked = checkOptionBit(radioSettings.bitfieldOptions, (byte)SettingBits.BIT_FORCE_10W_RADIO);
                            nmDayBacklight.Value = radioSettings.displayBacklightPercentageDay;
                            nmNightBacklight.Value = radioSettings.displayBacklightPercentageNight;
                            nmMinBacklight.Value = radioSettings.displayBacklightPercentageOff;
                            break;
                        case OpenGD77CommsTransferData.CommsAction.WRITE_SETTINGS:
                            openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
                            break;
                    }
                }
                else
                {
                    SystemSounds.Hand.Play();
                    MessageBox.Show(OpenGD77StringsDict["There_has_been_an_error._Refer_to_the_last_status_message_that_was_displayed"], OpenGD77StringsDict["Oops"], MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
        }

        private void perFormCommsTask(OpenGD77CommsTransferData dataObj)
        {
            try
            {
                worker = new BackgroundWorker();
                worker.DoWork += worker_DoWork;
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                worker.RunWorkerAsync(dataObj);
            }
            catch (Exception ex)
            {
                SystemSounds.Hand.Play();
                MessageBox.Show(ex.Message);
            }
        }

        private static RadioSettings ByteArrayToRadioSettings(byte[] bytes)
        {
            GCHandle gCHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (RadioSettings)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(RadioSettings));
            }
            finally
            {
                gCHandle.Free();
            }
        }

        private void btnReadSettings_Click(object sender, EventArgs e)
        {

            if (probeRadioModel())
            {
                if (!setupCommPort())
                {
                    SystemSounds.Hand.Play();
                    MessageBox.Show(OpenGD77StringsDict["No_com_port"]);
                    return;
                }
                if (MainForm.RadioInfo.radioType != 5 && MainForm.RadioInfo.radioType != 6 && MainForm.RadioInfo.radioType != 8 && MainForm.RadioInfo.radioType != 10 && MainForm.RadioInfo.radioType != 9 && MainForm.RadioInfo.radioType != 106)
                {
                    SystemSounds.Hand.Play();
                    MessageBox.Show("Тип рации не поддерживается!", OpenGD77StringsDict["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!OpenGD77Form.RadioInfoIsFeatureSet(OpenGD77Form.RadioInfoFeatures.SUPPORT_SETTINGS_ACCESS))
                {
                    SystemSounds.Hand.Play();
                    MessageBox.Show("В рации установлена устаревшая прошивка, не поддерживающая чтение и запись настроек через CPS!\r\nВерсия прошивки: " + MainForm.RadioInfo.buildDateTime + "\r\nНеобходимая версия прошивки: не ниже 20250120", OpenGD77StringsDict["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData(OpenGD77CommsTransferData.CommsAction.READ_SETTINGS);
                openGD77CommsTransferData.dataBuff = new byte[Settings.ADDR_OPENGD77_CUSTOM_DATA_END - Settings.ADDR_OPENGD77_CUSTOM_DATA_START];
                perFormCommsTask(openGD77CommsTransferData);
                SystemSounds.Exclamation.Play();
                
            }

        }

        private void filterNumerics(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }



        private void tbDMRFilter_Leave(object sender, EventArgs e)
        {
            int value = Int32.Parse(tbDMRFilter.Text);
            if (value < 2)
                value = 2;
            else if (value > 90)
                value = 90;
            tbDMRFilter.Text = value.ToString();
        }

        private void tbScanTime_Leave(object sender, EventArgs e)
        {
            int value = Int32.Parse(tbScanTime.Text);
            if (value < 30)
                value = 30;
            else if (value > 480)
                value = 480;
            value -= 30;
            value = (int)Math.Round(value / 30.0f) * 30 + 30;
            tbScanTime.Text = value.ToString();
        }

        private void tbScanPause_Leave(object sender, EventArgs e)
        {
            int value = Int32.Parse(tbScanPause.Text);
            if (value < 1)
                value = 1;
            else if (value > 30)
                value = 30;
            tbScanPause.Text = value.ToString();
        }

        private void nmPriority_Leave(object sender, EventArgs e)
        {
            int value = (int)nmPriority.Value;
            if (value < 2)
                value = 2;
            else if (value > 10)
                value = 10;
            nmPriority.Value = value;
        }

        private void nmVHFSquelch_Leave(object sender, EventArgs e)
        {
            int value = (int)nmVHFSquelch.Value;
            if (value < 0)
                value = 0;
            else if (value > 100)
                value = 100;
            value = (int)Math.Round(value / 5.0f) * 5;
            nmVHFSquelch.Value = value;
        }

        private void nmUHFSquelch_Leave(object sender, EventArgs e)
        {
            int value = (int)nmUHFSquelch.Value;
            if (value < 0)
                value = 0;
            else if (value > 100)
                value = 100;
            value = (int)Math.Round(value / 5.0f) * 5;
            nmUHFSquelch.Value = value;
        }

        private void nm220Squelch_Leave(object sender, EventArgs e)
        {
            int value = (int)nm220Squelch.Value;
            if (value < 0)
                value = 0;
            else if (value > 100)
                value = 100;
            value = (int)Math.Round(value / 5.0f) * 5;
            nm220Squelch.Value = value;
        }

        private void nmDayBacklight_Leave(object sender, EventArgs e)
        {
            int value = (int)nmDayBacklight.Value;
            if (value < 0)
                value = 0;
            else if (value > 100)
                value = 100;
            if (value > 10)
                value = (int)Math.Round(value / 10.0f) * 10;
            nmDayBacklight.Value = value;
        }

        private void nmNightBacklight_Leave(object sender, EventArgs e)
        {
            int value = (int)nmNightBacklight.Value;
            if (value < 0)
                value = 0;
            else if (value > 100)
                value = 100;
            if (value > 10)
                value = (int)Math.Round(value / 10.0f) * 10;
            nmNightBacklight.Value = value;
        }

        private void nmMinBacklight_Leave(object sender, EventArgs e)
        {
            int value = (int)nmMinBacklight.Value;
            if (value < 0)
                value = 0;
            else if (value > 90)
                value = 90;
            if (value > 10)
                value = (int)Math.Round(value / 10.0f) * 10;
            nmMinBacklight.Value = value;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (probeRadioModel())
            {
                if (!setupCommPort())
                {
                    SystemSounds.Hand.Play();
                    return;
                }
                if (int.Parse(MainForm.RadioInfo.buildDateTime.Substring(0, 4)) < 2025)
                    MessageBox.Show("Сброс настроек через CPS не поддерживается на этой версии прошивки!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    sendCommand(commPort, 77);
                    sendCommand(commPort, 6);
                    MessageBox.Show("Сброс настроек выполнен!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                commPort.Close();
                commPort = null;
            }
        }
    }
}
