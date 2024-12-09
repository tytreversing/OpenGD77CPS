using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class NormalScanForm : DockContent, IDisp
{
	private enum ScanCh
	{
		None,
		Selected
	}

	private enum TxDesignatedCh
	{
		LastActiveCh,
		SelectCh
	}

	private enum PlTypeE
	{
		NonPriorityCh,
		Disable,
		PriorityCh,
		PriorityChAndNonPriorityCh
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct NormalScanOne : IVerify<NormalScanOne>
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
		private byte[] name;

		private byte flag1;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		private ushort[] chList;

		private ushort priorityCh1;

		private ushort priorityCh2;

		private ushort txDesignatedCh;

		private byte signalingHold;

		private byte prioritySample;

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

		public bool Talkback
		{
			get
			{
				return Convert.ToBoolean(flag1 & 0x80);
			}
			set
			{
				if (value)
				{
					flag1 |= 128;
				}
				else
				{
					flag1 &= 127;
				}
			}
		}

		public int PlType
		{
			get
			{
				return (flag1 & 0x60) >> 5;
			}
			set
			{
				value = (value << 5) & 0x60;
				flag1 &= 159;
				flag1 |= (byte)value;
			}
		}

		public bool ChMark
		{
			get
			{
				return Convert.ToBoolean(flag1 & 0x10);
			}
			set
			{
				if (value)
				{
					flag1 |= 16;
				}
				else
				{
					flag1 &= 239;
				}
			}
		}

		public ushort[] ChList
		{
			get
			{
				return chList;
			}
			set
			{
				chList.Fill((ushort)0);
				Array.Copy(value, 0, chList, 0, Math.Min(32, value.Length));
			}
		}

		public int PriorityCh1
		{
			get
			{
				if (priorityCh1 <= 1024)
				{
					return priorityCh1;
				}
				return 0;
			}
			set
			{
				if (value <= 1024)
				{
					priorityCh1 = (ushort)value;
				}
			}
		}

		public int PriorityCh2
		{
			get
			{
				if (priorityCh2 <= 1024)
				{
					return priorityCh2;
				}
				return 0;
			}
			set
			{
				if (value <= 1024)
				{
					priorityCh2 = (ushort)value;
				}
			}
		}

		public int TxDesignatedCh
		{
			get
			{
				return txDesignatedCh;
			}
			set
			{
				txDesignatedCh = (ushort)value;
			}
		}

		public decimal SignalingHold
		{
			get
			{
				if (signalingHold < 2 || signalingHold > byte.MaxValue)
				{
					return 500m;
				}
				return signalingHold * 25;
			}
			set
			{
				byte b = (byte)(value / 25m);
				if (b >= 2 && b <= byte.MaxValue)
				{
					signalingHold = b;
				}
				else
				{
					signalingHold = 20;
				}
			}
		}

		public decimal PrioritySample
		{
			get
			{
				if (prioritySample < 3 || prioritySample > 31)
				{
					return 2000m;
				}
				return prioritySample * 250;
			}
			set
			{
				int num = (int)(value / 250m);
				if (num >= 3 && num <= 31)
				{
					prioritySample = (byte)num;
				}
				else
				{
					prioritySample = 8;
				}
			}
		}

		public NormalScanOne(int index)
		{
			this = default(NormalScanOne);
			name = new byte[15];
			chList = new ushort[32];
			chList[0] = 1;
		}

		public void Default()
		{
			chList.Fill((ushort)0);
			chList[0] = 1;
			priorityCh1 = 0;
			priorityCh2 = 0;
			txDesignatedCh = 1;
			SignalingHold = DefaultScan.SignalingHold;
			PrioritySample = DefaultScan.PrioritySample;
			Flag1 = DefaultScan.Flag1;
		}

		public NormalScanOne Clone()
		{
			return Settings.cloneObject(this);
		}

		public void Verify(NormalScanOne def)
		{
			int num = 0;
			int num2 = 0;
			List<ushort> list = new List<ushort>(chList);
			list[0] = 1;
			for (num = 1; num < list.Count; num++)
			{
				if (list[num] == 1)
				{
					list.RemoveAt(num);
					list.Add(0);
					num--;
				}
				else if (list[num] != 0)
				{
					num2 = list[num] - 2;
					if (!ChannelForm.data.DataIsValid(num2))
					{
						list.RemoveAt(num);
						list.Add(0);
						num--;
					}
				}
			}
			chList = list.ToArray();
			if (priorityCh1 == 0)
			{
				priorityCh2 = 0;
			}
			else if (priorityCh1 == priorityCh2)
			{
				priorityCh2 = 0;
			}
			else if (!list.Contains(priorityCh1))
			{
				priorityCh1 = 0;
			}
			if (priorityCh2 != 0 && !list.Contains(priorityCh1))
			{
				priorityCh2 = 0;
			}
			if (!Enum.IsDefined(typeof(TxDesignatedCh), (int)txDesignatedCh))
			{
				num2 = txDesignatedCh - 2;
				if (!ChannelForm.data.DataIsValid(num2))
				{
					txDesignatedCh = 0;
				}
			}
			Settings.ValidateNumberRangeWithDefault(ref signalingHold, 2, byte.MaxValue, def.signalingHold);
			Settings.ValidateNumberRangeWithDefault(ref prioritySample, 3, 31, def.prioritySample);
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class NormalScan : IData
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		private byte[] scanListIndex;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		private NormalScanOne[] scanList;

		public NormalScanOne this[int index]
		{
			get
			{
				if (index >= 64)
				{
					throw new ArgumentOutOfRangeException();
				}
				return scanList[index];
			}
			set
			{
				if (index >= Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				scanList[index] = value;
			}
		}

		public int Count => 64;

		public string Format => "ScanList{0}";

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

		public NormalScan()
		{
			int num = 0;
			scanListIndex = new byte[64];
			scanListIndex.Fill((byte)0);
			scanList = new NormalScanOne[64];
			for (num = 0; num < scanList.Length; num++)
			{
				scanList[num] = new NormalScanOne(num);
			}
		}

		public void ClearByData(int chIndex)
		{
			int num = 0;
			int num2 = 0;
			for (num = 0; num < Count; num++)
			{
				if (!DataIsValid(num))
				{
					continue;
				}
				num2 = Array.IndexOf(scanList[num].ChList, (byte)(chIndex + 2));
				if (num2 >= 0)
				{
					scanList[num].ChList.RemoveItemFromArray(num2);
					if (!scanList[num].ChList.Contains((byte)scanList[num].PriorityCh1))
					{
						scanList[num].PriorityCh1 = 0;
						scanList[num].PriorityCh2 = 0;
					}
				}
				if (scanList[num].TxDesignatedCh == chIndex + 2)
				{
					scanList[num].TxDesignatedCh = 0;
				}
			}
		}

		public bool DataIsValid(int index)
		{
			if (index < 64)
			{
				if (scanListIndex[index] != 0)
				{
					return scanListIndex[index] <= 2;
				}
				return false;
			}
			return false;
		}

		public int GetMinIndex()
		{
			int num = 0;
			num = 0;
			while (true)
			{
				if (num < Count)
				{
					if (scanListIndex[num] == 0 || scanListIndex[num] > 2)
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

		public void SetIndex(int index, int value)
		{
			if (index < 64)
			{
				scanListIndex[index] = (byte)value;
			}
		}

		public void ClearIndex(int index)
		{
			SetIndex(index, 0);
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
			if (index < 64)
			{
				scanList[index].Name = text;
			}
		}

		public string GetName(int index)
		{
			return scanList[index].Name;
		}

		public void Default(int index)
		{
			scanList[index].Default();
		}

		public void Paste(int from, int to)
		{
			Array.Copy(scanList[from].ChList, scanList[to].ChList, scanList[to].ChList.Length);
			scanList[to].PriorityCh1 = scanList[from].PriorityCh1;
			scanList[to].PriorityCh2 = scanList[from].PriorityCh2;
			scanList[to].TxDesignatedCh = scanList[from].TxDesignatedCh;
			scanList[to].SignalingHold = scanList[from].SignalingHold;
			scanList[to].PrioritySample = scanList[from].PrioritySample;
			scanList[to].Flag1 = scanList[from].Flag1;
		}

		public void Verify()
		{
			int num = 0;
			for (num = 0; num < Count; num++)
			{
				if (DataIsValid(num))
				{
					scanList[num].Verify(DefaultScan);
				}
				else
				{
					scanListIndex[num] = 0;
				}
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class NormalScanEx
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		private ushort[] priorityCh1;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		private ushort[] priorityCh2;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		private ushort[] specifyCh;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]
		private ushort[] scanChList;

		public ushort[] PriorityCh1 => priorityCh1;

		public ushort[] PriorityCh2 => priorityCh2;

		public ushort[] SpecifyCh => specifyCh;

		public ushort[] ScanChList => scanChList;

		public NormalScanEx()
		{
			priorityCh1 = new ushort[64];
			priorityCh2 = new ushort[64];
			specifyCh = new ushort[64];
			scanChList = new ushort[2048];
			int num = 0;
			for (num = 0; num < 64; num++)
			{
				scanChList[31] = 1;
			}
		}

		public void ClearByData(int chIndex)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (num = 0; num < 64; num++)
			{
				if (specifyCh[num] == chIndex + 2)
				{
					specifyCh[num] = 0;
				}
				if (priorityCh1[num] == chIndex + 2)
				{
					priorityCh1[num] = 0;
					priorityCh2[num] = 0;
				}
				else if (priorityCh2[num] == chIndex + 2)
				{
					priorityCh2[num] = 0;
				}
				for (num2 = 0; num2 < 32; num2++)
				{
					num3 = num * 32;
					ushort[] array = new ushort[32];
					Array.Copy(scanChList, num3, array, 0, array.Length);
					num4 = Array.IndexOf(array, Convert.ToUInt16(chIndex + 2));
					if (num4 >= 0)
					{
						array.smethod_3(num4, (ushort)0);
						Array.Copy(array, 0, scanChList, num3, array.Length);
					}
				}
			}
		}

		public void Default(int index)
		{
			priorityCh1[index] = 0;
			priorityCh2[index] = 0;
			specifyCh[index] = 1;
			scanChList.FillFromPositionWithLength((ushort)0, index * 32, 32);
			scanChList[index * 32] = 1;
		}

		public void Verify()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (num = 0; num < 32; num++)
			{
				ushort[] array = new ushort[32];
				Array.Copy(scanChList, num * 32, array, 0, array.Length);
				List<ushort> list = new List<ushort>(array);
				list[0] = 1;
				for (num2 = 1; num2 < list.Count; num2++)
				{
					if (list[num2] == 1)
					{
						list.RemoveAt(num2);
						list.Add(0);
						num2--;
					}
					else if (list[num2] != 0)
					{
						num3 = list[num2] - 2;
						if (!ChannelForm.data.DataIsValid(num3))
						{
							list.RemoveAt(num2);
							list.Add(0);
							num2--;
						}
					}
				}
				array = list.ToArray();
				Array.Copy(array, 0, scanChList, num * 32, array.Length);
				if (priorityCh1[num] == 0)
				{
					priorityCh2[num] = 0;
				}
				else if (priorityCh1[num] == priorityCh2[num])
				{
					priorityCh2[num] = 0;
				}
				else if (!list.Contains(priorityCh1[num]))
				{
					priorityCh1[num] = 0;
				}
				if (priorityCh2[num] != 0 && !list.Contains(priorityCh1[num]))
				{
					priorityCh2[num] = 0;
				}
			}
			for (num = 0; num < specifyCh.Length; num++)
			{
				if (!Enum.IsDefined(typeof(TxDesignatedCh), (int)specifyCh[num]))
				{
					num3 = specifyCh[num] - 2;
					if (!ChannelForm.data.DataIsValid(num3))
					{
						specifyCh[num] = 0;
					}
				}
			}
		}
	}

	public const int CNT_SCAN_LIST = 64;

	public const int LEN_SCAN_LIST_NAME = 15;

	private const int MAX_CH_PER_SCAN_LIST = 31;

	private const int CNT_CH_PER_SCAN_LIST = 32;

	public const string SZ_PRIORITY_CH_NAME = "PriorityCh";

	public const string SZ_TX_DESIGNATED_CH_NAME = "TxDesignatedCh";

	public const string SZ_PL_TYPE_NAME = "PlType";

	private const int MIN_SIGNALING_HOLD = 2;

	private const int MAX_SIGNALING_HOLD = 255;

	private const int INC_SIGNALING_HOLD = 1;

	private const int SCL_SIGNALING_HOLD = 25;

	private const int LEN_SIGNALING_HOLD = 4;

	private const int MIN_PRIORITY_SAMPLE = 3;

	private const int MAX_PRIORITY_SAMPLE = 31;

	private const int INC_PRIORITY_SAMPLE = 1;

	private const int SCL_PRIORITY_SAMPLE = 250;

	private const int LEN_PRIORITY_SAMPLE = 4;

	private static readonly string[] SZ_PRIORITY_CH;

	private static readonly string[] SZ_TX_DESIGNATED_CH;

	private static readonly string[] SZ_PL_TYPE;

	public static NormalScanOne DefaultScan;

	public static NormalScan data;

	private CustomNumericUpDown nudSignalingHold;

	private Button btnDel;

	private Button btnAdd;

	private ListBox lstSelected;

	private ListBox lstUnselected;

	private CustomCombo cmbTxDesignatedCh;

	private Label lblSignalingHold;

	private Label lblTxDesignatedCh;

	private CheckBox chkChMark;

	private CheckBox chkTalkback;

	private Label lblPrioritySample;

	private CustomNumericUpDown nudPrioritySample;

	private Label label_0;

	private CustomCombo cmbPlType;

	private SGTextBox txtName;

	private Label lblName;

	private CustomCombo cmbPriorityCh1;

	private CustomCombo cmbPriorityCh2;

	private Label lblPriorityCh1;

	private Label lblPriorityCh2;

	private GroupBox grpUnselected;

	private GroupBox grpSelected;

	private GroupBox grpListParam;

	private Button btnDown;

	private Button btnUp;

	private CustomPanel panel1;

	public TreeNode Node { get; set; }

	public void SaveData()
	{
		int num = 0;
		int num2 = 0;
		try
		{
			num2 = Convert.ToInt32(base.Tag);
			if (num2 == -1)
			{
				return;
			}
			if (txtName.Focused)
			{
				txtName_Leave(txtName, null);
			}
			NormalScanOne value = new NormalScanOne(num2);
			value.Name = txtName.Text;
			value.ChMark = chkChMark.Checked;
			value.Talkback = chkTalkback.Checked;
			value.PlType = cmbPlType.SelectedIndex;
			value.TxDesignatedCh = cmbTxDesignatedCh.method_3();
			value.PriorityCh1 = cmbPriorityCh1.method_3();
			value.PriorityCh2 = cmbPriorityCh2.method_3();
			value.SignalingHold = nudSignalingHold.Value;
			value.PrioritySample = nudPrioritySample.Value;
			for (num = 0; num < value.ChList.Length; num++)
			{
				value.ChList[num] = 0;
			}
			num = 0;
			foreach (SelectedItemUtils item in lstSelected.Items)
			{
				value.ChList[num] = (ushort)item.Value;
				num++;
			}
			data[num2] = value;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void DispData()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		string text = "";
		try
		{
			num2 = Convert.ToInt32(base.Tag);
			if (num2 == -1)
			{
				Close();
				return;
			}
			method_0();
			txtName.Text = data[num2].Name;
			chkChMark.Checked = data[num2].ChMark;
			chkTalkback.Checked = data[num2].Talkback;
			cmbTxDesignatedCh.method_2(data[num2].TxDesignatedCh);
			cmbPlType.SelectedIndex = data[num2].PlType;
			nudSignalingHold.Value = data[num2].SignalingHold;
			nudPrioritySample.Value = data[num2].PrioritySample;
			lstSelected.Items.Clear();
			for (num = 0; num < data[num2].ChList.Length; num++)
			{
				num3 = data[num2].ChList[num];
				if (num3 == 1)
				{
					lstSelected.Items.Add(new SelectedItemUtils(num, num3, Settings.SZ_SELECTED));
				}
				else if (num3 > 1 && num3 <= 1025)
				{
					num4 = num3 - 2;
					if (ChannelForm.data.DataIsValid(num4))
					{
						text = ChannelForm.data.GetName(num4);
						lstSelected.Items.Add(new SelectedItemUtils(num, num3, text));
					}
				}
			}
			if (lstSelected.Items.Count > 0)
			{
				lstSelected.SelectedIndex = 0;
			}
			int[] array = new int[32];
			for (num = 0; num < array.Length; num++)
			{
				array[num] = data[num2].ChList[num];
			}
			lstUnselected.Items.Clear();
			for (num = 0; num < ChannelForm.CurCntCh; num++)
			{
				if (ChannelForm.data.DataIsValid(num) && !array.Contains(num + 2))
				{
					num3 = num + 1;
					lstUnselected.Items.Add(new SelectedItemUtils(-1, num3 + 1, ChannelForm.data.GetName(num)));
				}
			}
			if (lstUnselected.Items.Count > 0)
			{
				lstUnselected.SelectedIndex = 0;
			}
			method_1();
			cmbPriorityCh1.method_2(data[num2].PriorityCh1);
			cmbPriorityCh2.method_2(data[num2].PriorityCh2);
			method_4();
			RefreshByUserMode();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void RefreshByUserMode()
	{
		bool flag = Settings.getUserExpertSettings() == Settings.UserMode.Expert;
		chkChMark.Enabled &= flag;
		lblSignalingHold.Enabled &= flag;
		nudSignalingHold.Enabled &= flag;
		lblPrioritySample.Enabled &= flag;
		nudPrioritySample.Enabled &= flag;
	}

	public void RefreshName()
	{
		int index = Convert.ToInt32(base.Tag);
		txtName.Text = data[index].Name;
	}

	public NormalScanForm()
	{
		InitializeComponent();
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	private void method_0()
	{
		Settings.smethod_45(cmbTxDesignatedCh, SZ_TX_DESIGNATED_CH, ChannelForm.data);
	}

	private void method_1()
	{
		Settings.smethod_46(cmbPriorityCh1, SZ_PRIORITY_CH, lstSelected);
		Settings.smethod_46(cmbPriorityCh2, SZ_PRIORITY_CH, lstSelected);
	}

	private void method_2()
	{
		int num = 0;
		string priCh3 = cmbPriorityCh1.Text;
		string priCh4 = cmbPriorityCh2.Text;
		method_1();
		if (cmbPriorityCh1.FindStringExact(priCh3) < 0)
		{
			num = Array.FindIndex(cmbPriorityCh1.Items.Cast<NameValuePair>().ToArray(), delegate(NameValuePair x)
			{
				if (x.Text == priCh3)
				{
					return true;
				}
				return x.Text.Contains(':') && priCh3.Contains(':') && x.Text.Split(':')[1] == priCh3.Split(':')[1];
			});
			if (num < 0)
			{
				cmbPriorityCh1.SelectedIndex = 0;
				cmbPriorityCh2.SelectedIndex = 0;
				return;
			}
			cmbPriorityCh1.SelectedIndex = num;
			num = Array.FindIndex(cmbPriorityCh2.Items.Cast<NameValuePair>().ToArray(), delegate(NameValuePair x)
			{
				if (x.Text == priCh4)
				{
					return true;
				}
				return x.Text.Contains(':') && priCh4.Contains(':') && x.Text.Split(':')[1] == priCh4.Split(':')[1];
			});
			if (num < 0)
			{
				cmbPriorityCh2.SelectedIndex = 0;
			}
			else
			{
				cmbPriorityCh2.SelectedIndex = num;
			}
			return;
		}
		cmbPriorityCh1.Text = priCh3;
		if (cmbPriorityCh2.FindStringExact(priCh4) < 0)
		{
			num = Array.FindIndex(cmbPriorityCh2.Items.Cast<NameValuePair>().ToArray(), delegate(NameValuePair x)
			{
				if (x.Text == priCh4)
				{
					return true;
				}
				return x.Text.Contains(':') && priCh4.Contains(':') && x.Text.Split(':')[1] == priCh4.Split(':')[1];
			});
			if (num < 0)
			{
				cmbPriorityCh2.SelectedIndex = 0;
			}
			else
			{
				cmbPriorityCh2.SelectedIndex = num;
			}
		}
		else
		{
			cmbPriorityCh2.Text = priCh4;
		}
	}

	private void method_3()
	{
		txtName.MaxByteLength = 15;
		txtName.KeyPress += Settings.smethod_54;
		Settings.fillComboBox(cmbPlType, SZ_PL_TYPE);
		Settings.smethod_36(nudSignalingHold, new Class13(2, 255, 1, 25m, 4));
		Settings.smethod_36(nudPrioritySample, new Class13(3, 31, 1, 250m, 4));
	}

	public static void RefreshCommonLang()
	{
		string name = typeof(NormalScanForm).Name;
		Settings.smethod_78("PriorityCh", SZ_PRIORITY_CH, name);
		Settings.smethod_78("TxDesignatedCh", SZ_TX_DESIGNATED_CH, name);
		Settings.smethod_78("PlType", SZ_PL_TYPE, name);
	}

	private void NormalScanForm_Load(object sender, EventArgs e)
	{
		Settings.smethod_59(base.Controls);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		method_3();
		DispData();
	}

	private void NormalScanForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveData();
	}

	private void method_4()
	{
		btnAdd.Enabled = lstUnselected.Items.Count > 0 && lstSelected.Items.Count < 32;
		btnDel.Enabled = lstSelected.Items.Count > 0 && !lstSelected.GetSelected(0);
		int count = lstSelected.Items.Count;
		int count2 = lstSelected.SelectedIndices.Count;
		btnUp.Enabled = lstSelected.SelectedItems.Count > 0 && lstSelected.Items.Count > 0 && lstSelected.SelectedIndices[count2 - 1] != count2 && !lstSelected.GetSelected(0);
		btnDown.Enabled = lstSelected.Items.Count > 0 && lstSelected.SelectedItems.Count > 0 && lstSelected.SelectedIndices[0] != count - count2 && !lstSelected.GetSelected(0);
	}

	private bool method_5()
	{
		if (lstSelected.GetSelected(0))
		{
			MessageBox.Show("Unable to operate selected");
			lstSelected.SetSelected(0, value: false);
			return false;
		}
		return true;
	}

	private void btnAdd_Click(object sender, EventArgs e)
	{
		int num = 0;
		int count = lstUnselected.SelectedIndices.Count;
		int num2 = lstUnselected.SelectedIndices[count - 1];
		lstSelected.SelectedItems.Clear();
		while (lstUnselected.SelectedItems.Count > 0 && lstSelected.Items.Count < 32)
		{
			SelectedItemUtils selectedItemUtils = (SelectedItemUtils)lstUnselected.SelectedItems[0];
			selectedItemUtils.method_1(lstSelected.Items.Count);
			num = lstSelected.Items.Add(selectedItemUtils);
			lstSelected.SetSelected(num, value: true);
			lstUnselected.Items.RemoveAt(lstUnselected.SelectedIndices[0]);
		}
		if (lstUnselected.SelectedItems.Count == 0)
		{
			int num3 = num2 - count + 1;
			if (num3 >= lstUnselected.Items.Count)
			{
				num3 = lstUnselected.Items.Count - 1;
			}
			lstUnselected.SelectedIndex = num3;
		}
		method_7();
		method_2();
		method_4();
		if (!btnAdd.Enabled)
		{
			lstSelected.Focus();
		}
	}

	private void btnDel_Click(object sender, EventArgs e)
	{
		if (method_5())
		{
			int num = 0;
			int count = lstSelected.SelectedIndices.Count;
			int num2 = lstSelected.SelectedIndices[count - 1];
			lstUnselected.SelectedItems.Clear();
			while (lstSelected.SelectedItems.Count > 0)
			{
				SelectedItemUtils selectedItemUtils = (SelectedItemUtils)lstSelected.SelectedItems[0];
				num = method_6(selectedItemUtils);
				selectedItemUtils.method_1(-1);
				lstUnselected.Items.Insert(num, selectedItemUtils);
				lstUnselected.SetSelected(num, value: true);
				lstSelected.Items.RemoveAt(lstSelected.SelectedIndices[0]);
			}
			int num3 = num2 - count + 1;
			if (num3 >= lstSelected.Items.Count)
			{
				num3 = lstSelected.Items.Count - 1;
			}
			lstSelected.SelectedIndex = num3;
			method_7();
			method_2();
			method_4();
		}
	}

	private int method_6(SelectedItemUtils class14_0)
	{
		int num = 0;
		num = 0;
		while (true)
		{
			if (num < lstUnselected.Items.Count)
			{
				SelectedItemUtils selectedItemUtils = (SelectedItemUtils)lstUnselected.Items[num];
				if (class14_0.Value < selectedItemUtils.Value)
				{
					break;
				}
				num++;
				continue;
			}
			return num;
		}
		return num;
	}

	private void btnUp_Click(object sender, EventArgs e)
	{
		if (!method_5())
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int count = lstSelected.SelectedIndices.Count;
		_ = lstSelected.SelectedIndices[count - 1];
		for (num = 0; num < count; num++)
		{
			num2 = lstSelected.SelectedIndices[num];
			if (num + 1 != num2)
			{
				object value = lstSelected.Items[num2];
				lstSelected.Items[num2] = lstSelected.Items[num2 - 1];
				lstSelected.Items[num2 - 1] = value;
				lstSelected.SetSelected(num2, value: false);
				lstSelected.SetSelected(num2 - 1, value: true);
			}
		}
		method_7();
		method_2();
	}

	private void btnDown_Click(object sender, EventArgs e)
	{
		if (!method_5())
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int count = lstSelected.Items.Count;
		int count2 = lstSelected.SelectedIndices.Count;
		_ = lstSelected.SelectedIndices[count2 - 1];
		num = count2 - 1;
		while (num >= 0)
		{
			num3 = lstSelected.SelectedIndices[num];
			if (count - 1 - num2 != num3)
			{
				object value = lstSelected.Items[num3];
				lstSelected.Items[num3] = lstSelected.Items[num3 + 1];
				lstSelected.Items[num3 + 1] = value;
				lstSelected.SetSelected(num3, value: false);
				lstSelected.SetSelected(num3 + 1, value: true);
			}
			num--;
			num2++;
		}
		method_7();
		method_2();
	}

	private void txtName_Leave(object sender, EventArgs e)
	{
		txtName.Text = txtName.Text.Trim();
		if (Node.Text != txtName.Text)
		{
			if (Settings.smethod_50(Node, txtName.Text))
			{
				txtName.Text = Node.Text;
			}
			else
			{
				Node.Text = txtName.Text;
			}
		}
	}

	private void cmbPriorityCh1_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (cmbPriorityCh1.SelectedIndex != 0 && cmbPriorityCh1.SelectedIndex != cmbPriorityCh2.SelectedIndex)
		{
			cmbPriorityCh2.Enabled = true;
			return;
		}
		cmbPriorityCh2.SelectedIndex = 0;
		cmbPriorityCh2.Enabled = false;
	}

	private void cmbPriorityCh2_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (cmbPriorityCh2.SelectedIndex != 0 && cmbPriorityCh1.SelectedIndex == cmbPriorityCh2.SelectedIndex)
		{
			cmbPriorityCh2.SelectedIndex = 0;
		}
	}

	private void method_7()
	{
		int num = 0;
		lstSelected.BeginUpdate();
		for (num = 0; num < lstSelected.Items.Count; num++)
		{
			SelectedItemUtils selectedItemUtils = (SelectedItemUtils)lstSelected.Items[num];
			if (selectedItemUtils.method_0() != num)
			{
				selectedItemUtils.method_1(num);
				bool selected = lstSelected.GetSelected(num);
				lstSelected.Items[num] = selectedItemUtils;
				if (selected)
				{
					lstSelected.SetSelected(num, value: true);
				}
			}
		}
		lstSelected.EndUpdate();
	}

	private void lstSelected_SelectedIndexChanged(object sender, EventArgs e)
	{
		method_4();
	}

	private void lstSelected_DoubleClick(object sender, EventArgs e)
	{
		if (lstSelected.SelectedItem != null)
		{
			SelectedItemUtils selectedItemUtils = lstSelected.SelectedItem as SelectedItemUtils;
			if (base.MdiParent is MainForm mainForm)
			{
				mainForm.DispChildForm(typeof(ChannelForm), selectedItemUtils.Value - 2);
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.panel1 = new CustomPanel();
		this.grpListParam = new System.Windows.Forms.GroupBox();
		this.chkTalkback = new System.Windows.Forms.CheckBox();
		this.chkChMark = new System.Windows.Forms.CheckBox();
		this.cmbPriorityCh2 = new CustomCombo();
		this.lblTxDesignatedCh = new System.Windows.Forms.Label();
		this.cmbPriorityCh1 = new CustomCombo();
		this.lblPriorityCh1 = new System.Windows.Forms.Label();
		this.lblSignalingHold = new System.Windows.Forms.Label();
		this.label_0 = new System.Windows.Forms.Label();
		this.nudPrioritySample = new CustomNumericUpDown();
		this.lblPriorityCh2 = new System.Windows.Forms.Label();
		this.nudSignalingHold = new CustomNumericUpDown();
		this.cmbTxDesignatedCh = new CustomCombo();
		this.cmbPlType = new CustomCombo();
		this.lblPrioritySample = new System.Windows.Forms.Label();
		this.btnDown = new System.Windows.Forms.Button();
		this.btnUp = new System.Windows.Forms.Button();
		this.txtName = new DMR.SGTextBox();
		this.grpSelected = new System.Windows.Forms.GroupBox();
		this.lstSelected = new System.Windows.Forms.ListBox();
		this.grpUnselected = new System.Windows.Forms.GroupBox();
		this.lstUnselected = new System.Windows.Forms.ListBox();
		this.lblName = new System.Windows.Forms.Label();
		this.btnDel = new System.Windows.Forms.Button();
		this.btnAdd = new System.Windows.Forms.Button();
		this.panel1.SuspendLayout();
		this.grpListParam.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nudPrioritySample).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudSignalingHold).BeginInit();
		this.grpSelected.SuspendLayout();
		this.grpUnselected.SuspendLayout();
		base.SuspendLayout();
		this.panel1.AutoScroll = true;
		this.panel1.AutoSize = true;
		this.panel1.Controls.Add(this.grpListParam);
		this.panel1.Controls.Add(this.btnDown);
		this.panel1.Controls.Add(this.btnUp);
		this.panel1.Controls.Add(this.txtName);
		this.panel1.Controls.Add(this.grpSelected);
		this.panel1.Controls.Add(this.grpUnselected);
		this.panel1.Controls.Add(this.lblName);
		this.panel1.Controls.Add(this.btnDel);
		this.panel1.Controls.Add(this.btnAdd);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Location = new System.Drawing.Point(0, 0);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(760, 744);
		this.panel1.TabIndex = 0;
		this.grpListParam.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.grpListParam.Controls.Add(this.chkTalkback);
		this.grpListParam.Controls.Add(this.chkChMark);
		this.grpListParam.Controls.Add(this.cmbPriorityCh2);
		this.grpListParam.Controls.Add(this.lblTxDesignatedCh);
		this.grpListParam.Controls.Add(this.cmbPriorityCh1);
		this.grpListParam.Controls.Add(this.lblPriorityCh1);
		this.grpListParam.Controls.Add(this.lblSignalingHold);
		this.grpListParam.Controls.Add(this.label_0);
		this.grpListParam.Controls.Add(this.nudPrioritySample);
		this.grpListParam.Controls.Add(this.lblPriorityCh2);
		this.grpListParam.Controls.Add(this.nudSignalingHold);
		this.grpListParam.Controls.Add(this.cmbTxDesignatedCh);
		this.grpListParam.Controls.Add(this.cmbPlType);
		this.grpListParam.Controls.Add(this.lblPrioritySample);
		this.grpListParam.Location = new System.Drawing.Point(113, 458);
		this.grpListParam.Name = "grpListParam";
		this.grpListParam.Size = new System.Drawing.Size(517, 269);
		this.grpListParam.TabIndex = 24;
		this.grpListParam.TabStop = false;
		this.grpListParam.Text = "Scanparameter";
		this.chkTalkback.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.chkTalkback.AutoSize = true;
		this.chkTalkback.Location = new System.Drawing.Point(251, 13);
		this.chkTalkback.Name = "chkTalkback";
		this.chkTalkback.Size = new System.Drawing.Size(82, 20);
		this.chkTalkback.TabIndex = 6;
		this.chkTalkback.Text = "Talkback";
		this.chkTalkback.UseVisualStyleBackColor = true;
		this.chkChMark.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.chkChMark.AutoSize = true;
		this.chkChMark.Location = new System.Drawing.Point(251, 43);
		this.chkChMark.Name = "chkChMark";
		this.chkChMark.Size = new System.Drawing.Size(128, 20);
		this.chkChMark.TabIndex = 7;
		this.chkChMark.Text = "Channel Marker";
		this.chkChMark.UseVisualStyleBackColor = true;
		this.cmbPriorityCh2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.cmbPriorityCh2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbPriorityCh2.FormattingEnabled = true;
		this.cmbPriorityCh2.Location = new System.Drawing.Point(251, 224);
		this.cmbPriorityCh2.Name = "cmbPriorityCh2";
		this.cmbPriorityCh2.Size = new System.Drawing.Size(166, 24);
		this.cmbPriorityCh2.TabIndex = 19;
		this.cmbPriorityCh2.SelectedIndexChanged += new System.EventHandler(cmbPriorityCh2_SelectedIndexChanged);
		this.lblTxDesignatedCh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lblTxDesignatedCh.Location = new System.Drawing.Point(73, 73);
		this.lblTxDesignatedCh.Name = "lblTxDesignatedCh";
		this.lblTxDesignatedCh.Size = new System.Drawing.Size(167, 24);
		this.lblTxDesignatedCh.TabIndex = 8;
		this.lblTxDesignatedCh.Text = "Tx Designated Channel";
		this.lblTxDesignatedCh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.cmbPriorityCh1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.cmbPriorityCh1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbPriorityCh1.FormattingEnabled = true;
		this.cmbPriorityCh1.Location = new System.Drawing.Point(251, 194);
		this.cmbPriorityCh1.Name = "cmbPriorityCh1";
		this.cmbPriorityCh1.Size = new System.Drawing.Size(166, 24);
		this.cmbPriorityCh1.TabIndex = 17;
		this.cmbPriorityCh1.SelectedIndexChanged += new System.EventHandler(cmbPriorityCh1_SelectedIndexChanged);
		this.lblPriorityCh1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lblPriorityCh1.Location = new System.Drawing.Point(73, 193);
		this.lblPriorityCh1.Name = "lblPriorityCh1";
		this.lblPriorityCh1.Size = new System.Drawing.Size(167, 24);
		this.lblPriorityCh1.TabIndex = 16;
		this.lblPriorityCh1.Text = "Priority Channel 1";
		this.lblPriorityCh1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.lblSignalingHold.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lblSignalingHold.Location = new System.Drawing.Point(73, 133);
		this.lblSignalingHold.Name = "lblSignalingHold";
		this.lblSignalingHold.Size = new System.Drawing.Size(167, 24);
		this.lblSignalingHold.TabIndex = 12;
		this.lblSignalingHold.Text = "Signaling Hold Time [ms]";
		this.lblSignalingHold.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.label_0.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.label_0.Location = new System.Drawing.Point(73, 103);
		this.label_0.Name = "label_0";
		this.label_0.Size = new System.Drawing.Size(167, 24);
		this.label_0.TabIndex = 10;
		this.label_0.Text = "PL Type";
		this.label_0.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.nudPrioritySample.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.nudPrioritySample.Increment = new decimal(new int[4] { 250, 0, 0, 0 });
		this.nudPrioritySample.Location = new System.Drawing.Point(251, 164);
		this.nudPrioritySample.Maximum = new decimal(new int[4] { 7750, 0, 0, 0 });
		this.nudPrioritySample.Minimum = new decimal(new int[4] { 750, 0, 0, 0 });
		this.nudPrioritySample.Name = "nudPrioritySample";
		this.nudPrioritySample.Size = new System.Drawing.Size(166, 23);
		this.nudPrioritySample.TabIndex = 15;
		this.nudPrioritySample.Value = new decimal(new int[4] { 750, 0, 0, 0 });
		this.lblPriorityCh2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lblPriorityCh2.Location = new System.Drawing.Point(73, 223);
		this.lblPriorityCh2.Name = "lblPriorityCh2";
		this.lblPriorityCh2.Size = new System.Drawing.Size(167, 24);
		this.lblPriorityCh2.TabIndex = 18;
		this.lblPriorityCh2.Text = "Priority Channel 2";
		this.lblPriorityCh2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.nudSignalingHold.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.nudSignalingHold.Increment = new decimal(new int[4] { 25, 0, 0, 0 });
		this.nudSignalingHold.Location = new System.Drawing.Point(251, 134);
		this.nudSignalingHold.Maximum = new decimal(new int[4] { 6375, 0, 0, 0 });
		this.nudSignalingHold.Minimum = new decimal(new int[4] { 50, 0, 0, 0 });
		this.nudSignalingHold.Name = "nudSignalingHold";
		this.nudSignalingHold.Size = new System.Drawing.Size(166, 23);
		this.nudSignalingHold.TabIndex = 13;
		this.nudSignalingHold.Value = new decimal(new int[4] { 50, 0, 0, 0 });
		this.cmbTxDesignatedCh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.cmbTxDesignatedCh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbTxDesignatedCh.FormattingEnabled = true;
		this.cmbTxDesignatedCh.Location = new System.Drawing.Point(251, 74);
		this.cmbTxDesignatedCh.Name = "cmbTxDesignatedCh";
		this.cmbTxDesignatedCh.Size = new System.Drawing.Size(166, 24);
		this.cmbTxDesignatedCh.TabIndex = 9;
		this.cmbPlType.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.cmbPlType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbPlType.FormattingEnabled = true;
		this.cmbPlType.Items.AddRange(new object[4] { "Disabled", "Priority channel", "Non-priority channel", "Priority channel and non-priority channel" });
		this.cmbPlType.Location = new System.Drawing.Point(251, 104);
		this.cmbPlType.Name = "cmbPlType";
		this.cmbPlType.Size = new System.Drawing.Size(166, 24);
		this.cmbPlType.TabIndex = 11;
		this.lblPrioritySample.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lblPrioritySample.Location = new System.Drawing.Point(73, 163);
		this.lblPrioritySample.Name = "lblPrioritySample";
		this.lblPrioritySample.Size = new System.Drawing.Size(167, 24);
		this.lblPrioritySample.TabIndex = 14;
		this.lblPrioritySample.Text = "Priority Sample Time [ms]";
		this.lblPrioritySample.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnDown.Location = new System.Drawing.Point(665, 223);
		this.btnDown.Name = "btnDown";
		this.btnDown.Size = new System.Drawing.Size(75, 23);
		this.btnDown.TabIndex = 23;
		this.btnDown.Text = "Down";
		this.btnDown.UseVisualStyleBackColor = true;
		this.btnDown.Click += new System.EventHandler(btnDown_Click);
		this.btnUp.Location = new System.Drawing.Point(665, 171);
		this.btnUp.Name = "btnUp";
		this.btnUp.Size = new System.Drawing.Size(75, 23);
		this.btnUp.TabIndex = 22;
		this.btnUp.Text = "Up";
		this.btnUp.UseVisualStyleBackColor = true;
		this.btnUp.Click += new System.EventHandler(btnUp_Click);
		this.txtName.InputString = null;
		this.txtName.Location = new System.Drawing.Point(322, 22);
		this.txtName.MaxByteLength = 0;
		this.txtName.Name = "txtName";
		this.txtName.Size = new System.Drawing.Size(146, 23);
		this.txtName.TabIndex = 1;
		this.txtName.Leave += new System.EventHandler(txtName_Leave);
		this.grpSelected.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.grpSelected.Controls.Add(this.lstSelected);
		this.grpSelected.Location = new System.Drawing.Point(425, 59);
		this.grpSelected.Name = "grpSelected";
		this.grpSelected.Size = new System.Drawing.Size(205, 412);
		this.grpSelected.TabIndex = 21;
		this.grpSelected.TabStop = false;
		this.grpSelected.Text = "Member";
		this.lstSelected.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lstSelected.FormattingEnabled = true;
		this.lstSelected.ItemHeight = 16;
		this.lstSelected.Location = new System.Drawing.Point(30, 31);
		this.lstSelected.Name = "lstSelected";
		this.lstSelected.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
		this.lstSelected.Size = new System.Drawing.Size(150, 324);
		this.lstSelected.TabIndex = 5;
		this.lstSelected.SelectedIndexChanged += new System.EventHandler(lstSelected_SelectedIndexChanged);
		this.lstSelected.DoubleClick += new System.EventHandler(lstSelected_DoubleClick);
		this.grpUnselected.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.grpUnselected.Controls.Add(this.lstUnselected);
		this.grpUnselected.Location = new System.Drawing.Point(113, 59);
		this.grpUnselected.Name = "grpUnselected";
		this.grpUnselected.Size = new System.Drawing.Size(205, 412);
		this.grpUnselected.TabIndex = 2;
		this.grpUnselected.TabStop = false;
		this.grpUnselected.Text = "Available";
		this.lstUnselected.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lstUnselected.FormattingEnabled = true;
		this.lstUnselected.ItemHeight = 16;
		this.lstUnselected.Location = new System.Drawing.Point(32, 31);
		this.lstUnselected.Name = "lstUnselected";
		this.lstUnselected.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
		this.lstUnselected.Size = new System.Drawing.Size(150, 324);
		this.lstUnselected.TabIndex = 0;
		this.lblName.Location = new System.Drawing.Point(258, 22);
		this.lblName.Name = "lblName";
		this.lblName.Size = new System.Drawing.Size(60, 24);
		this.lblName.TabIndex = 0;
		this.lblName.Text = "Name";
		this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnDel.Location = new System.Drawing.Point(333, 223);
		this.btnDel.Name = "btnDel";
		this.btnDel.Size = new System.Drawing.Size(75, 23);
		this.btnDel.TabIndex = 4;
		this.btnDel.Text = "Delete";
		this.btnDel.UseVisualStyleBackColor = true;
		this.btnDel.Click += new System.EventHandler(btnDel_Click);
		this.btnAdd.Location = new System.Drawing.Point(333, 171);
		this.btnAdd.Name = "btnAdd";
		this.btnAdd.Size = new System.Drawing.Size(75, 23);
		this.btnAdd.TabIndex = 3;
		this.btnAdd.Text = "Add";
		this.btnAdd.UseVisualStyleBackColor = true;
		this.btnAdd.Click += new System.EventHandler(btnAdd_Click);
		base.ClientSize = new System.Drawing.Size(760, 744);
		base.Controls.Add(this.panel1);
		this.Font = new System.Drawing.Font("Arial", 10f);
		base.Name = "NormalScanForm";
		this.Text = "Normal Scan";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(NormalScanForm_FormClosing);
		base.Load += new System.EventHandler(NormalScanForm_Load);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		this.grpListParam.ResumeLayout(false);
		this.grpListParam.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.nudPrioritySample).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudSignalingHold).EndInit();
		this.grpSelected.ResumeLayout(false);
		this.grpUnselected.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	static NormalScanForm()
	{
		SZ_PRIORITY_CH = new string[2] { "None", "Selected" };
		SZ_TX_DESIGNATED_CH = new string[2] { "Last Active Channel", "Selected" };
		SZ_PL_TYPE = new string[4] { "Non-Priority Channel", "Disable", "Priority Channel", "Priority and Non-Priority Channel" };
		data = new NormalScan();
	}
}
