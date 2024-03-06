using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class CountDecreasedEvent
    {
        public string ProductId { get; set; }
        public int DecrementCount { get; set; }
    }
}
