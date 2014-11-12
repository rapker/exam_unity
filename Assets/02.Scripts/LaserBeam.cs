using UnityEngine;
using System.Collections;

public class LaserBeam : MonoBehaviour {

    private Transform tr;
    private LineRenderer _line;

    private float _raycastDistance = 100.0f;

	// Use this for initialization
	void Start ()
    {
	    tr = GetComponent<Transform>();
        _line = GetComponent<LineRenderer>();

        _line.enabled = false;
        _line.SetWidth(0.3f, 0.05f);
	}
	
	// Update is called once per frame
	void Update ()
    {
        Ray ray = new Ray( tr.position, tr.forward );
        Debug.DrawRay(ray.origin, ray.direction * _raycastDistance, Color.blue);

        RaycastHit hit;

        if( Input.GetMouseButtonDown(0) )
        {
            _line.SetPosition(0, ray.origin);

            if (Physics.Raycast(ray, out hit, _raycastDistance))
            {
                _line.SetPosition(1, hit.point);
            }
            else
            {
                _line.SetPosition(1, ray.GetPoint(_raycastDistance));
            }

            StartCoroutine( this.ShowLaserBeam() );
        }
	}

    IEnumerator ShowLaserBeam()
    {
        _line.enabled = true;
        yield return new WaitForSeconds( Random.Range(0.01f, 0.2f) );
        _line.enabled = false;
    }
}
