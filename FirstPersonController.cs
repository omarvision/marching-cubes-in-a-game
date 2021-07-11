using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    #region --- helpers ---
    private static class MouseBtn
    {
        public static int primary = 0;
        public static int secondary = 1;
        public static int middle = 2;
    }
    public struct Sounds
    {
        public AudioSource erase;
        public AudioSource add;
        public AudioSource thrust;
    }
    #endregion

    public float Movespeed = 3.5f;
    public float Turnspeed = 120f;
    public float Thrust = 2.0f;
    public Camera cam = null;
    public GameObject terraineditor = null;
    public GameObject camfollow = null;
    private RaycastHit hit;
    private Sounds snd;
    private Renderer rend = null;
    private Rigidbody rb = null;

    private void Start()
    {
        GameObject go = this.gameObject;
        snd.erase = Globals.CreateAudioSource(go, "erase");
        snd.add = Globals.CreateAudioSource(go, "add");
        snd.thrust = Globals.CreateAudioSource(go, "thrust");

        rend = terraineditor.GetComponent<Renderer>();

        rb = this.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        //move, turn
        float vert = Input.GetAxis("Vertical");
        float horz = Input.GetAxis("Horizontal");
        this.transform.Translate(Vector3.forward * vert * Movespeed * Time.deltaTime);
        this.transform.localRotation *= Quaternion.AngleAxis(horz * Turnspeed * Time.deltaTime, Vector3.up);

        //look up down
        float my = Input.GetAxis("Mouse Y");
        if (Input.GetMouseButton(MouseBtn.middle) == true)
            camfollow.transform.Translate(Vector3.up * my * Time.deltaTime);

        //thrust
        if (Input.GetKey(KeyCode.Space) == true)
        {
            rb.AddForce(Vector3.up * Thrust * Time.deltaTime, ForceMode.VelocityChange);
            if (snd.thrust.isPlaying == false)
                snd.thrust.Play();
        }
        else if (snd.thrust.isPlaying == true)
        {
            snd.thrust.Stop();
        }

        //terrain editing
        if (Input.GetMouseButton(MouseBtn.primary) == true)
        {
            if (WhereDidIClick() == true)
                EditMarchCubeAdd();
        }
        else if (Input.GetMouseButton(MouseBtn.secondary) == true)
        {
            if (WhereDidIClick() == true)
                EditMarchCubeErase();
        }
        else
        {
            EditOff();
        }
    }
    private bool WhereDidIClick()
    {
        //Note:
        //  set gridpoints to Ignore Raycast (layer 2) and maybe the terraineditor gameobject too.
        //  this way raycast will pick up on marching cube mesh only.
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit);       
    }
    private void EditMarchCubeAdd()
    {
        if (hit.transform.CompareTag("MarchCube") == false)
        {
            Debug.Log(string.Format("Raycast Hit = {0}", hit.transform.name));
            return;
        }
        if (snd.add.isPlaying == false)
            snd.add.Play();
        terraineditor.tag = "Add";
        terraineditor.transform.localPosition = hit.point;
        rend.enabled = true;
        rend.material.color = Color.green;        
    }
    private void EditMarchCubeErase()
    {
        if (hit.transform.CompareTag("MarchCube") == false)
            return;
        if (snd.erase.isPlaying == false)
            snd.erase.Play();
        terraineditor.tag = "Erase";
        terraineditor.transform.position = hit.point;
        rend.enabled = true;
        rend.material.color = Color.red;        
    }
    private void EditOff()
    {
        if (snd.add.isPlaying == true)
            snd.add.Stop();
        if (snd.erase.isPlaying == true)
            snd.erase.Stop();
        terraineditor.tag = "Untagged";
        rend.material.color = Color.white;
        rend.enabled = false;
    }
}
