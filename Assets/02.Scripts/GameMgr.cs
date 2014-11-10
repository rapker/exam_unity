using UnityEngine;
using System.Collections;

public class GameMgr : MonoBehaviour {

    public Transform[] points;
    public GameObject monsterPrefab;

    public float createTime = 2.0f;
    public int maxMonster = 10;

    public bool isGameOver = false;

    public float sfxVolume = 1.0f;
    public bool isSfxMute = false;

	// Use this for initialization
	void Start () 
    {
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        if( points.Length > 0 )
        {
            StartCoroutine(this.CreateMonster());
        }
	}
	
    IEnumerator CreateMonster()
    {
        while( !isGameOver )
        {
            int monsterCount = (int)GameObject.FindGameObjectsWithTag("MONSTER").Length;

            if( monsterCount < maxMonster )
            {
                yield return new WaitForSeconds(createTime);

                int idx = Random.Range(1, points.Length);

                Instantiate(monsterPrefab, points[idx].position, points[idx].rotation);
            }
            else
            {
                yield return null;
            }
        }
    }

    public void PlaySfx( Vector3 pos, AudioClip sfx )
    {
        if (isSfxMute)
            return;

        GameObject soundObj = new GameObject("Sfx");
        soundObj.transform.position = pos;

        AudioSource _audioSource = soundObj.AddComponent<AudioSource>();

        _audioSource.clip = sfx;
        _audioSource.minDistance = 10.0f;
        _audioSource.maxDistance = 30.0f;

        _audioSource.volume = sfxVolume;

        _audioSource.Play();

        Destroy(soundObj, sfx.length);
    }

	// Update is called once per frame
	void Update () {
	
	}
}
