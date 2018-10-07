using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoundText : MonoBehaviour{

    public float distance;
    public float charaOffset;
    public float animationSpeedMultiplier;

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

        for (int index = 0; index < textInfo.characterCount; index++) {
            var charaInfo = textInfo.characterInfo[index];
            if (!charaInfo.isVisible) {
                continue;
            }

            int materialIndex = charaInfo.materialReferenceIndex;
            int vertexIndex = charaInfo.vertexIndex;

            Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            float sinValue = Mathf.Clamp01(Mathf.Sin(now * animationSpeedMultiplier));
            float cosValue = Mathf.Clamp01(Mathf.Sin(now * animationSpeedMultiplier + Mathf.PI));

            Vector3[] values = {
                distance * (Vector3.down * sinValue + Vector3.left * cosValue),
                distance * (Vector3.up * sinValue + Vector3.left * cosValue),
                distance * (Vector3.up * sinValue + Vector3.right * cosValue),
                distance * (Vector3.down * sinValue + Vector3.right * cosValue),
            };

            vertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] + values[0];
            vertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] + values[1];
            vertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] + values[2];
            vertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] + values[3];
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++) {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
