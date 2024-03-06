using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class AvailabilityChangedEvent
    {
        public string ProductId { get; set; }
        public bool IsAvailable { get; set; }
    }
}
