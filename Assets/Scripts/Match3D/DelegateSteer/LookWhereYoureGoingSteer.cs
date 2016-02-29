using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
public class LookWhereYoureGoingSteer : ISteer {
	
	// SoccerMotor motor;
	SteeringInfo steering = new SteeringInfo();
	
	void Start() {
		// motor = transform.parent.gameObject.GetComponentInChildren<SoccerMotor>();
	}
	
	public override SteeringInfo getSteering() {
		// SteeringInfo steering = new SteeringInfo();
		steering.angular = 0;
		
		/*
		if ( motor.Velocity.magnitude == 0 )
			return steering;
		
		steering.angular = Mathf.Atan2 (-motor.Velocity.x, motor.Velocity.z);
		*/
		
		steering.linear = Vector3.zero;
		return steering;
	}

}
	
}
