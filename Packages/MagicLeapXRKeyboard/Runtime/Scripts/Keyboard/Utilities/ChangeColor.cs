using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ChangeColor : MonoBehaviour
{
    [SerializeField] private XRBaseInteractable _interactable;
    [SerializeField] private Image[] _items;
    [SerializeField] private Color _defaultColor = new Color(0.1f, 0.1f, 0.1f);
    [SerializeField] private Color _highlightColor = Color.gray;
    [SerializeField] private Color _selectColor = Color.white;
    // Start is called before the first frame update
    private void OnDrawGizmosSelected()
    {
        SwitchColor(_items, _defaultColor);
    }

    private void Start()
    {
        SwitchColor(_items, _defaultColor);
    }

    private void Update()
    {
        if (_interactable.isSelected)
        {
            SwitchColor(_items, _selectColor);
        }

        else if (_interactable.isHovered)
        {
            SwitchColor(_items, _highlightColor);
        }
        else
        {
            SwitchColor(_items, _defaultColor);
        }
       
    }

    private void SwitchColor(Image[] items, Color color)
    {
        if(items==null)
        return;
        foreach (var image in items)
        {
            if(image)
            image.color = color;
        }
    }

}
