using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public interface IUI
{
    public void OnOpeningAnimationPlayed();
    public void OnClosingAnimationPlayed();
}
