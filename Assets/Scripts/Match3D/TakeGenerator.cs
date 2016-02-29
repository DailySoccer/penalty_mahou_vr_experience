// #define REGISTER_GAME_EVENTS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;
using System.IO;
using FootballStar.Common;
using FootballStar.Audio;
using System.Linq;
using System;
using ExtensionMethods;

namespace FootballStar.Match3D {
    public class TakeGenerator
    {

        public static AnimatorTimeline.JSONTake Generate(InteractiveType type)
        {
            // Aqui podemos crear las jugadas dinamicamente.
            // Solo necesito posicionar a los juagadores y añadirles las acciones.
            return new AnimatorTimeline.JSONTake()
            {
                takeName = "test",
                inits = new AnimatorTimeline.JSONInit[] {
                    new AnimatorTimeline.JSONInit() {
                        type = "position",
                        go = "Balon",
                        position = new AnimatorTimeline.JSONVector3() { x = 12.03f, y=0.1f, z=19.39f }
                    },
                    new AnimatorTimeline.JSONInit() {
                        type = "position",
                        go = "Local11",
                        position = new AnimatorTimeline.JSONVector3() { x = 12.03f, y=0.1f, z=19.39f }
                    }
                },
                actions = new AnimatorTimeline.JSONAction[] {
                    new AnimatorTimeline.JSONAction(){
                        method= "sendmessage",
                        go="Local11",
                        delay=0.0416666679f,
                        strings=new string[] {"Balon" }
                    },
                    new AnimatorTimeline.JSONAction() {
                        method= "moveto",
                        go="Local11",
                        delay=0.166666672f,
                        time=3.875f,
                        easeType=21,
                        customEase = new float[0],
                        path = new AnimatorTimeline.JSONVector3[] {
                            new AnimatorTimeline.JSONVector3() { x= 41.4010468f, y=0.1f, z=0.5811138f }
                        }
                    },
                    /*
                    new AnimatorTimeline.JSONAction() {
                        method= "invokemethod",
                        go="Local11",
                        delay=2.0f,
                        strings=new string[] { "Entrenador", "Pase" }
                    },
                    */
                    new AnimatorTimeline.JSONAction() {
                        method= "sendmessage",
                        go="Local11",
                        delay=2.0f,
                        strings=new string[] { "Chut" },
                        eventParams = new AnimatorTimeline.JSONEventParameter[] {
                            new AnimatorTimeline.JSONEventParameter(){
                                valueType=2,
                                val_float=0.4f
                            }
                        }
                    },
                }
            };
            
        }
    }
}


