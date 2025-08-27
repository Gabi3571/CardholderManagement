using CardholderApi.Entities;
using FluentValidation;

namespace CardholderApi.Validators
{
    public class CardholderValidator : AbstractValidator<Cardholder>
    {
        public CardholderValidator()
        {
            RuleFor(c => c.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .MaximumLength(50)
                .WithMessage("First name cannot exceed 50 characters.");

            RuleFor(c => c.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .MaximumLength(50)
                .WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(c => c.Address)
                .NotEmpty()
                .WithMessage("Address is required.")
                .MaximumLength(200)
                .WithMessage("Address cannot exceed 200 characters.");

            RuleFor(c => c.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .MaximumLength(30)
                .WithMessage("Phone number cannot exceed 30 characters.")
                .Matches(@"^\+\d{8,30}$")
                .WithMessage("Phone number must start with + and contain only digits, no spaces.");

            RuleFor(c => c.TransactionCount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Transaction count cannot be negative.");
        }
    }
}
