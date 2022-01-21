using SharpArch.Core.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseLeague.Models.Domain
{
    public class LogItem : EntityWithTypedId<int>
    {
#pragma warning disable CS0114 // 'LogItem.Id' hides inherited member 'EntityWithTypedId<int>.Id'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.
        public virtual int Id { get; set; }
#pragma warning restore CS0114 // 'LogItem.Id' hides inherited member 'EntityWithTypedId<int>.Id'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.
        public virtual DateTime Date { get; set; }
        public virtual string HostName { get; set; }
        public virtual string Thread { get; set; }
        public virtual string Level { get; set; }
        public virtual string User { get; set; }
        public virtual string Message { get; set; }
        public virtual string Exception { get; set; }
    }
}