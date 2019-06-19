using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simchas.Data;

namespace Simchas.Models
{
    public class ContributorsViewModel
    {
        public IEnumerable<Contributor> Contributors { get; set; }
        public string Date { get; set; }
    }
}