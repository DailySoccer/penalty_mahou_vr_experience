using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods
{
	public static class ExtensionMethods
	{		
		public static void ForEachWithIndex<T>(this IEnumerable<T> ie, Action<T, int> action)
		{
		    var i = 0;
		    foreach (var e in ie) 
				action( e, i++ );
		}
		
		public static string FormatAsMoney(this int num)
		{
			if (num == 0)
				return "0";
			
		    return num.ToString("#,#");
		}
		
		
		///<summary>Finds the index of the first occurence of an item in an enumerable.</summary>
	    ///<param name="items">The enumerable to search.</param>
	    ///<param name="item">The item to find.</param>
	    ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
	    public static int IndexOf<T>(this IEnumerable<T> items, T item)
	    {
	        return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
	    }
	
	    ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
	    ///<param name="items">The enumerable to search.</param>
	    ///<param name="predicate">The expression to test the items against.</param>
	    ///<returns>The index of the first matching item, or -1 if no items match.</returns>
	    public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
	    {
	        if (items == null) throw new ArgumentNullException("items");
	        if (predicate == null) throw new ArgumentNullException("predicate");
	
	        int retVal = 0;
	        foreach (var item in items)
	        {
	            if (predicate(item)) return retVal;
	            retVal++;
	        }
	        return -1;
	    }
		
		public static Quaternion SmoothDamp(this Vector3 current, Vector3 target, ref Vector3 velocity, float time)
		{
			Vector3 result = Vector3.zero;
			result.x = Mathf.SmoothDampAngle(current.x, target.x, ref velocity.x, time);
			result.y = Mathf.SmoothDampAngle(current.y, target.y, ref velocity.y, time);
			result.z = Mathf.SmoothDampAngle(current.z, target.z, ref velocity.z, time);
			return Quaternion.Euler(result);
		}
	}
}

