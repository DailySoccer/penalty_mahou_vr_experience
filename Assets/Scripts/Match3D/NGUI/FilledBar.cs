using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class FilledBar : MonoBehaviour {
		
		private float mPercent = 0;
		public float Percent {
			set { 
				if ( mPercent != value ) {
					mPercent = value;
					PercentChanged();
				}
			}
			get { return mPercent; }
		}
		
		[HideInInspector] public float Width;
	
		[HideInInspector] public UITexture TextureLeft;
		[HideInInspector] public UITexture TextureCenter;
		[HideInInspector] public UITexture TextureRight;
		
		bool mPercentChanged = false;
		
		void Awake () {
			TextureLeft 	= Helper.GetComponent<UITexture>( transform, "Left" );
			TextureCenter 	= Helper.GetComponent<UITexture>( transform, "Center" );
			TextureRight 	= Helper.GetComponent<UITexture>( transform, "Right" );
			
			Width = TextureCenter.transform.localScale.x;
		}
		
		void ApplyPercent() {
			float lenght = Percent * Width;

			Vector3 scale = TextureCenter.transform.localScale;
			scale.x = lenght;
			TextureCenter.transform.localScale = scale;
			
			Vector3 pos = TextureRight.transform.localPosition;
			pos.x = TextureCenter.transform.localPosition.x + TextureCenter.transform.localScale.x - 1f;
			TextureRight.transform.localPosition = pos;

			/*
			if ( lenght < TextureLeft.transform.localScale.x + TextureRight.transform.localScale.x ) {
				if ( TextureCenter.gameObject.activeSelf )
					TextureCenter.gameObject.SetActive( false );
				
				Vector3 pos = TextureRight.transform.localPosition;
				pos.x = TextureCenter.transform.localPosition.x;
				TextureRight.transform.localPosition = pos;
			}
			else {
				if ( !TextureCenter.gameObject.activeSelf )
					TextureCenter.gameObject.SetActive( true );
				
				Vector3 scale = TextureCenter.transform.localScale;
				scale.x = lenght;
				TextureCenter.transform.localScale = scale;
				
				Vector3 pos = TextureRight.transform.localPosition;
				pos.x = TextureCenter.transform.localPosition.x + TextureCenter.transform.localScale.x - 1f;
				TextureRight.transform.localPosition = pos;
			}
			*/
		}
		
		void Update () {
			if ( mPercentChanged ) {
				ApplyPercent();
				mPercentChanged = false;
			}
		}
		
		void PercentChanged() {
			if ( mPercent < 0.1f ) {
				if ( gameObject.activeSelf )
					gameObject.SetActive( false );
			}
			else {
				if ( !gameObject.activeSelf )
					gameObject.SetActive( true );
			}
			
			mPercentChanged = true;
		}
		
		public void Activate() {
			Percent = 0;
			PercentChanged();
		}
		
		public void Deactivate() {
			gameObject.SetActive( false );
		}		
	}
	
}

