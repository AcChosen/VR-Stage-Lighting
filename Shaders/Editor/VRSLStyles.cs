using UnityEngine;
using UnityEditor;

// help link https://docs.unity3d.com/ScriptReference/EditorStyles.html
// ---DISCLAIMER--- THIS CODE IS BASED OFF OF "SYNQARK"'s ARKTOON-SHADERS AND "XIEXE"'s UNITY-SHADERS. FOR MORE INFORMATION PLEASE REFER TO THE ORIGINAL BASE WRITER "https://github.com/synqark", "https://github.com/synqark/Arktoon-Shaders" or "https://github.com/Xiexe", "https://github.com/Xiexe/Xiexes-Unity-Shaders"

[InitializeOnLoad]
public class VRSLStyles : MonoBehaviour
{
    public static Texture logo = Resources.Load("VRStageLighting-Logo") as Texture;

    public static string ver = "VR Stage Lighting ver:" + "<color=#9b34ebff> 1.20</color>";

    public static void DepthPassWarning()
    {
        EditorGUILayout.HelpBox("Shader looking weird? \nPlease ensure that the depth texture is enabled by having the included 'Directional Light' prefab somewhere in your scene, located in \nAssets/VRStageLighting/VR-Stage-Lighting/Other", MessageType.Info);
    }

    public static void DrawLogo()
    {
        ///GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));
        // GUILayout.FlexibleSpace();
        //GUI.DrawTexture(pos,logo,ScaleMode.ScaleToFit);
        //EditorGUI.DrawPreviewTexture(new Rect(0,0,400,150), logo);
        Vector2 contentOffset = new Vector2(0f, -2f);
        GUIStyle style = new GUIStyle(EditorStyles.label);
        style.fixedHeight = 200;
        //style.fixedWidth = 300;
        style.contentOffset = contentOffset;
        style.alignment = TextAnchor.MiddleCenter;
        var rect = GUILayoutUtility.GetRect(300f, 190f, style);
        //GUILayout.Label(logo,style, GUILayout.MaxWidth(500), GUILayout.MaxHeight(200));
        GUI.Box(rect, logo,style);
        //GUILayout.Label(logo);
        // GUILayout.FlexibleSpace();
        //GUILayout.EndArea();
    }
    private static Rect DrawShuriken(string title, Vector2 contentOffset, int HeaderHeight)
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = HeaderHeight;
        style.contentOffset = contentOffset;
        var rect = GUILayoutUtility.GetRect(16f, HeaderHeight, style);
        GUI.Box(rect, title, style);
        return rect;
    }

    /// indent support
    private static Rect DrawShuriken(string title, Vector2 contentOffset, int HeaderHeight, int indent)
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = HeaderHeight;
        style.contentOffset = contentOffset;
        var rect = GUILayoutUtility.GetRect(16f, HeaderHeight, style);
        rect = new Rect(rect.x + indent, rect.y, rect.width - indent, rect.height);
        GUI.Box(rect, title, style);
        return rect;
    }

    private static Rect DrawShurikenCenteredTitle(string title, Vector2 contentOffset, int HeaderHeight)
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = HeaderHeight;
        style.contentOffset = contentOffset;
        style.alignment = TextAnchor.MiddleCenter;
        var rect = GUILayoutUtility.GetRect(16f, HeaderHeight, style);

        GUI.Box(rect, title, style);
        return rect;
    }

    public static bool ShurikenFoldout(string title, bool display)
    {
        var rect = DrawShuriken(title, new Vector2(20f, -2f), 22);
        var e = Event.current;
        var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
        if (e.type == EventType.Repaint)
        {
            EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        }
        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            display = !display;
            e.Use();
        }
        return display;
    }
        //// indent support
    public static bool ShurikenFoldout(string title, bool display, int intent)
    {
        // var indentAmmount = (float)(typeof(EditorGUI).GetField("kIndentPerLevel").GetValue(null));//// reeeee it was private, WHY UNITY?
        // Debug.log("blah " + (float)(typeof(EditorGUI).GetField("kIndentPerLevel").GetValue(null)));//// appears to NULL?
        const int indentAmmount = 15;
        var rect = DrawShuriken(title, new Vector2(20f, -2f), 22, intent * indentAmmount );
        var e = Event.current;
        var toggleRect = new Rect(rect.x + 4f , rect.y + 2f, 13f, 13f); //// the arrow
        if (e.type == EventType.Repaint)
        {
            EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        }
        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            display = !display;
            e.Use();
        }
        return display;
    }
    //parting line text
    private static Rect DrawShurikenPartingLineText(string title, Vector2 contentOffset, int HeaderHeight)
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = HeaderHeight;
        style.contentOffset = contentOffset;
        style.alignment = TextAnchor.MiddleCenter;
        var rect = GUILayoutUtility.GetRect(16f, HeaderHeight, style);

        GUI.Box(rect, title, style);
        return rect;
    }
    //end of parting line text

    //parting lines
    static public void PartingLine()
    {
        GUILayout.Space(5);
        GUILine(new Color(0.5f, 0.5f, 0.5f), 1.5f);
        GUILayout.Space(5);
    }

    static public void GUILine(float height = 0f)
    {
        GUILine(Color.black, height);
    }

    static public void GUILine(Color color, float height = 0f)
    {
        Rect position = GUILayoutUtility.GetRect(0f, float.MaxValue, height, height, LineStyle);

        if (Event.current.type == EventType.Repaint)
        {
            Color orgColor = GUI.color;
            GUI.color = orgColor * color;
            LineStyle.Draw(position, false, false, false, false);
            GUI.color = orgColor;
        }
    }

    static public GUIStyle _LineStyle;
    static public GUIStyle LineStyle
    {
        get
        {
            if (_LineStyle == null)
            {
                _LineStyle = new GUIStyle();
                _LineStyle.normal.background = EditorGUIUtility.whiteTexture;
                _LineStyle.stretchWidth = true;
            }

            return _LineStyle;
        }
    }
    public static void ShurikenHeader(string title)
    {
        DrawShuriken(title, new Vector2(6f, -2f), 22);
    }

    public static void ShurikenHeaderCentered(string title)
    {
        DrawShurikenCenteredTitle(title, new Vector2(0f, -2f), 22);
    }

    public static void DrawShurikenPartingLineText(string title)
    {
        DrawShurikenPartingLineText(title, new Vector2(6f, -2f), 22);
    }




}
