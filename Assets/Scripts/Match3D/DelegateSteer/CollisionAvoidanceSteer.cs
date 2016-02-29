using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {
	
	public class CollisionAvoidanceSteer : ISteer {
			
		// Hold a list of potential targets
		public List<SoccerMotor> Targets = new List<SoccerMotor>();
		
		// Hold the maximum acceleration
		public float MaxAcceleration = 1f;
		
		// Holds the collision radius of a character (we assume all characters have the same radius here)
		public float Radius = 1f;
	
		protected SoccerMotor motor;
		
		private SteeringInfo steering;
		
		void Start() {
			motor = gameObject.GetComponentInChildren<SoccerMotor>();
		}
		
		Vector3 AbsV(Vector3 v) {
			v.x = Mathf.Abs ( v.x );
			v.y = Mathf.Abs ( v.y );
			v.z = Mathf.Abs ( v.z );
			return v;
		}
		
		public override SteeringInfo getSteering() {
			steering = new SteeringInfo();
			
			// 1. Find the target that's closest to collision
			
			// Store the first collision time
			float shortestTime = 5f;
			
			// Store the target that collides then, and other data that we will need and can avoid recalculating
			SoccerMotor firstTarget = null;
			float firstMinSeparation = 0;
			//float firstDistance = 0;
			Vector3 firstRelativePos = Vector3.zero;
			Vector3 firstRelativeVel = Vector3.zero;
			
			if ( motor.CurrentVelocity.magnitude <= 0.1f )
				return steering;
				
			// Loop through each target
			foreach( SoccerMotor target in Targets ) {
				
				// Calculate the time to collision
				Vector3 relativePos = Helper.ZeroY ( target.transform.position - transform.position );
				relativePos = AbsV( relativePos );
				
				Vector3 relativeVel = Helper.ZeroY ( target.CurrentVelocity - motor.CurrentVelocity );
				relativeVel = AbsV( relativeVel );
				
				float relativeSpeed = relativeVel.magnitude;
				
				float timeToCollision = Vector3.Dot( relativePos, relativeVel ) / ( relativeSpeed * relativeSpeed );
				
				// Check if it is going to be a collision at all
				float distance = relativePos.magnitude;
				float minSeparation = distance - relativeSpeed; // * shortestTime;
				if ( minSeparation > 2 * Radius )
					continue;
				
				// Check if it is the shortest
				if ( timeToCollision > 0 && timeToCollision < shortestTime ) {
					
					// Store the time, target and other data
					shortestTime = timeToCollision;
					firstTarget = target;
					firstMinSeparation = minSeparation;
					//firstDistance = distance;
					firstRelativePos = relativePos;
					firstRelativeVel = relativeVel;
				}
				
				// 2. Calculate the steering
				
				// If we have no target, then exit
				if ( firstTarget == null )
					break;
				
				// If we're going to hit exactly, or if we're already colliding,
				//   then do the steering based on current position
				if ( (firstMinSeparation <= 0) || (distance < 2 * Radius) )
					relativePos = firstTarget.transform.position - transform.position;
				// Otherwise calculate the future relative position
				else
					relativePos = firstRelativePos + firstRelativeVel * shortestTime;
				
				// Avoid the target
				relativePos.Normalize();
				steering.linear = relativePos * MaxAcceleration;
			}
			
			steering.angular = 0;
			
			// Return the steering
			return steering;
		}
		
		void OnDrawGizmos() {
			if ( enabled && (steering != null) && (steering.linear.magnitude > 0) ) {
				Gizmos.color = Color.blue;
				Gizmos.DrawLine( transform.position + Vector3.up, transform.position + steering.linear + Vector3.up );
			}
		}
	}
	
}
