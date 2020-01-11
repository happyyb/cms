using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using SiteServer.CMS.Context;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Abstractions;

namespace SiteServer.CMS.DataCache
{
    public class PermissionConfigManager
	{
        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(PermissionConfigManager));

        public class PermissionConfig
        {
            public PermissionConfig(string name, string text)
            {
                Name = name;
                Text = text;
            }

            public string Name { get; set; }

            public string Text { get; set; }
        }

        

	    public List<PermissionConfig> GeneralPermissions { get; } = new List<PermissionConfig>();

	    public List<PermissionConfig> WebsitePermissions { get; } = new List<PermissionConfig>();

	    public List<PermissionConfig> WebsiteSysPermissions { get; } = new List<PermissionConfig>();

	    public List<PermissionConfig> WebsitePluginPermissions { get; } = new List<PermissionConfig>();

        public List<PermissionConfig> ChannelPermissions { get; } = new List<PermissionConfig>();

	    private PermissionConfigManager()
		{
		}

        public static async Task<PermissionConfigManager> GetInstanceAsync()
		{
            var permissionManager = DataCacheManager.Get<PermissionConfigManager>(CacheKey);
            if (permissionManager != null) return permissionManager;

            permissionManager = new PermissionConfigManager();

            var path = WebUtils.GetMenusPath("Permissions.config");
            if (FileUtils.IsFileExists(path))
            {
                var doc = new XmlDocument();
                doc.Load(path);
                await permissionManager.LoadValuesFromConfigurationXmlAsync(doc);
            }

            DataCacheManager.Insert(CacheKey, permissionManager, path);
            return permissionManager;
        }

        private async Task LoadValuesFromConfigurationXmlAsync(XmlDocument doc) 
		{
            var coreNode = doc.SelectSingleNode("Config");
		    if (coreNode != null)
		    {
                var isMultiple = true;
                foreach (XmlNode child in coreNode.ChildNodes)
		        {
		            if (child.NodeType == XmlNodeType.Comment) continue;
		            if (child.Name == "generalPermissions")
		            {
		                GetPermissions(child, GeneralPermissions);
		            }
		            else if (child.Name == "websitePermissions")
		            {
		                GetPermissions(child, WebsiteSysPermissions);
                        GetPermissions(child, WebsitePermissions);
		            }
		            else if (child.Name == "channelPermissions")
		            {
		                GetPermissions(child, ChannelPermissions);
		            }
		            else
		            {
		                isMultiple = false;
		                break;
		            }
		        }
		        if (!isMultiple)
		        {
		            GetPermissions(coreNode, GeneralPermissions);
		        }
		    }

            GeneralPermissions.AddRange(await PluginMenuManager.GetTopPermissionsAsync());
		    var pluginPermissions = await PluginMenuManager.GetSitePermissionsAsync(0);
            WebsitePluginPermissions.AddRange(pluginPermissions);
		    WebsitePermissions.AddRange(pluginPermissions);
        }

        private static void GetPermissions(XmlNode node, List<PermissionConfig> list) 
		{
            foreach (XmlNode permission in node.ChildNodes) 
			{
			    if (permission.Name == "add" && permission.Attributes != null)
			    {
                    list.Add(new PermissionConfig(permission.Attributes["name"].Value, permission.Attributes["text"].Value));
                }
			}
		}
	}
}