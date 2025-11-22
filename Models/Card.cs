using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleAnki.Models
{
    public class Card
    {
        [Key]
        public Guid Id {get; set;} = Guid.NewGuid();

        [Required]
        public Guid DeckId {get; set;}

        [ForeignKey(nameof(DeckId))]
        public Deck? Deck {get; set;}

        [Required]
        public required string Front {get; set;} 

        [Required]
        public required string Back {get; set;}

        public int Order {get; set;} = 0;

        public List<Example> Examples {get; set;} = new();
    }
}