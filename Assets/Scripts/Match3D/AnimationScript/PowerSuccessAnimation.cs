using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class PowerSuccessAnimation : AnimationScript {
	
		UITexture BrokenBarLeftTexture;
		UITexture BrokenBarRightTexture;
		UILabel PointsLabel;
		
		Vector3 mPowerBarLeftPosition;
		Vector3 mPowerBarRightPosition;
		
		void Awake () {
			BrokenBarLeftTexture = Helper.GetComponent<UITexture>( transform, "BrokenBar Left" );
			BrokenBarRightTexture = Helper.GetComponent<UITexture>( transform, "BrokenBar Right" );
			PointsLabel = Helper.GetComponent<UILabel>( transform, "Points" );

			SaveState();
			
			Stop();
		}

		void RestoreState() {
			BrokenBarLeftTexture.transform.localPosition = mPowerBarLeftPosition;
			BrokenBarRightTexture.transform.localPosition = mPowerBarRightPosition;
		}
		
		void SaveState() {
			mPowerBarLeftPosition = BrokenBarLeftTexture.transform.localPosition;
			mPowerBarRightPosition = BrokenBarRightTexture.transform.localPosition;
		}
		
		void Start () {
		}
		
		void Update () {
		}
		
		IEnumerator AnimationPlay() {
			RestoreState();
			
			PointsLabel.gameObject.SetActive( false );
			
			AMTween.MoveAdd( BrokenBarLeftTexture.gameObject, AMTween.Hash(
				"x", -0.05f, 
				"time",0.2f ) );

			AMTween.MoveAdd( BrokenBarRightTexture.gameObject, AMTween.Hash(
				"x", +0.05f,
				"time",0.2f ) );
			
			yield return new WaitForSeconds( 0.2f );
			
			PointsLabel.gameObject.SetActive( true );

			AMTween.MoveAdd( BrokenBarLeftTexture.gameObject, AMTween.Hash(
				"x", 0.05f, 
				"time",0.2f ) );

			AMTween.MoveAdd( BrokenBarRightTexture.gameObject, AMTween.Hash(
				"x", -0.05f,
				"time",0.2f ) );
			
			yield return new WaitForSeconds( 0.2f );
			
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
				StopTweens( BrokenBarLeftTexture.gameObject );
				StopTweens( BrokenBarRightTexture.gameObject );
			}
		}
		
	}
	
}
