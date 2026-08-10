namespace APICatalogo.Pagination;

public class ProdutosParameters
{
    const int MaxPageSize = 50;
    private int _pageSize;
    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get { return _pageSize; }
        set { _pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
    }
}
