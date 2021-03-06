﻿using Improbable;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Worker;
using System;
using Improbable.General;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Random = UnityEngine.Random;
using Improbable.Worker.Query;
using Improbable.Collections.Internal;
using Improbable.Unity.Core.EntityQueries;
using Improbable.Collections;

namespace Assets.Gamelogic.Goal.Behaviours
{
	// Enable this MonoBehaviour on UnityWorker (server-side) workers only : different syntax on v10 vs v9 : https://spatialos.improbable.io/docs/reference/10.0/releases/upgrade-guides/how-to-upgrade-10
	[WorkerType(WorkerPlatform.UnityWorker)]

	public class GoalController : MonoBehaviour {

		[Require] private WorldTransform.Writer WorldTransformWriter;
		[Require] private GoalParameters.Reader GoalParametersReader;

		private Vector3 goalPos, newGoalPos = Vector3.zero;
		private float tankSize, tankHeight, goalSpeed;
		private double getAwayRadius = 0.1;
		private bool getAway;
        private bool reachedDestination = false;

		public bool avoidFish = true;


		// Use this for initialization
		void OnEnable () {

			tankSize = GoalParametersReader.Data.tanksize;
			goalSpeed = GoalParametersReader.Data.goalspeed;
			tankHeight = GoalParametersReader.Data.tankheight;
			Debug.Log ("Goal Startup params: tankSize: " + tankSize + ", Goal Speed:" + goalSpeed);

            transform.position = WorldTransformWriter.Data.position.ToVector3();
            reachedDestination = true;
        }

        void Update()
        {
            //Condition to pick a new destination
            if ((Random.Range(0, 100) < 1) && (reachedDestination == true))
            {
                newGoalPos = new Vector3(Random.Range(-tankSize, tankSize),
                        Random.Range(0.0f, tankHeight),
                        Random.Range(-tankSize, tankSize));

                reachedDestination = false;
            }


            if(!reachedDestination)
            {
                float step = goalSpeed * Time.deltaTime;
                goalPos = Vector3.MoveTowards(transform.position, newGoalPos, step);
                //goalPos = Vector3.Lerp(transform.position, newGoalPos, step);
                //transform.Translate()
                //Apply it to the object
                //this.transform.position.Set(goalPos.x, goalPos.y,goalPos.z);
                transform.position = goalPos;

                //Check whether its reached there
                if (Vector3.Distance(goalPos, newGoalPos) <= 0.01f)
                    reachedDestination = true;

            }

            WorldTransformWriter.Send(new WorldTransform.Update()
                .SetPosition(transform.position.ToCoordinates()));

        }


        // Update is called once per frame
        /*void Update () 
		{
			//Do Some intelligent checking to see if the any fish are approaching the goal
			//If so, get away!
			getAway = false;

			//Setup a query to see if there are any entities within a radius
			var query = Query.InSphere(transform.position.x, transform.position.y, transform.position.z, getAwayRadius).ReturnOnlyEntityIds();

			SpatialOS.Commands.SendQuery(WorldTransformWriter, query, result => {
				if (result.StatusCode != StatusCode.Success) {
					Debug.Log("Goal's query for entities failed with error: " + result.ErrorMessage);
					return;
				}


				if ((result.Response.Count > 0)  && (Random.Range (0, 200) < 1))
				{
					//Debug.Log("Found " + result.Response.Count + " fish. Goal needs to escape!!");
					getAway = true;
				}
						
				if ((Random.Range (0, 500) < 1) || (getAway && avoidFish))
				{
					newGoalPos = new Vector3 (Random.Range (-tankSize, tankSize),
						Random.Range (0.0f, tankHeight),
						Random.Range (-tankSize, tankSize));			

				}

				goalPos = Vector3.Lerp (goalPos, newGoalPos, Time.deltaTime * goalSpeed);

				this.transform.position = goalPos;

				WorldTransformWriter.Send(new WorldTransform.Update ()
					.SetPosition (transform.position.ToCoordinates ()));

			});
			
		}*/




    }

    public static class Vector3Extensions
	{
		public static Coordinates ToCoordinates(this Vector3 vector3)
		{
			return new Coordinates(vector3.x, vector3.y, vector3.z);
		}

        public static Vector3 ToVector3(this Coordinates coordinates)
        {
            return new Vector3((float)coordinates.X, (float)coordinates.Y, (float)coordinates.Z);
        }
    }


}
