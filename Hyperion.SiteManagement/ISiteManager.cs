using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperion.SiteManagement
{
    public interface ISiteManager
    {
        IEnumerable<Site> GetSites();
        Task StartAsync(string siteName);
        Task StopAsync(string siteName);
        Task RestartAsync(string siteName);
    }
}
