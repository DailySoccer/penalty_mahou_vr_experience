using UnityEngine;
using System;
using System.Collections;
 
public class CameraFade : MonoBehaviour
{	
	void Awake()
	{
		mFadeTexture = new Texture2D(1, 1);
		mFadeTexture.SetPixel(0, 0, Color.black);
		mFadeTexture.Apply();
				
		var camFade = GameObject.FindObjectsOfType(typeof(CameraFade));
		
		if (camFade.Length != 1)
			throw new Exception("WTF 9123 - Only 1 CameraFade allowed");

		mCameraFadeInstance = camFade[0] as CameraFade;
						
		mScreenRect = new Rect(-10, -10, Screen.width + 20.0f, Screen.height + 20.0f);
		mTextureRect = new Rect(0, 0, 1, 1);
	}
 	
	void OnGUI()
	{
		if (mCurrentColor.a > 0 && Event.current.type.Equals(EventType.Repaint))
		{
			Graphics.DrawTexture(mScreenRect, mFadeTexture, mTextureRect,  0, 0, 0, 0, mCurrentColor);
		}
	}
	
	void Update()
	{		
		if (mTimeStart < 0.0f || Time.time < mTimeStart)
			return;
		
		if (Time.time < mTimeStart + mDuration)			
			mCurrentColor += mDeltaColor * Time.deltaTime;
		else
			FadeEnd();
	}
	
	void FadeEnd()
	{
		mCurrentColor = mTargetColor;
		mTimeStart = -1.0f;
		
		// Nos desactivamos despues de un fadein, para asegurar que no consumimos OnGUI
		if (mIsFadeIn)
			enabled = false;
	}
		
	void FadeStart(bool isFadeIn, float duration, float delay)
	{
		enabled = true;
		
		if (duration <= 0.0f)
			Debug.Log("CameraFade - TODO");
		
		mDuration = duration;
		mTimeStart = Time.time + delay;
		mIsFadeIn = isFadeIn;

		if (mIsFadeIn)
		{
			mCurrentColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
			mTargetColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
		} 
		else 
		{
			mCurrentColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
			mTargetColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);			
		}
		
		mDeltaColor = (mTargetColor - mCurrentColor) / duration;
		
		var uiCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<UICamera>();
		
		// Durante un Fade Out no admitimos input, y solo volvemos a admitir cuando nos mandan a hacer Fade In
		if (!mIsFadeIn)
			uiCamera.enabled = false;
		else
			uiCamera.enabled = true;
	}

	static public void Fade(bool isFadeIn, float duration, float delay)
	{
		mCameraFadeInstance.FadeStart(isFadeIn, duration, delay);
	}
	
	static public IEnumerator FadeCoroutine(bool isFadeIn, float duration, float delay)
	{		
		mCameraFadeInstance.FadeStart(isFadeIn, duration, delay);
		
		while (mCameraFadeInstance.mTimeStart > -1.0f)
			yield return null;
	}

	static public bool Enabled
	{
		get { return mCameraFadeInstance.enabled; }
		set { mCameraFadeInstance.enabled = value; }
	}
	
	private static CameraFade mCameraFadeInstance;
	
	Texture2D mFadeTexture;
	
	Color mCurrentColor = new Color(0,0,0,0);
	Color mTargetColor = new Color(0,0,0,0);
	Color mDeltaColor = new Color(0,0,0,0);
	
	float mTimeStart = -1.0f;
	float mDuration = -1.0f;
	bool mIsFadeIn = false;
	
	Rect mScreenRect;
	Rect mTextureRect;
}