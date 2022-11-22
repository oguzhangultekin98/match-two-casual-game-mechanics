using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delayer : MonoBehaviour
{
    private static Delayer instance;

    public static Delayer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Delayer>();
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Delay(Action action, float delay)
    {
        StartCoroutine(DelayCoroutine(action, delay));
    }

    private IEnumerator DelayCoroutine(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    public IEnumerator GetEnumerator(Action action, YieldInstruction delay)
    {
        yield return delay;
        action();
    }

    public IEnumerator GetEnumerator(Action action, IEnumerator delay)
    {
        yield return null;
        action();
        yield return StartCoroutine(delay);
    }

    public void Delay(Action action, YieldInstruction delay)
    {
        StartCoroutine(DelayCoroutine(action, delay));
    }

    private IEnumerator DelayCoroutine(Action action, YieldInstruction delay)
    {
        yield return delay;
        action();
    }

    public Coroutine Delay(List<IEnumerator> funcs)
    {
        return StartCoroutine(DelayCoroutine(funcs));
    }

    private IEnumerator DelayCoroutine(List<IEnumerator> enumerators)
    {
        for (int i = 0; i < enumerators.Count; i++)
            yield return StartCoroutine(enumerators[i]);
    }
}