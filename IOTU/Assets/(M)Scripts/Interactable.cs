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
public abstract class Interactable : MonoBehaviour
{
    public virtual void Awake()
    {
        gameObject.layer = 9;
    }
    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void onLoseFocus();
}
