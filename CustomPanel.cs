using System;
using System.Windows.Forms;

internal class CustomPanel : Panel
{
	protected override void OnClick(EventArgs e)
	{
		Focus();
		base.OnClick(e);
	}
}
