using System;
using Assets.Bet.UI;
using UnityEngine;

namespace Assets.Interaction
{
public class ManipulateGame : Interactable
{
    [SerializeField] ManipulatorUI manipulatorUI;
    [SerializeField] GameObject[] phone;
        public override void Interact()
        {
            base.Interact();
            manipulatorUI.OpenManipulator(this);
            playerMovement.LockCam(true);
            Cursor.visible = true;
            DisplayPhone(false);
            SoundEffects.Instance.PlayClip(SoundEffects.Instance.clips[9]);
        }
        void DisplayPhone(bool value)
        {
            foreach(var p in phone)
            {
                p.SetActive(value);
            }
        }
        public override void DeInteract()
        {
            base.DeInteract();
            playerMovement.LockCam(false);
            Cursor.visible = false;
            DisplayPhone(true);
        }
}
}