using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Graphics_Practice
{
    public static class NoiseMaker
    {
       


        public static Random randy = new Random();


        public static int[] pHash = GenerateHash(128);
       
        public static float[] CreateNoiseMap(int width, int height, float freq)
        {

            float[] noiseMap = new float[width * height];


            for (int i = 0; i < noiseMap.Length; i++)
            {
                float currentfreq = freq;
                float a = 1.0f;


                int column = i % width;
                int row = (int)Math.Floor((double)i / width);

                float num = 0;

                for (int o = 0; o < 8; o++)
                {
                    num += a * Perlin(column * currentfreq, row * currentfreq);

                    a *= 0.5f;
                    currentfreq *= 2;

                }

                num = (num + 1) / 2;




                noiseMap[i] = num;

            }



            return noiseMap;
        }


        public static Color[] DrawNoise(float[] noisemap, float a = 0, float b = 0, float c = 0)
        {
            //Currently just generates some colours based on the cutoff parameters a,b,c
            Color[] colormap = new Color[noisemap.Length];

            float num;

            for (int i = 0; i < noisemap.Length; i++)
            {
                num = noisemap[i];

                if(num > a)
                    colormap[i].R = (byte)(255 * (num - a));
                else
                    colormap[i].R = (byte)(255 * (a - num));
          
                if (num > b)
                    colormap[i].G = (byte)(255 * num - b);
                else
                    colormap[i].G = (byte)(255 * (b - num));

                if (num > c)
                    colormap[i].B = (byte)(255 * num - c);
                else
                    colormap[i].B = (byte)(255 * (c - num));

            }
            return colormap;
        }

        private static float Perlin(float x, float y)
        {
            int X = (int)MathF.Floor(x) & (pHash.Length/2) - 1;
            int Y = (int)MathF.Floor(y) & (pHash.Length / 2) - 1;


            /*
            (x,y) are floats based on the frequency, where
                
                x = gx * freq, y = gy * freq
                (gx, gy) = integer coordinates of the grid

                as frequency gets smaller and smaller, the noise will be more generalized creating larger clumps
            
             
             */
            
            float dx = x - MathF.Floor(x);
            float dy = y - MathF.Floor(y);


            



            //Inward Vectors:
            Vector2 topRight = new Vector2(dx - 1.0f, dy - 1.0f);
            Vector2 topLeft = new Vector2(dx, dy - 1.0f);
            Vector2 bottomLeft = new Vector2(dx, dy);        
            Vector2 bottomRight = new Vector2( dx - 1.0f, dy);


            //Does the ol dot product to make a weight
            //we do it for each corner of a cell
            //the second factor in each will be based on a random value in the hash
            //The gradient can only go in 4 directions
            //if dx is small we will have the same value for our square here as X and Y are floored 
            float dottopLeft = Vector2.Dot(topLeft, AssignGradient(pHash[ pHash[X] + Y + 1 ] ));

            float dottopRight = Vector2.Dot(topRight, AssignGradient(pHash[ pHash[X + 1] + Y  + 1] ));

            float dotbottomLeft = Vector2.Dot(bottomLeft, AssignGradient(pHash[pHash[X] + Y]));
            float dotbottomRight = Vector2.Dot(bottomRight, AssignGradient(pHash[ pHash[X + 1] + Y] ));

            float u = EaseCurve(dx);
            float v = EaseCurve(dy);

            

            float final = Lerp(Lerp(dotbottomLeft, dottopLeft, v),
                               Lerp(dotbottomRight, dottopRight, v),
                               u);
            return final;
        }

        private static Vector2 AssignGradient(int value)
        {
            //mods a value from the hash by 4 and assigns a direction
            int m = value & 3;

            

            switch(m)
            {
                case (0):
                    return Vector2.One;
                case (1):
                    return new Vector2(1, -1);
                case (2):
                    return new Vector2(-1, 1);
                case (3):
                    return new Vector2(-1, -1);
            }

          

            return Vector2.Zero;
        }

        public static float Lerp(float a, float b, float t)
        {
           
            //lerp
            return a + (t * (b - a));
        }

        public static float EaseCurve(float x)
        {
            //Literally just a copymathta
            //causes values to tend toward integers in a smoothy way
            
            return ((((6 * x) - 15) * x) + 10) * x * x * x;
        }


        public static int[] GenerateHash(int length)
        {
            //Makes a new seed for our noise
            //think of it like when you enter your dog's name as a minecraft seed
            Random ran = new Random();
            int[] hashArray = new int[length * 2];

            for(int i=0; i < length; i++)
            {
                hashArray[i] = ran.Next(0,length);
            }

            for(int i=0; i < length; i++)
            {
                hashArray[length  + i] = hashArray[i];
            }

            return hashArray;
        }



       
    }
}
