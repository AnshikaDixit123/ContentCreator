namespace ContentCreator.Application.Common.DTOs.ResponseDTOs
{
    public class ResponseData<T>
    {
        public bool IsSuccess { get; set; } = false;
        public int StatusCode { get; set; } = 201;
        public string Message { get; set; } = string.Empty;
        public T? Result { get; set; }
    }
}
