using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColorMoveText : MonoBehaviour {


    public Color32 accentColor;
    public Color32 baseColor;
    public float speed;

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

        Color32 nowColor = accentColor;
        int characterCount = textInfo.characterCount;

        for (int index = 0; index < textInfo.characterCount; index++) {
            var charaInfo = textInfo.characterInfo[index];
            if (!charaInfo.isVisible) {
                continue;
            }

            int materialIndex = charaInfo.materialReferenceIndex;
            int vertexIndex = charaInfo.vertexIndex;

            // 色の変化
            Color32[] verticesColors = textInfo.meshInfo[materialIndex].colors32;

            // sinの周期で0,1となる階段関数
            float t = Mathf.Repeat(Time.time * speed + index * 0.1f, 1);
            float now = Mathf.RoundToInt(-Mathf.Pow(2 * t - 1, 2) + 1);
            verticesColors[vertexIndex + 0] = Color32.Lerp(accentColor, baseColor, now);
            verticesColors[vertexIndex + 1] = Color32.Lerp(accentColor, baseColor, now);
            verticesColors[vertexIndex + 2] = Color32.Lerp(accentColor, baseColor, now);
            verticesColors[vertexIndex + 3] = Color32.Lerp(accentColor, baseColor, now);
        }

        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}
