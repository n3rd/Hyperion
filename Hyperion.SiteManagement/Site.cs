using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IIS = Microsoft.Web.Administration;

namespace Hyperion.SiteManagement
{

    public enum SiteState
    {
        Stopped,
        Started
    }

    public class Site
    {

        public string Name { get; private set; }

        public IEnumerable<Uri> Urls { get; private set; }

        public SiteState State { get; private set; }

        public bool IsCurrentSite { get; private set; }

        public Uri Url
        {
            get
            {
                return Urls.FirstOrDefault();
            }
        }

        internal Site(IIS.Site iisSite, bool isCurrentSite)
        {
            Name = iisSite.Name;
            Urls = iisSite.Bindings
                          .Select(b => new UriBuilder
                          {
                               Host = string.IsNullOrEmpty(b.Host) ? "localhost" : b.Host,
                               Scheme = b.Protocol,
                               Port = b.EndPoint.Port == 80 ? -1 : b.EndPoint.Port
                          }.Uri);

            if (new[] { IIS.ObjectState.Started, IIS.ObjectState.Starting }.Any(os => os == iisSite.State))
                State = SiteState.Started;

            IsCurrentSite = isCurrentSite;
        }

    }
}
