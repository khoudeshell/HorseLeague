using SharpArch.Core.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseLeague.Models.Domain
{
    public class UserReport : Entity
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool? HasPaid { get; set; }

        public DateTime LastActivityDate { get; set; }
       
    }
}