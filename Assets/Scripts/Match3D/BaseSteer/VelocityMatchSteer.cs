using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
public class VelocityMatchSteer : ISteer {

	public GameObject Target;
	public float MaxAcceleration = 1;
	public float TargetRadius = 0.3f;
	public float SlowRadius = 1;
	public float TimeToTarget = 0.1f;
	
	SoccerMotor motor;
	SoccerMotor targetMotor;
	
	void Start() {
		motor = gameObject.GetComponentInChildren<SoccerMotor>();
		targetMotor = Target.GetComponentInChildren<SoccerMotor>();
	}
	
	public override SteeringInfo getSteering() {
		SteeringInfo steering = new SteeringInfo();
		
		steering.linear = targetMotor.Velocity - motor.Velocity;
		steering.linear /= TimeToTarget;
		
		if ( steering.linear.magnitude > MaxAcceleration ) {
			steering.linear.Normalize();
			steering.linear *= MaxAcceleration;
		}
		
		steering.angular = 0;
		return steering;
	}
}
	
}
