using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
public class EvadeSteer : FleeSteer {
	public float MaxPrediction = 1;
	
	SoccerMotor motor;
	SoccerMotor targetMotor;
	
	
	void Start() {
		motor = gameObject.GetComponentInChildren<SoccerMotor>();
		targetMotor = Target.GetComponentInChildren<SoccerMotor>();
	}
	
	public override SteeringInfo getSteering() {
		Vector3 direction = Helper.ZeroY( Target.transform.position - gameObject.transform.position );
		float distance = direction.magnitude;
		
		float speed = motor.Velocity.magnitude;
		float prediction = 0;
		
		if ( speed <= (distance / MaxPrediction) )
			prediction = MaxPrediction;
		else
			prediction = distance / speed;
		
		Vector3 targetPosition = targetMotor.Velocity * prediction;
		
		return base.getSteering( targetPosition );
	}
}
	
}
