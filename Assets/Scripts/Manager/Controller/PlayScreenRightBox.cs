using System;
using System.Linq;
using UnityEngine;
using FootballStar.Common;
using FootballStar.Manager.Model;
using ExtensionMethods;

namespace FootballStar.Manager
{
	public class PlayScreenRightBox : MonoBehaviour
	{
		public Match MatchToDisplay 
		{ 
			get { return mMatchToDisplay; }
		 	set
			{
				mMatchToDisplay = value;
				ForceUpdate();
			}
		}
		
		void Awake()
		{
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
			
			var theLabels = gameObject.GetComponentsInChildren<UILabel>();
			mRewardLabel = theLabels.Where(label => label.name == "RewardValue Label").FirstOrDefault();
			mVSLabel = theLabels.Where(label => label.name == "VSValue Label").FirstOrDefault();
			mOwnTeamLabel = theLabels.Where(label => label.name == "VSOwnTeam").FirstOrDefault();
			mDifficultyLabel = theLabels.Where(label => label.name == "DifficultyValue Label").FirstOrDefault();
			mHelpText = theLabels.Where(label => label.name == "HelpText").FirstOrDefault();
			
			var theSprites = gameObject.GetComponentsInChildren<UISprite>();
			mMyBadgeSprite = theSprites.Where(comp => comp.name == "OwnBadge Sprite").FirstOrDefault();
			mOppBadgeSprite = theSprites.Where(comp => comp.name == "OppBadge Sprite").FirstOrDefault();			
			mDifficultySprites = theSprites.Where(comp => comp.name.Contains("Difficulty0")).OrderBy(comp => comp.name).ToArray();
			
			mPrecisionPanel = transform.FindChild("PrecisionPanel").gameObject;
			mRewardPanel = transform.FindChild("RewardPanel").gameObject;
			mAlreadyPlayed = transform.FindChild("AlreadyPlayed").gameObject;
			mNotPlayed = transform.FindChild("NotPlayed").gameObject;
			
			mBalls = gameObject.GetComponentsInChildren<UISprite>().Where(c => c.name.Contains("Ball")).OrderBy(c => c.name).ToArray();
		}

		void ForceUpdate()
		{
			var diffIndex = (int)mMainModel.GlobalDifficultyIndex(MatchToDisplay.Definition);
			var diffName = mMainModel.GlobalDifficultyName(MatchToDisplay.Definition);
			
			mRewardLabel.text = MatchToDisplay.Definition.Reward.FormatAsMoney();
			mVSLabel.text = MatchToDisplay.Definition.OpponentName.ToUpper();
			mOwnTeamLabel.text = mMainModel.CurrentTier.Definition.OwnTeamName.ToUpper();
			mDifficultyLabel.text = "[FFFFFF]DIFICULTAD [-]" + diffName;

			var diffColor = DIFF_COLORS[diffIndex];
			
			for (int c = 0;  c < mDifficultySprites.Count(); ++c)
			{
				if (c > diffIndex) {
					mDifficultySprites[c].enabled = false;
				}
				else {
					mDifficultySprites[c].enabled = true;
					mDifficultySprites[c].color = diffColor;
				}
			}
			
			mDifficultyLabel.color = diffColor;
			mMyBadgeSprite.spriteName = TierDefinition.GetTeamName(mMainModel.SelectedTeamId) + "_Small";
			mOppBadgeSprite.spriteName = MatchToDisplay.Definition.OpponentBadgeName + "_Small";
			
			if (MatchToDisplay.MatchResult == null || !mMatchToDisplay.MatchResult.PlayerWon)
			{
				mAlreadyPlayed.SetActive(false);
				mNotPlayed.SetActive(true);
				mPrecisionPanel.SetActive(false);
				mRewardPanel.SetActive(true);
				
				mHelpText.text = "Juega, gana y consigue recompensas\npara mejorar a tu jugador.";
			}
			else
			{
				mAlreadyPlayed.SetActive(true);
				mNotPlayed.SetActive(false);
				mPrecisionPanel.SetActive(true);
				mRewardPanel.SetActive(false);
				
				int numBalls = mMatchToDisplay.MatchResult.NumPrecisionBallsEndOfMatch;
				
				for (int c = 0; c < mBalls.Count(); ++c)
				{
					mBalls[c].enabled = c < numBalls;
				}

				mHelpText.text = (numBalls < mBalls.Count()) ? "Vuelve a jugar para aumentar tus [FFCB05]fans\n[FFFFFF]y conseguir más [FFCB05]dinero." : 
															   "Juega de nuevo y\n consigue más [FFCB05]dinero.";
			}
		}
		
		static Color32[] DIFF_COLORS = new Color32[] { new Color32(255, 190, 0, 255), 
													   new Color32(216, 145, 23, 255), 
													   new Color32(217, 84, 13, 255), 
													   new Color32(219, 0, 0, 255) };
		
		MainModel mMainModel;
		
		GameObject mPrecisionPanel;
		GameObject mRewardPanel;
		GameObject mAlreadyPlayed;
		GameObject mNotPlayed;
		
		UILabel mRewardLabel;
		UILabel mVSLabel;
		UILabel mOwnTeamLabel;
		UILabel mDifficultyLabel;
		UILabel mHelpText;
		
		UISprite[] mDifficultySprites;
		UISprite[] mBalls;

		UISprite mMyBadgeSprite;
		UISprite mOppBadgeSprite;
		Match mMatchToDisplay;
	}
}

