using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
             Rigidbody2D rb = GetComponent<Rigidbody2D>(); 
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;

        }
    }
    // Update is called once per frame
    void Update()
    {
    
    }
}
