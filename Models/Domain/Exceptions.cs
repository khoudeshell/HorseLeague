using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseLeague.Models.Domain
{
    public class InvalidPicksForARaceException : ApplicationException
    {
        public IList<string> MissingPicks { get; private set; }

        public InvalidPicksForARaceException() : this(String.Empty) {}
        
        public InvalidPicksForARaceException(string message) : base(message) 
        {
            MissingPicks = new List<string>();
        }
    }

}