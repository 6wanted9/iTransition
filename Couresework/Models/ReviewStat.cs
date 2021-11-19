using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Couresework.Models
{
    public class ReviewStat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public Review Review { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AspNetUsers AspNetUsers { get; set; }
        public int UserRated { get; set; }
        public bool UserLiked { get; set; }
    }
}
