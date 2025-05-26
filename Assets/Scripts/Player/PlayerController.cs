using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 1f;

    private void Update()
    {
        transform.Translate(transform.forward * Time.deltaTime * speed);
    }
}
