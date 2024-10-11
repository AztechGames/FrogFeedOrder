using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]GameObject StartP, InGameP, NextP, GameOverP;
    [SerializeField]Sprite MuteOn, MuteOff, TapticOn, TapticOff;

    public TextMeshProUGUI m_MoveText, m_LevelText;
    public GameObject m_Settings;
    
    private bool mute = true;

    private void Update()
    {
        m_MoveText.text = GameManager.Instance.MoveLeft + " / " + GameManager.Instance.MaxMove;
    }

    public void SetLevel()
    {
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 0) + 1);
        m_LevelText.text = "LEVEL " + PlayerPrefs.GetInt("Level", 0);
    }
    
    public void PanelControl()
    {
        StartP.SetActive(false);
        InGameP.SetActive(false);
        NextP.SetActive(false);
        GameOverP.SetActive(false);
        switch (GameManager.Instance.gamestate)
        {
            case GameManager.GAMESTATE.Start : StartP.SetActive(true);
                break;
            case GameManager.GAMESTATE.Ingame : InGameP.SetActive(true);
                break;
            case GameManager.GAMESTATE.Finish : NextP.SetActive(true);
                break;  
            case GameManager.GAMESTATE.GameOver : GameOverP.SetActive(true);
                break;
        }
    }
    
    public void Settings()
    {
        if(m_Settings.activeInHierarchy)
            m_Settings.SetActive(false);
        else 
            m_Settings.SetActive(true);
    }
    public void Mute()
    {
        mute = !mute;
        m_Settings.transform.GetChild(1).GetComponent<Image>().sprite = IconChanger(MuteOn, MuteOff, mute);
        AudioListener.pause = !mute;
    }
    public void Taptic()
    {
        GameManager.Instance.taptic = !GameManager.Instance.taptic;
        m_Settings.transform.GetChild(0).GetComponent<Image>().sprite = IconChanger(TapticOn, TapticOff, GameManager.Instance.taptic);
    }
    Sprite IconChanger(Sprite first, Sprite second,bool state)
    {
        return state ? first : second;
    }
}
