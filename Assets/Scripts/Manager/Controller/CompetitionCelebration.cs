using UnityEngine;
using System;
using System.Collections;
using FootballStar.Manager.Model;
using FootballStar.Audio;

public class CompetitionCelebration : MonoBehaviour {
	 
	public Transform ParticleContainer;
	public Transform TrophyContainer;
	public event EventHandler OnCelebrationEnds;

	// Diferentes mensajes para cada competicion
	public UILabel TitleLabel;
	public UILabel ContentLabel;
	public GameObject FBSharingButton;
	private string FinalCopaTitle 		= "¡Así se juega!";
	private string FinalCopaContent		= "Has ganado todos los partidos de la Copa. \n ¡Gracias a ti, la leyenda del Real Madrid es aún más grande!";									
	private string FinalEurocopaTitle 	= "¡Espectacular!";
	private string FinalEurocopaContent = "Has conseguido que el Real Madrid toque el cielo de Europa. \n ¡Eres un jugador 5 estrellas!";
	private string FinalLigaTitle 		= "¡Hala Madrid!";									
	private string FinalLigaContent 	= "Con tu ayuda, hemos conquistado la Liga. \n ¡Menuda lección de buen fútbol!";

	public void Initialize(Match.eMatchRelevance finalType) 
	{
		switch ( finalType )  
		{
			case Match.eMatchRelevance.FINAL_CUP:
			{
				mTrophy = (GameObject)Instantiate( Resources.Load("Trophies/trophy01",     typeof(GameObject)) );	
				TitleLabel.text = FinalCopaTitle;
				ContentLabel.text = FinalCopaContent;
				mCompetitionName = "la Copa";
				break;
			}
			case Match.eMatchRelevance.FINAL_EURO:
			{
				mTrophy = (GameObject)Instantiate( Resources.Load("Trophies/EuroTrophy01", typeof(GameObject)) );	
				TitleLabel.text = FinalEurocopaTitle;
				ContentLabel.text = FinalEurocopaContent;
				mCompetitionName = "la Eurocopa";
			break;
			}
			case Match.eMatchRelevance.FINAL_LEAGUE:
			{
				mTrophy = (GameObject)Instantiate( Resources.Load("Trophies/TrophyLiga01", typeof(GameObject)) );	
				TitleLabel.text = FinalLigaTitle;
				ContentLabel.text = FinalLigaContent;
				mCompetitionName = "la Liga";
			break;
			}
		}

		//Set The Parent
		mTrophy.transform.parent = m3DCam.transform; //TrophyContainer.transform;

		//Set the layer
		mTrophy.layer = m3DCam.gameObject.layer;
		foreach ( Transform child in mTrophy.transform)
		{
			child.gameObject.layer = m3DCam.gameObject.layer;
		}


		//Set the Position and Scale
		PutTheTrophy(finalType);

		//Deactivate (It is activated in the anumation)		
		mTrophy.SetActive(false);		

		//Put the Confetti
		mTheConfetti = (GameObject)Instantiate( Resources.Load("Particles/Confetti", typeof(GameObject)) );
		mTheConfetti.layer = ParticleContainer.gameObject.layer;
		mTheConfetti.transform.parent = ParticleContainer;
		mTheConfetti.transform.localPosition = new Vector3(0,20,0);
		mTheConfetti.transform.localScale = new Vector3(150,1,10);

		//Play the animation
		transform.GetComponent<Animation>().Play("YouWonTheCompetition");
	}

	void ShowTrophy()
	{
		mTrophy.SetActive(true);
		FBSharingButton.SetActive (true);
	}

	void OnCompetitionCelebrationButtonClick()
	{
		OnCelebrationEnds( this, EventArgs.Empty );
		mAudioGameController.PlayDefinition( SoundDefinitions.MATCH_ENDMUSIC_GOOD, false );
		mTrophy.SetActive(false);
	}

	void Awake()
	{
		GameObject gameModel = GameObject.FindGameObjectWithTag("GameModel");
		mMainModel = gameModel.GetComponent<MainModel>();
		m3DCam = GameObject.FindGameObjectWithTag("3DOverlayCamera").GetComponent<Camera>();
	}

	void Start () 
	{
		mAudioGameController = GameObject.FindGameObjectWithTag("GameModel").GetComponent<AudioInGameController>();
		mAudioGameController.PlayDefinition(SoundDefinitions.COMPETITION_VICTORY, true);
	}

	void Update () {}

	void PutTheTrophy(Match.eMatchRelevance finalType)
	{
		switch( finalType )
		{
			case Match.eMatchRelevance.FINAL_LEAGUE:
				mTrophy.transform.localPosition = new Vector3(0, -0.45f, 2.5f);
				mTrophy.transform.localScale = new Vector3(1f, 1f, 1f);
				break;
			case Match.eMatchRelevance.FINAL_CUP:
				mTrophy.transform.localPosition = new Vector3(0, -0.22f, 1.3f);
				mTrophy.transform.localScale = new Vector3(1f, 1f, 1f);
				break;
			case Match.eMatchRelevance.FINAL_EURO:
				mTrophy.transform.localPosition = new Vector3(0, -0.4f, 2.5f);
				mTrophy.transform.localScale = new Vector3(1f, 1f, 1f);
			break;
		}
	}


	void OnCompartirCampeonatoGanadoEnFacebook()
	{
		if (!mMainModel.FacebookInitialized)                                                                                                                         
			FBInit();
		else
			CallFBLogin();
	}

	private void FBInit()
	{
		FB.Init (OnFBInitComplete, OnHideUnity);
	}
	
	private void OnFBInitComplete()
	{
		//Debug.Log( string.Format("FB.Init completed: Is user logged in? {0} FB ID:{1}", FB.IsLoggedIn, FB.UserId) );
		mMainModel.FacebookInitialized = true;
		CallFBLogin();
	}
	
	private void OnHideUnity(bool isGameShown)
	{
		Debug.Log("Is game showing? " + isGameShown);
	} 
	
	public void CallFBLogin()
	{
		if (!FB.IsLoggedIn)
			FB.Login (FacebookUtils.FB_PERMISSIONS, FBLoginCallback);
		else
			PublicaVictoriaEnFB();
	}
	
	void FBLoginCallback(FBResult result)
	{
		mLastFBResponse = "";
		if (result.Error != null)
			mLastFBResponse = "Error Response:\n" + result.Error;
		else if (!FB.IsLoggedIn) {
			mLastFBResponse = "Login cancelled by Player";
		}
		else {
			PublicaVictoriaEnFB();
		}

		if(mLastFBResponse != "") 
			Debug.Log( String.Format("Respuesta del login: {0}", mLastFBResponse) );
	}
	
	void PublicaVictoriaEnFB()
	{
		FB.Feed(
				"", //ToID
				"", //Link
		        string.Format(FacebookUtils.PUBLISH_TITLE_FOR_WINNING_CHALLENGE, mCompetitionName), // Link Name
				" ", // Link Caption
		        string.Format(FacebookUtils.PUBLISH_DESCRIPTION_FOR_WINNING_CHALLENGE, mCompetitionName), // linkDescription
		        FacebookUtils.PUBLISH_PICTURE_FOR_WINNING_CHALLENGE, // picture
		        "", // media source
				"", //action name
				"", // action link
				"", //reference
				null, //properties
		        LogCallback // callback
		        );
	}
	
	void LogCallback(FBResult response) {
		Debug.Log(response.Text);
	}


	private Camera m3DCam;

	private GameObject mTrophy;
	private GameObject mTheConfetti;
	private AudioInGameController mAudioGameController;
	private MainModel mMainModel;

	private string mCompetitionName;
	private string mLastFBResponse = "";

}
