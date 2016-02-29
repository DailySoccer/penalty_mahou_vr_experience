using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class MoveTo_Goal : Goal {
		
		private Vector3 	position;
		private Vector3 	direction;
		private SoccerMotor motor;
			
		public static MoveTo_Goal New( GameObject _owner, Vector3 _position ) {
			MoveTo_Goal goal = _owner.AddComponent<MoveTo_Goal>();
			goal.Set( _position );
			return goal;
		}
			
		protected void Set( Vector3 _position ) {
			position = _position;
			
			direction = position - gameObject.transform.position;
			direction.y = 0;
			direction.Normalize();
		}
		
		public override void Activate () {
			status = Status.Active;
			
			// Debug.Log ( "Activate: " + ToString () );
		}
		
		public override Status Process () {
			ActivateIfInactive();
			
			float distance = Helper.DistanceInPlaneXZ( gameObject, position );
			if ( distance < 0.3f ) {
				status = Status.Completed;
			}
			
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();
		}	
	}
	
}
