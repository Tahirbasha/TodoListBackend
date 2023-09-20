namespace TodoList.Dto
{
    public class ResponseDto<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public string authToken { get; set; } = string.Empty;
    }
}
