using System;

namespace FlightBooking.Core
{
    public interface CartExpired
    {
        Guid CartId { get; }
    }
}
