using UnityEngine;
using System.Collections;

/// <summary>
/// Head controller.
/// </summary>
public class headController : MonoBehaviour {	
	private float 			_beatenRate	=	0.5f;
	private float			_lastBeaten	=	0.0f;
	private Transform		_myTransform;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Awake(){
		_myTransform = transform;
	}
	
	/// <summary>
	///	HIT ON HEAD, CONTROLS DIRECTION OF HEAD WHEN HIT IS DETECTED.
	/// 3 TIMES PER SECONDS (1.0f/3.0f)
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnTriggerEnter2D(Collider2D coll) 
	{
		_beatenRate=(1.0f/3.0f);
		if((coll.tag=="punchLeft") && Time.time>_beatenRate+_lastBeaten)
		{
			_myTransform.root.gameObject.SendMessage("OnHurtLeft",null,SendMessageOptions.DontRequireReceiver);
			_lastBeaten=Time.time;
		}
		if((coll.tag=="punchRight") && Time.time>_beatenRate+_lastBeaten)
		{
			_myTransform.root.gameObject.SendMessage("OnHurtRight",null,SendMessageOptions.DontRequireReceiver);
			_lastBeaten=Time.time;
		}
	}
}
