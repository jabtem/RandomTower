using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceCreater : MonoBehaviour
{


    //생성좌표 저장을위한 15개의 벡터2배열
    //Vector2[,] DicePoints;
    Vector2[] dicePoints;
    //타워생성에 사용된 좌표의 인덱스 저장용
    HashSet<int> useIndex = new HashSet<int>();
    public GameObject diceBase;
    Button createButton;
    Stack<GameObject> diceStack = new Stack<GameObject>();

    //생성되는 몬스터가 어느쪽 스포너에서 생성됬는지 판별용
    public MonsterSpawner spawner;
    

    void Awake()
    {
        createButton = GameObject.FindGameObjectWithTag("DiceCreateButton").GetComponent<Button>();
        dicePoints = new Vector2[15];
        dicePoints[0] = new Vector2(-1.5f, -0.6f);

        //2차원배열에서 1차원배열로 변경 -> 인덱스를 키값으로 사용하기위함
        //for (int i = 0; i < DicePoints.GetLength(0); ++i)
        //{

        //    for (int j = 0; j < DicePoints.GetLength(1); ++j)
        //    {
        //        if (i == 0 && j == 0)
        //            continue;

        //        DicePoints[i, j] = new Vector2(DicePoints[0, 0].x + (0.75f * j), DicePoints[0, 0].y + (0.7f * i));
        //    }

        //}

        for(int i=1; i<dicePoints.Length;++i)
        {
            if (i * 0.2 < 1)
            {
                dicePoints[i] = new Vector2(dicePoints[0].x + (0.75f * i), dicePoints[0].y);
            }
            else if (i * 0.2 < 2 && i * 0.2 >= 1)
            {
                int index = i % 5;
                dicePoints[i] = new Vector2(dicePoints[0].x + (0.75f * index), dicePoints[0].y + 0.7f);
            }
            else if (i * 0.2 < 3 && i * 0.2 >= 2)
            {
                int index = i % 10;
                dicePoints[i] = new Vector2(dicePoints[0].x + (0.75f * index), dicePoints[0].y + 1.4f);
            }
        }

    }

    //플레이어 타워생성
    public void GetDice()
    {
        int index;
        int randomType;
        GameObject reqObject = null;




        if (diceStack.Count <= 0)
        {
            CreateDiceBase();
        }

        index = GetRandomIndex();
        randomType = Random.Range(0, 3);

        reqObject = diceStack.Pop();
        DiceBase diceBase = reqObject.GetComponent<DiceBase>();
        diceBase.Type = (DiceBase.DiceType)randomType;
        diceBase.Grade = 1;
        //각타워가 해당스포너에서 생성된 몬스터의 정보를 가지고있게하기위함 => 타겟선정목표
        diceBase.MonsterSpawner = spawner;
        diceBase.DragOk = true;
        
        reqObject.transform.position = transform.TransformPoint(dicePoints[index]);
        reqObject.SetActive(true);

        //이미 모든자리가 다사용됬다면 작동안하며 버튼 비활성화
        if (useIndex.Count == dicePoints.Length)
        {
            if (createButton.interactable)
                createButton.interactable = false;
        }

    }

    void CreateDiceBase()
    {
        GameObject go = Instantiate(diceBase);
        go.transform.SetParent(this.transform);
        go.SetActive(false);
        diceStack.Push(go);
    }

    int GetRandomIndex()
    { 

        int ranNum = Random.Range(0, 15);
       
        //해당 인덱스가 이미 사용됬으면 랜덤인덱스 다시구함
        if (!useIndex.Add(ranNum))
        {
            ranNum = GetRandomIndex();
        }



        return ranNum;
    }

    private void OnDrawGizmosSelected()
    {
        //좌표 확인용 
        if(dicePoints !=null)
        {
            
            foreach(var point in dicePoints)
            {
                Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
                Gizmos.DrawSphere(transform.TransformPoint(point), 0.1f);
            }
        }


    }
}
