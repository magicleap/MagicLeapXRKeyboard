﻿using System;

namespace EasyButtons.Editor
{
    using UnityEditor;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Custom inspector for <see cref="UnityEngine.Object"/> including derived classes.
    /// </summary>
    [CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    internal class ObjectEditor : Editor
    {
        private ButtonsDrawer _buttonsDrawer;

        private void OnEnable()
        {
            _buttonsDrawer = new ButtonsDrawer(target);
        }

        public override void OnInspectorGUI()
        {
            if(_buttonsDrawer== null)
            {
                return;
            }

            try {
                DrawDefaultInspector();
                _buttonsDrawer.DrawButtons(targets);
            }
            catch
            {
                // ignored
            }
        }
    }
}
