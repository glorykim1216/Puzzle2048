using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public GameObject SettingUI;
    public Button SettingBtn;
    public Button DoneBtn;
    public Button SoundBtn;
    public AudioSource BGMSound;
    public AudioSource mergeSound;

    public GameObject soundOn;
    public GameObject soundOff;

    bool isSoundOn;
    public void Init()
    {
        if (DBManager.Instance.ItemList.sound == 1)
            isSoundOn = true;
        else
            isSoundOn = false;

        SetSoundOnOff();

        SettingBtn.onClick.AddListener(() => { ShowSettingUI(); });
        DoneBtn.onClick.AddListener(()=> { SaveSound(); });
        SoundBtn.onClick.AddListener(() => 
        {
            isSoundOn = !isSoundOn;
            SetSoundOnOff();
        });
    }

    public void PlayMergeSound()
    {
        if(isSoundOn == true)
        {
            mergeSound.Play();
        }
    }

    public void SetSoundOnOff()
    {
        if (isSoundOn == true)
        {
            // ON
            BGMSound.Play();
            soundOn.SetActive(true);
            soundOff.SetActive(false);
        }
        else
        {
            // OFF
            BGMSound.Stop();
            soundOn.SetActive(false);
            soundOff.SetActive(true);
        }
    }
    public void ShowSettingUI()
    {
        SettingUI.SetActive(true);
    }
    public void SaveSound()
    {
        if (isSoundOn == true)
            DBManager.Instance.UpdateItemTable_Sound(1);
        else
            DBManager.Instance.UpdateItemTable_Sound(0);

        SettingUI.SetActive(false);
    }
}
