using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simchas.Data  
{
    public class Simcha
    {
        public int Id { get; set; }
        public string SimchaName { get; set; }
        public int Contributors { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
    }
}
