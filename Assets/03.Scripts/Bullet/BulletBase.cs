using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    [SerializeField]
    GameObject target;

    public GameObject Target
    {
        set
        {
            target = value;
        }
    }

    DiceEye parent;
    public DiceEye Parent
    {
        set
        {
            parent = value;
        }
    }

    int damage;
    public int Damage
    {
        set
        {
            damage = value;
        }
    }

    MonsterBase targetMonster;

    DiceBase.DiceType type;
    //입력받은 타입에따라 총알의 효과변경목적
    public DiceBase.DiceType Type
    {
        set
        {
            type = value;
        }
    }

    private void OnEnable()
    {
        if(target !=null)
            targetMonster = target.GetComponent<MonsterBase>();
    }

    //타겟이 존재하면 타겟방향으로 날아감
    private void Update()
    {


        if(target !=null && !targetMonster.IsDie)
        {
            if ((transform.position - target.transform.position).sqrMagnitude <=0.01)
            {
                targetMonster.BulletHit(damage);
                parent.PushBullet(this.gameObject);
            }

            else
            {
                transform.position = Vector2.MoveTowards(transform.position, target.transform.position, 0.1f);
            }
        }
        else
            parent.PushBullet(this.gameObject);

    }

}
