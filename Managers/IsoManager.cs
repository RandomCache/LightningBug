using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LightningBug.UI;
using LightningBug.Levels;

namespace LightningBug
{
    public class IsoManager
    {
        Texture2D hilight;
        IsoLevel curLevel;
        Vector2 hilightPoint;

        // Iso Editor objects
        public IsoEditor isoEditor;
        public Dictionary<string, TileSet> tileSets;

        public Levels.IsoLevel CurLevel
        {
            get { return curLevel; }
            set { curLevel = CurLevel; }
        }

        public IsoManager()
        {
            curLevel = null;
            tileSets = new Dictionary<string, TileSet>();
        }

        public void Initialize(ContentManager cm)
        {
            curLevel = new Levels.IsoLevel(cm);
        }

        public Dictionary<string, TileSet> GetTileSets()
        {
            return tileSets;
        }

        public bool LoadIsoContent(GraphicsDeviceManager gdm, ContentManager cm)
        {
            Isometric.Tile.TileSetTexture = cm.Load<Texture2D>(@"TileSets\temp_ground_tileset_01");
            //texture = content.Load<Texture2D>(Art\\Vehicles\\SampleShip);
            hilight = cm.Load<Texture2D>(@"TileSets\hilight");
            curLevel.Tiles = new Isometric.TileMap();

            if (gdm == null || curLevel.Tiles == null)
            {
                Logging.Instance(Logging.DEFAULTLOG).Log("IsoManager::LoadIsoContent - No graphics device manager or myMap. \n");
                return false;
            }

            Isometric.IsoCamera.ViewWidth = gdm.PreferredBackBufferWidth;
            Isometric.IsoCamera.ViewHeight = gdm.PreferredBackBufferHeight;
            Isometric.IsoCamera.WorldWidth = ((curLevel.Tiles.MapWidth - 2) * Isometric.Tile.TileStepX);
            Isometric.IsoCamera.WorldHeight = ((curLevel.Tiles.MapHeight - 2) * Isometric.Tile.TileStepY);
            Isometric.IsoCamera.DisplayOffset = new Vector2(curLevel.BaseOffsetX, curLevel.BaseOffsetY);

            return true;
        }

        public void UpdateLevel(Levels.IsoLevel newLevel)
        {
            curLevel = newLevel;
        }

        public void HandleInput(GameTime gameTime, MouseState mouseState, KeyboardState keyState, MouseState prevMouseState, KeyboardState prevKeyState)
        {
            if (keyState.IsKeyDown(Keys.Left))
            {
                Isometric.IsoCamera.Move(new Vector2(-2, 0));
            }

            if (keyState.IsKeyDown(Keys.Right))
            {
                Isometric.IsoCamera.Move(new Vector2(2, 0));
            }

            if (keyState.IsKeyDown(Keys.Up))
            {
                Isometric.IsoCamera.Move(new Vector2(0, -2));
            }

            if (keyState.IsKeyDown(Keys.Down))
            {
                Isometric.IsoCamera.Move(new Vector2(0, 2));
            }

            if (Globals.curMode == GameMode.Editor)
            {
                if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
                    isoEditor.SelectedCell = hilightPoint;
                // Set the currently selected cell to the new one
                if (keyState.IsKeyDown(Keys.Enter) && prevKeyState.IsKeyDown(Keys.Enter))
                {
                    curLevel.Tiles.ChangeBaseTile(isoEditor.SelectedCell, isoEditor.GetUIManager().SelectedCellType);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            Vector2 tempOut = Vector2.Zero;
            Vector2 hilightLoc = Isometric.IsoCamera.ScreenToWorld(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
            hilightPoint = curLevel.Tiles.WorldToMapCell(new Vector2((int)hilightLoc.X, (int)hilightLoc.Y), out tempOut);
            if (Globals.curMode == GameMode.Main)
            {

            }
            else if(Globals.curMode == GameMode.Editor)
            {
                //isoEditor.SelectedCell = hilightPoint;
            }
        }

        public void MoveObjects()
        {
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, GameTime gameTime, ScreenInfo screenInfo)
        {
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            Vector2 firstSquare = new Vector2(Isometric.IsoCamera.Location.X / Isometric.Tile.TileStepX, Isometric.IsoCamera.Location.Y / Isometric.Tile.TileStepY);
            int firstX = (int)firstSquare.X;
            int firstY = (int)firstSquare.Y;

            Vector2 squareOffset = new Vector2(Isometric.IsoCamera.Location.X % Isometric.Tile.TileStepX, Isometric.IsoCamera.Location.Y % Isometric.Tile.TileStepY);
            int offsetX = (int)squareOffset.X;
            int offsetY = (int)squareOffset.Y;

            float maxdepth = ((curLevel.Tiles.MapWidth + 1) + ((curLevel.Tiles.MapHeight + 1) * Isometric.Tile.TileWidth)) * 10;
            float depthOffset;

            for (int y = 0; y < curLevel.TilesHigh; y++)
            {
                int rowOffset = 0;
                if ((firstY + y) % 2 == 1)
                    rowOffset = Isometric.Tile.OddRowXOffset;

                for (int x = 0; x < curLevel.TilesWide; x++)
                {
                    int mapx = (firstX + x);
                    int mapy = (firstY + y);
                    depthOffset = 0.7f - ((mapx + (mapy * Isometric.Tile.TileWidth)) / maxdepth);

                    if ((mapx >= curLevel.Tiles.MapWidth) || (mapy >= curLevel.Tiles.MapHeight))
                        continue;
                    /*foreach (int tileID in curLevel.Tiles.Rows[mapy].Columns[mapx].BaseTiles)
                    {
                        spriteBatch.Draw(Isometric.Tile.TileSetTexture, Isometric.IsoCamera.WorldToScreen(new Vector2((mapx * Isometric.Tile.TileStepX) + rowOffset,
                            mapy * Isometric.Tile.TileStepY)), Isometric.Tile.GetSourceRectangle(tileID), Color.White,
                            0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    }*/
                    spriteBatch.Draw(Isometric.Tile.TileSetTexture, Isometric.IsoCamera.WorldToScreen(new Vector2((mapx * Isometric.Tile.TileStepX) + rowOffset,
                        mapy * Isometric.Tile.TileStepY)), Isometric.Tile.GetSourceRectangle(curLevel.Tiles.Rows[mapy].Columns[mapx].BaseTile), Color.White,
                        0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    int heightRow = 0;

                    foreach (int tileID in curLevel.Tiles.Rows[mapy].Columns[mapx].HeightTiles)
                    {
                        spriteBatch.Draw(Isometric.Tile.TileSetTexture, Isometric.IsoCamera.WorldToScreen(new Vector2((mapx * Isometric.Tile.TileStepX) + rowOffset,
                                    mapy * Isometric.Tile.TileStepY - (heightRow * Isometric.Tile.HeightTileOffset))), Isometric.Tile.GetSourceRectangle(tileID),
                            Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depthOffset - ((float)heightRow * curLevel.HeightRowDepthMod));
                        heightRow++;
                    }

                    foreach (int tileID in curLevel.Tiles.Rows[mapy].Columns[mapx].Props)
                    {
                        spriteBatch.Draw(Isometric.Tile.TileSetTexture, Isometric.IsoCamera.WorldToScreen(new Vector2((mapx * Isometric.Tile.TileStepX) + rowOffset,
                                    mapy * Isometric.Tile.TileStepY - (heightRow * Isometric.Tile.HeightTileOffset))), Isometric.Tile.GetSourceRectangle(tileID),
                            Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depthOffset - ((float)heightRow * curLevel.HeightRowDepthMod));
                        heightRow++;
                    }

                    /*
                    // Draws the coordinates for each tile
                    spriteBatch.DrawString(Globals.gFonts["Miramonte"], (x + firstX).ToString() + "," + (y + firstY).ToString(),
                        new Vector2((x * Tile.TileStepX) - offsetX + rowOffset + baseOffsetX + 20,
                        (y * Tile.TileStepY) - offsetY + baseOffsetY + 38), Color.White, 0f, Vector2.Zero,
                        1.0f, SpriteEffects.None, 0.0f);
                    */
                }
            }
            
            // Characters
            /*Vector2 vladStandingOn = myMap.WorldToMapCell(vlad.Position, out tempOut);
            int vladHeight = myMap.Rows[(int)vladStandingOn.Y].Columns[(int)vladStandingOn.X].HeightTiles.Count * Tile.HeightTileOffset;
            vlad.Draw(spriteBatch, 0, -vladHeight);
            */


            int hilightrowOffset = 0;
            if ((hilightPoint.Y) % 2 == 1)
                hilightrowOffset = Isometric.Tile.OddRowXOffset;

            spriteBatch.Draw(hilight, Isometric.IsoCamera.WorldToScreen(new Vector2((hilightPoint.X * Isometric.Tile.TileStepX) + hilightrowOffset,
                                    (hilightPoint.Y + 2) * Isometric.Tile.TileStepY)),
                            new Rectangle(0, 0, 64, 32), Color.White * 0.3f, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);

            spriteBatch.End();
        }

        public void LoadTileTypes(string tileTypesPath)
        {
            XDocument xDoc = XDocument.Load(tileTypesPath);
            //XElement curElement, childElement;
            //curElement = xDoc.Root.Element("LightningBugLevel");
            if (xDoc.Root == null)
            {
                Logging.Instance(Logging.DEFAULTLOG).Log("IsoManager::LoadTileTypes: xDoc.Root is null \n");
                return;
            }
            XAttribute tempAtt;
            foreach (XElement file in xDoc.Root.Descendants("File"))
            {
                TileSet newTileSet = new TileSet();
                tempAtt = file.Attribute("FileName");
                if (tempAtt == null)
                {
                    Logging.Instance(Logging.DEFAULTLOG).Log("IsoManager::LoadTileTypes: FileName is null \n");
                    System.Environment.Exit(1); // Exitting in order to never not notice this error
                }
                else
                {
                    newTileSet.fileName = tempAtt.Value;
                }

                // Move on if this tile set has already been added
                if(tileSets.ContainsKey(newTileSet.fileName))
                    continue;

                tempAtt = file.Attribute("ElemWidth");
                if (tempAtt == null)
                {
                    Logging.Instance(Logging.DEFAULTLOG).Log("IsoManager::LoadTileTypes: ElemWidth is null \n");
                    System.Environment.Exit(1); // Exitting in order to never not notice this error                    
                }
                else
                {
                    newTileSet.elemWidth = Int32.Parse(tempAtt.Value);
                }
                tempAtt = file.Attribute("ElemHeight");
                if (tempAtt == null)
                {
                    Logging.Instance(Logging.DEFAULTLOG).Log("IsoManager::LoadTileTypes: ElemHeight is null \n");
                    System.Environment.Exit(1); // Exitting in order to never not notice this error                    
                }
                else
                {
                    newTileSet.elemHeight = Int32.Parse(tempAtt.Value);
                }
                tempAtt = file.Attribute("ItemsPerRow");
                if (tempAtt == null)
                {
                    Logging.Instance(Logging.DEFAULTLOG).Log("IsoManager::LoadTileTypes: ItemsPerRow is null \n");
                    System.Environment.Exit(1); // Exitting in order to never not notice this error                    
                }
                else
                {
                    newTileSet.itemsPerRow = Int32.Parse(tempAtt.Value);
                }
                newTileSet.numRows = file.Descendants("Row").Count();
                newTileSet.rows = new int[newTileSet.numRows];
                int count = 0;
                foreach (XElement row in file.Descendants("Row"))
                {
                    newTileSet.rows[count] = Int32.Parse(row.Value);
                    ++count;
                }

                tileSets.Add(newTileSet.fileName, newTileSet);
            } // foreach(XElement file in curElement.Nodes())            
        }

        public void InitEditor(UIManager ui)
        {
            isoEditor = new Levels.IsoEditor(ui);
            // Create the listbox of tilemaps
            LoadTileTypes(@"..\..\..\Content\Tilesets\TileSets.xml");
            LoadTileSetIds();
        }

        // Used by the editor only.  Calculates the tile id for each tile in a tileset.
        public void LoadTileSetIds()
        {
            TileSet tempSet;
            foreach (KeyValuePair<string, TileSet> setList in tileSets)
            {
                tempSet = setList.Value;
                int curId = 0;
                tempSet.tileIds = new int[tempSet.rows.Length][];
                for(int i = 0; i < tempSet.rows.Length; ++i)
                {
                    curId = i * tempSet.itemsPerRow;
                    tempSet.tileIds[i] = new int[tempSet.rows[i]];
                    for(int j = 0; j < tempSet.rows[i]; ++j)
                    {
                        tempSet.tileIds[i][j] = curId;
                        ++curId;
                    }
                }
                /* TODO Remove if not needed - 9/28
                XDocument xDoc = XDocument.Load(tempSet.fileName);
                if (xDoc == null)
                {
                    Logging.Instance(Logging.DEFAULTLOG).Log("IsoEditor::LoadTileSetIds - Invalid curTileSetFilePath.\n");
                    continue;
                }
                TileSet curTileSet;
                // If the tileset doesn't exist create and set it.
                if (!isoManager.tileSets.ContainsKey(curTileSetFilePath))
                {
                    curTileSet = new TileSet();
                    isoManager.tileSets.Add(curTileSetFilePath, curTileSet);
                }
                else
                    curTileSet = isoManager.tileSets[curTileSetFilePath];

                curTileSet.tileIds = new int[rows.Length][];
                //curTileSetIds = new int[rows.Length][];
                for (int i = 0; i < rows.Length; ++i)
                {
                    //curTileSetIds[i] = new int[rows[i]];
                    curTileSet.tileIds[i] = new int[rows[i]];
                }

                XElement curElement;
                curElement = xDoc.Root.Element("Tiles");
                int row, column, id;
                foreach (XElement curTile in curElement.Descendants("Tile"))
                {
                    if (!Int32.TryParse(curTile.Element("Row").Value, out row) ||
                        !Int32.TryParse(curTile.Element("Column").Value, out column) ||
                        !Int32.TryParse(curTile.Element("Id").Value, out id))
                    {
                        Logging.Instance(Logging.DEFAULTLOG).Log("IsoEditor::LoadTileSetIds - Parsing error.\n");
                        return;
                    }
                    //curTileSetIds[row][column] = id;
                    curTileSet.tileIds[row][column] = id;
                }*/
            } // foreach (TileSet tileSet in tileSets)
        }

        public void DrawEditor(SpriteBatch spriteBatch)
        {
            if (Globals.curMode != GameMode.Editor)
                return;
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            //Draw selected tile in the editor
            if (isoEditor.GetUIManager() != null && isoEditor.GetUIManager().SelectedCellType >= 0)
            {
                //new Vector2(100, 100), new Vector2(-10, 10) : size and position from top right of the selected background
                spriteBatch.Draw(Isometric.Tile.TileSetTexture, new Vector2(Isometric.IsoCamera.ViewWidth - 92, 28), Isometric.Tile.GetSourceRectangle(isoEditor.GetUIManager().SelectedCellType), Color.White,
                    0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            }
            spriteBatch.End();
        }

        public void DestroyEditor()
        {
            isoEditor = null;
        }
    }
}
