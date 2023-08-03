using UnityEngine;
using System.Collections;

/// <summary>
/// Fighter injured behaviour.
/// </summary>
public class FighterInjuredBehaviour : MonoBehaviour {


	//Avatars 3 avatars
	private float 			_damage 	= 	.5f;
	private Transform 		_body;
	private SpriteRenderer	_avatar;
	private int 			_hitLimit	=	2;

	/// <summary>
	/// Use this for initialization
	/// </summary> 
	void OnEnable ()
	{
			_body 	= transform.FindChild ("body");
			_avatar = transform.FindChild ("head").GetComponent<SpriteRenderer>();
	}
				
	/// <summary>
	/// Enable all the injuries.
	/// </summary>
	/// <param name="damage">Damage.</param>
	public void setInjuries(float damage)
	{
			if(Body!=null)
			{
				int childCount	=	Body.childCount;
				childCount 		= 	(int)(childCount * damage);
				for(int i=0 ;i < childCount ;i++)
				{
						Body.GetChild(i).gameObject.SetActive(true);
						Body.GetChild (i).GetComponent<FighterInjuryBehaviour> ().HitsLimit = HitLimit;
				}
			}
	}
				
	/// <summary>
	/// the fighter body.
	/// </summary>
	/// <value>The body.</value>
	public Transform Body
	{
			get{ return _body;}	
	}

	/// <summary>
	/// Gets or sets the damage amount.
	/// </summary>
	/// <value>The damage amount.</value>
	public float Damage
	{
			get{ return _damage;}
			set{ _damage = value;}
	}

	/// <summary>
	/// Gets the injuries amount.
	/// </summary>
	/// <value>The injuries.</value>
	public int Injuries
	{
			get{ 
					int counter 	= 	0;
					if(Body!=null)
					{
							int childCount	=	Body.childCount;
							for(int i=0 ;i < childCount ;i++)
							{

									if (Body.GetChild (i).gameObject.activeSelf)
									{
											counter++;
									}	
							}
					}
					return counter;
			}
	}

	/// <summary>
	/// Sets the hit limit.
	/// </summary>
	/// <value>The hit limit.</value>
	public int HitLimit{
			set{ _hitLimit = value;}
			get{ return _hitLimit;}
	}

		/// <summary>
		/// Sets the avatar.
		/// </summary>
		/// <value>The avatar.</value>
		public Sprite Avatar{
				set{ 
						_avatar.sprite = value;
				}
		}
}
