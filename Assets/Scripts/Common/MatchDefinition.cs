using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FootballStar.Common
{
	// Definicion estatica de un partido, el partido de base de datos, digamos
	public class MatchDefinition
	{
		public float        Difficulty;
		public string	    PlaySequence;						// "Tier1Friendly01", "Tier3Cup01"...
		public int          Reward;
        public int          MyID;
		//TODO: Modificar esto. He hecho una func. que devuelve el nombre desde tierdefinition
        public string       MyName { get { return Name(MyID); } }
        public string       MyBadgeName { get { return BadgeName(MyID); } }
        public int          OpponentID;
        //TODO: Modificar esto. He hecho una func. que devuelve el nombre desde tierdefinition
        public string       OpponentName { get { return Name(OpponentID); } }
        public string       OpponentBadgeName { get { return BadgeName(OpponentID); } }

        public static string Name(int id) { return FootballStar.Manager.Model.TierDefinition.TeamNames[id]; }
        public static string BadgeName(int id)
        {
                return Name(id).Replace(" ", "").Replace(".", "").Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                                                                     .Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "u");
        }

        public int RewardAsNotWinner {
			get { return Reward / 5; }
		}
	} 
	
	[JsonObject(MemberSerialization.OptOut)]
	public class MatchResult
	{
		// Puntuacion obtenida en cada una de las interacciones del partido
		public List<float> ScorePerInteractionSequence { get; set; } // <-- Promedio de las jugadas
		
		// Goles
		public int PlayerGoals { get; set; }  // Goles a favor
		public int OppGoals { get; set; } // Goles en contra.
		
		public bool PlayerWon  { get { return PlayerGoals > OppGoals; } }
		public bool PlayerTied { get { return PlayerGoals == OppGoals; } }
		public bool PlayerLost { get { return PlayerGoals < OppGoals; } }

		static public readonly int FANS_PER_PRECISION_BALL = 1500;

		// Numero de pelotas al final del partido => Haciendo redondeos al alza
		public int NumPrecisionBallsEndOfMatch
		{
			get {
				float avgScore = AvgScoreEndOfMatch;
				int numBalls = 0;
				
				// Si perdemos o empatamos, siempre 0 balones
				if (PlayerWon) {
					if (avgScore < 0.33f)	   numBalls = 3;
					else if (avgScore < 0.66f) numBalls = 2;
					else 					   numBalls = 1;
				}
				
				return numBalls;
			}
		}
		public int FansToAdd { get { return NumPrecisionBallsEndOfMatch * FANS_PER_PRECISION_BALL; } }

		// Score suponiendo que el partido ha acabado, normalizado entre 0 y 1.
		// 0 es el mejor posible (seria el delay acumulado respecto al Perfect)
		private float AvgScoreEndOfMatch
		{
			get 
			{
				if (ScorePerInteractionSequence != null)
					return CalcAvgScoreSoFar(ScorePerInteractionSequence, ScorePerInteractionSequence.Count);
				else
					return 0.0f;
			}
		}

		// Auxiliar para calcular un score dado una sequencia de interacciones so far y cuántas interacciones habra al final
		static float CalcAvgScoreSoFar(List<float> scoreSequence, int totalInteractionsCount) {
			float sum = 0; 

			foreach (var score in scoreSequence)
				sum += Mathf.Abs(score);

			// Tenemos que contar con que las interacciones que todavia no hemos hecho son "-1", es decir, como si todavia
			// fueran fallos.
			sum += totalInteractionsCount - scoreSequence.Count;

			return sum / (float)totalInteractionsCount;
		}

		// Numero de pelotas entre 0 y 3, sin hacer redondeos.
		static float CalcNumPrecisionBallsSoFar(List<float> scoreSequence, int totalInteractionsCount) {
			return CalcNumPrecisionBalls(CalcAvgScoreSoFar(scoreSequence, totalInteractionsCount));
		}

		// Puntuacion entre 0 y 3 para una sola interaccion. El score que entra es entre -1 y 1, directamente del QTE
		static float CalcNumPrecisionBalls(float score) {
			return (1.0f - Mathf.Abs(score)) * 3.0f;
		}
		
		public MatchResult()
		{
		}
	}
}

