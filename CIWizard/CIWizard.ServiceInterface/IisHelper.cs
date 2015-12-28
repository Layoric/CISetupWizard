using System;
using System.IO;
using System.Linq;
using Microsoft.Web.Administration;
using ServiceStack;

namespace CIWizard.ServiceInterface
{
    public static class IisHelper
    {
        public static void AddSite(string siteName, string hostName = null)
        {
            using (var sm = new ServerManager())
            {
                var invalidChars = SiteCollection.InvalidSiteNameCharacters();
                if (siteName.IndexOfAny(invalidChars) > -1)
                {
                    throw new Exception("Invalid Site Name: {0}".Fmt(siteName));
                }
                var appPool = AddAppPool(sm, siteName, "v4.0", ManagedPipelineMode.Integrated);
                if (sm.Sites[siteName] != null)
                {
                    return;
                }
                string path = "C:\\inetpub\\wwwroot\\{0}".Fmt(siteName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var site = hostName != null ? sm.Sites.Add(siteName, "http", "*:80:{0}".Fmt(hostName), path) : sm.Sites.Add(siteName, path, 80);
                site.ServerAutoStart = true;
                site.ApplicationDefaults.ApplicationPoolName = appPool.Name;
                // Set HostName info for binding

                sm.CommitChanges();

                AddMsDeployAccessToSite(sm, siteName, "wizard_deploy");

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

        private static void AddMsDeployAccessToSite(ServerManager sm,string siteName, string iisMgrUserName)
        {
            Configuration config = sm.GetAdministrationConfiguration();
            ConfigurationSection authorizationSection = config.GetSection("system.webServer/management/authorization");
            ConfigurationElementCollection authorizationRulesCollection = authorizationSection.GetCollection("authorizationRules");

            ConfigurationElement scopeElement = FindElement(authorizationRulesCollection, "scope", "path", @"/{0}".Fmt(siteName));
            if (scopeElement == null)
            {
                scopeElement = authorizationRulesCollection.CreateElement("scope");
                scopeElement["path"] = @"/{0}".Fmt(siteName);
                authorizationRulesCollection.Add(scopeElement);
            }

            ConfigurationElementCollection scopeCollection = scopeElement.GetCollection();
            ConfigurationElement addElement = scopeCollection.CreateElement("add");
            addElement["name"] = iisMgrUserName;
            scopeCollection.Add(addElement);

            sm.CommitChanges();
        }

        private static ConfigurationElement FindElement(ConfigurationElementCollection collection, string elementTagName, params string[] keyValues)
        {
            foreach (ConfigurationElement element in collection)
            {
                if (string.Equals(element.ElementTagName, elementTagName, StringComparison.OrdinalIgnoreCase))
                {
                    bool matches = true;
                    for (int i = 0; i < keyValues.Length; i += 2)
                    {
                        object o = element.GetAttributeValue(keyValues[i]);
                        string value = null;
                        if (o != null)
                        {
                            value = o.ToString();
                        }
                        if (!string.Equals(value, keyValues[i + 1], StringComparison.OrdinalIgnoreCase))
                        {
                            matches = false;
                            break;
                        }
                    }
                    if (matches)
                    {
                        return element;
                    }
                }
            }
            return null;
        }
    }
}
