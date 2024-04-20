using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 direction;

    public float speed;

    public Action destroyed;

    private void Update()
    {
        this.transform.position += this.speed * Time.deltaTime * this.direction;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(this.destroyed != null) {
            this.destroyed.Invoke();
        }
        
        Destroy(this.gameObject);
    }
}
