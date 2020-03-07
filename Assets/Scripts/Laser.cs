using UnityEngine;

public class Laser : MonoBehaviour
{
    private readonly float _boundary = 8.0f;
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private bool _isEnemyLaser = false;
    
    [SerializeField] private bool _isDirectionalLaser = false;
    [SerializeField] private Vector3 _direction;
    
    private Player _player;
    
    // Start is called before the first frame update
    private void Start()
    {
//        transform.position = new Vector3();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isEnemyLaser)
        {
            MoveDown();
        }
        else if (_isDirectionalLaser)
        {
            MoveDirection(_direction);
        }
        else
        {
            MoveUp();
        }
    }

    private void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if (transform.position.y >= _boundary)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
    }

    private void MoveDown()
    {
        transform.Translate(Vector3.up * _speed * -1.0f * Time.deltaTime);
        if (transform.position.y <= (_boundary * -1.0f))
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
    }

    // Phase I: Framework - Quiz - Secondary Fire Powerup
    private void MoveDirection(Vector3 direction)
    {
        transform.Translate(direction * _speed * Time.deltaTime);
        if (transform.position.y >= _boundary)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        } 
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyLaser)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            Destroy(gameObject, 2.8f);
        }
    }  
}