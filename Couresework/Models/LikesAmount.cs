using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Couresework.Models
{
    public class LikesAmount
    {
        public LikesAmount()
        {
            Likes = 0;
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        [Column(Order = 1)]
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AspNetUsers AspNetUsers { get; set; }
        public int Likes { get; set; }
    }
}
