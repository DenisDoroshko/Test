using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketManagement.EventManagerApi.Models
{
    public class EventModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Start { get; set; }

        public DateTime Finish { get; set; }

        public int LayoutId { get; set; }

        public string Address { get; set; }

        public string Image { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public List<EventAreaModel> EventAreas { get; set; }
    }
}
