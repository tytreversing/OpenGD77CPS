using System.Globalization;

namespace DMR;

public class KepAndSatData
{
	public string CatalogueNumber;

	public string DisplayName;

	public uint Rx1;

	public uint Tx1;

	public ushort TxCTCSS1;

	public ushort ArmCTCSS1;

	public uint Rx2;

	public uint Tx2;

	public uint Rx3;

	public uint Tx3;

	public string Reserved;

	public KepAndSatData(string s)
	{
		NumberFormatInfo numberFormat = CultureInfo.GetCultureInfo("en-US").NumberFormat;
		string[] array = s.Split(',');
		CatalogueNumber = array[0].Trim();
		DisplayName = array[1].Trim();
		Rx1 = (uint)(float.Parse(array[2].Trim(), numberFormat) * 1000000f);
		Tx1 = (uint)(float.Parse(array[3].Trim(), numberFormat) * 1000000f);
		TxCTCSS1 = (ushort)(float.Parse(array[4].Trim(), numberFormat) * 10f);
		ArmCTCSS1 = (ushort)(float.Parse(array[5].Trim(), numberFormat) * 10f);
		Rx2 = (uint)(float.Parse(array[6].Trim(), numberFormat) * 1000000f);
		Tx2 = (uint)(float.Parse(array[7].Trim(), numberFormat) * 1000000f);
		Rx3 = (uint)(float.Parse(array[8].Trim(), numberFormat) * 1000000f);
		Tx3 = (uint)(float.Parse(array[9].Trim(), numberFormat) * 1000000f);
		Reserved = array[10].Trim();
	}

	public static ushort ReverseBytes(ushort value)
	{
		return (ushort)(((value & 0xFF) << 8) | ((value & 0xFF00) >>> 8));
	}

	public static uint ReverseBytes(uint value)
	{
		return ((value & 0xFF) << 24) | ((value & 0xFF00) << 8) | ((value & 0xFF0000) >> 8) | ((value & 0xFF000000u) >> 24);
	}
}
