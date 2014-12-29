using UnityEngine;
using System.Collections;

public class BarrelCtrl : MonoBehaviour {

	public GameObject expEffect;

	public Texture[] _textures;

	private Transform tr;

	private int hitCount = 0;

	// Use this for initialization
	void Start ()
    {
		tr = GetComponent<Transform>();

		int idx = Random.Range (0, _textures.Length);
		GetComponentInChildren<MeshRenderer>().material.mainTexture = _textures [idx];
	}

	void OnCollisionEnter(Collision coll)
    {
		if (coll.collider.tag == "BULLET")
        {
			Destroy (coll.gameObject);

            Vector3 firePos = coll.gameObject.GetComponent<BulletCtrl>().firePos;
            Vector3 hitPos = coll.transform.position;
            HitDamage( firePos, hitPos );
            //if( ++hitCount >= 3 )
            //{
            //    StartCoroutine(this.ExplosionBarrel());
            //}
		}
	}

    void OnDamage_FromRayCast(object[] _params)
    {
        Vector3 firePos = (Vector3)_params[0];
        Vector3 hitPos = (Vector3)_params[1];

        HitDamage( firePos, hitPos );
    }

    private void HitDamage( Vector3 _firePos, Vector3 _hitPos )
    {
        Vector3 incomeVector = _hitPos - _firePos;

        incomeVector = incomeVector.normalized;
        rigidbody.AddForceAtPosition(incomeVector * 1000f, _hitPos);

        if (++hitCount >= 3)
        {
            StartCoroutine(this.ExplosionBarrel());
        }
    }

	IEnumerator ExplosionBarrel(){
		Instantiate (expEffect, tr.position, Quaternion.identity);

		Collider[] colls = Physics.OverlapSphere (tr.position, 10.0f);

		foreach (Collider coll in colls) {
			if(coll.rigidbody != null){
				coll.rigidbody.mass = 1.0f;
				coll.rigidbody.AddExplosionForce ( 800.0f, tr.position, 10.0f, 300.0f );
			}
		}

		Destroy (gameObject, 5.0f);
		yield return null;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
