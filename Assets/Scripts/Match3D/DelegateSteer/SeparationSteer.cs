using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {
	
	public class SeparationSteer : ISteer {
		
		public List<Transform> Targets = new List<Transform>();
		public float ThresholdDistance = 1f;
		public float DecayCoefficient = 1f;
		public float MaxAcceleration = 1f;
	
		void Start() {
		}
		
		public override SteeringInfo getSteering() {
			SteeringInfo steering = new SteeringInfo();
			
			foreach( Transform target in Targets ) {
				Vector3 direction = target.transform.position - gameObject.transform.position;
				float distance = direction.magnitude;
				if ( distance < ThresholdDistance ) {
					
					float strength = Mathf.Min( DecayCoefficient * distance * distance, MaxAcceleration );
					
					direction.Normalize();
					steering.linear = strength * direction;
					break;
				}
			}
			
			steering.angular = 0;
			return steering;
		}
	}
	
}
