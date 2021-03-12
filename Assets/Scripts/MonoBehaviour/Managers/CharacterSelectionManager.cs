using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionManager : MonoBehaviour
{
    public SceneLoaderManager scene;
    [SerializeField] private List<PlayerController> playerList;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI attack; 
    [SerializeField] private TextMeshProUGUI attackSpeed; 
    [SerializeField] private TextMeshProUGUI movementSpeed; 
    [SerializeField] private TextMeshProUGUI defense; 
    [SerializeField] private TextMeshProUGUI special;
    [SerializeField] private Image sprite;
    private int currentCharacter;
    private int horizontal;
    private float selectionDelay = 0f;
    private bool canSelectCharacter;

    private void Start()
    {
        canSelectCharacter = true;
        SetDescription();
    }

    void Update()
    {
        ChangeCharacter();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SelectCharacter();
        }
    }

    private void ChangeCharacter()
    {
        horizontal = (int)Input.GetAxisRaw("Horizontal");

        selectionDelay -= Time.deltaTime;
        if (horizontal < 0 && selectionDelay <= 0f)
        {
            SwitchCharacter(horizontal);
            
        }
        else if(horizontal > 0 && selectionDelay <= 0f)
        {
            SwitchCharacter(horizontal);
        }
    }

    public void SelectCharacter()
    {
        if (canSelectCharacter)
        {
            scene.PlayerPreferences.SetCharacter(playerList[currentCharacter]);
            
            if (scene.PlayerPreferences.GetScene() < 2)
            {
                scene.FirstStage();
            }
            else
            {
                scene.NextScene();
            }
            
        }
        
    }

    public void PreviousSelection()
    {
        SwitchCharacter(-1);
    }

    public void NextSelection()
    {
        SwitchCharacter(1);
    }

    private void SwitchCharacter(int amount)
    {
        if (currentCharacter + amount < 0)
        {
            currentCharacter = playerList.Count - 1;
        }
        else if(currentCharacter + amount >= playerList.Count)
        {
            currentCharacter = 0;
        }
        else
        {
            currentCharacter += amount;
        }
        SetDescription();
        selectionDelay = 0.4f;
    }

    private void SetDescription()
    {
        sprite.sprite = playerList[currentCharacter].Sprite.sprite;
        playerName.text = playerList[currentCharacter].Description.name;
        attack.text = playerList[currentCharacter].Description.attack.ToString();
        attackSpeed.text = playerList[currentCharacter].Description.attackSpeed.ToString();
        movementSpeed.text = playerList[currentCharacter].Description.movementSpeed.ToString();
        defense.text = playerList[currentCharacter].Description.health.ToString();
        special.text = playerList[currentCharacter].Description.special.ToString();
    }
}
