using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject playerNumberUIPrefab;

    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject winnerPanel;

    [SerializeField] private Slider entityCountSlider;
    [SerializeField] private TextMeshProUGUI entityCountText;

    [SerializeField] private TextMeshProUGUI entityWinnerNumberText;

    SpawnEntitySystem spawnEntitySystem;
    EntityManager entityManager;
    public int entityCount;

    private void Start()
    {
        spawnEntitySystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SpawnEntitySystem>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void OnEnable() {
        var spawnWorldUISpaceSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SpawnWorldSpaceUISystem>();
        var winnerCheckerSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<WinnerCheckerSystem>();
        spawnWorldUISpaceSystem.OnCharacterSpawn += SpawnPlayerNumberUI;
        winnerCheckerSystem.OnWinnerDecided += ShowWinnerPanel;
    }

    private void OnDisable() {
        var spawnWorldUISpaceSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SpawnWorldSpaceUISystem>();
        var winnerCheckerSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<WinnerCheckerSystem>();
        spawnWorldUISpaceSystem.OnCharacterSpawn -= SpawnPlayerNumberUI;
        winnerCheckerSystem.OnWinnerDecided -= ShowWinnerPanel;
    }

    private void ShowWinnerPanel(Entity winningEntity) {
        var character = entityManager.GetComponentData<Character>(winningEntity);
        entityWinnerNumberText.text = $"#{ character.characterNumber }";
        winnerPanel.SetActive(true);
    }

    private void SpawnPlayerNumberUI(int playerNumber, float3 spawnPosition, Entity entity)
    {
        var newSpawnPos = new Vector3(spawnPosition.x, spawnPosition.y + 1f, spawnPosition.z);
        var newPlayerNumberUI = Instantiate(playerNumberUIPrefab, newSpawnPos, Quaternion.identity, transform);
        var playerNumberText = newPlayerNumberUI.GetComponentInChildren<TextMeshProUGUI>();
        var playerNumberUI = newPlayerNumberUI.GetComponent<PlayerNumberUI>();

        playerNumberUI.characterEntity = entity;
        newPlayerNumberUI.transform.eulerAngles = new Vector3(45f, 0f, 0f);
        playerNumberText.text = $"#{ playerNumber }";
    }

    public void EntitySliderValueChanged() { 
        entityCount = (int)entityCountSlider.value;
        entityCountText.text = $" {entityCount} ";

        spawnEntitySystem.amountToSpawn = entityCount;
    }

    public void StartSimulationBTN() {

        spawnEntitySystem.Enabled = true;
        startPanel.SetActive(false);
    }

    public void QuitBTN() {
        Application.Quit();
    }
}
