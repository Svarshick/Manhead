using Microsoft.Xna.Framework;

namespace Manhead.Core.Logic.Gameplay.Data;

public class Field<T>
{
    private readonly List<T>[][] _data;
    
    public Field(int width, int height)
    {
         if (width <= 0 || height <= 0)
            throw new ArgumentOutOfRangeException("width and height must be greater than 0");
         
         _data = new List<T>[width][];
         for (int x = 0; x < width; x++)
         {
             var column = new List<T>[height];
             for (int y = 0; y < height; y++)
             {
                 column[y] = new List<T>();
             }
             _data[x] = column;
         }
    }

    public int Width => _data.Length;
    public int Height => _data[0].Length;
    public bool Has(Point position) => 
        0 <= position.X && position.X < Width && 
        0 <= position.Y && position.Y < Height;
    
    public bool Has(int x, int y) => 
        0 <= x && x < Width && 
        0 <= y && y < Height;
    
    public List<T> this[Point position] =>  _data[position.X][position.Y];
    public Column this[int x] => new Column(_data[x]);
    
    public readonly struct Column(List<T>[] data)
    {
        public List<T> this[int y] => data[y];
    }
}