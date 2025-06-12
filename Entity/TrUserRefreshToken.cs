using System.ComponentModel.DataAnnotations.Schema;

namespace ExperimentAPI.Entity
{
    [Table("tr_user_refresh_token")]
    public class TrUserRefreshToken : BaseEntity
    {
        [Column("user_id")]
        public required Guid UserID { get; set; }

        [Column("refresh_token")]
        public required string RefreshToken { get; set; }

        [Column("expired_date")]
        public required DateTime ExpiredDate { get; set; }

        [ForeignKey(nameof(UserID))]
        public MsUser User { get; set; } = default!;
    }
}