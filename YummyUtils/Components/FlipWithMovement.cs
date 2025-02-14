using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yummy{
public class FlipWithMovement : MonoBehaviour
{
    private Vector3 lastPos;
    void Start()
    {
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var scalex = Mathf.Abs(transform.localScale.x);
        var moveOffset = transform.position.x - lastPos.x;
        if(moveOffset > 0)
        {
            transform.localScale = new Vector3(scalex ,transform.localScale.y,transform.localScale.z);
        }
        else if (moveOffset < 0) 
        {
            transform.localScale = new Vector3(-scalex,transform.localScale.y,transform.localScale.z);
        }
        lastPos = transform.position;
    }
}
}