using UnityEngine;
using System.Collections;

public class FacebookUtils : MonoBehaviour 
{
	public  const string FB_PERMISSIONS = "publish_stream,email,user_birthday";
	public  const string PUBLISH_PICTURE_FOR_SINGNING = "http://download.unusualwonder.com/FutbolStarsMahou/Fichado90x90.png";
	public  const string PUBLISH_PICTURE_FOR_WINNING_MATCH = "http://download.unusualwonder.com/FutbolStarsMahou/PartidoGanado90x90.png";
	public  const string PUBLISH_PICTURE_FOR_WINNING_CHALLENGE = "http://download.unusualwonder.com/FutbolStarsMahou/TorneoGanado90x90.png";

	public  const string PUBLISH_TITLE_FOR_SIGNING = "¡Me ha fichado el Real Madrid!";
	public  const string PUBLISH_TITLE_FOR_WINNING_MATCH = "¡He ganado un partido con el Real Madrid!";
	public  const string PUBLISH_TITLE_FOR_WINNING_CHALLENGE = "¡He ganado {0} con el Real Madrid!";

	public  const string PUBLISH_DESCRIPTION_FOR_SGNING = "Si quieres que a ti tambi&eacute;n te fiche el mejor club del mundo, juega a F&uacute;tbol Stars. Disponible en iOs y Android. ¡Desc&aacute;rgalo gratis!";
	public  const string PUBLISH_DESCRIPTION_FOR_WINNING_MATCH =  "Si t&uacute; tambi&eacute;n quieres ganar partidos con el mejor club del mundo, juega a F&uacute;tbol Stars.\n Disponible en iOs o Android. ¡Desc&aacute;rgalo gratis!";
	public  const string PUBLISH_DESCRIPTION_FOR_WINNING_CHALLENGE =  "Si t&uacute; tambi&eacute;n quieres ganar {0} con el mejor club del mundo, juega a F&uacute;tbol Stars.\n Disponible en iOs o Android. ¡Desc&aacute;rgalo gratis!";
}
