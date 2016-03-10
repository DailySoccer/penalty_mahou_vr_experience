using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour {
    public Transform target;
    
    void Update() {
		Vector3 RectifiedTarget = target.position;
		RectifiedTarget.y = 0;
    	// Rotate the camera every frame so it keeps looking at the target 
		transform.LookAt(RectifiedTarget);
    }
}