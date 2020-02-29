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
    
    enum PowerUp {Empty, TripleShot, SpeedUp, ShieldUp, Ammo, Health, NewPowerUp, FireSpeedDebuff, MovementDebuff}
    
    [FormerlySerializedAs("_powerupID")] [SerializeField] private PowerUp _powerUp = PowerUp.Empty;
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
                switch (_powerUp)
                {
                    case PowerUp.TripleShot:
                        player.EnableTripleShot();
                        break;
                    case PowerUp.SpeedUp:
                        player.EnableSpeedup();
                        break;
                    case PowerUp.ShieldUp:
                        player.EnableShield();
                        break;
                    case PowerUp.Ammo:
                        player.AddAmmo();
                        break;
                    case PowerUp.Health:
                        player.RecoverHealth();
                        break;
                    case PowerUp.NewPowerUp:
                        player.EnableMultiShot();
                        break;
                    case PowerUp.MovementDebuff:
                        player.EnableMovementDebuff();
                        break;
                    case PowerUp.FireSpeedDebuff:
                        player.EnableFireSpeedDebuff();
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
