using UnityEngine;

public class RotateCam : MonoBehaviour
{
    public float speed;
    private Transform _rotator;
    // Start is called before the first frame update
    void Start()
    {
        speed = 10f;
        _rotator = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        _rotator.Rotate(0, speed * Time.deltaTime, 0) ;
    }
}
