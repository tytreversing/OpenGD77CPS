using System;
using System.Collections.Generic;

namespace DMR;

public sealed class BoyerMoore
{
	private readonly byte[] needle;

	private readonly int[] charTable;

	private readonly int[] offsetTable;

	public BoyerMoore(byte[] needle)
	{
		this.needle = needle;
		charTable = makeByteTable(needle);
		offsetTable = makeOffsetTable(needle);
	}

	public IEnumerable<int> Search(byte[] haystack)
	{
		if (needle.Length == 0)
		{
			yield break;
		}
		int j;
		for (int i = needle.Length - 1; i < haystack.Length; i += ((j == 0) ? 1 : Math.Max(offsetTable[needle.Length - 1 - j], charTable[haystack[i]])))
		{
			j = needle.Length - 1;
			while (needle[j] == haystack[i])
			{
				if (j == 0)
				{
					yield return i;
					i += needle.Length - 1;
					break;
				}
				int num = i - 1;
				i = num;
				num = j - 1;
				j = num;
			}
		}
	}

	private static int[] makeByteTable(byte[] needle)
	{
		int[] array = new int[256];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = needle.Length;
		}
		for (int j = 0; j < needle.Length - 1; j++)
		{
			array[needle[j]] = needle.Length - 1 - j;
		}
		return array;
	}

	private static int[] makeOffsetTable(byte[] needle)
	{
		int[] array = new int[needle.Length];
		int num = needle.Length;
		for (int num2 = needle.Length - 1; num2 >= 0; num2--)
		{
			if (isPrefix(needle, num2 + 1))
			{
				num = num2 + 1;
			}
			array[needle.Length - 1 - num2] = num - num2 + needle.Length - 1;
		}
		for (int i = 0; i < needle.Length - 1; i++)
		{
			int num3 = suffixLength(needle, i);
			array[num3] = needle.Length - 1 - i + num3;
		}
		return array;
	}

	private static bool isPrefix(byte[] needle, int p)
	{
		int num = p;
		int num2 = 0;
		while (num < needle.Length)
		{
			if (needle[num] != needle[num2])
			{
				return false;
			}
			num++;
			num2++;
		}
		return true;
	}

	private static int suffixLength(byte[] needle, int p)
	{
		int num = 0;
		int num2 = p;
		int num3 = needle.Length - 1;
		while (num2 >= 0 && needle[num2] == needle[num3])
		{
			num++;
			num2--;
			num3--;
		}
		return num;
	}
}
