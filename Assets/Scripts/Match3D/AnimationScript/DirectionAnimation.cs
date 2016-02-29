using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class DirectionAnimation : AnimationScript {

		UITexture mBackgroundTexture;
		UITexture mHandTexture;
		UITexture mArrowTexture;
		
		Vector3 mHandPosition;
		
		void Awake () {
			mBackgroundTexture = Helper.GetComponent<UITexture>( transform, "Background" );
			mHandTexture = Helper.GetComponent<UITexture>( transform, "Hand" );
			mArrowTexture = Helper.GetComponent<UITexture>( transform, "Arrow" );
			
			SaveState();
			
			Stop();
		}
		
		void RestoreState() {
			mHandTexture.transform.localPosition = mHandPosition;
		}
		
		void SaveState() {
			mHandPosition = mHandTexture.transform.localPosition;
		}

		IEnumerator AnimationPlay() {
			const float moveTime = 0.5f;
			Vector3 move = mArrowTexture.transform.localRotation * new Vector3( 0, 150f, 0 );
			
			while( true ) {
				RestoreState();

				mHandTexture.transform.localPosition = mHandPosition - move * 0.5f;

				float timer = Time.realtimeSinceStartup;
				yield return StartCoroutine( Helper.WaitCondition( () => ( Time.realtimeSinceStartup - timer >= 0.2f ) ) );
				// yield return new WaitForSeconds( 0.2f );

				timer = Time.realtimeSinceStartup;
				while ( Time.realtimeSinceStartup - timer < moveTime ) {
					float lerpTime = (Time.realtimeSinceStartup - timer) * (1f/moveTime);
					mHandTexture.transform.localPosition = Vector3.Lerp( mHandPosition - move * 0.5f, mHandPosition + move * 0.5f, lerpTime );
					yield return null;
				}
			}

			// Stop();
		}
		
		public override void Play (Hashtable args) {
			base.Play (args);
			
			float angle = 0f;
			
			if ( args != null ) {
				if ( args.Contains( "angle" ) ) {
					angle = (float)args["angle"];
				}
			}
			
			// Orientar la flecha hacia el objetivo (en coords. de pantalla)
			Quaternion rotArrow = transform.localRotation;
			rotArrow = Quaternion.AngleAxis( angle, new Vector3(0,0,-1) );
			mArrowTexture.transform.localRotation = rotArrow;

			mBackgroundTexture.gameObject.SetActive( true );
			mHandTexture.gameObject.SetActive( true );
			mArrowTexture.gameObject.SetActive( true );
			
			StartCoroutine( AnimationPlay() );
		}
		
		public void StopTweens ( GameObject go ) {
			AMTween tweenActivo = go.GetComponent<AMTween>();
			if ( tweenActivo ) {
				Destroy( tweenActivo );
			}
		}
		
		public override void Stop () {
			base.Stop ();

			if ( mBackgroundTexture != null ) {
				mBackgroundTexture.gameObject.SetActive( false );
			}

			if ( mHandTexture != null ) {
				StopTweens( mHandTexture.gameObject );
				mHandTexture.gameObject.SetActive( false );
			}

			if ( mArrowTexture != null ) {
				StopTweens( mArrowTexture.gameObject );
				mArrowTexture.gameObject.SetActive( false );
			}
		}
	}
	
}
