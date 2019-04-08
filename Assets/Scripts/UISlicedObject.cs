using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlicedObject : SlicedObject
{
    [SerializeField]
    Image[] images;

    public override void Slice(Sprite right, Sprite left, Vector2 originalVelocity, float torque, float sliceForce = 0.5F)
    {
        images[0].sprite = left;
        images[0].SetNativeSize();
        //images[0].transform.localScale = Vector3.one * 0.75f;
        images[1].sprite = right;
        images[1].SetNativeSize();
        //images[1].transform.localScale = Vector3.one * 0.75f;

        float leftForce = Random.Range(0.3f, sliceForce);
        float rightForce = Random.Range(.3f, sliceForce);


        bodies[0].velocity = originalVelocity + new Vector2(-leftForce, .3f);
        bodies[1].velocity = originalVelocity + new Vector2(rightForce, .3f);

        bodies[0].AddTorque(torque, ForceMode2D.Impulse);
        bodies[1].AddTorque(-torque, ForceMode2D.Impulse);

        Destroy(gameObject, 3f);
    }
}
