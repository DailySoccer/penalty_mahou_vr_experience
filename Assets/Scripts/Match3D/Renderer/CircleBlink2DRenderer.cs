using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class CircleBlink2DRenderer : MonoBehaviour {
	
		public Transform GizmoParent;
		
		public float DisplayHeight = 1.8f;
		
		public bool IsActivated {
			get {
				return GizmoParent.gameObject.activeSelf;
			}
		}
		
		void Awake () {
			if ( GizmoParent == null )
				GizmoParent = transform;
			
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			mPixelSizeAdjustment = mMatchManager.GetComponentInChildren<UIRoot>().pixelSizeAdjustment;
			
			mTextureInside = Helper.GetComponent<UITexture>( GizmoParent, "CircleInside" );
			mTextureBlink = Helper.GetComponent<UITexture>( GizmoParent, "CircleBlink" );

			mDimension2D = mTextureBlink.transform.localScale;
			
			Deactivate();
		}
		
		void Start () {
		}
		
		void Update () {
		}
		
		void LateUpdate() {
			if ( mTarget != null ) {
				Vector3 soccerPos = mTarget.transform.position;
				soccerPos.y = DisplayHeight;
	
				Vector3 onScreen = Camera.main.WorldToScreenPoint(soccerPos);
				
				// Clipeado
				mPosition2D.x = Mathf.Clamp(onScreen.x, mDimension2D.x * 0.5f, Screen.width - mDimension2D.x * 0.5f);
				mPosition2D.y = Mathf.Clamp(onScreen.y, mDimension2D.y * 0.5f, Screen.height - mDimension2D.y * 0.5f);
	
				UpdateGizmos( mPosition2D );
			}
		}
		
		private void UpdateGizmos( Vector2 position ) {
			// Pasamos al viewport ortografico
			var adjustedX = (position.x - Screen.width * 0.5f) * mPixelSizeAdjustment;
			var adjustedY = (position.y - Screen.height * 0.5f) * mPixelSizeAdjustment;
			
			GizmoParent.localPosition = new Vector3(adjustedX, adjustedY, 0.0f);
		}
		
		IEnumerator UpdateBlink() {
			while ( true ) {
				//AMTween.ScaleTo( mTextureBlink.gameObject, new Vector3(128,128,1), 0.05f );
				mTextureBlink.gameObject.SetActive( true );
				
				yield return new WaitForSeconds(0.1f);
				
				mTextureBlink.gameObject.SetActive( false );
				//AMTween.ScaleTo( mTextureBlink.gameObject, new Vector3(100f,100f,1), 0.05f );
				
				yield return new WaitForSeconds(0.1f);
			}
		}
		
		public void Activate( GameObject target ) {
			StopAllCoroutines();
			
			mTarget = target;
			
			mTextureInside.gameObject.SetActive( true );
			mTextureBlink.gameObject.SetActive( true );
			
			GizmoParent.gameObject.SetActive( true );
			
			StartCoroutine( UpdateBlink() );
		}
		
		void StopTweens ( GameObject go ) {
			AMTween[] tweenActivos = go.GetComponents<AMTween>();
			foreach( AMTween tween in tweenActivos ) {
				if ( tween ) {
					Destroy( tween );
				}
			}
		}		
		
		public void Deactivate() {
			StopAllCoroutines();

			StopTweens( gameObject );
			
			GizmoParent.gameObject.SetActive( false );
		}	
		
		public bool IsPointInRadio( Vector2 point, float radio ) {
			float distance2 = (point - mPosition2D).sqrMagnitude;
			return distance2 < (radio * radio);
		}
		
		public bool IsPointInside( Vector2 point ) {
			float radio = mDimension2D.x * 0.5f * mPixelSizeAdjustment;
			float distance2 = (point - mPosition2D).sqrMagnitude;
			return distance2 < (radio * radio);
		}
		
		/*
		public bool IsPointInside( Vector2 point ) {
			float radio = mTextureASize * 0.5f * mPixelSizeAdjustment;
			float distance2 = (point - mPosition2D).sqrMagnitude;
			return distance2 < (radio * radio);
		}
		*/
		
		private MatchManager mMatchManager;
		private GameObject mTarget;
		
		private UITexture mTextureInside;
		private UITexture mTextureBlink;

		private Vector2 mPosition2D;
		private Vector3 mDimension2D;
		
		private float mPixelSizeAdjustment;
	}
	
}