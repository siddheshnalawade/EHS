namespace EHS.Application.DTOs
{
    /// <summary>
    /// Standard API response wrapper for all endpoints.
    /// </summary>
    /// <typeparam name="T">Type of data returned in the response</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates if the request was successful.
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Response message (status or error message).
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The actual response data.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// List of validation errors if any.
        /// </summary>
        public List<ValidationError> Errors { get; set; } = [];
    }

    /// <summary>
    /// Represents a single validation error.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Name of the field that failed validation.
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// Error message describing the validation failure.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}