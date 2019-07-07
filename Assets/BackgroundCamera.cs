using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCamera : MonoBehaviour
{
    private void LateUpdate() {
        transform.rotation = Camera.main.transform.rotation;
    }
}
