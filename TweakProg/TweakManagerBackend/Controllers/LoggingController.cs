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
    }
}