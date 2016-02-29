using System;
using UnityEngine;
using System.Collections;
using FootballStar.Common;
	
namespace FootballStar.Match3D
{	
	public class QuickTimeEventDblTap : MonoBehaviour {	
		
		const int NumDifficultyLevels = 4;
				
		public Circle2DRenderer GizmoRenderer;
		public IconRenderer Iconx2;

		public TapAnimation HandAnimation;

		public bool TapInside = false;
			
		public int Difficulty = 0;
		
		public float ScalingTime = 0.8f;
		public float ReactionTime = 0.5f;
		public float[] DifficultyMultiplier = new float[NumDifficultyLevels];
		
		public bool PointOfInterest = true;
			
		public float TimeInvalid { get { return (TimeTotal - (TimeScaling+TimeReaction)); } }
		public float TimeScaling { get { return (ScalingTime * DifficultyMultiplier[Difficulty]); } }
		public float TimeReaction {	get { return (ReactionTime * DifficultyMultiplier[Difficulty]); } }
		public float TimeTotal { get { return (ScalingTime + ReactionTime); } }
		public float TimeInteraction { get { return (TimeScaling + TimeReaction); } }
		
		public bool IsEvaluating { get { return (mState == States.EVALUATE); } }
		public bool IsTap { get { return mTap; } }		// Ha habido un tap en en ultimo frame?
		
		public event EventHandler OnShowInteraction;
		public event EventHandler OnPerfectInteraction;
		public event EventHandler<EventQuickTimeResultArgs> OnEndInteraction;	// Tanto en caso de exito como de fracaso
		
		enum States {
			OFF,
			ACTIVATED,
			EVALUATE,
		};
		States mState = States.OFF;
		
		void Awake () {
			mMatchManager = gameObject.GetComponent<MatchManager>();
			mBallMotor = GameObject.FindGameObjectWithTag("Balon").GetComponentInChildren<BallMotor>();
			// HandAnimation = Helper.GetComponent<AnimationScript>( GizmoRenderer.transform, "Animation" );
		}
		
		public void Activate( InteractiveActions interactiveActions ) {
			mState = States.ACTIVATED;
			mActions = interactiveActions;
		}
		
		public void Deactivate() {
			mState = States.OFF;
		}
		
		public void EvaluateAction(InteractiveActions _actions) {
			//Debug.Log( "" + Time.time + ": DBLTAP ---------------" );

			mState = States.EVALUATE;
			
			StopAllCoroutines();
			
			mTap = false;
			mClicks = 2;
			
			mActions = _actions;
			mActions.CurrentAction.ActorData.EvaluateAction = SoccerData.ActionState.Wait;

			mNotifyWaiting = false;
			mActionTime = -1;

			mCurrentTimer = 0;
			mCurrentTimer0 = TimeInvalid;
			mCurrentTimer1 = mCurrentTimer0 + TimeScaling;
			mCurrentTimerEnd = mCurrentTimer1 + TimeReaction;

			//Debug.Log( "" + Time.time + ": DblTap: " + mCurrentTimer + " START" );

			StartCoroutine( UpdateTimers() );
			
			GizmoRenderer.Activate ( mActions.CurrentAction.Actor, TimeTotal, TimeScaling, TimeReaction );
			GizmoRenderer.TimerSync = (x) => mCurrentTimer;
			
			if ( Iconx2 )
				Iconx2.Activate( mActions.CurrentAction.Actor );
			
			if ( HandAnimation != null && mMatchManager.HandAnimations )
				HandAnimation.Play( Helper.Hash( "amount", 2 ) );	
		}

		IEnumerator WaitTimer( float maxTimer ) {
			while ( mCurrentTimer < maxTimer ) {
				yield return null;
			}
		}
		
		IEnumerator UpdateTimers() {
			if ( OnShowInteraction != null )
				OnShowInteraction( this, EventArgs.Empty );

			//Debug.Log( "" + Time.time + ": DblTap: " + mCurrentTimer + " INVALID" );
			yield return StartCoroutine( WaitTimer (mCurrentTimer0) );
			
			mActions.CurrentAction.InteractionOn = true;

			//Debug.Log( "" + Time.time + ": DblTap: " + mCurrentTimer + " SCALING" );
			yield return StartCoroutine( WaitTimer (mCurrentTimer1) );

			if ( OnPerfectInteraction != null )
				OnPerfectInteraction( this, EventArgs.Empty );
						
			//Debug.Log( "" + Time.time + ": DblTap: " + mCurrentTimer + " REACTION" );
			yield return StartCoroutine( WaitTimer (mCurrentTimerEnd) );

			StartCoroutine( WaitTimer (mCurrentTimerEnd + 1f) );
		}

		IEnumerator PostNotifyAction( bool success ) {
			mNotifyWaiting  = true;

			yield return new WaitForEndOfFrame();

			NotifyAction ( success );
		}
		
		bool ExitIfAutomaticTap() {
			bool exit = false;
			if ( mMatchManager.AutomaticTap.Enabled && mActions.InputEnabled ) {
				if ( mCurrentTimer <= mCurrentTimerEnd ) {
					/*if ( mCurrentTimer < mCurrentTimer0 ) {
					}
					else if ( mCurrentTimer < mCurrentTimer1 ) {
						if ( mMatchManager.AutomaticTap.IsWaiting(MomentoRespuesta.PRONTO) ) {
							// MomentoRespuesta.PRONTO
							exit = true;
						}
					}
					else {
						if ( mMatchManager.AutomaticTap.IsWaiting(MomentoRespuesta.PERFECTO) ) {
							// MomentoRespuesta.PERFECTO
							exit = true;
						}
					}*/
					exit = true;
				}
				else {
					// MomentoRespuesta.TARDE
					exit = true;
				}
			}
			return exit;
		}

		void Update() {
			if (!IsEvaluating)
				return;

			bool tapOld = IsTap;

			if ( mActions.InputEnabled ) {
				mTap = (mTap) ? !GameInput.IsTouchUp() : GameInput.IsTouchDown();

				if ( mTap ) {
					mTouchPosition = GameInput.TouchPosition;
				}
			}
			
			// Click? (Tap = true && Primer tap)
			bool click = IsTap && (IsTap != tapOld);
			
			bool clickInCircle = false;
			if ( click ) {
				clickInCircle = TapInside ? GizmoRenderer.IsPointInside( GameInput.TouchPosition ) : true;
			}

			// La notificación ya está en curso... (PostNotifyAction)
			if (mNotifyWaiting)
				return;

			mCurrentTimer += Time.deltaTime;
			
			if ( ExitIfAutomaticTap() ) {
				mActionTime = mCurrentTimer = (mCurrentTimer <= mCurrentTimerEnd) ? mCurrentTimer : mCurrentTimerEnd-0.01f;
				mClicks = 0;
			}
			else if ( mCurrentTimer < mCurrentTimer0 ) {
				if ( IsTap )
					StartCoroutine( PostNotifyAction (false) );
			}
			else if ( mCurrentTimer <= mCurrentTimerEnd ) {
				if ( clickInCircle ) {
					if ( Iconx2 && Iconx2.IsActivated ) {
						mActionTime = mCurrentTimer;
						Iconx2.Deactivate();	
					}
					mClicks--;
				}
			}
			else {
				if ( IsTap || mCurrentTimer > mCurrentTimerEnd + 0.2f )
					StartCoroutine( PostNotifyAction (false) );
			}
			
			if ( mCurrentTimer <= mCurrentTimerEnd && mClicks <= 0 ) {
				StartCoroutine( PostNotifyAction (true) );
			}
		}
		
		float EvaluateScore (bool success) {
			float valoracion = 0f;

			if ( success ) {
				if ( mActionTime <= mCurrentTimer1 ) {
					// Pronto!
					float diff = mCurrentTimer1 - mActionTime;
					valoracion = -(diff / ScalingTime);
					// Debug.Log ( "Diff: " + diff + " Scaling: " + TimeScaling );
				}
				else /*if ( mActionTime <= mCurrentTimerEnd )*/ {
					// Ok!
					float diff = mActionTime - mCurrentTimer1;
					valoracion = (diff / ReactionTime);
					// Debug.Log ( "Diff: " + diff + " Reaction: " + TimeReaction );
				}
			}
			else {
				// Muy pronto! | Muy tarde!
				valoracion = ( mCurrentTimer < mCurrentTimer0 ) ? -1f : 1f;
			}
			
			// Debug.Log ( "QuickTimeEventDblTap: Valoracion: " + valoracion );
			
			return valoracion;
		}
		
		public void NotifyAction(bool success) {
			StopAllCoroutines();
			
			mActions.CurrentAction.InteractionOn = false;
			
			float score = EvaluateScore (success);
			
			if ( OnEndInteraction != null ) {
				EventQuickTimeResultArgs args = new EventQuickTimeResultArgs( success, score );
				args.Direction = (mActions.Target.transform.position - mBallMotor.NewPropietary.transform.position).normalized;
				args.Distance  = 12f;
				args.TouchPosition = mTouchPosition;
				args.Error = (mClicks > 0);
				OnEndInteraction( this, args );
			}
			
			if ( Iconx2 )
				Iconx2.Deactivate();
			
			if ( GizmoRenderer && GizmoRenderer.IsActivated )
				GizmoRenderer.Deactivate();

			if ( HandAnimation != null )
				HandAnimation.Stop();			

			mState = States.OFF;
			mActions = null;
			mNotifyWaiting = false;
		}
		
		public void NotifyActionIncomplete() {
			Debug.LogWarning( "QuickTimeEvent cancelado: Necesita mas tiempo para interaccion: +" + (mCurrentTimerEnd - mCurrentTimer) );
			mCurrentTimer = mCurrentTimerEnd;
			NotifyAction(false);
		}
		
		private InteractiveActions mActions;
		private MatchManager mMatchManager;
		private BallMotor mBallMotor;

		private float mCurrentTimer;   		// Acumulador
		private float mCurrentTimer0;		// Estado 0
		private float mCurrentTimer1;		// Estado 1
		private float mCurrentTimerEnd;		// Estado 2 / Final
		private float mActionTime;			// Momento en el que se produjo la acción
		private bool  mNotifyWaiting = false;
		
		private bool mTap;	// Ha habido un tap en el ultimo frame?
		private Vector3 mTouchPosition;

		private int mClicks;
	}
}

