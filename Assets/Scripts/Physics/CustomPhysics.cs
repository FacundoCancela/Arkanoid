using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public static class CustomPhysics
{
    private static Vector2 _reusableSphereTestVector = new Vector2(0, 0);
    private static Ray _reusableContactRay = new Ray();
    
    //Movements
    public static void UniformLineMovement(GameObject objectToMove, float speed, Vector2 direction)
    {
        objectToMove.transform.Translate(speed * direction.x * Time.deltaTime, speed * direction.y * Time.deltaTime, 0);
    }

    //Collisions
    public static bool CollisionRectangleToRectangle(BoxCollider rect1, BoxCollider rect2)
    {
        if (rect1.bounds.min.x < rect2.bounds.max.x &&
        rect1.bounds.max.x > rect2.bounds.min.x &&
        rect1.bounds.min.y < rect2.bounds.max.y &&
        rect1.bounds.max.y > rect2.bounds.min.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool CollisionSphereToRectangle(BoxCollider rectangle, SphereCollider sphere)
    {
        _reusableSphereTestVector.x = sphere.transform.position.x;
        _reusableSphereTestVector.y = sphere.transform.position.y;

        if (_reusableSphereTestVector.x > rectangle.bounds.max.x)
            _reusableSphereTestVector.x = rectangle.bounds.max.x;
        else if (_reusableSphereTestVector.x < rectangle.bounds.min.x)
            _reusableSphereTestVector.x = rectangle.bounds.min.x;
        
        if (_reusableSphereTestVector.y > rectangle.bounds.max.y)
            _reusableSphereTestVector.y = rectangle.bounds.max.y;
        else if (_reusableSphereTestVector.y < rectangle.bounds.min.y)
            _reusableSphereTestVector.y = rectangle.bounds.min.y;

        float distance = Vector2.Distance(sphere.transform.position, _reusableSphereTestVector);

        if (distance < sphere.radius)
            return true;
        else
            return false;
    }

    //Aux use
    public static Vector2 GetDistanceVectorDirection(BoxCollider rectangle, SphereCollider sphere)
    {
        _reusableSphereTestVector.x = sphere.transform.position.x;
        _reusableSphereTestVector.y = sphere.transform.position.y;

        if (_reusableSphereTestVector.x > rectangle.bounds.max.x)
            _reusableSphereTestVector.x = rectangle.bounds.max.x;
        else if (_reusableSphereTestVector.x < rectangle.bounds.min.x)
            _reusableSphereTestVector.x = rectangle.bounds.min.x;
        
        if (_reusableSphereTestVector.y > rectangle.bounds.max.y)
            _reusableSphereTestVector.y = rectangle.bounds.max.y;
        else if (_reusableSphereTestVector.y < rectangle.bounds.min.y)
            _reusableSphereTestVector.y = rectangle.bounds.min.y;

        float distanceX = sphere.transform.position.x - _reusableSphereTestVector.x;
        float distanceY = sphere.transform.position.y - _reusableSphereTestVector.y;
        
        Vector2 collisionDirection = new Vector2(distanceX, distanceY).normalized;
        return collisionDirection;
    }

    public static Vector3 GetCollisionHitPoint(Transform a, Transform b)
    {
        RaycastHit hit;
        _reusableContactRay.origin = a.position;
        _reusableContactRay.direction = (b.position - a.position).normalized;

        //ver si usar non alloc - esto no se llama constantemente en update asi que no se si vale la pena
        if (Physics.Raycast(_reusableContactRay, out hit, Vector3.Distance(a.position, b.transform.position)))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
}
