using EHS.Application.DTOs;
using FluentValidation;

namespace EHS.Application.Validators
{
    public class AssignRoleRequestValidator : AbstractValidator<AssignRoleRequest>
    {
        public AssignRoleRequestValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role name is required")
                .Must(BeValidRole).WithMessage("Invalid role name. Valid roles are: Admin, SafetyOfficer, Implementor, Initiator");
        }

        private bool BeValidRole(string roleName)
        {
            var validRoles = new[] { "Admin", "SafetyOfficer", "Implementor", "Initiator" };
            return validRoles.Contains(roleName);
        }
    }

    public class RemoveRoleRequestValidator : AbstractValidator<RemoveRoleRequest>
    {
        public RemoveRoleRequestValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role name is required")
                .Must(BeValidRole).WithMessage("Invalid role name. Valid roles are: Admin, SafetyOfficer, Implementor, Initiator");
        }

        private bool BeValidRole(string roleName)
        {
            var validRoles = new[] { "Admin", "SafetyOfficer", "Implementor", "Initiator" };
            return validRoles.Contains(roleName);
        }
    }
}
