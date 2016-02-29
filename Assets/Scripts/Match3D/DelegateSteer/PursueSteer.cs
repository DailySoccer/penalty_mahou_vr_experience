using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class PursueSteer : ArriveToPositionSteer {
		public float MaxPrediction = 1;
		
		public GameObject Target;
		SoccerMotor targetMotor;
		
		void Start() {
			motor = gameObject.GetComponentInChildren<SoccerMotor>();
			targetMotor = Target.GetComponentInChildren<SoccerMotor>();
		}
		
		public override SteeringInfo getSteering() {
			if ( motor == null )
				motor = gameObject.GetComponentInChildren<SoccerMotor>();
			targetMotor = Target.GetComponentInChildren<SoccerMotor>();
			Assert.Test ( () => { return targetMotor != null; }, "GameObject sin componente SoccerMotor" );
				
			Vector3 direction = Helper.ZeroY( Target.transform.position - gameObject.transform.position );
			float distance = direction.magnitude;
			
			float speed = motor.CurrentVelocity.magnitude;
			float prediction = 0;
			
			if ( speed <= (distance / MaxPrediction) )
				prediction = MaxPrediction;
			else
				prediction = distance / speed;
			
			base.targetPosition = targetMotor.transform.position + (targetMotor.CurrentVelocity.normalized * prediction);
			return base.getSteering();
		}
	}
	
}
