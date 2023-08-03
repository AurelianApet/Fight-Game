using UnityEngine;
using System.Collections;
namespace game_core
{
/// <summary>
/// Close button behaviour.
/// </summary>
public class CloseButtonBehaviour : ButtonBehaviour {
		public AudioClip	soundEffect;
		
		/// <summary>
		/// Raises the mouse down event.
		/// </summary>
		protected override void action()
		{
						transform.parent.gameObject.SetActive (!transform.parent.gameObject.activeSelf);
						if(soundEffect!=null)	{ 	AudioSource.PlayClipAtPoint (soundEffect,Vector3.zero);}
		}
}
}
