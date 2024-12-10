using Microsoft.EntityFrameworkCore;

namespace PhoneShop.ViewModels
{
    public class PaginatedList<T> : List<T>
    {
        public int TotalCount { get; private set; } // Tổng số bản ghi
        public int TotalPages { get; private set; } // Tổng số trang
        public int CurrentPage { get; private set; } // Trang hiện tại
        public int PageSize { get; private set; } // Số sản phẩm trên mỗi trang

        // Constructor
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        // Phương thức giúp tạo PaginatedList từ IQueryable để sử dụng cho Entity Framework
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }

}
