using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
public class AlignSteer : ISteer {

	public GameObject Target;
	public float MaxAngularAcceleration = 5f;
	public float TargetRadius = 5f;
	public float SlowRadius = 10f;
	public float TimeToTarget = 0.1f;
	
	public override SteeringInfo getSteering() {
		SteeringInfo steering = new SteeringInfo();
		
		steering.angular = 0;
		return steering;
	}
}
	
}

