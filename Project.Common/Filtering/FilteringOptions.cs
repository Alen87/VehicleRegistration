namespace Project.Common.Filtering;

using System.Collections.Generic;

public class FilteringOptions
{
    public string? SearchText { get; set; }
    public string? SearchField { get; set; }
    public int? MakeId { get; set; }
    public int? ModelId { get; set; }
    public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
}