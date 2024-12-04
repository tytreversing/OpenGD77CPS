internal class NameValuePair
{
	private string _text;

	private object _value;

	public string Text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
		}
	}

	public object Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
		}
	}

	public override string ToString()
	{
		return _text;
	}

	public NameValuePair(string string_0, object object_0)
	{
		_text = string_0;
		_value = object_0;
	}
}
