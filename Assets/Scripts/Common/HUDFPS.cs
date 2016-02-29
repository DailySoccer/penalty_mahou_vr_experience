using UnityEngine;
using System.Collections;
 
[AddComponentMenu( "Utilities/HUDFPS")]
public class HUDFPS : MonoBehaviour
{
	// Attach this to any object to make a frames/second indicator.
	//
	// It calculates frames/second over each updateInterval,
	// so the display does not keep changing wildly.
	//
	// It is also fairly accurate at very low FPS counts (<10).
	// We do this not by simply counting frames per interval, but
	// by accumulating FPS for each frame. This way we end up with
	// corstartRect overall FPS even if the interval renders something like
	// 5.5 frames.
 
	public Rect startRect = new Rect( 10, 10, 75, 50 ); // The rect the window is initially displayed at.
	public  float frequency = 0.5F; // The update frequency of the fps
	public int nbDecimal = 1; // How many decimal do you want to display
	 
	private float accum   = 0f; 		// FPS accumulated over the interval
	private float frames  = 0f; 		// Frames drawn over the interval
	private float nextUpdateTime = 0.0f;
	private float lastTime = 0.0f;
	private string sFPS = ""; 			// The fps formatted into a string.
	private GUIStyle style;
 
	void Start()
	{
		nextUpdateTime = Time.realtimeSinceStartup + frequency;
	}
 
	void Update()
	{
		accum  += Time.realtimeSinceStartup - lastTime;
		frames += 1.0f;
		
		lastTime = Time.realtimeSinceStartup;
	    		
		if (Time.realtimeSinceStartup >= nextUpdateTime)
		{
		    float fps = frames / accum;
		    sFPS = fps.ToString( "f" + Mathf.Clamp( nbDecimal, 0, 10 ) );
 
	        accum = 0.0f;
	        frames = 0;
			
			nextUpdateTime = Time.realtimeSinceStartup + frequency;
		}
	}

	void OnGUI()
	{
		// Copy the default label skin, change the color and the alignement
		if( style == null ){
			style = new GUIStyle( GUI.skin.label );
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleCenter;
		}
 
		GUI.Label( new Rect(startRect.x, startRect.y, startRect.width, startRect.height), sFPS + " FPS", style );
	}
}