using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBase : MonoBehaviour
{
    public enum DiceType
    {
        fire,electric,wind,
    }

    //주사위 등급
    int grade;

    public SpriteRenderer[] dice_Eyes;
    SpriteRenderer spr;
    public Sprite[] diceImages;

    MonsterSpawner monsterSpawner;

    bool dragOk;
    public bool DragOk
    {
        set
        {
            dragOk = value;
        }
    }

    public MonsterSpawner MonsterSpawner
    {
        set
        {
            monsterSpawner = value;
        }
    }

    float startPosX;
    float startPosY;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }
    public int Grade
    {
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
            SetDiceColor();
        }
    }
    private void OnMouseDown()
    {
#if UNITY_EDITOR
        if(Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPosX = mousePos.x - transform.position.x;
            startPosY = mousePos.y - transform.position.y;
            Debug.Log("test");
        }


#endif
#if UNITY_ANDROID
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {

        }
#endif
    }

    private void OnMouseDrag()
    {
        
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
    void SetDiceColor()
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
