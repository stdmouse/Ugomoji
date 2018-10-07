using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RondomPositionAndReturnText : MonoBehaviour {

    public float time;
    public float randomAreaSize;

    private TextMeshPro textMeshPro;
    private float startTime;
    private Vector3[] initVerticesPosition;
    private Vector3[] startVerticesPosition;

    private void Awake() {
        textMeshPro = gameObject.GetComponent<TextMeshPro>();
        initVerticesPosition = new Vector3[textMeshPro.textInfo.characterCount * 4];
        startVerticesPosition = new Vector3[textMeshPro.textInfo.characterCount * 4];
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
        Debug.Log("init text");

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

            Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // 戻る位置を記録する
            initVerticesPosition[vertexIndex + 0] = vertices[vertexIndex + 0];
            initVerticesPosition[vertexIndex + 1] = vertices[vertexIndex + 1];
            initVerticesPosition[vertexIndex + 2] = vertices[vertexIndex + 2];
            initVerticesPosition[vertexIndex + 3] = vertices[vertexIndex + 3];

            // バラバラの場所に移動
            Vector3 randomPosition =
                new Vector3(
                    Random.Range(-1.0f * randomAreaSize, randomAreaSize),
                    Random.Range(-1.0f * randomAreaSize, randomAreaSize),
                    0f);

            vertices[vertexIndex + 0] += randomPosition;
            vertices[vertexIndex + 1] += randomPosition;
            vertices[vertexIndex + 2] += randomPosition;
            vertices[vertexIndex + 3] += randomPosition;

            startVerticesPosition[vertexIndex + 0] = vertices[vertexIndex + 0];
            startVerticesPosition[vertexIndex + 1] = vertices[vertexIndex + 1];
            startVerticesPosition[vertexIndex + 2] = vertices[vertexIndex + 2];
            startVerticesPosition[vertexIndex + 3] = vertices[vertexIndex + 3];
        }

        //for (int i = 0; i < textInfo.meshInfo.Length; i++) {
        //    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
        //    textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        //}

        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }


    /// <summary>
    /// 文字の更新処理
    /// </summary>
    private void UpdateText() {
        if (Time.time - startTime > time) {
            // アニメーション終了
            return;
        }

        Debug.Log("update text");
        textMeshPro.ForceMeshUpdate();

        var textInfo = textMeshPro.textInfo;
        if (textInfo.characterCount == 0) {
            return;
        }

        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        float now = (Time.time - startTime) / time;

        for (int i = 0; i < textInfo.characterCount; i++) {
            var charaInfo = textInfo.characterInfo[i];
            if (!charaInfo.isVisible) {
                continue;
            }

            int materialIndex = charaInfo.materialReferenceIndex;
            int vertexIndex = charaInfo.vertexIndex;

            Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // もともとの位置に戻す
            vertices[vertexIndex + 0] = Vector3.Lerp(startVerticesPosition[vertexIndex + 0], initVerticesPosition[vertexIndex + 0], now);
            vertices[vertexIndex + 1] = Vector3.Lerp(startVerticesPosition[vertexIndex + 1], initVerticesPosition[vertexIndex + 1], now);
            vertices[vertexIndex + 2] = Vector3.Lerp(startVerticesPosition[vertexIndex + 2], initVerticesPosition[vertexIndex + 2], now);
            vertices[vertexIndex + 3] = Vector3.Lerp(startVerticesPosition[vertexIndex + 3], initVerticesPosition[vertexIndex + 3], now);
        }

        //for (int i = 0; i < textInfo.meshInfo.Length; i++) {
        //    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
        //    textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        //}

        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
}
