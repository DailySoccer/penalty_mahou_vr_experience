using System.Collections;
using System;
using UnityEngine;
using FootballStar.Manager.Model;
using ExtensionMethods;
using FootballStar.Match3D;
using FootballStar.Audio;

namespace FootballStar.Manager
{
	public class Header : ScreenStack
	{
		public GameObject  ButtonBack;
		public GameObject  HomePanel;
		public GameObject  YouNeedEnergyScreen;
		public UILabel     SkillLabel;
		public UILabel     TierLabel;
		
		public UILabel     EnergyChangeLabel;
		public UILabel     EnergyTimerLabel;
		public ProgressBar EnergyProgressBar;		

		public UILabel     FansLabel;
		public GameObject  FansChangeContainer;		
				
		public UILabel     MoneyLabel;
		public GameObject  MoneyChangeContainer;

		public UISprite PlayerTeamBadge;
		public UILabel PlayerTeamName;
		
		
		protected override void Awake()
		{
			base.Awake();
			
			mMainModel.OnModelChanged    += HandleOnModelChanged;
			mMainModel.OnMoneyChanged    += HandleOnMoneyChanged;
			mMainModel.OnFansChanged     += HandleOnFansChanged;
			mMainModel.OnLastMatchResult += HandleOnLastMatchResult;
			mMainModel.OnEnergyChanged   += HandleOnEnergyChange;
			
			mPlayerHigh = GameObject.Find("Player_High@MainMenu");
			mCancha = GameObject.FindGameObjectWithTag("Cancha");

			mParticleManager = GameObject.Find("SmallTheater").GetComponent<ParticleManager>();
			mAudioControl = mMainModel.GetComponent<AudioInGameController>();
		}

		void OnDestroy()
		{
			mMainModel.OnModelChanged    -= HandleOnModelChanged;
			mMainModel.OnMoneyChanged    -= HandleOnMoneyChanged;
			mMainModel.OnFansChanged     -= HandleOnFansChanged;
			mMainModel.OnLastMatchResult -= HandleOnLastMatchResult;
			mMainModel.OnEnergyChanged   -= HandleOnEnergyChange;
		}

		void HandleOnModelChanged(object sender, MondelChangedEventArgs e)
		{
			MoneyLabel.text 			= e.Money.ToString();
			FansLabel.text 				= e.Fans.FormatAsMoney();
			EnergyProgressBar.Percent 	= e.EnergyPercent;
			SkillLabel.text				= MainModel.FormatVal(mMainModel.Player.Skill);
			TierLabel.text 				= e.CurrentTier.Definition.Name.ToUpper();
			SetupHudForSelectedTeam ();
		}
		
		void HandleOnMoneyChanged(object sender, MoneyChangedEventArgs e)
		{
			if(e.Amount != 0)
				ShowMoneyChangeEffect(e.Amount);		
		}

		void HandleOnFansChanged(object sender, FansChangedEventArgs e)
		{
			if(e.Amount != 0)
				ShowFansChangeEffect(e.Amount);		
		}


		void HandleOnLastMatchResult(object sender, LastMatchResultEventArgs e)
		{
			// Veamos si tenemos que adquirir el sponsor. Preferimos que la logica de adquisicion se haga desde la vista por
			// si en el futuro tiene sentido que el player decida si quiere adquirir o no
			
			// Buscar cual es el primero sin comprar
			var firstSponsorNotPurchased = mMainModel.Player.CurrentTier.Sponsors.GetFirstPurchaseableSponsorID();
			
			if (firstSponsorNotPurchased != -1) {
				// Este mensaje debera llegar siempre a la vuelta de un partido cuando estamos en el menu principal, asi que podemos hacer este
				// push sin mas logica de ver en realidad en que pantalla estamos o hacer un goto
				PushScreenController("SponsorsScreen");
			}

			StartCoroutine( ShowIncrementsEarnedCoroutine(e) );
		}		
		
		void HandleOnEnergyChange(object sender, EnergyChangedEventArgs e)
		{
			if(e.RemainEnergy > 0)
				UpdateEnergyTimer = false;

			if(e.EnergyCost != 0)
				ShowEnergyChangeEffect( (int)(e.EnergyCost / 0.1f) );
			EnergyProgressBar.MiddlePercent = EnergyProgressBar.Percent - e.EnergyCost;
		}	

		IEnumerator ShowIncrementsEarnedCoroutine(LastMatchResultEventArgs e)
		{			
			if (e.DiffMoney != 0) {
				yield return new WaitForSeconds (3f);
				ShowMoneyChangeEffect (e.DiffMoney);
			}

			if (e.DiffFans != 0) {						
				yield return new WaitForSeconds ( MoneyChangeContainer.GetComponent<Animation>().clip.length + 1.5f );
				ShowFansChangeEffect(e.DiffFans);			
			}

			yield return new WaitForSeconds (FansChangeContainer.GetComponent<Animation>().clip.length + 1.5f );
			if(mMainModel.PlayerRemainEnergy <= 0)
				PutOutOfEnergyScreen();
		}

		public void OnBackButtonClicked()
		{
			PopScreenController();
		}
		
		void OnScreenStackChanged()
		{
			// Se carga en streaming... puede ser null
			if (mStadium == null)
				mStadium = GameObject.FindGameObjectWithTag("Stadium");
			
			if (StackDepth > 1)
			{
				ButtonBack.SetActive(true);
				HomePanel.SetActive(false);
				
				mPlayerHigh.SetActive(false);
				mCancha.SetActive(false);
				
				if (mStadium != null)	
					mStadium.SetActive(false);
			}
			else
			{
				ButtonBack.SetActive(false);
				HomePanel.SetActive(true);
				
				mPlayerHigh.SetActive(true);
				mCancha.SetActive(true);
				
				if (mStadium != null)
					mStadium.SetActive(true);
			}
		}

		private void ShowMoneyChangeEffect(int amount)
		{
			string animToPlay = "MoneyChangeGoingUpAnim";
			UILabel moneyChangeLabel = MoneyChangeContainer.GetComponentInChildren<UILabel>();
			if(amount < 0)
			{
				moneyChangeLabel.color = Color.red;
				moneyChangeLabel.text = amount.ToString();	
				animToPlay = "MoneyChangeGoingUpAnim";
				mParticleManager.ShowRemoveMoneyParticles();
				StartCoroutine (ScoreCounter(amount, mMainModel.Player.Money, 0f, MoneyLabel));
			}
			else if (amount > 0)
			{
				moneyChangeLabel.color = new Color(0.96f, 1.0f, 0.0f, 1.0f);
				moneyChangeLabel.effectColor = new Color( 0.27f, 0.67f, 0.0f, 1.0f);
				moneyChangeLabel.text = "+" + amount.ToString();
				animToPlay = "MoneyChangeZoomOutAnim";
				mParticleManager.ShowAddMoneyParticles();
				StartCoroutine (ScoreCounter(amount, mMainModel.Player.Money, MoneyChangeContainer.GetComponent<Animation>().clip.length, MoneyLabel));
			}

			MoneyChangeContainer.GetComponent<Animation>().Play(animToPlay);
			mAudioControl.CustomPlay(SoundDefinitions.SWOOSH, -1.0f, 0.7f);

			mParticleManager.SetMoneyParticlesPosition( CalculateScreenPosition(MoneyLabel.transform.position, 6.0f) );
		}

		private void ShowFansChangeEffect(int amount)  
		{
			UILabel fansChangeLabel = FansChangeContainer.GetComponentInChildren<UILabel>();
			if(amount != 0)
			{
				fansChangeLabel.gameObject.SetActive(true);
				fansChangeLabel.text  = (amount > 0 ? "+" : "") + amount.ToString();
				FansChangeContainer.GetComponent<Animation>().Play();
				mParticleManager.ShowFansParticle();
			}
			StartCoroutine( ScoreCounter(amount, mMainModel.Player.Fans, FansChangeContainer.GetComponent<Animation>().clip.length, FansLabel) );
			mAudioControl.CustomPlay(SoundDefinitions.SWOOSH, -1, 1.0f);
			mParticleManager.SetFansParticlesPosition( CalculateScreenPosition(FansLabel.transform.position, 6.0f) );
		}

		IEnumerator ScoreCounter(int amountToAdd, int AmountAtEnd, float delay, UILabel nGuiLabel)
		{
			yield return new WaitForSeconds (delay);
			//to
			int finalAmount = AmountAtEnd;
			//from
			float currentFans = finalAmount - amountToAdd;
			//steps
			int steps = (100 > amountToAdd) ? amountToAdd : 100;
			
			float incremento = amountToAdd / steps;
			int contador = 0;
			
			while (contador < steps ) {
				nGuiLabel.text = string.Format("{0:0}", currentFans);
				currentFans += incremento;
				contador++;
				yield return new WaitForSeconds(0);
				if( contador % 2 == 0 )
					if(nGuiLabel.name == FansLabel.name) // Fans suena con pitch Agudo y Money con pitch Grave.
						mAudioControl.CustomPlay(SoundDefinitions.AWARD_DING, -1, 1.0f);
					else
						mAudioControl.CustomPlay(SoundDefinitions.AWARD_DING, -1, 0.6f);
			}
			nGuiLabel.text = finalAmount.ToString();
		}

		private Vector3 CalculateScreenPosition(Vector3 itemWolrdPosition,  float depth)
		{
			Camera uiCam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
			Vector3 positionTo = uiCam.WorldToScreenPoint(itemWolrdPosition);
			return new Vector3( positionTo.x, positionTo.y, depth);	
		}

		private void ShowEnergyChangeEffect(int amount)  
		{
			if(amount != 0)
			{
				EnergyChangeLabel.gameObject.SetActive(true);
				EnergyChangeLabel.color = Color.green;
				EnergyChangeLabel.text  = (amount > 0 ?  ((amount > 1) ? "RECARGA COMPLETA" :"+"  + amount.ToString()) : ""  + amount.ToString());
				EnergyChangeLabel.GetComponent<Animation>().Play();
				mParticleManager.ShowEnergyParticle();
			}
		}
		
		void Update()
		{
			//Actualizacion del timer que aparece sobre la EnergyProgressBar
			UpdateEnergyTimer = (EnergyProgressBar.Percent <= 0);
			
			EnergyTimerLabel.enabled = UpdateEnergyTimer;	
			EnergyProgressBar.gameObject.SetActive(!UpdateEnergyTimer);
			
			if (UpdateEnergyTimer)
			{
				TimeSpan clockValue = (mMainModel.Player.LastEnergyUse + new TimeSpan(0,6,0)) - DateTime.Now;
				EnergyTimerLabel.text = string.Format("{0:00}:{1:00} min", clockValue.Minutes, clockValue.Seconds);
			}			

			if((EnergyProgressBar.Percent - EnergyProgressBar.MiddlePercent) < 0 )
				EnergyProgressBar.MiddlePercent += (EnergyProgressBar.Percent - EnergyProgressBar.MiddlePercent) * Time.deltaTime;
			else
				EnergyProgressBar.MiddlePercent = EnergyProgressBar.Percent;


			if (Input.GetKeyUp (KeyCode.Escape)) {
					// Si hay una ventana abierta, hay que cerrarla primero con su boton "Aceptar"
					MessageOverlapDialog[] msgs = GameObject.FindObjectsOfType (typeof(MessageOverlapDialog)) as MessageOverlapDialog[];
					if (msgs.Length <= 0) 
							OnBackButtonClicked ();
			}
		}

		void PutOutOfEnergyScreen()
		{
			// Creamos nuestro mensajito explicativoSetupHudForSelectedTeam
			mMessageOverlap = NGUITools.AddChild(this.gameObject, YouNeedEnergyScreen);
			mMessageOverlap.GetComponentInChildren<UIButtonMessage>().target = this.gameObject;
			mMessageOverlap.GetComponentInChildren<YouNeedEnergy>().LastPlay = mMainModel.Player.LastEnergyUse;
			mMessageOverlap.GetComponentInChildren<YouNeedEnergy>().OnEnergyCountdownEnds += HandleOnEnergyCountdownEnds;
			mMessageOverlap.transform.localPosition = new Vector3(0, 0, -1000);
		}
		
		void HandleOnEnergyCountdownEnds(object sender, EventArgs e)
		{
			mMessageOverlap.GetComponentInChildren<YouNeedEnergy>().OnEnergyCountdownEnds -= HandleOnEnergyCountdownEnds;
			OnContinueYouNeedEnergyClick();
		}
		
		void OnContinueYouNeedEnergyClick() {
			Destroy(mMessageOverlap);
			UpdateEnergyTimer = true;
		}


		public void SetupHudForSelectedTeam() {

			PlayerTeamBadge.spriteName = FootballStar.Manager.Model.TierDefinition.GetTeamName(mMainModel.SelectedTeamId) + "_Small";
			PlayerTeamName.text = FootballStar.Manager.Model.TierDefinition.TeamShortNames[mMainModel.SelectedTeamId];
			//cargamos la textura del jugador
			StartCoroutine( LoadPlayerTexture() );
		}

		IEnumerator LoadPlayerTexture()
		{
			GameObject go = GameObject.Find("Player_High@MainMenu");
			if (go != null)
			{                
				string root = Application.streamingAssetsPath;
				if (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
					root = Application.dataPath + "/StreamingAssets/";
				else if (Application.platform != RuntimePlatform.Android)
					root = "file:///" + root;
				
				WWW www = new WWW(System.IO.Path.Combine(root, "HI/Ropa_" + FootballStar.Common.MatchDefinition.BadgeName(mMainModel.Player.SelectedTeamId) + ".jpg"));
				yield return www;
				go.transform.Find("Camiseta").GetComponent<Renderer>().sharedMaterial.mainTexture = www.texture;
				// Asignar la textura de la ropa
				www = new WWW(System.IO.Path.Combine(root, "HI/Medias_" + FootballStar.Common.MatchDefinition.BadgeName(mMainModel.Player.SelectedTeamId) + ".jpg"));
				yield return www;
				go.transform.Find("Medias").GetComponent<Renderer>().sharedMaterial.mainTexture = www.texture;
				// Asignar la textura de las medias.
			}
		}
		
		private bool UpdateEnergyTimer;
		
		private GameObject mMessageOverlap;
		
		private GameObject mPlayerHigh;
		private GameObject mStadium;
		private GameObject mCancha;
		
		[NonSerialized]
		AudioInGameController mAudioControl;

		private ParticleManager mParticleManager;
	}
}

