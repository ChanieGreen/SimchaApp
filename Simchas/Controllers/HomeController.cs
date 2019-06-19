using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Simchas.Data;
using Simchas.Models;

namespace Simchas.Controllers
{
    public class HomeController : Controller
    {
        private DbManager _mgr = new DbManager(Properties.Settings.Default.ConStr);

        public ActionResult Index()
        {
            SimchasViewModel vm = new SimchasViewModel();
            vm.Simchas = _mgr.GetSimchas();
            vm.Contributors = _mgr.GetContributorCount();
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddSimcha(Simcha simcha)
        {
            _mgr.AddSimcha(simcha);
            return Redirect("/home/index");
        }

        public ActionResult Contributions(int? simchaId)
        {
            if (!simchaId.HasValue)
            {
                return Redirect("/");
            }
            ContributionsViewModel vm = new ContributionsViewModel();
            vm.Simcha = _mgr.GetSimcha(simchaId.Value);
            vm.Contributions = _mgr.GetContributionsForSimcha(simchaId.Value);
            return View(vm);
        }

        [HttpPost]
        public ActionResult Contributions(IEnumerable<IncludeInContribution> contributions, int simchaId)
        {
            _mgr.UpdateSimchaContributions(contributions, simchaId);
            return Redirect("/");
        }
    }
}