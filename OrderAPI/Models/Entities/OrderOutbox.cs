using System.ComponentModel.DataAnnotations;

namespace OrderAPI.Models.Entities
{
    public class OrderOutbox
    {
        //public int Id { get; set; }
        [Key]
        public Guid IdempotentToken { get; set; }
        public DateTime OccurredOn { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
    }
}
