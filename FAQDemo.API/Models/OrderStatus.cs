namespace FAQDemo.API.Models
{
    public enum OrderStatus
    {
        Pending,    // order just created, not processed yet
        Confirmed,  // stock verified and accepted
        Shipped,    // handed over to delivery
        Delivered,  // customer received
        Cancelled   // cancelled by user or system
    }
}
