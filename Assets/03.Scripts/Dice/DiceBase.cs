using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBase : MonoBehaviour
{
    public enum DiceType
    {
        FIRE,ELECTRIC,WIND,MIMIC,MINE,IRON,BROKEN,SACRIFICE,ENERGE,COUNT
    }
    public enum TargetPrority
    {
        FRONT,DBURFF,HIGHHP,RANDOM,NOATTACK,COUNT
    }


    public SpriteRenderer[] diceEyeSpr;
    DiceEye[] diceEyes;
    SpriteRenderer spr;
    Collider2D col;
    public Sprite[] diceImages;

    GameObject target;
    public GameObject Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
        }
    }




    //공격가능여부, 드래그중에는 공격이 발사되면안되기때문
    bool attackOk;
    bool mergeOk;


    //공격력과 공격속도
    float attackSpeed;
    int attackPower;
    public int AttackPower
    {
        get
        {
            return attackPower;
        }
    }

    //등급업시킬 대상다이스
    DiceBase mergeTarget;

    //이다이스를 생성시킨 부모 생성기
    DiceCreater parent;
    public DiceCreater Parent
    {
        set
        {
            parent = value;
        }
    }
    bool isDrag;
    Vector2 startPos;

    //드래그가능여부, 적군다이스는 드래그할수없어야되기때문
    bool dragOk;
    public bool DragOk
    {
        get
        {
            return dragOk;
        }
        set
        {
            dragOk = value;
        }
    }
    //어느쪽 스포너에서 생성된 몬스터인지 파악(사거리가 무제한이므로 해당몬스터만때려야하기때문)
    MonsterSpawner monsterSpawner;
    public MonsterSpawner MonsterSpawner
    {
        set
        {
            monsterSpawner = value;
        }
    }
    DiceType type;
    public DiceType Type
    {
        get
        {
            return type;
        }
        set
        {
            type = value;
            StopAllCoroutines();
            SetDiceType();
        }
    }

    TargetPrority priority;
    //주사위 등급
    int grade;
    public int Grade
    {
        get
        {
            return grade;
        }

        set
        {
            grade = value;

            //등급변경시 기존활성화된 눈 전부 비활성화
            foreach(var eye in diceEyeSpr)
            {
                if(eye.gameObject.activeSelf)
                    eye.gameObject.SetActive(false);
            }
            SetDiceEyePositon(grade);
        }
    }

    int index;
    public int Index
    {
        set
        {
            index = value;
        }
    }
    float mimicColor;

    float attacDelay;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        diceEyes = gameObject.GetComponentsInChildren<DiceEye>(true);
    }
    private void OnEnable()
    {
        startPos = transform.position;
        
        if(Type == DiceType.MINE)
        {
            StartCoroutine(MiningStart());
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    IEnumerator MiningStart()
    {

        while(true)
        {
            yield return new WaitForSeconds(10f - (grade-1)*0.5f);
            if (!parent.isAI)
                GameManager.instance.Player.Sp += 3;
            else if (parent.isAI)
                GameManager.instance.Enemy.Sp += 3;
        }

    }

    private void Update()
    {
        if((Vector2)transform.position != startPos && !isDrag) 
        {
            transform.position = Vector2.MoveTowards(transform.position, startPos, 0.1f);
        }

        if(Type == DiceType.MIMIC)
        {

            foreach (var eye in diceEyeSpr)
            {
                eye.color = Color.HSVToRGB(mimicColor/360, 1f, 1f);
            }
            mimicColor += 4f;

            if(mimicColor>=360)
            {
                mimicColor = 0;
            }
        }

        if(priority != TargetPrority.NOATTACK)
        {

            if(attackOk)
            {
                attacDelay += Time.deltaTime;

                if(attacDelay > attackSpeed)
                {
                    TargetSet();
                    if(target != null)
                        Attack();
                    attacDelay = 0;
                }

            }

        }

    }

    void Attack()
    {
        foreach(var eye in diceEyes)
        {
            if(eye.gameObject.activeSelf)
                eye.PopBullet();
        }
    }

    void TargetSet()
    {
        float max = 0f;
        GameObject tempTarget = null;
        //현재 활성화되어있는 모든몬스터를 탐색해서 공격우선순위에따라 대상지정
        if(monsterSpawner.ActiveMonsterSet.Count!=0)
        {
            foreach (var monster in monsterSpawner.ActiveMonsterSet)
            {
                if (priority == TargetPrority.FRONT)
                {
                    if (monster.TotalMoveDistance > max)
                    {
                        max = monster.TotalMoveDistance;
                        tempTarget = monster.gameObject;
                    }
                }
                else if(priority == TargetPrority.HIGHHP)
                {
                    if(monster.Hp > max)
                    {
                        max = monster.Hp;
                        tempTarget = monster.gameObject;
                    }
                }
                else if(priority == TargetPrority.RANDOM)
                {
                    int ranNum = Random.Range(0, 10);

                    if(ranNum > max)
                    {
                        max = ranNum;
                        tempTarget = monster.gameObject;
                    }
                }

            }
        }


        Target = tempTarget;
    }

    private void OnMouseDown()
    {
        SortingOrderSet(2000);
        parent.CheckMergerOk(this);
        attackOk = false;
    }

    private void OnMouseDrag()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            if (!isDrag)
                isDrag = true;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector2(mousePos.x, mousePos.y);
        }


#endif
#if (UNITY_ANDROID || UNITY_IPHONE) 

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                if (!isDrag)
                    isDrag = true;

                Vector2 tochPos = Camera.main.ScreenToWorldPoint(touch.position);
                transform.position = new Vector2(tochPos.x, tochPos.y);
            }
        }

#endif
    }

    private void OnMouseUp()
    {
        parent.RecoveryDiceInfo();

        if (!mergeOk)
        {
            isDrag = false;
            if(priority != TargetPrority.NOATTACK)
            {
                attackOk = true;
            }
        }
        else if(mergeOk)
        {
            if(type == DiceType.SACRIFICE || (type == DiceType.MIMIC && mergeTarget.Type ==DiceType.SACRIFICE))
            {
                if (!parent.isAI)
                    GameManager.instance.Player.Sp += 80;
                else if(parent.isAI)
                {
                    GameManager.instance.Enemy.Sp += 80;
                }
            }

            int ranType = Random.Range(0, (int)DiceType.COUNT);
            mergeTarget.Grade += 1;
            mergeTarget.Type = (DiceType)ranType;
            transform.position = startPos;
            parent.PushDice(this.gameObject, index);
            gameObject.SetActive(false);
        }


        SortingOrderSet(10);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {


        if (collision.tag == "Dice")
        {

            DiceBase collisionDice = collision.gameObject.GetComponent<DiceBase>();

            if(Type != DiceType.MIMIC)
            {
                //타워의 타입이 같고 등급이 같거나 다이스타입이 미믹인경우
                if ((collisionDice.Type == type || collisionDice.Type == DiceType.MIMIC) && collisionDice.Grade == grade && isDrag && collisionDice.DragOk)
                {
                    mergeTarget = collisionDice;
                    mergeOk = true;
                }
            }
            else if(Type == DiceType.MIMIC)
            {
                if (collisionDice.Grade == grade && isDrag && collisionDice.DragOk)
                {
                    mergeTarget = collisionDice;
                    mergeOk = true;
                }
            }

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Dice")
        {
            mergeTarget = null;
            mergeOk = false;
        }
    }

    void SortingOrderSet(int layer)
    {
        spr.sortingOrder = layer;
        foreach (var eye in diceEyeSpr)
        {
            eye.sortingOrder = layer+1;
        }
    }
    //주사위색 변경
    public void SetSpriteColor(Color32 color)
    {

        spr.color = color;
    }
    //주사위 눈금 투명도 변경
    public void SetEyeAlpah(int alpha)
    {
        foreach (var eye in diceEyeSpr)
        {
            Color32 eyeColor32 = eye.color;
            eyeColor32.a = (byte)alpha;
            eye.color = eyeColor32;
        }
    }
    public void SetColliderActve(bool value)
    {
        col.enabled = value;
    }



    void SetDiceEyePositon(int grade)
    {
        switch (grade)
        {
            case 1:
                //가운데
                diceEyeSpr[0].transform.position = transform.TransformPoint(new Vector2(0, 0));
                diceEyeSpr[0].gameObject.SetActive(true);
                break;
            case 2:
                //좌측하단
                diceEyeSpr[0].transform.position = transform.TransformPoint(new Vector2(-0.3f, -0.3f));
                diceEyeSpr[0].gameObject.SetActive(true);
                //우측상단
                diceEyeSpr[1].transform.position = transform.TransformPoint(new Vector2(0.3f, 0.3f));
                diceEyeSpr[1].gameObject.SetActive(true);
                break;
            case 3:
                //가운데
                diceEyeSpr[0].transform.position = transform.TransformPoint(new Vector2(0, 0));
                diceEyeSpr[0].gameObject.SetActive(true);
                //좌측하단
                diceEyeSpr[1].transform.position = transform.TransformPoint(new Vector2(-0.3f, -0.3f));
                diceEyeSpr[1].gameObject.SetActive(true);
                //우측상단
                diceEyeSpr[2].transform.position = transform.TransformPoint(new Vector2(0.3f, 0.3f));
                diceEyeSpr[2].gameObject.SetActive(true);
                break;
            case 4:
                //좌측상단
                diceEyeSpr[0].transform.position = transform.TransformPoint(new Vector2(-0.3f, 0.3f));
                diceEyeSpr[0].gameObject.SetActive(true);
                //좌측하단
                diceEyeSpr[1].transform.position = transform.TransformPoint(new Vector2(-0.3f, -0.3f));
                diceEyeSpr[1].gameObject.SetActive(true);
                //우측상단
                diceEyeSpr[2].transform.position = transform.TransformPoint(new Vector2(0.3f, 0.3f));
                diceEyeSpr[2].gameObject.SetActive(true);
                //우측하단
                diceEyeSpr[3].transform.position = transform.TransformPoint(new Vector2(0.3f, -0.3f));
                diceEyeSpr[3].gameObject.SetActive(true);
                break;
            case 5:
                //가운데
                diceEyeSpr[0].transform.position = transform.TransformPoint(new Vector2(0, 0));
                diceEyeSpr[0].gameObject.SetActive(true);
                //좌측상단
                diceEyeSpr[1].transform.position = transform.TransformPoint(new Vector2(-0.3f, 0.3f));
                diceEyeSpr[1].gameObject.SetActive(true);
                //좌측하단
                diceEyeSpr[2].transform.position = transform.TransformPoint(new Vector2(-0.3f, -0.3f));
                diceEyeSpr[2].gameObject.SetActive(true);
                //우측상단
                diceEyeSpr[3].transform.position = transform.TransformPoint(new Vector2(0.3f, 0.3f));
                diceEyeSpr[3].gameObject.SetActive(true);
                //우측하단
                diceEyeSpr[4].transform.position = transform.TransformPoint(new Vector2(0.3f, -0.3f));
                diceEyeSpr[4].gameObject.SetActive(true);
                break;
            case 6:
                //좌측
                diceEyeSpr[0].transform.position = transform.TransformPoint(new Vector2(-0.3f, 0));
                diceEyeSpr[0].gameObject.SetActive(true);
                //좌측상단
                diceEyeSpr[1].transform.position = transform.TransformPoint(new Vector2(-0.3f, 0.3f));
                diceEyeSpr[1].gameObject.SetActive(true);
                //좌측하단
                diceEyeSpr[2].transform.position = transform.TransformPoint(new Vector2(-0.3f, -0.3f));
                diceEyeSpr[2].gameObject.SetActive(true);
                //우측상단
                diceEyeSpr[3].transform.position = transform.TransformPoint(new Vector2(0.3f, 0.3f));
                diceEyeSpr[3].gameObject.SetActive(true);
                //우측하단
                diceEyeSpr[4].transform.position = transform.TransformPoint(new Vector2(0.3f, -0.3f));
                diceEyeSpr[4].gameObject.SetActive(true);
                //우측
                diceEyeSpr[5].transform.position = transform.TransformPoint(new Vector2(0.3f, 0));
                diceEyeSpr[5].gameObject.SetActive(true);
                break;

        }
    }
    void SetDiceType()
    {
        switch(type)
        {
            case DiceType.FIRE:
                spr.sprite = diceImages[(int)DiceType.FIRE];
                priority = TargetPrority.FRONT;
                attackSpeed = 0.8f - 0.01f * (grade - 1);
                attackPower = 20 + 3*(grade-1);
                attackOk = true;
                foreach (var eye in diceEyeSpr)
                {
                    eye.color = new Color32(255, 0, 0, 255);
                }
                
                break;
            case DiceType.ELECTRIC:
                spr.sprite = diceImages[(int)DiceType.ELECTRIC];
                priority = TargetPrority.FRONT;
                attackSpeed = 0.7f -0.02f*(grade-1);
                attackPower = 30 + 3*(grade-1);
                attackOk = true;
                foreach (var eye in diceEyeSpr)
                {
                    eye.color = new Color32(255, 180, 0, 255);
                }
                break;
            case DiceType.WIND:
                spr.sprite = diceImages[(int)DiceType.WIND];
                priority = TargetPrority.FRONT;
                attackSpeed = 0.45f * (1+0.2f*(grade-1f));
                attackPower = 20 + 3*(grade - 1);
                attackOk = true;
                foreach (var eye in diceEyeSpr)
                {
                    eye.color = new Color32(0, 255, 212, 255);
                }
                break;
            case DiceType.MIMIC:
                spr.sprite = diceImages[(int)DiceType.MIMIC];
                priority = TargetPrority.NOATTACK;
                mimicColor = 0f;
                break;
            case DiceType.MINE:
                spr.sprite = diceImages[(int)DiceType.MINE];
                priority = TargetPrority.NOATTACK;
                foreach (var eye in diceEyeSpr)
                {
                    eye.color = new Color32(0, 255, 247, 255);
                }
                break;
            case DiceType.IRON:
                spr.sprite = diceImages[(int)DiceType.IRON];
                priority = TargetPrority.HIGHHP;
                attackPower = 100 + 10 * (grade - 1);
                attackSpeed = 1f;
                attackOk = true;
                foreach (var eye in diceEyeSpr)
                {
                    eye.color = new Color32(123, 123, 123, 255);
                }
                break;
            case DiceType.BROKEN:
                spr.sprite = diceImages[(int)DiceType.BROKEN];
                priority = TargetPrority.RANDOM;
                attackPower = 50 + 10 * (grade - 1);
                attackSpeed = 0.9f;
                attackOk = true;
                foreach (var eye in diceEyeSpr)
                {
                    eye.color = new Color32(194, 64, 255, 255);
                }
                break;
            case DiceType.SACRIFICE:
                spr.sprite = diceImages[(int)DiceType.SACRIFICE];
                priority = TargetPrority.FRONT;
                attackPower = 80 + 10 * (grade - 1);
                attackSpeed = 1f;
                attackOk = true;
                foreach (var eye in diceEyeSpr)
                {
                    eye.color = new Color32(23, 0, 217, 255);
                }
                break;
            case DiceType.ENERGE:
                spr.sprite = diceImages[(int)DiceType.ENERGE];
                priority = TargetPrority.FRONT;
                attackPower = 20 + 10 * (grade - 1);
                attackSpeed = 1.3f;
                attackOk = true;
                foreach (var eye in diceEyeSpr)
                {
                    eye.color = new Color32(0, 217, 190, 255);
                }
                break;
        }
    }


    

}
