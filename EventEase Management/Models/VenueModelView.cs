using System.ComponentModel.DataAnnotations;

namespace EventEase_Management.Models
{
    public class VenueModelView
    {

        [Key]
        public int VenueID { get; set; }


        [Required(ErrorMessage = " Name is required")]
        [MaxLength(50, ErrorMessage = "Max 100 character allowed.")]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [MaxLength(100, ErrorMessage = "100 character allowed")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 10000, ErrorMessage = "Capacity must be between 1 and 10,000.")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "status is required")]
        public string? Status { get; set; } // Available, Booked, Maintenance

        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? ImageUrl { get; set; }
    }
}
