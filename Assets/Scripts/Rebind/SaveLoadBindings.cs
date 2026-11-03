using UnityEngine;
using UnityEngine.InputSystem;
using System.IO; // Wymagane do zapisu i odczytu plików tekstowych
using FrostGame.Scriptable;

namespace FrostGame.UI
    {
    /// <summary>
    /// Klasa odpowiedzialna za zapis i odczyt ustawień sterowania gracza.
    /// </summary>

public class SaveLoadBindings : MonoBehaviour
{
    public static SaveLoadBindings Instance { get; private set; }

    private InputActionAsset inputActions;

    private const string FileName = "PlayerKeybindings.json";
    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            inputActions = FrostGameDataLoader.LoadActions();
                // Zmiana na 'persistentDataPath' gwarantuje trwałość danych w Edytorze ORAZ w gotowej grze (Buildzie)
                savePath = Path.Combine(Application.persistentDataPath, FileName);

            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveSettings()
    {
        if (inputActions == null) return;

        try
        {
            // 1. Generujemy strukturę JSON z aktualnymi nadpisaniami klawiszy
            string bindingsJson = inputActions.SaveBindingOverridesAsJson();

            // 2. Zapisujemy plik tekstowy na stałe na dysku twardym
            File.WriteAllText(savePath, bindingsJson);

            Debug.Log($"[InputManager] Klawisze zapisane na stałe w bezpiecznej lokalizacji: {savePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[InputManager] Błąd podczas zapisywania pliku: {e.Message}");
        }
    }

    public void LoadSettings()
    {
        if (inputActions == null) return;

        // Sprawdzamy, czy plik z zapisanymi klawiszami fizycznie istnieje na dysku
        if (File.Exists(savePath))
        {
            try
            {
                // 1. Odczytujemy zawartość pliku JSON
                string controlJson = File.ReadAllText(savePath);

                // 2. Wstrzykujemy zapisany układ z powrotem do Input Systemu
                inputActions.LoadBindingOverridesFromJson(controlJson);

                Debug.Log("[InputManager] Pomyślnie wczytano trwałe ustawienia klawiszy z dysku.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[InputManager] Błąd podczas wczytywania pliku: {e.Message}");
            }
        }
        else
        {
            Debug.Log("[InputManager] Brak zapisanego pliku. Gra uruchamia się z domyślnym sterowaniem.");
        }
    }

    public void ResetToDefault()
    {
        if (inputActions == null) return;

        // Resetujemy nadpisania w pamięci gry
        foreach (var map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }

        // Usuwamy fizyczny plik z dysku, by gra przy kolejnym starcie nie wczytała starych zmian
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        // Aktualizujemy wyświetlanie liter na przyciskach w menu
        SimpleRebindButton[] allButtons = FindObjectsByType<SimpleRebindButton>(FindObjectsInactive.Include);
        foreach (var rebindButton in allButtons)
        {
            rebindButton.UpdateUI();
        }

        Debug.Log("[InputManager] Przywrócono ustawienia fabryczne i usunięto plik zapisu.");
    }
}
}



//using UnityEngine;
//using UnityEngine.InputSystem;
//using System.IO; // Wymagane do obsługi plików tekstowych

//public class SaveLoadBindings : MonoBehaviour
//{
//    public static SaveLoadBindings Instance { get; private set; }

//    [Header("Główny zasób sterowania")]
//    [SerializeField] private InputActionAsset inputActions;

//    // Nazwa pliku, który utworzy się w folderze projektu
//    private const string FileName = "PlayerKeybindings.json";
//    private string savePath;

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);

//            // Zapis bezpośrednio w głównym folderze 'Assets' projektu
//            savePath = Path.Combine(Application.dataPath, FileName);

//            LoadSettings();
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    public void SaveSettings()
//    {
//        if (inputActions == null) return;

//        // 1. Generujemy ciąg tekstowy JSON z nadpisanymi klawiszami
//        string bindingsJson = inputActions.SaveBindingOverridesAsJson();

//        // 2. Zapisujemy plik na dysku w formacie tekstowym
//        File.WriteAllText(savePath, bindingsJson);

//#if UNITY_EDITOR
//        // Informujemy edytor Unity, że pojawił się nowy plik, aby odświeżył okno Project
//        UnityEditor.AssetDatabase.Refresh();
//#endif

//        Debug.Log($"[InputManager] Zapisano klawisze do pliku JSON w: {savePath}");
//    }

//    public void LoadSettings()
//    {
//        if (inputActions == null) return;

//        // Sprawdzamy, czy plik fizycznie istnieje, zanim go wczytamy
//        if (File.Exists(savePath))
//        {
//            // 1. Odczytujemy cały tekst z pliku JSON
//            string controlJson = File.ReadAllText(savePath);

//            // 2. Wstrzykujemy układ do nowego Input Systemu
//            inputActions.LoadBindingOverridesFromJson(controlJson);

//            Debug.Log("[InputManager] Pomyślnie wczytano klawisze z pliku JSON.");
//        }
//        else
//        {
//            Debug.Log("[InputManager] Brak zapisanego pliku JSON. Używam domyślnych klawiszy.");
//        }
//    }

//    public void ResetToDefault()
//    {
//        if (inputActions == null) return;

//        // Przywracamy domyślne mapowanie
//        foreach (var map in inputActions.actionMaps)
//        {
//            map.RemoveAllBindingOverrides();
//        }

//        // Jeśli plik istnieje, usuwamy go
//        if (File.Exists(savePath))
//        {
//            File.Delete(savePath);

//#if UNITY_EDITOR
//            UnityEditor.AssetDatabase.Refresh();
//#endif
//        }

//        // Odświeżamy widok przycisków w UI
//        UI_RebindButton[] allButtons = FindObjectsByType<UI_RebindButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);
//        foreach (var rebindButton in allButtons)
//        {
//            rebindButton.UpdateBindingDisplay();
//        }

//        Debug.Log("[InputManager] Przywrócono domyślne ustawienia klawiszy.");
//    }
//}



//using UnityEngine;
//using UnityEngine.InputSystem;

//public class SaveLoadBindings : MonoBehaviour
//{
//    // Singleton ułatwiający dostęp przyciskom UI
//    public static SaveLoadBindings Instance { get; private set; }

//    [Header("Główny zasób sterowania")]
//    [SerializeField] private InputActionAsset inputActions;

//    private const string SaveKey = "Unity6_PlayerKeybindings";

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//            LoadSettings();
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    public void SaveSettings()
//    {
//        if (inputActions == null) return;

//        // Pobiera nadpisane klawisze w formacie JSON i zapisuje w PlayerPrefs
//        string bindingsJson = inputActions.SaveBindingOverridesAsJson();
//        PlayerPrefs.SetString(SaveKey, bindingsJson);
//        PlayerPrefs.Save();
//        Debug.Log("Zapisano ustawienia sterowania: " + bindingsJson);
//    }

//    public void LoadSettings()
//    {
//        if (inputActions == null) return;

//        string savedBindings = PlayerPrefs.GetString(SaveKey, string.Empty);

//        if (!string.IsNullOrEmpty(savedBindings))
//        {
//            inputActions.LoadBindingOverridesFromJson(savedBindings); // Wczytanie układu
//        }
//    }

//    // Funkcja podpięta np. pod przycisk "Przywróć domyślne"
//    public void ResetToDefault()
//    {
//        if (inputActions == null) return;

//        // Usuwa wszystkie modyfikacje gracza ze wszystkich map
//        foreach (var map in inputActions.actionMaps)
//        {
//            map.RemoveAllBindingOverrides();
//        }

//        PlayerPrefs.DeleteKey(SaveKey);
//        PlayerPrefs.Save();

//        // Wykorzystanie nowoczesnego, wydajnego FindObjectsByType (zastąpiło stare FindObjectsOfType)
//        UI_RebindButton[] allButtons = FindObjectsByType<UI_RebindButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);
//        foreach (var rebindButton in allButtons)
//        {
//            rebindButton.UpdateBindingDisplay();
//        }
//    }
//}
