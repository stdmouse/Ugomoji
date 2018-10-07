using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class PurseText : MonoBehaviour
{

    public AnimationCurve topCurve;
    public AnimationCurve bottomCurve;
    public float scale;

    private TextMeshPro textMeshPro;

    private void Awake() {
        textMeshPro = gameObject.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update() {
        UpdateText();
    }

    private void UpdateText() {
        textMeshPro.ForceMeshUpdate();

        var textInfo = textMeshPro.textInfo;
        if (textInfo.characterCount == 0) {
            return;
        }

        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        for (int index = 0; index < textInfo.characterCount; index++) {
            var charaInfo = textInfo.characterInfo[index];
            if (!charaInfo.isVisible) {
                continue;
            }

            int materialIndex = charaInfo.materialReferenceIndex;
            int vertexIndex = charaInfo.vertexIndex;

            Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // 曲線に沿って各頂点の位置を移動
            float x = (float)index / (textInfo.characterCount - 1);
            float bottomY = bottomCurve.Evaluate(x) * scale;
            float topY = topCurve.Evaluate(x) * scale;

            vertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] + Vector3.up * bottomY;
            vertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] + Vector3.up * topY;
            vertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] + Vector3.up * topY;
            vertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] + Vector3.up * bottomY;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++) {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
