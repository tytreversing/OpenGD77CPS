using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DMR;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class RxListData : IData
{
	public const int CNT_RX_LIST = 76;

	public const int CNT_RX_LIST_INDEX = 128;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
	private byte[] rxListIndex;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 76)]
	private RxListOneData[] rxList;

	public RxListOneData[] RxList => rxList;

	public RxListOneData this[int index]
	{
		get
		{
			if (index >= Count)
			{
				throw new ArgumentOutOfRangeException();
			}
			return rxList[index];
		}
		set
		{
			if (index >= Count)
			{
				throw new ArgumentOutOfRangeException();
			}
			rxList[index] = value;
		}
	}

	public int Count => 76;

	public string Format => "GroupList{0}";

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

	public RxListData()
	{
		int num = 0;
		rxListIndex = new byte[128];
		rxList = new RxListOneData[76];
		for (num = 0; num < Count; num++)
		{
			rxList[num] = new RxListOneData(num);
		}
	}

	public void Clear()
	{
	}

	public int GetContactCntByIndex(int index)
	{
		if (index < 76)
		{
			if (rxListIndex[index] >= 2 && rxListIndex[index] <= 33)
			{
				return rxListIndex[index] - 1;
			}
			return 0;
		}
		return 0;
	}

	public void SetRxListIndex(int index, bool add)
	{
		if (add)
		{
			rxListIndex[index] = 1;
		}
		else
		{
			rxListIndex[index] = 0;
		}
	}

	public int GetMinIndex()
	{
		int num = 0;
		num = 0;
		while (true)
		{
			if (num < Count)
			{
				if (rxListIndex[num] == 0 || rxListIndex[num] > 33)
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
		if (rxListIndex[index] != 0)
		{
			return rxListIndex[index] <= 33;
		}
		return false;
	}

	public void SetIndex(int index, int value)
	{
		rxListIndex[index] = (byte)value;
	}

	public int GetContactsCountForIndex(int index)
	{
		return Math.Max(0, rxListIndex[index] - 1);
	}

	public void ClearIndex(int index)
	{
		rxListIndex[index] = 0;
		ChannelForm.data.ClearByRxGroup(index);
	}

	public void ClearByData(int contactIndex)
	{
		int num = 0;
		int num2 = 0;
		for (num = 0; num < Count; num++)
		{
			if (DataIsValid(num))
			{
				num2 = Array.IndexOf(rxList[num].ContactList, (ushort)(contactIndex + 1));
				if (num2 >= 0)
				{
					rxList[num].ContactList.RemoveItemFromArray(num2);
					rxListIndex[num]--;
				}
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

	public void SetName(int index, string text)
	{
		rxList[index].Name = text;
	}

	public string GetName(int index)
	{
		return rxList[index].Name;
	}

	public void Default(int index)
	{
		rxList[index].ContactList.Fill((ushort)0);
	}

	public void Paste(int from, int to)
	{
		rxListIndex[to] = rxListIndex[from];
		Array.Copy(rxList[from].ContactList, rxList[to].ContactList, rxList[from].ContactList.Length);
	}

	public void Verify()
	{
		int num = 0;
		for (num = 0; num < Count; num++)
		{
			if (DataIsValid(num))
			{
				rxList[num].Verify();
				rxListIndex[num] = (byte)(rxList[num].ValidCount + 1);
			}
			else
			{
				rxListIndex[num] = 0;
			}
		}
	}

	public int AddRxGroupWithName(string newGroupName)
	{
		int num = Array.FindIndex(RxGroupListForm.data.RxList, (RxListOneData item) => item.Name == newGroupName);
		if (num == -1)
		{
			num = GetMinIndex();
			if (num != -1)
			{
				SetIndex(num, 1);
				Default(num);
				SetName(num, newGroupName);
			}
		}
		return num;
	}
}
