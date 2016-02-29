using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class PowerFailAnimation : AnimationScript {
	
		UITexture BrokenBarLeftTexture;
		UITexture BrokenBarRightTexture;
		UITexture PowerFailTexture;
		
		Vector3 mPowerBarLeftPosition;
		Vector3 mPowerBarRightPosition;
		Vector3 mPowerFailScale;
		
		void Awake () {
			BrokenBarLeftTexture = Helper.GetComponent<UITexture>( transform, "BrokenBar Left" );
			BrokenBarRightTexture = Helper.GetComponent<UITexture>( transform, "BrokenBar Right" );
			PowerFailTexture = Helper.GetComponent<UITexture>( transform, "Fail Texture" );

			SaveState();
			
			Stop();
		}

		void RestoreState() {
			BrokenBarLeftTexture.transform.localPosition = mPowerBarLeftPosition;
			BrokenBarRightTexture.transform.localPosition = mPowerBarRightPosition;
			PowerFailTexture.transform.localScale = mPowerFailScale;
		}
		
		void SaveState() {
			mPowerBarLeftPosition = BrokenBarLeftTexture.transform.localPosition;
			mPowerBarRightPosition = BrokenBarRightTexture.transform.localPosition;
			mPowerFailScale = PowerFailTexture.transform.localScale;
		}
		
		void Start () {
		}
		
		void Update () {
		}
		
		IEnumerator AnimationPlay() {
			RestoreState();
			
			PowerFailTexture.gameObject.SetActive( false );
			
			AMTween.MoveAdd( BrokenBarLeftTexture.gameObject, AMTween.Hash(
				"x", -0.05f, 
				"time",0.2f ) );

			AMTween.MoveAdd( BrokenBarRightTexture.gameObject, AMTween.Hash(
				"x", +0.05f,
				"time",0.2f ) );
			
			yield return new WaitForSeconds( 0.2f );
			
			PowerFailTexture.gameObject.SetActive( true );
			
			AMTween.ScaleBy( PowerFailTexture.gameObject, AMTween.Hash(
				"amount", Vector3.one * 2f,
				"time",0.5f, 
				"easetype","easeOutElastic" ) );

			
			AMTween.MoveAdd( BrokenBarLeftTexture.gameObject, AMTween.Hash(
				"x", 0.05f, 
				"time",0.5f ) );

			AMTween.MoveAdd( BrokenBarRightTexture.gameObject, AMTween.Hash(
				"x", -0.05f,
				"time",0.5f ) );

			yield return new WaitForSeconds( 0.5f );

			
			Stop();
		}
		
		public override void Play () {
			base.Play ();
			
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

			if ( IsPlaying ) {
				StopTweens( PowerFailTexture.gameObject );
				StopTweens( BrokenBarLeftTexture.gameObject );
				StopTweens( BrokenBarRightTexture.gameObject );
			}
		}
		
	}
	
}
