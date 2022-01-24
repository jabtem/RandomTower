using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceEye : MonoBehaviour
{
    public GameObject bulletPrefab;
    Stack<GameObject> bulletStack = new Stack<GameObject>();

    //탄환데미지
    int damage;
    int index;
    public int Damage
    {
        get
        {
            return damage;
        }
        set
        {
            damage = value;
        }
    }

    SpriteRenderer spr;

    DiceBase parent;

    private void Awake()
    {
        parent = transform.parent.gameObject.GetComponent<DiceBase>();
        spr = GetComponent<SpriteRenderer>();
    }

    void CreateBullet()
    {
        GameObject go = Instantiate(bulletPrefab);
        go.name = $"Bullet {index++}";
        go.transform.SetParent(this.transform);
        go.SetActive(false);
        bulletStack.Push(go);
    }

    public void PopBullet()
    {
        GameObject reqObject = null;
        if(bulletStack.Count <= 0)
        {
            CreateBullet();
        }
        reqObject = bulletStack.Pop();
        BulletBase bullet = reqObject.GetComponent<BulletBase>();
        SpriteRenderer bulletColor = reqObject.GetComponent<SpriteRenderer>();
        bulletColor.color = spr.color;
        if(parent.Type != DiceBase.DiceType.ENERGE)
            bullet.Damage = parent.AttackPower;
        else if(parent.Type == DiceBase.DiceType.ENERGE)
        {
            bullet.Damage =  (int)(parent.AttackPower + GameManager.instance.Player.Sp * (0.04f + 0.003f * (parent.Grade - 1)));
        }
        reqObject.transform.position = transform.position;

        bullet.Target = parent.Target;
        bullet.Parent = this;
        reqObject.SetActive(true);

    }

    public void PushBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletStack.Push(bullet);
    }
}
