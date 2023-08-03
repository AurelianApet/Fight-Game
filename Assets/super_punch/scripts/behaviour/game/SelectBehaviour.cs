using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using game_core;
public class SelectBehaviour : ButtonBehaviour {
	public 	int				buttonID	=	0;
	public Sprite buttonON;
	public Sprite buttonOFF;
	public 	GameObject 			associatedObject;
	private SelectBehaviour[] 	objects;
	private Image image;
	protected override void OnEnable()
	{
				objects	=	FindObjectsOfType<SelectBehaviour> ();
				image = GetComponent<Image> ();
				if (buttonID == 0) {
						enable (true);

				} else {
						enable (false);
				}
			
	}

	public void enable(bool value){
				if (value) {
						SettingsManager.selectedID	=	buttonID;
						if (image != null && buttonON != null) {
								image.sprite = buttonON;
						}
				} else {
						if(image!=null && buttonON!=null)
						{
								image.sprite = buttonOFF;
						}
				}

			if (associatedObject != null) 
			{
					associatedObject.SetActive (value);
			}
	}
	

	/// <summary>
	/// Raises the mouse down event.
	/// </summary>
	protected override void action()
	{
		enable (true);
		if (objects != null) 
		{
				for (int i = 0; i < objects.Length; i++) {
						if (this != objects [i]) 
						{
								objects [i].enable (false);	
						}	
				}
		}
	}
}
