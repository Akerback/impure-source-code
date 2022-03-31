using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSTools {
    //Loads of stuff used in other places
    public static class tools
    {
        public static Quaternion noRotation = Quaternion.Euler(Vector3.zero);//Unneccessary, same as Quaternion.identity
        public static Quaternion rotationUpward = Quaternion.Euler(90, 0, 0);
        public static readonly ContactFilter2D emptyContactFilter = (new ContactFilter2D()).NoFilter();
        public static readonly ContactFilter2D worldContactFilter = getWorldFilter();

        //I don't like contact filters
        static ContactFilter2D getWorldFilter() {
            ContactFilter2D worldFilter = new ContactFilter2D();
            worldFilter.SetLayerMask(1 << 9);

            return worldFilter;
        }

        //Unit vector rotated angle degrees from the right
        public static Vector2 vectorFromAngle(float angle) 
        {
            float radAngle = Mathf.Deg2Rad * angle;

            return new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle));
        }

        //Get rotation from the right
        public static float angleFromVector(Vector2 inputVector, bool signed = true) 
        {
            //Calculate angle
            float angle = Vector2.SignedAngle(Vector2.right, inputVector);

            if ((signed) || (angle >= 0)) return angle;
            //Add 360 if it has to be unsigned
            else return angle + 360.0f;
        }

        //Random vector up to extents degrees away from angle
        public static Vector2 vectorInCone(float midAngle, float extents) 
        {
            return vectorFromAngle(Random.Range(-1.0f, 1.0f) * extents + midAngle);
        }

        //Identical as above but the midpoint is input as a vector
        public static Vector2 vectorInCone(Vector2 midVector, float extents) 
        {
            return vectorFromAngle(Random.Range(-1.0f, 1.0f) * extents + angleFromVector(midVector));
        }

        //Maps a 0-360 angle to -180-180 degrees
        public static float getRelativeRotation(float inputAngle) 
        {
            while (Mathf.Abs(inputAngle) > 180) inputAngle -= 360 * Mathf.Sign(inputAngle);

            return inputAngle;
        }

        //Angle relative to this object's right
        public static float getRelativeRotation(Vector2 inputVector, Transform inputTransform) 
        {
            return Vector2.SignedAngle(inputTransform.right, inputVector);
        }

        //Unity's sign returns 1 if input is 0. This doesn't
        //Could've also used System.Math.Sign()
        public static float betterSign(float inputValue) 
        {
            if (inputValue < 0.0f) return -1.0f;
            if (inputValue > 0.0f) return 1.0f;
            return 0.0f;
        }

        public static Vector3 getMouseWorldPos() 
        {
            Camera mainCam = Camera.main;
            //Get screen position
            Vector2 mPos = Input.mousePosition;

            //Screen-world conversion, at own depth
            Vector3 mWorldPos = mainCam.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -mainCam.transform.position.z));

            return mWorldPos;
        }

        public static bool oppositeSigns(float val1, float val2) 
        {
            return (Mathf.Abs(betterSign(val1) - betterSign(val2)) == 2);
        }

        //Unneccessary, if both values are floats the built in random returns a float
        public static float floatRandomRange(float val1, float val2) 
        {
            float difference = val2 - val1;
            //Random val and map to range
            float val = Random.value * difference + val1;

            return val;
        }

        //Rolls a set chance and returns if it was successful
        public static bool randomChance(float chance = 0.5f) 
        {
            return (Random.value <= chance);
        }

        //Random vector, any angle and length between 0 and 1
        public static Vector2 randomVector() 
        {
            return vectorFromAngle(Random.Range(0.0f, 360.0f)) * Random.value;
        }
        
        //Vector turned in 2d
        public static Vector2 vectorToLocal(Vector2 inputVector, Transform transformToAlignTo) 
        {
            //Returnerar den omräknade vektorn
            return inputVector.x * transformToAlignTo.right + inputVector.y * transformToAlignTo.up;
        }

        //Casts from collider to target
        public static bool visionCheck(Collider2D ignoredCollider, Vector3 targetPosition) 
        {
            //Casts against everyting
            List<RaycastHit2D> collisionsFound = new List<RaycastHit2D>();
            
            Vector2 ray = targetPosition - ignoredCollider.transform.position;
            //Dist
            float raylength = ray.magnitude;
            ray.Normalize();

            //Cast it
            ignoredCollider.Raycast(ray, tools.worldContactFilter, collisionsFound, raylength);

            if (collisionsFound.Count == 0) return true;
            return false;
        }
    
        public static bool minimalVisionCheck(Vector2 origin, Vector2 target) 
        {
            Vector2 vectorToGoal = target - origin;

            float worldDistance = Physics2D.Raycast(origin, vectorToGoal, vectorToGoal.magnitude, 1 << 9).distance;

            return (worldDistance == 0);
        }

        public static bool rangedVisionCheck(Vector2 origin, Vector2 direction, float range) {
            return minimalVisionCheck(origin, origin + direction.normalized * range);
        }

        public static Vector2 collisionFree(Vector2 origin, Vector2 target) 
        {
            Vector2 vectorToGoal = target - origin;

            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, vectorToGoal, vectorToGoal.magnitude);

            foreach (RaycastHit2D hit in hits) 
            {
                if (hit.collider.gameObject.tag == "visionBlocking") return vectorToGoal.normalized * hit.distance;
            }

            return vectorToGoal;
        }
    
        public static RaycastHit2D minimalWallCheck(Vector2 origin, Vector2 target) 
        {
            Vector2 ray = target - origin;
            float range = ray.magnitude;

            ray.Normalize();

            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, ray, range);

            foreach (RaycastHit2D hit in hits) 
            {
                if (hit.transform.tag == "visionBlocking") return hit;
            }

            return hits[0];
        }

        public static RaycastHit2D minimalWallCheck(Vector2 origin, Vector2 target, float range) 
        {
            Vector2 ray = target - origin;

            ray.Normalize();

            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, ray, range);

            foreach (RaycastHit2D hit in hits) 
            {
                if (hit.transform.tag == "visionBlocking") 
                    return hit;
            }

            return hits[0];
        }
    
        public static Vector2 positionInDirection(Vector2 origin, Vector2 target, float range) 
        {
            Vector2 ray = (target - origin).normalized;

            return origin + ray * range;
        }
    
        public static Vector2 vectorRotated(Vector2 inputVector, float degrees) 
        {
            return vectorFromAngle(angleFromVector(inputVector) + degrees);
        }

        //Round vector2
        public static Vector2 RoundVector2(Vector2 inputVector) 
        {
            float x, y;//, z;

            x = inputVector.x;
            y = inputVector.y;
            //z = inputVector.z;

            return new Vector3(Mathf.Round(x), Mathf.Round(y));//, Mathf.Round(z));
        }

        public static Vector2 FloorVector2(Vector2 inputVector) 
        {
            float x, y;//, z;

            x = inputVector.x;
            y = inputVector.y;
            //z = inputVector.z;

            return new Vector3(Mathf.Floor(x), Mathf.Floor(y));//, Mathf.Round(z));
        }

        public static Vector2 CeilVector2(Vector2 inputVector) 
        {
            float x, y;//, z;

            x = inputVector.x;
            y = inputVector.y;
            //z = inputVector.z;

            return new Vector3(Mathf.Ceil(x), Mathf.Ceil(y));//, Mathf.Round(z));
        }
    }
}