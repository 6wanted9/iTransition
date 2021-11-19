using Couresework.Data;
using Couresework.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [Authorize]
    public class ReviewManipulatingController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ReviewManipulatingController(ApplicationDbContext db)
        {
            _db = db;
        }
        public void CreateReview(string name, string group, List<string> tags, string reviewText, ushort rating, string authorId)
        {
            Response.Redirect("/");
            var reviewTags = String.Join(",", tags.ToArray());
            Review review = new Review(name, group, reviewTags, reviewText, rating, authorId);
            _db.Add(review);
            _db.SaveChanges();
            TagsRepository.AddTags(review);
        }
        public void EditReview(string name, string group, List<string> tags, string reviewText, ushort rating, int reviewId, string userId)
        {
            Response.Redirect("/");
            var reviewTags = String.Join(",", tags.ToArray());
            var review = _db.Reviews.FirstOrDefault(rev => rev.Id == reviewId);
            if (review.AuthorId == userId || _db.UserRoles.FirstOrDefault(role => role.UserId == userId && role.RoleId == "0") != null)
            {
                review.Name = name;
                review.Group = group;
                review.Tags = reviewTags;
                review.ReviewText = reviewText;
                review.Rating = rating;
                _db.SaveChanges();
            }
            TagsRepository.AddTags(review);
        }
        public void DeleteReview(int reviewId, string userId)
        {
            Response.Redirect("/");
            var review = _db.Reviews.FirstOrDefault(rev => rev.Id == reviewId);
            if (review.AuthorId == userId || _db.UserRoles.FirstOrDefault(role => role.UserId == userId && role.RoleId == "0") != null)
            {
                _db.Reviews.Remove(review);
                _db.SaveChanges();
            }
        }
        public void LikeReview(int reviewID, string userID)
        {
            string authorID = _db.Reviews.FirstOrDefault(rev => rev.Id == reviewID).AuthorId;
            var reviewStatObj = _db.ReviewStats.FirstOrDefault(stat => stat.UserId == userID && stat.ReviewId == reviewID);
            var likeStatObj = _db.LikesAmounts.FirstOrDefault(stat => stat.UserId == authorID);
            var review = _db.Reviews.First(rev => rev.Id == reviewID);
            if (reviewStatObj == null && review.AuthorId != userID)
            {
                ReviewStat reviewStat = new ReviewStat{ ReviewId = reviewID, UserId = userID, UserLiked = true };
                _db.Add(reviewStat);
                if (likeStatObj == null)
                {
                    var likeStat = new LikesAmount { UserId = authorID };
                    likeStat.Likes++;
                    _db.Add(likeStat);
                }
                else
                {
                    var likeStat = _db.LikesAmounts.FirstOrDefault(stat => stat.UserId == authorID);
                    likeStat.Likes++;
                }
                
            }
            else if(reviewStatObj != null && review.AuthorId != userID)
            {
                reviewStatObj.UserLiked = !reviewStatObj.UserLiked;
                if (likeStatObj == null)
                {
                    var likeStat = new LikesAmount { UserId = authorID };
                    likeStat.Likes++;
                    _db.Add(likeStat);
                }
                else
                {
                    var likeStat = _db.LikesAmounts.FirstOrDefault(stat => stat.UserId == authorID);
                    if (reviewStatObj.UserLiked)
                        likeStat.Likes++;
                    else
                        likeStat.Likes--;
                }
            }
            _db.SaveChanges();

            Response.Redirect($"/Home/WatchingReview/?reviewId={reviewID}&userId={userID}");
        }
        public void RateReview(int reviewID, string userID, int rating)
        {
            Response.Redirect($"/Home/WatchingReview/?reviewId={reviewID}&userId={userID}");

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
