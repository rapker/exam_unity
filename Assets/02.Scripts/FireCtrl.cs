using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour {

	public GameObject bullet;
	public Transform firePos;
	public AudioClip fireSfx;
	public MeshRenderer _renderer;

    private GameMgr _gameMgr;

	// Use this for initialization
	void Start () {
		_renderer.enabled = false;
        _gameMgr = GameObject.Find("GameManager").GetComponent<GameMgr>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Fire();
		}
	}

	private void Fire(){
		StartCoroutine (this.ShowMuzzleFlash() );
		StartCoroutine (this.CreateBullet());
		StartCoroutine (this.PlaySfx(fireSfx));
	}

	IEnumerator ShowMuzzleFlash(){
		_renderer.enabled = true;
		yield return new WaitForSeconds (Random.Range (0.01f, 0.2f));
		_renderer.enabled = false;
	}

	IEnumerator CreateBullet(){
		Instantiate (bullet, firePos.position, firePos.rotation);
		yield return null;
	}

	IEnumerator PlaySfx(AudioClip _clip){
		//audio.PlayOneShot(_clip, 0.9f );
        _gameMgr.PlaySfx(firePos.position, _clip);
		yield return null;
	}
}
