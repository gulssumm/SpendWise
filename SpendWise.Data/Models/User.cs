using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpendWise.Data.Models;

namespace SpendWise.Data.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
    }
}
