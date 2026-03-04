using System.Collections.Generic;
using UnityEngine;

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
