using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Web.Mvc;

namespace Hyperion.Web.WebViews
{
    public class BaseWebViewPage<T> : WebViewPage<T>
    {
        string _FQDN;
        public string FQDN
        {
            get
            {
                if (string.IsNullOrEmpty(_FQDN))
                {
                    string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
                    string hostName = Dns.GetHostName();

                    if (!hostName.EndsWith(domainName, StringComparison.Ordinal))
                        hostName += "." + domainName;

                    _FQDN = hostName;
                }

                return _FQDN;
            }
        }

        public override void Execute() { }
    }
}