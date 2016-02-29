using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class MarcajeAlHombre_Goal : GoalPipe {
		
		private SoccerData mData;
		// private BallMotor mBalonMotor;
		
		private PropietaryPosition mPointAttack;
		private ZonaAtaque mZonaAtaque;
		private GameObject mPorteria;
		
		private GameObject _target;
		private GameObject Target {
			get {
				return _target;
			}
			set {
				if ( _target != value ) {
					_target = value;
					if ( _target != null ) {
						mZonaAtaque = _target.GetComponentInChildren<ZonaAtaque>();
					}
				}
			}
		}
		
		enum SubState {
			ApproachToTarget
		};
		private SubState mSubstate;		
		
		ArriveSteer mArriveSteer;
		
		public static new MarcajeAlHombre_Goal New( GameObject _owner ) {
			MarcajeAlHombre_Goal goal = Create<MarcajeAlHombre_Goal>(_owner);
			goal.Set();
			return goal;
		}
			
		protected void Set() {
			if ( InitializeGlobal ) {
				mData = gameObject.GetComponent<SoccerData>();
				
				// GameObject balon = GameObject.FindGameObjectWithTag ("Balon");
				// mBalonMotor = balon.GetComponentInChildren<BallMotor>();
				
				mArriveSteer = gameObject.GetComponent<ArriveSteer>();
			}
		}
		
		private IEnumerator FSM() {
			while (true) {
				yield return StartCoroutine( mSubstate.ToString() );
				yield return null;
			}
		}
		
		private SoccerData CalculateEnemyNearest() {
			return mData.SoccerNearest( mData.EnemigoNear, soccer => true );
		}
		
		private IEnumerator ApproachToTarget () {
			mData.MulSpeed = 0.2f + 0.4f * Random.value;
			mData.MulSpeed = 1f;
			
			// Target = /*mData.BallNear ? mBalonMotor.NewPropietary :*/ CalculateEnemyNearest();
			if ( Target == null ) {
				status = Status.Completed;
				yield return null;
			}
			
			if ( mZonaAtaque != null ) {
				GameObject target = Target;
				mData.Aim = target;
			
				mPointAttack = mZonaAtaque.GetAttackPosition( gameObject );
				if ( mPointAttack != null ) {
					mArriveSteer.enabled = true;
					mArriveSteer.Target = null;
					mArriveSteer.targetPosition = mPointAttack.position;					
					
					yield return new WaitForSeconds(3f);
					
					mArriveSteer.enabled = false;
					
					
					float distanceToPorteria = Helper.DistanceInPlaneXZ( mPorteria, gameObject );
					
					Vector3 dir = (transform.position - mZonaAtaque.transform.position);
					
					float distance = dir.magnitude;
					if ( distance > 10f || distanceToPorteria < 40f )
						mData.MulSpeed = 0.2f + 0.6f * Random.value;
					else
						mData.MulSpeed = 0.2f + 0.4f * Random.value; 
					mData.MulSpeed = 1f;
					
					/*
					Vector3 dirPoint = (mPointAttack.position - mZonaAtaque.transform.position);
					float distancePoint = dirPoint.magnitude;
					float distanceToTarget = (target.transform.position - transform.position).magnitude;
					
					Quaternion quatPoint = Quaternion.LookRotation(dirPoint);
					Quaternion quatDir = Quaternion.LookRotation(dir);
					
					float angleToPoint = quatPoint.eulerAngles.y - quatDir.eulerAngles.y;
					if ( Mathf.Abs(angleToPoint) > 30f && (distanceToTarget < distancePoint) ) { 
						
						if ( angleToPoint < 0 )
							angleToPoint += 360;
						if ( angleToPoint > 180f && angleToPoint < 350f ) {
							// Helper.Log( Entity, "Right: " + angleToPoint );
							PushOrder( EOrder.MoveRight, target, 2f, 100f );
							yield return StartCoroutine( RunningOrders(1f, null) );
						}
						else if ( angleToPoint > 10f && angleToPoint < 180f ) {
							// Helper.Log( Entity, "Left: " + angleToPoint );
							PushOrder( EOrder.MoveLeft, target, 2f, 100f );
							yield return StartCoroutine( RunningOrders(1f, null) );
						}
						else {
							PushOrder( EOrder.Approach, target, dirPoint.magnitude, 100f );
							yield return StartCoroutine( RunningOrders(1f, null) );
						}
						// Helper.Log( Entity, "Angle: " + angleToPoint );
					}
					else {
						PushOrder( EOrder.Approach, target, dirPoint.magnitude, 100f );
						yield return StartCoroutine( RunningOrders(1f, null) );
					}
					*/
					
				}
				else {
					status = Status.Completed;
					yield return null;
				}
			}
			/*
			else {
				PushOrder( EOrder.None );
				yield return StartCoroutine( RunningOrders(1f, null ) );
			}
			*/
		}
		
		public override void Activate () {
			base.Activate ();
			
			status = Status.Active;
			
			RemoveAllSubgoals();
			
			mAnimator.SetInteger ( AnimatorID.MovingDirection, 0 );
			
			mPorteria = GameObject.FindGameObjectWithTag ("Porteria");
			
			if ( mData.Marcaje == null ) {
				SoccerData dataTarget = CalculateEnemyNearest();
				if ( dataTarget ) {
					ZonaAtaque zona = dataTarget.Entity.GetComponentInChildren<ZonaAtaque>();
					if ( zona.HasPositionFree() ) {
						mData.Marcaje = dataTarget.Entity;
					}
				}
			}
			
			Target = mData.Marcaje;
			
			mSubstate = SubState.ApproachToTarget;
			
			StartCoroutine ( FSM() );
			
			//Debug.Log ( ToString () + " Balon: " + balon.ToString() );
		}
		
		public override Status Process () {
			ActivateIfInactive();
			
			if ( isActive () ) {
				/*Status statusSubgoals =*/ ProcessSubgoals();
				
				status = Status.Completed;
			
				ReactivateIfFailed();
			}
			
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();
		}
	}
	
}