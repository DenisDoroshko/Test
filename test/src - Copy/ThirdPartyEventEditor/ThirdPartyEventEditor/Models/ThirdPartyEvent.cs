using System;
using System.ComponentModel.DataAnnotations;

namespace ThirdPartyEventEditor.Models
{
    public class ThirdPartyEvent
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Description { get; set; }

        public string PosterImage { get; set; }

        [Required]
        public string VenueName { get; set; }

        [Required]
        public string LayoutName { get; set; }
    }
}