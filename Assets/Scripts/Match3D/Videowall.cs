using UnityEngine;
using System.Collections;
using FootballStar.Common;

namespace FootballStar.Match3D
{
	public class Videowall : MonoBehaviour
	{
		void Awake ()
		{
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
						
			mVideowalls[0] = GameObject.Find("Videomarcador01");
			mVideowalls[1] = GameObject.Find("Videomarcador02");
			
			if (mVideowalls[0] == null || mVideowalls[1] == null) {
				enabled = false;
			} 
			else {
				mMatchManager.OnCutsceneBegin += OnCutsceneBegin;
				mMatchManager.OnMatchEnd += OnMatchEnd;

				for (int c = 0; c < 2; ++c) {
					mScoreLocalLabels[c] = mVideowalls[c].transform.Find("ScoreLocalParent/ScoreLocalLabel").GetComponent<UILabel>();
					mScoreVisitanteLabels[c] = mVideowalls[c].transform.Find("ScoreVisitanteParent/ScoreVisitanteLabel").GetComponent<UILabel>();
				}
			}
		}

		void OnMatchEnd (object sender, MatchManager.EventMatchEndArgs e)
		{
			// Aseguramos que al finalizar el partido siempre es el resultado correcto, independientemente de las cutscenes
			// Si el videowall se viera en otro momento, entonces habria que hacer una logica un poco mas lista que discirniera
			// entre OnCutsceneBegin / OnNewPlay
			for (int c = 0; c < 2; ++c) {
				mScoreLocalLabels[c].text = mMatchManager.CurrentPlayerGoals.ToString();
				mScoreVisitanteLabels[c].text = mMatchManager.CurrentOpponentGoals.ToString();
			}
		}

		void OnCutsceneBegin (object sender, System.EventArgs e)
		{
			StartCoroutine(OnCutsceneBeginCoroutine());
		}
		
		IEnumerator OnCutsceneBeginCoroutine()
		{
			// El score visitante lo cambiamos rapido, sin animacion
			for (int c = 0; c < 2; ++c)
				mScoreVisitanteLabels[c].text = mMatchManager.CurrentOpponentGoals.ToString();
			
			// Quitamos el valor antiguo
			for (int c = 0; c < 2; ++c)
				mScoreLocalLabels[c].GetComponent<Animation>().Play("MarcadorOut");
			
			yield return StartCoroutine(GeneralUtils.WaitForAnimation(mScoreLocalLabels[0].GetComponent<Animation>()));
			
			// Ponemos el nuevo
			for (int c = 0; c < 2; ++c) {
				mScoreLocalLabels[c].text = mMatchManager.CurrentPlayerGoals.ToString();
				mScoreLocalLabels[c].GetComponent<Animation>().Play("MarcadorIn");
			}						
		}
			
		void Start ()
		{
			mMatchBridge = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MatchBridge>();
			
			for (int c = 0; c < 2; ++c)
			{
				mVideowalls[c].transform.Find("TeamVisitanteParent/TeamVisitanteSprite").GetComponent<UISprite>().spriteName = mMatchBridge.CurrentMatchDefinition.OpponentBadgeName;
			}
		}
		
		void Update ()
		{
		}
		
		MatchManager mMatchManager;
		MatchBridge mMatchBridge;
		
		GameObject[] mVideowalls = new GameObject[2];
		
		UILabel[] mScoreLocalLabels = new UILabel[2];
		UILabel[] mScoreVisitanteLabels = new UILabel[2];
	}
	
}
