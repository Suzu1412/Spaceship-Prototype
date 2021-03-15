using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public bool IsShooting { get; private set; }

    Joystick joystick;

    private bool canClearInput;

    private void Awake()
    {
        joystick = FindObjectOfType<Joystick>();

        if (!Application.isMobilePlatform)
            joystick.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        CleanInput();
        ReadInput();
        ProcessTouchInputs();
        Horizontal = Mathf.Clamp(Horizontal, -1f, 1f);
        Vertical = Mathf.Clamp(Vertical, -1f, 1f);
    }

    private void FixedUpdate()
    {
        canClearInput = true;
    }

    private void CleanInput()
    {
        if (!canClearInput)
        {
            return;
        }

        Horizontal = 0f;
        Vertical = 0f;
        IsShooting = false;
        canClearInput = false;
    }

    private void ReadInput()
    {
        Horizontal += Input.GetAxisRaw("Horizontal");
        Vertical += Input.GetAxisRaw("Vertical");
        IsShooting = Input.GetButton("Shoot");
    }

    void ProcessTouchInputs()
    {
        if (!Application.isMobilePlatform)
        {
            return;
        }

        if (Time.timeScale == 0)
        {
            joystick.gameObject.SetActive(false);
        }
        else
        {
            joystick.gameObject.SetActive(true);
        }
        Horizontal += joystick.Horizontal * 2f;
        Vertical += joystick.Vertical * 2f;
    }

    public void DisableTouchJoystick()
    {
        if (joystick != null)
        joystick.gameObject.SetActive(false);
    }
}
