using System.Drawing;
using System.Windows.Forms;

[ToolboxBitmap(typeof(ListBox))]
internal class CustomListbox : ListBox
{
	private ToolTip tip;

	private void method_0(string string_0)
	{
		tip.SetToolTip(this, string_0);
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);
		int num = IndexFromPoint(e.Location);
		if (num == -1)
		{
			method_0("");
			return;
		}
		string itemText = GetItemText(base.Items[num]);
		method_0(itemText);
	}

	public CustomListbox()
	{
		tip = new ToolTip();
	}
}
