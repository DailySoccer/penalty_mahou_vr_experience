using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class AnimationScript : MonoBehaviour {
		protected bool IsPlaying = false;

		public virtual void Play () {
			Play( null );
		}
			
		public virtual void Play (Hashtable args) {
			Stop ();

			// gameObject.SetActive( true );
			IsPlaying = true;
		}
		
		public virtual void Stop () {
			if ( IsPlaying ) {
				StopAllCoroutines();
				
				// gameObject.SetActive( false );
				IsPlaying = false;
			}
		}
	}
	
}
