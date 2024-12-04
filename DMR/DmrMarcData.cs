using System.Collections.Generic;

namespace DMR;

internal class DmrMarcData
{
	public List<DmrMarcDataDataItem> users { get; set; }

	public override string ToString()
	{
		string text = string.Empty;
		foreach (DmrMarcDataDataItem user in users)
		{
			text = text + user.ToString() + "\n";
		}
		return text;
	}
}
