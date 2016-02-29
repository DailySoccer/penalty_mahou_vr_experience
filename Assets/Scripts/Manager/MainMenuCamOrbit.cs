using UnityEngine;
using System.Collections;

public class MainMenuCamOrbit : MonoBehaviour 
{
	public Transform Target;
	public float     Distance = 6;
	public float 	 Speed = 10.0f;
	public float     VerticalAngle = 0;
	
	void Start()
	{
	
	}
	
	void LateUpdate()
	{	
		if (Target != null)
		{
			mX += Speed * Time.deltaTime;
	         		       
	        Quaternion rotation = Quaternion.Euler(VerticalAngle, mX, 0);
	        Vector3 position = rotation * new Vector3(0.0f, 1.0f, -Distance) + Target.position;
			
	        transform.rotation = rotation;
	        transform.position = position;
		}
	}
	
	float mX = 0.0f;
}
