using System;
using System.Collections.Generic;
using System.Text;

namespace FlightBooking.Core
{
    public interface CartItemAdded
    {
        DateTime Timestamp { get; }
        string UserName { get; }
    }
}
