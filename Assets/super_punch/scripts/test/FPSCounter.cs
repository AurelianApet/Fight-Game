using UnityEngine;
using System.Collections;

/// <summary>
/// FPS counter.
/// </summary>
public class FPSCounter : MonoBehaviour {

	private Rect 		FPSRect;
	private GUIStyle 	style;
	private float		fps		=	0.0f;
	
	/// <summary>
	/// Start this instance.
	/// Use this for initialization
	/// </summary>
	void Start () {
		FPSRect 		= new Rect (100,100,400,100);
		style 			= new GUIStyle ();
		style.fontSize 	= 30;
		StartCoroutine (RecalculateFPS());
	}

	/// <summary>
	/// Recalculates the FPS.
	/// </summary>
	/// <returns>The FP.</returns>
	IEnumerator RecalculateFPS(){
		while(true){
			fps = 	1 / Time.deltaTime;
			yield return new WaitForSeconds(1.0f);
		}
	}

	/// <summary>
	/// Raises the GU event.
	/// </summary>
	void OnGUI()
	{
		GUI.Label (FPSRect,"FPS: "+fps);
	}
}
