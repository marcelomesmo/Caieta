using System;
using System.Collections.Generic;
using Caieta.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace Caieta
{
    public class TiledMap : Entity
    {
        TmxMap Map;
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        // TODO Notes: Later add support to multiple Tilesets (i.e. solid, environment, props, etc).
        public Texture2D Tileset { get; private set; }
        public int TilesetTilesWide { get; private set; }
        public int TilesetTilesHigh { get; private set; }

        public TiledMap(string name, string map, bool initial_visibility = true) : base(name, initial_visibility)
        {
            // Load Map
            Map = new TmxMap(map);

            TileWidth = Map.TileWidth;
            TileHeight = Map.TileHeight;

            // Load TileSet
            var tileset_path = "Tiled/" + System.IO.Path.GetFileNameWithoutExtension(Map.Tilesets[0].Image.Source);
            Tileset = Resources.Get<Texture2D>(tileset_path);

            TilesetTilesWide = Tileset.Width / TileWidth;
            TilesetTilesHigh = Tileset.Height / TileHeight;
        }

        public override void Create()
        {
            base.Create();

            Debug.Log("[Tilemap]: Loading map. Width: '" + Map.Width + "' Height: '" + Map.Height + "'.");

            Debug.Log("[Tilemap]: Loading tileset '" + Map.Tilesets[0].Name + "' path '" + Tileset.Name + "'.");
            Debug.Log("   Tiles wide: " + TilesetTilesWide + " Tiles high:" + TilesetTilesHigh + ".");

            // Load Tiles
            // Grab all layers
            for (var layer = 0; layer < Map.Layers.Count; layer++)
            {
                Debug.Log("[Tilemap]: Layer '" + Map.Layers[layer].Name + "' contain " + Map.Layers[layer].Tiles.Count + " tiles.");

                for (var tile = 0; tile < Map.Layers[layer].Tiles.Count; tile++)
                {
                    int gid = Map.Layers[layer].Tiles[tile].Gid;

                    // Empty tile gid = 0, do nothing
                    if (gid != 0)
                    {
                        // Create Tile
                        TiledTile Tile = new TiledTile(this);
                        Tile.Opacity = (float)Map.Layers[layer].Opacity * 100;

                        int tileFrame = gid - 1;
                        int column = tileFrame % TilesetTilesWide;
                        int row = (int)Math.Floor((double)tileFrame / (double)TilesetTilesWide);
                        //Debug.Log("     tileframe " + tileFrame + " c: " + column + " r: " + row);

                        float tile_x = ((tile % Map.Width) * TileWidth);
                        float tile_y = ((float)Math.Floor(tile / (double)Map.Width) * TileHeight);
                        Vector2 position = new Vector2(
                            tile_x + (float)Map.Layers[layer].OffsetX,
                            tile_y + (float)Map.Layers[layer].OffsetY
                           );

                        Rectangle tilesetRec = new Rectangle(
                           TileWidth * column,
                           TileHeight * row,
                           TileWidth,
                           TileHeight);

                        Tile.Position = position;
                        Tile.ClipRect = tilesetRec;

                        Add(Tile);
                    }
                }
            }

            // Load Solid Objects
            if (Map.ObjectGroups.Contains("Collision"))
            {
                Debug.Log("[Tilemap]: Map '" + Name + "'. Loading collision mask 'Solid'.");

                foreach (var objectMap in Map.ObjectGroups["Collision"].Objects)
                {
                    //string name = objectMap.Name + "_" + objectMap.Id;

                    // Notes: Only support basic (rectangle) shapes
                    if (objectMap.ObjectType == TmxObjectType.Basic)
                    {
                        // Create Collision Hitbox
                        BoxCollider Collider = new BoxCollider((float)objectMap.Width, (float)objectMap.Height, (float)objectMap.X, (float)objectMap.Y);

                        Add(Collider);
                    }
                    /*else if (objectMap.ObjectType == TmxObjectType.Polygon)
                    {
                        //todo
                    }*/
                }

            }
        }

        public List<BoxCollider> GetCollisionMask(string mask)
        {
            List<BoxCollider> list = new List<BoxCollider>();

            // Load Objects
            if (Map.ObjectGroups.Contains("CollisionMasks"))
            {
                Debug.Log("[Tilemap]: Map '" + Name + "'. Loading collision mask '" + mask + "'.");

                foreach (var objectMap in Map.ObjectGroups["CollisionMasks"].Objects)
                {
                    //string name = objectMap.Name + "_" + objectMap.Id;

                    if (objectMap.Name == mask && objectMap.ObjectType == TmxObjectType.Basic)
                    {
                        // Create Collision Hitbox
                        BoxCollider Collider = new BoxCollider((float)objectMap.Width, (float)objectMap.Height, (float)objectMap.X, (float)objectMap.Y);

                        list.Add(Collider);
                    }
                    /*else if (objectMap.ObjectType == TmxObjectType.Polygon)
                    {
                        //todo
                    }*/
                }
            }

            return list;
        }

        public TmxList<TmxObject> GetCollisionMask()
        {
            return Map.ObjectGroups["CollisionMasks"].Objects;
        }

        public override void Render()
        {
            base.Render();

            // Grab all layers
            /*for (var layer = 0; layer < Map.Layers.Count; layer++)
            {
                //Debug.Log("[Tilemap]: Layer '" + Map.Layers[layer].Name + "'");
                for (var tile = 0; tile < Map.Layers[layer].Tiles.Count; tile++)
                {
                    //Debug.Log("     Tile "+ tile + "/" + Map.Layers[layer].Tiles.Count + "' gid: " + Map.Layers[layer].Tiles[tile].Gid);
                    //tile.Opacity = Map.Layers[layer].Opacity * 255;
                    int gid = Map.Layers[layer].Tiles[tile].Gid;

                    // Empty tile gid = 0, do nothing
                    if (gid != 0)
                    {
                        int tileFrame = gid - 1;
                        int column = tileFrame % TilesetTilesWide;
                        int row = (int)Math.Floor((double)tileFrame / (double)TilesetTilesWide);
                        //Debug.Log("     tileframe " + tileFrame + " c: " + column + " r: " + row);

                        float tile_x = ((tile % Map.Width) * TileWidth);
                        float tile_y = ((float)Math.Floor(tile / (double)Map.Width) * TileHeight);
                        Vector2 position = new Vector2(
                            tile_x + (float)Map.Layers[layer].OffsetX + Entity.Transform.Position.X,
                            tile_y + (float)Map.Layers[layer].OffsetY + Entity.Transform.Position.Y
                           );

                        Rectangle tilesetRec = new Rectangle(
                           Til Width * column,
                           TileHeight * row,
                           TileWidth,
                           TileHeight);

                        Graphics.Draw(
                            Tileset,
                            new Rectangle((int)position.X, (int)position.Y, TileWidth, TileHeight),
                            tilesetRec,
                            Color * ((float)Map.Layers[layer].Opacity * 255));

                        
                        Graphics.Draw(
                            Tileset,
                            new Rectangle((int)position.X, (int)position.Y, TileWidth, TileHeight),
                            tilesetRec,
                            Color * ((float) Map.Layers[layer].Opacity * 255),
                            Entity.Transform.Rotation,
                            Entity.Transform.Position,
                            Effects,
                            0);


                    }
                }
            }

        }*/
        }
    }
}
