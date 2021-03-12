using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Preferences")]
public class PlayerPreferences : ScriptableObject
{
    private PlayerController player1;
    private int currentStage;

    public void SetCharacter(PlayerController player)
    {
        player1 = player;
    }

    public PlayerController GetCharacter()
    {
        return player1;
    }

    public void SetScene(int scene)
    {
        if (currentStage <= scene)
        {
            currentStage = scene;
        }
    }

    public int GetScene()
    {
        return currentStage;
    }
}
