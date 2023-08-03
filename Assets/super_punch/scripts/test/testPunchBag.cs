using UnityEngine;
using System.Collections;

public class testPunchBag : MonoBehaviour {

	private float lastPunch=0.0f;

	/// <summary>
	/// Raises the trigger enter2 d event.
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnTriggerEnter2D(Collider2D coll) 
	{
		if(coll.tag=="punchRight" || coll.tag=="punchLeft")
		{
			float rate = (1.0f/(Time.time-lastPunch));
			lastPunch = Time.time;
			Debug.Log ("RATE: "+rate);
		}

	}
}
