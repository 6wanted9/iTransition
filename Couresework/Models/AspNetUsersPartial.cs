using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Couresework.Models
{
    public partial class AspNetUsers: IdentityUser
    {
        public List<Review> Reviews { get; set; }
        public List<ReviewStat> ReviewStats { get; set; }
        public LikesAmount LikesAmount { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
