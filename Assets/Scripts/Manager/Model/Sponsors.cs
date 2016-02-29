using System;
using System.Collections.Generic;
using ExtensionMethods;
using Newtonsoft.Json;

namespace FootballStar.Manager.Model
{
	// Una instancia de Sponsors por tier. Tentativo: Podria seguir un approach como el de improvements. Cada sponsordefinition tendria un ID
	// y esto ya no seria "PerTier", sino que guardaria una lista global con todos los purchases, y estaria directamente en el Player.
	[JsonObject(MemberSerialization.OptOut)]
	public class SponsorsPerTier
	{
		public SponsorsPerTierDefinition Definition
		{ 
			get { 
				return SponsorsPerTierDefinition.AllTiers[mTierID];
			}
		}
		
		public SponsorDefinition GetSponsorDefinitionByID(int sponsorIdx)
		{
			return Definition.SponsorsDefinitions[sponsorIdx];
		}
		
		public List<int> PurchasedSponsorIDs { get { return mPurchasedSponsorIDs; } }
		
		public SponsorsPerTier()
		{
		}
		
		public SponsorsPerTier(Player player, int tierID)
		{
			mTierID = tierID;
			mPlayer = player;
			mPurchasedSponsorIDs = new List<int>();
		}
		
		public bool IsSponsorLocked(SponsorDefinition sponsor)
		{
			return mPlayer.Fans < sponsor.RequiredFans;
		}
		
		public bool IsSponsorAlreadyPurchased(SponsorDefinition sponsor)
		{
			return mPurchasedSponsorIDs.Contains(Definition.SponsorsDefinitions.IndexOf(sponsor));
		}

		// Primer sponsor que podemos comprar todavia no comprado en el tier
		public int GetFirstPurchaseableSponsorID() {
			for (int c = 0; c < Definition.SponsorsDefinitions.Length; ++c) {
				var sponsorDef = Definition.SponsorsDefinitions[c];
				if (!(IsSponsorLocked(sponsorDef) || IsSponsorAlreadyPurchased(sponsorDef)))
					return c;
			}
			return -1;
		}
	
		public bool BuySponsor(SponsorDefinition sponsor)
		{
			int sponsorID = Definition.SponsorsDefinitions.IndexOf(sponsor);
			bool successBougth = false;
			if (sponsorID == -1)
			{
				successBougth = false;
				throw new Exception("WTF 93 - Trying to buy a sponsor not in this tier");				
			}
			else
				successBougth = true;
			
			if (PurchasedSponsorIDs.Contains(sponsorID))
			{
				successBougth = false;
				throw new Exception("WTF 94 - Already bought");				
			}
			else
				successBougth = true;

			PurchasedSponsorIDs.Add(sponsorID);
			
			return successBougth;
		}
		
		public int AddSponsorshipBonuses()
		{
			int sum = 0;
			foreach(var id in mPurchasedSponsorIDs)
				sum += Definition.SponsorsDefinitions[id].SponsorshipBonus;
					
			return sum;
		}
	
		private int mTierID;
		private Player mPlayer;
		private List<int> mPurchasedSponsorIDs;
	}
	
	public class SponsorsPerTierDefinition
	{
		// Los N (3) sponsors
		public SponsorDefinition[] SponsorsDefinitions;
		
		// N Tiers
		static public SponsorsPerTierDefinition[] AllTiers = new SponsorsPerTierDefinition[]
		{
			new SponsorsPerTierDefinition()
			{
				// N Sponsors Per Tier
				SponsorsDefinitions = new SponsorDefinition[]
				{
					new SponsorDefinition() { RequiredFans = 1000, SponsorshipBonus = 1000 },
					new SponsorDefinition() { RequiredFans = 2000, SponsorshipBonus = 2000 },
					new SponsorDefinition() { RequiredFans = 4000, SponsorshipBonus = 4000 }
				}
			},
			new SponsorsPerTierDefinition()
			{
				SponsorsDefinitions = new SponsorDefinition[]
				{
					new SponsorDefinition() { RequiredFans = 1500, SponsorshipBonus = 8000 },
					new SponsorDefinition() { RequiredFans = 3000, SponsorshipBonus = 16000 },
					new SponsorDefinition() { RequiredFans = 9000, SponsorshipBonus = 32000 },
				}
			},
			new SponsorsPerTierDefinition()
			{
				SponsorsDefinitions = new SponsorDefinition[]
				{
					new SponsorDefinition() { RequiredFans = 15000, SponsorshipBonus = 480 },
					new SponsorDefinition() { RequiredFans = 60000, SponsorshipBonus = 520 },
					new SponsorDefinition() { RequiredFans = 105000, SponsorshipBonus = 650 },
				}
			}
		};
	}
	
	public class SponsorDefinition
	{
		public int RequiredFans;
		public int SponsorshipBonus;
	}
}

