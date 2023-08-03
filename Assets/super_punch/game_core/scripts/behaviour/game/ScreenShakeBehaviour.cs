using UnityEngine;
using System.Collections;

namespace game_core{
/// <summary>
/// Screen shake effect behaviour.
/// </summary>
public class ScreenShakeBehaviour : MonoBehaviour {

	/// <summary>
	/// Transform of the camera to shake. Grabs the gameObject's transform
	/// if null.
	/// </summary>
	public Transform camTransform;
	
	/// <summary>
	/// How long the object should shake for.
	/// </summary>
	public bool onEnableShake=true;
	public float shake 			= 0f;

	/// <summary>
	/// Amplitude of the shake. A larger value shakes the camera harder.
	/// </summary>
	public float shakeAmount 	= 0.7f;
	public float decreaseFactor = 1.0f;
	
	/// <summary>
	/// The initial position.
	/// </summary>
	private Vector3 _initialPosition;

	/// <summary>
	/// The semaphore.
	/// </summary>
	private bool _semaphore=false;

	/// <summary>
	/// Use this for initialization.
	/// </summary>
	void Awake()
	{
		if (camTransform == null)
		{
			camTransform 		= 	Camera.main.GetComponent(typeof(Transform)) as Transform;
			_initialPosition	=	camTransform.position;
			_semaphore 			= 	true;
		}
	}

	/// <summary>
	/// Screen Shake effect coroutine.
	/// </summary>
	IEnumerator Shake(){
		float sValue 	= shake;
		while(sValue>0)
		{
			camTransform.localPosition 	= 	_initialPosition + Random.insideUnitSphere * shakeAmount;
			sValue						-=	Time.deltaTime*decreaseFactor;
			yield return null;
		}
		camTransform.position	=	_initialPosition;
		_semaphore 				= 	!_semaphore;
	}

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable(){
		if (onEnableShake && _semaphore) 
		{
				_semaphore = !_semaphore;
				OnShake ();
		}

	}

	/// <summary>
	/// Raises the shake event.
	/// </summary>
	public void OnShake()
	{
		if(_semaphore)
		{
			_semaphore = !_semaphore;					
			StartCoroutine (Shake());
		}
	}
}
}