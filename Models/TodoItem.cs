namespace TodoList.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Todo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public int? UserId { get; set; }
    }
}
