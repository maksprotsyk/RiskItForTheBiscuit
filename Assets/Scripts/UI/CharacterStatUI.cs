using Characters;
using DataStorage.Generated;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatUI : MonoBehaviour
{
    public Slider HealthSlider;
    public Slider StaminaSlider;

    CharacterBase playerCharacter;


    // Start is called before the first frame update
    void Start()
    {
        GameplayManager gameplayManager = ManagersOwner.GetManager<GameplayManager>();
        if (!gameplayManager)
        {
            Debug.LogError("InventorySlot: No GameplayManager found.");
            return;
        }

        playerCharacter = gameplayManager.PlayerController.GetComponent<CharacterBase>();
        if (!playerCharacter)
        {
            Debug.LogError("InventorySlot: No CharacterBase found on PlayerController.");
            return;
        }

        if (StaminaSlider)
        {
            StaminaSlider.value = 0; // playerCharacter.Movement.StaminaPercentage;
        }

        if (HealthSlider)
        {
            HealthSlider.value = 0; // playerCharacter.Health.HealthPercentage
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (StaminaSlider)
        {
            StaminaSlider.value = playerCharacter.Movement.StaminaPercentage;
        }

        if (HealthSlider)
        {
            HealthSlider.value = playerCharacter.Health.HealthPercentage;
        }
    }
}
