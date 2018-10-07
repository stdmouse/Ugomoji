using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoopMoveText : MonoBehaviour {

    public float distance;
    public float speed;
    public float charaOffset;

    private TextMeshPro textMeshPro;

    private void Awake() {
        textMeshPro = gameObject.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update() {
        textMeshPro.ForceMeshUpdate();

        var textInfo = textMeshPro.textInfo;
        if (textInfo.characterCount == 0) {
            return;
        }

        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
        float now = Time.time;

        for (int i = 0; i < textInfo.characterCount; i++) {
            var charaInfo = textInfo.characterInfo[i];
            if (!charaInfo.isVisible) {
                continue;
            }

            int materialIndex = charaInfo.materialReferenceIndex;
            int vertexIndex = charaInfo.vertexIndex;

            Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            Vector3 tmpVector = distance * Vector3.up * Mathf.Sin((now * speed + charaOffset * i));
            vertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] + tmpVector;
            vertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] + tmpVector;
            vertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] + tmpVector;
            vertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] + tmpVector;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++) {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
