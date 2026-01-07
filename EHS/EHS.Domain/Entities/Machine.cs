namespace EHS.Domain.Entities
{
    /// <summary>
    /// Represents a machine or equipment on a production line.
    /// </summary>
    public class Machine : BaseEntity
    {
        /// <summary>
        /// Name or model of the machine.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Unique equipment identifier or serial number.
        /// </summary>
        public string EquipmentId { get; set; } = string.Empty;

        /// <summary>
        /// Description of the machine's function and specifications.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Manufacturer name.
        /// </summary>
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>
        /// Model number of the machine.
        /// </summary>
        public string ModelNumber { get; set; } = string.Empty;

        /// <summary>
        /// Date when the machine was installed or commissioned.
        /// </summary>
        public DateTime? InstalledDate { get; set; }

        /// <summary>
        /// Foreign key referencing the production line.
        /// </summary>
        public Guid ProductionLineId { get; set; }

        /// <summary>
        /// Navigation property for the production line.
        /// </summary>
        public ProductionLine ProductionLine { get; set; } = null!;

        /// <summary>
        /// Navigation property for incidents involving this machine.
        /// </summary>
        public List<Incident> Incidents { get; set; } = [];
    }
}