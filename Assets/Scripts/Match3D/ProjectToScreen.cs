using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class ProjectToScreen : MonoBehaviour {
	
		public GameObject Source;
		public float DisplayHeight;
		
		void Awake () {
			mPixelSizeAdjustment = Helper.GetParentComponent<UIRoot>( transform ).pixelSizeAdjustment;
		}
		
		void Start () {
		}
		
		void Update () {
			if ( Source != null ) {
				Vector3 position = Source.transform.position;
				position.y = DisplayHeight;
	
				Vector3 onScreen = Camera.main.WorldToScreenPoint(position);
				
				// Clipeado
				float positionX = onScreen.x; //Mathf.Clamp(onScreen.x, MinSize * 0.5f, Screen.width - MinSize * 0.5f);
				float positionY = onScreen.y; //Mathf.Clamp(onScreen.y, MinSize * 0.5f, Screen.height - MinSize * 0.5f);
		
				// Pasamos al viewport ortografico
				var adjustedX = (positionX - Screen.width * 0.5f) * mPixelSizeAdjustment;
				var adjustedY = (positionY - Screen.height * 0.5f) * mPixelSizeAdjustment;
				
				transform.localPosition = new Vector3(adjustedX, adjustedY, 0.0f);
			}
		}
		
		private float mPixelSizeAdjustment;
	}
	
}
