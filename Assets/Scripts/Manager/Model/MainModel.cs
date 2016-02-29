using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FootballStar.Audio;
using FootballStar.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Bson;

namespace FootballStar.Manager.Model
{
	public class MondelChangedEventArgs : EventArgs
	{
		public int Money { get; set; }
		public int Fans { get; set; }
		public float EnergyPercent { get; set; }
		public Tier CurrentTier	 { get; set; }
	}

	public class MoneyChangedEventArgs : EventArgs
	{
		public int Amount { get; set; }
	}

	public class FansChangedEventArgs : EventArgs
	{
		public int Amount { get; set; }
	}
	
	public class LastMatchResultEventArgs : EventArgs
	{
		public int DiffMoney { get; set; }
		public int DiffFans { get; set; }
	}
	
	public class EnergyChangedEventArgs : EventArgs
	{
		public float RemainEnergy { get; set; }
		public float EnergyCost { get; set; }
	}

	public class MainModel : MonoBehaviour
	{
		[HideInInspector]public bool  FacebookInitialized = false;
		[HideInInspector]public bool  FBPublishInstall = false;

		public float DifficultyRange = 0.10f / 4f;
		
		public bool  FinalVersion = false;
		public bool  AllMatchesUnlocked = false;
		public bool  LoadSaveModeJSON = true;
		public bool  SkipTutorial = false;

		public event EventHandler<MondelChangedEventArgs> OnModelChanged;

		public event EventHandler<MoneyChangedEventArgs> OnMoneyChanged;
		public event EventHandler<FansChangedEventArgs> OnFansChanged;

		public event EventHandler<LastMatchResultEventArgs> OnLastMatchResult;
		
		public event EventHandler<EnergyChangedEventArgs> OnEnergyChanged;


		public int SelectedTeamId {
			get {return mPlayer.SelectedTeamId;}
			set {
				mPlayer.SelectedTeamId = value;
				GameObject.Find("Header").GetComponent<Header>().SetupHudForSelectedTeam();
				SaveDefaultGame();
			}
		}

		
		public float PlayerRemainEnergy
		{
			get { return mPlayer.CurrentEnergy; }
		}

		public Genetsis Genetsis {
			get { return mGenetsis; }
		}
		
		//public Player PlayerPrefab;

		void Awake()
		{
			mMatchLoader = GetComponent<MatchLoader>();
			mMatchBridge = GetComponent<MatchBridge>();
			mGenetsis	 = GetComponent<Genetsis>();
			
			// En version final da igual como nos hayan dejado configurados, ponemos los valores sensibles
			if (FinalVersion)
			{
				AllMatchesUnlocked = false;
				LoadSaveModeJSON = false;
			}
			
			// Opcionalmente, si no quisieramos usar reserveReferencesHandling.All, en las propiedades hariamos [JsonProperty(ItemIsReference = true)]
			mSerializationSettings = new JsonSerializerSettings { ContractResolver = new DynamicContractResolver(),
																  ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
																  PreserveReferencesHandling = PreserveReferencesHandling.All };
			if (HasDefaultGame())
			{
				try {
					LoadDefaultGame ();
				}
				catch (Exception e) {
					Debug.LogError("Ha ocurrido un error durante LoadDefaultGame " + e.ToString());
					ResetDefaultGame();		// Error en el formato, nos recuperamos reseteando
				}
			}
			else
			{
				mPlayer = new Player();
				mPlayer.Init();
			}

			if (!FinalVersion && SkipTutorial)
				mPlayer.TutorialStage = TutorialStage.DONE;

			if (!FBPublishInstall) {
				FBInit ();
			}
		}
		
		void Start()
		{
			mAudioController = GetComponent<AudioInGameController>();
			
			// Cargamos las texturas de fondo de las categorias que las necesiten.
			ImprovementsDefinition.LoadBackgroundsForCategory(ImprovementsDefinition.PropertiesCategory);
			ImprovementsDefinition.LoadBackgroundsForCategory(ImprovementsDefinition.GymCategory);
			ImprovementsDefinition.LoadBackgroundsForCategory(ImprovementsDefinition.BlackboardCategory);
			ImprovementsDefinition.LoadBackgroundsForCategory(ImprovementsDefinition.TechniqueCategory);
			ImprovementsDefinition.LoadBackgroundsForCategory(ImprovementsDefinition.EventsCategory);
			ImprovementsDefinition.LoadBackgroundsForCategory(ImprovementsDefinition.LockerRoomCategory);
			ImprovementsDefinition.LoadBackgroundsForCategory(ImprovementsDefinition.VehiclesCategory);

			mMatchLoader.LoadStadium(mPlayer.CurrentTierIndex);
			
			if (OnModelChanged != null)
				OnModelChanged(this, new MondelChangedEventArgs{
					Money = mPlayer.Money,
					Fans = mPlayer.Fans,
					EnergyPercent = mPlayer.EnergyPercent,
					CurrentTier = CurrentTier
				});
			
			// Coroutina para el update de la energia mientras el juego esta activo
			StartCoroutine (EnergyUpdateCoroutine());
			mSessionStart = DateTime.Now;
		}
		
		// Para provocar un OnModelChanged activando/desactivando el componentente desde el editor (debug)
		void OnEnable()
		{
			if (OnModelChanged != null)
				OnModelChanged(this, new MondelChangedEventArgs{
					Money = mPlayer.Money,
					Fans = mPlayer.Fans,
					EnergyPercent = mPlayer.EnergyPercent,
					CurrentTier = CurrentTier
				});
		}
		
		void OnLevelWasLoaded(int levelIdx)
		{
			// A la vuelta del Match3D...
			if (levelIdx == 0 && mMatchBridge.ReturnMatchResult != null)
			{
				if (OnModelChanged != null)
					OnModelChanged(this, new MondelChangedEventArgs{
						Money =  (OnLastMatchResult == null) ? mPlayer.Money :  mPlayer.Money - mMatchBridge.TotalMoneyEarned,
						Fans =   (OnLastMatchResult == null) ? mPlayer.Fans  :  mPlayer.Fans  - mMatchBridge.TotalFansEarned,
						EnergyPercent = mPlayer.EnergyPercent,
						CurrentTier = CurrentTier
					});
				
				if (OnLastMatchResult != null)
				{
					OnLastMatchResult(this, new LastMatchResultEventArgs { DiffMoney = mMatchBridge.TotalMoneyEarned, DiffFans = mMatchBridge.TotalFansEarned }  );
					mCanUpdatePlayerEnergy = true;
				}
			}
			
			// Reduce la presion despues de una carga, parece que ayuda despues de la carga inicial
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}
		
		public bool CanIPlayMatches()
		{
			return mPlayer.CurrentEnergy >= -mPlayer.EnergyCostPerMatch;				
		}
		
		public void PlayMatch(Match match)
		{
			StartCoroutine( ConsumeEnergyAndPlayMatchCoroutine(match) );
		}

		public void PlayTutorial()
		{
			PlaySpecialMatch(new MatchSpecial((int)TierDefinition.Teams.Dortmund, "TutorialMatch", 1200, 0.5f));
		}
		
		public void PlayPerformanceMatch()
		{			
			PlaySpecialMatch(new MatchSpecial((int)TierDefinition.Teams.Dortmund, "MatchPerformance", 0, 0.0f));
		}
		
		private void PlaySpecialMatch(MatchSpecial theMatch)
		{
			mLastMatchPlayed = theMatch;
			mMatchBridge.CurrentMatchDefinition = mLastMatchPlayed.Definition;
			mMatchBridge.DifficultyPower = MatchBridge.Difficulty.EASY;
			mMatchBridge.DifficultyVision = MatchBridge.Difficulty.EASY;
			mMatchBridge.DifficultyTechnique = MatchBridge.Difficulty.EASY;
			mMatchBridge.SponsorshipBonus = 0;
			
			mMatchLoader.LoadMatch();
		}
		
		void OnMatchEnd()
		{
			AddMoney(mMatchBridge.TotalMoneyEarned);
			AddFans(mMatchBridge.TotalFansEarned);
			
			// Nos quedamos con el nuevo resultado solo si mejoramos
			if (mMatchBridge.IsResultImproved)
				mLastMatchPlayed.MatchResult = mMatchBridge.ReturnMatchResult;

			mLastMatchPlayed.NumTimesPlayed++;
		}
		
		void OnMatchRepeat()
		{
			SaveDefaultGame();

			if (mMatchBridge.IsResultImproved)
				mMatchBridge.CurrentMatchPrevResult = mMatchBridge.ReturnMatchResult;

			mMatchBridge.ReturnMatchResult = null;
		}

		public void GoBackToManager()
		{
			MixPanel.SendEventToMixPanel(AnalyticEvent.MATCH_END, new Dictionary<string, object>(){
				{ "Victoria", mMatchBridge.ReturnMatchResult.PlayerWon},
				{ "VisionDifficulty", mMatchBridge.DifficultyVision}, 
				{ "PowerDifficulty", mMatchBridge.DifficultyPower}, 
				{ "TechniqueDifficulty", mMatchBridge.DifficultyTechnique},
				{ "MatchType", mMatchBridge.CurrentMatchDefinition.PlaySequence},
				{ "Difficulty", mMatchBridge.CurrentMatchDefinition.Difficulty},
				{ "BadgeName", mMatchBridge.CurrentMatchDefinition.OpponentBadgeName + "_Small"}
			});
			NextStageTutorial();
			
			// Este parece un buen momento para refrescar la posicion del player dentro de la liga
			CurrentTier.League.RefreshPlayerPosInRanking(CurrentTier.LeagueMatches.Count, CurrentTier.MatchBrowser.LastLeagueIdx);
			
			SaveDefaultGame();
			
			mMatchLoader.LoadManager(mPlayer.CurrentTierIndex);
		}
		
		public void NextStageTutorial()
		{
			if (mPlayer.TutorialStage != TutorialStage.DONE)
				mPlayer.TutorialStage++;
		}
		
		// Nos permite quedarnos con miembros privados y solo los escribibles (ignoramos propiedades que solo tengan get & fields que sean readonly)
		private class DynamicContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
		{
			public DynamicContractResolver()
			{
				DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;	
			}
			
		    protected override IList<JsonProperty> CreateProperties(Type type, Newtonsoft.Json.MemberSerialization memberSerialization)
		    {
		        IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);				
				return properties.Where(prop => prop.Writable).ToList();
		    }
		}
		
		public void SaveDefaultGame()
		{
			Profiler.BeginSample("SaveDefaultGame");			
			string playerData;
			
			if (LoadSaveModeJSON) {
				playerData = JsonConvert.SerializeObject(mPlayer, Formatting.Indented, mSerializationSettings);
			}
			else {
				MemoryStream ms = new MemoryStream();
				
				using (BsonWriter writer = new BsonWriter(ms))
					JsonSerializer.Create(mSerializationSettings).Serialize(writer, mPlayer);
							
				playerData = System.Convert.ToBase64String(ms.ToArray());
			}
			
			PlayerPrefs.SetString(LoadSaveStorageKey, playerData);			
			Profiler.EndSample();
		}
	
		public void LoadDefaultGame()
		{	
			string playerPrefsPlayerData = PlayerPrefs.GetString(LoadSaveStorageKey);

			if (LoadSaveModeJSON)
			{
				mPlayer = JsonConvert.DeserializeObject<Player>(playerPrefsPlayerData, mSerializationSettings);
			}
			else
			{
				byte[] playerData = System.Convert.FromBase64String(playerPrefsPlayerData);
				
				using (BsonReader reader = new BsonReader(new MemoryStream(playerData)))
					mPlayer = JsonSerializer.Create(mSerializationSettings).Deserialize<Player>(reader);
			}

			if (mPlayer.CurrentSaveVersion == Player.SAVE_VERSION) {
				mPlayer.InitAfterDeserialization();
				
				// Tras cargar el juego, permitimos que se checkee la recarga de energia.
				mCanUpdatePlayerEnergy = true;	
				
				if (OnModelChanged != null)
					OnModelChanged(this, new MondelChangedEventArgs{
						Money = mPlayer.Money,
						Fans = mPlayer.Fans,
						EnergyPercent = mPlayer.EnergyPercent,
						CurrentTier = CurrentTier
					});
			}
			else {
				// De momento mientras testeamos, queremos que si la version es distinta, reseteamos la partida
				ResetDefaultGame();
			}
		}
		
		private string LoadSaveStorageKey
		{
			get { return LoadSaveModeJSON? "PlayerDataJSONV1" : "PlayerDataBinV1"; }
		}
		
		public bool HasDefaultGame()
		{
			return PlayerPrefs.HasKey(LoadSaveStorageKey);
		}
		
		public void ResetDefaultGame()
		{			
			mPlayer = new Player();
			mPlayer.Init();
			
			SaveDefaultGame();
			
			if (OnModelChanged != null)
				OnModelChanged(this, new MondelChangedEventArgs{
					Money = mPlayer.Money,
					Fans = mPlayer.Fans,
					EnergyPercent = mPlayer.EnergyPercent,
					CurrentTier = CurrentTier
				});

			GameObject.Find("Header").GetComponent<Header>().GoToMainScreen ();
		}

		public void AddMoney(int amount)
		{
			mPlayer.Money += amount;
			if (OnMoneyChanged != null)
				OnMoneyChanged(this, new MoneyChangedEventArgs(){ Amount = amount}  );
		}

		
		public void AddFans(int amount)
		{
			mPlayer.Fans += amount;
			if (OnFansChanged != null)
				OnFansChanged(this, new FansChangedEventArgs(){Amount = amount});	
		}
		
		public void AddEnergyUnit(int amount)
		{
			mPlayer.AddEnergy(amount);
			if (OnModelChanged != null)
				OnModelChanged(this, new MondelChangedEventArgs{
					Money = mPlayer.Money,
					Fans = mPlayer.Fans,
					EnergyPercent = mPlayer.EnergyPercent,
					CurrentTier = CurrentTier
				});
				OnEnergyChanged(this, new EnergyChangedEventArgs(){ RemainEnergy = mPlayer.CurrentEnergy, EnergyCost = amount * 0.1f });			
		}
				
		public void BuyImprovement(ImprovementItem theItem)
		{
			var moneyBefore = Player.Money;
			var powerBefore = Player.Power;
			var visionBefore = Player.Vision;
			var techniqueBefore = Player.Technique;
			var motivationBefore = Player.Motivation;

			bool boughtSuccess = Player.BuyImprovement(theItem);

			if (boughtSuccess)
			{
				MixPanel.SendEventToMixPanel(AnalyticEvent.BUY_IMPROVEMENT, 
				                             new Dictionary<string, object>{ {"ImprovementName", theItem.Name},

																			 {"MoneyBefore", moneyBefore},
																			 {"MoneyAfter", Player.Money},
																			 
																			 {"PowerBefore", powerBefore},
																			 {"VisionBefore", visionBefore},
																			 {"TechniqueBefore", techniqueBefore},
																			 {"MotivationBefore", motivationBefore},

																			 {"PowerAfter", Player.Power},
																			 {"VisionAfter", Player.Vision},
																			 {"TechniqueAfter", Player.Technique},
																			 {"MotivationAfter", Player.Motivation},
																		   });
				mAudioController.PlayDefinition(SoundDefinitions.CASHREGISTER, false);

				SaveDefaultGame();

				if (OnModelChanged != null)
					OnModelChanged(this, new MondelChangedEventArgs{
						Money = mPlayer.Money + theItem.Price,
						Fans = mPlayer.Fans,
						EnergyPercent = mPlayer.EnergyPercent,
						CurrentTier = CurrentTier
					});

				if (OnMoneyChanged != null)
					OnMoneyChanged(this, new MoneyChangedEventArgs(){ Amount = -theItem.Price}  );
			}
			else
				mAudioController.PlayDefinition(SoundDefinitions.LOCKED_ITEM, false);
		}
		
		public void BuySponsor(SponsorDefinition sponsor)
		{
			// En funcion de si la compra tiene Exito
			bool boughtSuccess = mPlayer.BuySponsor(sponsor);
			if (boughtSuccess)
				mAudioController.PlayDefinition(SoundDefinitions.CASHREGISTER, false);
			else
				mAudioController.PlayDefinition(SoundDefinitions.LOCKED_ITEM, false);

			SaveDefaultGame();
			if (OnModelChanged != null)
				OnModelChanged(this, new MondelChangedEventArgs{
					Money = mPlayer.Money,
					Fans = mPlayer.Fans,
					EnergyPercent = mPlayer.EnergyPercent,
					CurrentTier = CurrentTier
				});
		}
		
		public Player 	    Player 		 { get { return mPlayer; } }
		public Tier         CurrentTier  { get { return mPlayer.CurrentTier; } }
		public MatchBrowser MatchBrowser { get { return mPlayer.CurrentTier.MatchBrowser; } }

		static public string FormatVal(float val)
		{
			return string.Format("{0:0}", val * 1000);
		}
		
		static public string FormatDiff(float val)
		{
			if (val == 0f)
				return "";
			
			return string.Format ("{0}{1:0}", val > 0? "+" : "", val * 1000);
		}
		
		// Comparacion del Skill del player con la dificultad del partido
		public MatchBridge.Difficulty GlobalDifficultyIndex(MatchDefinition matchDef)
		{
			return ParameterDifficultyIndex(mPlayer.MatchSkill - matchDef.Difficulty);
		}
		
		// Para 1 parametro en concreto
		private MatchBridge.Difficulty ParameterDifficultyIndex(float diff)
		{			
			// 0 bajo, 1 medio, 2 alto, 3 extremo
			if (diff <= -DifficultyRange)
				return MatchBridge.Difficulty.EXTREME;
			else if (diff <= 0)
				return MatchBridge.Difficulty.HIGH;
			else if (diff < DifficultyRange)
				return MatchBridge.Difficulty.MEDIUM;
			
			return MatchBridge.Difficulty.EASY;
		}
		
		public string GlobalDifficultyName(MatchDefinition matchDef)
		{
			return DIFF_NAMES[(int)GlobalDifficultyIndex(matchDef)];
		}
		
		static private string[] DIFF_NAMES = new string[] { "BAJA", "MEDIA", "ALTA", "EXTREMA" };

		// Coroutina que se llama cada vez que vamos a jugar un partido
		IEnumerator ConsumeEnergyAndPlayMatchCoroutine(Match match)
		{
			var uiCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<UICamera>();
			// Si estamos en el tutorial no consumimos energia.
			if( mPlayer.TutorialStage != TutorialStage.FRIENDLY_EXPLANATION && mPlayer.TutorialStage != TutorialStage.CUP_EXPLANATION )
			{
				mPlayer.AddEnergy( mPlayer.EnergyCostPerMatch );
				OnEnergyChanged(this, new EnergyChangedEventArgs(){ RemainEnergy = mPlayer.CurrentEnergy, EnergyCost = mPlayer.EnergyCostPerMatch * 0.1f });	
				OnModelChanged(this, new MondelChangedEventArgs{
					Money = mPlayer.Money,
					Fans = mPlayer.Fans,
					EnergyPercent = mPlayer.EnergyPercent,
					CurrentTier = CurrentTier
				});

			}

			if ( Genetsis != null )
				Genetsis.AccionJugar();

			uiCamera.enabled = false;
			yield return new WaitForSeconds(1f); // Tiempo maximo para que se vea la animacion de la energia antes de empezar el partido.
			uiCamera.enabled = true;
			mCanUpdatePlayerEnergy = false;
			
			mLastMatchPlayed = match;
			mMatchBridge.CurrentMatchDefinition = match.Definition;
			mMatchBridge.CurrentMatchDefinition.MyID = SelectedTeamId;
			mMatchBridge.CurrentMatchPrevResult = match.MatchResult;
			mMatchBridge.MatchRelevance = match.MatchRelevance;
						
			mMatchBridge.SponsorshipBonus = mPlayer.AddSponsorshipBonuses();
			
	        // Expresar el parámetro de la dificultad como 4 características (3+1=Skill) - Difficulty
			mMatchBridge.DifficultyPower     = ParameterDifficultyIndex(mPlayer.Power*3f + mPlayer.Motivation - match.Definition.Difficulty);
			mMatchBridge.DifficultyVision    = ParameterDifficultyIndex(mPlayer.Vision*3f + mPlayer.Motivation - match.Definition.Difficulty);
			mMatchBridge.DifficultyTechnique = ParameterDifficultyIndex(mPlayer.Technique*3f + mPlayer.Motivation - match.Definition.Difficulty);

			mMatchBridge.DifficultyGlobal = GlobalDifficultyIndex(match.Definition);

			mMatchLoader.LoadMatch();
		}

		// Coroutina que se llama cada segundo
		IEnumerator EnergyUpdateCoroutine()
		{
			while (true)
			{
				if (mCanUpdatePlayerEnergy && mPlayer.CheckEnergyRecharge() > 0)
				{
					if (OnModelChanged != null)
						OnModelChanged(this, new MondelChangedEventArgs{
							Money = mPlayer.Money,
							Fans = mPlayer.Fans,
							EnergyPercent = mPlayer.EnergyPercent,
							CurrentTier = CurrentTier
						});	
				}
				yield return new WaitForSeconds(1.0f);
			}
		}
	
		void OnApplicationPause(bool suspended)
		{
			if (suspended) 
			{
				TimeSpan Datediff = DateTime.Now.Subtract(mSessionStart);

				MixPanel.SendEventToMixPanel (AnalyticEvent.SESSION_TIME, new Dictionary<string, object>(){
					{ "Session Time", String.Format("{00:hh\\:mm\\:ss}", Datediff) }
				});
			}
			else 
			{
				//Reseteamos la sesion
				mSessionStart = DateTime.Now;
			}
		}

		private void FBInit()
		{
			FB.Init (OnFBInitComplete, OnHideUnity);
		}
		
		private void OnFBInitComplete()
		{
			FacebookInitialized = true;
			CallFBPublishInstall ();
		}
		
		private void OnHideUnity(bool isGameShown)
		{
			Debug.Log("Is game showing? " + isGameShown);
		}

		private void CallFBPublishInstall()
		{
			FB.PublishInstall(FBPublishComplete);
		}
		
		private void FBPublishComplete(FBResult result)
		{
			FBPublishInstall = true;
			Debug.Log("publish response: " + result.Text);
		}
				
		Player mPlayer;
		
		bool mCanUpdatePlayerEnergy = true;
		
		MatchLoader mMatchLoader;
		MatchBridge mMatchBridge;		
		Match mLastMatchPlayed;
		Genetsis mGenetsis;
		
		AudioInGameController mAudioController;
		
		JsonSerializerSettings mSerializationSettings;

		private DateTime mSessionStart;

		Header mHeader;
	}
}