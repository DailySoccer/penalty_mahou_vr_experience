using UnityEngine;
using System.Collections;
using JsonFx.Json;
using System.IO;
using System;

namespace FootballStar.Match3D {
	
	public class MatchSequence : MonoBehaviour {		
		public PlaySequenceDef[] PlaySequence;
	}
	
	[Serializable]
	public class PlaySequenceDef
	{
		public GameObject PlayGameObject;
		public int OpponentGoalsToAdd;
		public int Minute;
	}
}
