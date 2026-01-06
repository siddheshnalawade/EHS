namespace EHS.Domain.Entities
{
    /// <summary>
    /// Represents the organization in the EHS system.
    /// </summary>
    public class Organization : BaseEntity
    {
        /// <summary>
        /// Name of the organization.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Full legal name of the organization.
        /// </summary>
        public string LegalName { get; set; } = string.Empty;

        /// <summary>
        /// Physical address of the organization.
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
        /// Country where the organization is located.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Postal code for the organization.
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
        /// Navigation property for departments within this organization.
        /// </summary>
        public List<Department> Departments { get; set; } = [];

        /// <summary>
        /// Navigation property for incidents in this organization.
        /// </summary>
        public List<Incident> Incidents { get; set; } = [];
    }
}