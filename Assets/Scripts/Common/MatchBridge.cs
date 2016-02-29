using System;
using UnityEngine;
using FootballStar.Manager.Model;

namespace FootballStar.Common
{
	public class MatchBridge : MonoBehaviour
	{
		// Al cargar el partido... (suministramos directamente el de la base de datos)
		public MatchDefinition CurrentMatchDefinition {	get; set; }
				
		// ...al acabar el partido (este es el que se crea y devuelve desde el partido)
		public MatchResult ReturnMatchResult { get; set; }
		
		// El resultado previo de nuestro partido actual (para ver cuando mejoramos puntuacion y demas...)
		public MatchResult CurrentMatchPrevResult { get; set; }
		
		// En cuanto tenemos resultado, podemos preguntar si hemos conseguido ganar por primera vez
		public bool IsResultFirstTimeWon
		{
			get {
				if (!ReturnMatchResult.PlayerWon)
					return false;
				
				if (CurrentMatchPrevResult == null)
					return true;
				
				return !CurrentMatchPrevResult.PlayerWon;					
			}
		}
		
		public bool IsResultImproved
		{
			get {
				if (!ReturnMatchResult.PlayerWon)
					return false;
				
				if (CurrentMatchPrevResult == null)
					return true;
				
				return ReturnMatchResult.NumPrecisionBallsEndOfMatch > CurrentMatchPrevResult.NumPrecisionBallsEndOfMatch;
			}
		}
		
		
		public int TotalMoneyEarned
		{
			get { 
				if (IsResultFirstTimeWon)
					return CurrentMatchDefinition.Reward + SponsorshipBonus;	// Pequenya recompensa en cualquier caso
				else if (ReturnMatchResult.PlayerWon)
					return SponsorshipBonusRepeatMatch;
				else
					return 0;
			}
		}
		
		public int TotalFansEarned
		{
			get {
				if (IsResultFirstTimeWon)
					return ReturnMatchResult.FansToAdd;
				else if (IsResultImproved) // Hemos vuelto a ganar?: Tenemos que recompensar la diferencia en bolas
					return MatchResult.FANS_PER_PRECISION_BALL * (ReturnMatchResult.NumPrecisionBallsEndOfMatch - CurrentMatchPrevResult.NumPrecisionBallsEndOfMatch);
				else
					return 0;
			}
		}	
		
		public enum Difficulty {
			EASY,
			MEDIUM,
			HIGH,
			EXTREME
		}

		// Parametros que dependen de la evolucion del player en el manager. Por ejemplo, el nivel de dificultad: 0 bajo, 1 medio, 2 alto, >= 3 extremo
		public Difficulty DifficultyVision { get;set; }
		public Difficulty DifficultyTechnique { get; set; }
		public Difficulty DifficultyPower { get; set; }

		public Difficulty DifficultyGlobal { get; set; }
				
		// Cantidad de dinero que se lleva adicional por estar patrocinado
		public int SponsorshipBonus { get; set; }

		// Cantidad de dinero que se lleva al repetir y ganar un partido patrocinado
		public int SponsorshipBonusRepeatMatch { 
			get {
				int bonus = 0;
				if ( DifficultyGlobal == Difficulty.EASY )			bonus = SponsorshipBonus / 10; // 10%
				else if ( DifficultyGlobal == Difficulty.MEDIUM )	bonus = SponsorshipBonus / 5; // 20%
				else if ( DifficultyGlobal == Difficulty.HIGH )   	bonus = SponsorshipBonus / 3; // 33%
				else 												bonus = SponsorshipBonus / 2; // 50%
				return bonus;
			}
		}

		// Usamos la relevancia del partido para felicitar al jugador si lo gana
		public FootballStar.Manager.Model.Match.eMatchRelevance MatchRelevance { get; set; }
	}
}