using System;
using System.Linq;
using UnityEngine;
using FootballStar.Common;
using FootballStar.Manager.Model;

namespace FootballStar.Manager
{
	public class EuroScreen : PlayScreen
	{
		protected override string SmallTheaterTrophyName { get { return "EuroTrophy01"; } }
		protected override Match MatchToPlay { get { return mMainModel.CurrentTier.MatchBrowser.CurrentEuro; } }
		
		protected override string MatchCount 
		{ 
			get {
				// Tiene que haber 7 partidos: 2 Octavos, 2 Cuartos, 2 Semis, 1 Final
				string[] All = new string[]
				{
					"EURO OCTAVOS IDA", "EURO OCTAVOS VUELTA",
					"EURO CUARTOS IDA", "EURO CUARTOS VUELTA",
					"EURO SEMIFINALES IDA", "EURO SEMIFINALES VUELTA",
					"EURO FINAL"
				};
				return "[FFFFFF]" + All[mMainModel.CurrentTier.MatchBrowser.CurrentEuroIdx];
			}
		}
		
		protected override void GotoNextMatch() { mMainModel.CurrentTier.MatchBrowser.NextEuro(); }
		protected override void GotoPrevMatch() { mMainModel.CurrentTier.MatchBrowser.PrevEuro(); }
	}
}

