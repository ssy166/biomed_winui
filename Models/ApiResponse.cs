namespace biomed.Models
{
    public class ApiResponse<T>
    {
        public int Code { get; set; }
        public T Data { get; set; }
        public string Msg { get; set; }
    }
} 