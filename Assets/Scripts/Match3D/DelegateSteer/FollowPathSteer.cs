using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class FollowPathSteer : ArriveToPositionSteer {
		
		public GameObject Target;
		Entrenador mEntrenador = null;
		
		SteeringInfo steering = new SteeringInfo();
		Vector3 mEndPosition = Vector3.zero;
		bool mEndOfPath = false;
		
		public override SteeringInfo getSteering() {
			if ( motor == null )
				motor = gameObject.GetComponentInChildren<SoccerMotor>();
			if ( mEntrenador == null ) {
				mEntrenador = Target.GetComponentInChildren<Entrenador>();
			}
			
			// SteeringInfo steering = new SteeringInfo();
			steering.linear = Vector3.zero;
			
			Vector3 nextPosition = mEntrenador.GetPositionAfter( 1f );
			
			if ( mEndOfPath && (mEndPosition - nextPosition).sqrMagnitude < 0.5f ) {
				return steering;
			}
			
			mEndOfPath = false;
			
			Vector3 dirToNextPosition = Helper.ZeroY( nextPosition - transform.position );
			float distToNext = dirToNextPosition.magnitude;
			
			// Helper.Log ( ()=> { return transform.parent.gameObject.name == "Local9"; }, "Distance: " + distance );
			
			/*
			if ( transform.parent.name == "Soccer-Local5" ) {
				Debug.Log( "DistToNext: " + distToNext );
			}
			*/
			
			// Comprobar si estamos en el destino
			if ( distToNext < TargetRadius ) {
				mEndOfPath = true;
				mEndPosition = nextPosition;
				return steering;
			}

			Vector3 direction = Helper.ZeroY( mEntrenador.transform.position - transform.position );
			float distance = direction.magnitude;

			Vector3 dirBetweenPositions = Helper.ZeroY( nextPosition - mEntrenador.transform.position );
			float distBetweenPositions = dirBetweenPositions.magnitude;
			
			// Si nos acercamos demasiado al dummy o estamos mas cerca del "dummy futuro" que del dummy actual
			if ( (distance < 0.3f) || ( distToNext < distBetweenPositions ) ) {
				// REVIEW: Nos dirigimos al dummy "mas cercano en el futuro"
				Vector3 nextPositionNear = mEntrenador.GetPositionAfter( 0.3f );
				Vector3 dirToNextPositionNear = Helper.ZeroY( nextPositionNear - transform.position );
				direction = dirToNextPositionNear;
			}
			
			float targetSpeed = distToNext * 0.9f;
			if ( distToNext < SlowRadius ) {
				targetSpeed = distToNext * 0.4f;
			}
			
			if ( targetSpeed > MaxAcceleration )
				targetSpeed = MaxAcceleration;
			
			Vector3 targetVelocity = direction;
			targetVelocity.Normalize();
			targetVelocity *= targetSpeed;
			
			/*
			steering.linear = targetVelocity - motor.Velocity;
			steering.linear /= TimeToTarget;
			*/
			
			steering.linear = targetVelocity;
			
			steering.angular = 0;
			return steering;
		}

	}
	
}
