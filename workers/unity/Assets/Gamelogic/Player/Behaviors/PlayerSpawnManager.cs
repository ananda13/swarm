﻿using Improbable.Entity.Component;
using UnityEngine;
using Improbable.Unity.Visualizer;
using Improbable.Player;
using Improbable.Worker;
using Improbable.Unity;
using Improbable.Unity.Core;
using Assets.EntityTemplates;

namespace Assets.Gamelogic.Player.Behaviours
{
	// Enable this MonoBehaviour on UnityWorker (server-side) workers only : different syntax on v10 vs v9 : https://spatialos.improbable.io/docs/reference/10.0/releases/upgrade-guides/how-to-upgrade-10
	[WorkerType(WorkerPlatform.UnityWorker)]
	public class PlayerSpawnManager : MonoBehaviour
	{
		/*This is an entity that spawns a new player everytime 
		 * someone logs into the experience 
		 * 
		 */

		// Enable this MonoBehaviour only on the worker which has write-access for the entity's Spawner component
		[Require] private Spawner.Writer SpawnerWriter;

		void OnEnable()
		{
			// Register command callback
            SpawnerWriter.CommandReceiver.OnSpawnPlayer.RegisterAsyncResponse(OnSpawnPlayer);
		}

		void OnDisable()
		{
			// Deregister command callback
            SpawnerWriter.CommandReceiver.OnSpawnPlayer.DeregisterResponse();
		}

		// Command callback for requests by new clients for a new player to be spawned
		private void OnSpawnPlayer(
			ResponseHandle<Spawner.Commands.SpawnPlayer, SpawnPlayerRequest, SpawnPlayerResponse> Handle)
		{

			var playerEntityTemplate = PlayerEntityTemplate.GeneratePlayerEntityTemplate(Handle.CallerInfo.CallerWorkerId,
                 Handle.Request.initialPosition);
            SpatialOS.Commands.CreateEntity(SpawnerWriter, "PlayerVR", playerEntityTemplate, result =>
            {
                if (result.StatusCode != StatusCode.Success)
                {
                    Debug.LogError("PlayerSpawnManager failed to create entity: " + result.ErrorMessage);
                    return;
                }
                var createdEntityId = result.Response.Value;
                Debug.Log("PlayerSpawnManager created player entity with entity ID: " + createdEntityId);

                // Acknowledge command receipt and provide client with ID for newly created entity
                Handle.Respond(new SpawnPlayerResponse(createdEntityId));
            });
		}
	}
}