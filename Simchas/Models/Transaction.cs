using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Simchas.Models
{
    public class Transaction
    {
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}