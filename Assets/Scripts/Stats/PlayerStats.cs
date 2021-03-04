using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Player Stats")]
public class PlayerStats : Stats
{
    public List<Levels> levels = new List<Levels>();
    private int level;
    private int experience;

    public int AddExperience(int amount)
    {
        experience += amount;

        for (int i = level; i < levels.Count; i++)
        {
            if (experience - levels[i].experienceToNextLevel < 0)
            {
                break;
            }
            else
            {
                experience -= levels[i].experienceToNextLevel;
                level++;
            }
        }

        return level;
    }

    private void OnEnable()
    {
        level = 0;
        experience = 0;
    }
}

[System.Serializable]
public class Levels
{
    public int experienceToNextLevel;
}
