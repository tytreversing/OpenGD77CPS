using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace DMR;
[Serializable]
[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 4)]

public struct BCDData
{
    [XmlAttribute(AttributeName = "b1")]
    [FieldOffset(3)]
    [MarshalAs(UnmanagedType.U1)]
    public byte byte1;
    [XmlAttribute(AttributeName = "b2")]
    [FieldOffset(2)]
    [MarshalAs(UnmanagedType.U1)]
    public byte byte2;
    [XmlAttribute(AttributeName = "b3")]
    [FieldOffset(1)]
    [MarshalAs(UnmanagedType.U1)]
    public byte byte3;
    [XmlAttribute(AttributeName = "b4")]
    [FieldOffset(0)]
    [MarshalAs(UnmanagedType.U1)]
    public byte byte4;

}

[Serializable]
public struct BYTE
{
    [XmlAttribute(AttributeName = "value")]
    public byte VALUE;

    public static explicit operator decimal(BYTE v)
    {
        return (decimal)v.VALUE;
    }

    public static implicit operator BYTE(int x)
    {
        return new BYTE { VALUE = (byte)x };
    }

    public static implicit operator BYTE(decimal x)
    {
        return new BYTE { VALUE = (byte)x };
    }
}

[Serializable]
[XmlRootAttribute("TransceiverCalibrations", Namespace = "https://opengd77rus.ru")]
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x200)]
public class CalibrationDataSTM32
{
    //0x00

    [MarshalAs(UnmanagedType.U1)]
    public byte VoxLevel1;              //calibration for Vox Setting 1

    [MarshalAs(UnmanagedType.U1)]
    public byte VoxLevel10;             //calibration for Vox Setting 10

    [MarshalAs(UnmanagedType.U1)]
    public byte RxLowVoltage;           //Exact use unknown

    [MarshalAs(UnmanagedType.U1)]
    public byte RxHighVoltage;          //Exact use unknown

    [MarshalAs(UnmanagedType.U1)]
    public byte RSSI120;                //RSSI Calibration for -120dBm

    [MarshalAs(UnmanagedType.U1)]
    public byte RSSI70;                 //RSSI Calibration for -70dBm
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    private byte[] UnknownBlock1;       //Unknown

    [MarshalAs(UnmanagedType.U1)]//0x08
    private byte Unknown1;               //Unknown

    [MarshalAs(UnmanagedType.U1)]
    public byte UHFOscRefTune;          //UHF reference tuning

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    private byte[] UnknownBlock2;       //Unknown

    [MarshalAs(UnmanagedType.U1)]
    public byte VHFOscRefTune;          //UHF reference tuning

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    private byte[] UnknownBlock3;       //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
    public BYTE[] UHFHighPowerCal; //UHF High Power Calibration 9 frequencies
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]                            //0x19
    public BYTE[] VHFHighPowerCal; //VHF High Power Calibration 5 frequencies
                                   //0x1E
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    private byte[] UnknownBlock4;       //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                //0x20
    public BYTE[] UHFLowPowerCal;      //UHF Low Power Calibration 9 frequencies
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]                                 //0x29
    public BYTE[] VHFLowPowerCal;      //VHF Low Power Calibration 5 frequencies

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]//0x2E
    private byte[] UnknownBlock5;       //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                  //0x30
    public BYTE[] UHFRxTuning;     //UHF Rx Front End Tuning 9 frequencies
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]                              //0x39
    public BYTE[] VHFRxTuning;     //VHF Rx Front End Tuning 5 frequencies
                                   //0x3E
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    private byte[] UnknownBlock6;       //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                  //0x40
    public BYTE[] UHFOpenSquelch9;   //UHF Squelch Level 9 Opening  9 frequencies

    [MarshalAs(UnmanagedType.U4)]               //0x49
    public UInt32 VHFLowFrequency;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]                                
    private byte[] UnknownBlock7;       //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                  //0x50
    public BYTE[] UHFCloseSquelch9;   //UHF Squelch Level 9 Closing 9 frequencies

    [MarshalAs(UnmanagedType.U4)]               //0x59
    public UInt32 UHFLowFrequency;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]                                 
    private byte[] UnknownBlock8;       //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                  //0x60
    public BYTE[] UHFOpenSquelch1;   //UHF Squelch Level 1 Opening  9 frequencies

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]                                //0x69
    private byte[] UnknownBlock9;       //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                  //0x70
    public BYTE[] UHFCloseSquelch1;   //UHF Squelch Level 1 Closing 9 frequencies

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]                                 //0x79
    private byte[] UnknownBlock10;      //Unknown

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]                                  //0x80
    private byte[] UnknownBlock11;     //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                  //0x90
    public BYTE[] UHFCTC67;            //UHF CTCSS Deviation for 67Hz Tone 9 frequencies

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]                                  //0x99
    private byte[] UnknownBlock12;      //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]                                  //0x9B
    public BYTE[] VHFCTC67;            //VHF CTCSS Deviation for 67Hz Tone 5 frequencies
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                  //0xA0
    public BYTE[] UHFCTC151;           //UHF CTCSS Deviation for 151.4Hz Tone 9 frequencies

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]                                  //0xA9
    private byte[] UnknownBlock13;      //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]                                  //0x9B
    public BYTE[] VHFCTC151;           //VHF CTCSS Deviation for 151.4Hz Tone 5 frequencies
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                  //0xB0
    public BYTE[] UHFCTC254;           //UHF CTCSS Deviation for 254.1Hz Tone 9 frequencies

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]                                  //0xB9
    private byte[] UnknownBlock14;      //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]                                  //0xBB
    public BYTE[] VHFCTC254;           //VHF CTCSS Deviation for 254.1Hz Tone 5 frequencies

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]                                  //0xC0
    private byte[] UnknownBlock15;     //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                  //0xD0
    public BYTE[] UHFDCS;              //UHF DCS Deviation 9 frequencies

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]                                  //0xD9
    private byte[] UnknownBlock16;      //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]                                  //0xDB
    public BYTE[] VHFDCS;              //VHF DCS Deviation 5 frequencies
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]                                  //0xE0
    public BYTE[] VHFOpenSquelch9;     //VHF Squelch Level 9 Opening  5 frequencies
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public BYTE[] VHFCloseSquelch9;     //VHF Squelch Level 9 Closing  5 frequencies
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public BYTE[] VHFOpenSquelch1;     //VHF Squelch Level 1 Opening  5 frequencies
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public BYTE[] VHFCloseSquelch1;     //VHF Squelch Level 1 Closing  5 frequencies

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]                                   //0xF4
    private byte[] UnknownBlock17;     //Unknown

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]                                  //0x100
    public BCDData[] VHFCalFreqs;        // VHF Calibration Frequencies 4 BCD bytes per freq, 5 pairs of freqs Rx and Tx

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]  //0x128
    private byte[] UnknownBlock18;      //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                  //0x130
    private BYTE[] UHFDMRIGain;         //UHF I Gain for DMR	9 Frequencies
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]                                  //0x139
    private BYTE[] VHFDMRIGain;         //VHF I Gain for DMR	5 Frequencies

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    private byte[] UnknownBlock19;      //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                  //0x140
    public BYTE[] UHFDMRQGain;         //UHF Q Gain for DMR	9 Frequencies
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]                                  //0x149
    public BYTE[] VHFDMRQGain;         //VHF Q Gain for DMR	5 Frequencies

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    private byte[] UnknownBlock20;      //Unknown

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]                                  //0x150
    private byte[] UnknownBlock21;     //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                  //0x170
    public BYTE[] UHFFMIGain;          //UHF I Gain for FM	9 Frequencies
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]                                  //0x179
    public BYTE[] VHFFMIGain;          //VHF I Gain for FM	5 Frequencies

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    private byte[] UnknownBlock22;      //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                  
    public BYTE[] UHFFMQGain;          //UHF Q Gain for FM	9 Frequencies
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]                                  
    public BYTE[] VHFFMQGain;          //VHF Q Gain for FM	5 Frequencies

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    private byte[] UnknownBlock23;      //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                  
    public BYTE[] UHFMidPowerCal;      //UHF Mid Power Calibration 9 frequencies
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]                                  
    public BYTE[] VHFMidPowerCal;      //VHF Mid Power Calibration 5 frequencies

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]                                  
    private byte[] UnknownBlock24;      //Unknown
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]                                  
    public BYTE[] UHFMidLowPowerCal;       //UHF MidLow Power Calibration 9 frequencies
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]                                      
    public BYTE[] VHFMidLowPowerCal;       //VHF MidLow Power Calibration 5 frequencies
 
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]                                      
    private byte[] UnknownBlock25;      //Unknown

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]                                  
    public BCDData[] UHFCalFreqs;        // UHF Calibration Frequencies 4 BCD bytes per freq, 9 pairs of freqs Rx and Tx

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    private byte[] UnknownBlock26;      

    public CalibrationDataSTM32()
    {
        VoxLevel1 = 0;             
        VoxLevel10 = 0; 
        RxLowVoltage = 0;
        RxHighVoltage = 0;          
        RSSI120 = 0;   
        RSSI70 = 0; 
        UnknownBlock1 = new byte[2]; 
        Unknown1 = 0;              
        UHFOscRefTune = 0;   
        UnknownBlock2 = new byte[2]; 
        VHFOscRefTune = 0;
        UnknownBlock3 = new byte[3];  
        UHFHighPowerCal = new BYTE[9]; 
        VHFHighPowerCal = new BYTE[5]; 
        UnknownBlock4 = new byte[2];
        UHFLowPowerCal = new BYTE[9]; 
        VHFLowPowerCal = new BYTE[5];      
        UnknownBlock5 = new byte[2];      
        UHFRxTuning = new BYTE[9];     
        VHFRxTuning = new BYTE[5];     
        UnknownBlock6 = new byte[2];      
        UHFOpenSquelch9 = new BYTE[9];
        VHFLowFrequency = 0;
        UnknownBlock7 = new byte[3]; 
        UHFCloseSquelch9 = new BYTE[9];
        UHFLowFrequency = 0;
        UnknownBlock8 = new byte[3];      
        UHFOpenSquelch1 = new BYTE[9];  
        UnknownBlock9 = new byte[7];       
        UHFCloseSquelch1 = new BYTE[9];   
        UnknownBlock10 = new byte[7];      
        UnknownBlock11 = new byte[16];    
        UHFCTC67 = new BYTE[9];            
        UnknownBlock12 = new byte[2];      
        VHFCTC67 = new BYTE[5];            
        UHFCTC151 = new BYTE[9];         
        UnknownBlock13 = new byte[2];      
        VHFCTC151 = new BYTE[5];           
        UHFCTC254 = new BYTE[9];         
        UnknownBlock14 = new byte[2];    
        VHFCTC254 = new BYTE[5];           
        UnknownBlock15 = new byte[16];     
        UHFDCS = new BYTE[9];              
        UnknownBlock16 = new byte[2];      
        VHFDCS = new BYTE[5];              
        VHFOpenSquelch9 = new BYTE[5];    
        VHFCloseSquelch9 = new BYTE[5];    
        VHFOpenSquelch1 = new BYTE[5];    
        VHFCloseSquelch1 = new BYTE[5];   
        UnknownBlock17 = new byte[12];     
        VHFCalFreqs = new BCDData[10];
        for (int i = 0; i < 10; i++)
        {
            VHFCalFreqs[i].byte1 = 0;
            VHFCalFreqs[i].byte2 = 0;
            VHFCalFreqs[i].byte3 = 0;
            VHFCalFreqs[i].byte4 = 0;
        }
        UnknownBlock18 = new byte[8];   
        UHFDMRIGain = new BYTE[9];        
        VHFDMRIGain = new BYTE[5];        
        UnknownBlock19 = new byte[2];      
        UHFDMRQGain = new BYTE[9];      
        VHFDMRQGain = new BYTE[5];        
        UnknownBlock20 = new byte[2];   
        UnknownBlock21 = new byte[32];    
        UHFFMIGain = new BYTE[9];          
        VHFFMIGain = new BYTE[5];          
        UnknownBlock22 = new byte[2];      
        UHFFMQGain = new BYTE[9];  
        VHFFMQGain = new BYTE[5];          
        UnknownBlock23 = new byte[2];   
        UHFMidPowerCal = new BYTE[9];    
        VHFMidPowerCal = new BYTE[5];     
        UnknownBlock24 = new byte[2];     
        UHFMidLowPowerCal = new BYTE[9];       
        VHFMidLowPowerCal = new BYTE[5];       
        UnknownBlock25 = new byte[2];      
        UHFCalFreqs = new BCDData[18];
        for (int i = 0; i < 18; i++)
        {
            UHFCalFreqs[i].byte1 = 0;
            UHFCalFreqs[i].byte2 = 0;
            UHFCalFreqs[i].byte3 = 0;
            UHFCalFreqs[i].byte4 = 0;
        }
        UnknownBlock26 = new byte[8];
    }
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 36)]
public class RadioBandlimits
{
    [FieldOffset(0)]
    [MarshalAs(UnmanagedType.U4)]
    public uint VHFLowCal;

    [FieldOffset(4)]
    [MarshalAs(UnmanagedType.U4)]
    public uint VHFLow;

    [FieldOffset(8)]
    [MarshalAs(UnmanagedType.U4)]
    public uint VHFHigh;

    [FieldOffset(12)]
    [MarshalAs(UnmanagedType.U4)]
    public uint f220LowCal;

    [FieldOffset(16)]
    [MarshalAs(UnmanagedType.U4)]
    public uint f220Low;

    [FieldOffset(20)]
    [MarshalAs(UnmanagedType.U4)]
    public uint f220High;

    [FieldOffset(24)]
    [MarshalAs(UnmanagedType.U4)]
    public uint UHFLowCal;

    [FieldOffset(28)]
    [MarshalAs(UnmanagedType.U4)]
    public uint UHFLow;

    [FieldOffset(32)]
    [MarshalAs(UnmanagedType.U4)]
    public uint UHFHigh;

    public RadioBandlimits()
    {
        VHFLowCal = 13600000;
        VHFLow = 12700000;
        VHFHigh = 17400000;
        f220LowCal = 20000000;
        f220Low = 20000000;
        f220High = 26000000;
        UHFLowCal = 40000000;
        UHFLow = 38000000;
        UHFHigh = 56400000;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class CalibrationData
{
	public ushort DigitalRxGainNarrowband_NOTCONFIRMED;

	public ushort DigitalTxGainNarrowband_NOTCONFIRMED;

	public ushort DigitalRxGainWideband_NOTCONFIRMED;

	public ushort DigitalTxGainWideband_NOTCONFIRMED;

	public short DACOscRefTune;

	public sbyte Q_MOD2_OFFSET;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public PowerSettingData[] PowerSettings;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	public byte[] UnknownBlock3;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	public byte[] UknownBlock9;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
	public byte[] UnknownBlock4;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	public byte[] UnknownBlock5;

	public byte MuteStrictWidebandClose1;

	public byte MuteStrictWidebandOpen1;

	public byte MuteStrictWidebandClose2;

	public byte MuteStrictWidebandOpen2;

	public byte MuteNormalWidebandClose1;

	public byte MuteNormalWidebandOpen1;

	public byte MuteStrictNarrowbandClose1;

	public byte MuteStrictNarrowbandOpen1;

	public byte MuteStrictNarrowbandClose2;

	public byte MuteStrictNarrowbandOpen2;

	public byte MuteNormalNarrowbandClose1;

	public byte MuteNormalNarrowbandOpen1;

	public byte RSSILowerThreshold;

	public byte RSSIUpperThreshold;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	public byte[] Dmr4FskDeviation;

	public byte DigitalRxAudioGainAndBeepVolume;

	public byte AnalogTxDeviationDTMF;

	public byte AnalogTxDeviation1750Toneburst;

	public byte AnalogTxDeviationCTCSSWideband;

	public byte AnalogTxDeviationCTCSSNarrowband;

	public byte AnalogTxDeviationDCSWideband;

	public byte AnalogTxDeviationDCSNarrowband;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	public byte[] UnknownBlock8;

	public byte AnalogMicGain;

	public byte ReceiveAGCGainTarget;

	public ushort AnalogTxOverallDeviationWideband;

	public ushort AnalogTxOverallDeviationNarrband;

	public byte AnalogRxAudioGainWideband;

	public byte AnalogRxAudioGainNarrowband;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	public byte[] UnknownBlock7;

	public CalibrationData()
	{
		PowerSettings = new PowerSettingData[16];
		for (int i = 0; i < 16; i++)
		{
			PowerSettings[i] = default(PowerSettingData);
		}
		UnknownBlock3 = new byte[8];
		UknownBlock9 = new byte[8];
		UnknownBlock4 = new byte[4];
		UnknownBlock5 = new byte[8];
		Dmr4FskDeviation = new byte[8];
		UnknownBlock7 = new byte[2];
		UnknownBlock8 = new byte[2];
	}
}
[Serializable]
[XmlRootAttribute("TransceiverCalibrations", Namespace = "https://opengd77rus.ru")]
public class CalibrationStruct
{
    //0x00
    public uint VoxLevel1;              //calibration for Vox Setting 1
    public uint VoxLevel10;             //calibration for Vox Setting 10
    public uint RxLowVoltage;           //Exact use unknown
    public uint RxHighVoltage;          //Exact use unknown
    public uint RSSI120;                //RSSI Calibration for -120dBm
    public uint RSSI70;                 //RSSI Calibration for -70dBm
    public uint UHFOscRefTune;          //UHF reference tuning
    public uint VHFOscRefTune;          //UHF reference tuning
    public uint[] UHFHighPowerCal;
    public uint[] VHFHighPowerCal;
    public uint[] UHFLowPowerCal;
    public uint[] VHFLowPowerCal;
    public uint[] UHFRxTuning;     //UHF Rx Front End Tuning 9 frequencies
    public uint[] VHFRxTuning;     //VHF Rx Front End Tuning 5 frequencies
    public uint[] UHFOpenSquelch9;   //UHF Squelch Level 9 Opening  9 frequencies
    public uint[] UHFCloseSquelch9;   //UHF Squelch Level 9 Closing 9 frequencies
    public uint[] UHFOpenSquelch1;   //UHF Squelch Level 1 Opening  9 frequencies
    public uint[] UHFCloseSquelch1;   //UHF Squelch Level 1 Closing 9 frequencies
    public uint[] UHFCTC67;            //UHF CTCSS Deviation for 67Hz Tone 9 frequencies               
    public uint[] VHFCTC67;            //VHF CTCSS Deviation for 67Hz Tone 5 frequencies
    public uint[] UHFCTC151;           //UHF CTCSS Deviation for 151.4Hz Tone 9 frequencies
    public uint[] VHFCTC151;           //VHF CTCSS Deviation for 151.4Hz Tone 5 frequencies
    public uint[] UHFCTC254;           //UHF CTCSS Deviation for 254.1Hz Tone 9 frequencies
    public uint[] VHFCTC254;           //VHF CTCSS Deviation for 254.1Hz Tone 5 frequencies
    public uint[] UHFDCS;              //UHF DCS Deviation 9 frequencies
    public uint[] VHFDCS;              //VHF DCS Deviation 5 frequencies
    public uint[] VHFOpenSquelch9;     //VHF Squelch Level 9 Opening  5 frequencies
    public uint[] VHFCloseSquelch9;     //VHF Squelch Level 9 Closing  5 frequencies
    public uint[] VHFOpenSquelch1;     //VHF Squelch Level 1 Opening  5 frequencies
    public uint[] VHFCloseSquelch1;     //VHF Squelch Level 1 Closing  5 frequencies
    public uint[] VHFCalFreqs;		// VHF Calibration Frequencies 4 BCD bytes per freq, 5 pairs of freqs Rx and Tx
    public uint[] UHFDMRIGain;         //UHF I Gain for DMR	9 Frequencies
    public uint[] VHFDMRIGain;         //VHF I Gain for DMR	5 Frequencies
    public uint[] UHFDMRQGain;         //UHF Q Gain for DMR	9 Frequencies
    public uint[] VHFDMRQGain;         //VHF Q Gain for DMR	5 Frequencies
    public uint[] UHFFMIGain;          //UHF I Gain for FM	9 Frequencies
    public uint[] VHFFMIGain;          //VHF I Gain for FM	5 Frequencies
    public uint[] UHFFMQGain;          //UHF Q Gain for FM	9 Frequencies
    public uint[] VHFFMQGain;          //VHF Q Gain for FM	5 Frequencies
    public uint[] UHFMidPowerCal;      //UHF Mid Power Calibration 9 frequencies
    public uint[] VHFMidPowerCal;      //VHF Mid Power Calibration 5 frequencies
    public uint[] UHFMidLowPowerCal;       //UHF MidLow Power Calibration 9 frequencies
    public uint[] VHFMidLowPowerCal;       //VHF MidLow Power Calibration 5 frequencies
    public uint[] UHFCalFreqs;		// UHF Calibration Frequencies 4 BCD bytes per freq, 9 pairs of freqs Rx and Tx



    public CalibrationStruct()
    {
        UHFHighPowerCal = new uint[9];
        VHFHighPowerCal = new uint[5];
        UHFLowPowerCal = new uint[9];
        VHFLowPowerCal = new uint[5];
        UHFRxTuning = new uint[9];
        VHFRxTuning = new uint[5];     //VHF Rx Front End Tuning 5 frequencies
        UHFOpenSquelch9 = new uint[9];   //UHF Squelch Level 9 Opening  9 frequencies
        UHFCloseSquelch9 = new uint[9];   //UHF Squelch Level 9 Closing 9 frequencies
        UHFOpenSquelch1 = new uint[9];   //UHF Squelch Level 1 Opening  9 frequencies
        UHFCloseSquelch1 = new uint[9];   //UHF Squelch Level 1 Closing 9 frequencies
        UHFCTC67 = new uint[9];            //UHF CTCSS Deviation for 67Hz Tone 9 frequencies               
        VHFCTC67 = new uint[5];            //VHF CTCSS Deviation for 67Hz Tone 5 frequencies
        UHFCTC151 = new uint[9];           //UHF CTCSS Deviation for 151.4Hz Tone 9 frequencies
        VHFCTC151 = new uint[5];           //VHF CTCSS Deviation for 151.4Hz Tone 5 frequencies
        UHFCTC254 = new uint[9];           //UHF CTCSS Deviation for 254.1Hz Tone 9 frequencies
        VHFCTC254 = new uint[5];           //VHF CTCSS Deviation for 254.1Hz Tone 5 frequencies
        UHFDCS = new uint[9];              //UHF DCS Deviation 9 frequencies
        VHFDCS = new uint[5];              //VHF DCS Deviation 5 frequencies
        VHFOpenSquelch9 = new uint[5];     //VHF Squelch Level 9 Opening  5 frequencies
        VHFCloseSquelch9 = new uint[9];     //VHF Squelch Level 9 Closing  5 frequencies
        VHFOpenSquelch1 = new uint[5];     //VHF Squelch Level 1 Opening  5 frequencies
        VHFCloseSquelch1 = new uint[9];     //VHF Squelch Level 1 Closing  5 frequencies
        VHFCalFreqs = new uint[10];      // VHF Calibration Frequencies 4 BCD bytes per freq, 5 pairs of freqs Rx and Tx
        UHFDMRIGain = new uint[9];         //UHF I Gain for DMR	9 Frequencies
        VHFDMRIGain = new uint[5];         //VHF I Gain for DMR	5 Frequencies
        UHFDMRQGain = new uint[9];         //UHF Q Gain for DMR	9 Frequencies
        VHFDMRQGain = new uint[5];         //VHF Q Gain for DMR	5 Frequencies
        UHFFMIGain = new uint[9];          //UHF I Gain for FM	9 Frequencies
        VHFFMIGain = new uint[5];          //VHF I Gain for FM	5 Frequencies
        UHFFMQGain = new uint[9];          //UHF Q Gain for FM	9 Frequencies
        VHFFMQGain = new uint[5];          //VHF Q Gain for FM	5 Frequencies
        UHFMidPowerCal = new uint[9];      //UHF Mid Power Calibration 9 frequencies
        VHFMidPowerCal = new uint[5];      //VHF Mid Power Calibration 5 frequencies
        UHFMidLowPowerCal = new uint[9];       //UHF MidLow Power Calibration 9 frequencies
        VHFMidLowPowerCal = new uint[5];       //VHF MidLow Power Calibration 5 frequencies
        UHFCalFreqs = new uint[18];

    }
}