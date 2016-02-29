using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace FootballStar.Manager.Model
{
	// View-Model, para ayudar a la visualizacion de los partidos
	[JsonObject(MemberSerialization.OptOut)]
	public class MatchBrowser
	{
		public MatchBrowser ()
		{
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
		}
			
		public MatchBrowser (Player player, Tier tier)
		{
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
			
			mTier = tier;
		}
		
		public int   CurrentFriendlyIdx { get { return mCurrentFriendlyIdx; } }
		public void  NextFriendly() { mCurrentFriendlyIdx = Next(mTier.FriendlyMatches, mCurrentFriendlyIdx, LastFriendly); }
		public void  PrevFriendly() { mCurrentFriendlyIdx = Prev(mTier.FriendlyMatches, mCurrentFriendlyIdx, LastFriendly); }
		public Match CurrentFriendly { get { return mTier.FriendlyMatches[CurrentFriendlyIdx]; } }		
		public Match LastFriendly { get { return FindLastMatch(mTier.FriendlyMatches); } }
		
		public int   CurrentLeagueIdx { get { return mCurrentLeagueIdx; } }
		public void  NextLeague() { mCurrentLeagueIdx = Next(mTier.LeagueMatches, mCurrentLeagueIdx, LastLeague); }
		public void  PrevLeague() { mCurrentLeagueIdx = Prev(mTier.LeagueMatches, mCurrentLeagueIdx, LastLeague); }
		public Match CurrentLeague { get { return mTier.LeagueMatches[CurrentLeagueIdx]; } }		
		public Match LastLeague { get { return FindLastMatch(mTier.LeagueMatches); } }
		public int   LastLeagueIdx { get { return mTier.LeagueMatches.IndexOf(FindLastMatch(mTier.LeagueMatches)); } }
		
		public int   CurrentCupIdx { get { return mCurrentCupIdx; } }
		public void  NextCup() { mCurrentCupIdx = Next(mTier.CupMatches, mCurrentCupIdx, LastCup); }
		public void  PrevCup() { mCurrentCupIdx = Prev(mTier.CupMatches, mCurrentCupIdx, LastCup); }
		public Match CurrentCup { get { return mTier.CupMatches[CurrentCupIdx]; } }		
		public Match LastCup { get { return FindLastMatch(mTier.CupMatches); } }

		public int   CurrentEuroIdx { get { return mCurrentEuroIdx; } }
		public void  NextEuro() { mCurrentEuroIdx = Next(mTier.EuroMatches, mCurrentEuroIdx, LastEuro); }
		public void  PrevEuro() { mCurrentEuroIdx = Prev(mTier.EuroMatches, mCurrentEuroIdx, LastEuro); }
		public Match CurrentEuro { get { return mTier.EuroMatches[CurrentEuroIdx]; } }		
		public Match LastEuro { get { return FindLastMatch(mTier.EuroMatches); } }
		
		
		public void ResetBrowsingToLast()
		{
			mCurrentFriendlyIdx = mTier.FriendlyMatches.IndexOf(FindLastMatch(mTier.FriendlyMatches));
			mCurrentLeagueIdx = mTier.LeagueMatches.IndexOf(FindLastMatch(mTier.LeagueMatches));
			mCurrentCupIdx = mTier.CupMatches.IndexOf(FindLastMatch(mTier.CupMatches));
			mCurrentEuroIdx = mTier.EuroMatches.IndexOf(FindLastMatch(mTier.EuroMatches));
		}
		
		
		static private int Next(List<Match> matches, int currentIdx, Match last)
		{
			if (currentIdx < matches.Count - 1)
			{
				if (matches[currentIdx] != last)
					currentIdx++;
				else
					currentIdx = 0;	// Loopear aqui es opcional
			}
			else
				currentIdx = 0;
			
			return currentIdx;
		}
		static private int Prev(List<Match> matches, int currentIdx, Match last)
		{
			if (currentIdx > 0)
				currentIdx--;
			else
				currentIdx = matches.IndexOf(last);

			return currentIdx;
		}
		
		private Match FindLastMatch(List<Match> matches)
		{
			Match ret = null;
			
			if (mMainModel.AllMatchesUnlocked)
				return matches[matches.Count - 1];
			
			for (int c=0; c < matches.Count; ++c)
			{
				if (matches[c].MatchResult == null)
				{
					if (c > 0)
					{
						if (matches[c-1].MatchResult.PlayerWon)
							ret = matches[c];
						else
							ret = matches[c-1];
					}
					else
					{
						ret = matches[c];
					}
					break;
				}
				
				// Si todos ya han sido jugados, devolvemos el ultimo
				if (ret == null && c == matches.Count - 1)
					ret = matches[c];
			}			
			return ret;
		}
		
		[JsonIgnore]
		private MainModel mMainModel;
		private Tier mTier;
		
		private int mCurrentFriendlyIdx;
		private int mCurrentLeagueIdx;
		private int mCurrentCupIdx;
		private int mCurrentEuroIdx;
	}
}

