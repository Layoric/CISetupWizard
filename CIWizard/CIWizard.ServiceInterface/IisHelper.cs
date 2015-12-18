using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;
using ServiceStack;

namespace CIWizard.ServiceInterface
{
    public static class IisHelper
    {
        public static void AddSite(string siteName)
        {
            using (var sm = new ServerManager())
            {
                var invalidChars = SiteCollection.InvalidSiteNameCharacters();
                if (siteName.IndexOfAny(invalidChars) > -1)
                {
                    throw new Exception(string.Format("Invalid Site Name: {0}", siteName));
                }
                AddAppPool(sm, siteName + "_AppPool", "v4.0", ManagedPipelineMode.Integrated);
                if (sm.Sites["siteName"] != null)
                {
                    return;
                }
                string path = "C:\\inetpub\\{0}\\".Fmt(siteName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var site = sm.Sites.Add(siteName, path, 80);
                site.ServerAutoStart = true;

                sm.CommitChanges();
            }
        }

        private static void AddAppPool(ServerManager sm, string poolName, string runtimeVersion,
                ManagedPipelineMode piplineMode)
        {
            if (sm.ApplicationPools.FirstOrDefault(x => x.Name == poolName) != null)
            {
                return;
            } 
            var appPool = sm.ApplicationPools.Add(poolName);
            appPool.ManagedRuntimeVersion = runtimeVersion;
            appPool.ManagedPipelineMode = piplineMode;
        }
    }
}
