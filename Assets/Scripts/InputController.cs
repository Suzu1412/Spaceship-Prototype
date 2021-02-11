using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public float horizontal { get; private set; }
    public float vertical { get; private set; }
    public bool isShooting { get; private set; }

    private bool canClearInput;

    // Update is called once per frame
    private void Update()
    {
        CleanInput();
        ReadInput();
        //ProcessTouchInputs();
        horizontal = Mathf.Clamp(horizontal, -1f, 1f);
        vertical = Mathf.Clamp(vertical, -1f, 1f);
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

        horizontal = 0f;
        vertical = 0f;
        isShooting = false;
        canClearInput = false;
    }

    private void ReadInput()
    {
        horizontal += Input.GetAxisRaw("Horizontal");
        vertical += Input.GetAxisRaw("Vertical");
        isShooting = Input.GetButton("Shoot");
    }
}
