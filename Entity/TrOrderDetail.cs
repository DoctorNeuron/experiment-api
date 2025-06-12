using System.ComponentModel.DataAnnotations.Schema;

namespace ExperimentAPI.Entity
{
    [Table("tr_order_detail")]
    public class TrOrderDetail : BaseEntity
    {
        [Column("order_header_id")]
        public required Guid OrderHeaderID { get; set; }

        [Column("product_id")]
        public required Guid ProductID { get; set; }

        [Column("quantity")]
        public required int Quantity { get; set; }

        [ForeignKey(nameof(OrderHeaderID))]
        public TrOrderHeader OrderHeader { get; set; } = default!;

        [ForeignKey(nameof(ProductID))]
        public MsProduct Product { get; set; } = default!;
    }
}