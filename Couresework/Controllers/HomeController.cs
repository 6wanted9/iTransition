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
using Dropbox.Api;
using Dropbox.Api.Files;

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
        private DropboxClient dropboxClient = new DropboxClient("gy6HcdfPF2sAAAAAAAAAASK76w_lmXptJEM7lcBfiym9x1x6QBB7s-Db3TBz-uOd");

        public async Task<IActionResult> Index(string contentTypeSort, int page = 1)
        {
            int elementsPerPage = 2;
            int pagesAmount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(_db.Reviews.Count()) / elementsPerPage));
            if (contentTypeSort == "Latest reviews" || contentTypeSort == null)
            {
                ViewData["contentTypeSort"] = "Latest reviews";
                ViewData["pagesAmount"] = pagesAmount;
                ViewData["currentPage"] = page;
                return View(await _db.Reviews.OrderByDescending(c => c.Id).Skip((page - 1) * elementsPerPage).Take(elementsPerPage).ToListAsync());
            }
            else if (contentTypeSort == "Popular reviews")
            {
                ViewData["contentTypeSort"] = "Popular reviews";
                ViewData["pagesAmount"] = pagesAmount;
                ViewData["currentPage"] = page;
                return View(await _db.Reviews.OrderByDescending(c => c.UsersRate).Skip((page - 1) * elementsPerPage).Take(elementsPerPage).ToListAsync());
            }
            else
            {
                ViewData["contentTypeSort"] = "TAGs";
                return View(await _db.Reviews.ToListAsync());
            }
        }
        [Authorize]
        public async Task<IActionResult> ManageAccount(string contentTypeSort, string userID, int page = 1)
        {
            if (userID != null)
            {
                int elementsPerPage = 1;
                int pagesAmount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(_db.Reviews.Count()) / elementsPerPage));
                ViewData["userID"] = userID;
                ViewData["pagesAmount"] = pagesAmount;
                ViewData["currentPage"] = page;
                if (contentTypeSort == "Latest reviews" || contentTypeSort == null)
                {
                    ViewData["contentTypeSort"] = "Latest reviews";
                    return View(await _db.Reviews.OrderByDescending(c => c.Id).Where(c => c.AuthorId == userID).Skip((page - 1) * elementsPerPage).Take(elementsPerPage).ToListAsync());
                }
                else if (contentTypeSort == "Popular reviews")
                {
                    ViewData["contentTypeSort"] = "Popular reviews";
                    return View(await _db.Reviews.OrderByDescending(c => c.UsersRate).Where(c => c.AuthorId == userID).Skip((page - 1) * elementsPerPage).Take(elementsPerPage).ToListAsync());
                }
                else
                {
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
        public async Task<IActionResult> WatchingReview(int reviewId, string userId)
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
            List<Metadata> images = new List<Metadata>();
            try
            {
                await dropboxClient.Files.ListFolderAsync($"/{reviewId}");
            }
            catch (ApiException<ListFolderError> e)
            {
                if (e.ErrorResponse.IsPath && e.ErrorResponse.AsPath.Value.IsNotFound)
                {
                    images = null;
                }
            }
            if (images != null)
            {
                images = dropboxClient.Files.ListFolderAsync($"/{reviewId}").Result.Entries.ToList();
                List<string> imagesLinks = new List<string>();
                foreach (var item in images)
                {

                    imagesLinks.Add(dropboxClient.Files.GetTemporaryLinkAsync(item.PathDisplay).Result.Link);
                }
                ViewData["imagesLinks"] = imagesLinks;
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
                string group = null;
                if (_multiLocalizer.GetAllStrings().FirstOrDefault(t => t.Value == searchStr) != null)
                    group = _multiLocalizer.GetAllStrings().FirstOrDefault(t => t.Value == searchStr).Name;

                var comments = await _db.Comments.Where(c => c.UserComment.Contains(searchStr)).Select(c => c.ReviewId).ToListAsync();
                var reviews = await _db.Reviews.Where(review => review.Group == group || review.Name.Contains(searchStr) || review.ReviewText.Contains(searchStr) || review.Tags.Contains(searchStr) || comments.Contains(review.Id)).OrderByDescending(review => review.Id).ToListAsync();
                if (reviews.Count != 0)
                    ViewData["searchData"] = reviews;
                return View();
            }
            else if(tag != null)
            {
                var reviews = await _db.Reviews.Where(review => review.Tags.Contains(tag)).OrderByDescending(review => review.Id).ToListAsync();
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

        public ActionResult Comments(int reviewId)
        {
            ViewData["Comments"] = _db.Comments.Where(c => c.ReviewId == reviewId).ToList();
            return PartialView("_Comments");
        }
    }
}
