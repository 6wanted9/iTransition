using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;

namespace Couresework.Data
{
    public static class GroupsCloud
    {
        
        public static List<SelectListItem> Groups { get; set; } = new List<SelectListItem> { new SelectListItem("Books", "Books"), new SelectListItem("Movies", "Movies"), new SelectListItem("Games", "Games") };
    }
}
