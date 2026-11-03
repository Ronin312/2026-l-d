using FrostGame.Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

namespace FrostGame.UI
{
    public class SimpleRebindButton : MonoBehaviour
    {
        [SerializeField]
        InputEnumType actionType;

        private InputActionReference actionReference; // Referencja do akcji (np. Jump)
        private int bindingIndex = 0; // Klawiatura = zazwyczaj 0 (Composite np. WASD ma więcej indeksów)

        [Header("UI Elements")]
        [SerializeField] private Button rebindButton;
        [SerializeField] private TextMeshProUGUI statusText;

        private InputActionRebindingExtensions.RebindingOperation _rebindOperation;

        private void OnEnable()
        {
            FrostGameAction fga = FrostGameDataLoader.LoadAction(actionType);
            actionReference = fga.actionRef;
            bindingIndex = fga.index;
            rebindButton.onClick.AddListener(StartRebinding);
            //UpdateUI();
        }

        private void OnDisable()
        {
            rebindButton.onClick.RemoveListener(StartRebinding);
            _rebindOperation?.Dispose(); // Bezpieczne czyszczenie pamięci
        }

        private void Start()
        {
            Invoke(nameof(UpdateUI), 0.05f);
        }

        private void StartRebinding()
        {
            if (actionReference == null) return;

            InputAction action = actionReference.action;

            // 1. Zawsze wyłącz akcję lub mapę przed remapowaniem
            action.Disable();

            statusText.text = "Naciśnij dowolny klawisz...";
            rebindButton.interactable = false;

            // 2. Uruchomienie interaktywnego nasłuchiwania klawiszy
            _rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                //.WithControlsExcluding("Mouse") // Opcjonalnie: ignoruj ruchy myszką
                // WYMUSZENIE TYLKO PRZYCISKÓW (pomija pełne osie analogowe)
                .WithExpectedControlType<ButtonControl>()

                // Opcjonalnie: wykluczenie zbędnych wskaźników ruchu myszy
                .WithControlsExcluding("<Pointer>/position")

                .OnComplete(operation => ValidateAndComplete(operation)) // Walidacja PO zakończeniu
                .OnCancel(operation => CleanUp())
                .Start();
        }


        private void ValidateAndComplete(InputActionRebindingExtensions.RebindingOperation operation)
        {
            InputAction currentAction = actionReference.action;

            // 1. Sprawdź, czy nowo przypisany klawisz jest duplikatem
            if (CheckIfDuplicate(currentAction, bindingIndex))
            {
                Debug.LogWarning("Wykryto duplikat! Cofam przypisanie.");

                // 2. JEŚLI DUPLIKAT: Usuń nowo nałożony override klawisza
                currentAction.RemoveBindingOverride(bindingIndex);

                // Wyświetlamy komunikat błędu i pozwalamy graczowi spróbować ponownie
                statusText.text = "Klawisz już zajęty!";

                CleanUp();
                return;
            }

            // JEŚLI WSZYSTKO OK: Kończymy sukcesem
            CleanUp();
        }

        private void CleanUp()
        {
            _rebindOperation?.Dispose();
            _rebindOperation = null;

            actionReference.action.Enable();
            rebindButton.interactable = true;

            UpdateUI();
        }


        private void RebindCompleted()
        {
            // 3. Posprzątaj operację i włącz akcję ponownie
            _rebindOperation.Dispose();
            _rebindOperation = null;

            actionReference.action.Enable();
            rebindButton.interactable = true;

            UpdateUI();
        }


        private bool CheckIfDuplicate(InputAction currentAction, int currentBindingIndex)
        {
            InputActionAsset asset = currentAction.actionMap?.asset;
            if (asset == null) return false;

            // Pobieramy dokładną, finalną ścieżkę klawisza, którą system WŁAŚNIE przypisał
            string newlyAssignedPath = currentAction.bindings[currentBindingIndex].effectivePath;

            foreach (var actionMap in asset.actionMaps)
            {
                foreach (var action in actionMap.actions)
                {
                    for (int i = 0; i < action.bindings.Count; i++)
                    {
                        // Ignorujemy dokładnie tę samą akcję i ten sam indeks, który edytujemy
                        if (action == currentAction && i == currentBindingIndex)
                            continue;

                        // Porównujemy efektywne ścieżki
                        if (action.bindings[i].effectivePath == newlyAssignedPath)
                        {
                            return true; // Znaleziono identyczny klawisz w innej akcji/indeksie
                        }
                    }
                }
            }
            return false;
        }

        //private bool CheckIfDuplicate(string newPath, InputAction currentAction)
        //{
        //    // Pobieramy cały plik z akcjami (Input Action Asset)
        //    InputActionAsset asset = currentAction.actionMap?.asset;

        //    foreach (var actionMap in asset.actionMaps)
        //    {
        //        foreach (var action in actionMap.actions)
        //        {
        //            // Przeszukujemy wszystkie przypisane klawisze w grze
        //            foreach (var binding in action.bindings)
        //            {
        //                // Sprawdzamy czy ścieżka się pokrywa (ignorujemy aktualnie edytowaną akcję)
        //                if (binding.effectivePath == newPath && action != currentAction)
        //                {
        //                    return true; // Znaleziono duplikat!
        //                }
        //            }
        //        }
        //    }
        //    return false; // Klawisz jest wolny
        //}

        public void UpdateUI()
        {
            if (actionReference != null)
            {
                // Wyświetla aktualną nazwę przypisanego klawisza (np. "Space", "W")
                statusText.text = actionReference.action.GetBindingDisplayString(bindingIndex);
                if (SaveLoadBindings.Instance != null) SaveLoadBindings.Instance.SaveSettings();
            }
        }
    }
}