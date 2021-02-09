using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpArch.Core.DomainModel;
using System.Text;

namespace HorseLeague.Models.Domain
{
    public class UserLeague : Entity
    {
        public enum PaymentTypes { NotPaid, PayPal, Check, Cash,  };

        public virtual IList<UserRaceDetail> UserRaceDetails { get; set; }
        public virtual User User { get; set; }
        public virtual League League { get; set; }
        public virtual bool? HasPaid { get; set; }
        public virtual PaymentTypes? PaymentType { get; set; }
        public virtual string PayPalPaymentToken { get; set; }
        public virtual string PayPalPayerId { get; set; }
        public virtual string PayPalPaymentId { get; set; }

        public virtual bool HasValidRaceCondition(LeagueRace leagueRace)
        {
            return GetPicksForARace(leagueRace).Count > 0;
        }


        public  virtual void AddUserPicksForRace(LeagueRace leagueRace, 
            IList<UserRaceDetail> userPicks)
        {
            if(userPicks == null || userPicks.Count != 4)
            {
                throw new InvalidPicksForARaceException("There must be 4 picks entered.");
            }
            else if(userPicks.Select(x => x.RaceDetail.Id).Distinct().ToList().Count != 4)
            {
                throw new InvalidPicksForARaceException("There must be a separate horse for each bet type.");
            }

            foreach(UserRaceDetail pick in userPicks)
            {
                addUserPickForARace(leagueRace, pick.RaceDetail, pick.BetType);
            }
        }
        public virtual bool HasUserSetPicksForRace(LeagueRace leagueRace)
        {
            return (from urd in UserRaceDetails
                    where urd.RaceDetail.LeagueRace == leagueRace
                    select urd).Count() > 0;
        }

        public virtual UserRaceDetail GetUserPickByType(LeagueRace leagueRace, BetTypes betType)
        {
            return this.GetPicksForARace(leagueRace).Where(x => x.BetType == betType).FirstOrDefault();
        }

        public virtual IList<UserRaceDetail> FixScratchesForRace(LeagueRace leagueRace)
        {
            IList<UserRaceDetail> userPicks = GetPicksForARace(leagueRace);

            IList<UserRaceDetail> adjustedPicks = findAndRemoveScratches(leagueRace, userPicks);
            shiftPicksUp(adjustedPicks);
            addRemainingPicks(leagueRace, adjustedPicks);

            foreach (UserRaceDetail newPick in adjustedPicks)
            {
                this.addUserPickForARace(leagueRace, newPick.RaceDetail, newPick.BetType);
            }

            return adjustedPicks;
        }

        public virtual IList<UserRaceDetail> GetPicksForARace(LeagueRace leagueRace)
        {
            return getPicksForARaceQueryable(leagueRace).ToList<UserRaceDetail>();
        }

        public virtual bool WasExactaWinner(LeagueRace leagueRace)
        {
            RaceDetailPayout winner = leagueRace.Win;
            RaceDetailPayout place = leagueRace.Place;

            IList<UserRaceDetail> userRaceDetail = GetPicksForARace(leagueRace);
            if (!this.HasValidRaceCondition(leagueRace))
                return false;

            return (winner.RaceDetail == userRaceDetail.Where(x => x.BetType == BetTypes.Win).FirstOrDefault().RaceDetail
                && place.RaceDetail == userRaceDetail.Where(x => x.BetType == BetTypes.Place).FirstOrDefault().RaceDetail);
        }

        public virtual bool WasTrifectaWinner(LeagueRace leagueRace)
        {
            RaceDetailPayout show = leagueRace.Show;
            IList<UserRaceDetail> userRaceDetail = GetPicksForARace(leagueRace);

            return WasExactaWinner(leagueRace) &&
                (show.RaceDetail == userRaceDetail.Where(x => x.BetType ==
                    BetTypes.Show).FirstOrDefault().RaceDetail);
        }

        private void addUserPickForARace(LeagueRace leagueRace, RaceDetail raceDetail, BetTypes betType)
        {
            var userPick = GetPicksForARace(leagueRace).Where(x => x.BetType == betType).FirstOrDefault();

            if (userPick != null && userPick.RaceDetail != raceDetail)
            {
                this.UserRaceDetails.Remove(userPick);
            }
            else if (userPick != null && userPick.RaceDetail == raceDetail)
            {
                return;
            }
            createAndAddUserRace(betType, raceDetail);
        }

        private void addRemainingPicks(LeagueRace leagueRace, IList<UserRaceDetail> adjustedPicks)
        {
            IList<RaceDetail> rds = leagueRace.RaceDetailsByOdds;
            foreach(RaceDetail rd in rds)
            {
                if ((adjustedPicks.Where(x => x.RaceDetail.Id == rd.Id).Count() == 0) && rd.IsScratched == 0)
                {
                    adjustedPicks.Add(new UserRaceDetail() { RaceDetail = rd, BetType = (BetTypes)(adjustedPicks.Count + 1) });
                }

                var disPicks = adjustedPicks.Select(x => x.RaceDetail.Id).Distinct();
                if (disPicks.ToList().Count == 4) break;
             }
        }
        
        private IList<UserRaceDetail> findAndRemoveScratches(LeagueRace leagueRace, IList<UserRaceDetail> picks)
        {
            IList<RaceDetail> scratches = leagueRace.GetScratches();
            IList<UserRaceDetail> adjustedPicks = new List<UserRaceDetail>();
   
            foreach (UserRaceDetail pick in picks)
            {
                if (!scratches.Contains(pick.RaceDetail))
                {
                    adjustedPicks.Add(pick.ShallowCopy());
                }
            }

            return adjustedPicks;
        }

        private void shiftPicksUp(IList<UserRaceDetail> picks)
        {
            int counter = (int)BetTypes.Win;

            foreach(UserRaceDetail pick in picks.OrderBy(x => x.BetType))
            {
                if(counter != (int)pick.BetType)
                {
                    pick.BetType = (BetTypes)counter;
                }

                counter++;
            }
        }
        private IQueryable<UserRaceDetail> getPicksForARaceQueryable(LeagueRace leagueRace)
        {
            return UserRaceDetails.Where(x => x.RaceDetail.LeagueRace == leagueRace).AsQueryable<UserRaceDetail>();
        }


        private void createAndAddUserRace(BetTypes betType, RaceDetail raceDetail)
        {
            UserRaceDetail urd = new UserRaceDetail();

            urd.BetType = betType;
            urd.RaceDetail = raceDetail;
            urd.UserLeague = this;
            urd.UpdateDate = System.DateTime.Now;

            this.UserRaceDetails.Add(urd);
        }

    }
}