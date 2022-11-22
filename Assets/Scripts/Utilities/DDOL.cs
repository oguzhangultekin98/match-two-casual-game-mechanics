using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOL : MonoBehaviour
{
    private void Awake()
    {
        if (transform != transform.root)
            transform.parent = null;

        DontDestroyOnLoad(gameObject);
    }
}
