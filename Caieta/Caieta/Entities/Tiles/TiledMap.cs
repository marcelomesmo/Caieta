using System;
using System.Collections.Generic;
using Caieta.Components.Attributes;
using Caieta.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace Caieta
{
    public class TiledMap : Entity
    {
        TmxMap Map;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        // TODO Notes: Later add support to multiple Tilesets (i.e. solid, environment, props, etc).
        public Texture2D[] Tileset { get; private set; }
        public int TilesetTilesWide { get; private set; }
        public int TilesetTilesHigh { get; private set; }

        //public Dictionary<int, Sprite> AnimatedTiles;

        public TiledMap(string name, string map, bool initial_visibility = true) : base(name, initial_visibility)
        {
            // Load Map
            // Notes: Remember to check file: Copy to Output folder.
            Map = new TmxMap(map);

            Width = Map.Width;
            Height = Map.Height;
            TileWidth = Map.TileWidth;
            TileHeight = Map.TileHeight;

            Tileset = new Texture2D[Map.Tilesets.Count];

            // Load TileSets
            var tileset_count = 0;
            foreach(TmxTileset tileset in Map.Tilesets)
            {
                // Notes: Check if this System.IO can break compatibility.
                var tileset_path = "Tiled/" + System.IO.Path.GetFileNameWithoutExtension(Map.Tilesets[tileset_count].Image.Source);
                //Debug.Log(tileset_path);
                Tileset[tileset_count] = Resources.Get<Texture2D>(tileset_path);
                tileset_count++;
                //Debug.Log(tileset_path + " Loaded");
                if (Tileset == null)
                    Debug.ErrorLog("[TileMap]: Couldn't find tileset at '" + tileset_path + "'. Make sure it's in the Tiled/ folder.");
            }

            TilesetTilesWide = Tileset[0].Width / TileWidth;
            TilesetTilesHigh = Tileset[0].Height / TileHeight;

            //AnimatedTiles = new Dictionary<int, Sprite>();
        }

        public override void Create()
        {
            base.Create();

            //Debug.Log("[Tilemap]: Loading map. Width: '" + Map.Width + "' Height: '" + Map.Height + "'.");

            var tileset_count = 0;
            foreach(TmxTileset tileset in Map.Tilesets)
            {
                Debug.Log("[Tilemap]: Loading tileset '" + Map.Tilesets[tileset_count].Name + "' path '" + Tileset[tileset_count].Name + "'.");

                // Has animation?
                /*int totalAnimations = tileset.Tiles.Count;
                if (totalAnimations > 0)
                {
                    int[] animIds, animDur;
                    int count = 0;
                    foreach (KeyValuePair<int, TmxTilesetTile> tiles  in tileset.Tiles)
                    {
                        int tileIdOnMap = tiles.Key;
                        TmxTilesetTile tileAnim = tiles.Value;

                        animIds = new int[tileAnim.AnimationFrames.Count];
                        animDur = new int[tileAnim.AnimationFrames.Count]; 
                        Debug.Log("[TiledMap]: New animation ID '" + tileIdOnMap + "'. Frame Ids [Duration]: ");
                        foreach (TmxAnimationFrame frame in tileAnim.AnimationFrames)
                        {
                            animIds[count] = frame.Id;
                            animDur[count] = frame.Duration;
                            Debug.Log(animIds[count] + " [" + animDur[count] + "ms]");
                        }

                        Sprite animatedTile = new Sprite()
                            .Add(new Animation("default", Tileset[tileset_count], new Vector2(TilesetTilesWide, TilesetTilesHigh), animIds).SetDuration(animDur));
                        animatedTile.SetAnimation("default");

                        AnimatedTiles.Add(tileIdOnMap, animatedTile);
                    }
                }*/

                tileset_count++;
            }

            // Add Sprite Animations
            //foreach (Sprite animatedTile in AnimatedTiles.Values)
            //    Add(animatedTile);


            //Debug.Log("   Tiles wide: " + TilesetTilesWide + " Tiles high:" + TilesetTilesHigh + ".");

            // Load Tiles
            // Grab all layers
            for (var layer = 0; layer < Map.Layers.Count; layer++)
            {
                //Debug.Log("[Tilemap]: Layer '" + Map.Layers[layer].Name + "' contain " + Map.Layers[layer].Tiles.Count + " tiles.");
                //Debug.Log("    Will load from tileset '" + Tileset[layer < tileset_count ? layer : 0].Name + "' [" + layer + "]["+ tileset_count+"].");

                for (var tile = 0; tile < Map.Layers[layer].Tiles.Count; tile++)
                {
                    int gid = Map.Layers[layer].Tiles[tile].Gid;

                    // Empty tile gid = 0, do nothing
                    if (gid != 0)
                    {
                        // Create Tile
                        TiledTile Tile = new TiledTile(this);
                        Tile.Opacity = (float)Map.Layers[layer].Opacity * 100;

                        int tileFrame = gid - Map.Tilesets[layer < tileset_count ? layer : 0].FirstGid;

                        // Already a Animation Tile, dont add
                        //if (AnimatedTiles.ContainsKey(tileFrame))
                        //    continue;

                        int column = tileFrame % TilesetTilesWide;
                        int row = (int)Math.Floor((double)tileFrame / (double)TilesetTilesWide);
                        //Debug.Log("     gid "+ gid + " tileframe " + tileFrame + " c: " + column + " r: " + row);

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

                        Tile.TileID = tileFrame;
                        Tile.Position = position;
                        Tile.ClipRect = tilesetRec;
                        Tile.TilesetNum = layer < tileset_count ? layer : 0;

                        Add(Tile);
                    }
                }
            }

            // Load Solid Objects and Platforms
            if (Map.ObjectGroups.Contains("Collision"))
            {
                // Debug.Log("[Tilemap]: Map '" + Name + "'. Loading collision mask 'Collision' for solids and platforms.");

                foreach (var mapObject in Map.ObjectGroups["Collision"].Objects)
                {
                    // Notes: Only support basic (rectangle) shapes
                    if (mapObject.ObjectType == TmxObjectType.Basic)
                    {
                        //BoxCollider Collider = new BoxCollider((float)mapObject.Width, (float)mapObject.Height, (float)mapObject.X, (float)mapObject.Y);
                        BoxCollider Collider;

                        if (mapObject.Name.Equals("OneWayPlatform"))
                        {
                            try
                            {
                                var _errorCheck = mapObject.Properties["FallDown"];
                            }
                            catch
                            {
                                Debug.ErrorLog("[TiledMap]: OneWayPlatform without property 'FallDown' at position X: " + mapObject.X + " Y: " + mapObject.Y + ".");
                            }
                            Collider = new OneWayPlatform(mapObject.Properties["FallDown"] == "true" ? true : false, (float)mapObject.Width, (float)mapObject.Height, (float)mapObject.X, (float)mapObject.Y);
                        }
                        else
                            Collider = new Solid((float)mapObject.Width, (float)mapObject.Height, (float)mapObject.X, (float)mapObject.Y);

                        Add(Collider);
                    }
                    /*else if (objectMap.ObjectType == TmxObjectType.Polygon)
                    {
                        //TODO
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
                //Debug.Log("[Tilemap]: Map '" + Name + "'. Loading collision mask '" + mask + "'.");

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
