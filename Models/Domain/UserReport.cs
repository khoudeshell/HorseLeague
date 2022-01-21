using SharpArch.Core.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseLeague.Models.Domain
{
    public class UserReport : Entity
    {
#pragma warning disable CS0114 // 'UserReport.Id' hides inherited member 'EntityWithTypedId<int>.Id'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.
        public Guid Id { get; set; }
#pragma warning restore CS0114 // 'UserReport.Id' hides inherited member 'EntityWithTypedId<int>.Id'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool? HasPaid { get; set; }

        public DateTime LastActivityDate { get; set; }
       
    }
}