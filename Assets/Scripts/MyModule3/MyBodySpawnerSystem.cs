using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.VisualScripting;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MyBodySpawnerSystem : ISystem
{
    uint counter;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MyBodySpawnerData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var bodiesQuery = SystemAPI.QueryBuilder().WithAll<MyBodyData>().Build();

        if (bodiesQuery.IsEmpty)
        {
            var prefab = SystemAPI.GetSingleton<MyBodySpawnerData>().Prefab;
            var instances = state.EntityManager.Instantiate(prefab, SystemAPI.GetSingleton<MyBodySpawnerData>().NumToSpawn, Allocator.Temp);
            var random = Unity.Mathematics.Random.CreateFromIndex(counter++);

            foreach(var entity in instances)
            {
                var transforms = SystemAPI.GetComponentRW<LocalTransform>(entity);
                transforms.ValueRW.Position =
                (new float3(random.NextFloat(), 0, random.NextFloat()) - new float3(0.5f, 0, 0.5f)) * 10;
            }
            
            foreach(var entity in SystemAPI.Query<RefRW<MyBodyData>>())
            {
                entity.ValueRW.Velocity =
                random.NextFloat3() - new float3(0.5f, 0.5f, 0.5f) * 10;
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
