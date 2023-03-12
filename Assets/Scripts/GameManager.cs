using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool uiOpen = false;
    private void Awake()
    {
        if (Instance != null)
        {
            throw new UnityException("GameManager has already an Instance");
        }
        Instance = this;

    }
}
