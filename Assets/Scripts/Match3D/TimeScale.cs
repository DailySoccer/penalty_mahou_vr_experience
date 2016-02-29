using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class TimeScale : MonoBehaviour {
		public float factor = 1f;
		
		void Start () {
		}
		
		void Update () {
            if ( Time.timeScale != factor ) {
				Time.timeScale = factor;
			}
		}
	}
	
}
