using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {
	
	public class GoalPipe : GoalComposite {
		
		public delegate bool ConditionBool ();
		
		protected enum EResult {
			Waiting,
			Success,
			Failed
		};
		
		protected enum EMoveDirection {
			Stop,
			Forward,
			Backward,
			Left,
			Right
		};
		
		protected enum EOrder {
			None,
			Approach,
			MoveAway,
			MoveLeft,
			MoveRight
		};
		
		protected EResult Result;
		
		protected EOrder Order;
		protected GameObject OrderTarget;
		protected float MinDistance = 1f;
		protected float MaxDistance = 100f;
		
		protected EMoveDirection mMoveDirection;
		protected Vector3 mCurrentDirection;
		
		protected Animator mAnimator;
		private SeekSteer mMoveSteer;
		
		public new static GoalPipe New (GameObject _owner) {
			GoalPipe goal = _owner.AddComponent<GoalPipe>();
			return goal;
		}
		
		public override void Activate () {
			base.Activate ();
			mMoveSteer = gameObject.GetComponentInChildren<SeekSteer>();
			mAnimator = Entity.GetComponent<Animator>();
		}
			
		public override void Terminate ()
		{
			base.Terminate ();
			mMoveSteer.enabled = false;
		}
		
		protected bool ConditionsBase () {
			bool conditions = true;
			
			if ( OrderTarget != null ) {
				float distance = Helper.DistanceInPlaneXZ( gameObject, OrderTarget );
				
				conditions = (Result == EResult.Waiting) && (distance >= MinDistance) && (distance <= MaxDistance);
			}
			
			return conditions;
		}
		
		/*
		 * Order = Approach
		 * Target = Soccer-Local6
		 * MinDistance = 1
		 * MaxDistance = 3
		 * 
		 * */
		protected void PushOrder ( EOrder order ) {
			Order = order;
			OrderTarget = null;
		}
		
		protected void PushOrder ( EOrder order, GameObject target, float minDistance, float maxDistance ) {
			Order = order;
			OrderTarget = target;
			MinDistance = minDistance;
			MaxDistance = maxDistance;
			
			// Helper.Log ( Entity, order.ToString() );
		}
		
		void UpdateAnimation () {
			switch ( Order ) {
				
			case EOrder.None: {
				mAnimator.SetInteger ( AnimatorID.MovingDirection, 0 );
			}
			break;
				
			case EOrder.Approach: {
				mAnimator.SetInteger ( AnimatorID.MovingDirection, 1 );
			}
			break;

			case EOrder.MoveAway: {
				mAnimator.SetInteger ( AnimatorID.MovingDirection, 2 );
			}
			break;
				
			case EOrder.MoveLeft: {
				mAnimator.SetInteger ( AnimatorID.MovingDirection, 3 );
			}
			break;
				
			case EOrder.MoveRight: {
				mAnimator.SetInteger ( AnimatorID.MovingDirection, 4 );
			}
			break;
				
			}
			
		}
		
		protected Vector3 NextDirection( GameObject target, EOrder order, Vector3 position ) {
			Vector3 next = Vector3.zero;
			
			if ( target != null ) {
				Vector3 targetPosition = target.transform.position;
				Vector3 toTarget = Helper.ZeroY( targetPosition - position );
				Vector3 dirToTarget = (toTarget).normalized;
				
				switch ( order ) {
					
				case EOrder.Approach: {
					next = dirToTarget;
				}
				break;
	
				case EOrder.MoveAway: {
					next = -dirToTarget;
				}
				break;
					
				case EOrder.MoveLeft: {
					next = Quaternion.Euler(0, -90f, 0) * dirToTarget;
				}
				break;
					
				case EOrder.MoveRight: {
					next = Quaternion.Euler(0, 90f, 0) * dirToTarget;
				}
				break;
					
				}
			}
			
			return next;
		}
		
		EResult IsDistanceValid ( Vector3 position ) {
			EResult positionResult = EResult.Waiting;
			
			float distance = Helper.DistanceInPlaneXZ( OrderTarget, position );
			
			bool near = (distance < MinDistance);
			bool far = (distance > MaxDistance);
			if ( near || far ) {
				
				switch ( Order ) {
					
				case EOrder.Approach: {
					positionResult = near ? EResult.Success : EResult.Failed;
				}
				break;
	
				case EOrder.MoveAway: {
					positionResult = far ? EResult.Success : EResult.Failed;
				}
				break;
					
				case EOrder.MoveLeft: {
					positionResult = EResult.Failed;
				}
				break;
					
				case EOrder.MoveRight: {
					positionResult = EResult.Failed;
				}
				break;
					
				}
				
			}
			
			return positionResult;
		}
		
		static int frameCount = 0;
		static int numOrdersInFrame = 0;
		static int maxOrdersInFrame = 100;
		
		bool CanApplyOrder() {
			if ( frameCount != Time.frameCount ) {
				frameCount = Time.frameCount;
				numOrdersInFrame = 0;
			}
			
			return ( numOrdersInFrame++ < maxOrdersInFrame );
		}

		void ApplyOrder () {
			if ( (OrderTarget == null) || !CanApplyOrder () ) 
				return;
			
			mCurrentDirection = NextDirection ( OrderTarget, Order, transform.position );
			
			EResult positionResult = IsDistanceValid( transform.position + mCurrentDirection );
			if ( positionResult == EResult.Waiting ) {
				mMoveSteer.TargetPosition = transform.position + mCurrentDirection * 10f;
			}
			else {
				Result = positionResult;
			}
			
			UpdateAnimation();
		}
		
		protected IEnumerator RunningOrders ( float delayTime, ConditionBool additionalConditions ) {
			Result = EResult.Waiting;
			
			float endTime = Time.time + delayTime;
			
			mMoveSteer.enabled = (Order != EOrder.None);
			mMoveSteer.Target = null;
			
			mCurrentDirection = NextDirection ( OrderTarget, Order, transform.position );
			
			while ( (Time.time <= endTime) && ConditionsBase() && (additionalConditions == null || additionalConditions()) ) {
				while ( !CanApplyOrder() ) {
					yield return null;
				}
				
				ApplyOrder ();
				yield return new WaitForSeconds( 0.1f );
			}
			
			if ( Result == EResult.Waiting ) {
				Result = EResult.Success;
			}
			
			mMoveSteer.enabled = false;
		}
	}
	
}
