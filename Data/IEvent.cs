using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface IEvent : IEntity
    {
        Guid UserId { get; set; }
        string Description { get; set; }
        DateTime Timestamp { get; set; }
    }
}
