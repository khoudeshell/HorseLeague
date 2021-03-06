﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpArch.Core.DomainModel;
using System.Web.Security;

namespace HorseLeague.Models.Domain
{
    public class User : EntityWithTypedId<Guid>
    {
        public User() : base() 
        {
            this.UserLeagues = new List<UserLeague>();
        }

        public virtual string UserName { get; set; }
        public virtual IList<UserLeague> UserLeagues { get; set; }

        public virtual MembershipUser SecurityUser { get; set; }

        public virtual UserLeague GetUserLeague(League league)
        {
            return UserLeagues.Where(x => x.League == league).FirstOrDefault();
        }

        public virtual bool HasPaid
        {
            get
            {
                bool hasPaid = true;
                foreach (var ul in UserLeagues)
                {
                    hasPaid = hasPaid && (ul.HasPaid.HasValue ? ul.HasPaid.Value : false);
                    if (!hasPaid)
                        break;
                }

                return hasPaid;
            }
        }



    }
}