using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {
	
	public class PointOfInterest {
		
		public GameObject Balon;
		public GameObject Source;
		public GameObject Target;
		
		private BallMotor mBallMotor = null;
		private MatchManager mMatchManager;
		
		public PointOfInterest() {
			Balon = GameObject.FindGameObjectWithTag ("Balon");
			mBallMotor = Balon.GetComponentInChildren<BallMotor>();
			
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
		}
		
		public void Update() {
			if ( mBallMotor ) {
				InteractiveActions actions = mMatchManager.InteractiveActions;
				Source = mBallMotor.NewPropietary;
				Target = actions.Target;
            }
        }
	};
}
