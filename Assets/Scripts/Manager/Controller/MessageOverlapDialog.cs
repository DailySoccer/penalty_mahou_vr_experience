using UnityEngine;
using System.Collections;

namespace FootballStar.Manager {
	
	public class MessageOverlapDialog : MonoBehaviour {
		
		void Awake() {
			mSmallTheater = GameObject.Find("SmallTheater").GetComponent<SmallTheater>();
		}
		void OnEnable() {
			mSmallTheater.DisableCamera();
		}
		
		void OnDisable() {
			mSmallTheater.EnableCamera();
		}
		
		SmallTheater mSmallTheater;
	}
	
}
