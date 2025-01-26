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
    private Label label7;
    private Label label8;
    private Label label9;
    private Label label10;
    private Label label11;
    private Label label12;
    private Button btnReadFactoryFromRadio;
    private Label label14;
    private Label label13;
    private TextBox tbVHFMax;
    private TextBox tbVHFMin;
    private TextBox tbVHFMinCal;
    private Label label15;
    private Label lblCalc;
    private GroupBox gbCalcVHF;
    private NumericUpDown nmCalc;
    private Label label17;
    private Button btnCalc;
    private TextBox tbVHFHighPower;
    private Label label18;
    private TextBox tbVHFMidPower;
    private Label label19;
    private TextBox tbVHFMidLowPower;
    private Label label20;
    private TextBox tbVHFLowPower;
    private Label label21;
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

    private int IntFromBCDData(BCDData bcd)
    {
        int result = 0;
        result += (10 * (bcd.byte4 >> 4));
        result += bcd.byte4 & 0xf;
        result += (1000 * (bcd.byte3 >> 4));
        result += (100 * (bcd.byte3 & 0xf));
        result += (100000 * (bcd.byte2 >> 4));
        result += (10000 * (bcd.byte2 & 0xf));
        result += (10000000 * (bcd.byte1 >> 4));
        result += (1000000 * (bcd.byte2 & 0xf));
        return result;
    }

    private string IntToFrequency(int f)
    {
        return (f / 100000.0f).ToString();
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

    public bool readDataFromRadio(bool factory = false)
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
        if (factory)
            openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadFactoryCalibrations;
        else
            openGD77CommsTransferData.mode = OpenGD77CommsTransferData.CommsDataMode.DataModeReadFlash;
        openGD77CommsTransferData.dataBuff = new byte[CALIBRATION_DATA_SIZE_STM32];
        openGD77CommsTransferData.localDataBufferStartPosition = 0;
        openGD77CommsTransferData.startDataAddressInTheRadio = MEMORY_LOCATION_STM32;
        openGD77CommsTransferData.transferLength = CALIBRATION_DATA_SIZE_STM32;
        if (factory)
        {
            if (!ReadData(commPort, openGD77CommsTransferData))
            {
                result = false;
                openGD77CommsTransferData.responseCode = 1;
            }
            else
            {
                SystemSounds.Exclamation.Play();
            }
        }
        else
        {
            if (!ReadFlashOrEEPROM(commPort, openGD77CommsTransferData))
            {
                result = false;
                openGD77CommsTransferData.responseCode = 1;
            }
            else
            {
                SystemSounds.Exclamation.Play();
            }
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
        openGD77CommsTransferData.startDataAddressInTheRadio = ((MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32) ? MEMORY_LOCATION_STM32 : MEMORY_LOCATION);
        openGD77CommsTransferData.transferLength = ((MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32) ? CALIBRATION_DATA_SIZE_STM32 : CALIBRATION_DATA_SIZE);
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
            //MessageBox.Show(StringsDict["NowSaveCalibrations"]);
            showButtons();
        }
        if (readBandlimits())
        {
            tbVHFMinCal.Text = (radioBandlimits.VHFLowCal / 100000.0f).ToString("N3");
            tbVHFMin.Text = (radioBandlimits.VHFLow / 100000.0f).ToString("N3");
            tbVHFMax.Text = (radioBandlimits.VHFHigh / 100000.0f).ToString("N3");
        }
    }

    private void btnReadFactoryFromRadio_Click(object sender, EventArgs e)
    {
        if (readDataFromRadio(true))
        {
            //MessageBox.Show(StringsDict["NowSaveCalibrations"]);
            showButtons();
        }
        if (readBandlimits())
        {
            tbVHFMinCal.Text = (radioBandlimits.VHFLowCal / 100000.0f).ToString("N3");
            tbVHFMin.Text = (radioBandlimits.VHFLow / 100000.0f).ToString("N3");
            tbVHFMax.Text = (radioBandlimits.VHFHigh / 100000.0f).ToString("N3");
        }
    }

    private void showButtons()
    {
        btnWrite.Visible = true;
        btnSaveCalibration.Visible = true;
        gbCalcVHF.Visible = true;
        lblCalc.Visible = true;
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

    private TextBox[] frequenciesRxVHF = new TextBox[5];
    private TextBox[] frequenciesTxVHF = new TextBox[5];
    private NumericUpDown[] maxPowersVHF = new NumericUpDown[5];
    private NumericUpDown[] midPowersVHF = new NumericUpDown[5];
    private NumericUpDown[] midLowPowersVHF = new NumericUpDown[5];
    private NumericUpDown[] minPowersVHF = new NumericUpDown[5];

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
            frequenciesRxVHF[i] = new TextBox();
            frequenciesRxVHF[i].Width = 74;
            frequenciesRxVHF[i].Height = 20;
            frequenciesRxVHF[i].Margin = margin;
            frequenciesRxVHF[i].ReadOnly = true;
            maxPowersVHF[i] = new NumericUpDown();
            maxPowersVHF[i].Width = 74;
            maxPowersVHF[i].Height = 20;
            maxPowersVHF[i].Margin = margin;
            maxPowersVHF[i].Minimum = 0;
            maxPowersVHF[i].Maximum = 255;
            midPowersVHF[i] = new NumericUpDown();
            midPowersVHF[i].Width = 74;
            midPowersVHF[i].Height = 20;
            midPowersVHF[i].Margin = margin;
            midPowersVHF[i].Minimum = 0;
            midPowersVHF[i].Maximum = 255;
            midLowPowersVHF[i] = new NumericUpDown();
            midLowPowersVHF[i].Width = 74;
            midLowPowersVHF[i].Height = 20;
            midLowPowersVHF[i].Margin = margin;
            midLowPowersVHF[i].Minimum = 0;
            midLowPowersVHF[i].Maximum = 255;
            minPowersVHF[i] = new NumericUpDown();
            minPowersVHF[i].Width = 74;
            minPowersVHF[i].Height = 20;
            minPowersVHF[i].Margin = margin;
            minPowersVHF[i].Minimum = 0;
            minPowersVHF[i].Maximum = 255;
            tlpVHF.Controls.Add(frequenciesRxVHF[i], i + 1, 0);
            tlpVHF.Controls.Add(frequenciesTxVHF[i], i + 1, 1);
            tlpVHF.Controls.Add(maxPowersVHF[i], i + 1, 2);
            tlpVHF.Controls.Add(midPowersVHF[i], i + 1, 3);
            tlpVHF.Controls.Add(midLowPowersVHF[i], i + 1, 4);
            tlpVHF.Controls.Add(minPowersVHF[i], i + 1, 5);
        }
    }

    private void buildVariablesFromCalData(CalibrationDataSTM32 c)
    {
        nmVOXMinLevel.Value = c.VoxLevel1;
        nmVOXMaxLevel.Value = c.VoxLevel10;
        nmRxLowLevel.Value = c.RxLowVoltage;
        nmRxHighLevel.Value = c.RxHighVoltage;
        nmRSSI120.Value = c.RSSI120;
        nmRSSI70.Value = c.RSSI70;
        //tlpVHF

        for (int i = 0; i < 5; i ++)
        {
            frequenciesRxVHF[i].Text = IntToFrequency(IntFromBCDData(CalData.VHFCalFreqs[i * 2]));
            frequenciesTxVHF[i].Text = IntToFrequency(IntFromBCDData(CalData.VHFCalFreqs[i * 2 + 1]));
            maxPowersVHF[i].Value = (decimal)CalData.VHFHighPowerCal[i];
            midPowersVHF[i].Value = (decimal)CalData.VHFMidPowerCal[i];
            midLowPowersVHF[i].Value = (decimal)CalData.VHFMidLowPowerCal[i];
            minPowersVHF[i].Value = (decimal)CalData.VHFLowPowerCal[i];
        }
    }

    private void buildCalDataFromVariables(CalibrationDataSTM32 c)
    {
        c.VoxLevel1 = (byte)nmVOXMinLevel.Value;
        c.VoxLevel10 = (byte)nmVOXMaxLevel.Value;
        c.RxLowVoltage = (byte)nmRxLowLevel.Value;
        c.RxHighVoltage = (byte)nmRxHighLevel.Value;
        c.RSSI120 = (byte)nmRSSI120.Value;
        c.RSSI70 = (byte)nmRSSI70.Value;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalibrationFormMDUV380));
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnReadFile = new System.Windows.Forms.Button();
            this.btnReadFromRadio = new System.Windows.Forms.Button();
            this.btnSaveCalibration = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabVHF = new System.Windows.Forms.TabPage();
            this.tlpVHF = new System.Windows.Forms.TableLayoutPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tabUHF = new System.Windows.Forms.TabPage();
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
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.tbVHFMinCal = new System.Windows.Forms.TextBox();
            this.tbVHFMin = new System.Windows.Forms.TextBox();
            this.tbVHFMax = new System.Windows.Forms.TextBox();
            this.lblCalc = new System.Windows.Forms.Label();
            this.gbCalcVHF = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.nmCalc = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.tbVHFHighPower = new System.Windows.Forms.TextBox();
            this.btnCalc = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.tbVHFMidPower = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.tbVHFMidLowPower = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.tbVHFLowPower = new System.Windows.Forms.TextBox();
            this.tabs.SuspendLayout();
            this.tabVHF.SuspendLayout();
            this.tlpVHF.SuspendLayout();
            this.gbCommons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmRSSI70)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRSSI120)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRxHighLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRxLowLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmVOXMaxLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmVOXMinLevel)).BeginInit();
            this.gbCalcVHF.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmCalc)).BeginInit();
            this.SuspendLayout();
            // 
            // btnWrite
            // 
            this.btnWrite.BackColor = System.Drawing.SystemColors.Control;
            this.btnWrite.Font = new System.Drawing.Font("Arial", 8F);
            this.btnWrite.Location = new System.Drawing.Point(771, 342);
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
            this.btnReadFile.Location = new System.Drawing.Point(771, 313);
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
            this.btnReadFromRadio.Location = new System.Drawing.Point(771, 255);
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
            this.btnSaveCalibration.Location = new System.Drawing.Point(771, 371);
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
            this.tabs.Size = new System.Drawing.Size(739, 608);
            this.tabs.TabIndex = 2;
            // 
            // tabVHF
            // 
            this.tabVHF.Controls.Add(this.lblCalc);
            this.tabVHF.Controls.Add(this.tbVHFMax);
            this.tabVHF.Controls.Add(this.tbVHFMin);
            this.tabVHF.Controls.Add(this.tbVHFMinCal);
            this.tabVHF.Controls.Add(this.label15);
            this.tabVHF.Controls.Add(this.label14);
            this.tabVHF.Controls.Add(this.label13);
            this.tabVHF.Controls.Add(this.tlpVHF);
            this.tabVHF.Location = new System.Drawing.Point(4, 22);
            this.tabVHF.Name = "tabVHF";
            this.tabVHF.Padding = new System.Windows.Forms.Padding(3);
            this.tabVHF.Size = new System.Drawing.Size(731, 582);
            this.tabVHF.TabIndex = 0;
            this.tabVHF.Text = "Диапазон 2 м";
            this.tabVHF.UseVisualStyleBackColor = true;
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
            this.tlpVHF.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tlpVHF.Controls.Add(this.label7, 0, 0);
            this.tlpVHF.Controls.Add(this.label8, 0, 1);
            this.tlpVHF.Controls.Add(this.label9, 0, 2);
            this.tlpVHF.Controls.Add(this.label10, 0, 3);
            this.tlpVHF.Controls.Add(this.label11, 0, 4);
            this.tlpVHF.Controls.Add(this.label12, 0, 5);
            this.tlpVHF.Location = new System.Drawing.Point(103, 111);
            this.tlpVHF.Margin = new System.Windows.Forms.Padding(0);
            this.tlpVHF.Name = "tlpVHF";
            this.tlpVHF.RowCount = 11;
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
            this.tlpVHF.Size = new System.Drawing.Size(526, 264);
            this.tlpVHF.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(1, 1);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(150, 20);
            this.label7.TabIndex = 0;
            this.label7.Text = "Частота Rx";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(1, 22);
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
            this.label9.Location = new System.Drawing.Point(4, 43);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(144, 20);
            this.label9.TabIndex = 2;
            this.label9.Text = "Уровень мощности 4";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Location = new System.Drawing.Point(4, 64);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(144, 20);
            this.label10.TabIndex = 3;
            this.label10.Text = "Уровень мощности 3";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Location = new System.Drawing.Point(4, 85);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(144, 20);
            this.label11.TabIndex = 4;
            this.label11.Text = "Уровень мощности 2";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label12
            // 
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.Location = new System.Drawing.Point(4, 106);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(144, 20);
            this.label12.TabIndex = 5;
            this.label12.Text = "Уровень мощности 1";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabUHF
            // 
            this.tabUHF.Location = new System.Drawing.Point(4, 22);
            this.tabUHF.Name = "tabUHF";
            this.tabUHF.Padding = new System.Windows.Forms.Padding(3);
            this.tabUHF.Size = new System.Drawing.Size(731, 440);
            this.tabUHF.TabIndex = 1;
            this.tabUHF.Text = "Диапазон 70 см";
            this.tabUHF.UseVisualStyleBackColor = true;
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
            this.gbCommons.Location = new System.Drawing.Point(771, 12);
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
            this.btnReadFactoryFromRadio.Location = new System.Drawing.Point(771, 284);
            this.btnReadFactoryFromRadio.Name = "btnReadFactoryFromRadio";
            this.btnReadFactoryFromRadio.Size = new System.Drawing.Size(268, 23);
            this.btnReadFactoryFromRadio.TabIndex = 5;
            this.btnReadFactoryFromRadio.Text = "button1";
            this.btnReadFactoryFromRadio.UseVisualStyleBackColor = false;
            this.btnReadFactoryFromRadio.Click += new System.EventHandler(this.btnReadFactoryFromRadio_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(100, 20);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(269, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "Минимальная частота калибровок OpenGD77 RUS:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(100, 40);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(250, 13);
            this.label14.TabIndex = 2;
            this.label14.Text = "Минимальная частота диапазона для расчетов:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(100, 60);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(256, 13);
            this.label15.TabIndex = 3;
            this.label15.Text = "Максимальная частота диапазона для расчетов:";
            // 
            // tbVHFMinCal
            // 
            this.tbVHFMinCal.Location = new System.Drawing.Point(374, 17);
            this.tbVHFMinCal.Name = "tbVHFMinCal";
            this.tbVHFMinCal.ReadOnly = true;
            this.tbVHFMinCal.Size = new System.Drawing.Size(79, 20);
            this.tbVHFMinCal.TabIndex = 4;
            // 
            // tbVHFMin
            // 
            this.tbVHFMin.Location = new System.Drawing.Point(374, 37);
            this.tbVHFMin.Name = "tbVHFMin";
            this.tbVHFMin.ReadOnly = true;
            this.tbVHFMin.Size = new System.Drawing.Size(79, 20);
            this.tbVHFMin.TabIndex = 5;
            // 
            // tbVHFMax
            // 
            this.tbVHFMax.Location = new System.Drawing.Point(374, 57);
            this.tbVHFMax.Name = "tbVHFMax";
            this.tbVHFMax.ReadOnly = true;
            this.tbVHFMax.Size = new System.Drawing.Size(79, 20);
            this.tbVHFMax.TabIndex = 6;
            // 
            // lblCalc
            // 
            this.lblCalc.Location = new System.Drawing.Point(464, 15);
            this.lblCalc.Name = "lblCalc";
            this.lblCalc.Size = new System.Drawing.Size(249, 83);
            this.lblCalc.TabIndex = 7;
            this.lblCalc.Text = resources.GetString("lblCalc.Text");
            this.lblCalc.Visible = false;
            // 
            // gbCalcVHF
            // 
            this.gbCalcVHF.Controls.Add(this.tbVHFLowPower);
            this.gbCalcVHF.Controls.Add(this.label21);
            this.gbCalcVHF.Controls.Add(this.tbVHFMidLowPower);
            this.gbCalcVHF.Controls.Add(this.label20);
            this.gbCalcVHF.Controls.Add(this.tbVHFMidPower);
            this.gbCalcVHF.Controls.Add(this.label19);
            this.gbCalcVHF.Controls.Add(this.btnCalc);
            this.gbCalcVHF.Controls.Add(this.tbVHFHighPower);
            this.gbCalcVHF.Controls.Add(this.label18);
            this.gbCalcVHF.Controls.Add(this.nmCalc);
            this.gbCalcVHF.Controls.Add(this.label17);
            this.gbCalcVHF.Location = new System.Drawing.Point(771, 412);
            this.gbCalcVHF.Name = "gbCalcVHF";
            this.gbCalcVHF.Size = new System.Drawing.Size(268, 204);
            this.gbCalcVHF.TabIndex = 8;
            this.gbCalcVHF.TabStop = false;
            this.gbCalcVHF.Text = "Калькулятор параметров";
            this.gbCalcVHF.Visible = false;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(22, 30);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(52, 13);
            this.label17.TabIndex = 0;
            this.label17.Text = "Частота:";
            // 
            // nmCalc
            // 
            this.nmCalc.Location = new System.Drawing.Point(145, 28);
            this.nmCalc.Maximum = new decimal(new int[] {
            564,
            0,
            0,
            0});
            this.nmCalc.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nmCalc.Name = "nmCalc";
            this.nmCalc.Size = new System.Drawing.Size(65, 20);
            this.nmCalc.TabIndex = 1;
            this.nmCalc.Value = new decimal(new int[] {
            144,
            0,
            0,
            0});
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(22, 51);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(118, 13);
            this.label18.TabIndex = 2;
            this.label18.Text = "Уровень мощности 4:";
            // 
            // tbVHFHighPower
            // 
            this.tbVHFHighPower.Location = new System.Drawing.Point(145, 48);
            this.tbVHFHighPower.Name = "tbVHFHighPower";
            this.tbVHFHighPower.Size = new System.Drawing.Size(65, 20);
            this.tbVHFHighPower.TabIndex = 3;
            // 
            // btnCalc
            // 
            this.btnCalc.BackColor = System.Drawing.SystemColors.Control;
            this.btnCalc.Location = new System.Drawing.Point(62, 177);
            this.btnCalc.Name = "btnCalc";
            this.btnCalc.Size = new System.Drawing.Size(167, 21);
            this.btnCalc.TabIndex = 4;
            this.btnCalc.Text = "Подсчитать";
            this.btnCalc.UseVisualStyleBackColor = false;
            this.btnCalc.Click += new System.EventHandler(this.btnCalc_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(22, 71);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(118, 13);
            this.label19.TabIndex = 5;
            this.label19.Text = "Уровень мощности 3:";
            // 
            // tbVHFMidPower
            // 
            this.tbVHFMidPower.Location = new System.Drawing.Point(145, 68);
            this.tbVHFMidPower.Name = "tbVHFMidPower";
            this.tbVHFMidPower.Size = new System.Drawing.Size(65, 20);
            this.tbVHFMidPower.TabIndex = 6;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(22, 90);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(118, 13);
            this.label20.TabIndex = 7;
            this.label20.Text = "Уровень мощности 2:";
            // 
            // tbVHFMidLowPower
            // 
            this.tbVHFMidLowPower.Location = new System.Drawing.Point(145, 88);
            this.tbVHFMidLowPower.Name = "tbVHFMidLowPower";
            this.tbVHFMidLowPower.Size = new System.Drawing.Size(65, 20);
            this.tbVHFMidLowPower.TabIndex = 8;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(22, 110);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(118, 13);
            this.label21.TabIndex = 9;
            this.label21.Text = "Уровень мощности 1:";
            // 
            // tbVHFLowPower
            // 
            this.tbVHFLowPower.Location = new System.Drawing.Point(145, 108);
            this.tbVHFLowPower.Name = "tbVHFLowPower";
            this.tbVHFLowPower.Size = new System.Drawing.Size(65, 20);
            this.tbVHFLowPower.TabIndex = 10;
            // 
            // CalibrationFormMDUV380
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1051, 644);
            this.Controls.Add(this.gbCalcVHF);
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
            this.tlpVHF.ResumeLayout(false);
            this.gbCommons.ResumeLayout(false);
            this.gbCommons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmRSSI70)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRSSI120)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRxHighLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRxLowLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmVOXMaxLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmVOXMinLevel)).EndInit();
            this.gbCalcVHF.ResumeLayout(false);
            this.gbCalcVHF.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmCalc)).EndInit();
            this.ResumeLayout(false);

    }

    private int CLAMP(int x, int low, int high)
    {
        return (((x) > (high)) ? (high) : (((x) < (low)) ? (low) : (x)));
    }

    private int interpolate(int lowerpoint, int upperpoint, int numerator, int denominator)
    {
        return lowerpoint + (((upperpoint - lowerpoint) * numerator) / denominator);
    }

    private void calibrationGetPowerForFrequency(int freq)
    {
        int index;
        int offset;
        int limit;
        int upper;
        int lower;

        freq *= 100000;

        if (freq > 34900000)

        {
            index = (int)((freq - radioBandlimits.UHFLowCal) / 1000000);
            offset = (int)((freq - radioBandlimits.UHFLowCal) % 1000000);
            limit = 8;
            index = CLAMP(index, 0, limit);
            lower = CalData.UHFLowPowerCal[index].VALUE << 4;             

            if (index < limit)
            {
                upper = CalData.UHFLowPowerCal[index + 1].VALUE << 4;         
            }
            else
            {
                upper = lower + (lower - (CalData.UHFLowPowerCal[index - 1].VALUE << 4));       
            }

           /* powerSettings->lowPower = CLAMP(interpolate(lower, upper, offset, 1000000), 0, 4096);
            lower = calibrationData.UHFMidLowPowerCal[index] << 4;              // get the Lower lookup point and scale it to 12 bits

            if (index < limit)
            {
                upper = calibrationData.UHFMidLowPowerCal[index + 1] << 4;          //get the higher lookup point and scale it to 12 bits
            }
            else
            {
                upper = lower + (lower - (calibrationData.UHFMidLowPowerCal[index - 1] << 4));       //extrapolate outside top point using the same slope
            }

            powerSettings->midLowPower = CLAMP(interpolate(lower, upper, offset, 1000000), 0, 4096);
            lower = calibrationData.UHFMidPowerCal[index] << 4;             // get the Lower lookup point and scale it to 12 bits

            if (index < limit)
            {
                upper = calibrationData.UHFMidPowerCal[index + 1] << 4;         //get the higher lookup point and scale it to 12 bits
            }
            else
            {
                upper = lower + (lower - (calibrationData.UHFMidPowerCal[index - 1] << 4));       //extrapolate outside top point using the same slope
            }

            powerSettings->midPower = CLAMP(interpolate(lower, upper, offset, 1000000), 0, 4096);
            lower = calibrationData.UHFHighPowerCal[index] << 4;                // get the Lower lookup point and scale it to 12 bits

            if (index < limit)
            {
                upper = calibrationData.UHFHighPowerCal[index + 1] << 4;            //get the higher lookup point and scale it to 12 bits
            }
            else
            {
                upper = lower + (lower - (calibrationData.UHFHighPowerCal[index - 1] << 4));       //extrapolate outside top point using the same slope
            }

            powerSettings->highPower = CLAMP(interpolate(lower, upper, offset, 1000000), 0, 4096);*/

            return;
        }

        index = (int)((freq - radioBandlimits.VHFLowCal) / 950000);
        offset = (int)((freq - radioBandlimits.VHFLowCal) % 950000);
        limit = 4;
        index = CLAMP(index, 0, limit);
        lower = CalData.VHFLowPowerCal[index].VALUE << 4;             

        if (index < limit)
        {
            upper = CalData.VHFLowPowerCal[index + 1].VALUE << 4;        
        }
        else
        {
            upper = lower + (lower - (CalData.VHFLowPowerCal[index - 1].VALUE << 4));       
        }

        tbVHFLowPower.Text = CLAMP(interpolate(lower, upper, offset, 950000), 0, 4096).ToString();
        lower = CalData.VHFMidLowPowerCal[index].VALUE << 4;             

        if (index < limit)
        {
            upper = CalData.VHFMidLowPowerCal[index + 1].VALUE << 4;         
        }
        else
        {
            upper = lower + (lower - (CalData.VHFMidLowPowerCal[index - 1].VALUE << 4));       
        }

        tbVHFMidLowPower.Text = CLAMP(interpolate(lower, upper, offset, 950000), 0, 4096).ToString();
        lower = CalData.VHFMidPowerCal[index].VALUE << 4;             

        if (index < limit)
        {
            upper = CalData.VHFMidPowerCal[index + 1].VALUE << 4;        
        }
        else
        {
            upper = lower + (lower - (CalData.VHFMidPowerCal[index - 1].VALUE << 4));     
        }

        tbVHFMidPower.Text = CLAMP(interpolate(lower, upper, offset, 950000), 0, 4096).ToString();
        lower = CalData.VHFHighPowerCal[index].VALUE << 4;                

        if (index < limit)
        {
            upper = CalData.VHFHighPowerCal[index + 1].VALUE << 4;            
        }
        else
        {
            upper = lower + (lower - (CalData.VHFHighPowerCal[index - 1].VALUE << 4));       
        }

        tbVHFHighPower.Text = CLAMP(interpolate(lower, upper, offset, 950000), 0, 4096).ToString();
    }

    private void btnCalc_Click(object sender, EventArgs e)
    {
        calibrationGetPowerForFrequency((int)nmCalc.Value);
    }

 
}
