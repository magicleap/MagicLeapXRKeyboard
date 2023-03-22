namespace MagicLeap.XRKeyboard.Extensions
{
    using UnityEngine;

    public static class ComponentExtensions
    {
        /// <summary>
        /// Searches for a component and adds it if not found
        /// </summary>
        public static T GetOrAddComponent<T,T2>(this T2 obj) where T : Component
                                                          where T2 : MonoBehaviour
        {
            // Attempt to get component from GameObject
            T retreivedComp = obj.GetComponent<T>();

            if (retreivedComp != null)
                return retreivedComp;

            // This component wasn't found on the object, so add it.
            return obj.gameObject.AddComponent<T>();
        }


        /// <summary>
        /// if cache is null, searches for a component and adds it if not found
        /// </summary>
        public static T GetOrAddCachedComponent<T, T2>(this T2 obj, ref T cache) where T : Component
                                                                                 where T2 : MonoBehaviour
        {
            if (cache == null)
            {
                // Attempt to get component from GameObject
                T retreivedComp = obj.GetComponent<T>();

                return retreivedComp != null ? retreivedComp : obj.gameObject.AddComponent<T>();

            }

            return cache;
        }

        /// <summary>
        /// gets component if cache is null
        /// </summary>
        public static T GetCachedComponent<T, T2>(this T2 obj, ref T cache) where T : Component
                                                                            where T2 : MonoBehaviour
        {
            return cache == null ? obj.GetComponent<T>() : cache;
        }
        /// <summary>
        /// Searches for a component and adds it if not found
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            // Attempt to get component from GameObject
            T retreivedComp = obj.GetComponent<T>();

            if (retreivedComp != null)
                return retreivedComp;

            // This component wasn't found on the object, so add it.
            return obj.AddComponent<T>();
        }

        /// <summary>
        /// if cache is null, searches for a component and adds it if not found
        /// </summary>
        public static T GetOrAddCachedComponent<T>(this GameObject obj, ref T cache) where T : Component
        {
            if (cache == null)
            {
                // Attempt to get component from GameObject
                T retreivedComp = obj.GetComponent<T>();

                return retreivedComp != null ? retreivedComp : obj.AddComponent<T>();

            }

            return cache;
        }

        /// <summary>
        /// gets component if cache is null
        /// </summary>
        public static T GetCachedComponent<T>(this GameObject obj, ref T cache) where T : Component
        {
            return cache == null ? obj.GetComponent<T>() : cache;
        }
        /// <summary>
        /// Searches for a component and adds it if not found
        /// </summary>
        public static T GetOrAddComponent<T>(this Transform obj) where T : Component
        {
            // Attempt to get component from GameObject
            T retreivedComp = obj.GetComponent<T>();

            if (retreivedComp != null)
                return retreivedComp;

            // This component wasn't found on the object, so add it.
            return obj.gameObject.AddComponent<T>();
        }


        /// <summary>
        /// if cache is null, searches for a component and adds it if not found
        /// </summary>
        public static T GetOrAddCachedComponent<T>(this Transform obj, ref T cache) where T : Component
        {
            if (cache == null)
            {
                // Attempt to get component from GameObject
                T retreivedComp = obj.GetComponent<T>();

                return retreivedComp != null ? retreivedComp : obj.gameObject.AddComponent<T>();

            }

            return cache;
        }

        /// <summary>
        /// gets component if cache is null
        /// </summary>
        public static T GetCachedComponent<T>(this Transform obj, ref T cache) where T : Component
        {
            return cache == null ? obj.GetComponent<T>() : cache;
        }

        /// <summary>
        /// Searches for a component in children (and self). Adds it to the root if not found
        /// </summary>
        public static T GetOrAddComponentInChildren<T, T2>(this T2 obj, bool includeInactive = false) where T : Component
                                                                                                      where T2 : MonoBehaviour
        {
            // Attempt to get component from GameObject
            T retreivedComp = obj.GetComponentInChildren<T>(includeInactive);

            if (retreivedComp != null)
                return retreivedComp;

            // This component wasn't found on the object, so add it.
            return obj.gameObject.AddComponent<T>();
        }

        /// <summary>
        /// if cache is null, Searches for a component in children (and self). Adds it to the root if not found
        /// </summary>
        public static T GetOrAddCachedComponentInChildren<T, T2>(this T2 obj, ref T cache, bool includeInactive = false) where T : Component
                                                                                                                                 where T2 : MonoBehaviour
        {
            if (cache == null)
            {
                // Attempt to get component from GameObject
                T retreivedComp = obj.GetComponentInChildren<T>(includeInactive);

                return retreivedComp != null ? retreivedComp : obj.gameObject.AddComponent<T>();

                // This component wasn't found on the object, so add it.
            }

            return cache;
        }

        /// <summary>
        /// gets component in children if cache is null
        /// </summary>
        public static T GetCachedComponentInChildren<T, T2>(this T2 obj, ref T cache, bool includeInactive = false) where T : Component
                                                                                                                            where T2 : MonoBehaviour
        {
            return cache == null ? obj.GetComponentInChildren<T>(includeInactive) : cache;
        }
        /// <summary>
        /// Searches for a component in children (and self). Adds it to the root if not found
        /// </summary>
        public static T GetOrAddComponentInChildren<T>(this GameObject obj, bool includeInactive = false) where T : Component
        {
            // Attempt to get component from GameObject
            T retreivedComp = obj.GetComponentInChildren<T>(includeInactive);

            if (retreivedComp != null)
                return retreivedComp;

            // This component wasn't found on the object, so add it.
            return obj.AddComponent<T>();
        }

        /// <summary>
        /// if cache is null, Searches for a component in children (and self). Adds it to the root if not found
        /// </summary>
        public static T GetOrAddCachedComponentInChildren<T>(this GameObject obj, ref T cache, bool includeInactive = false) where T : Component
        {
            if (cache == null)
            {
                // Attempt to get component from GameObject
                T retreivedComp = obj.GetComponentInChildren<T>(includeInactive);

                return retreivedComp != null ? retreivedComp : obj.gameObject.AddComponent<T>();

                // This component wasn't found on the object, so add it.
            }

            return cache;
        }

        /// <summary>
        /// gets component in children if cache is null
        /// </summary>
        public static T GetCachedComponentInChildren<T>(this GameObject obj, ref T cache, bool includeInactive = false) where T : Component
        {
            return cache == null ? obj.GetComponentInChildren<T>(includeInactive) : cache;
        }
        /// <summary>
        /// Searches for a component in children (and self). Adds it to the root if not found
        /// </summary>
        public static T GetOrAddComponentInChildren<T>(this Transform obj,bool includeInactive=false) where T : Component
        {
            // Attempt to get component from GameObject
            T retreivedComp = obj.GetComponentInChildren<T>(includeInactive);

            if (retreivedComp != null)
                return retreivedComp;

            // This component wasn't found on the object, so add it.
            return obj.gameObject.AddComponent<T>();
        }

        /// <summary>
        /// if cache is null, Searches for a component in children (and self). Adds it to the root if not found
        /// </summary>
        public static T GetOrAddCachedComponentInChildren<T>(this Transform obj, ref T cache, bool includeInactive = false) where T : Component
        {
            if (cache == null)
            {
                // Attempt to get component from GameObject
                T retreivedComp = obj.GetComponentInChildren<T>(includeInactive);

                return retreivedComp != null ? retreivedComp : obj.gameObject.AddComponent<T>();

                // This component wasn't found on the object, so add it.
            }

            return cache;
        }

        /// <summary>
        /// gets component in children if cache is null
        /// </summary>
        public static T GetCachedComponentInChildren<T>(this Transform obj, ref T cache, bool includeInactive = false) where T : Component
        {
            return cache == null ? obj.GetComponentInChildren<T>(includeInactive) : cache;
        }

        public static void MarkAsDirty(this Object o, string message)
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(o, message);
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(o);
#endif
        }
    }
}
