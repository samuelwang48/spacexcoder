
#if UNITY_EDITOR // exclude from build

using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

using Clayxels;

namespace Clayxels{
	[CustomEditor(typeof(ClayContainer))]
	public class ClayxelInspector : Editor{
		static bool extrasPanel = false;
		
		public override void OnInspectorGUI(){
			Color defaultColor = GUI.backgroundColor;

			ClayContainer clayxel = (ClayContainer)this.target;

			EditorGUILayout.LabelField("Clayxels V1.31");

			EditorGUILayout.Space();

			string userWarn = clayxel._getUserWarning();
			if(userWarn != ""){
				GUIStyle s = new GUIStyle();
				s.wordWrap = true;
				s.normal.textColor = Color.yellow;
				EditorGUILayout.LabelField(userWarn, s);
			}

			if(clayxel.getNumSolids() > clayxel.getMaxSolids()){
				GUIStyle s = new GUIStyle();
				s.wordWrap = true;
				s.normal.textColor = Color.yellow;
				EditorGUILayout.LabelField("Max solid count exeeded, open Global Config to tweak settings.");
			}

			if(clayxel.instanceOf != null){
				ClayContainer newInstance = (ClayContainer)EditorGUILayout.ObjectField(new GUIContent("instance", "Set this to point at another clayContainer in scene to make this into an instance and avoid having to compute the same thing twice."), clayxel.instanceOf, typeof(ClayContainer), true);
			
				if(newInstance != clayxel.instanceOf && newInstance != clayxel){
					clayxel.instanceOf = newInstance;
					clayxel.init();

					// UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
					// ClayContainer.getSceneView().Repaint();

					if(!Application.isPlaying){
						EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
					}
				}

				EditorGUILayout.Space();
				if(GUILayout.Button((new GUIContent("global config", "")))){
					ClayxelsPrefsWindow.Open();
				}

				return;
			}

			if(clayxel.isFrozen()){
				if(GUILayout.Button(new GUIContent("defrost clayxels", "Back to live clayxels."))){
					clayxel.defrostContainersHierarchy();
					// UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
					// ClayContainer.getSceneView().Repaint();
				}

				EditorGUILayout.Space();
				if(GUILayout.Button((new GUIContent("global config", "")))){
					ClayxelsPrefsWindow.Open();
				}

				return;
			}

			EditorGUI.BeginChangeCheck();

			int clayxelDetail = EditorGUILayout.IntField(new GUIContent("clayxel detail", "How coarse or finely detailed is your sculpt. Enable Gizmos in your viewport to see the boundaries."), clayxel.getClayxelDetail());
			
			if(EditorGUI.EndChangeCheck()){
				ClayContainer._inspectorUpdate();

				clayxel.setClayxelDetail(clayxelDetail);
				
				if(!Application.isPlaying){
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				}

				return;
			}

			GUILayout.BeginHorizontal();

			GUI.backgroundColor = defaultColor;

			if(!clayxel.isAutoBoundsActive()){
				EditorGUI.BeginChangeCheck();
				Vector3Int boundsScale = EditorGUILayout.Vector3IntField(new GUIContent("bounds scale", "How much work area you have for your sculpt within this container. Enable Gizmos in your viewport to see the boundaries."), clayxel.getBoundsScale());
				
				if(EditorGUI.EndChangeCheck()){
					ClayContainer._inspectorUpdate();

					clayxel.setBoundsScale(boundsScale.x, boundsScale.y, boundsScale.z);

					clayxel.init();
					clayxel.needsUpdate = true;
					// UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
					// ClayContainer.getSceneView().Repaint();

					if(!Application.isPlaying){
						EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
					}

					return;
				}

				if(GUILayout.Button(new GUIContent("-", ""))){
					ClayContainer._inspectorUpdate();

					Vector3Int bounds = clayxel.getBoundsScale();
					clayxel.setBoundsScale(bounds.x - 1, bounds.y - 1, bounds.z - 1);

					clayxel.init();
					clayxel.needsUpdate = true;
					// UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
					// ClayContainer.getSceneView().Repaint();

					if(!Application.isPlaying){
						EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
					}

					return;
				}

				if(GUILayout.Button(new GUIContent("+", ""))){
					ClayContainer._inspectorUpdate();

					Vector3Int bounds = clayxel.getBoundsScale();
					clayxel.setBoundsScale(bounds.x + 1, bounds.y + 1, bounds.z + 1);

					clayxel.init();
					clayxel.needsUpdate = true;
					// UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
					// ClayContainer.getSceneView().Repaint();

					if(!Application.isPlaying){
						EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
					}

					return;
				}

				if(GUILayout.Button(new GUIContent("auto", ""))){
					clayxel.setAutoBoundsActive(true);

					// UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
					// ClayContainer.getSceneView().Repaint();

					if(!Application.isPlaying){
						EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
					}
				}
			}
			else{
				GUI.backgroundColor = Color.yellow;

				GUILayout.BeginHorizontal();
				
				EditorGUILayout.LabelField("bounds scale");

				if(GUILayout.Button(new GUIContent("auto", ""))){
					clayxel.setAutoBoundsActive(false);
				}

				GUILayout.EndHorizontal();
			}

			GUI.backgroundColor = defaultColor;

			GUILayout.EndHorizontal();

			EditorGUILayout.Space();

			if(GUILayout.Button(new GUIContent("add clay", "lets get this party started"))){
				ClayObject clayObj = ((ClayContainer)this.target).addClayObject();

				Undo.RegisterCreatedObjectUndo(clayObj.gameObject, "added clayxel solid");
				UnityEditor.Selection.objects = new GameObject[]{clayObj.gameObject};

				clayxel.needsUpdate = true;
				// UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
				// ClayContainer.getSceneView().Repaint();

				if(!Application.isPlaying){
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				}

				return;
			}

			if(GUILayout.Button(new GUIContent("pick clay ("+ClayContainer.pickingKey+")", "Press p on your keyboard to mouse pick ClayObjects from the viewport. Pressing Shift will add to a previous selection."))){
				ClayContainer.startScenePicking();
			}

			if(GUILayout.Button((new GUIContent("global config", "")))){
				ClayxelsPrefsWindow.Open();
			}

			clayxel.forceUpdate = EditorGUILayout.Toggle(new GUIContent("animate (forceUpdate)", "Enable if you're animating/moving the container as well as the clayObjects inside it."), clayxel.forceUpdate);

			EditorGUILayout.Space();

			ClayxelInspector.extrasPanel = EditorGUILayout.Foldout(ClayxelInspector.extrasPanel, "extras", true);

			if(ClayxelInspector.extrasPanel){
				ClayContainer instance = (ClayContainer)EditorGUILayout.ObjectField(new GUIContent("instance", "Set this to point at another clayContainer in scene to make this into an instance and avoid having to compute the same thing twice."), clayxel.instanceOf, typeof(ClayContainer), true);
				if(instance != clayxel.instanceOf && instance != clayxel){
					clayxel.instanceOf = instance;

					// UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
					// ClayContainer.getSceneView().Repaint();

					if(!Application.isPlaying){
						EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
					}
				}

				EditorGUILayout.Space();

				if(clayxel.storeAssetPath == ""){
					clayxel.storeAssetPath = clayxel.gameObject.name;
				}
				clayxel.storeAssetPath = EditorGUILayout.TextField(new GUIContent("frozen asset name", "Specify an asset name to store frozen mesh or claymation on disk. Files are saved relative to this project's Assets folder."), clayxel.storeAssetPath);
				string[] paths = clayxel.storeAssetPath.Split('.');
				if(paths.Length > 0){
					clayxel.storeAssetPath = paths[0];
				}

				EditorGUILayout.Space();

				#if CLAYXELS_RETOPO
				clayxel.shouldRetopoMesh = EditorGUILayout.Toggle(new GUIContent("retopology", "Use this to generate meshes with a better topology."), clayxel.shouldRetopoMesh);
				if(clayxel.shouldRetopoMesh){
					clayxel.retopoMaxVerts = EditorGUILayout.IntField(new GUIContent("vertex count", "-1 will let the tool decide on the best number of vertices."), clayxel.retopoMaxVerts);
				}
				#endif

				if(GUILayout.Button(new GUIContent("freeze mesh", "Switch between live clayxels and a frozen mesh."))){
					clayxel.freezeContainersHierarchyToMesh();

					#if CLAYXELS_RETOPO
						if(clayxel.shouldRetopoMesh){
							int targetVertCount = RetopoUtils.getRetopoTargetVertsCount(clayxel.gameObject, clayxel.retopoMaxVerts);
							if(targetVertCount == 0){
								return;
							}

							RetopoUtils.retopoMesh(clayxel.gameObject.GetComponent<MeshFilter>().sharedMesh, targetVertCount, -1);
						}
					#endif
					
					clayxel.transferMaterialPropertiesToMesh();

					if(clayxel.storeAssetPath != ""){
						clayxel.storeMesh(clayxel.storeAssetPath);
					}
				}

				EditorGUILayout.Space();

				AnimationClip claymationAnimClip = (AnimationClip)EditorGUILayout.ObjectField(new GUIContent("animClip (optional)", "Freeze an animation to disk using the claymation file format"), clayxel.claymationAnimClip, typeof(AnimationClip), true);
				if(claymationAnimClip != null && claymationAnimClip != clayxel.claymationAnimClip){
					clayxel.claymationStartFrame = 0;
					clayxel.claymationEndFrame = (int)(claymationAnimClip.length * claymationAnimClip.frameRate);
				}
				clayxel.claymationAnimClip = claymationAnimClip;

				if(clayxel.claymationAnimClip != null){
					clayxel.claymationStartFrame = EditorGUILayout.IntField(new GUIContent("start", ""), clayxel.claymationStartFrame);
					clayxel.claymationEndFrame = EditorGUILayout.IntField(new GUIContent("end", ""), clayxel.claymationEndFrame);
				}

				if(GUILayout.Button(new GUIContent("freeze claymation", "Freeze this container to a point-cloud file stored on disk and skip heavy computing."))){
					clayxel.freezeClaymation();
				}
				
				EditorGUI.BeginChangeCheck();

				EditorGUILayout.Space();

				bool castShadows = EditorGUILayout.Toggle("cast shadows", clayxel.getCastShadows());

				if(EditorGUI.EndChangeCheck()){
					ClayContainer._inspectorUpdate();

					clayxel.setCastShadows(castShadows);

					if(!Application.isPlaying){
						EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
					}
				}

				// end of extras
			}

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.Space();

			Material customMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("customMaterial", "Custom materials need to use shaders specifically made for clayxels. Use the provided shaders and examples as reference. "), clayxel.customMaterial, typeof(Material), false);
			
			if(EditorGUI.EndChangeCheck()){
				ClayContainer._inspectorUpdate();
				
				Undo.RecordObject(this.target, "changed clayxel container");

				if(customMaterial != clayxel.customMaterial){
					clayxel.setCustomMaterial(customMaterial);

					// clayxel.needsUpdate = true;
					// clayxel.forceUpdateAllSolids();
				}
				
				// UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
				// ClayContainer.getSceneView().Repaint();

				if(!Application.isPlaying){
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				}
			}

			if(!clayxel.isFrozenToMesh()){
				this.inspectMaterial(clayxel);
			}
		}

		MaterialEditor materialEditor = null;
		void inspectMaterial(ClayContainer clayContainer){
			EditorGUI.BeginChangeCheck();

			if(this.materialEditor != null){
				DestroyImmediate(this.materialEditor);
				this.materialEditor = null;
			}

			Material mat = clayContainer.getMaterial();
			if(mat != null){
				this.materialEditor = (MaterialEditor)CreateEditor(mat);
			}

			if(this.materialEditor != null){
				this.materialEditor.DrawHeader();
				this.materialEditor.OnInspectorGUI();
			}
			
			if(EditorGUI.EndChangeCheck()){
				if(!Application.isPlaying){
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				}
			}
		}

		void OnDisable (){
			if(this.materialEditor != null) {
				DestroyImmediate(this.materialEditor);
				this.materialEditor = null;
			}
		}
	}

	[CustomEditor(typeof(ClayObject)), CanEditMultipleObjects]
	public class ClayObjectInspector : Editor{
		public override void OnInspectorGUI(){
			UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
			
			ClayObject clayObj = (ClayObject)this.targets[0];
			ClayContainer clayxel = clayObj.getClayContainer();
			if(clayxel == null){
				return;
			}

			EditorGUI.BeginChangeCheck();
			
			string[] solidsLabels = clayxel.getSolidsCatalogueLabels();
	 		int primitiveType = EditorGUILayout.Popup("solidType", clayObj.primitiveType, solidsLabels);

			float blend = EditorGUILayout.Slider("blend", Mathf.Abs(clayObj.blend) * 100.0f, 0.0f, 100.0f);
			if(clayObj.blend < 0.0f){
				if(blend < 0.001f){
					blend = 0.001f;
				}

				blend *= -1.0f;
			}

			blend *= 0.01f;
			if(blend > 1.0f){
				blend = 1.0f;
			}
			else if(blend < -1.0f){
				blend = -1.0f;
			}

			GUILayout.BeginHorizontal();

			Color defaultColor = GUI.backgroundColor;

			if(clayObj.blend >= 0.0f){
				GUI.backgroundColor = Color.yellow;
			}

			if(GUILayout.Button(new GUIContent("add", "Additive blend"))){
				blend = Mathf.Abs(blend);
			}
			
			GUI.backgroundColor = defaultColor;

			if(clayObj.blend < 0.0f){
				GUI.backgroundColor = Color.yellow;
			}

			if(GUILayout.Button(new GUIContent("sub", "Subtractive blend"))){
				if(blend == 0.0f){
					blend = 0.0001f;
				}

				blend = blend * -1.0f;
			}

			GUI.backgroundColor = defaultColor;

			GUILayout.EndHorizontal();

			Color color = EditorGUILayout.ColorField("color", clayObj.color);

	 		Dictionary<string, float> paramValues = new Dictionary<string, float>();
	 		paramValues["x"] = clayObj.attrs.x;
	 		paramValues["y"] = clayObj.attrs.y;
	 		paramValues["z"] = clayObj.attrs.z;
	 		paramValues["w"] = clayObj.attrs.w;

	 		List<string[]> parameters = clayxel.getSolidsCatalogueParameters(primitiveType);
	 		List<string> wMaskLabels = new List<string>();
	 		for(int paramIt = 0; paramIt < parameters.Count; ++paramIt){
	 			string[] parameterValues = parameters[paramIt];
	 			string attr = parameterValues[0];
	 			string label = parameterValues[1];
	 			string defaultValue = parameterValues[2];

	 			if(primitiveType != clayObj.primitiveType){
	 				// reset to default params when changing primitive type
	 				paramValues[attr] = float.Parse(defaultValue, CultureInfo.InvariantCulture);
	 			}
	 			
	 			if(attr.StartsWith("w")){
	 				wMaskLabels.Add(label);
	 			}
	 			else{
	 				paramValues[attr] = EditorGUILayout.FloatField(label, paramValues[attr] * 100.0f) * 0.01f;
	 			}
	 		}

	 		if(wMaskLabels.Count > 0){
	 			paramValues["w"] = (float)EditorGUILayout.MaskField("options", (int)clayObj.attrs.w, wMaskLabels.ToArray());
	 		}
	 		
	 		if(EditorGUI.EndChangeCheck()){
	 			ClayContainer._inspectorUpdate();
	 			ClayContainer._skipHierarchyChanges = true;
				
	 			Undo.RecordObjects(this.targets, "changed clayobject");

	 			for(int i = 1; i < this.targets.Length; ++i){
	 				bool somethingChanged = false;
	 				ClayObject currentClayObj = (ClayObject)this.targets[i];
	 				bool shouldAutoRename = false;

	 				if(Mathf.Abs(clayObj.blend - blend) > 0.001f || Mathf.Sign(clayObj.blend) != Mathf.Sign(blend)){
	 					currentClayObj.blend = blend;
	 					somethingChanged = true;
	 					shouldAutoRename = true;
	 				}

	 				if(clayObj.color != color){
	 					currentClayObj.color = color;
	 					somethingChanged = true;
	 				}
					
	 				if(clayObj.primitiveType != primitiveType){
	 					currentClayObj.primitiveType = primitiveType;
	 					somethingChanged = true;
	 					shouldAutoRename = true;
	 				}

	 				if(clayObj.attrs.x != paramValues["x"]){
	 					currentClayObj.attrs.x = paramValues["x"];
	 					somethingChanged = true;
	 				}

	 				if(clayObj.attrs.y != paramValues["y"]){
	 					currentClayObj.attrs.y = paramValues["y"];
	 					somethingChanged = true;
	 				}

	 				if(clayObj.attrs.z != paramValues["z"]){
	 					currentClayObj.attrs.z = paramValues["z"];
	 					somethingChanged = true;
	 				}

	 				if(clayObj.attrs.w != paramValues["w"]){
	 					currentClayObj.attrs.w = paramValues["w"];
	 					somethingChanged = true;
	 					shouldAutoRename = true;
	 				}

	 				if(somethingChanged){
	 					currentClayObj.getClayContainer().clayObjectUpdated(currentClayObj);

	 					if(shouldAutoRename){
		 					if(currentClayObj.gameObject.name.StartsWith("clay_")){
		 						clayxel.autoRenameClayObject(currentClayObj);
		 					}
		 				}
	 				}

	 				ClayContainer._skipHierarchyChanges = false;
				}

	 			clayObj.blend = blend;
	 			clayObj.color = color;
	 			clayObj.primitiveType = primitiveType;
	 			clayObj.attrs.x = paramValues["x"];
	 			clayObj.attrs.y = paramValues["y"];
	 			clayObj.attrs.z = paramValues["z"];
	 			clayObj.attrs.w = paramValues["w"];

	 			if(clayObj.gameObject.name.StartsWith("clay_")){
					clayxel.autoRenameClayObject(clayObj);
				}

				clayObj.forceUpdate();
	 			
	 			UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
	 			ClayContainer.getSceneView().Repaint();

	 			if(!Application.isPlaying){
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				}
			}

			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();

			ClayObject.ClayObjectMode mode = (ClayObject.ClayObjectMode)EditorGUILayout.EnumPopup("mode", clayObj.mode);
			
			if(EditorGUI.EndChangeCheck()){
				clayObj.setMode(mode);

				UnityEditor.EditorApplication.QueuePlayerLoopUpdate();

				if(!Application.isPlaying){
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				}
			}

			if(clayObj.mode == ClayObject.ClayObjectMode.offset){
				this.drawOffsetMode(clayObj);
			}
			else if(clayObj.mode == ClayObject.ClayObjectMode.spline){
				this.drawSplineMode(clayObj);
			}

			EditorGUILayout.Space();
			GUILayout.BeginHorizontal();
			GUI.enabled = !clayxel.isClayObjectsOrderLocked();
			int clayObjectId = EditorGUILayout.IntField("order", clayObj.clayObjectId);
			GUI.enabled = true;

			if(!clayxel.isClayObjectsOrderLocked()){
				if(clayObjectId != clayObj.clayObjectId){
					int idOffset = clayObjectId - clayObj.clayObjectId; 
					clayxel.reorderClayObject(clayObj.clayObjectId, idOffset);
				}
			}

			if(GUILayout.Button(new GUIContent("↑", ""))){
				clayxel.reorderClayObject(clayObj.clayObjectId, -1);
			}
			if(GUILayout.Button(new GUIContent("↓", ""))){
				clayxel.reorderClayObject(clayObj.clayObjectId, 1);
			}
			if(GUILayout.Button(new GUIContent("⋮", ""))){
				EditorUtility.DisplayPopupMenu(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0), "Component/Clayxels/ClayObject", null);
			}
			GUILayout.EndHorizontal();
		}

		[MenuItem("Component/Clayxels/ClayObject/Mirror Duplicate (m)")]
    	static void MirrorDuplicate(MenuCommand command){
    		ClayContainer.shortcutMirrorDuplicate();
    	}

		[MenuItem("Component/Clayxels/ClayObject/Unlock Order From Hierarchy")]
    	static void OrderFromHierarchyOff(MenuCommand command){
    		if(UnityEditor.Selection.gameObjects.Length > 0){
    			ClayObject clayObj = UnityEditor.Selection.gameObjects[0].GetComponent<ClayObject>();
    			if(clayObj != null){
    				clayObj.getClayContainer().setClayObjectsOrderLocked(false);
    			}
    		}
    	}

    	[MenuItem("Component/Clayxels/ClayObject/Lock Order To Hierarchy")]
    	static void OrderFromHierarchyOn(MenuCommand command){
    		if(UnityEditor.Selection.gameObjects.Length > 0){
    			ClayObject clayObj = UnityEditor.Selection.gameObjects[0].GetComponent<ClayObject>();
    			if(clayObj != null){
    				clayObj.getClayContainer().setClayObjectsOrderLocked(true);
    			}
    		}
    	}

		[MenuItem("Component/Clayxels/ClayObject/Send Before ClayObject")]
    	static void sendBeforeClayObject(MenuCommand command){
    		if(UnityEditor.Selection.gameObjects.Length > 0){
    			ClayObject clayObj = UnityEditor.Selection.gameObjects[0].GetComponent<ClayObject>();
    			if(clayObj != null){
    				clayObj.getClayContainer().selectToReorder(clayObj, 0);
    			}
    		}
    	}

		[MenuItem("Component/Clayxels/ClayObject/Send After ClayObject")]
    	static void sendAfterClayObject(MenuCommand command){
    		if(UnityEditor.Selection.gameObjects.Length > 0){
    			ClayObject clayObj = UnityEditor.Selection.gameObjects[0].GetComponent<ClayObject>();
    			if(clayObj != null){
    				clayObj.getClayContainer().selectToReorder(clayObj, 1);
    			}
    		}
    	}

    	[MenuItem("Component/Clayxels/ClayObject/Rename all ClayObjects to Animate")]
    	static void renameToAnimate(MenuCommand command){
    		if(UnityEditor.Selection.gameObjects.Length > 0){
    			ClayObject clayObj = UnityEditor.Selection.gameObjects[0].GetComponent<ClayObject>();
    			if(clayObj != null){
    				ClayContainer container = clayObj.getClayContainer();
    				ClayContainer._skipHierarchyChanges = true;// otherwise each rename will trigger onHierarchyChange

    				int numClayObjs = container.getNumClayObjects();

    				for(int i = 0; i < numClayObjs; ++i){
    					ClayObject currentClayObj = container.getClayObject(i);

    					if(currentClayObj.gameObject.name.StartsWith("clay_")){
    						container.autoRenameClayObject(currentClayObj);
    						currentClayObj.name = "(" + i + ")" + currentClayObj.gameObject.name;
    					}
    				}

    				ClayContainer._skipHierarchyChanges = false;
    			}
    		}
    	}

		void drawSplineMode(ClayObject clayObj){
			EditorGUI.BeginChangeCheck();

			int subdivs = EditorGUILayout.IntField("subdivs", clayObj.getSplineSubdiv());

			GUILayout.BeginHorizontal();

			int numPoints = clayObj.splinePoints.Count - 2;
			EditorGUILayout.LabelField("control points: " + numPoints);

			if(GUILayout.Button(new GUIContent("+", ""))){
				clayObj.addSplineControlPoint();
			}

			if(GUILayout.Button(new GUIContent("-", ""))){
				clayObj.removeLastSplineControlPoint();
			}

			GUILayout.EndHorizontal();

			// var list = this.serializedObject.FindProperty("splinePoints");
			// EditorGUILayout.PropertyField(list, new GUIContent("spline points"), true);

			if(EditorGUI.EndChangeCheck()){
				// this.serializedObject.ApplyModifiedProperties();

				clayObj.setSplineSubdiv(subdivs);

				UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
			}
		}

		void drawOffsetMode(ClayObject clayObj){
			EditorGUI.BeginChangeCheck();
				
			int numSolids = EditorGUILayout.IntField("solids", clayObj.getNumSolids());
			bool allowSceneObjects = true;
			clayObj.offsetter = (GameObject)EditorGUILayout.ObjectField("offsetter", clayObj.offsetter, typeof(GameObject), allowSceneObjects);
			
			if(EditorGUI.EndChangeCheck()){
				if(numSolids < 1){
					numSolids = 1;
				}
				else if(numSolids > 100){
					numSolids = 100;
				}

				clayObj.setOffsetNum(numSolids);
				
				UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
			}
		}
	}

	[CustomEditor(typeof(Claymation))]
	public class ClaymationInspector : Editor{
		public override void OnInspectorGUI(){
			Claymation claymation = (Claymation)this.target;
			
			if(claymation.instanceOf == null){
				EditorGUI.BeginChangeCheck();

				TextAsset claymationFile = (TextAsset)EditorGUILayout.ObjectField(new GUIContent("claymation asset", ""), claymation.claymationFile, typeof(TextAsset), true);
				
				if(EditorGUI.EndChangeCheck()){
					if(claymation.claymationFile != claymationFile){
						claymation.claymationFile = claymationFile;
						claymation.init();
					}
				}

				if(GUILayout.Button(new GUIContent("reload file", "click this when your claymation file changes and you need to refresh this player"))){
					claymation.init();
				}

				Material material = (Material)EditorGUILayout.ObjectField(new GUIContent("material", "Use the same materials you use on the clayContainers"), claymation.material, typeof(Material), true);
				claymation.material = material;

				if(claymation.getNumFrames() > 1){
					int frameRate = EditorGUILayout.IntField(new GUIContent("frame rate", "how fast will this claymation play"), claymation.frameRate);

					claymation.frameRate = frameRate;

					claymation.playAnim = EditorGUILayout.Toggle(new GUIContent("play anim", "Always play the anim in loop"), claymation.playAnim);
					
					int currentFrame = EditorGUILayout.IntField(new GUIContent("frame", ""), claymation.getFrame());
					if(currentFrame != claymation.getFrame()){
						claymation.loadFrame(currentFrame);
					}
				}
			}

			EditorGUILayout.Space();

			Claymation instance = (Claymation)EditorGUILayout.ObjectField(new GUIContent("instance", "Set this to point at another Claymation in scene to make this into an instance and avoid duplicating memory."), claymation.instanceOf, typeof(Claymation), true);
			
			if(instance != claymation.instanceOf && instance != claymation){
				claymation.instanceOf = instance;
				claymation.init();

				UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
				ClayContainer.getSceneView().Repaint();

				if(!Application.isPlaying){
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				}
			}
		}
	}
}

public class ClayxelsPrefsWindow : EditorWindow{
	static ClayxelsPrefs prefs;

    [MenuItem("Component/Clayxels/Config")]
    public static void Open(){
    	if(Application.isPlaying){
    		return;
    	}

    	ClayxelsPrefsWindow.prefs = ClayContainer.loadPrefs();

        ClayxelsPrefsWindow window = (ClayxelsPrefsWindow)EditorWindow.GetWindow(typeof(ClayxelsPrefsWindow));
        window.Show();
    }

    void OnLostFocus(){
    	if(Application.isPlaying){
    		return;
    	}
    	
    	ClayContainer.savePrefs(ClayxelsPrefsWindow.prefs);
    	ClayContainer.reloadAll();
    }

    void OnGUI(){
    	if(Application.isPlaying){
    		return;
    	}

    	if(ClayxelsPrefsWindow.prefs == null){
    		ClayxelsPrefsWindow.prefs = ClayContainer.loadPrefs();
    	}

    	EditorGUI.BeginChangeCheck();

    	Color boundsColor = new Color((float)ClayxelsPrefsWindow.prefs.boundsColor[0] / 255.0f, (float)ClayxelsPrefsWindow.prefs.boundsColor[1] / 255.0f, (float)ClayxelsPrefsWindow.prefs.boundsColor[2] / 255.0f, (float)ClayxelsPrefsWindow.prefs.boundsColor[3] / 255.0f);
    	boundsColor = EditorGUILayout.ColorField(new GUIContent("boundsColor", "Color of the bounds indicator in the viewport, enable Gizmos in the viewport to see this."), boundsColor);
    	ClayxelsPrefsWindow.prefs.boundsColor[0] = (byte)(boundsColor.r * 255);
    	ClayxelsPrefsWindow.prefs.boundsColor[1] = (byte)(boundsColor.g * 255);
    	ClayxelsPrefsWindow.prefs.boundsColor[2] = (byte)(boundsColor.b * 255);
    	ClayxelsPrefsWindow.prefs.boundsColor[3] = (byte)(boundsColor.a * 255);

    	ClayxelsPrefsWindow.prefs.pickingKey = EditorGUILayout.TextField(new GUIContent("picking shortcut", "Press this shortcut to pick/select containers and clayObjects in scene."), ClayxelsPrefsWindow.prefs.pickingKey);
    	ClayxelsPrefsWindow.prefs.mirrorDuplicateKey = EditorGUILayout.TextField(new GUIContent("mirrorDuplicate shortcut", "Press this shortcut to duplicate and mirror a clayObject on the X axis."), ClayxelsPrefsWindow.prefs.mirrorDuplicateKey);

    	string[] pointCountPreset = new string[]{"low", "mid", "high"};
    	ClayxelsPrefsWindow.prefs.maxPointCount = EditorGUILayout.Popup(new GUIContent("pointCloud memory", "Preset to allocate video ram to handle bigger point clouds."), ClayxelsPrefsWindow.prefs.maxPointCount, pointCountPreset);

    	string[] solidsCountPreset = new string[]{"low", "mid", "high"};
    	ClayxelsPrefsWindow.prefs.maxSolidsCount = EditorGUILayout.Popup(new GUIContent("clayObjects memory", "Preset to allocate video ram to handle more clayObjects per container."), ClayxelsPrefsWindow.prefs.maxSolidsCount, solidsCountPreset);
    	
    	string[] solidsPerVoxelPreset = new string[]{"best performance", "balanced", "max sculpt detail"};
    	ClayxelsPrefsWindow.prefs.maxSolidsPerVoxel = EditorGUILayout.Popup(new GUIContent("clayObjects per voxel", "Preset to handle more clayObjects per voxel, it might fix some artifacts caused by having a lot of clayObjects all close to each other."), ClayxelsPrefsWindow.prefs.maxSolidsPerVoxel, solidsPerVoxelPreset);
    	
    	int frameSkip = EditorGUILayout.IntField(new GUIContent("frame skip", ""), ClayxelsPrefsWindow.prefs.frameSkip);
    	if(frameSkip < 0){
    		frameSkip = 0;
    	}
    	else if(frameSkip > 100){
    		frameSkip = 100;
    	}
    	ClayxelsPrefsWindow.prefs.frameSkip = frameSkip;

    	int maxBounds = EditorGUILayout.IntField(new GUIContent("max bounds size", "Smaller bounds use less video memory but give you less space to work with."), ClayxelsPrefsWindow.prefs.maxBounds);
    	if(maxBounds < 1){
    		maxBounds = 1;
    	}
    	else if(maxBounds > 4){
    		maxBounds = 4;
    	}
    	ClayxelsPrefsWindow.prefs.maxBounds = maxBounds;

    	ClayxelsPrefsWindow.prefs.vramLimitEnabled = EditorGUILayout.Toggle(new GUIContent("video ram limit enabled", "When this limit is enabled you won't be able to exceed your available vram when creating new container."), ClayxelsPrefsWindow.prefs.vramLimitEnabled);

    	EditorGUILayout.Space();

		EditorGUILayout.MinMaxSlider(new GUIContent("LOD: " + Mathf.Round(ClayxelsPrefsWindow.prefs.lodNear) + " - " + Mathf.Round(ClayxelsPrefsWindow.prefs.lodFar), "Level Of Detail in scene unit, measures the distance from the camera to automatically reduce the amount of points rendered."), 
			ref ClayxelsPrefsWindow.prefs.lodNear, ref ClayxelsPrefsWindow.prefs.lodFar, 0.0f, 1000.0f);

    	if(EditorGUI.EndChangeCheck()){
    		ClayContainer.savePrefs(ClayxelsPrefsWindow.prefs);
    		ClayContainer.reloadAll();
    	}

    	EditorGUILayout.Space();

	    int[] memStats = ClayContainer.getMemoryStats();
    	EditorGUILayout.LabelField("- vram rough usage -");
	    EditorGUILayout.LabelField("upfront vram allocated: " + memStats[0] + "MB");
	    EditorGUILayout.LabelField("containers in scene: " + memStats[1] + "MB");

	    EditorGUILayout.Space();

		if(GUILayout.Button((new GUIContent("reload all", "This is necessary after you make changes to the shaders or to the claySDF file.")))){
			ClayContainer.reloadAll();
		}
    }
}

#endif // end if UNITY_EDITOR
