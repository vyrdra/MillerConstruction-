﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CapStoneProject.Controllers
{
    public class GalleryController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public ViewResult Concrete()
        {
            return View();
        }

        public ViewResult Framing()
        {
            return View();
        }

        public ViewResult Remodel()
        {
            return View();
        }

    }
}
