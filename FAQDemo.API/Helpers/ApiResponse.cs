namespace FAQDemo.API.Helpers
{
    public class ApiResponse<T>
    {
        public bool? Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        // success response
        public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

        // multiple errors
        public static ApiResponse<T> FailResponse(List<string> errors, string? message = null)
            => new() { Success = false, Errors = errors, Message = message };

        // single error
        public static ApiResponse<T> FailResponse(string error, string? message = null)
            => new() { Success = false, Errors = [error], Message = message };
    }
}