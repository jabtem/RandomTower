using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterSpawner : MonoBehaviour
{
    Stack<GameObject> monsterStack = new Stack<GameObject>();

    public GameObject monsterBase;

    public Transform[] movePoints;

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
