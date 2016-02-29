using UnityEngine;
using System;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class GizmoRenderer : MonoBehaviour {
		public void TimerReset() { TimerSync = (x) => x + Time.deltaTime; }
		public Func<float, float> TimerSync = (x) => x + Time.deltaTime;

		//public Func<float, bool> IsPerfect = (x) => false;
	}
	
}
