﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float speed;
    public AudioClip enemy_explosion;
    public ParticleSystem particle;
    private GameObject plane;
    private Canvas ui;
    private Dictionary<string, float> screen;

    void Start(){
        speed = Random.Range(0.5f, 5f);
        plane = GameObject.Find("Plane");
        screen = plane.GetComponent<PlaneController>().get_screen();
        ui = GameObject.FindObjectOfType<Canvas>();
    }

    void Update(){
        this.transform.position -= transform.forward * Time.deltaTime * 5f * speed;
        if (this.gameObject.transform.position.z > screen["zMax"] ||
            this.gameObject.transform.position.z < screen["zMin"] ||
            this.gameObject.transform.position.x > screen["xMax"] ||
            this.gameObject.transform.position.x < screen["xMin"]) {
            Destroy (this.gameObject);
        }
    }

    void spaceship_handle(GameObject spaceship){
        int health = spaceship.GetComponent<SpaceShipController>().health;
        string heart = "heart_" + health;
        ui.GetComponent<UIController>().display_heart(heart);
        spaceship.GetComponent<SpaceShipController>().health -= 1;
        AudioSource.PlayClipAtPoint(enemy_explosion, new Vector3(0, 30, 0));
        if(spaceship.GetComponent<SpaceShipController>().health <= 0){
            ScoreController.gameover = true;
        }
    }

    void OnTriggerEnter(Collider other) {
        Instantiate(particle, other.transform.position,Quaternion.identity);
        AudioSource.PlayClipAtPoint(enemy_explosion, new Vector3(0, 30, 0));
        ScoreController.score += 100;
        Destroy(this.gameObject);
        if(other.gameObject.name == "Spaceship"){
            GameObject spaceship = other.gameObject;
            spaceship_handle(spaceship);
        }
    }
}

