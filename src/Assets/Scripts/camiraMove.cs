using UnityEngine;

public class camiraMove : MonoBehaviour
{
    public Transform camiraLuga;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = camiraLuga.position;
    }
}
