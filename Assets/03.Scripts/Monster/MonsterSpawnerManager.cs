using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnerManager : MonoBehaviour
{

    float spawnTime;
    float spawnDelay = 3f;

    MonsterSpawner[] spawners;

    public static MonsterSpawnerManager instance;

    //보스몹생성시 일반몹생성 막기위함
    public bool spawnOk;

    void Awake()
    {
        //매니저이므로 오로지 한개체만 존재하도록
        if (instance == null)
            instance = this;
        else if(instance != null)
        {
            Destroy(this.gameObject);
        }

        //태그를이용해 몬스터 스포너 오브젝트 찾음
        GameObject[] spawnObj = GameObject.FindGameObjectsWithTag("MonsterSpawner");

        //해당 오브젝트 개수만큼 배열할당
        spawners = new MonsterSpawner[spawnObj.Length];

        //각각의 스포너 정보를 받아옴
        for(int i=0; i<spawnObj.Length ; ++i)
        {
            spawners[i] = spawnObj[i].GetComponent<MonsterSpawner>();
        }

    }

    void Update()
    {
        if(spawnOk)
        {
            spawnTime += Time.deltaTime;

            if (spawnTime > spawnDelay)
            {
                int randomType = Random.Range(0, (int)MonsterBase.MonsterType.COUNT);
                //최초 몬스터 생성이후부터 타이머시작
                GameManager.instance.TimerStart = true;
                foreach (MonsterSpawner spawner in spawners)
                {
                    spawner.Type = randomType;
                    spawner.HP = GameManager.instance.DefaultHp * GameManager.instance.HpMul;
                    spawner.PopMonster();
                }
                spawnTime = 0f;
            }
        }

    }

}
