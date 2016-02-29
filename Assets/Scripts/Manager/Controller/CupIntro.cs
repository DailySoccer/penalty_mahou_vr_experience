using UnityEngine;
using System.Collections;
using FootballStar.Manager.Model;

public class CupIntro : MonoBehaviour {

	public UILabel texto;
	// Use this for initialization
	void Awake () {
		mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
		texto.text = texto.text.Replace ("@TeamName", TierDefinition.GetTeamName(mMainModel.SelectedTeamId));
	}

	MainModel mMainModel;
}
