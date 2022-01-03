using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;

namespace Couresework.Data
{
    public static class GroupsCloud
    {
        
        public static List<SelectListItem> Groups { get; set; } = new List<SelectListItem> { new SelectListItem("Politics", "Politics"), new SelectListItem("Culture", "Culture"), new SelectListItem("Technologies", "Technologies") };
    }
}
