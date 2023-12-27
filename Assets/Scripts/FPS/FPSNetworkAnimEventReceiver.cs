using System.Collections;
using System.Collections.Generic;
using Demo.Scripts.Runtime;
using UnityEngine;

public class FPSNetworkAnimEventReceiver : MonoBehaviour
{
    [SerializeField] private FPSNetworkController controller;

    private void Start()
    {
        if (controller == null)
        {
            controller = GetComponentInParent<FPSNetworkController>();
        }
    }
        
    public void SetActionActive(int isActive)
    {
        if(isActive == 0) controller.ResetActionState();
    }

    public void ChangeWeapon()
    {
    }

    public void RefreshStagedState()
    {
        controller.RefreshStagedState();
    }

    public void ResetStagedState()
    {
        controller.ResetStagedState();
    }
}
