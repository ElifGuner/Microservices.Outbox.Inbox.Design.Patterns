using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace StockAPI.Models.Entities
{
    public class OrderInbox
    {
        //public int Id { get; set; }
        [Key]
        public Guid IdempotentToken { get; set; }
        public bool Processed { get; set; }
        public string Payload { get; set; }
    }
}
