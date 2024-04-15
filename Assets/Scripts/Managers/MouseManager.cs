using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//[System.Serializable]

//public class EventVector3 : UnityEvent<Vector3> { }

public class MouseManager : Singleton<MouseManager>
{

    //单例模式
    //public static MouseManager Instance;

    public Camera mainCamera;//我的相机
    RaycastHit hitInfo;
    public event Action<Vector3> OnMouseClicked;//事件
    public event Action<GameObject> OnEnemyClicked;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(mainCamera);
    }

    //鼠标光标图片
    public Texture2D point;


    //private void Awake()
    //{
    //    if (Instance != null)
    //    {
    //        Destroy(Instance);
    //    }
    //    Instance = this;
    // }

    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }

    void SetCursorTexture()
    {
        Ray ray = new Ray();
        
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
      
        

        if (Physics.Raycast(ray, out hitInfo))
        {

            //切换鼠标贴图
            //Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);

        }
    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
                OnMouseClicked?.Invoke(hitInfo.point);
            if(hitInfo.collider.gameObject.CompareTag("Enemy"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject); 
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
                OnMouseClicked?.Invoke(hitInfo.point);

        }
    }
}
