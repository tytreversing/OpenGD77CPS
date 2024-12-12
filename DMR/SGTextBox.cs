using System;
using System.Text;
using System.Windows.Forms;

namespace DMR;

public class SGTextBox : TextBox
{
	private const int WM_CHAR = 258;

	private const int WM_PASTE = 770;

	public int MaxByteLength { get; set; }

	public string InputString { get; set; }

	public bool OnlyAllowInputStringAndCapitaliseCharacters { get; set; }

	public SGTextBox()
	{
		OnlyAllowInputStringAndCapitaliseCharacters = false;
		ContextMenu = new ContextMenu();
	}


	protected override void WndProc(ref Message m)
	{
		if (m.Msg == 770)
		{
			method_0();
		}
		else
		{
			base.WndProc(ref m);
		}
	}

	private void method_0()
	{
		string text = Clipboard.GetText();
		foreach (char c in text)
		{
			Message m = default(Message);
			m.HWnd = base.Handle;
			m.Msg = 258;
			m.WParam = (IntPtr)c;
			m.LParam = IntPtr.Zero;
			base.WndProc(ref m);
		}
	}
}
