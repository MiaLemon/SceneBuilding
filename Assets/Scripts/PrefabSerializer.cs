using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SceneBuilder
{
    class Prefab
    {
        public string name;
        public Vector3 scale;
        public string tag;
        public int layer;
        public bool isStatic;

        public List<GameObject> children;
        public List<Component> components;
    }

    [System.Serializable]
    class GameObject
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public string tag;
        public int layer;
        public bool isStatic;

        public List<Component> components;
    }

    public class PrefabSerializer
    {
        public static string Save(UnityEngine.GameObject prefabOriginal)
        {
            Prefab prefabCopy = new Prefab();
            prefabCopy.name = prefabOriginal.name;
            prefabCopy.scale = prefabOriginal.transform.localScale;
            prefabCopy.tag = prefabOriginal.tag;
            prefabCopy.layer = prefabOriginal.layer;
            prefabCopy.isStatic = prefabOriginal.isStatic;

            prefabCopy.children = TranslateGameObjects(prefabOriginal);
            prefabCopy.components = TranslateComponents(prefabOriginal);

            return JsonUtility.ToJson(prefabCopy, true);
        }

        private static List<GameObject> TranslateGameObjects(UnityEngine.GameObject gameObjectParent)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            UnityEngine.Transform[] children = gameObjectParent.GetComponentsInChildren<UnityEngine.Transform>();

            for (int i = 0; i < children.Length; i++)
            {
                UnityEngine.GameObject gameObjectOriginal = children[i].gameObject;
                GameObject gameObjectCopy = new GameObject();

                gameObjectCopy.name = gameObjectOriginal.name;

                gameObjectCopy.position = gameObjectOriginal.transform.position;
                gameObjectCopy.rotation = gameObjectOriginal.transform.rotation;

                //compensate for base parents scale
                Vector3 scale = new Vector3(gameObjectOriginal.transform.lossyScale.x/gameObjectParent.transform.localScale.x,
                                            gameObjectOriginal.transform.lossyScale.y / gameObjectParent.transform.localScale.y,
                                            gameObjectOriginal.transform.lossyScale.z / gameObjectParent.transform.localScale.z);
                gameObjectCopy.scale = scale;

                gameObjectCopy.tag = gameObjectOriginal.tag;
                gameObjectCopy.layer = gameObjectOriginal.layer;
                gameObjectCopy.isStatic = gameObjectOriginal.isStatic;

                gameObjectCopy.components = TranslateComponents(gameObjectOriginal);

                gameObjects.Add(gameObjectCopy);
            }

            return gameObjects;
        }

        private static List<Component> TranslateComponents(UnityEngine.GameObject gameObject)
        {
            return new List<Component>(gameObject.GetComponents<Component>());
        }

        public static UnityEngine.GameObject Load(string json)
        {
            UnityEngine.GameObject prefabScene = new UnityEngine.GameObject();

            Prefab prefabJson = JsonUtility.FromJson<Prefab>(json);

            prefabScene.name = prefabJson.name;
            prefabScene.transform.localScale = prefabJson.scale;
            prefabScene.tag = prefabJson.tag;
            prefabScene.layer = prefabJson.layer;
            prefabScene.isStatic = prefabJson.isStatic;

            RecreateGameObjects(prefabScene, prefabJson.children);

            AddComponents(prefabScene, prefabJson.components);

            return prefabScene;
        }

        static void AddComponents(UnityEngine.GameObject prefabScene, List<Component> components)
        {
            for (int i = 0; i < components.Count; i++)
            {
                //TODO use reflection instead since this can't be used runtime


                if (UnityEditorInternal.ComponentUtility.CopyComponent(components[i]))
                {
                    if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(prefabScene))
                    {
                        //Debug.Log("Success");
                    }
                }
            }
        }

        static void RecreateGameObjects(UnityEngine.GameObject prefabScene, List<GameObject> children)
        {
            for (int i = 0; i < children.Count; i++)
            {
                GameObject gameObjectJson = children[i];

                UnityEngine.GameObject gameObjectChildInScene = new UnityEngine.GameObject();
                gameObjectChildInScene.transform.parent = prefabScene.transform;

                gameObjectChildInScene.name = gameObjectJson.name;

                //Fix correct local pos/rot/scale
                gameObjectChildInScene.transform.position = gameObjectJson.position;
                gameObjectChildInScene.transform.rotation = gameObjectJson.rotation;
                gameObjectChildInScene.transform.localScale = gameObjectJson.scale;

                gameObjectChildInScene.tag = gameObjectJson.tag;
                gameObjectChildInScene.layer = gameObjectJson.layer;
                gameObjectChildInScene.isStatic = gameObjectJson.isStatic;

                AddComponents(gameObjectChildInScene, gameObjectJson.components);
            }
        }
    }
}
