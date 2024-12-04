using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DMR;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RxListOneData
{
	public const int LEN_RX_LIST_NAME = 16;

	public const int CNT_CONTACT_PER_RX_LIST = 32;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	private byte[] name;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	private ushort[] contactList;

	public string Name
	{
		get
		{
			return Settings.smethod_25(name);
		}
		set
		{
			byte[] array = Settings.smethod_23(value);
			name.Fill(byte.MaxValue);
			Array.Copy(array, 0, name, 0, Math.Min(array.Length, name.Length));
		}
	}

	public ushort[] ContactList
	{
		get
		{
			return contactList;
		}
		set
		{
			contactList.Fill((ushort)0);
			Array.Copy(value, 0, contactList, 0, value.Length);
		}
	}

	public byte ValidCount => (byte)new List<ushort>(contactList).FindAll(isValidGroupOrPrivateCallCall).Count;

	public RxListOneData(int index)
	{
		this = default(RxListOneData);
		name = new byte[16];
		contactList = new ushort[32];
	}

	public void Verify()
	{
		List<ushort> list = new List<ushort>(contactList).FindAll(isValidGroupOrPrivateCallCall);
		while (list.Count < contactList.Length)
		{
			list.Add(0);
		}
		contactList = list.ToArray();
	}

	public bool ContainsContact(ushort id)
	{
		return Array.FindIndex(contactList, (ushort item) => item == id) != -1;
	}

	[CompilerGenerated]
	private static bool isValidGroupCall1(ushort ushort_0)
	{
		if (ushort_0 != 0 && ContactForm.data.DataIsValid(ushort_0 - 1))
		{
			return ContactForm.data.IsGroupCall(ushort_0 - 1);
		}
		return false;
	}

	[CompilerGenerated]
	private static bool isValidGroupCall2(ushort ushort_0)
	{
		if (ushort_0 != 0 && ContactForm.data.DataIsValid(ushort_0 - 1))
		{
			return ContactForm.data.IsGroupCall(ushort_0 - 1);
		}
		return false;
	}

	private static bool isValidGroupOrPrivateCallCall(ushort ushort_0)
	{
		if (ushort_0 != 0 && ContactForm.data.DataIsValid(ushort_0 - 1))
		{
			if (!ContactForm.data.IsGroupCall(ushort_0 - 1) && !ContactForm.data.IsPrivateCall(ushort_0 - 1))
			{
				return ContactForm.data.IsAllCall(ushort_0 - 1);
			}
			return true;
		}
		return false;
	}
}
