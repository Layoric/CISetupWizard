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
            //using (var sm = new ServerManager("C:\\Windows\\System32\\inetsrv\\config\\applicationHost.config")) // IIS Express is default, force IIS localhost
            using (var sm = new ServerManager())
            {
                var invalidChars = SiteCollection.InvalidSiteNameCharacters();
                if (siteName.IndexOfAny(invalidChars) > -1)
                {
                    throw new Exception(string.Format("Invalid Site Name: {0}", siteName));
                }
                var appPool = AddAppPool(sm, siteName, "v4.0", ManagedPipelineMode.Integrated);
                if (sm.Sites[siteName] != null)
                {
                    //return;
                    sm.Sites[siteName].Delete();
                    sm.CommitChanges();
                }
                string path = "C:\\inetpub\\{0}".Fmt(siteName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var site = sm.Sites.Add(siteName, path, 80);
                site.ServerAutoStart = true;
                site.ApplicationDefaults.ApplicationPoolName = appPool.Name;
                // Set HostName info for binding

                sm.CommitChanges();
            }
        }

        private static ApplicationPool AddAppPool(ServerManager sm, string poolName, string runtimeVersion,
                ManagedPipelineMode piplineMode)
        {
            if (sm.ApplicationPools.FirstOrDefault(x => x.Name == poolName) != null)
            {
                return sm.ApplicationPools.FirstOrDefault(x => x.Name == poolName);
            } 
            var appPool = sm.ApplicationPools.Add(poolName);
            appPool.ManagedRuntimeVersion = runtimeVersion;
            appPool.ManagedPipelineMode = piplineMode;
            return appPool;
        }
    }
}
