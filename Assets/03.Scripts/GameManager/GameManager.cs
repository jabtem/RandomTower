﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    enum LifeState { ON, OFF};
    public class UserInfo
    {
        int sp;
        virtual public int Sp
        {
            get
            {
                return sp;

            }
            set
            {
                sp = value;
            }
        }
        int cost;
        virtual public int Cost
        {
            get
            {
                return cost;
            }
            set
            {
                cost = value;
            }
        }
        int life;
        virtual public int Life
        {
            get
            {
                return life;
            }
            set
            {
                life = value;
                instance.SetLifeImages(LifeImage, life);
            }
        }

        Image[] lifeImage;
        virtual public Image[] LifeImage
        {
            get
            {
                return lifeImage;
            }
            set
            {
                lifeImage = value;
            }
        }

        virtual public Text CostText { get; set; }
        virtual public Text SpText { get; set; }
    }
    public class PlayerInfo : UserInfo
    {
        int sp;
        public override int Sp
        {
            get
            {
                return sp;
            }
            set
            {
                sp = value;
                SpText.text = sp.ToString();
            }
        }
        Text spText;
        public override Text SpText
        {
            get
            {
                return spText;
            }
            set
            {
                spText = value;
            }
        }
        int cost;
        public override int Cost
        {
            get
            {
                return cost;
            }
            set
            {
                cost = value;
                costText.text = cost.ToString();
            }
        }
        Text costText;
        public override Text CostText
        {
            get
            {
                return costText;
            }
            set
            {
                costText = value;
            }
        }

    }
    public void SetLifeImages(Image[] images, int life)
    {
        switch (life)
        {
            case 3:
                images[0].sprite = lifeSprite[(int)LifeState.ON];
                images[1].sprite = lifeSprite[(int)LifeState.ON];
                images[2].sprite = lifeSprite[(int)LifeState.ON];
                break;
            case 2:
                images[0].sprite = lifeSprite[(int)LifeState.OFF];
                break;
            case 1:
                images[1].sprite = lifeSprite[(int)LifeState.OFF];
                break;
            case 0:
                images[2].sprite = lifeSprite[(int)LifeState.OFF];
                break;
        }
    }

    

    public static GameManager instance;

    UserInfo player;
    public UserInfo Player
    {
        get
        {
            return player;
        }
    }
    UserInfo enemy;
    public UserInfo Enemy
    {
        get
        {
            return enemy;
        }
    }

    public Sprite[] lifeSprite;

    float gameTime;
    public float GameTime
    {
        get
        {
            return gameTime;
        }
    }

    //몬스터의 시간에따른 체력배수
    int hpMul;
    bool gameOver;
    bool timerStart;
    public bool TimerStart
    {
        set
        {
            timerStart = value;
        }
    }
    public int HpMul
    {
        get
        {
            return hpMul;
        }
    }

    //웨이브 숫자
    int waveCount;
    public int WaveCount
    {
        get
        {
            return waveCount;
        }
        set
        {
            waveCount = value;

            //웨이브가 변경될때마다 기본체력계수와 체력에대한 배수값이 초기화
            hpMul = 1;
            gameTime = 100 - waveCount * 10;
            deltaTime = 0;
            switch (waveCount)
            {
                case 1:
                    defaultHP = 100;
                    break;
                case 2:
                    defaultHP = 960;
                    break;
                case 3:
                    defaultHP = 2520;
                    break;

            }
        }
    }
    //몬스터 HP 기본수치, 현재 웨이브에따라 변경
    int defaultHP;
    public int DefaultHp
    {
        get
        {
            return defaultHP;
        }
    }

    float deltaTime;
    Text TimerText;
    Text WaveText;
    GameObject result;
    Text resultText;
    Button restartButton;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if(instance !=null)
        {
            Destroy(this.gameObject);
        }

        Application.targetFrameRate = 60;
        TimerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<Text>();
        WaveText = GameObject.FindGameObjectWithTag("Wave").GetComponent<Text>();
        WaveCount = 1;
        player = new PlayerInfo();
        enemy = new UserInfo();
        player.LifeImage = GameObject.FindGameObjectWithTag("PlayerLife").GetComponentsInChildren<Image>();
        enemy.LifeImage = GameObject.FindGameObjectWithTag("EnemyLife").GetComponentsInChildren<Image>();
        player.CostText = GameObject.FindGameObjectWithTag("PlayerCost").GetComponent<Text>();
        player.SpText = GameObject.FindGameObjectWithTag("PlayerSp").GetComponent<Text>();
        result = GameObject.FindGameObjectWithTag("Result");
        resultText = GameObject.FindGameObjectWithTag("ResultText").GetComponent<Text>();
        restartButton = GameObject.FindGameObjectWithTag("RestartButton").GetComponent<Button>();
        DefaultSetting(Player);
        DefaultSetting(Enemy);
    }

    private void Start()
    {
        TimerText.text = string.Format("{0:D2}:{1:D2}", (int)GameTime / 60, (int)GameTime % 60);
        WaveText.text = $"Wave {WaveCount}";
        restartButton.onClick.AddListener(() => { Restart(); });
        result.SetActive(false);
    }


    private void Update()
    {
        if(!gameOver && timerStart)
            Timer();

    }

    //기본값설정
    void DefaultSetting(UserInfo info)
    {
        info.Sp = 100;
        info.Cost = 10;
        info.Life = 3;
    }

    public bool bossSpawnOk;

    void Timer()
    {
        if(player.Life ==0)
        {
            SetResult(player.Life);
            gameOver = true;
        }
        else if (WaveCount >= 4)
        {
            SetResult(player.Life);
            gameOver = true;
        }

        //보스소환조건충족
        if (GameTime <= 0 && !bossSpawnOk && MonsterSpawnerManager.instance.spawnOk)
        {
            //WaveCount += 1;
            bossSpawnOk = true;
            //일반몬스터가 생성되지않게함
            MonsterSpawnerManager.instance.spawnOk = false;
        }
        else if(GameTime >= 0 && MonsterSpawnerManager.instance.spawnOk)
        {
            gameTime -= Time.deltaTime;
            deltaTime += Time.deltaTime;
            TimerText.text = string.Format("{0:D2}:{1:D2}", ((int)GameTime) / 60, ((int)GameTime) % 60);
        }
        else if(!MonsterSpawnerManager.instance.spawnOk)
        {
            TimerText.text = "보스 출현중";
        }


        WaveText.text = $"Wave {WaveCount}";
        if (deltaTime >=10)
        {
            hpMul += 1;
            deltaTime = 0;
        }
    }

    void SetResult(int life)
    {
        result.SetActive(true);
        if (life > 0)
        {
            resultText.text = "Win !";
        }
        else
            resultText.text = "Lose..";
    }

    void Restart()
    {
        //별도의 씬이 존재하지않으므로 현재씬 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void WaveUp()
    {
        WaveCount += 1;
        deltaTime = 0;
        MonsterSpawnerManager.instance.spawnOk = true;
    }
}
