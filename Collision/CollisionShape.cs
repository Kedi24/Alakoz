using System;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using TiledCS;

namespace Alakoz.Collision{
    // This class represents the bounds for a collision. It contains all the code that checkes for
    // interesections between shapes.

    public class CollisionShape
    {
        public Vector2 position;

        float x;
        float y;
        float width;
        float height;

        public float left {get {return x;} set{}}
        public float right {get {return x + width;} set{}}
        public float top {get {return y ;} set{}}
        public float bottom {get {return y + height;} set{}}

        Vector2 origin;

        // // ========================================================== CONSTRUCTORS ==========================================================
        // Rectangle
        public CollisionShape(float newX, float newY, float newWidth, float newHeight)
        {
            x = newX;
            y = newY;
            position = new Vector2(newX, newY);
            width = newWidth;
            height = newHeight;
            origin = new Vector2(newWidth / 2, newHeight / 2);
        }

        // Circle
        public CollisionShape(float newX, float newY, float radius)
        {
            x = newX;
            y = newY;
            position = new Vector2(newX, newY);
            width = radius;
            height = radius;
            origin = new Vector2(newX + radius, newY + radius);
        }

        // ========================================================== INTERSECTIONS ==========================================================
        // Currently the code only checks for rectangle <-> rectangle collision. Other collision methods can be added later
        // ------------------------------------------ Top
        public bool topIntersection(CollisionShape shapeA)
        {    
            return (bottom > shapeA.top) && (top < shapeA.top) && (right > shapeA.left) && (left < shapeA.right);
        }
        
        // ------------------------------------------ Bottom
        public bool bottomIntersection(CollisionShape shapeA)
        {
            return (top <= shapeA.bottom) && (bottom >=  shapeA.bottom) && (right >= shapeA.left) && (left <=  shapeA.right);
        }
        
        // ------------------------------------------ Left
        public bool leftIntersection(CollisionShape shapeA) 
        {
            return (right >= shapeA.left) && (left < shapeA.left) &&  (top <= shapeA.bottom) && (bottom >= shapeA.top);
        }
        
        // ------------------------------------------ Right
        public bool rightIntersection(CollisionShape shapeA)
        {
            return (left <= shapeA.right) && (right > shapeA.right) && (top <= shapeA.bottom) && (bottom >= shapeA.top);
        }
       
        // ------------------------------------------ Any
        public bool isIntersecting(CollisionShape shapeA)
        {   
            // Boolean values for each type of intersection
            return leftIntersection(shapeA) || rightIntersection(shapeA) || topIntersection(shapeA) || bottomIntersection(shapeA);
        }

        public bool isInside(CollisionShape shapeA)
        {
            return leftIntersection(shapeA) && rightIntersection(shapeA) && topIntersection(shapeA) && bottomIntersection(shapeA);
        }




    }
}