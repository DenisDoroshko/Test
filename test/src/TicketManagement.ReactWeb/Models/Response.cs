using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketManagement.ReactWeb.Models
{
    public class Response<T>
    {
        public bool IsSuccess { get; set; } = true;

        public T Result { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
    }
}
