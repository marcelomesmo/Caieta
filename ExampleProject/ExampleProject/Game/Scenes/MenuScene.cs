using System;
using Caieta;
using Caieta.Audio;
using Caieta.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TiledSharp;

namespace ExampleProject.Game.Scenes
{
    public class MenuScene : Scene
    {
        Player Player1;

        Background Bg;
        TiledBackground Bg2;
        ScrollingBackground Bg3;
        Floor floor;

        TiledMap Scenario;

        public MenuScene() : base()
        {
            OnSceneStart += LoadTextStart;
            OnSceneEnd += LoadTextEnd;

            Layout = new Rectangle(0, 0, 640, 360);
        }

        public override void Awake()
        {
            base.Awake();

            Add(new Layer("Background"));
            Layers["Background"].Parallax = new Vector2(0, 0);
            Add(new Layer("HUD"));
            Layers["HUD"].SetGlobal();
            Add(new Layer("Objects"));

            Player1 = new Player("Player1");
            Layers["HUD"].Add(Player1);

            Bg = new Background("Background1");
            Layers["Background"].Add(Bg);

            //Bg2 = new TiledBackground("Background1", Resources.Get<Texture2D>("BgResTest2"), new Vector2(2, 2));
            Bg2 = new TiledBackground("Background2", Resources.Get<Texture2D>("BgResTest2"), 600, 280);
            //Layers["Background"].Add(Bg2);

            Bg3 = new ScrollingBackground("Background3", Resources.Get<Texture2D>("Profectus_Background_1"), new Vector2(-1, 0), new Vector2(100, 100));
            Layers["Background"].Add(Bg3);

            floor = new Floor("Chao");
            Layers["Objects"].Add(floor);

            Scenario = new TiledMap("Tiled Bg", "Content/Tiled/untitled.tmx");
            Layers["Objects"].Add(Scenario);

            AudioManager.Music.Load("mus_rmx_01_forsakencity_intro");
            AudioManager.Music.Load("Profectus_Theme_01");

            AudioManager.SFX.Load("Coleta");
            AudioManager.SFX.Load("Correndo_2_v2");
            AudioManager.SFX.Load("Morrendo");
            AudioManager.SFX.Load("Pulo");
            AudioManager.SFX.Load("respawn");
            AudioManager.SFX.Load("start");
            //Input.Touch.OnTap += OnTap;

            // Create objects from Tiled CollisionMask
            foreach (var tiled_obj in Scenario.GetCollisionMask())
            {
                if (tiled_obj.ObjectType == TmxObjectType.Basic)
                {
                    switch (tiled_obj.Name)
                    {
                        case "Blue_Orb":
                        case "Red_Orb":
                            // Create Orb
                            Orb orb = new Orb(tiled_obj.Name);

                            // Create Sprite
                            var orb_color = tiled_obj.Name.Split('_')[0];  // Cut "Color_Orb" name into "Color"
                            orb.image = new Sprite()
                                       .Add(new Animation("idle", Resources.Get<Texture2D>("Sheet/Profectus_Collectable_" + orb_color), 1, 8, 300));
                            orb.image.SetAnimation("idle");
                            orb.image.SetOrigin(Animation.Anchor.TOP_LEFT);

                            // Create Collision Hitbox
                            BoxCollider Collider = new BoxCollider((float)tiled_obj.Width, (float)tiled_obj.Height, (float)tiled_obj.X, (float)tiled_obj.Y);
                            orb.collider = Collider;
                            orb.collider.IsTrigger = true;

                            // Notes: This will swap orb position with collider position we got from tiled.
                            orb.Transform.Position = new Vector2(Collider.Left, Collider.Top);
                            orb.collider.Origin = Vector2.Zero;

                            // Add Orb to Layer
                            Layers["Objects"].Add(orb);

                            break;

                        case "Hazard":
                            //Debug.Log("Hazard '" + tiled_obj.Name + "_" + tiled_obj.Id + "' located.");
                            Entity spikes = new Entity("Spikes");

                            BoxCollider SpikeCollider = new BoxCollider((float)tiled_obj.Width, (float)tiled_obj.Height, (float)tiled_obj.X, (float)tiled_obj.Y);
                            spikes.Add(SpikeCollider);

                            Layers["Objects"].Add(spikes);

                            break;

                        default:
                            break;
                    }
                }
            }

            /*foreach(var obj in Scenario.GetCollisionMask("Blue_Orb"))
            {
                Orb orb = new Orb("Blue Orb");

                orb.image = new Sprite()
                           .Add(new Animation("idle", Resources.Get<Texture2D>("Sheet/Profectus_Collectable_Blue"), 1, 8, 300));
                orb.image.SetAnimation("idle");
                orb.image.SetOrigin(Animation.Anchor.TOP_LEFT);

                orb.collider = obj;
                orb.collider.IsTrigger = true;

                // Notes: This will swap orb position with collider position we got from tiled.
                orb.Transform.Position = new Vector2(obj.Left, obj.Top);
                orb.collider.Origin = Vector2.Zero;

                Layers["Objects"].Add(orb);
            }

            foreach (var obj in Scenario.GetCollisionMask("Red_Orb"))
            {
                Orb orb = new Orb("Red Orb");

                orb.image = new Sprite()
                           .Add(new Animation("idle", Resources.Get<Texture2D>("Sheet/Profectus_Collectable_Red"), 1, 8, 300));
                orb.image.SetAnimation("idle");
                orb.image.SetOrigin(Animation.Anchor.TOP_LEFT);

                orb.collider = obj;
                orb.collider.IsTrigger = true;

                // Notes: This will swap orb position with collider position we got from tiled.
                orb.Transform.Position = new Vector2(obj.Left, obj.Top);
                orb.collider.Origin = Vector2.Zero;

                Layers["Objects"].Add(orb);
            }*/

        }

        public void OnTap(Entity e)
        {
            Debug.Log("Test");
        }

        public override void End()
        {
            base.End();

            // Unload Audio Tracks added this Scene
            AudioManager.Unload();
        }

        int blueorb = 0;
        int orbcolides = 0;

        public override void Update()
        {
            base.Update();

            Input.Touch.OnTouchStart = () =>
            {
                var WorldPos = Engine.SceneManager.Camera.ScreenToCamera(Input.Touch.Position);
                Debug.Log("Touch pos (World): X " + WorldPos.X + " Y " + WorldPos.Y);
                Debug.Log("Touch pos (Screen): X " + Input.Touch.Position.X + " Y " + Input.Touch.Position.Y);
            };

            Player1.Hitbox.OnCollision = (e) =>
            {
                if(e.Name == "Blue_Orb")
                {
                    Debug.Log("Collision on Blue Orb");
                    blueorb++;
                    //Debug.Log(blueorb);
                    //e.Destroy();
                }
                else if (e.Name == "Red_Orb")
                {
                    Debug.Log("Collision on Red Orb");
                    //blueorb++;
                    //Debug.Log(blueorb);
                    //e.Destroy();
                }
                /*else if (e.Name == "Spikes")
                {
                    Debug.Log("Collision on Spikes");
                    Player1.Destroy();
                }*/
            };

            foreach (Orb orb in Layers["Objects"].GetAllEntities<Orb>())
            {
                orb.collider.OnCollision = (e) =>
                {
                    if(e == Player1)
                    {
                        orbcolides++;
                        //Debug.Log(orbcolides);
                        //orb.Destroy();
                    }

                };
            }

            Input.Touch.OnDoubleTap = () =>
            {
                Debug.Log("Double Tap 2");
            };

            if (Input.Keyboard.Pressed(Keys.L))
            {
                Engine.SceneManager.LoadScene("Fase1");
            }

            // SFX
            if (Input.Keyboard.Pressed(Keys.OemCloseBrackets))
                AudioManager.SFX.SetVolume(AudioManager.SFX.Volume + 1);

            if (Input.Keyboard.Pressed(Keys.OemOpenBrackets))
                AudioManager.SFX.SetVolume(AudioManager.SFX.Volume - 1);

            if (Input.Keyboard.Pressed(Keys.Z))
                AudioManager.SFX.Play("Coleta", true);

            if (Input.Keyboard.Pressed(Keys.X))
                AudioManager.SFX.Play("Correndo_2_v2");

            if (Input.Keyboard.Pressed(Keys.C))
                AudioManager.SFX.Play("Morrendo");

            if (Input.Keyboard.Pressed(Keys.V))
                AudioManager.SFX.Play("Pulo");

            if (Input.Keyboard.Pressed(Keys.B))
                AudioManager.SFX.Play("respawn");

            if (Input.Keyboard.Pressed(Keys.N))
                AudioManager.SFX.Play("start");


            // MUSIC
            if (Input.Keyboard.Pressed(Keys.O))
            {
                AudioManager.Music.Play("mus_rmx_01_forsakencity_intro");
            }

            if (Input.Keyboard.Pressed(Keys.P))
            {
                AudioManager.Music.Play("Profectus_Theme_01");
            }

            if (Input.Keyboard.Pressed(Keys.S))
            {
                AudioManager.Music.Stop();
            }

            if (Input.Keyboard.Pressed(Keys.T))
            {
                AudioManager.Music.Pause();
            }

            if (Input.Keyboard.Pressed(Keys.R))
            {
                AudioManager.Music.Resume();
            }

            if (Input.Keyboard.Hold(Keys.OemPlus))
            {
                AudioManager.Music.SetVolume(AudioManager.Music.Volume + 1);
            }

            if (Input.Keyboard.Hold(Keys.OemMinus))
            {
                AudioManager.Music.SetVolume(AudioManager.Music.Volume - 1);
            }

            if (Input.Keyboard.Pressed(Keys.M))
            {
                AudioManager.Music.Mute();
            }

            if (Input.Keyboard.Pressed(Keys.U))
            {
                AudioManager.Music.UnMute();
            }
        }

        /*
        public override void Render()
        {
            base.Render();

            Graphics.DrawText("MENU SCREEN", new Vector2(Graphics.Width/2, Graphics.Height/2), Color.White);

        }
        */       

        public void LoadTextStart()
        {
            Debug.Log("[MenuScene]: OnSceneStart test.");
        }

        public void LoadTextEnd()
        {
            Debug.Log("[MenuScene]: OnSceneEnd entity test " + Player1.Name);
            Debug.Log("[MenuScene]: OnSceneEnd entity list test " + Layers["Background"].Population());
            Debug.Log("[MenuScene]: OnSceneEnd test.");
        }
    }
}
