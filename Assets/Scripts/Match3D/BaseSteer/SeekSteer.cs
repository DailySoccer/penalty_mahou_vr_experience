using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class SeekSteer : SeekPositionSteer {
	
		public GameObject Target;
		
		public override SteeringInfo getSteering() {
			if ( Target != null ) {			
				base.TargetPosition = Target.transform.position;
			}
			return base.getSteering();
		}
	}
	
}

