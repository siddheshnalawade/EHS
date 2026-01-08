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
}