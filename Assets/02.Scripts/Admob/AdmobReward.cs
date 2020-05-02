using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobReward : MonoSingleton<AdmobReward>
{
    private readonly string unitID = "ca-app-pub-1161518349039487/5921247929";
    private readonly string test_unitID = "ca-app-pub-3940256099942544/5224354917";

    private RewardBasedVideoAd rewardAd;

    public override void Init()
    {
        rewardAd = RewardBasedVideoAd.Instance;
        InitAd();
        base.Init();
    }

    private void InitAd()
    {
        AdRequest request = new AdRequest.Builder().Build();

        rewardAd.LoadAd(request, unitID);

        rewardAd.OnAdClosed += (sender, e) => { InitAd(); };    // 광고를 보면 다시로드
        //rewardAd.OnAdCompleted += (sender, e) => { GlobalManager.Instance.RewardAdCompleted(); };
    }

    public void ShowRewardAd()
    {
        StartCoroutine("cor_ShowRewardAd");
    }
    private IEnumerator cor_ShowRewardAd()
    {
        while (!rewardAd.IsLoaded())
        {
            yield return null;
        }
        rewardAd.Show();
    }

}
