using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobBanner : MonoSingleton<AdmobBanner>
{
    private readonly string unitID = "ca-app-pub-1161518349039487/2909099446";
    private readonly string test_unitID = "ca-app-pub-3940256099942544/6300978111";

    private BannerView banner;

    public override void Init()
    {
        InitAd();
        base.Init();
    }

    public void InitAd()
    {
        banner = new BannerView(unitID, AdSize.SmartBanner, AdPosition.Bottom);

        AdRequest request = new AdRequest.Builder().Build();   // 광고 요청

        banner.LoadAd(request);
    }

    public void ToggleAd(bool active)
    {
        if (active)
        {
            banner.Show();
        }
        else
        {
            banner.Hide();
        }
    }

    public void DestroyAd()
    {
        banner.Destroy();
    }
}
