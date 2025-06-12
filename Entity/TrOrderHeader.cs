using System.ComponentModel.DataAnnotations.Schema;

namespace ExperimentAPI.Entity
{
    [Table("tr_order_header")]
    public class TrOrderHeader : BaseEntity
    {
        [Column("user_id")]
        public required Guid UserID { get; set; }

        [Column("issued_date")]
        public required DateTime IssuedDate { get; set; } = DateTime.UtcNow;

        [Column("offer")]
        public required int Offer { get; set; }

        [InverseProperty(nameof(TrOrderDetail.OrderHeader))]
        public List<TrOrderDetail> OrderDetails { get; set; } = [];
    }
}