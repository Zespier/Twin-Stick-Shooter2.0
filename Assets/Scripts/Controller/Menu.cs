using System.Collections.Generic;
using System;
using UnityEngine;

public class Menu : MonoBehaviour {
    public CanvasGroup _canvasGroup;

    public bool IsOpen => _canvasGroup.alpha >= 0.99f;

    private void Awake() {
        ActiveCanvasGroup(false);
    }

    public virtual void UseItem() {
    }

    public virtual void ActiveCanvasGroup(bool active) {
        _canvasGroup.alpha = active ? 1 : 0;
        _canvasGroup.interactable = active;
        _canvasGroup.blocksRaycasts = active;
    }
}
