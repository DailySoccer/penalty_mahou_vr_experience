using UnityEngine;
using System.Collections;
using FootballStar.Common;
using FootballStar.Manager.Model;
using ExtensionMethods;
using System;
using System.Collections.Generic;

namespace FootballStar.Match3D {

	public class MatchEndStarter : MonoBehaviour
	{		
		public GameObject MatchEndPrefab;
		public GameObject MatchEndGameOverPrefab;
		public GameObject CompetitionCelebrationPrefab;
		public GameObject MatchEndBackground;

		void Awake()
		{
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			mMatchManager.OnMatchStart += OnMatchStart; 
			mMatchManager.OnMatchEnd += OnMatchEnd;
			if (MatchEndBackground == null) {
				Debug.LogError("MatchEndStarter: No tengo Background");
			}

		}

		void Start()
		{
			if (GameObject.FindGameObjectWithTag("GameModel") != null)
				mMatchBridge = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MatchBridge>();
		}
		
		void OnDestroy()
		{
			MatchEndBackground.SetActive (false);
			mMatchManager.OnMatchStart -= OnMatchStart;
			mMatchManager.OnMatchEnd -= OnMatchEnd;			
		}

		void Update()
		{
			// Queremos asegurar que el dialogo esta creado desde el principio (todavia en negro) para evitar el tiron visible
			if (mMatchEndDialog == null)	
				CreateMatchEndDialog();

			if (mMatchEndGameOverDialog == null)
				CreateMatchEndGameOverDialog();
		}
		
		void OnMatchStart (object sender, System.EventArgs e)
		{

			MatchEndBackground.SetActive(false);

			// Tratamos el Repeat
			if (mMatchEndDialog != null)
			{
				// Esto se producira en fade-out y en el siguiente Update haremos el Create (todavia en negro)
				// No podemos hacer el Create() aqui porque el Destroy no es inmediato, y DestroyImmeditate esta desaconsejado
				Destroy (mMatchEndDialog);
				mMatchEndDialog = null;
			}

			if (mMatchEndGameOverDialog != null)
			{
				Destroy (mMatchEndGameOverDialog);
				mMatchEndGameOverDialog = null;
			}
		}
		
		void OnMatchEnd (object sender, MatchManager.EventMatchEndArgs e)
		{
			if ( mMatchBridge != null && mMatchBridge.MatchRelevance != Match.eMatchRelevance.IRRELEVANT && mMatchBridge.ReturnMatchResult.PlayerWon ) {

				CreateCelebration();
				mCelebrationScreen.SetActive(true);
				mCelebrationScreen.GetComponent<CompetitionCelebration>().Initialize(mMatchBridge.MatchRelevance);
			}
			else {
				MatchEndBackground.SetActive(true);
				if (mMatchManager.MatchFailed)
					mMatchEndGameOverDialog.SetActive(true);
				else
					mMatchEndDialog.SetActive(true);
			}
		}

		private void CreateMatchEndDialog() {
			mMatchEndDialog = (Instantiate(MatchEndPrefab) as GameObject);
			mMatchEndDialog.transform.parent = this.transform;
			mMatchEndDialog.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			mMatchEndDialog.SetActive(false);
		}

		private void CreateMatchEndGameOverDialog() {
			mMatchEndGameOverDialog = (Instantiate(MatchEndGameOverPrefab) as GameObject);
			mMatchEndGameOverDialog.transform.parent = this.transform;
			mMatchEndGameOverDialog.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			mMatchEndGameOverDialog.SetActive(false);
		}

		private void CreateCelebration() {
			mCelebrationScreen = (Instantiate(CompetitionCelebrationPrefab) as GameObject);
			mCelebrationScreen.transform.parent = this.transform;
			mCelebrationScreen.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

			//TODO: Crear un evento que se dispare al cerrar la ventana y subscribirnos a el para lanzar el MatchendDialog al terminar la presentacion.
			mCelebrationScreen.GetComponent<CompetitionCelebration>().OnCelebrationEnds += OnCelebrationEndsHandler;
			mCelebrationScreen.SetActive(false);
		}

		private void OnCelebrationEndsHandler(object sender, EventArgs e)
		{
			mCelebrationScreen.GetComponent<CompetitionCelebration>().OnCelebrationEnds -= OnCelebrationEndsHandler;
			Destroy(mCelebrationScreen);
			mMatchEndDialog.SetActive(true);
		}
		
		GameObject mMatchEndDialog;
		GameObject mMatchEndGameOverDialog;
		GameObject mCelebrationScreen;
		MatchManager mMatchManager;
		MatchBridge mMatchBridge;


	}
}
