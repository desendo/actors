﻿#if!ODIN_INSPECTOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pixeye
{
	[CustomEditor(typeof(Object), true, isFallback = true)]
	[CanEditMultipleObjects]
	public class EditorOverride : ReorderableArrayInspector
	{

		Dictionary<string, CacheFoldProp> cacheFolds = new Dictionary<string, CacheFoldProp>();
		List<SerializedProperty> props = new List<SerializedProperty>();
		List<MethodInfo> methods;
		bool initialized;

		void OnEnable()
		{
			initialized = false;
 
	 	}
 
		void OnDisable()
		{
			if (Toolbox.isQuittingOrChangingScene()) return;
			if (target != null)
				foreach ( var c in cacheFolds )
				{
					EditorPrefs.SetBool(string.Format($"{c.Value.atr.name}{c.Value.props[0].name}{target.name}"), c.Value.expanded);
					c.Value.Dispose();
				}
		}
 
		
		
	 
		
		
		public override bool RequiresConstantRepaint()
		{
			return EditorActorsFramework.needToRepaint;
		}

		public override void OnInspectorGUI()
		{
			 
			serializedObject.Update();

	 
			
			Setup();

			if (props.Count == 0)
			{
				DrawDefaultInspector();
				return;
			}

			Header();
			Body();

			serializedObject.ApplyModifiedProperties();

			void Header()
			{
				using (new EditorGUI.DisabledScope("m_Script" == props[0].propertyPath))
				{
					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(props[0], true);
					EditorGUILayout.Space();
				}
			}

			void Body()
			{
				foreach ( var pair in cacheFolds )
				{
					this.UseVerticalLayout(() => Foldout(pair.Value), EditorUIStyles.box);
					EditorGUI.indentLevel = 0;
				}

				EditorGUILayout.Space();

				for ( var i = 1; i < props.Count; i++ )
				{
					
					
					if (props[i].isArray)
					{
						DrawPropertySortableArray(props[i]);
					}
					else
					{
						EditorGUILayout.PropertyField(props[i], true);
					}
					
					
			 
				}

				EditorGUILayout.Space();

				if (methods == null) return;
				foreach ( MethodInfo memberInfo in methods )
				{
					this.UseButton(memberInfo);
				}
			}

			void Foldout(CacheFoldProp cache)
			{
				cache.expanded = EditorGUILayout.Foldout(cache.expanded, cache.atr.name, true,
						EditorUIStyles.foldout);

				if (cache.expanded)
				{
					EditorGUI.indentLevel = 1;

					for ( int i = 0; i < cache.props.Count; i++ )
					{
						this.UseVerticalLayout(() => Child(i), EditorUIStyles.boxChild);
					}
				}

				void Child(int i)
				{

					if (cache.props[i].isArray)
					{
						DrawPropertySortableArray(cache.props[i]);
					}
					else
					{
						EditorGUILayout.PropertyField(cache.props[i], new GUIContent(cache.props[i].name.FirstLetterToUpperCase()), true);
					}
 
				}
			}

			void Setup()
			{
				EditorActorsFramework.currentEvent = Event.current;
				if (!initialized)
				{
					InitInspector();
					SetupButtons();

					List<FieldInfo>       objectFields;
			  		FoldoutGroupAttribute prevFold = default;

					var length = EditorTypes.Get(target, out objectFields);

					for ( var i = 0; i < length; i++ )
					{
						#region FOLDERS

						var           fold = Attribute.GetCustomAttribute(objectFields[i], typeof(FoldoutGroupAttribute)) as FoldoutGroupAttribute;
						CacheFoldProp c;
						if (fold == null)
						{
							if (prevFold != null && prevFold.foldEverything)
							{
								if (!cacheFolds.TryGetValue(prevFold.name, out c))
								{
									cacheFolds.Add(prevFold.name, new CacheFoldProp {atr = prevFold, types = new HashSet<string> {objectFields[i].Name}});
								}
								else
								{
									c.types.Add(objectFields[i].Name);
								}
							}

							continue;
						}

						prevFold = fold;

						if (!cacheFolds.TryGetValue(fold.name, out c))
						{
							var expanded = EditorPrefs.GetBool(string.Format($"{fold.name}{objectFields[i].Name}{target.name}"), false);
							cacheFolds.Add(fold.name, new CacheFoldProp {atr = fold, types = new HashSet<string> {objectFields[i].Name}, expanded = expanded});
						}
						else c.types.Add(objectFields[i].Name);

						#endregion
					}

					var property = serializedObject.GetIterator();
					var next     = property.NextVisible(true);
					if (next)
					{
						do
						{
							HandleFoldProp(property);
						} while (property.NextVisible(false));
					}

					initialized = true;
				}
			}

			void SetupButtons()
			{
				var members = GetButtonMembers(target);

				foreach ( var memberInfo in members )
				{
					var method = memberInfo as MethodInfo;
					if (method == null)
					{
						continue;
					}

					if (method.GetParameters().Length > 0)
					{
						continue;
					}

					if (methods == null) methods = new List<MethodInfo>();
					methods.Add(method);
				}
			}
		}

		public void HandleFoldProp(SerializedProperty prop)
		{
			bool shouldBeFolded = false;

			foreach ( var pair in cacheFolds )
			{
				if (pair.Value.types.Contains(prop.name))
				{
					var pr = prop.Copy();
					shouldBeFolded = true;
					pair.Value.props.Add(pr);

					break;
				}
			}

			if (shouldBeFolded == false)
			{
				var pr = prop.Copy();
				props.Add(pr);
			}
		}

		IEnumerable<MemberInfo> GetButtonMembers(object target)
		{
			return target.GetType()
					.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
					.Where(CheckButtonAttribute);
		}

		bool CheckButtonAttribute(MemberInfo memberInfo)
		{
			return Attribute.IsDefined(memberInfo, typeof(ButtonAttribute));
		}

		class CacheFoldProp
		{

			public HashSet<string> types = new HashSet<string>();
			public List<SerializedProperty> props = new List<SerializedProperty>();
			public FoldoutGroupAttribute atr;
			public bool expanded;

			public void Dispose()
			{
				props.Clear();
				types.Clear();
				atr = null;
			}

		}

	}
}
#endif