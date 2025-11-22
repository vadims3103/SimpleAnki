using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleAnki.Models
{
    public class Deck
    {
        [Key]
        public Guid Id {get; set;} = Guid.NewGuid();

        [Required, MaxLength(200)]
        public required string Title {get; set;}

        public string? Description {get; set;}
        
        public List<Card> Cards {get; set;} = new();

        public Guid UserId { get; set; }
        public User? User { get; set; }  
    }
}