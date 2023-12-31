using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public PlayerInput playerInput;
    
    
    public void Awake()
    {
        if (null == instance && instance != this)
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }
}
