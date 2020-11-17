using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
        private List<List<Rectangle>> RecList = new List<List<Rectangle>>();
        private List<List<TextBlock>> TextBlockList = new List<List<TextBlock>>();
        private TextBlock Text;
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

        public CanvasRender(Canvas canvas)
        {
            this.canvas = canvas;
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
            Text = (System.Windows.Controls.TextBlock)sender;
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
                TextAlignment = System.Windows.TextAlignment.Center,
                Text = $"{0}",
                FontSize = canvas.Width / 20,
            };
            TextBlockList[i][j].MouseDown += SelectCell;
            TextBlockList[i][j].KeyDown += ChangeText;
            Canvas.SetLeft(RecList[i][j], j * 60);
            Canvas.SetLeft(TextBlockList[i][j], j * 60);
            Canvas.SetTop(RecList[i][j], i * 60);
            Canvas.SetTop(TextBlockList[i][j], i * 60);
        }

        public async Task SolveSudoku()
        {
            List<List<int>> BoardList = new List<List<int>>();
            
            for (int i = 0; i < 9; i++)
            {
                BoardList.Add(new List<int>());

                for (int j = 0; j < 9; j ++)
                {
                    int add = (int)TextBlockList[i][j].Text[0] - 48;
                    BoardList[i].Add(add);
                }
            }

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

            Stopwatch sw = new Stopwatch();
            sw.Start();
            await Solver(BoardList, ValidMembers, Members, Starts, Ends, Steps);
            sw.Stop();
            Console.WriteLine($"Time taken: {sw.ElapsedMilliseconds}ms");
            Text = null;

        }

        private async Task Solver(List<List<int>> BoardList, List<PossibleMembers> ValidMembers, List<int> Members, List<List<int>> Starts, List<List<int>> Ends, int Steps)
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
                        for (int i = 0; i < 9; i++)
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
                Steps++;

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
                            Refresh(TextBlockList[i][j]);
                            TextBlockList[i][j].Text = await Task.Run (() => $"{BoardList[i][j]}");
                            TextBlockList[i][j].Background = await Task.Run (() => ChoosenColor);
                            await Task.Delay(100);
                        }
                    }
                }

                if (ValidMembers.Count > 0)
                {
                    Steps++;
                    await Solver(BoardList, ValidMembers, Members, Starts, Ends, Steps);
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