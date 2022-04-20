using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThirdPartyEventEditor.Models
{
    public class ThirdPartyEventDto
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Description { get; set; }

        public string PosterImage { get; set; }

        public string VenueName { get; set; }

        public string LayoutName { get; set; }
    }
}