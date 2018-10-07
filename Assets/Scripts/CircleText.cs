using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CircleText : MonoBehaviour {

    public float radius;
    public float radiusSpeed;

    private RectTransform rectTransform;
    private TextMeshPro textMeshPro;
    private float nowDegree;

    private void Awake() {
        rectTransform = gameObject.GetComponent<RectTransform>();
        textMeshPro = gameObject.GetComponent<TextMeshPro>();
    }

    // Use this for initialization
    void Start() {
        PutAroundCircle(0);
    }

    // Update is called once per frame
    void Update() {
        if (radiusSpeed > 0) {
            nowDegree = Mathf.Repeat(nowDegree + radiusSpeed, 360);

            PutAroundCircle(nowDegree);
        } else if (radiusSpeed < 0) {
            nowDegree = Mathf.Repeat(nowDegree + -1.0f * radiusSpeed, 360);

            PutAroundCircle(-1.0f * nowDegree);
        }
    }

    private void PutAroundCircle(float baseDegree) {
        textMeshPro.ForceMeshUpdate();

        var textInfo = textMeshPro.textInfo;
        if (textInfo.characterCount == 0) {
            return;
        }

        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        // 文字ごとの角度
        float degreeByCharactor = 360f / textInfo.characterCount;

        float nowCharactorDegree = baseDegree;
        Matrix4x4 matrix;
        for (int index = 0; index < textInfo.characterCount; index++) {
            var charaInfo = textInfo.characterInfo[index];
            if (!charaInfo.isVisible) {
                continue;
            }

            int materialIndex = charaInfo.materialReferenceIndex;
            int vertexIndex = charaInfo.vertexIndex;

            Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // 円の0度の位置を求める
            Vector3 zeroDegreePoint = Vector3.up * radius;
            Vector3 moveVector = zeroDegreePoint - 0.5f * (vertices[vertexIndex + 2] + vertices[vertexIndex + 0]);
            vertices[vertexIndex + 0] += moveVector;
            vertices[vertexIndex + 1] += moveVector;
            vertices[vertexIndex + 2] += moveVector;
            vertices[vertexIndex + 3] += moveVector;

            // 文字ごとの角度分だけ回転させる
            matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, nowCharactorDegree));
            vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
            vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
            vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
            vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);

            // 時計回りに文字を並べるために引く
            nowCharactorDegree -= degreeByCharactor;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++) {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
