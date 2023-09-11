﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace EasyGameStudio.Jeremy
{
    public class Demo_control : MonoBehaviour
    {
        public Material[] materials;
        public Renderer render;

        public Text text_title;
        public string[] titles;

        public AudioSource audio_source;
        public AudioClip ka;

        private int index = 0;


        public float[] min_threshold;
        public float[] max_threshold;

        public Dissolve dissolve;


        void Start()
        {
            //this.index = 0;

            //this.render.material = this.materials[this.index];

            //this.text_title.text = this.titles[this.index];

            //this.audio_source.PlayOneShot(this.ka, 0.66f);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                this.index++;
                if (this.index >= this.materials.Length)
                    this.index = 0;

                this.render.material = this.materials[this.index];

                this.text_title.text = this.titles[this.index];

                this.audio_source.PlayOneShot(this.ka, 0.66f);

                this.dissolve.min_threshold = this.min_threshold[this.index];
                this.dissolve.max_threshold = this.max_threshold[this.index];

                this.dissolve.reset_material();

            }

            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                this.index--;
                if (this.index <= -1)
                    this.index = (this.materials.Length - 1);

                this.render.material = this.materials[this.index];

                this.text_title.text = this.titles[this.index];

                this.audio_source.PlayOneShot(this.ka, 0.66f);

                this.dissolve.min_threshold = this.min_threshold[this.index];
                this.dissolve.max_threshold = this.max_threshold[this.index];

                this.dissolve.reset_material();

            }
        }

    }
}