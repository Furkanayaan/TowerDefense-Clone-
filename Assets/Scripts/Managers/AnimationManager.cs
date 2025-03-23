using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager I;
    public enum Type {
        Linear,
        QuadraticThree,
        QuadraticFour
    }

    public class AnimationProperties {
        //Target object to apply the animation.
        public Transform targetTransform; 
        //Animation type (Linear, Quadratic, etc.).
        public Type animType;
        //List of positions for the animation path.
        public List<Vector3> posses;
        //Action to be executed at the end of the animation.
        public Action<Transform> action;
        //Animation curve index for the position animation.
        public int animCurvePos;
        //Animation curve index for the sca;e animation.
        public int animCurveScale;
        //Animation speed.
        public float speed = 1f;
        //Elapsed animation time.
        public float animTimer = 0f;

        //Constructor that takes and assigns all values.
        public AnimationProperties(Transform targetTransform, Type animType, List<Vector3> posses, Action<Transform> action, int animCurvePos, int animCurveScale, float speed = 1f, float animTimer = 0f) {
            this.targetTransform = targetTransform;
            this.animType = animType;
            this.posses = posses;
            this.action = action;
            this.animCurvePos = animCurvePos;
            this.animCurveScale = animCurveScale;
            this.speed = speed;
            this.animTimer = animTimer;
        }
    }
    
    [Serializable]
    public class SObjectsToAnim {


        public Transform transforms;

        public List<Vector3> posses;
        public Vector3 startScales;

        public float timer;
        public float speed;

        public Type animTypes;
        public int animationCurvePoses;
        public int animationCurveScales;

        public Action<Transform> Actions;

        public SObjectsToAnim() {
            transforms = null;
            posses = new();
            startScales = new();
            timer = 0;
            speed = 0;
            animTypes = Type.Linear;

            animationCurvePoses = 0;
            animationCurveScales = 0;

            Actions = null;
        }

        //Adds a new animation property.
        public void Add(AnimationProperties animationProperties) {
            if(animationProperties == null || animationProperties.targetTransform == null) {
                return;
            }

            transforms = animationProperties.targetTransform;
            posses = animationProperties.posses;
            startScales = animationProperties.targetTransform.localScale;
            Actions = animationProperties.action;
            speed = animationProperties.speed;

            animationCurvePoses = animationProperties.animCurvePos;
            animationCurveScales = animationProperties.animCurveScale;
            animTypes = animationProperties.animType;

            timer = animationProperties.animTimer;
        }
        //Called when the animation is complete and performs necessary cleanup.
        public void Remove(Transform targetTransform) {
            try {
                if(Actions != null) 
                    Actions.Invoke(targetTransform);
            }
            catch (System.Exception e) {
                string error = e.ToString();
            }
                
            transforms = null;
            posses = new List<Vector3>();
            startScales = new Vector3();
            timer = 0;
            speed = 0;
            animationCurvePoses = 0;
            animationCurveScales = 0;
            animTypes = Type.Linear;
            Actions = null;
        }
    }
    
    //List of animations.
    public List<SObjectsToAnim> objectsToAnim = new List<SObjectsToAnim>();

    //Position animation curves.
    [SerializeField] private AnimationCurve[] curvesPos;
    //Scale animation curves.
    [SerializeField] private AnimationCurve[] curvesScale;

    public void Awake() {
        I = this;
    }

    public void Update() {
        OneTimeAnimations();
    }


    private void OneTimeAnimations() {
        for (int i = 0; i < objectsToAnim.Count; i++) {
            if(objectsToAnim[i] == null || objectsToAnim[i].transforms == null ) {
                objectsToAnim.RemoveAt(i);
                return;
            }
            if(!objectsToAnim[i].transforms.gameObject.activeSelf) continue;
            //Evaluate animation curves.
            float evaluated = curvesPos[objectsToAnim[i].animationCurvePoses].Evaluate(objectsToAnim[i].timer);
            float evaluatedScale = curvesScale[objectsToAnim[i].animationCurveScales].Evaluate(objectsToAnim[i].timer);

            //Apply position and scale.
            objectsToAnim[i].transforms.position = PosReturner(i, evaluated);
            objectsToAnim[i].transforms.localScale = objectsToAnim[i].startScales + Vector3.one * evaluatedScale;

            //Update animation duration.
            objectsToAnim[i].timer += Time.smoothDeltaTime / objectsToAnim[i].speed;
            if (objectsToAnim[i].timer > 1f) {
                //Set the position and scale to the final values when the animation ends.
                objectsToAnim[i].transforms.position =
                    PosReturner(i, curvesPos[objectsToAnim[i].animationCurvePoses].keys[^1].value);

                objectsToAnim[i].transforms.localScale = 
                    objectsToAnim[i].startScales + Vector3.one * curvesScale[objectsToAnim[i].animationCurveScales].keys[^1].value;
                //Clear the animation
                objectsToAnim[i].Remove(objectsToAnim[i].transforms);
                objectsToAnim.RemoveAt(i);
            }
        }
    }

    
    //Function to calculate position.
    private Vector3 PosReturner(int index, float time) {
        switch (objectsToAnim[index].animTypes) {
            default:
                return new Vector3();
            case Type.Linear:
                return Linear(index, time);
            
            case Type.QuadraticThree:
                return QuadraticThree(index, time);
            
            case Type.QuadraticFour:
                return QuadraticFour(index, time);
        }
    }

    //Calculate position using linear interpolation.
    private Vector3 Linear(int index, float t) {
        return objectsToAnim[index].posses[0] + t * (objectsToAnim[index].posses[1] - objectsToAnim[index].posses[0]);
    }
    
    //Calculate position using three-point quadratic interpolation
    private Vector3 QuadraticThree(int index, float t) {
        Vector3 p = (1f - t) * ((1 - t) * objectsToAnim[index].posses[0] + t * objectsToAnim[index].posses[1]) + t * ((1 - t) * objectsToAnim[index].posses[1] + t * objectsToAnim[index].posses[2]);
        return p;
    }
    
    //Calculate position using four-point quadratic interpolation.
    private Vector3 QuadraticFour(int index, float t) {
        Vector3 p = Mathf.Pow(1f - t, 3f) * objectsToAnim[index].posses[0] + 
                    3f * Mathf.Pow(1f - t, 2f) * t * objectsToAnim[index].posses[1] + 
                    3f * (1f - t) * t * t * objectsToAnim[index].posses[2] + 
                    Mathf.Pow(t, 3f) * objectsToAnim[index].posses[3];
        
        return p;
    }
}
