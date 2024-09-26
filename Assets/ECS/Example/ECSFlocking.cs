using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Example
{
    public class ECSFlocking : MonoBehaviour
    {
        public int entityCount = 5;
        public float velocity = 2f;
        public float radius = 1.0f;

        public float detectionRadious = 3.0f;
        public float aligmentWeight = 1;
        public float cohesionWeight = 1.5f;
        public float separationWeight = 2;


        public GameObject prefab;
        public GrapfView GrapfView;

        private const int MAX_OBJS_PER_DRAWCALL = 1000;
        private Mesh prefabMesh;
        private Material prefabMaterial;
        private Vector3 prefabScale;
        public Agent agentPrefab;

        private Dictionary<uint, Agent> entities;


        [ContextMenu("RaiseAlarm")]
        public void RaiseAlarm()
        {
            foreach (KeyValuePair<uint, Agent> entity in entities)
            {
                entity.Value.InvokeAlarmOn();
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                if (entities?.Count > 0)
                {
                    foreach (KeyValuePair<uint, Agent> entity in entities)

                    {
                        SetBoidParams(entity.Value.boid);
                    }
                }
            }
        }

        [ContextMenu("StopAlarm")]
        public void StopAlarm()
        {
            foreach (KeyValuePair<uint, Agent> entity in entities)
            {
                entity.Value.InvokeAlarmOff();
            }
        }

        void Start()
        {
            ECSManager.Init();
            entities = new Dictionary<uint, Agent>();
            for (int i = 0; i < entityCount; i++)
            {
                uint entityID = ECSManager.CreateEntity();
                ECSManager.AddComponent<PositionComponent>(entityID, new PositionComponent(0, 0, 0));
                ECSManager.AddComponent<AlignmentComponent>(entityID, new AlignmentComponent(0, 0, 0));
                ECSManager.AddComponent<CohesionComponent>(entityID, new CohesionComponent(0, 0, 0));
                ECSManager.AddComponent<SeparationComponent>(entityID, new SeparationComponent(0, 0, 0));
                ECSManager.AddComponent<DirectionComponent>(entityID, new DirectionComponent(0, 0, 0));
                ECSManager.AddComponent<ObjectiveComponent>(entityID, new ObjectiveComponent(0, 0, 0));
                ECSManager.AddComponent<FowardComponent>(entityID, new FowardComponent(0, 1, 0));
                ECSManager.AddComponent<SpeedComponent>(entityID, new SpeedComponent(velocity));
                ECSManager.AddComponent<RadiusComponent>(entityID, new RadiusComponent(radius));
                entities.Add(entityID, Instantiate(agentPrefab, Vector3.zero, Quaternion.identity));
                SetBoidParams(entities[entityID].boid);
                entities[entityID].grafp = GrapfView;
                entities[entityID].gameObject.SetActive(true);
            }

            prefabMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
            prefabMaterial = prefab.GetComponent<MeshRenderer>().sharedMaterial;
            prefabScale = prefab.transform.localScale;
        }

        void Update()
        {
            ECSManager.Tick(Time.deltaTime);
        }

        void LateUpdate()
        {
            foreach (KeyValuePair<uint, Agent> entity in entities)
            {
                PositionComponent position = ECSManager.GetComponent<PositionComponent>(entity.Key);
                AlignmentComponent alignment = ECSManager.GetComponent<AlignmentComponent>(entity.Key);
                CohesionComponent cohesion = ECSManager.GetComponent<CohesionComponent>(entity.Key);
                SeparationComponent separation = ECSManager.GetComponent<SeparationComponent>(entity.Key);
                DirectionComponent direction = ECSManager.GetComponent<DirectionComponent>(entity.Key);
                ObjectiveComponent objetive = ECSManager.GetComponent<ObjectiveComponent>(entity.Key);
                FowardComponent foware = ECSManager.GetComponent<FowardComponent>(entity.Key);


                var Alig = new Vector3(alignment.X, alignment.Y, alignment.Z);
                var Cohe = new Vector3(cohesion.X, cohesion.Y, cohesion.Z);
                var Sep = new Vector3(separation.X, separation.Y, separation.Z);
                var dir = new Vector3(direction.X, direction.Y, direction.Z);
                var Pos = new Vector3(position.X, position.Y, position.Z);

                entity.Value.boid.SetACS(Alig, Cohe, Sep, dir);
                Matrix4x4 drawMatrix = new Matrix4x4();
                for (int j = 0; j < prefabMesh.subMeshCount; j++)
                {
                    drawMatrix.SetTRS(entity.Value.transform.position, quaternion.identity,
                        prefab.transform.localScale);
                    Graphics.DrawMesh(prefabMesh, drawMatrix, prefabMaterial, 0, null, j);
                }

                position.X = entity.Value.transform.position.x;
                position.Y = entity.Value.transform.position.y;
                position.Z = entity.Value.transform.position.z;
                objetive.X = entity.Value.boid.objective.x;
                objetive.Y = entity.Value.boid.objective.y;
                objetive.Z = entity.Value.boid.objective.z;
                foware.X = entity.Value.transform.forward.x;
                foware.Y = entity.Value.transform.forward.y;
                foware.Z = entity.Value.transform.forward.z;
            }
        }

        private void SetBoidParams(BoidAgent boid)
        {
            boid.detectionRadious = detectionRadious;
            boid.aligmentWeight = aligmentWeight;
            boid.cohesionWeight = cohesionWeight;
            boid.separationWeight = separationWeight;
            boid.speed = velocity;
        }
    }
}