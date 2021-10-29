using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using task4.Models;
using static task4.Models.BroAndSis;

namespace task4.Controllers
{
    public class CounterController : Controller
    {
        public void BroCL(string User)
        {

            BroClick(User);
            Response.Redirect("/");
        }
        public void SisCL(string User)
        {
            SisClick(User);
            Response.Redirect("/");
        }
    }
}
