using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject Player;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CameraMove();
    }

    void CameraMove()
    {
        Vector3 cameraVector = Player.transform.position;
        Vector3 s = new Vector3();
        s.x = cameraVector.x -0.6914886f;
        s.y = cameraVector.y + 4.631566f;
        s.z = cameraVector.z  -8.585955f;
        transform.position = s;
    }
}
