using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MedExpertSystem.Models;

namespace MedExpertSystem.Controllers
{
    public class BaseController : Controller
    {
        //public ApplicationDbContext DbContext;

        public BaseController()
        {
            //DbContext = new ApplicationDbContext();
            //DbContext.Database.Initialize(true);
        }
    }
}