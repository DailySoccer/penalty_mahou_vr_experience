using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RadialProgressBar : MonoBehaviour {


	public float value = 1f;

	public UISprite foregroundItem;

	public void Show() {
		gameObject.SetActive(true);
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void Reset() {
		value = 1;
		foregroundItem.type = UISprite.Type.Filled;
		foregroundItem.fillDirection = UISprite.FillDirection.Radial360;
		foregroundItem.invert = false;
	}
	
	// Use this for initialization
	void OnEnable () {
		if (foregroundItem == null) { 
			Debug.LogError ("Not foreground item attached. Please provide me one");
		} else {
			Reset();
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (foregroundItem != null){
			value = Mathf.Clamp(value, 0, 1);
			foregroundItem.fillAmount = value;
		}
	}
}
