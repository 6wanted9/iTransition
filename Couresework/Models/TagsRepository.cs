using Couresework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Couresework.Models
{
    public static class TagsRepository
    {
        public static List<string> Tags { get; set; } = new List<string>();
        public static void AddTags(Review review)
        {
            if (review.Tags != null)
            {
                if (review.Tags.Contains(","))
                {
                    var newTags = review.Tags.Split(",").ToList();
                    Tags.AddRange(newTags);
                }
                else
                {
                    Tags.Add(review.Tags);
                }
            }
            Tags = Tags.Distinct().ToList();
        }
        public static void Initial(ApplicationDbContext db)
        {
            var reviews = db.Reviews.ToList();
            foreach (var review in reviews)
            {
                if (review.Tags != null && review.Tags.Contains(","))
                {
                    var newTags = review.Tags.Split(",").ToList();
                    Tags.AddRange(newTags);
                }
                else
                {
                    Tags.Add(review.Tags);
                }
            }
            Tags = Tags.Distinct().ToList();
        }
    }
}
