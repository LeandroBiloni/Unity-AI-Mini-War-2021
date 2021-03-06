﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float minSpawnDist;
    public float maxSpawnDist;
    public Leader blueLeader;
    public Leader redLeader;
    public List<Being> entities;
    public Follower redFollower;
    public Follower blueFollower;
    public Transform redSpawn;
    public Transform blueSpawn;
    public Material redMat;
    public Material blueMat;

    public GameObject winBG;
    public GameObject winBlue;
    public GameObject winRed;
    public GameObject addRedButton;
    public GameObject addBlueButton;
    public GameObject startButton;
    public GameObject reloadButton;

    public Image blueHPBar;
    public Image redHPBar;

    float _redTeamCounter;
    float _blueTeamCounter;

    public TextMeshProUGUI blueTeamCounter;
    public TextMeshProUGUI redTeamCounter;

    float instanceNumber = 1;
    private void Awake()
    {
        GetEnemies();
        winBG.SetActive(false);
        winBlue.SetActive(false);
        winRed.SetActive(false);
        startButton.SetActive(true);
        reloadButton.SetActive(false);
        blueTeamCounter.text = "0";
        redTeamCounter.text = "0";
    }

    public void GetEnemies()
    {
        var enemies = FindObjectsOfType<Being>();

        entities.Clear();
        blueLeader.enemyTeam.Clear();
        redLeader.enemyTeam.Clear();
        foreach (var enemy in enemies)
        {
            if (enemy.selectedTeam == Being.Team.Red)
                blueLeader.enemyTeam.Add(enemy);
            else if (redLeader != null)
                redLeader.enemyTeam.Add(enemy);

            entities.Add(enemy);
        }
    }

    public void EndSimulation(Being.Team deadTeam)
    {
        foreach (var entity in entities)
        {
            if (entity)
                entity.StopWorking();
        }

        winBG.SetActive(true);
        switch (deadTeam)
        {
            case Being.Team.Blue:
                winRed.SetActive(true);
                break;

            case Being.Team.Red:
                winBlue.SetActive(true);
                break;
        }
    }
    public void StartSimulation()
    {
        GetEnemies();
        addBlueButton.SetActive(false);
        addRedButton.SetActive(false);
        foreach (var entity in entities)
        {
            if (entity)
            {
                entity.rb.isKinematic = false;
                entity.StartWorking();
            }
        }
        startButton.SetActive(false);
        reloadButton.SetActive(true);
    }

    public void AddAI(int team)
    {
        switch (team)
        {
            case 1:
                for (int i = 0; i < 5; i++)
                {
                    Follower blue = Instantiate(blueFollower, blueSpawn, false);
                    blue.transform.localPosition = new Vector3(Random.Range(minSpawnDist, maxSpawnDist), 0, Random.Range(minSpawnDist, maxSpawnDist));
                    blue.transform.localScale = new Vector3(1f, 1f, 1f);
                    blue.myLeader = blueLeader.transform;
                    blue.rb.isKinematic = true;
                    blue.GetComponent<LeaderBehavior>().target = blueLeader.transform;
                    blue.GetComponent<AwayBehavior>().target = blueLeader.transform;
                    blue.instanceNumber = instanceNumber;
                    blue.gameObject.name = "Follower " + instanceNumber;
                    instanceNumber++;
                    _blueTeamCounter++;
                }
                break;

            case 2:
                for (int i = 0; i < 5; i++)
                {
                    Follower red = Instantiate(redFollower, redSpawn, false);
                    red.transform.localPosition = new Vector3(Random.Range(minSpawnDist, maxSpawnDist), 0, Random.Range(minSpawnDist, maxSpawnDist));
                    red.transform.localScale = new Vector3(1f, 1f, 1f);
                    red.myLeader = redLeader.transform;
                    red.rb.isKinematic = true;
                    red.GetComponent<LeaderBehavior>().target = redLeader.transform;
                    red.GetComponent<AwayBehavior>().target = redLeader.transform;
                    red.instanceNumber = instanceNumber;
                    red.gameObject.name = "Follower " + instanceNumber;
                    instanceNumber++;
                    _redTeamCounter++;
                }
                break;
        }
        UpdateTeamCounter(_blueTeamCounter, _redTeamCounter);
    }

    public void UpdateHP(Teams.Team team, float maxHP, float currentHP)
    {
        if (team == Teams.Team.Blue)
        {
            blueHPBar.fillAmount = currentHP / maxHP;
            if (currentHP > maxHP / 2)
                blueHPBar.color = new Color(0f, 0.5754717f, 0.1331484f);

            if (currentHP <= maxHP / 2 && currentHP > maxHP / 4)
                blueHPBar.color = Color.yellow;

            if (currentHP <= maxHP / 4)
                blueHPBar.color = Color.red;

        }

        if (team == Teams.Team.Red)
        {
            redHPBar.fillAmount = currentHP / maxHP;
            if (currentHP > maxHP / 2)
                redHPBar.color = new Color(0f, 0.5754717f, 0.1331484f);

            if (currentHP <= maxHP / 2 && currentHP > maxHP / 4)
                redHPBar.color = Color.yellow;

            if (currentHP <= maxHP / 4)
                redHPBar.color = Color.red;
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReduceCounter(Teams.Team selectedTeam)
    {
        if (selectedTeam == Teams.Team.Blue)
            _blueTeamCounter--;
        if (selectedTeam == Teams.Team.Red)
            _redTeamCounter--;

        UpdateTeamCounter(_blueTeamCounter, _redTeamCounter);
    }

    void UpdateTeamCounter(float blue, float red)
    {
        blueTeamCounter.text = blue.ToString();
        redTeamCounter.text = red.ToString();
    }
}
