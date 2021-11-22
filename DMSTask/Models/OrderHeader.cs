using DMSTask.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMSTask.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DueDate { get; set; }
        public OrderStatus Status { get; set; }
        public int? TaxCode { get; set; }
        public double TaxValue { get; set; }
        public int? DiscountCode { get; set; }
        public double DiscountValue { get; set; }
        public double TotalPrice { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
