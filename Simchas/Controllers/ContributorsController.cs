using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Simchas.Models;
using Simchas.Data;

namespace Simchas.Controllers
{
    public class ContributorsController : Controller
    {
        private DbManager _mgr = new DbManager(Properties.Settings.Default.ConStr);

        public ActionResult Index()
        {
            ContributorsViewModel vm = new ContributorsViewModel();
            vm.Contributors = _mgr.GetContributors();
            vm.Date = DateTime.Now.ToString("yyyy-MM-dd");
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddContributor(Contributor c, Deposit d)
        {
            _mgr.AddContributor(c);
            d.ContributorId = c.Id;
            _mgr.AddDeposit(d);
            return Redirect("/contributors/index");
        }

        [HttpPost]
        public ActionResult UpdateContributor(Contributor c)
        {
            _mgr.UpdateContributor(c);
            return Redirect("/contributors/index");
        }

        [HttpPost]
        public ActionResult AddDeposit(Deposit d)
        {
            _mgr.AddDeposit(d);
            return Redirect("/contributors/index");
        }

        public ActionResult ShowHistory (int id)
        {
            IEnumerable<Deposit> deposits = _mgr.GetDepositsById(id);
            IEnumerable<Contribution> contributions = _mgr.GetContributionsById(id);
            IEnumerable<Transaction> transactions = deposits.Select(d => new Transaction
            {
                Type = "Deposit",
                Amount = d.Amount,
                Date = d.Date
            }).Concat(contributions.Select(c => new Transaction
            {
                Type = $"Contribution for the {c.SimchaName} simcha",
                Amount = -c.Amount,
                Date = c.Date
            })).OrderByDescending(t => t.Date);
            var vm = new HistoryViewModel
            {
                Transactions = transactions
            };
            return View(vm);
        }
    }
}