using UnityEngine;
using TapsellPlusSDK;
using UnityEngine.UI;
using System.Collections;


public class TapsellManager : MonoBehaviour
{
    public static TapsellManager Instance;
    

    public static string TAPSELLPLUS_KEY = "idkctijjtqssabpqgtmdbpcoprrotghkqfhpdnomamnaelimedqjemakdntdtdjgnhdhct";

    public static string INTERSTITIALAD_KEY = "61d03065aee24564854943c3";
    public static string REWARDAD_KEY = "61d024a00f13b31484dc7b7c";
    public static string NATIVE_KEY = "61d024deec273e4ff6db98bc";


    public bool isNativeLoaded = false;
    public bool isRewardLoaded = false;
    public bool isIntertitialLoaded = false;

    [HideInInspector] public int numberOfCanShowRewardAdAfterEveryLoss;  // valueed
    [SerializeField] int _numberOfCanShowRewardAdAfterEveryLoss;

    [SerializeField] int _countOfCanSendRequestAfetrFailLoad;

    int _countRequestIntertitial = 0;
    int _countRequestReward = 0;
    int _countRequestNative = 0;

    string _responInterstitial_Id;
    string _responReward_Id;
    string _responNative_Id;

    bool _isRewardReceived = false;

    [SerializeField] Text adHeadline;
    [SerializeField] Text adCallToAction;
    [SerializeField] Text adBody;
    [SerializeField] RawImage adImage;
    [SerializeField] RawImage adIcon;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        numberOfCanShowRewardAdAfterEveryLoss = _numberOfCanShowRewardAdAfterEveryLoss;
    }

    void Start()
    {
       TapsellPlus.Initialize(TAPSELLPLUS_KEY,
           adNetworkName => Debug.Log(adNetworkName + " Initialized Successfully."),
           error => Debug.Log(error.ToString()));
      
       TapsellPlus.SetGdprConsent(true);
      
       RequestNativeAd();
       RequestInterstitialAd();
       RequestRewardAd();
    }

    public void ReloadAds()
    {
        StartCoroutine(reloadAds());
    }

    IEnumerator reloadAds()
    {
        yield return null;

        ResetNumberOfCanShowRewardVideoAfterEveryLoss();
        RequestInterstitialAd();
        RequestNativeAd();
        RequestRewardAd();
    }

    #region Reward

    public void ResetNumberOfCanShowRewardVideoAfterEveryLoss()
    {
        numberOfCanShowRewardAdAfterEveryLoss = _numberOfCanShowRewardAdAfterEveryLoss;
    }
    public void RequestRewardAd()
    {
        TapsellPlus.RequestRewardedVideoAd(REWARDAD_KEY,

                  tapsellPlusAdModel =>
                  {
                      Debug.Log("on response " + tapsellPlusAdModel.responseId);
                      _responReward_Id = tapsellPlusAdModel.responseId;
                      isRewardLoaded = true;
                  },
                  error =>
                  {
                      isRewardLoaded = false;
                      Debug.Log("Error " + error.message);
                      if (_countRequestReward < _countOfCanSendRequestAfetrFailLoad)
                      {
                          _countRequestReward++;
                          RequestRewardAd();
                      }
                      else
                      {
                          _countRequestReward = 0;
                      }
                  }
              );
    }

    public void ShowReward()
    {
        TapsellPlus.ShowRewardedVideoAd(_responReward_Id,

                  tapsellPlusAdModel =>
                  {
                      Debug.Log("onOpenAd " + tapsellPlusAdModel.zoneId);
                  },
                  tapsellPlusAdModel =>
                  {
                      Debug.Log("onReward " + tapsellPlusAdModel.zoneId);
                      _isRewardReceived = true;
                  },
                  tapsellPlusAdModel =>
                  {
                      Debug.Log("onCloseAd " + tapsellPlusAdModel.zoneId);
                      if (_isRewardReceived)
                      {
                          UIManeger.instance.HideLossMenu();
                          GameManeger.Instance.ContinueAfterLoss();
                          UIManeger.instance.showHeader(true);
                          _isRewardReceived = false;
                      }
                      else
                      {
                          UIManeger.instance.ShowLossMenuAfterNotCompletWatchRewardVideo();
                      }
                      isRewardLoaded = false;

                      RequestRewardAd();
                  },
                  error =>
                  {
                      Debug.Log("onError " + error.errorMessage);
                  }
              );

        
    }
    #endregion

    #region Intertitial
    public void RequestInterstitialAd()
    {
        TapsellPlus.RequestInterstitialAd(INTERSTITIALAD_KEY,

                  tapsellPlusAdModel =>
                  {
                      Debug.Log("on response " + tapsellPlusAdModel.responseId);
                      _responInterstitial_Id = tapsellPlusAdModel.responseId;
                      isIntertitialLoaded = true;
                  },
                  error =>
                  {
                      isIntertitialLoaded = false;
                      Debug.Log("Error " + error.message);

                      if (_countRequestIntertitial < _countOfCanSendRequestAfetrFailLoad)
                      {
                          RequestInterstitialAd();
                          _countRequestIntertitial++;
                      }
                      else
                      {
                          _countRequestIntertitial = 0;
                      }
                  }
              );

        
    }

    public void ShowInterstitialAd()
    {
        TapsellPlus.ShowInterstitialAd(_responInterstitial_Id,

           tapsellPlusAdModel =>
           {
               Debug.Log("onOpenAd " + tapsellPlusAdModel.zoneId);
           },
           tapsellPlusAdModel =>
           {
               Debug.Log("onCloseAd " + tapsellPlusAdModel.zoneId);

           },
           error =>
           {
               Debug.Log("onError " + error.errorMessage);
           }
       );

        isIntertitialLoaded = false;
        RequestInterstitialAd();

    
    }

    #endregion

    #region Native

    public void RequestNativeAd()
    {
        TapsellPlus.RequestNativeBannerAd(NATIVE_KEY,

            tapsellPlusAdModel =>
            {
                Debug.Log("On Response " + tapsellPlusAdModel.responseId);
                _responNative_Id = tapsellPlusAdModel.responseId;
                isNativeLoaded = true;
                RigesterNativeItems();
            },
            error =>
            {
                Debug.Log("Error " + error.message);
                if (_countRequestNative < _countOfCanSendRequestAfetrFailLoad)
                {
                    _countRequestNative++;
                    RequestNativeAd();
                }
                else
                {
                    _countRequestNative = 0;
                }
            }
        );

    }

    public void RigesterNativeItems()
    {
        TapsellPlus.ShowNativeBannerAd(_responNative_Id, this,

                  tapsellPlusNativeBannerAd =>
                  {
                      Debug.Log("onOpenAd " + tapsellPlusNativeBannerAd.zoneId);
                      adHeadline.text = ArabicSupport.ArabicFixer.Fix(tapsellPlusNativeBannerAd.title);
                      adCallToAction.text = ArabicSupport.ArabicFixer.Fix(tapsellPlusNativeBannerAd.callToActionText);
                      adBody.text = ArabicSupport.ArabicFixer.Fix(tapsellPlusNativeBannerAd.description);
                      adImage.texture = tapsellPlusNativeBannerAd.landscapeBannerImage;
                      adIcon.texture = tapsellPlusNativeBannerAd.iconImage;

                      tapsellPlusNativeBannerAd.RegisterImageGameObject(adImage.gameObject);
                      tapsellPlusNativeBannerAd.RegisterIconImageGameObject(adIcon.gameObject);
                      tapsellPlusNativeBannerAd.RegisterHeadlineTextGameObject(adHeadline.gameObject);
                      tapsellPlusNativeBannerAd.RegisterCallToActionGameObject(adCallToAction.gameObject);
                      tapsellPlusNativeBannerAd.RegisterBodyTextGameObject(adBody.gameObject);
                  },
                  error =>
                  {
                      Debug.Log("onError " + error.errorMessage);
                  }
              );
    }

    #endregion
}
