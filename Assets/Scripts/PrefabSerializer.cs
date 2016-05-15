using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SceneBuilder
{
    class Prefab
    {
        public Vector3 scale;
        public string tag;
        public int layer;
        public bool isStatic;

        public List<GameObject> gameObjects;
        public List<MonoBehaviour> components;
    }

    [System.Serializable]
    class GameObject
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public string tag;
        public int layer;
        public bool isStatic;

        public List<MonoBehaviour> components;
    }

    public class PrefabSerializer
    {
        public static string Save(UnityEngine.GameObject prefabOriginal)
        {
            Prefab prefabCopy = new Prefab();
            prefabCopy.scale = prefabOriginal.transform.localScale;
            prefabCopy.tag = prefabOriginal.tag;
            prefabCopy.layer = prefabOriginal.layer;
            prefabCopy.isStatic = prefabOriginal.isStatic;

            prefabCopy.gameObjects = TranslateGameObjects(prefabOriginal);
            prefabCopy.components = TranslateMonoBehaviours(prefabOriginal);

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

                //Fix correct local pos/rot/scale
                gameObjectCopy.position = gameObjectOriginal.transform.position;
                gameObjectCopy.rotation = gameObjectOriginal.transform.rotation;
                gameObjectCopy.scale = gameObjectOriginal.transform.lossyScale;

                gameObjectCopy.tag = gameObjectOriginal.tag;
                gameObjectCopy.layer = gameObjectOriginal.layer;
                gameObjectCopy.isStatic = gameObjectOriginal.isStatic;

                gameObjectCopy.components = TranslateMonoBehaviours(gameObjectOriginal);

                gameObjects.Add(gameObjectCopy);
            }

            return gameObjects;
        }

        private static List<MonoBehaviour> TranslateMonoBehaviours(UnityEngine.GameObject gameObject)
        {
            return new List<MonoBehaviour>(gameObject.GetComponents<MonoBehaviour>());
        }

        public static void Load()
        {

        }
    }
}
