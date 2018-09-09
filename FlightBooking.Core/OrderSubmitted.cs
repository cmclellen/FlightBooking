using System;

namespace FlightBooking.Core
{
    public interface OrderSubmitted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        Guid CartId { get; }
        string UserName { get; }
    }
}
