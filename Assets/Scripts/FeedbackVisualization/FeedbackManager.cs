﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ExerciseExplanation
{
    BallFeedback
}

public abstract class InseilFeedback : MonoBehaviour
{
    public ExerciseExplanation type;

    public abstract void InitFeedback(StaticJoint joint, Transform relTo, BoneMap bones);
    public abstract void InitFeedback(MotionJoint joint, Transform relTo, BoneMap bones);
}

public class FeedbackManager : MonoBehaviour {

    // Singleton
    public static FeedbackManager instance;

    public List<ExerciseExplanation> feedbackTypes = new List<ExerciseExplanation>();

    //Exercise Data
    public float bodyHeight;
    public string exercisePath;
    public int sizeOfSet;
    public string nameOfPerson;
    public int set;
    private Object[] exerciseData;
    private List<string> exerciseNames = new List<string>();

    // Avatars
    public BoneMap boneMap;
    public Transform coordinatesRelToJoint;

    // Stores all exercises found in the scene
    private List<InseilExercise> exercises = new List<InseilExercise>();
    
    // Index of active exercise
    private int index = 0;

    // Called before Start
    void Awake()
    {
        instance = this;
    }

	// Use this for initialization
	void Start () {        

        exerciseData = Resources.LoadAll(exercisePath);

        // Init exercises
        for (int i = 0; i < exerciseData.Length; i++){
            exerciseNames.Add(exerciseData[i].name);

            GameObject exerciseObject = new GameObject(exerciseData[i].name);
            exerciseObject.transform.parent = transform;

            ExerciseInfo eInfo = new ExerciseInfo();
            eInfo.sizeOfSet = sizeOfSet;
            eInfo.set = set;
            eInfo.nameOfPerson = nameOfPerson;

            InseilExercise ex = exerciseObject.AddComponent<InseilExercise>();
            ex.InitExercise(exerciseData[i].name, eInfo, feedbackTypes, bodyHeight, coordinatesRelToJoint, boneMap);
            exercises.Add(ex);
            
        }

        // Deactivate all exercises
        foreach (InseilExercise iEX in exercises)
        {
            iEX.gameObject.SetActive(false);
        }

        // Activate first exercise
        exercises[index].gameObject.SetActive(true);
        exercises[index].CameraExerciseSwitch();
	}
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(KeyCode.Space))
        {
            SwitchExercise((index + 1) % exercises.Count);
        }

        // Print exercise Info into external files
        //exercises[index].PrintExersiceInfo();
	}

    public List<string> GetExerciseFileNames()
    {
        return exerciseNames;
    }

    // Used by exercises to register
    /*public void AddExercise(InseilExercise ex)
    {
        exercises.Add(ex);
    }*/

    // Shows feedback by type
    public void ShowFeedback(int type)
    {
        // Tells feedback which feedback state is active at the moment
        exercises[index].enabledFeedBackType = (ExerciseExplanation)type;
    }

    // Called by GUI
    private void SwitchExercise(int id)
    {
        // Deactivate old exercise and activate new one
        if (id >= 0 && id < exercises.Count && exercises.Count > 0)
        {
            exercises[index].gameObject.SetActive(false);
            index = id;
            exercises[index].gameObject.SetActive(true);
            InseilMainCamera.instance.ResetCamera();
            exercises[index].CameraExerciseSwitch();
        }
    }

    // Called by GUI
    public void NextExercise()
    {
        SwitchExercise((index + 1) % exercises.Count);
    }
}
