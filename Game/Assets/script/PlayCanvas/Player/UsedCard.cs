using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsedCard : GameBase
{
    public Vector3 target;
    public float aliveTime = 0.5f;
    public float speed=10;
    public bool active = false;
    public float activeTime;
    public bool alive = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3 dis = target - transform.localPosition;
        if (dis.sqrMagnitude > 0.001)
        {
            float delta = Time.deltaTime;
            dis.Normalize();
            dis *= speed * delta;
            transform.localPosition += dis;
        }
        else
        {
            if (active)
            {
                if (Time.time - activeTime > aliveTime)
                {
                    Destroy(this.gameObject);
                }
            }
            else
            {
                active = true;
                activeTime = Time.time;
                speed = 0;
            }
        }*/
        if (alive)
        {
            if (active)
            {
                if (Time.time - activeTime > aliveTime)
                {
                    Vector3 dis = target - transform.localPosition;
                    if (dis.sqrMagnitude > 0.001)
                    {
                        float delta = Time.deltaTime;
                        dis *= speed * delta;
                        transform.localPosition += dis;
                    }
                    else
                    {
                        alive = false;
                    }
                }
            }
            else
            {
                active = true;
                activeTime = Time.time;
            }
        }

    }
}
