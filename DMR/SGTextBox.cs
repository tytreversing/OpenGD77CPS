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

	protected override void OnKeyPress(KeyPressEventArgs e)
	{
		if (char.IsControl(e.KeyChar))
		{
			return;
		}
		if (OnlyAllowInputStringAndCapitaliseCharacters && !string.IsNullOrEmpty(InputString))
		{
			if (InputString.IndexOf(char.ToUpper(e.KeyChar)) == -1)
			{
				e.Handled = e.KeyChar != '\b';
				return;
			}
			e.KeyChar = char.ToUpper(e.KeyChar);
			e.Handled = false;
			return;
		}
		if (!string.IsNullOrEmpty(InputString) && InputString.IndexOf(char.ToUpper(e.KeyChar)) < 0)
		{
			e.Handled = true;
			return;
		}
		base.OnKeyPress(e);
		if (MaxByteLength != 0 && !char.IsControl(e.KeyChar))
		{
			int byteCount = Encoding.GetEncoding(936).GetByteCount(Text + e.KeyChar);
			int byteCount2 = Encoding.GetEncoding(936).GetByteCount(SelectedText);
			if (byteCount - byteCount2 > MaxByteLength)
			{
				e.Handled = true;
			}
		}
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
