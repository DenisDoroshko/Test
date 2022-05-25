﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketManagement.EventManagerApi.Models
{
    public class EventAreaModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string Description { get; set; }

        public int CoordX { get; set; }

        public int CoordY { get; set; }

        public string Color { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public decimal Price { get; set; }

        public List<EventSeatModel> EventSeats { get; set; }
    }
}
