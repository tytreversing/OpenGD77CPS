using System;
using System.Collections;
using System.Windows.Forms;

internal class CustomCombo : ComboBox
{
	private Hashtable ItemList;

	public CustomCombo()
	{
		ItemList = new Hashtable();
	}

	public void method_0()
	{
		base.Items.Clear();
		ItemList.Clear();
	}

	public void method_1(string string_0, int int_0)
	{
		int num = base.Items.Add(new NameValuePair(string_0, int_0));
		if (!ItemList.Contains(int_0))
		{
			ItemList.Add(int_0, num);
		}
	}

	public void method_2(int int_0)
	{
		if (ItemList.ContainsKey(int_0))
		{
			SelectedIndex = int.Parse(ItemList[int_0].ToString());
		}
		else if (base.Items.Count > 0)
		{
			SelectedIndex = 0;
		}
		else
		{
			SelectedIndex = -1;
		}
	}

	public int method_3()
	{
		if (base.SelectedItem is NameValuePair nameValuePair)
		{
			return Convert.ToInt32(nameValuePair.Value);
		}
		return 0;
	}
}
