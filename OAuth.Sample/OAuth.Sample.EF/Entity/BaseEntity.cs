using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OAuth.Sample.EF.Entity
{
    /// <summary>
    /// 共用Entity
    /// </summary>
    public class BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 建立時間(utc)
        /// </summary>
        public DateTime CreateDateTimeUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 異動時間(utc)
        /// </summary>
        public DateTime ModifyDateTimeUtc { get; set; } = DateTime.UtcNow;
    }
}

