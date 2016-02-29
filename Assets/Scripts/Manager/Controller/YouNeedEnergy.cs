using UnityEngine;
using System.Collections;
using System;

namespace FootballStar.Manager {

	public class YouNeedEnergy : MonoBehaviour {
	
		public UILabel ClockLabel;

		public DateTime LastPlay { get; set; }
		public event EventHandler OnEnergyCountdownEnds;
		
		// Use this for initialization
		void Start () {	}
		
		// Update is called once per frame
		void Update () 
		{
			TimeSpan mClockValue = (LastPlay + new TimeSpan(0,6,0)) - DateTime.Now;
			ClockLabel.text 	 = string.Format("{0:00}:{1:00} minutos", mClockValue.Minutes, mClockValue.Seconds);
			if( mClockValue.Ticks <= 0 )
			{
				OnEnergyCountdownEnds(this, EventArgs.Empty);
			}
		}
	}

}