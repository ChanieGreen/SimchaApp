using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simchas.Data;

namespace Simchas.Models
{
    public class ContributionsViewModel
    {
        public Simcha Simcha { get; set; }
        public IEnumerable<SimchaContributor> Contributions { get; set; }
    }
}