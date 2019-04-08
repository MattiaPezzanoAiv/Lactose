using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicedObject : MonoBehaviour
{
    [SerializeField]
    protected Rigidbody2D[] bodies;

    [SerializeField]
    private SpriteRenderer[] renderers;

    public virtual void Slice(Sprite right, Sprite left, Vector2 originalVelocity, float torque, float sliceForce = 0.5f)
    {
        renderers[0].sprite = left;
        renderers[1].sprite = right;

        float leftForce = Random.Range(0.3f, sliceForce);
        float rightForce = Random.Range(0.3f, sliceForce);


        bodies[0].velocity = originalVelocity + new Vector2(-leftForce, 0.3f);
        bodies[1].velocity = originalVelocity + new Vector2(rightForce, 0.3f);

        bodies[0].AddTorque(torque, ForceMode2D.Impulse);
        bodies[1].AddTorque(-torque, ForceMode2D.Impulse);

        Destroy(gameObject, 5f);
    }

}
