using UnityEngine;
using System.Collections;

public class ContinousRotation : MonoBehaviour 
{
	public float RotSpeed;
		
	void Update()
	{
		transform.Rotate(Vector3.up, Time.deltaTime * RotSpeed, Space.World);
	}
}
