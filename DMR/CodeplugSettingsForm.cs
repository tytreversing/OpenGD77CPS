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
/*
typedef struct __attribute__ ((__packed__))
{
uint32_t 		magicNumber;
uint8_t			timezone;// Lower 7 bits are the timezone. 64 = UTC, values < 64 are negative TZ values.  Bit 8 is a flag which indicates TZ/UTC. 0 = UTC
uint8_t			beepOptions; // 2 pairs of bits + 1 (TX and RX beeps)
uint32_t		bitfieldOptions; // see bitfieldOptions_t
uint32_t		aprsBeaconingSettingsPart1;
uint32_t		aprsBeaconingSettingsPart2;
uint16_t		aprsBeaconingSettingsPart3;
uint8_t			txPowerLevel;
uint8_t			txTimeoutBeepX5Secs;
uint8_t			beepVolumeDivider;
uint8_t			micGainDMR;
uint8_t			micGainFM;
uint8_t			backlightMode; // see BACKLIGHT_MODE enum
uint8_t			backLightTimeout; // 0 = never timeout. 1 - 255 time in seconds
int8_t			displayContrast;
int8_t			displayBacklightPercentageDay;
int8_t			displayBacklightPercentageNight;
int8_t			displayBacklightPercentageOff; // backlight level when "off"
uint8_t			extendedInfosOnScreen;
uint8_t			scanModePause;
uint8_t			scanDelay;
uint8_t			DMR_RxAGC;
uint8_t			hotspotType;
uint8_t			scanStepTime;
uint8_t			dmrCaptureTimeout;
uint8_t    		privateCalls;
uint8_t			contactDisplayPriority;
uint8_t			splitContact;
uint8_t			voxThreshold; // 0: disabled
uint8_t			voxTailUnits; // 500ms units
uint8_t			audioPromptMode;
uint8_t			batteryCalibration; // Units of 0.01V (NOTE: only the 4 lower bits are used)
uint8_t			squelchDefaultVHF; // VHF, 200 and UHF
uint8_t			squelchDefaultUHF; // VHF, 200 and UHF
uint8_t			squelchDefault220; // VHF, 200 and UHF
uint8_t			ecoLevel;// Power saving / economy level
uint8_t			apo; // unit: 30 minutes (5 is skipped, as we want 0, 30, 60, 90, 120 and 180)
uint8_t			keypadTimerLong;
uint8_t			keypadTimerRepeat;
uint8_t			autolockTimer; // in minutes
uint8_t         buttonP3; //Режим работы кнопки P3
uint8_t         buttonP3Long;
uint8_t         scanPriority; //множитель приоритетного сканирования, 2...10
} settingsAlignedStruct_t;
*/

        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 56)]
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
            public uint beepOptions;

            
        }


        public CodeplugSettingsForm()
        {
            InitializeComponent();
            base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Scale(Settings.smethod_6());
            Settings.ReadCommonsForSectionIntoDictionary(OpenGD77StringsDict, "OpenGD77Form");
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
                        openGD77CommsTransferData.dataBuff = new byte[56];
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

                            break;
                        case OpenGD77CommsTransferData.CommsAction.WRITE_SETTINGS:
                            openGD77CommsTransferData.action = OpenGD77CommsTransferData.CommsAction.NONE;
                            break;
                    }
                }
                else
                {
                    SystemSounds.Hand.Play();
                    MessageBox.Show(OpenGD77StringsDict["There_has_been_an_error._Refer_to_the_last_status_message_that_was_displayed"], OpenGD77StringsDict["Oops"]);

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
    }
}
