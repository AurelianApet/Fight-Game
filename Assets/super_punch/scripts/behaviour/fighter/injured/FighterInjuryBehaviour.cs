using UnityEngine;
using System.Collections;
using game_core;
/// <summary>
/// Fighter injury behaviour.
/// </summary>
public class FighterInjuryBehaviour : TouchBehaviour
{
	public 	AudioClip 		onTouchSFX;
	public 	AudioClip 		onCuredSFX;
	private int 			_maxHits 		= 	2;
	private int 			_hitsCounter	=	0;
	private float 			_maxAlphaColor	=	0.0f;
	private Color 			_alphaColor;
	private SpriteRenderer 	_spriteRenderer;

	/// <summary>
	///  Use this for initialization
	/// </summary>
	public override void OnEnable () 
	{
		_hitsCounter 		= 	0;
		if (_maxAlphaColor != 	0) 
		{
				_alphaColor.a 			= 	_maxAlphaColor;
				_spriteRenderer.color	=	_alphaColor;
		}else{
			_spriteRenderer	=	GetComponent<SpriteRenderer> ();
			_alphaColor 	= 	_spriteRenderer.color;
			_maxAlphaColor	= 	_alphaColor.a;
		}
	}
				
	/// <summary>
	/// Raises the touch down event.
	/// </summary>
	public override void OnTouchBegan(Vector3 value)
	{
				_alphaColor.a 			-= 	(_maxAlphaColor / HitsLimit);
				_spriteRenderer.color 	= 	_alphaColor;
				_hitsCounter++;	

				if(onTouchSFX!=null)
				{
						AudioSource.PlayClipAtPoint (onTouchSFX,transform.position);
				}
				if (_hitsCounter >= HitsLimit) 
				{
						gameObject.SetActive (false);
						if(onCuredSFX!=null)
						{
								AudioSource.PlayClipAtPoint (onCuredSFX,Vector3.zero);
						}
				}
	}
	
	/// <summary>
	/// Gets or sets the hits limit.
	/// </summary>
	/// <value>The hits limit.</value>
	public int HitsLimit{
			get{ return _maxHits;}
			set{ _maxHits = value; }
	}
}
