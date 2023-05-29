using System;
using System.ComponentModel.DataAnnotations;

namespace SuperShop.Data.Entities
{
    public class Product : IEntity
    {
        [Key] //to define that is a primary key EF
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters length.")]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] //formats type, if in edit mode doesnt use the format
        public decimal Price { get; set; }

        [Display(Name = "Image")] //how the name of the field will be shown 
        public string ImageUrl { get; set; }

        [Display(Name = "Last Purchase")]
        public DateTime? LastPurchase { get; set; }

        [Display(Name = "Last Sale")]
        public DateTime? LastSale { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double Stock { get; set; }
    }
}
