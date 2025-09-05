namespace TweakManagerBackend.Models
{
    public class LicenseKey
    {
        public int Id { get; set; }
        public required string KeyValue { get; set; }
        public required string AssignedToUsername { get; set; }
        public required string AssignedToHardwareId { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public DateTime? ExpiryDate { get; set; } = null;
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}