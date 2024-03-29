﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipController : MonoBehaviour{

    public int health;
    public float speed;
    public GameObject myBullet, plane;
    public AudioClip spaceship_explosion;
    public ParticleSystem particle;
    private Dictionary<string, float> screen;
    private float xMin, xMax, zMin, zMax;
    private bool isCoroutineStarted = false;
    private LevelManager levelManager;

    void Start(){
        screen = plane.GetComponent<PlaneController>().get_screen();
        levelManager = GameObject.FindObjectOfType<LevelManager>();
    }

    float get_angle(){
        Vector3 mousePos = Input.mousePosition;
        Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        return angle;
    }
    void rotate(){
        float angle = get_angle();
        this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, -angle + 180, 0));
    }

    void movement(){
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        this.gameObject.GetComponent<Rigidbody>().velocity = movement * speed * Time.deltaTime * 10f;
        float posx = this.gameObject.GetComponent<Rigidbody>().position.x;
        float posz = this.gameObject.GetComponent<Rigidbody>().position.z;
        this.GetComponent<Rigidbody>().position = new Vector3 
            (
             Mathf.Clamp (posx, screen["xMin"], screen["xMax"]), 
             0.0f,
             Mathf.Clamp (posz, screen["zMin"], screen["zMax"])
             );
    }

    IEnumerator ending(float waitTime){
        Instantiate(particle, this.transform.position,Quaternion.identity);
        AudioSource.PlayClipAtPoint(spaceship_explosion, new Vector3(0, 30, 0));
        this.gameObject.transform.position = new Vector3(-999f, -999f, -999f);
        yield return new WaitForSeconds(waitTime);
        levelManager.LoadLevel("GameOver");
    }

    void Update(){
        if(!ScoreController.gameover){
            movement();
            rotate();
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)){
                Instantiate(myBullet,this.gameObject.transform.position,Quaternion.Euler(new Vector3(0, -get_angle() + 90, 0)));
            }
        }
        else{
            this.gameObject.GetComponent<Renderer>().enabled = false;
            if(!isCoroutineStarted){
                StartCoroutine(ending(2f));
                isCoroutineStarted = true;
            }
        }
    }
}
