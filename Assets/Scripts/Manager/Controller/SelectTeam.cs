using UnityEngine;
using System.Collections;
using FootballStar.Manager.Model;

namespace FootballStar.Manager
	{
	public class SelectTeam : MonoBehaviour {

		public GameObject Selector;
		public UIButton PlayButton;

		GameObject selection;

		int SelectedTeam;
		
		void Awake () {
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
			mHeader = GameObject.Find("Header").GetComponent<Header>();

			PlayButton.isEnabled = false;
			Selector.SetActive (false);
		}

		void Update () {
			
		}

		public void Select(GameObject sender) {
			PlayButton.isEnabled = true;
			Selector.SetActive(true);
			Selector.transform.position = sender.transform.position;
			SelectedTeam = int.Parse(sender.name);
			Debug.Log ("El jugador elige: " + FootballStar.Manager.Model.TierDefinition.GetTeamName(SelectedTeam));
		}

		void SetupTeamAndContinue() {
			if (!PlayButton.isEnabled) {
				return;
			}

			mMainModel.SelectedTeamId = SelectedTeam;
			mMainModel.Player.IsTeamSelected = true;

			// Nos tenemos que desactivar mientras se hace el fade-out, etc

			StartCoroutine(GoToMainScreen());
				//enabled = false;
		}

		IEnumerator GoToMainScreen()
		{
			yield return StartCoroutine(CameraFade.FadeCoroutine(false, 1.0f, 0.0f));
			mHeader.GoToMainScreen();
		}
		
		MainModel mMainModel;
		Header mHeader;
	}
}