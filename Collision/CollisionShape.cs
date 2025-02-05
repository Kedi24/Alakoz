using System;
using System.Collections.Generic;
using System.Transactions;
using Alakoz.GameInfo;
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
        public Vector2 origin;
        float x;
        float y;
        float width;
        float height;
        public float left {get {return x;} set{}}
        public float right {get {return x + width;} set{}}
        public float top {get {return y ;} set{}}
        public float bottom {get {return y + height;} set{}}
        TShape shape = TShape.RECTANGLE;

        // // ========================================================== CONSTRUCTORS ==========================================================
        // Rectangle
        public CollisionShape(float newX, float newY, float newWidth, float newHeight){
            x = newX;
            y = newY;
            position = new Vector2(newX, newY);
            width = newWidth;
            height = newHeight;
            origin = new Vector2(newWidth / 2, newHeight / 2);
            shape = TShape.RECTANGLE;
        }

        // Circle
        public CollisionShape(float newX, float newY, float radius)
        {
            x = newX;
            y = newY;
            position = new Vector2(newX, newY);
            width = radius*2;
            height = radius*2;
            origin = new Vector2(newX + radius, newY + radius);
            shape = TShape.CIRCLE;
        }

        // ========================================================== INTERSECTIONS ==========================================================
        // Currently the code only checks for rectangle <-> rectangle collision. Other collision methods can be added later
        // ------------------------------------------ Top
        public bool topIntersection(CollisionShape shapeA)
        {    
            return (bottom > shapeA.top) && (top < shapeA.top) && (right > shapeA.left) && (left < shapeA.right);
        }
        public static bool topIntersection( float firstX, float firstY, float firstWidth, float firstHeight, string firstType, float secondX, float secondY, float secondWidth, float secondHeight, string secondType){
            bool intersectTop = ( 
                firstX <= (secondX + secondWidth) ) && // f-Left < s-Right
                ((firstX + firstWidth) > secondX ) && // f-Right > s-Left

                (firstY < secondY) && // f-Top < s-Top
                ((firstY + firstHeight) > secondY); // f-Bottom > s-Top
            return intersectTop;
        }
        
        // ------------------------------------------ Bottom
        public bool bottomIntersection(CollisionShape shapeA)
        {
            return (top <= shapeA.bottom) && (bottom >=  shapeA.bottom) && (right >= shapeA.left) && (left <=  shapeA.right);
        }

        public static bool bottomIntersection( float firstX, float firstY, float firstWidth, float firstHeight, string firstType, float secondX, float secondY, float secondWidth, float secondHeight, string secondType){
            bool intersectBottom = ( 
                firstX <= (secondX + secondWidth) ) && // f-Left < s-Right
                ((firstX + firstWidth) > secondX ) && // f-Right > s-Left

                ((firstY + firstHeight) > (secondY + secondHeight)) && // f-Bottom > s-Bottom
                (firstY < (secondY + secondHeight)); // f-Top < s-Bottom;
            return intersectBottom;
        }
        
        // ------------------------------------------ Left
        public bool leftIntersection(CollisionShape shapeA) 
        {
            return (right >= shapeA.left) && (left < shapeA.left) &&  (top <= shapeA.bottom) && (bottom >= shapeA.top);
        }

        public static bool leftIntersection( float firstX, float firstY, float firstWidth, float firstHeight, string firstType, float secondX, float secondY, float secondWidth, float secondHeight, string secondType){
            bool intersectLeft = ( 
                firstX < secondX ) && // f-Left < s-Left
                ((firstX + firstWidth) > secondX ) && // f-Right > s-Left

                (firstY < (secondY + secondHeight)) && // f-Top < s-Bottom
                ((firstY + firstHeight) > secondY); // f-Bottom > s-Top
            return intersectLeft;
        }

        
        // ------------------------------------------ Right
        public bool rightIntersection(CollisionShape shapeA)
        {
            return (left <= shapeA.right) && (right > shapeA.right) && (top <= shapeA.bottom) && (bottom >= shapeA.top);
        }

                public static bool rightIntersection( float firstX, float firstY, float firstWidth, float firstHeight, string firstType, float secondX, float secondY, float secondWidth, float secondHeight, string secondType){
            bool intersectRight = (
                (firstX + firstWidth) > (secondX + secondWidth) ) && // f-Right > s-Right
                (firstX < (secondX + secondWidth) ) && // f-Left < s-Right

                (firstY < (secondY + secondHeight)) && // f-Top < s-Bottom
                ((firstY + firstHeight) > secondY); // f-Bottom > s-Top
            return intersectRight;
        }

                // ------------------------------------------ Inside
        public static bool insideIntersection( float firstX, float firstY, float firstWidth, float firstHeight, string firstType, float secondX, float secondY, float secondWidth, float secondHeight, string secondType){
            bool intersectInside = (
                firstX >= secondX) &&
                ((firstX + firstWidth) <= (secondX + secondWidth))  &&

                (firstY >= secondY) &&
                ((firstY + firstHeight) <= (secondY + secondHeight));
            return intersectInside;
        }
        public static bool isOutside( float firstX, float firstY, float firstWidth, float firstHeight, string firstType, float secondX, float secondY, float secondWidth, float secondHeight, string secondType){
            bool intersectOutside = (
                firstX <= secondX) &&
                ((firstX + firstWidth) >= (secondX + secondWidth))  &&

                (firstY <= secondY) &&
                ((firstY + firstHeight) >= (secondY + secondHeight));
            return intersectOutside;
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

        public static bool[] testIntersection( float firstX, float firstY, float firstWidth, float firstHeight, string firstType, float secondX, float secondY, float secondWidth, float secondHeight, string secondType){
                
                bool intersectLeft = leftIntersection(
                    firstX, firstY, firstWidth, firstHeight, firstType,
                    secondX, secondY, secondWidth, secondHeight, secondType
                );

                bool intersectRight = rightIntersection(
                    firstX, firstY, firstWidth, firstHeight, firstType,
                    secondX, secondY, secondWidth, secondHeight, secondType
                );

                bool intersectTop = topIntersection(
                    firstX, firstY, firstWidth, firstHeight, firstType,
                    secondX, secondY, secondWidth, secondHeight, secondType
                );

                bool intersectBottom = bottomIntersection(
                    firstX, firstY, firstWidth, firstHeight, firstType,
                    secondX, secondY, secondWidth, secondHeight, secondType
                );

                bool intersectInside = insideIntersection(
                    firstX, firstY, firstWidth, firstHeight, firstType,
                    secondX, secondY, secondWidth, secondHeight, secondType
                );
            bool result = intersectLeft || intersectRight || intersectTop || intersectBottom || intersectInside;

            bool[] resultList = {result, intersectLeft, intersectRight, intersectTop, intersectBottom, intersectInside};

            return resultList;
        }
    }
}