using System;
using System.Collections.Generic;
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
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("Id")]
        public AspNetUsers AspNetUsers { get; set; }
        public int Likes { get; set; }
    }
}
