using UnityEngine;
using System.Collections;
using game_core;

[System.Serializable]
/// <summary>
/// Game pad configuration.
/// </summary>

public enum GamePadConfiguration
{
		ArrowUp		=	0,
		ArrowDown	=	1,
		ArrowRight	=	2,
		ArrowLeft	=	3,
		Fire1		=	4,
		Fire2		=	5,

}
/// <summary>
/// Game pad controller.
/// </summary>
public class GamePadController : TouchBehaviour {

	public 	GamePadConfiguration 	configuration;
	public	Sprite					buttonNormal;
	public 	Sprite					buttonPushed;
	private SpriteRenderer			_sRenderer;

	/// <summary>
	/// Start this instance.
	/// </summary>
	public override void Start(){
			base.Start ();
			_sRenderer=GetComponent<SpriteRenderer> ();
	}

	/// <summary>
	/// Raises the start button behaivour event.
	/// </summary>
	public void OnStartButtonBehaivour()
	{
				switch(configuration)
				{
				case GamePadConfiguration.ArrowDown:
						InputManager.axisV			=	-1.0f;
						break;
				case GamePadConfiguration.ArrowUp:
						InputManager.axisV			=	1.0f;
						break;
				case GamePadConfiguration.ArrowRight:
						InputManager.axisH			=	1.0f;
						break;
				case GamePadConfiguration.ArrowLeft:
						InputManager.axisH			=	-1.0f;
						break;
				case GamePadConfiguration.Fire1:
						InputManager.triggerFire1	=	true;
						break;
				case GamePadConfiguration.Fire2:
						InputManager.triggerFire2	=	true;
						break;
				}

	}
	
	/// <summary>
	///	ON  TOUCH END BEHAIVOUR
	/// </summary>
	public void OnEndButtonBehaivour()
	{



				switch(configuration)
				{
				case GamePadConfiguration.ArrowDown:
				case GamePadConfiguration.ArrowUp:
						InputManager.axisV 			= 	0.0f;
						break;
				case GamePadConfiguration.ArrowRight:
				case GamePadConfiguration.ArrowLeft:
						InputManager.axisH			=	0f;
						break;
				case GamePadConfiguration.Fire1:
						InputManager.triggerFire1	=	false;
						break;
				case GamePadConfiguration.Fire2:
						InputManager.triggerFire2	=	false;
						break;
				}
	}
	
	/// <summary>
	/// Raises the touch down event.
	/// </summary>
	public override void OnTouchBegan(Vector3 value)
	{
		if (InputManager.touchCount > 0) 
		{
			OnStartButtonBehaivour ();
		}
				if(_sRenderer!=null){_sRenderer.sprite = buttonPushed;}
	}

	/// <summary>
	/// Raises the touch end event.
	/// </summary>
	public override void OnTouchEnded(Vector3 value)
	{
		if (InputManager.touchCount > 0) 
		{
			
			OnEndButtonBehaivour ();
		}
				if (_sRenderer != null) {
						_sRenderer.sprite = buttonNormal;
				}
	}
}
