using UnityEngine;
using System.Collections;
/// <summary>
/// Spawner behaviour.
/// </summary>
public class SpawnerBehaviour : MonoBehaviour {
	#region VARIABLES
	public 	Vector2 		timeRange		= 	new Vector2(5.0f,10.0f);
	public	Vector2			force			=	new Vector2(0,-200);
	private ObjectPool		_objectPool;
	#endregion
	#region UNITY CALLBACKS
	/// <summary>
	/// Use this for initialization
	/// </summary>
	void OnEnable () {
		_objectPool=transform.GetComponent<ObjectPool>();
		StartCoroutine (Spawn());
	}
	#endregion
	#region COROUTINES
	/// <summary>
	/// Spawn a new object
	/// </summary>
	IEnumerator Spawn()
	{
		yield return new WaitForSeconds (1.0f);
		while (true) 
		{
			GameObject 	obj				=	_objectPool.activateObject();
			if(obj!=null)
			{
				obj.transform.position	=	transform.position;
				obj.transform.rotation 	= 	Quaternion.identity;
				obj.SetActive(true);
				obj.GetComponent<Rigidbody2D>().AddForce(force);
			}
			yield return new WaitForSeconds(Random.Range (timeRange.x,timeRange.y));
		}
	}
	#endregion
}
