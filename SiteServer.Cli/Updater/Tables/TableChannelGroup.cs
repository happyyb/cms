﻿using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;


namespace SiteServer.Cli.Updater.Tables
{
    public partial class TableChannelGroup
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nodeGroupName")]
        public string NodeGroupName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class TableChannelGroup
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_NodeGroup",
            "wcm_NodeGroup"
        };

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.ChannelGroupRepository.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.ChannelGroupRepository.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(ChannelGroup.GroupName), nameof(NodeGroupName)},
                {nameof(ChannelGroup.SiteId), nameof(PublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}