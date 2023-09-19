using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SuperShop.Models
{
    public class RegisterNewUserViewModel
    {
        [Required]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)] //tipo de dados para Username
        public string Username { get; set; }

        [MaxLength(100, ErrorMessage ="The field {0} can only contain {1} characters lenght.")]
        public string Address { get; set; }

        [MaxLength(20, ErrorMessage = "The field {0} can only contain {1} characters lenght.")]
        public string PhoneNumber { get; set; }

        [Display(Name ="City")]
        [Range(1, int.MaxValue, ErrorMessage ="You must select a city.")]
        public int CityId { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        [Display(Name = "Country")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a city.")]
        public int CountryId { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")] //compara o campo com o indicado, neste caso compara a propriedade Confirm com a propriedade Password
        public string Confirm { get; set; }
    }
}
