using System;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

/// <summary>
/// Google mobile ads demo handler.
/// </summary>
public class GoogleMobileAdsDemoHandler : IInAppPurchaseHandler
{
    private readonly string[] validSkus = { "android.test.purchased" };

    
	/// <summary>
	/// Will only be sent on a success.
	/// </summary>
	/// <param name="result">Result.</param>
    public void OnInAppPurchaseFinished(IInAppPurchaseResult result)
    {
        result.FinishPurchase();
        GoogleMobileAdsDemoScript.OutputMessage = "Purchase Succeeded! Credit user here.";
    }

    //
	/// <summary>
	/// Check SKU against valid SKUs.
	/// </summary>
	/// <returns><c>true</c> if this instance is valid purchase the specified sku; otherwise, <c>false</c>.</returns>
	/// <param name="sku">Sku.</param>
    public bool IsValidPurchase(string sku)
    {
        foreach(string validSku in validSkus) {
            if (sku == validSku) {
                return true;
            }
        }
        return false;
    }
	
	/// <summary>
	/// Return the app's public key.
	/// </summary>
	/// <value>The android public key.</value>
    public string AndroidPublicKey
    {
        //In a real app, return public key instead of null.
        get { return null; }
    }
}

// 

/// <summary>
/// Example script showing how to invoke the 
/// Google Mobile Ads Unity plugin.
/// </summary>
public class GoogleMobileAdsDemoScript : MonoBehaviour
{

    private BannerView bannerView;
    private InterstitialAd interstitial;
    private static string outputMessage = "";

	/// <summary>
	/// Sets the output message.
	/// </summary>
	/// <value>The output message.</value>
    public static string OutputMessage
    {
        set { outputMessage = value; }
    }

	/// <summary>
	/// Awake this instance.
	/// </summary>
	public void Awake(){
		RequestInterstitial ();
		RequestBanner ();
	}

	/// <summary>
	/// Raises the GU event.
	/// </summary>
    void OnGUI()
    {
        // Puts some basic buttons onto the screen.
        GUI.skin.button.fontSize = (int) (0.05f * Screen.height);
        GUI.skin.label.fontSize = (int) (0.025f * Screen.height);

        Rect requestBannerRect = new Rect(0.1f * Screen.width, 0.05f * Screen.height,
                                   0.8f * Screen.width, 0.1f * Screen.height);
        if (GUI.Button(requestBannerRect, "Request Banner"))
        {
            RequestBanner();
        }

        Rect showBannerRect = new Rect(0.1f * Screen.width, 0.175f * Screen.height,
                                       0.8f * Screen.width, 0.1f * Screen.height);
        if (GUI.Button(showBannerRect, "Show Banner"))
        {
            bannerView.Show();
        }

        Rect hideBannerRect = new Rect(0.1f * Screen.width, 0.3f * Screen.height,
                                       0.8f * Screen.width, 0.1f * Screen.height);
        if (GUI.Button(hideBannerRect, "Hide Banner"))
        {
            bannerView.Hide();
        }

        Rect destroyBannerRect = new Rect(0.1f * Screen.width, 0.425f * Screen.height,
                                          0.8f * Screen.width, 0.1f * Screen.height);
        if (GUI.Button(destroyBannerRect, "Destroy Banner"))
        {
            bannerView.Destroy();
        }

        Rect requestInterstitialRect = new Rect(0.1f * Screen.width, 0.55f * Screen.height,
                                                0.8f * Screen.width, 0.1f * Screen.height);
        if (GUI.Button(requestInterstitialRect, "Request Interstitial"))
        {
            RequestInterstitial();
        }

        Rect showInterstitialRect = new Rect(0.1f * Screen.width, 0.675f * Screen.height,
                                             0.8f * Screen.width, 0.1f * Screen.height);
        if (GUI.Button(showInterstitialRect, "Show Interstitial"))
        {
            ShowInterstitial();
        }

        Rect destroyInterstitialRect = new Rect(0.1f * Screen.width, 0.8f * Screen.height,
                                                0.8f * Screen.width, 0.1f * Screen.height);
        if (GUI.Button(destroyInterstitialRect, "Destroy Interstitial"))
        {
            interstitial.Destroy();
        }

        Rect textOutputRect = new Rect(0.1f * Screen.width, 0.925f * Screen.height,
                                   0.8f * Screen.width, 0.05f * Screen.height);
        GUI.Label(textOutputRect, outputMessage);
    }

	/// <summary>
	/// Requests the banner.
	/// </summary>
    private void RequestBanner()
    {
        #if UNITY_EDITOR
            string adUnitId = "unused";
        #elif UNITY_ANDROID
		string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        #elif UNITY_IPHONE
            string adUnitId = "INSERT_IOS_BANNER_AD_UNIT_ID_HERE";
        #else
            string adUnitId = "unexpected_platform";
        #endif

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Top);
        // Register for ad events.
        bannerView.AdLoaded += HandleAdLoaded;
        bannerView.AdFailedToLoad += HandleAdFailedToLoad;
        bannerView.AdOpened += HandleAdOpened;
        bannerView.AdClosing += HandleAdClosing;
        bannerView.AdClosed += HandleAdClosed;
        bannerView.AdLeftApplication += HandleAdLeftApplication;
        // Load a banner ad.
        bannerView.LoadAd(createAdRequest());
    }


	/// <summary>
	/// Requests the interstitial.
	/// </summary>
    private void RequestInterstitial()
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
        interstitial = new InterstitialAd(adUnitId);
        // Register for ad events.
        interstitial.AdLoaded 				+= 	HandleInterstitialLoaded;
        interstitial.AdFailedToLoad 		+= 	HandleInterstitialFailedToLoad;
        interstitial.AdOpened 				+= 	HandleInterstitialOpened;
        interstitial.AdClosing 				+= 	HandleInterstitialClosing;
        interstitial.AdClosed 				+= 	HandleInterstitialClosed;
        interstitial.AdLeftApplication 		+= 	HandleInterstitialLeftApplication;

        GoogleMobileAdsDemoHandler handler 	= 	new GoogleMobileAdsDemoHandler();
        interstitial.SetInAppPurchaseHandler(handler);
        // Load an interstitial ad.
        interstitial.LoadAd(createAdRequest());
    }

	/// <summary>
	/// Returns an ad request with custom ad targeting.
	/// </summary>
	/// <returns>The ad request.</returns>
    private AdRequest createAdRequest()
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
    private void ShowInterstitial()
    {
        if (interstitial.IsLoaded())
        {
            interstitial.Show ();

        }
        else
        {
            print("Interstitial is not ready yet.");
        }
    }

    #region Banner callback handlers

	/// <summary>
	/// Handles the ad loaded.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
    public void HandleAdLoaded(object sender, EventArgs args)
    {
        print("HandleAdLoaded event received.");
    }

	/// <summary>
	/// Handles the ad failed to load.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("HandleFailedToReceiveAd event received with message: " + args.Message);
    }

	/// <summary>
	/// Handles the ad opened.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
    public void HandleAdOpened(object sender, EventArgs args)
    {
        print("HandleAdOpened event received");
    }

	/// <summary>
	/// Handles the ad closing.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
    void HandleAdClosing(object sender, EventArgs args)
    {
        print("HandleAdClosing event received");
    }

	/// <summary>
	/// Handles the ad closed.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
    public void HandleAdClosed(object sender, EventArgs args)
    {
        print("HandleAdClosed event received");
    }

	/// <summary>
	/// Handles the ad left application.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
    public void HandleAdLeftApplication(object sender, EventArgs args)
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
    public void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        print("HandleInterstitialLoaded event received.");
    }

	/// <summary>
	/// Handles the interstitial failed to load.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("HandleInterstitialFailedToLoad event received with message: " + args.Message);
    }

	/// <summary>
	/// Handles the interstitial opened.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
    public void HandleInterstitialOpened(object sender, EventArgs args)
    {
        print("HandleInterstitialOpened event received");
    }

	/// <summary>
	/// Handles the interstitial closing.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
    void HandleInterstitialClosing(object sender, EventArgs args)
    {
        print("HandleInterstitialClosing event received");
    }

	/// <summary>
	/// Handles the interstitial closed.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
    public void HandleInterstitialClosed(object sender, EventArgs args)
    {
        print("HandleInterstitialClosed event received");
    }

	/// <summary>
	/// Handles the interstitial left application.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
    public void HandleInterstitialLeftApplication(object sender, EventArgs args)
    {
        print("HandleInterstitialLeftApplication event received");
    }

    #endregion
}
