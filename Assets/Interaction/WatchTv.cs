using UnityEngine;

namespace Assets.Interaction
{
    public class WatchTv : LockToScreen
    {
        public TELEVISAO tELEVISAO;
        public override void Interact()
        {
            base.Interact();
            tELEVISAO.ShowController(this);
            Cursor.visible = true;
        }
        public override void DeInteract()
        {
            base.DeInteract();
            Cursor.visible = false;
        }
    }
}