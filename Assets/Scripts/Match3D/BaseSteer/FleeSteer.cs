using UnityEngine;
using System.Collections;


namespace FootballStar.Match3D {
	
public class FleeSteer : ISteer {

	public GameObject Target;
	public float MaxAcceleration = 1;
	
	public SteeringInfo getSteering(Vector3 targetPosition) {
		SteeringInfo steering = new SteeringInfo();
		
		steering.linear = Helper.ZeroY( gameObject.transform.position - targetPosition );
		
		steering.linear.Normalize();
		steering.linear *= MaxAcceleration;
		
		steering.angular = 0;
		return steering;
	}
	
	public override SteeringInfo getSteering() {
		return getSteering( Target.transform.position );
	}
}
	
}
