using UnityEngine;
using System.Collections;


namespace FootballStar.Match3D {
	
public class SeekPositionSteer : ISteer {
	public Vector3 TargetPosition;
	public float MaxAcceleration = 1;
	//public float distance = 0;
	
	void Start() {
	}
	
	public override SteeringInfo getSteering() {
		SteeringInfo steering = new SteeringInfo();
		
		Vector3 dir = TargetPosition - gameObject.transform.position;
		//distance = dir.magnitude;
		
		steering.linear = Helper.ZeroY( dir );
		
		steering.linear.Normalize();
		steering.linear *= MaxAcceleration;
		
		steering.angular = 0;
		return steering;
	}
}
	
}
