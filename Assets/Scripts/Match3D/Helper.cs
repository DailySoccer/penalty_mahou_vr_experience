using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {
	
	public class Helper {
		
		public static Vector3 ZeroY (Vector3 v) {
			v.y = 0;
			return v;
		}
		
		public static void Log ( Func<bool> test, System.String mensaje) {
			if ( test() )
				Debug.Log (Time.time.ToString("0.00") + " -> " + mensaje);
		}
		
		public static void Log (GameObject gameObject, System.String mensaje) {
			Debug.Log (gameObject.name + ": Tiempo: " + Time.time.ToString("0.00") + " -> " + mensaje);
		}
		
		public static int ParseNumber(string theName) {
		    string b = string.Empty;
		    int val = 0;
		    for (int i=0; i< theName.Length; i++)
		    {
		        if (Char.IsDigit(theName[i]))
		            b += theName[i];
		    }
		    if (b.Length>0)
		        val = int.Parse(b);
			
			return val;
		}
		
		public static float DistanceInPlaneXZ( GameObject obj, Vector3 pos ) {
			Vector3 ori = obj.transform.position;
			Vector3 dst = pos;
			
			ori.y = dst.y = 0;
			return Vector3.Distance( ori, dst );
		}
		
		public static float DistanceInPlaneXZ( GameObject objOri, GameObject objDst ) {
			return Helper.DistanceInPlaneXZ( objOri, objDst.transform.position );
		}	

		public static bool InRadio( Vector3 ori, Vector3 dst, float radio ) {
			// Plano XZ
			ori.y = dst.y = 0;
			
			// Distance Squared
			float dist2 = (dst - ori).sqrMagnitude;
			float radio2 = radio * radio;
			return ( dist2 <= radio2 );
		}
		
		public static bool InRadio( GameObject objOri, Vector3 dst, float radio ) {
			return InRadio ( objOri.transform.position, dst, radio );
		}
		
		public static bool InRadio( GameObject objOri, GameObject objDst, float radio ) {
			return InRadio ( objOri.transform.position, objDst.transform.position, radio );
		}
		
		public static Vector3 CalculateBestThrowSpeed( Vector3 origin, Vector3 target, float timeToTarget ) {
		    // calculate vectors
		    Vector3 toTarget = target - origin;
		    Vector3 toTargetXZ = toTarget;
		    toTargetXZ.y = 0;
		 
		    // calculate xz and y
		    float y = toTarget.y;
		    float xz = toTargetXZ.magnitude;
		 
		    // calculate starting speeds for xz and y. Physics forumulase deltaX = v0 * t + 1/2 * a * t * t
		    // where a is "-gravity" but only on the y plane, and a is 0 in xz plane.
		    // so xz = v0xz * t => v0xz = xz / t
		    // and y = v0y * t - 1/2 * gravity * t * t => v0y * t = y + 1/2 * gravity * t * t => v0y = y / t + 1/2 * gravity * t
		    float t = timeToTarget;
		    float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
		    float v0xz = xz / t;
		 
		    // create result vector for calculated starting speeds
		    Vector3 result = toTargetXZ.normalized;   // get direction of xz but with magnitude 1
		    result *= v0xz;                    // set magnitude of xz to v0xz (starting speed in xz plane)
		    result.y = v0y;                    // set y to v0y (starting speed of y plane)
		 
		    return result;
		}
		
		public static Vector3 CalculateFiringSolution( Vector3 start, Vector3 end, float speed ) {
			// calculate the vector from the target back to the start
			Vector3 delta = start - end;
			Vector3 gravity = Physics.gravity;
			
			// calculate the real-valued a,b,c coefficients of a conventional quadratic equation
			float a = Vector3.Dot(gravity, gravity);
			float b = -4 * ( Vector3.Dot(gravity, delta) + (speed * speed) );
			float c = 4 * Vector3.Dot (delta, delta);
			
			// check for no real solutions
			if ( 4*a*c > b*b) return Vector3.zero;
			
			float time0 = Mathf.Sqrt((-b + Mathf.Sqrt(b*b-4*a*c)) / (2*a));
			float time1 = Mathf.Sqrt((-b - Mathf.Sqrt(b*b-4*a*c)) / (2*a));
			
			float ttt = 0;
			
			// find the time to target
			if ( time0 < 0 ) {
				if ( time1 < 0 )
					return Vector3.zero;
				else
					ttt = time1;
			}
			else {
				if ( time1 < 0 )
					ttt = time0;
				else
					ttt = Mathf.Min(time0, time1);
			}
			
			// return the firing vector
			return (2 * delta - gravity * ttt*ttt) / (2 * speed * ttt);
		}	
		
		public static IEnumerator WaitCondition( Func<bool> test, float timeMax=1f ) {
			while ( !test() /*&& (timeMax > 0f)*/ ) {
				yield return null;
				timeMax -= Time.deltaTime;
			}
		}
		
		public static bool IsAnimationPlaying( Animator animator, int animationHash ) {
			bool playing = false;
			
			AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
			playing = (state.tagHash == animationHash);
			
			if ( !playing && animator.IsInTransition(0) ) {
				AnimatorStateInfo stateNext = animator.GetNextAnimatorStateInfo(0);
				playing = (stateNext.tagHash == animationHash);
			}
			
			return playing;
		}
		
		public static IEnumerator WaitAnimation( Animator animator, int animationHash ) {
			while ( true ) {
				if ( IsAnimationPlaying( animator, animationHash ) ) 
					break;
				yield return null;
			}
		}
		
		public static bool IsAnimationPlaying( Animator animator, string animationTag ) {
			return IsAnimationPlaying ( animator, Animator.StringToHash(animationTag) );
		}
		
		public static IEnumerator WaitAnimation( Animator animator, string animationTag ) {
			while ( true ) {
				if ( IsAnimationPlaying( animator, Animator.StringToHash(animationTag) ) ) 
					break;
				yield return null;
			}
		}

		public static IEnumerator WaitAnimationEndTransition( Animator animator ) {
			while ( true ) {
				if ( !animator.IsInTransition(0) ) 
					break;
				yield return null;
			}
		}
		
		public static IEnumerator WaitAnimationTime( Animator animator, string animationTag, float normalizedTime ) {
			while ( true ) {			
				AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
				bool playing = state.IsTag( animationTag );
			
				if ( !playing && animator.IsInTransition(0) ) {
					state = animator.GetNextAnimatorStateInfo(0);
					playing = state.IsTag( animationTag );
				}
				
				if ( !playing )
					break;
				
				if ( state.normalizedTime >= normalizedTime ) {
					// Helper.Log ( animator.gameObject, "Clip: " + AnimationClipNames(animator) + " AnimTime: " + state.normalizedTime + " Time: " + normalizedTime );
					break;
				}
				
				yield return null;
			}
		}

		public static GameObject FindName( string name, GameObject[] gameObjectList ) {
			GameObject gameObject = null;
			foreach( GameObject go in gameObjectList ) {
				if ( go.name == name ) {
					gameObject = go;
					break;
				}
			}
			return gameObject;
		}

    	public static Transform FindTransform( Transform sparent, string sname) {
	        if (sparent.name.Equals(sname)) return sparent;
	
	        foreach (Transform child in sparent){
	            Transform result = FindTransform(child, sname);
	            if (result != null) return result;
    	    }
	        return null;
 		}		
		
		public static T GetComponent<T>( Transform parent, string name ) where T : Component {
			Transform transform = Helper.FindTransform( parent, name );
			return transform.GetComponent<T>();
		}

    	public static T GetParentComponent<T>( Transform sparent ) where T : Component {
			if ( !sparent ) 
				return default(T);
			
			T component = sparent.GetComponent<T>();
			if ( component ) 
				return component;

			return GetParentComponent<T>( sparent.parent );
 		}		
		
		public static string CurrentAnimationClipNames( Animator animator ) {
			string names = "";
			
			AnimatorClipInfo[] animationsInfo = animator.GetCurrentAnimatorClipInfo(0);
			foreach (AnimatorClipInfo info in animationsInfo ) {
				if ( names != "" ) names += ", ";
				names += info.clip.name;
			}

			return names;
		}

		public static string NextAnimationClipNames( Animator animator ) {
			string names = "";
			
			AnimatorClipInfo[] animationsInfo = animator.GetNextAnimatorClipInfo(0);
			if ( animationsInfo.Length > 0 ) {
				foreach (AnimatorClipInfo info in animationsInfo ) {
					if ( names != "" ) names += ", ";
					names += info.clip.name;
				}
			}

			return names;
		}

		public static string AnimationClipNames( Animator animator ) {
			string names = "";

			names += CurrentAnimationClipNames( animator ) + " -> " + NextAnimationClipNames ( animator );

			/*
			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
			names += "( " + (stateInfo.normalizedTime - (int)stateInfo.normalizedTime) + " )";
			*/
			
			return names;
		}
		
		public static IEnumerator LogPlayingAnimations ( GameObject gameObject ) {
			Animator anim = gameObject.GetComponent<Animator>();
			string currentTextInfo = "";
			
			while ( true ) {
				string textInfo = AnimationClipNames( anim );
				
				if ( textInfo != currentTextInfo ) {
					currentTextInfo = textInfo;
					Helper.Log ( gameObject, "Anims: " + currentTextInfo + " Speed: " + anim.GetFloat ("Speed") + " Direction: " + anim.GetFloat("Direction") );
				}
				
				yield return null;
			}
		}
		
		public static Hashtable Hash(params object[] args){
			Hashtable hashTable = new Hashtable(args.Length/2);
			if (args.Length %2 != 0) {
				Debug.LogError("Tween Error: Hash requires an even number of arguments!"); 
				return null;
			}else{
				int i = 0;
				while(i < args.Length - 1) {
					hashTable.Add(args[i], args[i+1]);
					i += 2;
				}
				return hashTable;
			}
		}			
	}

}
