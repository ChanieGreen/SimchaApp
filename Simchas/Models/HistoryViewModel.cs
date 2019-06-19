using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Simchas.Models
{
    public class HistoryViewModel
    {
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}