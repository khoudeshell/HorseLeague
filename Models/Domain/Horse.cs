using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpArch.Core.DomainModel;

namespace HorseLeague.Models.Domain
{
    public class Horse : Entity
    {
        private string name;

        public virtual string Name { get { return name; } set { this.name = SanitizedName(value); } }

        public static string SanitizedName(string unSanitizedName)
        {
            return unSanitizedName != null ? unSanitizedName.Trim() : unSanitizedName;
        }
    }
}