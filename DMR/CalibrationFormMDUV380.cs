using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using static DMR.CodeplugSettingsForm;

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
    private Label lblMessage;
    private Button btnSaveCalibration;
    private SaveFileDialog saveFileDialog;
    XmlSerializer xmlSerializer = new XmlSerializer(typeof(CalibrationDataSTM32));

    CalibrationDataSTM32 CalData = new CalibrationDataSTM32();

    public CalibrationFormMDUV380()
    {
        InitializeComponent();
        base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        Settings.ReadCommonsForSectionIntoDictionary(StringsDict, "OpenGD77Form");
        calibrationDataSTM32 = new byte[CALIBRATION_DATA_SIZE_STM32];
    }

    private Form getMainForm()
    {
        foreach (Form openForm in Application.OpenForms)
        {
            if (openForm is MainForm)
            {
                return openForm;
            }
        }
        return null;
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
        openGD77CommsTransferData.dataBuff = new byte[(MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32) ? CALIBRATION_DATA_SIZE_STM32 : CALIBRATION_DATA_SIZE];
        openGD77CommsTransferData.localDataBufferStartPosition = 0;
        openGD77CommsTransferData.startDataAddressInTheRadio = ((MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32) ? MEMORY_LOCATION_STM32 : MEMORY_LOCATION);
        openGD77CommsTransferData.transferLength = ((MainForm.RadioType == MainForm.RadioTypeEnum.RadioTypeSTM32) ? CALIBRATION_DATA_SIZE_STM32 : CALIBRATION_DATA_SIZE);
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
        //Array.Copy(openGD77CommsTransferData.dataBuff, 0, calibrationDataSTM32, 0, calibrationDataSTM32.Length);
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
        OpenGD77CommsTransferData openGD77CommsTransferData = new OpenGD77CommsTransferData();
        openGD77CommsTransferData.dataBuff = new byte[CALIBRATION_DATA_SIZE_STM32];
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


    public static byte[] DataToByte(CalibrationData calData)
    {
        if (calData.DACOscRefTune < 0)
        {
            calData.DACOscRefTune = (short)(1024 + calData.DACOscRefTune);
        }
        calData.Q_MOD2_OFFSET = (sbyte)calData.DACOscRefTune;
        int num = Marshal.SizeOf(typeof(CalibrationData));
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



    private void btnReadFile_Click(object sender, EventArgs e)
    {
        byte[] array = null;
        OpenFileDialog openFileDialog = new OpenFileDialog();
        if (openFileDialog.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(openFileDialog.FileName))
        {
            return;
        }
        int num = Marshal.SizeOf(typeof(CalibrationData));
        byte[] array2 = new byte[num];
        array = File.ReadAllBytes(openFileDialog.FileName);

        if (array.Length == CALIBRATION_DATA_SIZE_STM32)
        {
            int num2 = 432;
            if (array[num2] == 0 && array[num2 + 1] == 37 && array[num2 + 2] == 0 && array[num2 + 3] == 64 && array[num2 + 4] == 0 && array[num2 + 5] == 69 && array[num2 + 6] == 1 && array[num2 + 7] == 64)
            {
                ((MainForm)getMainForm())?.changeRadioType(MainForm.RadioTypeEnum.RadioTypeSTM32);
                Array.Copy(array, 0, calibrationDataSTM32, 0, CALIBRATION_DATA_SIZE_STM32);
                showButtons();
            }
            else
            {
                MessageBox.Show(StringsDict["File_contains_invalid_STM32_calibration_header"]);
            }
        }
        else
        {
            MessageBox.Show(StringsDict["File_is_the_wrong_size"]);
        }
    }

    private void btnReadFromRadio_Click(object sender, EventArgs e)
    {
        if (readDataFromRadio())
        {
            MessageBox.Show(StringsDict["NowSaveCalibrations"]);
            showButtons();
        }
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
                //CalibrationStruct d = new();
                //buildCalibrationStructFromRaw(calibrationDataSTM32);
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

    void buildCalibrationStructFromRaw(byte[] data)
    {

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
            this.lblMessage = new System.Windows.Forms.Label();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // btnWrite
            // 
            this.btnWrite.BackColor = System.Drawing.SystemColors.Control;
            this.btnWrite.Font = new System.Drawing.Font("Arial", 8F);
            this.btnWrite.Location = new System.Drawing.Point(12, 41);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(238, 23);
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
            this.btnReadFile.Location = new System.Drawing.Point(256, 12);
            this.btnReadFile.Name = "btnReadFile";
            this.btnReadFile.Size = new System.Drawing.Size(238, 23);
            this.btnReadFile.TabIndex = 1;
            this.btnReadFile.Text = "Open Calibration file";
            this.btnReadFile.UseVisualStyleBackColor = false;
            this.btnReadFile.Click += new System.EventHandler(this.btnReadFile_Click);
            // 
            // btnReadFromRadio
            // 
            this.btnReadFromRadio.BackColor = System.Drawing.SystemColors.Control;
            this.btnReadFromRadio.Font = new System.Drawing.Font("Arial", 8F);
            this.btnReadFromRadio.Location = new System.Drawing.Point(12, 12);
            this.btnReadFromRadio.Name = "btnReadFromRadio";
            this.btnReadFromRadio.Size = new System.Drawing.Size(238, 23);
            this.btnReadFromRadio.TabIndex = 1;
            this.btnReadFromRadio.Text = "Read calibration from radio";
            this.btnReadFromRadio.UseVisualStyleBackColor = false;
            this.btnReadFromRadio.Click += new System.EventHandler(this.btnReadFromRadio_Click);
            // 
            // btnSaveCalibration
            // 
            this.btnSaveCalibration.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveCalibration.Font = new System.Drawing.Font("Arial", 8F);
            this.btnSaveCalibration.Location = new System.Drawing.Point(256, 41);
            this.btnSaveCalibration.Name = "btnSaveCalibration";
            this.btnSaveCalibration.Size = new System.Drawing.Size(238, 23);
            this.btnSaveCalibration.TabIndex = 1;
            this.btnSaveCalibration.Text = "Save Calibration file";
            this.btnSaveCalibration.UseVisualStyleBackColor = false;
            this.btnSaveCalibration.Visible = false;
            this.btnSaveCalibration.Click += new System.EventHandler(this.btnSaveCalibration_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblMessage.ForeColor = System.Drawing.Color.Blue;
            this.lblMessage.Location = new System.Drawing.Point(12, 77);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(482, 119);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "label1";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Файлы калибровок|*.ogdc";
            // 
            // CalibrationFormMDUV380
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(505, 204);
            this.Controls.Add(this.lblMessage);
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
            this.ResumeLayout(false);

    }
}
