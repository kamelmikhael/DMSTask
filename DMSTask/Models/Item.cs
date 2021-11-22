using System.ComponentModel.DataAnnotations.Schema;

namespace DMSTask.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int UOM { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        [ForeignKey("UOM")]
        public UnitOfMeasure UnitOfMeasure { get; set; }
    }
}
