using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


class TextureSwapAnimator : MonoBehaviour {
	public Texture2D[] frames;
	
	[SerializeField]
	float framesPerSecond = 10.0f;

	void Start() {
		SortFrameByName ();
	}

	void SortFrameByName() {
		List<Texture2D> list = frames.ToList ();
		list.Sort((t1,t2) => t1.name.CompareTo(t2.name));
		frames = list.ToArray();
	}

	void Update () {
		int index = (int)(Time.time * framesPerSecond);
		index = index % frames.Length;
		GetComponent<Renderer>().material.mainTexture = frames[index];
	}

}