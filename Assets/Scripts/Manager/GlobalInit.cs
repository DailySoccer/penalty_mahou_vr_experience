using UnityEngine;
using System.Collections;


//
// Queremos conservar nuestro GameModel a lo largo de toda la vida del juego. Para esto necesitamos instanciarlo una
// unica vez y que este siempre en memoria
//
public class GlobalInit : MonoBehaviour 
{
	public GameObject GameModelPrefab;
	public GameObject UiPrefab;
	
	void Awake()
	{ 
		// Chequeamos si nuestro GameModel existe ya para evitar duplicados a la vuelta del partido (puesto que es 
		// un DontDestroyOnLoad)
	    if (!GameObject.Find("GameModel"))
		{
	        GameObject newGameModel = Instantiate(GameModelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
	        newGameModel.name = "GameModel";
			DontDestroyOnLoad(newGameModel);
	    }
		
		// Creamos el interfaz
		GameObject newUI = Instantiate(UiPrefab) as GameObject;
		newUI.transform.parent = (GameObject.Find ("MainScene") as GameObject).transform;

		System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
	}
}
