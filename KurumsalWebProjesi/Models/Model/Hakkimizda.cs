using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KurumsalWebProjesi.Models.Model
{
    [Table("Hakkimizda")]
    public class Hakkimizda
    {
        [Key] // primary key olduğunu belirtir
        public int HakkimizdaId { get; set; }
        [Required] //zorunlu olduğunu belirtir
        [DisplayName("Hakkımızda Açıklama")]
        public String Aciklama { get; set; }
    }
}