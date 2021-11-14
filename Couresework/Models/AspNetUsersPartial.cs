using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Couresework.Models
{
    public partial class AspNetUsers: IdentityUser
    {
        public List<ReviewStat> ReviewStats { get; set; }
        public LikesAmount LikesAmount { get; set; }
    }
}
