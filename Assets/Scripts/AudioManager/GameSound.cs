using UnityEngine;
using System.Collections;

namespace FootballStar.Audio
{
	// Sound definition
	[System.Serializable]
	public class GameSound
	{
		[SerializeField]
		public SoundDefinitions SoundDef;
		
		[SerializeField]
		public float Volume = 1f;
		
		[SerializeField]		
		public AudioClip[] Audios;
	   	
		private AudioClip mSound;
		public AudioClip TheSound
		{
			get { return mSound; }
			set { mSound = value; }
		}
		
		private int mCurrenTrack = -1;
		public int CurrentTrack
		{
			get { return mCurrenTrack; }
			set { mCurrenTrack = value; }
		}
		
		
		/**** Constructor *****/
		public  GameSound(){}
		
		/**** Metodos *****/
		public void SetMainClip()
		{
			int i = Random.Range(0, Audios.Length);
			TheSound = Audios[i];	
		}
	}
}