using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hyperion.SiteManagement;
using System.Net.NetworkInformation;
using System.Net;
using System.Threading.Tasks;

namespace Hyperion.Web.Controllers
{
    public class ApplicationController : Controller
    {
        readonly ISiteManager _siteManager;

        public ApplicationController(ISiteManager siteManager)
        {
            _siteManager = siteManager;
        }

        public ActionResult Index()
        {
            var sites = (from site in _siteManager.GetSites()
                         orderby site.State descending, site.Name
                         select site).ToList();

            return View(sites);
        }

        [HttpPost, ActionName("site-action")]
        public async Task<ActionResult> SiteAction(string site, string action)
        {
            switch (action.ToLower())
            {
                case "stop":
                    await _siteManager.StopAsync(site);
                    break;
                case "restart":
                    await _siteManager.RestartAsync(site);
                    break;
                default:
                    await _siteManager.StartAsync(site);
                    break;
            }

            return RedirectToAction("Index");
        }
    }
}