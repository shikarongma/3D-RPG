using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType { SameScene, DifferentScene}

    [Header("Transition Info")]
    //需要传送到的场景
    public string sceneName;
    //指定场景
    public TransitionType transitionType;
    //指定终点
    public TransitionDestination.DestinationTag destinationTag;

    private bool canTrans;

     void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canTrans)
        {
            //SceneController 传送
            SceneController.Instance.TransitionToDestination(this);
        }
    }

     void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = true;
    }
     void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = false;
    }
}
