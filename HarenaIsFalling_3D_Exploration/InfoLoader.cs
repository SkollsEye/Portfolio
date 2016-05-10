using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Xml;

/// <summary>
/// Author: Mark DiVelbiss
/// The Info Loader is attached to an inactive object until the user requests to read a letter (usually due to some letter interactable).
/// Once active, the Letters.xml is extracted and the correct letter text is inserted into the Text components attached to the linked gameobjects letterNameObject and letterObject.
/// </summary>
public class InfoLoader : MonoBehaviour {
    public GameObject letterNameObject;
    public GameObject letterObject;

    const string file = "Letters";
    const string xmlRootName = "Letters";

    void OnEnable()
    {
        TextAsset textAsset = (TextAsset)Resources.Load(file);

        // Attempt to retrieve the XML
        XmlDocument xml = new XmlDocument();
        try { xml.LoadXml(textAsset.text); }
        catch (Exception e) { print(file + ".xml not found"); return; }

        // Attempt to retrieve the Text components
        Text nameText, letterText;
        try { nameText = letterNameObject.GetComponent<Text>(); letterText = letterObject.GetComponent<Text>(); }
        catch (Exception e) { print("Text Game Objects are not attached to Info Loader."); return; }

        // Initialize Variables for search
        string nodeName = GameManager.Instance.CurrentInfoLetter;
        string nameString = "", letterString = "";
        XmlNodeList list = xml.ChildNodes;

        // Make sure you are at the correct set of child nodes
        if (list.Count > 1 && String.Compare(list[1].Name, xmlRootName, false) == 0) list = list[1].ChildNodes;
        else return;

        // Loop until you find the node name indicated by the GameManager, then store those values
        for (int i=0; i < list.Count; i++)
        {
            if (String.Compare(list[i].Name, nodeName, false) == 0)
            {
                if (list[i].Attributes.Count > 0) nameString = list[i].Attributes[0].Value;
                if (list[i].ChildNodes.Count > 0) letterString = list[i].FirstChild.Value;
                break;
            }
        }

        // Assign text values to Text components.
        nameText.text = nameString;
        letterText.text = letterString;
    }
}
