using UnityEngine;
using System.Collections;

namespace FootballStar.Common {
	
	public class BlobShadowTracker : MonoBehaviour {
	
		public float Height = 0.05f;
		
		void Start () 
		{		
		}
		
		void OnBecameVisible() {
			mVisible = true;
		}
		
		void OnBecameInvisible() {
			mVisible = false;
		}
		
		void LateUpdate () {
			if (mVisible) {
				this.transform.rotation = Quaternion.Euler(270, 176, 0);
				var worldPos = this.transform.parent.position;
				worldPos.y = Height;
				this.transform.position = worldPos;
			}			
		}
		
		private bool mVisible = true;
	}
}
