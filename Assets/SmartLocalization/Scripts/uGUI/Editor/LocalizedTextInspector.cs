
namespace SmartLocalization.Editor{
    using UnityEngine;
using UnityEditor;

    [CustomEditor(typeof(LocalizedText))]
public class LocalizedTextInspector : Editor 
{
	private string selectedKey = null;
	
	void Awake()
	{
		var textObject = ((LocalizedText)target);
		if(textObject != null)
		{
			selectedKey = textObject.localizedKey;
		}
	}
	
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		
		selectedKey = LocalizedKeySelector.SelectKeyGUI(selectedKey, true, LocalizedObjectType.STRING);
		
		if(!Application.isPlaying && GUILayout.Button("Use Key", GUILayout.Width(70)))
		{
			var textObject = ((LocalizedText)target);
			textObject.localizedKey = selectedKey;
		}
	}
	
}
}