using System;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using FootballStar.Common;
using FootballStar.Manager.Model;

namespace FootballStar.Manager
{
	public class FriendlyScreen : PlayScreen
	{
		protected override string SmallTheaterTrophyName { get { return "Amistoso"; } }
		protected override Match  MatchToPlay { get { return mMainModel.CurrentTier.MatchBrowser.CurrentFriendly; } }

		protected override string MatchCount 
		{ 
			get { 
				return "[FFFFFF]AMISTOSO [-]" + (mMainModel.CurrentTier.MatchBrowser.CurrentFriendlyIdx + 1).ToString() + "/" + 
												 mMainModel.CurrentTier.FriendlyMatches.Count.ToString();
			}
		}

		protected override void GotoNextMatch() { mMainModel.CurrentTier.MatchBrowser.NextFriendly(); }
		protected override void GotoPrevMatch() { mMainModel.CurrentTier.MatchBrowser.PrevFriendly(); }
	}
}

