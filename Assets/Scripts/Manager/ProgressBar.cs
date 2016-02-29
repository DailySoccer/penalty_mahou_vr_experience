using System;
using UnityEngine;

namespace FootballStar.Manager
{
	public class ProgressBar : MonoBehaviour
	{
		public float Percent;		// Entre 0 y 1
		public float MiddlePercent;
		public Color MiddleColor;

		public void Start()
		{
			ForceUpdate();
		}
		
		public void Update()
		{
			ForceUpdate();
		}
		
		void ForceUpdate()
		{
			if (Foreground != null)
				Foreground.fillAmount = Percent;
			
			if (Middleground != null)
			{
				Middleground.fillAmount = MiddlePercent;
				Middleground.color = MiddleColor;
			}
		}
				
		public UISprite Foreground;
		public UISprite Middleground;
		public UISprite Background;
	}
}

