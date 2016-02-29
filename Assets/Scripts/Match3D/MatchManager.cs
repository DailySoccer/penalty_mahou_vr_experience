// #define REGISTER_GAME_EVENTS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;
using System.IO;
using FootballStar.Common;
using FootballStar.Audio;
using System.Linq;
using System;
using ExtensionMethods;

namespace FootballStar.Match3D {
	public enum MomentoRespuesta {
		NUNCA,
		FALLO,
		ACIERTO
	};

	[System.Serializable]
	public class AutomaticTap {
		public MomentoRespuesta Cuando = MomentoRespuesta.ACIERTO;
		
		public bool Enabled { get { return (Cuando != MomentoRespuesta.NUNCA); } }
		public bool IsWaiting( MomentoRespuesta momento ) { return Enabled && (Cuando == momento); }
	};
	
	public class MatchManager : MonoBehaviour 
	{
        //static public bool LifeBar = true;

        public AutomaticTap AutomaticTap;
		public bool VersionFinal = false;
		public int TargetFrameRate = -1;		
		public GameObject GameModelPrefab;              // En caso de no venir desde el manager, en cualquier caso instanciamos un GameModel

		public GameObject Narrator;

        public GameObject CurrentPlay { get; set; }
//		public bool IsLastPlay { get { return true; } }

		public bool IsTutorialMatch { get; set; } 
		
		public PointOfInterest PointOfInterest { get; set; }
		public bool MatchFailed  { get { return mResultado == Resultado.FracasoMatch; } }
		
		public int CurrentPlayerGoals { get { return mMatchResult.PlayerGoals; } set { mMatchResult.PlayerGoals = value; }  }
		public int CurrentOpponentGoals { get { return mMatchResult.OppGoals; } set { mMatchResult.OppGoals = value; } }

		public MatchResult MatchResult { get { return mMatchResult; } }
		public InteractiveActions InteractiveActions { get { return mInteractiveActions; } }

		public int TotalInteractiveActions { get; set; }

		public float Life 	 { get; set; }
		public float MaxLife { get; set; }
		public float MinLife { get; set; }

		public event EventHandler<EventNewPlayArgs> OnNewPlay;
		public event EventHandler OnGol;
		public event EventHandler OnMatchFailed;
		
		public class EventNewPlayArgs : EventArgs {
			public GameObject NewPlay;
			public Resultado PrevPlayResult;	// Resultado.Esperando si no ha habido previous play
		}
		
		public event EventHandler OnCutsceneBegin;
		public event EventHandler OnCutsceneEnd;
		
		public event EventHandler OnMatchStart;		
		public event EventHandler<EventMatchEndArgs> OnMatchEnd;


//        public event EventHandler OnEndInteractiveAction;

        public class EventMatchEndArgs : EventArgs {
			public MatchResult MatchResult { get; set; }
			public EventMatchEndArgs(MatchResult m) { MatchResult = m; }
		}
		
		public PlayerMarkRenderer SoccerMark;
		public PlayerMarkRenderer MainCharacterMark;
		public bool HandAnimations = true;


		public RadialProgressBar QTEFirstPersonTimerBar;
		
		MatchBridge mMatchBridge;
		MatchResult mMatchResult;
		MatchAutoCam mMatchAutoCam;
		
		TriggerManager mTriggerManager;
		InteractiveActions mInteractiveActions;
		BallMotor mBalonMotor;
		
		AudioInGameController mAudioController;
        public Customization Customization { get { return mCustomization; } }

        Customization mCustomization;
				
		bool mShowCutscenes;
		bool mWaitingForTutorialStart = false;
		
		GameObject mAnimatorData;
		
		public enum Resultado {
			Ninguno,
			Esperando,
			Correcto,
			Incorrecto,
			FracasoMatch
		}
		Resultado mResultado = Resultado.Ninguno;
		
		// Score conseguido en la jugada en curso
		List<float> mPartialScore = new List<float>();

		void Awake () {
            Time.maximumDeltaTime = 1f / 10f;
            AnimatorID.Init ();
			PrepareGameModel();
            mMatchBridge.DifficultyVision = MatchBridge.Difficulty.MEDIUM;
            mAnimatorData = GameObject.Find ("AnimatorData");
			mMatchAutoCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MatchAutoCam>();
			mBalonMotor = GameObject.FindGameObjectWithTag ("Balon").GetComponent<BallMotor>();
			mInteractiveActions = GetComponent<InteractiveActions>();
			// Qué partido vamos a jugar? Siempre tiene que haber una MatchDefinition (PrepareGameModel se encarga de ello)
//			var matchSequenceGO = GameObject.Find( mMatchBridge.CurrentMatchDefinition.PlaySequence );
//			CurrentMatch = matchSequenceGO == null? DefaultMatch : matchSequenceGO.GetComponent<MatchSequence>();
//			Debug.Log ("Match: " + CurrentMatch.name);
			// Configuramos el QTE
			var qteStandard = GetComponent<QuickTimeEventStandard>();
			qteStandard.Difficulty = (int)mMatchBridge.DifficultyVision;
			/**** BEG: nueva version *****/
			var qteDirection = GetComponent<QuickTimeEventDirection>();
			qteDirection.Difficulty = (int)mMatchBridge.DifficultyPower;
			/*var qteDblTap = GetComponent<QuickTimeEventDblTap>();
			qteDblTap.Difficulty = (int)mMatchBridge.DifficultyTechnique;*/
			// Suscribimos a evento de Interaccion
//			InteractiveActions.OnEvent += OnInteractiveAction;

			// Otros componentes...
			mCustomization = GetComponent<Customization>();
			mAudioController = mMatchBridge.GetComponent<AudioInGameController>();
			mAudioController.SubscribeToMatchEvents();
			SetupMatch();
			QTEFirstPersonTimerBar.Hide();
		}
		
		void PrepareGameModel() {
			// Creamos el GameModel si no existe ya (caso de estar cargando las jugadas en solitario)
			if (GameObject.FindGameObjectWithTag("GameModel") == null) {
				var gameModel = Instantiate(GameModelPrefab) as GameObject;
				int myID = gameModel.GetComponent<FootballStar.Manager.Model.MainModel>().SelectedTeamId;
				// No queremos que interfiera...
				Destroy(gameModel.GetComponent<FootballStar.Manager.Model.MainModel>());
				mMatchBridge = gameModel.GetComponent<MatchBridge>();
                mMatchBridge.CurrentMatchDefinition = new MatchDefinition() {
                    PlaySequence = "DefaultMatch.name", // CACAs
                    MyID = myID != 0 ? myID:1, // Para que no use el cero.
                    OpponentID  = (int)Manager.Model.TierDefinition.Teams.Dortmund,
                };
				mShowCutscenes = false;
			}
			else {
				mMatchBridge = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MatchBridge>();
				mShowCutscenes = true;
			}
		}
				
		void SetupMatch() {
            IsTutorialMatch = false;// CurrentMatch.name.StartsWith("Tutorial");
			PrepareMatch();
		}
		
		void PrepareMatch () {
			mMatchResult = new MatchResult() {	ScorePerInteractionSequence = new List<float>() };
			DeactivatePartidos ();
			DeactivateJugadas ();
		}
		
		void Start () {
            StartMatch();
		}
		
		void StartMatch() {
            MixPanel.SendEventToMixPanel( AnalyticEvent.MATCH_START, new Dictionary<string,object>() {
				{ "MatchName", "CurrentMatch.name" }, // CACA
				{ "VisionDifficulty", mMatchBridge.DifficultyVision}, 
				{ "PowerDifficulty", mMatchBridge.DifficultyPower}, 
				{ "TechniqueDifficulty", mMatchBridge.DifficultyTechnique},
				{ "MatchType", mMatchBridge.CurrentMatchDefinition.PlaySequence},
				{ "Difficulty", mMatchBridge.CurrentMatchDefinition.Difficulty},
				{ "BadgeName", mMatchBridge.CurrentMatchDefinition.OpponentBadgeName + "_Small"}
			});
			OnMatchStart(this, EventArgs.Empty);
            StartCoroutine(Playing());
        }

		public void Repeat () {
            Debug.Log(">>> Repeat");
            MixPanel.SendEventToMixPanel(AnalyticEvent.MATCH_REPLAY, new Dictionary<string,object>() {
				{ "MatchName", "CurrentMatch.name" }, // CACA
				{ "VisionDifficulty", mMatchBridge.DifficultyVision}, 
				{ "PowerDifficulty", mMatchBridge.DifficultyPower}, 
				{ "TechniqueDifficulty", mMatchBridge.DifficultyTechnique},
				{ "MatchType", mMatchBridge.CurrentMatchDefinition.PlaySequence},
				{ "Difficulty", mMatchBridge.CurrentMatchDefinition.Difficulty},
				{ "BadgeName", mMatchBridge.CurrentMatchDefinition.OpponentBadgeName + "_Small"}
			});
			PrepareMatch();
			StartMatch();
		}

		void DeactivatePartidos() {
			GameObject[] partidos = GameObject.FindGameObjectsWithTag( "Partido" );
			foreach ( GameObject partido in partidos ) {
				partido.gameObject.SetActive( false );
			}
		}
		
		void DeactivateJugadas() {
            EraseJugada (  );			
			GameObject[] jugadas = GameObject.FindGameObjectsWithTag( "Jugada" );
			foreach ( GameObject jugada in jugadas ) {
				jugada.gameObject.SetActive( false );
			}
		}
		
		List<Entrenador> FindEntrenadores() {
			Profiler.BeginSample( "FindEntrenadores" );
			List<Entrenador> lEntrenadores = new List<Entrenador>(22);
			GameObject[] locales = GameObject.FindGameObjectsWithTag( "Local" );
			foreach( GameObject local in locales ) {
				lEntrenadores.Add ( local.GetComponent<Entrenador>() );
			}
			GameObject[] visitantes = GameObject.FindGameObjectsWithTag( "Visitante" );
			foreach( GameObject visitante in visitantes ) {
				lEntrenadores.Add ( visitante.GetComponent<Entrenador>() );
			}
			Profiler.EndSample();
			return lEntrenadores;
		}
		
		void CreateJugada( ) {
            CurrentPlay.SetActive( true );
			// Quitar la física del balón
			Rigidbody rigidBody = mBalonMotor.gameObject.GetComponent<Rigidbody>();
			if ( rigidBody )
				Destroy( rigidBody );

			mBalonMotor.enabled = true;
			mBalonMotor.gameObject.SetActive( true );

			var name = CurrentPlay.name;

            Profiler.BeginSample( "InstantiateSoccer (Todos)" );
			mCustomization.Setup ();
			Profiler.EndSample();

			PointOfInterest = new PointOfInterest();
			mTriggerManager = gameObject.AddComponent( typeof( TriggerManager ) ) as TriggerManager;
			mPartialScore.Clear ();
		}
		
		void EraseJugada( ) {
            if (CurrentPlay == null)
                return;
            mBalonMotor.Setup();
			if ( mTriggerManager )
				Destroy( mTriggerManager );
			// Deslinkar el balon de cualquier futbolista
			mBalonMotor.UnAttach();
			List<Entrenador> entrenadores = FindEntrenadores();
			foreach (Entrenador entrena in entrenadores) {
				entrena.DestroySoccer();
			}
            CurrentPlay.SetActive(false);
            Resources.UnloadUnusedAssets();
			GC.Collect();
		}

		// Elimina los componentes fisicos de los entrenadores.
		void SetupVersionFinal() {
			if ( !VersionFinal ) {
				Application.targetFrameRate = TargetFrameRate;
				return;
			}
			
			GameObject porteria = GameObject.FindGameObjectWithTag ("Porteria");
			if ( porteria ) {
				MeshRenderer renderer = porteria.GetComponent<MeshRenderer>();
				if ( renderer ) {
					renderer.enabled = false;
					Destroy ( renderer );
				}
			}
			
			List<Entrenador> entrenadores = FindEntrenadores();
			foreach ( Entrenador entrena in entrenadores ) {
				MeshFilter filter = entrena.GetComponent<MeshFilter>();
				if ( filter ) {
					Destroy ( filter );
				}
				
				Collider collider = entrena.GetComponent<Collider>();
				if ( collider ) {
					Destroy ( collider );
				}
				
				MeshRenderer renderer = entrena.GetComponent<MeshRenderer>();
				if ( renderer ) {
					renderer.enabled = false;
					Destroy ( renderer );
				}
			}
		}
		
		void SetupJugada( ) {
            CreateJugada ();
			SetupVersionFinal();
		}

		IEnumerator EsperandoResultado() {
			mResultado = Resultado.Esperando;			
			while ( mResultado == Resultado.Esperando ) {
				// Marcamos al protagonista de la jugada
				if ( MainCharacterMark != null )
					MainCharacterMark.Target = mInteractiveActions.MainCharacter;
	
				// Mostramos la marca del futbolista que tiene el balon si es distinto al Protagonista
				if ( SoccerMark != null )
					SoccerMark.Target = (mInteractiveActions.MainCharacter != mBalonMotor.NewPropietary) ? mBalonMotor.NewPropietary : null;
				
				PointOfInterest.Update();
				yield return null;
			}
			
			if ( MainCharacterMark != null )
				MainCharacterMark.Target = null;

			if ( SoccerMark != null )
				SoccerMark.Target = null;
		}

		// El tutorial nos llama aqui para indicarnos que podemos empezar a tocar la cutscene y empezar la primera jugada.
		public void OnTutorialStart() {
			mWaitingForTutorialStart = false;
		}

		IEnumerator IntroCutscene() {
			if (mShowCutscenes) {
				OnCutsceneBegin(this, EventArgs.Empty);
				StartCoroutine( IntroCutsceneSound() );
				StartCoroutine( CameraFade.FadeCoroutine(true, 0.5f, 0.1f) );
				yield return StartCoroutine( mMatchAutoCam.IntroCutscene(0.5f, IsTutorialMatch) );
				yield return StartCoroutine( CameraFade.FadeCoroutine(false, 0.5f, 0.0f) );
				OnCutsceneEnd(this, EventArgs.Empty);
			}
		}

		IEnumerator IntroCutsceneSound() {
			mAudioController.PlayDefinition(SoundDefinitions.MATCH_INTRO, false);
			yield return new WaitForSeconds(0.5f);
			mAudioController.PlayDefinition(SoundDefinitions.CITY_AMBIENT, false);
			yield return new WaitForSeconds(3.0f);
			mAudioController.PlayDefinition(SoundDefinitions.CROWD_GOINGUP, false);
		}

		IEnumerator Playing () {
			mAudioController.StopAllActiveAudios(true);
            //			yield return StartCoroutine(WaitForTutorialStart());
            //			yield return StartCoroutine(IntroCutscene());

            ManoloLama.Instance.Init();
            NarratorUI.Instance.Start();

            int cullingMask;
            cullingMask = Camera.main.cullingMask;

            while (true) {
                // Aqui se le meteria el bucle de jugadas. //////////////////////////////////////////////////////////
                // Borramos los datos de la jugada anterior
                EraseJugada();

                // Metemos al narrador hasta que llegue una accion interactiva o se temine el partido.
                Camera.main.cullingMask = (1 << 5) | (1 << 8);
                NarratorUI.Instance.gameObject.SetActive(true);
                yield return StartCoroutine(CameraFade.FadeCoroutine(true, 1.0f, 0.0f));
                yield return StartCoroutine(ManoloLama.Instance.Narrador());
                NarratorUI.Instance.gameObject.SetActive(false);
                if (ManoloLama.Instance.mInteractiveType != InteractiveType.End)
                {
                    yield return StartCoroutine(CameraFade.FadeCoroutine(false, 1.0f, 0.0f));
                    Camera.main.cullingMask = cullingMask;
                }
                else {
                    break;
                }

                // Esperamos hasta que se reseteen los componentes (p.ej. AnimatorController necesita tiempo para actualizar su estado)
                yield return StartCoroutine(Helper.WaitCondition(() => {
                    return (AnimationContext.IsResetFinished && mCustomization.IsTextureAssigned);
                }));
#if true
                CurrentPlay = Instantiate(Resources.Load("Acciones/Base")) as GameObject;
                CurrentPlay.SetActive(false);
                SetupJugada();

                // Aqui podemos crear las jugadas dinamicamente.
                // Solo necesito posicionar a los juagadores y añadirles las acciones.

                AnimatorTimeline.ParseJSONTake( TakeGenerator.Generate(ManoloLama.Instance.mInteractiveType) );
                // Voy a poner a los bichos en sus posiciones iniciales.
                for( int i = 1; i <= 11;++i) {
                    var org = GameObject.Find("Local" + i);
                    var dst = GameObject.Find("Soccer-Local" + i);
                    dst.transform.position = org.transform.position;
                    org = GameObject.Find("Visitante" + i);
                    dst = GameObject.Find("Soccer-Visitante" + i);
                    dst.transform.position = org.transform.position;
                }
#else

                CurrentPlay = Instantiate(Resources.Load("Acciones/Regate")) as GameObject;
                CurrentPlay.SetActive(false);
                SetupJugada();

                var takeJSON = CurrentPlay.GetComponent<TakeJSON>();
                if (takeJSON != null) takeJSON.Run();
#endif

                OnNewPlay(this, new EventNewPlayArgs() { NewPlay = CurrentPlay, PrevPlayResult = mResultado });

                // En este momento conocemos quién es el MainCharacter,
                //   informar al Customization
                mCustomization.SetupMainCharacter(InteractiveActions.MainCharacter);

                // Fade-In con delay para que no se vea el frame impostor
                Camera.main.cullingMask = cullingMask;
                StartCoroutine(CameraFade.FadeCoroutine(true, 1.0f, 0.1f));

                // Esperamos a que la jugada acabe
                yield return StartCoroutine( EsperandoResultado() );
                ManoloLama.Instance.Reevaluate(mResultado == Resultado.Correcto);
                //                if (OnEndInteractiveAction != null) OnEndInteractiveAction(this, null);
                mInteractiveActions.Stop();
                yield return new WaitForSeconds(2.5f);


                // Si existen mas jugadas (y no hemos acabado el partido)
                //                if (!IsLastPlay && !MatchFailed) {
                //                    if (mResultado == Resultado.ChutGol)
                //                        yield return StartCoroutine(InterJugadaCutscene());
                //                    else
                yield return StartCoroutine(CameraFade.FadeCoroutine(false, 1.0f, 0.0f));

                if (mAnimatorData != null)
                    AnimatorTimeline.Stop();
            }
            // Hasta aqui ///////////////////////////////////////////////////////////////////////////////////
            NarratorUI.Instance.Flush();
            MatchEnd ();
		}
	
		void MatchEnd () {
            Destroy(CurrentPlay);
            CurrentPlay = null;
            if ( MainCharacterMark != null )
				MainCharacterMark.Target = null;
			if ( SoccerMark != null )
				SoccerMark.Target = null;
			// Primero le llega el mensaje al MainModel
			mMatchBridge.ReturnMatchResult = mMatchResult;
			mMatchBridge.SendMessage("OnMatchEnd", SendMessageOptions.DontRequireReceiver);
					
			// Y ahora, al resto del GUI del Match3D
			OnMatchEnd(this, new EventMatchEndArgs(mMatchResult));
		}

		public void FinDeJugada(bool result, bool gol ) {
			if ( mResultado != Resultado.FracasoMatch ) {
                mResultado = result ? Resultado.Correcto : Resultado.Incorrecto;
                if (gol && result) {
                    CurrentPlayerGoals++;
					if ( OnGol != null )
						OnGol(this, EventArgs.Empty);
				}
			}
		}

		void OnDestroy () {
			//Indicamos al AudioInGameController que se subscriba a los eventos:
			mAudioController.DeSubscribeFromMatchEvents();
		}
	}
	
}


