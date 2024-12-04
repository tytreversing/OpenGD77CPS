namespace DMR;

public class DmrMarcDataDataItem
{
	public string Id { get; set; }

	public string Callsign { get; set; }

	public string Name { get; set; }

	public string City { get; set; }

	public string State { get; set; }

	public string Country { get; set; }

	public string Remarks { get; set; }

	public override string ToString()
	{
		return Id + "  " + Callsign + " " + Name + " " + City + " " + State + " " + Country + " " + Remarks;
	}

	public static DmrMarcDataDataItem FromCsv(string csvLine)
	{
		if (csvLine != "")
		{
			DmrMarcDataDataItem dmrMarcDataDataItem = new DmrMarcDataDataItem();
			string[] array = csvLine.Split(',');
			dmrMarcDataDataItem.Id = array[0].Trim('"');
			dmrMarcDataDataItem.Callsign = array[1].Trim('"');
			dmrMarcDataDataItem.Name = array[2].Trim('"');
			dmrMarcDataDataItem.City = array[3].Trim('"');
			dmrMarcDataDataItem.State = array[4].Trim('"');
			dmrMarcDataDataItem.Country = array[5].Trim('"');
			dmrMarcDataDataItem.Remarks = array[6].Trim('"');
			return dmrMarcDataDataItem;
		}
		return null;
	}
}
