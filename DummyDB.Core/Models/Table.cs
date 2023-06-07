namespace DummyDB.Core.Models;

public class Table
{
    public string Name { get; private set; }
    public List<Row> Rows { get; }
    public Schema Schema { get; }

    public Table(string name, Schema schema)
    {
        Name = name;
        Rows = new List<Row>();
        Schema = schema;
    }
}