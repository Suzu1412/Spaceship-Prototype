using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] ItemPickUp item;
    [SerializeField] float speed = 3f;
    private bool arrivedAtEndPosition;
    private bool setEndMarker;
    [SerializeField] private Vector3 endMarker;
    bool moveTowardsPlayer = false;
    Transform player;
    Vector2 playerDirection;
    
    public float smoothTimeStart = 0.2F;
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
        moveTowardsPlayer = false;
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
            if (!moveTowardsPlayer)
            {
                transform.position += Vector3.down * speed * Time.deltaTime;
            }
            else
            {
                playerDirection = (player.position - transform.position).normalized;
                transform.position += new Vector3(playerDirection.x, playerDirection.y, 0f) * 5 * Time.deltaTime; 
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UseItem(collision.GetComponent<PlayerController>());
            this.gameObject.SetActive(false);
        }

        if (collision.name == "ItemMagnet")
        {
            moveTowardsPlayer = true;
            player = collision.GetComponentInParent<Transform>();
        }
    }
    private void OnBecameInvisible()
    {
        this.gameObject.SetActive(false);
    }
}
