using Application.Models;
using Application.Paginations;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;

namespace Application.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySortFilter<T>(this IQueryable<T> query, DataFilter filter)
            where T : class
        {
            var sortColumn = !string.IsNullOrEmpty(filter.SortBy)
                ? filter.SortBy
                : filter.DefaultSortColumn;

            var sortOrder = filter.SortDescending ? "DESC" : "ASC";

            try
            {
                query = query.OrderBy($"{sortColumn} {sortOrder}");
            }
            catch
            {
                query = query.OrderBy($"{filter.DefaultSortColumn} {sortOrder}");
            }

            return query;
        }

        public static IQueryable<T> ApplySearchFilter<T>(this IQueryable<T> query, DataFilter filter)
            where T : class
        {
            if (string.IsNullOrEmpty(filter.SearchTerm) || filter.SearchColumns.Length == 0)
                return query;

            var searchTerm = filter.SearchTerm.ToLower();
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? expression = null;

            foreach (var column in filter.SearchColumns)
            {
                try
                {
                    var property = Expression.Property(parameter, column);
                    var method = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
                    var toLower = Expression.Call(property, typeof(object).GetMethod("ToString")!);
                    var call = Expression.Call(toLower, method, Expression.Constant(searchTerm));

                    expression = expression == null
                        ? call
                        : Expression.OrElse(expression, call);
                }
                catch
                {
                    continue;
                }
            }

            if (expression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        public static async Task<PagedData<T>> ApplyPagedDataAsync<T>(this IQueryable<T> query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
            where T : class
        {
            if (pageNumber < 1) 
                pageNumber = 1;

            if (pageSize < 1) 
                pageSize = 10;

            if (pageSize > 100) 
                pageSize = 100;

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedData<T>
            {
                Data = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
