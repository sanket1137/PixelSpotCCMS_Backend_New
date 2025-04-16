using PixelSpot.Domain.Common;

namespace PixelSpot.Domain.Entities;

public class ScreenPricing : Entity
{
    public Guid ScreenId { get; private set; }
    public decimal HourlyRate { get; private set; }
    public decimal DailyRate { get; private set; }
    public decimal WeeklyRate { get; private set; }
    public decimal MonthlyRate { get; private set; }
    public string Currency { get; private set; } = "USD";
    public decimal MinimumBookingFee { get; private set; }
    
    // Navigation property
    public Screen Screen { get; private set; } = null!;
    
    private ScreenPricing() { }
    
    public ScreenPricing(Guid screenId, decimal hourlyRate, decimal dailyRate, 
                        decimal weeklyRate, decimal monthlyRate, string currency, 
                        decimal minimumBookingFee)
    {
        if (screenId == Guid.Empty)
            throw new ArgumentException("Screen ID cannot be empty", nameof(screenId));
            
        if (hourlyRate < 0)
            throw new ArgumentException("Hourly rate cannot be negative", nameof(hourlyRate));
            
        if (dailyRate < 0)
            throw new ArgumentException("Daily rate cannot be negative", nameof(dailyRate));
            
        if (weeklyRate < 0)
            throw new ArgumentException("Weekly rate cannot be negative", nameof(weeklyRate));
            
        if (monthlyRate < 0)
            throw new ArgumentException("Monthly rate cannot be negative", nameof(monthlyRate));
            
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));
            
        if (minimumBookingFee < 0)
            throw new ArgumentException("Minimum booking fee cannot be negative", nameof(minimumBookingFee));
        
        ScreenId = screenId;
        HourlyRate = hourlyRate;
        DailyRate = dailyRate;
        WeeklyRate = weeklyRate;
        MonthlyRate = monthlyRate;
        Currency = currency;
        MinimumBookingFee = minimumBookingFee;
    }
    
    public void Update(decimal hourlyRate, decimal dailyRate, decimal weeklyRate, 
                     decimal monthlyRate, string currency, decimal minimumBookingFee)
    {
        if (hourlyRate >= 0)
            HourlyRate = hourlyRate;
            
        if (dailyRate >= 0)
            DailyRate = dailyRate;
            
        if (weeklyRate >= 0)
            WeeklyRate = weeklyRate;
            
        if (monthlyRate >= 0)
            MonthlyRate = monthlyRate;
            
        if (!string.IsNullOrWhiteSpace(currency))
            Currency = currency;
            
        if (minimumBookingFee >= 0)
            MinimumBookingFee = minimumBookingFee;
    }
    
    public decimal CalculatePrice(DateTime startTime, DateTime endTime)
    {
        var duration = endTime - startTime;
        
        if (duration.TotalDays >= 30)
        {
            var months = Math.Ceiling(duration.TotalDays / 30);
            return MonthlyRate * (decimal)months;
        }
        else if (duration.TotalDays >= 7)
        {
            var weeks = Math.Ceiling(duration.TotalDays / 7);
            return WeeklyRate * (decimal)weeks;
        }
        else if (duration.TotalDays >= 1)
        {
            var days = Math.Ceiling(duration.TotalDays);
            return DailyRate * (decimal)days;
        }
        else
        {
            var hours = Math.Ceiling(duration.TotalHours);
            var price = HourlyRate * (decimal)hours;
            return price < MinimumBookingFee ? MinimumBookingFee : price;
        }
    }
}
