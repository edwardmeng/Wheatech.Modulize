using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Wheatech.Modulize.Web
{
    internal class ModulePageHandlerFactory : PageHandlerFactory
    {
        private readonly string _modulePath;

        protected internal ModulePageHandlerFactory(string modulePath)
        {
            _modulePath = modulePath;
        }

        public virtual IHttpHandler GetHandler(HttpContextBase context, string virtualPath)
        {
            // Virtual Path ----s up with query strings, so we need to strip them off
            int qmark = virtualPath.IndexOf('?');
            if (qmark != -1)
            {
                virtualPath = virtualPath.Substring(0, qmark);
            }
            virtualPath = ToAppRelativePath(context, PathUtils.ResolvePath(_modulePath, virtualPath));
            return base.GetHandler(context.ApplicationInstance.Context, context.Request.RequestType, virtualPath, null);
        }

        private string ToAppRelativePath(HttpContextBase context, string absolutePath)
        {
            var appSegments = SplitPathSegments(context.Request.PhysicalApplicationPath);
            var pathSegments = SplitPathSegments(absolutePath);
            while (appSegments.Count > 0 && pathSegments.Count > 0)
            {
                if (!string.Equals(appSegments[0], pathSegments[0], StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                appSegments.RemoveAt(0);
                pathSegments.RemoveAt(0);
            }
            var sb = new StringBuilder("~/");
            while (appSegments.Count > 0)
            {
                sb.Append("../");
                appSegments.RemoveAt(0);
            }
            for (int i = 0; i < pathSegments.Count; i++)
            {
                sb.Append(pathSegments[i]);
                if (i != pathSegments.Count - 1)
                {
                    sb.Append("/");
                }
            }
            return sb.ToString();
        }

        private IList<string> SplitPathSegments(string path)
        {
            List<string> parts = new List<string>();

            if (string.IsNullOrEmpty(path))
            {
                return parts;
            }

            int currentIndex = 0;

            // Split the incoming URL into individual parts
            while (currentIndex < path.Length)
            {
                int indexOfNextSeparator = path.IndexOfAny(new[] { '/', '\\' }, currentIndex);
                if (indexOfNextSeparator == -1)
                {
                    // If there are no more separators, the rest of the string is the last part
                    string finalPart = path.Substring(currentIndex);
                    if (finalPart.Length > 0)
                    {
                        parts.Add(finalPart);
                    }
                    break;
                }

                string nextPart = path.Substring(currentIndex, indexOfNextSeparator - currentIndex);
                if (nextPart.Length > 0)
                {
                    parts.Add(nextPart);
                }
                currentIndex = indexOfNextSeparator + 1;
            }

            return parts;
        }
    }
}
