using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] ItemPickUp item;
    [SerializeField] float speed = 2f;
    private bool arrivedAtEndPosition;
    private bool setEndMarker;
    [SerializeField] private Vector3 endMarker;
    
    public float smoothTimeStart = 0.6F;
    private Vector3 velocity = Vector3.zero;

    public void UseItem(PlayerController player)
    {
        switch (item.type)
        {
            case ItemType.Power:
                player.AddPower(item.amount);
                break;

            case ItemType.Health:
                player.Heal(item.amount);
                break;
        }
    }

    private void OnEnable()
    {
        arrivedAtEndPosition = false;
        setEndMarker = false;
    }

    private void Update()
    {
        if (!setEndMarker)
        {
            endMarker = this.transform.position + Vector3.up * 1;
            setEndMarker = true;
        }
        if (!arrivedAtEndPosition)
        {
            Vector3 targetPosition = endMarker;

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTimeStart);

            if (Vector3.Distance(transform.position, endMarker) < 0.01f) arrivedAtEndPosition = true;
        }
        else
        { 
            transform.position += Vector3.down * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UseItem(collision.GetComponent<PlayerController>());
            this.gameObject.SetActive(false);
        }
    }
    private void OnBecameInvisible()
    {
        this.gameObject.SetActive(false);
    }
}
