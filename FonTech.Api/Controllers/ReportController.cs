﻿using Microsoft.AspNetCore.Mvc;

namespace FonTech.Api.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
