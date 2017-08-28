using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MathBeat.Game
{
    public class ObjectPool : MonoBehaviour
    {
        ///<summary>The prefab that this object pool returns instances of</summary> 
        public GameObject prefab;
        ///<summary>Collection of currently inactive instances of the prefab</summary>
        private Stack<GameObject> cachedObj = new Stack<GameObject>();

        ///<summary>Returns an instance of the prefab</summary>
        public GameObject GetObject()
        {
            GameObject spawnedGameObject;

            // if there is an inactive instance of the prefab ready to return, return that
            if (cachedObj.Count > 0)
            {
                // remove the instance from the collection of inactive instances
                spawnedGameObject = cachedObj.Pop();
            }
            // otherwise, create a new instance
            else
            {
                spawnedGameObject = Instantiate(prefab, transform);

                // add the PooledObject component to the prefab so we know it came from this pool
                PooledObject pooledObject = spawnedGameObject.AddComponent<PooledObject>();
                pooledObject.pool = this;
            }

            // enable the instance
            spawnedGameObject.SetActive(true);

            // return a reference to the instance
            // to be used later
            return spawnedGameObject;
        }

        ///<summary>Returns an instance of the prefab</summary>
        public GameObject GetObject(float position)
        {
            GameObject spawnedGameObject;

            // if there is an inactive instance of the prefab ready to return, return that
            if (cachedObj.Count > 0)
            {
                // remove the instance from the collection of inactive instances
                spawnedGameObject = cachedObj.Pop();
                spawnedGameObject.transform.position = new Vector3(position, transform.position.y);
            }
            // otherwise, create a new instance
            else
            {
                spawnedGameObject = Instantiate(prefab, new Vector3(position, transform.position.y), prefab.transform.rotation, transform);

                // add the PooledObject component to the prefab so we know it came from this pool
                PooledObject pooledObject = spawnedGameObject.AddComponent<PooledObject>();
                pooledObject.pool = this;
            }

            // enable the instance
            spawnedGameObject.SetActive(true);


            // return a reference to the instance
            return spawnedGameObject;
        }

        ///<summary>Return an instance of the prefab to the pool</summary>
        public void ReturnObject(GameObject toReturn)
        {
            PooledObject pooledObject = toReturn.GetComponent<PooledObject>();

            // if the instance came from this pool, return it to the pool
            if (pooledObject != null && pooledObject.pool == this)
            {
                // resets toReturn to its default position
                // aka the prefab position
                toReturn.transform.position = prefab.transform.position;

                // disable the instance
                toReturn.SetActive(false);

                // add the instance to the collection of inactive instances
                cachedObj.Push(toReturn);
            }
            // otherwise, just destroy it
            else
            {
                Debug.LogWarning(toReturn.name + " was returned to a pool it wasn't spawned from! Destroying.");
                Destroy(toReturn);
            }
        }
    }

    public class PooledObject : MonoBehaviour
    {
        public ObjectPool pool;
    }
}
