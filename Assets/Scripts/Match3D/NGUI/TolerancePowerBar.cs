using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class TolerancePowerBar : MonoBehaviour {
	
		[HideInInspector] public UITexture TextureLeft;
		[HideInInspector] public UITexture TextureRight;
		
		float mMinX;		// Valor mínimo de la barra
		float mMaxX;		// Dimensión máxima de la barra
		
		void Awake () {
			TextureLeft 	= Helper.GetComponent<UITexture>( transform, "Left" );
			TextureRight 	= Helper.GetComponent<UITexture>( transform, "Right" );
			
			mMinX = -TextureLeft.transform.localScale.x;
			mMaxX = TextureRight.transform.localScale.x;
		}
		
		void Start () {
		}
		
		public void SetRange( float leftMargin, float center, float rightMargin ) {
			
			// LEFT
			if ( center - leftMargin < mMinX ) {
				leftMargin = center - mMinX;
			}
			
			if ( leftMargin > 0f ) {
				TextureLeft.gameObject.SetActive( true );

				Vector3 p = TextureLeft.transform.localPosition;
				p.x = center;
				TextureLeft.transform.localPosition = p;
				
				Vector3 scale = TextureLeft.transform.localScale;
				scale.x = leftMargin;
				TextureLeft.transform.localScale = scale;
			} else {
				TextureLeft.gameObject.SetActive( false );
			}

			
			// RIGHT
			if ( center + rightMargin > mMaxX ) {
				rightMargin = mMaxX - center;
			}

			if ( rightMargin > 0f ) {
				TextureRight.gameObject.SetActive( true );
				
				Vector3 p = TextureRight.transform.localPosition;
				p.x = center;
				TextureRight.transform.localPosition = p;
				
				Vector3 scale = TextureRight.transform.localScale;
				scale.x = rightMargin;
				TextureRight.transform.localScale = scale;
			} else {
				TextureRight.gameObject.SetActive( false );
			}
		}
		
		void Update () {
		}
	
	}
	
}
