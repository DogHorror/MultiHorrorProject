using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [Header("UI Settings")] 
    public Camera camera;
    public RectTransform handCursor;
    public TextMeshProUGUI promptMessage;
    [SerializeField] private bool interpolateTargetCursor = false;
    [SerializeField] private float smoothingFactor = 0.2f;
    
    public bool hasHandCursorTarget = false;
    public Vector3 handCursorGoalPosition;
    public Vector3 handCursorBeforePosition;
    public float handCursorProgress = 0f;
    

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


    public void SetHandCursorTarget(Vector3 target)
    {
        handCursorGoalPosition = target;
    }

    public Vector2 GetScreenPosition(Vector3 position)
    {
        return camera.WorldToScreenPoint(position);
    }
    
    public void Update()
    {
        if (hasHandCursorTarget)
        {
            if (handCursorGoalPosition != handCursorBeforePosition)
            {
                interpolateTargetCursor = true;
                if (Vector2.Distance(handCursor.position, GetScreenPosition(handCursorGoalPosition)) < 1f)
                {
                    interpolateTargetCursor = false;
                    handCursorProgress = 1f;
                    handCursorBeforePosition = handCursorGoalPosition;
                }
            }

            if (interpolateTargetCursor)
            {
                handCursor.position = Vector2.Lerp(handCursor.position, GetScreenPosition(handCursorGoalPosition),
                    smoothingFactor);
            }
            else
                handCursor.position = GetScreenPosition(handCursorGoalPosition);
        }
        else
        {
            handCursor.anchoredPosition = Vector2.zero;
        }
    }
}
