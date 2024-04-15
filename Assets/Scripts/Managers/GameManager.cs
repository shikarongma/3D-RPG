using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStates playerStats;

    //�����б�
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void RigisterPlayer(CharacterStates player)
    {
        playerStats = player;
    }

    //�����б�
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    //�Ƴ��б�
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    //�㲥
    public void NotifyObservers()
    {
        foreach(var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }
}
