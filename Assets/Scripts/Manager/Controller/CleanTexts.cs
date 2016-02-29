using UnityEngine;
using System.Collections;

public class CleanTexts : MonoBehaviour {
	//Lista de campos de texto a limpiar
	public UIInput[] CleanMeList;

	void OnEnable () {
		foreach (UIInput field in CleanMeList) {
			field.value = "";
		}
	}
}
