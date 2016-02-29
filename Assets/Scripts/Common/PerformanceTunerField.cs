using UnityEngine;
using System.Collections;


namespace FootballStar.Common
{
	public class PerformanceTunerField : MonoBehaviour
	{
		public GameObject[] MiscObjects;
		
		void Start()
		{
			var perfTuner = GameObject.FindGameObjectWithTag("GameModel").GetComponent<PerformanceTuner>();
			
			if (perfTuner != null && perfTuner.OwnQualityLevel <= 2)
				DowngradeField();
		}
		
		void DowngradeField()
		{			
			foreach(var go in MiscObjects)
				Destroy(go);
		}
	}
}