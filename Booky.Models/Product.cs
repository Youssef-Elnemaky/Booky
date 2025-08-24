using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booky.Models
{
    public class Product
    {
        
        public int Id { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Book title cannot be more than 100 characters")]
        [MinLength(3, ErrorMessage = "Book title must be at least 3 characters")]
        public string Title { get; set; }

        [Required]
        public string ISBN { get; set; }

        [MaxLength(100, ErrorMessage ="Author name cannot be more than 100 characters")]
        [MinLength(3, ErrorMessage ="Author name must be at least 3 characters")]
        public string Author { get; set; }
        public string Description { get; set; }

        [Required]
        [Display(Name = "List Price")]
        [Range(1,1000)]
        public decimal ListPrice { get; set; }

        [Required]
        [Display(Name = "Price for 1-50")]
        [Range(1,1000)]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Price for 51-100")]
        [Range(1, 1000)]
        public decimal Price50 { get; set; }

        [Required]
        [Display(Name = "Price for 100+")]
        [Range(1, 1000)]
        public decimal Price100 { get; set; }

        [Display(Name ="Category")]
        public int CategoryId { get; set; }

        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; }
    }
}
