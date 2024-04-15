using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//扩展方法
public static class ExtensionMethod
{
    //玩家在怪物前方时才会受到攻击减血量
    private const float dotThreshold = 0.5f;
    public static bool IsFacingTarget(this Transform transfrom, Transform target)
    {
        var vectorToTarget = target.position - transfrom.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transfrom.forward, vectorToTarget);

        return dot >= dotThreshold;
    }
}
