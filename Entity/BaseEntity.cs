using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExperimentAPI.Entity
{
    public class BaseEntity
    {
        [Key]
        [Column("id")]
        public required Guid ID { get; set; }
    }
}