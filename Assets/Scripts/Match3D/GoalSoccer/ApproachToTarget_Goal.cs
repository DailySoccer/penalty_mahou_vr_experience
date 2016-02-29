using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class ApproachToTarget_Goal : Goal {
		
		private GameObject target;
		private float distanceToTarget;
		
		public static ApproachToTarget_Goal New( GameObject _owner, GameObject _target, float _distance = 1f ) {
			ApproachToTarget_Goal goal = Create<ApproachToTarget_Goal>(_owner);
			goal.Set( _target, _distance );
			return goal;
		}
		
		protected void Set( GameObject _target, float _distance = 1f ) {
			target = _target;
			distanceToTarget = _distance;
		}
	
		public override void Activate () {
			status = Status.Active;
			
			SeekSteer steer = gameObject.GetComponentInChildren<SeekSteer>();
			steer.enabled = true;
			steer.Target = target;
			
			LookWhereYoureGoingSteer rotationSteer = gameObject.GetComponentInChildren<LookWhereYoureGoingSteer>();
			rotationSteer.enabled = true;		
			
			// Debug.Log ( "Activate: " + ToString () );
		}
		
		public override Status Process () {
			ActivateIfInactive();
	
			float distance = Helper.DistanceInPlaneXZ( gameObject, target.transform.position );
			if ( distance <= distanceToTarget ) {
				status = Status.Completed;
			}
			/*
			else {
				status = ProcessSubgoals();
			}
			*/
			
			ReactivateIfFailed();
			
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();
			
			SeekSteer steer = gameObject.GetComponentInChildren<SeekSteer>();
			steer.enabled = false;
			
			LookWhereYoureGoingSteer rotationSteer = gameObject.GetComponentInChildren<LookWhereYoureGoingSteer>();
			rotationSteer.enabled = false;
			
			status = Status.Inactive;
			
			// Debug.Log ( "Terminate: " + ToString () );
		}	
	}
	
}
