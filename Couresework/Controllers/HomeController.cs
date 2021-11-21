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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;

namespace Couresework.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IStringLocalizer<MultiLanguage> _multiLocalizer;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IStringLocalizer<HomeController> localizer,
                   IStringLocalizer<MultiLanguage> multiLocalizer)
        {
            _logger = logger;
            _db = db;

            _localizer = localizer;
            _multiLocalizer = multiLocalizer;
        }
        public async Task<IActionResult> Index(string contentTypeSort)
        {
            if (contentTypeSort == _multiLocalizer["Latest reviews"].Value || contentTypeSort == null)
            {
                ViewData["contentTypeSort"] = _multiLocalizer["Latest reviews"].Value;
                return View(await _db.Reviews.OrderByDescending(c => c.Id).Take(10).ToListAsync());
            }
            else if (contentTypeSort == _multiLocalizer["Popular reviews"].Value)
            {
                ViewData["contentTypeSort"] = _multiLocalizer["Popular reviews"].Value;
                return View(await _db.Reviews.OrderByDescending(c => c.UsersRate).Take(10).ToListAsync());
            }
            else
            {
                ViewData["contentTypeSort"] = _multiLocalizer["TAGs"].Value;
                return View(await _db.Reviews.ToListAsync());
            }
        }
        [Authorize]
        public async Task<IActionResult> ManageAccount(string contentTypeSort, string userID)
        {
            if (userID != null)
            {
                ViewData["userID"] = userID;
                if (contentTypeSort == _multiLocalizer["Latest reviews"].Value || contentTypeSort == null)
                {
                    ViewData["contentTypeSort"] = _multiLocalizer["Latest reviews"].Value;
                    return View(await _db.Reviews.OrderByDescending(c => c.Id).Where(c => c.AuthorId == userID).ToListAsync());
                }
                else if (contentTypeSort == _multiLocalizer["Popular reviews"].Value)
                {
                    ViewData["contentTypeSort"] = _multiLocalizer["Popular reviews"].Value;
                    return View(await _db.Reviews.OrderByDescending(c => c.UsersRate).Where(c => c.AuthorId == userID).ToListAsync());
                }
                else
                {
                    ViewData["contentTypeSort"] = _multiLocalizer["TAGs"].Value;
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
        [Authorize]
        public IActionResult CreatingReview(string userId)
        {
            ViewData["userId"] = userId;
            return View();
        }
        [Authorize]
        public IActionResult WatchingReview(int reviewId, string userId)
        {
            var review = _db.Reviews.Find(reviewId);
            if (userId != null)
            {
                var reviewStat = _db.ReviewStats.FirstOrDefault(stat => stat.UserId == userId && stat.ReviewId == reviewId);
                string likeVal;
                string rateMsg;
                int rateVal;
                if ((reviewStat != null && reviewStat.UserLiked != true) || reviewStat == null)
                    likeVal = "Like";
                else
                    likeVal = "Remove Like";
                ViewData["likeVal"] = likeVal;
                //
                if (reviewStat != null && reviewStat.UserRated != 0)
                {
                    rateMsg = "Rerate.Current:";
                    rateVal = reviewStat.UserRated;
                    ViewData["rateVal"] = rateVal;
                }
                else
                    rateMsg = "Rate this review";
                ViewData["rateMsg"] = rateMsg;
            }
            return View(review);
        }
        [Authorize]
        public IActionResult EditingReview(int reviewId, string userId)
        {
            var review = _db.Reviews.Find(reviewId);
            if (userId == review.AuthorId || (_db.UserRoles != null && _db.UserRoles.FirstOrDefault(role => role.UserId == userId && role.RoleId == "0") != null))
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
        [Authorize]

        public IActionResult DeletingReview(int reviewId, string userId)
        {
            var review = _db.Reviews.Find(reviewId);
            if (userId == review.AuthorId || _db.UserRoles.FirstOrDefault(role => role.UserId == userId && role.RoleId == "0") != null)
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

        public async Task<IActionResult> Search(string searchStr, string tag)
        {
            if (searchStr != null)
            {
                var reviews = await _db.Reviews.Where(review => review.Group.Contains(searchStr) || review.Name.Contains(searchStr) || review.ReviewText.Contains(searchStr) || review.Tags.Contains(searchStr)).OrderByDescending(review => review.Id).ToListAsync();
                if (reviews.Count != 0)
                    ViewData["searchData"] = reviews;
                return View();
            }
            else if(tag != null)
            {
                var reviews = await _db.Reviews.Where(review => review.Tags.Contains(searchStr)).OrderByDescending(review => review.Id).ToListAsync();
                if(reviews.Count != 0)
                    ViewData["searchData"] = reviews;
                return View();
            }
            else
            {
                return Redirect("/");
            }
        }

        [Authorize]
        public IActionResult AdminPanel()
        {
            return View(_db.Users.OrderBy(user => user.Id).ToList());
        }
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
 
            return LocalRedirect(returnUrl);
        }
        public IActionResult ChangeTheme(string returnUrl)
        {
            if (Request.Cookies.ContainsKey("theme"))
            {
                string theme = Request.Cookies["theme"];
                if (theme == "dark")
                {
                    Response.Cookies.Append("theme", "light");

                }
                else
                {
                    Response.Cookies.Append("theme", "dark");

                }
            }
            else
            {
                Response.Cookies.Append("theme", "dark");
            }
            ViewData["theme"] = "dark";
            return LocalRedirect(returnUrl);
        }
    }
}
