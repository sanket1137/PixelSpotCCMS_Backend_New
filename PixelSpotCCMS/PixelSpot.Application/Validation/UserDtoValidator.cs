using FluentValidation;
using PixelSpot.Application.DTOs;

namespace PixelSpot.Application.Validation;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email address is required");
            
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
            
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");
            
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");
            
        RuleFor(x => x.CompanyName)
            .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters");
            
        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters");
            
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(role => role == "Advertiser" || role == "ScreenOwner" || role == "Admin")
            .WithMessage("Role must be either 'Advertiser', 'ScreenOwner', or 'Admin'");
    }
}

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.FirstName));
            
        RuleFor(x => x.LastName)
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.LastName));
            
        RuleFor(x => x.CompanyName)
            .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.CompanyName));
            
        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}

public class UpdateBankDetailsDtoValidator : AbstractValidator<UpdateBankDetailsDto>
{
    public UpdateBankDetailsDtoValidator()
    {
        RuleFor(x => x.AccountHolderName)
            .NotEmpty().WithMessage("Account holder name is required")
            .MaximumLength(100).WithMessage("Account holder name cannot exceed 100 characters");
            
        RuleFor(x => x.BankName)
            .NotEmpty().WithMessage("Bank name is required")
            .MaximumLength(100).WithMessage("Bank name cannot exceed 100 characters");
            
        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Account number is required")
            .MaximumLength(50).WithMessage("Account number cannot exceed 50 characters");
            
        RuleFor(x => x.RoutingNumber)
            .NotEmpty().WithMessage("Routing number is required")
            .MaximumLength(50).WithMessage("Routing number cannot exceed 50 characters");
    }
}

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required");
            
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
            
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage("Password and confirmation password do not match");
    }
}

public class CreateSubUserDtoValidator : AbstractValidator<CreateSubUserDto>
{
    public CreateSubUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email address is required");
            
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
            
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");
            
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");
    }
}

public class UpdateSubUserDtoValidator : AbstractValidator<UpdateSubUserDto>
{
    public UpdateSubUserDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.FirstName));
            
        RuleFor(x => x.LastName)
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.LastName));
            
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("A valid email address is required")
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}
