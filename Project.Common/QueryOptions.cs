using Project.Common.Filtering;
using Project.Common.Paging;
using Project.Common.Sorting;


namespace Project.Common;

/// <summary>
/// Klasa koja kombinira opcije za filtriranje, sortiranje i stranice
/// </summary>
public class QueryOptions
{

    public FilterOptions Filtering { get; set; } = new FilterOptions();


    public SortOptions Sorting { get; set; } = new SortOptions();


    public PagingOptions Paging { get; set; } = new PagingOptions();
}