using UnityEngine;
using System.Collections;
using FootballStar.Manager.Model;

public class PlayButtonTweener : MonoBehaviour {

	// Use this for initialization
	void Start () {
		mMainModelComponent = GameObject.Find("GameModel").GetComponent<MainModel>();
	}
	
	void Update(){}
	
	// Update is called once per frame
	void OnClick () 
	{
		if( mMainModelComponent.CanIPlayMatches() )
		 	AMTween.MoveBy( gameObject, AMTween.Hash("time", 0.5f, "x", 0.65f, "easetype","easeInOutBack" ) );
	}
	
	private MainModel mMainModelComponent;
}
