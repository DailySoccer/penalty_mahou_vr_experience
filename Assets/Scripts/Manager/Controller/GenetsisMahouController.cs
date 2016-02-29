using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using FootballStar.Manager.Model;
using FootballStar.Manager;
using FootballStar.Common;
using System.Text.RegularExpressions;

public class GenetsisMahouController : MonoBehaviour {
	
	public GameObject WelcomeScreen;
	public GameObject LoginScreen;
	public GameObject RegisterScreen;
	public GameObject RewardsScreen;
	public GameObject AddPincodesScreen;
	public GameObject DataValidationScreen;
	public GameObject RememberPasswordScreen;
	public GameObject RegisterWithFBButon;

	public UIButtonMessage CloseMeButton;
	
	private const string KEY_COINS_REDEEMED = "COINS_REDEEMED";
	private const string PENDING_REWARD     = "PENDING_REWARD"; // clave para identificar en PlayerPrefs si el usuario ha redimido un codigo(boolean)
	
	public string GenetsisScreenTag 		= "GenetsisScreen";
	public string TextoPoliticas;
	
	/****  Variables del registro ****/
	public UIInput RegName;
	public UIInput RegSurName;
	public UIInput RegPassword;
	public UIInput RegMail;
	public UIToggle RegAceptaMailing;
	public UIToggle RegAceptaPolitics;
	public UIInput RegDay;
	public UIPopupList RegMonth;
	private string RegMonthNumericString;
	
	public UIInput RegYear;
	
	
	/****  Variables del login ****/
	public UIInput LoginMail;
	public UIInput LoginPassword;
	
	/****  Variables de Pincode ****/
	public UIInput PinCodeString;
	
	/**** Variable para mensajes de error ****/
	public UILabel ValidationMessage;
	
	/***** Variables para recordar password *****/
	public UIInput rmmberPass;
	
	public event EventHandler OnGenetsisClose;
	
	void Start(){}
	
	void Awake()
	{
		ScreenList.AddRange(GameObject.FindGameObjectsWithTag(GenetsisScreenTag));
		HideAllScreens();
		mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
		if ( PlayerPrefs.HasKey (PENDING_REWARD) ) 
		{
			if( PlayerPrefs.GetInt(PENDING_REWARD) > 0){
				ChangeScreen (RewardsScreen.name);
			} else {
				ChangeToInitialScreen();
			}
		} else {
			ChangeToInitialScreen();
		}
	}
	
	private void ChangeToInitialScreen()
	{
		switch (mMainModel.Genetsis.Status) 
		{
		case FootballStar.Common.GenetsisStatus.USER_ONLINE:	
			ChangeScreen (AddPincodesScreen.name);
			break;
		case FootballStar.Common.GenetsisStatus.USER_OFFLINE:
			ChangeScreen (WelcomeScreen.name);
			break;
		default:
			ChangeScreen (WelcomeScreen.name);
			break;
		}
	}
	
	public void ChangeScreen(string screen)
	{
		// Solo mostramos la pantalla de premios si el user no ha elegido ya el premio de la pasta 5 veces.
		if ( PlayerPrefs.HasKey (KEY_COINS_REDEEMED) ) 
		{
			int totalCoinsRedeemed = PlayerPrefs.GetInt(KEY_COINS_REDEEMED);
			if (totalCoinsRedeemed >= 5 && screen == RewardsScreen.name)
				screen = "NoCoins" +  screen;
		}
		
		mLastScreen = mCurrentScreen;
		StartCoroutine(ChangeScreenControllerCoRoutine(screen));
		mCurrentScreen = screen;
	}
	
	private IEnumerator ChangeScreenControllerCoRoutine(string screenName)
	{
		yield return new WaitForEndOfFrame();
		
		foreach(var screen in ScreenList)
		{
			screen.SetActive(screen.name == screenName);
		}

		#if UNITY_WEBPLAYER
			if(screenName == RegisterScreen.name)
			{
				RegisterWithFBButon.SetActive(false);
				OnRegisterWithFB();
			}
		#endif
	}
	
	private void HideAllScreens()
	{
		foreach(var screen in ScreenList)
		{
			screen.SetActive(false);
		}
	}
	
	private string GetPreviousScreen(string previousScreenName)
	{
		string retorno = WelcomeScreen.name;
		switch (previousScreenName) 
		{
		case "LoginScreen":
		case "RegisterScreen":
			retorno = WelcomeScreen.name;
			break;
			
		case "RewardsScreen":
		case "AddPinCodesScreen":
			OnGenetsisClose(this, EventArgs.Empty);
			break;
			
		case "RememberPasswordScreen":
			retorno = LoginScreen.name;
			break;
			
		case "DataValidationScreen":
			retorno = mLastScreen;
			break;
		}
		return retorno;
	}
	
	public void OnGoToLogin()
	{
		#if UNITY_WEBPLAYER
			OnLoginWithFB();
		#else
			ChangeScreen(LoginScreen.name);
		#endif

	}
	
	public void OnGoToPrevious()
	{
		ChangeScreen (GetPreviousScreen(mCurrentScreen));
	}
	
	public void OnGoToRegister()
	{
		ChangeScreen(RegisterScreen.name);
	}
	
	public void OnCloseRemember()
	{
		ChangeScreen(WelcomeScreen.name);
	}
	
	public void OnRegister()
	{
		bool validParameters = true;
		string name ="";
		string surName="";
		
		if (validParameters) {
			if( RegName.value == string.Empty )
			{
				GotoValidationScreen("Inserta un nombre valido.\n\n Ej:'Nombre'" );
				return;
			}
			else
			{
				name = RegName.value;
				validParameters = true;
			}
		}
		
		if (validParameters) {
			if (RegSurName.value == string.Empty) {
				GotoValidationScreen ("Inserta un nombre válido.\n\n Ej:'Apellido'");
				validParameters = false;
				return;
			} else {
				surName = RegSurName.value;
				validParameters = true;
			}
		}
		
		if (validParameters) {
			if( !IsValidEmail( RegMail.value.Trim() ) ){
				GotoValidationScreen("El mail introducido no tiene el formato esperado:\n\n Ej:'nombre@dominio.ext'");
				validParameters = false;
				return;
			}
			else{
				validParameters = true;
			}
		}
		if (validParameters) {
			if (RegPassword.value == string.Empty) {
				GotoValidationScreen ("No has introducido una contraseña válida o has dejado el campo vacío.");
				validParameters = false;
				return;
			} else {
				validParameters = true;
			}
		}
		
		int day = 0;
		if (validParameters) {
			if (int.TryParse (RegDay.value, out day))
				validParameters = true;
			else {
				GotoValidationScreen ("Introduce el día de tu nacimiento");
				validParameters = false;
			}
		}
		
		if (validParameters) {
			//Si el dia es valido para ese mes.
			if ( IsValidDayOfTheMonth (day, RegMonth.value) )
				validParameters = true;
			else {
				GotoValidationScreen ("El día que has introducido no es válido para el mes seleccionado");
				validParameters = false;
			}
		}
		
		int year = 0;
		if (validParameters) {
			if (int.TryParse (RegYear.value, out year))
				validParameters = true;
			else {
				GotoValidationScreen ("Introduce el día de tu nacimiento");
				validParameters = false;
			}
		}
		
		if (validParameters) {
			// Si el año esta entre 1900 y el actual
			if (year >= 1900 && 
			    year <= DateTime.Now.Year) 
				validParameters = true;
			else {
				GotoValidationScreen ("El año indicado no es válido. \n\n Los años válidos van desde 1900 hasta el actual");
				validParameters = false;
			}
		}
		
		if (validParameters) {
			if (IsValidDate (day.ToString (), GetMonthIndex (RegMonth.value), year.ToString ())) 
				validParameters = true;
			else
			{
				GotoValidationScreen ("La fecha de nacimiento no es válida.:");
				validParameters = false;
			}
			
		}
		
		if (validParameters) {
			if (RegAceptaPolitics.value != true) {
				GotoValidationScreen ("Para continuar has de aceptar la politica de privacidad.");
				validParameters = false;
				return;
			} else {
				validParameters = true;
			}
		}
		
		if(validParameters)
		{

			mMainModel.Genetsis.OnResult += OnRegisterCallBack;
			mMainModel.Genetsis.RegistrarUsuario(name, surName, RegMail.value.Trim(), RegPassword.value, RegAceptaMailing.value, FB.IsLoggedIn ? FB.UserId : "");
		}
		
		
	}
	
	void OnRegisterCallBack(object sender, EventGenetsisResultArgs  e)
	{
		mMainModel.Genetsis.OnResult -= OnRegisterCallBack;
		if (!e.Result)
		{
			GotoValidationScreen(e.Msg);
		}
		else
		{
			ChangeScreen(AddPincodesScreen.name);
		}
	}
	
	void OnGoToRememberPassword()
	{
		ChangeScreen(RememberPasswordScreen.name);
	}
	
	void OnRememberPassword()
	{
		if (IsValidEmail(rmmberPass.value.Trim()))
		{
			mMainModel.Genetsis.OnResult += OnRememberPasswordCallback;
			mMainModel.Genetsis.RecordatorioPassword(rmmberPass.value.Trim());
		}
		else
		{
			GotoValidationScreen("El mail introducido no tiene el formato esperado:\n\n 'nombre@dominio.ext'");
		}
	}
	
	public void OnRememberPasswordCallback(object sender, EventGenetsisResultArgs e)
	{
		mMainModel.Genetsis.OnResult -= OnRememberPasswordCallback;
		if (e.Result)
		{
			ChangeScreen(WelcomeScreen.name);
		}
		else{
			GotoValidationScreen(e.Msg);
		}
	}
	
	
	public void OnLogin()
	{
		if( IsValidEmail( LoginMail.value.Trim() ) )
		{
			mMainModel.Genetsis.OnResult += OnLoginCallBack;
			mMainModel.Genetsis.LoginUsuario(LoginMail.value, LoginPassword.value);
		}
		else
		{
			GotoValidationScreen("El mail introducido no tiene el formato esperado:\n\n 'nombre@dominio.ext'");
		}
	}
	
	void OnLoginCallBack(object sender, EventGenetsisResultArgs  e)
	{
		//Debug.Log ("4.- Respuesta de Genetsis");
		mMainModel.Genetsis.OnResult -= OnLoginCallBack;
		
		if (e.Result)
		{
			ChangeScreen(AddPincodesScreen.name);
		}
		else
		{
			GotoValidationScreen(e.Msg);
			
		}
	}
	
	public void OnUsePincode()
	{
		
		if (PinCodeString.value != string.Empty) {
			mMainModel.Genetsis.OnResult += OnUsePincodeCallBack;
			mMainModel.Genetsis.UsarPincode (PinCodeString.value);
		} else
			Debug.Log ("El campo de texto esta vacio");
	}
	
	public void OnUsePincodeCallBack(object sender, EventGenetsisResultArgs e)
	{
		mMainModel.Genetsis.OnResult -= OnUsePincodeCallBack;
		if (e.Result)
		{
			//Guardamos en las preferencias que el usuario ha canjeado un pincode
			PlayerPrefs.SetInt (PENDING_REWARD, 1);
			ChangeScreen(RewardsScreen.name);
		}
		else{
			GotoValidationScreen(e.Msg);
		}
	}
	
	public void GotoValidationScreen(string msg)
	{
		ValidationMessage.text = msg;
		ChangeScreen(DataValidationScreen.name);
	}
	
	public void OnGetMoney()
	{
		if (!PlayerPrefs.HasKey (KEY_COINS_REDEEMED)) 
		{
			Debug.Log("Se ha elegido por primera vez la pasta");
			PlayerPrefs.SetInt (KEY_COINS_REDEEMED, 1);
		} else {
			int count = (PlayerPrefs.GetInt (KEY_COINS_REDEEMED) + 1);
			Debug.Log("Se ha elegido por " + count + "vez la pasta");
			PlayerPrefs.SetInt (KEY_COINS_REDEEMED, count);
		}
		mMainModel.AddMoney(500);
		mMainModel.SaveDefaultGame ();
		PlayerPrefs.SetInt (PENDING_REWARD, 0);
		OnGenetsisClose(this, EventArgs.Empty);
	}
	
	public void OnGetEnergy()
	{
		mMainModel.AddEnergyUnit(10);
		mMainModel.SaveDefaultGame ();
		PlayerPrefs.SetInt (PENDING_REWARD, 0);
		OnGenetsisClose(this, EventArgs.Empty);
	}
	
	public void OnShowPolitics()
	{
#if UNITY_WEBPLAYER
		Application.ExternalEval("window.open('http://download.unusualwonder.com/mahou/MahouPolitica.html','Política de privacidad y protección de datos personales')");
#else
		Application.OpenURL("http://download.unusualwonder.com/mahou/MahouPolitica.html");
#endif
	}

	public void OnShowLegalBases()
	{
#if UNITY_WEBPLAYER
		Application.ExternalEval("window.open('http://download.unusualwonder.com/mahou/MahouBasesLegales.html','Política de privacidad y protección de datos personales')");
#else
		/* Original de Mahou  */ Application.OpenURL("http://download.unusualwonder.com/mahou/MahouBasesLegales.html");
#endif
	}
	public void OnCloseGenetsisControllerClick()
	{
		OnGenetsisClose(this, EventArgs.Empty);
	}
	
	/***** FB SDK *****/
	public void OnLoginWithFB()
	{
		mCurrentFBActionRequest = FBActionRequest.LOGIN;
		//Debug.Log ("1.- El usuario quiere loguearse con FB");
		CallFBInit();
	}
	
	public void OnRegisterWithFB()
	{
		mCurrentFBActionRequest = FBActionRequest.REGISTER;
		CallFBInit();
	}
	
	private void CallFBInit()
	{
		//Debug.Log ("1.- Llamada a FBInit");
		if (!mMainModel.FacebookInitialized)
			FB.Init (OnFBInitComplete, OnHideUnity);
		else
			CallFBLogin ();
	}
	
	private void OnFBInitComplete()
	{
		//Debug.Log( string.Format("FB.Init completed: Is user logged in? {0} FB ID:{1}", FB.IsLoggedIn, FB.UserId) );
		mMainModel.FacebookInitialized = true;
		CallFBLogin ();
	}
	
	private void OnHideUnity(bool isGameShown)
	{
		Debug.Log("Is game showing? " + isGameShown);
	}
	
	private void CallFBLogin()
	{
		//Debug.Log ("2.- Llamada a FBLogin");
		if (!FB.IsLoggedIn)
			FB.Login (FacebookUtils.FB_PERMISSIONS, FBLoginCallback);
		else
			FBDataRequest();
	}
	
	void FBLoginCallback(FBResult result)
	{
		mLastFBResponse = "";
		if (result.Error != null)
			mLastFBResponse = "Error Response:\n" + result.Error;
		else if (!FB.IsLoggedIn) {
			mLastFBResponse = "Login cancelled by Player";
		}
		else {
			FBDataRequest();
		}
		
		if(mLastFBResponse != "") 
			Debug.Log( String.Format("Respuesta del login: {0}", mLastFBResponse) );
	}
	
	void FBDataRequest()
	{
		FB.API("/me?fields=id,first_name,last_name,email,birthday", Facebook.HttpMethod.GET, APICallback);
	}
	
	public static Dictionary<string, string> DeserializeJSONProfile(string response)
	{
		var responseObject = Facebook.MiniJSON.Json.Deserialize(response) as Dictionary<string, object>;
		object first_name, last_name, email, birthday;
		
		var profile = new Dictionary<string, string>();
		if (responseObject.TryGetValue("first_name", out first_name))
		{
			profile["first_name"] = (string)first_name;
		}
		if (responseObject.TryGetValue("last_name", out last_name))
		{
			profile["last_name"] = (string)last_name;
		}
		if (responseObject.TryGetValue("email", out email))
		{
			profile["email"] = (string)email;
		}
		if (responseObject.TryGetValue("birthday", out birthday))
		{
			profile["birthday"] = (string)birthday;
		}
		return profile;
	}
	
	void APICallback(FBResult result)                                                                                              
	{                                                                                                                              
		FbDebug.Log("APICallback");                                                                                                
		if (result.Error != null)                                                                                                  
		{                                                                                                                          
			FbDebug.Error(result.Error);                                                                                           
			// Let's just try again                                                                                                
			FB.API("/me?fields=id,first_name,last_name,email,birthday", Facebook.HttpMethod.GET, APICallback); 
			return;                                                                                                                
		}        
		
		//Debug.Log( String.Format("Resultado del login: {0}", result.Text) );
		
		Dictionary<string, string> profile = DeserializeJSONProfile(result.Text);  
		switch (mCurrentFBActionRequest) 
		{
		case FBActionRequest.LOGIN:
			mMainModel.Genetsis.OnResult += OnLoginCallBack;
			mMainModel.Genetsis.LoginFacebook(profile["email"], FB.UserId);
			//Debug.Log ("3.- Llamada a Login de Genetsis Mediante FB");
			break;
			
		case FBActionRequest.REGISTER:
			RegName.value 		= profile["first_name"]; 
			RegSurName.value	= profile["last_name"].ToString().Split(' ')[0];
			RegMail.value		= profile["email"];
			
			string[] birthday	= profile["birthday"].ToString().Split('/');
			if ( birthday.Length == 3 ) {
				Debug.Log( "Birthday: " + birthday[1] + " - " + birthday[0] + " - " + birthday[2] );
				RegDay.value	= int.Parse(birthday[1]).ToString();
				RegMonth.value	= MONTHS[ int.Parse(birthday[0]) - 1 ];
				RegYear.value	= birthday[2];
			}
			break;
		}
	}   
	/*
	private void CallFBPublishInstall()
	{
		FB.PublishInstall(FBPublishComplete);
	}
	
	private void FBPublishComplete(FBResult result)
	{
		Debug.Log("publish response: " + result.Text);
	}
	*/
	/***** FB SDK *****/
	
	/***** Functions *****/
	
	string[] MONTHS = { "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE" };
	
	public string GetMonthIndex(string monthName)
	{
		string retorno = "1";
		for ( int i=0; i<MONTHS.Length; i++ ) {
			if ( monthName.ToUpper() == MONTHS[i] ) {
				retorno = (i+1).ToString();
				break;
			}
		}
		/*
		switch (monthName.ToUpper()) 
		{
			case "FEBRERO": 	retorno = "2"; 	break;		
			case "MARZO": 		retorno = "3"; 	break;		
			case "ABRIL": 		retorno = "4"; 	break;		
			case "MAYO": 		retorno = "5"; 	break;		
			case "JUNIO": 		retorno = "6"; 	break;		
			case "JULIO": 		retorno = "7"; 	break;		
			case "AGOSTO": 		retorno = "8"; 	break;		
			case "SEPTIEMBRE": 	retorno = "9"; 	break;		
			case "OCTUBRE": 	retorno = "10"; 	break;		
			case "NOVIEMBRE":	retorno = "11"; 	break;		
			case "DICIEMBRE":	retorno = "12"; 	break;		
			default: 			retorno = "1";		break;	
		}
		*/
		return retorno;
	}
	
	private bool IsValidDayOfTheMonth(int day, string month)
	{
		bool retorno = false;
		var RegMonthNumericString = GetMonthIndex (month);
		
		// Si el dia es superior al maximo permitido en un mes establecemos el maximo
		switch (RegMonthNumericString) 
		{
		case "1": case "3": case "5": case "7": case "8": case "10": case "12":
			if( day > 0 && day <= 31 )
				retorno = true;
			else 
			{
				retorno = false;
			}
			break;
		case "4": case "6": case "9": case "11":
			if( day > 0 && day <= 30 )
				retorno = true;
			else 
			{
				retorno = false;
			}
			break;
		case "2":
			if( day > 0 && day <= 29 )
				retorno = true;
			else 
			{
				retorno = false;
			}
			break;
		}
		return retorno;
	}
	
	
	private static bool IsValidDate(string day, string month, string year) 
	{
		bool retorno = false;
		string [] format = new string []{"yyyy-m-d"};
		string value = year + "-" + month + "-" + day;
		
		DateTime datetime;
		if (DateTime.TryParseExact (value, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out datetime))
		{
			Console.WriteLine ("Valido  : " + datetime);
			retorno = true;
		}
		else
		{
			Console.WriteLine ("Invalido");
			retorno = false;
		}
		return retorno;
	}
	
	private static bool IsValidEmail(string inputEmail)
	{
		string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
			@"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + 
				@".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
		Regex re = new Regex(strRegex);
		if (re.IsMatch(inputEmail))
			return (true);
		else
			return (false);
	}
	
	private MainModel mMainModel;
	private string mCurrentScreen;
	private string mLastScreen;
	private string ValidationScreenMessage;
	private List<GameObject> ScreenList 	= new List<GameObject>();
	
	private string mLastFBResponse 			= "";
	FBActionRequest mCurrentFBActionRequest = FBActionRequest.LOGIN;
	private enum FBActionRequest
	{
		LOGIN,
		REGISTER
	}
	
	
	
}
