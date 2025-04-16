using PixelSpot.Domain.Common;

namespace PixelSpot.Domain.ValueObjects;

public class BankDetails : ValueObject
{
    public string AccountHolderName { get; private set; }
    public string BankName { get; private set; }
    public string AccountNumber { get; private set; }
    public string RoutingNumber { get; private set; }
    
    private BankDetails() { }
    
    public BankDetails(string accountHolderName, string bankName, string accountNumber, string routingNumber)
    {
        if (string.IsNullOrWhiteSpace(accountHolderName))
            throw new ArgumentException("Account holder name cannot be empty", nameof(accountHolderName));
            
        if (string.IsNullOrWhiteSpace(bankName))
            throw new ArgumentException("Bank name cannot be empty", nameof(bankName));
            
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account number cannot be empty", nameof(accountNumber));
            
        if (string.IsNullOrWhiteSpace(routingNumber))
            throw new ArgumentException("Routing number cannot be empty", nameof(routingNumber));
        
        AccountHolderName = accountHolderName;
        BankName = bankName;
        AccountNumber = accountNumber;
        RoutingNumber = routingNumber;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AccountHolderName;
        yield return BankName;
        yield return AccountNumber;
        yield return RoutingNumber;
    }
}
