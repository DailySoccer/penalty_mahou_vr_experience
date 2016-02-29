using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace FootballStar.Manager.Model
{
	[JsonObject(MemberSerialization.OptOut)]
	public class League
	{
		public League()
		{
		}
		
		public League(int tierID)
		{
			mTierID = tierID;
			
			mCurrentTeamsOrder = new List<int>();
			
			// + 1 para incluir al player, empieza siempre el ultimo
			for (int c=0; c < LeagueDefinition.AllTiers[mTierID].InitialTeams.Length + 1; ++c)
			{
				mCurrentTeamsOrder.Add(c);
			}
			
			// El nombre del player siempre esta en la ultima posicion, pero evitamos anadirlo al array para que continue
			// siendo independiente
			mPlayerTeamOrder = LeagueDefinition.AllTiers[mTierID].InitialTeams.Length;
		}
		
		public List<string> CurrentTeamsOrder
		{
			get
			{
				var ret = new List<string>(mCurrentTeamsOrder.Count);
			
				for (int c = 0; c < mCurrentTeamsOrder.Count; ++c)
				{
					var teamIndex = mCurrentTeamsOrder[c];
					
					if (c == mPlayerTeamOrder)
						ret.Add(TierDefinition.AllTiers[mTierID].OwnTeamName);
					else
						ret.Add(LeagueDefinition.AllTiers[mTierID].InitialTeams[teamIndex]);
				}
				
				return ret;
			}
		}
		
		public int PlayerTeamOrder
		{
			get { return mPlayerTeamOrder; }
		}
		
		public void RefreshPlayerPosInRanking(int totalMatches, int currentMatchIdx)
		{
			int remainingMatches = totalMatches - currentMatchIdx;			
			int ratio = (int)(mCurrentTeamsOrder.Count / totalMatches);
			
			var oldPlayerOrder = mPlayerTeamOrder;
			mPlayerTeamOrder = (remainingMatches * ratio) - 1;
			
			if (mPlayerTeamOrder < 0)
				mPlayerTeamOrder = 0;
			
			mCurrentTeamsOrder.RemoveAt(oldPlayerOrder);
			mCurrentTeamsOrder.Insert(mPlayerTeamOrder, LeagueDefinition.AllTiers[mTierID].InitialTeams.Length);
		}
		
		private int mTierID;
		private List<int> mCurrentTeamsOrder;
		private int mPlayerTeamOrder;
	}
	
	public class LeagueDefinition
	{	
		public string[] InitialTeams;
		
		
		static public LeagueDefinition[] AllTiers = new LeagueDefinition[]
		{
			new LeagueDefinition()
			{ 
				InitialTeams = new string[] 
				{
					"Equipo 01 Tier1",
					"Equipo 02 Tier1",
					"Equipo 03 Tier1",
					"Equipo 04 Tier1",
					"Equipo 05 Tier1",
					"Equipo 06 Tier1",
					"Equipo 07 Tier1",
					"Equipo 08 Tier1",
					"Equipo 09 Tier1",
					"Equipo 10 Tier1",
					"Equipo 11 Tier1",
					"Equipo 12 Tier1",
					"Equipo 13 Tier1",
					"Equipo 14 Tier1",
					"Equipo 15 Tier1",
					"Equipo 16 Tier1",
					"Equipo 17 Tier1",
					"Equipo 18 Tier1",
					"Equipo 19 Tier1",
					"Equipo 20 Tier1",
					"Equipo 21 Tier1",
				}
			},
			new LeagueDefinition()
			{
				InitialTeams = new string[]
				{
					"Equipo 01 Tier2",
					"Equipo 02 Tier2",
					"Equipo 03 Tier2",
					"Equipo 04 Tier2",
					"Equipo 05 Tier2",
					"Equipo 06 Tier2",
					"Equipo 07 Tier2",
					"Equipo 08 Tier2",
					"Equipo 09 Tier2",
					"Equipo 10 Tier2",
					"Equipo 11 Tier2",
					"Equipo 12 Tier2",
					"Equipo 13 Tier2",
					"Equipo 14 Tier2",
					"Equipo 15 Tier2",
					"Equipo 16 Tier2",
					"Equipo 17 Tier2",
					"Equipo 18 Tier2",
					"Equipo 19 Tier2",
					"Equipo 20 Tier2",
					"Equipo 21 Tier2",
				}
			},
			new LeagueDefinition()
			{
				InitialTeams = new string[]
				{
					"F. C. Barcelona",
					"Atlético de Madrid",
					"Real Sociedad",
					"Valencia C. F.",
					"Málaga C. F.",
					"Real Betis Balompié",
					"Rayo Vallecano",
					"Sevilla F. C.",
					"Getafe C. F.",
					"Levante U.D.",
					"Athletic Club",
					"R.C.D. Espanyol",
					"Real Valladolid C.F.",
					"Granada C.F.",
					"C.A. Osasuna",
					"R.C. Celta de Vigo",
					"R.C.D. Mallorca",
					"R.C.D. de la Coruña",
					"Real Zaragoza",
				}
			}
		};
	}
}

