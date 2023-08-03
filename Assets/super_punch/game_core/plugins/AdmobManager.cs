using System;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

/// <summary>
/// Admob manager.
/// </summary>
public class AdmobManager : MonoBehaviour {
	protected 	static AdmobManager		instance;
	private BannerView 		_bannerView;
	private InterstitialAd 	_interstitial;


	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	public static AdmobManager Instance
	{
		get
		{
			if(instance == null)
			{
				GameObject obj=new GameObject("AdmobManager");
				instance =obj.AddComponent<AdmobManager>();
				DontDestroyOnLoad(Instance.gameObject);
				if (instance == null)
				{
					Debug.LogError("An instance of is needed in the scene, but there is none.");
				}
			}
			
			return instance;
		}
	}

	/// <summary>
	/// http://answers.unity3d.com/questions/834119/admob-device-id-on-ios-for-test-ads.html
	/// http://wiki.unity3d.com/index.php?title=MD5
	/// HOW TO GET THE DEVICE ID.
	/// UnityEngine.AndroidJavaClass 	up 				= new UnityEngine.AndroidJavaClass("com.unity3d.player.UnityPlayer");
	/// UnityEngine.AndroidJavaObject 	currentActivity = up.GetStatic<UnityEngine.AndroidJavaObject>("currentActivity");
	/// UnityEngine.AndroidJavaObject 	contentResolver = currentActivity.Call<UnityEngine.AndroidJavaObject>("getContentResolver");
	/// UnityEngine.AndroidJavaObject 	secure 			= new UnityEngine.AndroidJavaObject("android.provider.Settings$Secure");
	/// string deviceID = secure.CallStatic<string>("getString", contentResolver, "android_id");
	/// Debug.Log(Md5Sum(deviceID).ToUpper());
	/// </summary>


	/// <summary>
	/// Md5Sum.
	/// </summary>
	/// <returns>MD5</returns>
	/// <param name="strToEncrypt">String to encrypt.</param>
	public static string Md5Sum(string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
		
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
		
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		
		return hashString.PadLeft(32, '0');
	}

	/// <summary>
	/// Requests the banner.
	/// http://stackoverflow.com/questions/24268888/how-to-test-admob-in-real-device-if-without-ad-unit-id
	/// https://github.com/googleads/googleads-mobile-android-examples/blob/master/admob/InterstitialExample/app/src/main/res/values/strings.xml
	/// https://developers.google.com/admob/android/quick-start
	/// </summary>
	public static void RequestBanner()
	{
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = "ca-app-pub-3940256099942544/6300978111";
		#elif UNITY_IPHONE
		string adUnitId = "ca-app-pub-3940256099942544/6300978111";
		#else
		string adUnitId = "unexpected_platform";
		#endif
		
		// Create a 320x50 banner at the top of the screen.
		//https://developers.google.com/admob/android/banner
		///320x50 	Standard Banner 	Phones and Tablets 	BANNER
		///320x100 	Large Banner 	Phones and Tablets 	LARGE_BANNER
		///300x250 	IAB Medium Rectangle 	Phones and Tablets 	MEDIUM_RECTANGLE
		///468x60 	IAB Full-Size Banner 	Tablets 	FULL_BANNER
		/// 728x90 	IAB Leaderboard 	Tablets 	LEADERBOARD
		/// Screen width x 32|50|90 	Smart Banner 	Phones and Tablets 	SMART_BANNER
		Instance._bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
		// Register for ad events.
		Instance._bannerView.AdLoaded 			+= HandleAdLoaded;
		Instance._bannerView.AdFailedToLoad 	+= HandleAdFailedToLoad;
		Instance._bannerView.AdOpened 			+= HandleAdOpened;
		Instance._bannerView.AdClosing 			+= HandleAdClosing;
		Instance._bannerView.AdClosed 			+= HandleAdClosed;
		Instance._bannerView.AdLeftApplication 	+= HandleAdLeftApplication;
		// Load a banner ad.
		Instance._bannerView.LoadAd(createAdRequest());
	}

	/// <summary>
	/// Requests the interstitial.
	/// http://stackoverflow.com/questions/24268888/how-to-test-admob-in-real-device-if-without-ad-unit-id
	/// https://github.com/googleads/googleads-mobile-android-examples/blob/master/admob/InterstitialExample/app/src/main/res/values/strings.xml
	/// https://developers.google.com/admob/android/quick-start
	/// </summary>
	public static void RequestInterstitial()
	{
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = "ca-app-pub-3940256099942544/1033173712";
		#elif UNITY_IPHONE
		string adUnitId = "INSERT_IOS_INTERSTITIAL_AD_UNIT_ID_HERE";
		#else
		string adUnitId = "unexpected_platform";
		#endif
		
		// Create an interstitial.
		Instance._interstitial = new InterstitialAd(adUnitId);
		// Register for ad events.
		Instance._interstitial.AdLoaded 				+= 	HandleInterstitialLoaded;
		Instance._interstitial.AdFailedToLoad 			+= 	HandleInterstitialFailedToLoad;
		Instance._interstitial.AdOpened 				+= 	HandleInterstitialOpened;
		Instance._interstitial.AdClosing 				+= 	HandleInterstitialClosing;
		Instance._interstitial.AdClosed 				+= 	HandleInterstitialClosed;
		Instance._interstitial.AdLeftApplication 		+= 	HandleInterstitialLeftApplication;
		
		//GoogleMobileAdsDemoHandler handler 	= 	new GoogleMobileAdsDemoHandler();
		//Instance._interstitial.SetInAppPurchaseHandler(handler);

		// Load an interstitial ad.
		Instance._interstitial.LoadAd(createAdRequest());
	}

	/// <summary>
	/// Returns an ad request with custom ad targeting.
	/// </summary>
	/// <returns>The ad request.</returns>
	public static AdRequest createAdRequest()
	{
		return new AdRequest.Builder()
				.AddTestDevice(AdRequest.TestDeviceSimulator)
				.AddTestDevice("3ED9A278ECB5ACD023376C72D399C1C0F")
				.AddKeyword("game")
				.SetGender(Gender.Male)
				.SetBirthday(new DateTime(1985, 1, 1))
				.TagForChildDirectedTreatment(false)
				.AddExtra("color_bg", "9B30FF")
				.Build();
	}

	/// <summary>
	/// Shows the interstitial.
	/// </summary>
	public static void ShowInterstitial()
	{
		if (Instance._interstitial!=null && Instance._interstitial.IsLoaded())
		{
			Instance._interstitial.Show ();
			
		}
	}

	/// <summary>
	/// Hides the interstitial.
	/// </summary>
	public static void DestroyInterstitial(){
		if (Instance._interstitial!=null && Instance._interstitial.IsLoaded())
		{
			Instance._interstitial.Destroy ();
			
		}
	}
	/// <summary>
	/// Shows the banner.
	/// </summary>
	public static void ShowBanner(){

				if(Instance._bannerView!=null){
					Instance._bannerView.Show ();
				}
		
	}

	/// <summary>
	/// Hides the banner.
	/// </summary>
	public static void HideBanner(){

				if (Instance._bannerView!=null) {
						Instance._bannerView.Hide ();
				}
	}

	/// <summary>
	/// Destroies the banner.
	/// </summary>
	public static void DestroyBanner()
	{
				if (Instance._bannerView!=null) {
						Instance._bannerView.Destroy ();
				}
	}

	#region Banner callback handlers
	
	/// <summary>
	/// Handles the ad loaded.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public static void HandleAdLoaded(object sender, EventArgs args)
	{
		print("HandleAdLoaded event received.");
	}
	
	/// <summary>
	/// Handles the ad failed to load.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public static void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		print("HandleFailedToReceiveAd event received with message: " + args.Message);
	}
	
	/// <summary>
	/// Handles the ad opened.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public static void HandleAdOpened(object sender, EventArgs args)
	{
		print("HandleAdOpened event received");
	}
	
	/// <summary>
	/// Handles the ad closing.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public static void HandleAdClosing(object sender, EventArgs args)
	{
		print("HandleAdClosing event received");
	}
	
	/// <summary>
	/// Handles the ad closed.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public static void HandleAdClosed(object sender, EventArgs args)
	{
		print("HandleAdClosed event received");
	}
	
	/// <summary>
	/// Handles the ad left application.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public static void HandleAdLeftApplication(object sender, EventArgs args)
	{
		print("HandleAdLeftApplication event received");
	}
	
	#endregion
	
	#region Interstitial callback handlers
	
	/// <summary>
	/// Handles the interstitial loaded.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public static void HandleInterstitialLoaded(object sender, EventArgs args)
	{
		print("HandleInterstitialLoaded event received.");
	}
	
	/// <summary>
	/// Handles the interstitial failed to load.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public static void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		print("HandleInterstitialFailedToLoad event received with message: " + args.Message);
	}
	
	/// <summary>
	/// Handles the interstitial opened.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public static void HandleInterstitialOpened(object sender, EventArgs args)
	{
		print("HandleInterstitialOpened event received");
	}
	
	/// <summary>
	/// Handles the interstitial closing.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public static void HandleInterstitialClosing(object sender, EventArgs args)
	{
		print("HandleInterstitialClosing event received");
	}
	
	/// <summary>
	/// Handles the interstitial closed.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public static void HandleInterstitialClosed(object sender, EventArgs args)
	{
		print("HandleInterstitialClosed event received");
	}
	
	/// <summary>
	/// Handles the interstitial left application.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public static void HandleInterstitialLeftApplication(object sender, EventArgs args)
	{
		print("HandleInterstitialLeftApplication event received");
	}
	
	#endregion
}
