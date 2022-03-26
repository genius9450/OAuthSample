using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OAuth.Sample.EF.Entity
{
    /// <summary>
    /// 系統設定
    /// </summary>
    [Table("SystemSetting")]
    public class SystemSetting : BaseEntity
    {
        /// <summary>
        /// 系統來源ID
        /// </summary>
        [Column(TypeName = "varchar(30)")]
        [Required]
        public string SourceID { get; set; }

        /// <summary>
        /// 系統Key
        /// </summary>
        [Column(TypeName = "varchar(50)")]
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// 系統名稱
        /// </summary>
        [Column(TypeName = "nvarchar(30)")]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Api金鑰
        /// </summary>
        [Column(TypeName = "varchar(500)")]
        public string ApiKey { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string Remark { get; set; }

    }
}
