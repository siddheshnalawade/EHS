namespace EHS.Application.DTOs
{
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
}