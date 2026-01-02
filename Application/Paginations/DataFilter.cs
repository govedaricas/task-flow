using Application.Abstraction;

namespace Application.Paginations
{
    public abstract class DataFilter : IRequest<object>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
        public virtual string[] SearchColumns => Array.Empty<string>();
        public virtual string DefaultSortColumn => "Id";
    }
}
