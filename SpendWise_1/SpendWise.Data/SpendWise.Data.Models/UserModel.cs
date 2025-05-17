using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendWise.Data.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
    }
}