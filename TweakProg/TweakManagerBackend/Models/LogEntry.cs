namespace TweakManagerBackend.Models
{
    public class LogEntry
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}