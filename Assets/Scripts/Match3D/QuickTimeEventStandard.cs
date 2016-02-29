using System;
using UnityEngine;
using System.Collections;
using FootballStar.Common;
	
namespace FootballStar.Match3D
{	
	public class EventQuickTimeResultArgs : EventArgs {
		public bool Success { get; set; }
		public float Result { get; set; }
		public Vector3 Direction { get; set; }
		public float Distance { get; set; }
		public Vector3 TouchPosition { get; set; }
		public bool Error = false;
		
		public EventQuickTimeResultArgs( bool success, float result ) {
			Success = success;
			Result = result;
		}
	};
	
	public class QuickTimeEventStandard : MonoBehaviour
	{	
		const int NumDifficultyLevels = 4;
		
		public Circle2DRenderer GizmoRenderer;
		public TapAnimation HandAnimation;

		public bool TapInside = false;
			
		public int Difficulty = 0;
		
		public float ScalingTime = 2.0f;
		public float ReactionTime = 2.0f;

		public float[] DifficultyMultiplier = new float[NumDifficultyLevels];
		
		public bool PointOfInterest = true;
			
		public float TimeInvalid { get { return (TimeTotal - (TimeScaling+TimeReaction)); } }
		public float TimeScaling { get { return (ScalingTime * DifficultyMultiplier[Difficulty]); } }
		public float TimeReaction {	get { return (ReactionTime * DifficultyMultiplier[Difficulty]); } }
		public float TimeTotal { get { return (ScalingTime + ReactionTime); } }
		public float TimeInteraction { get { return (TimeScaling + TimeReaction); } }
		
		public bool IsEvaluating { get { return (mState == EStates.EVALUATE); } }
		public bool IsTap { get { return mTap; } }		// Ha habido un tap en en ultimo frame?
		
		public event EventHandler OnShowInteraction;
//		public event EventHandler OnPerfectInteraction;
		public event EventHandler<EventQuickTimeResultArgs> OnEndInteraction;	// Tanto en caso de exito como de fracaso
		
		enum EStates {
			OFF,
			ACTIVATED,
			EVALUATE,
		};
		EStates mState = EStates.OFF;
		
		void Awake ()
		{
			mMatchManager = gameObject.GetComponent<MatchManager>();
			mBallMotor = GameObject.FindGameObjectWithTag("Balon").GetComponentInChildren<BallMotor>();
			// HandAnimation = Helper.GetComponent<AnimationScript>( GizmoRenderer.transform, "Animation" );
		}
		
		public void Activate( InteractiveActions interactiveActions ) {
			mState = EStates.ACTIVATED;
			mActions = interactiveActions;
		}
		
		public void Deactivate() {
			mState = EStates.OFF;
		}
		
		public void EvaluateAction(InteractiveActions _actions)	{
			//Debug.Log( "" + Time.time + ": STANDARD ---------------" );

			mState = EStates.EVALUATE;
			
			StopAllCoroutines();
			
			mTap = false;
			
			mActions = _actions;
			mActions.CurrentAction.ActorData.EvaluateAction = SoccerData.ActionState.Wait;

			mNotifyWaiting = false;
			mActionTime = -1;
			
			mCurrentTimer = 0;
			mCurrentTimer0 = TimeInvalid;
			mCurrentTimer1 = mCurrentTimer0 + TimeScaling;
			mCurrentTimerEnd = mCurrentTimer1 + TimeReaction;

			StartCoroutine( UpdateTimers() );

//            Camera.main.GetComponent<MatchAutoCam>().ChangeMode(MatchAutoCam.eMode.PASS_MODE);
/*
            if ( GizmoRenderer ) {
				if ( PointOfInterest ) {
					GizmoRenderer.Activate ( mActions.Target, TimeTotal, TimeScaling, TimeReaction );
				}
				else {
					GizmoRenderer.Activate ( mBallMotor.NewPropietary, TimeTotal, TimeScaling, TimeReaction );
				}
				GizmoRenderer.TimerSync = (x) => mCurrentTimer;
			}
*/
			if ( HandAnimation != null && mMatchManager.HandAnimations )
				HandAnimation.Play( Helper.Hash( "amount", 1 ) );
		}

		IEnumerator WaitTimer( float maxTimer )	{
			while ( mCurrentTimer < maxTimer ) {
				yield return null;
			}
		}
		
        // CACA: Ver que hace esta funcion.
		IEnumerator UpdateTimers() 
		{
			if ( OnShowInteraction != null )
				OnShowInteraction( this, EventArgs.Empty );

			yield return StartCoroutine( WaitTimer (mCurrentTimer0) );

            if (GizmoRenderer)
            {
                if (PointOfInterest)
                {
                    GizmoRenderer.Activate(mActions.Target, TimeTotal, TimeScaling, TimeReaction);
                }
                else
                {
                    GizmoRenderer.Activate(mBallMotor.NewPropietary, TimeTotal, TimeScaling, TimeReaction);
                }
                GizmoRenderer.TimerSync = (x) => mCurrentTimer;
            }
            yield return StartCoroutine( WaitTimer (mCurrentTimer1) );
//			if ( OnPerfectInteraction != null ) OnPerfectInteraction( this, EventArgs.Empty );
			yield return StartCoroutine( WaitTimer (mCurrentTimerEnd) );
			StartCoroutine( WaitTimer (mCurrentTimerEnd + 1f) );
		}
		
		IEnumerator PostNotifyAction( bool success ) {
			mNotifyWaiting = true;
			mActionTime = mCurrentTimer;
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
		
		void Update()
		{
			if (!IsEvaluating)
				return;

			if ( mActions.InputEnabled ) {
				mTap = (mTap) ? !GameInput.IsTouchUp() : GameInput.IsTouchDown();

				if ( mTap ) {
					mTouchPosition = GameInput.TouchPosition;
				}
			}

			bool clickInCircle = false;
			if ( IsTap ) {
				clickInCircle = TapInside ? GizmoRenderer.IsPointInside( GameInput.TouchPosition ) : true;
			}
			
			// La notificación ya está en curso... (PostNotifyAction)
			if (mNotifyWaiting)
				return;

			mCurrentTimer += Time.deltaTime / Time.timeScale;
            
			if ( ExitIfAutomaticTap() ) {
				mCurrentTimer = (mCurrentTimer <= mCurrentTimerEnd) ? mCurrentTimer : mCurrentTimerEnd-0.01f;
				StartCoroutine( PostNotifyAction ( true ) );
			}
			else if ( mCurrentTimer < mCurrentTimer0 )
			{
				if ( IsTap )
					StartCoroutine( PostNotifyAction ( false ) );
			}
			else if ( mCurrentTimer < mCurrentTimer1 ) 
			{
				mActions.CurrentAction.InteractionOn = true;
				
				if ( clickInCircle ) {
					StartCoroutine( PostNotifyAction ( true ) );
				}
			}
			else if ( mCurrentTimer <= mCurrentTimerEnd )
			{
				if ( clickInCircle ) {
					StartCoroutine( PostNotifyAction ( true ) );
				}
			}
			else {
				// Esperamos durante un tiempo (para que se muestre el círculo ROJO)
				if ( IsTap || mCurrentTimer > mCurrentTimerEnd + 0.2f ) {
					StartCoroutine( PostNotifyAction ( false ) );
				}
			}
		}
		
		float EvaluateScore ()
		{
			float valoracion = 0f;
			
			if (mActionTime <= mCurrentTimer0) {
				// Muy Pronto!
				valoracion = -1;
			} 
			else if (mActionTime <= mCurrentTimer1 || mActionTime <= mCurrentTimerEnd) {
				valoracion = 0;
			}
			/*else if ( mActionTime <= mCurrentTimer1 ) {
				// Pronto!
				float diff = mCurrentTimer1 - mActionTime;
				valoracion = -(diff / ScalingTime);
			}
			else if ( mActionTime <= mCurrentTimerEnd ) {
				// Ok!
				float diff = mActionTime - mCurrentTimer1;
				valoracion = (diff / ReactionTime);
			}*/
			else {
				// Muy tarde!
				valoracion = 1f;
			}

			//Debug.Log ( "QuictTimeEventStardard: Valoracion: " + valoracion );
			
			return valoracion;
		}
		
		public void NotifyAction(bool success)
		{
			StopAllCoroutines();
			
			mActions.CurrentAction.InteractionOn = false;
			
			float score = EvaluateScore ();
			
			if ( OnEndInteraction != null ) {
				EventQuickTimeResultArgs args = new EventQuickTimeResultArgs( success, score );
                args.Direction = (mActions.Target.transform.position - mBallMotor.NewPropietary.transform.position).normalized;
                args.Distance = 12f;
                args.TouchPosition = mTouchPosition;
				OnEndInteraction( this, args );
			}
			
			if ( GizmoRenderer && GizmoRenderer.IsActivated )
				GizmoRenderer.Deactivate();

			if ( HandAnimation != null )
				HandAnimation.Stop();

			mActionTime = -1f;
			mState = EStates.OFF;
			mActions = null;
			mNotifyWaiting = false;

//            Camera.main.GetComponent<MatchAutoCam>().ChangeMode(MatchAutoCam.eMode.FOLLOWING_PLAY);

        }

        public void NotifyActionIncomplete() 
		{
			Debug.LogWarning( "QuickTimeEvent cancelado: Necesita mas tiempo para interaccion: +" + (mCurrentTimerEnd - mCurrentTimer) );
			mCurrentTimer = mCurrentTimerEnd;
			NotifyAction(false);
		}
		
		/*
		void OnDrawGizmos () {
			if ( !IsEvaluating) return;
			
			if ( mPowerActive && mPowerStartTime > 0 ) {
				if ( mBallMotor.NewPropietary != null && mActions.Target != null ) {
					Vector3 dirToTarget = (mActions.Target.transform.position - mBallMotor.NewPropietary.transform.position).normalized;
					float distance = InterpolatePowerDistance( Time.time - mPowerStartTime );
					Vector3 ori = mBallMotor.NewPropietary.transform.position + Vector3.up;
					
					//Gizmos.color = (PowerFromDistance( distance ) == mPowerDistance) ? Color.green : Color.red;
					Gizmos.DrawLine( ori, ori + dirToTarget * distance );
				}
			}
		}
		*/

		private InteractiveActions mActions;
		private MatchManager mMatchManager;
		private BallMotor mBallMotor;

		// Tiempo según estado
		private float mCurrentTimer;   		// Acumulador
		private float mCurrentTimer0;		// Estado 0
		private float mCurrentTimer1;		// Estado 1
		private float mCurrentTimerEnd;		// Estado 2 / Final
		private float mActionTime;			// Momento en el que se produjo la acción
		private bool  mNotifyWaiting = false;
		
		private bool mTap;	// Ha habido un tap en el ultimo frame?
		private Vector3 mTouchPosition;
	}
}

