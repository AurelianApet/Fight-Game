using UnityEngine;
using System.Collections;
public class HitData
{
		public string 	tag 	= "";
		public float 	damage 	= 0.0f;
}
/// <summary>
/// Punch controller.
/// </summary>
public class PunchController : MonoBehaviour {


	private float		_defense		=	0.0f;
	private int 		_hits 			= 	0;
	private HitData 	_hitData			=	new HitData();
	/// <summary>
	/// Gets or sets the effective power.
	/// </summary>
	/// <value>The effective power.</value>
	public float effectivePower
	{
		get{
						return _hitData.damage;
				}
		set{
						_hitData.damage	=	value;
				}
	}

	/// <summary>
	/// Gets or sets the defense.
	/// NOT IN USE. 
	/// </summary>
	/// <value>The defense.</value>
	public float defense
	{
		get{return _defense;}
		set{_defense	=	value;}
	}
	
		public int Hits{
				get{return _hits;}
		}
	/// <summary>
	/// Raises the trigger enter2 d event.
	/// ON HIT DETECTED APPLYDAMAGE/POWER.
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnTriggerEnter2D(Collider2D coll) {
		
		//HIT ON BODY APPLY DAMAGE
			
		if(coll.tag=="head")
		{
			_hitData.damage 	= effectivePower;
			_hitData.tag 	= tag;
			_hits++;
			coll.transform.root.gameObject.SendMessage("ApplyDamage",_hitData,SendMessageOptions.DontRequireReceiver);
		}
	}
	
}
