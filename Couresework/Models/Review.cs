using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Couresework.Models
{
    public class Review
    {
        public Review(string name, string group, string tags, string reviewText, ushort rating, string authorId)
        {
            Name = name;
            Group = group;
            Tags = tags;
            ReviewText = reviewText;
            Rating = rating;
            AuthorId = authorId;
            UsersRate = 0;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Tags { get; set; }
        public string ReviewText { get; set; }
        public ushort Rating { get; set; }
        public string AuthorId { get; set; }
        public double UsersRate { get; set; }
        public List<ReviewStat> ReviewStats { get; set; }
    }
}
