using System;
using System.Linq;
using UnityEngine;
using FootballStar.Common;
using FootballStar.Manager.Model;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Manager
{	
	public class CupScreen : PlayScreen
	{
		public GameObject CupIntroPrefab;

		protected override string SmallTheaterTrophyName { get { return "CupTrophy01"; } }
		protected override Match  MatchToPlay { get { return mMainModel.CurrentTier.MatchBrowser.CurrentCup; } }
		
		protected override string MatchCount 
		{ 
			get {
				// Tiene que haber 7 partidos: 2 Octavos, 2 Cuartos, 2 Semis, 1 Final
				string[] All = new string[]
				{
					"COPA OCTAVOS IDA", "COPA OCTAVOS VUELTA",
					"COPA CUARTOS IDA", "COPA CUARTOS VUELTA",
					"COPA SEMIFINALES IDA", "COPA SEMIFINALES VUELTA",
					"COPA FINAL"
				};
				return "[FFFFFF]" + All[mMainModel.CurrentTier.MatchBrowser.CurrentCupIdx];
			}
		}
		
		protected override void GotoNextMatch() { mMainModel.CurrentTier.MatchBrowser.NextCup(); }
		protected override void GotoPrevMatch() { mMainModel.CurrentTier.MatchBrowser.PrevCup(); }

		protected override void PlayMatch()
		{
			StartCoroutine(PlayMatchCoroutine());
		}
		
		IEnumerator PlayMatchCoroutine()
		{
			if (mMainModel.Player.TutorialStage == TutorialStage.CUP_EXPLANATION)
			{
				yield return StartCoroutine(CameraFade.FadeCoroutine(false, 0.4f, 0.0f));
				
				GameObject.Find("SmallTheater").GetComponent<SmallTheater>().HideCurrentObject();
				var cameraFade = GameObject.FindGameObjectWithTag("GameModel").GetComponent<CameraFade>();
								
				mCupIntro = NGUITools.AddChild(this.gameObject, CupIntroPrefab);
				mCupIntro.transform.localPosition = new Vector3(0, 0, -900);
				mCupIntro.GetComponentInChildren<UIButtonMessage>().target = this.gameObject;

				yield return StartCoroutine(CameraFade.FadeCoroutine(true, 0.4f, 0.0f));
				cameraFade.enabled = false;
			}
			else
			{
				base_PlayMatch();
			}
		}

		void base_PlayMatch() 
		{
			base.PlayMatch();
		}

		// Click del background de la CUP_EXPLANATION
		void OnContinueClick()
		{
			MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "COMPETICION PLAY"} });
			base_PlayMatch();
		}


		GameObject mCupIntro;
	}
}

