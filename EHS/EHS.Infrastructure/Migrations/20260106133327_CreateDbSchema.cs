using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EHS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateDbSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Benefits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Benefits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClosureActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClosureActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncidentNatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentNatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncidentSeverities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeverityLevel = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentSeverities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncidentStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StageOrder = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncidentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManagerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentSafetyOfficers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SafetyOfficerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnassignedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentSafetyOfficers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentSafetyOfficers_AspNetUsers_SafetyOfficerId",
                        column: x => x.SafetyOfficerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentSafetyOfficers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupervisorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionLines_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModelNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstalledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductionLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Machines_ProductionLines_ProductionLineId",
                        column: x => x.ProductionLineId,
                        principalTable: "ProductionLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Incidents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidentNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IncidentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidentNatureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidentSeverityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidentStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IncidentTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductionLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MachineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IncidentArea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProposedSolution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InitiatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedToSafetyOfficerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedToImplementorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ImplementorAssignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewerComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosureComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incidents_AspNetUsers_AssignedToImplementorId",
                        column: x => x.AssignedToImplementorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Incidents_AspNetUsers_AssignedToSafetyOfficerId",
                        column: x => x.AssignedToSafetyOfficerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Incidents_AspNetUsers_InitiatedByUserId",
                        column: x => x.InitiatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidents_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidents_IncidentNatures_IncidentNatureId",
                        column: x => x.IncidentNatureId,
                        principalTable: "IncidentNatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidents_IncidentSeverities_IncidentSeverityId",
                        column: x => x.IncidentSeverityId,
                        principalTable: "IncidentSeverities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidents_IncidentStatuses_IncidentStatusId",
                        column: x => x.IncidentStatusId,
                        principalTable: "IncidentStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidents_IncidentTypes_IncidentTypeId",
                        column: x => x.IncidentTypeId,
                        principalTable: "IncidentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidents_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Incidents_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidents_ProductionLines_ProductionLineId",
                        column: x => x.ProductionLineId,
                        principalTable: "ProductionLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "IncidentComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IncidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentedAsRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsInternal = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentComments_AspNetUsers_CommentedByUserId",
                        column: x => x.CommentedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidentComments_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncidentImplementations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstimatedDaysToComplete = table.Column<int>(type: "int", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RootCauseAnalysis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectiveActionsDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClosureActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdditionalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImplementedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentImplementations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentImplementations_AspNetUsers_ImplementedByUserId",
                        column: x => x.ImplementedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidentImplementations_ClosureActions_ClosureActionId",
                        column: x => x.ClosureActionId,
                        principalTable: "ClosureActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidentImplementations_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropertyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuditLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogDetails_AuditLogs_AuditLogId",
                        column: x => x.AuditLogId,
                        principalTable: "AuditLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImplementationBenefits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImplementationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BenefitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BenefitValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImplementationBenefits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImplementationBenefits_Benefits_BenefitId",
                        column: x => x.BenefitId,
                        principalTable: "Benefits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImplementationBenefits_IncidentImplementations_ImplementationId",
                        column: x => x.ImplementationId,
                        principalTable: "IncidentImplementations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RootCauseAnalysisDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RootCause = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectiveMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponsibleParty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImplementationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RootCauseAnalysisDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RootCauseAnalysisDetails_IncidentImplementations_ImplementationId",
                        column: x => x.ImplementationId,
                        principalTable: "IncidentImplementations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Benefits",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsDeleted", "ModifiedAt", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("60000000-0000-0000-0000-000000000001"), "Safety", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Decrease in the number of workplace injuries due to implemented controls.", false, null, null, "Reduced Injury Rate" },
                    { new Guid("60000000-0000-0000-0000-000000000002"), "Efficiency", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Streamlined processes leading to increased productivity and reduced waste.", false, null, null, "Improved Process Efficiency" },
                    { new Guid("60000000-0000-0000-0000-000000000003"), "Quality", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Improved product or service quality as a result of corrective actions.", false, null, null, "Enhanced Quality" },
                    { new Guid("60000000-0000-0000-0000-000000000004"), "Financial", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Financial savings from reduction in accidents, downtime, or resource consumption.", false, null, null, "Cost Savings" },
                    { new Guid("60000000-0000-0000-0000-000000000005"), "HR", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Enhanced employee confidence and satisfaction due to safer working conditions.", false, null, null, "Improved Employee Morale" },
                    { new Guid("60000000-0000-0000-0000-000000000006"), "Compliance", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Enhanced compliance with regulations, standards, and company policies.", false, null, null, "Better Compliance" }
                });

            migrationBuilder.InsertData(
                table: "ClosureActions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsDeleted", "ModifiedAt", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Implementation of engineering controls to eliminate or reduce hazards (e.g., machine guards, interlocks).", false, null, null, "Engineering Control" },
                    { new Guid("50000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Implementation of work procedures, policies, or process changes to reduce risk.", false, null, null, "Administrative Control" },
                    { new Guid("50000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Provision of training or awareness programs to workers.", false, null, null, "Training" },
                    { new Guid("50000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Provision of appropriate personal protective equipment.", false, null, null, "Personal Protective Equipment (PPE)" },
                    { new Guid("50000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Preventive or corrective maintenance of equipment or facilities.", false, null, null, "Maintenance" },
                    { new Guid("50000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Modification of processes or workflows to enhance safety and efficiency.", false, null, null, "Process Improvement" }
                });

            migrationBuilder.InsertData(
                table: "IncidentNatures",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsDeleted", "ModifiedAt", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "First occurrence of this type of incident.", false, null, null, "New" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Similar incident has occurred previously.", false, null, null, "Repeated" }
                });

            migrationBuilder.InsertData(
                table: "IncidentSeverities",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsDeleted", "ModifiedAt", "ModifiedBy", "Name", "SeverityLevel" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Minor incident with minimal impact. Low risk of injury or damage.", false, null, null, "Minor", 1 },
                    { new Guid("30000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Major incident with significant impact. Moderate risk of serious injury or substantial damage.", false, null, null, "Major", 2 },
                    { new Guid("30000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Catastrophic incident with severe impact. High risk of fatality or major damage. Requires immediate intervention.", false, null, null, "Catastrophic", 3 }
                });

            migrationBuilder.InsertData(
                table: "IncidentStatuses",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsDeleted", "ModifiedAt", "ModifiedBy", "Name", "StageOrder" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Incident has been submitted by the initiator and is awaiting review.", false, null, null, "Submitted", 1 },
                    { new Guid("40000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Incident has been assigned to a safety officer for review and approval.", false, null, null, "Assigned", 2 },
                    { new Guid("40000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Incident has been approved by the safety officer and assigned to an implementor.", false, null, null, "Approved", 3 },
                    { new Guid("40000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Implementation is in progress. Implementor is working on corrective actions.", false, null, null, "InProgress", 4 },
                    { new Guid("40000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Implementation is complete. Awaiting safety officer verification.", false, null, null, "VerificationPending", 5 },
                    { new Guid("40000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Incident has been verified and closed by the safety officer.", false, null, null, "Closed", 6 },
                    { new Guid("40000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Incident has been rejected and requires resubmission or correction.", false, null, null, "Rejected", 0 }
                });

            migrationBuilder.InsertData(
                table: "IncidentTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsDeleted", "ModifiedAt", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "An incident where no injury occurred, but had potential for harm.", false, null, null, "Near Miss" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "An unsafe environmental or equipment condition that could cause harm.", false, null, null, "Unsafe Condition" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "An unsafe behavior or action performed by an individual.", false, null, null, "Unsafe Action" }
                });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "Address", "City", "Country", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Email", "IsDeleted", "LegalName", "ModifiedAt", "ModifiedBy", "Name", "PhoneNumber", "PostalCode", "State" },
                values: new object[,]
                {
                    { new Guid("70000000-0000-0000-0000-000000000001"), "123 Industrial Avenue, Industrial Zone", "Mumbai", "India", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "info@plantaalpha.com", false, "Manufacturing Plant Alpha Pvt. Ltd.", null, null, "Manufacturing Plant Alpha", "+91-22-1234-5678", "400001", "Maharashtra" },
                    { new Guid("70000000-0000-0000-0000-000000000002"), "456 Industrial Park, Tech Zone", "Bangalore", "India", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "info@plantbeta.com", false, "Manufacturing Plant Beta Pvt. Ltd.", null, null, "Manufacturing Plant Beta", "+91-80-5678-1234", "560001", "Karnataka" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "Email", "IsDeleted", "ManagerName", "ModifiedAt", "ModifiedBy", "Name", "OrganizationId" },
                values: new object[,]
                {
                    { new Guid("80000000-0000-0000-0000-000000000001"), "PROD", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Main production department responsible for manufacturing operations.", "rajesh.kumar@plantaalpha.com", false, "Rajesh Kumar", null, null, "Production", new Guid("70000000-0000-0000-0000-000000000001") },
                    { new Guid("80000000-0000-0000-0000-000000000002"), "QA", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Quality assurance and testing department.", "priya.singh@plantaalpha.com", false, "Priya Singh", null, null, "Quality Assurance", new Guid("70000000-0000-0000-0000-000000000001") },
                    { new Guid("80000000-0000-0000-0000-000000000003"), "MAINT", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Equipment maintenance and support department.", "arjun.patel@plantaalpha.com", false, "Arjun Patel", null, null, "Maintenance", new Guid("70000000-0000-0000-0000-000000000001") },
                    { new Guid("80000000-0000-0000-0000-000000000004"), "PROD", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Main production department at Beta facility.", "vikram.desai@plantbeta.com", false, "Vikram Desai", null, null, "Production", new Guid("70000000-0000-0000-0000-000000000002") },
                    { new Guid("80000000-0000-0000-0000-000000000005"), "LOG", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Logistics and warehouse operations.", "meera.nair@plantbeta.com", false, "Meera Nair", null, null, "Logistics", new Guid("70000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "ProductionLines",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DepartmentId", "Description", "IsDeleted", "ModifiedAt", "ModifiedBy", "Name", "SupervisorName" },
                values: new object[,]
                {
                    { new Guid("90000000-0000-0000-0000-000000000001"), "AL1", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, new Guid("80000000-0000-0000-0000-000000000001"), "Primary assembly line for main product manufacturing.", false, null, null, "Assembly Line 1", "Suresh Rao" },
                    { new Guid("90000000-0000-0000-0000-000000000002"), "AL2", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, new Guid("80000000-0000-0000-0000-000000000001"), "Secondary assembly line for alternative products.", false, null, null, "Assembly Line 2", "Kavya Sharma" },
                    { new Guid("90000000-0000-0000-0000-000000000003"), "TC1", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, new Guid("80000000-0000-0000-0000-000000000002"), "Primary quality testing cell.", false, null, null, "Testing Cell 1", "Anita Verma" },
                    { new Guid("90000000-0000-0000-0000-000000000004"), "AL1", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, new Guid("80000000-0000-0000-0000-000000000004"), "Primary assembly line at Beta facility.", false, null, null, "Assembly Line 1", "Ashok Kumar" }
                });

            migrationBuilder.InsertData(
                table: "Machines",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "EquipmentId", "InstalledDate", "IsDeleted", "Manufacturer", "ModelNumber", "ModifiedAt", "ModifiedBy", "Name", "ProductionLineId" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "High-precision CNC milling machine for component processing.", "EQP-2023-001", new DateTime(2020, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Siemens", "SINUMERIK 810D", null, null, "CNC Milling Machine M1", new Guid("90000000-0000-0000-0000-000000000001") },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Collaborative robotic arm for assembly operations.", "EQP-2023-002", new DateTime(2021, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "ABB", "IRB 1200", null, null, "Robotic Arm RA1", new Guid("90000000-0000-0000-0000-000000000001") },
                    { new Guid("a0000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Automated conveyor system for part transport and handling.", "EQP-2023-003", new DateTime(2019, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Bosch Rexroth", "AS/RS Conveyor", null, null, "Automated Conveyor System C1", new Guid("90000000-0000-0000-0000-000000000002") },
                    { new Guid("a0000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Automated vision system for quality control inspection.", "EQP-2023-004", new DateTime(2022, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Cognex", "In-Sight 7010", null, null, "Vision Inspection System V1", new Guid("90000000-0000-0000-0000-000000000003") },
                    { new Guid("a0000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Hydraulic press for forming and compression operations.", "EQP-2023-005", new DateTime(2020, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "AMADA", "HFE-1700", null, null, "Hydraulic Press HP1", new Guid("90000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogDetails_AuditLogId",
                table: "AuditLogDetails",
                column: "AuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityId",
                table: "AuditLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_IncidentId",
                table: "AuditLogs",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_OrganizationId_Code",
                table: "Departments",
                columns: new[] { "OrganizationId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentSafetyOfficers_DepartmentId",
                table: "DepartmentSafetyOfficers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentSafetyOfficers_SafetyOfficerId",
                table: "DepartmentSafetyOfficers",
                column: "SafetyOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_ImplementationBenefits_BenefitId",
                table: "ImplementationBenefits",
                column: "BenefitId");

            migrationBuilder.CreateIndex(
                name: "IX_ImplementationBenefits_ImplementationId",
                table: "ImplementationBenefits",
                column: "ImplementationId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentComments_CommentedByUserId",
                table: "IncidentComments",
                column: "CommentedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentComments_IncidentId",
                table: "IncidentComments",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentImplementations_ClosureActionId",
                table: "IncidentImplementations",
                column: "ClosureActionId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentImplementations_ImplementedByUserId",
                table: "IncidentImplementations",
                column: "ImplementedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentImplementations_IncidentId",
                table: "IncidentImplementations",
                column: "IncidentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_AssignedToImplementorId",
                table: "Incidents",
                column: "AssignedToImplementorId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_AssignedToSafetyOfficerId",
                table: "Incidents",
                column: "AssignedToSafetyOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_DepartmentId",
                table: "Incidents",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_IncidentDate",
                table: "Incidents",
                column: "IncidentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_IncidentNatureId",
                table: "Incidents",
                column: "IncidentNatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_IncidentNumber",
                table: "Incidents",
                column: "IncidentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_IncidentSeverityId",
                table: "Incidents",
                column: "IncidentSeverityId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_IncidentStatusId",
                table: "Incidents",
                column: "IncidentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_IncidentTypeId",
                table: "Incidents",
                column: "IncidentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_InitiatedByUserId",
                table: "Incidents",
                column: "InitiatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_MachineId",
                table: "Incidents",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_OrganizationId",
                table: "Incidents",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_ProductionLineId",
                table: "Incidents",
                column: "ProductionLineId");

            migrationBuilder.CreateIndex(
                name: "IX_Machines_EquipmentId",
                table: "Machines",
                column: "EquipmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Machines_ProductionLineId",
                table: "Machines",
                column: "ProductionLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionLines_DepartmentId_Code",
                table: "ProductionLines",
                columns: new[] { "DepartmentId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RootCauseAnalysisDetails_ImplementationId",
                table: "RootCauseAnalysisDetails",
                column: "ImplementationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogDetails");

            migrationBuilder.DropTable(
                name: "DepartmentSafetyOfficers");

            migrationBuilder.DropTable(
                name: "ImplementationBenefits");

            migrationBuilder.DropTable(
                name: "IncidentComments");

            migrationBuilder.DropTable(
                name: "RootCauseAnalysisDetails");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Benefits");

            migrationBuilder.DropTable(
                name: "IncidentImplementations");

            migrationBuilder.DropTable(
                name: "ClosureActions");

            migrationBuilder.DropTable(
                name: "Incidents");

            migrationBuilder.DropTable(
                name: "IncidentNatures");

            migrationBuilder.DropTable(
                name: "IncidentSeverities");

            migrationBuilder.DropTable(
                name: "IncidentStatuses");

            migrationBuilder.DropTable(
                name: "IncidentTypes");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "ProductionLines");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
