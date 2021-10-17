using _3_10_HW.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace _3_10_HW.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Simchas;Integrated Security=true;";

        public IActionResult Simchos()
        {
            var db = new SimchosDb(_connectionString);
            var vm = new SimchaViewModel();
            vm.Simchas = db.GetSimchas();
            vm.Contributers = db.GetTotalContributers();
            return View(vm);
        }
        public IActionResult Contributions(int id)
        {
            var db = new SimchosDb(_connectionString);           
            var vm = new SimchaViewModel();
            vm.Simcha = db.GetSimchaById(id);
            vm.Funds = db.GetFundsPerSimcha(id);
            vm.People = db.GetPeople();

            return View(vm);
        }
        [HttpPost]
        public IActionResult NewSimcha(Simcha simcha)
        {
            var db = new SimchosDb(_connectionString);
            db.AddSimcha(simcha);
            return Redirect("/home/simchos");
            
        }
        public IActionResult Contributers()
        {
            var db = new SimchosDb(_connectionString);
            var vm = new SimchaViewModel();
            vm.People = db.GetPeople();
            vm.Total = db.GetTotalFunds();
            return View(vm);
        }
        [HttpPost]
        public IActionResult NewContributer(Person person, Fund fund)
        {
            var db = new SimchosDb(_connectionString);
            db.AddPerson(person);
            db.AddFund(new Fund
            {
                Amount = fund.Amount,
                SimchaId = 1,
                Date = fund.Date,
                ContributerId = person.Id
            }
            ); ;
            return Redirect("/home/contributers");

        }
        [HttpPost]
        public IActionResult EditContributer(Person person)
        {
            var db = new SimchosDb(_connectionString);
            db.EditPerson(person);
            return Redirect("/home/contributers");

        }
        [HttpPost]
        public IActionResult NewFund(Fund fund)
        {
            var db = new SimchosDb(_connectionString);
            db.AddFund(new Fund
            {
                Amount = fund.Amount,
                SimchaId = 1,
                Date = fund.Date,
                ContributerId = fund.ContributerId
            }
            ); ;
            return Redirect("/home/contributers");

        }
        public IActionResult ShowHistory(int personId)
        {
            var vm = new SimchaViewModel();
            var db = new SimchosDb(_connectionString);
            vm.Funds = db.GetFundsPerPerson(personId);
            return View(vm);
        }
        [HttpPost]
        public IActionResult UpdateContributions(List<Fund>funds)
        {

            var db = new SimchosDb(_connectionString);
            db.UpdateContributions(funds);
            
            return Redirect($"/home/Contributions?id={funds[0].SimchaId}");
        }
    }
}
       
    