using Couresework.Data;
using Couresework.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Westwind.AspNetCore.Markdown;

namespace Couresework.Controllers
{
    public class ReviewManipulatingController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ReviewManipulatingController(ApplicationDbContext db)
        {
            _db = db;
        }
        public void CreateReview(string name, string group, string tags, string reviewText, ushort rating, string authorId)
        {
            Response.Redirect("/");
            Review review = new Review(name, group, tags, reviewText, rating, authorId);
            _db.Add(review);
            _db.SaveChanges();
        }
        public void LikeReview(int reviewID, string userID)
        {
            var reviewStatObj = _db.ReviewStats.FirstOrDefault(stat => stat.UserId == userID && stat.ReviewId == reviewID);
            var review = _db.Reviews.First(rev => rev.Id == reviewID);
            if (reviewStatObj == null && review.AuthorId != userID)
            {
                ReviewStat reviewStat = new ReviewStat{ ReviewId = reviewID, UserId = userID, UserLiked = true };
                _db.Add(reviewStat);
            }
            else if(reviewStatObj != null && review.AuthorId != userID)
            {
                reviewStatObj.UserLiked = !reviewStatObj.UserLiked;
            }
            _db.SaveChanges();
            Response.Redirect($"/Home/WatchingReview/?reviewId={reviewID}&userId={userID}");
        }
        public void RateReview(int reviewID, string userID, int rating)
        {
            var reviewStatObj = _db.ReviewStats.FirstOrDefault(stat => stat.UserId == userID && stat.ReviewId == reviewID);
            var review = _db.Reviews.First(rev => rev.Id == reviewID);
            if (reviewStatObj == null && review.AuthorId != userID)
            {
                ReviewStat reviewStat = new ReviewStat { ReviewId = reviewID, UserId = userID };
                reviewStat.UserRated = rating;
                _db.Add(reviewStat);
                _db.SaveChanges();
            }
            else if (reviewStatObj != null && review.AuthorId != userID)
            {
                reviewStatObj.UserRated = rating;
            }
            UsersRatingUpd(reviewID);
            _db.SaveChanges();
            Response.Redirect($"/Home/WatchingReview/?reviewId={reviewID}&userId={userID}");
        }

        private void UsersRatingUpd(int reviewID)
        {
            double averageRating = 0;
            var rates = _db.ReviewStats.Where(stat => stat.ReviewId == reviewID).ToArray();
            foreach (var rate in rates)
            {
                averageRating += Convert.ToDouble(rate.UserRated);
            }
            averageRating /= rates.Length;
            var review = _db.Reviews.FirstOrDefault(revID => revID.Id == reviewID);
            review.UsersRate = averageRating;
        }
    }
}
