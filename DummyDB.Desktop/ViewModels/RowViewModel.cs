using System.Collections.Generic;

namespace DummyDB.Desktop.ViewModels;

public class RowViewModel
{
    public List<object> Elements { get; set; }

    public RowViewModel(int index)
    {
        var temp = new object[index];
        for (var i = 0; i < index; i++)
        {
            temp[i] = "";
        }

        Elements = new List<object>(temp);
    }

    public RowViewModel() { }
}