using System.Collections.Specialized;
using System.Web;

namespace Wheatech.Modulize.Mvc
{
    internal class UrlRewriterHelper
    {
        private const string UrlWasRewrittenServerVar = "IIS_WasUrlRewritten";
        private const string UrlRewriterEnabledServerVar = "IIS_UrlRewriteModule";

        private readonly object _lockObject = new object();
        private bool _urlRewriterIsTurnedOnValue;
        private volatile bool _urlRewriterIsTurnedOnCalculated;

        private static bool WasThisRequestRewritten(HttpContextBase httpContext)
        {
            NameValueCollection serverVars = httpContext.Request.ServerVariables;
            bool requestWasRewritten = serverVars?[UrlWasRewrittenServerVar] != null;
            return requestWasRewritten;
        }

        private bool IsUrlRewriterTurnedOn(HttpContextBase httpContext)
        {
            // Need to do double-check locking because a single instance of this class is shared in the entire app domain (see PathHelpers)
            if (!_urlRewriterIsTurnedOnCalculated)
            {
                lock (_lockObject)
                {
                    if (!_urlRewriterIsTurnedOnCalculated)
                    {
                        var serverVars = httpContext.Request.ServerVariables;
                        bool urlRewriterIsEnabled = serverVars?[UrlRewriterEnabledServerVar] != null;
                        _urlRewriterIsTurnedOnValue = urlRewriterIsEnabled;
                        _urlRewriterIsTurnedOnCalculated = true;
                    }
                }
            }
            return _urlRewriterIsTurnedOnValue;
        }

        public virtual bool WasRequestRewritten(HttpContextBase httpContext)
        {
            return IsUrlRewriterTurnedOn(httpContext) && WasThisRequestRewritten(httpContext);
        }
    }
}
