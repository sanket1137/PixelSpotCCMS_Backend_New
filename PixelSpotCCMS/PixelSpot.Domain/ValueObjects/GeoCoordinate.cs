using PixelSpot.Domain.Common;

namespace PixelSpot.Domain.ValueObjects;

public class GeoCoordinate : ValueObject
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    
    private GeoCoordinate() { }
    
    public GeoCoordinate(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));
            
        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));
        
        Latitude = latitude;
        Longitude = longitude;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }
    
    public override string ToString()
    {
        return $"{Latitude}, {Longitude}";
    }
    
    public double CalculateDistance(GeoCoordinate other)
    {
        // Haversine formula for calculating distance between two points on a sphere
        const double earthRadiusKm = 6371.0;
        var dLat = DegreesToRadians(other.Latitude - Latitude);
        var dLon = DegreesToRadians(other.Longitude - Longitude);
        
        var lat1 = DegreesToRadians(Latitude);
        var lat2 = DegreesToRadians(other.Latitude);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        return earthRadiusKm * c;
    }
    
    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
