using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class PresionSobreContrario_Goal : GoalPipe {
		
		private SoccerData mData;
		private SoccerMotor mMotor;
		private BallMotor mBalonMotor;
		
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
					else {
						mZonaAtaque = null;
					}
				}
			}
		}
		
		// Steers
		private ArriveSteer moveSteer;
	
		enum SubState {
			OrientToTarget,
			ApproachBall,
			
			ApproachToTarget
		};
		private SubState mSubstate;
		
		public static new PresionSobreContrario_Goal New( GameObject _owner ) {
			PresionSobreContrario_Goal goal = Create<PresionSobreContrario_Goal>(_owner);
			goal.Set();
			return goal;
		}
			
		protected void Set() {
			if ( InitializeGlobal ) {
				mData = gameObject.GetComponent<SoccerData>();
				mMotor = gameObject.GetComponent<SoccerMotor>();
				
				GameObject balon = GameObject.FindGameObjectWithTag ("Balon");
				mBalonMotor = balon.GetComponent<BallMotor>();
				
				moveSteer = gameObject.GetComponent<ArriveSteer>();
			}
		}
		
		private IEnumerator FSM() {
			while (true) {
				yield return StartCoroutine( mSubstate.ToString() );
				yield return null;
			}
		}
		
		IEnumerator updateSteerPosition() {
			while ( moveSteer.enabled && (mSubstate == SubState.ApproachBall) ) {
				
				if ( mZonaAtaque == null )
					break;
				
				mPointAttack = mZonaAtaque.GetAttackPosition( gameObject );
				if ( mPointAttack == null )
					break;
				
				float dist = Helper.ZeroY ( moveSteer.targetPosition - mPointAttack.position ).magnitude;
				
				bool needUpdate = ( mMotor.CurrentSpeed < 1f ) ? ( dist > 1f ) : ( dist > 0.2f );
				if ( needUpdate )
					moveSteer.targetPosition = mPointAttack.position;
				
				// yield return new WaitForSeconds(0.5f);
				yield return null;
			}
		}
		
		private IEnumerator ApproachBall() {
			while ( mBalonMotor.NewPropietary == null ) {
				yield return null;
			}
			
			if ( (mBalonMotor.NewPropietary != null) && (mPointAttack != null) ) {
				/*data.Aim =*/ Target = mBalonMotor.NewPropietary;
				mData.Aim = mBalonMotor.gameObject;
				
				moveSteer.enabled = true;
				moveSteer.Target = null;
				StartCoroutine( updateSteerPosition() );
				
				// Mientras el balón esté cerca y sigamos teniendo un "punto de ataque"
				while ( mData.BallNear && (Target == mBalonMotor.NewPropietary) 
						&& (mPointAttack != null) && (mPointAttack.propietary == gameObject) ) {
					
					if ( mMotor.CurrentSpeed < 2f ) {
						bool near = Helper.InRadio( gameObject, moveSteer.targetPosition, 1f );
						if ( near ) {
							mSubstate = SubState.OrientToTarget;
							break;
						}
					}
					
					yield return null; //new WaitForSeconds(0.5f);
				}
				
				moveSteer.enabled = false;

				if ( mPointAttack != null )
					mPointAttack.propietary = null;
			}
			
			status = Status.Completed;
		}
		
		private IEnumerator OrientToTarget() {
			if ( Target == null ) {
				/*data.Aim =*/ Target = mBalonMotor.NewPropietary;
				mData.Aim = mBalonMotor.gameObject;
			}
			
			while ( Target != null && mZonaAtaque != null ) {
				mPointAttack = mZonaAtaque.GetAttackPosition( gameObject );
				if ( mPointAttack == null ) break;
				
				bool near = Helper.InRadio ( gameObject, mPointAttack.position, 0.5f );
				if ( !near ) {
					mSubstate = SubState.ApproachBall;
					break;
				}
				
				yield return new WaitForSeconds(1f);
			}
			
			if ( mSubstate == SubState.OrientToTarget ) {
				status = Status.Completed;
			}
		}
		
		bool IsPorteriaNear( Vector3 direction ) {
			bool valid = true;
			
			if ( mPorteria != null ) {
				Vector3 nextPosition = transform.position + direction;
				
				float distanceToPorteria = Helper.DistanceInPlaneXZ( mPorteria, gameObject );
				float distanceNextToPorteria = Helper.DistanceInPlaneXZ( mPorteria, nextPosition );
				valid = ( distanceNextToPorteria < distanceToPorteria );
			}
			
			return valid;
		}
		
		private IEnumerator ApproachToTarget () {
			mData.MulSpeed = 0.2f + 0.4f * Random.value;
			
			Target = mBalonMotor.NewPropietary;
			if ( Target == null ) {
				status = Status.Completed;
				yield return null;
			}
			
			GameObject target = Target;
			mData.Aim = target;
			
			if ( mZonaAtaque != null ) {
				mPointAttack = mZonaAtaque.GetAttackPosition( gameObject );
				if ( mPointAttack != null ) {
					float distanceToPorteria = Helper.DistanceInPlaneXZ( mPorteria, gameObject );
					float distanceToTarget = (target.transform.position - transform.position).magnitude;
					
					Vector3 dirPoint = (mPointAttack.position - mZonaAtaque.transform.position);
					Quaternion quatPoint = Quaternion.LookRotation(dirPoint);
					float distancePoint = dirPoint.magnitude;
					
					Vector3 dir = (transform.position - mZonaAtaque.transform.position);
					Quaternion quatDir = Quaternion.LookRotation(dir);
					
					float distance = dir.magnitude;
					if ( distance > 10f || distanceToPorteria < 40f )
						mData.MulSpeed = 0.2f + 0.6f * Random.value;
					else
						mData.MulSpeed = 0.2f + 0.4f * Random.value; 
					
					float angleToPoint = quatPoint.eulerAngles.y - quatDir.eulerAngles.y;
					if ( Mathf.Abs(angleToPoint) > 30f && (distanceToTarget < distancePoint) ) { 
						
						if ( angleToPoint < 0 )
							angleToPoint += 360;
						if ( angleToPoint > 180f && angleToPoint < 350f ) {
							// Helper.Log( Entity, "Right: " + angleToPoint );
							PushOrder( EOrder.MoveRight, target, 2f, 20f );
							yield return StartCoroutine( RunningOrders(1f, null) );
						}
						else if ( angleToPoint > 10f && angleToPoint < 180f ) {
							// Helper.Log( Entity, "Left: " + angleToPoint );
							PushOrder( EOrder.MoveLeft, target, 2f, 20f );
							yield return StartCoroutine( RunningOrders(1f, null) );
						}
						else {
							/*
							PushOrder( EOrder.Approach, target, 6f, 20f );
							yield return StartCoroutine( RunningOrders(2f, null) );
							*/
							yield return null;
						}
						// Helper.Log( Entity, "Angle: " + angleToPoint );
					}
					else {
						PushOrder( EOrder.Approach, target, dirPoint.magnitude, 20f );
						yield return StartCoroutine( RunningOrders(1f, null) );
					}
					
				}
				else {
					status = Status.Completed;
					yield return null;
				}
			}
			else {
				PushOrder( EOrder.None );
				yield return StartCoroutine( RunningOrders(1f, null ) );
			}
			
			/*
			PushOrder( EOrder.MoveLeft, target, 5f, 20f );
			yield return StartCoroutine( RunningOrders(2f, null) );
			*/
			
			/*
			PushOrder( EOrder.Approach, target, 5f, 20f );
			yield return StartCoroutine( RunningOrders(3f, null) );
			
			Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.MoveLeft, transform.position );
			bool valid = IsPorteriaNear (nextDirection);
			if ( valid ) {
				PushOrder( EOrder.MoveLeft, target, 5f, 20f );
				yield return StartCoroutine( RunningOrders(2f, null) );
			}
			
			if ( !valid ) {
				nextDirection = NextDirection(target, GoalPipe.EOrder.MoveRight, transform.position );
				valid = IsPorteriaNear (nextDirection);
				if ( valid ) {
					PushOrder( EOrder.MoveRight, target, 5f, 20f );
					yield return StartCoroutine( RunningOrders(2f, null) );
				}
			}

			if ( !valid ) {
				nextDirection = NextDirection(target, GoalPipe.EOrder.MoveAway, transform.position );
				valid = IsPorteriaNear (nextDirection);
				if ( valid ) {
					PushOrder( EOrder.MoveAway, target, 1f, 3f );
					yield return StartCoroutine( RunningOrders(2f, null) );
				}
			}
			*/
			
			/*
			PushOrder( EOrder.None );
			yield return StartCoroutine( RunningOrders(1f, null ) );
			*/
		}
		
		public override void Activate () {
			base.Activate ();
			
			status = Status.Active;
		
			// REVIEW: Limitar la velocidad?
			mData.MulSpeed = 0.8f;
			
			RemoveAllSubgoals();
		
			mAnimator.SetInteger ( AnimatorID.MovingDirection, 0 );
			
			mPorteria = GameObject.FindGameObjectWithTag ("Porteria");
			
			mSubstate = SubState.OrientToTarget;
			// mSubstate = SubState.ApproachToTarget;
			StartCoroutine ( FSM() );			
			//Debug.Log ( ToString () + " Balon: " + balon.ToString() );
		}
		
		public override Status Process () {
			ActivateIfInactive();
			
			if ( isActive () ) {
				/*Status statusSubgoals =*/ //ProcessSubgoals();
				
				// status = Status.Completed;
			
				ReactivateIfFailed();
			}
			
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();
			
			moveSteer.enabled = false;

			StopAllCoroutines();
			
			// Helper.Log ( Entity, "Presion Terminate" );
		}
	}
	
}
