namespace RentAppBE.Shared
{
    public class PagingRequest
    {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public string? SortColumn { get; set; } = string.Empty;
        public bool IsSortDesc { get; set; } = false;

    }
}
