namespace EHS.Application.DTOs
{
    /// <summary>
    /// Response DTO containing organization statistics and metrics.
    /// </summary>
    public class OrganizationStatisticsResponse
    {
        /// <summary>
        /// Organization ID.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Organization name.
        /// </summary>
        public string OrganizationName { get; set; } = string.Empty;

        /// <summary>
        /// Total number of departments.
        /// </summary>
        public int TotalDepartments { get; set; }

        /// <summary>
        /// Total number of production lines across all departments.
        /// </summary>
        public int TotalProductionLines { get; set; }

        /// <summary>
        /// Total number of machines across all production lines.
        /// </summary>
        public int TotalMachines { get; set; }

        /// <summary>
        /// Total number of incidents (all time).
        /// </summary>
        public int TotalIncidents { get; set; }

        /// <summary>
        /// Number of open/pending incidents.
        /// </summary>
        public int OpenIncidents { get; set; }

        /// <summary>
        /// Number of incidents in progress.
        /// </summary>
        public int InProgressIncidents { get; set; }

        /// <summary>
        /// Number of closed incidents.
        /// </summary>
        public int ClosedIncidents { get; set; }

        /// <summary>
        /// Number of rejected incidents.
        /// </summary>
        public int RejectedIncidents { get; set; }

        /// <summary>
        /// Average days to close an incident.
        /// </summary>
        public double AverageResolutionDays { get; set; }

        /// <summary>
        /// Date when organization was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}