using UnityEngine;
using UnityEditor;

namespace KaiwareStyle
{
    public class ObjectMaskPointMeshMaker : EditorWindow
    {
        [Tooltip("PointMeshのファイル名")]
        public string filename = "mesh";

        [Tooltip("X軸の頂点の数")]
        public int pointXnum = 10;
        [Tooltip("Y軸の頂点の数")]
        public int pointYnum = 10;
        [Tooltip("Z軸の頂点の数")]
        public int pointZnum = 10;

        [Tooltip("頂点を配置するBoxの中心座標")]
        public Vector3 center = Vector3.one * -0.5f;
        [Tooltip("頂点を配置するBoxの中心座標のオフセット")]
        public Vector3 offset = Vector3.zero;
        [Tooltip("頂点を配置するBoxの大きさ")]
        public Vector3 scale = Vector3.one;

        [Tooltip("頂点群をBoxの面ピッタリに敷き詰めるか？")]
        public bool isFill = true;

        [Tooltip("頂点群をくり抜くコライダーの配列")]
        public Collider[] colliders;

        SerializedProperty filenameProp;

        SerializedProperty xnumProp;
        SerializedProperty ynumProp;
        SerializedProperty znumProp;

        SerializedProperty centerProp;
        SerializedProperty offsetProp;
        SerializedProperty scaleProp;

        SerializedProperty isFillProp;
        SerializedProperty collidersProp;

        Vector3[] vertices = null;
        ObjectMaskMeshMaker meshMaker = new ObjectMaskMeshMaker();
        SerializedObject so;

        [MenuItem("Custom/Object Mask Point Mesh Maker")]
        static void Init()
        {
            var win = GetWindow<ObjectMaskPointMeshMaker>();
            win.OnEnable();
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;

            meshMaker.colliders = colliders;
            vertices = meshMaker.GenerateVertices(pointXnum, pointYnum, pointZnum, center, scale, offset, isFill);

            ScriptableObject target = this;
            so = new SerializedObject(target);

            filenameProp = so.FindProperty("filename");
            xnumProp = so.FindProperty("pointXnum");
            ynumProp = so.FindProperty("pointYnum");
            znumProp = so.FindProperty("pointZnum");

            centerProp = so.FindProperty("center");
            offsetProp = so.FindProperty("offset");
            scaleProp = so.FindProperty("scale");

            isFillProp = so.FindProperty("isFill");
            collidersProp = so.FindProperty("colliders");
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnGUI()
        {
            EditorGUILayout.PropertyField(filenameProp, true);

            EditorGUILayout.Space(8);

            EditorGUILayout.LabelField(new GUIContent("Volume Settings", "頂点を敷き詰めるBox領域の設定"), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(centerProp, false);
            EditorGUILayout.PropertyField(scaleProp, false);
            EditorGUILayout.PropertyField(offsetProp, false);

            EditorGUILayout.Space(8);

            EditorGUILayout.LabelField(new GUIContent("Points Settings", "頂点の設定"), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(xnumProp, true);
            EditorGUILayout.PropertyField(ynumProp, true);
            EditorGUILayout.PropertyField(znumProp, true);

            EditorGUILayout.Space(8);

            EditorGUILayout.PropertyField(isFillProp, true);

            EditorGUILayout.Space(8);

            EditorGUILayout.PropertyField(collidersProp, true);

            if (GUILayout.Button(new GUIContent("Check", "パラメータを反映して確認")))
            {
                meshMaker.colliders = colliders;
                vertices = meshMaker.GenerateVertices(pointXnum, pointYnum, pointZnum, center, scale, offset, isFill);
            }

            so.ApplyModifiedProperties();
            
            if(vertices != null)
            {
                EditorGUILayout.LabelField("Vertex Count : " + vertices.Length);
            }
            if (GUILayout.Button(new GUIContent("Create Point Mesh", "PointMeshを作成して/Assets/以下に保存")))
            {
                OnWizardCreate();
            }
        }

        /// <summary>
        /// Createボタンが押された
        /// </summary>
        private void OnWizardCreate()
        {
            meshMaker.colliders = colliders;

            Mesh mesh = meshMaker.GenerateMesh(pointXnum, pointYnum, pointZnum, center, scale, offset, isFill);

            AssetDatabase.CreateAsset(mesh, "Assets/" + filename + ".asset");
            AssetDatabase.SaveAssets();
        }

        void OnSceneGUI(SceneView sceneView)
        {

            var pos = Vector3.one * 0.5f + center;
            pos.Scale(scale);
            pos += offset;
            Handles.color = Color.yellow;
            Handles.DrawWireCube(pos, scale);

            if (vertices != null)
            {
                Handles.color = Color.cyan;
                for (int i = 0; i < vertices.Length; i++)
                {
                    float size = HandleUtility.GetHandleSize(vertices[i]);
                    Handles.DrawLine(vertices[i], vertices[i] + Vector3.one * size * 0.01f);
                }
            }
        }
    }

    public class ObjectMaskMeshMaker : MeshMaker
    {
        public Collider[] colliders;

        protected override bool IsInShape(Vector3 pos)
        {
            if (colliders != null)
            {
                foreach (var col in colliders)
                {
                    if (col == null)
                        continue;

                    if (col.ClosestPoint(pos) == pos)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}