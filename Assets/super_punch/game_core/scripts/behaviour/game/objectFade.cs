using UnityEngine;
using System.Collections;
namespace game_core{
/// <summary>
/// Object fade behaviour.
/// </summary>
public class objectFade : MonoBehaviour {

	public Vector3 	startMarker;
	public Vector3 	endMarker;
	public float 	speed = 1.0F;
	public bool loop = false;
				public bool smooth=false;
	private float startTime;
	private float journeyLength;
	
	/// <summary>
	/// Use this for initialization.
	/// </summary>
	void Start() {
		startTime = Time.time;
		journeyLength = Vector3.Distance(startMarker, endMarker);
	}
	
	/// <summary>
	/// This method  is called once per frame.
	/// </summary>
	void Update() {
		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = distCovered / journeyLength;
	
						transform.position = (smooth)?Vector3.Lerp(startMarker, endMarker,Mathf.SmoothStep(0.0f,1.0f,fracJourney)):Vector3.Lerp(startMarker, endMarker,Mathf.Lerp(0.0f,1.0f,fracJourney));
						if(loop && fracJourney>=1.0f)
						{
								startTime 	= Time.time;
								distCovered = 0.0f;
								transform.position = startMarker;
						}
	}
}
}