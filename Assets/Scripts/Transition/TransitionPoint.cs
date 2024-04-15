using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType { SameScene, DifferentScene}

    [Header("Transition Info")]
    //��Ҫ���͵��ĳ���
    public string sceneName;
    //ָ������
    public TransitionType transitionType;
    //ָ���յ�
    public TransitionDestination.DestinationTag destinationTag;

    private bool canTrans;

     void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canTrans)
        {
            //SceneController ����
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
