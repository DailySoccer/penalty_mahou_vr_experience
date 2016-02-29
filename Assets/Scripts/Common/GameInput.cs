using UnityEngine;
using System.Collections;

namespace FootballStar.Common
{
	public class GameInput {
	
		static int TouchCount() {
        	int count = 0;
       		foreach (Touch touch in Input.touches) {
            	if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                	count++;
        	}
			return count;
		}
		
		public static Vector3 TouchPosition {
			get { 
				Vector3 position = Vector3.zero;
				if ( Input.touches.Length > 0 ) {
		       		foreach (Touch touch in Input.touches) {
		            	if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled) {
							position = touch.position;
							break;
						}
		        	}
				}
				else {
					position = Input.mousePosition;
				}
				return position; 
			}
		}
		
		public static bool IsTouchUp() {
			int fingerCount = TouchCount();
			return (fingerCount == 0) && Input.GetMouseButtonUp(0);
		}
		
		public static bool IsTouchDown() {
			int fingerCount = TouchCount();
			return (fingerCount > 0) || Input.GetMouseButtonDown(0);
		}
		
	}
	
}
