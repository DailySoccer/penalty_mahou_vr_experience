using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class ArriveSteer : ArriveToPositionSteer {
	
		public GameObject Target;
		
		public override SteeringInfo getSteering() {
			if ( Target != null ) {
				base.targetPosition = Target.transform.position;
			}
			return base.getSteering();
		}
	}
	
}
