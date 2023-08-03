using UnityEngine;
using System.Collections;

/// <summary>
/// Game Pad behaviour.
/// </summary>
static public class InputManager {
	static public bool 	triggerFire1	=	false;
	static public bool 	triggerFire2	=	false;
	static private float _axisH			=	0.0f;
	static private float _axisV			=	0.0f;

	/// <summary>
	/// Gets or sets the axis h.
	/// </summary>
	/// <value>The axis h.</value>
	static public float axisH
	{
		get
		{
			if(Input.touchCount!=0 && _axisH!=0){return _axisH;}
			_axisH	=	Input.GetAxis("Horizontal");
			return _axisH;
		}
		set
		{
			_axisH=value;
		}
	}

	/// <summary>
	/// Gets or sets the axis v.
	/// </summary>
	/// <value>The axis v.</value>
	static public float axisV{
		get{
			if(Input.touchCount!=0 && _axisV!=0){return _axisV;}
			_axisV	=	Input.GetAxis("Vertical");
			return _axisV;
		}
		set{
			_axisV=value;
		}
	}

	/// <summary>
	/// Gets the touch count.
	/// </summary>
	/// <value>The touch count.</value>
	static public int touchCount
	{
		get{
			return Input.touchCount;
		}
	}
}
