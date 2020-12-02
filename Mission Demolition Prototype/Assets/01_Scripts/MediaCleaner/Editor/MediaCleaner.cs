using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

/* MADE BY LOUIS EDBROOKE*/
public class MediaCleaner : EditorWindow
{
    private List<string> spritePathsToRemove = new List<string>();
    private List<string> texturePathsToRemove = new List<string>();
    private List<string> videosPathsToRemove = new List<string>();
    private List<string> audioPathsToRemove = new List<string>();
    private List<string> allPathsToRemove = new List<string>();
    private List<string> currentPathsToRemove = new List<string>();
    private List<string> dontremovePaths = new List<string>();

    private List<string> spriteNames = new List<string>();
    private List<string> textureNames = new List<string>();
    private List<string> videosNames = new List<string>();
    private List<string> audioNames = new List<string>();

    //private List<string> assetNames = new List<string>();

    private List<string> assetGUIDs = new List<string>();
    private List<string> spriteGUIDs = new List<string>();
    private List<string> textureGUIDs = new List<string>();
    private List<string> videosGUIDs = new List<string>();
    private List<string> audioGUIDs = new List<string>();
    private List<string> currentGUIDs = new List<string>();

    private List<string> spriteRemovalGUIDs = new List<string>();
    private List<string> textureRemovalGUIDs = new List<string>();
    private List<string> videosRemovalGUIDs = new List<string>();
    private List<string> audioRemovalGUIDs = new List<string>();
    private List<string> dontremoveGUIDs = new List<string>();

    private List<bool> spriteDeleteBools = new List<bool>();
    private List<bool> textureDeleteBools = new List<bool>();
    private List<bool> videosDeleteBools = new List<bool>();
    private List<bool> audioDeleteBools = new List<bool>();
    private List<bool> currentDeleteBools = new List<bool>();
    private List<bool> allDeleteBools = new List<bool>();

    private int tab;

    private Vector2 scrollPos;

    [MenuItem("Edit/Media Cleaner")]
    static void Init()
    {
        MediaCleaner window = (MediaCleaner)EditorWindow.GetWindow(typeof(MediaCleaner));

        window.Show();
    }

    void OnGUI()
    {
        tab = GUILayout.Toolbar(tab, new string[] {
            "Sprites ("+ GetActiveBools(spriteDeleteBools)+"/"+ spritePathsToRemove.Count+")",
            "Textures ("+ GetActiveBools(textureDeleteBools)+"/"+ texturePathsToRemove.Count+")",
            "Videos ("+ GetActiveBools(videosDeleteBools)+"/"+ videosPathsToRemove.Count+")",
            "Audio ("+ GetActiveBools(audioDeleteBools)+"/"+ audioPathsToRemove.Count+")"
        });

        // Calculate the selected ones
        allDeleteBools = new List<bool>();
        allDeleteBools.AddRange(spriteDeleteBools);
        allDeleteBools.AddRange(textureDeleteBools);
        allDeleteBools.AddRange(videosDeleteBools);
        allDeleteBools.AddRange(audioDeleteBools);

        string unusedString = "Total number of unused Assets: " + allPathsToRemove.Count;

        // Find tab type
        switch (tab)
        {
            case 0:
                // Sprites
                currentDeleteBools = spriteDeleteBools;
                currentPathsToRemove = spritePathsToRemove;
                currentGUIDs = spriteRemovalGUIDs;
                GUILayout.Label(unusedString + "  (" + FileSizeOfList(false) + ")", EditorStyles.boldLabel);
                break;
            case 1:
                // Textures
                currentDeleteBools = textureDeleteBools;
                currentPathsToRemove = texturePathsToRemove;
                currentGUIDs = textureRemovalGUIDs;
                GUILayout.Label(unusedString + "  (" + FileSizeOfList(false) + ")", EditorStyles.boldLabel);
                break;
            case 2:
                // Videos
                currentDeleteBools = videosDeleteBools;
                currentPathsToRemove = videosPathsToRemove;
                currentGUIDs = videosRemovalGUIDs;
                GUILayout.Label(unusedString + "  (" + FileSizeOfList(false) + ")", EditorStyles.boldLabel);
                break;
            case 3:
                // Audio
                currentDeleteBools = audioDeleteBools;
                currentPathsToRemove = audioPathsToRemove;
                currentGUIDs = audioRemovalGUIDs;
                GUILayout.Label(unusedString + "  (" + FileSizeOfList(false) + ")", EditorStyles.boldLabel);
                break;
        }

        #region Window set up

        // Function Buttons
        GUILayout.BeginHorizontal();
        // Create the search button
        if (GUILayout.Button("Refresh", GUILayout.Height(25f)))
        {
            RefreshList();
        }
        // Create the select all button
        if (GUILayout.Button("Select all", GUILayout.Height(25f)))
        {
            SelectAll(currentDeleteBools);
        }
        GUILayout.EndHorizontal();

        // Create the list of toggles for the unused assets
        CreateToggleList();

        GUILayout.FlexibleSpace();

        int selectedAssets = SelectedAmount();

        GUILayout.Label("Selected Assets: " + selectedAssets + "  (" + FileSizeOfList(true) + ")", EditorStyles.boldLabel);
        if (selectedAssets == 0)
        {
            GUI.backgroundColor = Color.red;
            // Create the cleanup button
            if (GUILayout.Button("No Assets selected", GUILayout.Height(42.5f)))
            {
                RemoveUnusedAssets();
            }
        }
        else
        {
            GUI.backgroundColor = Color.green;
            // Create the cleanup button
            if (GUILayout.Button("Clean selected assets!", GUILayout.Height(42.5f)))
            {
                RemoveUnusedAssets();
            }
        }


        #endregion
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    // Refreshes all tabs lists
    void RefreshList()
    {
        ResetLists();
        FindUnusedAssets();
        spriteDeleteBools = CreateBools(spritePathsToRemove);
        textureDeleteBools = CreateBools(texturePathsToRemove);
        videosDeleteBools = CreateBools(videosPathsToRemove);
        audioDeleteBools = CreateBools(audioPathsToRemove);
    }

    // Creates a list of bools
    private List<bool> CreateBools(List<string> filePaths)
    {
        List<bool> bools = new List<bool>();

        foreach (string removePath in filePaths)
        {
            bools.Add(false);
        }

        return bools;
    }

    // Creates a list of assets with toggles to signify if they should be deleted
    void CreateToggleList()
    {
        if (currentDeleteBools.Count.Equals(0))
        {
            return;
        }
        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        // Creates list from guid
        foreach (string guid in currentGUIDs)
        {
            currentDeleteBools[currentGUIDs.IndexOf(guid)] = EditorGUILayout.ToggleLeft("  " + AssetDatabase.GUIDToAssetPath(guid), currentDeleteBools[currentGUIDs.IndexOf(guid)]);
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    // 
    void SelectAll(List<bool> deleteBools)
    {
        // If all are already selected then deselect all
        bool allSelected = true;
        for (int i = 0; i < deleteBools.Count; i++)
        {
            allSelected &= deleteBools[i];
        }
        if (allSelected)
        {
            for (int i = 0; i < deleteBools.Count; i++)
            {
                deleteBools[i] = false;
            }
        }
        else
        {
            for (int i = 0; i < deleteBools.Count; i++)
            {
                deleteBools[i] = true;
            }
        }

    }

    // Loops through all the scene finding any unused assets
    void FindUnusedAssets()
    {
        // Save the current scene changes
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        // Store current scene
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string currentScenePath = null;

        string[] sceneGuid = AssetDatabase.FindAssets("t:Scene");

        List<string> spriteSceneNames = new List<string>();

        // For each scene found, open it and run the cleaner
        for (int i = 0; i < sceneGuid.Length; i++)
        {
            //Debug.Log("Scanning scene " + AssetDatabase.GUIDToAssetPath(sceneGuid[i]));
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid[i]);
            if (scenePath.Contains(currentSceneName) && currentScenePath == null)
            {
                currentScenePath = scenePath;
            }

            EditorSceneManager.OpenScene(scenePath);
            FindUsedSprites(spriteSceneNames);
            FindUsedTextures();
            FindUsedVideos();
            FindUsedAudio();
        }

        EditorSceneManager.OpenScene(currentScenePath);
    }

    void FindUsedSprites(List<string> sceneAssetNames)
    {
        //Find all sprites in scene
        foreach (GameObject obj in GetAllUserGameObjects())
        {
            // Check if it has a Image and if so then add it to the list
            if (obj.GetComponentInChildren<Image>() != null)
            {
                if (obj.GetComponentInChildren<Image>().sprite != null)
                {
                    Sprite sprite = obj.GetComponentInChildren<Image>().sprite;
                    // if they arent in the names list then add them
                    if (!sceneAssetNames.Contains(sprite.name))
                    {
                        sceneAssetNames.Add(sprite.name);
                    }
                }
            }
            // Check for sprites in animations
            if (obj.GetComponentInChildren<Animator>() != null)
            {
                if (obj.GetComponentInChildren<Animator>().runtimeAnimatorController != null)
                {
                    AnimationClip[] animationClips = obj.GetComponentInChildren<Animator>().runtimeAnimatorController.animationClips;
                    foreach (AnimationClip clip in animationClips)
                    {
                        AddSpritesFromClip(clip, sceneAssetNames);
                    }
                }
            }
        }

        // Find all the Sprites that are placed in 'Assets' folder
        string[] guids = AssetDatabase.FindAssets("t:Sprite");

        // Go through the assets and put them in the lists
        for (int i = 0; i < guids.Length; i++)
        {
            if (AssetDatabase.GUIDToAssetPath(guids[i]).Split('/')[0] == "Assets")
            {
                string assetName = AssetDatabase.GUIDToAssetPath(guids[i]).Split('.')[0];

                // Check it is a Sprite
                TextureImporter tImporter = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guids[i])) as TextureImporter;
                if (tImporter != null)
                {
                    if (tImporter.textureType.ToString() == "Sprite")
                    {
                        // Add them to the lists
                        if (!spriteNames.Contains(assetName))
                        {
                            spriteNames.Add(assetName);
                        }
                        if (!assetGUIDs.Contains(guids[i]))
                        {
                            assetGUIDs.Add(guids[i]);
                        }
                        if (!spriteGUIDs.Contains(guids[i]))
                        {
                            spriteGUIDs.Add(guids[i]);
                        }
                    }
                }
            }
        }

        // Check which ones are being used in the scene
        AddContained(spriteNames, sceneAssetNames, spritePathsToRemove, spriteGUIDs, spriteRemovalGUIDs);


    }

    void FindUsedTextures()
    {
        List<string> sceneAssetNames = new List<string>();
        List<Material> sceneAssets = new List<Material>();

        // For each object in the scene
        foreach (GameObject obj in GetAllUserGameObjects())
        {
            // Check if it has a material and if so then add it to the list
            if (obj.GetComponentInChildren<Renderer>() != null)
            {
                Material mat = obj.GetComponentInChildren<Renderer>().sharedMaterial;
                // if they arent in the names list then add them
                if (!sceneAssets.Contains(mat))
                {
                    sceneAssets.Add(mat);
                }
            }
        }

        //Loop through and add the names to the list for comparison
        foreach (Material matt in sceneAssets)
        {
            // Retrieve all textures from this material
            foreach (Texture texture in GetAllTextures(matt))
            {
                if (texture == null)
                    continue;
                // if they arent in the names list then add them
                if (!sceneAssetNames.Contains(texture.name))
                {
                    sceneAssetNames.Add(texture.name);
                }
            }
        }
        // Find all the Textures that are placed in 'Assets' folder
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");

        // Go through the assets and put them in the lists
        for (int i = 0; i < guids.Length; i++)
        {
            if (AssetDatabase.GUIDToAssetPath(guids[i]).Split('/')[0] == "Assets")
            {
                string assetName = AssetDatabase.GUIDToAssetPath(guids[i]).Split('.')[0];

                // Check it is a Sprite
                TextureImporter tImporter = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guids[i])) as TextureImporter;
                if (tImporter != null)
                {
                    if (tImporter.textureType.ToString() == "Default")
                    {
                        // Check if lists contain this and if not then add it
                        if (!textureNames.Contains(assetName))
                        {
                            textureNames.Add(assetName);
                        }
                        if (!assetGUIDs.Contains(guids[i]))
                        {
                            assetGUIDs.Add(guids[i]);
                        }
                        if (!textureGUIDs.Contains(guids[i]))
                        {
                            textureGUIDs.Add(guids[i]);
                        }
                    }
                }
            }
        }

        // Check which ones are being used in the scene
        AddContained(textureNames, sceneAssetNames, texturePathsToRemove, textureGUIDs, textureRemovalGUIDs);

    }

    void FindUsedVideos()
    {
        List<string> sceneAssetNames = new List<string>();
        List<UnityEngine.Video.VideoClip> sceneAssets = new List<UnityEngine.Video.VideoClip>();

        // For each object in the scene
        foreach (GameObject obj in GetAllUserGameObjects())
        {
            // Check if it has a material and if so then add it to the list
            if (obj.GetComponentInChildren<UnityEngine.Video.VideoPlayer>() != null)
            {
                UnityEngine.Video.VideoClip vid = obj.GetComponentInChildren<UnityEngine.Video.VideoPlayer>().clip;
                // if they arent in the names list then add them
                if (!sceneAssets.Contains(vid))
                {
                    sceneAssets.Add(vid);
                }
            }
        }

        //Loop through and add the names to the list for comparison
        foreach (UnityEngine.Video.VideoClip video in sceneAssets)
        {
            // if they arent in the names list then add them
            if (!sceneAssetNames.Contains(video.name))
            {
                sceneAssetNames.Add(video.name);
            }
        }

        // Find all the Video clips that are placed in 'Assets' folder
        string[] guids = AssetDatabase.FindAssets("t:VideoClip");

        // Go through the assets and put them in the lists
        for (int i = 0; i < guids.Length; i++)
        {
            if (AssetDatabase.GUIDToAssetPath(guids[i]).Split('/')[0] == "Assets")
            {
                string assetName = AssetDatabase.GUIDToAssetPath(guids[i]).Split('.')[0];

                // Check if lists contain this and if not then add it
                if (!videosNames.Contains(assetName))
                {
                    videosNames.Add(assetName);
                }
                if (!assetGUIDs.Contains(guids[i]))
                {
                    assetGUIDs.Add(guids[i]);
                }
                if (!videosGUIDs.Contains(guids[i]))
                {
                    videosGUIDs.Add(guids[i]);
                }
            }
        }


        // Check which ones are being used in the scene
        AddContained(videosNames, sceneAssetNames, videosPathsToRemove, videosGUIDs, videosRemovalGUIDs);

    }

    void FindUsedAudio()
    {
        List<string> sceneAssetNames = new List<string>();
        List<AudioClip> sceneAssets = new List<AudioClip>();

        // For each object in the scene
        foreach (GameObject obj in GetAllUserGameObjects())
        {
            // Check if it has a material and if so then add it to the list
            if (obj.GetComponentInChildren<AudioSource>() != null)
            {
                AudioClip audioClip = obj.GetComponentInChildren<AudioSource>().clip;
                // if they arent in the names list then add them
                if (!sceneAssets.Contains(audioClip))
                {
                    sceneAssets.Add(audioClip);
                }
            }
        }

        //Loop through and add the names to the list for comparison
        foreach (AudioClip audio in sceneAssets)
        {
            // if they arent in the names list then add them
            if (!sceneAssetNames.Contains(audio.name))
            {
                sceneAssetNames.Add(audio.name);
            }
        }

        // Find all the Audio clips that are placed in 'Assets' folder
        string[] guids = AssetDatabase.FindAssets("t:AudioClip");

        // Go through the assets and put them in the lists
        for (int i = 0; i < guids.Length; i++)
        {
            if (AssetDatabase.GUIDToAssetPath(guids[i]).Split('/')[0] == "Assets")
            {
                string assetName = AssetDatabase.GUIDToAssetPath(guids[i]).Split('.')[0];

                // Check if lists contain this and if not then add it
                if (!audioNames.Contains(assetName))
                {
                    audioNames.Add(assetName);
                }
                if (!assetGUIDs.Contains(guids[i]))
                {
                    assetGUIDs.Add(guids[i]);
                }
                if (!audioGUIDs.Contains(guids[i]))
                {
                    audioGUIDs.Add(guids[i]);
                }
            }
        }

        // Check which ones are being used in the scene
        AddContained(audioNames, sceneAssetNames, audioPathsToRemove, audioGUIDs, audioRemovalGUIDs);

    }

    void RemoveUnusedAssets()
    {
        // Save the current scene
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        int remove = 0;
        // Loop through the images to remove and if their corresponding bool is true then remove them
        foreach (string removalPath in allPathsToRemove)
        {
            if (allDeleteBools[allPathsToRemove.IndexOf(removalPath)])
            {
                AssetDatabase.DeleteAsset(removalPath);
                remove++;
            }
        }

        Debug.Log("Media cleaner has removed " + remove + "/" + allPathsToRemove.Count + " unused assets.");
        RefreshList();
    }

    string UnitConverter(long bytes)
    {
        int typeCounter = 0;
        string[] suffix = { "B", "KB", "MB", "GB" };
        float convertedFloat = (float)bytes;
        while (convertedFloat > 1024)
        {
            convertedFloat = convertedFloat / 1024;
            typeCounter++;
        }

        string covertedString = convertedFloat.ToString("f2") + suffix[typeCounter];
        return covertedString;
    }

    private string FileSizeOfList(bool selectedOnly)
    {
        //return "0";
        long byteTotal = 0;

        if (selectedOnly)
        {
            for (int i = 0; i < currentPathsToRemove.Count; i++)
            {
                if (currentDeleteBools[i])
                {
                    var fileInfo = new System.IO.FileInfo(currentPathsToRemove[i]);
                    byteTotal += fileInfo.Length;
                }
            }
        }
        else
        {
            if (allDeleteBools.Count != allPathsToRemove.Count)
            {
                Debug.LogError("Paths and bools are out of sync.");
            }
            for (int i = 0; i < allPathsToRemove.Count; i++)
            {
                try
                {
                    var fileInfo = new System.IO.FileInfo(allPathsToRemove[i]);
                    byteTotal += fileInfo.Length;
                }
                catch (System.IO.FileNotFoundException)
                {
                    byteTotal = 0;
                    Debug.LogWarning("Media Cleaner failed to locate a file.");
                }
            }
        }


        return UnitConverter(byteTotal);
    }

    private void ResetLists()
    {
        allPathsToRemove = new List<string>();
        currentPathsToRemove = new List<string>();
        spritePathsToRemove = new List<string>();
        texturePathsToRemove = new List<string>();
        videosPathsToRemove = new List<string>();
        audioPathsToRemove = new List<string>();

        spriteDeleteBools = new List<bool>();
        textureDeleteBools = new List<bool>();
        videosDeleteBools = new List<bool>();
        audioDeleteBools = new List<bool>();
        currentDeleteBools = new List<bool>();
        allDeleteBools = new List<bool>();

        spriteNames = new List<string>();
        textureNames = new List<string>();
        videosNames = new List<string>();
        audioNames = new List<string>();


        assetGUIDs = new List<string>();
        spriteGUIDs = new List<string>();
        textureGUIDs = new List<string>();
        videosGUIDs = new List<string>();
        audioGUIDs = new List<string>();

        spriteRemovalGUIDs = new List<string>();
        textureRemovalGUIDs = new List<string>();
        videosRemovalGUIDs = new List<string>();
        audioRemovalGUIDs = new List<string>();
    }

    // Checks if a material contains a texture
    private List<Texture> GetAllTextures(Material mat)
    {
        List<Texture> allTexture = new List<Texture>();
        Shader shader = mat.shader;

        for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
        {
            if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
            {
                Texture texture = mat.GetTexture(ShaderUtil.GetPropertyName(shader, i));
                allTexture.Add(texture);
            }
        }

        return allTexture;
    }

    // Returns all player objects in scene including inactive ones
    private List<GameObject> GetAllUserGameObjects()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        List<GameObject> sceneObjects = new List<GameObject>();

        // Checks for internal flag
        for (int i = 0; i < allObjects.Length; i++)
        {
            if (allObjects[i].hideFlags != HideFlags.HideAndDontSave)
            {
                sceneObjects.Add(allObjects[i]);
            }
        }

        return sceneObjects;
    }

    // Adds the unused assets to the paths to remove
    private void AddContained(List<string> assetNames, List<string> sceneNames, List<string> pathsToRemove, List<string> guids, List<string> removalGUIDS)
    {
        // Add based on guid
        foreach (string thisName in assetNames)
        {
            string splitName1 = thisName.Split('.')[0];
            string splitName2 = thisName.Split('/')[thisName.Split('/').Length - 1];
            bool contained = false;
            for (int i = 0; i < sceneNames.Count; i++)
            {
                if (splitName2 == sceneNames[i])
                {
                    contained = true;
                }
            }
            // If it isnt contained in the scene then add it to the list of items that should be removed
            if (!contained)
            {
                if (!removalGUIDS.Contains(guids[assetNames.IndexOf(thisName)]) && !dontremoveGUIDs.Contains(guids[assetNames.IndexOf(thisName)]))
                {
                    removalGUIDS.Add(guids[assetNames.IndexOf(thisName)]);
                }
                if (!pathsToRemove.Contains(AssetDatabase.GUIDToAssetPath(guids[assetNames.IndexOf(thisName)])) && !dontremovePaths.Contains(AssetDatabase.GUIDToAssetPath(guids[assetNames.IndexOf(thisName)])))
                {
                    pathsToRemove.Add(AssetDatabase.GUIDToAssetPath(guids[assetNames.IndexOf(thisName)]));
                }
                if (!allPathsToRemove.Contains(AssetDatabase.GUIDToAssetPath(guids[assetNames.IndexOf(thisName)])) && !dontremovePaths.Contains(AssetDatabase.GUIDToAssetPath(guids[assetNames.IndexOf(thisName)])))
                {
                    allPathsToRemove.Add(AssetDatabase.GUIDToAssetPath(guids[assetNames.IndexOf(thisName)]));
                }
            }
            // If it is contained make sure it hasnt been added to the lists
            else
            {
                // Protect it
                dontremoveGUIDs.Add(guids[assetNames.IndexOf(thisName)]);
                dontremovePaths.Add(AssetDatabase.GUIDToAssetPath(guids[assetNames.IndexOf(thisName)]));
                dontremovePaths.Add(AssetDatabase.GUIDToAssetPath(guids[assetNames.IndexOf(thisName)]));

                if (removalGUIDS.Contains(guids[assetNames.IndexOf(thisName)]))
                {
                    removalGUIDS.Remove(guids[assetNames.IndexOf(thisName)]);
                }
                if (pathsToRemove.Contains(AssetDatabase.GUIDToAssetPath(guids[assetNames.IndexOf(thisName)])))
                {
                    pathsToRemove.Remove(AssetDatabase.GUIDToAssetPath(guids[assetNames.IndexOf(thisName)]));
                }
                if (allPathsToRemove.Contains(AssetDatabase.GUIDToAssetPath(guids[assetNames.IndexOf(thisName)])))
                {
                    allPathsToRemove.Remove(AssetDatabase.GUIDToAssetPath(guids[assetNames.IndexOf(thisName)]));
                }
            }
        }
    }

    // Checks every frame of an animation clip to make sure there are no sprites
    private void AddSpritesFromClip(AnimationClip clip, List<string> sprites)
    {
        if (clip != null)
        {
            foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            {
                var keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                foreach (var frame in keyframes)
                {
                    if (frame.value != null)
                    {
                        if (!sprites.Contains(((Sprite)frame.value).name))
                        {
                            sprites.Add(((Sprite)frame.value).name);
                        }
                    }
                }
            }
        }
    }

    // Checks how many are selected on each tab
    private int SelectedAmount()
    {
        int amount = 0;

        foreach (bool delete in spriteDeleteBools)
        {
            if (delete)
            {
                amount++;
            }
        }
        foreach (bool delete in textureDeleteBools)
        {
            if (delete)
            {
                amount++;
            }
        }
        foreach (bool delete in videosDeleteBools)
        {
            if (delete)
            {
                amount++;
            }
        }
        foreach (bool delete in audioDeleteBools)
        {
            if (delete)
            {
                amount++;
            }
        }

        return amount;
    }

    private int GetActiveBools(List<bool> bools)
    {
        int actives = 0;

        foreach (bool boolean in bools)
        {
            if (boolean)
            {
                actives++;
            }
        }

        return actives;
    }
}
