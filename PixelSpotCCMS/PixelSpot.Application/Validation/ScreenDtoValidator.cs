using FluentValidation;
using PixelSpot.Application.DTOs;

namespace PixelSpot.Application.Validation;

public class CreateScreenDtoValidator : AbstractValidator<CreateScreenDto>
{
    public CreateScreenDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Screen name is required")
            .MaximumLength(100).WithMessage("Screen name cannot exceed 100 characters");
            
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Screen type is required")
            .MaximumLength(50).WithMessage("Screen type cannot exceed 50 characters");
            
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(200).WithMessage("Address cannot exceed 200 characters");
            
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters");
            
        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(100).WithMessage("State cannot exceed 100 characters");
            
        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters");
            
        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code is required")
            .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters");
            
        RuleFor(x => x.Location)
            .NotNull().WithMessage("Location is required");
            
        RuleFor(x => x.Location.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");
            
        RuleFor(x => x.Location.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");
            
        RuleFor(x => x.Size)
            .NotNull().WithMessage("Size is required");
            
        RuleFor(x => x.Size.Width)
            .GreaterThan(0).WithMessage("Width must be greater than zero");
            
        RuleFor(x => x.Size.Height)
            .GreaterThan(0).WithMessage("Height must be greater than zero");
            
        RuleFor(x => x.Size.Unit)
            .NotEmpty().WithMessage("Unit is required")
            .MaximumLength(20).WithMessage("Unit cannot exceed 20 characters");
            
        RuleFor(x => x.Pricing)
            .NotNull().WithMessage("Pricing is required");
            
        RuleFor(x => x.Pricing.HourlyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Hourly rate cannot be negative");
            
        RuleFor(x => x.Pricing.DailyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Daily rate cannot be negative");
            
        RuleFor(x => x.Pricing.WeeklyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Weekly rate cannot be negative");
            
        RuleFor(x => x.Pricing.MonthlyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Monthly rate cannot be negative");
            
        RuleFor(x => x.Pricing.MinimumBookingFee)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum booking fee cannot be negative");
            
        // Skip validation for availabilities if null
        When(x => x.Availabilities != null, () => {
            RuleForEach(x => x.Availabilities).ChildRules(validator => {
                validator.RuleFor(a => a.StartTime)
                    .LessThan(a => a.EndTime)
                    .WithMessage("Start time must be before end time");
            });
        });
    }
}

public class UpdateScreenDtoValidator : AbstractValidator<UpdateScreenDto>
{
    public UpdateScreenDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Screen name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));
            
        RuleFor(x => x.Type)
            .MaximumLength(50).WithMessage("Screen type cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Type));
            
        RuleFor(x => x.Address)
            .MaximumLength(200).WithMessage("Address cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Address));
            
        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.City));
            
        RuleFor(x => x.State)
            .MaximumLength(100).WithMessage("State cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.State));
            
        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Country));
            
        RuleFor(x => x.PostalCode)
            .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.PostalCode));
            
        When(x => x.Location != null, () => {
            RuleFor(x => x.Location.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");
            
            RuleFor(x => x.Location.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");
        });
            
        When(x => x.Size != null, () => {
            RuleFor(x => x.Size.Width)
                .GreaterThan(0).WithMessage("Width must be greater than zero");
            
            RuleFor(x => x.Size.Height)
                .GreaterThan(0).WithMessage("Height must be greater than zero");
        });
            
        When(x => x.Size != null, () => {
            RuleFor(x => x.Size.Unit)
                .NotEmpty().WithMessage("Unit is required")
                .MaximumLength(20).WithMessage("Unit cannot exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.Size.Unit));
        });
    }
}

public class UpdateScreenPricingDtoValidator : AbstractValidator<UpdateScreenPricingDto>
{
    public UpdateScreenPricingDtoValidator()
    {
        RuleFor(x => x.HourlyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Hourly rate cannot be negative");
            
        RuleFor(x => x.DailyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Daily rate cannot be negative");
            
        RuleFor(x => x.WeeklyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Weekly rate cannot be negative");
            
        RuleFor(x => x.MonthlyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Monthly rate cannot be negative");
            
        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .MaximumLength(10).WithMessage("Currency cannot exceed 10 characters");
            
        RuleFor(x => x.MinimumBookingFee)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum booking fee cannot be negative");
    }
}

public class CreateScreenAvailabilityDtoValidator : AbstractValidator<CreateScreenAvailabilityDto>
{
    public CreateScreenAvailabilityDtoValidator()
    {
        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime).WithMessage("Start time must be before end time");
    }
}

public class CreateScreenMetricsDtoValidator : AbstractValidator<CreateScreenMetricsDto>
{
    public CreateScreenMetricsDtoValidator()
    {
        RuleFor(x => x.EstimatedViews)
            .GreaterThanOrEqualTo(0).WithMessage("Estimated views cannot be negative");
            
        RuleFor(x => x.EstimatedImpressions)
            .GreaterThanOrEqualTo(0).WithMessage("Estimated impressions cannot be negative");
            
        RuleFor(x => x.EstimatedEngagement)
            .GreaterThanOrEqualTo(0).WithMessage("Estimated engagement cannot be negative");
    }
}

public class ScreenSearchDtoValidator : AbstractValidator<ScreenSearchDto>
{
    public ScreenSearchDtoValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1");
            
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");
            
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90")
            .When(x => x.Latitude.HasValue);
            
        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180")
            .When(x => x.Longitude.HasValue);
            
        RuleFor(x => x.RadiusInKm)
            .GreaterThan(0).WithMessage("Radius must be greater than zero")
            .When(x => x.RadiusInKm.HasValue);
            
        RuleFor(x => x.Longitude)
            .NotNull().WithMessage("Longitude is required when latitude is provided")
            .When(x => x.Latitude.HasValue);
            
        RuleFor(x => x.Latitude)
            .NotNull().WithMessage("Latitude is required when longitude is provided")
            .When(x => x.Longitude.HasValue);
            
        RuleFor(x => x.RadiusInKm)
            .NotNull().WithMessage("Radius is required when coordinates are provided")
            .When(x => x.Latitude.HasValue && x.Longitude.HasValue);
            
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate).WithMessage("Start date must be before or equal to end date")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}
