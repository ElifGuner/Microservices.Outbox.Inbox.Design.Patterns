using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderOutboxTablePublisherService.Entities
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
