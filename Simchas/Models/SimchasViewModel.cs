using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simchas.Data;

namespace Simchas.Models
{
    public class SimchasViewModel
    {
        public IEnumerable<Simcha> Simchas { get; set; }
        public int Contributors { get; set; }
    }
}