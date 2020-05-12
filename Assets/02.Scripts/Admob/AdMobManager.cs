using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobManager : MonoSingleton<AdMobManager>
{
    public override void Init()
    {
        MobileAds.Initialize(initStatus => {
            AdmobBanner.Instance.Init();
            AdmobInterstitial.Instance.Init();
            //AdmobReward.Instance.Init();
        });
        base.Init();
    }

    IEnumerator Cor_Init()
    {
        yield return new WaitForSeconds(3);
        AdmobBanner.Instance.Init();
        AdmobReward.Instance.Init();
    }

    public void ShowAd()
    {
        AdmobInterstitial.Instance.show();
    }
}
