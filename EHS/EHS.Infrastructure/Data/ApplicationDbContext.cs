using EHS.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EHS.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
    {
        /// <summary>
        /// Organization and structural entities.
        /// </summary>
        public DbSet<Organization> Organizations { get; set; }

        public DbSet<Department> Departments { get; set; }
        public DbSet<ProductionLine> ProductionLines { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<DepartmentSafetyOfficer> DepartmentSafetyOfficers { get; set; }

        /// <summary>
        /// Master data entities.
        /// </summary>
        public DbSet<IncidentType> IncidentTypes { get; set; }

        public DbSet<IncidentNature> IncidentNatures { get; set; }
        public DbSet<IncidentSeverity> IncidentSeverities { get; set; }
        public DbSet<IncidentStatus> IncidentStatuses { get; set; }
        public DbSet<ClosureAction> ClosureActions { get; set; }
        public DbSet<Benefit> Benefits { get; set; }

        /// <summary>
        /// Core incident management entities.
        /// </summary>
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<IncidentAttachment> IncidentAttachments { get; set; }

        public DbSet<IncidentComment> IncidentComments { get; set; }

        /// <summary>
        /// Implementation workflow entities.
        /// </summary>
        public DbSet<IncidentImplementation> IncidentImplementations { get; set; }

        public DbSet<RootCauseAnalysisDetail> RootCauseAnalysisDetails { get; set; }
        public DbSet<ImplementationBenefit> ImplementationBenefits { get; set; }

        /// <summary>
        /// Audit logging entities.
        /// </summary>
        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<AuditLogDetail> AuditLogDetails { get; set; }

        /// <summary>
        /// Configures the model relationships, constraints, and indexes.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==================== Seed Master Data ====================

            SeedIncidentTypes(modelBuilder);
            SeedIncidentNatures(modelBuilder);
            SeedIncidentSeverities(modelBuilder);
            SeedIncidentStatuses(modelBuilder);
            SeedClosureActions(modelBuilder);
            SeedBenefits(modelBuilder);
            SeedOrganizations(modelBuilder);
            SeedDepartments(modelBuilder);
            SeedProductionLines(modelBuilder);
            SeedMachines(modelBuilder);

            // ==================== Incident Relationships ====================

            // Incident -> IncidentType (Many-to-One)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.IncidentType)
                .WithMany(it => it.Incidents)
                .HasForeignKey(i => i.IncidentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Incident -> IncidentNature (Many-to-One)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.IncidentNature)
                .WithMany(in_ => in_.Incidents)
                .HasForeignKey(i => i.IncidentNatureId)
                .OnDelete(DeleteBehavior.Restrict);

            // Incident -> IncidentSeverity (Many-to-One)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.IncidentSeverity)
                .WithMany(iss => iss.Incidents)
                .HasForeignKey(i => i.IncidentSeverityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Incident -> IncidentStatus (Many-to-One)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.IncidentStatus)
                .WithMany(ist => ist.Incidents)
                .HasForeignKey(i => i.IncidentStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Incident -> Organization (Many-to-One)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Organization)
                .WithMany(o => o.Incidents)
                .HasForeignKey(i => i.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Incident -> Department (Many-to-One)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Department)
                .WithMany(d => d.Incidents)
                .HasForeignKey(i => i.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Incident -> ProductionLine (Many-to-One, Optional)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.ProductionLine)
                .WithMany(pl => pl.Incidents)
                .HasForeignKey(i => i.ProductionLineId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Incident -> Machine (Many-to-One, Optional)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Machine)
                .WithMany(m => m.Incidents)
                .HasForeignKey(i => i.MachineId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Incident -> InitiatedByUser (Many-to-One)
            // Changed from Restrict to NoAction to prevent cascade path cycles
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.InitiatedByUser)
                .WithMany()
                .HasForeignKey(i => i.InitiatedByUserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);

            // Incident -> AssignedToSafetyOfficer (Many-to-One, Optional)
            // Changed from SetNull to NoAction to prevent cascade path cycles
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.AssignedToSafetyOfficer)
                .WithMany()
                .HasForeignKey(i => i.AssignedToSafetyOfficerId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            // Incident -> AssignedToImplementor (Many-to-One, Optional)
            // Changed from SetNull to NoAction to prevent cascade path cycles
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.AssignedToImplementor)
                .WithMany()
                .HasForeignKey(i => i.AssignedToImplementorId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            // Incident -> IncidentComment (One-to-Many)
            modelBuilder.Entity<Incident>()
                .HasMany(i => i.Comments)
                .WithOne(c => c.Incident)
                .HasForeignKey(c => c.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Incident -> IncidentImplementation (One-to-One)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Implementation)
                .WithOne(impl => impl.Incident)
                .HasForeignKey<IncidentImplementation>(impl => impl.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== Department Relationships ====================

            // Department -> Organization (Many-to-One)
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Organization)
                .WithMany(o => o.Departments)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Department -> ProductionLine (One-to-Many)
            modelBuilder.Entity<Department>()
                .HasMany(d => d.ProductionLines)
                .WithOne(pl => pl.Department)
                .HasForeignKey(pl => pl.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Department -> DepartmentSafetyOfficer (One-to-Many)
            modelBuilder.Entity<Department>()
                .HasMany(d => d.SafetyOfficers)
                .WithOne(dso => dso.Department)
                .HasForeignKey(dso => dso.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== ProductionLine Relationships ====================

            // ProductionLine -> Machine (One-to-Many)
            modelBuilder.Entity<ProductionLine>()
                .HasMany(pl => pl.Machines)
                .WithOne(m => m.ProductionLine)
                .HasForeignKey(m => m.ProductionLineId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== IncidentComment Relationships ====================

            // IncidentComment -> CommentedByUser (Many-to-One)
            // Changed from Restrict to NoAction to prevent cascade path cycles
            modelBuilder.Entity<IncidentComment>()
                .HasOne(c => c.CommentedByUser)
                .WithMany()
                .HasForeignKey(c => c.CommentedByUserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);

            // ==================== IncidentImplementation Relationships ====================

            // IncidentImplementation -> ClosureAction (Many-to-One)
            modelBuilder.Entity<IncidentImplementation>()
                .HasOne(impl => impl.ClosureAction)
                .WithMany(ca => ca.Implementations)
                .HasForeignKey(impl => impl.ClosureActionId)
                .OnDelete(DeleteBehavior.Restrict);

            // IncidentImplementation -> ImplementedByUser (Many-to-One)
            // Changed from Restrict to NoAction to prevent cascade path cycles
            modelBuilder.Entity<IncidentImplementation>()
                .HasOne(impl => impl.ImplementedByUser)
                .WithMany()
                .HasForeignKey(impl => impl.ImplementedByUserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);

            // IncidentImplementation -> RootCauseAnalysisDetail (One-to-Many)
            modelBuilder.Entity<IncidentImplementation>()
                .HasMany(impl => impl.RootCauseDetails)
                .WithOne(rca => rca.Implementation)
                .HasForeignKey(rca => rca.ImplementationId)
                .OnDelete(DeleteBehavior.Cascade);

            // IncidentImplementation -> ImplementationBenefit (One-to-Many)
            modelBuilder.Entity<IncidentImplementation>()
                .HasMany(impl => impl.Benefits)
                .WithOne(ib => ib.Implementation)
                .HasForeignKey(ib => ib.ImplementationId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== ImplementationBenefit Relationships ====================

            // ImplementationBenefit -> Benefit (Many-to-One)
            modelBuilder.Entity<ImplementationBenefit>()
                .HasOne(ib => ib.Benefit)
                .WithMany(b => b.Implementations)
                .HasForeignKey(ib => ib.BenefitId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==================== DepartmentSafetyOfficer Relationships ====================

            // DepartmentSafetyOfficer -> SafetyOfficer (Many-to-One)
            // Changed from Cascade to NoAction to prevent cascade path cycles
            modelBuilder.Entity<DepartmentSafetyOfficer>()
                .HasOne(dso => dso.SafetyOfficer)
                .WithMany()
                .HasForeignKey(dso => dso.SafetyOfficerId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);

            // ==================== AuditLog Relationships ====================

            // AuditLog -> AuditLogDetail (One-to-Many)
            modelBuilder.Entity<AuditLog>()
                .HasMany(al => al.Details)
                .WithOne(ald => ald.AuditLog)
                .HasForeignKey(ald => ald.AuditLogId)
                .OnDelete(DeleteBehavior.Cascade);

            // Incident -> IncidentAttachment (One-to-Many)
            modelBuilder.Entity<Incident>()
                .HasMany(i => i.Attachments)
                .WithOne(a => a.Incident)
                .HasForeignKey(a => a.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // IncidentAttachment -> UploadedByUser
            modelBuilder.Entity<IncidentAttachment>()
                .HasOne(a => a.UploadedByUser)
                .WithMany()
                .HasForeignKey(a => a.UploadedByUserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);

            // AuditLog -> Incident (Many-to-One, Optional)
            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.Incident)
                .WithMany(i => i.AuditLogs)
                .HasForeignKey(al => al.IncidentId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // ==================== Indexes ====================

            // Performance indexes
            modelBuilder.Entity<Incident>()
                .HasIndex(i => i.Id)
                .IsUnique();

            modelBuilder.Entity<Incident>()
                .HasIndex(i => i.IncidentStatusId);

            modelBuilder.Entity<Incident>()
                .HasIndex(i => i.DepartmentId);

            modelBuilder.Entity<Incident>()
                .HasIndex(i => i.IncidentDate);

            modelBuilder.Entity<Incident>()
                .HasIndex(i => i.InitiatedByUserId);

            modelBuilder.Entity<IncidentImplementation>()
                .HasIndex(ii => ii.IncidentId)
                .IsUnique();

            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => al.EntityId);

            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => al.Timestamp);

            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => al.UserId);

            modelBuilder.Entity<Department>()
                .HasIndex(d => new { d.OrganizationId, d.Code })
                .IsUnique();

            modelBuilder.Entity<ProductionLine>()
                .HasIndex(pl => new { pl.DepartmentId, pl.Code })
                .IsUnique();

            modelBuilder.Entity<Machine>()
                .HasIndex(m => m.EquipmentId)
                .IsUnique();

            // ==================== Soft Delete Query Filters ====================
            // Apply soft delete filters to all entities that inherit from BaseEntity
            // Child entities automatically inherit the filter through relationships

            modelBuilder.Entity<Organization>()
                .HasQueryFilter(o => !o.IsDeleted);

            modelBuilder.Entity<Department>()
                .HasQueryFilter(d => !d.IsDeleted);

            modelBuilder.Entity<ProductionLine>()
                .HasQueryFilter(pl => !pl.IsDeleted);

            modelBuilder.Entity<Machine>()
                .HasQueryFilter(m => !m.IsDeleted);

            modelBuilder.Entity<Incident>()
                .HasQueryFilter(i => !i.IsDeleted);

            modelBuilder.Entity<IncidentImplementation>()
                .HasQueryFilter(ii => !ii.IsDeleted);

            // ==================== Child Entity Query Filters ====================
            // Apply matching filters to child entities to prevent orphaned records
            // when parent is soft-deleted

            modelBuilder.Entity<DepartmentSafetyOfficer>()
                .HasQueryFilter(dso => !dso.IsDeleted && !dso.Department.IsDeleted);

            modelBuilder.Entity<IncidentComment>()
                .HasQueryFilter(ic => !ic.IsDeleted && !ic.Incident.IsDeleted);

            modelBuilder.Entity<IncidentAttachment>()
                .HasQueryFilter(ia => !ia.IsDeleted && !ia.Incident.IsDeleted);

            modelBuilder.Entity<RootCauseAnalysisDetail>()
                .HasQueryFilter(rca => !rca.IsDeleted && !rca.Implementation.IsDeleted);

            modelBuilder.Entity<ImplementationBenefit>()
                .HasQueryFilter(ib => !ib.IsDeleted && !ib.Implementation.IsDeleted);
        }

        /// <summary>
        /// Seeds incident types into the database.
        /// </summary>
        private static void SeedIncidentTypes(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<IncidentType>().HasData(
                new IncidentType
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    Name = "Near Miss",
                    Description = "An incident where no injury occurred, but had potential for harm.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new IncidentType
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    Name = "Unsafe Condition",
                    Description = "An unsafe environmental or equipment condition that could cause harm.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new IncidentType
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    Name = "Unsafe Action",
                    Description = "An unsafe behavior or action performed by an individual.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                }
            );
        }

        /// <summary>
        /// Seeds incident natures into the database.
        /// </summary>
        private static void SeedIncidentNatures(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<IncidentNature>().HasData(
                new IncidentNature
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                    Name = "New",
                    Description = "First occurrence of this type of incident.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new IncidentNature
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                    Name = "Repeated",
                    Description = "Similar incident has occurred previously.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                }
            );
        }

        /// <summary>
        /// Seeds incident severity levels into the database.
        /// </summary>
        private static void SeedIncidentSeverities(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<IncidentSeverity>().HasData(
                new IncidentSeverity
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                    Name = "Minor",
                    SeverityLevel = 1,
                    Description = "Minor incident with minimal impact. Low risk of injury or damage.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new IncidentSeverity
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                    Name = "Major",
                    SeverityLevel = 2,
                    Description = "Major incident with significant impact. Moderate risk of serious injury or substantial damage.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new IncidentSeverity
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                    Name = "Catastrophic",
                    SeverityLevel = 3,
                    Description = "Catastrophic incident with severe impact. High risk of fatality or major damage. Requires immediate intervention.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                }
            );
        }

        /// <summary>
        /// Seeds incident statuses into the database.
        /// </summary>
        private static void SeedIncidentStatuses(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<IncidentStatus>().HasData(
                new IncidentStatus
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                    Name = "Submitted",
                    StageOrder = 1,
                    Description = "Incident has been submitted by the initiator and is awaiting review.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new IncidentStatus
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000002"),
                    Name = "Assigned",
                    StageOrder = 2,
                    Description = "Incident has been assigned to a safety officer for review and approval.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new IncidentStatus
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000003"),
                    Name = "Approved",
                    StageOrder = 3,
                    Description = "Incident has been approved by the safety officer and assigned to an implementor.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new IncidentStatus
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000004"),
                    Name = "InProgress",
                    StageOrder = 4,
                    Description = "Implementation is in progress. Implementor is working on corrective actions.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new IncidentStatus
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000005"),
                    Name = "VerificationPending",
                    StageOrder = 5,
                    Description = "Implementation is complete. Awaiting safety officer verification.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new IncidentStatus
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000006"),
                    Name = "Closed",
                    StageOrder = 6,
                    Description = "Incident has been verified and closed by the safety officer.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new IncidentStatus
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000007"),
                    Name = "Rejected",
                    StageOrder = 0,
                    Description = "Incident has been rejected and requires resubmission or correction.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                }
            );
        }

        /// <summary>
        /// Seeds closure actions into the database.
        /// </summary>
        private static void SeedClosureActions(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<ClosureAction>().HasData(
                new ClosureAction
                {
                    Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                    Name = "Engineering Control",
                    Description = "Implementation of engineering controls to eliminate or reduce hazards (e.g., machine guards, interlocks).",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new ClosureAction
                {
                    Id = Guid.Parse("50000000-0000-0000-0000-000000000002"),
                    Name = "Administrative Control",
                    Description = "Implementation of work procedures, policies, or process changes to reduce risk.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new ClosureAction
                {
                    Id = Guid.Parse("50000000-0000-0000-0000-000000000003"),
                    Name = "Training",
                    Description = "Provision of training or awareness programs to workers.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new ClosureAction
                {
                    Id = Guid.Parse("50000000-0000-0000-0000-000000000004"),
                    Name = "Personal Protective Equipment (PPE)",
                    Description = "Provision of appropriate personal protective equipment.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new ClosureAction
                {
                    Id = Guid.Parse("50000000-0000-0000-0000-000000000005"),
                    Name = "Maintenance",
                    Description = "Preventive or corrective maintenance of equipment or facilities.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new ClosureAction
                {
                    Id = Guid.Parse("50000000-0000-0000-0000-000000000006"),
                    Name = "Process Improvement",
                    Description = "Modification of processes or workflows to enhance safety and efficiency.",
                    CreatedAt = seedDate,
                    IsDeleted = false
                }
            );
        }

        /// <summary>
        /// Seeds benefits into the database.
        /// </summary>
        private static void SeedBenefits(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Benefit>().HasData(
                new Benefit
                {
                    Id = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                    Name = "Reduced Injury Rate",
                    Description = "Decrease in the number of workplace injuries due to implemented controls.",
                    Category = "Safety",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Benefit
                {
                    Id = Guid.Parse("60000000-0000-0000-0000-000000000002"),
                    Name = "Improved Process Efficiency",
                    Description = "Streamlined processes leading to increased productivity and reduced waste.",
                    Category = "Efficiency",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Benefit
                {
                    Id = Guid.Parse("60000000-0000-0000-0000-000000000003"),
                    Name = "Enhanced Quality",
                    Description = "Improved product or service quality as a result of corrective actions.",
                    Category = "Quality",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Benefit
                {
                    Id = Guid.Parse("60000000-0000-0000-0000-000000000004"),
                    Name = "Cost Savings",
                    Description = "Financial savings from reduction in accidents, downtime, or resource consumption.",
                    Category = "Financial",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Benefit
                {
                    Id = Guid.Parse("60000000-0000-0000-0000-000000000005"),
                    Name = "Improved Employee Morale",
                    Description = "Enhanced employee confidence and satisfaction due to safer working conditions.",
                    Category = "HR",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Benefit
                {
                    Id = Guid.Parse("60000000-0000-0000-0000-000000000006"),
                    Name = "Better Compliance",
                    Description = "Enhanced compliance with regulations, standards, and company policies.",
                    Category = "Compliance",
                    CreatedAt = seedDate,
                    IsDeleted = false
                }
            );
        }

        /// <summary>
        /// Seeds organizations into the database.
        /// </summary>
        private static void SeedOrganizations(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Organization>().HasData(
                new Organization
                {
                    Id = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    Name = "Manufacturing Plant Alpha",
                    LegalName = "Manufacturing Plant Alpha Pvt. Ltd.",
                    Address = "123 Industrial Avenue, Industrial Zone",
                    City = "Mumbai",
                    State = "Maharashtra",
                    Country = "India",
                    PostalCode = "400001",
                    PhoneNumber = "+91-22-1234-5678",
                    Email = "info@plantaalpha.com",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Organization
                {
                    Id = Guid.Parse("70000000-0000-0000-0000-000000000002"),
                    Name = "Manufacturing Plant Beta",
                    LegalName = "Manufacturing Plant Beta Pvt. Ltd.",
                    Address = "456 Industrial Park, Tech Zone",
                    City = "Bangalore",
                    State = "Karnataka",
                    Country = "India",
                    PostalCode = "560001",
                    PhoneNumber = "+91-80-5678-1234",
                    Email = "info@plantbeta.com",
                    CreatedAt = seedDate,
                    IsDeleted = false
                }
            );
        }

        /// <summary>
        /// Seeds departments into the database.
        /// </summary>
        private static void SeedDepartments(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            var orgAlphaId = Guid.Parse("70000000-0000-0000-0000-000000000001");
            var orgBetaId = Guid.Parse("70000000-0000-0000-0000-000000000002");

            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    Id = Guid.Parse("80000000-0000-0000-0000-000000000001"),
                    Name = "Production",
                    Code = "PROD",
                    Description = "Main production department responsible for manufacturing operations.",
                    ManagerName = "Rajesh Kumar",
                    Email = "rajesh.kumar@plantaalpha.com",
                    OrganizationId = orgAlphaId,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Department
                {
                    Id = Guid.Parse("80000000-0000-0000-0000-000000000002"),
                    Name = "Quality Assurance",
                    Code = "QA",
                    Description = "Quality assurance and testing department.",
                    ManagerName = "Priya Singh",
                    Email = "priya.singh@plantaalpha.com",
                    OrganizationId = orgAlphaId,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Department
                {
                    Id = Guid.Parse("80000000-0000-0000-0000-000000000003"),
                    Name = "Maintenance",
                    Code = "MAINT",
                    Description = "Equipment maintenance and support department.",
                    ManagerName = "Arjun Patel",
                    Email = "arjun.patel@plantaalpha.com",
                    OrganizationId = orgAlphaId,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Department
                {
                    Id = Guid.Parse("80000000-0000-0000-0000-000000000004"),
                    Name = "Production",
                    Code = "PROD",
                    Description = "Main production department at Beta facility.",
                    ManagerName = "Vikram Desai",
                    Email = "vikram.desai@plantbeta.com",
                    OrganizationId = orgBetaId,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Department
                {
                    Id = Guid.Parse("80000000-0000-0000-0000-000000000005"),
                    Name = "Logistics",
                    Code = "LOG",
                    Description = "Logistics and warehouse operations.",
                    ManagerName = "Meera Nair",
                    Email = "meera.nair@plantbeta.com",
                    OrganizationId = orgBetaId,
                    CreatedAt = seedDate,
                    IsDeleted = false
                }
            );
        }

        /// <summary>
        /// Seeds production lines into the database.
        /// </summary>
        private static void SeedProductionLines(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            var deptProdAlphaId = Guid.Parse("80000000-0000-0000-0000-000000000001");
            var deptQaAlphaId = Guid.Parse("80000000-0000-0000-0000-000000000002");
            var deptMaintAlphaId = Guid.Parse("80000000-0000-0000-0000-000000000003");
            var deptProdBetaId = Guid.Parse("80000000-0000-0000-0000-000000000004");

            modelBuilder.Entity<ProductionLine>().HasData(
                new ProductionLine
                {
                    Id = Guid.Parse("90000000-0000-0000-0000-000000000001"),
                    Name = "Assembly Line 1",
                    Code = "AL1",
                    Description = "Primary assembly line for main product manufacturing.",
                    SupervisorName = "Suresh Rao",
                    DepartmentId = deptProdAlphaId,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new ProductionLine
                {
                    Id = Guid.Parse("90000000-0000-0000-0000-000000000002"),
                    Name = "Assembly Line 2",
                    Code = "AL2",
                    Description = "Secondary assembly line for alternative products.",
                    SupervisorName = "Kavya Sharma",
                    DepartmentId = deptProdAlphaId,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new ProductionLine
                {
                    Id = Guid.Parse("90000000-0000-0000-0000-000000000003"),
                    Name = "Testing Cell 1",
                    Code = "TC1",
                    Description = "Primary quality testing cell.",
                    SupervisorName = "Anita Verma",
                    DepartmentId = deptQaAlphaId,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new ProductionLine
                {
                    Id = Guid.Parse("90000000-0000-0000-0000-000000000004"),
                    Name = "Assembly Line 1",
                    Code = "AL1",
                    Description = "Primary assembly line at Beta facility.",
                    SupervisorName = "Ashok Kumar",
                    DepartmentId = deptProdBetaId,
                    CreatedAt = seedDate,
                    IsDeleted = false
                }
            );
        }

        /// <summary>
        /// Seeds machines into the database.
        /// </summary>
        private static void SeedMachines(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            var al1Id = Guid.Parse("90000000-0000-0000-0000-000000000001");
            var al2Id = Guid.Parse("90000000-0000-0000-0000-000000000002");
            var tc1Id = Guid.Parse("90000000-0000-0000-0000-000000000003");

            modelBuilder.Entity<Machine>().HasData(
                new Machine
                {
                    Id = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                    Name = "CNC Milling Machine M1",
                    EquipmentId = "EQP-2023-001",
                    Description = "High-precision CNC milling machine for component processing.",
                    Manufacturer = "Siemens",
                    ModelNumber = "SINUMERIK 810D",
                    InstalledDate = new DateTime(2020, 6, 15),
                    ProductionLineId = al1Id,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Machine
                {
                    Id = Guid.Parse("a0000000-0000-0000-0000-000000000002"),
                    Name = "Robotic Arm RA1",
                    EquipmentId = "EQP-2023-002",
                    Description = "Collaborative robotic arm for assembly operations.",
                    Manufacturer = "ABB",
                    ModelNumber = "IRB 1200",
                    InstalledDate = new DateTime(2021, 3, 20),
                    ProductionLineId = al1Id,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Machine
                {
                    Id = Guid.Parse("a0000000-0000-0000-0000-000000000003"),
                    Name = "Automated Conveyor System C1",
                    EquipmentId = "EQP-2023-003",
                    Description = "Automated conveyor system for part transport and handling.",
                    Manufacturer = "Bosch Rexroth",
                    ModelNumber = "AS/RS Conveyor",
                    InstalledDate = new DateTime(2019, 11, 10),
                    ProductionLineId = al2Id,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Machine
                {
                    Id = Guid.Parse("a0000000-0000-0000-0000-000000000004"),
                    Name = "Vision Inspection System V1",
                    EquipmentId = "EQP-2023-004",
                    Description = "Automated vision system for quality control inspection.",
                    Manufacturer = "Cognex",
                    ModelNumber = "In-Sight 7010",
                    InstalledDate = new DateTime(2022, 1, 5),
                    ProductionLineId = tc1Id,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Machine
                {
                    Id = Guid.Parse("a0000000-0000-0000-0000-000000000005"),
                    Name = "Hydraulic Press HP1",
                    EquipmentId = "EQP-2023-005",
                    Description = "Hydraulic press for forming and compression operations.",
                    Manufacturer = "AMADA",
                    ModelNumber = "HFE-1700",
                    InstalledDate = new DateTime(2020, 9, 12),
                    ProductionLineId = al2Id,
                    CreatedAt = seedDate,
                    IsDeleted = false
                }
            );
        }
    }
}