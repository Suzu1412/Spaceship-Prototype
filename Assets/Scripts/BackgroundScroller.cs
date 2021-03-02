using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] Transform bg1, bg2;
    [SerializeField] float scrollSpeed;
    [SerializeField] Transform fg1, fg2;

    // Update is called once per frame
    void Update()
    {
        bg1.position -= new Vector3(0f, scrollSpeed * Time.deltaTime ,0f);
        bg2.position -= new Vector3(0f, scrollSpeed * Time.deltaTime, 0f);

        if (bg1.position.y < -15f)
        {
            bg1.position += new Vector3(0f, 30f, 0);
        }

        if (bg2.position.y < -15f)
        {
            bg2.position += new Vector3(0f, 30f, 0);
        }

        fg1.position -= new Vector3(0f, scrollSpeed * 3 * Time.deltaTime, 0f);
        fg2.position -= new Vector3(0f, scrollSpeed * 3 * Time.deltaTime, 0f);

        if (fg1.position.y < -15f)
        {
            fg1.position += new Vector3(0f, 30f, 0);
        }

        if (fg2.position.y < -15f)
        {
            fg2.position += new Vector3(0f, 30f, 0);
        }
    }
}
