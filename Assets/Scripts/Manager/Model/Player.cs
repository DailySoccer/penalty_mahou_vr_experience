using System;
using System.Collections.Generic;
using FootballStar.Common;
using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using ExtensionMethods;

namespace FootballStar.Manager.Model
{
	public enum TutorialStage 
	{
		FIRST_MATCH,
		FRIENDLY_EXPLANATION,
		CUP_EXPLANATION,
		YOU_NEED_TO_IMPROVE,
		DONE,
	}
	
	[JsonObject(MemberSerialization.OptOut)]
	public class Player
	{	
		public readonly int EnergyCostPerMatch = -1;
		public readonly int MaxEnergy = 10;
		
		
		// Acabamos de entrar al juego o ya hemos recorrido el ciclo partido/menu al menos una vez?
		public bool ShowIntro { get; set; }
		
		// En que fase del tutorial nos encontramos
		public TutorialStage TutorialStage { get; set; }
				
		public float Power { get; set; }
		public float Vision { get; set; }				
		public float Technique { get; set; }				
		public float Motivation { get; set; }
		public float Skill { get { return Vision + Power + Technique + Motivation; } }

		// Habilidad del Player para enfrentarse a un partido
		public float MatchSkill {
			get {
				float skill = 0f;
				
				// Buscar la característica menor
				if ( Technique < Vision ) {
					if ( Technique < Power ) {
						// Technique <
						skill = Technique;
					}
					else {
						// Vision < OR Power <
						skill = ( Vision < Power ) ? Vision : Power;
					}
				}
				else {
					// Vision < OR Power <
					skill = ( Vision < Power ) ? Vision : Power;
				}
				
				// Habilidad del Partido = 3 Skill + Motivacion
				return (3 * skill) + Motivation;
			}
		}
		
		public int Fans { get; set; }
		public int Money { get; set; }
		public int CurrentEnergy { get; set; }
		public float EnergyPercent { get { return (float)((float)CurrentEnergy/(float)MaxEnergy); } }

		public bool IsTeamSelected;

		private int mSelectedTeamId;
		public int SelectedTeamId { 
			get {return mSelectedTeamId;} 
			set {
				mSelectedTeamId = value;
				CreateTiers();
			}
		}
		
		public DateTime LastEnergyUse { get; set; }
		
		public int  CurrentTierIndex { get { return mCurrentTierIndex; } }
		public Tier CurrentTier { get { return mTiers[CurrentTierIndex]; }	}
		
		public Improvements Improvements { get { return mImprovements; } }
		
		public static readonly int SAVE_VERSION = 2;
		public int CurrentSaveVersion { get { return mCurrentSaveVersion; } }

		// Para marcar que ya hemos mostrado el mensaje de Euro desbloqueada
		public bool EuroUnlockScreenAlreadyShown { get; set; }

		// Para marcar que ya hemos mostrado las pantallas del tutorial de cuando entramos la primera vez en entrenar y en vida
		public bool EntrenarTutorialScreenAlreadyShown { get; set; }
		public bool VidaTutorialScreenAlreadyShown { get; set; }


		public Player()
		{
			mCurrentSaveVersion = SAVE_VERSION;
		}
		
		public void Init()
		{
			//CreateTiers();

			mImprovements = new Improvements();
			
			ShowIntro = true;
			TutorialStage = TutorialStage.FIRST_MATCH;
			EuroUnlockScreenAlreadyShown = false;
			EntrenarTutorialScreenAlreadyShown = false;
			VidaTutorialScreenAlreadyShown = false;
			
			Power = 0.025f;
			Vision = 0.025f;
			Technique = 0.025f;
			Motivation = 0.025f;
			
			Fans = 1500;
			Money = 0;

			SelectedTeamId = 0;     // El equipo por defecto
			IsTeamSelected = false; 
			
			CurrentEnergy = MaxEnergy;

			// TEMP
			mCurrentTierIndex = 2;
		}

		public void InitAfterDeserialization() {

			if (mCurrentSaveVersion != SAVE_VERSION) {
				// Aqui van todas las conversiones, valores por defecto etc
				Debug.Log("Old version " + mCurrentSaveVersion + " loaded, converting to " + SAVE_VERSION);
				mCurrentSaveVersion = SAVE_VERSION;
			}

			GeneratePlaySequenceNames();
		}
	
		private void CreateTiers()
		{
			mTiers = new List<Tier>();
						
			mTiers.Add(new Tier(this, 0));
			mTiers.Add(new Tier(this, 1));
			mTiers.Add(new Tier(this, 2));
			
			GeneratePlaySequenceNames();
		}
		
		private void GeneratePlaySequenceNames()
		{
			// Autogeneramos los nombres (TierXFriendlyXX) de las sequencias que tengan PlaySequence = null
			mTiers.ForEachWithIndex( (theTier, tierIndex) =>
			{
				GeneratePlaySequenceNames(theTier.Definition.FriendlyMatchesDefs, tierIndex, "Friendly");
				GeneratePlaySequenceNames(theTier.Definition.LeagueMatchesDefs, tierIndex, "League");
				GeneratePlaySequenceNames(theTier.Definition.CupMatchesDefs, tierIndex, "Cup");
				GeneratePlaySequenceNames(theTier.Definition.EuroMatchesDefs, tierIndex, "Europe");
			});
		}
		
		private void GeneratePlaySequenceNames(MatchDefinition[] matchDefs, int tierIndex, string kind)
		{
			if (matchDefs == null)
				return;
			
			matchDefs.ForEachWithIndex((theMatchDef, matchIndex) =>
			{
				if (theMatchDef.PlaySequence == null)
					theMatchDef.PlaySequence = string.Format("Tier{0}{1}{2:00}", tierIndex + 1, kind, matchIndex + 1);
			});
		}
		
		public bool BuyImprovement(ImprovementItem theItem)
		{			
			if (Money < theItem.Price)				
				return false;
			
			Improvements.BuyImprovement(theItem);
			
			Vision += theItem.VisionDiff;
			Power += theItem.PowerDiff;
			Technique += theItem.TechniqueDiff;
			Motivation += theItem.MotivationDiff;
			
			Vision = Mathf.Clamp01(Vision);
			Power = Mathf.Clamp01(Power);
			Technique = Mathf.Clamp01(Technique);
			Motivation = Mathf.Clamp01(Motivation);
			
			Money -= theItem.Price;
			
			return true;
		}
		
		public bool BuySponsor(SponsorDefinition sponsor)
		{
			if (Fans >= sponsor.RequiredFans)
				return CurrentTier.Sponsors.BuySponsor(sponsor);
			
			return false;
		}
		
		public int AddSponsorshipBonuses()
		{
			// Tentative: Quiza deberia ser la suma global
			return CurrentTier.Sponsors.AddSponsorshipBonuses();
		}

		public int CheckEnergyRecharge()
		{
			// Tras la carga, reponemos 1 unidad de energia por cada 6 minutos
			int energyAdded = 0;
			Double timeCalculation =  ( (TimeSpan)(DateTime.Now - LastEnergyUse) ).TotalMinutes;
			int minutesBetween = (int)Math.Floor(timeCalculation);
			//Debug.Log( "Minutos hasta que se recargue la siguietne unidad de energia: " + (minutesBetween) );
			int cycles = minutesBetween/6;
			if( cycles > 0 )
			{
				energyAdded = -EnergyCostPerMatch * cycles;
				AddEnergy( energyAdded );
			}
			
			return energyAdded;
		}
		
		public void AddEnergy( int amount )
		{
			if ( amount != 0 )
			{
				CurrentEnergy = Mathf.Clamp(CurrentEnergy + amount, 0, MaxEnergy);
				LastEnergyUse = DateTime.Now;
			}

			if(CurrentEnergy == 0)
			{
				MixPanel.SendEventToMixPanel(AnalyticEvent.NO_ENERGY);
			}
		}

		int mCurrentSaveVersion = -1;
		List<Tier> mTiers;
		Improvements mImprovements;
		int mCurrentTierIndex = -1;
	}
}

