using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BoxColliderScaleFixerWindow : EditorWindow
{
	private GameObject previousRoot;
	private GameObject currentRoot;

	private Vector2 scrollPos;

	private class BoxColliderInfo
	{
		public Transform transform;
		public Vector3 originalScale;
		public bool originalFlipX;
		public bool originalFlipY;
		public bool isFixed = false;
		public Vector3 originalOffset;
	}


	private List<BoxColliderInfo> boxColliderInfos = new List<BoxColliderInfo>();

	[MenuItem("TA/Playable ADS/Box Collider Scale Fixer")]
	public static void ShowWindow()
	{
		GetWindow<BoxColliderScaleFixerWindow>("Box Collider Scale Fixer");
	}

	void OnGUI()
	{
		EditorGUI.BeginChangeCheck();
		currentRoot = (GameObject)EditorGUILayout.ObjectField("Root GameObject", currentRoot, typeof(GameObject), true);
		if (EditorGUI.EndChangeCheck())
		{
			ScanForBadBoxColliders();
		}

		EditorGUILayout.Space();

		if (boxColliderInfos.Count > 0)
		{
			EditorGUILayout.LabelField($"Found {boxColliderInfos.Count} collider(s) with negative scale:", EditorStyles.boldLabel);

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			foreach (var info in boxColliderInfos)
			{
				EditorGUILayout.BeginHorizontal();

				GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
				{
					normal = { textColor = Color.cyan }
				};

				if (GUILayout.Button(info.transform.name, labelStyle, GUILayout.Width(200)))
				{
					EditorGUIUtility.PingObject(info.transform.gameObject);
					Selection.activeGameObject = info.transform.gameObject;
				}

				string buttonText = info.isFixed ? "Undo it" : "Fix it";
				if (GUILayout.Button(buttonText, GUILayout.Width(70)))
				{
					ToggleFix(info);
					UpdateFixAllState();
				}

				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndScrollView();

			EditorGUILayout.Space();

			string fixAllText = AllFixed() ? "Undo All" : "Fix All";
			if (GUILayout.Button(fixAllText))
			{
				bool toFix = !AllFixed();
				foreach (var info in boxColliderInfos)
				{
					if (info.isFixed != toFix)
						ToggleFix(info);
				}
			}
		}
		else if (currentRoot != null)
		{
			EditorGUILayout.LabelField("No colliders with negative scale found.");
		}
	}

	private void ScanForBadBoxColliders()
	{
		boxColliderInfos.Clear();

		if (currentRoot == null)
			return;

		BoxCollider[] colliders = currentRoot.GetComponentsInChildren<BoxCollider>(true);

		foreach (var col in colliders)
		{
			Vector3 scale = col.transform.localScale;
			if (scale.x < 0 || scale.y < 0 || scale.z < 0)
			{
				SpriteRenderer sr = col.GetComponent<SpriteRenderer>();
				boxColliderInfos.Add(new BoxColliderInfo
				{
					transform = col.transform,
					originalScale = col.transform.localScale,
					originalOffset = col.center,
					originalFlipX = sr != null ? sr.flipX : false,
					originalFlipY = sr != null ? sr.flipY : false,
					isFixed = false
				});
			}
		}

		previousRoot = currentRoot;
	}

	private void ToggleFix(BoxColliderInfo info)
	{
		Undo.RecordObject(info.transform, info.isFixed ? "Undo Collider Fix" : "Fix Collider Scale");

		SpriteRenderer sr = info.transform.GetComponent<SpriteRenderer>();
		if (sr != null)
		{
			Undo.RecordObject(sr, info.isFixed ? "Undo SpriteRenderer Flip" : "Fix SpriteRenderer Flip");
		}

		BoxCollider bc = info.transform.GetComponent<BoxCollider>();
		if (bc != null)
		{
			Undo.RecordObject(bc, info.isFixed ? "Undo BoxCollider Center" : "Fix BoxCollider Center");
		}

		if (info.isFixed)
		{
			info.transform.localScale = info.originalScale;

			if (sr != null)
			{
				sr.flipX = info.originalFlipX;
				sr.flipY = info.originalFlipY;
			}

			if (bc != null)
			{
				bc.center = info.originalOffset;
			}

			info.isFixed = false;
		}
		else
		{
			info.originalScale = info.transform.localScale;

			Vector3 newScale = new Vector3(
				Mathf.Abs(info.transform.localScale.x),
				Mathf.Abs(info.transform.localScale.y),
				Mathf.Abs(info.transform.localScale.z)
			);

			if (sr != null)
			{
				info.originalFlipX = sr.flipX;
				info.originalFlipY = sr.flipY;

				if (info.transform.localScale.x < 0)
					sr.flipX = !sr.flipX;

				if (info.transform.localScale.y < 0)
					sr.flipY = !sr.flipY;
			}

			if (bc != null)
			{
				if (info.transform.localScale.x < 0)
					bc.center = new Vector3(-bc.center.x, bc.center.y, bc.center.z);

				if (info.transform.localScale.y < 0)
					bc.center = new Vector3(bc.center.x, -bc.center.y, bc.center.z);
			}

			info.transform.localScale = newScale;
			info.isFixed = true;
		}

		EditorUtility.SetDirty(info.transform);
		if (sr != null) EditorUtility.SetDirty(sr);
		if (bc != null) EditorUtility.SetDirty(bc);
	}

	private bool AllFixed()
	{
		foreach (var info in boxColliderInfos)
		{
			if (!info.isFixed)
				return false;
		}
		return true;
	}

	private void UpdateFixAllState()
	{
		Repaint();
	}
}
