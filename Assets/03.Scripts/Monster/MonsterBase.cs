using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterBase : MonoBehaviour
{
    public enum MonsterType
    {
        NORMAL,SMALL,BIG,COUNT,KNIGHT,SNAKE,SILENCE,BOSSCOUNT
    }


    Queue<Vector3> movePointsQueue = new Queue<Vector3>();

    public Sprite[] MonsterImages;
    public TextMeshPro hpText;
    SpriteRenderer spr;
    MonsterSpawner parent;

    //총이동거리
    float totalMoveDistance;
    public float TotalMoveDistance
    {
        get
        {
            return totalMoveDistance;
        }
    }

    bool isDie;
    public bool IsDie
    {
        get
        {
            return isDie;
        }
    }

    int sp;
    public int Sp
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

    float hp;
    public float Hp
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
            hpText.text = hp.ToString();
        }
    }
    MonsterType type;
    public MonsterType Type
    {
        get
        {
            return type;
        }
        set
        {
            type = value;

            switch (type)
            {
                case MonsterType.NORMAL:
                    spr.sprite = MonsterImages[(int)MonsterType.NORMAL];
                    sp = 10;
                    speed = 1f;
                    break;
                case MonsterType.BIG:
                    spr.sprite = MonsterImages[(int)MonsterType.BIG];
                    Hp *= 5;
                    sp = 50;
                    speed = 0.88f;
                    break;
                case MonsterType.SMALL:
                    spr.sprite = MonsterImages[(int)MonsterType.SMALL];
                    sp = 10;
                    speed = 1.5f;
                    break;
                case MonsterType.KNIGHT:
                    spr.sprite = MonsterImages[(int)MonsterType.KNIGHT - 1];
                    sp = 100*GameManager.instance.WaveCount;
                    speed = 1f;
                    break;
                case MonsterType.SNAKE:
                    spr.sprite = MonsterImages[(int)MonsterType.SNAKE - 1];
                    sp = 100 * GameManager.instance.WaveCount;
                    speed = 1f;
                    break;
                case MonsterType.SILENCE:
                    spr.sprite = MonsterImages[(int)MonsterType.SILENCE - 1];
                    sp = 100 * GameManager.instance.WaveCount;
                    speed = 1f;
                    break;

            }
            if (movePointsQueue.Count != 0)
                movePoint = movePointsQueue.Dequeue();
        }
    }

    float speed;
    Vector3 movePoint;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        isDie = false;
        totalMoveDistance = 0f;
    }


    void MonsterDie()
    {
        isDie = true;

        movePointsQueue.Clear();

        parent.PushMonster(this.gameObject);
        if(!parent.IsAi)
        {
            GameManager.instance.Player.Sp += sp;
        }

        else if(parent.IsAi)
        {
            GameManager.instance.Enemy.Sp += sp;
        }

        if(type >= MonsterType.KNIGHT)
        {
            MonsterSpawnerManager.instance.BossDieCount += 1;
        }
    }

    private void Update()
    {
        //보스몹이아닌경우 보스가소환되면 삭제처리
        if(type < MonsterType.KNIGHT && !MonsterSpawnerManager.instance.spawnOk)
        {
             parent.PushMonster(this.gameObject);
        }

        totalMoveDistance += speed;
        transform.position = Vector2.MoveTowards(transform.position, movePoint,speed*0.005f);
        SortingOrderSet(1000 + (int)(totalMoveDistance));

        if(transform.position == movePoint)
        {
            if (movePointsQueue.Count == 0)
            {
                MonsterDie();
                if (!parent.IsAi)
                {
                    GameManager.instance.Player.Life -= 1;
                }
                else if (parent.IsAi)
                {
                    GameManager.instance.Enemy.Life -= 1;
                }
            }
                
            else
                movePoint = movePointsQueue.Dequeue();

        }
    }

    public void SetParentSpawner(MonsterSpawner monsterSpawner)
    {
        parent = monsterSpawner;
    }
    public void EnqueueMovePoints(Transform[] tr)
    {
        foreach(var point in tr)
        {
            movePointsQueue.Enqueue(point.position);
        }
    }

    void SortingOrderSet(int num)
    {
        spr.sortingOrder = num;
        hpText.sortingOrder = num + 1;
    }

    public void BulletHit(int num)
    {
        if (isDie)
            return;

        Hp -= num;
        if(Hp<= 0)
        {
            MonsterDie();
        }
    }

    public void ClearMovePoints()
    {
        if(movePointsQueue.Count !=0)
            movePointsQueue.Clear();
    }
}
