using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendWise.Data.Models
{
    public class UserEventModel : EventModel
    {
        public Guid UserId { get; set; }

        public UserEventModel(Guid userId, string description)
        {
            UserId = userId;
            Description = description;
            Timestamp = DateTime.Now;
        }
    }
}