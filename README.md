# RPG_ECS
a RPG game developed using DOTS and ECS in unity

rebuild the kinematica asset before playing which is located in.
Assets->Animation->KinematicaAsset->PlayerMovement
by double clicking the asset and clicking build on the top right.

TODO
Hard
- Remove Nav Mesh Agent component from player and replace it with NavMeshPath
- Move the player by root motion from Kinematica.
- Have 10-15 movement animation so kinematica can play the corrent animation depending on the animation trajectory to the destination.

Easy
 - Convert from ordinary raycast batching job scheduling to entity raycast job scheduling and jobify it (original raycast will only get GameObject not entities, and PhysicsWorld raycast will only get entity not gameObject)


tip Setting the Blend Duration higher give the animation a better blending betweeen animation and make it less choppy

The current plug-in in project (both UniRx and Linq to gameObject) are developed and owned by oshifumi Kawai(a.k.a. neuecc) under the MIT License.
https://github.com/neuecc/UniRx

