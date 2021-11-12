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
        public async Task<IActionResult> Index()
        {    
            return View(await _db.Reviews.OrderByDescending(c => c.Id).Take(10).ToListAsync());
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
    }
}
