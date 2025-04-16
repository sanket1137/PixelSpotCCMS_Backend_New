using PixelSpot.Domain.Common;

namespace PixelSpot.Domain.ValueObjects;

public class ScreenSize : ValueObject
{
    public double Width { get; private set; }
    public double Height { get; private set; }
    public string Unit { get; private set; }
    
    private ScreenSize() { }
    
    public ScreenSize(double width, double height, string unit)
    {
        if (width <= 0)
            throw new ArgumentException("Width must be greater than zero", nameof(width));
            
        if (height <= 0)
            throw new ArgumentException("Height must be greater than zero", nameof(height));
            
        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Unit cannot be empty", nameof(unit));
        
        Width = width;
        Height = height;
        Unit = unit;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Width;
        yield return Height;
        yield return Unit;
    }
    
    public double GetArea()
    {
        return Width * Height;
    }
    
    public override string ToString()
    {
        return $"{Width}x{Height} {Unit}";
    }
}
