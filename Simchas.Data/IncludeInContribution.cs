using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simchas.Data
{
    public class IncludeInContribution
    {
        public bool Include { get; set; }
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
    }
}
