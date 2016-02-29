using System;
using UnityEngine;
using System.Collections;

namespace FootballStar.Common
{
	public class GeneralUtils
	{
		static public GameObject GetDisabledGameObject(GameObject parent, string name)
		{
			var allGOs = parent.GetComponentsInChildren<Transform>(true);
			
			foreach (var go in allGOs)
		    {
		    	if (go.name == name)
					return go.gameObject;
		    }
			
			return null;
		}
		
		
		static public IEnumerator WaitForAnimation(Animation anim)
		{
			 while ( anim.isPlaying )
	            yield return null;
		}
	}
}

