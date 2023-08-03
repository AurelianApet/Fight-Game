using UnityEngine;
using System.Collections;

/// <summary>
/// Object pool.
/// </summary>
public class ObjectPool : MonoBehaviour {

	public GameObject[] objectPool;
	public int 			numberOfObjects	=	0;
	public GameObject	prefab;
	
	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {
		objectPool = new GameObject[numberOfObjects];
		initObjectArray ();
	}

	/// <summary>
	/// Inits the object array.
	/// </summary>
	private void initObjectArray()
	{
		if(prefab!=null)
		{
			for(int i=0;i<numberOfObjects;i++)
			{
				objectPool[i]=Instantiate(prefab) as GameObject;
				objectPool[i].SetActive(false);
			}
		}
	}

	/// <summary>
	/// Activates the object.
	/// </summary>
	public GameObject activateObject()
	{
		for(int i=0;i<objectPool.Length;i++)
		{
			if(!objectPool[i].activeSelf)
			{
				return objectPool[i];
			}
		}
		return null;
	}
}
