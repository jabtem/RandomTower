using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceCreater : MonoBehaviour
{


    //생성좌표 저장을위한 15개의 벡터2배열
    //Vector2[,] DicePoints;
    Vector2[] dicePoints;
    public GameObject diceBase;
    Button createButton;
    Stack<GameObject> diceStack = new Stack<GameObject>();

    //현재 보드에 생성되어있는 다이스들의 정보 보관및 중복인덱스 제거용
    Dictionary<int, DiceBase> DiceInfoDic = new Dictionary<int, DiceBase>();

    //생성되는 몬스터가 어느쪽 스포너에서 생성됬는지 판별용
    public MonsterSpawner spawner;

    //플레이어쪽 생성기인지 AI쪽 생성기인지 판별용
    public bool isAI;
    

    void Awake()
    {
        if(!isAI)
        {
            //플레이어용일때만 버튼에대한 정보를가진다
            createButton = GameObject.FindGameObjectWithTag("DiceCreateButton").GetComponent<Button>();
            //버튼이벤트 동적할당
            createButton.onClick.AddListener(() => { PopDice(); });
            //createButton.onClick.AddListener(delegate { PopDice(); });
        }

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
    public void PopDice()
    {
        //보유 SP가 코스트보다 많거나 같을때만 
        if(GameManager.instance.Player.Sp < GameManager.instance.Player.Cost)
        {
            return;
        }

        GameManager.instance.Player.Sp -= GameManager.instance.Player.Cost;
        GameManager.instance.Player.Cost += 10;

        int index;
        int randomType;
        GameObject reqObject = null;




        if (diceStack.Count <= 0)
        {
            CreateDiceBase();
        }
        reqObject = diceStack.Pop();
        DiceBase diceBase = reqObject.GetComponent<DiceBase>();

        index = GetRandomIndex(diceBase);
        randomType = Random.Range(0, (int)DiceBase.DiceType.COUNT);


        //해당베이스 재활용목적
        diceBase.Index = index;
        diceBase.Type = (DiceBase.DiceType)randomType;
        diceBase.Grade = 1;
        //각타워가 해당스포너에서 생성된 몬스터의 정보를 가지고있게하기위함 => 타겟선정목표
        diceBase.MonsterSpawner = spawner;
        //부모 정보를 넘김, 어느 다이스 생성기에서 만들어졌는지 알기위함, 타워 삭제또는 승급시 스택에 되돌려야하기때문
        diceBase.Parent = this;
        diceBase.DragOk = true;
        
        reqObject.transform.position = transform.TransformPoint(dicePoints[index]);
        reqObject.SetActive(true);

        //이미 모든자리가 다사용됬다면 작동안하며 버튼 비활성화
        if (DiceInfoDic.Count == dicePoints.Length)
        {
            if (createButton.interactable)
                createButton.interactable = false;
        }

    }

    //현재 드래그한 다이스와 병합가능한 다이스들 강조목적
    public void CheckMergerOk(DiceBase dice)
    {

        if(dice.Type != DiceBase.DiceType.MIMIC)
        {
            foreach (var diceinfo in DiceInfoDic)
            {
                if ((diceinfo.Value.Type != dice.Type && diceinfo.Value.Type != DiceBase.DiceType.MIMIC) || diceinfo.Value.Grade != dice.Grade)
                {
                    diceinfo.Value.SetColliderActve(false);
                    diceinfo.Value.SetSpriteColor(new Color32(125, 125, 125, 255));
                    diceinfo.Value.SetEyeAlpah(50);
                }
            }
        }
        else if(dice.Type == DiceBase.DiceType.MIMIC)
        {
            foreach (var diceinfo in DiceInfoDic)
            {
                if (diceinfo.Value.Grade != dice.Grade)
                {
                    diceinfo.Value.SetColliderActve(false);
                    diceinfo.Value.SetSpriteColor(new Color32(125, 125, 125, 255));
                    diceinfo.Value.SetEyeAlpah(50);
                }
            }
        }


    }

    //다이스 색상 원상복구
    public void RecoveryDiceInfo()
    {
        foreach (var diceinfo in DiceInfoDic)
        {
            diceinfo.Value.SetColliderActve(true);
            diceinfo.Value.SetSpriteColor(new Color32(255, 255, 255, 255));
            diceinfo.Value.SetEyeAlpah(255);
        }
    }


    public void PushDice(GameObject dice, int num)
    {
        //사용가능한다이스베이스 스택에 추가
        diceStack.Push(dice);
        //자리가 미사용된걸로 처리
        DiceInfoDic.Remove(num);
        if(createButton !=null && !createButton.interactable)
        {
            createButton.interactable = true;
        }
    }

    void CreateDiceBase()
    {
        GameObject go = Instantiate(diceBase);
        go.transform.SetParent(this.transform);
        go.SetActive(false);
        diceStack.Push(go);
    }

    int GetRandomIndex(DiceBase dice)
    { 

        int ranNum = Random.Range(0, 15);
       
        //해당 인덱스가 이미 사용됬으면 랜덤인덱스 다시구함
        if (DiceInfoDic.ContainsKey(ranNum))
        {
            ranNum = GetRandomIndex(dice);
        }
        else
        {
            DiceInfoDic.Add(ranNum, dice);
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
