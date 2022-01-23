﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBase : MonoBehaviour
{
    public enum DiceType
    {
        fire,electric,wind,
    }



    public SpriteRenderer[] dice_Eyes;
    SpriteRenderer spr;
    Collider2D col;
    public Sprite[] diceImages;





    //공격가능여부, 드래그중에는 공격이 발사되면안되기때문
    bool attackOk;
    [SerializeField]
    bool mergeOk;
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
    [SerializeField]
    bool isDrag;
    Vector2 startPos;

    //드래그가능여부, 적군다이스는 드래그할수없어야되기때문
    [SerializeField]
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
            SetDiceType();
        }
    }
    //주사위 등급
    [SerializeField]
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
            foreach(var eye in dice_Eyes)
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


    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }
    private void OnEnable()
    {
        startPos = transform.position;
    }
    private void Update()
    {
        if((Vector2)transform.position != startPos && !isDrag) 
        {
            transform.position = Vector2.MoveTowards(transform.position, startPos, 0.1f);
        }

    }

    private void OnMouseDown()
    {
        SortingOrderSet(12);
        parent.CheckMergerOk(this);
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
        }
        else if(mergeOk)
        {
            int ranType = Random.Range(0, 3);

            mergeTarget.Type = (DiceType)ranType;
            mergeTarget.Grade += 1;
            transform.position = startPos;
            parent.PushDice(this.gameObject, index);
            gameObject.SetActive(false);
        }


        SortingOrderSet(10);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.tag == "Dice")
        {

            DiceBase collisionDice = collision.gameObject.GetComponent<DiceBase>();

            //타워의 타입이 같고 등급이 같으면 등급업가능
            if (collisionDice.Type == type && collisionDice.Grade == grade && isDrag &&collisionDice.DragOk)
            {
                mergeTarget = collisionDice;
                mergeOk = true;
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
        foreach (var eye in dice_Eyes)
        {
            eye.sortingOrder = layer+1;
        }
    }

    public void SetSpriteColor(Color32 color)
    {
        //주사위색 변경
        spr.color = color;
        //주사위 눈금색 변경
    }
    public void SetEyeAlpah(int alpha)
    {
        foreach (var eye in dice_Eyes)
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
                dice_Eyes[0].transform.position = transform.TransformPoint(new Vector2(0, 0));
                dice_Eyes[0].gameObject.SetActive(true);
                break;
            case 2:
                //좌측하단
                dice_Eyes[0].transform.position = transform.TransformPoint(new Vector2(-0.3f, -0.3f));
                dice_Eyes[0].gameObject.SetActive(true);
                //우측상단
                dice_Eyes[1].transform.position = transform.TransformPoint(new Vector2(0.3f, 0.3f));
                dice_Eyes[1].gameObject.SetActive(true);
                break;
            case 3:
                //가운데
                dice_Eyes[0].transform.position = transform.TransformPoint(new Vector2(0, 0));
                dice_Eyes[0].gameObject.SetActive(true);
                //좌측하단
                dice_Eyes[1].transform.position = transform.TransformPoint(new Vector2(-0.3f, -0.3f));
                dice_Eyes[1].gameObject.SetActive(true);
                //우측상단
                dice_Eyes[2].transform.position = transform.TransformPoint(new Vector2(0.3f, 0.3f));
                dice_Eyes[2].gameObject.SetActive(true);
                break;
            case 4:
                //좌측상단
                dice_Eyes[0].transform.position = transform.TransformPoint(new Vector2(-0.3f, 0.3f));
                dice_Eyes[0].gameObject.SetActive(true);
                //좌측하단
                dice_Eyes[1].transform.position = transform.TransformPoint(new Vector2(-0.3f, -0.3f));
                dice_Eyes[1].gameObject.SetActive(true);
                //우측상단
                dice_Eyes[2].transform.position = transform.TransformPoint(new Vector2(0.3f, 0.3f));
                dice_Eyes[2].gameObject.SetActive(true);
                //우측하단
                dice_Eyes[3].transform.position = transform.TransformPoint(new Vector2(0.3f, -0.3f));
                dice_Eyes[3].gameObject.SetActive(true);
                break;
            case 5:
                //가운데
                dice_Eyes[0].transform.position = transform.TransformPoint(new Vector2(0, 0));
                dice_Eyes[0].gameObject.SetActive(true);
                //좌측상단
                dice_Eyes[1].transform.position = transform.TransformPoint(new Vector2(-0.3f, 0.3f));
                dice_Eyes[1].gameObject.SetActive(true);
                //좌측하단
                dice_Eyes[2].transform.position = transform.TransformPoint(new Vector2(-0.3f, -0.3f));
                dice_Eyes[2].gameObject.SetActive(true);
                //우측상단
                dice_Eyes[3].transform.position = transform.TransformPoint(new Vector2(0.3f, 0.3f));
                dice_Eyes[3].gameObject.SetActive(true);
                //우측하단
                dice_Eyes[4].transform.position = transform.TransformPoint(new Vector2(0.3f, -0.3f));
                dice_Eyes[4].gameObject.SetActive(true);
                break;
            case 6:
                //좌측
                dice_Eyes[0].transform.position = transform.TransformPoint(new Vector2(-0.3f, 0));
                dice_Eyes[0].gameObject.SetActive(true);
                //좌측상단
                dice_Eyes[1].transform.position = transform.TransformPoint(new Vector2(-0.3f, 0.3f));
                dice_Eyes[1].gameObject.SetActive(true);
                //좌측하단
                dice_Eyes[2].transform.position = transform.TransformPoint(new Vector2(-0.3f, -0.3f));
                dice_Eyes[2].gameObject.SetActive(true);
                //우측상단
                dice_Eyes[3].transform.position = transform.TransformPoint(new Vector2(0.3f, 0.3f));
                dice_Eyes[3].gameObject.SetActive(true);
                //우측하단
                dice_Eyes[4].transform.position = transform.TransformPoint(new Vector2(0.3f, -0.3f));
                dice_Eyes[4].gameObject.SetActive(true);
                //우측
                dice_Eyes[5].transform.position = transform.TransformPoint(new Vector2(0.3f, 0));
                dice_Eyes[5].gameObject.SetActive(true);
                break;

        }
    }
    void SetDiceType()
    {
        switch(type)
        {
            case DiceType.fire:
                spr.sprite = diceImages[(int)DiceType.fire];
                foreach (var eye in dice_Eyes)
                {
                    eye.color = new Color32(255, 0, 0, 255);
                }
                
                break;
            case DiceType.electric:
                spr.sprite = diceImages[(int)DiceType.electric];
                foreach (var eye in dice_Eyes)
                {
                    eye.color = new Color32(255, 180, 0, 255);
                }
                break;
            case DiceType.wind:
                spr.sprite = diceImages[(int)DiceType.wind];
                foreach (var eye in dice_Eyes)
                {
                    eye.color = new Color32(0, 255, 212, 255);
                }
                break;
        }
    }


}
