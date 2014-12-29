using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour {

	public GameObject bullet;
	public Transform firePos;
	public AudioClip fireSfx;
	public MeshRenderer _renderer;

    private GameMgr _gameMgr;
    private float _rayDistance = 10.0f;

	// Use this for initialization
	void Start ()
    {
		_renderer.enabled = false;
        _gameMgr = GameObject.Find("GameManager").GetComponent<GameMgr>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.DrawRay(firePos.position, firePos.forward * _rayDistance, Color.green);

		if (Input.GetMouseButtonDown (0))
        {
			//Fire_Bullet();
            Fire_Raycast();

            StartCoroutine(this.ShowMuzzleFlash());
            StartCoroutine(this.PlaySfx(fireSfx));
		}
	}

    private void Fire_Bullet()
    {
		StartCoroutine (this.CreateBullet());
	}

    private void Fire_Raycast()
    {
        RaycastHit hit;

        if (Physics.Raycast(firePos.position, firePos.forward, out hit, _rayDistance))
        {
            if (hit.collider.tag == "MONSTER")
            {
                object[] _params = new object[2];
                _params[0] = hit.point;
                _params[1] = 20;

                hit.collider.gameObject.SendMessage("OnDamage_FromRayCast", _params, SendMessageOptions.DontRequireReceiver);
            }
            else if (hit.collider.tag == "BARREL")
            {
                object[] _params = new object[2];
                _params[0] = firePos.position;
                _params[1] = hit.point;

                hit.collider.gameObject.SendMessage("OnDamage_FromRayCast", _params, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

	IEnumerator ShowMuzzleFlash()
    {
		_renderer.enabled = true;
		yield return new WaitForSeconds (Random.Range (0.01f, 0.2f));
		_renderer.enabled = false;
	}

	IEnumerator CreateBullet()
    {
		Instantiate (bullet, firePos.position, firePos.rotation);
		yield return null;
	}

	IEnumerator PlaySfx(AudioClip _clip)
    {
		//audio.PlayOneShot(_clip, 0.9f );
        _gameMgr.PlaySfx(firePos.position, _clip);
		yield return null;
	}
}