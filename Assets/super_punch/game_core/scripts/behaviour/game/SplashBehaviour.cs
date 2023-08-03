using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

namespace game_core{

/// <summary>
/// Available Ad systems.
/// </summary>
public enum AdSystems
{
		none			=	0,
		unityAds		= 	1,
		AdmobManager	=	2,
}

/// <summary>
/// Splash behaviour.
/// </summary>
public class SplashBehaviour: MonoBehaviour {
				
	//VARIABLES
	public float 		timeOut		=	1.0f;
	public string 		sceneName	=	"menu";
	public AdSystems 	adSystem;
	
	/// <summary>
	/// Use this for initialization 
	/// 1053965 App Store
	/// 1053966 Google PLay
	/// </summary>
	void Start () {

		switch(adSystem)
		{
			case AdSystems.unityAds:

								#if UNITY_ANDROID
								Advertisement.Initialize ("1053966");
								#elif UNITY_IPHONE
								Advertisement.Initialize ("1053965");
								#endif
					
					break;
			case AdSystems.AdmobManager:
					AdmobManager.RequestBanner ();
					AdmobManager.RequestInterstitial ();
					break;
		}
		Invoke ("jumpSplash", timeOut);
	}

	//END SPLASH
	/// <summary>
	/// END SPLASH.
	/// </summary>
	void jumpSplash()
	{
			LevelManager.Load (sceneName);
	}

}
}
