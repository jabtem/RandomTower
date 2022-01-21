using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnerManager : MonoBehaviour
{
    float spawnTime;
    float spawnDelay = 2f;
    int hpMul;
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
    //웨이브에따른 HP기본계수받기위함
    int defaultHP;
    MonsterSpawner[] spawners;

    static MonsterSpawnerManager instance;

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

        WaveCount = 1;
    }

    void Update()
    {
        if(spawnOk)
        {
            spawnTime += Time.deltaTime;

            if (spawnTime > spawnDelay)
            {
                int randomType = Random.Range(0, 3);
                foreach (MonsterSpawner spawner in spawners)
                {
                    spawner.Type = randomType;
                    spawner.HP = defaultHP * hpMul;
                    spawner.PopMonster();
                }
                spawnTime = 0f;
            }
        }

    }

}
