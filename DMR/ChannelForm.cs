using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class ChannelForm : DockContent, IDisp
{
    public enum ChModeE
    {
        Analog,
        Digital
    }

    private enum LibreDMR_Power_E
    {
        Power_NoOverride,
        Power_50mW,
        Power_250mW,
        Power_500mW,
        Power_750mW,
        Power_1W,
        Power_2W,
        Power_3W,
        Power_4W,
        Power_5W,
        Power_5W_Plus
    }

    private enum RefFreqE
    {
        Low,
        Middle,
        High
    }

    private enum AdmitCritericaE
    {
        Always = 0,
        ChFree = 1,
        CtcssDcs = 2,
        ColorCode = 2,
        CorectPl = 3
    }

    private enum BandwidthE
    {
        Band12_5,
        Band25
    }

    private enum SquelchE
    {
        Tight,
        Normal
    }

    private enum VoiceEmphasisE
    {
        None,
        DeEmphasisAndPreEmphasis,
        DeEmphasis,
        PreEmphasis
    }

    private enum SteE
    {
        Frequency,
        Ste120,
        Ste180,
        Ste240
    }

    private enum NoneSteE
    {
        Off,
        Frequency
    }

    private enum SignalingSystemE
    {
        Off,
        Dtmf
    }

    private enum UnmuteRuleE
    {
        StdUnmuteMute,
        AndUnmuteMute,
        AndUnmuteOrMute
    }

    private enum PttidTypeE
    {
        None,
        Front,
        Post,
        FrontAndPost
    }

    private enum TimingPreference
    {
        First,
        Qualified,
        Unqualified
    }

    public class ChModeChangeEventArgs : EventArgs
    {
        public int ChIndex { get; set; }

        public int ChMode { get; set; }

        public ChModeChangeEventArgs(int chIndex, int chMode)
        {
            ChIndex = chIndex;
            ChMode = chMode;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ChannelOne : IVerify<ChannelOne>
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        private byte[] name;

        private uint rxFreq;

        private uint txFreq;

        private byte chMode;

        private byte libreDMR_Power;

        private byte locationLat0;

        private byte tot;

        private byte locationLat1;

        private byte locationLat2;

        private byte locationLon0;

        private byte locationLon1;

        private ushort rxTone;

        private ushort txTone;

        private byte locationLon2;

        private byte openGD77RUS;

        private byte LibreDMR_flag1;

        private byte rxSignaling;

        private byte artsInterval;

        private byte encrypt;

        private byte _UNUSED_2;

        private byte rxGroupList;

        private byte txColor;

        private byte aprsSystem;

        private ushort contact;

        private byte flag1;

        private byte flag2;

        private byte flag3;

        private byte flag4;

        private ushort reserve2;

        private byte reserve;

        private byte sql;

        public string Name
        {
            get
            {
                return Settings.bufferToString(name);
            }
            set
            {
                byte[] array = Settings.stringToBuffer(value);
                name.Fill(byte.MaxValue);
                Array.Copy(array, 0, name, 0, Math.Min(array.Length, name.Length));
            }
        }

        public string RxFreq
        {
            get
            {
                try
                {
                    uint uint_ = 0u;
                    double double_ = Settings.convert10HzStepFreqToDecimalText(int.Parse($"{rxFreq:x}"), 100000);
                    if (Settings.checkFrequecyIsInValidRange(double_, ref uint_) == -1)
                    {
                        double_ = uint_;
                    }
                    return double_.ToString("f5");
                }
                catch
                {
                    return "";
                }
            }
            set
            {
                try
                {
                    decimal value2 = decimal.Parse(value) * 100000m;
                    RxFreqDec = Convert.ToUInt32(value2);
                }
                catch
                {
                    rxFreq = uint.MaxValue;
                }
            }
        }

        public uint UInt32_0
        {
            get
            {
                return rxFreq;
            }
            set
            {
                rxFreq = value;
            }
        }

        public uint RxFreqDec
        {
            get
            {
                return Settings.smethod_34(rxFreq);
            }
            set
            {
                rxFreq = Settings.smethod_35(value);
            }
        }

        public string TxFreq
        {
            get
            {
                try
                {
                    uint uint_ = 0u;
                    double double_ = Settings.convert10HzStepFreqToDecimalText(int.Parse($"{txFreq:x}"), 100000);
                    if (Settings.checkFrequecyIsInValidRange(double_, ref uint_) == -1)
                    {
                        double_ = uint_;
                    }
                    return double_.ToString("f5");
                }
                catch
                {
                    return "";
                }
            }
            set
            {
                try
                {
                    decimal value2 = decimal.Parse(value) * 100000m;
                    TxFreqDec = Convert.ToUInt32(value2);
                }
                catch
                {
                    txFreq = uint.MaxValue;
                }
            }
        }

        public uint UInt32_1
        {
            get
            {
                return txFreq;
            }
            set
            {
                txFreq = value;
            }
        }

        public uint TxFreqDec
        {
            get
            {
                return Settings.smethod_34(txFreq);
            }
            set
            {
                txFreq = Settings.smethod_35(value);
            }
        }

        public int ChMode
        {
            get
            {
                if (Enum.IsDefined(typeof(ChModeE), (int)chMode))
                {
                    return chMode;
                }
                return 0;
            }
            set
            {
                if (Enum.IsDefined(typeof(ChModeE), value))
                {
                    chMode = (byte)value;
                }
            }
        }

        public string ChModeS
        {
            get
            {
                if (chMode < SZ_CH_MODE.Length)
                {
                    return SZ_CH_MODE[chMode];
                }
                return "";
            }
            set
            {
                int num = Array.IndexOf(SZ_CH_MODE, value);
                if (num < 0)
                {
                    num = 0;
                }
                chMode = (byte)num;
            }
        }

        public int LibreDMR_Power
        {
            get
            {
                if (Enum.IsDefined(typeof(LibreDMR_Power_E), (int)libreDMR_Power))
                {
                    return libreDMR_Power;
                }
                return 0;
            }
            set
            {
                if (Enum.IsDefined(typeof(LibreDMR_Power_E), value))
                {
                    libreDMR_Power = (byte)value;
                }
            }
        }

        public string LibreDMR_PowerStr
        {
            get
            {
                return SZ_LIBREDMR_POWER[LibreDMR_Power];
            }
            set
            {
                LibreDMR_Power = Array.IndexOf(SZ_LIBREDMR_POWER, value);
            }
        }

        public string Longitude
        {
            get
            {
                return LatLonBin24ToString((uint)((locationLon2 << 16) + (locationLon1 << 8) + locationLon0));
            }
            set
            {
                try
                {
                    double num = double.Parse(value);
                    if (num >= -180.0 && num <= 180.0)
                    {
                        uint num2 = LatLonStringToBin24(value);
                        locationLon0 = (byte)(num2 & 0xFF);
                        locationLon1 = (byte)((num2 >> 8) & 0xFF);
                        locationLon2 = (byte)((num2 >> 16) & 0xFF);
                        return;
                    }
                    throw new Exception();
                }
                catch
                {
                    locationLon0 = (locationLon1 = (locationLon2 = 0));
                }
            }
        }

        public string Latitude
        {
            get
            {
                return LatLonBin24ToString((uint)((locationLat2 << 16) + (locationLat1 << 8) + locationLat0));
            }
            set
            {
                try
                {
                    double num = double.Parse(value);
                    if (num >= -90.0 && num <= 90.0)
                    {
                        uint num2 = LatLonStringToBin24(value);
                        locationLat0 = (byte)(num2 & 0xFF);
                        locationLat1 = (byte)((num2 >> 8) & 0xFF);
                        locationLat2 = (byte)((num2 >> 16) & 0xFF);
                        return;
                    }
                    throw new Exception();
                }
                catch
                {
                    locationLat0 = (locationLat1 = (locationLat2 = 0));
                }
            }
        }

        public string LibreDMR_DMRidChannel
        {
            get
            {
                if ((LibreDMR_flag1 & 0x80) != 0)
                {
                    int num = (rxSignaling << 16) | (artsInterval << 8) | encrypt;
                    if (num >= 1 && num <= 16777215)
                    {
                        string text = num.ToString();
                        if (!string.IsNullOrEmpty(text) && ContactForm.IsValidId(text))
                        {
                            return text;
                        }
                    }
                }
                return "";
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && ContactForm.IsValidId(value))
                {
                    int num = int.Parse(value);
                    if (num >= 1 && num <= 16777215)
                    {
                        LibreDMR_flag1 |= 128;
                        rxSignaling = (byte)((num >> 16) & 0xFF);
                        artsInterval = (byte)((num >> 8) & 0xFF);
                        encrypt = (byte)(num & 0xFF);
                    }
                }
                else
                {
                    LibreDMR_flag1 = (byte)(LibreDMR_flag1 & -129);
                    rxSignaling = 0;
                    artsInterval = 22;
                    encrypt = 0;
                }
            }
        }

        public bool LibreDMR_NoBeepChannel
        {
            get
            {
                return (LibreDMR_flag1 & 0x40) != 0;
            }
            set
            {
                if (value)
                {
                    LibreDMR_flag1 |= 64;
                }
                else
                {
                    LibreDMR_flag1 = (byte)(LibreDMR_flag1 & -65);
                }
            }
        }

        public bool LibreDMR_NoEcoChannel
        {
            get
            {
                return (LibreDMR_flag1 & 0x20) != 0;
            }
            set
            {
                if (value)
                {
                    LibreDMR_flag1 |= 32;
                }
                else
                {
                    LibreDMR_flag1 = (byte)(LibreDMR_flag1 & -33);
                }
            }
        }

        public bool openGD77RUS_FastcallChannel
        {
            get
            {
                return (openGD77RUS & 0x80) != 0;
            }
            set
            {
                if (value)
                {
                    openGD77RUS |= 128;
                }
                else
                {
                    openGD77RUS = (byte)(openGD77RUS & -129);
                }
            }
        }

        public bool openGD77RUS_PriorityChannel
        {
            get
            {
                return (openGD77RUS & 0x40) != 0;
            }
            set
            {
                if (value)
                {
                    openGD77RUS |= 64;
                }
                else
                {
                    openGD77RUS = (byte)(openGD77RUS & -65);
                }
            }
        }

        public void clearFastcall()
        {
            openGD77RUS = (byte)(openGD77RUS & -129);
        }

        public bool LibreDMR_DMRForceDMO
        {
            get
            {
                return (LibreDMR_flag1 & 4) != 0;
            }
            set
            {
                if (value)
                {
                    LibreDMR_flag1 |= 4;
                }
                else
                {
                    LibreDMR_flag1 = (byte)(LibreDMR_flag1 & -5);
                }
            }
        }

        public bool LibreDMR_UseLocation
        {
            get
            {
                return (LibreDMR_flag1 & 8) != 0;
            }
            set
            {
                if (value)
                {
                    LibreDMR_flag1 |= 8;
                }
                else
                {
                    LibreDMR_flag1 = (byte)(LibreDMR_flag1 & -9);
                }
            }
        }

        public bool LibreDMR_Roaming
        {
            get
            {
                return (LibreDMR_flag1 & 1) != 0;
            }
            set
            {
                if (value)
                {
                    LibreDMR_flag1 |= 1;
                }
                else
                {
                    LibreDMR_flag1 = (byte)(LibreDMR_flag1 & -2);
                }
            }
        }

        public decimal Tot
        {
            get
            {
                if (tot >= 0 && tot <= 33)
                {
                    return tot * 15;
                }
                return 0m;
            }
            set
            {
                int num = (int)(value / 15m);
                if (num >= 0 && num <= 33)
                {
                    tot = (byte)num;
                }
                else
                {
                    tot = 0;
                }
            }
        }

        public string RxTone
        {
            get
            {
                if (rxTone != ushort.MaxValue && rxTone != 0)
                {
                    if ((rxTone & 0xC000) == 49152)
                    {
                        return $"D{rxTone & 0xFFF:x3}I";
                    }
                    if ((rxTone & 0xC000) == 32768)
                    {
                        return $"D{rxTone & 0xFFF:x3}N";
                    }
                    double num = (double)(int)Settings.smethod_32(rxTone) * 0.1;
                    if (num > 60.0 && num < 260.0)
                    {
                        return num.ToString("f1");
                    }
                    return Settings.SZ_NONE;
                }
                return Settings.SZ_NONE;
            }
            set
            {
                string text = "";
                rxTone = ushort.MaxValue;
                if (string.IsNullOrEmpty(value) || !(value != Settings.SZ_NONE))
                {
                    return;
                }
                if (new Regex("D[0-7]{3}N$").IsMatch(value))
                {
                    text = value.Substring(1, 3);
                    rxTone = (ushort)(ushort.Parse(text, NumberStyles.HexNumber) + 32768);
                    return;
                }
                if (new Regex("D[0-7]{3}I$").IsMatch(value))
                {
                    text = value.Substring(1, 3);
                    rxTone = (ushort)(ushort.Parse(text, NumberStyles.HexNumber) + 49152);
                    return;
                }
                double num = double.Parse(value);
                if (num > 60.0 && num < 260.0)
                {
                    rxTone = Settings.smethod_33(Convert.ToUInt16(num * 10.0));
                }
            }
        }

        public string TxTone
        {
            get
            {
                if (txTone != ushort.MaxValue && txTone != 0)
                {
                    if ((txTone & 0xC000) == 49152)
                    {
                        return $"D{txTone & 0xFFF:x3}I";
                    }
                    if ((txTone & 0xC000) == 32768)
                    {
                        return $"D{txTone & 0xFFF:x3}N";
                    }
                    double num = (double)(int)Settings.smethod_32(txTone) * 0.1;
                    if (num > 60.0 && num < 260.0)
                    {
                        return num.ToString("f1");
                    }
                    return Settings.SZ_NONE;
                }
                return Settings.SZ_NONE;
            }
            set
            {
                string text = "";
                txTone = ushort.MaxValue;
                if (string.IsNullOrEmpty(value) || !(value != Settings.SZ_NONE))
                {
                    return;
                }
                if (new Regex("D[0-7]{3}N$").IsMatch(value))
                {
                    text = value.Substring(1, 3);
                    txTone = (ushort)(ushort.Parse(text, NumberStyles.HexNumber) + 32768);
                    return;
                }
                if (new Regex("D[0-7]{3}I$").IsMatch(value))
                {
                    text = value.Substring(1, 3);
                    txTone = (ushort)(ushort.Parse(text, NumberStyles.HexNumber) + 49152);
                    return;
                }
                double num = double.Parse(value);
                if (num > 60.0 && num < 260.0)
                {
                    txTone = Settings.smethod_33(Convert.ToUInt16(num * 10.0));
                }
            }
        }

        public int UnmuteRule
        {
            get
            {
                return LibreDMR_flag1;
            }
            set
            {
                LibreDMR_flag1 = (byte)value;
            }
        }

        public int RxSignaling
        {
            get
            {
                return rxSignaling;
            }
            set
            {
                rxSignaling = (byte)value;
            }
        }

        public decimal ArtsInterval
        {
            get
            {
                return artsInterval;
            }
            set
            {
                artsInterval = (byte)value;
            }
        }

        public int Key
        {
            get
            {
                return encrypt;
            }
            set
            {
                encrypt = (byte)value;
            }
        }

        public int RxGroupList
        {
            get
            {
                return rxGroupList;
            }
            set
            {
                if (value <= RxGroupListForm.data.Count)
                {
                    rxGroupList = (byte)value;
                }
            }
        }

        public string RxGroupListString
        {
            get
            {
                if (RxGroupList == 0)
                {
                    return Settings.SZ_NONE;
                }
                if (RxGroupList <= 76)
                {
                    return RxGroupListForm.data.GetName(RxGroupList - 1);
                }
                return Settings.SZ_NONE;
            }
            set
            {
                if (value == Settings.SZ_NONE)
                {
                    RxGroupList = 0;
                    return;
                }
                for (int i = 0; i < RxGroupListForm.data.Count; i++)
                {
                    if (value == RxGroupListForm.data.GetName(i))
                    {
                        RxGroupList = i;
                        break;
                    }
                }
            }
        }

        public decimal TxColor
        {
            get
            {
                if (txColor >= 0 && txColor <= 15)
                {
                    return txColor;
                }
                return 0m;
            }
            set
            {
                if (value >= 0m && value <= 15m)
                {
                    txColor = (byte)value;
                }
            }
        }

        public int APRS_System
        {
            get
            {
                if (aprsSystem <= APRSForm.data.Count)
                {
                    return aprsSystem;
                }
                return 0;
            }
            set
            {
                if (value <= APRSForm.data.Count)
                {
                    aprsSystem = (byte)value;
                }
            }
        }

        public int Contact
        {
            get
            {
                if (contact <= ContactForm.data.Count)
                {
                    return contact;
                }
                return 0;
            }
            set
            {
                if (value <= ContactForm.data.Count)
                {
                    contact = (ushort)value;
                }
            }
        }

        public int ContactType
        {
            get
            {
                if (Contact == 0)
                {
                    return 0;
                }
                if (contact <= ContactForm.data.Count)
                {
                    return ContactForm.data[contact - 1].CallType;
                }
                return 0;
            }
        }

        public string ContactIdString
        {
            get
            {
                if (Contact == 0)
                {
                    return Settings.SZ_NA;
                }
                if (contact <= ContactForm.data.Count)
                {
                    return ContactForm.data[contact - 1].CallId;
                }
                return Settings.SZ_NA;
            }
        }

        public string ContactString
        {
            get
            {
                if (Contact == 0)
                {
                    return Settings.SZ_NA;
                }
                if (Contact <= 1024)
                {
                    return ContactForm.data.GetName(Contact - 1);
                }
                return Settings.SZ_NA;
            }
            set
            {
                if (value == Settings.SZ_NA)
                {
                    Contact = 0;
                    return;
                }
                int indexForName = ContactForm.data.GetIndexForName(value);
                if (indexForName != -1)
                {
                    Contact = indexForName + 1;
                }
                else
                {
                    Contact = 0;
                }
            }
        }
        public byte OpenGD77RUS
        {
            get
            {
                return openGD77RUS;
            }
            set
            {
                openGD77RUS = value;
            }
        }
        public byte Flag1
        {
            get
            {
                return flag1;
            }
            set
            {
                flag1 = value;
            }
        }

        public byte Flag2
        {
            get
            {
                return flag2;
            }
            set
            {
                flag2 = value;
            }
        }

        public byte Flag3
        {
            get
            {
                return flag3;
            }
            set
            {
                flag3 = value;
            }
        }

        public byte Flag4
        {
            get
            {
                return flag4;
            }
            set
            {
                flag4 = value;
            }
        }

        public int Power
        {
            get
            {
                return (flag4 & 0x80) >> 7;
            }
            set
            {
                flag4 &= 127;
                value = (value << 7) & 0x80;
                flag4 |= (byte)value;
            }
        }

        public string PowerString
        {
            get
            {
                if (Power < SZ_POWER.Length)
                {
                    return SZ_POWER[Power];
                }
                return "";
            }
            set
            {
                int num = Array.IndexOf(SZ_POWER, value);
                if (num < 0)
                {
                    num = 0;
                }
                Power = (byte)num;
            }
        }

        public bool Vox
        {
            get
            {
                return Convert.ToBoolean(flag4 & 0x40);
            }
            set
            {
                if (value)
                {
                    flag4 |= 64;
                }
                else
                {
                    flag4 &= 191;
                }
            }
        }

        public bool AutoScan
        {
            get
            {
                return Convert.ToBoolean(flag4 & 0x20);
            }
            set
            {
                if (value)
                {
                    flag4 |= 32;
                }
                else
                {
                    flag4 &= 223;
                }
            }
        }

        public bool LoneWoker
        {
            get
            {
                return Convert.ToBoolean(flag4 & 0x10);
            }
            set
            {
                if (value)
                {
                    flag4 |= 16;
                }
                else
                {
                    flag4 &= 239;
                }
            }
        }

        public bool AllowTalkaround
        {
            get
            {
                return Convert.ToBoolean(flag4 & 8);
            }
            set
            {
                if (value)
                {
                    flag4 |= 8;
                }
                else
                {
                    flag4 &= 247;
                }
            }
        }

        public bool OnlyRx
        {
            get
            {
                return Convert.ToBoolean(flag4 & 4);
            }
            set
            {
                if (value)
                {
                    flag4 |= 4;
                }
                else
                {
                    flag4 &= 251;
                }
            }
        }

        public string OnlyRxString
        {
            get
            {
                if (!OnlyRx)
                {
                    return "No";
                }
                return "Yes";
            }
            set
            {
                OnlyRx = value == "Yes";
            }
        }

        public int Bandwidth
        {
            get
            {
                return (flag4 & 2) >> 1;
            }
            set
            {
                flag4 &= 253;
                value = (value << 1) & 2;
                flag4 |= (byte)value;
            }
        }

        public string BandwidthString
        {
            get
            {
                return SZ_BANDWIDTH[Bandwidth];
            }
            set
            {
                for (int i = 0; i < SZ_BANDWIDTH.Length; i++)
                {
                    if (SZ_BANDWIDTH[i] == value)
                    {
                        Bandwidth = i;
                        break;
                    }
                }
            }
        }

        public int Squelch
        {
            get
            {
                return flag4 & 1;
            }
            set
            {
                flag4 &= 254;
                flag4 |= (byte)(value & 1);
            }
        }

        public string SquelchString
        {
            get
            {
                return SZ_SQUELCH[Squelch];
            }
            set
            {
                for (int i = 0; i < SZ_SQUELCH.Length; i++)
                {
                    if (value == SZ_SQUELCH[i])
                    {
                        Squelch = i;
                    }
                }
            }
        }

        public int Ste
        {
            get
            {
                return (flag3 & 0xC0) >> 6;
            }
            set
            {
                value = (value << 6) & 0xC0;
                flag3 &= 63;
                flag3 |= (byte)value;
            }
        }

        public int NonSte
        {
            get
            {
                return (flag3 & 0x20) >> 5;
            }
            set
            {
                value = (value << 5) & 0x20;
                flag3 &= 223;
                flag3 |= (byte)value;
            }
        }

        public bool DataPl
        {
            get
            {
                return Convert.ToBoolean(flag3 & 0x10);
            }
            set
            {
                if (value)
                {
                    flag3 |= 16;
                }
                else
                {
                    flag3 &= 239;
                }
            }
        }

        public int PttidType
        {
            get
            {
                return (flag3 & 0xC) >> 2;
            }
            set
            {
                value = (value << 2) & 0xC;
                flag3 &= 243;
                flag3 |= (byte)value;
            }
        }

        public bool DualCapacity
        {
            get
            {
                return Convert.ToBoolean(flag3 & 1);
            }
            set
            {
                if (value)
                {
                    flag3 |= 1;
                }
                else
                {
                    flag3 &= 254;
                }
            }
        }

        public int TimingPreference
        {
            get
            {
                return (flag2 & 0x80) >> 7;
            }
            set
            {
                value = (value << 7) & 0x80;
                flag2 &= 127;
                flag2 |= (byte)value;
            }
        }

        public int RepeaterSlot
        {
            get
            {
                return (flag2 & 0x40) >> 6;
            }
            set
            {
                value = (value << 6) & 0x40;
                flag2 &= 191;
                flag2 |= (byte)value;
            }
        }

        public string RepeaterSlotS
        {
            get
            {
                return SZ_REPEATER_SLOT[RepeaterSlot];
            }
            set
            {
                int num = Array.IndexOf(SZ_REPEATER_SLOT, value);
                if (num < 0)
                {
                    num = 0;
                }
                RepeaterSlot = num;
            }
        }

        public int Ars
        {
            get
            {
                return (flag2 & 0x20) >> 5;
            }
            set
            {
                value = (value << 5) & 0x20;
                flag2 &= 223;
                flag2 |= (byte)value;
            }
        }

        public int KeySwitch
        {
            get
            {
                return (flag2 & 0x10) >> 4;
            }
            set
            {
                value = (value << 4) & 0x10;
                flag2 &= 239;
                flag2 |= (byte)value;
            }
        }

        public bool UdpDataHead
        {
            get
            {
                return Convert.ToBoolean(flag2 & 8);
            }
            set
            {
                if (value)
                {
                    flag2 |= 8;
                }
                else
                {
                    flag2 &= 247;
                }
            }
        }

        public bool AllowTxInterupt
        {
            get
            {
                return Convert.ToBoolean(flag2 & 4);
            }
            set
            {
                if (value)
                {
                    flag2 |= 4;
                }
                else
                {
                    flag2 &= 251;
                }
            }
        }

        public bool TxInteruptFreq
        {
            get
            {
                return Convert.ToBoolean(flag2 & 2);
            }
            set
            {
                if (value)
                {
                    flag2 |= 2;
                }
                else
                {
                    flag2 &= 253;
                }
            }
        }

        public bool PrivateCall
        {
            get
            {
                return Convert.ToBoolean(flag2 & 1);
            }
            set
            {
                if (value)
                {
                    flag2 |= 1;
                }
                else
                {
                    flag2 &= 254;
                }
            }
        }

        public int TaTxTS1
        {
            get
            {
                return flag1 & 3;
            }
            set
            {
                value &= 3;
                flag1 &= 252;
                flag1 |= (byte)value;
            }
        }

        public int TaTxTS2
        {
            get
            {
                return (flag1 & 0xC) >> 2;
            }
            set
            {
                value <<= 2;
                flag1 &= 243;
                flag1 |= (byte)value;
            }
        }

        public int Sql
        {
            get
            {
                if (sql >= 0 && sql <= 21)
                {
                    return sql;
                }
                return 0;
            }
            set
            {
                if (value >= 0 && sql <= 21)
                {
                    sql = (byte)value;
                }
            }
        }

        public string SquelchLevelStr
        {
            get
            {
                return SZ_SQUELCH_LEVEL[Sql];
            }
            set
            {
                Sql = Array.IndexOf(SZ_SQUELCH_LEVEL, value);
            }
        }

        public static string LatLonBin24ToString(uint value)
        {
            bool num = (value & 0x800000) != 0;
            uint num2 = value & 0x7FFFFF;
            uint num3 = num2 & 0x7FFF;
            int num4 = (int)(num2 >> 15);
            decimal num5 = num3;
            num5 /= 10000m;
            num5 += (decimal)num4;
            if (num)
            {
                num5 *= -1m;
            }
            return num5.ToString();
        }

        public static uint LatLonStringToBin24(string value)
        {
            NumberFormatInfo numberFormat = CultureInfo.CurrentCulture.NumberFormat;
            string s = value.Replace(".", numberFormat.NumberDecimalSeparator);
            decimal num = default(decimal);
            try
            {
                num = decimal.Parse(s);
            }
            catch (Exception)
            {
            }
            bool flag = num < 0m;
            int num2 = (int)num;
            num -= (decimal)num2;
            num *= 10000m;
            num = Math.Abs(num);
            uint num3 = (uint)num;
            uint num4 = (uint)(Math.Abs(num2) << 15) + num3;
            if (flag)
            {
                num4 |= 0x800000;
            }
            return num4;
        }

        public string GetZoneStringForChannelIndex(int index)
        {
            index++;
            ZoneForm.ZoneOne[] zoneList = ZoneForm.data.ZoneList;
            for (int i = 0; i < zoneList.Length; i++)
            {
                ZoneForm.ZoneOne zoneOne = zoneList[i];
                if (Array.FindAll(zoneOne.ChList, (ushort ch) => ch == index).Length != 0)
                {
                    return zoneOne.Name;
                }
            }
            return Settings.SZ_NONE;
        }

        public ChannelOne(int index)
        {
            this = default(ChannelOne);
            name = new byte[16];
        }

        public void Default()
        {
            ChMode = DefaultCh.ChMode;
            LibreDMR_Power = DefaultCh.LibreDMR_Power;
            Tot = DefaultCh.Tot;
            Latitude = DefaultCh.Latitude;
            Longitude = DefaultCh.Longitude;
            RxTone = DefaultCh.RxTone;
            TxTone = DefaultCh.TxTone;
            UnmuteRule = 0;
            RxSignaling = 0;
            ArtsInterval = 22m;
            Key = 0;
            TxColor = DefaultCh.TxColor;
            APRS_System = DefaultCh.APRS_System;
            Contact = DefaultCh.Contact;
            Flag1 = DefaultCh.Flag1;
            Flag2 = DefaultCh.Flag2;
            Flag3 = DefaultCh.Flag3;
            Flag4 = DefaultCh.Flag4;
            Sql = DefaultCh.Sql;
            Latitude = DefaultCh.Latitude;
            Longitude = DefaultCh.Longitude;
            OpenGD77RUS = DefaultCh.OpenGD77RUS;
        }

        public ChannelOne Clone()
        {
            return Settings.cloneObject(this);
        }

        public void Verify(ChannelOne def)
        {
            if (!Enum.IsDefined(typeof(ChModeE), ChMode))
            {
                ChMode = def.ChMode;
            }
            if (!Enum.IsDefined(typeof(LibreDMR_Power_E), LibreDMR_Power))
            {
                LibreDMR_Power = def.LibreDMR_Power;
            }
            Settings.ValidateNumberRangeWithDefault(ref tot, 0, 33, def.tot);
            if (rxGroupList != 0 && rxGroupList <= 76)
            {
                if (!RxGroupListForm.data.DataIsValid(rxGroupList - 1))
                {
                    rxGroupList = 0;
                }
            }
            else
            {
                rxGroupList = 0;
            }
            if (aprsSystem != 0 && aprsSystem <= APRSForm.data.Count)
            {
                if (!APRSForm.data.DataIsValid(aprsSystem - 1))
                {
                    aprsSystem = 0;
                }
            }
            else
            {
                aprsSystem = 0;
            }
            if (new Regex("D[0-7]{3}[N|I]$").IsMatch(RxTone))
            {
                Ste = 0;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Channel : IData
    {
        public delegate void ChModeDelegate(object sender, ChModeChangeEventArgs e);

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        private byte[] chIndex;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public ChannelOne[] chList;

        public ChannelOne this[int index]
        {
            get
            {
                if (index >= 1024)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return chList[index];
            }
            set
            {
                if (index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }
                chList[index] = value;
            }
        }

        public int Count => 1024;

        public int ValidCount
        {
            get
            {
                int num = 0;
                int num2 = 0;
                BitArray bitArray = new BitArray(chIndex);
                for (num = 0; num < bitArray.Count; num++)
                {
                    if (bitArray[num])
                    {
                        num2++;
                    }
                }
                return num2;
            }
        }

        public string Format => "Канал {0}";

        public bool ListIsEmpty
        {
            get
            {
                int num = 0;
                while (true)
                {
                    if (num < Count)
                    {
                        if (DataIsValid(num))
                        {
                            break;
                        }
                        num++;
                        continue;
                    }
                    return true;
                }
                return false;
            }
        }

        public event ChModeDelegate ChModeChangeEvent;

        public Channel()
        {
            int num = 0;
            chIndex = new byte[128];
            chList = new ChannelOne[1024];
            for (num = 0; num < chList.Length; num++)
            {
                chList[num] = new ChannelOne(num);
            }
        }

        public int GetDispIndex(int index)
        {
            int num = 0;
            int num2 = 0;
            BitArray bitArray = new BitArray(chIndex);
            for (num = 0; num <= index; num++)
            {
                if (bitArray[num])
                {
                    num2++;
                }
            }
            return num2;
        }

        public int GetMinIndex()
        {
            int num = 0;
            BitArray bitArray = new BitArray(chIndex);
            num = 0;
            while (true)
            {
                if (num < Count)
                {
                    if (!bitArray[num])
                    {
                        break;
                    }
                    num++;
                    continue;
                }
                return -1;
            }
            return num;
        }

        public bool DataIsValid(int index)
        {
            if (index > -1 && index < 1024)
            {
                return new BitArray(chIndex)[index];
            }
            return false;
        }

        public bool IsGroupCall(int index)
        {
            if (index > -1 && index < 1024 && data.DataIsValid(index) && chList[index].ChMode == 1)
            {
                int contact = chList[index].Contact;
                if (contact >= 1 && contact <= ContactForm.data.Count)
                {
                    return ContactForm.data.IsGroupCall(contact - 1);
                }
                return false;
            }
            return false;
        }

        public void SetIndex(int index, int value)
        {
            BitArray bitArray = new BitArray(chIndex);
            bitArray.Set(index, Convert.ToBoolean(value));
            bitArray.CopyTo(chIndex, 0);
            OpenGD77Form.ClearLastUsedChannelsData();
        }

        public void ClearIndex(int index)
        {
            SetIndex(index, 0);
            ZoneForm.data.ClearByData(index);
            NormalScanForm.data.ClearByData(index);
        }

        public void ClearIndexAndReset(int index)
        {
            ClearIndex(index);
            SetName(index, "");
            Default(index);
        }

        public void ClearByContact(int contactIndex)
        {
            int num = 0;
            for (num = 0; num < Count; num++)
            {
                if (DataIsValid(num) && chList[num].Contact == contactIndex + 1)
                {
                    chList[num].Contact = 0;
                }
            }
        }

        public void ClearByAPRS(int aprsIndex)
        {
            for (int i = 0; i < Count; i++)
            {
                if (DataIsValid(i) && chList[i].APRS_System == aprsIndex + 1)
                {
                    chList[i].APRS_System = 0;
                }
            }
        }

        public void ChangeAPRS_Index(int oldIndex, int newIndex)
        {
            for (int i = 0; i < Count; i++)
            {
                if (DataIsValid(i) && chList[i].APRS_System == oldIndex + 1)
                {
                    chList[i].APRS_System = newIndex + 1;
                }
            }
        }

        public void ClearByEncrypt(int keyIndex)
        {
        }

        public void ClearByRxGroup(int rxGrpIndex)
        {
            int num = 0;
            for (num = 0; num < Count; num++)
            {
                if (DataIsValid(num) && chList[num].RxGroupList == rxGrpIndex + 1)
                {
                    chList[num].RxGroupList = 0;
                }
            }
        }

        public string GetMinName(TreeNode node)
        {
            int num = 0;
            int num2 = 0;
            string text = "";
            num2 = GetMinIndex();
            text = string.Format(Format, num2 + 1);
            if (!Settings.smethod_51(node, text))
            {
                return text;
            }
            num = 0;
            while (true)
            {
                if (num < Count)
                {
                    text = string.Format(Format, num + 1);
                    if (!Settings.smethod_51(node, text))
                    {
                        break;
                    }
                    num++;
                    continue;
                }
                return "";
            }
            return text;
        }

        public bool NameExist(string name)
        {
            return chList.Any((ChannelOne x) => x.Name == name);
        }

        public int FindIndexForName(string name)
        {
            for (int i = 0; i < chList.Length; i++)
            {
                if (chList[i].Name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        public void SetName(int index, string text)
        {
            chList[index].Name = text;
        }

        public string GetName(int index)
        {
            return chList[index].Name;
        }

        public void Default(int index)
        {
            chList[index].Default();
        }

        public void Paste(int from, int to)
        {
            int chMode = chList[to].ChMode;
            chList[to].RxFreq = chList[from].RxFreq;
            chList[to].TxFreq = chList[from].TxFreq;
            chList[to].ChMode = chList[from].ChMode;
            chList[to].LibreDMR_Power = chList[from].LibreDMR_Power;
            chList[to].Tot = chList[from].Tot;
            chList[to].Latitude = chList[from].Latitude;
            chList[to].Longitude = chList[from].Longitude;
            chList[to].RxTone = chList[from].RxTone;
            chList[to].TxTone = chList[from].TxTone;
            chList[to].UnmuteRule = chList[from].UnmuteRule;
            chList[to].LibreDMR_DMRidChannel = chList[from].LibreDMR_DMRidChannel;
            chList[to].RxGroupList = chList[from].RxGroupList;
            chList[to].TxColor = chList[from].TxColor;
            chList[to].APRS_System = chList[from].APRS_System;
            chList[to].Contact = chList[from].Contact;
            chList[to].Flag1 = chList[from].Flag1;
            chList[to].Flag2 = chList[from].Flag2;
            chList[to].Flag3 = chList[from].Flag3;
            chList[to].Flag4 = chList[from].Flag4;
            chList[to].Sql = chList[from].Sql;
            chList[to].OpenGD77RUS = chList[from].OpenGD77RUS;
            if (chList[to].ChMode != chMode && this.ChModeChangeEvent != null)
            {
                this.ChModeChangeEvent(null, new ChModeChangeEventArgs(to, chList[to].ChMode));
            }
        }

        public int GetChMode(int index)
        {
            return chList[index].ChMode;
        }

        public void Verify()
        {
            int num = 0;
            uint num2 = 0u;
            uint uint_ = 0u;
            BitArray bitArray = new BitArray(chIndex);
            for (num = 0; num < Count; num++)
            {
                if (bitArray[num])
                {
                    num2 = chList[num].RxFreqDec / 100000;
                    if (Settings.checkFrequecyIsInValidRange(num2, ref uint_) < 0)
                    {
                        num2 = uint_ * 100000;
                        chList[num].RxFreqDec = num2;
                    }
                    if (Settings.checkFrequecyIsInValidRange(chList[num].TxFreqDec / 100000, ref uint_) < 0)
                    {
                        chList[num].TxFreqDec = chList[num].RxFreqDec;
                    }
                    chList[num].Verify(DefaultCh);
                }
            }
        }

        public void SetDefaultFreq(int index)
        {
            chList[index].UInt32_0 = Settings.smethod_35(Settings.MIN_FREQ[0] * 100000);
            chList[index].UInt32_1 = chList[index].UInt32_0;
        }

        public int FreqIsSameRange(int index)
        {
            return Settings.smethod_20(chList[index].RxFreqDec / 100000, chList[index].TxFreqDec / 100000);
        }

        public void SetChMode(int index, ChModeE chMode)
        {
            chList[index].ChMode = (int)chMode;
        }

        public void SetChMode(int index, string chMode)
        {
            chList[index].ChModeS = chMode;
        }

        public void SetRxFreq(int index, string rxFreq)
        {
            chList[index].RxFreq = rxFreq;
        }

        public void SetTxFreq(int index, string txFreq)
        {
            chList[index].TxFreq = txFreq;
        }

        public void SetPower(int index, string power)
        {
            chList[index].PowerString = power;
        }

        public void SetRepeaterSlot(int index, string repeaterSlot)
        {
            chList[index].RepeaterSlotS = repeaterSlot;
        }

        public void SetColorCode(int index, decimal colorCode)
        {
            chList[index].TxColor = colorCode;
        }

        public void SetRxGroupList(int index, int rxGroupList)
        {
            chList[index].RxGroupList = rxGroupList;
        }

        public void SetContact(int index, int contact)
        {
            chList[index].Contact = contact;
        }

        public void SetChName(int index, string name)
        {
            chList[index].Name = name;
        }

        public void SetRxTone(int index, string rxTone)
        {
            chList[index].RxTone = rxTone;
        }

        public void SetTxTone(int index, string txTone)
        {
            chList[index].TxTone = txTone;
        }

        public void SetChannelDMRid(int index, string dmrid)
        {
            chList[index].LibreDMR_DMRidChannel = dmrid;
        }

        public void SetChannelNoBeep(int index, bool value)
        {
            chList[index].LibreDMR_NoBeepChannel = value;
        }

        public void SetChannelNoEco(int index, bool value)
        {
            chList[index].LibreDMR_NoEcoChannel = value;
        }

        public void SetChannelFastcall(int index, bool value)
        {
            if (value) //если в канале устанавливаем быстрый вызов, в остальных стираем 
            {
                for (int num = 0; num < Count; num++)
                    chList[num].openGD77RUS_FastcallChannel = false;

            }
            chList[index].openGD77RUS_FastcallChannel = value;
        }

        public void SetChannelPriority(int index, bool value)
        {
            chList[index].openGD77RUS_PriorityChannel = value;
        }

        public void SetChannelRoaming(int index, bool value)
        {
            chList[index].LibreDMR_Roaming = value;
        }

        public void SetChannelUseLocation(int index, bool value)
        {
            chList[index].LibreDMR_UseLocation = value;
        }

        public void SetChannelDMRForceDMO(int index, bool value)
        {
            chList[index].LibreDMR_DMRForceDMO = value;
        }

        public int FindNextValidIndex(int index)
        {
            int num = index + 1;
            while (true)
            {
                if (num < Count)
                {
                    if (DataIsValid(num))
                    {
                        break;
                    }
                    num++;
                    continue;
                }
                num = index - 1;
                while (true)
                {
                    if (num >= 0)
                    {
                        if (DataIsValid(num))
                        {
                            break;
                        }
                        num--;
                        continue;
                    }
                    return -1;
                }
                return num;
            }
            return num;
        }

        public int FindPreviousValidIndex(int index)
        {
            int num = index - 1;
            while (true)
            {
                if (num >= 0)
                {
                    if (DataIsValid(num))
                    {
                        break;
                    }
                    num--;
                    continue;
                }
                return num;
            }
            return num;
        }

        public byte[] ToEerom(int chGroupIndex)
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            byte[] array = new byte[7184];
            num2 = 0;
            Array.Copy(chIndex, chGroupIndex * 16, array, 0, 16);
            num2 = 16;
            for (num = 0; num < 128; num++)
            {
                num3 = chGroupIndex * 128 + num;
                byte[] array2 = Settings.objectToByteArray(chList[num3], Marshal.SizeOf(chList[num3]));
                Array.Copy(array2, 0, array, num2, array2.Length);
                num2 += array2.Length;
            }
            return array;
        }

        public void FromEerom(int chGroupIndex, byte[] data)
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            num2 = 0;
            Array.Copy(data, 0, chIndex, chGroupIndex * 16, 16);
            num2 = 16;
            for (num = 0; num < 128; num++)
            {
                num3 = chGroupIndex * 128 + num;
                byte[] array = new byte[SPACE_CH];
                Array.Copy(data, num2, array, 0, array.Length);
                chList[num3] = (ChannelOne)Settings.byteArrayToObject(array, typeof(ChannelOne));
                num2 += array.Length;
                if (GeneralSetForm.data.CodeplugVersion == 0)
                {
                    chList[num3].LibreDMR_Power = 0;
                }
            }
        }
    }

    public const int CNT_CH_GRP = 8;

    public const int CNT_CH = 1024;

    public const int CNT_CH_PER_GROUP = 128;

    public const int ACTUAL_CNT_CH = 32;

    public const int LEN_CH_NAME = 16;

    public const int SPACE_GROUP_CH_INDEX = 16;

    public const string SZ_CH_MODE_NAME = "ChMode";

    public const int LEN_FREQ = 9;

    private const int SCL_FREQ = 100000;

    public const string SZ_REF_FREQ_NAME = "RefFreq";

    public const string SZ_POWER_NAME = "Power";

    private const string SZ_INFINITE = "Infinity";

    private const int MIN_TOT = 0;

    private const int MAX_TOT = 33;

    private const int INC_TOT = 1;

    private const int SCL_TOT = 15;

    private const int LEN_TOT = 3;

    private const int MIN_TOT_REKEY = 0;

    private const int MAX_TOT_REKEY = 255;

    private const int INC_TOT_REKEY = 1;

    private const int SCL_TOT_REKEY = 1;

    private const int LEN_TOT_REKEY = 3;

    public const string SZ_ADMIT_CRITERICA_NAME = "AdmitCriterica";

    public const string SZ_ADMIT_CRITERICA_D_NAME = "AdmitCritericaD";

    private const int MIN_RSSI_THRESHOLD = 80;

    private const int MAX_RSSI_THRESHOLD = 124;

    private const int INC_RSSI_THRESHOLD = 1;

    private const int SCL_RSSI_THRESHOLD = -1;

    private const int LEN_RSSI_THRESHOLD = 4;

    private const int MIN_SQL = 0;

    private const int MAX_SQL = 9;

    public const string SZ_SQUELCH_NAME = "Squelch";

    public const string SZ_SQUELCH_LEVEL_NAME = "SquelchLevel";

    public const string SZ_VOICE_EMPHASIS_NAME = "VoiceEmphasis";

    public const string SZ_STE_NAME = "Ste";

    public const string SZ_NON_STE_NAME = "NonSte";

    public const string SZ_SIGNALING_SYSTEM_NAME = "SignalingSystem";

    public const string SZ_UNMUTE_RULE_NAME = "UnmuteRule";

    public const string SZ_PTTID_TYPE_NAME = "PttidType";

    public const string SZ_ARTS_NAME = "Arts";

    public const string SZ_TIMING_PREFERENCE_NAME = "TimingPreference";

    public const string SZ_KEY_SWITCH_NAME = "KeySwitch";

    public static readonly int SPACE_CH;

    public static readonly int SPACE_CH_GROUP;

    public static readonly string[] SZ_CH_MODE;

    private static readonly string[] SZ_REF_FREQ;

    private static readonly string[] SZ_LIBREDMR_POWER;

    public static readonly string[] SZ_POWER;

    private static readonly string[] SZ_ADMIT_CRITERICA;

    private static readonly string[] SZ_ADMIT_CRITERICA_D;

    private static readonly string[] SZ_BANDWIDTH;

    private static readonly string[] SZ_SQUELCH;

    private static readonly string[] SZ_SQUELCH_LEVEL;

    private static readonly string[] SZ_VOICE_EMPHASIS;

    private static readonly string[] SZ_STE;

    private static readonly string[] SZ_NON_STE;

    private static readonly string[] SZ_SIGNALING_SYSTEM;

    private static readonly string[] SZ_UNMUTE_RULE;

    private static readonly string[] SZ_PTTID_TYPE;

    public static readonly string[] SZ_TA_TX;

    private static readonly string[] SZ_TIMING_PREFERENCE;

    public static readonly string[] SZ_REPEATER_SLOT;

    private static readonly string[] SZ_ARS;

    private static readonly string[] SZ_KEY_SWITCH;

    public static ChannelOne DefaultCh;

    public static Channel data;

    private Label lblContact;

    private CustomCombo cmbContact;

    private Label lblAPRSSystem;

    private CustomCombo cmbAPRS_System;

    private Label lblTxColor;

    private Label lblRxGroup;

    private CustomCombo cmbRxGroup;

    private Label lblRepeaterSlot;

    private ComboBox cmbRepeaterSlot;

    private Label lblTxTone;

    private ComboBox cmbTxTone;

    private Label lblRxTone;

    private ComboBox cmbRxTone;

    private Label lblChBandwidth;

    private ComboBox cmbChBandwidth;

    private Label lblChMode;

    private ComboBox cmbChMode;

    private Label lblChName;

    private SGTextBox txtName;

    private Label lblRxFreq;

    private TextBox txtRxFreq;

    private Label lblLibreDMR_Power;

    private ComboBox cmbLibreDMR_Power;

    private Label lblTxFreq;

    private TextBox txtTxFreq;

    private Label lblTot;

    private CustomNumericUpDown nudTot;

    private CheckBox chkVox;

    private CheckBox chkOpenGD77ScanZoneSkip;

    private CheckBox chkOpenGD77ScanAllSkip;

    private CheckBox chkRxOnly;

    private CheckBox chkNoBeep;

    private CheckBox chkNoEco;

    private DoubleClickGroupBox grpAnalog;

    private DoubleClickGroupBox grpDigit;

    private CustomNumericUpDown nudTxColor;

    private CustomCombo cmbTS1TaTx;

    private Label lblTS1TaTx;

    private ToolStrip tsrCh;

    private ToolStripButton tsbtnFirst;

    private ToolStripButton tsbtnPrev;

    private ToolStripButton tsbtnNext;

    private ToolStripButton tsbtnLast;

    private ToolStripSeparator toolStripSeparator1;

    private ToolStripButton tsbtnAdd;

    private ToolStripButton tsbtnDel;

    private ComboBox cmbSql;

    private Label lblSql;

    private MenuStrip mnsCh;

    private ToolStripMenuItem tsmiCh;

    private ToolStripMenuItem tsmiFirst;

    private ToolStripMenuItem tsmiPrev;

    private ToolStripMenuItem tsmiNext;

    private ToolStripMenuItem tsmiLast;

    private ToolStripMenuItem tsmiAdd;

    private ToolStripMenuItem tsmiDel;

    private ToolStripLabel tslblInfo;

    private ToolStripSeparator toolStripSeparator2;

    private CustomPanel pnlChannel;

    private Button btnCopy;

    private Label lblOverrideTxDMRId;

    private CustomCombo cmbTS2TaTx;

    private Label lblTS2TaTx;

    private Label lblLatitude;

    private SGTextBox txtLatitude;

    private Label lblLongitude;

    private SGTextBox txtLongitude;

    private CheckBox chkRoaming;

    private CheckBox chkDMRForceDMO;

    private GroupBox grpLocation;

    private CheckBox chkUseLocation;
    private CheckBox chkFastcall;
    private CheckBox chkPriority;
    private SGTextBox txtRadioId;

    public static int CurCntCh { get; set; }

    public TreeNode Node { get; set; }

    public void SaveData()
    {
        int num = Convert.ToInt32(base.Tag);
        if (num != -1)
        {
            int index = num % 1024;
            //
            ValidateChildren();
            ChannelOne value = new ChannelOne(num);
            if (txtName.Focused)
            {
                txtName_Leave(txtName, null);
            }
            if (txtRadioId.Focused)
            {
                txtRadioId_Leave(txtRadioId, null);
            }
            if (txtLongitude.Focused)
            {
                txtLatLong_Leave(txtLongitude, null);
            }
            if (txtLatitude.Focused)
            {
                txtLatLong_Leave(txtLatitude, null);
            }
            value.ChMode = cmbChMode.SelectedIndex;
            value.Name = txtName.Text;
            value.RxFreq = txtRxFreq.Text;
            value.LibreDMR_Power = cmbLibreDMR_Power.SelectedIndex;
            value.TxFreq = txtTxFreq.Text;
            value.Tot = nudTot.Value;
            value.Vox = chkVox.Checked;
            value.Latitude = txtLatitude.Text;
            value.Longitude = txtLongitude.Text;
            value.AutoScan = chkOpenGD77ScanZoneSkip.Checked;
            value.LoneWoker = chkOpenGD77ScanAllSkip.Checked;
            value.OnlyRx = chkRxOnly.Checked;
            value.openGD77RUS_FastcallChannel = chkFastcall.Checked;
            value.openGD77RUS_PriorityChannel = chkPriority.Checked;
            value.Bandwidth = cmbChBandwidth.SelectedIndex;
            value.Sql = cmbSql.SelectedIndex;
            value.UnmuteRule = data[index].UnmuteRule;
            value.RxSignaling = data[index].RxSignaling;
            value.ArtsInterval = data[index].ArtsInterval;
            value.Key = data[index].Key;
            value.RxTone = cmbRxTone.Text;
            value.TxTone = cmbTxTone.Text;
            value.TaTxTS1 = cmbTS1TaTx.method_3();
            value.TaTxTS2 = cmbTS2TaTx.method_3();
            value.RepeaterSlot = cmbRepeaterSlot.SelectedIndex;
            value.RxGroupList = cmbRxGroup.method_3();
            value.TxColor = nudTxColor.Value;
            value.APRS_System = cmbAPRS_System.method_3();
            value.Contact = cmbContact.method_3();
            data[index] = value;
        }
    }

    public void DispData()
    {
        int num = Convert.ToInt32(base.Tag);
        if (num == -1)
        {
            Close();
            return;
        }
        if (!data.DataIsValid(num))
        {
            num = data.FindNextValidIndex(num);
            Node = ((MainForm)base.MdiParent).GetTreeNodeByType(typeof(ChannelsForm), num);
            base.Tag = num;
        }
        int index = num % 1024;
        ChannelOne channelOne = data[index];
        method_1();
        if (channelOne.ChMode == 1)
        {
            if (channelOne.RxGroupList != 0)
            {
                channelOne.Contact = 0;
            }
        }
        else
        {
            channelOne.RxGroupList = 0;
            channelOne.Contact = 0;
        }
        cmbChMode.SelectedIndex = channelOne.ChMode;
        txtName.Text = channelOne.Name;
        txtRxFreq.Text = channelOne.RxFreq;
        cmbLibreDMR_Power.SelectedIndex = channelOne.LibreDMR_Power;
        txtTxFreq.Text = channelOne.TxFreq;
        nudTot.Value = channelOne.Tot;
        chkVox.Checked = channelOne.Vox;
        txtLatitude.Text = channelOne.Latitude;
        txtLongitude.Text = channelOne.Longitude;
        chkOpenGD77ScanZoneSkip.Checked = channelOne.AutoScan;
        chkOpenGD77ScanAllSkip.Checked = channelOne.LoneWoker;
        chkRxOnly.Checked = channelOne.OnlyRx;
        cmbChBandwidth.SelectedIndex = channelOne.Bandwidth;
        cmbSql.SelectedIndex = channelOne.Sql;
        txtLatitude.Text = channelOne.Latitude;
        txtLongitude.Text = channelOne.Longitude;
        txtRadioId.Text = channelOne.LibreDMR_DMRidChannel;
        chkNoBeep.Checked = channelOne.LibreDMR_NoBeepChannel;
        chkNoEco.Checked = channelOne.LibreDMR_NoEcoChannel;
        chkFastcall.Checked = channelOne.openGD77RUS_FastcallChannel;
        chkPriority.Checked = channelOne.openGD77RUS_PriorityChannel;
        chkRoaming.Checked = channelOne.LibreDMR_Roaming;
        chkUseLocation.Checked = channelOne.LibreDMR_UseLocation;
        chkDMRForceDMO.Checked = channelOne.LibreDMR_DMRForceDMO;
        cmbRxTone.Text = channelOne.RxTone;
        cmbTxTone.Text = channelOne.TxTone;
        cmbTS1TaTx.method_2(channelOne.TaTxTS1);
        cmbTS2TaTx.method_2(channelOne.TaTxTS2);
        cmbRepeaterSlot.SelectedIndex = channelOne.RepeaterSlot;
        cmbRxGroup.method_2(channelOne.RxGroupList);
        nudTxColor.Value = channelOne.TxColor;
        cmbAPRS_System.method_2(channelOne.APRS_System);
        cmbContact.method_2(channelOne.Contact);
        method_7();
        configureNavigationButtons();
        RefreshByUserMode();
        ValidateChildren();
    }

    public void RefreshByUserMode()
    {
        bool flag = Settings.getUserExpertSettings() == Settings.UserMode.Expert;
        lblTot.Enabled &= flag;
        nudTot.Enabled &= flag;
        chkOpenGD77ScanZoneSkip.Enabled = true;
        chkOpenGD77ScanAllSkip.Enabled = true;
        chkNoBeep.Enabled = true;
        chkNoEco.Enabled = true;
    }

    public void RefreshName()
    {
        int index = Convert.ToInt32(base.Tag) % 1024;
        txtName.Text = data[index].Name;
    }

    public ChannelForm()
    {
        InitializeComponent();
        ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ChannelForm));
        tsbtnFirst.Image = (Image)componentResourceManager.GetObject("tsbtnFirst.Image");
        tsbtnPrev.Image = (Image)componentResourceManager.GetObject("tsbtnPrev.Image");
        tsbtnNext.Image = (Image)componentResourceManager.GetObject("tsbtnNext.Image");
        tsbtnLast.Image = (Image)componentResourceManager.GetObject("tsbtnLast.Image");
        tsbtnAdd.Image = (Image)componentResourceManager.GetObject("tsbtnAdd.Image");
        tsbtnDel.Image = (Image)componentResourceManager.GetObject("tsbtnDel.Image");
        base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        Scale(Settings.smethod_6());
        CurCntCh = 1024;
    }

    public static void RefreshCommonLang()
    {
        string name = typeof(ChannelForm).Name;
        Settings.smethod_78("ChMode", SZ_CH_MODE, name);
        Settings.smethod_78("RefFreq", SZ_REF_FREQ, name);
        Settings.smethod_78("LibreDMR_Power", SZ_LIBREDMR_POWER, name);
        Settings.smethod_78("Power", SZ_POWER, name);
        Settings.smethod_78("AdmitCriterica", SZ_ADMIT_CRITERICA, name);
        Settings.smethod_78("AdmitCritericaD", SZ_ADMIT_CRITERICA_D, name);
        Settings.smethod_78("Squelch", SZ_SQUELCH, name);
        Settings.smethod_78("SquelchLevel", SZ_SQUELCH_LEVEL, name);
        Settings.smethod_78("VoiceEmphasis", SZ_VOICE_EMPHASIS, name);
        Settings.smethod_78("Ste", SZ_STE, name);
        Settings.smethod_78("NonSte", SZ_NON_STE, name);
        Settings.smethod_78("SignalingSystem", SZ_SIGNALING_SYSTEM, name);
        Settings.smethod_78("UnmuteRule", SZ_UNMUTE_RULE, name);
        Settings.smethod_78("PttidType", SZ_PTTID_TYPE, name);
        Settings.smethod_78("TaTx", SZ_TA_TX, name);
        Settings.smethod_78("TimingPreference", SZ_TIMING_PREFERENCE, name);
        Settings.smethod_78("Ars", SZ_ARS, name);
        Settings.smethod_78("KeySwitch", SZ_KEY_SWITCH, name);
    }

    private void ChannelForm_Shown(object sender, EventArgs e)
    {
        pnlChannel.Focus();
    }

    private void ChannelForm_Load(object sender, EventArgs e)
    {
        try
        {
            Settings.smethod_59(base.Controls);
            Settings.UpdateComponentTextsFromLanguageXmlData(this);
            Settings.UpdateToolStripFromLanguageXmlData(tsrCh.smethod_10(), base.Name);
            data.ChModeChangeEvent += method_2;
            populateRxAndTxToneComboboxesFromToneTxtFile();
            method_0();
            DispData();
            txtName.Focus();
            pnlChannel.Focus();
            txtName.Focus();
            int index = Convert.ToInt32(base.Tag) % 1024;
            txtRadioId.Text = data[index].LibreDMR_DMRidChannel;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void ChannelForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        SaveData();
    }

    private void method_0()
    {
        Settings.fillComboBox(cmbChMode, SZ_CH_MODE);
        txtName.MaxByteLength = 16;
        txtName.KeyPress += Settings.smethod_54;
        txtRxFreq.MaxLength = 9;
        txtRxFreq.KeyPress += Settings.smethod_55;
        txtTxFreq.MaxLength = 9;
        txtTxFreq.KeyPress += Settings.smethod_55;
        Settings.fillComboBox(cmbLibreDMR_Power, SZ_LIBREDMR_POWER);
        Settings.smethod_36(nudTot, new Class13(0, 33, 1, 15m, 3));
        nudTot.method_4(0m);
        nudTot.method_6("∞");
        Settings.fillComboBox(cmbChBandwidth, SZ_BANDWIDTH);
        Settings.fillComboBox(cmbSql, SZ_SQUELCH_LEVEL);
        Settings.smethod_39(cmbTS1TaTx, SZ_TA_TX);
        Settings.smethod_39(cmbTS2TaTx, SZ_TA_TX);
        Settings.fillComboBox(cmbRepeaterSlot, SZ_REPEATER_SLOT);
        Settings.smethod_36(nudTxColor, new Class13(0, 15, 1, 1m, 2));
        txtRadioId.MaxLength = 8;
        txtRadioId.InputString = "0123456789\b";
    }

    private void method_1()
    {
        Settings.smethod_44(cmbRxGroup, RxGroupListForm.data);
        Settings.smethod_44(cmbAPRS_System, APRSForm.data);
        Settings.smethod_44(cmbContact, ContactForm.data, Settings.SZ_NA);
    }

    private void btnCopy_Click(object sender, EventArgs e)
    {
        txtTxFreq.Text = txtRxFreq.Text;
        ValidateChildren();
    }

    private void txtName_Leave(object sender, EventArgs e)
    {
        txtName.Text = txtName.Text.Trim();
        if (Node.Text != txtName.Text)
        {
            if (Settings.nodeNameExistsOrEmpty(Node, txtName.Text))
            {
                MessageBox.Show(Settings.dicCommon[Settings.SZ_NAME_EXIST_NAME]);
                txtName.Text = Node.Text;
                return;
            }
            Node.Text = txtName.Text;
            int index = Convert.ToInt32(base.Tag);
            data.SetChName(index, txtName.Text);
            ((MainForm)base.MdiParent).RefreshRelatedForm(GetType(), self: false);
        }
    }

    private void cmbChMode_SelectedIndexChanged(object sender, EventArgs e)
    {
        int num = 0;
        switch (cmbChMode.SelectedIndex)
        {
            case 0:
                num = 2;
                grpAnalog.Enabled = true;
                grpDigit.Enabled = false;
                break;
            case 1:
                num = 6;
                grpAnalog.Enabled = false;
                grpDigit.Enabled = true;
                break;
            case 2:
                num = 54;
                grpAnalog.Enabled = true;
                grpDigit.Enabled = true;
                break;
            case 3:
                num = 54;
                grpAnalog.Enabled = true;
                grpDigit.Enabled = true;
                break;
        }
        Node.SelectedImageIndex = num;
        Node.ImageIndex = num;
        int index = Convert.ToInt32(base.Tag);
        data.SetChMode(index, cmbChMode.Text);
        ((MainForm)base.MdiParent).RefreshRelatedForm(GetType(), self: false);
    }

    private void method_2(object sender, ChModeChangeEventArgs e)
    {
        if (base.MdiParent is MainForm mainForm)
        {
            TreeNode treeNodeByTypeAndIndex = mainForm.GetTreeNodeByTypeAndIndex(typeof(ChannelForm), e.ChIndex, Node.Parent.Nodes);
            if (e.ChMode == 0)
            {
                treeNodeByTypeAndIndex.ImageIndex = 2;
                treeNodeByTypeAndIndex.SelectedImageIndex = 2;
            }
            else if (e.ChMode == 1)
            {
                treeNodeByTypeAndIndex.ImageIndex = 6;
                treeNodeByTypeAndIndex.SelectedImageIndex = 6;
            }
        }
    }

    private void txtRxFreq_Validating(object sender, CancelEventArgs e)
    {
        double num = 0.0;
        string s = txtRxFreq.Text;
        _ = txtTxFreq.Text;
        int index = Convert.ToInt32(base.Tag);
        try
        {
            uint uint_ = 0u;
            num = double.Parse(s);
            if (Settings.checkFrequecyIsInValidRange(num, ref uint_) >= 0)
            {
                num = Settings.convert10HzStepFreqToDecimalText(Settings.convertDecimalFreqTo10HzStepValue(num, 100000.0), 100000);
                txtRxFreq.Text = $"{num:f5}";
            }
            else
            {
                txtRxFreq.Text = $"{uint_:f5}";
            }
            num = double.Parse(txtRxFreq.Text);
            data.SetRxFreq(index, txtRxFreq.Text);
            ((MainForm)base.MdiParent).RefreshRelatedForm(GetType(), index);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            txtRxFreq.Text = $"{Settings.MIN_FREQ[0]:f5}";
        }
    }

    private void txtTxFreq_Validating(object sender, CancelEventArgs e)
    {
        double num = 0.0;
        string s = txtTxFreq.Text;
        _ = txtRxFreq.Text;
        int index = Convert.ToInt32(base.Tag);
        try
        {
            uint uint_ = 0u;
            num = double.Parse(s);
            if (Settings.checkFrequecyIsInValidRange(num, ref uint_) >= 0)
            {
                num = Settings.convert10HzStepFreqToDecimalText(Settings.convertDecimalFreqTo10HzStepValue(num, 100000.0), 100000);
                txtTxFreq.Text = $"{num:f5}";
            }
            else
            {
                txtTxFreq.Text = $"{uint_:f5}";
            }
            num = double.Parse(txtTxFreq.Text);
            data.SetTxFreq(index, txtTxFreq.Text);
            ((MainForm)base.MdiParent).RefreshRelatedForm(GetType(), index);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            txtTxFreq.Text = $"{Settings.MIN_FREQ[0]:f5}";
        }
    }

    private void nudTot_ValueChanged(object sender, EventArgs e)
    {
    }

    private void chkRxOnly_CheckedChanged(object sender, EventArgs e)
    {
        method_3();
    }

    private void method_3()
    {
        bool enabled = !chkRxOnly.Checked;
        txtTxFreq.Enabled = enabled;
        nudTot.Enabled = enabled;
        chkVox.Enabled = enabled;
    }

    private void method_4()
    {
        chkOpenGD77ScanAllSkip.Enabled = true;
        if (!chkOpenGD77ScanAllSkip.Enabled)
        {
            chkOpenGD77ScanAllSkip.Checked = false;
        }
    }

    private void method_5()
    {
        if (base.MdiParent is MainForm mainForm)
        {
            mainForm.RefreshRelatedForm(typeof(ChannelForm));
        }
    }

    private void handleInsertClick()
    {
        if (Node.Parent.Nodes.Count < CurCntCh)
        {
            SaveData();
            TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
            int minIndex = data.GetMinIndex();
            string minName = data.GetMinName(Node);
            data.SetIndex(minIndex, 1);
            TreeNodeItem tag = new TreeNodeItem(treeNodeItem.Cms, treeNodeItem.Type, null, 0, minIndex, treeNodeItem.ImageIndex, treeNodeItem.Data);
            TreeNode treeNode = new TreeNode(minName);
            treeNode.Tag = tag;
            treeNode.ImageIndex = 2;
            treeNode.SelectedImageIndex = 2;
            Node.Parent.Nodes.Insert(minIndex, treeNode);
            data.SetName(minIndex, minName);
            Node = treeNode;
            base.Tag = minIndex;
            DispData();
            method_5();
        }
    }

    private void handleDeleteClick()
    {
        if (Node.Parent.Nodes.Count > 1 && Node.Index != 0)
        {
            if (ZoneForm.data.FstZoneFstCh == (int)base.Tag + 1)
            {
                MessageBox.Show(Settings.dicCommon["FirstChNotDelete"]);
                return;
            }
            SaveData();
            TreeNode node = Node.NextNode ?? Node.PrevNode;
            TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
            data.ClearIndex(treeNodeItem.Index);
            Node.Remove();
            Node = node;
            TreeNodeItem treeNodeItem2 = Node.Tag as TreeNodeItem;
            base.Tag = treeNodeItem2.Index;
            DispData();
            method_5();
        }
    }

    private void method_7()
    {
    }

    private void cmbAPRS_SelectedIndexChanged(object sender, EventArgs e)
    {
        cmbTxTone.Enabled = cmbAPRS_System.SelectedIndex == 0;
    }

    private void populateRxAndTxToneComboboxesFromToneTxtFile()
    {
        NumberFormatInfo numberFormat = CultureInfo.CurrentCulture.NumberFormat;
        cmbRxTone.Items.Clear();
        cmbTxTone.Items.Clear();
        cmbRxTone.Items.Add(Settings.SZ_NONE);
        cmbTxTone.Items.Add(Settings.SZ_NONE);
        for (int i = 0; i < Settings.TONES_LIST.Length; i++)
        {
            string item = Settings.TONES_LIST[i].Replace(".", numberFormat.NumberDecimalSeparator);
            cmbRxTone.Items.Add(item);
            cmbTxTone.Items.Add(item);
        }
    }

    private void cmbRxTone_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Return)
        {
            SendKeys.Send("{tab}");
        }
    }

    private void cmbRxTone_Validating(object sender, CancelEventArgs e)
    {
        _ = string.Empty;
        string text = cmbRxTone.Text;
        try
        {
            if (!(text == Settings.SZ_NONE) && !string.IsNullOrEmpty(text))
            {
                if (new Regex("D[0-7]{3}N$").IsMatch(text))
                {
                    if (Convert.ToUInt16(text.Substring(1, 3), 8) < 777)
                    {
                        return;
                    }
                    cmbRxTone.Text = Settings.SZ_NONE;
                }
                if (new Regex("D[0-7]{3}I$").IsMatch(text))
                {
                    if (Convert.ToUInt16(text.Substring(1, 3), 8) < 777)
                    {
                        return;
                    }
                    cmbRxTone.Text = Settings.SZ_NONE;
                }
                double num = double.Parse(text);
                if (num >= 60.0 && num < 260.0)
                {
                    cmbRxTone.Text = num.ToString("0.0");
                }
                else
                {
                    cmbRxTone.Text = Settings.SZ_NONE;
                }
            }
            else
            {
                e.Cancel = false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            cmbRxTone.Text = Settings.SZ_NONE;
        }
        finally
        {
            int index = Convert.ToInt32(base.Tag);
            data.SetRxTone(index, cmbRxTone.Text);
            ((MainForm)base.MdiParent).RefreshRelatedForm(GetType(), index);
        }
    }

    private void cmbTxTone_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    private void cmbTxTone_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Return)
        {
            SendKeys.Send("{tab}");
        }
    }

    private void cmbTxTone_Validating(object sender, CancelEventArgs e)
    {
        _ = string.Empty;
        string text = cmbTxTone.Text;
        try
        {
            if (text == Settings.SZ_NONE)
            {
                return;
            }
            if (new Regex("D[0-7]{3}N$").IsMatch(text))
            {
                if (Convert.ToUInt16(text.Substring(1, 3), 8) < 777)
                {
                    return;
                }
                cmbTxTone.Text = Settings.SZ_NONE;
            }
            if (new Regex("D[0-7]{3}I$").IsMatch(text))
            {
                if (Convert.ToUInt16(text.Substring(1, 3), 8) < 777)
                {
                    return;
                }
                cmbTxTone.Text = Settings.SZ_NONE;
            }
            double num = double.Parse(text);
            if (num > 60.0 && num < 260.0)
            {
                cmbTxTone.Text = num.ToString("0.0");
            }
            else
            {
                cmbTxTone.Text = Settings.SZ_NONE;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            cmbTxTone.Text = Settings.SZ_NONE;
        }
        finally
        {
            int index = Convert.ToInt32(base.Tag);
            data.SetTxTone(index, cmbTxTone.Text);
            ((MainForm)base.MdiParent).RefreshRelatedForm(GetType(), index);
        }
    }

    private void tsbtnFirst_Click(object sender, EventArgs e)
    {
        SaveData();
        Node = Node.Parent.FirstNode;
        TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
        base.Tag = treeNodeItem.Index;
        DispData();
    }

    private void tsbtnPrev_Click(object sender, EventArgs e)
    {
        SaveData();
        Node = Node.PrevNode;
        TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
        base.Tag = treeNodeItem.Index;
        DispData();
    }

    private void tsbtnNext_Click(object sender, EventArgs e)
    {
        SaveData();
        Node = Node.NextNode;
        TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
        base.Tag = treeNodeItem.Index;
        DispData();
    }

    private void tsbtnLast_Click(object sender, EventArgs e)
    {
        SaveData();
        Node = Node.Parent.LastNode;
        TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
        base.Tag = treeNodeItem.Index;
        DispData();
    }

    private void tsmiAdd_Click(object sender, EventArgs e)
    {
        handleInsertClick();
    }

    private void tsmiDel_Click(object sender, EventArgs e)
    {
        handleDeleteClick();
    }

    private void configureNavigationButtons()
    {
        tsbtnAdd.Enabled = Node.Parent.Nodes.Count != CurCntCh;
        tsbtnDel.Enabled = Node.Parent.Nodes.Count != 1 && Node.Index != 0 && !method_17();
        tsbtnFirst.Enabled = Node != Node.Parent.FirstNode;
        tsbtnPrev.Enabled = Node != Node.Parent.FirstNode;
        tsbtnNext.Enabled = Node != Node.Parent.LastNode;
        tsbtnLast.Enabled = Node != Node.Parent.LastNode;
        tslblInfo.Text = $" {data.GetDispIndex(Convert.ToInt32(base.Tag))} / {data.ValidCount}";
    }

    private bool method_17()
    {
        if (ZoneForm.data.FstZoneFstCh == (int)base.Tag + 1)
        {
            return true;
        }
        return false;
    }

    private void cmbPower_SelectedIndexChanged(object sender, EventArgs e)
    {
        Convert.ToInt32(base.Tag);
        ((MainForm)base.MdiParent).RefreshRelatedForm(GetType(), self: false);
    }

    private void cmbRepeaterSlot_SelectedIndexChanged(object sender, EventArgs e)
    {
        int index = Convert.ToInt32(base.Tag);
        data.SetRepeaterSlot(index, cmbRepeaterSlot.Text);
        ((MainForm)base.MdiParent).RefreshRelatedForm(GetType(), index);
    }

    private void cmbRxGroup_SelectedIndexChanged(object sender, EventArgs e)
    {
        int index = Convert.ToInt32(base.Tag);
        int num = cmbRxGroup.method_3();
        data.SetRxGroupList(index, num);
        ((MainForm)base.MdiParent).RefreshRelatedForm(GetType(), index);
        if (num != 0 && data[index].ChMode == 1 && data[index].RxGroupList != 0)
        {
            data.SetContact(index, 0);
            cmbContact.SelectedIndex = 0;
        }
    }

    private void cmbContact_SelectedIndexChanged(object sender, EventArgs e)
    {
        int index = Convert.ToInt32(base.Tag);
        int num = cmbContact.method_3();
        data.SetContact(index, num);
        ((MainForm)base.MdiParent).RefreshRelatedForm(GetType(), index);
        if (num != 0 && data[index].ChMode == 1 && data[index].Contact != 0)
        {
            data.SetRxGroupList(index, 0);
            cmbRxGroup.SelectedIndex = 0;
        }
    }

    private void nudTxColor_ValueChanged(object sender, EventArgs e)
    {
        int index = Convert.ToInt32(base.Tag);
        data.SetColorCode(index, nudTxColor.Value);
        ((MainForm)base.MdiParent).RefreshRelatedForm(GetType(), index);
    }

    private void txtLatLong_Leave(object sender, EventArgs e)
    {
        SGTextBox obj = sender as SGTextBox;
        NumberFormatInfo numberFormat = CultureInfo.CurrentCulture.NumberFormat;
        uint value = ChannelOne.LatLonStringToBin24(obj.Text.Replace(".", numberFormat.NumberDecimalSeparator));
        obj.Text = ChannelOne.LatLonBin24ToString(value);
    }

    private void txtRadioId_Validating(object sender, CancelEventArgs e)
    {
        if (!string.IsNullOrEmpty(txtRadioId.Text))
        {
            int num = Convert.ToInt32(txtRadioId.Text);
            if (num < 1 || num > 16776415)
            {
                txtRadioId.Text = "";
            }
        }
    }

    private void txtRadioId_Leave(object sender, EventArgs e)
    {
        if (txtRadioId.Text.Length <= 8)
        {
            int num = Convert.ToInt32(base.Tag);
            data.SetChannelDMRid(num % 1024, txtRadioId.Text);
            txtRadioId.Text = data[num % 1024].LibreDMR_DMRidChannel;
        }
    }

    private void chkNoBeep_CheckedChanged(object sender, EventArgs e)
    {
        int num = Convert.ToInt32(base.Tag);
        data.SetChannelNoBeep(num % 1024, chkNoBeep.Checked);
    }

    private void chkNoEco_CheckedChanged(object sender, EventArgs e)
    {
        int num = Convert.ToInt32(base.Tag);
        data.SetChannelNoEco(num % 1024, chkNoEco.Checked);
    }

    private void chkFastcall_CheckedChanged(object sender, EventArgs e)
    {
        int num = Convert.ToInt32(base.Tag);
        data.SetChannelFastcall(num % 1024, chkFastcall.Checked);
    }

    private void chkPriority_CheckedChanged(object sender, EventArgs e)
    {
        int num = Convert.ToInt32(base.Tag);
        data.SetChannelPriority(num % 1024, chkPriority.Checked);
    }

    private void chkDMRForceDMO_CheckedChanged(object sender, EventArgs e)
    {
        int num = Convert.ToInt32(base.Tag);
        data.SetChannelDMRForceDMO(num % 1024, chkDMRForceDMO.Checked);
    }

    private void chkRoaming_CheckedChanged(object sender, EventArgs e)
    {
        int num = Convert.ToInt32(base.Tag);
        data.SetChannelRoaming(num % 1024, chkRoaming.Checked);
    }

    private void chkLocationValid_CheckedChanged(object sender, EventArgs e)
    {
        int num = Convert.ToInt32(base.Tag);
        data.SetChannelUseLocation(num % 1024, chkUseLocation.Checked);
    }

    public static void MoveChannelUp(int index, int prevIndex)
    {
        if (prevIndex != -1 && index != -1)
        {
            ChannelOne channelOne = data.chList[prevIndex].Clone();
            data.chList[prevIndex] = data.chList[index];
            data.chList[index] = channelOne;
            ZoneForm.UpdateZonesDataForChannelMoveUpDown(index, prevIndex);
            OpenGD77Form.ClearLastUsedChannelsData();
        }
    }

    public static void MoveChannelDown(int index, int nextIndex)
    {
        if (nextIndex != -1 && index != -1)
        {
            ChannelOne channelOne = data.chList[nextIndex].Clone();
            data.chList[nextIndex] = data.chList[index];
            data.chList[index] = channelOne;
            ZoneForm.UpdateZonesDataForChannelMoveUpDown(index, nextIndex);
            OpenGD77Form.ClearLastUsedChannelsData();
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
            this.tsrCh = new System.Windows.Forms.ToolStrip();
            this.tslblInfo = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnFirst = new System.Windows.Forms.ToolStripButton();
            this.tsbtnPrev = new System.Windows.Forms.ToolStripButton();
            this.tsbtnNext = new System.Windows.Forms.ToolStripButton();
            this.tsbtnLast = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnAdd = new System.Windows.Forms.ToolStripButton();
            this.tsbtnDel = new System.Windows.Forms.ToolStripButton();
            this.mnsCh = new System.Windows.Forms.MenuStrip();
            this.tsmiCh = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFirst = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPrev = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNext = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLast = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDel = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlChannel = new CustomPanel();
            this.chkPriority = new System.Windows.Forms.CheckBox();
            this.chkFastcall = new System.Windows.Forms.CheckBox();
            this.grpLocation = new System.Windows.Forms.GroupBox();
            this.chkUseLocation = new System.Windows.Forms.CheckBox();
            this.chkRoaming = new System.Windows.Forms.CheckBox();
            this.txtLatitude = new DMR.SGTextBox();
            this.txtLongitude = new DMR.SGTextBox();
            this.lblLatitude = new System.Windows.Forms.Label();
            this.lblLongitude = new System.Windows.Forms.Label();
            this.btnCopy = new System.Windows.Forms.Button();
            this.txtName = new DMR.SGTextBox();
            this.grpDigit = new DoubleClickGroupBox();
            this.chkDMRForceDMO = new System.Windows.Forms.CheckBox();
            this.txtRadioId = new DMR.SGTextBox();
            this.nudTxColor = new CustomNumericUpDown();
            this.cmbRepeaterSlot = new System.Windows.Forms.ComboBox();
            this.lblRepeaterSlot = new System.Windows.Forms.Label();
            this.cmbRxGroup = new CustomCombo();
            this.lblTxColor = new System.Windows.Forms.Label();
            this.cmbTS2TaTx = new CustomCombo();
            this.cmbTS1TaTx = new CustomCombo();
            this.cmbContact = new CustomCombo();
            this.lblContact = new System.Windows.Forms.Label();
            this.lblTS2TaTx = new System.Windows.Forms.Label();
            this.lblTS1TaTx = new System.Windows.Forms.Label();
            this.lblRxGroup = new System.Windows.Forms.Label();
            this.lblOverrideTxDMRId = new System.Windows.Forms.Label();
            this.chkRxOnly = new System.Windows.Forms.CheckBox();
            this.cmbLibreDMR_Power = new System.Windows.Forms.ComboBox();
            this.grpAnalog = new DoubleClickGroupBox();
            this.cmbSql = new System.Windows.Forms.ComboBox();
            this.lblSql = new System.Windows.Forms.Label();
            this.cmbChBandwidth = new System.Windows.Forms.ComboBox();
            this.lblChBandwidth = new System.Windows.Forms.Label();
            this.cmbRxTone = new System.Windows.Forms.ComboBox();
            this.lblRxTone = new System.Windows.Forms.Label();
            this.cmbAPRS_System = new CustomCombo();
            this.lblAPRSSystem = new System.Windows.Forms.Label();
            this.cmbTxTone = new System.Windows.Forms.ComboBox();
            this.lblTxTone = new System.Windows.Forms.Label();
            this.chkOpenGD77ScanAllSkip = new System.Windows.Forms.CheckBox();
            this.chkVox = new System.Windows.Forms.CheckBox();
            this.chkOpenGD77ScanZoneSkip = new System.Windows.Forms.CheckBox();
            this.chkNoBeep = new System.Windows.Forms.CheckBox();
            this.chkNoEco = new System.Windows.Forms.CheckBox();
            this.cmbChMode = new System.Windows.Forms.ComboBox();
            this.lblChName = new System.Windows.Forms.Label();
            this.txtTxFreq = new System.Windows.Forms.TextBox();
            this.lblChMode = new System.Windows.Forms.Label();
            this.lblTot = new System.Windows.Forms.Label();
            this.txtRxFreq = new System.Windows.Forms.TextBox();
            this.lblLibreDMR_Power = new System.Windows.Forms.Label();
            this.lblRxFreq = new System.Windows.Forms.Label();
            this.lblTxFreq = new System.Windows.Forms.Label();
            this.nudTot = new CustomNumericUpDown();
            this.tsrCh.SuspendLayout();
            this.mnsCh.SuspendLayout();
            this.pnlChannel.SuspendLayout();
            this.grpLocation.SuspendLayout();
            this.grpDigit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTxColor)).BeginInit();
            this.grpAnalog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTot)).BeginInit();
            this.SuspendLayout();
            // 
            // tsrCh
            // 
            this.tsrCh.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslblInfo,
            this.toolStripSeparator2,
            this.tsbtnFirst,
            this.tsbtnPrev,
            this.tsbtnNext,
            this.tsbtnLast,
            this.toolStripSeparator1,
            this.tsbtnAdd,
            this.tsbtnDel});
            this.tsrCh.Location = new System.Drawing.Point(0, 0);
            this.tsrCh.Name = "tsrCh";
            this.tsrCh.Size = new System.Drawing.Size(1209, 25);
            this.tsrCh.TabIndex = 31;
            this.tsrCh.Text = "toolStrip1";
            // 
            // tslblInfo
            // 
            this.tslblInfo.AutoSize = false;
            this.tslblInfo.Name = "tslblInfo";
            this.tslblInfo.Size = new System.Drawing.Size(100, 22);
            this.tslblInfo.Text = " 0 / 0";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbtnFirst
            // 
            this.tsbtnFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnFirst.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnFirst.Name = "tsbtnFirst";
            this.tsbtnFirst.Size = new System.Drawing.Size(23, 22);
            this.tsbtnFirst.Text = "First";
            this.tsbtnFirst.Click += new System.EventHandler(this.tsbtnFirst_Click);
            // 
            // tsbtnPrev
            // 
            this.tsbtnPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnPrev.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnPrev.Name = "tsbtnPrev";
            this.tsbtnPrev.Size = new System.Drawing.Size(23, 22);
            this.tsbtnPrev.Text = "Previous";
            this.tsbtnPrev.Click += new System.EventHandler(this.tsbtnPrev_Click);
            // 
            // tsbtnNext
            // 
            this.tsbtnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnNext.Name = "tsbtnNext";
            this.tsbtnNext.Size = new System.Drawing.Size(23, 22);
            this.tsbtnNext.Text = "Next";
            this.tsbtnNext.Click += new System.EventHandler(this.tsbtnNext_Click);
            // 
            // tsbtnLast
            // 
            this.tsbtnLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnLast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnLast.Name = "tsbtnLast";
            this.tsbtnLast.Size = new System.Drawing.Size(23, 22);
            this.tsbtnLast.Text = "Last";
            this.tsbtnLast.Click += new System.EventHandler(this.tsbtnLast_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbtnAdd
            // 
            this.tsbtnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnAdd.Name = "tsbtnAdd";
            this.tsbtnAdd.Size = new System.Drawing.Size(23, 22);
            this.tsbtnAdd.Text = "Add";
            this.tsbtnAdd.Click += new System.EventHandler(this.tsmiAdd_Click);
            // 
            // tsbtnDel
            // 
            this.tsbtnDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnDel.Name = "tsbtnDel";
            this.tsbtnDel.Size = new System.Drawing.Size(23, 22);
            this.tsbtnDel.Text = "Delete";
            this.tsbtnDel.Click += new System.EventHandler(this.tsmiDel_Click);
            // 
            // mnsCh
            // 
            this.mnsCh.AllowMerge = false;
            this.mnsCh.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCh});
            this.mnsCh.Location = new System.Drawing.Point(0, 0);
            this.mnsCh.Name = "mnsCh";
            this.mnsCh.Size = new System.Drawing.Size(1104, 25);
            this.mnsCh.TabIndex = 32;
            this.mnsCh.Text = "menuStrip1";
            this.mnsCh.Visible = false;
            // 
            // tsmiCh
            // 
            this.tsmiCh.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFirst,
            this.tsmiPrev,
            this.tsmiNext,
            this.tsmiLast,
            this.tsmiAdd,
            this.tsmiDel});
            this.tsmiCh.Name = "tsmiCh";
            this.tsmiCh.Size = new System.Drawing.Size(83, 21);
            this.tsmiCh.Text = "Operation";
            // 
            // tsmiFirst
            // 
            this.tsmiFirst.Name = "tsmiFirst";
            this.tsmiFirst.Size = new System.Drawing.Size(130, 24);
            this.tsmiFirst.Text = "First";
            // 
            // tsmiPrev
            // 
            this.tsmiPrev.Name = "tsmiPrev";
            this.tsmiPrev.Size = new System.Drawing.Size(130, 24);
            this.tsmiPrev.Text = "Previous";
            // 
            // tsmiNext
            // 
            this.tsmiNext.Name = "tsmiNext";
            this.tsmiNext.Size = new System.Drawing.Size(130, 24);
            this.tsmiNext.Text = "Next";
            // 
            // tsmiLast
            // 
            this.tsmiLast.Name = "tsmiLast";
            this.tsmiLast.Size = new System.Drawing.Size(130, 24);
            this.tsmiLast.Text = "Last";
            // 
            // tsmiAdd
            // 
            this.tsmiAdd.Name = "tsmiAdd";
            this.tsmiAdd.Size = new System.Drawing.Size(130, 24);
            this.tsmiAdd.Text = "Add";
            this.tsmiAdd.Click += new System.EventHandler(this.tsmiAdd_Click);
            // 
            // tsmiDel
            // 
            this.tsmiDel.Name = "tsmiDel";
            this.tsmiDel.Size = new System.Drawing.Size(130, 24);
            this.tsmiDel.Text = "Delete";
            this.tsmiDel.Click += new System.EventHandler(this.tsmiDel_Click);
            // 
            // pnlChannel
            // 
            this.pnlChannel.AutoScroll = true;
            this.pnlChannel.AutoSize = true;
            this.pnlChannel.Controls.Add(this.chkPriority);
            this.pnlChannel.Controls.Add(this.chkFastcall);
            this.pnlChannel.Controls.Add(this.grpLocation);
            this.pnlChannel.Controls.Add(this.btnCopy);
            this.pnlChannel.Controls.Add(this.txtName);
            this.pnlChannel.Controls.Add(this.grpDigit);
            this.pnlChannel.Controls.Add(this.chkRxOnly);
            this.pnlChannel.Controls.Add(this.cmbLibreDMR_Power);
            this.pnlChannel.Controls.Add(this.grpAnalog);
            this.pnlChannel.Controls.Add(this.chkOpenGD77ScanAllSkip);
            this.pnlChannel.Controls.Add(this.chkVox);
            this.pnlChannel.Controls.Add(this.chkOpenGD77ScanZoneSkip);
            this.pnlChannel.Controls.Add(this.chkNoBeep);
            this.pnlChannel.Controls.Add(this.chkNoEco);
            this.pnlChannel.Controls.Add(this.cmbChMode);
            this.pnlChannel.Controls.Add(this.lblChName);
            this.pnlChannel.Controls.Add(this.txtTxFreq);
            this.pnlChannel.Controls.Add(this.lblChMode);
            this.pnlChannel.Controls.Add(this.lblTot);
            this.pnlChannel.Controls.Add(this.txtRxFreq);
            this.pnlChannel.Controls.Add(this.lblLibreDMR_Power);
            this.pnlChannel.Controls.Add(this.lblRxFreq);
            this.pnlChannel.Controls.Add(this.lblTxFreq);
            this.pnlChannel.Controls.Add(this.nudTot);
            this.pnlChannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChannel.Location = new System.Drawing.Point(0, 25);
            this.pnlChannel.Name = "pnlChannel";
            this.pnlChannel.Size = new System.Drawing.Size(1209, 395);
            this.pnlChannel.TabIndex = 0;
            this.pnlChannel.TabStop = true;
            // 
            // chkPriority
            // 
            this.chkPriority.AutoSize = true;
            this.chkPriority.Location = new System.Drawing.Point(888, 11);
            this.chkPriority.Name = "chkPriority";
            this.chkPriority.Size = new System.Drawing.Size(104, 20);
            this.chkPriority.TabIndex = 38;
            this.chkPriority.Text = "Priority scan";
            this.chkPriority.UseVisualStyleBackColor = true;
            this.chkPriority.CheckedChanged += new System.EventHandler(this.chkPriority_CheckedChanged);
            // 
            // chkFastcall
            // 
            this.chkFastcall.AutoSize = true;
            this.chkFastcall.Location = new System.Drawing.Point(888, 144);
            this.chkFastcall.Name = "chkFastcall";
            this.chkFastcall.Size = new System.Drawing.Size(75, 20);
            this.chkFastcall.TabIndex = 37;
            this.chkFastcall.Text = "Fastcall";
            this.chkFastcall.UseVisualStyleBackColor = true;
            this.chkFastcall.CheckedChanged += new System.EventHandler(this.chkFastcall_CheckedChanged);
            // 
            // grpLocation
            // 
            this.grpLocation.Controls.Add(this.chkUseLocation);
            this.grpLocation.Controls.Add(this.chkRoaming);
            this.grpLocation.Controls.Add(this.txtLatitude);
            this.grpLocation.Controls.Add(this.txtLongitude);
            this.grpLocation.Controls.Add(this.lblLatitude);
            this.grpLocation.Controls.Add(this.lblLongitude);
            this.grpLocation.Location = new System.Drawing.Point(20, 87);
            this.grpLocation.Name = "grpLocation";
            this.grpLocation.Size = new System.Drawing.Size(531, 78);
            this.grpLocation.TabIndex = 36;
            this.grpLocation.TabStop = false;
            this.grpLocation.Text = "Location";
            // 
            // chkUseLocation
            // 
            this.chkUseLocation.AccessibleDescription = "Use location";
            this.chkUseLocation.AccessibleName = "Use location";
            this.chkUseLocation.Location = new System.Drawing.Point(34, 52);
            this.chkUseLocation.Name = "chkUseLocation";
            this.chkUseLocation.Size = new System.Drawing.Size(466, 20);
            this.chkUseLocation.TabIndex = 35;
            this.chkUseLocation.Text = "Use location";
            this.chkUseLocation.UseVisualStyleBackColor = true;
            this.chkUseLocation.CheckedChanged += new System.EventHandler(this.chkLocationValid_CheckedChanged);
            // 
            // chkRoaming
            // 
            this.chkRoaming.AccessibleDescription = "Roaming";
            this.chkRoaming.AccessibleName = "Roaming";
            this.chkRoaming.Location = new System.Drawing.Point(305, 52);
            this.chkRoaming.Name = "chkRoaming";
            this.chkRoaming.Size = new System.Drawing.Size(195, 20);
            this.chkRoaming.TabIndex = 35;
            this.chkRoaming.Text = "Roaming...";
            this.chkRoaming.UseVisualStyleBackColor = true;
            this.chkRoaming.Visible = false;
            this.chkRoaming.CheckedChanged += new System.EventHandler(this.chkRoaming_CheckedChanged);
            // 
            // txtLatitude
            // 
            this.txtLatitude.AccessibleDescription = "Latitude";
            this.txtLatitude.AccessibleName = "Latitude";
            this.txtLatitude.InputString = null;
            this.txtLatitude.Location = new System.Drawing.Point(97, 20);
            this.txtLatitude.MaxByteLength = 0;
            this.txtLatitude.Name = "txtLatitude";
            this.txtLatitude.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtLatitude.Size = new System.Drawing.Size(120, 23);
            this.txtLatitude.TabIndex = 20;
            this.txtLatitude.Leave += new System.EventHandler(this.txtLatLong_Leave);
            // 
            // txtLongitude
            // 
            this.txtLongitude.AccessibleDescription = "Longitude";
            this.txtLongitude.AccessibleName = "Longitude";
            this.txtLongitude.InputString = null;
            this.txtLongitude.Location = new System.Drawing.Point(383, 21);
            this.txtLongitude.MaxByteLength = 0;
            this.txtLongitude.Name = "txtLongitude";
            this.txtLongitude.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtLongitude.Size = new System.Drawing.Size(120, 23);
            this.txtLongitude.TabIndex = 21;
            this.txtLongitude.Leave += new System.EventHandler(this.txtLatLong_Leave);
            // 
            // lblLatitude
            // 
            this.lblLatitude.Location = new System.Drawing.Point(12, 20);
            this.lblLatitude.Name = "lblLatitude";
            this.lblLatitude.Size = new System.Drawing.Size(76, 24);
            this.lblLatitude.TabIndex = 2;
            this.lblLatitude.Text = "Latitude";
            this.lblLatitude.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblLongitude
            // 
            this.lblLongitude.Location = new System.Drawing.Point(267, 19);
            this.lblLongitude.Name = "lblLongitude";
            this.lblLongitude.Size = new System.Drawing.Size(106, 24);
            this.lblLongitude.TabIndex = 2;
            this.lblLongitude.Text = "Longitude";
            this.lblLongitude.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnCopy
            // 
            this.btnCopy.AccessibleDescription = "Copy Rx frequency to Tx frequency";
            this.btnCopy.AccessibleName = "Copy Rx frequency to Tx frequency";
            this.btnCopy.BackColor = System.Drawing.SystemColors.Control;
            this.btnCopy.Location = new System.Drawing.Point(490, 25);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(33, 23);
            this.btnCopy.TabIndex = 3;
            this.btnCopy.Text = ">>";
            this.btnCopy.UseVisualStyleBackColor = false;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // txtName
            // 
            this.txtName.AccessibleDescription = "Channel Name";
            this.txtName.AccessibleName = "Name";
            this.txtName.InputString = null;
            this.txtName.Location = new System.Drawing.Point(82, 56);
            this.txtName.MaxByteLength = 0;
            this.txtName.Name = "txtName";
            this.txtName.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtName.Size = new System.Drawing.Size(119, 23);
            this.txtName.TabIndex = 10;
            this.txtName.Leave += new System.EventHandler(this.txtName_Leave);
            // 
            // grpDigit
            // 
            this.grpDigit.Controls.Add(this.chkDMRForceDMO);
            this.grpDigit.Controls.Add(this.txtRadioId);
            this.grpDigit.Controls.Add(this.nudTxColor);
            this.grpDigit.Controls.Add(this.cmbRepeaterSlot);
            this.grpDigit.Controls.Add(this.lblRepeaterSlot);
            this.grpDigit.Controls.Add(this.cmbRxGroup);
            this.grpDigit.Controls.Add(this.lblTxColor);
            this.grpDigit.Controls.Add(this.cmbTS2TaTx);
            this.grpDigit.Controls.Add(this.cmbTS1TaTx);
            this.grpDigit.Controls.Add(this.cmbContact);
            this.grpDigit.Controls.Add(this.lblContact);
            this.grpDigit.Controls.Add(this.lblTS2TaTx);
            this.grpDigit.Controls.Add(this.lblTS1TaTx);
            this.grpDigit.Controls.Add(this.lblRxGroup);
            this.grpDigit.Controls.Add(this.lblOverrideTxDMRId);
            this.grpDigit.Location = new System.Drawing.Point(441, 171);
            this.grpDigit.Name = "grpDigit";
            this.grpDigit.Size = new System.Drawing.Size(735, 210);
            this.grpDigit.TabIndex = 30;
            this.grpDigit.TabStop = false;
            this.grpDigit.Text = "Digital";
            // 
            // chkDMRForceDMO
            // 
            this.chkDMRForceDMO.AccessibleDescription = "DMR Force DMO";
            this.chkDMRForceDMO.AccessibleName = "DMR Force DMO";
            this.chkDMRForceDMO.AutoSize = true;
            this.chkDMRForceDMO.Location = new System.Drawing.Point(245, 51);
            this.chkDMRForceDMO.Name = "chkDMRForceDMO";
            this.chkDMRForceDMO.Size = new System.Drawing.Size(99, 20);
            this.chkDMRForceDMO.TabIndex = 35;
            this.chkDMRForceDMO.Text = "Force DMO";
            this.chkDMRForceDMO.UseVisualStyleBackColor = true;
            this.chkDMRForceDMO.CheckedChanged += new System.EventHandler(this.chkDMRForceDMO_CheckedChanged);
            // 
            // txtRadioId
            // 
            this.txtRadioId.AccessibleDescription = "DMR ID";
            this.txtRadioId.AccessibleName = "DMR ID";
            this.txtRadioId.InputString = null;
            this.txtRadioId.Location = new System.Drawing.Point(245, 21);
            this.txtRadioId.MaxByteLength = 0;
            this.txtRadioId.Name = "txtRadioId";
            this.txtRadioId.OnlyAllowInputStringAndCapitaliseCharacters = false;
            this.txtRadioId.Size = new System.Drawing.Size(120, 23);
            this.txtRadioId.TabIndex = 50;
            this.txtRadioId.Leave += new System.EventHandler(this.txtRadioId_Leave);
            this.txtRadioId.Validating += new System.ComponentModel.CancelEventHandler(this.txtRadioId_Validating);
            // 
            // nudTxColor
            // 
            this.nudTxColor.AccessibleDescription = "Colour code";
            this.nudTxColor.AccessibleName = "Colour code";
            this.nudTxColor.Location = new System.Drawing.Point(519, 50);
            this.nudTxColor.Name = "nudTxColor";
            this.nudTxColor.Size = new System.Drawing.Size(120, 23);
            this.nudTxColor.TabIndex = 61;
            this.nudTxColor.ValueChanged += new System.EventHandler(this.nudTxColor_ValueChanged);
            // 
            // cmbRepeaterSlot
            // 
            this.cmbRepeaterSlot.AccessibleDescription = "Timeslot";
            this.cmbRepeaterSlot.AccessibleName = "Timeslot";
            this.cmbRepeaterSlot.BackColor = System.Drawing.Color.White;
            this.cmbRepeaterSlot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRepeaterSlot.FormattingEnabled = true;
            this.cmbRepeaterSlot.Location = new System.Drawing.Point(519, 107);
            this.cmbRepeaterSlot.Name = "cmbRepeaterSlot";
            this.cmbRepeaterSlot.Size = new System.Drawing.Size(119, 24);
            this.cmbRepeaterSlot.TabIndex = 63;
            this.cmbRepeaterSlot.SelectedIndexChanged += new System.EventHandler(this.cmbRepeaterSlot_SelectedIndexChanged);
            // 
            // lblRepeaterSlot
            // 
            this.lblRepeaterSlot.Location = new System.Drawing.Point(315, 107);
            this.lblRepeaterSlot.Name = "lblRepeaterSlot";
            this.lblRepeaterSlot.Size = new System.Drawing.Size(191, 24);
            this.lblRepeaterSlot.TabIndex = 37;
            this.lblRepeaterSlot.Text = "Repeater/Time Slot";
            this.lblRepeaterSlot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbRxGroup
            // 
            this.cmbRxGroup.AccessibleDescription = "Talkgroup list";
            this.cmbRxGroup.AccessibleName = "Talkgroup list";
            this.cmbRxGroup.BackColor = System.Drawing.Color.White;
            this.cmbRxGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRxGroup.FormattingEnabled = true;
            this.cmbRxGroup.Location = new System.Drawing.Point(519, 21);
            this.cmbRxGroup.Name = "cmbRxGroup";
            this.cmbRxGroup.Size = new System.Drawing.Size(119, 24);
            this.cmbRxGroup.TabIndex = 60;
            this.cmbRxGroup.SelectedIndexChanged += new System.EventHandler(this.cmbRxGroup_SelectedIndexChanged);
            // 
            // lblTxColor
            // 
            this.lblTxColor.Location = new System.Drawing.Point(315, 50);
            this.lblTxColor.Name = "lblTxColor";
            this.lblTxColor.Size = new System.Drawing.Size(191, 24);
            this.lblTxColor.TabIndex = 33;
            this.lblTxColor.Text = "Color Code";
            this.lblTxColor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbTS2TaTx
            // 
            this.cmbTS2TaTx.AccessibleDescription = "TS2 Talker alias transmit";
            this.cmbTS2TaTx.AccessibleName = "TS2 Talker Alias Transmit";
            this.cmbTS2TaTx.BackColor = System.Drawing.Color.White;
            this.cmbTS2TaTx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTS2TaTx.FormattingEnabled = true;
            this.cmbTS2TaTx.Location = new System.Drawing.Point(519, 167);
            this.cmbTS2TaTx.Name = "cmbTS2TaTx";
            this.cmbTS2TaTx.Size = new System.Drawing.Size(118, 24);
            this.cmbTS2TaTx.TabIndex = 65;
            // 
            // cmbTS1TaTx
            // 
            this.cmbTS1TaTx.AccessibleDescription = "TS1 Talker alias transmit";
            this.cmbTS1TaTx.AccessibleName = "TS1 Talker alias Transmit";
            this.cmbTS1TaTx.BackColor = System.Drawing.Color.White;
            this.cmbTS1TaTx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTS1TaTx.FormattingEnabled = true;
            this.cmbTS1TaTx.Location = new System.Drawing.Point(519, 137);
            this.cmbTS1TaTx.Name = "cmbTS1TaTx";
            this.cmbTS1TaTx.Size = new System.Drawing.Size(118, 24);
            this.cmbTS1TaTx.TabIndex = 64;
            // 
            // cmbContact
            // 
            this.cmbContact.AccessibleDescription = "Contact name";
            this.cmbContact.AccessibleName = "Contact name";
            this.cmbContact.BackColor = System.Drawing.Color.White;
            this.cmbContact.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbContact.FormattingEnabled = true;
            this.cmbContact.Location = new System.Drawing.Point(519, 79);
            this.cmbContact.Name = "cmbContact";
            this.cmbContact.Size = new System.Drawing.Size(119, 24);
            this.cmbContact.TabIndex = 62;
            this.cmbContact.SelectedIndexChanged += new System.EventHandler(this.cmbContact_SelectedIndexChanged);
            // 
            // lblContact
            // 
            this.lblContact.Location = new System.Drawing.Point(312, 79);
            this.lblContact.Name = "lblContact";
            this.lblContact.Size = new System.Drawing.Size(194, 24);
            this.lblContact.TabIndex = 35;
            this.lblContact.Text = "Contact Name";
            this.lblContact.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTS2TaTx
            // 
            this.lblTS2TaTx.Location = new System.Drawing.Point(312, 167);
            this.lblTS2TaTx.Name = "lblTS2TaTx";
            this.lblTS2TaTx.Size = new System.Drawing.Size(194, 24);
            this.lblTS2TaTx.TabIndex = 41;
            this.lblTS2TaTx.Text = "TS2 Talker Alias Tx";
            this.lblTS2TaTx.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTS1TaTx
            // 
            this.lblTS1TaTx.Location = new System.Drawing.Point(312, 137);
            this.lblTS1TaTx.Name = "lblTS1TaTx";
            this.lblTS1TaTx.Size = new System.Drawing.Size(194, 24);
            this.lblTS1TaTx.TabIndex = 39;
            this.lblTS1TaTx.Text = "TS1 Talker Alias Tx";
            this.lblTS1TaTx.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblRxGroup
            // 
            this.lblRxGroup.Location = new System.Drawing.Point(315, 21);
            this.lblRxGroup.Name = "lblRxGroup";
            this.lblRxGroup.Size = new System.Drawing.Size(191, 24);
            this.lblRxGroup.TabIndex = 31;
            this.lblRxGroup.Text = "Rx Group List";
            this.lblRxGroup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblOverrideTxDMRId
            // 
            this.lblOverrideTxDMRId.Location = new System.Drawing.Point(13, 22);
            this.lblOverrideTxDMRId.Name = "lblOverrideTxDMRId";
            this.lblOverrideTxDMRId.Size = new System.Drawing.Size(226, 24);
            this.lblOverrideTxDMRId.TabIndex = 29;
            this.lblOverrideTxDMRId.Text = "Override DMR Tx ID";
            this.lblOverrideTxDMRId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkRxOnly
            // 
            this.chkRxOnly.AccessibleDescription = "Rx only";
            this.chkRxOnly.AccessibleName = "Rx only";
            this.chkRxOnly.Location = new System.Drawing.Point(888, 87);
            this.chkRxOnly.Name = "chkRxOnly";
            this.chkRxOnly.Size = new System.Drawing.Size(236, 20);
            this.chkRxOnly.TabIndex = 33;
            this.chkRxOnly.Text = "Rx Only";
            this.chkRxOnly.UseVisualStyleBackColor = true;
            this.chkRxOnly.CheckedChanged += new System.EventHandler(this.chkRxOnly_CheckedChanged);
            // 
            // cmbLibreDMR_Power
            // 
            this.cmbLibreDMR_Power.AccessibleDescription = "Power level";
            this.cmbLibreDMR_Power.AccessibleName = "Power level";
            this.cmbLibreDMR_Power.BackColor = System.Drawing.Color.White;
            this.cmbLibreDMR_Power.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLibreDMR_Power.FormattingEnabled = true;
            this.cmbLibreDMR_Power.Location = new System.Drawing.Point(368, 57);
            this.cmbLibreDMR_Power.Name = "cmbLibreDMR_Power";
            this.cmbLibreDMR_Power.Size = new System.Drawing.Size(170, 24);
            this.cmbLibreDMR_Power.TabIndex = 11;
            // 
            // grpAnalog
            // 
            this.grpAnalog.Controls.Add(this.cmbSql);
            this.grpAnalog.Controls.Add(this.lblSql);
            this.grpAnalog.Controls.Add(this.cmbChBandwidth);
            this.grpAnalog.Controls.Add(this.lblChBandwidth);
            this.grpAnalog.Controls.Add(this.cmbRxTone);
            this.grpAnalog.Controls.Add(this.lblRxTone);
            this.grpAnalog.Controls.Add(this.cmbAPRS_System);
            this.grpAnalog.Controls.Add(this.lblAPRSSystem);
            this.grpAnalog.Controls.Add(this.cmbTxTone);
            this.grpAnalog.Controls.Add(this.lblTxTone);
            this.grpAnalog.Location = new System.Drawing.Point(20, 171);
            this.grpAnalog.Name = "grpAnalog";
            this.grpAnalog.Size = new System.Drawing.Size(413, 210);
            this.grpAnalog.TabIndex = 29;
            this.grpAnalog.TabStop = false;
            this.grpAnalog.Text = "Analog";
            // 
            // cmbSql
            // 
            this.cmbSql.AccessibleDescription = "Channel squelch level";
            this.cmbSql.AccessibleName = "Channel squelch level";
            this.cmbSql.BackColor = System.Drawing.Color.White;
            this.cmbSql.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSql.FormattingEnabled = true;
            this.cmbSql.Location = new System.Drawing.Point(282, 123);
            this.cmbSql.Name = "cmbSql";
            this.cmbSql.Size = new System.Drawing.Size(119, 24);
            this.cmbSql.TabIndex = 42;
            // 
            // lblSql
            // 
            this.lblSql.Location = new System.Drawing.Point(17, 122);
            this.lblSql.Name = "lblSql";
            this.lblSql.Size = new System.Drawing.Size(253, 24);
            this.lblSql.TabIndex = 27;
            this.lblSql.Text = "OpenGD77 Squelch Level";
            this.lblSql.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbChBandwidth
            // 
            this.cmbChBandwidth.AccessibleDescription = "Bandwidth";
            this.cmbChBandwidth.AccessibleName = "Bandwidth";
            this.cmbChBandwidth.BackColor = System.Drawing.Color.White;
            this.cmbChBandwidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChBandwidth.FormattingEnabled = true;
            this.cmbChBandwidth.Location = new System.Drawing.Point(282, 24);
            this.cmbChBandwidth.Name = "cmbChBandwidth";
            this.cmbChBandwidth.Size = new System.Drawing.Size(117, 24);
            this.cmbChBandwidth.TabIndex = 40;
            // 
            // lblChBandwidth
            // 
            this.lblChBandwidth.Location = new System.Drawing.Point(15, 24);
            this.lblChBandwidth.Name = "lblChBandwidth";
            this.lblChBandwidth.Size = new System.Drawing.Size(256, 24);
            this.lblChBandwidth.TabIndex = 23;
            this.lblChBandwidth.Text = "Channel Bandwidth [KHz]";
            this.lblChBandwidth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbRxTone
            // 
            this.cmbRxTone.AccessibleDescription = "Rx CTCSS / DCS";
            this.cmbRxTone.AccessibleName = "Rx CTCSS / DCS";
            this.cmbRxTone.BackColor = System.Drawing.Color.White;
            this.cmbRxTone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRxTone.FormattingEnabled = true;
            this.cmbRxTone.Location = new System.Drawing.Point(282, 90);
            this.cmbRxTone.MaxLength = 5;
            this.cmbRxTone.Name = "cmbRxTone";
            this.cmbRxTone.Size = new System.Drawing.Size(117, 24);
            this.cmbRxTone.TabIndex = 22;
            this.cmbRxTone.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbRxTone_KeyDown);
            this.cmbRxTone.Validating += new System.ComponentModel.CancelEventHandler(this.cmbRxTone_Validating);
            // 
            // lblRxTone
            // 
            this.lblRxTone.AutoSize = true;
            this.lblRxTone.Location = new System.Drawing.Point(137, 94);
            this.lblRxTone.Name = "lblRxTone";
            this.lblRxTone.Size = new System.Drawing.Size(134, 16);
            this.lblRxTone.TabIndex = 21;
            this.lblRxTone.Text = "Rx CTCSS/DCS [Hz]";
            this.lblRxTone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbAPRS_System
            // 
            this.cmbAPRS_System.BackColor = System.Drawing.Color.White;
            this.cmbAPRS_System.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAPRS_System.FormattingEnabled = true;
            this.cmbAPRS_System.Location = new System.Drawing.Point(282, 156);
            this.cmbAPRS_System.Name = "cmbAPRS_System";
            this.cmbAPRS_System.Size = new System.Drawing.Size(119, 24);
            this.cmbAPRS_System.TabIndex = 43;
            this.cmbAPRS_System.SelectedIndexChanged += new System.EventHandler(this.cmbAPRS_SelectedIndexChanged);
            // 
            // lblAPRSSystem
            // 
            this.lblAPRSSystem.Location = new System.Drawing.Point(139, 158);
            this.lblAPRSSystem.Name = "lblAPRSSystem";
            this.lblAPRSSystem.Size = new System.Drawing.Size(129, 24);
            this.lblAPRSSystem.TabIndex = 18;
            this.lblAPRSSystem.Text = "APRS";
            this.lblAPRSSystem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbTxTone
            // 
            this.cmbTxTone.AccessibleDescription = "Tx CTXCSS / DCS";
            this.cmbTxTone.AccessibleName = "Tx CTXCSS / DCS";
            this.cmbTxTone.BackColor = System.Drawing.Color.White;
            this.cmbTxTone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTxTone.FormattingEnabled = true;
            this.cmbTxTone.Location = new System.Drawing.Point(282, 57);
            this.cmbTxTone.MaxLength = 5;
            this.cmbTxTone.Name = "cmbTxTone";
            this.cmbTxTone.Size = new System.Drawing.Size(117, 24);
            this.cmbTxTone.TabIndex = 41;
            this.cmbTxTone.SelectedIndexChanged += new System.EventHandler(this.cmbTxTone_SelectedIndexChanged);
            this.cmbTxTone.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbTxTone_KeyDown);
            this.cmbTxTone.Validating += new System.ComponentModel.CancelEventHandler(this.cmbTxTone_Validating);
            // 
            // lblTxTone
            // 
            this.lblTxTone.AutoSize = true;
            this.lblTxTone.Location = new System.Drawing.Point(137, 60);
            this.lblTxTone.Name = "lblTxTone";
            this.lblTxTone.Size = new System.Drawing.Size(133, 16);
            this.lblTxTone.TabIndex = 25;
            this.lblTxTone.Text = "Tx CTCSS/DCS [Hz]";
            this.lblTxTone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkOpenGD77ScanAllSkip
            // 
            this.chkOpenGD77ScanAllSkip.AccessibleDescription = "Scan. All skip";
            this.chkOpenGD77ScanAllSkip.AccessibleName = "Scan. All skip";
            this.chkOpenGD77ScanAllSkip.AutoSize = true;
            this.chkOpenGD77ScanAllSkip.Location = new System.Drawing.Point(888, 49);
            this.chkOpenGD77ScanAllSkip.Name = "chkOpenGD77ScanAllSkip";
            this.chkOpenGD77ScanAllSkip.Size = new System.Drawing.Size(109, 20);
            this.chkOpenGD77ScanAllSkip.TabIndex = 31;
            this.chkOpenGD77ScanAllSkip.Text = "Scan: All skip";
            this.chkOpenGD77ScanAllSkip.UseVisualStyleBackColor = true;
            // 
            // chkVox
            // 
            this.chkVox.AccessibleDescription = "VOX";
            this.chkVox.AccessibleName = "VOX";
            this.chkVox.Location = new System.Drawing.Point(888, 68);
            this.chkVox.Name = "chkVox";
            this.chkVox.Size = new System.Drawing.Size(236, 20);
            this.chkVox.TabIndex = 32;
            this.chkVox.Text = "Vox";
            this.chkVox.UseVisualStyleBackColor = true;
            // 
            // chkOpenGD77ScanZoneSkip
            // 
            this.chkOpenGD77ScanZoneSkip.AccessibleDescription = "Scan. Zone skip";
            this.chkOpenGD77ScanZoneSkip.AccessibleName = "Scan. Zone skip";
            this.chkOpenGD77ScanZoneSkip.AutoSize = true;
            this.chkOpenGD77ScanZoneSkip.Location = new System.Drawing.Point(888, 30);
            this.chkOpenGD77ScanZoneSkip.Name = "chkOpenGD77ScanZoneSkip";
            this.chkOpenGD77ScanZoneSkip.Size = new System.Drawing.Size(127, 20);
            this.chkOpenGD77ScanZoneSkip.TabIndex = 30;
            this.chkOpenGD77ScanZoneSkip.Text = "Scan: Zone skip";
            this.chkOpenGD77ScanZoneSkip.UseVisualStyleBackColor = true;
            // 
            // chkNoBeep
            // 
            this.chkNoBeep.AccessibleDescription = "No Beep";
            this.chkNoBeep.AccessibleName = "No Beep";
            this.chkNoBeep.Location = new System.Drawing.Point(888, 106);
            this.chkNoBeep.Name = "chkNoBeep";
            this.chkNoBeep.Size = new System.Drawing.Size(236, 20);
            this.chkNoBeep.TabIndex = 34;
            this.chkNoBeep.Text = "No Beep";
            this.chkNoBeep.UseVisualStyleBackColor = true;
            this.chkNoBeep.CheckedChanged += new System.EventHandler(this.chkNoBeep_CheckedChanged);
            // 
            // chkNoEco
            // 
            this.chkNoEco.AccessibleDescription = "No Economy";
            this.chkNoEco.AccessibleName = "No Economy";
            this.chkNoEco.Location = new System.Drawing.Point(888, 125);
            this.chkNoEco.Name = "chkNoEco";
            this.chkNoEco.Size = new System.Drawing.Size(236, 20);
            this.chkNoEco.TabIndex = 35;
            this.chkNoEco.Text = "No Eco";
            this.chkNoEco.UseVisualStyleBackColor = true;
            this.chkNoEco.CheckedChanged += new System.EventHandler(this.chkNoEco_CheckedChanged);
            // 
            // cmbChMode
            // 
            this.cmbChMode.AccessibleDescription = "Mode Analogue or digital";
            this.cmbChMode.AccessibleName = "Mode";
            this.cmbChMode.BackColor = System.Drawing.Color.White;
            this.cmbChMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChMode.FormattingEnabled = true;
            this.cmbChMode.Location = new System.Drawing.Point(81, 26);
            this.cmbChMode.Name = "cmbChMode";
            this.cmbChMode.Size = new System.Drawing.Size(119, 24);
            this.cmbChMode.TabIndex = 1;
            this.cmbChMode.SelectedIndexChanged += new System.EventHandler(this.cmbChMode_SelectedIndexChanged);
            // 
            // lblChName
            // 
            this.lblChName.Location = new System.Drawing.Point(9, 56);
            this.lblChName.Name = "lblChName";
            this.lblChName.Size = new System.Drawing.Size(64, 24);
            this.lblChName.TabIndex = 2;
            this.lblChName.Text = "Name";
            this.lblChName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTxFreq
            // 
            this.txtTxFreq.AccessibleDescription = "Tx Frequency";
            this.txtTxFreq.AccessibleName = "Tx Frequency";
            this.txtTxFreq.Location = new System.Drawing.Point(748, 26);
            this.txtTxFreq.Name = "txtTxFreq";
            this.txtTxFreq.Size = new System.Drawing.Size(119, 23);
            this.txtTxFreq.TabIndex = 4;
            this.txtTxFreq.Validating += new System.ComponentModel.CancelEventHandler(this.txtTxFreq_Validating);
            // 
            // lblChMode
            // 
            this.lblChMode.Location = new System.Drawing.Point(9, 26);
            this.lblChMode.Name = "lblChMode";
            this.lblChMode.Size = new System.Drawing.Size(63, 24);
            this.lblChMode.TabIndex = 0;
            this.lblChMode.Text = "Mode";
            this.lblChMode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTot
            // 
            this.lblTot.Location = new System.Drawing.Point(544, 61);
            this.lblTot.Name = "lblTot";
            this.lblTot.Size = new System.Drawing.Size(198, 16);
            this.lblTot.TabIndex = 11;
            this.lblTot.Text = "TOT [s]";
            this.lblTot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtRxFreq
            // 
            this.txtRxFreq.AccessibleDescription = "Rx Frequency";
            this.txtRxFreq.AccessibleName = "Rx Frequency";
            this.txtRxFreq.BackColor = System.Drawing.SystemColors.Window;
            this.txtRxFreq.Location = new System.Drawing.Point(368, 26);
            this.txtRxFreq.Name = "txtRxFreq";
            this.txtRxFreq.Size = new System.Drawing.Size(119, 23);
            this.txtRxFreq.TabIndex = 2;
            this.txtRxFreq.Validating += new System.ComponentModel.CancelEventHandler(this.txtRxFreq_Validating);
            // 
            // lblLibreDMR_Power
            // 
            this.lblLibreDMR_Power.AccessibleDescription = "Per channel power";
            this.lblLibreDMR_Power.AccessibleName = "Per channel power";
            this.lblLibreDMR_Power.Location = new System.Drawing.Point(218, 57);
            this.lblLibreDMR_Power.Name = "lblLibreDMR_Power";
            this.lblLibreDMR_Power.Size = new System.Drawing.Size(140, 24);
            this.lblLibreDMR_Power.TabIndex = 6;
            this.lblLibreDMR_Power.Text = "Per channel Power";
            this.lblLibreDMR_Power.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblRxFreq
            // 
            this.lblRxFreq.Location = new System.Drawing.Point(224, 26);
            this.lblRxFreq.Name = "lblRxFreq";
            this.lblRxFreq.Size = new System.Drawing.Size(134, 24);
            this.lblRxFreq.TabIndex = 4;
            this.lblRxFreq.Text = "Rx Frequency [MHz]";
            this.lblRxFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTxFreq
            // 
            this.lblTxFreq.Location = new System.Drawing.Point(609, 26);
            this.lblTxFreq.Name = "lblTxFreq";
            this.lblTxFreq.Size = new System.Drawing.Size(133, 24);
            this.lblTxFreq.TabIndex = 9;
            this.lblTxFreq.Text = "Tx Frequency [MHz]";
            this.lblTxFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudTot
            // 
            this.nudTot.AccessibleDescription = "Timeout";
            this.nudTot.AccessibleName = "Timeout";
            this.nudTot.Location = new System.Drawing.Point(748, 57);
            this.nudTot.Name = "nudTot";
            this.nudTot.Size = new System.Drawing.Size(120, 23);
            this.nudTot.TabIndex = 12;
            this.nudTot.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudTot.ValueChanged += new System.EventHandler(this.nudTot_ValueChanged);
            // 
            // ChannelForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1209, 420);
            this.Controls.Add(this.pnlChannel);
            this.Controls.Add(this.tsrCh);
            this.Controls.Add(this.mnsCh);
            this.Font = new System.Drawing.Font("Arial", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.mnsCh;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChannelForm";
            this.Text = "Channel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChannelForm_FormClosing);
            this.Load += new System.EventHandler(this.ChannelForm_Load);
            this.Shown += new System.EventHandler(this.ChannelForm_Shown);
            this.tsrCh.ResumeLayout(false);
            this.tsrCh.PerformLayout();
            this.mnsCh.ResumeLayout(false);
            this.mnsCh.PerformLayout();
            this.pnlChannel.ResumeLayout(false);
            this.pnlChannel.PerformLayout();
            this.grpLocation.ResumeLayout(false);
            this.grpLocation.PerformLayout();
            this.grpDigit.ResumeLayout(false);
            this.grpDigit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTxColor)).EndInit();
            this.grpAnalog.ResumeLayout(false);
            this.grpAnalog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTot)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    static ChannelForm()
    {
        SPACE_CH = Marshal.SizeOf(typeof(ChannelOne));
        SPACE_CH_GROUP = 16 + SPACE_CH * 128;
        SZ_CH_MODE = new string[2] { "Analog", "Digital" };
        SZ_LIBREDMR_POWER = new string[11]
        {
            "Master", "50mW | 50mW | 100mW ", "250mW | 250mW |250mW", "500mW |500mW | 500mW", "750mW | 750mW |750mW", "1W | 1W | 1W", "2W | 2W | 2W", "3W | 3W | 10W", "4W | 5W | 25W", "5W | 10W | 40W",
            "+W-"
        };
        SZ_REF_FREQ = new string[3] { "Low", "Middle", "High" };
        SZ_POWER = new string[2] { "Low", "High" };
        SZ_ADMIT_CRITERICA = new string[3] { "Always", "Channel Free", "CTCSS/DCS" };
        SZ_ADMIT_CRITERICA_D = new string[3] { "Always", "Channel Free", "Color Code" };
        SZ_BANDWIDTH = new string[2] { "12.5", "25" };
        SZ_SQUELCH = new string[2] { "Tight", "Normal" };
        SZ_SQUELCH_LEVEL = new string[22]
        {
            "По рации", "Открытый", "5%", "10%", "15%", "20%", "25%", "30%", "35%", "40%",
            "45%", "50%", "55%", "60%", "65%", "70%", "75%", "80%", "85%", "90%",
            "95%", "Закрытый"
        };
        SZ_VOICE_EMPHASIS = new string[4] { "None", "De & Pre", "De Only", "Pre Only" };
        SZ_STE = new string[4] { "Frequency", "120°", "180°", "240°" };
        SZ_NON_STE = new string[2] { "Off", "Frequency" };
        SZ_SIGNALING_SYSTEM = new string[2] { "Off", "DTMF" };
        SZ_UNMUTE_RULE = new string[3] { "Std Unmute, Mute", "And Unmute, Mute", "And Unmute, Or Mute" };
        SZ_PTTID_TYPE = new string[4] { "None", "Only Front", "Only Post", "Front & Post" };
        SZ_TA_TX = new string[4] { "Выкл.", "APRS", "Текст", "APRS и текст" };
        SZ_TIMING_PREFERENCE = new string[3] { "Preferred", "Eligibel", "Ineligibel" };
        SZ_REPEATER_SLOT = new string[2] { "1", "2" };
        SZ_ARS = new string[2] { "Disable", "On System Change" };
        SZ_KEY_SWITCH = new string[2] { "Off", "On" };
        data = new Channel();
    }


}