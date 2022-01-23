using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterBase : MonoBehaviour
{
    public enum MonsterType
    {
        Noraml,Small,Big
    }


    Queue<Vector3> movePointsQueue = new Queue<Vector3>();

    public Sprite[] MonsterImages;
    public TextMeshPro hpText;
    SpriteRenderer spr;
    MonsterSpawner parent;
    bool isDie;

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
                case MonsterType.Noraml:
                    spr.sprite = MonsterImages[(int)MonsterType.Noraml];
                    hpText.text = hp.ToString();
                    sp = 10;
                    speed = 1f;
                    break;
                case MonsterType.Big:
                    spr.sprite = MonsterImages[(int)MonsterType.Big];
                    hpText.text = hp.ToString();
                    sp = 50;
                    speed = 0.88f;
                    break;
                case MonsterType.Small:
                    spr.sprite = MonsterImages[(int)MonsterType.Small];
                    hpText.text = hp.ToString();
                    sp = 10;
                    speed = 1.5f;
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


    void MonsterDie()
    {
         parent.PushMonster(this.gameObject);
        if(!parent.IsAi)
        {
            GameManager.instance.Player.Sp += sp;
            GameManager.instance.Player.Life -= 1;

        }

        else if(parent.IsAi)
        {
            GameManager.instance.Enemy.Sp += sp;
            GameManager.instance.Enemy.Life -= 1;
        }

    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, movePoint,speed*0.005f);
        if(transform.position == movePoint)
        {
            if (movePointsQueue.Count == 0)
            {
                MonsterDie();
            }
                
            else
                movePoint = movePointsQueue.Dequeue();

        }
    }

    public void SetParentSpawner(MonsterSpawner m)
    {
        parent = m;
    }
    public void EnqueueMovePoints(Transform[] tr)
    {
        foreach(var point in tr)
        {
            movePointsQueue.Enqueue(point.position);
        }
    }
}
