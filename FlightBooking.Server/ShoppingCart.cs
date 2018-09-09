using Automatonymous;
using System;
using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Server
{
    public class ShoppingCart : SagaStateMachineInstance
    {
        public string CurrentState { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid? ExpirationId { get; set; }
        public Guid? OrderId { get; set; }

        [Key]
        public Guid CorrelationId { get; set; }
    }
}
