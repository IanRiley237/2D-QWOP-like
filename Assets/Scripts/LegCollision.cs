using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LegCollision : MonoBehaviour
{
    public Toggle checkbox;

    private int layer;

    private void Start()
    {
        // Get the layer IDs of the assigned layers
        layer = LayerMask.NameToLayer("Player Leg Shafts");
    }

    public void Toggler()
    {
        Physics2D.IgnoreLayerCollision(layer, layer, !checkbox.isOn);
    }
}
