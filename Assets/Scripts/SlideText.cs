using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class SlideText : MonoBehaviour {
    public float time;

    private TextMeshPro textMeshPro;
    private Vector3[] startVerticsPosition;
    private Vector3[] endVerticsPosition;
    private Vector3 initPosition;

    private float startTime;

    private void Awake() {
        textMeshPro = gameObject.GetComponent<TextMeshPro>();

        startVerticsPosition = new Vector3[textMeshPro.textInfo.characterCount * 4];
        endVerticsPosition = new Vector3[textMeshPro.textInfo.characterCount * 4];
    }

    // Use this for initialization
    void Start() {
        startTime = Time.time;

        InitText();
    }

    // Update is called once per frame
    void Update() {
        UpdateText();
    }

    /// <summary>
    /// 文字の初期化処理
    /// </summary>
    private void InitText() {
        textMeshPro.ForceMeshUpdate();

        var textInfo = textMeshPro.textInfo;
        if (textInfo.characterCount == 0) {
            return;
        }

        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        // 初期位置を保存
        for (int i = 0; i < textInfo.characterCount; i++) {
            var charaInfo = textInfo.characterInfo[i];
            if (!charaInfo.isVisible) {
                continue;
            }

            int materialIndex = charaInfo.materialReferenceIndex;
            int vertexIndex = charaInfo.vertexIndex;
            Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;

            // 1文字目の中心点へのベクトルと求める
            initPosition =
                new Vector3(
                    sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x,
                    sourceVertices[vertexIndex + 0].y + sourceVertices[vertexIndex + 2].y) * 0.5f;
            break;
        }

        for (int i = 0; i < textInfo.characterCount; i++) {
            var charaInfo = textInfo.characterInfo[i];
            if (!charaInfo.isVisible) {
                continue;
            }

            int materialIndex = charaInfo.materialReferenceIndex;
            int vertexIndex = charaInfo.vertexIndex;

            Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // 最終的に戻る位置を記録する
            endVerticsPosition[vertexIndex + 0] = sourceVertices[vertexIndex + 0];
            endVerticsPosition[vertexIndex + 1] = sourceVertices[vertexIndex + 1];
            endVerticsPosition[vertexIndex + 2] = sourceVertices[vertexIndex + 2];
            endVerticsPosition[vertexIndex + 3] = sourceVertices[vertexIndex + 3];

            // 文字を初期位置に移動
            Vector3 center = new Vector3(
                    sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x,
                    sourceVertices[vertexIndex + 0].y + sourceVertices[vertexIndex + 2].y) * 0.5f;
            vertices[vertexIndex + 0] = initPosition + vertices[vertexIndex + 0] - center;
            vertices[vertexIndex + 1] = initPosition + vertices[vertexIndex + 1] - center;
            vertices[vertexIndex + 2] = initPosition + vertices[vertexIndex + 2] - center;
            vertices[vertexIndex + 3] = initPosition + vertices[vertexIndex + 3] - center;

            // 開始位置を記録
            startVerticsPosition[vertexIndex + 0] = vertices[vertexIndex + 0];
            startVerticsPosition[vertexIndex + 1] = vertices[vertexIndex + 1];
            startVerticsPosition[vertexIndex + 2] = vertices[vertexIndex + 2];
            startVerticsPosition[vertexIndex + 3] = vertices[vertexIndex + 3];
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++) {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    /// <summary>
    /// 文字の更新処理
    /// </summary>
    private void UpdateText() {
        if (Time.time - startTime > time) {
            // アニメーション終了
            return;
        }

        textMeshPro.ForceMeshUpdate();

        var textInfo = textMeshPro.textInfo;
        if (textInfo.characterCount == 0) {
            return;
        }

        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        for (int i = 0; i < textInfo.characterCount; i++) {
            var charaInfo = textInfo.characterInfo[i];
            if (!charaInfo.isVisible) {
                continue;
            }

            int materialIndex = charaInfo.materialReferenceIndex;
            int vertexIndex = charaInfo.vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // 
            float now = (Time.time - startTime) / time;
            vertices[vertexIndex + 0] = Vector3.Lerp(startVerticsPosition[vertexIndex + 0], endVerticsPosition[vertexIndex + 0], now);
            vertices[vertexIndex + 1] = Vector3.Lerp(startVerticsPosition[vertexIndex + 1], endVerticsPosition[vertexIndex + 1], now);
            vertices[vertexIndex + 2] = Vector3.Lerp(startVerticsPosition[vertexIndex + 2], endVerticsPosition[vertexIndex + 2], now);
            vertices[vertexIndex + 3] = Vector3.Lerp(startVerticsPosition[vertexIndex + 3], endVerticsPosition[vertexIndex + 3], now);
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++) {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
