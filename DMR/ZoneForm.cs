using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DMR;

public class ZoneForm : DockContent, IDisp
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ZoneOne
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		private byte[] name;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
		private ushort[] chList;

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

		public ushort[] ChList
		{
			get
			{
				return chList;
			}
			set
			{
				chList.Fill(ushort.MaxValue);
				Array.Copy(value, 0, chList, 0, value.Length);
			}
		}

		public ZoneOne(int index)
		{
			this = default(ZoneOne);
			name = new byte[16];
			chList = new ushort[80];
		}

		public ZoneOne(ZoneOne zone)
		{
			this = default(ZoneOne);
			name = new byte[16];
			chList = new ushort[80];
			Array.Copy(zone.name, name, 16);
			Array.Copy(zone.chList, chList, 80);
			for (int i = 0; i < chList.Length && chList[i] != ushort.MaxValue; i++)
			{
				chList[i] &= 16383;
			}
		}

		public void UpdateZonesDataForChannelMoveUpDown(int channel1, int channel2)
		{
			channel1++;
			channel2++;
			for (int i = 0; i < 80; i++)
			{
				ushort num = chList[i];
				if (num == channel1)
				{
					chList[i] = (ushort)channel2;
				}
				else if (num == channel2)
				{
					chList[i] = (ushort)channel1;
				}
			}
		}

		public byte[] ToEerom()
		{
			int num = 0;
			byte[] array = new byte[96];
			array.Fill((byte)0);
			Array.Copy(name, array, 16);
			ushort[] array2 = chList;
			for (int i = 0; i < array2.Length; i++)
			{
				array[16 + num] = (byte)chList[num];
			}
			return array;
		}

		public void Verify()
		{
			List<ushort> list = new List<ushort>(chList).FindAll((ushort ushort_0) => ushort_0 != 0 && ChannelForm.data.DataIsValid(ushort_0 - 1));
			while (list.Count < chList.Length)
			{
				list.Add(0);
			}
			chList = list.ToArray();
		}

		public bool AddChannelToZone(ushort channel)
		{
			channel++;
			if (Array.FindIndex(chList, (ushort item) => item == channel) != -1)
			{
				return true;
			}
			int num = Array.FindIndex(chList, (ushort item) => item == 0 || item == ushort.MaxValue);
			if (num == -1)
			{
				return false;
			}
			chList[num] = channel;
			return true;
		}

		[CompilerGenerated]
		private static bool smethod_0(ushort ushort_0)
		{
			if (ushort_0 != 0)
			{
				return ChannelForm.data.DataIsValid(ushort_0 - 1);
			}
			return false;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class Zone : IData
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		private byte[] zoneIndex;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 68)]
		private ZoneOne[] zoneList;

		public byte[] ZoneIndex => zoneIndex;

		public ZoneOne[] ZoneList => zoneList;

		public ZoneOne this[int index]
		{
			get
			{
				if (index >= Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				return zoneList[index];
			}
			set
			{
				if (index >= Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				zoneList[index] = value;
			}
		}

		public int Count => 68;

		public int ValidCount
		{
			get
			{
				int num = 0;
				int num2 = 0;
				BitArray bitArray = new BitArray(zoneIndex);
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

		public string Format => "Zone{0}";

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

		public int FstZoneFstCh => zoneList[0].ChList[0];

		public Zone()
		{
			int num = 0;
			zoneIndex = new byte[32];
			zoneList = new ZoneOne[68];
			for (num = 0; num < zoneList.Length; num++)
			{
				zoneList[num] = new ZoneOne(num);
			}
		}

		public int GetDispIndex(int index)
		{
			int num = 0;
			int num2 = 0;
			BitArray bitArray = new BitArray(zoneIndex);
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
			BitArray bitArray = new BitArray(zoneIndex);
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

		public int GetMinIndexIncludeCh()
		{
			int num = 0;
			BitArray bitArray = new BitArray(zoneIndex);
			num = 0;
			while (true)
			{
				if (num < Count)
				{
					if (bitArray[num] && zoneList[num].ChList[0] != 0)
					{
						break;
					}
					num++;
					continue;
				}
				return GetMinIndex();
			}
			return num;
		}

		public int GetMinValidIndex()
		{
			int num = 0;
			num = 0;
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
				return 0;
			}
			return num;
		}

		public bool DataIsValid(int index)
		{
			if (index < 68)
			{
				return new BitArray(zoneIndex)[index];
			}
			return false;
		}

		public bool ZoneChIsValid(int index)
		{
			if (index < 68 && new BitArray(zoneIndex)[index] && zoneList[index].ChList[0] != 0)
			{
				return true;
			}
			return false;
		}

		public void SetIndex(int index, int value)
		{
			int num = index / 8;
			int num2 = index % 8;
			if (Convert.ToBoolean(value))
			{
				zoneIndex[num] |= (byte)(1 << num2);
				return;
			}
			zoneIndex[num] &= (byte)(~(1 << num2));
			OpenGD77Form.ClearLastUsedChannelsData();
		}

		public void ClearIndex(int index)
		{
			int num = index / 8;
			int num2 = index % 8;
			zoneIndex[num] &= (byte)(~(1 << num2));
			OpenGD77Form.ClearLastUsedChannelsData();
		}

		public void ClearByData(int chIndex)
		{
			int num = 0;
			int num2 = 0;
			basicData.ClearByData(chIndex);
			for (num = 0; num < Count; num++)
			{
				if (DataIsValid(num))
				{
					num2 = Array.IndexOf(zoneList[num].ChList, (ushort)(chIndex + 1));
					if (num2 >= 0)
					{
						zoneList[num].ChList.RemoveItemFromArray(num2);
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
			zoneList[index].Name = text;
		}

		public string GetName(int index)
		{
			return zoneList[index].Name;
		}

		public void Default(int index)
		{
			zoneList[index].ChList.Fill((ushort)0);
		}

		public void Paste(int from, int to)
		{
			Array.Copy(zoneList[from].ChList, 0, zoneList[to].ChList, 0, zoneList[to].ChList.Length);
		}

		public byte[] ToEerom(int index, int length)
		{
			int num = 0;
			byte[] array = new byte[96 * length];
			for (num = 0; num < length; num++)
			{
				byte[] array2 = ZoneList[index + num].ToEerom();
				Array.Copy(array2, 0, array, num * array2.Length, array2.Length);
			}
			return array;
		}

		public bool ZoneChIsValid(int _zoneIndex, int _zoneChIndex)
		{
			try
			{
				int ch = ZoneList[_zoneIndex].ChList[_zoneChIndex];
				return ChIsValid(ch);
			}
			catch
			{
				return false;
			}
		}

		public bool ChIsValid(int ch)
		{
			if (ch >= 1)
			{
				return ch <= ChannelForm.CurCntCh;
			}
			return false;
		}

		public int GetZoneChCnt(int _zoneIndex)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (num = 0; num < ZoneList[_zoneIndex].ChList.Length; num++)
			{
				num2 = ZoneList[_zoneIndex].ChList[num];
				if (ChIsValid(num2))
				{
					num3++;
				}
			}
			return num3;
		}

		public void Verify()
		{
			int num = 0;
			for (num = 0; num < Count; num++)
			{
				if (DataIsValid(num))
				{
					zoneList[num].Verify();
				}
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class BasicZone
	{
		private ushort curZone;

		private ushort mainCh;

		private ushort subCh;

		private ushort subZone;

		public int CurZone
		{
			get
			{
				if (curZone < 68)
				{
					if (data.ZoneChIsValid(curZone))
					{
						return curZone;
					}
					return 0;
				}
				return 0;
			}
			set
			{
				if (value < 68)
				{
					curZone = (ushort)value;
				}
			}
		}

		public int MainCh
		{
			get
			{
				if (mainCh < ChannelForm.CurCntCh)
				{
					return mainCh;
				}
				return 0;
			}
			set
			{
				if (value < ChannelForm.CurCntCh)
				{
					mainCh = (ushort)value;
				}
			}
		}

		public int SubCh
		{
			get
			{
				if (subCh < ChannelForm.CurCntCh)
				{
					return subCh;
				}
				return 0;
			}
			set
			{
				if (value < ChannelForm.CurCntCh)
				{
					subCh = (ushort)value;
				}
			}
		}

		public int SubZone
		{
			get
			{
				if (subZone < 68)
				{
					if (data.ZoneChIsValid(subZone))
					{
						return subZone;
					}
					return 0;
				}
				return 0;
			}
			set
			{
				if (value < 68)
				{
					subZone = (ushort)value;
				}
			}
		}

		public byte[] ToEerom()
		{
			return new byte[4]
			{
				(byte)CurZone,
				(byte)MainCh,
				(byte)SubCh,
				(byte)SubZone
			};
		}

		public void FromEerom(byte[] data)
		{
			if (data.Length >= 4)
			{
				CurZone = data[0];
				MainCh = data[1];
				SubCh = data[2];
				SubZone = data[3];
			}
		}

		public void ClearByData(int index)
		{
			int num = 0;
			int num2 = index + 1;
			num = Array.IndexOf(data.ZoneList[CurZone].ChList, (ushort)num2);
			if (num >= 0)
			{
				if (data.GetZoneChCnt(CurZone) == 1)
				{
					CurZone = data.GetMinIndexIncludeCh();
					MainCh = 0;
				}
				else if (num == MainCh)
				{
					MainCh = 0;
				}
				else
				{
					MainCh = Math.Max(0, MainCh - 1);
				}
			}
			num = Array.IndexOf(data.ZoneList[SubZone].ChList, (ushort)num2);
			if (num >= 0)
			{
				if (data.GetZoneChCnt(SubZone) == 1)
				{
					SubZone = data.GetMinIndexIncludeCh();
					SubCh = 0;
				}
				else if (num == SubCh)
				{
					SubCh = 0;
				}
				else
				{
					SubCh = Math.Max(0, SubCh - 1);
				}
			}
		}

		public void Verify()
		{
			if (data.ZoneChIsValid(CurZone))
			{
				if (data.ZoneList[CurZone].ChList[MainCh] == 0)
				{
					MainCh = 0;
				}
			}
			else
			{
				CurZone = data.GetMinIndexIncludeCh();
				MainCh = 0;
			}
			if (data.ZoneChIsValid(SubZone))
			{
				if (data.ZoneList[SubZone].ChList[SubCh] == 0)
				{
					SubCh = 0;
				}
			}
			else
			{
				SubZone = data.GetMinIndexIncludeCh();
				SubCh = 0;
			}
		}
	}

	private const int ZONE_NAME_LENGTH = 16;

	private const int ZONES_IN_USE_DATA_LENGTH = 32;

	public const int NUM_CHANNELS_PER_ZONE = 80;

	public const int NUM_ZONES = 68;

	private const int UNKNOWN_VAR_OF_32 = 96;

	private Button btnDel;

	private Button btnAdd;

	private ListBox lstSelected;

	private ListBox lstUnselected;

	private Label lblName;

	private SGTextBox txtName;

	private GroupBox grpUnselected;

	private GroupBox grpSelected;

	private Button btnDown;

	private Button btnUp;

	private ToolStrip tsrZone;

	private ToolStripButton tsbtnFirst;

	private ToolStripButton tsbtnPrev;

	private ToolStripButton tsbtnNext;

	private ToolStripButton tsbtnLast;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripButton tsbtnAdd;

	private ToolStripButton tsbtnDel;

	private MenuStrip mnsZone;

	private ToolStripMenuItem tsmiCh;

	private ToolStripMenuItem tsmiFirst;

	private ToolStripMenuItem tsmiPrev;

	private ToolStripMenuItem tsmiNext;

	private ToolStripMenuItem tsmiLast;

	private ToolStripMenuItem tsmiAdd;

	private ToolStripMenuItem tsmiDel;

	private ToolStripSeparator toolStripSeparator2;

	private ToolStripLabel tslblInfo;

	private CustomPanel pnlZone;

	public static readonly int SPACE_ZONE;

	public static BasicZone basicData;

	public static Zone data;

	public static readonly int SPACE_BASIC_ZONE;

	private ComponentResourceManager componentResourceManager;

	public static int MainChIndex { get; set; }

	public static int SubChIndex { get; set; }

	public TreeNode Node { get; set; }

	public static void SwapZones(int zoneId1, int zoneId2)
	{
	}

	public static void UpdateZonesDataForChannelMoveUpDown(int channel1, int channel2)
	{
		for (int i = 0; i < 68; i++)
		{
			if (data.DataIsValid(i))
			{
				data.ZoneList[i].UpdateZonesDataForChannelMoveUpDown(channel1, channel2);
			}
		}
		OpenGD77Form.ClearLastUsedChannelsData();
	}

	public static void MoveZoneDown(int zoneIndex)
	{
		if (data.DataIsValid(zoneIndex + 1))
		{
			ZoneOne zoneOne = data.ZoneList[zoneIndex + 1];
			data.ZoneList[zoneIndex + 1] = data.ZoneList[zoneIndex];
			data.ZoneList[zoneIndex] = zoneOne;
			OpenGD77Form.ClearLastUsedChannelsData();
		}
	}

	public static void MoveZoneUp(int zoneIndex)
	{
		if (zoneIndex > 0)
		{
			ZoneOne zoneOne = data.ZoneList[zoneIndex - 1];
			data.ZoneList[zoneIndex - 1] = data.ZoneList[zoneIndex];
			data.ZoneList[zoneIndex] = zoneOne;
			OpenGD77Form.ClearLastUsedChannelsData();
		}
	}

	public static void copyZonesDownwards(int sourceZoneNum, int destZoneNum)
	{
		for (int i = sourceZoneNum; i < data.ZoneList.Length; i++)
		{
			ZoneOne zone = data.ZoneList[i];
			bool num = data.DataIsValid(i);
			data.ZoneList[destZoneNum] = new ZoneOne(zone);
			if (num)
			{
				data.SetIndex(destZoneNum, 1);
			}
			else
			{
				data.ClearIndex(destZoneNum);
			}
			destZoneNum++;
		}
	}

	public static void CompactZones()
	{
		int num = -1;
		for (int i = 0; i < data.ZoneList.Length; i++)
		{
			if (num == -1 && !data.DataIsValid(i))
			{
				num = i;
			}
			else if (num != -1 && data.DataIsValid(i))
			{
				copyZonesDownwards(i, num);
				num = -1;
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.pnlZone = new CustomPanel();
		this.tsrZone = new System.Windows.Forms.ToolStrip();
		this.tslblInfo = new System.Windows.Forms.ToolStripLabel();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.tsbtnFirst = new System.Windows.Forms.ToolStripButton();
		this.tsbtnPrev = new System.Windows.Forms.ToolStripButton();
		this.tsbtnNext = new System.Windows.Forms.ToolStripButton();
		this.tsbtnLast = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.tsbtnAdd = new System.Windows.Forms.ToolStripButton();
		this.tsbtnDel = new System.Windows.Forms.ToolStripButton();
		this.mnsZone = new System.Windows.Forms.MenuStrip();
		this.tsmiCh = new System.Windows.Forms.ToolStripMenuItem();
		this.tsmiFirst = new System.Windows.Forms.ToolStripMenuItem();
		this.tsmiPrev = new System.Windows.Forms.ToolStripMenuItem();
		this.tsmiNext = new System.Windows.Forms.ToolStripMenuItem();
		this.tsmiLast = new System.Windows.Forms.ToolStripMenuItem();
		this.tsmiAdd = new System.Windows.Forms.ToolStripMenuItem();
		this.tsmiDel = new System.Windows.Forms.ToolStripMenuItem();
		this.btnDown = new System.Windows.Forms.Button();
		this.btnUp = new System.Windows.Forms.Button();
		this.txtName = new DMR.SGTextBox();
		this.grpSelected = new System.Windows.Forms.GroupBox();
		this.lstSelected = new System.Windows.Forms.ListBox();
		this.btnAdd = new System.Windows.Forms.Button();
		this.grpUnselected = new System.Windows.Forms.GroupBox();
		this.lstUnselected = new System.Windows.Forms.ListBox();
		this.btnDel = new System.Windows.Forms.Button();
		this.lblName = new System.Windows.Forms.Label();
		this.pnlZone.SuspendLayout();
		this.tsrZone.SuspendLayout();
		this.mnsZone.SuspendLayout();
		this.grpSelected.SuspendLayout();
		this.grpUnselected.SuspendLayout();
		base.SuspendLayout();
		this.pnlZone.AutoScroll = true;
		this.pnlZone.AutoSize = true;
		this.pnlZone.Controls.Add(this.tsrZone);
		this.pnlZone.Controls.Add(this.mnsZone);
		this.pnlZone.Controls.Add(this.btnDown);
		this.pnlZone.Controls.Add(this.btnUp);
		this.pnlZone.Controls.Add(this.txtName);
		this.pnlZone.Controls.Add(this.grpSelected);
		this.pnlZone.Controls.Add(this.btnAdd);
		this.pnlZone.Controls.Add(this.grpUnselected);
		this.pnlZone.Controls.Add(this.btnDel);
		this.pnlZone.Controls.Add(this.lblName);
		this.pnlZone.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnlZone.Location = new System.Drawing.Point(0, 0);
		this.pnlZone.Name = "pnlZone";
		this.pnlZone.Size = new System.Drawing.Size(794, 560);
		this.pnlZone.TabIndex = 8;
		this.tsrZone.Items.AddRange(new System.Windows.Forms.ToolStripItem[9] { this.tslblInfo, this.toolStripSeparator2, this.tsbtnFirst, this.tsbtnPrev, this.tsbtnNext, this.tsbtnLast, this.toolStripSeparator1, this.tsbtnAdd, this.tsbtnDel });
		this.tsrZone.Location = new System.Drawing.Point(0, 0);
		this.tsrZone.Name = "tsrZone";
		this.tsrZone.Size = new System.Drawing.Size(794, 25);
		this.tsrZone.TabIndex = 33;
		this.tsrZone.Text = "toolStrip1";
		this.tslblInfo.AutoSize = false;
		this.tslblInfo.Name = "tslblInfo";
		this.tslblInfo.Size = new System.Drawing.Size(100, 52);
		this.tslblInfo.Text = " 0 / 0";
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
		this.tsbtnFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbtnFirst.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.tsbtnFirst.Name = "tsbtnFirst";
		this.tsbtnFirst.Size = new System.Drawing.Size(23, 22);
		this.tsbtnFirst.Text = "First";
		this.tsbtnFirst.Click += new System.EventHandler(tsmiFirst_Click);
		this.tsbtnPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbtnPrev.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.tsbtnPrev.Name = "tsbtnPrev";
		this.tsbtnPrev.Size = new System.Drawing.Size(23, 22);
		this.tsbtnPrev.Text = "Previous";
		this.tsbtnPrev.Click += new System.EventHandler(tsmiPrev_Click);
		this.tsbtnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbtnNext.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.tsbtnNext.Name = "tsbtnNext";
		this.tsbtnNext.Size = new System.Drawing.Size(23, 22);
		this.tsbtnNext.Text = "Next";
		this.tsbtnNext.Click += new System.EventHandler(tsmiNext_Click);
		this.tsbtnLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbtnLast.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.tsbtnLast.Name = "tsbtnLast";
		this.tsbtnLast.Size = new System.Drawing.Size(23, 22);
		this.tsbtnLast.Text = "Last";
		this.tsbtnLast.Click += new System.EventHandler(tsmiLast_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
		this.tsbtnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbtnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.tsbtnAdd.Name = "tsbtnAdd";
		this.tsbtnAdd.Size = new System.Drawing.Size(23, 22);
		this.tsbtnAdd.Text = "Add..";
		this.tsbtnAdd.Click += new System.EventHandler(tsmiAdd_Click);
		this.tsbtnDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.tsbtnDel.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.tsbtnDel.Name = "tsbtnDel";
		this.tsbtnDel.Size = new System.Drawing.Size(23, 22);
		this.tsbtnDel.Text = "Delete";
		this.tsbtnDel.Click += new System.EventHandler(tsmiDel_Click);
		this.mnsZone.AllowMerge = false;
		this.mnsZone.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.tsmiCh });
		this.mnsZone.Location = new System.Drawing.Point(0, 0);
		this.mnsZone.Name = "mnsZone";
		this.mnsZone.Size = new System.Drawing.Size(794, 25);
		this.mnsZone.TabIndex = 34;
		this.mnsZone.Text = "menuStrip1";
		this.mnsZone.Visible = false;
		this.tsmiCh.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.tsmiFirst, this.tsmiPrev, this.tsmiNext, this.tsmiLast, this.tsmiAdd, this.tsmiDel });
		this.tsmiCh.Name = "tsmiCh";
		this.tsmiCh.Size = new System.Drawing.Size(79, 21);
		this.tsmiCh.Text = "Operation";
		this.tsmiFirst.Name = "tsmiFirst";
		this.tsmiFirst.Size = new System.Drawing.Size(159, 22);
		this.tsmiFirst.Text = "Fist";
		this.tsmiFirst.Click += new System.EventHandler(tsmiFirst_Click);
		this.tsmiPrev.Name = "tsmiPrev";
		this.tsmiPrev.Size = new System.Drawing.Size(159, 22);
		this.tsmiPrev.Text = "Previous";
		this.tsmiPrev.Click += new System.EventHandler(tsmiPrev_Click);
		this.tsmiNext.Name = "tsmiNext";
		this.tsmiNext.Size = new System.Drawing.Size(159, 22);
		this.tsmiNext.Text = "Next";
		this.tsmiNext.Click += new System.EventHandler(tsmiNext_Click);
		this.tsmiNext.ShortcutKeys = System.Windows.Forms.Keys.N | System.Windows.Forms.Keys.Control;
		this.tsmiLast.Name = "tsmiLast";
		this.tsmiLast.Size = new System.Drawing.Size(159, 22);
		this.tsmiLast.Text = "Last";
		this.tsmiLast.Click += new System.EventHandler(tsmiLast_Click);
		this.tsmiAdd.Name = "tsmiAdd";
		this.tsmiAdd.Size = new System.Drawing.Size(159, 22);
		this.tsmiAdd.Text = "Add";
		this.tsmiAdd.Click += new System.EventHandler(tsmiAdd_Click);
		this.tsmiDel.Name = "tsmiDel";
		this.tsmiDel.Size = new System.Drawing.Size(159, 22);
		this.tsmiDel.Text = "Delete";
		this.tsmiDel.Click += new System.EventHandler(tsmiDel_Click);
		this.btnDown.Location = new System.Drawing.Point(676, 310);
		this.btnDown.Name = "btnDown";
		this.btnDown.Size = new System.Drawing.Size(75, 23);
		this.btnDown.TabIndex = 11;
		this.btnDown.Text = "Down";
		this.btnDown.UseVisualStyleBackColor = true;
		this.btnDown.Click += new System.EventHandler(btnDown_Click);
		this.btnUp.Location = new System.Drawing.Point(676, 258);
		this.btnUp.Name = "btnUp";
		this.btnUp.Size = new System.Drawing.Size(75, 23);
		this.btnUp.TabIndex = 10;
		this.btnUp.Text = "Up";
		this.btnUp.UseVisualStyleBackColor = true;
		this.btnUp.Click += new System.EventHandler(btnUp_Click);
		this.txtName.InputString = null;
		this.txtName.Location = new System.Drawing.Point(316, 62);
		this.txtName.MaxByteLength = 0;
		this.txtName.Name = "txtName";
		this.txtName.Size = new System.Drawing.Size(150, 23);
		this.txtName.TabIndex = 1;
		this.txtName.Leave += new System.EventHandler(txtName_Leave);
		this.grpSelected.Controls.Add(this.lstSelected);
		this.grpSelected.Location = new System.Drawing.Point(419, 110);
		this.grpSelected.Name = "grpSelected";
		this.grpSelected.Size = new System.Drawing.Size(215, 388);
		this.grpSelected.TabIndex = 7;
		this.grpSelected.TabStop = false;
		this.grpSelected.Text = "Member";
		this.lstSelected.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lstSelected.FormattingEnabled = true;
		this.lstSelected.ItemHeight = 16;
		this.lstSelected.Location = new System.Drawing.Point(27, 37);
		this.lstSelected.Name = "lstSelected";
		this.lstSelected.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
		this.lstSelected.Size = new System.Drawing.Size(160, 324);
		this.lstSelected.TabIndex = 5;
		this.lstSelected.SelectedIndexChanged += new System.EventHandler(lstSelected_SelectedIndexChanged);
		this.lstSelected.DoubleClick += new System.EventHandler(lstSelected_DoubleClick);
		this.btnAdd.Location = new System.Drawing.Point(327, 258);
		this.btnAdd.Name = "btnAdd";
		this.btnAdd.Size = new System.Drawing.Size(75, 23);
		this.btnAdd.TabIndex = 3;
		this.btnAdd.Text = "Add";
		this.btnAdd.UseVisualStyleBackColor = true;
		this.btnAdd.Click += new System.EventHandler(btnAdd_Click);
		this.grpUnselected.Controls.Add(this.lstUnselected);
		this.grpUnselected.Location = new System.Drawing.Point(86, 110);
		this.grpUnselected.Name = "grpUnselected";
		this.grpUnselected.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.grpUnselected.Size = new System.Drawing.Size(215, 388);
		this.grpUnselected.TabIndex = 6;
		this.grpUnselected.TabStop = false;
		this.grpUnselected.Text = "Available";
		this.lstUnselected.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lstUnselected.FormattingEnabled = true;
		this.lstUnselected.ItemHeight = 16;
		this.lstUnselected.Location = new System.Drawing.Point(32, 37);
		this.lstUnselected.Name = "lstUnselected";
		this.lstUnselected.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
		this.lstUnselected.Size = new System.Drawing.Size(160, 324);
		this.lstUnselected.TabIndex = 2;
		this.btnDel.Location = new System.Drawing.Point(327, 310);
		this.btnDel.Name = "btnDel";
		this.btnDel.Size = new System.Drawing.Size(75, 23);
		this.btnDel.TabIndex = 4;
		this.btnDel.Text = "Delete";
		this.btnDel.UseVisualStyleBackColor = true;
		this.btnDel.Click += new System.EventHandler(btnDel_Click);
		this.lblName.Location = new System.Drawing.Point(216, 63);
		this.lblName.Name = "lblName";
		this.lblName.Size = new System.Drawing.Size(90, 23);
		this.lblName.TabIndex = 0;
		this.lblName.Text = "Name";
		this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.ClientSize = new System.Drawing.Size(794, 560);
		base.Controls.Add(this.pnlZone);
		this.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Regular);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ZoneForm";
		this.Text = "Zone";
		base.Load += new System.EventHandler(ZoneForm_Load);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ZoneForm_FormClosing);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		this.pnlZone.ResumeLayout(false);
		this.pnlZone.PerformLayout();
		this.tsrZone.ResumeLayout(false);
		this.tsrZone.PerformLayout();
		this.mnsZone.ResumeLayout(false);
		this.mnsZone.PerformLayout();
		this.grpSelected.ResumeLayout(false);
		this.grpUnselected.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

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
			ZoneOne value = new ZoneOne(num2);
			if (txtName.Focused)
			{
				txtName_Leave(txtName, null);
			}
			value.Name = txtName.Text;
			ushort[] array = new ushort[lstSelected.Items.Count];
			foreach (SelectedItemUtils item in lstSelected.Items)
			{
				array[num++] = (ushort)item.Value;
			}
			value.ChList = array;
			data[num2] = value;
			if (basicData.CurZone == num2 || basicData.SubZone == num2)
			{
				RefreshCh();
				basicData.Verify();
				((MainForm)base.MdiParent).RefreshForm(typeof(ZoneBasicForm));
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void RefreshCh()
	{
		int num = Convert.ToInt32(base.Tag);
		int num2 = 0;
		if (basicData.CurZone == num)
		{
			num2 = Array.IndexOf(data.ZoneList[num].ChList, (ushort)MainChIndex);
			if (num2 >= 0)
			{
				basicData.MainCh = num2;
			}
		}
		if (basicData.SubZone == num)
		{
			num2 = Array.IndexOf(data.ZoneList[num].ChList, (ushort)SubChIndex);
			if (num2 >= 0)
			{
				basicData.SubCh = num2;
			}
		}
	}

	public void DispData()
	{
		int num = 0;
		int num2 = 0;
		string text = "";
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		try
		{
			num3 = Convert.ToInt32(base.Tag);
			if (num3 == -1)
			{
				Close();
				return;
			}
			ZoneOne zoneOne = data[num3];
			txtName.Text = zoneOne.Name;
			lstSelected.Items.Clear();
			for (num = 0; num < zoneOne.ChList.Length; num++)
			{
				num2 = zoneOne.ChList[num];
				if (num2 == 0)
				{
					break;
				}
				int num6 = (num2 &= 0x7FF);
				if (num2 != 65535 && num6 <= 1024 && ChannelForm.data.DataIsValid(num6 - 1))
				{
					text = ChannelForm.data.GetName(num6 - 1);
					lstSelected.Items.Add(new SelectedItemUtils(num, num6, text));
				}
			}
			if (lstSelected.Items.Count > 0)
			{
				lstSelected.SelectedIndex = 0;
			}
			lstUnselected.Items.Clear();
			for (num = 0; num < 1024; num++)
			{
				if (!zoneOne.ChList.Contains((ushort)(num + 1)) && ChannelForm.data.DataIsValid(num))
				{
					num2 = num + 1;
					text = ChannelForm.data.GetName(num);
					lstUnselected.Items.Add(new SelectedItemUtils(-1, num2, text));
				}
			}
			if (lstUnselected.Items.Count > 0)
			{
				lstUnselected.SelectedIndex = 0;
			}
			method_5();
			method_6();
			num4 = basicData.CurZone;
			num5 = basicData.MainCh;
			MainChIndex = data.ZoneList[num4].ChList[num5];
			num4 = basicData.SubZone;
			num5 = basicData.SubCh;
			SubChIndex = data.ZoneList[num4].ChList[num5];
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void RefreshName()
	{
		int index = Convert.ToInt32(base.Tag);
		txtName.Text = data[index].Name;
	}

	public ZoneForm()
	{
		componentResourceManager = new ComponentResourceManager(typeof(ZoneForm));
		InitializeComponent();
		tsbtnDel.Image = (Image)componentResourceManager.GetObject("tsbtnDel.Image");
		tsbtnFirst.Image = (Image)componentResourceManager.GetObject("tsbtnFirst.Image");
		tsbtnLast.Image = (Image)componentResourceManager.GetObject("tsbtnLast.Image");
		tsbtnPrev.Image = (Image)componentResourceManager.GetObject("tsbtnPrev.Image");
		tsbtnAdd.Image = (Image)componentResourceManager.GetObject("tsbtnAdd.Image");
		tsbtnNext.Image = (Image)componentResourceManager.GetObject("tsbtnNext.Image");
		base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Scale(Settings.smethod_6());
	}

	private void method_1()
	{
		txtName.MaxByteLength = 15;
		txtName.KeyPress += Settings.smethod_54;
	}

	private void ZoneForm_Load(object sender, EventArgs e)
	{
		Settings.smethod_59(base.Controls);
		Settings.UpdateComponentTextsFromLanguageXmlData(this);
		Settings.UpdateToolStripFromLanguageXmlData(tsrZone.smethod_10(), base.Name);
		method_1();
		DispData();
	}

	private void ZoneForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveData();
	}

	private void btnAdd_Click(object sender, EventArgs e)
	{
		int num = 0;
		int count = lstUnselected.SelectedIndices.Count;
		int num2 = lstUnselected.SelectedIndices[count - 1];
		lstSelected.SelectedItems.Clear();
		while (lstUnselected.SelectedItems.Count > 0 && lstSelected.Items.Count < 80)
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
		method_4();
		method_5();
		if (!btnAdd.Enabled)
		{
			lstSelected.Focus();
		}
		OpenGD77Form.ClearLastUsedChannelsData();
	}

	private void btnDel_Click(object sender, EventArgs e)
	{
		int num = 0;
		int count = lstSelected.SelectedIndices.Count;
		int num2 = lstSelected.SelectedIndices[count - 1];
		lstUnselected.SelectedItems.Clear();
		while (lstSelected.SelectedItems.Count > 0)
		{
			SelectedItemUtils selectedItemUtils = (SelectedItemUtils)lstSelected.SelectedItems[0];
			num = method_3(selectedItemUtils);
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
		method_4();
		method_5();
		OpenGD77Form.ClearLastUsedChannelsData();
	}

	private bool method_2()
	{
		if (Convert.ToInt32(base.Tag) == 0)
		{
			return lstSelected.SelectedIndices.Contains(0);
		}
		return false;
	}

	private void btnUp_Click(object sender = null, EventArgs e = null)
	{
		int num = 0;
		int num2 = 0;
		int count = lstSelected.SelectedIndices.Count;
		_ = lstSelected.SelectedIndices[count - 1];
		for (num = 0; num < count; num++)
		{
			num2 = lstSelected.SelectedIndices[num];
			if (num != num2)
			{
				object value = lstSelected.Items[num2];
				lstSelected.Items[num2] = lstSelected.Items[num2 - 1];
				lstSelected.Items[num2 - 1] = value;
				lstSelected.SetSelected(num2, value: false);
				lstSelected.SetSelected(num2 - 1, value: true);
			}
		}
		method_4();
		OpenGD77Form.ClearLastUsedChannelsData();
	}

	private void btnDown_Click(object sender = null, EventArgs e = null)
	{
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
		method_4();
		OpenGD77Form.ClearLastUsedChannelsData();
	}

	private int method_3(SelectedItemUtils class14_0)
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

	private void method_4()
	{
		int num = 0;
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
	}

	private void method_5()
	{
		int num = Convert.ToInt32(base.Tag);
		btnAdd.Enabled = lstUnselected.Items.Count > 0 && lstSelected.Items.Count < 80;
		if (num == 0 && lstSelected.SelectedIndices.Contains(0))
		{
			btnDel.Enabled = false;
		}
		else
		{
			btnDel.Enabled = lstSelected.Items.Count > 0;
		}
		int count = lstSelected.Items.Count;
		int count2 = lstSelected.SelectedIndices.Count;
		btnUp.Enabled = lstSelected.SelectedItems.Count > 0 && lstSelected.Items.Count > 0 && lstSelected.SelectedIndices[count2 - 1] != count2 - 1;
		btnDown.Enabled = lstSelected.Items.Count > 0 && lstSelected.SelectedItems.Count > 0 && lstSelected.SelectedIndices[0] != count - count2;
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

	private void lstSelected_SelectedIndexChanged(object sender, EventArgs e)
	{
		method_5();
	}

	private void lstSelected_DoubleClick(object sender, EventArgs e)
	{
		if (lstSelected.SelectedItem != null)
		{
			SelectedItemUtils selectedItemUtils = lstSelected.SelectedItem as SelectedItemUtils;
			if (base.MdiParent is MainForm mainForm)
			{
				mainForm.DispChildForm(typeof(ChannelForm), selectedItemUtils.Value - 1);
			}
		}
	}

	private void tsmiFirst_Click(object sender, EventArgs e)
	{
		SaveData();
		Node = Node.Parent.FirstNode;
		TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
		base.Tag = treeNodeItem.Index;
		DispData();
	}

	private void handlePreviousClick()
	{
		SaveData();
		if (Node.PrevNode != null)
		{
			Node = Node.PrevNode;
			TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
			base.Tag = treeNodeItem.Index;
			DispData();
		}
	}

	private void tsmiPrev_Click(object sender, EventArgs e)
	{
		handlePreviousClick();
	}

	private void handleNextClick()
	{
		SaveData();
		if (Node.NextNode != null)
		{
			Node = Node.NextNode;
			TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
			base.Tag = treeNodeItem.Index;
			DispData();
		}
	}

	private void tsmiNext_Click(object sender, EventArgs e)
	{
		handleNextClick();
	}

	private void tsmiLast_Click(object sender, EventArgs e)
	{
		SaveData();
		Node = Node.Parent.LastNode;
		TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
		base.Tag = treeNodeItem.Index;
		DispData();
	}

	private void handleInsertClick()
	{
		if (Node.Parent.Nodes.Count < 68)
		{
			SaveData();
			TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
			int minIndex = data.GetMinIndex();
			string minName = data.GetMinName(Node);
			data.SetIndex(minIndex, 1);
			TreeNodeItem tag = new TreeNodeItem(treeNodeItem.Cms, treeNodeItem.Type, null, 0, minIndex, treeNodeItem.ImageIndex, treeNodeItem.Data);
			TreeNode treeNode = new TreeNode(minName);
			treeNode.Tag = tag;
			treeNode.ImageIndex = 25;
			treeNode.SelectedImageIndex = 25;
			Node.Parent.Nodes.Insert(minIndex, treeNode);
			data.SetName(minIndex, minName);
			Node = treeNode;
			base.Tag = minIndex;
			DispData();
			method_7();
			OpenGD77Form.ClearLastUsedChannelsData();
		}
	}

	private void tsmiAdd_Click(object sender, EventArgs e)
	{
		handleInsertClick();
	}

	private void handleDeleteClick()
	{
		if (Node.Parent.Nodes.Count > 1 && Node.Index != 0)
		{
			SaveData();
			TreeNode node = Node.NextNode ?? Node.PrevNode;
			TreeNodeItem treeNodeItem = Node.Tag as TreeNodeItem;
			data.ClearIndex(treeNodeItem.Index);
			Node.Remove();
			Node = node;
			TreeNodeItem treeNodeItem2 = Node.Tag as TreeNodeItem;
			base.Tag = treeNodeItem2.Index;
			DispData();
			method_7();
			OpenGD77Form.ClearLastUsedChannelsData();
		}
		else
		{
			MessageBox.Show(Settings.dicCommon["FirstNotDelete"]);
		}
	}

	private void tsmiDel_Click(object sender, EventArgs e)
	{
		handleDeleteClick();
	}

	private void method_6()
	{
		tsbtnAdd.Enabled = Node.Parent.Nodes.Count != 68;
		tsbtnDel.Enabled = Node.Parent.Nodes.Count != 1 && Node.Index != 0;
		tsbtnFirst.Enabled = Node != Node.Parent.FirstNode;
		tsbtnPrev.Enabled = Node != Node.Parent.FirstNode;
		tsbtnNext.Enabled = Node != Node.Parent.LastNode;
		tsbtnLast.Enabled = Node != Node.Parent.LastNode;
		tslblInfo.Text = $" {data.GetDispIndex(Convert.ToInt32(base.Tag))} / {data.ValidCount}";
	}

	private void method_7()
	{
		if (base.MdiParent is MainForm mainForm)
		{
			mainForm.RefreshRelatedForm(typeof(ZoneForm));
		}
	}

	static ZoneForm()
	{
		SPACE_ZONE = Marshal.SizeOf(typeof(ZoneOne));
		basicData = new BasicZone();
		data = new Zone();
		SPACE_BASIC_ZONE = Marshal.SizeOf(typeof(BasicZone));
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Right | Keys.Control:
			handleNextClick();
			return true;
		case Keys.Left | Keys.Control:
			handlePreviousClick();
			return true;
		case Keys.Insert | Keys.Control:
		case Keys.I | Keys.Control:
			handleInsertClick();
			return true;
		case Keys.Delete | Keys.Control:
			handleDeleteClick();
			return true;
		case Keys.Up | Keys.Control:
		case Keys.U | Keys.Control:
			btnUp_Click();
			return true;
		case Keys.Down | Keys.Control:
		case Keys.D | Keys.Control:
			btnDown_Click();
			return true;
		default:
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
