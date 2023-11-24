using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// #####################################################################
// ***************^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^***************
// ***************^                                     ^***************
// ***************^  Published by The indie club        ^***************
// ***************^                                     ^***************
// ***************^  Copyright © 2023 by Ali Mohamed    ^***************
// ***************^                                     ^***************
// ***************^   All rights reserved.              ^***************
// ***************^                                     ^***************
// ***************^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^***************
// #####################################################################
public class testInteraction : Interactable
{
    public override void OnFocus()
    {
        print("Look at" + gameObject.name);
    }

    public override void OnInteract()
    {
        print("interact" + gameObject.name);
    }
    
    public override void onLoseFocus()
    {
        print("stop" + gameObject.name);
    }
}
