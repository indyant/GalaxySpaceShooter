using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    

    // Phase I: Framework - Quiz - Ammo Collectable
    // Phase I: Framework - Quiz - Health Collectable
    // Phase I: Framework - Quiz - Secondary Fire Powerup
    // _powerupID - 0: Triple_Shot, 1: Speed_Up, 2: Shield_Up, 3: Ammo, 4: Health, 5: New
    
    public enum PowerUpType {Empty, TripleShot, SpeedUp, ShieldUp, Ammo, Health, MultiShotPowerUp, FireSpeedDebuff, MovementDebuff}
    
    [SerializeField] private PowerUpType powerUpType = PowerUpType.Empty;
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
    
    // Phase I: Framework - Quiz - Ammo Collectable
    // Phase I: Framework - Quiz - Health Collectable
    // Phase I: Framework - Quiz - Secondary Fire Powerup
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            
            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
            if (player != null)
            {
                switch (powerUpType)
                {
                    case PowerUpType.TripleShot:
                        player.EnableTripleShot();
                        break;
                    case PowerUpType.SpeedUp:
                        player.EnableSpeedup();
                        break;
                    case PowerUpType.ShieldUp:
                        player.EnableShield();
                        break;
                    case PowerUpType.Ammo:
                        player.AddAmmo();
                        break;
                    case PowerUpType.Health:
                        player.RecoverHealth();
                        break;
                    case PowerUpType.MultiShotPowerUp:
                        player.EnableMultiShot();
                        break;
                    case PowerUpType.MovementDebuff:
                        player.EnableMovementDebuff();
                        break;
                    case PowerUpType.FireSpeedDebuff:
                        player.EnableFireSpeedDebuff();
                        break;
                    default:
                        Debug.Log("Invalid _powerupID");
                        break;
                }
            }

            Destroy(gameObject);
        }
        else if (other.tag == "Enemy_Laser") 
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
    
}
