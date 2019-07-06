using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalObjectSpawner : MonoBehaviour
{
    public void SpawnObject(GameObject prefab, Color color) {
        var instance = Instantiate(prefab);

        instance.transform.SetParent(this.transform, true);

        instance.transform.position = this.transform.position;
        instance.transform.forward = this.transform.forward;

        instance.GetComponent<MeshRenderer>().material.color = color;
        
    }

    
}
