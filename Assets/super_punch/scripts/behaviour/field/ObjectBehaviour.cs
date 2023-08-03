using UnityEngine;
using System.Collections;

/// <summary>
/// Object behaviour.
/// Behaviour of the objects thrown by spectators.
/// </summary>
public class ObjectBehaviour : MonoBehaviour {
	public 	float		blinkRate		=	6.0f;
	public 	float		rotationSpeed	=	500.0f;
	public 	Vector2		timeOutRange	=	new Vector2(2.0f,3.0f);
	public 	bool 		blink			=	true;
	public float		damage			= 	10.0f;
	private float 		_blinkRate		=	0.0f;
	private float		_lastBlink		=	0.0f;
	private bool 		_activeEffect	=	true;
		private HitData	_hitData;
	/// <summary>
	///	Use this for initialization.
	/// </summary>
	void OnEnable () {
		_hitData 		= new HitData ();
		_hitData.damage = damage;
		_hitData.tag	= tag;
		Invoke ("disable", Random.Range(timeOutRange.x,timeOutRange.y));
	}
	

	/// <summary>
	///  Update is called once per frame.
	/// </summary>
	void Update () 
	{
		_blinkRate	=	(1.0f/blinkRate);
		if(Time.time>_blinkRate+_lastBlink && blink)
		{
			_activeEffect	=	!_activeEffect;
			transform.FindChild("effect").gameObject.SetActive(_activeEffect);
			_lastBlink		=	Time.time;
		}
		transform.Rotate(Vector3.forward * Time.deltaTime*Mathf.Lerp(0,rotationSpeed,Time.time));
	}

	/// <summary>
	/// Disable this instance.
	/// </summary>
	void disable () {
		gameObject.SetActive (false);
	}

	/// <summary>
	/// On hit detected applydamage
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnTriggerEnter2D(Collider2D coll) {
		string tag = coll.gameObject.tag;
		//HIT ON BODY APPLY DAMAGE
		
		//HIT ON HEAD APPLY DAMAGE
		if(tag=="head")
		{
			coll.transform.root.gameObject.SendMessage("ApplyDamage",_hitData	,SendMessageOptions.DontRequireReceiver);
		}
	}
}
