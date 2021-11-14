using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Couresework.Models;
using Couresework.Data;
using Microsoft.EntityFrameworkCore;

namespace Couresework.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public async Task<IActionResult> Index(string contentTypeSort)
        {
            if (contentTypeSort == "Latest reviews" || contentTypeSort == null)
            {
                ViewData["contentTypeSort"] = "Latest reviews";
                return View(await _db.Reviews.OrderByDescending(c => c.Id).Take(10).ToListAsync());
            }
            else if (contentTypeSort == "Popular reviews")
            {
                ViewData["contentTypeSort"] = "Popular reviews";
                return View(await _db.Reviews.OrderByDescending(c => c.UsersRate).Take(10).ToListAsync());
            }
            else
            {
                ViewData["contentTypeSort"] = "TAGs";
                return View();
            }
        }
        public async Task<IActionResult> ManageAccount(string contentTypeSort, string userID)
        {
            if (userID != null)
            {
                ViewData["userID"] = userID;
                ViewData["userRole"] = _db.UserRoles.FirstOrDefault(role => role.UserId == userID);
                if (contentTypeSort == "Latest reviews" || contentTypeSort == null)
                {
                    ViewData["contentTypeSort"] = "Latest reviews";
                    return View(await _db.Reviews.OrderByDescending(c => c.Id).Where(c => c.AuthorId == userID).ToListAsync());
                }
                else if (contentTypeSort == "Popular reviews")
                {
                    ViewData["contentTypeSort"] = "Popular reviews";
                    return View(await _db.Reviews.OrderByDescending(c => c.UsersRate).Where(c => c.AuthorId == userID).ToListAsync());
                }
                else
                {
                    ViewData["contentTypeSort"] = "TAGs";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult CreatingReview()
        {
            return View();
        }
        public IActionResult WatchingReview(int reviewId, string userId)
        {
            var review = _db.Reviews.Find(reviewId);
            if (userId != null)
            {
                var reviewStat = _db.ReviewStats.FirstOrDefault(stat => stat.UserId == userId && stat.ReviewId == reviewId);
                string likeVal;
                string rateVal;
                if ((reviewStat != null && reviewStat.UserLiked != true) || reviewStat == null)
                    likeVal = "Like";
                else
                    likeVal = "Remove Like";
                ViewData["likeVal"] = likeVal;
                //
                if (reviewStat != null && reviewStat.UserRated != 0)
                    rateVal = $"Rerate.Current: {reviewStat.UserRated}";
                else
                    rateVal = "Rate this review";
                ViewData["rateVal"] = rateVal;
            }
            return View(review);
        }
        public IActionResult EditingReview(int reviewId, string userId)
        {
            ViewData["userRole"] = _db.UserRoles.FirstOrDefault(role => role.UserId == userId);
            var review = _db.Reviews.Find(reviewId);
            if (userId == review.AuthorId)
            {
                var reviewStat = _db.ReviewStats.FirstOrDefault(stat => stat.UserId == userId && stat.ReviewId == reviewId);
                ViewData["authorId"] = review.AuthorId;
                ViewData["review"] = review;
                return View(review);
            }
            else
            {
                return View();
            }
        }
    }
}
