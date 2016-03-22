using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class MenuManager : ResetElement
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   public delegate void VoidNoParams();
   public event VoidNoParams StartCall;
   /// <summary>
   /// MenuElement to start application when hovered.
   /// </summary>
   public MenuElement StartButton;
   /// <summary>
   /// Button to restart application when hovered.
   /// </summary>
   public MenuElement RestartButton;
   /// <summary>
   /// Prefab reference for menu element button.
   /// </summary>
   public GameObject ButtonPrefab;
   /// <summary>
   /// Group structure of team emblem and uniform texture.
   /// </summary>
   [System.Serializable]
   public class SpriteTexturePair
   {
      public Sprite SpriteRef;
      public Texture TextureRef;
      public Texture TextureLeg;
   }
   /// <summary>
   /// Configuration elements.
   /// </summary>
   public List<SpriteTexturePair> SelectElements = new List<SpriteTexturePair>();
   /// <summary>
   /// Menu background to be used.
   /// </summary>
   public RectTransform Background;
   /// <summary>
   /// Final uniform rescale factor
   /// </summary>
   public float FinalUniformScale = 0.01f;
   /// <summary>
   /// Reference to team selection animator.
   /// </summary>
   public Animator SelectTeamAnimator;
   /// <summary>
   /// Material to change its texture
   /// </summary>
   public Material PlayersMaterial;
   /// <summary>
   /// Material to change its texture for leggings.
   /// </summary>
   public Material PlayersLegMaterial;
   /*/// <summary>
   /// MenuElement references to select from when hovered.
   /// </summary>
   public List<MenuElement> SelectElements = new List<MenuElement>();*/
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
   /// <summary>
   /// Assign method to be called when start button
   /// </summary>
   /// <param name="method"></param>
   public void SetStartCall(VoidNoParams method)
   {
      if (StartCall == null)
      {
         StartCall = method;
      }
      else
      {
         Debug.Log("<color=#8B2252FF>" + this.GetType().ToString() + ".cs - Warning: Parameter conflict." + " There is already a trigger event defined."
            + " </color>");
      }
   }
   /// <summary>
   /// Clear method to be called when start button triggerd.
   /// </summary>
   public void ClearStartCall()
   {
      StartCall = null;
   }
   /// <summary>
   /// 
   /// </summary>
   public override void Restart()
   {
      if (_lastSelected != null)
      {
         _lastSelected.Deselect();
      }
      _lastSelected = null;
      _selectedOption = -1;
   }
   #endregion  //End public methods

   //-----------------------------------------------------------//
   //                  MONOBEHAVIOUR METHODS                    //
   //-----------------------------------------------------------//
   #region Monobehaviour methods
   /// <summary>
   /// Unity Start() method
   /// </summary>
   void Start()
   {
      _initiated = PlayersLegMaterial != null && PlayersMaterial != null && SelectTeamAnimator != null &&  Background != null && ButtonPrefab != null && RestartButton != null && StartButton != null && SelectElements != null && SelectElements.Count > 0;
      if (_initiated)
      {
         SetStartCall(TriggerFadeOut);
         Background.SetParent(transform);
         Background.localPosition = Vector3.zero;
         Background.localScale = Vector3.one;
         RestartGame reference = GameObject.FindObjectOfType<RestartGame>();
         if (reference != null)
         {
            RestartButton.SetTriggerFunction(reference.Restart);
         }
         StartButton.SetTriggerFunction(StartButtonAction);
         int firstHalf = (SelectElements.Count / 2) + (SelectElements.Count % 2);
         for (int i = 0; i< SelectElements.Count; ++i)
         {
            SpriteTexturePair stp = SelectElements[i];
            GameObject prefIns = GameObject.Instantiate(ButtonPrefab);
            prefIns.name = "Button_" + stp.SpriteRef.texture.name;
            prefIns.transform.SetParent(Background);
            prefIns.transform.localPosition = Vector3.zero;
            MenuElement me = prefIns.GetComponentInChildren<MenuElement>();
            if (!_menuElements.Contains(me))
            {
               _menuElements.Add(me);
            }
            if (me != null)
            {
               me.gameObject.GetComponent<Image>().sprite = stp.SpriteRef;
               RectTransform rectTrans = me.gameObject.GetComponent<RectTransform>();
               if (rectTrans != null)
               {
                  rectTrans.anchorMax = rectTrans.anchorMin = rectTrans.pivot = Vector2.one * 0.5f;
                  rectTrans.sizeDelta = new Vector2(stp.SpriteRef.texture.width, stp.SpriteRef.texture.height);
                  rectTrans.localScale = Vector3.one;
                  RectTransform parentRectTrans = rectTrans.parent.gameObject.GetComponent<RectTransform>();
                  if (parentRectTrans != null)
                  {
                     parentRectTrans.anchorMax = parentRectTrans.anchorMin = parentRectTrans.pivot = Vector2.one * 0.5f;
                     Image parentImg = parentRectTrans.GetComponent<Image>();
                     if (parentImg != null) {
                        parentRectTrans.sizeDelta = new Vector2(parentImg.sprite.texture.width, parentImg.sprite.texture.height);
                        if (_menuElementSize == Vector2.zero)
                        {
                           _menuElementSize = parentRectTrans.sizeDelta;
                        }
                     }
                     parentRectTrans.localScale = Vector3.one;
                     prefIns.transform.localPosition = new Vector3(
                                                               ((i < firstHalf ? i - firstHalf * 0.5f : (i - firstHalf) - (SelectElements.Count - firstHalf) * 0.5f) + 0.5f) * -_menuElementSize.x,
                                                               (i < firstHalf ? 0.5f : -0.5f) * _menuElementSize.y,
                                                               0
                                                               );
                  }
                  BoxCollider box = rectTrans.transform.GetComponent<BoxCollider>();
                  if (box != null)
                  {
                     box.size = new Vector3(_menuElementSize.x, _menuElementSize.y, 0.05f);
                  }
               }
            }
            me.SetTriggerFunction(SelectButtonAction);
         }
         transform.localScale = Vector3.one * FinalUniformScale;
         _selectedOption = -1;
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined." + (StartButton == null ? " Start button \'MenuElement\' reference missing." : string.Empty) +
                   (RestartButton == null ? " Restart button \'MenuElement\' reference missing." : string.Empty) +
                   (ButtonPrefab == null ? " Menu element button prefab reference missing." : string.Empty) +
                   (Background == null ? " Menu background reference missing." : string.Empty) +
                   (SelectTeamAnimator == null ? " Menu animator reference missing." : string.Empty) +
                   (PlayersMaterial == null ? " Players material reference missing." : string.Empty) +
                   (PlayersLegMaterial == null ? " Players leggings material reference missing." : string.Empty) +
                   (SelectElements == null || SelectElements.Count == 0 ? " Selection elements \'MenuElement\' references missing." : string.Empty) +
                   " </color>");
      }
   }
   /// <summary>
   /// Unity Update() method
   /// </summary>
   void Update()
   {
   }
   #endregion  //End monobehaviour methods

   //-----------------------------------------------------------//
   //                      PRIVATE METHODS                      //
   //-----------------------------------------------------------//
   #region Private methods
   private void StartButtonAction(MenuElement invoker)
   {
      if (_initiated)
      {
         if (StartCall != null && _selectedOption != -1)
         {
            StartCall();
         }
      }
   }
   private void SelectButtonAction(MenuElement invoker)
   {
      invoker.transform.parent.parent.SetSiblingIndex(invoker.transform.parent.parent.parent.childCount - 1);
      if (_lastSelected != invoker)
      {
         invoker.Select();
         if (_lastSelected != null)
         {
            _lastSelected.Deselect();
         }
         _lastSelected = invoker;
      }
      _selectedOption = _menuElements.IndexOf(invoker);
      PlayersMaterial.SetTexture("_MainTex", SelectElements[_selectedOption].TextureRef);
      PlayersLegMaterial.SetTexture("_MainTex", SelectElements[_selectedOption].TextureLeg);
      //Debug.Log("<color=blue>Selected index: " + _selectedOption + "</color>");
   }
   private void TriggerFadeOut()
   {
      if (_initiated)
      {
         SelectTeamAnimator.SetBool("FadeOut", true);
      }
   }
   #endregion  //End private methods

   //-----------------------------------------------------------//
   //                      PRIVATE MEMBERS                      //
   //-----------------------------------------------------------//
   #region Private members
   private bool _initiated;
   private MenuElement _lastSelected = null;
   private int _selectedOption;
   private List<MenuElement> _menuElements = new List<MenuElement>();
   private Vector2 _menuElementSize = Vector2.zero;
   #endregion  //End private members
}
