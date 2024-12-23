using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityShortcutsEz
{
	internal static class UnityShortcutsEz
	{
		[Shortcut("Toggle Debug Mode Inspector", KeyCode.D, ShortcutModifiers.Control | ShortcutModifiers.Alt)]
		private static void ToggleDebugModeInspector()
		{
			Debug.Log($"Toggle inspector debug (Ctrl+Alt+D)");
			
			var unityEditorAssembly = typeof(Editor).Assembly;
			var propertyEditorType = unityEditorAssembly.GetType("UnityEditor.PropertyEditor");
			var instance = EditorWindow.GetWindow(propertyEditorType);
			var propInfo = propertyEditorType.GetProperty("inspectorMode", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var current = (InspectorMode)propInfo.GetValue(instance);
			propInfo.SetValue(instance, current == InspectorMode.Normal ? InspectorMode.Debug : InspectorMode.Normal);
		}
		
		[Shortcut("Discard Scene Changes", KeyCode.D, ShortcutModifiers.Control | ShortcutModifiers.Shift)]
		private static void DiscardSceneChanges()
		{
			Debug.Log($"Discarded changes on scenes (Ctrl+Shift+D)");

			MethodInfo method = typeof(EditorSceneManager).GetMethod("ReloadScene", BindingFlags.NonPublic | BindingFlags.Static);

			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				object scene = SceneManager.GetSceneAt(i);
				method?.Invoke(null, new[] { scene });
			}
		}
		
		[Shortcut("Toggle Lock Inspector", KeyCode.Q, ShortcutModifiers.Control)]
		public static void ToggleLockInspector()
		{
			Debug.Log($"Toggled lock inspector (Ctrl+Q)");
			
			ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
			ActiveEditorTracker.sharedTracker.ForceRebuild();
		}
		
		[Shortcut("Clear Unity Console", KeyCode.X)]
		static void ClearUnityConsole()
		{
			var assembly = Assembly.GetAssembly(typeof(SceneView));
			Type type = assembly.GetType("UnityEditor.LogEntries");
			MethodInfo method = type.GetMethod("Clear");
			method.Invoke(new object(), null);
		}
		
		[Shortcut("Fold Inspector", KeyCode.I, ShortcutModifiers.Control | ShortcutModifiers.Shift)]
		public static void FoldInspector()
		{
			Debug.Log($"Folded Inspector (Ctrl+Shift+I)");
			
			Component[] components = Selection.activeGameObject.GetComponents<Component>();
			foreach (Component obj in components)
				UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(obj, false);

			GameObject go = Selection.activeGameObject;
			Selection.activeGameObject = null;
			Selection.activeGameObject = go;
		}
		
		[MenuItem("Tools/Regenerate Rider Project Files")]
		[Shortcut("Regenerate Proj Files Rider", KeyCode.R, ShortcutModifiers.Control | ShortcutModifiers.Alt)]
		private static void RegenerateProjFilesRider()
		{
			Debug.Log($"Regenerating csproj Files Rider (Ctrl+Alt+R)");
			
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (Assembly assembly in assemblies)
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (type.Name != "RiderScriptEditor") continue;

					FieldInfo field = type.GetField("m_ProjectGeneration",
						BindingFlags.Static | BindingFlags.NonPublic);
					object obj = field.GetValue(null);
					MethodInfo meth = obj.GetType().GetMethod("Sync");
					meth.Invoke(obj, null);
				}
			}
		}

		[Shortcut("Show Project Settings", KeyCode.S, ShortcutModifiers.Alt)]
		public static void ShowProjectSettings()
		{
			Debug.Log($"Show Project Settings (Alt+S)");
			SettingsService.OpenProjectSettings("Project/Player");
		}
		
		[Shortcut("Show Preferences", KeyCode.P, ShortcutModifiers.Alt)]
		public static void ShowPreferences()
		{
			Debug.Log($"Show Prefs (Alt+P)");
			EditorApplication.ExecuteMenuItem("Edit/Preferences...");
		}
	}
}