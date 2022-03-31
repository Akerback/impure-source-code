using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newGame : travelButton
{
    public override void goToLevel(string levelToGoTo)
    {
        globalContainer.savedPlayer = null;
        base.goToLevel(levelToGoTo);
    }
}
