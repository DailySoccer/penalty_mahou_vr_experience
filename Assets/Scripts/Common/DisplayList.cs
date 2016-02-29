using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayList : MonoBehaviour {

	public List<GameObject> GameObjects;

	void Awake () {
		mVisibleIndex = -1;

		foreach( GameObject go in GameObjects ) {
			go.SetActive( false );
		}
	}

	void Start () {
		if ( GameObjects.Count > 0 ) {
			GameObjects[0].SetActive( true );
			mVisibleIndex = 0;
		}
	}

	void NextGameObject() {
		if ( mVisibleIndex >= 0 ) {
			GameObjects[mVisibleIndex].SetActive( false );

			mVisibleIndex++;

			if ( mVisibleIndex >= GameObjects.Count )
				mVisibleIndex = 0;

			GameObjects[mVisibleIndex].SetActive( true );
		}
	}

	void Update () {
		if (Input.GetKeyDown("space") || Input.GetMouseButtonDown(0)) {
			NextGameObject();
		}
	}

	private int mVisibleIndex;
}
