using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Player Stats")]
public class PlayerStats : Stats
{
    [Header("Player Stats")]
    [Range(5f, 15f)] public float homingRange;
    [Range(0.01f, 0.1f)] public float chanceToInstakill;


    [Header("Level Stats")]
    public List<Levels> levels = new List<Levels>();
    private int _level;
    private int _experience;
    private bool _maxLevel;
    private bool _limitBreak;
    private float _limitBreakDuration;
    public int Level {
        get {
            if (LimitBreak)
            {
                return _level + 1;
            }
            return _level;
        }
    }
    public bool LimitBreak { get { return _limitBreak; } }

    public bool MaxLevel { get { return _maxLevel;  } }

    public int Experience
    {
        get
        {
            if (_maxLevel)
            {
                return ExperienceToNextLevel;
            }
            else
            {
                return _experience;
            }
        }
    }

    public int ExperienceToNextLevel
    {
        get
        {
            return levels[_level].experienceToNextLevel;
        }
    }

    public float LimitBreakDuration { get { return _limitBreakDuration; } }

    public int AddExperience(int amount)
    {
        if (_maxLevel)
        {
            _limitBreakDuration += amount;

            if (_limitBreakDuration >= levels[_level].experienceToNextLevel)
            {
                _limitBreakDuration = levels[_level].experienceToNextLevel;

                if (!LimitBreak)
                {
                    _limitBreak = true;
                }
            }

            return Level;
        }

        _experience += amount;

        for (int i = _level; i < levels.Count; i++)
        {
            if (_experience - levels[i].experienceToNextLevel < 0)
            {
                break;
            }
            else
            {
                if (Level + 2 < levels.Count)
                {
                    _experience -= levels[i].experienceToNextLevel;
                    _level++;
                }
                else
                {
                    _experience -= levels[i].experienceToNextLevel;
                    _level++;
                    _maxLevel = true;
                }
            }
        }

        return Level;
    }

    public void ReleaseLimitBreak()
    {
        if (!LimitBreak) return;

        if (LimitBreak)
        {
            _limitBreakDuration -=  5f * Time.deltaTime;

            if (_limitBreakDuration <= 0)
            {
                _limitBreakDuration = 0;
                _limitBreak = false;
            }
        }
    }

    public void DamageLimitBreak()
    {
        _limitBreakDuration -= levels[_level].experienceToNextLevel / 4;
    }

    private void OnEnable()
    {
        ResetValues();
    }

    public void ResetValues()
    {
        _level = 0;
        _experience = 0;
        _limitBreakDuration = 0f;
        _maxLevel = false;
        _limitBreak = false;
    }
}

[System.Serializable]
public class Levels
{
    public int experienceToNextLevel;
}
