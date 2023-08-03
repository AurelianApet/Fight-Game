using UnityEngine;
using System.Collections;

/// <summary>
/// Blood behaviour.
/// Blood flesh is "instantiated" when fighter is
/// punched on the face and this class sets the
/// timer on when is enabled and when time is over
/// disables the gameObject.
/// </summary>
public class BloodBehaviour : MonoBehaviour {

	/// <summary>
	/// The time out range.
	/// </summary>
	public 	Vector2 timeOutRange=new Vector2(2,5);

	/// <summary>
	/// This function is called When the GameObject is enabled  and 
	/// initializes the blood effect behaviour.
	/// </summary>
	void OnEnable()
	{
		Invoke ("disable", Random.Range(timeOutRange.x,timeOutRange.y));
	}

	/// <summary>
	/// Set Active to false on time out event.
	/// </summary>
	void disable () {
		gameObject.SetActive (false);
	}
}
