using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Phase I: Framework - Quiz - Camera Shake
public class CameraShake : MonoBehaviour
{
    private Vector3 _originPos;
    
    // Start is called before the first frame update
    void Start()
    {
        _originPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Shake(float amount, float duration)
    {
        float timer = 0;

        while (timer <= duration)
        {
            transform.localPosition = (Vector3) Random.insideUnitCircle * amount + _originPos;
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = _originPos;
    }
}
