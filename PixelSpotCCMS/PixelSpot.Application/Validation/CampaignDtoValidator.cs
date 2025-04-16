using FluentValidation;
using PixelSpot.Application.DTOs;

namespace PixelSpot.Application.Validation;

public class CreateCampaignDtoValidator : AbstractValidator<CreateCampaignDto>
{
    public CreateCampaignDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Campaign name is required")
            .MaximumLength(100).WithMessage("Campaign name cannot exceed 100 characters");
            
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .LessThan(x => x.EndDate).WithMessage("Start date must be before end date");
            
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required");
            
        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than zero");
    }
}

public class UpdateCampaignDtoValidator : AbstractValidator<UpdateCampaignDto>
{
    public UpdateCampaignDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Campaign name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));
            
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate).WithMessage("Start date must be before end date")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
            
        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than zero")
            .When(x => x.Budget.HasValue);
            
        RuleFor(x => x.Status)
            .Must(status => status == "Draft" || status == "Active" || status == "Completed" || status == "Cancelled")
            .WithMessage("Status must be either 'Draft', 'Active', 'Completed', or 'Cancelled'")
            .When(x => !string.IsNullOrEmpty(x.Status));
    }
}

public class CreateCreativeDtoValidator : AbstractValidator<CreateCreativeDto>
{
    public CreateCreativeDtoValidator()
    {
        RuleFor(x => x.CampaignId)
            .NotEmpty().WithMessage("Campaign ID is required");
            
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Creative name is required")
            .MaximumLength(100).WithMessage("Creative name cannot exceed 100 characters");
            
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Creative type is required")
            .Must(type => type == "Image" || type == "Video" || type == "HTML")
            .WithMessage("Creative type must be either 'Image', 'Video', or 'HTML'");
            
        RuleFor(x => x.ContentUrl)
            .NotEmpty().WithMessage("Content URL is required")
            .MaximumLength(500).WithMessage("Content URL cannot exceed 500 characters");
            
        RuleFor(x => x.DurationSeconds)
            .GreaterThan(0).WithMessage("Duration must be greater than zero");
    }
}

public class UpdateCreativeDtoValidator : AbstractValidator<UpdateCreativeDto>
{
    public UpdateCreativeDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Creative name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));
            
        RuleFor(x => x.Type)
            .Must(type => type == "Image" || type == "Video" || type == "HTML")
            .WithMessage("Creative type must be either 'Image', 'Video', or 'HTML'")
            .When(x => !string.IsNullOrEmpty(x.Type));
            
        RuleFor(x => x.ContentUrl)
            .MaximumLength(500).WithMessage("Content URL cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ContentUrl));
            
        RuleFor(x => x.DurationSeconds)
            .GreaterThan(0).WithMessage("Duration must be greater than zero")
            .When(x => x.DurationSeconds.HasValue);
    }
}

public class ApproveRejectCreativeDtoValidator : AbstractValidator<ApproveRejectCreativeDto>
{
    public ApproveRejectCreativeDtoValidator()
    {
        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("Rejection reason is required when rejecting a creative")
            .When(x => !x.IsApproved);
    }
}

public class CreateScreenBookingDtoValidator : AbstractValidator<CreateScreenBookingDto>
{
    public CreateScreenBookingDtoValidator()
    {
        RuleFor(x => x.ScreenId)
            .NotEmpty().WithMessage("Screen ID is required");
            
        RuleFor(x => x.CampaignId)
            .NotEmpty().WithMessage("Campaign ID is required");
            
        RuleFor(x => x.CreativeId)
            .NotEmpty().WithMessage("Creative ID is required");
            
        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required")
            .LessThan(x => x.EndTime).WithMessage("Start time must be before end time");
            
        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required");
    }
}

public class UpdateScreenBookingStatusDtoValidator : AbstractValidator<UpdateScreenBookingStatusDto>
{
    public UpdateScreenBookingStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(status => status == "Pending" || status == "Confirmed" || status == "Cancelled" || status == "Completed")
            .WithMessage("Status must be either 'Pending', 'Confirmed', 'Cancelled', or 'Completed'");
    }
}

public class UpdateScreenBookingPaymentDtoValidator : AbstractValidator<UpdateScreenBookingPaymentDto>
{
    public UpdateScreenBookingPaymentDtoValidator()
    {
        RuleFor(x => x.PaymentStatus)
            .NotEmpty().WithMessage("Payment status is required")
            .Must(status => status == "Pending" || status == "Paid" || status == "Refunded")
            .WithMessage("Payment status must be either 'Pending', 'Paid', or 'Refunded'");
    }
}

public class CreateCampaignRequestDtoValidator : AbstractValidator<CreateCampaignRequestDto>
{
    public CreateCampaignRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Campaign request name is required")
            .MaximumLength(100).WithMessage("Campaign request name cannot exceed 100 characters");
            
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .LessThan(x => x.EndDate).WithMessage("Start date must be before end date");
            
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required");
            
        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than zero");
    }
}

public class UpdateCampaignRequestDtoValidator : AbstractValidator<UpdateCampaignRequestDto>
{
    public UpdateCampaignRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Campaign request name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));
            
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate).WithMessage("Start date must be before end date")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
            
        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than zero")
            .When(x => x.Budget.HasValue);
    }
}

public class ApproveCampaignRequestDtoValidator : AbstractValidator<ApproveCampaignRequestDto>
{
    public ApproveCampaignRequestDtoValidator()
    {
        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("Rejection reason is required when rejecting a campaign request")
            .When(x => !x.IsApproved);
    }
}

public class CreateWaitlistEntryDtoValidator : AbstractValidator<CreateWaitlistEntryDto>
{
    public CreateWaitlistEntryDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email address is required");
            
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");
            
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");
            
        RuleFor(x => x.UserType)
            .NotEmpty().WithMessage("User type is required")
            .Must(type => type == "Advertiser" || type == "ScreenOwner" || type == "Both")
            .WithMessage("User type must be either 'Advertiser', 'ScreenOwner', or 'Both'");
    }
}

public class UpdateWaitlistEntryDtoValidator : AbstractValidator<UpdateWaitlistEntryDto>
{
    public UpdateWaitlistEntryDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.FirstName));
            
        RuleFor(x => x.LastName)
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.LastName));
            
        RuleFor(x => x.UserType)
            .Must(type => type == "Advertiser" || type == "ScreenOwner" || type == "Both")
            .WithMessage("User type must be either 'Advertiser', 'ScreenOwner', or 'Both'")
            .When(x => !string.IsNullOrEmpty(x.UserType));
    }
}

public class UpdateWaitlistEntryStatusDtoValidator : AbstractValidator<UpdateWaitlistEntryStatusDto>
{
    public UpdateWaitlistEntryStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(status => status == "Pending" || status == "Invited" || status == "Registered")
            .WithMessage("Status must be either 'Pending', 'Invited', or 'Registered'");
    }
}
