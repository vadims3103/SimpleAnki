using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleAnki.Models
{
    public class Example
    {
        [Key]
        public Guid Id {get; set;} = Guid.NewGuid();

        [Required]
        public Guid CardId {get; set;}

        [ForeignKey(nameof(CardId))]
        public Card? Card {get; set;}

        [Required]
        public required string Text {get; set;} 
    }
}
