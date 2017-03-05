﻿using System;
using System.Collections.Generic;
using Improbable;
using Improbable.General;
using Improbable.Worker;
using Improbable.Math;
using Improbable.Player;
using Improbable.Unity.Core.Acls;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.EntityTemplates
{
    public class SwarmGoalEntityTemplate : MonoBehaviour
    {
		private static float spawnDiameter = 10.0f;	//TODO: Have to find a way to connect this to global variable, tankSize
		private static float goalSpeed = 1.0f;
		private static UInt32 numFish = 100;

        // Template definition for a Swarm Goal entity
        public static SnapshotEntity GenerateGoalSnapshotEntityTemplate()
        {
			//Spawn to a random position
			Coordinates goalInitialCoordinates = new Coordinates (Random.Range (-spawnDiameter, spawnDiameter),
				                             Random.Range (-spawnDiameter, spawnDiameter),
				                             Random.Range (-spawnDiameter, spawnDiameter));


			Vector3f zero = new Vector3f ();

            // Set name of Unity prefab associated with this entity
            //var SwarmGoalEntity = new SnapshotEntity { Prefab = "ExampleEntity" };
			var SwarmGoalEntity = new SnapshotEntity { Prefab = "Goal" };


            // Define components attached to snapshot entity
			SwarmGoalEntity.Add(new WorldTransform.Data(new WorldTransformData(goalInitialCoordinates, zero, 0.0f)));
			SwarmGoalEntity.Add (new GoalParameters.Data (new GoalParametersData (spawnDiameter, goalSpeed, numFish, 1.0f)));

			//Alastair's recommendation:
			var acl = Acl.Build()
				.SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
				.SetWriteAccess<WorldTransform>(CommonRequirementSets.PhysicsOnly);

			SwarmGoalEntity.SetAcl(acl);

            return SwarmGoalEntity;
        }


		//This function is *not* autogenerated, but can be called anything; it is called in SnapshotMenu.cs
		public static void PopulateSnapshotWithSwarmGoalEntities(ref Dictionary<EntityId, SnapshotEntity> snapshotEntities, ref int nextAvailableId)
		{
			snapshotEntities.Add (new EntityId (nextAvailableId++), GenerateGoalSnapshotEntityTemplate ());

		}


    }
}