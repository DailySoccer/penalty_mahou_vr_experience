using UnityEngine;
using System.Collections;

public enum NarratorMessageType{
	Descriptive,
	LocalPlayerMessage,
	VisitantPlayerMessage
}

public class narratorSlot : MonoBehaviour {

	public UILabel minutoJugadaLocal;
	public UISprite iconoGolLocal;

	public UILabel textoSlot;

	public UISprite iconoGolVisitant;
	public UILabel minutoJugadaVisitant;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetUpSlot(string minuto, string texto, bool isGoal, NarratorMessageType type) {

		textoSlot.text = texto;
		minutoJugadaLocal.text = "";
		iconoGolLocal.enabled = false;
		minutoJugadaVisitant.text = "";
		iconoGolVisitant.enabled = false;

		switch (type) {
		case NarratorMessageType.Descriptive:
				textoSlot.color = _descriptiveColor;
			break;

		case NarratorMessageType.LocalPlayerMessage:
				textoSlot.color = _localPlayerColor;
				minutoJugadaLocal.color = _localPlayerColor;
				minutoJugadaLocal.text = minuto + "'";
				iconoGolLocal.enabled = isGoal;
			break;

		case NarratorMessageType.VisitantPlayerMessage:
				textoSlot.color = _visitantColor;
				minutoJugadaVisitant.color = _visitantColor;
				minutoJugadaVisitant.text = minuto + "'";
				iconoGolVisitant.enabled = isGoal;
			break;
		}
	}

	Color _descriptiveColor = new Color32( 255 , 255, 255, 255);
	Color _localPlayerColor = new Color32( 255, 203, 11, 255);
	Color _visitantColor = new Color32( 197, 197, 195, 255);

}
