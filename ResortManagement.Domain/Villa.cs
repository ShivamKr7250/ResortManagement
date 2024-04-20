using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResortManagement.Domain
{
    public class Villa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Display (Name = "Price Per Night")]
        [Range(10,10000)]
        public double Price { get; set; }

        public int Sqft { get; set; }

        [Range(1,10)]
        public int Occupancy { get; set; }

        [Display (Name = "Image Url")]
        public string? ImageUrl { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set;}
    }
}
