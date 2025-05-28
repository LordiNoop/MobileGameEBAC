using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayScene : MonoBehaviour
{
    public PlayerController controller;

    public void StartScene()
    {
        controller.StartToRun();
    }
}
