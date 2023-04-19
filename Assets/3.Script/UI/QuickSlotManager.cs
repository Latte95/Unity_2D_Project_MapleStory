using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotManager : MonoBehaviour
{
    Dictionary<KeyCode, string> keyCodes = new Dictionary<KeyCode, string>()
{
    { KeyCode.LeftShift, "LeftShift" },
    { KeyCode.Insert, "Insert" },
    { KeyCode.Home, "Home" }
};

    private void Update()
    {
        if (keyCodes.TryGetValue(Input.inputString[0], out string expectedName) && gameObject.name.Equals(expectedName))
        {
            // ½ÇÇàÇÒ ÄÚµå
        }
        // Äü½½·Ô
        if (Input.GetKeyDown(KeyCode.LeftShift) && gameObject.name.Equals("Shift"))
        {

        }
        if (Input.GetKeyDown(KeyCode.Insert) && gameObject.name.Equals("Ins"))
        {

        }
        if (Input.GetKeyDown(KeyCode.Home) && gameObject.name.Equals("Hm"))
        {

        }
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            gameObject.name.Equals("Pup");

        }
        //if (Input.GetKeyDown(KeyCode.LeftControl))
        //{
        //gameObject.name.Equals("Ctrl");

        //}
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            gameObject.name.Equals("Del");

        }
        if (Input.GetKeyDown(KeyCode.End))
        {
            gameObject.name.Equals("End");

        }
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            gameObject.name.Equals("Pdn");
        }
    }
}
