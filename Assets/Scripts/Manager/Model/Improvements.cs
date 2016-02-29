using System;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FootballStar.Manager.Model
{
	public class ImprovementsDefinition
	{
		static public void LoadBackgroundsForCategory(ImprovementCategory category)
		{
			if (category.Background != null)
			{
				var theBackground = Resources.Load(category.Background, typeof(Texture2D)) as Texture2D;
				
				for (int c = 0; c < category.Items.Length; c++)
				{
					category.Items[c].Background = theBackground;
				}
			}
			else
			{
				for (int c = 0; c < category.Items.Length; c++)
				{
					var textureName = category.BackgroundPrefix + (c + 1).ToString("D2");
					category.Items[c].Background = Resources.Load(textureName, typeof(Texture2D)) as Texture2D;
					
					if (category.Items[c].Background == null)
						Debug.Log("Improvement Background Didn't Load... " + textureName);
				}
			}
		}
		
		// Este approach es experimental y en contraste con el de Match/Sponsors
		static public ImprovementItem[] AllImprovementItems
		{
			get 
			{
				if (mAllImprovementItems != null)
					return mAllImprovementItems;
				
				var helper = new List<ImprovementItem>();
				
				helper.AddRange(GymCategory.Items);
				helper.AddRange(BlackboardCategory.Items);
				helper.AddRange(TechniqueCategory.Items);
				helper.AddRange(LockerRoomCategory.Items);
				helper.AddRange(EventsCategory.Items);
				helper.AddRange(PropertiesCategory.Items);
				
				mAllImprovementItems = helper.ToArray();
				
				return mAllImprovementItems;
			}
		}
		
		static public ImprovementItem GetImprovementItemByID(int id)
		{
			var items = AllImprovementItems;
			
			foreach (var item in items)
			{
				if (item.ImprovementItemID == id)
					return item;
			}
			return null;
		}
		
		static private ImprovementItem[] mAllImprovementItems;
				
		
		static public ImprovementCategory GymCategory = new ImprovementCategory()
		{
			Description = "MEJORANDO EL\nGIMNASIO ENTRENARÁS\nMEJOR TU POTENCIA.",
			BackgroundPrefix = "TrainingGymBackground",
			Items = new ImprovementItem[]
			{ 
			  new ImprovementItem() { Name = "Gimnasio nivel 1", Price = 1606, VisionDiff = 0f, PowerDiff = 0.059f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 0 },
			  new ImprovementItem() { Name = "Gimnasio nivel 2", Price = 2759, VisionDiff = 0f, PowerDiff = 0.029f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 1 },
			  new ImprovementItem() { Name = "Gimnasio nivel 3", Price = 3995, VisionDiff = 0f, PowerDiff = 0.029f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 2 },
			  new ImprovementItem() { Name = "Gimnasio nivel 4", Price = 8154, VisionDiff = 0f, PowerDiff = 0.022f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 3 },
			  new ImprovementItem() { Name = "Gimnasio nivel 5", Price = 11253, VisionDiff = 0f, PowerDiff = 0.029f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 4 },
			}
		};
		
		static public ImprovementCategory BlackboardCategory = new ImprovementCategory()
		{
			Description = "APRENDE LAS\nTÁCTICAS DE\nPARTIDOS HISTÓRICOS\nY MEJORA TU VISIÓN\nDE JUEGO",
			Background = "TrainingBlackboardBackground",
			Items = new ImprovementItem[]
			{ 
				new ImprovementItem() { Name = "El primer partido (2 de mayo de 1902)", Price = 1634, VisionDiff = 0.022f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 10 },
			 	new ImprovementItem() { Name = "La primera conquista de Europa", Price = 2815, VisionDiff = 0.037f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 11 },
				new ImprovementItem() { Name = "La táctica de 'El Miedo Escénico'", Price = 4080, VisionDiff = 0.051f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 12 },
				new ImprovementItem() { Name = "La séptima conquista de Europa", Price = 8338, VisionDiff = 0.066f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 13 },
				new ImprovementItem() { Name = "La liga de las remontadas", Price = 11508, VisionDiff = 0.074f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 14 },
			}
		};
		
		static public ImprovementCategory TechniqueCategory = new ImprovementCategory()
		{
			Description = "APRENDE LOS\nTRUCOS DE LOS MEJORES\nY MEJORA TU TÉCNICA",
			BackgroundPrefix = "TrainingTechniqueBackground",
			Items = new ImprovementItem[]
			{ 
				new ImprovementItem() { Name = "La Fohla Seca", Price = 1770, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0.059f, MotivationDiff = 0f, ImprovementItemID = 20 },
			 	new ImprovementItem() { Name = "El Amago Neutro", Price = 3094, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0.044f, MotivationDiff = 0f, ImprovementItemID = 21 },
				new ImprovementItem() { Name = "La Superchilena", Price = 4511, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0.051f, MotivationDiff = 0f, ImprovementItemID = 22 },
				new ImprovementItem() { Name = "La Roulette Marsellesa", Price = 9254, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0.015f, MotivationDiff = 0f, ImprovementItemID = 23 },
				new ImprovementItem() { Name = "El Aguanís", Price = 12788, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0.022f, MotivationDiff = 0f, ImprovementItemID = 24 },
			}
		};
		
		static public ImprovementCategory LockerRoomCategory = new ImprovementCategory()
		{
			Description = "AMPLÍA TU EQUIPACIÓN\nY MEJORARÁ TU\nRENDIMIENTO GLOBAL",
			BackgroundPrefix = "TrainingLockerRoomBackground",
			Items = new ImprovementItem[]
			{ 
				new ImprovementItem() { Name = "Botas clásicas", Price = 1198, VisionDiff = 0f, PowerDiff = 0.029f, TechniqueDiff= 0f, MotivationDiff = 0f, ImprovementItemID = 30 },
			 	new ImprovementItem() { Name = "Botas modernas", Price = 1928, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0.022f, MotivationDiff = 0f, ImprovementItemID = 31 },
				new ImprovementItem() { Name = "Botas Next Gen", Price = 3057, VisionDiff = 0f, PowerDiff = 0.015f, TechniqueDiff= 0.007f, MotivationDiff = 0f, ImprovementItemID = 32 },
				new ImprovementItem() { Name = "Botas Hi-Tech", Price = 6533, VisionDiff = 0f, PowerDiff = 0.0155f, TechniqueDiff= 0.015f, MotivationDiff = 0.015f, ImprovementItemID = 33 },
				new ImprovementItem() { Name = "Botas Evolution", Price = 22788, VisionDiff = 0f, PowerDiff = 0.022f, TechniqueDiff= 0.015f, MotivationDiff = 0.015f, ImprovementItemID = 34 },
			}
		};
		
		static public ImprovementCategory EventsCategory = new ImprovementCategory()
		{
			Description = "MEJORA TU MOTIVACIÓN\nY AUMENTARÁ TU\nRENDIMIENTO GLOBAL.",
			BackgroundPrefix = "LifeEventsBackground",
			Items = new ImprovementItem[]
			{ 
				new ImprovementItem() { Name = "Firma de autógrafos", Price = 759, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.015f, ImprovementItemID = 40 },
			 	new ImprovementItem() { Name = "Rueda de prensa", Price = 1031, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.007f, ImprovementItemID = 41 },
				new ImprovementItem() { Name = "Acto benéfico", Price = 1841, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.022f, ImprovementItemID = 42 },
				new ImprovementItem() { Name = "Sesión puertas abiertas", Price = 4692, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.029f, ImprovementItemID = 43 },
				new ImprovementItem() { Name = "Acto Fundación Real Madrid ", Price = 8012, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.015f, ImprovementItemID = 44 },
			}
		};
		
		static public ImprovementCategory PropertiesCategory = new ImprovementCategory()
		{
			Description = "MEJORA TU MOTIVACIÓN\nY AUMENTARÁ TU\nRENDIMIENTO GLOBAL.",
			BackgroundPrefix = "LifePropertiesBackground",
			Items = new ImprovementItem[]
			{ 
				new ImprovementItem() { Name = "Ático en la ciudad", Price = 1497, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.007f, ImprovementItemID = 50 },
				new ImprovementItem() { Name = "Residencia exclusiva", Price = 2535, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.015f, ImprovementItemID = 51 },
				new ImprovementItem() { Name = "Casa de campo", Price = 3650, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.022f, ImprovementItemID = 52 },
				new ImprovementItem() { Name = "Villa en la playa", Price = 7422, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.015f, ImprovementItemID = 53 },
				new ImprovementItem() { Name = "Chalet en estación de esquí", Price = 10230, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.015f, ImprovementItemID = 54 },
			}
		};
		
		static public ImprovementCategory VehiclesCategory = new ImprovementCategory()
		{
			Description = "MEJORA TU MOTIVACIÓN\nY AUMENTARÁ TU\nRENDIMIENTO GLOBAL.",
			BackgroundPrefix = "LifeVehiclesBackground",
			Items = new ImprovementItem[]
			{ 
				/*
				Precio antiguo de Los coches

				new ImprovementItem() { Name = "Utilitario", Price = 351, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.007f, ImprovementItemID = 60 },
				new ImprovementItem() { Name = "Minivan", Price = 557, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.007f, ImprovementItemID = 61 },
				new ImprovementItem() { Name = "SUV", Price = 940, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.007f, ImprovementItemID = 62 },
				new ImprovementItem() { Name = "Deportivo", Price = 3177, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.015f, ImprovementItemID = 63 },
				new ImprovementItem() { Name = "Clásico", Price = 8905, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.007f, ImprovementItemID = 64 },

				Suma de todos los precios de los coches: 13.930

				*/

				//Precio nuevo de los coches:

				new ImprovementItem() { Name = "Utilitario", Price = 1177, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.007f, ImprovementItemID = 60 },
				new ImprovementItem() { Name = "Minivan", Price = 2210, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.007f, ImprovementItemID = 61 },
				new ImprovementItem() { Name = "SUV", Price = 5570, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.014f, ImprovementItemID = 62 },
				new ImprovementItem() { Name = "Deportivo", Price = 8905, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.015f, ImprovementItemID = 63 },
				new ImprovementItem() { Name = "Clásico", Price = 9400, VisionDiff = 0f, PowerDiff = 0f, TechniqueDiff= 0f, MotivationDiff = 0.015f, ImprovementItemID = 64 },

				//Suma de todos los precios de los coches despues del cambio: 30.562

			}
		};
	}
	
	public class ImprovementCategory
	{
		public string Description { get; set; }
		public string Background { get; set; }
		public string BackgroundPrefix { get; set; }
		public ImprovementItem[] Items { get; set; }
	}

	public class ImprovementItem
	{
		public int ImprovementItemID;
		public string Name;
		public int Price;
		public float VisionDiff;
		public float PowerDiff;
		public float TechniqueDiff;
		public float MotivationDiff;
		public Texture2D Background;
	}
	
	[JsonObject(MemberSerialization.OptOut)]
	public class Improvements
	{
		public List<int> PurchasedImprovementItemIDs  = new List<int>();
		
		public Improvements()
		{
		}
							
		public void BuyImprovement(ImprovementItem theItem)
		{
			if (IsItemAlreadyPurchased(theItem))
				throw new Exception("WTF 3033 - Item already bought");
						
			PurchasedImprovementItemIDs.Add(theItem.ImprovementItemID);
		}
		
		public bool IsItemAlreadyPurchased(ImprovementItem theItem)
		{
			return PurchasedImprovementItemIDs.Contains(theItem.ImprovementItemID);
		}
	}
}

