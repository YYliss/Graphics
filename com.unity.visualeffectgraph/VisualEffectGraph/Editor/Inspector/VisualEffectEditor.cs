using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Experimental.VFX;

using UnityEditor.VFX;
using UnityEditor.VFX.UI;
using UnityEditor.Experimental.UIElements.GraphView;
using EditMode = UnityEditorInternal.EditMode;
using UnityObject = UnityEngine.Object;


static class VisualEffectEditorStyles
{
    static GUIContent[] m_Icons;

    public enum Icon
    {
        Pause,
        Play,
        Restart,
        Step,
        Stop
    }

    static VisualEffectEditorStyles()
    {
        m_Icons = new GUIContent[1 + (int)Icon.Stop];
        for (int i = 0; i <= (int)Icon.Stop; ++i)
        {
            Icon icon = (Icon)i;
            string name = icon.ToString();

            //TODO replace with editor default resource call when going to trunk
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/VFXEditor/Editor/SceneWindow/Textures/" + name + ".png");
            if (texture == null)
            {
                Debug.LogError("Can't find icon for " + name + " in VisualEffectEditorStyles");
                continue;
            }
            m_Icons[i] = new GUIContent(texture);
        }
    }

    public static GUIContent GetIcon(Icon icon)
    {
        return m_Icons[(int)icon];
    }
}

static class VisualEffectUtility
{
    public static string GetTypeField(Type type)
    {
        if (type == typeof(Vector2))
        {
            return "m_Vector2f";
        }
        else if (type == typeof(Vector3))
        {
            return "m_Vector3f";
        }
        else if (type == typeof(Vector4))
        {
            return "m_Vector4f";
        }
        else if (type == typeof(Color))
        {
            return "m_Vector4f";
        }
        else if (type == typeof(AnimationCurve))
        {
            return "m_AnimationCurve";
        }
        else if (type == typeof(Gradient))
        {
            return "m_Gradient";
        }
        else if (type == typeof(Texture2D))
        {
            return "m_NamedObject";
        }
        else if (type == typeof(Texture2DArray))
        {
            return "m_NamedObject";
        }
        else if (type == typeof(Texture3D))
        {
            return "m_NamedObject";
        }
        else if (type == typeof(Cubemap))
        {
            return "m_NamedObject";
        }
        else if (type == typeof(CubemapArray))
        {
            return "m_NamedObject";
        }
        else if (type == typeof(Mesh))
        {
            return "m_NamedObject";
        }
        else if (type == typeof(float))
        {
            return "m_Float";
        }
        else if (type == typeof(int))
        {
            return "m_Int";
        }
        else if (type == typeof(uint))
        {
            return "m_Uint";
        }
        else if (type == typeof(bool))
        {
            return "m_Bool";
        }
        else if (type == typeof(Matrix4x4))
        {
            return "m_Matrix4x4f";
        }
        //Debug.LogError("unknown vfx property type:"+type.UserFriendlyName());
        return null;
    }
}

//using UnityEngine.Experimental.VFX;
/*
public class SlotValueBinder : VFXPropertySlotObserver
{
    public SlotValueBinder(string name, VisualEffect component, VFXPropertySlot slot)
    {
        m_Name = name;
        m_Component = component;
        m_Slot = slot;

        Update(); // Update before adding the observer in order not to receive a spurious event
        m_Slot.AddObserver(this);
    }

    public bool Update()
    {
        switch (m_Slot.ValueType)
        {
            case VFXValueType.kInt:
                if (m_Component.HasInt(m_Name))
                    m_Slot.Set<int>(m_Component.GetInt(m_Name));
                break;
            case VFXValueType.kUint:
                if (m_Component.HasUInt(m_Name))
                    m_Slot.Set<UInt32>(m_Component.GetUInt(m_Name));
                break;
            case VFXValueType.kFloat:
                if (m_Component.HasFloat(m_Name))
                    m_Slot.Set<float>(m_Component.GetFloat(m_Name));
                break;
            case VFXValueType.kFloat2:
                if (m_Component.HasVector2(m_Name))
                    m_Slot.Set<Vector2>(m_Component.GetVector2(m_Name));
                break;
            case VFXValueType.kFloat3:
                if (m_Component.HasVector3(m_Name))
                    m_Slot.Set<Vector3>(m_Component.GetVector3(m_Name));
                break;
            case VFXValueType.kFloat4:
                if (m_Component.HasVector4(m_Name))
                    m_Slot.Set<Vector4>(m_Component.GetVector4(m_Name));
                break;
            case VFXValueType.kTexture2D:
                if (m_Component.HasTexture2D(m_Name))
                    m_Slot.Set<Texture2D>(m_Component.GetTexture2D(m_Name));
                break;
            case VFXValueType.kTexture3D:
                if (m_Component.HasTexture3D(m_Name))
                    m_Slot.Set<Texture3D>(m_Component.GetTexture3D(m_Name));
                break;
        }

        bool dirty = m_Dirty;
        m_Dirty = false;
        return dirty;
    }

    public void OnSlotEvent(VFXPropertySlot.Event type, VFXPropertySlot slot)
    {
        if (m_Slot != slot || type != VFXPropertySlot.Event.kValueUpdated)
            throw new Exception("Something wrong went on !"); // This should never happen

        if (!VisualEffectEditor.CanSetOverride) // Hack
            return;

        switch (slot.ValueType)
        {
            case VFXValueType.kFloat:
                if (m_Component.HasFloat(m_Name))
                    m_Component.SetFloat(m_Name, m_Slot.Get<float>());
                break;
            case VFXValueType.kFloat2:
                if (m_Component.HasVector2(m_Name))
                    m_Component.SetVector2(m_Name, m_Slot.Get<Vector2>());
                break;
            case VFXValueType.kFloat3:
                if (m_Component.HasVector3(m_Name))
                    m_Component.SetVector3(m_Name, m_Slot.Get<Vector3>());
                break;
            case VFXValueType.kFloat4:
                if (m_Component.HasVector4(m_Name))
                    m_Component.SetVector4(m_Name, m_Slot.Get<Vector4>());
                break;
            case VFXValueType.kTexture2D:
                if (m_Component.HasTexture2D(m_Name))
                    m_Component.SetTexture2D(m_Name, m_Slot.Get<Texture2D>());
                break;
            case VFXValueType.kTexture3D:
                if (m_Component.HasTexture3D(m_Name))
                    m_Component.SetTexture3D(m_Name, m_Slot.Get<Texture3D>());
                break;
        }

        m_Dirty = true;
    }

    public string Name { get { return m_Name; } }

    private string m_Name;
    private VisualEffect m_Component;
    private VFXPropertySlot m_Slot;
    private bool m_Dirty = false;
}
*/
[CustomEditor(typeof(VisualEffect))]
public class VisualEffectEditor : Editor
{
    public static bool CanSetOverride = false;

    SerializedProperty m_VisualEffectAsset;
    SerializedProperty m_ReseedOnPlay;
    SerializedProperty m_RandomSeed;
    SerializedProperty m_VFXPropertySheet;
    bool m_useNewSerializedField = false;

    private Contents m_Contents;
    private Styles m_Styles;
    /*
    private class ExposedData
    {
        public VFXOutputSlot slot;
        public List<SlotValueBinder> valueBinders = new List<SlotValueBinder>();
        public VFXUIWidget widget = null;
    }

    private List<ExposedData> m_ExposedData = new List<ExposedData>();*/
    //private List<VFXOutputSlot> m_Slots = new List<VFXOutputSlot>();
    //private List<SlotValueBinder> m_ValueBinders = new List<SlotValueBinder>();

    //private VisualEffectDebugPanel m_DebugPanel;
    private bool m_ShowDebugStats = false;

    void OnEnable()
    {
        m_RandomSeed = serializedObject.FindProperty("m_StartSeed");
        m_ReseedOnPlay = serializedObject.FindProperty("m_ResetSeedOnPlay");
        m_VisualEffectAsset = serializedObject.FindProperty("m_Asset");
        m_VFXPropertySheet = serializedObject.FindProperty("m_PropertySheet");

        InitSlots();

        m_Infos.Clear();
    }

    void OnDisable()
    {
        VisualEffect effect = ((VisualEffect)targets[0]);
        effect.pause = false;
        effect.playRate = 1.0f;
    }

    struct Infos
    {
        public VFXPropertyIM propertyIM;
        public Type type;
    }

    Dictionary<VFXParameterController, Infos> m_Infos = new Dictionary<VFXParameterController, Infos>();

    void OnParamGUI(VFXParameter parameter)
    {
        VisualEffect comp = (VisualEffect)target;

        string fieldName = VisualEffectUtility.GetTypeField(parameter.type);


        var vfxField = m_VFXPropertySheet.FindPropertyRelative(fieldName + ".m_Array");
        SerializedProperty property = null;
        if (vfxField != null)
        {
            for (int i = 0; i < vfxField.arraySize; ++i)
            {
                property = vfxField.GetArrayElementAtIndex(i);
                var nameProperty = property.FindPropertyRelative("m_Name").stringValue;
                if (nameProperty == parameter.exposedName)
                {
                    break;
                }
                property = null;
            }
        }
        if (property != null)
        {
            SerializedProperty overrideProperty = property.FindPropertyRelative("m_Overridden");
            property = property.FindPropertyRelative("m_Value");
            string firstpropName = property.name;

            Color previousColor = GUI.color;
            var animated = AnimationMode.IsPropertyAnimated(target, property.propertyPath);
            if (animated)
            {
                GUI.color = AnimationMode.animatedPropertyColor;
            }

            EditorGUIUtility.SetBoldDefaultFont(overrideProperty.boolValue);

            EditorGUI.BeginChangeCheck();
            if (parameter.type == typeof(Color))
            {
                Vector4 vVal = property.vector4Value;
                Color c = new Color(vVal.x, vVal.y, vVal.z, vVal.w);
                c = EditorGUILayout.ColorField(parameter.exposedName, c);

                if (c.r != vVal.x || c.g != vVal.y || c.b != vVal.z || c.a != vVal.w)
                    property.vector4Value = new Vector4(c.r, c.g, c.b, c.a);
            }
            else
                EditorGUILayout.PropertyField(property, new GUIContent(parameter.exposedName), true);

            if (EditorGUI.EndChangeCheck())
            {
                overrideProperty.boolValue = true;
            }

            if (animated)
            {
                GUI.color = previousColor;
            }
        }
    }

    private void InitSlots()
    {
        /*foreach (var exposed in m_ExposedData)
            exposed.slot.RemoveAllObservers();

        m_ExposedData.Clear();
        */
        if (m_VisualEffectAsset == null)
            return;

        VisualEffectAsset asset = m_VisualEffectAsset.objectReferenceValue as VisualEffectAsset;
        if (asset == null)
            return;


        /*
        int nbDescs = asset.GetNbEditorExposedDesc();
        for (int i = 0; i < nbDescs; ++i)
        {
            string semanticType = asset.GetEditorExposedDescSemanticType(i);
            string exposedName = asset.GetEditorExposedDescName(i);
            bool worldSpace = asset.GetEditorExposedDescWorldSpace(i);

            var dataBlock = VFXEditor.Block.GetDataBlock(semanticType);
            if (dataBlock != null)
            {
                var property = new VFXProperty(dataBlock.Semantics, exposedName);
                var slot = new VFXOutputSlot(property);
                slot.WorldSpace = worldSpace;

                var exposedData = new ExposedData();
                exposedData.slot = slot;
                m_ExposedData.Add(exposedData);

                CreateValueBinders(exposedData,slot);
            }
        }*/
    }

    /*
    private void CreateValueBinders(ExposedData data,VFXPropertySlot slot,string parentName = "")
    {
        string name = VFXPropertySlot.AggregateName(parentName, slot.Name);
        if (slot.GetNbChildren() > 0)
            for (int i = 0; i < slot.GetNbChildren(); ++i)
            {
                var child = slot.GetChild(i);
                CreateValueBinders(data,child, name);
            }
        else
        {
            data.valueBinders.Add(new SlotValueBinder(name, (VisualEffect)target, slot));
        }
    }
    */
    public void InitializeGUI()
    {
        if (m_Contents == null)
            m_Contents = new Contents();

        if (m_Styles == null)
            m_Styles = new Styles();
    }

    public static bool s_IsEditingAsset = false;


    private void SceneViewGUICallback(UnityObject target, SceneView sceneView)
    {
        VisualEffect effect = ((VisualEffect)targets[0]);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(VisualEffectEditorStyles.GetIcon(VisualEffectEditorStyles.Icon.Stop)))
        {
            effect.Reinit();
            effect.pause = true;
        }
        if (GUILayout.Button(VisualEffectEditorStyles.GetIcon(VisualEffectEditorStyles.Icon.Play)))
        {
            effect.pause = false;
        }
        if (GUILayout.Button(VisualEffectEditorStyles.GetIcon(VisualEffectEditorStyles.Icon.Pause)))
        {
            effect.pause = !effect.pause;
        }
        if (GUILayout.Button(VisualEffectEditorStyles.GetIcon(VisualEffectEditorStyles.Icon.Step)))
        {
            effect.pause = true;
            effect.AdvanceOneFrame();
        }
        if (GUILayout.Button(VisualEffectEditorStyles.GetIcon(VisualEffectEditorStyles.Icon.Restart)))
        {
            effect.Reinit();
            effect.pause = false;
        }
        GUILayout.EndHorizontal();

        float playbackRate = EditorGUILayout.FloatField("Playback Rate", effect.playRate);
        if (playbackRate < 0)
            playbackRate = 0;
        effect.playRate = playbackRate;
    }

    protected virtual void OnSceneGUI()
    {
        SceneViewOverlay.Window(ParticleSystemInspector.playBackTitle, SceneViewGUICallback, (int)SceneViewOverlay.Ordering.ParticleEffect, SceneViewOverlay.WindowDisplayOption.OneWindowPerTitle);

        if (EditMode.editMode == EditMode.SceneViewEditMode.Collider && EditMode.IsOwner(this))
            VFXGizmo.OnDrawComponentGizmo(target as VisualEffect);
    }

    /*
    public void OnSceneGUI()
    {
        InitializeGUI();

        if(m_ShowDebugStats)
        {
            m_DebugPanel.UpdateDebugData();
            m_DebugPanel.OnSceneGUI();
        }

        GameObject sceneCamObj = GameObject.Find("SceneCamera");
        if (sceneCamObj != null)
        {
            GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
            Handles.BeginGUI();
            Camera cam = sceneCamObj.GetComponent<Camera>();
            Rect windowRect = new Rect(cam.pixelWidth / 2 - 140, cam.pixelHeight - 64 , 324, 68);
            GUI.Window(666, windowRect, DrawPlayControlsWindow, "VFX Playback Control");

            if(m_ShowDebugStats)
                m_DebugPanel.OnWindowGUI();

            Handles.EndGUI();
            GL.sRGBWrite = false;
        }


        CanSetOverride = true;
        foreach (var exposed in m_ExposedData)
            if (exposed.widget != null)
                exposed.widget.OnSceneGUI(SceneView.currentDrawingSceneView);
        CanSetOverride = false;
    }

    public void DrawPlayControlsWindow(int windowID)
    {
        var component = (VisualEffect)target;

        m_ShowDebugStats = GUI.Toggle(new Rect(260,0,64,16),m_ShowDebugStats, m_Contents.infoButton, EditorStyles.miniButton);

        // PLAY CONTROLS
        using (new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button(new GUIContent(VFXEditor.styles.ToolbarRestart), EditorStyles.miniButtonLeft, m_Styles.PlayControlsHeight))
            {
                component.pause = false;
                component.Reinit();
            }

            if (GUILayout.Button(new GUIContent(VFXEditor.styles.ToolbarPlay), EditorStyles.miniButtonMid, m_Styles.PlayControlsHeight))
                component.pause = false;

            component.pause = GUILayout.Toggle(component.pause, new GUIContent(VFXEditor.styles.ToolbarPause), EditorStyles.miniButtonMid, m_Styles.PlayControlsHeight);


            if (GUILayout.Button(new GUIContent(VFXEditor.styles.ToolbarStop), EditorStyles.miniButtonMid, m_Styles.PlayControlsHeight))
            {
                component.pause = true;
                component.Reinit();
            }

            if (GUILayout.Button(new GUIContent(VFXEditor.styles.ToolbarFrameAdvance), EditorStyles.miniButtonRight, m_Styles.PlayControlsHeight))
            {
                component.pause = true;
                component.AdvanceOneFrame();
            }
        }

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label(m_Contents.PlayRate, GUILayout.Width(54));
            // Play Rate
            float r = component.playRate;
            float nr = Mathf.Pow(GUILayout.HorizontalSlider(Mathf.Sqrt(component.playRate), 0.0f, Mathf.Sqrt(8.0f)), 2.0f);
            GUILayout.Label(Mathf.Round(nr * 100) + "%", GUILayout.Width(36));
            if (r != nr)
                SetPlayRate(nr);

            if (GUILayout.Button(m_Contents.SetPlayRate, EditorStyles.miniButton, m_Styles.MiniButtonWidth))
            {
                GenericMenu toolsMenu = new GenericMenu();
                float rate = component.playRate;
                toolsMenu.AddItem(new GUIContent("800%"), rate == 8.0f, SetPlayRate, 8.0f);
                toolsMenu.AddItem(new GUIContent("200%"), rate == 2.0f, SetPlayRate, 2.0f);
                toolsMenu.AddItem(new GUIContent("100% (RealTime)"), rate == 1.0f, SetPlayRate, 1.0f);
                toolsMenu.AddItem(new GUIContent("50%"), rate == 0.5f, SetPlayRate, 0.5f);
                toolsMenu.AddItem(new GUIContent("25%"), rate == 0.25f, SetPlayRate, 0.25f);
                toolsMenu.AddItem(new GUIContent("10%"), rate == 0.1f, SetPlayRate, 0.1f);
                toolsMenu.AddItem(new GUIContent("1%"), rate == 0.01f, SetPlayRate, 0.01f);
                toolsMenu.ShowAsContext();
            }
        }

        // Handle click in window to avoid unselecting asset
        if (Event.current.type == EventType.mouseDown)
            Event.current.Use();
    }*/

    private VisualEffectAsset m_asset;
    private VFXGraph m_graph;

    public override void OnInspectorGUI()
    {
        InitializeGUI();

        var component = (VisualEffect)target;

        //Asset
        GUILayout.Label(m_Contents.HeaderMain, m_Styles.InspectorHeader);

        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.PropertyField(m_VisualEffectAsset, m_Contents.AssetPath);

            GUI.enabled = component.visualEffectAsset != null; // Enabled state will be kept for all content until the end of the inspectorGUI.
            if (GUILayout.Button(m_Contents.OpenEditor, EditorStyles.miniButton, m_Styles.MiniButtonWidth))
            {
                VFXViewWindow window = EditorWindow.GetWindow<VFXViewWindow>();

                window.LoadAsset(component.visualEffectAsset);
            }
        }

        //Seed
        EditorGUI.BeginChangeCheck();
        using (new GUILayout.HorizontalScope())
        {
            using (new EditorGUI.DisabledGroupScope(m_ReseedOnPlay.boolValue))
            {
                EditorGUILayout.PropertyField(m_RandomSeed, m_Contents.RandomSeed);
                if (GUILayout.Button(m_Contents.SetRandomSeed, EditorStyles.miniButton, m_Styles.MiniButtonWidth))
                {
                    m_RandomSeed.intValue = UnityEngine.Random.Range(0, int.MaxValue);
                    component.startSeed = (uint)m_RandomSeed.intValue; // As accessors are bypassed with serialized properties...
                }
            }
        }
        EditorGUILayout.PropertyField(m_ReseedOnPlay, m_Contents.ReseedOnPlay);
        bool reinit = EditorGUI.EndChangeCheck();

        //Field
        GUILayout.Label(m_Contents.HeaderParameters, m_Styles.InspectorHeader);

        if (m_graph == null || m_asset != component.visualEffectAsset)
        {
            m_asset = component.visualEffectAsset;
            if (m_asset != null)
            {
                m_graph = m_asset.GetOrCreateGraph();
            }
        }

        if (m_graph != null)
        {
            var newList = m_graph.children.OfType<VFXParameter>().Where(t => t.exposed).OrderBy(t => t.order).ToArray();
            foreach (var parameter in newList)
            {
                OnParamGUI(parameter);
            }
        }

        serializedObject.ApplyModifiedProperties();
        if (reinit)
        {
            component.Reinit();
        }

        EditMode.DoEditModeInspectorModeButton(
            EditMode.SceneViewEditMode.Collider,
            "Edit Asset Values",
            UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle.editModeButton,
            this
            );
        GUI.enabled = true;

        s_IsEditingAsset = EditMode.editMode == EditMode.SceneViewEditMode.Collider && EditMode.IsOwner(this);

        if (s_IsEditingAsset && !m_WasEditingAsset)
        {
            VFXViewWindow window = EditorWindow.GetWindow<VFXViewWindow>();
            window.LoadAsset(component.visualEffectAsset);
        }
        m_WasEditingAsset = s_IsEditingAsset;
    }

    bool m_WasEditingAsset;

    private void SetPlayRate(object rate)
    {
        var component = (VisualEffect)target;
        component.playRate = (float)rate;
    }

    private class Styles
    {
        public GUIStyle InspectorHeader;
        public GUIStyle ToggleGizmo;

        public GUILayoutOption MiniButtonWidth = GUILayout.Width(48);
        public GUILayoutOption PlayControlsHeight = GUILayout.Height(24);

        public Styles()
        {
            InspectorHeader = new GUIStyle("ShurikenModuleTitle");
            InspectorHeader.fontSize = 12;
            InspectorHeader.fontStyle = FontStyle.Bold;
            InspectorHeader.contentOffset = new Vector2(2, -2);
            InspectorHeader.border = new RectOffset(4, 4, 4, 4);
            InspectorHeader.overflow = new RectOffset(4, 4, 4, 4);
            InspectorHeader.margin = new RectOffset(4, 4, 16, 8);

            Texture2D showIcon = EditorGUIUtility.Load("VisibilityIcon.png") as Texture2D;
            Texture2D hideIcon = EditorGUIUtility.Load("VisibilityIconDisabled.png") as Texture2D;

            ToggleGizmo = new GUIStyle();
            ToggleGizmo.margin = new RectOffset(0, 0, 4, 0);
            ToggleGizmo.active.background = hideIcon;
            ToggleGizmo.onActive.background = showIcon;
            ToggleGizmo.normal.background = hideIcon;
            ToggleGizmo.onNormal.background = showIcon;
            ToggleGizmo.focused.background = hideIcon;
            ToggleGizmo.onFocused.background = showIcon;
            ToggleGizmo.hover.background = hideIcon;
            ToggleGizmo.onHover.background = showIcon;
        }
    }

    private class Contents
    {
        public GUIContent HeaderMain = new GUIContent("VFX Asset");
        public GUIContent HeaderPlayControls = new GUIContent("Play Controls");
        public GUIContent HeaderParameters = new GUIContent("Parameters");

        public GUIContent AssetPath = new GUIContent("Asset Template");
        public GUIContent RandomSeed = new GUIContent("Random Seed");
        public GUIContent ReseedOnPlay = new GUIContent("Reseed on play");
        public GUIContent OpenEditor = new GUIContent("Edit");
        public GUIContent SetRandomSeed = new GUIContent("Reseed");
        public GUIContent SetPlayRate = new GUIContent("Set");
        public GUIContent PlayRate = new GUIContent("PlayRate");
        public GUIContent ResetOverrides = new GUIContent("Reset");

        public GUIContent ButtonRestart = new GUIContent();
        public GUIContent ButtonPlay = new GUIContent();
        public GUIContent ButtonPause = new GUIContent();
        public GUIContent ButtonStop = new GUIContent();
        public GUIContent ButtonFrameAdvance = new GUIContent();

        public GUIContent ToggleWidget = new GUIContent();

        public GUIContent infoButton = new GUIContent("Debug", EditorGUIUtility.IconContent("console.infoicon").image);
    }
}
