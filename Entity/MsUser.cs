using System.ComponentModel.DataAnnotations.Schema;

namespace ExperimentAPI.Entity
{
    [Table("ms_user")]
    public class MsUser : BaseEntity
    {
        [Column("username")]
        public required string Username { get; set; }

        [Column("email")]
        public required string Email { get; set; }

        [InverseProperty(nameof(TrUserRefreshToken.User))]
        public TrUserRefreshToken? RefreshToken { get; set; } = default!;
    }
}