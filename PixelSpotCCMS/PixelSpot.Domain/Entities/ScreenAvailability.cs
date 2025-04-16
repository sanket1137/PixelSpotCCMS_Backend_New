using PixelSpot.Domain.Common;

namespace PixelSpot.Domain.Entities;

public class ScreenAvailability : Entity
{
    public Guid ScreenId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    
    // Navigation property
    public Screen Screen { get; private set; } = null!;
    
    private ScreenAvailability() { }
    
    public ScreenAvailability(Guid screenId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
    {
        if (screenId == Guid.Empty)
            throw new ArgumentException("Screen ID cannot be empty", nameof(screenId));
            
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time", nameof(startTime));
        
        ScreenId = screenId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
    }
    
    public void Update(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time", nameof(startTime));
            
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
    }
    
    public bool IsTimeSlotAvailable(DateTime startDateTime, DateTime endDateTime)
    {
        // Check if dates span multiple days
        if (startDateTime.Date != endDateTime.Date)
        {
            // For multi-day bookings, check each day
            var currentDate = startDateTime.Date;
            while (currentDate <= endDateTime.Date)
            {
                if (currentDate == startDateTime.Date)
                {
                    // First day: check from start time to end of day
                    if (currentDate.DayOfWeek == DayOfWeek && 
                        startDateTime.TimeOfDay >= StartTime && 
                        TimeSpan.FromHours(23).Add(TimeSpan.FromMinutes(59)) <= EndTime)
                        return false;
                }
                else if (currentDate == endDateTime.Date)
                {
                    // Last day: check from start of day to end time
                    if (currentDate.DayOfWeek == DayOfWeek && 
                        TimeSpan.Zero >= StartTime && 
                        endDateTime.TimeOfDay <= EndTime)
                        return false;
                }
                else
                {
                    // Middle days: check entire day
                    if (currentDate.DayOfWeek == DayOfWeek)
                        return false;
                }
                
                currentDate = currentDate.AddDays(1);
            }
            
            return true;
        }
        else
        {
            // Same day booking
            return startDateTime.DayOfWeek == DayOfWeek && 
                   startDateTime.TimeOfDay >= StartTime && 
                   endDateTime.TimeOfDay <= EndTime;
        }
    }
}
