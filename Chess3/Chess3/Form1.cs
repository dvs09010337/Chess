using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess3
{
    public partial class Form1 : Form
    {
        PictureBox tempFigure;
        public Dictionary<ChessType, PictureBox> WhiteFigures = new Dictionary<ChessType, PictureBox>();
        public Dictionary<ChessType, PictureBox> BlackFigures = new Dictionary<ChessType, PictureBox>();
        PictureBox[,] cells = new PictureBox[8, 8];
        int cellsSize = 100;
        public Dictionary<ChessType, Action<PlayerColor, int, int>> ChessHandlers;

        Image black_bishop = Bitmap.FromFile("Images\\black_bishop.png");
        Image black_king = Bitmap.FromFile("Images\\black_king.png");
        Image black_knight = Bitmap.FromFile("Images\\black_knight.png");
        Image black_pawn = Bitmap.FromFile("Images\\black_pawn.png");
        Image black_queen = Bitmap.FromFile("Images\\black_queen.png");
        Image black_rook = Bitmap.FromFile("Images\\black_rook.png");

        Image white_bishop = Bitmap.FromFile("Images\\white_bishop.png");
        Image white_king = Bitmap.FromFile("Images\\white_king.png");
        Image white_knight = Bitmap.FromFile("Images\\white_knight.png");
        Image white_pawn = Bitmap.FromFile("Images\\white_pawn.png");
        Image white_queen = Bitmap.FromFile("Images\\white_queen.png");
        Image white_rook = Bitmap.FromFile("Images\\white_rook.png");

        public Form1()
        {
            InitializeComponent();

            ChessHandlers = new Dictionary<ChessType, Action<PlayerColor, int, int>>()
            {
                [ChessType.Bishop] = BishopHandle,
                [ChessType.King] = KingHandle,
                [ChessType.Knight] = KnightHandle,
                [ChessType.Queen] = QueenHandle,
                [ChessType.Rook] = RookHandle,
                [ChessType.Pawn] = PawnHandle,
            };

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    var pb = new PictureBox();
                    pb.Click += CellClickHandler;
                    pb.Size = new Size(cellsSize, cellsSize);
                    pb.Location = new Point(i * cellsSize, j * cellsSize);
                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0)

                            pb.BackColor = Color.White;
                        else
                            pb.BackColor = Color.Gray;
                    }
                    else
                    {
                        if (j % 2 != 0)
                        {
                            pb.BackColor = Color.White;
                        }
                        else
                        {
                            pb.BackColor = Color.Gray;
                        }
                    }
                    cells[i, j] = pb;
                    pb.Tag = $"{i};{j}";
                    Controls.Add(pb);
                }
            }

            SetFigure(white_rook, 7, 0, "white;rook");
            SetFigure(white_rook, 0, 0, "white;rook");
            SetFigure(black_rook, 0, 7, "black;rook");
            SetFigure(black_rook, 7, 7, "black;rook");
            SetFigure(black_bishop, 2, 7, "black;bishop"); ;
            SetFigure(black_bishop, 5, 7, "black;bishop");
            SetFigure(white_bishop, 2, 0, "white;bishop");
            SetFigure(white_bishop, 5, 0, "white;bishop");
            SetFigure(white_king, 3, 0, "white;king");
            SetFigure(black_king, 3, 7, "black;king");
            SetFigure(white_queen, 4, 0, "white;queen");
            SetFigure(black_queen, 4, 7, "black;queen");
            SetFigure(white_knight, 1, 0, "white;knight");
            SetFigure(white_knight, 6, 0, "white;knight");
            SetFigure(black_knight, 1, 7, "black;knight");
            SetFigure(black_knight, 6, 7, "black;knight");
            for (int i = 0; i < 8; i++)
            {
                SetFigure(white_pawn, i, 1, "white;pawn");
            }
            for (int i = 7; i >= 0; i--)
            {
                SetFigure(black_pawn, i, 6, "black;pawn");
            }
        }

        private void SetFigure(Image image, int i, int j, string tag)
        {
            var figure = new PictureBox(); //
            figure.Image = image; //
            figure.Size = new Size(cellsSize, cellsSize); //
            figure.BackColor = Color.Transparent; //
            figure.SizeMode = PictureBoxSizeMode.StretchImage; //
            figure.Location = new Point(0, 0); //
            figure.Click += ClickHandler; //
            figure.Tag = tag; // 
            cells[i, j].Controls.Add(figure); // Вставляет картинку в картинку
        }
        private void CellClickHandler(object sender, EventArgs e)
        {
            var cell = (PictureBox)sender;
            var index = cell.Tag.ToString().Split(';');
            int i = int.Parse(index[0]);
            int j = int.Parse(index[1]);

            if (cell.BackColor != Color.Red)
            {
                return;
            }

            tempFigure.Parent.Controls.Clear();
            cell.Controls.Add(tempFigure);
            tempFigure = null;
            ClearColors();
        }
        private void ClickHandler(object sender, EventArgs e)
        {
            ClearColors();

            var figure = (PictureBox)sender;
            tempFigure = figure;
            var cell = (PictureBox)figure.Parent;
            var index = cell.Tag.ToString().Split(';');
            int i = int.Parse(index[0]);
            int j = int.Parse(index[1]);
            var index2 = figure.Tag.ToString().Split(';');
            ChessType chessType = (ChessType)Enum.Parse(typeof(ChessType), index2[1], true);
            PlayerColor playerColor = (PlayerColor)Enum.Parse(typeof(PlayerColor), index2[0], true);
            ChessHandlers[chessType](playerColor, i, j);
        }
        private void RookHandle(PlayerColor playerColor, int i, int j)
        {
            for(int k = j; j < 7; j++)
            {
                cells[i, k + 1].BackColor = Color.Red;
            }
        }
        private void KingHandle(PlayerColor playerColor, int i, int j)
        {
            
            if (j != 7)
            {
                var cellType = CheckStepAvaliable(playerColor, i, j + 1);
                if(cellType == CellType.Empty || cellType == CellType.EnemyFigure)
                    cells[i, j + 1].BackColor = Color.Red;
            }

            if (i != 7)
            {
                var cellType = CheckStepAvaliable(playerColor, i + 1, j);
                if (cellType == CellType.Empty || cellType == CellType.EnemyFigure)
                    cells[i + 1, j].BackColor = Color.Red;
            }

            if (i != 0)
            {
                var cellType = CheckStepAvaliable(playerColor, i - 1, j);
                if ((cellType == CellType.Empty || cellType == CellType.EnemyFigure))
                {
                    cells[i - 1, j].BackColor = Color.Red;
                }
            }

            if (j != 0)
            {
                var cellType = CheckStepAvaliable(playerColor, i, j - 1);
                if ((cellType == CellType.Empty || cellType == CellType.EnemyFigure))
                {
                    cells[i, j - 1].BackColor = Color.Red;
                }
            }

            if (i != 7 && j != 7)
            {
                var cellType = CheckStepAvaliable(playerColor, i + 1, j + 1);
                if ((cellType == CellType.Empty || cellType == CellType.EnemyFigure))
                {
                    cells[i + 1, j + 1].BackColor = Color.Red;
                }
            }

            if (i != 0 && j != 0)
            {
                var cellType = CheckStepAvaliable(playerColor, i - 1, j - 1);
                if (cellType == CellType.Empty || cellType == CellType.EnemyFigure)
                {
                    cells[i - 1, j - 1].BackColor = Color.Red;
                }
            }

            if (i != 7 && j != 0)
            {
                var cellType = CheckStepAvaliable(playerColor, i + 1, j - 1);
                if (cellType == CellType.Empty || cellType == CellType.EnemyFigure)
                {
                    cells[i + 1, j - 1].BackColor = Color.Red;
                }
            }

            if (i != 0 && j != 7)
            {
                var cellType = CheckStepAvaliable(playerColor, i - 1, j + 1);
                if (cellType == CellType.Empty || cellType == CellType.EnemyFigure)
                {
                    cells[i - 1, j + 1].BackColor = Color.Red;
                }
            }
        }

        private void KnightHandle(PlayerColor playerColor, int i, int j)
        {

        }
        private void QueenHandle(PlayerColor playerColor, int i, int j)
        {

        }
        private void PawnHandle(PlayerColor playerColor, int i, int j)
        {
            int multipluer = 1;
            if ((playerColor == PlayerColor.White && j == 1) || (playerColor == PlayerColor.Black && j == 6))
                multipluer *= 2;

            if (playerColor == PlayerColor.White)
            {
                var cellType = CheckStepAvaliable(playerColor, i, j + 1);
                if (cellType == CellType.Empty)
                {
                    for (int k = j; k < j + multipluer; k++)
                        cells[i, k + 1].BackColor = Color.Red;
                }

                cellType = CheckStepAvaliable(playerColor, i - 1, j + 1);
                if (cellType == CellType.EnemyFigure)
                    cells[i - 1, j + 1].BackColor = Color.Red;

                cellType = CheckStepAvaliable(playerColor, i + 1, j + 1);
                if (cellType == CellType.EnemyFigure)
                    cells[i + 1, j + 1].BackColor = Color.Red;
            }
            else
            {
                var cellType = CheckStepAvaliable(playerColor, i, j - 1);
                if (cellType == CellType.Empty && cellType != CellType.EnemyFigure)
                {
                    for (int k = j; k > j - multipluer; k--)
                        cells[i, k - 1].BackColor = Color.Red;
                }

                cellType = CheckStepAvaliable(playerColor, i + 1, j - 1);
                if (cellType == CellType.EnemyFigure)
                    cells[i + 1, j - 1].BackColor = Color.Red;

                cellType = CheckStepAvaliable(playerColor, i - 1, j - 1);
                if (cellType == CellType.EnemyFigure)
                    cells[i - 1, j - 1].BackColor = Color.Red;
            }
        }
        private void BishopHandle(PlayerColor playerColor, int i, int j)
        {

        }

        private void ClearColors()
        {
            for (int i1 = 0; i1 < cells.GetLength(0); i1++)
            {
                for (int j1 = 0; j1 < cells.GetLength(1); j1++)
                {
                    if (i1 % 2 == 0)
                    {
                        if (j1 % 2 == 0)
                            cells[i1, j1].BackColor = Color.White;
                        else
                            cells[i1, j1].BackColor = Color.Gray;
                    }
                    else
                    {
                        if (j1 % 2 != 0)
                        {
                            cells[i1, j1].BackColor = Color.White;
                        }
                        else
                        {
                            cells[i1, j1].BackColor = Color.Gray;
                        }
                    }
                }
            }
        }

        private CellType CheckStepAvaliable(PlayerColor playerColor, int i, int j)
        {
            PictureBox figure;
            if (cells[i, j].Controls.Count == 0)
                return CellType.Empty;

            figure = (PictureBox)cells[i, j].Controls[0];
            var index2 = figure.Tag.ToString().Split(';');
            ChessType chessType = (ChessType)Enum.Parse(typeof(ChessType), index2[1], true);
            PlayerColor figurePlayerColor = (PlayerColor)Enum.Parse(typeof(PlayerColor), index2[0], true);

            if (figurePlayerColor == playerColor)
                return CellType.AllieFigure;

            return CellType.EnemyFigure;
        }
    }
}
