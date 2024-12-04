using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DMR;

public class DMRDataItem : IEquatable<DMRDataItem>, IComparable<DMRDataItem>
{
	private static char[] DECOMPRESS_LUT = new char[64]
	{
		' ', '0', '1', '2', '3', '4', '5', '6', '7', '8',
		'9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I',
		'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S',
		'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c',
		'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
		'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w',
		'x', 'y', 'z', '.'
	};

	private static int[] COMPRESS_LUT = new int[256]
	{
		63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 0, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63, 63, 63, 1, 2,
		3, 4, 5, 6, 7, 8, 9, 10, 63, 63,
		63, 63, 63, 63, 63, 11, 12, 13, 14, 15,
		16, 17, 18, 19, 20, 21, 22, 23, 24, 25,
		26, 27, 28, 29, 30, 31, 32, 33, 34, 35,
		36, 63, 63, 63, 63, 63, 63, 37, 38, 39,
		40, 41, 42, 43, 44, 45, 46, 47, 48, 49,
		50, 51, 52, 53, 54, 55, 56, 57, 58, 59,
		60, 61, 62, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
		63, 63, 63, 63, 63, 63
	};

	public int DMRId { get; set; }

	public string DMRIdString { get; set; }

	public string Callsign { get; set; }

	public string Details { get; set; }

	public string Name { get; set; }

	public string AgeInDays { get; set; }

	public int AgeAsInt { get; set; }

	public DMRDataItem()
	{
		Callsign = "";
		DMRId = 0;
	}

	public DMRDataItem FromHamDigital(string CSVLine)
	{
		string[] array = CSVLine.Split(';');
		Callsign = array[1];
		Name = array[3];
		DMRId = int.Parse(array[2]);
		try
		{
			AgeAsInt = int.Parse(array[4]);
			AgeInDays = array[4];
		}
		catch (Exception)
		{
		}
		return this;
	}

	public DMRDataItem FromURLOrCSV(string CSVLine, CheckBox[] columnsFilter, string separator = ".")
	{
		Details = "";
		string[] array = CSVLine.Split(',');
		if (array.Length < 3)
		{
			return null;
		}
		DMRIdString = array[0];
		DMRId = int.Parse(array[0]);
		if (DMRId < 1 || DMRId > 16777215)
		{
			return null;
		}
		Callsign = array[1].Trim();
		for (int i = 0; i < 5; i++)
		{
			if (array.Length >= i + 3 && columnsFilter[i].Checked && array[i + 2].Trim() != "")
			{
				Details = Details + separator + array[i + 2].Trim();
			}
		}
		for (int j = 0; j < Details.Length; j++)
		{
			if (Details[j] >= '\u0080')
			{
				Details = RemoveDiacritics(Details);
				break;
			}
		}
		if (Details.Length > 1)
		{
			Details = Details.Substring(1);
		}
		Details = Regex.Replace(Details, "\\s{2,}", " ").Trim();
		return this;
	}

	private static string RemoveDiacritics(string text)
	{
		string text2 = text.Normalize(NormalizationForm.FormD);
		StringBuilder stringBuilder = new StringBuilder();
		string text3 = text2;
		foreach (char c in text3)
		{
			if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
	}

	private byte Int8ToBCD(int val)
	{
		int num = val / 10;
		int num2 = val % 10;
		return (byte)(num * 16 + num2);
	}

	private byte BCDToByte(byte val)
	{
		int num = val >> 4;
		int num2 = val & 0xF;
		return (byte)(num * 10 + num2);
	}

	public static byte compressChar(char b)
	{
		switch (b)
		{
		case ' ':
			return 0;
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
			return (byte)(b - 48 + 1);
		default:
			if (b >= 'a' && b <= 'z')
			{
				return (byte)(b - 97 + 10 + 26 + 1);
			}
			if (b >= 'A' && b <= 'Z')
			{
				return (byte)(b - 65 + 10 + 1);
			}
			return 63;
		}
	}

	public static int compressSize(int fromSize)
	{
		return fromSize / 4 * 3 + fromSize % 4;
	}

	public byte[] compress(string txtBuf)
	{
		byte[] array = new byte[compressSize(txtBuf.Length)];
		int num = 0;
		int num2 = 0;
		do
		{
			array[num] = (byte)(COMPRESS_LUT[(byte)txtBuf[num2]] << 2);
			num2++;
			if (num2 == txtBuf.Length)
			{
				break;
			}
			array[num] |= (byte)(COMPRESS_LUT[(byte)txtBuf[num2]] >> 4);
			array[num + 1] = (byte)(COMPRESS_LUT[(byte)txtBuf[num2]] << 4);
			num2++;
			if (num2 == txtBuf.Length)
			{
				break;
			}
			array[num + 1] |= (byte)(COMPRESS_LUT[(byte)txtBuf[num2]] >> 2);
			array[num + 2] = (byte)(COMPRESS_LUT[(byte)txtBuf[num2]] << 6);
			num2++;
			if (num2 == txtBuf.Length)
			{
				break;
			}
			array[num + 2] |= (byte)COMPRESS_LUT[(byte)txtBuf[num2]];
			num2++;
			num += 3;
		}
		while (num2 < txtBuf.Length);
		return array;
	}

	public string decompress(byte[] compressedBuf)
	{
		string text = "";
		int num = 0;
		do
		{
			byte b = compressedBuf[num++];
			text += DECOMPRESS_LUT[b >> 2];
			if (num == compressedBuf.Length)
			{
				break;
			}
			byte b2 = compressedBuf[num++];
			text += DECOMPRESS_LUT[((b & 3) << 4) + (b2 >> 4)];
			if (num == compressedBuf.Length)
			{
				break;
			}
			byte b3 = compressedBuf[num++];
			text += DECOMPRESS_LUT[((b2 & 0xF) << 2) + (b3 >> 6)];
			text += DECOMPRESS_LUT[b3 & 0x3F];
		}
		while (num < compressedBuf.Length);
		return text;
	}

	public byte[] getRadioData(int stringLength)
	{
		int num = 3;
		byte[] array = new byte[compressSize(stringLength) + num];
		if (DMRId != 0)
		{
			string text = Callsign + " " + Details;
			byte[] array2 = compress(text.Substring(0, Math.Min(text.Length, stringLength)));
			Array.Copy(array2, 0, array, num, array2.Length);
			byte[] bytes = BitConverter.GetBytes(DMRId);
			array[0] = bytes[0];
			array[1] = bytes[1];
			array[2] = bytes[2];
		}
		return array;
	}

	public int CompareTo(DMRDataItem comparePart)
	{
		if (comparePart == null)
		{
			return 1;
		}
		if (comparePart.DMRId < DMRId)
		{
			return 1;
		}
		if (comparePart.DMRId > DMRId)
		{
			return -1;
		}
		return 0;
	}

	public bool Equals(DMRDataItem other)
	{
		if (other == null)
		{
			return false;
		}
		if (other == this)
		{
			return true;
		}
		return DMRId == other.DMRId;
	}

	public override int GetHashCode()
	{
		return DMRId;
	}
}
