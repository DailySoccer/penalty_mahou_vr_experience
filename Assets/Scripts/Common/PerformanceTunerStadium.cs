using UnityEngine;
using System.Collections;


namespace FootballStar.Common
{
	public class PerformanceTunerStadium : MonoBehaviour
	{
		public GameObject Banderas;
		public GameObject Bufandas;
		public GameObject[] MiscObjects;
		
		void Start()
		{
			var mainModel = GameObject.FindGameObjectWithTag("GameModel");
			
			if (mainModel != null)
			{
				var perfTuner = mainModel.GetComponent<PerformanceTuner>();
				
				if (perfTuner != null)
				{
					if (perfTuner.OwnQualityLevel <= 2)
					{
						Destroy(Banderas);
						Destroy(Bufandas);
						
						foreach(var go in MiscObjects)
							Destroy(go);
					}
					else if (perfTuner.OwnQualityLevel == 3)
					{
						// Al iphone4s no le sientan especialmente bien las particulas (ParticleSystem.WaitForUpdateThread)
						// En realidad, como el 3 es nuestro default, vamos a aligerar
						Destroy(Banderas);
						Destroy(Bufandas);
					}
				}
			}
		}

	}
}