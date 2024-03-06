﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class PriceDecreasedEvent
    {
        public string ProductId { get; set; }
        public decimal DecrementAmount { get; set; }
    }
}
