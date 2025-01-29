using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using static DMR.OpenGD77Form;


namespace DMR;

public class CalibrationFormMDUV380 : Form
{
    public const int MAX_TRANSFER_BUFFER_SIZE = 1032;

    public static int MEMORY_LOCATION = 61440;

    public static int MEMORY_LOCATION_STM32 = 65536;

    public static int CALIBRATION_MEMORY_LOCATION_OFFICIAL_USB_PROTOCOL = 31744;

    public static int CALIBRATION_DATA_SIZE = 224;

    public static int CALIBRATION_DATA_SIZE_STM32 = 512;

    public static int CALIBRATION_HEADER_SIZE = 2;

    public static byte[] CALIBRATION_HEADER = new byte[8] { 160, 15, 192, 18, 160, 15, 192, 18 };

    private char writeCommandCharacter = 'W';

    private SerialPort commPort;

    public static Dictionary<string, string> StringsDict = new Dictionary<string, string>();

    public static byte[] calibrationDataSTM32;

    private IContainer components;

    private Button btnWrite;

    private Button btnReadFile;

    private Button btnReadFromRadio;
    private Button btnSaveCalibration;
    private SaveFileDialog saveFileDialog;
    private OpenFileDialog openFileDialog;

    XmlSerializer xmlSerializer = new XmlSerializer(typeof(CalibrationDataSTM32));
    private TabControl tabs;
    private TabPage tabVHF;
    private TabPage tabUHF;
    private GroupBox gbCommons;
    private NumericUpDown nmVOXMaxLevel;
    private NumericUpDown nmVOXMinLevel;
    private Label label2;
    private Label label1;
    private Label label3;
    private NumericUpDown nmRxLowLevel;
    private NumericUpDown nmRxHighLevel;
    private Label label4;
    private NumericUpDown nmRSSI120;
    private Label label5;
    private NumericUpDown nmRSSI70;
    private Label label6;
    private TableLayoutPanel tlpVHF;
    private Label label8;
    private Label label9;
    private Label label10;
    private Label label11;
    private Label label12;
    private Button btnReadFactoryFromRadio;
    private Label label16;
    private Label lblRadioType;
    private Label label22;
    private Label label23;
    private Label label24;
    private Label label25;
    private Label label26;
    private Label label27;
    private Label label28;
    private Label label29;
    private Label label30;
    private Label label31;
    private Label label32;
    private Label label33;
    private NumericUpDown nmVHFOscRef;
    private Label label34;
    private TableLayoutPanel tlpUHF;
    private NumericUpDown nmUHFOscRef;
    private Label label35;
    private Label label40;
    private Label label41;
    private Label label42;
    private Label label43;
    private Label label44;
    private Label label7;
    private Label label13;
    private Label label14;
    private Label label15;
    private Label label21;
    private Label label17;
    private Label label18;
    private Label label19;
    private Label label20;
    private Label label36;
    private Label label37;
    private Label label38;
    private Label label39;
    private Label label45;
    private Label label46;
    private Label label47;
    private Label label48;
    private Label label49;
    private Label label50;
    private Label label51;
    private Label label52;
    private Label label53;
    private Label label54;
    CalibrationDataSTM32 CalData = new CalibrationDataSTM32();

    public CalibrationFormMDUV380()
    {
        InitializeComponent();
        base.Icon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
        Settings.ReadCommonsForSectionIntoDictionary(StringsDict, "OpenGD77Form");
        calibrationDataSTM32 = new byte[CALIBRATION_DATA_SIZE_STM32];
        prepareTables();
    }

    private Form getMainForm()
    {
        foreach (Form openForm in System.Windows.Forms.Application.OpenForms)
        {
            if (openForm is MainForm)
            {
                return openForm;
            }
        }
        return null;
    }

    private RadioBandlimits radioBandlimits = new();

    private static RadioBandlimits ByteArrayToRadioBandlimits(byte[] bytes)
    {
        GCHandle gCHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            return (RadioBandlimits)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(RadioBandlimits));
        }
        finally
        {
            gCHandle.Free();
        }
    }

    private static bool ReadRadioBandlimits(SerialPort port, OpenGD77CommsTransferData dataObj)
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
        return false;
    }
    private bool readBandlimits()
    {
        if (!setupCommPort())
        {
            SystemSounds.Hand.Play();
            MessageBox.Show(StringsDict["No_com_port"]);
            return false;
        }

        OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData();
        openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadBandlimits;
        openGD77CommsTransferData.localDataBufferStartPosition = 0;
        openGD77CommsTransferData.transferLength = 0;
        openGD77CommsTransferData.dataBuff = new byte[128];
        radioBandlimits = default(RadioBandlimits);
        if (ReadRadioBandlimits(commPort, openGD77CommsTransferData))
        {
            radioBandlimits = ByteArrayToRadioBandlimits(openGD77CommsTransferData.dataBuff);
            
        }
        commPort.Close();
        commPort = null;
        
        return true;

    }

    private bool probeRadioModel()
    {
        if (!setupCommPort())
        {
            SystemSounds.Hand.Play();
            MessageBox.Show(StringsDict["No_com_port"]);
            return false;
        }
        sendCommand(commPort, 254);
        commPort.DiscardInBuffer();
        if (!sendCommand(commPort, 0))
        {
            MessageBox.Show(StringsDict["Error_connecting_to_the_radio"]);
            commPort = null;
            return false;
        }
        MainForm.RadioInfo = OpenGD77Form.readOpenGD77RadioInfoAndUpdateUSBBufferSize(commPort);
        if (MainForm.RadioInfo.radioType == 5 || MainForm.RadioInfo.radioType == 6 || MainForm.RadioInfo.radioType == 8 || MainForm.RadioInfo.radioType == 10 || MainForm.RadioInfo.radioType == 9 || MainForm.RadioInfo.radioType == 7 || MainForm.RadioInfo.radioType == 106)
        {
            writeCommandCharacter = 'X';
            ((MainForm)getMainForm())?.changeRadioType(MainForm.RadioTypeEnum.RadioTypeSTM32);
        }
        else
        {
            writeCommandCharacter = 'W';
            ((MainForm)getMainForm())?.changeRadioType(MainForm.RadioTypeEnum.RadioTypeMK22);
        }
        sendCommand(commPort, 5);
        commPort.Close();
        commPort = null;
        return true;
    }

    private bool sendCommand(SerialPort port, int commandNumber, int x_or_command_option_number = 0, int y = 0, int iSize = 0, int alignment = 0, int isInverted = 0, string message = "")
    {
        byte[] array = new byte[64];
        array[0] = 67;
        array[1] = (byte)commandNumber;
        switch (commandNumber)
        {
            case 2:
                array[3] = (byte)y;
                array[4] = (byte)iSize;
                array[5] = (byte)alignment;
                array[6] = (byte)isInverted;
                Encoding inEncoding = Encoding.Unicode;
                Encoding outEncoding = Encoding.GetEncoding(1251);
                byte[] sourceBuffer = inEncoding.GetBytes(message);
                byte[] destBuffer = Encoding.Convert(inEncoding, outEncoding, sourceBuffer);
                Buffer.BlockCopy(destBuffer, 0, array, 7, Math.Min(message.Length, 16));
                break;
            case 6:
                array[2] = (byte)x_or_command_option_number;
                break;
        }
        commPort.Write(array, 0, 32);
        while (commPort.BytesToRead == 0)
        {
            Thread.Sleep(0);
        }
        commPort.Read(array, 0, 64);
        return array[1] == commandNumber;
    }

    private void updateProgess(int progressPercentage)
    {
    }


    private bool ReadFlashOrEEPROM(SerialPort port, OpenGD77CommsTransferData dataObj)
    {
        int num = 0;
        byte[] array = new byte[512];
        byte[] array2 = new byte[512];
        bool result = true;
        int num2 = dataObj.startDataAddressInTheRadio;
        int localDataBufferStartPosition = dataObj.localDataBufferStartPosition;
        for (int num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2; num3 > 0; num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2)
        {
            if (num3 > 32)
            {
                num3 = 32;
            }
            array[0] = 82;
            array[1] = (byte)dataObj.mode;
            array[2] = (byte)((num2 >> 24) & 0xFF);
            array[3] = (byte)((num2 >> 16) & 0xFF);
            array[4] = (byte)((num2 >> 8) & 0xFF);
            array[5] = (byte)(num2 & 0xFF);
            array[6] = (byte)((num3 >> 8) & 0xFF);
            array[7] = (byte)(num3 & 0xFF);
            port.Write(array, 0, 8);
            while (port.BytesToRead == 0)
            {
                Thread.Sleep(0);
            }
            port.Read(array2, 0, 64);
            if (array2[0] == 82)
            {
                int num4 = (array2[1] << 8) + array2[2];
                for (int i = 0; i < num4; i++)
                {
                    dataObj.dataBuff[localDataBufferStartPosition++] = array2[i + 3];
                }
                int num5 = (num2 - dataObj.startDataAddressInTheRadio) * 100 / dataObj.transferLength;
                if (num != num5)
                {
                    updateProgess(num5);
                    num = num5;
                }
                num2 += num4;
            }
            else
            {
                result = false;
            }
        }
        return result;
    }



    private static CalibrationDataSTM32 ByteArrayToCalData(byte[] bytes)
    {
        GCHandle gCHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            return (CalibrationDataSTM32)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(CalibrationDataSTM32));
        }
        finally
        {
            gCHandle.Free();
        }
    }

    public bool readDataFromRadio()
    {
        bool result = true;
        int num = Marshal.SizeOf(typeof(CalibrationData));
        byte[] array = new byte[num];
        if (!probeRadioModel())
        {
            return false;
        }
        if (!setupCommPort())
        {
            SystemSounds.Hand.Play();
            MessageBox.Show(StringsDict["No_com_port"]);
            return false;
        }
        OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData();
        sendCommand(commPort, 0);
        sendCommand(commPort, 1);
        sendCommand(commPort, 2, 0, 0, 3, 1, 0, "CPS");
        sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Reading"]);
        sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_Calibrations"]);
        sendCommand(commPort, 3);
        sendCommand(commPort, 6, 3);
        openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadFlash;
        openGD77CommsTransferData.dataBuff = new byte[CALIBRATION_DATA_SIZE_STM32];
        openGD77CommsTransferData.localDataBufferStartPosition = 0;
        openGD77CommsTransferData.startDataAddressInTheRadio = MEMORY_LOCATION_STM32;
        openGD77CommsTransferData.transferLength = CALIBRATION_DATA_SIZE_STM32;
        if (!ReadFlashOrEEPROM(commPort, openGD77CommsTransferData))
        {
            result = false;
            openGD77CommsTransferData.responseCode = 1;
        }
        else
        {
            SystemSounds.Exclamation.Play();
        }


        sendCommand(commPort, 5);
        sendCommand(commPort, 7);
        commPort.Close();
        commPort = null;
        CalData = ByteArrayToCalData(openGD77CommsTransferData.dataBuff);
        buildVariablesFromCalData(CalData);
        return result;
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
                MessageBox.Show(StringsDict["Please_connect_the_radio,_and_try_again."], StringsDict["Radio_not_detected."]);
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
            MessageBox.Show(StringsDict["Failed_to_open_comm_port"], StringsDict["Error"]);
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
            MessageBox.Show(StringsDict["Comm_port_not_available"]);
            commPort = null;
            return false;
        }
        return true;
    }

    private void btnWrite_Click(object sender, EventArgs e)
    {
        MainForm.RadioTypeEnum radioType = MainForm.RadioType;
        if (!probeRadioModel() || radioType != MainForm.RadioType)
        {
            return;
        }
        if (!setupCommPort())
        {
            SystemSounds.Hand.Play();
            MessageBox.Show(StringsDict["No_com_port"]);
            return;
        }
        buildCalDataFromVariables(CalData);
        OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData();
        openGD77CommsTransferData.dataBuff = new byte[CALIBRATION_DATA_SIZE_STM32];
        calibrationDataSTM32 = DataToByte(CalData);
        Array.Copy(calibrationDataSTM32, 0, openGD77CommsTransferData.dataBuff, 0, CALIBRATION_DATA_SIZE_STM32);
        sendCommand(commPort, 0);
        sendCommand(commPort, 1);
        sendCommand(commPort, 2, 0, 0, 3, 1, 0, "CPS");
        sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Restoring"]);
        sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_Calibrations"]);
        sendCommand(commPort, 3);
        sendCommand(commPort, 6, 4);
        openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteFlash;
        openGD77CommsTransferData.localDataBufferStartPosition = 0;
        openGD77CommsTransferData.startDataAddressInTheRadio = MEMORY_LOCATION_STM32 ;
        openGD77CommsTransferData.transferLength = CALIBRATION_DATA_SIZE_STM32;
        if (!WriteFlash(commPort, openGD77CommsTransferData))
        {
            MessageBox.Show("Ошибка при восстановлении!");
            openGD77CommsTransferData.responseCode = 1;
        }
        sendCommand(commPort, 6, 2);
        sendCommand(commPort, 6, 1);
        commPort.Close();
        commPort = null;
    }

    private bool flashWriteSector(SerialPort port, char writeCharacter, ref byte[] sendbuffer, ref byte[] readbuffer, OpenGD77CommsTransferData dataObj)
    {
        int num = 100;
        dataObj.data_sector = -1;
        sendbuffer[0] = (byte)writeCharacter;
        sendbuffer[1] = 3;
        port.Write(sendbuffer, 0, 2);
        while (port.BytesToRead == 0)
        {
            Thread.Sleep(1);
        }
        Thread.Sleep(100);
        while (port.BytesToRead == 0 && num-- > 0)
        {
            Thread.Sleep(5);
        }
        port.Read(readbuffer, 0, port.BytesToRead);
        if (readbuffer[0] == sendbuffer[0] && readbuffer[1] == sendbuffer[1])
        {
            return num != -1;
        }
        return false;
    }

    private bool flashWritePrepareSector(SerialPort port, char writeCharacter, int address, ref byte[] sendbuffer, ref byte[] readbuffer, OpenGD77CommsTransferData dataObj)
    {
        int num = 100;
        dataObj.data_sector = address / 4096;
        sendbuffer[0] = (byte)writeCharacter;
        sendbuffer[1] = 1;
        sendbuffer[2] = (byte)((dataObj.data_sector >> 16) & 0xFF);
        sendbuffer[3] = (byte)((dataObj.data_sector >> 8) & 0xFF);
        sendbuffer[4] = (byte)(dataObj.data_sector & 0xFF);
        port.Write(sendbuffer, 0, 5);
        while (port.BytesToRead == 0)
        {
            Thread.Sleep(1);
        }
        Thread.Sleep(50);
        while (port.BytesToRead == 0 && num-- > 0)
        {
            Thread.Sleep(1);
        }
        if (num != -1)
        {
            port.Read(readbuffer, 0, port.BytesToRead);
        }
        if (readbuffer[0] == sendbuffer[0] && readbuffer[1] == sendbuffer[1])
        {
            return num != -1;
        }
        return false;
    }

    private bool flashSendData(SerialPort port, char writeCharacter, int address, int len, ref byte[] sendbuffer, ref byte[] readbuffer)
    {
        int num = 100;
        sendbuffer[0] = (byte)writeCharacter;
        sendbuffer[1] = 2;
        sendbuffer[2] = (byte)((address >> 24) & 0xFF);
        sendbuffer[3] = (byte)((address >> 16) & 0xFF);
        sendbuffer[4] = (byte)((address >> 8) & 0xFF);
        sendbuffer[5] = (byte)(address & 0xFF);
        sendbuffer[6] = (byte)((len >> 8) & 0xFF);
        sendbuffer[7] = (byte)(len & 0xFF);
        port.Write(sendbuffer, 0, len + 8);
        while (port.BytesToRead == 0)
        {
            Thread.Sleep(1);
        }
        Thread.Sleep(20);
        while (port.BytesToRead == 0 && num-- > 0)
        {
            Thread.Sleep(1);
        }
        if (num != -1)
        {
            port.Read(readbuffer, 0, port.BytesToRead);
        }
        if (readbuffer[0] == sendbuffer[0] && readbuffer[1] == sendbuffer[1])
        {
            return num != -1;
        }
        return false;
    }

    private bool WriteFlash(SerialPort port, OpenGD77CommsTransferData dataObj)
    {
        int num = 0;
        byte[] sendbuffer = new byte[1032];
        byte[] readbuffer = new byte[1032];
        int num2 = dataObj.startDataAddressInTheRadio;
        int localDataBufferStartPosition = dataObj.localDataBufferStartPosition;
        dataObj.data_sector = -1;
        for (int num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2; num3 > 0; num3 = dataObj.startDataAddressInTheRadio + dataObj.transferLength - num2)
        {
            if (num3 > OpenGD77Form.getUSBWriteBufferSize())
            {
                num3 = OpenGD77Form.getUSBWriteBufferSize();
            }
            if (dataObj.data_sector == -1 && !flashWritePrepareSector(port, writeCommandCharacter, num2, ref sendbuffer, ref readbuffer, dataObj))
            {
                return false;
            }
            if (dataObj.mode != 0)
            {
                int num4 = 0;
                for (int i = 0; i < num3; i++)
                {
                    sendbuffer[i + 8] = dataObj.dataBuff[localDataBufferStartPosition++];
                    num4++;
                    if (dataObj.data_sector != (num2 + num4) / 4096)
                    {
                        break;
                    }
                }
                if (!flashSendData(port, writeCommandCharacter, num2, num4, ref sendbuffer, ref readbuffer))
                {
                    return false;
                }
                int num5 = (num2 - dataObj.startDataAddressInTheRadio) * 100 / dataObj.transferLength;
                if (num != num5)
                {
                    updateProgess(num5);
                    num = num5;
                }
                num2 += num4;
                if (dataObj.data_sector != num2 / 4096 && !flashWriteSector(port, writeCommandCharacter, ref sendbuffer, ref readbuffer, dataObj))
                {
                    return false;
                }
            }
        }
        if (dataObj.data_sector != -1 && !flashWriteSector(port, writeCommandCharacter, ref sendbuffer, ref readbuffer, dataObj))
        {
            Console.WriteLine($"Error. Write stopped (write sector error at {num2:X8})");
            return false;
        }
        return true;
    }


    public static byte[] DataToByte(CalibrationDataSTM32 calData)
    {
        
        int num = Marshal.SizeOf(typeof(CalibrationDataSTM32));
        byte[] array = new byte[num];
        IntPtr intPtr = Marshal.AllocHGlobal(num);
        Marshal.StructureToPtr(calData, intPtr, fDeleteOld: false);
        Marshal.Copy(intPtr, array, 0, num);
        Marshal.FreeHGlobal(intPtr);
        return array;
    }

    private void onFormLoad(object sender, EventArgs e)
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

    private string unknownElements = "";
    private bool hasUnknownElements = false;

    private void unknownElementEvent(object sender, XmlElementEventArgs e)
    {
        hasUnknownElements = true;
        unknownElements += ("\r\n" + e.Element.Name + ": " + e.Element.InnerXml);
    }

    private void btnReadFile_Click(object sender, EventArgs e)
    {
        string profileStringWithDefault = IniFileUtils.getProfileStringWithDefault("Setup", "LastFilePath", "");
        string initialDirectory;
        try
        {
            initialDirectory = ((!(profileStringWithDefault == "")) ? Path.GetDirectoryName(profileStringWithDefault) : Environment.GetFolderPath(Environment.SpecialFolder.Personal));
        }
        catch (Exception)
        {
            initialDirectory = "";
        }
        
        saveFileDialog.InitialDirectory = initialDirectory;
        if (openFileDialog.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(openFileDialog.FileName))
        {
            return;
        }
        try
        {
            using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open))
            {
                xmlSerializer.UnknownElement += new XmlElementEventHandler(unknownElementEvent);
                CalData = xmlSerializer.Deserialize(fs) as CalibrationDataSTM32;
                if (hasUnknownElements)
                {
                    hasUnknownElements = false;
                    SystemSounds.Exclamation.Play();
                    MessageBox.Show("В блоке калибровок обнаружены неподдерживаемые поля.\r\nПоля, которые не могут быть распознаны:" + unknownElements, "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    unknownElements = "";
                }
                else
                    showButtons();
                buildVariablesFromCalData(CalData);
                
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось открыть файл " + saveFileDialog.FileName + "\r\n" + ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }

    private void btnReadFromRadio_Click(object sender, EventArgs e)
    {
        if (readDataFromRadio())
        {
            showButtons();
        }
        if (readBandlimits())
        {
            
        }

    }

    private void btnReadFactoryFromRadio_Click(object sender, EventArgs e)
    {
        MainForm.RadioTypeEnum radioType = MainForm.RadioType;
        if (!probeRadioModel() || radioType != MainForm.RadioType)
        {
            return;
        }
        if (!setupCommPort())
        {
            SystemSounds.Hand.Play();
            MessageBox.Show(StringsDict["No_com_port"]);
            return;
        }
        for (int i = 0; i < 4; i++)
        {
            CalData.MARKER[i] = 0x00;
        }
        OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData();
        openGD77CommsTransferData.dataBuff = new byte[CALIBRATION_DATA_SIZE_STM32];
        calibrationDataSTM32 = DataToByte(CalData);
        Array.Copy(calibrationDataSTM32, 0, openGD77CommsTransferData.dataBuff, 0, CALIBRATION_DATA_SIZE_STM32);
        sendCommand(commPort, 0);
        sendCommand(commPort, 1);
        sendCommand(commPort, 2, 0, 0, 3, 1, 0, "CPS");
        sendCommand(commPort, 2, 0, 16, 3, 1, 0, StringsDict["RADIO_DISPLAY_Restoring"]);
        sendCommand(commPort, 2, 0, 32, 3, 1, 0, StringsDict["RADIO_DISPLAY_Calibrations"]);
        sendCommand(commPort, 3);
        sendCommand(commPort, 6, 4);
        openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeWriteFlash;
        openGD77CommsTransferData.localDataBufferStartPosition = 0;
        openGD77CommsTransferData.startDataAddressInTheRadio = MEMORY_LOCATION_STM32;
        openGD77CommsTransferData.transferLength = CALIBRATION_DATA_SIZE_STM32;
        if (!WriteFlash(commPort, openGD77CommsTransferData))
        {
            MessageBox.Show("Ошибка при восстановлении!");
            openGD77CommsTransferData.responseCode = 1;
        }
        sendCommand(commPort, 6, 2);
        sendCommand(commPort, 6, 1);
        commPort.Close();
        commPort = null;
        MessageBox.Show("После перезагрузки рация перестроит таблицы, исходя из заводских калибровок, сохраненных в защищенной памяти.", "Сброс калибровок", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void showButtons()
    {
        btnWrite.Visible = true;
        btnSaveCalibration.Visible = true;
    }

    private void btnSaveCalibration_Click(object sender, EventArgs e)
    {
        string profileStringWithDefault = IniFileUtils.getProfileStringWithDefault("Setup", "LastFilePath", "");
        string initialDirectory;
        string radioType = "";

        try
        {
            initialDirectory = ((!(profileStringWithDefault == "")) ? Path.GetDirectoryName(profileStringWithDefault) : Environment.GetFolderPath(Environment.SpecialFolder.Personal));
        }
        catch (Exception)
        {
            initialDirectory = "";
        }
        switch (MainForm.RadioInfo.radioType)
        {
            case 5:
                radioType = "MD-9600_RT-90";
                break;
            case 6:
                radioType = "MD-UV380_MD-UV390_RT-3S";
                break;
            case 8:
            case 10:
                radioType = "DM-1701_RT-84";
                break;
            case 9:
                radioType = "MD-2017_RT-82";
                break;
            case 106:
                radioType = "MD-UV390(10W_Plus)";
                break;
        }
        saveFileDialog.FileName = "Калибровки_" + radioType + "_" + DateTime.Now.ToString("MMdd_HHmmss");
        saveFileDialog.InitialDirectory = initialDirectory;
        if (saveFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(saveFileDialog.FileName))
        {
            //File.WriteAllBytes(saveFileDialog.FileName, calibrationDataSTM32);
            try
            {
                using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    xmlSerializer.Serialize(fs, CalData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось сохранить файл " + saveFileDialog.FileName + "\r\n" + ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    
    private TextBox[] frequenciesTxVHF = new TextBox[5];
    private NumericUpDown[] maxPowersVHF = new NumericUpDown[5];
    private NumericUpDown[] midPowersVHF = new NumericUpDown[5];
    private NumericUpDown[] midLowPowersVHF = new NumericUpDown[5];
    private NumericUpDown[] minPowersVHF = new NumericUpDown[5];
    private NumericUpDown[] power4VHF = new NumericUpDown[5];
    private NumericUpDown[] power3VHF = new NumericUpDown[5];
    private NumericUpDown[] power2VHF = new NumericUpDown[5];
    private NumericUpDown[] power1VHF = new NumericUpDown[5];
    private NumericUpDown[] power0VHF = new NumericUpDown[5];
    private NumericUpDown[] rxTuningVHF = new NumericUpDown[5];
    private NumericUpDown[] OpenSQL9VHF = new NumericUpDown[5];
    private NumericUpDown[] CloseSQL9VHF = new NumericUpDown[5];
    private NumericUpDown[] OpenSQL1VHF = new NumericUpDown[5];
    private NumericUpDown[] CloseSQL1VHF = new NumericUpDown[5];
    private NumericUpDown[] ctc67VHF = new NumericUpDown[5];
    private NumericUpDown[] ctc151VHF = new NumericUpDown[5];
    private NumericUpDown[] ctc254VHF = new NumericUpDown[5];
    private NumericUpDown[] dcsVHF = new NumericUpDown[5];
    private NumericUpDown[] iGainDMRVHF = new NumericUpDown[5];
    private NumericUpDown[] qGainDMRVHF = new NumericUpDown[5];
    private NumericUpDown[] iGainFMVHF = new NumericUpDown[5];
    private NumericUpDown[] qGainFMVHF = new NumericUpDown[5];
    
    private TextBox[] frequenciesTxUHF = new TextBox[9];
    private NumericUpDown[] maxPowersUHF = new NumericUpDown[9];
    private NumericUpDown[] midPowersUHF = new NumericUpDown[9];
    private NumericUpDown[] midLowPowersUHF = new NumericUpDown[9];
    private NumericUpDown[] minPowersUHF = new NumericUpDown[9];
    private NumericUpDown[] power4UHF = new NumericUpDown[9];
    private NumericUpDown[] power3UHF = new NumericUpDown[9];
    private NumericUpDown[] power2UHF = new NumericUpDown[9];
    private NumericUpDown[] power1UHF = new NumericUpDown[9];
    private NumericUpDown[] power0UHF = new NumericUpDown[9];
    private NumericUpDown[] rxTuningUHF = new NumericUpDown[9];
    private NumericUpDown[] OpenSQL9UHF = new NumericUpDown[9];
    private NumericUpDown[] CloseSQL9UHF = new NumericUpDown[9];
    private NumericUpDown[] OpenSQL1UHF = new NumericUpDown[9];
    private NumericUpDown[] CloseSQL1UHF = new NumericUpDown[9];
    private NumericUpDown[] ctc67UHF = new NumericUpDown[9];
    private NumericUpDown[] ctc151UHF = new NumericUpDown[9];
    private NumericUpDown[] ctc254UHF = new NumericUpDown[9];
    private NumericUpDown[] dcsUHF = new NumericUpDown[9];
    private NumericUpDown[] iGainDMRUHF = new NumericUpDown[9];
    private NumericUpDown[] qGainDMRUHF = new NumericUpDown[9];
    private NumericUpDown[] iGainFMUHF = new NumericUpDown[9];
    private NumericUpDown[] qGainFMUHF = new NumericUpDown[9];

    private void prepareTables()
    {
        Padding margin = new Padding(0);

        for (int i = 0; i < 5; i++)
        {
            frequenciesTxVHF[i] = new TextBox();
            frequenciesTxVHF[i].Width = 74;
            frequenciesTxVHF[i].Height = 20;
            frequenciesTxVHF[i].Margin = margin;
            frequenciesTxVHF[i].ReadOnly = true;

            maxPowersVHF[i] = new NumericUpDown();
            maxPowersVHF[i].Width = 74;
            maxPowersVHF[i].Increment = 16;
            maxPowersVHF[i].Height = 20;
            maxPowersVHF[i].Margin = margin;
            maxPowersVHF[i].Minimum = 0;
            maxPowersVHF[i].Maximum = 4080;
            maxPowersVHF[i].ValueChanged += new EventHandler(nmValueChanged);
            midPowersVHF[i] = new NumericUpDown();
            midPowersVHF[i].Width = 74;
            midPowersVHF[i].Increment = 16;
            midPowersVHF[i].Height = 20;
            midPowersVHF[i].Margin = margin;
            midPowersVHF[i].Minimum = 0;
            midPowersVHF[i].Maximum = 4080;
            midPowersVHF[i].ValueChanged += new EventHandler(nmValueChanged);
            midLowPowersVHF[i] = new NumericUpDown();
            midLowPowersVHF[i].Width = 74;
            midLowPowersVHF[i].Increment = 16;
            midLowPowersVHF[i].Height = 20;
            midLowPowersVHF[i].Margin = margin;
            midLowPowersVHF[i].Minimum = 0;
            midLowPowersVHF[i].Maximum = 4080;
            midLowPowersVHF[i].ValueChanged += new EventHandler(nmValueChanged);
            minPowersVHF[i] = new NumericUpDown();
            minPowersVHF[i].Width = 74;
            minPowersVHF[i].Increment = 16;
            minPowersVHF[i].Height = 20;
            minPowersVHF[i].Margin = margin;
            minPowersVHF[i].Minimum = 0;
            minPowersVHF[i].Maximum = 4080;
            minPowersVHF[i].ValueChanged += new EventHandler(nmValueChanged);
            power4VHF[i] = new NumericUpDown();
            power4VHF[i].Width = 74;
            power4VHF[i].Increment = 1;
            power4VHF[i].Height = 20;
            power4VHF[i].Margin = margin;
            power4VHF[i].Minimum = 0;
            power4VHF[i].Maximum = 4080;
            power4VHF[i].ValueChanged += new EventHandler(nmValueChanged);
            power3VHF[i] = new NumericUpDown();
            power3VHF[i].Width = 74;
            power3VHF[i].Increment = 1;
            power3VHF[i].Height = 20;
            power3VHF[i].Margin = margin;
            power3VHF[i].Minimum = 0;
            power3VHF[i].Maximum = 4080;
            power3VHF[i].ValueChanged += new EventHandler(nmValueChanged);
            power2VHF[i] = new NumericUpDown();
            power2VHF[i].Width = 74;
            power2VHF[i].Increment = 1;
            power2VHF[i].Height = 20;
            power2VHF[i].Margin = margin;
            power2VHF[i].Minimum = 0;
            power2VHF[i].Maximum = 4080;
            power2VHF[i].ValueChanged += new EventHandler(nmValueChanged);
            power1VHF[i] = new NumericUpDown();
            power1VHF[i].Width = 74;
            power1VHF[i].Increment = 1;
            power1VHF[i].Height = 20;
            power1VHF[i].Margin = margin;
            power1VHF[i].Minimum = 0;
            power1VHF[i].Maximum = 4080;
            power1VHF[i].ValueChanged += new EventHandler(nmValueChanged);
            power0VHF[i] = new NumericUpDown();
            power0VHF[i].Width = 74;
            power0VHF[i].Increment = 1;
            power0VHF[i].Height = 20;
            power0VHF[i].Margin = margin;
            power0VHF[i].Minimum = 0;
            power0VHF[i].Maximum = 4080;
            power0VHF[i].ValueChanged += new EventHandler(nmValueChanged);
            rxTuningVHF[i] = new NumericUpDown();
            rxTuningVHF[i].Width = 74;
            rxTuningVHF[i].Height = 20;
            rxTuningVHF[i].Margin = margin;
            rxTuningVHF[i].Minimum = 0;
            rxTuningVHF[i].Maximum = 255;
            rxTuningVHF[i].ValueChanged += new EventHandler(nmValueChanged);
            OpenSQL9VHF[i] = new NumericUpDown();
            OpenSQL9VHF[i].Width = 74;
            OpenSQL9VHF[i].Height = 20;
            OpenSQL9VHF[i].Margin = margin;
            OpenSQL9VHF[i].Minimum = 0;
            OpenSQL9VHF[i].Maximum = 255;
            OpenSQL9VHF[i].ValueChanged += new EventHandler(nmValueChanged);
            CloseSQL9VHF[i] = new NumericUpDown();
            CloseSQL9VHF[i].Width = 74;
            CloseSQL9VHF[i].Height = 20;
            CloseSQL9VHF[i].Margin = margin;
            CloseSQL9VHF[i].Minimum = 0;
            CloseSQL9VHF[i].Maximum = 255;
            CloseSQL9VHF[i].ValueChanged += new EventHandler(nmValueChanged);
            OpenSQL1VHF[i] = new NumericUpDown();
            OpenSQL1VHF[i].Width = 74;
            OpenSQL1VHF[i].Height = 20;
            OpenSQL1VHF[i].Margin = margin;
            OpenSQL1VHF[i].Minimum = 0;
            OpenSQL1VHF[i].Maximum = 255;
            OpenSQL1VHF[i].ValueChanged += new EventHandler(nmValueChanged);
            CloseSQL1VHF[i] = new NumericUpDown();
            CloseSQL1VHF[i].Width = 74;
            CloseSQL1VHF[i].Height = 20;
            CloseSQL1VHF[i].Margin = margin;
            CloseSQL1VHF[i].Minimum = 0;
            CloseSQL1VHF[i].Maximum = 255;
            ctc67VHF[i] = new NumericUpDown();
            ctc67VHF[i].Width = 74;
            ctc67VHF[i].Height = 20;
            ctc67VHF[i].Margin = margin;
            ctc67VHF[i].Minimum = 0;
            ctc67VHF[i].Maximum = 255;
            ctc67VHF[i].ValueChanged += new EventHandler(nmValueChanged);
            ctc151VHF[i] = new NumericUpDown();
            ctc151VHF[i].Width = 74;
            ctc151VHF[i].Height = 20;
            ctc151VHF[i].Margin = margin;
            ctc151VHF[i].Minimum = 0;
            ctc151VHF[i].Maximum = 255;
            ctc151VHF[i].ValueChanged += new EventHandler(nmValueChanged);
            ctc254VHF[i] = new NumericUpDown();
            ctc254VHF[i].Width = 74;
            ctc254VHF[i].Height = 20;
            ctc254VHF[i].Margin = margin;
            ctc254VHF[i].Minimum = 0;
            ctc254VHF[i].Maximum = 255;
            ctc254VHF[i].ValueChanged += new EventHandler(nmValueChanged);
            dcsVHF[i] = new NumericUpDown();
            dcsVHF[i].Width = 74;
            dcsVHF[i].Height = 20;
            dcsVHF[i].Margin = margin;
            dcsVHF[i].Minimum = 0;
            dcsVHF[i].Maximum = 255;
            dcsVHF[i].ValueChanged += new EventHandler(nmValueChanged);
            iGainDMRVHF[i] = new NumericUpDown();
            iGainDMRVHF[i].Width = 74;
            iGainDMRVHF[i].Height = 20;
            iGainDMRVHF[i].Margin = margin;
            iGainDMRVHF[i].Minimum = 0;
            iGainDMRVHF[i].Maximum = 255;
            iGainDMRVHF[i].ValueChanged += new EventHandler(nmValueChanged);
            qGainDMRVHF[i] = new NumericUpDown();
            qGainDMRVHF[i].Width = 74;
            qGainDMRVHF[i].Height = 20;
            qGainDMRVHF[i].Margin = margin;
            qGainDMRVHF[i].Minimum = 0;
            qGainDMRVHF[i].Maximum = 255;
            qGainDMRVHF[i].ValueChanged += new EventHandler(nmValueChanged);
            iGainFMVHF[i] = new NumericUpDown();
            iGainFMVHF[i].Width = 74;
            iGainFMVHF[i].Height = 20;
            iGainFMVHF[i].Margin = margin;
            iGainFMVHF[i].Minimum = 0;
            iGainFMVHF[i].Maximum = 255;
            iGainFMVHF[i].ValueChanged += new EventHandler(nmValueChanged);
            qGainFMVHF[i] = new NumericUpDown();
            qGainFMVHF[i].Width = 74;
            qGainFMVHF[i].Height = 20;
            qGainFMVHF[i].Margin = margin;
            qGainFMVHF[i].Minimum = 0;
            qGainFMVHF[i].Maximum = 255;
            qGainFMVHF[i].ValueChanged += new EventHandler(nmValueChanged);
            
            tlpVHF.Controls.Add(frequenciesTxVHF[i], i + 1, 0);
            tlpVHF.Controls.Add(maxPowersVHF[i], i + 1, 1);
            tlpVHF.Controls.Add(midPowersVHF[i], i + 1, 2);
            tlpVHF.Controls.Add(midLowPowersVHF[i], i + 1, 3);
            tlpVHF.Controls.Add(minPowersVHF[i], i + 1, 4);
            tlpVHF.Controls.Add(power4VHF[i], i + 1, 5);
            tlpVHF.Controls.Add(power3VHF[i], i + 1, 6);
            tlpVHF.Controls.Add(power2VHF[i], i + 1, 7);
            tlpVHF.Controls.Add(power1VHF[i], i + 1, 8);
            tlpVHF.Controls.Add(power0VHF[i], i + 1, 9);
            tlpVHF.Controls.Add(rxTuningVHF[i], i + 1, 10);
            tlpVHF.Controls.Add(OpenSQL9VHF[i], i + 1, 11);
            tlpVHF.Controls.Add(CloseSQL9VHF[i], i + 1, 12);
            tlpVHF.Controls.Add(OpenSQL1VHF[i], i + 1, 13);
            tlpVHF.Controls.Add(CloseSQL1VHF[i], i + 1, 14);
            tlpVHF.Controls.Add(ctc67VHF[i], i + 1, 15);
            tlpVHF.Controls.Add(ctc151VHF[i], i + 1, 16);
            tlpVHF.Controls.Add(ctc254VHF[i], i + 1, 17);
            tlpVHF.Controls.Add(dcsVHF[i], i + 1, 18);
            tlpVHF.Controls.Add(iGainDMRVHF[i], i + 1, 19);
            tlpVHF.Controls.Add(qGainDMRVHF[i], i + 1, 20);
            tlpVHF.Controls.Add(iGainFMVHF[i], i + 1, 21);
            tlpVHF.Controls.Add(qGainFMVHF[i], i + 1, 22);
        }
        for (int i = 0; i < 9; i++)
        {
            frequenciesTxUHF[i] = new TextBox();
            frequenciesTxUHF[i].Width = 74;
            frequenciesTxUHF[i].Height = 20;
            frequenciesTxUHF[i].Margin = margin;
            frequenciesTxUHF[i].ReadOnly = true;
            
            maxPowersUHF[i] = new NumericUpDown();
            maxPowersUHF[i].Width = 74;
            maxPowersUHF[i].Increment = 16;
            maxPowersUHF[i].Height = 20;
            maxPowersUHF[i].Margin = margin;
            maxPowersUHF[i].Minimum = 0;
            maxPowersUHF[i].Maximum = 4080;
            maxPowersUHF[i].ValueChanged += new EventHandler(nmValueChanged);
            midPowersUHF[i] = new NumericUpDown();
            midPowersUHF[i].Width = 74;
            midPowersUHF[i].Increment = 16;
            midPowersUHF[i].Height = 20;
            midPowersUHF[i].Margin = margin;
            midPowersUHF[i].Minimum = 0;
            midPowersUHF[i].Maximum = 4080;
            midPowersUHF[i].ValueChanged += new EventHandler(nmValueChanged);
            midLowPowersUHF[i] = new NumericUpDown();
            midLowPowersUHF[i].Width = 74;
            midLowPowersUHF[i].Increment = 16;
            midLowPowersUHF[i].Height = 20;
            midLowPowersUHF[i].Margin = margin;
            midLowPowersUHF[i].Minimum = 0;
            midLowPowersUHF[i].Maximum = 4096;
            midLowPowersUHF[i].ValueChanged += new EventHandler(nmValueChanged);
            minPowersUHF[i] = new NumericUpDown();
            minPowersUHF[i].Width = 74;
            minPowersUHF[i].Increment = 16;
            minPowersUHF[i].Height = 20;
            minPowersUHF[i].Margin = margin;
            minPowersUHF[i].Minimum = 0;
            minPowersUHF[i].Maximum = 4080;
            minPowersUHF[i].ValueChanged += new EventHandler(nmValueChanged);
            power4UHF[i] = new NumericUpDown();
            power4UHF[i].Width = 74;
            power4UHF[i].Increment = 1;
            power4UHF[i].Height = 20;
            power4UHF[i].Margin = margin;
            power4UHF[i].Minimum = 0;
            power4UHF[i].Maximum = 4080;
            power4UHF[i].ValueChanged += new EventHandler(nmValueChanged);
            power3UHF[i] = new NumericUpDown();
            power3UHF[i].Width = 74;
            power3UHF[i].Increment = 1;
            power3UHF[i].Height = 20;
            power3UHF[i].Margin = margin;
            power3UHF[i].Minimum = 0;
            power3UHF[i].Maximum = 4080;
            power3UHF[i].ValueChanged += new EventHandler(nmValueChanged);
            power2UHF[i] = new NumericUpDown();
            power2UHF[i].Width = 74;
            power2UHF[i].Increment = 1;
            power2UHF[i].Height = 20;
            power2UHF[i].Margin = margin;
            power2UHF[i].Minimum = 0;
            power2UHF[i].Maximum = 4080;
            power2UHF[i].ValueChanged += new EventHandler(nmValueChanged);
            power1UHF[i] = new NumericUpDown();
            power1UHF[i].Width = 74;
            power1UHF[i].Increment = 1;
            power1UHF[i].Height = 20;
            power1UHF[i].Margin = margin;
            power1UHF[i].Minimum = 0;
            power1UHF[i].Maximum = 4080;
            power1UHF[i].ValueChanged += new EventHandler(nmValueChanged);
            power0UHF[i] = new NumericUpDown();
            power0UHF[i].Width = 74;
            power0UHF[i].Increment = 1;
            power0UHF[i].Height = 20;
            power0UHF[i].Margin = margin;
            power0UHF[i].Minimum = 0;
            power0UHF[i].Maximum = 4080;
            power0UHF[i].ValueChanged += new EventHandler(nmValueChanged);
            rxTuningUHF[i] = new NumericUpDown();
            rxTuningUHF[i].Width = 74;
            rxTuningUHF[i].Height = 20;
            rxTuningUHF[i].Margin = margin;
            rxTuningUHF[i].Minimum = 0;
            rxTuningUHF[i].Maximum = 255;
            rxTuningUHF[i].ValueChanged += new EventHandler(nmValueChanged);
            OpenSQL9UHF[i] = new NumericUpDown();
            OpenSQL9UHF[i].Width = 74;
            OpenSQL9UHF[i].Height = 20;
            OpenSQL9UHF[i].Margin = margin;
            OpenSQL9UHF[i].Minimum = 0;
            OpenSQL9UHF[i].Maximum = 255;
            OpenSQL9UHF[i].ValueChanged += new EventHandler(nmValueChanged);
            CloseSQL9UHF[i] = new NumericUpDown();
            CloseSQL9UHF[i].Width = 74;
            CloseSQL9UHF[i].Height = 20;
            CloseSQL9UHF[i].Margin = margin;
            CloseSQL9UHF[i].Minimum = 0;
            CloseSQL9UHF[i].Maximum = 255;
            CloseSQL9UHF[i].ValueChanged += new EventHandler(nmValueChanged);
            OpenSQL1UHF[i] = new NumericUpDown();
            OpenSQL1UHF[i].Width = 74;
            OpenSQL1UHF[i].Height = 20;
            OpenSQL1UHF[i].Margin = margin;
            OpenSQL1UHF[i].Minimum = 0;
            OpenSQL1UHF[i].Maximum = 255;
            OpenSQL1UHF[i].ValueChanged += new EventHandler(nmValueChanged);
            CloseSQL1UHF[i] = new NumericUpDown();
            CloseSQL1UHF[i].Width = 74;
            CloseSQL1UHF[i].Height = 20;
            CloseSQL1UHF[i].Margin = margin;
            CloseSQL1UHF[i].Minimum = 0;
            CloseSQL1UHF[i].Maximum = 255;
            ctc67UHF[i] = new NumericUpDown();
            ctc67UHF[i].Width = 74;
            ctc67UHF[i].Height = 20;
            ctc67UHF[i].Margin = margin;
            ctc67UHF[i].Minimum = 0;
            ctc67UHF[i].Maximum = 255;
            ctc67UHF[i].ValueChanged += new EventHandler(nmValueChanged);
            ctc151UHF[i] = new NumericUpDown();
            ctc151UHF[i].Width = 74;
            ctc151UHF[i].Height = 20;
            ctc151UHF[i].Margin = margin;
            ctc151UHF[i].Minimum = 0;
            ctc151UHF[i].Maximum = 255;
            ctc151UHF[i].ValueChanged += new EventHandler(nmValueChanged);
            ctc254UHF[i] = new NumericUpDown();
            ctc254UHF[i].Width = 74;
            ctc254UHF[i].Height = 20;
            ctc254UHF[i].Margin = margin;
            ctc254UHF[i].Minimum = 0;
            ctc254UHF[i].Maximum = 255;
            ctc254UHF[i].ValueChanged += new EventHandler(nmValueChanged);
            dcsUHF[i] = new NumericUpDown();
            dcsUHF[i].Width = 74;
            dcsUHF[i].Height = 20;
            dcsUHF[i].Margin = margin;
            dcsUHF[i].Minimum = 0;
            dcsUHF[i].Maximum = 255;
            dcsUHF[i].ValueChanged += new EventHandler(nmValueChanged);
            iGainDMRUHF[i] = new NumericUpDown();
            iGainDMRUHF[i].Width = 74;
            iGainDMRUHF[i].Height = 20;
            iGainDMRUHF[i].Margin = margin;
            iGainDMRUHF[i].Minimum = 0;
            iGainDMRUHF[i].Maximum = 255;
            iGainDMRUHF[i].ValueChanged += new EventHandler(nmValueChanged);
            qGainDMRUHF[i] = new NumericUpDown();
            qGainDMRUHF[i].Width = 74;
            qGainDMRUHF[i].Height = 20;
            qGainDMRUHF[i].Margin = margin;
            qGainDMRUHF[i].Minimum = 0;
            qGainDMRUHF[i].Maximum = 255;
            qGainDMRUHF[i].ValueChanged += new EventHandler(nmValueChanged);
            iGainFMUHF[i] = new NumericUpDown();
            iGainFMUHF[i].Width = 74;
            iGainFMUHF[i].Height = 20;
            iGainFMUHF[i].Margin = margin;
            iGainFMUHF[i].Minimum = 0;
            iGainFMUHF[i].Maximum = 255;
            iGainFMUHF[i].ValueChanged += new EventHandler(nmValueChanged);
            qGainFMUHF[i] = new NumericUpDown();
            qGainFMUHF[i].Width = 74;
            qGainFMUHF[i].Height = 20;
            qGainFMUHF[i].Margin = margin;
            qGainFMUHF[i].Minimum = 0;
            qGainFMUHF[i].Maximum = 255;
            qGainFMUHF[i].ValueChanged += new EventHandler(nmValueChanged);
            
            tlpUHF.Controls.Add(frequenciesTxUHF[i], i + 1, 0);
            tlpUHF.Controls.Add(maxPowersUHF[i], i + 1, 1);
            tlpUHF.Controls.Add(midPowersUHF[i], i + 1, 2);
            tlpUHF.Controls.Add(midLowPowersUHF[i], i + 1, 3);
            tlpUHF.Controls.Add(minPowersUHF[i], i + 1, 4);
            tlpUHF.Controls.Add(power4UHF[i], i + 1, 5);
            tlpUHF.Controls.Add(power3UHF[i], i + 1, 6);
            tlpUHF.Controls.Add(power2UHF[i], i + 1, 7);
            tlpUHF.Controls.Add(power1UHF[i], i + 1, 8);
            tlpUHF.Controls.Add(power0UHF[i], i + 1, 9);
            tlpUHF.Controls.Add(rxTuningUHF[i], i + 1, 10);
            tlpUHF.Controls.Add(OpenSQL9UHF[i], i + 1, 11);
            tlpUHF.Controls.Add(CloseSQL9UHF[i], i + 1, 12);
            tlpUHF.Controls.Add(OpenSQL1UHF[i], i + 1, 13);
            tlpUHF.Controls.Add(CloseSQL1UHF[i], i + 1, 14);
            tlpUHF.Controls.Add(ctc67UHF[i], i + 1, 15);
            tlpUHF.Controls.Add(ctc151UHF[i], i + 1, 16);
            tlpUHF.Controls.Add(ctc254UHF[i], i + 1, 17);
            tlpUHF.Controls.Add(dcsUHF[i], i + 1, 18);
            tlpUHF.Controls.Add(iGainDMRUHF[i], i + 1, 19);
            tlpUHF.Controls.Add(qGainDMRUHF[i], i + 1, 20);
            tlpUHF.Controls.Add(iGainFMUHF[i], i + 1, 21);
            tlpUHF.Controls.Add(qGainFMUHF[i], i + 1, 22);
        }
    }

    private bool isReading = false;

    private void nmValueChanged(Object sender, EventArgs e)
    {
         buildCalDataFromVariables(CalData);
         buildOldPowers(CalData);
    }

    private void buildVariablesFromCalData(CalibrationDataSTM32 c)
    {
        lblRadioType.Text = "Тип рации: ";
        switch (MainForm.RadioInfo.radioType)
        {
            case 5:
                lblRadioType.Text += "TYT MD-9600/Retevis RT-90";
                break;
            case 6:
                lblRadioType.Text += "TYT MD-UV380/TYT MD-UV390/Retevis RT-3S";
                break;
            case 8:
            case 10:
                lblRadioType.Text += "Baofeng DM-1701/Retevis RT-84";
                break;
            case 9:
                lblRadioType.Text += "TYT MD-2017/Retevis RT-82";
                break;
            case 106:
                lblRadioType.Text += "TYT MD-UV390 (10W Plus)";
                break;
            default:
                lblRadioType.Text = "";
                break;
        }
        isReading = true;
        nmVOXMinLevel.Value = c.VoxLevel1;
        nmVOXMaxLevel.Value = c.VoxLevel10;
        nmRxLowLevel.Value = c.RxLowVoltage;
        nmRxHighLevel.Value = c.RxHighVoltage;
        nmRSSI120.Value = c.RSSI120;
        nmRSSI70.Value = c.RSSI70;
        nmVHFOscRef.Value = c.VHFOscRefTune;
        nmUHFOscRef.Value = c.UHFOscRefTune;
        //tlpVHF


        for (int i = 0; i < 5; i++)
        {

            frequenciesTxVHF[i].Text = ((radioBandlimits.VHFLowCal + (i * 950000)) / 100000.0f).ToString("N3");
            maxPowersVHF[i].Value = (decimal)(c.VHFHighPowerCal[i].VALUE * 16);
            midPowersVHF[i].Value = (decimal)(c.VHFMidPowerCal[i].VALUE * 16);
            midLowPowersVHF[i].Value = (decimal)(c.VHFMidLowPowerCal[i].VALUE * 16);
            minPowersVHF[i].Value = (decimal)(c.VHFLowPowerCal[i].VALUE * 16);
            power4VHF[i].Value = (decimal)(c.VHFCalPower4[i]);
            power3VHF[i].Value = (decimal)(c.VHFCalPower3[i]);
            power2VHF[i].Value = (decimal)(c.VHFCalPower2[i]);
            power1VHF[i].Value = (decimal)(c.VHFCalPower1[i]);
            power0VHF[i].Value = (decimal)(c.VHFCalPower0[i]);
            rxTuningVHF[i].Value = (decimal)c.VHFRxTuning[i];
            OpenSQL9VHF[i].Value = (decimal)c.VHFOpenSquelch9[i];
            CloseSQL9VHF[i].Value = (decimal)c.VHFCloseSquelch9[i];
            OpenSQL1VHF[i].Value = (decimal)c.VHFOpenSquelch1[i];
            CloseSQL1VHF[i].Value = (decimal)c.VHFCloseSquelch1[i];
            ctc67VHF[i].Value = (decimal)c.VHFCTC67[i];
            ctc151VHF[i].Value = (decimal)c.VHFCTC151[i];
            ctc254VHF[i].Value = (decimal)c.VHFCTC254[i];
            dcsVHF[i].Value = (decimal)c.VHFDCS[i];
            iGainDMRVHF[i].Value = (decimal)c.VHFDMRIGain[i];
            qGainDMRVHF[i].Value = (decimal)c.VHFDMRQGain[i];
            iGainFMVHF[i].Value = (decimal)c.VHFFMIGain[i];
            qGainFMVHF[i].Value = (decimal)c.VHFFMQGain[i];
        }
        for (int i = 0; i < 9; i++)
        {

            frequenciesTxUHF[i].Text = ((radioBandlimits.UHFLowCal + (i * 1000000)) / 100000.0f).ToString("N3");
            maxPowersUHF[i].Value = (decimal)(c.UHFHighPowerCal[i].VALUE * 16);
            midPowersUHF[i].Value = (decimal)(c.UHFMidPowerCal[i].VALUE * 16);
            midLowPowersUHF[i].Value = (decimal)(c.UHFMidLowPowerCal[i].VALUE * 16);
            minPowersUHF[i].Value = (decimal)(c.UHFLowPowerCal[i].VALUE * 16);
            power4UHF[i].Value = (decimal)(c.UHFCalPower4[i]);
            power3UHF[i].Value = (decimal)(c.UHFCalPower3[i]);
            power2UHF[i].Value = (decimal)(c.UHFCalPower2[i]);
            power1UHF[i].Value = (decimal)(c.UHFCalPower1[i]);
            power0UHF[i].Value = (decimal)(c.UHFCalPower0[i]);
            rxTuningUHF[i].Value = (decimal)c.UHFRxTuning[i];
            OpenSQL9UHF[i].Value = (decimal)c.UHFOpenSquelch9[i];
            CloseSQL9UHF[i].Value = (decimal)c.UHFCloseSquelch9[i];
            OpenSQL1UHF[i].Value = (decimal)c.UHFOpenSquelch1[i];
            CloseSQL1UHF[i].Value = (decimal)c.UHFCloseSquelch1[i];
            ctc67UHF[i].Value = (decimal)c.UHFCTC67[i];
            ctc151UHF[i].Value = (decimal)c.UHFCTC151[i];
            ctc254UHF[i].Value = (decimal)c.UHFCTC254[i];
            dcsUHF[i].Value = (decimal)c.UHFDCS[i];
            iGainDMRUHF[i].Value = (decimal)c.UHFDMRIGain[i];
            qGainDMRUHF[i].Value = (decimal)c.UHFDMRQGain[i];
            iGainFMUHF[i].Value = (decimal)c.UHFFMIGain[i];
            qGainFMUHF[i].Value = (decimal)c.UHFFMQGain[i];
        }
        isReading = false;
    }

    private void buildOldPowers(CalibrationDataSTM32 c)
    {
        for (int i = 0; i < 5; i++)
        {
            maxPowersVHF[i].Value = (decimal)(c.VHFHighPowerCal[i].VALUE * 16);
            midPowersVHF[i].Value = (decimal)(c.VHFMidPowerCal[i].VALUE * 16);
            midLowPowersVHF[i].Value = (decimal)(c.VHFMidLowPowerCal[i].VALUE * 16);
            minPowersVHF[i].Value = (decimal)(c.VHFLowPowerCal[i].VALUE * 16);
        }
        for (int i = 0; i < 9; i++)
        {
            maxPowersUHF[i].Value = (decimal)(c.UHFHighPowerCal[i].VALUE * 16);
            midPowersUHF[i].Value = (decimal)(c.UHFMidPowerCal[i].VALUE * 16);
            midLowPowersUHF[i].Value = (decimal)(c.UHFMidLowPowerCal[i].VALUE * 16);
            minPowersUHF[i].Value = (decimal)(c.UHFLowPowerCal[i].VALUE * 16);
        }
    }
    private void buildCalDataFromVariables(CalibrationDataSTM32 c)
    {
        if (!isReading)
        {
            c.VoxLevel1 = (byte)nmVOXMinLevel.Value;
            c.VoxLevel10 = (byte)nmVOXMaxLevel.Value;
            c.RxLowVoltage = (byte)nmRxLowLevel.Value;
            c.RxHighVoltage = (byte)nmRxHighLevel.Value;
            c.RSSI120 = (byte)nmRSSI120.Value;
            c.RSSI70 = (byte)nmRSSI70.Value;
            c.VHFOscRefTune = (byte)nmVHFOscRef.Value;
            c.UHFOscRefTune = (byte)nmUHFOscRef.Value;
            for (int i = 0; i < 5; i++)
            {
                c.VHFHighPowerCal[i] = maxPowersVHF[i].Value / 16;
                c.VHFMidPowerCal[i] = midPowersVHF[i].Value / 16;
                c.VHFMidLowPowerCal[i] = midLowPowersVHF[i].Value / 16;
                c.VHFLowPowerCal[i] = minPowersVHF[i].Value / 16;
                c.VHFCalPower4[i] = (ushort)power4VHF[i].Value;
                c.VHFCalPower3[i] = (ushort)power3VHF[i].Value;
                c.VHFCalPower2[i] = (ushort)power2VHF[i].Value;
                c.VHFCalPower1[i] = (ushort)power1VHF[i].Value;
                c.VHFCalPower0[i] = (ushort)power0VHF[i].Value;
                c.VHFRxTuning[i] = rxTuningVHF[i].Value;
                c.VHFOpenSquelch9[i] = OpenSQL9VHF[i].Value;
                c.VHFCloseSquelch9[i] = CloseSQL9VHF[i].Value;
                c.VHFOpenSquelch1[i] = OpenSQL1VHF[i].Value;
                c.VHFCloseSquelch1[i] = CloseSQL1VHF[i].Value;
                c.VHFCTC67[i] = ctc67VHF[i].Value;
                c.VHFCTC151[i] = ctc151VHF[i].Value;
                c.VHFCTC254[i] = ctc254VHF[i].Value;
                c.VHFDCS[i] = dcsVHF[i].Value;
                c.VHFDMRIGain[i] = iGainDMRVHF[i].Value;
                c.VHFDMRQGain[i] = qGainDMRVHF[i].Value;
                c.VHFFMIGain[i] = iGainFMVHF[i].Value;
                c.VHFFMQGain[i] = qGainFMVHF[i].Value;
            }
            for (int i = 0; i < 9; i++)
            {
                c.UHFHighPowerCal[i] = maxPowersUHF[i].Value / 16;
                c.UHFMidPowerCal[i] = midPowersUHF[i].Value / 16;
                c.UHFMidLowPowerCal[i] = midLowPowersUHF[i].Value / 16;
                c.UHFLowPowerCal[i] = minPowersUHF[i].Value / 16;
                c.UHFCalPower4[i] = (ushort)power4UHF[i].Value;
                c.UHFCalPower3[i] = (ushort)power3UHF[i].Value;
                c.UHFCalPower2[i] = (ushort)power2UHF[i].Value;
                c.UHFCalPower1[i] = (ushort)power1UHF[i].Value;
                c.UHFCalPower0[i] = (ushort)power0UHF[i].Value;
                c.UHFRxTuning[i] = rxTuningUHF[i].Value;
                c.UHFOpenSquelch9[i] = OpenSQL9UHF[i].Value;
                c.UHFCloseSquelch9[i] = CloseSQL9UHF[i].Value;
                c.UHFOpenSquelch1[i] = OpenSQL1UHF[i].Value;
                c.UHFCloseSquelch1[i] = CloseSQL1UHF[i].Value;
                c.UHFCTC67[i] = ctc67UHF[i].Value;
                c.UHFCTC151[i] = ctc151UHF[i].Value;
                c.UHFCTC254[i] = ctc254UHF[i].Value;
                c.UHFDCS[i] = dcsUHF[i].Value;
                c.UHFDMRIGain[i] = iGainDMRUHF[i].Value;
                c.UHFDMRQGain[i] = qGainDMRUHF[i].Value;
                c.UHFFMIGain[i] = iGainFMUHF[i].Value;
                c.UHFFMQGain[i] = qGainFMUHF[i].Value;
            }
            // {0xCD, 0xE8, 0xEA, 0xE0};
            c.MARKER[0] = 0xCD;
            c.MARKER[1] = 0xE8;
            c.MARKER[2] = 0xEA;
            c.MARKER[3] = 0xE0;
        }
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
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnReadFile = new System.Windows.Forms.Button();
            this.btnReadFromRadio = new System.Windows.Forms.Button();
            this.btnSaveCalibration = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabVHF = new System.Windows.Forms.TabPage();
            this.nmVHFOscRef = new System.Windows.Forms.NumericUpDown();
            this.label34 = new System.Windows.Forms.Label();
            this.tlpVHF = new System.Windows.Forms.TableLayoutPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.tabUHF = new System.Windows.Forms.TabPage();
            this.nmUHFOscRef = new System.Windows.Forms.NumericUpDown();
            this.label35 = new System.Windows.Forms.Label();
            this.tlpUHF = new System.Windows.Forms.TableLayoutPanel();
            this.label21 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.gbCommons = new System.Windows.Forms.GroupBox();
            this.nmRSSI70 = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.nmRSSI120 = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.nmRxHighLevel = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nmRxLowLevel = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nmVOXMaxLevel = new System.Windows.Forms.NumericUpDown();
            this.nmVOXMinLevel = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReadFactoryFromRadio = new System.Windows.Forms.Button();
            this.lblRadioType = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.label54 = new System.Windows.Forms.Label();
            this.tabs.SuspendLayout();
            this.tabVHF.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmVHFOscRef)).BeginInit();
            this.tlpVHF.SuspendLayout();
            this.tabUHF.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmUHFOscRef)).BeginInit();
            this.tlpUHF.SuspendLayout();
            this.gbCommons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmRSSI70)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRSSI120)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRxHighLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRxLowLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmVOXMaxLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmVOXMinLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // btnWrite
            // 
            this.btnWrite.BackColor = System.Drawing.SystemColors.Control;
            this.btnWrite.Font = new System.Drawing.Font("Arial", 8F);
            this.btnWrite.Location = new System.Drawing.Point(877, 342);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(268, 23);
            this.btnWrite.TabIndex = 1;
            this.btnWrite.Text = "Write to radio";
            this.btnWrite.UseVisualStyleBackColor = false;
            this.btnWrite.Visible = false;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // btnReadFile
            // 
            this.btnReadFile.BackColor = System.Drawing.SystemColors.Control;
            this.btnReadFile.Font = new System.Drawing.Font("Arial", 8F);
            this.btnReadFile.Location = new System.Drawing.Point(877, 313);
            this.btnReadFile.Name = "btnReadFile";
            this.btnReadFile.Size = new System.Drawing.Size(268, 23);
            this.btnReadFile.TabIndex = 1;
            this.btnReadFile.Text = "Open Calibration file";
            this.btnReadFile.UseVisualStyleBackColor = false;
            this.btnReadFile.Click += new System.EventHandler(this.btnReadFile_Click);
            // 
            // btnReadFromRadio
            // 
            this.btnReadFromRadio.BackColor = System.Drawing.SystemColors.Control;
            this.btnReadFromRadio.Font = new System.Drawing.Font("Arial", 8F);
            this.btnReadFromRadio.Location = new System.Drawing.Point(877, 284);
            this.btnReadFromRadio.Name = "btnReadFromRadio";
            this.btnReadFromRadio.Size = new System.Drawing.Size(268, 23);
            this.btnReadFromRadio.TabIndex = 1;
            this.btnReadFromRadio.Text = "Read calibration from radio";
            this.btnReadFromRadio.UseVisualStyleBackColor = false;
            this.btnReadFromRadio.Click += new System.EventHandler(this.btnReadFromRadio_Click);
            // 
            // btnSaveCalibration
            // 
            this.btnSaveCalibration.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveCalibration.Font = new System.Drawing.Font("Arial", 8F);
            this.btnSaveCalibration.Location = new System.Drawing.Point(877, 371);
            this.btnSaveCalibration.Name = "btnSaveCalibration";
            this.btnSaveCalibration.Size = new System.Drawing.Size(268, 23);
            this.btnSaveCalibration.TabIndex = 1;
            this.btnSaveCalibration.Text = "Save Calibration file";
            this.btnSaveCalibration.UseVisualStyleBackColor = false;
            this.btnSaveCalibration.Visible = false;
            this.btnSaveCalibration.Click += new System.EventHandler(this.btnSaveCalibration_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Файлы калибровок|*.ogdc";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Файлы калибровки|*.ogdc";
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabVHF);
            this.tabs.Controls.Add(this.tabUHF);
            this.tabs.Location = new System.Drawing.Point(14, 12);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(857, 628);
            this.tabs.TabIndex = 2;
            // 
            // tabVHF
            // 
            this.tabVHF.Controls.Add(this.nmVHFOscRef);
            this.tabVHF.Controls.Add(this.label34);
            this.tabVHF.Controls.Add(this.tlpVHF);
            this.tabVHF.Location = new System.Drawing.Point(4, 22);
            this.tabVHF.Name = "tabVHF";
            this.tabVHF.Padding = new System.Windows.Forms.Padding(3);
            this.tabVHF.Size = new System.Drawing.Size(849, 602);
            this.tabVHF.TabIndex = 0;
            this.tabVHF.Text = "Диапазон 2 м";
            this.tabVHF.UseVisualStyleBackColor = true;
            // 
            // nmVHFOscRef
            // 
            this.nmVHFOscRef.Location = new System.Drawing.Point(285, 528);
            this.nmVHFOscRef.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nmVHFOscRef.Name = "nmVHFOscRef";
            this.nmVHFOscRef.Size = new System.Drawing.Size(89, 20);
            this.nmVHFOscRef.TabIndex = 9;
            this.nmVHFOscRef.ValueChanged += new System.EventHandler(this.nmValueChanged);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(161, 532);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(112, 13);
            this.label34.TabIndex = 8;
            this.label34.Text = "Подстройка частоты";
            // 
            // tlpVHF
            // 
            this.tlpVHF.BackColor = System.Drawing.SystemColors.Window;
            this.tlpVHF.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlpVHF.ColumnCount = 6;
            this.tlpVHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlpVHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tlpVHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tlpVHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tlpVHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tlpVHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpVHF.Controls.Add(this.label8, 0, 0);
            this.tlpVHF.Controls.Add(this.label9, 0, 1);
            this.tlpVHF.Controls.Add(this.label10, 0, 2);
            this.tlpVHF.Controls.Add(this.label11, 0, 3);
            this.tlpVHF.Controls.Add(this.label12, 0, 4);
            this.tlpVHF.Controls.Add(this.label16, 0, 10);
            this.tlpVHF.Controls.Add(this.label22, 0, 11);
            this.tlpVHF.Controls.Add(this.label23, 0, 12);
            this.tlpVHF.Controls.Add(this.label24, 0, 13);
            this.tlpVHF.Controls.Add(this.label25, 0, 14);
            this.tlpVHF.Controls.Add(this.label26, 0, 15);
            this.tlpVHF.Controls.Add(this.label27, 0, 16);
            this.tlpVHF.Controls.Add(this.label28, 0, 17);
            this.tlpVHF.Controls.Add(this.label29, 0, 18);
            this.tlpVHF.Controls.Add(this.label30, 0, 19);
            this.tlpVHF.Controls.Add(this.label31, 0, 20);
            this.tlpVHF.Controls.Add(this.label32, 0, 21);
            this.tlpVHF.Controls.Add(this.label33, 0, 22);
            this.tlpVHF.Controls.Add(this.label45, 0, 5);
            this.tlpVHF.Controls.Add(this.label46, 0, 6);
            this.tlpVHF.Controls.Add(this.label47, 0, 7);
            this.tlpVHF.Controls.Add(this.label48, 0, 8);
            this.tlpVHF.Controls.Add(this.label49, 0, 9);
            this.tlpVHF.Location = new System.Drawing.Point(157, 29);
            this.tlpVHF.Margin = new System.Windows.Forms.Padding(0);
            this.tlpVHF.Name = "tlpVHF";
            this.tlpVHF.RowCount = 24;
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpVHF.Size = new System.Drawing.Size(526, 484);
            this.tlpVHF.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(1, 1);
            this.label8.Margin = new System.Windows.Forms.Padding(0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(150, 20);
            this.label8.TabIndex = 1;
            this.label8.Text = "Частота Tx";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Location = new System.Drawing.Point(4, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(144, 20);
            this.label9.TabIndex = 2;
            this.label9.Text = "Уровень мощности 9";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Location = new System.Drawing.Point(4, 43);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(144, 20);
            this.label10.TabIndex = 3;
            this.label10.Text = "Уровень мощности 8";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Location = new System.Drawing.Point(4, 64);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(144, 20);
            this.label11.TabIndex = 4;
            this.label11.Text = "Уровень мощности 7";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label12
            // 
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.Location = new System.Drawing.Point(4, 85);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(144, 20);
            this.label12.TabIndex = 5;
            this.label12.Text = "Уровень мощности 6";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label16
            // 
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.Location = new System.Drawing.Point(4, 211);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(144, 20);
            this.label16.TabIndex = 6;
            this.label16.Text = "Настройка Rx";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label22
            // 
            this.label22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label22.Location = new System.Drawing.Point(4, 232);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(144, 20);
            this.label22.TabIndex = 7;
            this.label22.Text = "Открытие SQL 9";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label23
            // 
            this.label23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label23.Location = new System.Drawing.Point(4, 253);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(144, 20);
            this.label23.TabIndex = 8;
            this.label23.Text = "Закрытие SQL 9";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label24
            // 
            this.label24.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label24.Location = new System.Drawing.Point(4, 274);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(144, 20);
            this.label24.TabIndex = 9;
            this.label24.Text = "Открытие SQL 1";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label25
            // 
            this.label25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label25.Location = new System.Drawing.Point(4, 295);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(144, 20);
            this.label25.TabIndex = 10;
            this.label25.Text = "Закрытие SQL 1";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label26
            // 
            this.label26.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label26.Location = new System.Drawing.Point(4, 316);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(144, 20);
            this.label26.TabIndex = 11;
            this.label26.Text = "Девиация CTCSS 67";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label27
            // 
            this.label27.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label27.Location = new System.Drawing.Point(4, 337);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(144, 20);
            this.label27.TabIndex = 12;
            this.label27.Text = "Девиация CTCSS 151";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label28
            // 
            this.label28.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label28.Location = new System.Drawing.Point(4, 358);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(144, 20);
            this.label28.TabIndex = 13;
            this.label28.Text = "Девиация CTCSS 254";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label29
            // 
            this.label29.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label29.Location = new System.Drawing.Point(4, 379);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(144, 20);
            this.label29.TabIndex = 14;
            this.label29.Text = "Девиация DCS";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label30
            // 
            this.label30.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label30.Location = new System.Drawing.Point(4, 400);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(144, 20);
            this.label30.TabIndex = 15;
            this.label30.Text = "Усиление I (DMR)";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label31
            // 
            this.label31.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label31.Location = new System.Drawing.Point(4, 421);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(144, 20);
            this.label31.TabIndex = 16;
            this.label31.Text = "Усиление Q (DMR)";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label32
            // 
            this.label32.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label32.Location = new System.Drawing.Point(4, 442);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(144, 20);
            this.label32.TabIndex = 17;
            this.label32.Text = "Усиление I (FM)";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label33
            // 
            this.label33.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label33.Location = new System.Drawing.Point(4, 463);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(144, 20);
            this.label33.TabIndex = 18;
            this.label33.Text = "Усиление Q (FM)";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabUHF
            // 
            this.tabUHF.Controls.Add(this.nmUHFOscRef);
            this.tabUHF.Controls.Add(this.label35);
            this.tabUHF.Controls.Add(this.tlpUHF);
            this.tabUHF.Location = new System.Drawing.Point(4, 22);
            this.tabUHF.Name = "tabUHF";
            this.tabUHF.Padding = new System.Windows.Forms.Padding(3);
            this.tabUHF.Size = new System.Drawing.Size(849, 602);
            this.tabUHF.TabIndex = 1;
            this.tabUHF.Text = "Диапазон 70 см";
            this.tabUHF.UseVisualStyleBackColor = true;
            // 
            // nmUHFOscRef
            // 
            this.nmUHFOscRef.Location = new System.Drawing.Point(140, 536);
            this.nmUHFOscRef.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nmUHFOscRef.Name = "nmUHFOscRef";
            this.nmUHFOscRef.Size = new System.Drawing.Size(89, 20);
            this.nmUHFOscRef.TabIndex = 16;
            this.nmUHFOscRef.ValueChanged += new System.EventHandler(this.nmValueChanged);
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(16, 540);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(112, 13);
            this.label35.TabIndex = 15;
            this.label35.Text = "Подстройка частоты";
            // 
            // tlpUHF
            // 
            this.tlpUHF.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlpUHF.ColumnCount = 10;
            this.tlpUHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlpUHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tlpUHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tlpUHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tlpUHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tlpUHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tlpUHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tlpUHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tlpUHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tlpUHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.tlpUHF.Controls.Add(this.label21, 0, 18);
            this.tlpUHF.Controls.Add(this.label40, 0, 0);
            this.tlpUHF.Controls.Add(this.label41, 0, 1);
            this.tlpUHF.Controls.Add(this.label42, 0, 2);
            this.tlpUHF.Controls.Add(this.label43, 0, 3);
            this.tlpUHF.Controls.Add(this.label44, 0, 4);
            this.tlpUHF.Controls.Add(this.label7, 0, 10);
            this.tlpUHF.Controls.Add(this.label13, 0, 11);
            this.tlpUHF.Controls.Add(this.label14, 0, 12);
            this.tlpUHF.Controls.Add(this.label15, 0, 13);
            this.tlpUHF.Controls.Add(this.label17, 0, 14);
            this.tlpUHF.Controls.Add(this.label18, 0, 15);
            this.tlpUHF.Controls.Add(this.label19, 0, 16);
            this.tlpUHF.Controls.Add(this.label20, 0, 17);
            this.tlpUHF.Controls.Add(this.label36, 0, 19);
            this.tlpUHF.Controls.Add(this.label37, 0, 20);
            this.tlpUHF.Controls.Add(this.label38, 0, 21);
            this.tlpUHF.Controls.Add(this.label39, 0, 22);
            this.tlpUHF.Controls.Add(this.label50, 0, 5);
            this.tlpUHF.Controls.Add(this.label51, 0, 6);
            this.tlpUHF.Controls.Add(this.label52, 0, 7);
            this.tlpUHF.Controls.Add(this.label53, 0, 8);
            this.tlpUHF.Controls.Add(this.label54, 0, 9);
            this.tlpUHF.Location = new System.Drawing.Point(12, 16);
            this.tlpUHF.Name = "tlpUHF";
            this.tlpUHF.RowCount = 24;
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpUHF.Size = new System.Drawing.Size(827, 484);
            this.tlpUHF.TabIndex = 0;
            // 
            // label21
            // 
            this.label21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label21.Location = new System.Drawing.Point(4, 379);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(144, 20);
            this.label21.TabIndex = 17;
            this.label21.Text = "Девиация DCS";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label40
            // 
            this.label40.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label40.Location = new System.Drawing.Point(4, 1);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(144, 20);
            this.label40.TabIndex = 1;
            this.label40.Text = "Частота Tx";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label41
            // 
            this.label41.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label41.Location = new System.Drawing.Point(4, 22);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(144, 20);
            this.label41.TabIndex = 2;
            this.label41.Text = "Уровень мощности 9";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label42
            // 
            this.label42.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label42.Location = new System.Drawing.Point(4, 43);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(144, 20);
            this.label42.TabIndex = 3;
            this.label42.Text = "Уровень мощности 8";
            this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label43
            // 
            this.label43.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label43.Location = new System.Drawing.Point(4, 64);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(144, 20);
            this.label43.TabIndex = 4;
            this.label43.Text = "Уровень мощности 7";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label44
            // 
            this.label44.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label44.Location = new System.Drawing.Point(4, 85);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(144, 20);
            this.label44.TabIndex = 5;
            this.label44.Text = "Уровень мощности 6";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(4, 211);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(144, 20);
            this.label7.TabIndex = 6;
            this.label7.Text = "Настройка Rx";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.Location = new System.Drawing.Point(4, 232);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(144, 20);
            this.label13.TabIndex = 7;
            this.label13.Text = "Открытие SQL 9";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label14.Location = new System.Drawing.Point(4, 253);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(144, 20);
            this.label14.TabIndex = 8;
            this.label14.Text = "Закрытие SQL 9";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label15.Location = new System.Drawing.Point(4, 274);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(144, 20);
            this.label15.TabIndex = 9;
            this.label15.Text = "Открытие SQL 1";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label17.Location = new System.Drawing.Point(4, 295);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(144, 20);
            this.label17.TabIndex = 10;
            this.label17.Text = "Закрытие SQL 1";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label18
            // 
            this.label18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label18.Location = new System.Drawing.Point(4, 316);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(144, 20);
            this.label18.TabIndex = 12;
            this.label18.Text = "Девиация CTCSS 67";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label19
            // 
            this.label19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label19.Location = new System.Drawing.Point(4, 337);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(144, 20);
            this.label19.TabIndex = 13;
            this.label19.Text = "Девиация CTCSS 151";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label20
            // 
            this.label20.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label20.Location = new System.Drawing.Point(4, 358);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(144, 20);
            this.label20.TabIndex = 14;
            this.label20.Text = "Девиация CTCSS 254";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label36
            // 
            this.label36.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label36.Location = new System.Drawing.Point(4, 400);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(144, 20);
            this.label36.TabIndex = 18;
            this.label36.Text = "Усиление I (DMR)";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label37
            // 
            this.label37.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label37.Location = new System.Drawing.Point(4, 421);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(144, 20);
            this.label37.TabIndex = 19;
            this.label37.Text = "Усиление Q (DMR)";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label38
            // 
            this.label38.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label38.Location = new System.Drawing.Point(4, 442);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(144, 20);
            this.label38.TabIndex = 20;
            this.label38.Text = "Усиление I (FM)";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label39
            // 
            this.label39.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label39.Location = new System.Drawing.Point(4, 463);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(144, 20);
            this.label39.TabIndex = 21;
            this.label39.Text = "Усиление Q (FM)";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gbCommons
            // 
            this.gbCommons.Controls.Add(this.nmRSSI70);
            this.gbCommons.Controls.Add(this.label6);
            this.gbCommons.Controls.Add(this.nmRSSI120);
            this.gbCommons.Controls.Add(this.label5);
            this.gbCommons.Controls.Add(this.nmRxHighLevel);
            this.gbCommons.Controls.Add(this.label4);
            this.gbCommons.Controls.Add(this.nmRxLowLevel);
            this.gbCommons.Controls.Add(this.label3);
            this.gbCommons.Controls.Add(this.nmVOXMaxLevel);
            this.gbCommons.Controls.Add(this.nmVOXMinLevel);
            this.gbCommons.Controls.Add(this.label2);
            this.gbCommons.Controls.Add(this.label1);
            this.gbCommons.Location = new System.Drawing.Point(877, 12);
            this.gbCommons.Name = "gbCommons";
            this.gbCommons.Size = new System.Drawing.Size(268, 201);
            this.gbCommons.TabIndex = 3;
            this.gbCommons.TabStop = false;
            this.gbCommons.Text = "Общие настройки";
            // 
            // nmRSSI70
            // 
            this.nmRSSI70.Location = new System.Drawing.Point(179, 162);
            this.nmRSSI70.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nmRSSI70.Name = "nmRSSI70";
            this.nmRSSI70.Size = new System.Drawing.Size(66, 20);
            this.nmRSSI70.TabIndex = 11;
            this.nmRSSI70.ValueChanged += new System.EventHandler(this.nmValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 164);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(124, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Уровень RSSI -70 дБм:";
            // 
            // nmRSSI120
            // 
            this.nmRSSI120.Location = new System.Drawing.Point(179, 133);
            this.nmRSSI120.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nmRSSI120.Name = "nmRSSI120";
            this.nmRSSI120.Size = new System.Drawing.Size(67, 20);
            this.nmRSSI120.TabIndex = 9;
            this.nmRSSI120.ValueChanged += new System.EventHandler(this.nmValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 135);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Уровень RSSI -120 дБм:";
            // 
            // nmRxHighLevel
            // 
            this.nmRxHighLevel.Location = new System.Drawing.Point(179, 105);
            this.nmRxHighLevel.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nmRxHighLevel.Name = "nmRxHighLevel";
            this.nmRxHighLevel.Size = new System.Drawing.Size(69, 20);
            this.nmRxHighLevel.TabIndex = 7;
            this.nmRxHighLevel.ValueChanged += new System.EventHandler(this.nmValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(165, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Верхний порог напряжения Rx:";
            // 
            // nmRxLowLevel
            // 
            this.nmRxLowLevel.Location = new System.Drawing.Point(179, 78);
            this.nmRxLowLevel.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nmRxLowLevel.Name = "nmRxLowLevel";
            this.nmRxLowLevel.Size = new System.Drawing.Size(69, 20);
            this.nmRxLowLevel.TabIndex = 5;
            this.nmRxLowLevel.ValueChanged += new System.EventHandler(this.nmValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(163, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Нижний порог напряжения Rx:";
            // 
            // nmVOXMaxLevel
            // 
            this.nmVOXMaxLevel.Location = new System.Drawing.Point(179, 51);
            this.nmVOXMaxLevel.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nmVOXMaxLevel.Name = "nmVOXMaxLevel";
            this.nmVOXMaxLevel.Size = new System.Drawing.Size(68, 20);
            this.nmVOXMaxLevel.TabIndex = 3;
            this.nmVOXMaxLevel.ValueChanged += new System.EventHandler(this.nmValueChanged);
            // 
            // nmVOXMinLevel
            // 
            this.nmVOXMinLevel.Location = new System.Drawing.Point(179, 25);
            this.nmVOXMinLevel.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nmVOXMinLevel.Name = "nmVOXMinLevel";
            this.nmVOXMinLevel.Size = new System.Drawing.Size(69, 20);
            this.nmVOXMinLevel.TabIndex = 2;
            this.nmVOXMinLevel.ValueChanged += new System.EventHandler(this.nmValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(158, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Максимальный уровень VOX:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Минимальный уровень VOX:";
            // 
            // btnReadFactoryFromRadio
            // 
            this.btnReadFactoryFromRadio.BackColor = System.Drawing.SystemColors.Control;
            this.btnReadFactoryFromRadio.Location = new System.Drawing.Point(877, 472);
            this.btnReadFactoryFromRadio.Name = "btnReadFactoryFromRadio";
            this.btnReadFactoryFromRadio.Size = new System.Drawing.Size(268, 23);
            this.btnReadFactoryFromRadio.TabIndex = 5;
            this.btnReadFactoryFromRadio.Text = "button1";
            this.btnReadFactoryFromRadio.UseVisualStyleBackColor = false;
            this.btnReadFactoryFromRadio.Click += new System.EventHandler(this.btnReadFactoryFromRadio_Click);
            // 
            // lblRadioType
            // 
            this.lblRadioType.Location = new System.Drawing.Point(559, 5);
            this.lblRadioType.Name = "lblRadioType";
            this.lblRadioType.Size = new System.Drawing.Size(308, 22);
            this.lblRadioType.TabIndex = 9;
            this.lblRadioType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label45.Location = new System.Drawing.Point(4, 106);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(144, 20);
            this.label45.TabIndex = 19;
            this.label45.Text = "Уровень мощности 5";
            this.label45.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label46.Location = new System.Drawing.Point(4, 127);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(144, 20);
            this.label46.TabIndex = 20;
            this.label46.Text = "Уровень мощности 4";
            this.label46.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label47.Location = new System.Drawing.Point(4, 148);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(144, 20);
            this.label47.TabIndex = 21;
            this.label47.Text = "Уровень мощности 3";
            this.label47.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label48.Location = new System.Drawing.Point(4, 169);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(144, 20);
            this.label48.TabIndex = 22;
            this.label48.Text = "Уровень мощности 2";
            this.label48.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label49.Location = new System.Drawing.Point(4, 190);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(144, 20);
            this.label49.TabIndex = 23;
            this.label49.Text = "Уровень мощности 1";
            this.label49.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label50.Location = new System.Drawing.Point(4, 106);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(144, 20);
            this.label50.TabIndex = 22;
            this.label50.Text = "Уровень мощности 5";
            this.label50.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label51.Location = new System.Drawing.Point(4, 127);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(144, 20);
            this.label51.TabIndex = 23;
            this.label51.Text = "Уровень мощности 4";
            this.label51.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label52.Location = new System.Drawing.Point(4, 148);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(144, 20);
            this.label52.TabIndex = 24;
            this.label52.Text = "Уровень мощности 3";
            this.label52.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label53.Location = new System.Drawing.Point(4, 169);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(144, 20);
            this.label53.TabIndex = 25;
            this.label53.Text = "Уровень мощности 2";
            this.label53.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label54.Location = new System.Drawing.Point(4, 190);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(144, 20);
            this.label54.TabIndex = 26;
            this.label54.Text = "Уровень мощности 1";
            this.label54.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CalibrationFormMDUV380
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1156, 673);
            this.Controls.Add(this.lblRadioType);
            this.Controls.Add(this.btnReadFactoryFromRadio);
            this.Controls.Add(this.gbCommons);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.btnReadFromRadio);
            this.Controls.Add(this.btnSaveCalibration);
            this.Controls.Add(this.btnReadFile);
            this.Controls.Add(this.btnWrite);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalibrationFormMDUV380";
            this.Text = "Calibration";
            this.Load += new System.EventHandler(this.onFormLoad);
            this.tabs.ResumeLayout(false);
            this.tabVHF.ResumeLayout(false);
            this.tabVHF.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmVHFOscRef)).EndInit();
            this.tlpVHF.ResumeLayout(false);
            this.tlpVHF.PerformLayout();
            this.tabUHF.ResumeLayout(false);
            this.tabUHF.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmUHFOscRef)).EndInit();
            this.tlpUHF.ResumeLayout(false);
            this.tlpUHF.PerformLayout();
            this.gbCommons.ResumeLayout(false);
            this.gbCommons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmRSSI70)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRSSI120)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRxHighLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRxLowLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmVOXMaxLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmVOXMinLevel)).EndInit();
            this.ResumeLayout(false);

    }

    
 
}
