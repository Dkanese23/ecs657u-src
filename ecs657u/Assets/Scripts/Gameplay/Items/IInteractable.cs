using UnityEngine;

// This file defines the "Contract" that both Cards and Clues must sign.
public interface IInteractable
{
    void Interact(GameObject interactor);
}