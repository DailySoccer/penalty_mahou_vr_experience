using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Common {
	public class EquipacionesCache : MonoBehaviour {
		void Start () {
		
		}
		
		// El dorsal es el del nuestro equipo siempre. Como la textura del portero es la misma para ambos y se cachea, el dorsal debe coincidir
		public void AddToCache(Texture2D equipacion, int dorsal) {
			mEquipacionesCache[dorsal] = equipacion;
		}
		
		public Texture2D GetFromCache(int dorsal) {
			return mEquipacionesCache[dorsal];
		}
		
		public bool IsEmpty {
			get { return mEquipacionesCache.Count == 0; }
		}
		
		Dictionary<int, Texture2D> mEquipacionesCache = new Dictionary<int, Texture2D>();
	}
}
