using System;
using System.Text;

internal class Base64Utils
{
	public static string smethod_0(string string_0)
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(string_0));
	}

	public static string smethod_1(string string_0)
	{
		byte[] bytes = Convert.FromBase64String(string_0);
		return Encoding.UTF8.GetString(bytes);
	}
}
