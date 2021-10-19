using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public interface IView
{
    // ReSharper disable once UnusedMember.Global
    public void OnOpeningAnimationPlayed();
    // ReSharper disable once UnusedMember.Global
    public void OnClosingAnimationPlayed();
}
