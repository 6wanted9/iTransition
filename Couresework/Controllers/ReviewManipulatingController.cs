﻿using Couresework.Data;
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
using Dropbox.Api;
using System.Text;
using Dropbox.Api.Files;
using Microsoft.AspNetCore.Http;

namespace Couresework.Controllers
{
    [Authorize]
    public class ReviewManipulatingController : Controller
    {
        private readonly ApplicationDbContext _db;
        private DropboxClient dropboxClient = new DropboxClient("gy6HcdfPF2sAAAAAAAAAASK76w_lmXptJEM7lcBfiym9x1x6QBB7s-Db3TBz-uOd");
        public ReviewManipulatingController(ApplicationDbContext db)
        {
            _db = db;
        }
        public void CreateReview(string name, string group, List<string> _tags, string reviewText, List<IFormFile> _imagesURLs, ushort rating, string authorId)
        {
            Response.Redirect("/");
            foreach (var tag in _tags.ToList())
            {
                if (tag == null || tag.All(Char.IsWhiteSpace))
                {
                    _tags.Remove(tag);
                }
            }
            var reviewTags = String.Join(",", _tags.ToArray());
            Review review = new Review(name, group, reviewTags, reviewText, rating, authorId);
            _db.Add(review);
            _db.SaveChanges();
            TagsRepository.AddTags(review);
            //
            if (_imagesURLs != null)
            {
                UploadImages(_imagesURLs, review);
            }
        }
        public void EditReview(string name, string group, List<string> _tags, string reviewText, List<IFormFile> _imagesURLs, ushort rating, int reviewId, string userId)
        {
            Response.Redirect("/");
            foreach (var tag in _tags.ToList())
            {
                if (tag == null || tag=="" || tag.All(Char.IsWhiteSpace))
                {
                    _tags.Remove(tag);
                }
            }
            var reviewTags = String.Join(",", _tags.ToArray());
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
            if (_imagesURLs != null)
            {
                UploadImages(_imagesURLs, review);
            }
        }
        private void UploadImages(List<IFormFile> _imagesURLs, Review review)
        {
            var imagesURLs = new List<string>();
            int fileIndex = 0;
            var fileFolder = $"/{review.Id}";
            if (CheckExisting(fileFolder).Result == true)
            {
                var deletingFolder = dropboxClient.Files.DeleteV2Async(fileFolder);
                deletingFolder.Wait();
            }
            foreach (var item in _imagesURLs)
            {
                string fileName = $"{fileIndex}{Path.GetExtension(item.FileName)}";
                var filePath = Path.GetTempFileName();

                using (var stream = System.IO.File.Create(filePath))
                {
                    // The formFile is the method parameter which type is IFormFile
                    // Saves the files to the local file system using a file name generated by the app.
                    item.CopyTo(stream);
                }
                using (var mem = new MemoryStream(System.IO.File.ReadAllBytes(filePath)))
                {
                    var updated = dropboxClient.Files.UploadAsync(
                        $"{fileFolder}/{fileName}",
                        WriteMode.Overwrite.Instance,
                        body: mem);
                    updated.Wait();           
                }
                fileIndex++;
            }
            _db.SaveChanges();
        }

        private void DeleteDropBoxFolder(Review review)
        {
            var fileFolder = $"/{review.Id}";
            if (CheckExisting(fileFolder).Result == true)
            {
                var deletingFolder = dropboxClient.Files.DeleteV2Async(fileFolder);
                deletingFolder.Wait();
            }
        }
        private async Task<bool> CheckExisting(string path)
        {
            bool folderExists = true;
            try
            {
                await dropboxClient.Files.GetMetadataAsync(path);
            }
            catch (ApiException<Dropbox.Api.Files.GetMetadataError> e)
            {
                if (e.ErrorResponse.IsPath && e.ErrorResponse.AsPath.Value.IsNotFound)
                {
                    folderExists = false;
                }
            }
            return folderExists;
        }

        public void DeleteReview(int reviewId, string userId)
        {
            Response.Redirect("/");
            var review = _db.Reviews.FirstOrDefault(rev => rev.Id == reviewId);
            DeleteDropBoxFolder(review);
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
