using System;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{


    public List<Action>         UpdateActionList      = new();
    public List<Action>         FixedUpdateActionList = new();
    public List<Action>         LateUpdateActionList  = new();

    public List<IUpdateManager> UpdateList            = new();
    public List<IUpdateManager> FixedUpdateList       = new();
    public List<IUpdateManager> LateUpdateList        = new();

    private bool             isUpdating            = false;
    public UpdateManager        Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Update() { }
}