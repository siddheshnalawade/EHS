namespace EHS.Domain.Settings
{
    public class FileStorageSettings
    {
        public string Provider { get; set; } = "Local"; // "Local" or "Azure"
        public string ConnectionString { get; set; } = string.Empty; // For Azure
        public string BasePath { get; set; } = "uploads"; // For Local
    }
}