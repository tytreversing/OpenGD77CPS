using System.Runtime.CompilerServices;

internal class SelectedItemUtils
{
	private int _DispNum;

	public int Value { get; set; }

	public string Name { get; set; }

	[CompilerGenerated]
	public int method_0()
	{
		return _DispNum;
	}

	[CompilerGenerated]
	public void method_1(int int_0)
	{
		_DispNum = int_0;
	}

	public SelectedItemUtils(int int_0, int int_1, string string_0)
	{
		Value = int_1;
		method_1(int_0);
		Name = string_0;
	}

	public override string ToString()
	{
		if (method_0() < 0)
		{
			return Name;
		}
		return $"{method_0() + 1:d3}:{Name}";
	}
}
