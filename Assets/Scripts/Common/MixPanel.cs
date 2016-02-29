using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using JsonFx.Json;
using UnityEngine;
using FootballStar.Manager.Model;

public enum AnalyticEvent
{
	GAME_START = 0,
	SCREEN_VIEW,
	MATCH_START,
	MATCH_END,
	MATCH_REPLAY,
	MATCH_TAP,

	TUTORIAL,
	BUY_IMPROVEMENT,

	NO_ENERGY,
	
	SESSION_TIME,

	
	ANALYTIC_EVENT_END
}


public class MixPanel : MonoBehaviour 
{	
	// Set to true to enable debug logging.
	public bool EnableLogging;	
	public string BuildVersion = "1.2";

	// Add any custom "super properties" to this dictionary. These are properties sent with every event.
	public Dictionary<string, object> SuperProperties = new Dictionary<string, object>();

	void Awake()
	{
		InitializeListOfEventNames();

		var mixPanel = GameObject.FindObjectsOfType(typeof(MixPanel));
		
		if (mixPanel.Length != 1)
			throw new Exception("WTF 9123 - Only 1 MixPanel instance is Allowed");
		
		mMixPanelInstance = mixPanel[0] as MixPanel;


		if( GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>().FinalVersion )
		{
			Token = (Debug.isDebugBuild) ? mFootballStarToken_Dev : mFootballStarToken;
		}
		else
		{
			Token = mFootballStarToken_Dev;
		}

		if(Debug.isDebugBuild)
			Debug.Log("Ejecutando una DebugBuild - Dirigiendo eventos de Mixpanel a FootballStars_Dev");


		DistinctID = SystemInfo.deviceUniqueIdentifier;
		
		// Set some "super properties" to be sent with every event.
		SuperProperties.Add("Platform", Application.platform.ToString());
		SuperProperties.Add("Resolution", Screen.width + "x" + Screen.height);
		SuperProperties.Add("Device", SystemInfo.deviceModel);
		SuperProperties.Add("DeviceType", SystemInfo.deviceType);
		SuperProperties.Add("AppVersion", BuildVersion);
		
		SendEventToMixPanel(AnalyticEvent.GAME_START);

	}

	void Start () {}	

	private void InitializeListOfEventNames()
	{
		AnaliticEventNames = new List<string>();
		foreach ( string str in Enum.GetNames( typeof(AnalyticEvent) ) )
		{
			AnaliticEventNames.Add(StringHumanReadableFormat(str));
		}
	}

	private static string StringHumanReadableFormat(string theString)
	{
		string resultString	= "";
		string[] theWords 	= theString.ToString().Split('_');
		
		for(int i = 0; i < theWords.Length; i++)
		{
			string capitalLetter = theWords[i].ToString().Substring(0,1).ToUpper();
			string stringBody    = theWords[i].ToString().ToLower().Substring(1);
			
			resultString += capitalLetter + stringBody + " ";
		}
		
		return resultString.Trim();
	}

	// Call this to send an event to Mixpanel.
	// eventName: The name of the event. (Can be anything you'd like.)
	public static void SendEventToMixPanel(AnalyticEvent eventName)
	{
		SendEventToMixPanel(eventName, null);
	}
	
	public static void SendEventToMixPanel(AnalyticEvent evt, IDictionary<string, object> properties)
	{
#if !UNITY_WEBPLAYER
		SendEvent(mMixPanelInstance.AnaliticEventNames[(int)evt], properties);
#endif
	}

	// Call this to send an event to Mixpanel.
	// eventName: The name of the event. (Can be anything you'd like.)
	// properties: A dictionary containing any properties in addition to those in the Mixpanel.SuperProperties dictionary.
	private static void SendEvent(string eventName, IDictionary<string, object> properties)
	{
		if(string.IsNullOrEmpty( mMixPanelInstance.Token))
		{
			Debug.LogError("Attempted to send an event without setting the Mixpanel.Token variable.");
			return;
		}
		
		if(string.IsNullOrEmpty(mMixPanelInstance.DistinctID))
		{
			if(!PlayerPrefs.HasKey("mixpanel_distinct_id"))
				PlayerPrefs.SetString("mixpanel_distinct_id", Guid.NewGuid().ToString());
			mMixPanelInstance.DistinctID = PlayerPrefs.GetString("mixpanel_distinct_id");
		}
		
		StringWriter sw = new StringWriter();
		JsonTextWriter writer = new JsonTextWriter(sw);
		writer.WriteStartObject();
		{
			writer.WritePropertyName("event");
			writer.WriteValue(eventName);
			
			writer.WritePropertyName("properties");
			writer.WriteStartObject();
			{
				writer.WritePropertyName("distinct_id");
				writer.WriteValue(mMixPanelInstance.DistinctID);
				
				writer.WritePropertyName("token");
				writer.WriteValue(mMixPanelInstance.Token);

				foreach( KeyValuePair<string, object> sp in mMixPanelInstance.SuperProperties)
				{
					writer.WritePropertyName(sp.Key);
					WriteValue(writer, sp.Value);
				}
				if(properties != null)
				{
					foreach( KeyValuePair<string, object> p in properties)
					{
						writer.WritePropertyName(p.Key);
						WriteValue(writer, p.Value);
					}
				}
			}
			writer.WriteEndObject();
		}
		writer.WriteEndObject();
		
		
		string jsonStr = sw.ToString();

		if(mMixPanelInstance.EnableLogging)
			Debug.Log("Sending mixpanel event: " + jsonStr);

		string jsonStr64 = EncodeTo64(jsonStr);
		string url = string.Format(API_URL_FORMAT, jsonStr64);
		mMixPanelInstance.StartCoroutine(SendEventCoroutine(url));
	}
	
	private static string EncodeTo64(string toEncode)
	{
		var toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
		var returnValue = Convert.ToBase64String(toEncodeAsBytes);
		return returnValue;
	}

	private static IEnumerator SendEventCoroutine(string url)
	{
		WWW www = new WWW(url);
		yield return www;
		if(www.error != null)
			Debug.LogWarning("Error sending mixpanel event: " + www.error);
		else if(www.text.Trim() == "0")
			Debug.LogWarning("Error on mixpanel processing event: " + www.text);
		else if(mMixPanelInstance.EnableLogging)
			Debug.Log("Mixpanel processed event: " + www.text);
	}

	public static void WriteValue(JsonTextWriter writer, object val)
	{
		if (val == null)
			writer.WriteNull();
		else if (val is string)
			writer.WriteValue((string)val);
		else if (val is bool)
			writer.WriteValue((bool)val);
		else if (val is int)
			writer.WriteValue((int)val);
		else if (val is double)
			writer.WriteValue((double)val);
		
		else
			writer.WriteValue(val.ToString());
	}

	// Tokens of the MixPanel Projects
	const string mFootballStarToken_Dev = "29ac6216cf12ef84e9d093b1c84cb9f3";
	const string mFootballStarToken     = "89ff135120c561a92a420c2095b0b5ab";

	// The distinct ID of the current user.
	private string DistinctID;	
	// Set this to your Mixpanel token.
	private string Token;
	// The MixPanel Querystring Format
	private const string API_URL_FORMAT = "http://api.mixpanel.com/track/?data={0}";
	// Singletone Instance
	private static MixPanel mMixPanelInstance;
	// List of HumanReadableNames os the events
	private List<string> AnaliticEventNames;
}
