using System;
using System.Collections.Generic;
using System.Text;

namespace FlightBooking.Core
{
    public interface CartRemoved
    {
        Guid CartId { get; }
        string UserName { get; }
    }
}
