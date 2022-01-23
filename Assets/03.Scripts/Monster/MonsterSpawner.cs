using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterSpawner : MonoBehaviour
{
    enum UserLayer { Player = 8, Enemy};

    Stack<GameObject> monsterStack = new Stack<GameObject>();

    public GameObject monsterBase;

    public Transform[] movePoints;

    bool isAi;
    public bool IsAi
    {
        get
        {
            return isAi;
        }
    }

    int type;
    public int Type
    {
        set
        {
            type = value;
        }
    }
    int hp;
    public int HP
    {
        set
        {
            hp = value;
        }
    }

    private void Awake()
    {
        if (gameObject.layer == (int)UserLayer.Player)
        {
            isAi = false;
        }
        else if (gameObject.layer == (int)UserLayer.Enemy)
        {
            isAi = true;
        }
    }

    void CreateMonater()
    {
        GameObject go = Instantiate(monsterBase);
        go.transform.SetParent(this.transform);
        go.SetActive(false);
        monsterStack.Push(go);
    }
    
    public void PopMonster()
    {
        GameObject reqObject = null;

        //개수 부족시 추가생성
        if(monsterStack.Count <= 0)
        {
            CreateMonater();
        }
        reqObject = monsterStack.Pop();
        
        //스포너위치에서 시작하도록
        reqObject.transform.position = transform.position;
        MonsterBase monsterBase = reqObject.GetComponent<MonsterBase>();
        monsterBase.SetParentSpawner(this);
        monsterBase.EnqueueMovePoints(movePoints);
        monsterBase.Hp = hp;
        monsterBase.Type = (MonsterBase.MonsterType)type;




        reqObject.SetActive(true);
    }

    public void PushMonster(GameObject obj)
    {
        obj.SetActive(false);
        monsterStack.Push(obj);
    }

}
