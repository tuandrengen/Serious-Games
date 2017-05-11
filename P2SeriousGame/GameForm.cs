﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2SeriousGame
{
    public partial class GameForm : Form
    {
        Panel gamePanel = new Panel();
        Database SQL = new Database();
        MapTest FirstLevel;
        IPathfinding path = new Pathfinding();

        public GameForm()
        {
            InitializeComponent();
            InitializePanels();
            FirstLevel = new MapTest(this, 11, 11, path);
            SQL.StartStopwatch();
        }

        #region formatting
        private int ButtonWidth;
        private int ButtonHeight;
        private int ButtonHeightOffset => (3 * (ButtonHeight / 4));

        int ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
        int ScreenHeight = Screen.PrimaryScreen.Bounds.Height;

        //These constants declare the amount of reserved space or margins, where 0.05 equals 5%
        private const double _leftWidthReserved = 0.05;
        private const double _endWidthReserved = 0.12;
        private const double _topHeightReserved = 0.05;
        private const double _bottomHeightReserved = 0.03;

        //The gamescreen variables sets the height and width of the area on the screen where hexagonbutton can be drawn
        private double _gameScreenWidth = Screen.PrimaryScreen.Bounds.Width * (1 - (_leftWidthReserved + _endWidthReserved));
        private double _gameScreenHeight = Screen.PrimaryScreen.Bounds.Height * (1 - (_topHeightReserved + _bottomHeightReserved));

        //Centers the hexagonmap starting placement, if the hexagonmap doesnt fill out the entire gamescreen width
        private double WidthCentering => (_gameScreenWidth - (ButtonWidth * MapTest.TotalHexagonColumns)) / 2;

        //WidthStart and heightStart sets the starting place for the hexagonmap
        private int WidthStart => (int)((_leftWidthReserved * Screen.PrimaryScreen.Bounds.Width) + WidthCentering);

		

		private int _heightStart = (int)(_topHeightReserved * Screen.PrimaryScreen.Bounds.Height);
        #endregion

        private void InitializePanels()
        {
            this.Controls.Add(gamePanel);
            gamePanel.Width = ScreenWidth;
            gamePanel.Height = ScreenHeight;
            gamePanel.Visible = true;
            AddExitButton(gamePanel);
            AddResetButton(gamePanel);
        }

        /// <summary>
        /// 
        /// </summary>
        public void CalculateButtonDimension()
        {
            CalculateButtonDimensionBasedOnScreenHeight();

            //Does the calculated width fit the screen width, if not then calculate height and width based on screen width
            if ((ButtonWidth * MapTest.TotalHexagonColumns) > _gameScreenWidth)
                CalculateButtonDimensionBasedOnScreenWidth();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculateButtonDimensionBasedOnScreenHeight()
        {
            double rowHeight;
            double hexagonRows = MapTest.TotalHexagonRows;
            const double evenRowsToHeight = 0.75;

            //The height to width ratio for a pointy top regulare hexagon
            double heightToWidth = System.Math.Sqrt(3) / 2;

            //These series of if-else calculates the height of one button, determined by the number of rows and the screen height
            if (hexagonRows == 1)
                ButtonHeight = (int)(_gameScreenHeight / hexagonRows);
            else if (hexagonRows % 2 == 0)
            {
                rowHeight = (hexagonRows * evenRowsToHeight) + 0.25;
                ButtonHeight = (int)(_gameScreenHeight / rowHeight);
            }
            else if (hexagonRows % 2 == 1 && hexagonRows > 1)
            {
                rowHeight = ((hexagonRows - 1) / 4) + ((hexagonRows + 1) / 2);
                ButtonHeight = (int)(_gameScreenHeight / rowHeight);
            }

            //We calculate the width by multiplying height to width ratio
            ButtonWidth = (int)((ButtonHeight * heightToWidth));
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculateButtonDimensionBasedOnScreenWidth()
        {
            //The width to height ratio for a pointy top regulare hexagon
            double widthToHeight = System.Math.Sqrt(3) * ((double)2 / 3);

            double buttonWidthTemp;

            //We calculate the button width by dividing the screen width with number of columns + 0.5 (because we have an offset)
            buttonWidthTemp = (int)(_gameScreenWidth / (MapTest.TotalHexagonColumns + 0.5));

            //We calculate the height by multiplying width to height ratio
            ButtonHeight = (int)(buttonWidthTemp * widthToHeight);

            //Now we do not need the buttonWidthTemp with precision, so we typecast the double to an int
            ButtonWidth = (int)buttonWidthTemp;
        }

        public void DrawWindow(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Initialises and draws a hexagon button, 
        /// and adds a click event calculates a new route when an HexButton is clicked.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="map"></param>
        public void DrawButton(HexagonButton button, MapTest map)
        {
            button.Size = new Size((int)(ConvertPointToPixel(ButtonHeight)), (int)(ConvertPointToPixel(ButtonWidth)));
            button.TabStop = false;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.LightGray;
            button.Paint += ButtonPainter;
            button.MouseClick += button.HexClicked;
            button.MouseClick += HexClickedColor;
            button.MouseClick += map.MousePositioner;
            gamePanel.Controls.Add(button);
        }

		public static float hexClickedRound;


        public void HexClickedColor(object sender, MouseEventArgs e)
        {
            HexagonButton sender_Button = sender as HexagonButton;
            sender_Button.BackColor = Color.FromArgb(255, 105, 180);
            hexClickedRound += 1;
        }

        /// <summary>
        /// Places HexagonButtons in GameWindow based on the coordinates assigned in the button.
        /// </summary>
        /// <param name="button"></param>
        public void PlaceHexagonButton(HexagonButton button)
        {
            //For at farve midten før man har klikket på skærmen.
            if (button.XCoordinate == MapTest.TotalHexagonColumns / 2 && button.YCoordinate == MapTest.TotalHexagonRows / 2)
            {
                button.BackColor = System.Drawing.Color.Aqua;
                button.Enabled = false;
            }

            button.Left = CalculateButtonWidthOffset(button.XCoordinate, button.YCoordinate);
            button.Top = CalculateButtonHeightOffset(button.YCoordinate);
        }

        /// <summary>
        /// Calculates the points in a hexagon and makes it a button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ButtonPainter(object sender, PaintEventArgs e)
        {
            System.Drawing.Drawing2D.GraphicsPath buttonPath =
            new System.Drawing.Drawing2D.GraphicsPath();
            Button hexagonButton = sender as Button;

            System.Drawing.Rectangle newRectangle = hexagonButton.ClientRectangle;
            e.Graphics.DrawPolygon(Pens.Black, Math.GetPoints(ButtonHeight, ButtonWidth));

            // Create a hexagon within the new rectangle.
            buttonPath.AddPolygon(Math.GetPoints(ButtonHeight, ButtonWidth));
            // Hexagon region.
            hexagonButton.Region = new System.Drawing.Region(buttonPath);
        }

        private void AddResetButton(Panel panel)
        {
            Button ResetButton = new Button();
            ResetButton.Size = new Size(100, 25);
            ResetButton.TabStop = false;
            ResetButton.FlatStyle = FlatStyle.Flat;
            ResetButton.FlatAppearance.BorderSize = 0;
            ResetButton.BackColor = Color.Red;
            ResetButton.Location = new Point(this.Bounds.Right - ResetButton.Width - 20, this.Bounds.Top + 60);
            ResetButton.MouseClick += SQL.ResetGameToList;
            ResetButton.MouseClick += ResetButtonClick;
            ResetButton.Text = "Reset Game";
            ResetButton.TextAlign = ContentAlignment.MiddleCenter;
            panel.Controls.Add(ResetButton);
        }

        private void AddExitButton(Panel panel)
        {
            Button ExitButton = new Button();
            ExitButton.Size = new Size(100, 25);
            ExitButton.TabStop = false;
            ExitButton.FlatStyle = FlatStyle.Flat;
            ExitButton.FlatAppearance.BorderSize = 0;
            ExitButton.BackColor = Color.LightGray;
            ExitButton.Location = new Point(this.Bounds.Right - ExitButton.Width - 20, this.Bounds.Top + 20);
            //ExitButton.MouseClick += ExitButtonClick;
            ExitButton.MouseClick += ReturnToMainMenu;
            ExitButton.Text = "Return to menu";
            ExitButton.TextAlign = ContentAlignment.MiddleCenter;
            panel.Controls.Add(ExitButton);
        }

        /// <summary>
        /// Converts a coordinate into a position in a hexgrid.
        /// </summary>
        /// <param name="xCoordinate"></param>
        /// <param name="yCoordinate"></param>
        /// <returns></returns>
        private int CalculateButtonWidthOffset(int xCoordinate, int yCoordinate)
        {
            int width = WidthStart;
            width += (xCoordinate * ButtonWidth);
            //Gives every second button an offset to make the grid
            if (yCoordinate % 2 == 1)
            {
                width += ButtonWidth / 2;
            }
            return width;
        }

        /// <summary>
        /// Converts a coordinate into a position in a hexgrid.
        /// </summary>
        /// <param name="yCoordinate"></param>
        /// <returns></returns>
        private int CalculateButtonHeightOffset(int yCoordinate)
        {
            int height = _heightStart;

            height += (yCoordinate * ButtonHeightOffset);

            return height;
        }

        public void ExitButtonClick(object sender, MouseEventArgs e)
        {
            SQL.ExitGameToDatabase();
            Close();
        }

        private void ResetButtonClick(object sender, MouseEventArgs e)
        {
            foreach (HexagonButton hex in MapTest.hexMap)
            {
                hex.Visited = false;
                hex.Passable = true;
                hex.Enabled = true;
                hex.BackColor = System.Drawing.Color.LightGray;
                PlaceHexagonButton(hex);
            }
            MapTest.ResetMouse();
        }

        //We assume that there is 72 points per inch and 96 pixels per inch
        private double ConvertPointToPixel(double point)
        {
            return point * 96 / 72;
        }

        private void ReturnToMainMenu(object sender, MouseEventArgs e)
        {
            Close();
        }
    }
}

