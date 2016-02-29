using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Manager
{
	public class SmallTheater : MonoBehaviour
	{
		void Start() 
		{
			mChildren = GameObject.FindGameObjectsWithTag("SmallTheater");
			TheaterCam = transform.GetComponentInChildren<Camera>().gameObject;
			foreach (GameObject child in mChildren)
			{
				child.SetActive(false);
			}
		}
		
		public GameObject ShowObject(string name)
		{
			if (mCurrentObject != null)
			{
				if (mCurrentObject.name != name)
				{
					mCurrentObject.SetActive(false);
					mCurrentObject = null;	
				}				
			}
			
			if (mCurrentObject == null)
			{
				foreach (var go in mChildren)
				{
					if (go.name == name)
					{
						mCurrentObject = go;
						mCurrentObjectName = go.name;
						mCurrentObject.SetActive(true);
						break;
					}
				}
			}
			
			return mCurrentObject;
		}
		
		public void HideCurrentObject()
		{
			if (mCurrentObject != null)
			{
				mCurrentObject.SetActive(false);
				mCurrentObject = null;
			}
			else {
				// Olvidamos cual fue nuestro ultimo objeto si nos mandan a ocultar algo cuando no mostrabamos nada. 
				// Es decir, un doublehide olvida el ultimo objeto mostrado.
				mCurrentObjectName = null;
			}
		}

		public void DisableCamera()
		{
			if(TheaterCam != null)
				TheaterCam.SetActive(false);
		}

		public void EnableCamera()
		{
			if(TheaterCam != null)
				TheaterCam.SetActive(true);
		}

		public void ShowCurrentObject()
		{
			if (mCurrentObjectName != null)
				ShowObject(mCurrentObjectName);
		}

		GameObject TheaterCam;
		GameObject mCurrentObject;
		string mCurrentObjectName;
		GameObject[] mChildren;
	}
}
