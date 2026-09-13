using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
     public class ChuDe
    {
        [Key]
        public int MaCD { get; set; }

        [Required]
        [StringLength(100)]
        public string TenChuDe { get; set; }
    }
}