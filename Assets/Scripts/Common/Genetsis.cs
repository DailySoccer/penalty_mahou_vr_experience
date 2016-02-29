using UnityEngine;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.IO;
using SimpleJSON;
using FootballStar.Manager.Model;

namespace FootballStar.Common {

	[System.Serializable]
	public class MahouPromocion {
		public int ID;
		public string Token;
	}

	public enum GenetsisType {
		REGISTER_USER,
		LOGIN_USER,
		REMEMBER_USER_PASSWORD,
		VALIDATE_PINCODE,
		USE_PINCODE,
		ACTION_PLAY
	}

	public enum GenetsisStatus {
		USER_UNKNOWN,
		USER_ONLINE,
		USER_OFFLINE
	}
	
	public class EventGenetsisResultArgs : EventArgs {
		public GenetsisType Type;
		public bool Result;
		public string Msg;

		public EventGenetsisResultArgs(GenetsisType type) { Type = type; }
	}

	/*
	 * GENETSIS
	 * 
	 * AUTENTICACIÓN:
	 * - SIN usuario logado:   	id. promoción + token promoción 
	 * - CON usuario logado:	id. promoción + token usuario + token promoción
	 * 
	 * Parámetro: Authorization
	 * Codificación: SHA-1
	 */
	public class Genetsis : MonoBehaviour {

		const string KEY_EMAIL = "USER_EMAIL";
		const string KEY_PASSWORD = "USER_PASSWORD";
		const string KEY_FACEBOOK = "USER_FACEBOOK";

		const string ERROR_ACCESS_GENERIC = "Problemas para conectar con el servidor";

		public string URLServicioWeb;
		public MahouPromocion RegistroMovil;
		public MahouPromocion Acciones;
		public MahouPromocion Pincodes;
		public bool ModoTEST = true;
		public bool LoginAutomatico = true;

		public GenetsisStatus Status {
			get { return mStatus; }
		}

		public event EventHandler<EventGenetsisResultArgs> OnResult;

		void Awake () {
			mStatus = GenetsisStatus.USER_UNKNOWN;

			if ( ModoTEST ) {
				Debug.Log( "Genetsis TEST" );
				URLServicioWeb 		= "http://testmahou.serviciosmsm.com";
				RegistroMovil.ID 	= 1270;
				RegistroMovil.Token = "DF7EB04B-8049-423A-9884-3F7267927DEB";
				Acciones.ID 		= 1271;
				Acciones.Token 		= "CDAF242D-2846-4BA4-BF28-45472F324D69";
				Pincodes.ID 		= 1706;
				Pincodes.Token 		= "0E74707A-C0E7-40C1-B83D-F8782E654B20";
			}
			else {
				Debug.Log( "Genetsis PRODUCCION" );
			}
		}

		void Start () {
			// Test ();

			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();

			if ( (mMainModel.Player.TutorialStage == TutorialStage.DONE) && LoginAutomatico && PlayerPrefs.HasKey( KEY_EMAIL ) && PlayerPrefs.HasKey( KEY_PASSWORD ) ) {
				mStatus = GenetsisStatus.USER_OFFLINE;
				LoginUsuario( PlayerPrefs.GetString( KEY_EMAIL ), PlayerPrefs.GetString( KEY_PASSWORD ) );
			}

			/*
			// Test Documentación Genetsis
			
			string autenticacion = "1208045C722-44F3-4B0E-8A7D-C6D582286817A4B3BB52-F41F-476F- 9BA3-DA945010AA35";
			Debug.Log( "Autenticacion: " + autenticacion + " Hash: " + GetSHA1Hash( autenticacion ) );

			autenticacion = "120A4B3BB52-F41F-476F-9BA3-DA945010AA35";
			Debug.Log( "Autenticacion: " + autenticacion + " Hash: " + GetSHA1Hash( autenticacion ) );
			*/
		}
		
		void Update () {
		}

		/*
		 * Promoción: “Football Star - Registro móvil”
		 * Método: 2.1 Registro de usuarios
		 * AUTENTICACIÓN SIN usuario logado
		 * POST: /ws/v2/users
		 * 
		 * OK: Devuelve los datos del usuario
		 * 
		 * {"user":{"codigo_postal":"","id":7656,"localidad":"madrid","street_type":"http://testmahou.serviciosmsm.com/ws/v2/street_types/3","street_type_id":3,"numero_via":12,
		 * "fecha_nacimiento":19801215,"usuario":"diego_m7","twitter":"jblanco","email":"pruebas111@genetsis.com","apellido1":"apellido pruebas","apellido2":"","origen":4,
		 * "movil":687061400,"acepta_comunicaciones_email":"Si","nif":"33527333C","direccion":"","acepta_comunicaciones_movil":"","telefono":914444444,
		 * "url":"http://testmahou.serviciosmsm.com/ws/v2/users/7656","region":"http://testmahou.serviciosmsm.com/ws/v2/regions/34","region_id":34,"resto_direccion":"",
		 * "token_acceso":"4E080541-D5C8-43E9-8F50-B33AED2B0AC2","nombre":"Pruebas"}}
		 * 
		 * Códigos error: -20, -21, -23, -24, -52, -53, -54, -55, -56, -57, -58
		 */
		public void RegistrarUsuario(string nombre, string apellido, string email, string password, bool aceptaComunicacionesEmail, string facebookId) {
			mEmail 		= email;
			mPassword 	= password;
			mFacebookID = facebookId;

			string URLrecurso = "/ws/v2/users";
			string authorization = AuthorizationHeader( RegistroMovil );

			string url = URLServicioWeb + URLrecurso;
			
			WWWForm form = new WWWForm();
			form.AddField( "Authorization", authorization );
			form.AddField( "format", "json" );
			form.AddField( "promocion", RegistroMovil.ID );
			form.AddField( "nombre", nombre );
			form.AddField( "apellido1", apellido );
			form.AddField( "email", email );
			form.AddField( "Contrasena", password );
			form.AddField( "acepta_comunicaciones_email", aceptaComunicacionesEmail ? "SI" : "NO" );

			if ( !String.IsNullOrEmpty(facebookId) ) {
				form.AddField( "id_usuario_facebook", facebookId );
			}

			POSTRequest( GenetsisType.REGISTER_USER, url, form ); 
		}

		public void RegistrarUsuario(string nombre, string apellido, string email, string password, bool aceptaComunicacionesEmail) {
			RegistrarUsuario( nombre, apellido, email, password, aceptaComunicacionesEmail, "" );
		}

		/*
		 * Promoción: “Football Star - Registro móvil”
		 * Método: 2.2 Login
		 * AUTENTICACIÓN SIN usuario logado
		 * GET: /ws/v2/users
		 * 
		 * OK: Devuelve los datos del usuario
		 * Códigos error: -20, -59, -60, -61
		 */
		public void LoginUsuario(string email, string password) {
			mEmail 		= email;
			mPassword 	= password;

			Debug.Log ("GENETSIS.- LoginUsuario");
			
			string URLrecurso = "/ws/v2/users";
			string authorization = AuthorizationHeader( RegistroMovil );
			string URLparams = String.Format("?Authorization={0}&format={1}&email={2}&Contrasena={3}&promocion={4}", 
			                                 authorization, 
			                                 "json", 
			                                 Uri.EscapeDataString(email),
			                                 Uri.EscapeDataString(password),
			                                 RegistroMovil.ID);

			string url = URLServicioWeb + URLrecurso + URLparams;
			Debug.Log ("GENETSIS.- LoginUsuario - Request");
			GETRequest( GenetsisType.LOGIN_USER, url );
		}

		public void LoginFacebook(string email, string idFacebook) {
			mFacebookID	= idFacebook;
			mEmail 		= email;

			string URLrecurso = "/ws/v2/users";
			string authorization = AuthorizationHeader( RegistroMovil );
			string URLparams = String.Format("?Authorization={0}&format={1}&email={2}&id_usuario_facebook={3}&promocion={4}", 
			                                 authorization, 
			                                 "json", 
			                                 Uri.EscapeDataString(email),
			                                 idFacebook,
			                                 RegistroMovil.ID);

			string url = URLServicioWeb + URLrecurso + URLparams;
			GETRequest( GenetsisType.LOGIN_USER, url );
		}

		/*
		 * Promoción: “Football Star - Registro móvil”
		 * Método: 2.6 Recordar contraseña
		 * AUTENTICACIÓN SIN usuario logado
		 * GET: /ws/v2/users
		 * 
		 * OK: {"resultado":"1"} 
		 * Códigos error: -24, -60, -62 
		 */
		public void RecordatorioPassword(string email) {
			string URLrecurso = "/ws/v2/users";
			string authorization = AuthorizationHeader( RegistroMovil );
			string URLparams = String.Format("?Authorization={0}&format={1}&email={2}&promocion={3}", 
			                                 Uri.EscapeDataString(authorization), 
			                                 "json", 
			                                 Uri.EscapeDataString(email), 
			                                 RegistroMovil.ID);
			
			string url = URLServicioWeb + URLrecurso + URLparams;
			GETRequest( GenetsisType.REMEMBER_USER_PASSWORD, url );
		}

		/*
		 * Promoción: “Football Star - Pincodes”
		 * Método: 7.9 Validación de un pincode.
		 * AUTENTICACIÓN CON usuario logado
		 * POST: /ws/v2/pincodes/pincode   (pincode = pincode que se quiere validar)
		 * 
		 * OK: Resultado de participación en promoción
		 * 
		 * {"participacion": {"resultado":1,"id_premio":27, "id_participacion":242} }
		 * 
		 * Códigos error: -20, -21, -100, -101
		 */
		public void ValidarPincode(string pincode) {
			if ( Status != GenetsisStatus.USER_ONLINE ) return;
			
			string URLrecurso = "/ws/v2/pincodes/{0}";
			string authorization = AuthorizationHeader( Pincodes, mUserToken );
			
			string url = URLServicioWeb + String.Format( URLrecurso, pincode );
			
			WWWForm form = new WWWForm();
			form.AddField( "Authorization", authorization );
			form.AddField( "format", "json" );
			form.AddField( "id_usuario", mUserID );
			form.AddField( "promocion", Pincodes.ID );
			
			POSTRequest( GenetsisType.VALIDATE_PINCODE, url, form );
		}

		/*
		 * Promoción: “Football Star - Pincodes”
		 * Método: 7.7 Participación en un sorteo con pincode.
		 * AUTENTICACIÓN CON usuario logado
		 * POST: /ws/v2/promociones/promocion   (promocion = identificador promoción)
		 * 
		 * OK: Resultado de participación en promoción
		 * 
		 * {"participacion": {"resultado":1,"id_premio":27, "id_participacion":242} }
		 * 
		 * Códigos error: -20, -21, -23, -36, -37, -38, -70, -100, -101, -165
		 */
		public void UsarPincode(string pincode) {
			if ( Status != GenetsisStatus.USER_ONLINE ) return;

			string URLrecurso = "/ws/v2/promociones/{0}";
			string authorization = AuthorizationHeader( Pincodes, mUserToken );
			
			string url = URLServicioWeb + String.Format( URLrecurso, Pincodes.ID );
			
			WWWForm form = new WWWForm();
			form.AddField( "Authorization", authorization );
			form.AddField( "format", "json" );
			form.AddField( "id_usuario", mUserID );
			form.AddField( "pincode", pincode );

			POSTRequest( GenetsisType.USE_PINCODE, url, form );
		}

		/*
		 * Promoción: “Football Star - Acciones”
		 * Método: 10.1 Guardar acciones de los usuarios
		 * AUTENTICACIÓN CON usuario logado
		 * POST: /ws/v2/users/idUsuario/acciones  (idUsuario = identificador del usuario)
		 * 
		 * tipoaccion: 6
		 * tipoobjeto: 4
		 * origen: Football Star
		 * objeto: Juego Football Star
		 * 
		 * OK: Devuelve el id_accion
		 * 
		 * { "id_accion": "789", "correcto": "Si" }
		 * 
		 * Códigos de error: -35, -36, -37, -38, -105, -122, -123
		 */
		public void AccionJugar() {
			if ( Status != GenetsisStatus.USER_ONLINE ) return;

			string URLrecurso = "/ws/v2/users/{0}/acciones";
			string authorization = AuthorizationHeader( Acciones, mUserToken );

			string url = URLServicioWeb + String.Format( URLrecurso, mUserID );

			WWWForm form = new WWWForm();
			form.AddField( "Authorization", authorization );
			form.AddField( "format", "json" );
			form.AddField( "promocion", Acciones.ID );
			form.AddField( "tipoaccion", 6 );
			form.AddField( "tipoobjeto", 4 );
			form.AddField( "origen", "Football Star" );
			form.AddField( "objeto", "Juego Football Star" );

			POSTRequest( GenetsisType.ACTION_PLAY, url, form );
		}

		/*
		 * AUTENTICACIÓN:
	 	 * - SIN usuario logado:   	id. promoción + token promoción 
	 	 * - CON usuario logado:	id. promoción + token usuario + token promoción
	 	 * 
	 	 * Parámetro: Authorization
	 	 * Codificación: SHA-1
	 	 */
		static string GetSHA1Hash ( string texto ) {
			SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
			// SHA1 sha1 = SHA1Managed.Create();
			byte[] hash = sha1.ComputeHash( Encoding.UTF8.GetBytes( texto ) );
			// string delimitedHexHash = BitConverter.ToString(hash);
			// delimitedHexHash.Replace("-", "");

			StringBuilder sBuilder = new StringBuilder();
			for( int i=0; i < hash.Length; i++ ) {
				sBuilder.Append( hash[i].ToString("x2") );
			}
			return sBuilder.ToString();
		}

		/*
		 * id. promoción + token promoción 
		 */
		string ConcatenarAutenticacion(MahouPromocion promocion) {
			return string.Format( "{0}{1}", promocion.ID, promocion.Token );
		}

		/*
		 * id. promoción + token usuario + token promoción
		 */
		string ConcatenarAutenticacion(MahouPromocion promocion, string tokenUser) {
			return string.Format( "{0}{1}{2}", promocion.ID, tokenUser, promocion.Token );
		}

		string AuthorizationHeader( MahouPromocion promocion ) {
			string autenticacion = ConcatenarAutenticacion( promocion );
			// Debug.Log( "Autenticacion: " + autenticacion );
			return GetSHA1Hash( autenticacion );
		}

		string AuthorizationHeader( MahouPromocion promocion, string tokenUser ) {
			string autenticacion = ConcatenarAutenticacion( promocion, tokenUser );
			// Debug.Log( "Autenticacion: " + autenticacion );
			return GetSHA1Hash( autenticacion );
		}

		void GETRequest( GenetsisType type, string url ) {
			try {
				WebRequest webRequest = WebRequest.Create(url);
				webRequest.Method = "GET";
				Debug.Log ("GENETSIS.- LoginUsuario- Request Starting coroutine");
				StartCoroutine( WaitForRequest(type, webRequest) );
				// StartCoroutine( WaitForRequest(type, (WWW)null) );
				
				// WWW www = new WWW(url);
				// StartCoroutine( WaitForRequest(type, www) );

				Debug.Log( String.Format( "{0}: URL({1})", type, url ) );
			}
			catch {
				if ( OnResult != null ) {
					EventGenetsisResultArgs args = new EventGenetsisResultArgs( type );
					args.Result = false;
					args.Msg = ERROR_ACCESS_GENERIC;
					OnResult( this, args );	
				}

				Debug.Log( String.Format( "GETRequest Error: {0}: URL({1})", type, url ) );
			}
		}

		void POSTRequest( GenetsisType type, string url, WWWForm form ) {
			try {
				WebRequest webRequest = WebRequest.Create(url);
				webRequest.Method = "POST";
				webRequest.ContentType = "application/x-www-form-urlencoded";
				webRequest.ContentLength = form.data.Length;

				// copiar los datos del post al request
				Stream postStream = webRequest.GetRequestStream();
				postStream.Write(form.data, 0, form.data.Length);
				postStream.Flush();
				postStream.Close();

				StartCoroutine( WaitForRequest(type, webRequest) );
				// StartCoroutine( WaitForRequest(type, (WWW)null) );
				
				// WWW www = new WWW(url, form);
				// StartCoroutine( WaitForRequest(type, www) );
				
				Debug.Log( String.Format( "{0}: URL({1} Data({2})", type, url, System.Text.Encoding.UTF8.GetString(form.data) ) );
			}
			catch {
				if ( OnResult != null ) {
					EventGenetsisResultArgs args = new EventGenetsisResultArgs( type );
					args.Result = false;
					args.Msg = ERROR_ACCESS_GENERIC;
					OnResult( this, args );	
				}

				Debug.Log( String.Format( "POSTRequest Error: {0}: URL({1})", type, url ) );
			}
		}

		static MemoryStream CopyStreamToMemory(Stream inputStream)
		{
			MemoryStream ret = new MemoryStream();
			const int BUFFER_SIZE = 1024;
			byte[] buf = new byte[BUFFER_SIZE];
			
			int bytesread = 0;
			while ((bytesread = inputStream.Read(buf, 0, BUFFER_SIZE)) > 0)
				ret.Write(buf, 0, bytesread);
			
			ret.Position = 0;
			return ret;
		}

		public static bool IsJson(string input){
			input = input.TrimStart();
			return input.StartsWith("{") || input.StartsWith("[");
		}

		void RequestOk( GenetsisType type, RequestState result ) {
			if ( OnResult != null ) {
				EventGenetsisResultArgs args = new EventGenetsisResultArgs( type );
				args.Result = true;
				OnResult( this, args );	
			}
			
			MemoryStream memoryStream = CopyStreamToMemory( ((HttpWebResponse)result.webResponse).GetResponseStream() );
			memoryStream.Position = 0;
			StreamReader reader = new StreamReader( memoryStream );

			string responseString = reader.ReadToEnd();
			Debug.Log( "Genetsis: Response: " + responseString );
			
			if ( type == GenetsisType.REGISTER_USER || type == GenetsisType.LOGIN_USER ) {
				mStatus = GenetsisStatus.USER_ONLINE;
				
				PlayerPrefs.SetString( KEY_EMAIL, mEmail );
				PlayerPrefs.SetString( KEY_PASSWORD, mPassword );
				PlayerPrefs.SetString( KEY_FACEBOOK, mFacebookID );

				if ( IsJson(responseString) ) {

					JSONNode jsonNode = JSON.Parse( responseString );
					JSONNode userNode = jsonNode["USER"];
					if ( userNode == null ) 
						userNode = jsonNode["user"];
					if ( userNode != null ) {
						JSONNode accesoNode = userNode["token_acceso"];
						if ( accesoNode != null ) {
							mUserToken = accesoNode.Value;
						}
						JSONNode idNode = userNode["id"];
						if ( idNode != null ) {
							mUserID = idNode.Value;
						}
					}
				
					Debug.Log( "WWW Ok!: Token User: " + mUserToken + " ID: " + mUserID );
				}
			}
		}

		void RequestError( GenetsisType type, RequestState result ) {
			string msgError = "";
			string codeError = "";
			
			if ( result.webResponse != null ) {
				MemoryStream memoryStream = CopyStreamToMemory( ((HttpWebResponse)result.webResponse).GetResponseStream() );
				
				memoryStream.Position = 0;
				StreamReader reader = new StreamReader( memoryStream );

				string responseString = reader.ReadToEnd();
				Debug.Log( "Genetsis: Response: " + responseString );

				if ( IsJson(responseString) ) {

					JSONNode jsonNode = JSON.Parse( responseString );
					if (jsonNode != null ) {
						JSONNode errorNode = jsonNode["error"];
						if ( errorNode != null ) {
							JSONNode msgNode = errorNode["msg_text"];
							if ( msgNode != null ) {
								msgError = msgNode.Value;
							}
							JSONNode codeNode = errorNode["code"];
							if ( codeNode != null ) {
								codeError = codeNode.Value;

								int code = Int32.Parse( codeError );
								msgError = GetMsgError( code );
							}
						}
					}
				}
			}
			else {
				msgError = ERROR_ACCESS_GENERIC;
			}
			
			if ( OnResult != null ) {
				EventGenetsisResultArgs args = new EventGenetsisResultArgs( type );
				args.Result = false;
				args.Msg = msgError;
				OnResult( this, args );	
			}
			
			Debug.Log( "WWW Error: " + codeError + " Msg: " + msgError );
		}

		static string GetMsgError( int code ) {
			string msg = "";

			switch( code ) {
			case -23: msg = "La promoción no está activada"; break;
			case -35: msg = "La promoción no ha empezado"; break;
			case -36: msg = "La promoción ya ha finalizado"; break;
			case -37: msg = "Ya ha ganado una promoción excluyente"; break;
			case -38: msg = "Ha igualado el número máximo de participaciones permitidas en la promoción"; break;
			case -52: msg = "El email del usuario ya existe"; break;
			case -59: msg = "Email y/o Password incorrectos"; break; //"La contraseña del usuario pasada no coincide con la registrada"; break;
			case -60: msg = "El usuario no existe"; break;
			case -61: msg = "El usuario no está activado"; break;
			case -62: msg = "Se ha superado el número máximo de reenvíos de contraseñas al móvil"; break;
			case -70: msg = "No hay ningún premio asociado a estos datos"; break;
			case -100: msg = "Pincode usado"; break; //"Pincode gastado"; break;
			case -101: msg = "Pincode no válido"; break;
			case -105: msg = "El usuario no tiene acceso a esa marca"; break;
			case -165: msg = "Se ha alcanzado el número máximo de participaciones en la promoción."; break;
			}

			return msg;
		}

		IEnumerator WaitForRequest(GenetsisType type, WebRequest webRequest) {
			//Debug.Log ("GENETSIS.- LoginUsuario- WaitForRequest (start)");
			WebAsync webAsync = new WebAsync(); StartCoroutine( webAsync.GetResponse(webRequest) );
			do {
				//Debug.Log ("GENETSIS.- time: " + Time.time);
				yield return null;
			} while (!webAsync.isResponseCompleted);
			//Debug.Log ("GENETSIS.- LoginUsuario- WaitForRequest (end)");
			RequestState result = webAsync.requestState;
			if ( String.IsNullOrEmpty(result.errorMessage) && (result.webResponse != null) ) {
				RequestOk( type, result );
			}
			else {
				RequestError( type, result );
			}
		}

		IEnumerator WaitForRequest(GenetsisType type, WWW www) {
			if ( www != null ) {
				yield return www;

				if ( String.IsNullOrEmpty(www.error) ) {
					if ( OnResult != null ) {
						EventGenetsisResultArgs args = new EventGenetsisResultArgs( type );
						args.Result = true;
						OnResult( this, args );	
					}

					if ( type == GenetsisType.REGISTER_USER || type == GenetsisType.LOGIN_USER ) {
						mStatus = GenetsisStatus.USER_ONLINE;
					}

					Debug.Log( "WWW Ok!: " + www.text );
				}
				else {
					if ( OnResult != null ) {
						EventGenetsisResultArgs args = new EventGenetsisResultArgs( type );
						args.Result = false;
						args.Msg = www.error;
						OnResult( this, args );	
					}

					Debug.Log( "WWW Error: " + www.error + " URL: " + www.url );
				}
			}
			else {
				// --> TEST <--
				yield return null;
				
				if ( OnResult != null ) {
					EventGenetsisResultArgs args = new EventGenetsisResultArgs( type );
					args.Result = true;
					args.Msg = "Mensaje OK";
					OnResult( this, args );		

					Debug.Log( "Genetsis: " + type + ": " + args.Result );
				}

				if ( type == GenetsisType.REGISTER_USER || type == GenetsisType.LOGIN_USER ) {
					mUserID = "1";
					mUserToken = "User-Test";
					mStatus = GenetsisStatus.USER_ONLINE;
				}

				// -------------
			}
		}

		void Test() {
			RegistrarUsuario( "Test", "Unusual", "test@unusual.com", "xxx", false );
			LoginUsuario( "test@unusual.com", "xxx" );
			RecordatorioPassword( "test@unusual.com" );

			mUserID = "1";
			mUserToken = "111";
			mStatus = GenetsisStatus.USER_ONLINE;

			ValidarPincode( "1234567891011" ); 
			AccionJugar();
		}

		MainModel mMainModel;

		GenetsisStatus mStatus;
		string mUserID;
		string mUserToken;

		string mEmail;
		string mPassword;
		string mFacebookID;
	}
}

/*
 * CODIGOS DE ERROR:
 * 
 * -20: Faltan parámetros obligatorios en la petición
 * -21: El formato solicitado para la respuesta no es válido
 * -23: La promoción no está activada
 * -24: La promoción no tiene asignada ninguna plantilla para hacer el envío del mail
 * -35: La promoción no ha empezado
 * -36: La promoción ya ha finalizado
 * -37: Ya ha ganado una promoción excluyente
 * -38: Ha igualado el número máximo de participaciones permitidas en la promoción
 * -52: El email del usuario ya existe
 * -53: El móvil del usuario ya existe
 * -54: El nif del usuario ya existe
 * -55: El twitter del usuario ya existe
 * -56: La fecha de nacimiento no es una fecha válida
 * -57: El prefijo del código postal no coincide con la provincia
 * -58: El identificador del parámetro origen no es válido
 * -59: La contraseña del usuario pasada no coincide con la registrada
 * -60: El usuario no existe
 * -61: El usuario no está activado
 * -62: Se ha superado el número máximo de reenvíos de contraseñas al móvil
 * -70: No hay ningún premio asociado a estos datos
 * -100: Pincode gastado
 * -101: Pincode no válido
 * -105: El usuario no tiene acceso a esa marca
 * -122: El tipo de accion no existe
 * -123: El tipo de objeto no existe
 * -165: Se ha alcanzado el número máximo de participaciones en la promoción.
 */

