using SharpArch.Core.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseLeague.Models.Domain
{
    public class LogItem : EntityWithTypedId<int>
    {
        public virtual int Id { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string HostName { get; set; }
        public virtual string Thread { get; set; }
        public virtual string Level { get; set; }
        public virtual string User { get; set; }
        public virtual string Message { get; set; }
        public virtual string Exception { get; set; }
    }
}