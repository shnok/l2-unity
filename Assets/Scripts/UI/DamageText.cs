using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlotatingText {
    public string value;
    public Color color;
    public Vector2 offset;
    public int fontSize;
    public Transform master;

    public FlotatingText(string value, Color color, Vector2 offset, int fontSize, Transform master) {
        this.value = value;
        this.color = color;
        this.offset = offset;
        this.fontSize = fontSize;
        this.master = master;
    }
}

public class DamageText : MonoBehaviour {

    public FlotatingText myText;

    void Start() {
        transform.SetParent(GameObject.Find("Damages").transform);
        StartCoroutine("FlotatingText");
    }

    public IEnumerator FlotatingText() {

        while(myText == null)
            yield return null;

        float startTime = Time.time;
        float y = 0.5f;
        Vector3 randomPos = new Vector3(myText.offset.x + Random.Range(-20, 20f), myText.offset.y + Random.Range(-10, 10f), 0);
        Color textColor = myText.color;
        GetComponent<Text>().text = myText.value;
        GetComponent<Text>().fontSize = myText.fontSize;

        while(Time.time - startTime < 1f) {
            y += Time.deltaTime * 1.5f;
            transform.position = Camera.main.WorldToScreenPoint(myText.master.position + Vector3.up * y) + randomPos;
            if(Time.time - startTime > 0.5)
                textColor.a -= Time.deltaTime * 2f;
            GetComponent<Text>().color = textColor;
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
