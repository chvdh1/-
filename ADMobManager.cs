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
        });//�ʱ�ȭ �Ϸ�
    }

    // �̷��� ���� ������ �׻� �׽�Ʈ ���� �����ϵ��� �����˴ϴ�.
#if UNITY_ANDROID
    private string f_adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
  private string f_adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
    private string f_adUnitId = "unused";
#endif


    //���� ����
    public void LoadInterstitialAd() //���� �ε�
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");//���� ���� �ε��ϴ� ���Դϴ�.

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
                    //���� ���� ������ ���� ���� �ε����� ���߽��ϴ�.
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());
                //������ ���Ե� ���� ����

                interstitialAd = ad;
            });
        RegisterEventHandlers(interstitialAd); //�̺�Ʈ ���
    }

    public void ShowAd() //���� ����
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            //���� ���� ǥ���ϰ� �ֽ��ϴ�.
            interstitialAd.Show();
        }
        else
        {
            LoadInterstitialAd(); //���� ��ε�

            Debug.LogError("Interstitial ad is not ready yet.");
            //���� ���� ���� �غ���� �ʾҽ��ϴ�.
        }
    }

    private void RegisterEventHandlers(InterstitialAd ad) //���� �̺�Ʈ
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {

            //���� �ֱ�
            Debug.Log(string.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
            //���� ����� {0} {1}��(��) �����߽��ϴ�.
        };
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
            //���� ���� ������ ��ϵǾ����ϴ�.
        };
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
            //���� ���� Ŭ���߽��ϴ�.
        };
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
            //���� ���� ��ü ȭ�� �������� ���Ƚ��ϴ�.
        };
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            //���� ���� ��ü ȭ�� �������� �������ϴ�.
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
            //���� ���� ������ ���� ��ü ȭ�� �������� ���� ���߽��ϴ�.
        };
    }

    private void RegisterReloadHandler(InterstitialAd ad) //�������� ���� ��ε�(���� �ʿ�)
        //�̰� �� �ִ°���...?
    {
        ad.OnAdFullScreenContentClosed += (null);
        {
            Debug.Log("Interstitial Ad full screen content closed.");
            //���� ���� ��ü ȭ�� �������� �������ϴ�.
            LoadInterstitialAd();
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
            //���� ���� ������ ���� ��ü ȭ�� �������� ���� ���߽��ϴ�.
            LoadInterstitialAd();
        };
    }



    //������ ����

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
        // �� ���� �ε��ϱ� ���� ���� ���� �����մϴ�.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // ���� �ε��ϴ� �� ���Ǵ� ��û�� ����ϴ�.
        var adRequest = new AdRequest();

        // s���� �ε� ��û�� �����ϴ�.
        RewardedAd.Load(r_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // ������ null�� �ƴϸ� �ε� ��û�� ������ ���Դϴ�.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                //"����� �Բ� �ε�� ������ ����: "
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
        //"������ ����� ����ڿ��� ������ �����մϴ�. ����: {0}, �ݾ�: {1}.";
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                //����ڿ��� �����մϴ�.
                ui.txt("���� �Ϸ�!");
                Debug.Log(string.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // ����� ������ �߻��� ������ �����Ǵ� ��� �߻��մϴ�.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        //������ ��ϵǸ� �߻��մϴ�.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // ���� Ŭ���� ��ϵǸ� �߻��մϴ�.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        //������ �� �߻��մϴ�.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        //�������� ���� �� �߻��մϴ�.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            LoadRewardedAd();
        };
        // ���� ��ü ȭ�� �������� ���� ���� ��� �߻��մϴ�.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
            LoadRewardedAd();
        };
    }
}

