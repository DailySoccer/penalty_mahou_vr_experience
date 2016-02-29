using UnityEngine;
using System.Collections;
using FootballStar.Manager.Model;

namespace FootballStar.Manager
{
	public class OpenDebugMenu : MonoBehaviour 
	{
		public GameObject DebugMenuPrefab;
		private MainModel mMainModel;
		
		public void OnClick()
		{
			var theDebugMenuTransform = gameObject.transform.FindChild("DebugMenu(Clone)");
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
			if(!mMainModel.FinalVersion)
			{
				if (theDebugMenuTransform == null)
				{
					var newMenu = Instantiate(DebugMenuPrefab) as GameObject;
					newMenu.transform.parent = this.transform;
					newMenu.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				}
				else
				{
					Destroy(theDebugMenuTransform.gameObject);
				}
			}
			else
			{
#if UNITY_WEBPLAYER
				Application.ExternalEval("window.open('http://download.unusualwonder.com/mahou/MahouBasesLegales.html','Política de privacidad y protección de datos personales')");
#else
				Application.OpenURL("http://download.unusualwonder.com/mahou/MahouBasesLegales.html");
#endif
			}
		}
	}
}
