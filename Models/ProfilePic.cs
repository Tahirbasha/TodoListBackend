namespace TodoList.Models
{
    public class ProfilePic
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[] ImageData { get; set; }
    }
}
