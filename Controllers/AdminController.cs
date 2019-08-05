using InsuranceQuote.Models;
using InsuranceQuote.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace InsuranceQuote.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()

        {
            using (InsuranceDetailsEntities db = new InsuranceDetailsEntities())
            {
                var quotes = db.UserDetails.Where(x=>x.Id>0).ToList();
                var quoteVms = new List<QuoteVm>();
                foreach (var x in quotes)
                {
                    var quoteVm = new QuoteVm();
                    quoteVm.Id = x.Id;
                    quoteVm.FirstName = x.FirstName;
                    quoteVm.LastName = x.LastName;
                    quoteVm.EmailAddress = x.EmailAddress;

                    quoteVms.Add(quoteVm);
                }
                return View(quoteVms);
            }
        }
    }
}