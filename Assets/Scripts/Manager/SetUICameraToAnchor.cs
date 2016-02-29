using System;
using UnityEngine;

namespace FootballStar.Manager
{
	public class SetUICameraToAnchor : MonoBehaviour
	{		
		void Awake()
		{
			GetComponent<UIAnchor>().uiCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
		}
	}
}

