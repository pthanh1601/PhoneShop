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

            // Bảo đảm CurrentPage nằm trong giới hạn hợp lệ
            CurrentPage = pageIndex < 1 ? 1 : pageIndex > (int)Math.Ceiling(count / (double)pageSize)
                ? (int)Math.Ceiling(count / (double)pageSize)
                : pageIndex;

            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        // Kiểm tra trạng thái trang
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        // Phương thức tạo PaginatedList từ IQueryable
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            // Đếm tổng số phần tử
            var count = await source.CountAsync();

            // Lấy danh sách phần tử cho trang hiện tại
            var items = await source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
