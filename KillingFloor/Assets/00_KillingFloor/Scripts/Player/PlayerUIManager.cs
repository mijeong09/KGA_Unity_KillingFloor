using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static UnityEngine.Rendering.DebugUI;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<PlayerUIManager>();
            }

            return m_instance;
        }
    }
    private static PlayerUIManager m_instance; // 싱글톤이 할당될 변수

    public TMP_Text playerLevel;
    public TMP_Text hpText;         // 체력 표시
    public TMP_Text shiedldText;    // 실드 표시
    public TMP_Text ammoText;       // 탄약 표시
    public TMP_Text totalAmmoText;  // 남은 탄약
    public TMP_Text grenadeText;    // 남은 수류탄
    public TMP_Text coinText;       // 현재 재화
    public TMP_Text weightText;     // 현재 무게
    public Slider healSlider;        // 힐 슬라이더
    public GameObject equipUI;
    public GameObject shopUI;

    // 코인 증가효과 계산용 변수
    private int coin;
    private int targetCoin;



    //JunOh
    public TMP_Text warningSubText;   // 알림 내용
    public TMP_Text noticeTextText;   // 알림 로고 정보
    public TMP_Text noticeCountText;  // 알림 웨이브 정보
    public TMP_Text zombieCountText;  // 좀비 수
    public TMP_Text timerCountText;   // 타이머
    public TMP_Text zombieWaveText;   // 좀비 웨이브 정보
    public GameObject CountUI;
    public GameObject TimerUI;

    public void Update()
    {
        CoinUpdate();
        SetNoticeWave();
        SetZombieWave();
    }

    // 체력 텍스트 갱신
    public void SetLevel(float value)
    {
        playerLevel.text = string.Format("{0}", value);
    }
    public void SetHP(float value)
    {
        hpText.text = string.Format("{0}", value);
    }
    // 실드 텍스트 갱신
    public void SetArmor(float value)
    {
        shiedldText.text = string.Format("{0}", value);
    }
    public void SetAmmo(float value)
    {
        if (value == 999)
        { ammoText.text = string.Format("∞"); }
        else
            ammoText.text = string.Format("{0}", value);
    }
    public void SetRemainingAmmo(float value)
    {
        if (value == 999)
        { totalAmmoText.text = string.Format("∞"); }
        else
            totalAmmoText.text = string.Format("{0}", value);
    }
    public void SetGrenade(float value)
    {
        grenadeText.text = string.Format("{0}", value);
    }
    public void SetHeal(float value)
    {
        healSlider.value = value;
    }

    // 코인 획득
    public void SetCoin(int value)
    {
        targetCoin = value;

    }
    // 코인 증가 업데이트
    public void CoinUpdate()
    {

        if (coin < targetCoin)
        {
            coin += Mathf.CeilToInt(1f * Time.deltaTime); // 초당 코인 업데이트
            if (coin >= targetCoin)
            {
                coin = targetCoin; // 현재 코인에 도달하면 멈춤
            }
            coinText.text = string.Format("{0}", coin);
        }

        else
        {
            coin -= Mathf.CeilToInt(1f * Time.deltaTime); // 초당 코인 업데이트
            if (coin <= targetCoin)
            {
                coin = targetCoin; // 현재 코인에 도달하면 멈춤
            }
            coinText.text = string.Format("{0}", coin);
        }


    }
    public void SetWeight(float value)
    {
        weightText.text = string.Format("{0}", value);
    }

    //JunOh
    public void SetNotice(string value)
    {
        warningSubText.text = string.Format("{0}", value);
    }
    public void SetNoticeWave()
    {
        noticeCountText.text = string.Format("[ {0}/ {1} ]", GameManager.instance.round, GameManager.instance.wave);
    }

    public void SetNoticeLogo(string noticeTextValue)
    {
        noticeTextText.text = string.Format("{0}", noticeTextValue);
    }

    public void SetZombieCount(float countValue)
    {
        zombieCountText.text = string.Format("{0}", countValue);
    }

    public void SetTimerCount(int value)
    {
        timerCountText.text = string.Format("{0}:{1:D2}", value / 60, value % 60);
    }

    public void SetZombieWave()
    {
        zombieWaveText.text = string.Format("{0}/ {1}", GameManager.instance.round, GameManager.instance.wave);
    }
    //JunOh
}