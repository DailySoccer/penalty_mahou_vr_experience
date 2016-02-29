using UnityEngine;
using System.Collections;
using JsonFx.Json;

public class TakeJSON : MonoBehaviour {
	public int NumInteractiveActions = 0;
	public int NumPases 	= 0;
	public int NumRegates 	= 0;
	public int NumChuts 	= 0;
	[HideInInspector] public string Text;

	public void Run () {
        AnimatorTimeline.JSONTake j = JsonReader.Deserialize<AnimatorTimeline.JSONTake>(Text);
        AnimatorTimeline.ParseJSONTake(j);
	}
	
}
