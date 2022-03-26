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
        /// 建立時間
        /// </summary>
        public DateTime CreateDateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 異動時間
        /// </summary>
        public DateTime ModifyDateTime { get; set; } = DateTime.Now;

    }
}

