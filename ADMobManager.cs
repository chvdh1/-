using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;


public class ADMobManager : MonoBehaviour
{
    public ui ui;

    private InterstitialAd interstitialAd;

    public void Start()
    {
        MobileAds.Initialize((InitializationStatus initStatus) => {
            LoadInterstitialAd();
            LoadRewardedAd();
        });//초기화 완료
    }

    // 이러한 광고 단위는 항상 테스트 광고를 게재하도록 구성됩니다.
#if UNITY_ANDROID
    private string f_adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
  private string f_adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
    private string f_adUnitId = "unused";
#endif


    //전명 광고
    public void LoadInterstitialAd() //광고 로드
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");//전면 광고를 로드하는 중입니다.

        var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

        InterstitialAd.Load(f_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    //전면 광고가 오류로 인해 광고를 로드하지 못했습니다.
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());
                //응답이 포함된 전면 광고

                interstitialAd = ad;
            });
        RegisterEventHandlers(interstitialAd); //이벤트 등록
    }

    public void ShowAd() //광고 보기
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            //전면 광고를 표시하고 있습니다.
            interstitialAd.Show();
        }
        else
        {
            LoadInterstitialAd(); //광고 재로드

            Debug.LogError("Interstitial ad is not ready yet.");
            //전면 광고가 아직 준비되지 않았습니다.
        }
    }

    private void RegisterEventHandlers(InterstitialAd ad) //광고 이벤트
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {

            //보상 주기
            Debug.Log(string.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
            //전면 광고는 {0} {1}을(를) 지불했습니다.
        };
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
            //전면 광고에 노출이 기록되었습니다.
        };
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
            //전면 광고를 클릭했습니다.
        };
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
            //전면 광고 전체 화면 콘텐츠가 열렸습니다.
        };
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            //전면 광고 전체 화면 콘텐츠가 닫혔습니다.
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
            //전면 광고가 오류로 인해 전체 화면 콘텐츠를 열지 못했습니다.
        };
    }

    private void RegisterReloadHandler(InterstitialAd ad) //수동으로 광고 재로드(선언 필요)
        //이건 왜 있는거지...?
    {
        ad.OnAdFullScreenContentClosed += (null);
        {
            Debug.Log("Interstitial Ad full screen content closed.");
            //전면 광고 전체 화면 콘텐츠가 닫혔습니다.
            LoadInterstitialAd();
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
            //전면 광고가 오류로 인해 전체 화면 콘텐츠를 열지 못했습니다.
            LoadInterstitialAd();
        };
    }



    //리워드 광고

    private RewardedAd rewardedAd;
#if UNITY_ANDROID
    private string r_adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
  private string r_adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
  private string r_adUnitId = "unused";
#endif

    public void LoadRewardedAd()
    {
        // 새 광고를 로드하기 전에 이전 광고를 정리합니다.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // 광고를 로드하는 데 사용되는 요청을 만듭니다.
        var adRequest = new AdRequest();

        // s광고 로드 요청을 보냅니다.
        RewardedAd.Load(r_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // 오류가 null이 아니면 로드 요청이 실패한 것입니다.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                //"응답과 함께 로드된 보상형 광고: "
                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());
               
                rewardedAd = ad;

                RegisterEventHandlers(rewardedAd);
            });
    }

    public void ShowRewardedAd()
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";
        //"보상형 광고는 사용자에게 보상을 제공합니다. 유형: {0}, 금액: {1}.";
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                //사용자에게 보상합니다.
                ui.txt("보상 완료!");
                Debug.Log(string.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // 광고로 수익이 발생한 것으로 추정되는 경우 발생합니다.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        //노출이 기록되면 발생합니다.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // 광고 클릭이 기록되면 발생합니다.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        //열었을 때 발생합니다.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        //콘텐츠를 닫을 때 발생합니다.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            LoadRewardedAd();
        };
        // 광고가 전체 화면 콘텐츠를 열지 못한 경우 발생합니다.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
            LoadRewardedAd();
        };
    }
}

