using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilBag : MonoBehaviour
{
    public Soil soil = Soil.dirt;

    public Transform stickyNotePosition;

    public GameObject[] stickyNoteModels;

    public Material stickyNoteBoneChar;
    public Material stickyNoteTar;
    public Material stickyNoteAsh;
    public Material stickyNoteSand;
    public Material stickyNoteDirt;

    private void Awake() {
        
        GameObject noteModel = stickyNoteModels[Random.Range(0, stickyNoteModels.Length)];

        Material noteMaterial = null;

        switch (soil) {
            case Soil.ash:
            noteMaterial = stickyNoteAsh;
            break;
            case Soil.bonechar:
            noteMaterial = stickyNoteBoneChar;
            break;
            case Soil.dirt:
            noteMaterial = stickyNoteDirt;
            break;
            case Soil.sand:
            noteMaterial = stickyNoteSand;
            break;
            case Soil.tar:
            noteMaterial = stickyNoteTar;
            break;
        }

        
        var note = Instantiate(noteModel);

        note.transform.SetParent(stickyNotePosition, true);
        note.transform.position = stickyNotePosition.position;
        note.transform.rotation = stickyNotePosition.rotation;

        note.GetComponent<MeshRenderer>().material = noteMaterial;

    }

    private Soil[] soils = (Soil[])System.Enum.GetValues(typeof(Soil));
    public int contents
    {
        get
        {
            for (int i = 0; i < soils.Length; i++)
            {
                if (soils[i] == soil)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
