using UnityEngine;
using System.Collections;

public class WallCtrl : MonoBehaviour {

	public GameObject sparkEffect;

	void OnCollisionEnter(Collision coll){
		if (coll.collider.tag == "BULLET") {
			Vector3 firePos = coll.gameObject.GetComponent<BulletCtrl>().firePos;
			Vector3 relativePos = firePos - coll.transform.position;

			Object obj = Instantiate (sparkEffect, coll.transform.position, Quaternion.LookRotation(relativePos));
			Destroy ( obj, 2.0f );
			Destroy (coll.gameObject);
		}
	}
}