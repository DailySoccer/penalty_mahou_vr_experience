using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class ArriveToPositionSteer : ISteer {
		
		public Vector3 targetPosition;
		public float MaxAcceleration = 1;
		public float TargetRadius = 0.3f;
		public float SlowRadius = 1;
		public float TimeToTarget = 0.1f;
		
		SteeringInfo steering = new SteeringInfo();
		protected SoccerMotor motor;
		
		void Start() {
			motor = gameObject.GetComponentInChildren<SoccerMotor>();
		}
		
		public override SteeringInfo getSteering() {
			if ( motor == null )
				motor = gameObject.GetComponentInChildren<SoccerMotor>();
			
			// SteeringInfo steering = new SteeringInfo();
			steering.linear = Vector3.zero;
			
			Vector3 direction = Helper.ZeroY( targetPosition - gameObject.transform.position );
			float distance = direction.magnitude;
			
			// Helper.Log ( ()=> { return transform.parent.gameObject.name == "Local9"; }, "Distance: " + distance );
			
			// Comprobar si estamos en el destino
			if ( distance < TargetRadius ) {
				return steering;
			}
			
			float targetSpeed = 0f;
			
			// Si estamos fuera del SlowRadius, iremos a la máxima velocidad
			/*
			if ( distance > SlowRadius ) {
				targetSpeed = motor.MaxSpeed;
			}
			else {
				targetSpeed = motor.MaxSpeed * distance / SlowRadius;
			}
			*/
			targetSpeed = ( distance < 1f ) ? distance * 0.9f : distance * 2f;
			if ( targetSpeed > MaxAcceleration ) 
				targetSpeed = MaxAcceleration;
			
			// Reducir la velocidad si necesitamos un "cambio de direccion"
			Vector3 orientToDir = Quaternion.Inverse(transform.rotation) * direction;
			float angleDiff = Mathf.Atan2(orientToDir.x, orientToDir.z) * Mathf.Rad2Deg;
			
			angleDiff = Mathf.Abs ( angleDiff );
			if ( angleDiff > 135f ) {
				targetSpeed *= 0.3f;
			} 
			else if ( angleDiff > 90f ) {
				targetSpeed *= 0.6f;
			}
			else if ( angleDiff > 25f ) {
				targetSpeed *= 0.8f;
			}
			
			
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
		
		/*
		void OnDrawGizmos () {
			Gizmos.DrawLine( transform.position + Vector3.up, targetPosition + Vector3.up );
			Gizmos.DrawSphere( targetPosition + Vector3.up, 0.1f );
		}
		*/			
	}
	
}
