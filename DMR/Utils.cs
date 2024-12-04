using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DMR;

public sealed class Utils
{
	private static Dictionary<string, string> StringsDict = new Dictionary<string, string>();

	public Utils()
	{
		Settings.ReadCommonsForSectionIntoDictionary(StringsDict, "Utils");
	}

	public bool MergeLanguageFile(IWin32Window parentWindow, ref byte[] openFirmwareData, byte[] languageData)
	{
		if (new BoyerMoore(new byte[8] { 73, 71, 78, 82, 76, 65, 78, 71 }).Search(openFirmwareData).Count() >= 2)
		{
			return true;
		}
		BoyerMoore boyerMoore = new BoyerMoore(new byte[8] { 71, 68, 55, 55, 76, 65, 78, 71 });
		IEnumerable<int> source = boyerMoore.Search(openFirmwareData);
		IEnumerable<int> source2 = boyerMoore.Search(languageData);
		if (source.Count() > 1 && source2.Count() == 1 && source2.ElementAt(0) == 0)
		{
			byte[] needle = new byte[7] { 69, 110, 103, 108, 105, 115, 104 };
			int num = (languageData[8] << 24) | (languageData[9] << 16) | (languageData[10] << 8) | languageData[11];
			IEnumerable<int> source3 = new BoyerMoore(needle).Search(openFirmwareData);
			for (int i = 0; i < source.Count(); i++)
			{
				for (int j = 0; j < source3.Count(); j++)
				{
					if (source.ElementAt(i) + 12 != source3.ElementAt(j))
					{
						continue;
					}
					int num2 = source.ElementAt(i);
					int num3 = (openFirmwareData[num2 + 8] << 24) | (openFirmwareData[num2 + 9] << 16) | (openFirmwareData[num2 + 10] << 8) | openFirmwareData[num2 + 11];
					if (num3 == num)
					{
						if (i + 1 < source.Count())
						{
							Array.Copy(languageData, 0, openFirmwareData, source.ElementAt(i + 1), languageData.Length);
							return true;
						}
						MessageBox.Show(parentWindow, string.Format(StringsDict["bad_file_format"] + ": {0}", source.Count()));
					}
					else
					{
						MessageBox.Show(parentWindow, string.Format(StringsDict["versions_mismatch"] + ": {0} vs {1}", num3, num));
					}
				}
			}
		}
		else
		{
			if (source.Count() <= 1)
			{
				MessageBox.Show(parentWindow, string.Format(StringsDict["bad_file_format"] + ": {0}", source.Count()));
			}
			if (source2.Count() != 1)
			{
				MessageBox.Show(parentWindow, string.Format(StringsDict["wrong_language_header"] + ": {0:X} {1:X} {2:X} {3:X} {4:X} {5:X} {6:X} {7:X} (len: {8})", languageData[0], languageData[1], languageData[2], languageData[3], languageData[4], languageData[5], languageData[6], languageData[7], languageData.Length));
			}
		}
		MessageBox.Show(parentWindow, string.Format(StringsDict["failed_to_merge_language_file"]));
		return false;
	}
}
