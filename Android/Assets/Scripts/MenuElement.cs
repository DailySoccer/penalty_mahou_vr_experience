using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuElement : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   public delegate void Void1MenuElement(MenuElement caller);
   /// <summary>
   /// Time for the element to be triggered.
   /// </summary>
   [Range(0.01f,10)]
   public float LoadTime = 2;
   /// <summary>
   /// Time for the element to discharge trigger function.
   /// </summary>
   [Range(0.01f, 10)]
   public float UnloadTime = 1;
   /// <summary>
   /// Reference to image to be used as loading graphic.
   /// </summary>
   public Image LoadImage;
   /// <summary>
   /// State if this element can be selected.
   /// </summary>
   public bool Selection;
   /// <summary>
   /// Time for selection animation.
   /// </summary>
   [Range(0.01f, 3)]
   public float SelectionAnimTime = 1;
   [System.Serializable]
   public class SelectedState
   {
      /// <summary>
      /// Size for selected state
      /// </summary>
      public Vector3 SelectedScale = Vector3.one * 1.2f;
      /// <summary>
      /// New local position when selected
      /// </summary>
      public Vector3 SelectedPosition = Vector3.zero;
      /// <summary>
      /// New local rotation when selected
      /// </summary>
      public Vector3 SelectedRotation = Vector3.zero;
   }
   /// <summary>
   /// Parameters for selected
   /// </summary>
   public SelectedState SelectedParams;
   /// <summary>
   /// Transform to be modified by selected parameters.
   /// </summary>
   public Transform SelectedTarget;
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
   /// <summary>
   /// Function to select this element.
   /// </summary>
   public void Select()
   {
      if (Selection)
      {
         if (!_isSelected)
         {
            SetSelection(true);
         }
      }
   }
   /// <summary>
   /// Function to deselect this element.
   /// </summary>
   public void Deselect()
   {
      if (Selection)
      {
         if (_isSelected)
         {
            SetSelection(false);
         }
      }
   }
   /// <summary>
   /// Set a trigger function.
   /// </summary>
   /// <param name="triggerFunction">Function to be triggerd.</param>
   public void SetTriggerFunction(Void1MenuElement triggerFunction)
   {
      if (TriggerEvent == null)
      {
         TriggerEvent += triggerFunction;
      }
      else
      {
         Debug.Log("<color=#8B2252FF>" + this.GetType().ToString() + ".cs - Warning: Parameter conflict." + " There is already a trigger event defined."
            + " </color>");
      }
   }
   /// <summary>
   /// Unloads trigger function for this objejct.
   /// </summary>
   public void UnloadTriggerFunction()
   {
      TriggerEvent = null;
   }
   /// <summary>
   /// Call this function to start hover event.
   /// </summary>
   public void Hover()
   {
      if (!_onHover)
      {
         SetState(true);
      }
   }
   /// <summary>
   /// Call this function to end hover event.
   /// </summary>
   public void Unhover()
   {
      if (_onHover)
      {
         SetState(false);
      }
   }
   #endregion  //End public methods

   //-----------------------------------------------------------//
   //                  MONOBEHAVIOUR METHODS                    //
   //-----------------------------------------------------------//
   #region Monobehaviour methods
   void OnDestroy()
   {
      UnloadTriggerFunction();
   }
   /// <summary>
   /// Unity Start() method.
   /// </summary>
   void Start() {
      _initiated = LoadImage != null && (!Selection || SelectedTarget != null);
      if (_initiated)
      {
         RestartOptions();
         if (Selection)
         {
            _INIT.SelectedPosition = SelectedTarget.localPosition;
            _INIT.SelectedRotation = SelectedTarget.localEulerAngles;
            _INIT.SelectedScale = SelectedTarget.localScale;
         }
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined." + (LoadImage == null ? " Load image reference missing." : string.Empty)
                   + (SelectedTarget == null ? " Transform reference to modify by selection missing.": string.Empty)
                   + " </color>");
      }
   }
   /// <summary>
   /// Unity Update() method.
   /// </summary>
   void Update()
   {
      if (_initiated)
      {
         float now = Time.time;
         if (_workToDo) {
            if (_onHover)
            {
               _loadPerc = (now - _startTime) / LoadTime;
            }
            else
            {
               _loadPerc = 1 - ((now - _startTime) / UnloadTime);
            }
            _loadPerc = Mathf.Clamp01(_loadPerc);
            LoadImage.fillAmount = _loadPerc;
            _workToDo = _onHover && _loadPerc < 1 || !_onHover && _loadPerc > 0;
            if (_onHover && _loadPerc >= 1)
            {
               CallTriggerFunction();
            }
         }
         if (Selection && _animToDo)
         {
            float perc = (now - _startTimeAnim) / SelectionAnimTime;
            _animPerc = Mathf.Clamp01(_isSelected ? perc : 1 - perc);
            SelectedTarget.localPosition = Vector3.Slerp(_INIT.SelectedPosition, SelectedParams.SelectedPosition, _animPerc);
            SelectedTarget.localScale = Vector3.Slerp(_INIT.SelectedScale, SelectedParams.SelectedScale, _animPerc);
            SelectedTarget.localRotation = Quaternion.Slerp(Quaternion.Euler(_INIT.SelectedRotation), Quaternion.Euler(SelectedParams.SelectedRotation), _animPerc);
            _animToDo = perc < 1;
         }
      }
   }
   #endregion  //End monobehaviour methods

   //-----------------------------------------------------------//
   //                      PRIVATE METHODS                      //
   //-----------------------------------------------------------//
   #region Private methods
   private void RestartOptions()
   {
      LoadImage.fillAmount = _loadPerc = 0;
      _onHover = false;
      _workToDo = false;
   }
   private void SetState(bool onHover)
   {
      _workToDo = true;
      _onHover = onHover;
      _startTime = Time.time - (_onHover ? LoadTime * _loadPerc : UnloadTime * (1 - _loadPerc));
   }
   private void CallTriggerFunction()
   {
      if (_initiated)
      {
         _workToDo = false;
         if (TriggerEvent != null)
         {
            TriggerEvent(this);
         }
      }
   }
   private void SetSelection(bool isSelected)
   {
      _animToDo = true;
      _isSelected = isSelected;
      _startTimeAnim = Time.time - (_isSelected ? _animPerc : 1 - _animPerc) * SelectionAnimTime;
   }
   #endregion  //End private methods

   //-----------------------------------------------------------//
   //                      PRIVATE MEMBERS                      //
   //-----------------------------------------------------------//
   #region Private members
   private bool _initiated;
   private float _startTime;
   private float _startTimeAnim;
   private bool _onHover;
   private float _loadPerc;
   private event Void1MenuElement TriggerEvent = null;
   private bool _workToDo;
   private bool _animToDo;
   private SelectedState _INIT = new SelectedState();
   private bool _isSelected;
   private float _animPerc;
   #endregion  //End private members
}
