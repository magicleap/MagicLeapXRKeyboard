using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace MagicLeap.Common
{
   public class MoveHandle : MonoBehaviour
   {

       public enum ActionState
       {
           Default,
           Selected,
           Highlighted
       }
       [SerializeField] private XRBaseInteractable _interactable;
       [Space] [SerializeField] private Color _defaultColor = new Color(0.1f, 0.1f, 0.1f);
       [SerializeField] private Color _highlightColor = Color.gray;
       [SerializeField] private Color _selectColor = Color.white;
       [Space]
       [SerializeField] private Collider[] _colliders;
       [Space]
       [SerializeField] private Image[] _images;
       [Space]
       [SerializeField] private bool _lookAtOnGrab;
       [SerializeField] private Renderer[] _renderers;
       [SerializeField] private string[] _shaderKeyWords = new []{"_Color", "_BaseColor","_Tint","_TintColor"};
     
       private Dictionary<Material, string> _keywordByMaterials = new Dictionary<Material, string>();
       private List<Material> _materials = new List<Material>();
       private ActionState _currentActionState;
      
      
       private void OnDrawGizmosSelected()
       {
           SwitchImageColor(_defaultColor);
       }
       private void Reset()
       {
           _images = gameObject.GetComponentsInChildren<Image>(true);
           _colliders = gameObject.GetComponentsInChildren<Collider>(true);
           _interactable = GetComponentInParent<XRBaseInteractable>(true);
           if (_interactable == null)
           {
               _interactable = GetComponentInChildren<XRBaseInteractable>(true);
           }
       }

       void Start()
       {

           if (_interactable == null)
           {
               var parentXRBaseInteractable = GetComponentInParent<XRBaseInteractable>(true);
               if (parentXRBaseInteractable == null)
               {
                   _interactable = GetComponentInChildren<XRBaseInteractable>(true);
               }
               else
               {
                   _interactable = parentXRBaseInteractable;
               }
           }

           if (_interactable == null)
           {
             gameObject.SetActive(false);
             return;
           }
           _interactable.AddColliders(_colliders);
           GetMaterials();
           SwitchImageColor(_defaultColor);
           SwitchMaterialColor(_defaultColor);
           _currentActionState = ActionState.Default;
       }

       public void GetMaterials()
       {
           if (_renderers == null)
               return;
          
           foreach (var targetRenderer in _renderers)
           {
               if (targetRenderer)
               {
                   foreach (var targetRendererMaterial in targetRenderer.materials)
                   {
                       foreach (var shaderKeyWord in _shaderKeyWords)
                       {
                           if (targetRendererMaterial.HasProperty(shaderKeyWord))
                           {
                               _keywordByMaterials.Add(targetRendererMaterial,shaderKeyWord);
                               _materials.Add(targetRendererMaterial);
                               break;
                           }
                       }
                   }
               }
              
           }
       }


       // Update is called once per frame
       private void Update()
       {
         
           if (_interactable.isSelected )
           {
               if (_lookAtOnGrab)
               {
                   Vector3 directionToCamera = Camera.main.transform.position - transform.position;
                   Vector3 correctedDirection =
                       new Vector3(directionToCamera.x, directionToCamera.y, -directionToCamera.z);
                   Quaternion rotation = Quaternion.LookRotation(correctedDirection, Vector3.up);
                   _interactable.transform.rotation = rotation;
               }

               if (_currentActionState == ActionState.Selected) return;
               SwitchImageColor(_selectColor);
               SwitchMaterialColor(_selectColor);
               _currentActionState = ActionState.Selected;

           }

           else if (_interactable.isHovered)
           {
              
               if (_currentActionState == ActionState.Highlighted) return;
               SwitchImageColor(_highlightColor);
               SwitchMaterialColor(_highlightColor);
               _currentActionState = ActionState.Highlighted;
           }
           else
           {
               if (_currentActionState == ActionState.Default) return;
               SwitchImageColor(_defaultColor);
               SwitchMaterialColor(_defaultColor);
               _currentActionState = ActionState.Default;
           }

       }

       private void SwitchImageColor(Color color)
       {
           if (_images == null)
               return;
           foreach (var image in _images)
           {
               if (image)
                   image.color = color;
           }
       }
       private void SwitchMaterialColor(Color color)
       {
           if (_renderers == null)
               return;
          
           foreach (var material in _materials)
           {
               var shaderKeyword = _keywordByMaterials[material];
               material.SetColor(shaderKeyword,color);
           }
       }

   }
}

