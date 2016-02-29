using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	public class AnimatorInfo : MonoBehaviour {

		public string CurrentClipName;
		public string NextClipName;

		void Start () {
			mAnimator = GetComponent<Animator>();

			if ( mAnimator == null ) {
				Debug.LogWarning( "AnimatorInfo sin Animator: " + gameObject.name );
				enabled = false;
			}
		}
		
		void LateUpdate () {
			if ( mAnimator ) {
				CurrentClipName = Helper.CurrentAnimationClipNames( mAnimator );
				NextClipName 	= Helper.NextAnimationClipNames( mAnimator );
			}
		}

		private Animator mAnimator;
	}
}

