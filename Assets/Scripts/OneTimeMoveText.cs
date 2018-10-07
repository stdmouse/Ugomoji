using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OneTimeMoveText : MonoBehaviour
{

    public float speed;
    public float distance;

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

            // 2関数の0以下を切り捨ててるので、1つの山形になる
            // distance/2を引いているのは(x,y)=(0,0)とするため
            float t = now * speed - index * 0.1f - Mathf.Sqrt(distance);
            float moveDistance = Mathf.Max(-1.0f * Mathf.Pow(t, 2) + distance, 0);

            vertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] + Vector3.up * moveDistance;
            vertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] + Vector3.up * moveDistance;
            vertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] + Vector3.up * moveDistance;
            vertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] + Vector3.up * moveDistance;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++) {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}