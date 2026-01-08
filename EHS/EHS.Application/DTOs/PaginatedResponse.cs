namespace EHS.Application.DTOs
{
    /// <summary>
    /// Generic paginated response wrapper for list endpoints.
    /// </summary>
    /// <typeparam name="T">Type of items in the paginated response</typeparam>
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// List of items in the current page.
        /// </summary>
        public List<T> Items { get; set; } = [];

        /// <summary>
        /// Current page number (1-based).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Number of items per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of items across all pages.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Total number of pages.
        /// </summary>
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;

        /// <summary>
        /// Indicates if there are more pages available.
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Indicates if there is a previous page available.
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;
    }
}