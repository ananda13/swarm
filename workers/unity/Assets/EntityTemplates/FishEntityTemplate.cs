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
    public class FishEntityTemplate : MonoBehaviour
    {
		private static UInt32 numFish = 100;
		private static float initialSpeed = 3.0f;	
		private static float tankSize = 5.0f;
		private static float tankHeight = 4.0f;
        // Template definition for a Fish entity
        public static SnapshotEntity GenerateFishSnapshotEntityTemplate()
        {
			//Spawn to a random position
			Coordinates fishInitialCoordinates = new Coordinates (Random.Range (-tankSize, tankSize),
				                            						Random.Range (0.0f, tankHeight),
																	Random.Range (-tankSize, tankSize));

			Vector3f fishInitialRotation = new Vector3f (Random.Range (-30, 30),
				                                  Random.Range (-30, 30),
				                                  Random.value * 360);	//roll, pitch, yaw

			float speed = Random.Range (initialSpeed / 2.0f, initialSpeed);

			//float fishInitialSpeed = Random.Range (initialSpeed / 2.0f, initialSpeed);

            // Set name of Unity prefab associated with this entity
            //var FishEntity = new SnapshotEntity { Prefab = "ExampleEntity" };
			var FishEntity = new SnapshotEntity { Prefab = "Fish" };


            // Define components attached to snapshot entity
			FishEntity.Add(new WorldTransform.Data(new WorldTransformData(fishInitialCoordinates, fishInitialRotation, speed)));
			FishEntity.Add (new FishParameters.Data (new FishParametersData (numFish, initialSpeed, tankSize, tankHeight)));

			// Grant UnityWorker (server-side) workers write-access over all of this entity's components, read-access for visual (e.g. client) workers
			//var acl = Acl.GenerateServerAuthoritativeAcl (FishEntity); //Does not currently work

			//Alastair's recommendation:
			var acl = Acl.Build()
				.SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
				.SetWriteAccess<WorldTransform>(CommonRequirementSets.PhysicsOnly)
				.SetWriteAccess<FishParameters>(CommonRequirementSets.PhysicsOnly);

			FishEntity.SetAcl(acl);

            return FishEntity;
        }


		//This function is *not* autogenerated, but can be called anything; it is called in SnapshotMenu.cs
		public static void PopulateSnapshotWithFishEntities(ref Dictionary<EntityId, SnapshotEntity> snapshotEntities, ref int nextAvailableId)
		{
			for (var i = 0; i < numFish; i++)
				snapshotEntities.Add (new EntityId (nextAvailableId++), GenerateFishSnapshotEntityTemplate ());

		}


    }
}