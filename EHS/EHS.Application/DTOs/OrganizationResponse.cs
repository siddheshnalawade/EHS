namespace EHS.Application.DTOs
{
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
}