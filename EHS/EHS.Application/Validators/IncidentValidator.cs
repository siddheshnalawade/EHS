using EHS.Application.DTOs;
using FluentValidation;

namespace EHS.Application.Validators
{
    public class CreateIncidentRequestValidator : AbstractValidator<CreateIncidentRequest>
    {
        public CreateIncidentRequestValidator()
        {
            RuleFor(x => x.IncidentTypeId)
                .NotEmpty().WithMessage("Incident type is required.");

            RuleFor(x => x.IncidentNatureId)
                .NotEmpty().WithMessage("Incident nature is required.");

            RuleFor(x => x.IncidentSeverityId)
                .NotEmpty().WithMessage("Incident severity is required.");

            RuleFor(x => x.IncidentDate)
                .NotEmpty().WithMessage("Incident date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Incident date cannot be in the future.");

            RuleFor(x => x.OrganizationId)
                .NotEmpty().WithMessage("Organization is required.");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(5, 200).WithMessage("Title must be between 5 and 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .Length(10, 2000).WithMessage("Description must be between 10 and 2000 characters.");

            RuleFor(x => x.IncidentArea)
                .NotEmpty().WithMessage("Incident area is required.")
                .MaximumLength(500).WithMessage("Incident area cannot exceed 500 characters.");

            RuleFor(x => x.ProposedSolution)
                .NotEmpty().WithMessage("Proposed solution is required.")
                .Length(10, 2000).WithMessage("Proposed solution must be between 10 and 2000 characters.");
        }
    }

    public class UpdateIncidentRequestValidator : AbstractValidator<UpdateIncidentRequest>
    {
        public UpdateIncidentRequestValidator()
        {
            RuleFor(x => x.IncidentTypeId)
                .NotEmpty().WithMessage("Incident type is required.");

            RuleFor(x => x.IncidentNatureId)
                .NotEmpty().WithMessage("Incident nature is required.");

            RuleFor(x => x.IncidentSeverityId)
                .NotEmpty().WithMessage("Incident severity is required.");

            RuleFor(x => x.IncidentDate)
                .NotEmpty().WithMessage("Incident date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Incident date cannot be in the future.");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(5, 200).WithMessage("Title must be between 5 and 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .Length(10, 2000).WithMessage("Description must be between 10 and 2000 characters.");

            RuleFor(x => x.IncidentArea)
                .NotEmpty().WithMessage("Incident area is required.")
                .MaximumLength(500).WithMessage("Incident area cannot exceed 500 characters.");

            RuleFor(x => x.ProposedSolution)
                .NotEmpty().WithMessage("Proposed solution is required.")
                .Length(10, 2000).WithMessage("Proposed solution must be between 10 and 2000 characters.");
        }
    }

    public class RejectIncidentRequestValidator : AbstractValidator<RejectIncidentRequest>
    {
        public RejectIncidentRequestValidator()
        {
            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Rejection comment is required.")
                .Length(10, 1000).WithMessage("Comment must be between 10 and 1000 characters.");
        }
    }

    public class ReassignToInitiatorRequestValidator : AbstractValidator<ReassignToInitiatorRequest>
    {
        public ReassignToInitiatorRequestValidator()
        {
            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Reassignment comment is required.")
                .Length(10, 1000).WithMessage("Comment must be between 10 and 1000 characters.");
        }
    }

    public class AcceptAndAssignRequestValidator : AbstractValidator<AcceptAndAssignRequest>
    {
        public AcceptAndAssignRequestValidator()
        {
            RuleFor(x => x.ImplementorId)
                .NotEmpty().WithMessage("Implementor is required.");

            RuleFor(x => x.Comment)
                .MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.Comment))
                .WithMessage("Comment cannot exceed 1000 characters.");
        }
    }

    public class CloseIncidentRequestValidator : AbstractValidator<CloseIncidentRequest>
    {
        public CloseIncidentRequestValidator()
        {
            RuleFor(x => x.ClosureComment)
                .NotEmpty().WithMessage("Closure comment is required.")
                .Length(10, 1000).WithMessage("Closure comment must be between 10 and 1000 characters.");
        }
    }

    public class AcceptIncidentRequestValidator : AbstractValidator<AcceptIncidentRequest>
    {
        public AcceptIncidentRequestValidator()
        {
            RuleFor(x => x.EstimatedDaysToComplete)
                .GreaterThan(0).WithMessage("Estimated days must be greater than 0.")
                .LessThanOrEqualTo(365).WithMessage("Estimated days cannot exceed 365 days.");
        }
    }

    public class PassToPeerRequestValidator : AbstractValidator<PassToPeerRequest>
    {
        public PassToPeerRequestValidator()
        {
            RuleFor(x => x.PeerImplementorId)
                .NotEmpty().WithMessage("Peer implementor is required.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment is required when passing to peer.")
                .Length(10, 1000).WithMessage("Comment must be between 10 and 1000 characters.");
        }
    }

    public class UpdateImplementationRequestValidator : AbstractValidator<UpdateImplementationRequest>
    {
        public UpdateImplementationRequestValidator()
        {
            RuleFor(x => x.RootCauseAnalysis)
                .NotEmpty().WithMessage("Root cause analysis is required.")
                .Length(20, 2000).WithMessage("Root cause analysis must be between 20 and 2000 characters.");

            RuleFor(x => x.CorrectiveActionsDescription)
                .NotEmpty().WithMessage("Corrective actions description is required.")
                .Length(20, 2000).WithMessage("Corrective actions must be between 20 and 2000 characters.");

            RuleFor(x => x.ClosureActionId)
                .NotEmpty().WithMessage("Closure action is required.");

            RuleFor(x => x.BenefitIds)
                .NotEmpty().WithMessage("At least one benefit must be selected.");

            RuleFor(x => x.RootCauseDetails)
                .NotEmpty().WithMessage("Root cause details are required.")
                .Must(x => x.Count >= 1).WithMessage("At least one root cause detail is required.");

            RuleForEach(x => x.RootCauseDetails).ChildRules(detail =>
            {
                detail.RuleFor(x => x.Why)
                    .NotEmpty().WithMessage("Why field is required.")
                    .MaximumLength(100).WithMessage("Why field cannot exceed 100 characters.");

                detail.RuleFor(x => x.Analysis)
                    .NotEmpty().WithMessage("Analysis is required.")
                    .Length(10, 500).WithMessage("Analysis must be between 10 and 500 characters.");

                detail.RuleFor(x => x.CorrectiveAction)
                    .NotEmpty().WithMessage("Corrective action is required.")
                    .Length(10, 500).WithMessage("Corrective action must be between 10 and 500 characters.");
            });
        }
    }
}