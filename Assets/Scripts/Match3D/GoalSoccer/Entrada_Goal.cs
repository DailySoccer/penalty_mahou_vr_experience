using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class Entrada_Goal : GoalComposite {

		private MatchManager mMatchManager;

		private SoccerData mData;
		private SoccerMotor mMotor;
		private Animator mAnimator;
		private BallMotor mBalonMotor;
		
		ArriveSteer mArriveSteer;
		
		Vector3 mInterceptionPos;
		
		public static new Entrada_Goal New( GameObject _owner ) {
			Entrada_Goal goal = Create<Entrada_Goal>(_owner);
			goal.Set();
			return goal;
		}
			
		protected void Set() {
			if ( InitializeGlobal ) {
				mData = gameObject.GetComponent<SoccerData>();
				mMotor = gameObject.GetComponent<SoccerMotor>();
				mAnimator = Entity.GetComponent<Animator>();
				mArriveSteer = gameObject.GetComponent<ArriveSteer>();
				
				GameObject balon = GameObject.FindGameObjectWithTag ("Balon");
				mBalonMotor = balon.GetComponent<BallMotor>();

				mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			}
		}
		
		private void UpdateInfoAnimator() {
			mAnimator.SetFloat ( AnimatorID.Tiempo, mData.CoordinatedTime - Time.time );
			
			float distancia = Helper.DistanceInPlaneXZ( gameObject, mInterceptionPos );
			mAnimator.SetFloat ( AnimatorID.Distancia, distancia );	
			
			GameObject target = mBalonMotor.gameObject;
			Vector3 dirToTarget = Helper.ZeroY ( target.transform.position - transform.position );
			Quaternion rot = Quaternion.FromToRotation( transform.forward, dirToTarget );
			mAnimator.SetFloat( AnimatorID.DireccionBalon, rot.eulerAngles.y );
		}
		
		IEnumerator ApproachToDistance(Vector3 position, float distance) {
			float distanceInterception = Helper.DistanceInPlaneXZ( gameObject, position );
			
			if ( distanceInterception > distance ) {
				mData.MulSpeed = (distanceInterception - distance) / mMotor.MaxSpeed;
				
				mArriveSteer.enabled = true;
				mArriveSteer.Target = null;
				mArriveSteer.targetPosition = position;
				
				while ( (distanceInterception > distance) && (Time.time < mData.CoordinatedTime) ) {
					UpdateInfoAnimator ();
					yield return null;
					distanceInterception = Helper.DistanceInPlaneXZ( gameObject, position );
				}
				
				mArriveSteer.enabled = false;
			}
		}
		
		private Vector3 InteceptionPositionNearest( Vector3 position, Vector3 direction ) {
			Vector3 posRight = position + Quaternion.Euler (0,90,0) * direction;
			Vector3 posLeft = position + Quaternion.Euler (0,-90,0) * direction;
			
			float distanceRight = (posRight - transform.position).sqrMagnitude;
			float distanceLeft = (posLeft - transform.position).sqrMagnitude;
			
			return (distanceLeft < distanceRight) ? posLeft : posRight;
		}
		
		private IEnumerator UpdateState() {
			mAnimator.SetBool( AnimatorID.Entrada, true );
			UpdateInfoAnimator ();

			mMotor.OrientationToAim = 45f;

			SoccerData dataTarget = mBalonMotor.NewPropietary.GetComponentInChildren<SoccerData>();
			Entrenador entrenadorTarget = dataTarget.dummyMirror.GetComponentInChildren<Entrenador>();
			mInterceptionPos = entrenadorTarget.GetPositionAfter( mData.CoordinatedTime - Time.time + 0.3f );
			
			Vector3 nextPos = entrenadorTarget.GetPositionAfter( mData.CoordinatedTime - Time.time + 0.6f );
			Vector3 dirTarget = (nextPos - mInterceptionPos).normalized * 2f;
			nextPos = InteceptionPositionNearest( mInterceptionPos, dirTarget );
			
			float distanceInterception = Helper.DistanceInPlaneXZ( gameObject, nextPos );
			if ( distanceInterception < 2f ) {
				// Helper.Log ( Entity, "Pre Time " + (mData.CoordinatedTime - Time.time) );
				while ( Time.time + 1f < mData.CoordinatedTime ) {
					yield return null;
				}
			}
			
			// Helper.Log ( Entity, "Approach " + (mData.CoordinatedTime - Time.time) );
			yield return StartCoroutine( ApproachToDistance(nextPos, 1.5f) );
			
			yield return StartCoroutine( Helper.WaitCondition( () => {
				UpdateInfoAnimator ();
				return (Time.time + 0.5f > mData.CoordinatedTime);
			} ) );
			
			// Helper.Log ( Entity, "Approach2 " + (mData.CoordinatedTime - Time.time) );
			yield return StartCoroutine( ApproachToDistance(mBalonMotor.transform.position, 0f) );
			
			// Helper.Log ( Entity, "Waiting " + (mData.CoordinatedTime - Time.time) );
			yield return StartCoroutine( Helper.WaitCondition( () => {
				UpdateInfoAnimator ();
				// return mAnimator.GetBool ("Entrada"); 
				return (Time.time > mData.CoordinatedTime);
			} ) );
			
			//Helper.Log ( Entity, "Direccion Balon: " + mAnimator.GetFloat ("DireccionBalon") );
			// Helper.Log ( Entity, "Animation " + (mData.CoordinatedTime - Time.time) );
			yield return new WaitForSeconds(0.1f);
			
			mAnimator.SetBool( AnimatorID.Entrada, false );
			
			mData.CoordinatedTime = -1;
			
			status = Status.Completed;			
		}
		
		private IEnumerator UpdateInstantAction () {
			mData.CoordinatedTime = -1;

			bool quitarBalon = (mMatchManager.InteractiveActions.CurrentAction != null) && (mMatchManager.InteractiveActions.CurrentAction.Action == ActionType.REGATE);
			/*
			if ( quitarBalon ) {
				Helper.Log( Entity, "--> Regate: Puedo Quitar Balon: " + mMatchManager.InteractiveActions.CurrentAction.Action );
			}
			*/

			// Informar al futbolista de que vamos a Entrarle
			if ( mBalonMotor.NewPropietary != null ) {
				SoccerData targetData = mBalonMotor.NewPropietary.GetComponentInChildren<SoccerData>();
				targetData.CoordinatedTarget = Entity;
			}

			GameObject balon = GameObject.FindGameObjectWithTag ("Balon");
			BallMotor ballMotor = balon.GetComponent<BallMotor>();

			mAnimator.SetBool( AnimatorID.Entrada, true );
			UpdateInfoAnimator ();

			mMotor.OrientationToAim = 0f;
			
			yield return StartCoroutine( Helper.WaitCondition( () => { 
				UpdateInfoAnimator ();
				return ( Helper.IsAnimationPlaying( mAnimator, AnimatorID.Entrada ) );
			} ) );
			
			// Helper.Log( Entity, Helper.AnimationClipNames(mAnimator) );

			float distanceMin = 1000f;
			yield return StartCoroutine( Helper.WaitCondition( () => {
				if ( quitarBalon && mMatchManager.MatchFailed ) {
					float distance = (Entity.transform.position - ballMotor.transform.position).magnitude;
					if ( distance < distanceMin ) distanceMin = distance;

					if ( distance < 0.5f || ((distanceMin < 1.5f) && (distance > distanceMin)) ) {
						// Debug.Log( "--> Distance to Balon: " + distance );

						// Quitar el balón al contrincante
						ballMotor.UnAttach();
						ballMotor.ApplyImpulseToPosition( Entity.transform.position + Entity.transform.forward * 20f, 1f );
						quitarBalon = false;
					}
				}
				return ( !Helper.IsAnimationPlaying( mAnimator, AnimatorID.Entrada ) );
			} ) );

			yield return null;
			
			mAnimator.SetBool( AnimatorID.Entrada, false );
			
			mData.CoordinatedTime = -1;
			
			status = Status.Completed;			
		}
		
		public override void Activate () {
			status = Status.Active;
			
			RemoveAllSubgoals();
			
			if ( mData.Entrada ) {
				StartCoroutine ( UpdateInstantAction () );
			}
			else {
				StartCoroutine ( UpdateState () );
			}
			
			//Debug.Log ( ToString () + " Balon: " + balon.ToString() );
		}
		
		public override Status Process () {
			ActivateIfInactive();
			
			if ( isActive () ) {
				/*Status statusSubgoals =*/ // ProcessSubgoals();
				
				// status = Status.Completed;
			
				ReactivateIfFailed();
			}
			
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();
			
			mArriveSteer.enabled = false;
			
			mData.Entrada = false;
		}
		
		void OnDrawGizmos () {
			Gizmos.DrawLine( transform.position + Vector3.up, mInterceptionPos + Vector3.up );
		}			
		
	}
	
}

