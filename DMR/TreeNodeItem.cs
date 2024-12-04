using System;
using System.Windows.Forms;

namespace DMR;

public class TreeNodeItem
{
	public int Index { get; set; }

	public Type Type { get; set; }

	public Type SubType { get; set; }

	public ContextMenuStrip Cms { get; set; }

	public int Count { get; set; }

	public int ImageIndex { get; set; }

	public IData Data { get; set; }

	public TreeNodeItem()
	{
		Type = null;
		Index = -1;
		Count = 0;
		Cms = null;
		ImageIndex = 0;
	}

	public TreeNodeItem(ContextMenuStrip cms, Type type, Type subType, int count, int index, int imgIndex, IData data)
	{
		Cms = cms;
		Type = type;
		SubType = subType;
		Count = count;
		Index = index;
		ImageIndex = imgIndex;
		Data = data;
	}

	public void Paste(TreeNodeItem copyItem)
	{
		Data.Paste(copyItem.Index, Index);
	}
}
