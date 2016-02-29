using UnityEngine;
using System.Collections;

public class Message : MonoBehaviour {
    public GUIText Text;
    public GUIText Time;

    // Use this for initialization
    public void SetValues(string text, string time) {
        Text.text = text;
        Time.text = time;
    }
}
