using NUnit.Framework;
using System;
using UnityEngine;
using FrostGame.InteractableObjects;
using System.Collections.Generic;
namespace FrostGame.PlayerStuf
{
    public class PlayerInteract : MonoBehaviour
    {
        [SerializeField] private bool takeAll = false;
        [SerializeField] private PlayerControl playerControl;
        List<InteractableObiect> interactableObjects = new List<InteractableObiect>();
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            playerControl.OnInteractAction += HandleInteract;
        }

        private void HandleInteract()
        {
            if(interactableObjects.Count > 0)
            {
                if(takeAll)
                {
                    for (int i = interactableObjects.Count - 1; i >= 0; i--)
                    {
                        if (interactableObjects[i].Interact())
                        {
                            interactableObjects.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    if (interactableObjects[interactableObjects.Count - 1].Interact())
                    {
                        interactableObjects.RemoveAt(interactableObjects.Count - 1);
                    }
                }   
                //if (interactableObjects[interactableObjects.Count - 1].Interact())
                //{
                //    interactableObjects.RemoveAt(interactableObjects.Count - 1);
                //}
                //if (interactableObjects[0].Interact())
                //{
                //    interactableObjects.RemoveAt(0);
                //}
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out InteractableObiect interactable))
            {
                interactableObjects.Add(interactable);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out InteractableObiect interactable))
            {
                interactableObjects.Remove(interactable);
            }   
        }


    }
}
