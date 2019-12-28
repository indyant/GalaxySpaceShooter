using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    
    // _powerupID - 0: Triple_Shot, 1: Speed_Up, 2: Shield_Up
    [SerializeField] private int _powerupID = 0;
    [SerializeField] private AudioClip _audioClip;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -6.5f)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            
            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.EnableTripleShot();
                        break;
                    case 1:
                        player.EnableSpeedup();
                        break;
                    case 2:
                        player.EnableShield();
                        break;
                    default:
                        Debug.Log("Invalid _powerupID");
                        break;
                }
            }

            Destroy(gameObject);
        }
    }
    
}
