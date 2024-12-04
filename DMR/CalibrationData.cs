using System.Runtime.InteropServices;

namespace DMR;

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
