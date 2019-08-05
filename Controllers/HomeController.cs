using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using InsuranceQuote.Models;
using InsuranceQuote.ViewModels;
using System.Collections;

namespace InsuranceQuote.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Quoter(string firstName, string lastName, string emailAddress, DateTime? dateOfBirth, int carYear, string carMake, string carModel, int? speedingTicketCount, bool? duiCheck, bool? optionCheck)
        {
            string dateOfBirthString = dateOfBirth.ToString();
            string carYearString = carYear.ToString();
            string speedingTicketCountString = speedingTicketCount.ToString();
            string duiCheckString = duiCheck.ToString();
            string optionCheckString = optionCheck.ToString();

            var allStrings = new List<string>() { firstName, lastName, emailAddress, dateOfBirthString, carYearString, carMake, carModel, speedingTicketCountString, duiCheckString, optionCheckString };

            foreach (string y in allStrings)
            {
                if (string.IsNullOrEmpty(y)) { return View("~/Shared/Error"); }
                else { break; }
            }
            using (InsuranceDetailsEntities db = new InsuranceDetailsEntities())
            {
                //easy way with ef
                var quote = new UserDetail();
                quote.FirstName = firstName;
                quote.LastName = lastName;
                quote.EmailAddress = emailAddress;
                quote.DateOfBirth = dateOfBirth;
                quote.CarYear = carYear;
                quote.CarMake = carMake;
                quote.CarModel = carModel;
                quote.SpeedingTicketCount = speedingTicketCount;
                quote.DUI = duiCheck;
                quote.Coverage = optionCheck;

                db.UserDetails.Add(quote);
                db.SaveChanges();

                int linkId = quote.Id;
                ViewBag.LinkableId = linkId;

                //declaring
                int basePrice = 50;
                int ageAdder;
                int yearAdder;
                int carAdder;
                int ticketAdder;
                double duiPrice;
                double coveragePrice;

                //ticket adder
                ticketAdder = Convert.ToInt32(quote.SpeedingTicketCount) * 10;

                //Find age of user and price accordingly
                int ageAdd1 = 25;
                int ageAdd2 = 100;
                DateTime dOB = Convert.ToDateTime(quote.DateOfBirth);
                int birthYear = dOB.Year;
                int nowYear = DateTime.Now.Year;
                int userAge = nowYear - birthYear;
                if (userAge < 25 && userAge > 18 || userAge > 100) { ageAdder = ageAdd1; }
                else if (userAge < 18) { ageAdder = ageAdd2; }
                else { ageAdder = 0; };

                //make, year and model adders
                int modelYearAdd = 25;
                if (quote.CarYear < 2000 || quote.CarYear > 2015) { yearAdder = modelYearAdd; }
                else { yearAdder = 0; };

                int porscheModifier = 25;
                int porscheCarreraModifier = porscheModifier * 2;
                if (quote.CarMake == "Porsche" && quote.CarModel != "911 Carrera") { carAdder = porscheModifier; }
                else if (quote.CarMake == "Porsche" && quote.CarModel == "911 Carrera") { carAdder = porscheCarreraModifier; }
                else { carAdder = 0; }

                double subPrice = Convert.ToDouble(basePrice + ticketAdder + ageAdder + yearAdder + carAdder);

                if (Convert.ToBoolean(quote.DUI)) { duiPrice = subPrice * .25; }
                else { duiPrice = 0; }
                if (Convert.ToBoolean(quote.Coverage)) { coveragePrice = subPrice * .5; }
                else { coveragePrice = 0; }

                double totalPrice = subPrice + coveragePrice + duiPrice;


                var price = new QuotePrice();
                price.Id = quote.Id;
                price.BasePrice = basePrice;
                price.AgeAdder = ageAdder;
                price.YearAdder = yearAdder;
                price.CarAdder = carAdder;
                price.TicketAdder = ticketAdder;
                price.DUIPrice = duiPrice;
                price.CoveragePrice = coveragePrice;
                price.TotalPrice = totalPrice;

                db.QuotePrices.Add(price);
                db.SaveChanges();

            }
            return View("FinishedQuote");
        }
    }
}

   