using FluentNHibernate.Mapping;
using HorseLeague.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseLeague.Models.DataAccess.Mapping
{
    public class LogsMap : ClassMap<LogItem>
    {
        public LogsMap()
        {
            Table("Log");

            Id(x => x.Id, "Id");

            Map(x => x.Date, "Date");
            Map(x => x.HostName, "HostName");
            Map(x => x.Thread, "Thread");
            Map(x => x.Level, "Level");
            Map(x => x.User, "[User]");
            Map(x => x.Message, "Message");
            Map(x => x.Exception, "Exception");
            
            Cache.ReadOnly();
        }
    }
}