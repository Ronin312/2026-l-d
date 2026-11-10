using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FrostGame.Scriptable
{
    /// <summary>
    /// Klasa reprezentująca dane gry w formie ScriptableObject.
    /// </summary>

    // Ta linijka pozwala na stworzenie pliku prawym przyciskiem myszy w Unity
    [CreateAssetMenu(fileName = "FrostGameData", menuName = "ScriptableObjects/FrostGameData")]
    public class FrostGameData : ScriptableObject
    {
        public InputActionAsset inputActions;
        public InputActionReference[] actions;
    }

    public class FrostGameDataLoader
    {
        static FrostGameData frostGameData;
        public static FrostGameAction LoadAction(InputEnumType type)
        {
            if (frostGameData == null)
                LoadActions();

            switch (type)
            {
                case InputEnumType.Attack1:
                    return new FrostGameAction { actionRef = frostGameData.actions[0], index = 0 };
                case InputEnumType.Attack2:
                    return new FrostGameAction { actionRef = frostGameData.actions[1], index = 0 };
                case InputEnumType.Interact:
                    return new FrostGameAction { actionRef = frostGameData.actions[2], index = 0 };
                case InputEnumType.Skill:
                    return new FrostGameAction { actionRef = frostGameData.actions[3], index = 0 };
                case InputEnumType.ControlUP:
                    return new FrostGameAction { actionRef = frostGameData.actions[4], index = 1 };
                case InputEnumType.ControlDown:
                    return new FrostGameAction { actionRef = frostGameData.actions[4], index = 2 };
                case InputEnumType.ControlLeft:
                    return new FrostGameAction { actionRef = frostGameData.actions[4], index = 3 };
                case InputEnumType.ControlRight:
                    return new FrostGameAction { actionRef = frostGameData.actions[4], index = 4 };
                default:
                    Debug.LogError("This shouldn't happen");
                    return new FrostGameAction() ;

            }
        }

        public static void ResetBindings()
        {
            if (frostGameData == null)
                LoadActions();
            frostGameData.inputActions.RemoveAllBindingOverrides();
        }

        public static InputActionAsset LoadActions()
        {
            // Wczytanie zasobu ScriptableObject z folderu Resources
            if(frostGameData == null)
                frostGameData = Resources.Load<FrostGameData>("FrostGameData");

            if (frostGameData != null)
            {
                return frostGameData.inputActions;
            }
            else
            {
                Debug.LogError("Nie znaleziono zasobu FrostGameData w folderze Resources!");
                return null;
            }
        }

        //public InputActionAsset LoadFrostGameData()
        //{
        //    // Wczytanie zasobu ScriptableObject z folderu Resources
        //    FrostGameData frostGameData = Resources.Load<FrostGameData>("FrostGameData");
        //    if (frostGameData != null)
        //    {
        //        return frostGameData.inputActions;
        //    }
        //    else
        //    {
        //        Debug.LogError("Nie znaleziono zasobu FrostGameData w folderze Resources!");
        //        return null;
        //    }
        //}

    }

    public struct FrostGameAction
    {
       public InputActionReference actionRef;
        public int index;
    }

    public enum InputEnumType
    {
        ControlUP =1,
        ControlDown = 2,
        ControlLeft = 3,
        ControlRight = 4,
        Attack1 = 5,
        Attack2 = 6,
        Interact = 7,
        Skill = 8
    }
}