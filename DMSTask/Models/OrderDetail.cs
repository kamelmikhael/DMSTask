using System.ComponentModel.DataAnnotations.Schema;

namespace DMSTask.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public double ItemPrice { get; set; }
        public int Qty { get; set; }
        public double TotalPrice { get; set; }
        public int UOM { get; set; }
        public double Tax { get; set; }
        public double Discount { get; set; }

        [ForeignKey("OrderId")]
        public OrderHeader Order { get; set; }

        [ForeignKey("ItemId")]
        public Item Item { get; set; }

        [ForeignKey("UOM")]
        public UnitOfMeasure UnitOfMeasure { get; set; }
    }
}
