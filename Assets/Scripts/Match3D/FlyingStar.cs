using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {

	public class FlyingStar : MonoBehaviour {

		
		void Start () {
			mMainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
			m3DOverlayCamera = GameObject.FindGameObjectWithTag("3DOverlayCamera").GetComponent<Camera>();
			mUICamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
			mRenderer = GetComponent<MeshRenderer>();

			gameObject.SetActive(false);
		}

		public void Fly(Vector3 onScreenFrom, Transform to, int delay) {
			mRenderer.enabled = false;
			gameObject.SetActive(true);

			StartCoroutine(FlyCoroutine(onScreenFrom, to, delay));
		}

		IEnumerator FlyCoroutine(Vector3 onScreenFrom, Transform to, int delay) {

			yield return new WaitForSeconds((float)delay * 0.3f);
			mRenderer.enabled = true;

			// Salimos desde una posicion en pantalla
			transform.position = ProjectFromScreenToWorld(onScreenFrom, 40.0f);

			// Interpolamos hasta la Star del UI
			AMTween.MoveTo(gameObject, AMTween.Hash( "time", 0.75f,
			                                        "position", ProjectFromUI(to.position, 27.0f),
			                                        "easetype", AMTween.EaseType.easeOutCubic, 
			                                        "oncomplete", "OnPosTweenComplete"
			                                        )
			               );

			// Mientras, vamos dando vueltas
			AMTween.RotateTo(gameObject, AMTween.Hash( "time", 0.75f,
			                                          "islocal", true,
			                                          "z", 720,
			                                          "easetype", AMTween.EaseType.linear
			                                          )
			                 );
		}

		Vector3 ProjectFromScreenToWorld(Vector3 onScreen, float depth) {
			onScreen.z = depth;
			return m3DOverlayCamera.ScreenToWorldPoint(onScreen);
		}
		 
		Vector3 ProjectFromMain(Vector3 fromPos) {
			return m3DOverlayCamera.ScreenToWorldPoint(mMainCamera.WorldToScreenPoint(fromPos));
		}

		Vector3 ProjectFromUI(Vector3 fromPos, float depth) {
			var onScreen = mUICamera.WorldToScreenPoint(fromPos);
			onScreen.z = depth;
			return m3DOverlayCamera.ScreenToWorldPoint(onScreen);
		}

		void OnPosTweenComplete() {
			AMTween.Stop(gameObject);
			gameObject.SetActive(false);
		}

		Camera mMainCamera;
		Camera m3DOverlayCamera;
		Camera mUICamera;
		MeshRenderer mRenderer;
	}

}
