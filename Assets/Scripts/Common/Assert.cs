using UnityEngine;
using System.Collections;
using System.Diagnostics;

public delegate bool AssertTest();

public class Assert {
	
#if UNITY_EDITOR
	static Hashtable asserts;
#endif	
	
    [Conditional("UNITY_EDITOR")]
    static private void Error( StackFrame myFrame, string assertString ) {

#if UNITY_EDITOR

		if ( asserts == null ) 
			asserts = new Hashtable();

        string assertInformation = "Filename: " + myFrame.GetFileName() + "\nMethod: " + myFrame.GetMethod() + " at Line: " + myFrame.GetFileLineNumber();
		int hashCode = assertInformation.GetHashCode();
		
		// Hemos mostrado el mensaje anteriormente?
		if ( !asserts.Contains( hashCode ) ) {
			// Paramos el tiempo
			float timeScale = Time.timeScale;
			Time.timeScale = 0;

            if(UnityEditor.EditorUtility.DisplayDialog("Assert!", assertString + "\n\n" + assertInformation,"View Code", "Continue")) {
				// Mostramos el código
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(myFrame.GetFileName(),myFrame.GetFileLineNumber());
				// Se tendrá que quitar la "pausa" del unity
				// (de esta forma permitimos que se pueda comprobar el estado de las variables que provocaron el mensaje de Assert)
	            UnityEngine.Debug.Break();
            }
			
			// Registrar el mensaje para que no vuelva a aparecer
			asserts.Add ( hashCode, true );
			// Informamos del error en la consola
			UnityEngine.Debug.Log(assertInformation);
			
			// Restauramos el tiempo
			Time.timeScale = timeScale;
		}
		
#endif
    }
	
    [Conditional("UNITY_EDITOR")]
    static public void Test( bool test, string assertString ) {

#if UNITY_EDITOR

        if(!test) {
            StackTrace myTrace = new StackTrace(true);
			StackFrame myFrame = myTrace.GetFrame(1);			
			Error ( myFrame, assertString );
        }
		
#endif
    }
	
    [Conditional("UNITY_EDITOR")]
    static public void Test( AssertTest test, string assertString ) {

#if UNITY_EDITOR

        if(!test()) {
            StackTrace myTrace = new StackTrace(true);
			StackFrame myFrame = myTrace.GetFrame(1);			
			Error ( myFrame, assertString );
        }
		
#endif
    }

}