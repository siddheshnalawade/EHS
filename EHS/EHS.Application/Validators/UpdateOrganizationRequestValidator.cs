using EHS.Application.DTOs;
using FluentValidation;

namespace EHS.Application.Validators
{
    /// <summary>
    /// Fluent validation rules for updating organizations.
    /// All fields are optional during update.
    /// </summary>
    public class UpdateOrganizationRequestValidator : AbstractValidator<UpdateOrganizationRequest>
    {
        public UpdateOrganizationRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 256).WithMessage("Organization name must be between 3 and 256 characters")
                .Matches(@"^[a-zA-Z0-9\s\-.,&()]*$").WithMessage("Organization name contains invalid characters");

            RuleFor(x => x.LegalName)
                .NotEmpty()
                .Length(3, 256).WithMessage("Legal name must be between 3 and 256 characters");

            RuleFor(x => x.Address)
                .NotEmpty()
                .Length(5, 500).WithMessage("Address must be between 5 and 500 characters");

            RuleFor(x => x.City)
                .NotEmpty()
                .Length(2, 100).WithMessage("City must be between 2 and 100 characters");

            RuleFor(x => x.State)
                .NotEmpty()
                .Length(2, 100).WithMessage("State must be between 2 and 100 characters");

            RuleFor(x => x.Country)
                .NotEmpty()
                .Length(2, 100).WithMessage("Country must be between 2 and 100 characters");

            RuleFor(x => x.PostalCode)
                .NotEmpty()
                .Matches(@"^\d{5,10}$").WithMessage("Postal code must be 5-10 digits");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^\+?[\d\-\s\(\)]{10,20}$").WithMessage("Phone number format is invalid");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("Email format is invalid");
        }
    }
}