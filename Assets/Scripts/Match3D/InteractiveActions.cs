using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {
	
	public enum InteractionStates {
		BEGIN,
		PERFECT,
		END,
		BALL_KICK,
		TUTORIAL_GESTURE
	};

	public enum InteractionResponseKind {
		TOO_EARLY,
		EARLY,
		PERFECT,
		LATE,
		TOO_LATE
	}

	public class EventInteractiveActionArgs : EventArgs {
		public InteractionStates State { get; set; }

		public ActionType ActionType { get; set; }

		private float mResult;
		public float Result { 
			get { return mResult; } 
			set { mResult = value; ResultKind = EvaluateResult( mResult ); }
		}
		
		public int Current { get; set; }
		public int Total { get; set; }
		public bool Success { get; set; }
		public EventQuickTimeResultArgs QuickTimeResult { get; set; }

		public InteractionResponseKind ResultKind { get; set; }
		
		public EventInteractiveActionArgs(InteractionStates state) { State = state; }

		static public InteractionResponseKind EvaluateResult( float result ) {
			InteractionResponseKind kind;
            if (result >= 1)
				kind = InteractionResponseKind.TOO_LATE;
			else if (result > 0.10f)
				kind = InteractionResponseKind.LATE;
			else if (result >= -0.02f)
				kind = InteractionResponseKind.PERFECT;
			else if (result > -1)
				kind = InteractionResponseKind.EARLY;
			else
				kind = InteractionResponseKind.TOO_EARLY;

			return kind;
		}
	}
	
	public class ConditionDistance {
		public Vector3 Position;
		public float Distance;
		
		public bool IsValid( GameObject go ) {
			return (go.transform.position - Position).sqrMagnitude < (Distance * Distance);
		}
	}
	
	public class InteractiveActions : MonoBehaviour {
	
		private GameObject mMainCharacter = null;
		public GameObject MainCharacter {
			get { 
				if ( mMainCharacter == null )
					mMainCharacter = FindMainCharacter();
				return mMainCharacter; 
			}
		}
		
		public GameObject Target { get; set; }
		[HideInInspector] public int Index = -1;
		public int Count {
			get { return mActions.Count; }
		}

		public bool InputEnabled { get; set; }

		public event EventHandler<EventInteractiveActionArgs> OnEvent;
		List<TweenAction> mActions = new List<TweenAction>();
		public List<TweenAction> Actions {
			get { return mActions; }
		}
		
		int mCurrentActionIndex = -1;
		public TweenAction CurrentAction {
			get {
				if ( mCurrentActionIndex == -1 )
					return null;
				return mActions[ mCurrentActionIndex ];
			}
		}

		public TweenAction NextAction {
			get {
				int next = NextActionIndex();
				if ( next == -1 )
					return null;
				return mActions[ next ];
			}
		}


		// bool mCoordinatedAction = false;
		float mStartTime;

		void Awake () {
			InputEnabled = true;
		}

		void Start () {
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			mMatchManager.OnNewPlay += OnNewPlay;
			
			// Registramos el QTE
			mQTEDirection = mMatchManager.GetComponent<QuickTimeEventDirection>();
			mQTEDirection.OnShowInteraction 	+= OnShowInteraction;
			mQTEDirection.OnPerfectInteraction 	+= OnPerfectInteraction;
			mQTEDirection.OnEndInteraction 	+= OnEndInteraction;

            // Registramos el Balon
            GameObject balon = GameObject.FindGameObjectWithTag ("Balon");
			mBalonMotor = balon.GetComponentInChildren<BallMotor>();
		}

		void Initialize () {
			mMainCharacter = null;
			Target = null;
			
			Index = -1;
			mActions.Clear();
			mCurrentActionIndex = -1;
			
			// Tiempo en el que se inicio esta jugada
			mStartTime = Time.time;
		}
		
		void OnNewPlay(object sender, MatchManager.EventNewPlayArgs e) {
			Initialize ();
			SetupActions ();
		}

		public void Stop() {
			ChangeAction( -1 );
		}
		
		List<AMTween> FindTweens() {
			List<AMTween> lTweens = new List<AMTween>();
			GameObject[] locales = GameObject.FindGameObjectsWithTag( "Local" );
			foreach( GameObject local in locales ) {
				AMTween[] tweens = local.GetComponents<AMTween>();
				lTweens.AddRange( tweens );
			}

			GameObject[] visitantes = GameObject.FindGameObjectsWithTag( "Visitante" );
			foreach( GameObject visitante in visitantes ) {
				AMTween[] tweens = visitante.GetComponents<AMTween>();
				lTweens.AddRange( tweens );
			}
			
			return lTweens;
		}		
		
		void SetupActions () {
			// Inicializamos la tabla de acciones interactivas
			List<AMTween> tweens = FindTweens();
			foreach( AMTween tween in tweens ) {
				ActionType actionType = TweenAction.GetActionType( tween );
				if ( actionType == ActionType.NINGUNO
					|| actionType == ActionType.BALON 
				  	|| actionType == ActionType.MOVIMIENTO ) {
					continue;
				}
				
				float startTime = mStartTime + tween.delay;
				TweenAction tweenAction = TweenAction.Create ( startTime, tween );
				if ( tweenAction != null ) {
					mActions.Add( tweenAction );
				}
			}
				
			if ( mActions.Count > 0 ) {
				mActions.Sort( TweenAction.CompareStartTime );
				mCurrentActionIndex = 0;
                mQTEDirection.Activate(this);
    			
				// Modificar el inicio de las acciones para que tenga en cuenta el tiempo para llegar el balón al destino
				float tiempo = 0f;
				float hastaDestino = 0f;
				foreach( TweenAction action in mActions ) {
					if ( tiempo + hastaDestino > action.StartTime ) {
						action.StartTime = tiempo + hastaDestino;
					}
					
					tiempo = action.StartTime;
					hastaDestino = action.HastaDestino;
				}
			}
		}
		
		GameObject FindMainCharacter () {
			GameObject mainCharacter = null;
			foreach( TweenAction action in mActions ) {
				if ( action.Action == ActionType.CHUT || action.Action == ActionType.PASE || action.Action == ActionType.REGATE || action.Action == ActionType.REMATE ) {
					mainCharacter = action.Actor;
					break;
				}
			}
            if (mainCharacter == null) mainCharacter = GameObject.Find("Soccer-Local11");

            return mainCharacter;
		}
		
		bool IsActionValid ( TweenAction tweenAction ) {
			return (mBalonMotor.NewPropietary == CurrentAction.Actor );
		}
		
		public float TimeForAction( TweenAction tweenAction ) {
			float tiempo = tweenAction.StartTime;
			// REVIEW: No modificamos el tiempo de la acción (aunque el futbolista no tenga el balón)
			if ( !tweenAction.ActorData.BallPropietary && (mBalonMotor.NewPropietary == tweenAction.Actor) ) {
				float balonTime = Time.time + mBalonMotor.TimeToTarget;
				if ( balonTime > tiempo ) {
					tiempo = balonTime;
				}
			}
			// Corregimos ligeramente el tiempo para detectar la interacción (antes de la acción en sí misma)
			return tiempo - 0.1f;
		}

		public float TimeInteraction( TweenAction tweenAction ) {
			return mQTEDirection.TimeReaction;
		}

        public void EvaluateAction() {
            mMatchManager.GetComponent<TimeScale>().factor = 0.1f;
            mQTEDirection.EvaluateAction(this);
        }
		
		void Update () {
			if ( CurrentAction == null ) 
				return;
			
			if ( CurrentAction.State != TweenAction.EState.None )
				return;

            if ( IsActionValid(CurrentAction) ) {
                Target = CurrentAction.Target;
                Index = mCurrentActionIndex;
                CurrentAction.State = TweenAction.EState.Waiting;
            }
		}
		
		void ChangeAction( int actionIndex ) {
			if ( mCurrentActionIndex != actionIndex ) {
				if ( CurrentAction != null ) {
                    mQTEDirection.Deactivate();
				}
				mCurrentActionIndex = actionIndex;
                if ( CurrentAction != null )
                    mQTEDirection.Activate(this);
            }
        }
		
		int NextActionIndex () {
			int next = mCurrentActionIndex + 1;
			if ( next >= mActions.Count ) {
				next = -1;
			}
			return next;
		}
		
		public bool HasAction( ActionType actionType ) {
			foreach( TweenAction action in mActions ) {
				if ( action.Action == actionType ) {
					return true;
				}
			}
			return false;
		}
		
		public void ActionSuccess( float score ) {
			CurrentAction.Result = true;
			SoccerData actorData = CurrentAction.ActorData;
			actorData.NewOrder = true;
			actorData.EvaluateAction = SoccerData.ActionState.None;
			actorData.ActionSuccess = true;
		}
		
		public void ActionFail( float score ) {
			CurrentAction.Result = false;

			SoccerData actorData = CurrentAction.ActorData;
			actorData.NewOrder = true;
			actorData.EvaluateAction = SoccerData.ActionState.None;
			actorData.ActionSuccess = true;
		}

		public void ActionFail( float score, Vector3 direction, float distance ) {
			CurrentAction.Result = false;
			
			SoccerData actorData = CurrentAction.ActorData;
			actorData.NewOrder = true;
			actorData.EvaluateAction = SoccerData.ActionState.None;

			actorData.ActionSuccess = true;
			actorData.ActionDirection = direction;
			actorData.ActionDistance = distance;
		}
        		
		public void NotifyActionIncomplete() {
	        mQTEDirection.NotifyActionIncomplete();
		}

		public void MsgAction( InteractionStates state ) {
			if ( OnEvent != null ) {
				EventInteractiveActionArgs args = new EventInteractiveActionArgs( state );
				args.Current = Index + 1;
				args.Total = Count;
				OnEvent( this, args );
			}
		}
		
		public void MsgBallKick( ActionType type ) {
            if (OnEvent != null) {
                EventInteractiveActionArgs args = new EventInteractiveActionArgs(InteractionStates.BALL_KICK);
                args.Success = true;
                args.Current = Index + 1;
                args.Total = Count;
                OnEvent(this, args);
            }
		}
		
		void OnShowInteraction (object sender, EventArgs e) {
			MsgAction( InteractionStates.BEGIN );
		}
		
		void OnPerfectInteraction (object sender, EventArgs e) {
			MsgAction( InteractionStates.PERFECT );
		}

		void OnEndInteraction (object sender, EventQuickTimeResultArgs e) {
            // Estadísticas
            /*
			MixPanel.SendEventToMixPanel(AnalyticEvent.MATCH_TAP, new Dictionary<string,object>() {
				{ "MatchName", mMatchManager.CurrentMatch.name },
				{ "Play", mMatchManager.CurrentPlay.PlayGameObject.name },
				{ "Success", e.Success }, 
				{ "Type", CurrentAction.Action.ToString() }, 
				{ "Score", e.Result.ToString("0.000") },
				{ "EventIndex", Index+1 }, 
				{ "EventTotal", Count },
				{ "Difficulty", ActionDifficulty( CurrentAction ) },
			} );
			*/

            // Reenviar el evento a otros interesados
            if ( OnEvent != null ) {
				EventInteractiveActionArgs args = new EventInteractiveActionArgs( InteractionStates.END );
				args.ActionType = CurrentAction.Action;
				args.Success = e.Success;
				args.Result  = e.Result;
				args.Current = Index + 1;
				args.Total = Count;
				args.QuickTimeResult = e;

				OnEvent( this, args );
			}
			// Procesar el resultado de la acción
			if ( e.Success ) {
				ActionSuccess( e.Result );
			}
			else {
                ActionFail( e.Result );
			}

            // EndAction
            mMatchManager.GetComponent<TimeScale>().factor = 1.0f;
        }


        MatchManager mMatchManager;
		QuickTimeEventDirection mQTEDirection;
		BallMotor mBalonMotor;
	}
	
}
