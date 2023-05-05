using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Net.Http;

internal sealed class APITest : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    private TextField apiUrlTextField;
    private Button sendRequestbutton;

    [MenuItem("Tools/APITest")]
    public static void ShowExample()
    {
        APITest wnd = GetWindow<APITest>();
        wnd.titleContent = new GUIContent("APITest");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement visualTree = m_VisualTreeAsset.Instantiate();
        root.Add(visualTree);

        apiUrlTextField = root.Query<TextField>("APIURL");

        sendRequestbutton = root.Query<Button>("SendRequestButton");
        sendRequestbutton.clicked += OnSendRequestButtonCLick;
    }

    public async void OnSendRequestButtonCLick()
    {
        await ModelDownloadHandler.GetModel(apiUrlTextField.text);
    }

}
