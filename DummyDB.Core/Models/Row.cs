namespace DummyDB.Core.Models;

public class Row
{
    public Dictionary<Column, object> Elements { get; set; }

    public Row()
    {
        Elements = new Dictionary<Column, object>();
    }
}