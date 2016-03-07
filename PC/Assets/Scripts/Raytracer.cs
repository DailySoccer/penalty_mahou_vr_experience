using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Raytracer : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   /// <summary>
   /// Reference to the transform where the ray direction will be taken from.
   /// </summary>
   public Transform ForwardRef;
   /// <summary>
   /// Area width the ray will be cast to.
   /// </summary>
   public float WidthArea = 10;
   /// <summary>
   /// Area height the ray will be cast to.
   /// </summary>
   public float HeightArea = 10;
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
   public Vector3 RaycastPoint()
   {
      Vector3 solution;
      if (!_initiated)
      {
         solution = Vector3.zero;
      }
      else
      {
         Vector3 relPos = ForwardRef.position - transform.position;
         float normalDis = Vector3.Dot(relPos, transform.forward);
         float cosNorSol = Vector3.Dot(ForwardRef.forward, transform.forward);
         if (cosNorSol != 0)
         {//There is intersection
            float solModule = -normalDis / cosNorSol;
            solution = ForwardRef.transform.position + ForwardRef.forward * solModule;
         }
         else
         {//Look direction is parallel to plane
            solution = Vector3.zero;
         }
      }
      return solution;
   }
   #endregion  //End public methods

   //-----------------------------------------------------------//
   //                  MONOBEHAVIOUR METHODS                    //
   //-----------------------------------------------------------//
   #region Monobehaviour methods

   // Use this for initialization
   /// <summary>
   /// Unity Start() method.
   /// </summary>
   void Start()
   {
      _initiated = ForwardRef != null;
      if (_initiated)
      {
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined." + (ForwardRef == null ? " Camera reference missing." : string.Empty) +
                   " </color>");
      }
   }
   
   void Update()
   {
      if (_initiated)
      {
      }
   }

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
   #endregion  //End private members
}
