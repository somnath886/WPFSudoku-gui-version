using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFSudoku
{
    class CanvasRender
    {
        private Canvas canvas;
        private List<List<Rectangle>> RecList = new List<List<Rectangle>>();
        private List<List<TextBlock>> TextBlockList = new List<List<TextBlock>>();
        private TextBlock Text;
        private List<Tuple<byte, byte, byte>> Colours = new List<Tuple<byte, byte, byte>>
        {
            new Tuple<byte, byte, byte>(255, 0, 0),
            new Tuple<byte, byte, byte>(0, 255, 0),
            new Tuple<byte, byte, byte>(0, 0, 255),
            new Tuple<byte, byte, byte>(255, 255, 0),
            new Tuple<byte, byte, byte>(0, 255, 255),
            new Tuple<byte, byte, byte>(255, 0, 255)
        };
        private int TempCount;

        public CanvasRender(Canvas canvas)
        {
            this.canvas = canvas;
            ForLoopstoInitialize();
        }

        private void ForLoopstoInitialize()
        {
            for (int i = 0; i < 6; i++)
            {
                RecList.Add(new List<Rectangle>());
                TextBlockList.Add(new List<TextBlock>());

                for (int j = 0; j < 6; j++)
                {
                    RecList[i].Add(new Rectangle());
                    TextBlockList[i].Add(new TextBlock());
                }

            }

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    CreateRecTextBlockList(i, j);
                }
            }

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    canvas.Children.Add(RecList[i][j]);
                    canvas.Children.Add(TextBlockList[i][j]);
                }
            }
        }

        private void SelectCell(object sender, MouseEventArgs e)
        {
            SolidColorBrush ChoosenColor = new SolidColorBrush();
            ChoosenColor.Color = Colors.Green;
            Text = (System.Windows.Controls.TextBlock)sender;
            Text.Focus();
            Text.Background = ChoosenColor;
        }

        private void ChangeText(object sender, KeyEventArgs e)
        {
            List<int> Range = new List<int> { 1, 2, 3, 4, 5, 6 };
            string value = e.Key.ToString();
            int index = (int)value[1] - 48;
            if (Range.Contains(index))
            {
                Text.Text = index.ToString();
            }

            var RGB = Colours[index - 1];
            var r = RGB.Item1; var g = RGB.Item2; var b = RGB.Item3;
            SolidColorBrush ChoosenColor = new SolidColorBrush();
            ChoosenColor.Color = Color.FromRgb(r, g, b);
            Text.Background = ChoosenColor;
        }

        private void CreateRecTextBlockList(int i, int j)
        {
            RecList[i][j] = new Rectangle
            {
                Fill = Brushes.Beige,
                Stroke = Brushes.Black,
                Width = canvas.Width / 6,
                Height = canvas.Height / 6,
            };

            TextBlockList[i][j] = new TextBlock
            {
                Height = 90,
                Width = 90,
                TextAlignment = System.Windows.TextAlignment.Center,
                Text = $"{0}",
                FontSize = canvas.Width / 20,
                Focusable = true,
                IsEnabled = true,
            };
            TextBlockList[i][j].MouseDown += SelectCell;
            TextBlockList[i][j].KeyDown += ChangeText;
            Canvas.SetLeft(RecList[i][j], j * 90);
            Canvas.SetLeft(TextBlockList[i][j], j * 90);
            Canvas.SetTop(RecList[i][j], i * 90);
            Canvas.SetTop(TextBlockList[i][j], i * 90);
        }

        public void SolveSudoku()
        {
            List<List<int>> BoardList = new List<List<int>>();
            
            for (int i = 0; i < 6; i++)
            {
                BoardList.Add(new List<int>());

                for (int j = 0; j < 6; j ++)
                {
                    int add = (int)TextBlockList[i][j].Text[0] - 48;
                    BoardList[i].Add(add);
                }
            }

            List<List<int>> Starts = new List<List<int>>();
            List<List<int>> Ends = new List<List<int>>();
            List<PossibleMembers> ValidMembers = new List<PossibleMembers>();
            List<int> Members = new List<int> { 1, 2, 3, 4, 5, 6 };
            int StartIndex = 0;
            int EndIndex = 0;
            int Steps = 0;

            for (int i = 0; i < BoardList[0].Count; i++)
            {
                Steps++;
                for (int j = 0; j < BoardList[0].Count; j++)
                {
                    Steps++;
                    if (i % 2 == 0 && j % 3 == 0)
                    {
                        Steps++;
                        Starts.Add(new List<int>());
                        Starts[StartIndex].Add(i);
                        Starts[StartIndex].Add(j);
                        StartIndex++;
                    }

                    else if (i % 2 == 1 && j % 3 == 2)
                    {
                        Steps++;
                        Ends.Add(new List<int>());
                        Ends[EndIndex].Add(i);
                        Ends[EndIndex].Add(j);
                        EndIndex++;
                    }
                }
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Solver(BoardList, ValidMembers, Members, Starts, Ends, Steps);
            sw.Stop();
            Console.WriteLine($"Time taken: {sw.ElapsedMilliseconds}ms");
            Text = null;

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (BoardList[i][j] > 0 && TextBlockList[i][j].Background == null)
                    {
                        var RGB = Colours[BoardList[i][j] - 1];
                        var r = RGB.Item1; var g = RGB.Item2; var b = RGB.Item3;
                        SolidColorBrush ChoosenColor = new SolidColorBrush();
                        ChoosenColor.Color = Color.FromRgb(r, g, b);
                        TextBlockList[i][j].Text = $"{BoardList[i][j]}";
                        TextBlockList[i][j].Background = ChoosenColor;
                    }
                }
            }

        }

        private void Solver(List<List<int>> BoardList, List<PossibleMembers> ValidMembers, List<int> Members, List<List<int>> Starts, List<List<int>> Ends, int Steps)
        {
            Steps++;
            ValidMembers.Clear();

            for (int i = 0; i < BoardList[0].Count; i++)
            {
                Steps++;
                for (int j = 0; j < BoardList[0].Count; j++)
                {
                    Steps++;
                    if (BoardList[i][j] == 0)
                    {
                        Steps++;
                        ValidMembers.Add(new PossibleMembers
                        {
                            x = i,
                            y = j,
                            members = new List<int>()
                        });
                    }
                }
            }

            if (TempCount == ValidMembers.Count)
            {
                Console.WriteLine("Not Valid");
            }

            else
            {
                TempCount = ValidMembers.Count;
                foreach (var Item in ValidMembers)
                {
                    Steps++;
                    foreach (var Member in Members)
                    {
                        Steps++;
                        if (!BoardList[Item.x].Contains(Member))
                        {
                            Steps++;
                            Item.members.Add(Member);
                        }
                    }
                }

                foreach (var Item in ValidMembers)
                {
                    Steps++;
                    foreach (var Member in Members)
                    {
                        Steps++;
                        for (int i = 0; i < 6; i++)
                        {
                            Steps++;
                            if (BoardList[i][Item.y] == Member)
                            {
                                Item.members.Remove(Member);
                            }
                        }
                    }
                }

                for (int i = 0; i < Starts.Count; i++)
                {
                    Steps++;
                    List<PossibleMembers> TempList = new List<PossibleMembers>();

                    for (int j = Starts[i][0]; j < Ends[i][0] + 1; j++)
                    {
                        Steps++;
                        for (int k = Starts[i][1]; k < Ends[i][1] + 1; k++)
                        {
                            Steps++;
                            if (BoardList[j][k] == 0)
                            {
                                Steps++;
                                TempList.Add(ValidMembers.Find(Item => Item.x == j && Item.y == k));
                            }
                        }
                    }

                    foreach (var Temp in TempList)
                    {
                        Steps++;
                        for (int j = Starts[i][0]; j < Ends[i][0] + 1; j++)
                        {
                            Steps++;
                            for (int k = Starts[i][1]; k < Ends[i][1] + 1; k++)
                            {
                                Steps++;
                                if (BoardList[j][k] != 0)
                                {
                                    Steps++;
                                    if (Temp.members.Contains(BoardList[j][k]))
                                    {
                                        Steps++;
                                        Temp.members.Remove(BoardList[j][k]);
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var Item in ValidMembers)
                {
                    Steps++;
                    if (Item.members.Count == 1)
                    {
                        Steps++;
                        BoardList[Item.x][Item.y] = Item.members[0];
                    }
                }

                RemoveMembers(ValidMembers);
                Steps++;

                if (ValidMembers.Count > 0)
                {
                    Steps++;
                    Solver(BoardList, ValidMembers, Members, Starts, Ends, Steps);
                }

                else if (ValidMembers.Count == 0)
                {
                    Console.WriteLine($"No of steps taken: {Steps}");
                }
            }
        }

        private void RemoveMembers(List<PossibleMembers> ValidMembers)
        {
            foreach (PossibleMembers Item in ValidMembers.ToArray())
            {
                if (Item.members.Count == 1)
                {
                    ValidMembers.Remove(Item);
                }
            }
        }
    }

    class PossibleMembers
    {
        public int x;
        public int y;
        public List<int> members;
    }
}