using System;
using System.Collections;
using System.Collections.Generic;
using RaindowStudio.DesignPattern;
using UnityEngine;

public class EventManager : SingletonUnityEternal<EventManager>
{
    public event Action<Actor, Actor, ActType> ActorActingEvent;

    public void ActorActing(Actor source, Actor target, ActType type)
    {
        switch (type)
        {
            case ActType.Attack:

                break;

            case ActType.Buff:

                break;

            case ActType.DeBuff:

                break;
        }

        ActorActingEvent?.Invoke(source, target, type);
    }
}