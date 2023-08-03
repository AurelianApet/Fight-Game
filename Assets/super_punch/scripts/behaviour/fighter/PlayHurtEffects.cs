using UnityEngine;
using System.Collections;

/// <summary>
/// Play hurt effects.
/// </summary>
public class PlayHurtEffects : MonoBehaviour {

	public AudioClip 	hurtSFX;
	public int 			particles	=	3;
	private AudioSource _audioSource;
	private ObjectPool 	_objectPool;
	private Transform	_myTransform;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void OnEnable()
	{
		_objectPool 	=	transform.GetComponent<ObjectPool> ();
	}

	/// <summary>
	/// Applies the damage.
	/// </summary>
	/// <param name="hitData">Hit data.</param>
	public void ApplyDamage(HitData hitData)
	{
			for(int i=0;i < particles ;i++){
					GameObject blood = _objectPool.activateObject ();
					if (blood != null) {
							float bloodyRadius = Random.Range (-.5f, .5f);
							blood.transform.position = new Vector3 (transform.position.x+ bloodyRadius, transform.position.y + bloodyRadius, transform.position.z);
							blood.transform.rotation = transform.rotation;
							blood.SetActive (true);
					}
			}

			if (hurtSFX!=null) {
					AudioSource.PlayClipAtPoint(hurtSFX, transform.position);
			}
	}
	
}
