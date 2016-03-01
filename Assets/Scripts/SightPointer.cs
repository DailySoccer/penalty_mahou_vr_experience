using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SightPointer : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   /// <summary>
   /// Outer part of the pointer.
   /// </summary>
   public Sprite Outer;
   /// <summary>
   /// Innerpar of the pointer.
   /// </summary>
   public Sprite Inner;
   /// <summary>
   /// Size of the pointer
   /// </summary>
   [Range(0.01f,10f)]
   public float PointerSize = 1f;
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
   /// <summary>
   /// Unity Start() method.
   /// </summary>
   public void Init()
   {
      _initiated = Outer != null && Inner != null;
      if (_initiated)
      {
         _imageHolder = new GameObject("Holder").transform;
         GameObject auxRefOuter = new GameObject("Outer"), auxRefInner = new GameObject("Inner");
         _outerImage = auxRefOuter.AddComponent<SpriteRenderer>();
         _innerImage = auxRefInner.AddComponent<SpriteRenderer>();
         _outerImage.sprite = Outer;
         _innerImage.sprite = Inner;
         _imageHolder.SetParent(transform);
         auxRefOuter.transform.SetParent(_imageHolder);
         auxRefInner.transform.SetParent(_imageHolder);
         auxRefOuter.transform.localPosition = auxRefInner.transform.localPosition = _imageHolder.localPosition = Vector3.zero;
         auxRefOuter.transform.localRotation = auxRefInner.transform.localRotation = _imageHolder.localRotation = Quaternion.identity;
         _imageHolder.localScale = Vector3.one * PointerSize;
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined." + (Outer == null ? " Outer image reference missing." : string.Empty) +
                   (Inner == null ? " Inner image reference missing." : string.Empty) + " </color>");
      }
   }
   #endregion  //End public methods

   //-----------------------------------------------------------//
   //                  MONOBEHAVIOUR METHODS                    //
   //-----------------------------------------------------------//
   #region Monobehaviour methods

   // Use this for initialization

   #endregion  //End monobehaviour methods

   //-----------------------------------------------------------//
   //                      PRIVATE METHODS                      //
   //-----------------------------------------------------------//
   #region Private methods
   #endregion  //End private methods

   //-----------------------------------------------------------//
   //                      PRIVATE MEMBERS                      //
   //-----------------------------------------------------------//
   #region Private members
   private bool _initiated;
   private Transform _imageHolder;
   private SpriteRenderer _outerImage;
   private SpriteRenderer _innerImage;
   #endregion  //End private members
}
