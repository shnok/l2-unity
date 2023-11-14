using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Label : MonoBehaviour {
	private bool _visible;
	private float _distance;
	private GameObject _target;

	public RectTransform _healthBar;
	public RectTransform _barContour;
	public Text _name;
	public Text _level;

	public Vector3 offset = new Vector3(0, 0.5f, 0);
	public float maxRange = 15f;

    void Start() {
		_healthBar = transform.GetChild(0).Find("HealthBar").GetComponent<RectTransform>();
		_barContour = transform.GetChild(0).Find("Contour").GetComponent<RectTransform>();
		_level = transform.GetChild(0).Find("Level").GetComponent<Text>();
		_name = transform.GetChild(0).Find("Name").GetComponent<Text>();
	}

  /*  void Update() {
		if(_target == null) {
			Destroy(gameObject);
		}

		if(World.GetInstance().mainPlayer == null)
			return;

		try {
			bool objectVisible = Camera.main.GetComponent<CameraController>().IsObjectVisible(_target);
			bool inRange = Vector3.Distance(_target.transform.position, World.GetInstance().mainPlayer.transform.position) < maxRange;

			_visible = objectVisible && inRange;

			if(_visible) {
				SkinnedMeshRenderer renderer = _target.GetComponentInChildren<SkinnedMeshRenderer>();
				float height = 0f;
				if(renderer != null) {
					height = renderer.bounds.extents.y * 2f;
                }

				transform.position = Camera.main.WorldToScreenPoint(_target.transform.position + Vector3.up * height + offset);
				transform.GetChild(0).gameObject.SetActive(true);

				Status targetStatus = _target.GetComponent<Entity>().status;
				NetworkIdentity identity = _target.GetComponent<Entity>().Identity;
				float hpPercent = (float)targetStatus.Hp / (float)targetStatus.MaxHp;
				hpPercent = Mathf.Clamp(hpPercent, 0.0f, 1.0f);
				_healthBar.sizeDelta = new Vector2(hpPercent * _barContour.sizeDelta.x, _healthBar.sizeDelta.y);
				_level.text = targetStatus.Level.ToString();
				_name.text = identity.Name;
			} else {
				transform.GetChild(0).gameObject.SetActive(false);
			}
		} catch (Exception ex) {

        }

	}

	public void SetTarget(GameObject target) {
		_target = target;
	}*/

}
