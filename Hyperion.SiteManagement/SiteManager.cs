using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using IIS = Microsoft.Web.Administration;

namespace Hyperion.SiteManagement
{
    public class SiteManager : ISiteManager
    {

        public IEnumerable<Site> GetSites()
        {
            using (var iis = GetServerManager())
            {
                return iis.Sites
                          .Select(s => new Site(s, IsThisSite(s.Name)))
                          .ToList();
            }
        }

        public async Task StartAsync(string siteName)
        {
            var site = FindSite(siteName);

            if (site.State != IIS.ObjectState.Stopped)
                throw new InvalidOperationException($"Can only start a stopped site, site {siteName} is not stopped.");

            await Task.Factory.StartNew(() => site.Start());
        }

        public async Task StopAsync(string siteName)
        {
            var site = FindSite(siteName);

            if (site.State != IIS.ObjectState.Started)
                throw new InvalidOperationException($"Can only stop a started site, site {siteName} is not started.");

            if(IsThisSite(siteName))
                throw new InvalidOperationException($"Can't stop the Hyperion site {siteName}.");

            await Task.Factory.StartNew(() => site.Stop());
        }

        public async Task RestartAsync(string siteName)
        {
            var site = FindSite(siteName);

            if (site.State != IIS.ObjectState.Started)
                throw new InvalidOperationException($"Can only restart a started site, site {siteName} is not started.");

            await Task.Factory.StartNew(() =>
            {
                site.Stop();
                site.Start();
            });
        }

        IIS.Site FindSite(string siteName)
        {
            if (string.IsNullOrWhiteSpace(siteName))
                throw new ArgumentException("Can not be null or whitespace", nameof(siteName));

            using (var iis = GetServerManager())
            {
                var site = iis.Sites.FirstOrDefault(s => siteName.Equals(s.Name, StringComparison.Ordinal));

                if (site == null)
                    throw new Exception($"Site with name {siteName} could not be found.");

                return site;
            }
        }

        static IIS.ServerManager GetServerManager()
        {
            return new IIS.ServerManager(Environment.ExpandEnvironmentVariables("%windir%\\system32\\inetsrv\\config\\applicationHost.config"));
        }

        static bool IsThisSite(string siteName)
        {
            return siteName.Equals(HostingEnvironment.ApplicationHost.GetSiteName(), StringComparison.OrdinalIgnoreCase);
        }

    }
}
