using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if(instance !=null)
        {
            Destroy(this.gameObject);
        }

        Application.targetFrameRate = 60; 

        player = new PlayerInfo();
        enemy = new UserInfo();
        player.LifeImage = GameObject.FindGameObjectWithTag("PlayerLife").GetComponentsInChildren<Image>();
        enemy.LifeImage = GameObject.FindGameObjectWithTag("EnemyLife").GetComponentsInChildren<Image>();
        player.CostText = GameObject.FindGameObjectWithTag("PlayerCost").GetComponent<Text>();
        player.SpText = GameObject.FindGameObjectWithTag("PlayerSp").GetComponent<Text>();
        DefaultSetting(Player);
        DefaultSetting(Enemy);
    }

    //기본값설정
    void DefaultSetting(UserInfo info)
    {
        info.Sp = 100;
        info.Cost = 10;
        info.Life = 3;
    }


}
