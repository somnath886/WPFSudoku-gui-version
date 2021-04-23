using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WPFSudoku
{
    class CanvasRender
    {
        private Canvas canvas;
        private Canvas tcanvas;
        private List<List<Rectangle>> RecList = new List<List<Rectangle>>();
        private List<List<TextBlock>> TextBlockList = new List<List<TextBlock>>();
        private TextBlock Text;
        private int TempValid;
        private List<List<int>> Backup = new List<List<int>>();
        private int track = 0;
        private List<List<int>> BackupBoardList = new List<List<int>>();
        private List<List<int>> FinalBoardList = new List<List<int>>();
        private List<Tuple<byte, byte, byte>> Colours = new List<Tuple<byte, byte, byte>>
        {
            new Tuple<byte, byte, byte>(255,0,0),
            new Tuple<byte, byte, byte>(255,255,0),
            new Tuple<byte, byte, byte>(0,234,255),
            new Tuple<byte, byte, byte>(170,0,255),
            new Tuple<byte, byte, byte>(255,127,0),
            new Tuple<byte, byte, byte>(0,149,255),
            new Tuple<byte, byte, byte>(255,0,170),
            new Tuple<byte, byte, byte>(106,255,0),
            new Tuple<byte, byte, byte>(0,0,255),
        };
        private int TempCount;

        public CanvasRender(Canvas canvas, Canvas tcanvas)
        {
            this.canvas = canvas;
            this.tcanvas = tcanvas;
            ForLoopstoInitialize();
        }

        private void ForLoopstoInitialize()
        {
            for (int i = 0; i < 9; i++)
            {
                RecList.Add(new List<Rectangle>());
                TextBlockList.Add(new List<TextBlock>());

                for (int j = 0; j < 9; j++)
                {
                    RecList[i].Add(new Rectangle());
                    TextBlockList[i].Add(new TextBlock());
                }

            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    CreateRecTextBlockList(i, j);
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
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
            Text = (TextBlock)sender;
            Text.Focusable = true;
            Text.IsEnabled = true;
            Text.Focus();
            Text.Background = ChoosenColor;            
        }

        private void ChangeText(object sender, KeyEventArgs e)
        {
            List<int> KeyRange = new List<int> { 35, 36, 37, 38, 39, 40, 41, 42, 43 };

            if (!KeyRange.Contains((int)e.Key))
            {
                Text.FontSize = 20;
                Text.Text = "Not Valid";
                Text.Background = Brushes.Beige;
            }

            else
            {
                Text.FontSize = canvas.Width / 20;
                string value = e.Key.ToString();
                int index = (int)value[1] - 48;
                Text.Text = index.ToString();
                var RGB = Colours[index - 1];
                var r = RGB.Item1; var g = RGB.Item2; var b = RGB.Item3;
                SolidColorBrush ChoosenColor = new SolidColorBrush();
                ChoosenColor.Color = Color.FromRgb(r, g, b);
                Text.Background = ChoosenColor;
            }
        }

        private void CreateRecTextBlockList(int i, int j)
        {
            RecList[i][j] = new Rectangle
            {
                Fill = Brushes.Beige,
                Stroke = Brushes.Black,
                Width = canvas.Width / 9,
                Height = canvas.Height / 9,
            };

            TextBlockList[i][j] = new TextBlock
            {
                Height = 60,
                Width = 60,
                TextAlignment = TextAlignment.Center,
                Text = $"{0}",
                FontSize = canvas.Width / 20,
            };
            TextBlockList[i][j].MouseDown += SelectCell;
            TextBlockList[i][j].KeyDown += ChangeText;
            Canvas.SetLeft(RecList[i][j], j * 60);
            Canvas.SetLeft(TextBlockList[i][j], j * 60);
            Canvas.SetTop(RecList[i][j], (i * 60));
            Canvas.SetTop(TextBlockList[i][j], (i * 60));
        }

        public async Task SolveSudoku()
        {
            List<List<int>> BoardList = new List<List<int>>();

            for (int i = 0; i < 9; i++)
            {
                BoardList.Add(new List<int>());

                for (int j = 0; j < 9; j++)
                {
                    int add = (int)TextBlockList[i][j].Text[0] - 48;
                    BoardList[i].Add(add);
                }
            }

            //BoardList.Add(new List<int> { 4, 0, 0, 1, 0, 0, 0, 3, 8 });
            //BoardList.Add(new List<int> { 0, 0, 0, 3, 9, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 2, 0, 0, 0, 0, 1 });
            //BoardList.Add(new List<int> { 5, 3, 0, 0, 7, 0, 0, 8, 0 });
            //BoardList.Add(new List<int> { 0, 0, 9, 0, 0, 0, 7, 0, 0 });
            //BoardList.Add(new List<int> { 0, 2, 0, 0, 0, 0, 0, 1, 0 });
            //BoardList.Add(new List<int> { 0, 0, 5, 0, 0, 3, 9, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 4, 0, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 9, 0, 0, 0, 6, 5 });

            //BoardList.Add(new List<int> { 0, 0, 0, 0, 8, 0, 4, 0, 9 });
            //BoardList.Add(new List<int> { 9, 0, 0, 0, 0, 0, 0, 6, 2 });
            //BoardList.Add(new List<int> { 6, 0, 0, 0, 0, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 7, 0, 0, 0, 0, 0, 1, 3 });
            //BoardList.Add(new List<int> { 1, 5, 0, 0, 4, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 3, 0, 0, 9, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 8, 0, 0, 0, 0, 4, 5 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 1, 7, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 2, 0, 0, 3, 0, 0, 0 });

            //BoardList.Add(new List<int> { 0, 3, 0, 6, 0, 5, 0, 0, 0 });
            //BoardList.Add(new List<int> { 6, 0, 0, 0, 9, 0, 0, 0, 2 });
            //BoardList.Add(new List<int> { 0, 7, 0, 1, 0, 0, 0, 0, 6 });
            //BoardList.Add(new List<int> { 0, 9, 0, 0, 0, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 8, 1, 0, 0, 5, 0, 0, 6, 9 }); //Other Source
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 8, 0 });
            //BoardList.Add(new List<int> { 4, 0, 0, 0, 0, 3, 0, 2, 0 });
            //BoardList.Add(new List<int> { 9, 0, 0, 0, 2, 0, 0, 0, 5 });
            //BoardList.Add(new List<int> { 0, 0, 0, 9, 0, 8, 0, 3, 0 });

            //BoardList.Add(new List<int> { 4, 0, 0, 0, 9, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 1, 0, 0, 0, 0, 3, 0, 0 });
            //BoardList.Add(new List<int> { 0, 8, 0, 0, 0, 0, 1, 0, 7 });
            //BoardList.Add(new List<int> { 0, 0, 6, 0, 0, 0, 0, 0, 3 });
            //BoardList.Add(new List<int> { 0, 0, 9, 0, 7, 2, 0, 5, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 4, 0, 2, 6 });
            //BoardList.Add(new List<int> { 0, 0, 2, 0, 0, 7, 0, 8, 0 });
            //BoardList.Add(new List<int> { 5, 0, 0, 0, 0, 6, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 8, 5, 0, 0, 0, 0 });

            //BoardList.Add(new List<int> { 0, 0, 5, 7, 0, 0, 0, 9, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 1, 0, 8, 0 });
            //BoardList.Add(new List<int> { 3, 0, 0, 0, 8, 0, 0, 0, 6 });
            //BoardList.Add(new List<int> { 0, 2, 0, 5, 0, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 3, 0, 0, 2, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 6, 4, 0, 0, 0, 0, 0, 7, 0 });
            //BoardList.Add(new List<int> { 4, 9, 0, 0, 0, 0, 0, 0, 3 });
            //BoardList.Add(new List<int> { 0, 0, 8, 0, 0, 9, 0, 5, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 0, 1, 0, 0 });

            //BoardList.Add(new List<int> { 0, 4, 7, 0, 0, 0, 9, 0, 1 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 8 });
            //BoardList.Add(new List<int> { 0, 6, 0, 0, 0, 0, 0, 0, 5 });
            //BoardList.Add(new List<int> { 0, 5, 0, 7, 3, 4, 0, 2, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 6, 8, 0, 4, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 2, 0, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 4, 3, 0, 0, 0, 1, 0, 6, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 7, 0, 0, 0, 9 });
            //BoardList.Add(new List<int> { 0, 0, 1, 0, 0, 0, 0, 0, 0 });

            //BoardList.Add(new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 });

            //BoardList.Add(new List<int> { 0, 8, 0, 0, 0, 7, 0, 6, 0 });
            //BoardList.Add(new List<int> { 4, 0, 5, 3, 0, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 1, 0, 2, 0, 0, 5, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 4, 0, 0, 0, 0, 1, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 6 });
            //BoardList.Add(new List<int> { 0, 6, 3, 0, 0, 0, 0, 0, 8 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 1, 0, 0, 2, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 6, 0, 0, 5, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 9, 0, 0, 7, 0, 3 });

            //BoardList.Add(new List<int> { 0, 3, 1, 0, 6, 0, 4, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 2, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 3 });
            //BoardList.Add(new List<int> { 0, 0, 0, 4, 0, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 6, 2, 0, 0, 0, 0, 0, 9 });
            //BoardList.Add(new List<int> { 0, 0, 4, 9, 0, 0, 0, 0, 1 });
            //BoardList.Add(new List<int> { 0, 0, 0, 0, 0, 4, 5, 0, 0 });
            //BoardList.Add(new List<int> { 0, 9, 7, 0, 8, 0, 0, 0, 0 });
            //BoardList.Add(new List<int> { 0, 8, 0, 0, 1, 0, 7, 6, 0 });

            List<List<int>> Starts = new List<List<int>>();
            List<List<int>> Ends = new List<List<int>>();
            List<PossibleMembers> ValidMembers = new List<PossibleMembers>();
            List<int> Members = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int StartIndex = 0;
            int EndIndex = 0;
            int Steps = 0;

            for (int i = 0; i < BoardList[0].Count; i++)
            {
                Steps++;
                for (int j = 0; j < BoardList[0].Count; j++)
                {
                    Steps++;
                    if (i % 3 == 0 && j % 3 == 0)
                    {
                        Steps++;
                        Starts.Add(new List<int>());
                        Starts[StartIndex].Add(i);
                        Starts[StartIndex].Add(j);
                        StartIndex++;
                    }

                    else if (i % 3 == 2 && j % 3 == 2)
                    {
                        Steps++;
                        Ends.Add(new List<int>());
                        Ends[EndIndex].Add(i);
                        Ends[EndIndex].Add(j);
                        EndIndex++;
                    }
                }
            }

            for (int i = 0; i < BoardList.Count; i++)
            {
                BackupBoardList.Add(new List<int>());
                for (int j = 0; j < BoardList[i].Count; j++)
                {
                    int add = BoardList[i][j];
                    BackupBoardList[i].Add(add);
                }
            }

            for (int i = 0; i < BoardList.Count; i++)
            {
                FinalBoardList.Add(new List<int>());
                for (int j = 0; j < BoardList[i].Count; j++)
                {
                    int add = BoardList[i][j];
                    FinalBoardList[i].Add(add);
                }
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            await Solver(BoardList, ValidMembers, Members, Starts, Ends, Steps);
            sw.Stop();
            TextBlock New = new TextBlock
            {
                FontSize = 50,
                Text = $"Time taken: {sw.ElapsedMilliseconds}ms"
            };
            tcanvas.Children.Add(New);
            Trace.WriteLine($"Time taken: {sw.ElapsedMilliseconds}ms");

            BoardList = FinalBoardList;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (BoardList[i][j] > 0 && TextBlockList[i][j].Background == null)

                    {
                        var RGB = Colours[BoardList[i][j] - 1];
                        var r = RGB.Item1; var g = RGB.Item2; var b = RGB.Item3;
                        SolidColorBrush ChoosenColor = new SolidColorBrush();
                        ChoosenColor.Color = Color.FromRgb(r, g, b);
                        //Refresh(TextBlockList[i][j]);
                        TextBlockList[i][j].Text = $"{BoardList[i][j]}";
                        TextBlockList[i][j].Background = ChoosenColor;
                        //await Task.Delay(100);
                    }
                }
            }

            FinalBoardList.Clear();
            Text = null;
        }

        private async Task Solver(List<List<int>> BoardList, List<PossibleMembers> ValidMembers, List<int> Members, List<List<int>> Starts, List<List<int>> Ends, int Steps)
        {
            //for (int i = 0; i < 9; i++)
            //{
            //    for (int j = 0; j < 9; j++)
            //    {
            //        if (BoardList[i][j] > 0 && TextBlockList[i][j].Background == null)

            //        {
            //            var RGB = Colours[BoardList[i][j] - 1];
            //            var r = RGB.Item1; var g = RGB.Item2; var b = RGB.Item3;
            //            SolidColorBrush ChoosenColor = new SolidColorBrush();
            //            ChoosenColor.Color = Color.FromRgb(r, g, b);
            //            Refresh(TextBlockList[i][j]);
            //            TextBlockList[i][j].Text = $"{BoardList[i][j]}";
            //            TextBlockList[i][j].Background = ChoosenColor;
            //            await Task.Delay(100);
            //        }
            //    }
            //}
            ValidMembers.Clear();

            for (int i = 0; i < BoardList[0].Count; i++)
            {
                for (int j = 0; j < BoardList[0].Count; j++)
                {
                    if (BoardList[i][j] == 0)
                    {
                        ValidMembers.Add(new PossibleMembers
                        {
                            x = i,
                            y = j,
                            members = new List<int>()
                        });
                    }
                }
            }

            if (TempCount == 100)
            {
                Console.WriteLine("Not Valid");
            }

            else
            {
                TempCount = ValidMembers.Count;
                foreach (var Item in ValidMembers)
                {
                    foreach (var Member in Members)
                    {
                        if (!BoardList[Item.x].Contains(Member))
                        {
                            Steps++;
                            Item.members.Add(Member);
                        }
                    }
                }

                foreach (var Item in ValidMembers)
                {
                    foreach (var Member in Members)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (BoardList[i][Item.y] == Member)
                            {
                                Item.members.Remove(Member);
                            }
                        }
                    }
                }

                for (int i = 0; i < Starts.Count; i++)
                {

                    List<PossibleMembers> TempList = new List<PossibleMembers>();

                    for (int j = Starts[i][0]; j < Ends[i][0] + 1; j++)
                    {

                        for (int k = Starts[i][1]; k < Ends[i][1] + 1; k++)
                        {
                            if (BoardList[j][k] == 0)
                            {
                                TempList.Add(ValidMembers.Find(Item => Item.x == j && Item.y == k));
                            }
                        }
                    }

                    foreach (var Temp in TempList)
                    {
                        for (int j = Starts[i][0]; j < Ends[i][0] + 1; j++)
                        {
                            for (int k = Starts[i][1]; k < Ends[i][1] + 1; k++)
                            {
                                if (BoardList[j][k] != 0)
                                {
                                    if (Temp.members.Contains(BoardList[j][k]))
                                    {
                                        Temp.members.Remove(BoardList[j][k]);
                                    }
                                }
                            }
                        }
                    }
                }

                List<List<PossibleMembers>> Colbased = new List<List<PossibleMembers>>();
                List<List<PossibleMembers>> Rowbased = new List<List<PossibleMembers>>();
                List<List<PossibleMembers>> GridBased = new List<List<PossibleMembers>>();

                for (int i = 0; i < 9; i++)
                {
                    Colbased.Add(new List<PossibleMembers>());
                    Rowbased.Add(new List<PossibleMembers>());

                    for (int j = 0; j < 9; j++)
                    {
                        foreach (var ValidMember in ValidMembers)
                        {
                            if (ValidMember.x == i && ValidMember.y == j)
                            {
                                Colbased[i].Add(ValidMember);
                            }

                            if (ValidMember.x == j && ValidMember.y == i)
                            {
                                Rowbased[i].Add(ValidMember);
                            }
                        }
                    }
                }

                for (int i = 0; i < Starts.Count; i++)
                {
                    GridBased.Add(new List<PossibleMembers>());

                    for (int j = Starts[i][0]; j < Ends[i][0] + 1; j++)
                    {
                        for (int k = Starts[i][1]; k < Ends[i][1] + 1; k++)
                        {
                            foreach (var ValidMember in ValidMembers)
                            {
                                if (ValidMember.x == j && ValidMember.y == k)
                                {
                                    GridBased[i].Add(ValidMember);
                                }
                            }
                        }
                    }
                }

                List<int> AllMembers = new List<int>();

                foreach (var MemberList in Colbased)
                {
                    foreach (var Member in MemberList.ToArray())
                    {
                        foreach (var member in Member.members)
                        {
                            AllMembers.Add(member);
                        }
                    }

                    foreach (var Member in MemberList.ToArray())
                    {
                        foreach (var member in Member.members.ToArray())
                        {
                            int count = AllMembers.Where(temp => temp.Equals(member))
                                        .Select(temp => temp)
                                        .Count();

                            if (count == 1)
                            {
                                var find = Member.members.FindAll(item => item != member);

                                foreach (var item in find)
                                {
                                    Member.members.Remove(item);
                                }
                            }
                        }
                    }

                    AllMembers.Clear();
                }

                foreach (var MemberList in Rowbased)
                {
                    foreach (var Member in MemberList)
                    {
                        foreach (var member in Member.members)
                        {
                            AllMembers.Add(member);
                        }
                    }

                    foreach (var Member in MemberList.ToArray())
                    {
                        foreach (var member in Member.members.ToArray())
                        {
                            int count = AllMembers.Where(temp => temp.Equals(member))
                                        .Select(temp => temp)
                                        .Count();

                            if (count == 1)
                            {
                                var find = Member.members.FindAll(item => item != member);

                                foreach (var item in find)
                                {
                                    Member.members.Remove(item);
                                }
                            }
                        }
                    }

                    AllMembers.Clear();
                }

                foreach (var MemberList in GridBased)
                {
                    foreach (var Member in MemberList)
                    {
                        foreach (var member in Member.members)
                        {
                            AllMembers.Add(member);
                        }
                    }

                    foreach (var Member in MemberList.ToArray())
                    {
                        foreach (var member in Member.members.ToArray())
                        {
                            int count = AllMembers.Where(temp => temp.Equals(member))
                                        .Select(temp => temp)
                                        .Count();

                            if (count == 1)
                            {
                                var find = Member.members.FindAll(item => item != member);

                                foreach (var item in find)
                                {
                                    Member.members.Remove(item);
                                }
                            }
                        }
                    }

                    AllMembers.Clear();
                }

                foreach (var item in ValidMembers)
                {
                    if (item.members.Count == 1)
                    {
                        BoardList[item.x][item.y] = item.members[0];
                    }
                }

                RemoveMembers(ValidMembers);

                RemoveMemberList(Colbased);
                RemoveMemberList(Rowbased);
                RemoveMemberList(GridBased);

                List<int> Matchingmembers = new List<int>();
                List<PossibleMembers> Matching = new List<PossibleMembers>();

                foreach (var MemberList in Colbased)
                {
                    foreach (var Member in MemberList)
                    {
                        if (Member.members.Count == 2)
                        {
                            foreach (var TwoCountMembers in MemberList)
                            {
                                int index = 0;

                                if (TwoCountMembers.y != Member.y)
                                {
                                    if (!Enumerable.SequenceEqual(Member.members, TwoCountMembers.members))
                                    {
                                        index++;
                                    }

                                    if (index == 0)
                                    {
                                        Matchingmembers.AddRange(Member.members);
                                        Matching.Add(Member);
                                    }
                                }
                            }
                        }
                    }

                    if (Matching.Count == 2)
                    {
                        foreach (var Member in MemberList)
                        {
                            if (!Matching.Contains(Member))
                            {
                                foreach (var member in Member.members.ToArray())
                                {
                                    if (Matchingmembers.Contains(member))
                                    {
                                        Member.members.Remove(member);
                                    }
                                }
                            }
                        }
                    }

                    Matchingmembers.Clear();
                    Matching.Clear();
                }

                foreach (var MemberList in Rowbased)
                {
                    foreach (var Member in MemberList)
                    {
                        if (Member.members.Count == 2)
                        {
                            foreach (var TwoCountMembers in MemberList)
                            {
                                int index = 0;

                                if (TwoCountMembers.x != Member.x)
                                {
                                    if (!Enumerable.SequenceEqual(Member.members, TwoCountMembers.members))
                                    {
                                        index++;
                                    }

                                    if (index == 0)
                                    {
                                        Matchingmembers.AddRange(Member.members);
                                        Matching.Add(Member);
                                    }
                                }
                            }
                        }
                    }

                    if (Matching.Count == 2)
                    {
                        foreach (var Member in MemberList)
                        {
                            if (!Matching.Contains(Member))
                            {
                                foreach (var member in Member.members.ToArray())
                                {
                                    if (Matchingmembers.Contains(member))
                                    {
                                        Member.members.Remove(member);
                                    }
                                }
                            }
                        }
                    }

                    Matchingmembers.Clear();
                    Matching.Clear();
                }

                foreach (var MemberList in GridBased)
                {
                    foreach (var Member in MemberList)
                    {
                        if (Member.members.Count == 2)
                        {
                            foreach (var TwoCountMembers in MemberList)
                            {
                                int index = 0;

                                if (TwoCountMembers.y != Member.y && TwoCountMembers.x != Member.x)
                                {
                                    if (!Enumerable.SequenceEqual(Member.members, TwoCountMembers.members))
                                    {
                                        index++;
                                    }

                                    if (index == 0)
                                    {
                                        Matchingmembers.AddRange(Member.members);
                                        Matching.Add(Member);
                                    }
                                }
                            }
                        }
                    }

                    if (Matching.Count == 2)
                    {
                        foreach (var Member in MemberList)
                        {
                            if (!Matching.Contains(Member))
                            {
                                foreach (var member in Member.members.ToArray())
                                {
                                    if (Matchingmembers.Contains(member))
                                    {
                                        Member.members.Remove(member);
                                    }
                                }
                            }
                        }
                    }

                    Matchingmembers.Clear();
                    Matching.Clear();
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

                var Instances = ValidMembers.FindAll(x => x.members.Count == 2);
                int NextStep = 0;
                int AnotherNextStep = 0;
                foreach (var I in Instances)
                {
                    var FindGrid = GridBased.Find(x => x.Contains(I));
                    var FindTwo = FindGrid.Intersect(Instances).ToList();
                    var FindTwoFilter = FindTwo.FindAll(x => x.members.Intersect(I.members).ToList().Count == 2);
                    if (FindTwoFilter.Count == 2)
                    {
                        var Filter = FindGrid.FindAll(x => x != FindTwoFilter[0] && x != FindTwoFilter[1]);
                        foreach (var F in Filter)
                        {
                            F.members.Remove(I.members[0]);
                            F.members.Remove(I.members[1]);
                        }

                        var FilterWithOne = Filter.FindAll(x => x.members.Count == 1);

                        if (FilterWithOne.Count >= 1)
                        {
                            foreach (var FWO in FilterWithOne)
                            {
                                foreach (var F in Filter)
                                {
                                    if (!FilterWithOne.Contains(F))
                                    {
                                        F.members.Remove(FWO.members[0]);
                                    }
                                }
                            }

                            FilterWithOne.AddRange(Filter.FindAll(x => x.members.Count == 1));

                            foreach (var FWO in FilterWithOne)
                            {
                                BoardList[FWO.x][FWO.y] = FWO.members[0];
                                ValidMembers.Remove(FWO);
                                NextStep++;
                            }
                        }
                    }
                }
                if (NextStep == 0)
                {
                    Instances = ValidMembers.FindAll(x => x.members.Count == 2);
                    foreach (var I in Instances)
                    {
                        var M = I;
                        var FindGrid = GridBased.Find(x => x.Contains(I));
                        var CommonTwoOrThree = FindGrid.FindAll(x => x.members.Intersect(I.members).ToList().Count == 2 && (x.members.Count == 2 || x.members.Count == 3));
                        if (CommonTwoOrThree.Count == 2)
                        {
                            var Three = CommonTwoOrThree.Find(x => x.members.Count == 3);
                            List<int> All = new List<int>();
                            foreach (var a in FindGrid)
                            {
                                All.AddRange(a.members.Intersect(I.members));
                            }
                            int one = 0;
                            int two = 0;
                            foreach (var a in All)
                            {
                                if (a == I.members[0])
                                {
                                    one++;
                                }
                                if (a == I.members[1])
                                {
                                    two++;
                                }
                            }
                            if (one == FindGrid.Count() || two == FindGrid.Count())
                            {
                                continue;
                            }
                            if (Three == null)
                            {
                                continue;
                            }
                            else
                            {
                                var TwoFind = FindGrid.FindAll(x => x.members.Count == 2);
                                if (TwoFind.Count == 2)
                                {
                                    int a = TwoFind[0].members.Intersect(Three.members).ToList().Count;
                                    int b = TwoFind[1].members.Intersect(Three.members).ToList().Count;
                                    if (a == b)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                                Three.members.RemoveAll(x => x != I.members[0] && x != I.members[1]);
                                if (Three.members.Count == 2)
                                {
                                    foreach (var F in FindGrid)
                                    {
                                        if (!CommonTwoOrThree.Contains(F))
                                        {
                                            F.members.RemoveAll(x => x == Three.members[0]);
                                            F.members.RemoveAll(x => x == Three.members[1]);
                                            if (F.members.Count == 1)
                                            {
                                                BoardList[F.x][F.y] = F.members[0];
                                                ValidMembers.Remove(F);
                                                AnotherNextStep++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (ValidMembers.Count > 0)
                {
                    if (TempValid == ValidMembers.Count)
                    {
                        List<PossibleMembers> TempTwo = new List<PossibleMembers>();
                        List<PossibleMembers> Filter = new List<PossibleMembers>();
                        PossibleMembers FindFirst = null;

                        foreach (var v in ValidMembers)
                        {
                            if (v.members.Count == 2)
                            {
                                TempTwo.Add(v);
                            }
                        }
                        if (TempTwo.Count > 0)
                        {
                            foreach (var T in TempTwo)
                            {
                                var Find = GridBased.Find(x => x.Contains(T));
                                if (Find.FindAll(x => x.members.Count == 2).Count == 1)
                                {
                                    FindFirst = T;
                                }
                            }
                            if (FindFirst != null)
                            {
                                BoardList[FindFirst.x][FindFirst.y] = FindFirst.members[0];
                                ValidMembers.Remove(FindFirst);
                                Backup.Add(new List<int> { FindFirst.x, FindFirst.y, FindFirst.members[0], FindFirst.members[1] });
                            }
                            else
                            {
                                var NotZero = GridBased.First(x => x.Count >= 1);
                                int max = NotZero.Count;
                                var Choosen = NotZero;
                                foreach (var Grid in GridBased)
                                {
                                    if (Grid.Count > max && Grid.Count != 0 && Grid.FindAll(x => x.members.Count == 2).Count >= 1)
                                    {
                                        max = Grid.Count;
                                        Choosen = Grid;
                                    }
                                }
                                Filter = Choosen.FindAll(x => x.members.Count == 2);
                                FindFirst = Filter[0];
                                BoardList[FindFirst.x][FindFirst.y] = FindFirst.members[0];
                                ValidMembers.Remove(FindFirst);
                                Backup.Add(new List<int> { FindFirst.x, FindFirst.y, FindFirst.members[0], FindFirst.members[1] });
                            }
                        }
                        else
                        {
                            var NotZero = GridBased.First(x => x.Count >= 1);
                            int min = NotZero.Count;
                            var Choosen = NotZero;
                            foreach (var Grid in GridBased)
                            {
                                if (Grid.Count < min && Grid.Count != 0)
                                {
                                    min = Grid.Count;
                                    Choosen = Grid;
                                }
                            }
                            Filter = Choosen.FindAll(x => x.members.Count >= 1);
                            FindFirst = Filter[0];
                            foreach (var Firstmin in Filter)
                            {
                                if (Firstmin.members.Count < FindFirst.members.Count)
                                    FindFirst = Firstmin;
                            }
                            BoardList[FindFirst.x][FindFirst.y] = FindFirst.members[0];
                            ValidMembers.Remove(FindFirst);
                            Backup.Add(new List<int> { FindFirst.x, FindFirst.y, FindFirst.members[0], FindFirst.members[1] });
                        }
                    }

                    int lol = 0;
                    foreach (var V in ValidMembers)
                    {
                        if (V.members.Count == 0)
                        {
                            lol++;
                        }
                    }

                    if (lol > 0)
                    {
                        if (Backup.Count == 0)
                        {
                            TempValid = ValidMembers.Count;
                            await Solver(BoardList, ValidMembers, Members, Starts, Ends, Steps);
                        }
                        RestoreBackup(BoardList);
                        TempValid = 0;
                        ValidMembers.Clear();
                        var First = Backup[0];
                        BoardList[First[0]][First[1]] = First[3];
                        Backup.Clear();
                        CreateBackup(BoardList);
                        await Solver(BoardList, ValidMembers, Members, Starts, Ends, Steps);
                    }

                    TempValid = ValidMembers.Count;
                    if (TempValid == 0)
                    {
                        FinalBoardList.Clear();
                        for (int i = 0; i < BoardList.Count; i++)
                        {
                            FinalBoardList.Add(new List<int>());
                            for (int j = 0; j < BoardList[i].Count; j++)
                            {
                                int add = BoardList[i][j];
                                FinalBoardList[i].Add(add);
                            }
                        }

                    }
                    await Solver(BoardList, ValidMembers, Members, Starts, Ends, Steps);
                }
                else if (ValidMembers.Count == 0)
                {
                    FinalBoardList.Clear();
                    for (int i = 0; i < BoardList.Count; i++)
                    {
                        FinalBoardList.Add(new List<int>());
                        for (int j = 0; j < BoardList[i].Count; j++)
                        {
                            int add = BoardList[i][j];
                            FinalBoardList[i].Add(add);
                        }
                    }
                }
            }
        }

        private void RestoreBackup(List<List<int>> BoardList)
        {
            for (int i = 0; i < BackupBoardList.Count; i++)
            {
                for (int j = 0; j < BackupBoardList[i].Count; j++)
                {
                    BoardList[i][j] = BackupBoardList[i][j];
                }
            }
        }

        private void CreateBackup(List<List<int>> BoardList)
        {
            BackupBoardList.Clear();
            for (int i = 0; i < BoardList.Count; i++)
            {
                BackupBoardList.Add(new List<int>());
                for (int j = 0; j < BoardList[i].Count; j++)
                {
                    int add = BoardList[i][j];
                    BackupBoardList[i].Add(add);
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

        private static void RemoveMemberList(List<List<PossibleMembers>> ValidMembers)
        {
            foreach (var MemberList in ValidMembers)
            {
                foreach (var Member in MemberList.ToArray())
                {
                    if (Member.members.Count == 1)
                    {
                        MemberList.Remove(Member);
                    }
                }
            }
        }

        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }

    class PossibleMembers
    {
        public int x;
        public int y;
        public List<int> members;
    }
}
