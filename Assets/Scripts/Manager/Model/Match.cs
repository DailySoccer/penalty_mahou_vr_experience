using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FootballStar.Common;
using Newtonsoft.Json;

namespace FootballStar.Manager.Model
{
	[JsonObject(MemberSerialization.OptOut)]
	public class Match
	{
		public enum eMatchKind
		{
			FRIENDLY = 0,
			LEAGUE,
			CUP,
			EURO
		}

		public enum eMatchRelevance {
			IRRELEVANT,
			FINAL_LEAGUE,
			FINAL_CUP,
			FINAL_EURO
		}


		public int 		   		NumTimesPlayed 	{ get; set; }
		public MatchResult 		MatchResult 	{ get; set; }
		public TierDefinition 	TierDef			{ get { return TierDefinition.AllTiers[mTierID]; } }

		virtual public MatchDefinition Definition
		{
			get {
				return TierDef.GetMatchByKindAndID(mMatchKind, mMatchID);
			}
		}

		public Match() {
		}
		
		public Match(int tierID, eMatchKind kind, int matchID)
		{
			mTierID = tierID;
			mMatchKind = kind;
			mMatchID = matchID;

			mMatchRelevance = eMatchRelevance.IRRELEVANT;

			if (mMatchKind == eMatchKind.LEAGUE &&  matchID == TierDef.LeagueMatchesDefs.Length - 1)
				mMatchRelevance = eMatchRelevance.FINAL_LEAGUE;
			else if (mMatchKind == eMatchKind.CUP && matchID == TierDef.CupMatchesDefs.Length - 1)
				mMatchRelevance = eMatchRelevance.FINAL_CUP;
			else if (mMatchKind == eMatchKind.EURO && matchID == TierDef.EuroMatchesDefs.Length - 1)
				mMatchRelevance = eMatchRelevance.FINAL_EURO;			 
		}
		
		public bool IsAlreadyWon
		{
			get {
				return MatchResult != null && MatchResult.PlayerWon;
			}
		}

		public eMatchRelevance MatchRelevance { get { return mMatchRelevance; } }

		private int mTierID;
		private int mMatchID;
		private eMatchKind mMatchKind;
		private eMatchRelevance mMatchRelevance;

	}
}

