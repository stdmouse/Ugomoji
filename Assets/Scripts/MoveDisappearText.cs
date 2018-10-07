using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveDisappearText : MonoBehaviour {

    public Vector3 distance;
    public float time;

    private TextMeshPro textMeshPro;
    private float startTime;
    private Color32 startColor;

    private void Awake() {
        textMeshPro = gameObject.GetComponent<TextMeshPro>();
    }

    // Use this for initialization
    void Start() {
        startTime = Time.time;
        startColor = textMeshPro.color;
    }

    // Update is called once per frame
    void Update() {
        textMeshPro.ForceMeshUpdate();

        var textInfo = textMeshPro.textInfo;
        if (textInfo.characterCount == 0) {
            return;
        }

        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        float now = (Time.time - startTime) / time;
        byte nowAlpha = (byte)Mathf.Ceil(Mathf.Lerp(startColor.a, 0, now));

        for (int i = 0; i < textInfo.characterCount; i++) {
            var charaInfo = textInfo.characterInfo[i];
            if (!charaInfo.isVisible) {
                continue;
            }

            int materialIndex = charaInfo.materialReferenceIndex;
            int vertexIndex = charaInfo.vertexIndex;

            Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // 位置の移動
            vertices[vertexIndex + 0] = Vector3.Lerp(sourceVertices[vertexIndex + 0], sourceVertices[vertexIndex + 0] + distance, now);
            vertices[vertexIndex + 1] = Vector3.Lerp(sourceVertices[vertexIndex + 1], sourceVertices[vertexIndex + 1] + distance, now);
            vertices[vertexIndex + 2] = Vector3.Lerp(sourceVertices[vertexIndex + 2], sourceVertices[vertexIndex + 2] + distance, now);
            vertices[vertexIndex + 3] = Vector3.Lerp(sourceVertices[vertexIndex + 3], sourceVertices[vertexIndex + 3] + distance, now);

            // 色の変化
            // 線形に透明にする
            Color32[] verticesColors = textInfo.meshInfo[materialIndex].colors32;
            verticesColors[vertexIndex + 0].a = nowAlpha;
            verticesColors[vertexIndex + 1].a = nowAlpha;
            verticesColors[vertexIndex + 2].a = nowAlpha;
            verticesColors[vertexIndex + 3].a = nowAlpha;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++) {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }

        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}
