using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
    public class WeddingUser
    {
        [Key]
        public int UserId {get;set;}
        [Display(Name="First Name")]
        [Required]
        public string FirstName {get;set;}
        [Required]
        [Display(Name="Last Name")]
        public string LastName {get;set;}
        [Required]
        [EmailAddress]
        public string Email {get;set;}
        [DataType(DataType.Password)]
        public string Password {get;set;}
        [Compare("Password")]
        [NotMapped]
        public string Confirm {get;set;}

        // Helpful, but not in DB (no {get;set;})
        public string FullName
        {
            get 
            {
                return $"{FirstName} {LastName}";
            } 
        }
        
        public List<Wedding> WeddingsPlanned {get;set;}
    }
    public class LoginUser
    {
        [EmailAddress]
        [Required]
        [Display(Name="Email")]
        public string EmailAttempt {get;set;}
        [DataType(DataType.Password)]
        [Display(Name="Password")]
        public string PasswordAttempt {get;set;}
    }

}