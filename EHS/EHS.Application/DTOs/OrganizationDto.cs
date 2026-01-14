namespace EHS.Application.DTOs
{
    /// <summary>
    /// Request DTO for creating a new organization.
    /// Admin only operation.
    /// </summary>
    public class CreateOrganizationRequest
    {
        /// <summary>
        /// Organization name (e.g., "Manufacturing Plant Alpha").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Full legal name of the organization.
        /// </summary>
        public string LegalName { get; set; } = string.Empty;

        /// <summary>
        /// Physical street address.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// City where the organization is located.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// State or province.
        /// </summary>
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// Country of operation.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Postal code for the address.
        /// </summary>
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Primary contact phone number.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Primary contact email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request DTO for updating an existing organization.
    /// Admin only operation.
    /// </summary>
    public class UpdateOrganizationRequest
    {
        /// <summary>
        /// Organization name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Full legal name of the organization.
        /// </summary>
        public string? LegalName { get; set; }

        /// <summary>
        /// Physical street address.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// City where the organization is located.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// State or province.
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Country of operation.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Postal code for the address.
        /// </summary>
        public string? PostalCode { get; set; }

        /// <summary>
        /// Primary contact phone number.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Primary contact email address.
        /// </summary>
        public string? Email { get; set; }
    }

    /// <summary>
    /// Response DTO for organization data.
    /// Returned in all organization-related endpoints.
    /// </summary>
    public class OrganizationResponse
    {
        /// <summary>
        /// Unique identifier for the organization.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Organization name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Full legal name of the organization.
        /// </summary>
        public string LegalName { get; set; } = string.Empty;

        /// <summary>
        /// Physical street address.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// City where the organization is located.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// State or province.
        /// </summary>
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// Country of operation.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Postal code for the address.
        /// </summary>
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Primary contact phone number.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Primary contact email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the organization was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Number of departments in this organization.
        /// </summary>
        public int DepartmentCount { get; set; }

        /// <summary>
        /// Total number of active incidents in this organization.
        /// </summary>
        public int ActiveIncidentsCount { get; set; }
    }

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