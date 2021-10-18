using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool Sprint()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }

    public bool Jump()
    {
        return Input.GetKey(KeyCode.Space);
    }

    public bool ADS()
    {
        return Input.GetMouseButton(1);
    }

    public bool Fire()
    {
        return Input.GetButtonDown("Fire1");
    }

    public float Vertical()
    {
        return Input.GetAxisRaw("Vertical");
    }

    public float Horizontal()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public float MouseY()
    {
        return Input.GetAxis("Mouse Y");
    }

    public float MouseX()
    {
        return Input.GetAxis("Mouse X");
    }
}
