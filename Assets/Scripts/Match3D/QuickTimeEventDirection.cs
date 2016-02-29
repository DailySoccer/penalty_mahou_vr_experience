using System;
using UnityEngine;
using System.Collections;
using FootballStar.Common;
	
namespace FootballStar.Match3D
{	
	public class QuickTimeEventDirection : MonoBehaviour {
		const int NumDifficultyLevels = 4;
				
		public CircleArrow2DRenderer GizmoRenderer;
		public Circle2DRenderer CircleRenderer;
		public DirectionAnimation HandAnimation;

		public int Difficulty = 0;
		public float ReactionTime = 0.5f;
		
		[System.Serializable]
		public class Params {
			public float multiplier = 1f;
			public float angle = 90f;
		}
		public Params[] DifficultyParams = new Params[NumDifficultyLevels];
        
        public bool PointOfInterest = true;
		public float TimeReaction { get { return (ReactionTime * DifficultyParams[Difficulty].multiplier); } }
		
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
			mDirectionGesture = GetComponent<DirectionGesture>();
		}
		
		void Start () {
		}
		
		public void Activate( InteractiveActions interactiveActions ) {
			mState = States.ACTIVATED;
			mActions = interactiveActions;
		}
		
		public void Deactivate() {
			mState = States.OFF;
		}
		
		public void EvaluateAction(InteractiveActions _actions)	{
            mState = States.EVALUATE;
            StopAllCoroutines();
            mActions = _actions;
            if(mActions.CurrentAction!=null && mActions.CurrentAction.ActorData!=null)
			    mActions.CurrentAction.ActorData.EvaluateAction = SoccerData.ActionState.Wait;
            mUseAngle = mActions.CurrentAction.Action == ActionType.REGATE;

            mTap = mActions.InputEnabled && GameInput.IsTouchDown();
            mGestureEnabled = false;
            mNotifyWaiting = false;
            mCurrentTimer = 0;
            mScore = 0;
            mCurrentTimerEnd = TimeReaction + 3.0f;
            if (mUseAngle) {
                GameObject actor = mActions.CurrentAction.Actor;
//                GizmoRenderer.Activate(actor, mActions.Target);
//                CircleRenderer.Activate(actor, TimeReaction, 0, 0.1f);
//                CircleRenderer.TimerSync = (x) => mCurrentTimer;

                Vector3 dirToTarget = Helper.ZeroY(mActions.Target.transform.position - actor.transform.position);
                float[] angles = { 45f, 135f, 225f, 315f };
                mAngle = angles[UnityEngine.Random.Range(0, angles.Length - 1)];
                if (HandAnimation != null && mMatchManager.HandAnimations)
                    HandAnimation.Play (Helper.Hash("angle", mAngle) );
            }
            else {
                if (GizmoRenderer) GizmoRenderer.Deactivate();
                if (CircleRenderer) CircleRenderer.Deactivate();
                if (HandAnimation != null) HandAnimation.Stop();
            }
        }

        public void RestartEvaluateAction()	{
			mState  = States.EVALUATE;
			mTap    = GameInput.IsTouchDown();
            mGestureEnabled = false;
            mNotifyWaiting = false;
		}

        IEnumerator PostNotifyAction( bool success ) {
			mNotifyWaiting = true;
			yield return new WaitForEndOfFrame();
			NotifyAction ( success );
		}

		void Update() {
			if (!IsEvaluating)
				return;

			if ( mActions.InputEnabled ) {
				mTap = (mTap) ? !GameInput.IsTouchUp() : GameInput.IsTouchDown();
				if ( mTap ) {
					mTouchPosition = GameInput.TouchPosition;
				}
			}
			// La notificación ya está en curso... (PostNotifyAction)
			if (mNotifyWaiting)
				return;

			mCurrentTimer += Time.deltaTime / Time.timeScale;
            if (mUseAngle)
            {
                if (IsTap && GizmoRenderer.IsActivated ) {
                    GizmoRenderer.Deactivate();
                    CircleRenderer.Deactivate();
                }
            }

            if ( mCurrentTimer <= mCurrentTimerEnd ) {
                if(mActions!=null && mActions.CurrentAction!=null)
                    mActions.CurrentAction.InteractionOn = true;

                if (mActions.InputEnabled && !mGestureEnabled) {
                    mDirectionGesture.enabled = true;
                    mDirectionGesture.Activate( mAngle );
                    mGestureEnabled = true;
                }

                if (mGestureEnabled && (mDirectionGesture.Result == DirectionGesture.EResult.SUCCESS || mDirectionGesture.IsFinished))
                {
                    StartCoroutine( PostNotifyAction( mDirectionGesture.Result == DirectionGesture.EResult.SUCCESS) );
                }
			}
			else {
                mScore = 1; // Se paso el tiempo.
                if ( mDirectionGesture.IsActivated )
                    mDirectionGesture.Deactivate();
                StartCoroutine(PostNotifyAction(false));
			}
		}
		
		public void NotifyAction(bool success) {
			StopAllCoroutines();
			mActions.CurrentAction.InteractionOn = false;
			if ( OnEndInteraction != null ) {
                EventQuickTimeResultArgs args = new EventQuickTimeResultArgs( success, mScore );
				args.Direction = new Vector3( mDirectionGesture.UserDirection.x, 0f, mDirectionGesture.UserDirection.y );
				args.Distance  = 12f;
				args.TouchPosition = mTouchPosition;
				args.Error = (mDirectionGesture.Result == DirectionGesture.EResult.FAIL);
				OnEndInteraction( this, args );
			}

            if (mUseAngle) {
                if (GizmoRenderer && GizmoRenderer.IsActivated) GizmoRenderer.Deactivate();
                if (CircleRenderer && CircleRenderer.IsActivated) CircleRenderer.Deactivate();
                if (HandAnimation != null) HandAnimation.Stop();
            }
            mDirectionGesture.Deactivate();
			
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

        private bool mGestureEnabled = false;
        private DirectionGesture mDirectionGesture;

		// Tiempo según estado
		private float mCurrentTimer;   		// Acumulador
		private float mCurrentTimerEnd;		// Final

		private float mStartGestureTime;
		private bool  mNotifyWaiting = false;

		private bool mTap;  // Ha habido un tap en el ultimo frame?
        private bool mUseAngle = false;
        private float mAngle = 0;
        private float mScore = 0;
        private Vector3 mTouchPosition;
	}
}
