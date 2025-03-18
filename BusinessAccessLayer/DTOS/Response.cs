
namespace BusinessAccessLayer.DTOS
{
    public class Response<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }

        // Constructors for easy usage
        public Response(T data, bool isSuccess = true, string message = "Success")
        {
            Data = data;
            IsSuccess = isSuccess;
            Message = message;
        }

        public Response(string message, bool isSuccess = false)
        {
            Data = default;
            IsSuccess = isSuccess;
            Message = message;
        }
    }

}
