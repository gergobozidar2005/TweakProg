using Microsoft.AspNetCore.Mvc;
using TweakManagerBackend.Data;
using TweakManagerBackend.Models;

namespace TweakManagerBackend.Controllers
{
    // Ez egy adatstruktúra, amit a kliens fog küldeni a naplózáshoz
    public class LogRequest
    {
        public required string Username { get; set; }
        public required string Message { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class LoggingController : ControllerBase
    {
        private readonly ApiDbContext _context;
        public LoggingController(ApiDbContext context)
        {
            _context = context;
        }

        // Új naplóbejegyzés. Pl: POST /api/logging/log
        [HttpPost("log")]
        public async Task<IActionResult> CreateLog([FromBody] LogRequest request)
        {
            var logEntry = new LogEntry
            {
                Username = request.Username,
                Message = request.Message
            };

            _context.LogEntries.Add(logEntry);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Recent logs for dashboard
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentLogs()
        {
            var recentLogs = await _context.LogEntries
                .OrderByDescending(l => l.Timestamp)
                .Take(10)
                .Select(l => new
                {
                    l.Timestamp,
                    l.Username,
                    Action = ExtractAction(l.Message),
                    l.Message
                })
                .ToListAsync();

            return Ok(recentLogs);
        }

        private string ExtractAction(string message)
        {
            if (message.Contains("cleanup") || message.Contains("clean"))
                return "System Cleanup";
            if (message.Contains("login"))
                return "User Login";
            if (message.Contains("register"))
                return "User Registration";
            if (message.Contains("temp"))
                return "Temp Cleanup";
            if (message.Contains("recycle"))
                return "Recycle Bin";
            return "System Activity";
        }
    }
}