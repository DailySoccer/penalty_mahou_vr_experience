using System;
using UnityEngine;
using FootballStar.Common;

namespace FootballStar.Manager.Model
{
	public class MatchSpecial : Match
	{
		public MatchSpecial(int oppID, string playSeq, int reward, float diff ) : base(-1, eMatchKind.FRIENDLY, -1)
		{
			mMatchDef = new MatchDefinition() 
						{
							OpponentID = oppID,
							PlaySequence = playSeq,
							Reward = reward,
							Difficulty = diff,
						}; 
		}
		
		override public MatchDefinition Definition
		{
			get { return mMatchDef; }
		}
		
		private MatchDefinition mMatchDef;
	}
}

