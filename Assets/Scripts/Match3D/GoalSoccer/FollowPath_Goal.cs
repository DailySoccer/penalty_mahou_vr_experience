using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class FollowPath_Goal : GoalComposite {
		
		SoccerData data;
		SoccerMotor motor;
		GameObject balon;
			
		FollowPathSteer arriveSteer;
		LookWhereYoureGoingSteer rotationSteer;
		
		public static new FollowPath_Goal New( GameObject _owner ) {
			FollowPath_Goal goal = Create<FollowPath_Goal>(_owner);
			goal.Set();
			return goal;
		}
			
		protected void Set() {
			if ( InitializeGlobal ) {
				data = gameObject.GetComponent<SoccerData>();
				motor = gameObject.GetComponent<SoccerMotor>();
				
				balon = GameObject.FindGameObjectWithTag ("Balon");
			}
		}
		
		public override void Activate () {
			status = Status.Active;
			
			RemoveAllSubgoals();
			
			// data.MulSpeed = 0.7f;
			
			arriveSteer = gameObject.GetComponent<FollowPathSteer>();
			arriveSteer.enabled = true;
			arriveSteer.Target = data.dummyMirror;
			
			rotationSteer = gameObject.GetComponent<LookWhereYoureGoingSteer>();
			rotationSteer.enabled = true;
			
			data.Aim = balon;
			
			data.FollowingPath = true;
			
			motor.ForcedZeroY = true;
		}
		
		public override Status Process () {
			
			ActivateIfInactive();
			
			/*
			Animator animator = Entity.GetComponentInChildren<Animator>();
			if ( animator ) {
				if ( animator.IsInTransition(0) ) {
					AnimatorTransitionInfo transition = animator.GetAnimatorTransitionInfo(0);
					if ( transition.IsUserName( "Idle" ) ) {
						animator.MatchTarget(arriveSteer.targetPosition, Quaternion.identity, AvatarTarget.Root, 
							new MatchTargetWeightMask(new Vector3(1, 0, 1), 0), 0f, 1.0f);
					}
				}
			}
			*/
			
			/*
			if ( isActive () ) {
				status = ProcessSubgoals();
			
				ReactivateIfFailed();
			}
			*/
			
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();
			
			arriveSteer.enabled = false;
			rotationSteer.enabled = false;
			
			data.FollowingPath = false;
			
			motor.ForcedZeroY = false;
		}
		
		/*
		void OnDrawGizmos () {
			Gizmos.color = Color.red;
			Gizmos.DrawLine( transform.position + Vector3.up, arriveSteer.targetPosition + Vector3.up );
		}			
		*/
	}
	
}
