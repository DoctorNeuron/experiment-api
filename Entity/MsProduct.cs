using System.ComponentModel.DataAnnotations.Schema;

namespace ExperimentAPI.Entity
{
    [Table("ms_product")]
    public class MsProduct : BaseEntity
    {
        [Column("name")]
        public string Name { get; set; } = "";

        [Column("image")]
        public string Image { get; set; } = "";

        [Column("category")]
        public string Category { get; set; } = "";

        [Column("price")]
        public double Price { get; set; }

        [Column("is_raw")]
        public bool IsRaw { get; set; }

        [Column("time")]
        public int Time { get; set; }

        [Column("level")]
        public int Level { get; set; }

        [Column("xp")]
        public int XP { get; set; }

        [InverseProperty(nameof(TrOrderDetail.Product))]
        public List<TrOrderDetail> OrderDetails { get; set; } = [];
    }
}