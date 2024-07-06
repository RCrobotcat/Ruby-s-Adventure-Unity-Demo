using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollectable : MonoBehaviour
{
    public AudioClip collectedClip;
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            if (controller.bullet < controller.maxBullet && UIHealthBar.instance.hasTask)
            {
                controller.ChangeBullet(1);
                Destroy(gameObject);
                controller.PlaySound(collectedClip);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
