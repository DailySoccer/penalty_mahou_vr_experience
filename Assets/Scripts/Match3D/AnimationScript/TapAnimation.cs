using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class TapAnimation : AnimationScript {

		UITexture mBackgroundTexture;
		UITexture mHandTap1Texture;
		UITexture mHandTap2Texture;
		UITexture mTapsNumberTexture;
		
		int mAmount = 10;
		
		void Awake () {
			mBackgroundTexture = Helper.GetComponent<UITexture>( transform, "Background" );
			mHandTap1Texture = Helper.GetComponent<UITexture>( transform, "HandTap1" );
			mHandTap2Texture = Helper.GetComponent<UITexture>( transform, "HandTap2" );
			mTapsNumberTexture = Helper.GetComponent<UITexture>( transform, "TapsNumber" );

			Stop();
		}
		
		IEnumerator AnimationPlay() {
			float timer = 0f;
			while ( true ) {
				for ( int i=0; i<mAmount; i++ ) {

					timer = Time.realtimeSinceStartup;
					yield return StartCoroutine( Helper.WaitCondition( () => ( Time.realtimeSinceStartup - timer >= 0.1f ) ) );
					// yield return new WaitForSeconds( 0.1f );
					
					mHandTap1Texture.gameObject.SetActive( false );
					mHandTap2Texture.gameObject.SetActive( true );
					
					timer = Time.realtimeSinceStartup;
					yield return StartCoroutine( Helper.WaitCondition( () => ( Time.realtimeSinceStartup - timer >= 0.1f ) ) );
					// yield return new WaitForSeconds( 0.1f );

					mHandTap1Texture.gameObject.SetActive( true );
					mHandTap2Texture.gameObject.SetActive( false );
				}
				
				timer = Time.realtimeSinceStartup;
				yield return StartCoroutine( Helper.WaitCondition( () => ( Time.realtimeSinceStartup - timer >= 0.5f ) ) );
				// yield return new WaitForSeconds( 0.5f );
			}
			
			// Stop();
		}
		
		public override void Play (Hashtable args) {
			base.Play (args);

			if ( args != null ) {
				if ( args.Contains( "amount" ) ) {
					mAmount = (int)args["amount"];
				}
			}

			mBackgroundTexture.gameObject.SetActive( true );
			mHandTap1Texture.gameObject.SetActive( true );
			mHandTap2Texture.gameObject.SetActive( false );
			mTapsNumberTexture.gameObject.SetActive( mAmount>1 );
			
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

			if ( mHandTap1Texture != null ) {
				StopTweens( mHandTap1Texture.gameObject );
				mHandTap1Texture.gameObject.SetActive( false );
			}

			if ( mHandTap2Texture != null ) {
				StopTweens( mHandTap2Texture.gameObject );
				mHandTap2Texture.gameObject.SetActive( false );
			}

			if ( mTapsNumberTexture != null ) {
				mTapsNumberTexture.gameObject.SetActive( false );
			}
			
		}
	}
	
}