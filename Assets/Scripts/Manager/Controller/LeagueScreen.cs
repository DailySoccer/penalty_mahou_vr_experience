using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FootballStar.Common;
using FootballStar.Manager.Model;

namespace FootballStar.Manager
{	
	public class LeagueScreen : PlayScreen
	{
		protected override string SmallTheaterTrophyName { get { return ""; } }	// El small theater deja de pintar lo q estuviera pintando
		protected override Match  MatchToPlay { get { return mMainModel.CurrentTier.MatchBrowser.CurrentLeague; } }
		
		protected override string MatchCount 
		{ 
			get { return "[FFFFFF]PARTIDO DE LIGA [-]" + (mMainModel.CurrentTier.MatchBrowser.CurrentLeagueIdx + 1).ToString(); }
		}
		
		protected override void GotoNextMatch() { mMainModel.CurrentTier.MatchBrowser.NextLeague(); }
		protected override void GotoPrevMatch() { mMainModel.CurrentTier.MatchBrowser.PrevLeague(); }


		protected override void PostAwake(GameObject leftBox, UILabel[] theLabels)
		{			
			// En esta pantalla la parte de abajo tiene la tabla de clasificacion, quitamos el background del PlayScreenLeftBox.
			leftBox.transform.FindChild("BackgroundLower").gameObject.SetActive(false);
									
			mPositionLabels = theLabels.Where(label => label.name.Contains("Number")).OrderBy(label => label.transform.parent.name).ToList();
			mTeamName = theLabels.Where(label => label.name.Contains("TeamName")).OrderBy(label => label.transform.parent.name).ToList();
			mRowBackground = gameObject.GetComponentsInChildren<UISprite>().Where(spr => spr.name == "SlotBackground").OrderBy(spr => spr.transform.parent.name).ToList();
			
			if (mPositionLabels.Count != mTeamName.Count)
				throw new Exception("WTF 405a - Deberia haber igual numero de labels");
		}

		
		protected override void ForceUpdate()
		{
			base.ForceUpdate();

			List<string> currentOrder = mMainModel.CurrentTier.League.CurrentTeamsOrder;
			int playerIdx = mMainModel.CurrentTier.League.PlayerTeamOrder;
			
			int lowerIdx = playerIdx - (int)Mathf.Floor((float)mPositionLabels.Count/2);
			int highIdx = playerIdx + (int)Mathf.Ceil((float)mPositionLabels.Count/2);
			
			if (lowerIdx < 0)
			{
				highIdx += -lowerIdx;
				lowerIdx = 0;
			}
			else
			if (highIdx >= currentOrder.Count)
			{
				lowerIdx -= (highIdx - currentOrder.Count);
				highIdx = currentOrder.Count;
			}
			
			for (int c = 0; c < mPositionLabels.Count; ++c, ++lowerIdx)
			{
				mPositionLabels[c].text = (lowerIdx + 1).ToString();
				mTeamName[c].text = currentOrder[lowerIdx];
				
				if (lowerIdx == playerIdx)
					mRowBackground[c].color = new Color(249.0f/255.0f, 191.0f/255.0f, 33.0f/255.0f, 1.0f);
				else
					mRowBackground[c].color = new Color(140.0f/255.0f, 140.0f/255.0f, 140.0f/255.0f, 1.0f);
			}
		}

		List<UILabel> mPositionLabels;
		List<UILabel> mTeamName;
		List<UISprite> mRowBackground;
	}
}

