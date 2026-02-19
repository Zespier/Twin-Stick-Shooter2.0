using System.Collections.Generic;
using System;
using UnityEngine;

public class Menu : MonoBehaviour {
    // me podrían quitar el carne de programador por esto
    protected bool _isInTutorial;
    [SerializeField] private CanvasGroup _canvasGroup;

    public CanvasGroup canvasGroup => _canvasGroup;

    public bool IsInTutorial => _isInTutorial;
    public bool isOpen => _canvasGroup.alpha >= 0.99f;
    public Action<bool> OnMenuDisplayed;

    public virtual void UseItem() {
    }

    public virtual void SetTutorialState(bool state) {
        _isInTutorial = state;
    }

    public virtual void ActiveCanvasGroup(bool active) {
        _canvasGroup.alpha = active ? 1 : 0;
        _canvasGroup.interactable = active;
        _canvasGroup.blocksRaycasts = active;

        OnMenuDisplayed?.Invoke(active);
    }

    public void FadeCanvasGroup(bool active, float time, bool timeScaled = false, Action onComplete = null) {
        //UIAnimator.Fade(canvasGroup, active, time, timeScaled, onComplete);
    }
}

public class GroupForController : MonoBehaviour {

    public List<SelectableItemForController> items;
    public SelectableItemForController lastUsedItem;
    public bool alwaysHoverLastUsedItem;
    //Quiero moverme entre items, eso bien, pero me gustaría que grupos tuvieran diferentes comportamientos, es decir, si voy al grupo de teamSlots, me gustaría que el cursor se pusiera en el que esté actualmente abierto
    //Realmente por lo demás me da igual xd.
    //Luego entre items si que necesitaría herencias. Aunque es cierto que no va a haber muchas cosas además de botones, pero solo por si acaso.

    public void HoverPreferedItem(MenuWithSelectableItems menu, SelectableItemForController posibleItem) {
        if (alwaysHoverLastUsedItem) {
            lastUsedItem.Hover();
            //menu.hoveredItem = alwaysHoverLastUsedItem ? lastUsedItem : posibleItem;
        } else {
            posibleItem.Hover();
            //menu.hoveredItem = posibleItem;
        }
    }
}
