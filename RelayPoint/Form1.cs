using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TextLib;

namespace RelayPoint
{
    public partial class Form1 : Form
    {
        //ファイルリスト
        private List<string> fileLists = new List<string>();
        //フォルダリスト
        private List<string> folderLists = new List<string>();
        private string folderIniFile;

        //設定ファイル
        IniFile inifile = new IniFile();

        public static class AppInfo
        {
            public static string Filepath => System.Reflection.Assembly.GetExecutingAssembly().Location;
            public static string Directory => Path.GetDirectoryName(Filepath);
            public static string DirectoryYen => Path.GetDirectoryName(Filepath) + @"\";
            public static string FileName => Path.GetFileName(Filepath);
            public static string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(Filepath);
            public static string Extension => Path.GetExtension(Filepath).ToLower();
        }

        public Form1()
        {
            InitializeComponent();

            //フォーム
            TopMost = true;
            ShowInTaskbar = false;
            Icon = Properties.Resources.app;
            BackColor = SystemColors.Window;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Text = "RelayPoint";

            //フォルダリストの読み込み
            folderIniFile = AppInfo.DirectoryYen + "folder.ini";
            ReadFoloderIni();

            //設定ファイルの読み込み
            タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem.Checked = inifile.GetKeyValueBool("Setting", "mousemove", true, true);
            フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem.Checked = inifile.GetKeyValueBool("Setting", "fullpath", false, true);
            ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Checked = inifile.GetKeyValueBool("Setting", "tooltip", false, true);
            int bootPosition = inifile.GetKeyValueInt("Setting", "position", 0, 0, 3, true);
            int bootPositionTop = inifile.GetKeyValueInt("Setting", "Top", 0, true);
            int bootPositionLeft = inifile.GetKeyValueInt("Setting", "Left", 0, true);
            int bootPositionFixTop = inifile.GetKeyValueInt("Setting", "FixTop", 0, true);
            int bootPositionFixLeft = inifile.GetKeyValueInt("Setting", "FixLeft", 0, true);

            //起動時位置
            StartPosition = FormStartPosition.Manual;
            if (bootPosition == 0)
            {
                //マウスカーソルの近く
                マウスカーソルの近くToolStripMenuItem.Checked = true;
                Location = GetBootLocation();
            }
            else if (bootPosition == 1)
            {
                //マウスカーソルの位置
                マウスカーソルの位置ToolStripMenuItem.Checked = true;
                Location = new Point(Cursor.Position.X - Width / 2, Cursor.Position.Y - 14);
            }
            else if (bootPosition == 2)
            {
                //前回終了位置
                前回終了位置ToolStripMenuItem.Checked = true;
                Top = bootPositionTop;
                Left = bootPositionLeft;
            }
            else if (bootPosition == 3)
            {
                //固定
                固定ToolStripMenuItem.Checked = true;
                Top = bootPositionFixTop;
                Left = bootPositionFixLeft;
            }

            //マウスで移動させる
            MouseDown += (sender, e) => control_MouseDown(sender, e);
            MouseMove += (sender, e) => control_MouseMove(sender, e);
            MouseUp += (sender, e) => control_MouseUp(sender, e);

            labelRelaypoint.MouseDown += (sender, e) => control_MouseDown(sender, e);
            labelRelaypoint.MouseMove += (sender, e) => control_MouseMove(sender, e);
            labelRelaypoint.MouseUp += (sender, e) => control_MouseUp(sender, e);

            labelFolderOpen.MouseDown += (sender, e) => control_MouseDown(sender, e);
            labelFolderOpen.MouseMove += (sender, e) => control_MouseMove(sender, e);
            labelFolderOpen.MouseUp += (sender, e) => control_MouseUp(sender, e);

            pictureBoxSetting.MouseDown += (sender, e) => control_MouseDown(sender, e);
            pictureBoxSetting.MouseMove += (sender, e) => control_MouseMove(sender, e);
            pictureBoxSetting.MouseUp += (sender, e) => control_MouseUp(sender, e);

            //Ctrl+Cイベント
            KeyPreview = true;
            KeyDown += (sender, e) => form_KeyDown(sender, e);

            //ラベル
            labelRelaypoint.Font = new Font(SystemInformation.MenuFont.FontFamily, 10);
            labelFolderOpen.Font = SystemInformation.MenuFont;
            labelFolderOpen.MouseClick += (sender, e) => open_MouseClick(sender, e);
            foreach (var control in Controls)
            {
                if (control.GetType() == typeof(Label))
                {
                    ((Label)control).AllowDrop = true;
                    ((Label)control).DragEnter += (sender, e) => control_DragEnter(sender, e);
                    ((Label)control).DragDrop += (sender, e) => control_DragDrop(sender, e);
                }
            }

            //設定ボタン
            pictureBoxSetting.BackgroundImage = Properties.Resources.setting.ToBitmap();
            pictureBoxSetting.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBoxSetting.MouseClick += (sender, e) => pictureBoxSetting_MouseClick(sender, e);
        }

        #region event(MouseDown/Move/Up)

        private Point lastMousePosition;
        private bool mouseCapture;
        private bool clickAction = true;

        private void control_MouseDown(object sender, MouseEventArgs e)
        {
            clickAction = true;

            if (sender is Label && ((Label)sender).Name == labelRelaypoint.Name && fileLists.Count >= 1)
            {
                //中継地点でありファイルが存在している場合
                if (IsExists(fileLists))
                {
                    try
                    {
                        DataObject dataObj = new DataObject(DataFormats.FileDrop, fileLists.ToArray());
                        DragDropEffects effect = DragDropEffects.None | DragDropEffects.Link | DragDropEffects.All;
                        ListView dummy = new ListView();
                        dummy.DoDragDrop(dataObj, effect);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }

                if (!IsMouseInWindow)
                {
                    Application.Exit();
                }
            }
            else
            {
                if (e.Button != MouseButtons.Left)
                {
                    return;
                }

                lastMousePosition = MousePosition;
                mouseCapture = true;
            }
        }

        private void control_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseCapture == false)
            {
                return;
            }

            //現在位置取得
            Point mp = MousePosition;

            //差分確認
            int offsetX = mp.X - lastMousePosition.X;
            int offsetY = mp.Y - lastMousePosition.Y;

            if (System.Math.Abs(offsetX) > 5 || System.Math.Abs(offsetY) > 5)
            {
                clickAction = false;
            }

            //差分移動
            if (タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem.Checked)
            {
                Location = new Point(Left + offsetX, Top + offsetY);
            }

            lastMousePosition = mp;
        }

        private void control_MouseUp(object sender, MouseEventArgs e)
        {
            clickAction = true;

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            mouseCapture = false;
        }

        //マウスがウィンドウ内にいるか
        private bool IsMouseInWindow => Bounds.Contains(Cursor.Position);

        #endregion

        #region event(DragEnter/DragDrop)

        //常時D&D許可
        private void control_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        //ドロップ時の処理
        private void control_DragDrop(object sender, DragEventArgs e)
        {
            if (sender is Label)
            {
                if (((Label)sender).Name == labelRelaypoint.Name)
                {
                    //ファイルパスを追加
                    fileLists.AddRange((string[])e.Data.GetData(DataFormats.FileDrop, false));

                    //重複削除
                    fileLists = fileLists.Distinct().ToList();

                    //フォントサイズ変更
                    labelRelaypoint.Font = new Font(SystemInformation.MenuFont.FontFamily, 12);

                    //ファイルカウント
                    int fileCount = 0;
                    int folderCount = 0;

                    foreach (string file in fileLists)
                    {
                        if (File.Exists(file))
                        {
                            fileCount++;
                        }
                        else if (Directory.Exists(file))
                        {
                            folderCount++;
                        }
                    }

                    labelRelaypoint.Text = "";
                    if (fileCount >= 1)
                    {
                        labelRelaypoint.Text += fileCount.ToString() + " file" + (fileCount == 1 ? "" : "s");
                    }

                    if (folderCount >= 1)
                    {
                        if (fileCount >= 1)
                        {
                            labelRelaypoint.Text += System.Environment.NewLine;
                        }

                        labelRelaypoint.Text += folderCount.ToString() + " folder" + (folderCount == 1 ? "" : "s");
                    }

                    //ツールチップ変更
                    if (ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Checked)
                    {
                        toolTip1.SetToolTip(labelRelaypoint, string.Join(System.Environment.NewLine, fileLists.ToArray()));
                    }
                }
                else if (((Label)sender).Name == labelFolderOpen.Name)
                {
                    //ファイルパス確保
                    List<string> dropfiles = new List<string>();
                    dropfiles.AddRange((string[])e.Data.GetData(DataFormats.FileDrop, false));

                    //重複削除
                    foreach (string file in dropfiles)
                    {
                        if (Directory.Exists(file) && !folderLists.Contains(file))
                        {
                            folderLists.Add(file);
                        }
                    }

                    //フォルダリスト更新
                    TextFile.Write(folderIniFile, string.Join(System.Environment.NewLine, folderLists), false, EncodeLib.UTF8);
                }
            }
        }

        #endregion

        #region event(フォルダを開く)

        //フォルダメニューを開く
        private void open_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (clickAction)
            {
                //メニュー作成
                ContextMenuStrip contextMenuStrip = makeContextMenuStrip(true);

                //メニュー表示
                Point p = labelFolderOpen.PointToScreen(new Point(0, labelFolderOpen.Height));
                contextMenuStrip.Show(p);
            }
        }

        //メニュー作成
        private ContextMenuStrip makeContextMenuStrip(bool edit = false)
        {
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

            if (edit)
            {
                ToolStripMenuItem item1 = new ToolStripMenuItem { Text = "ダイアログで選択する" };
                item1.Click += (sender, e) => ToolStripMenuItem_FolderOpen(sender, e);
                contextMenuStrip.Items.Add(item1);
            }

            ToolStripMenuItem item2 = new ToolStripMenuItem { Text = "デスクトップ" };
            item2.Click += (sender, e) => ToolStripMenuItem_FolderOpen(sender, e);
            contextMenuStrip.Items.Add(item2);

            bool isFullpathDisplayed = フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem.Checked;
            if (folderLists.Count > 0)
            {
                ToolStripSeparator sep1 = new ToolStripSeparator();
                contextMenuStrip.Items.Add(sep1);

                int idx = -1;
                foreach (string folder in folderLists)
                {
                    idx++;
                    ToolStripMenuItem item = new ToolStripMenuItem();

                    //メニュー名
                    string text = folder;
                    try
                    {
                        if (!isFullpathDisplayed)
                        {
                            int idx2 = -1;
                            text = Path.GetFileName(folder);
                            foreach (string otherfolder in folderLists)
                            {
                                idx2++;
                                if (idx != idx2)
                                {
                                    if (text == Path.GetFileName(otherfolder))
                                    {
                                        text += "(" + folder + ")";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception)
                    {
                        text = folder;
                    }

                    item.Text = text;
                    item.Name = "folder" + idx.ToString("D");
                    item.Click += (sender, e) => ToolStripMenuItem_FolderOpen(sender, e);
                    item.Enabled = Directory.Exists(folder);
                    contextMenuStrip.Items.Add(item);
                }
            }

            ToolStripSeparator sep2 = new ToolStripSeparator();
            contextMenuStrip.Items.Add(sep2);

            if (edit)
            {
                ToolStripMenuItem item3 = new ToolStripMenuItem { Text = "編集" };
                item3.Click += (sender, e) => ToolStripMenuItem_FolderOpen(sender, e);
                contextMenuStrip.Items.Add(item3);
            }

            ToolStripMenuItem item4 = new ToolStripMenuItem { Text = "キャンセル" };
            item4.Click += (sender, e) => ToolStripMenuItem_FolderOpen(sender, e);
            contextMenuStrip.Items.Add(item4);

            return contextMenuStrip;
        }

        //メニュー選択時
        private void ToolStripMenuItem_FolderOpen(object sender, System.EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            string folder = "";

            if (item.Text == "キャンセル")
            {
                return;
            }
            else if (item.Text == "編集")
            {
                if (!File.Exists(folderIniFile))
                {
                    TextFile.Write(folderIniFile, "", false, EncodeLib.UTF8);
                }

                if (File.Exists(folderIniFile))
                {
                    bool fileAccessOk = true;

                    //更新日時確認
                    System.DateTime dt1 = System.DateTime.Now;
                    System.DateTime dt2 = System.DateTime.Now;
                    try
                    {
                        dt1 = File.GetLastWriteTime(folderIniFile);
                    }
                    catch (System.Exception)
                    {
                        fileAccessOk = false;
                    }

                    System.Diagnostics.Process p = System.Diagnostics.Process.Start(folderIniFile);
                    p.WaitForExit();

                    //編集後の更新日時確認
                    try
                    {
                        dt2 = File.GetLastWriteTime(folderIniFile);
                    }
                    catch (System.Exception)
                    {
                        fileAccessOk = false;
                    }

                    if (fileAccessOk)
                    {
                        System.TimeSpan ts1 = dt1 - dt2;

                        //差分が1.5秒以上の場合
                        if (System.Math.Abs(ts1.TotalSeconds) > 1.5)
                        {
                            try
                            {
                                ReadFoloderIni();
                            }
                            catch (System.Exception)
                            {
                                ;
                            }
                        }
                    }
                }
                return;
            }
            else if (item.Text == "デスクトップ")
            {
                folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
            }
            else if (item.Name.StartsWith("folder"))
            {
                //登録フォルダの場合
                try
                {
                    string str = item.Name.Substring("folder".Length);
                    int idx = int.Parse(str);
                    if (Directory.Exists(folderLists[idx]))
                    {
                        folder = folderLists[idx];
                    }
                }
                catch (System.Exception)
                {
                    ;
                }
            }

            if (Directory.Exists(folder))
            {
                System.Diagnostics.Process.Start(folder);
            }
            else
            {
                //選択ダイアログを開く
                FolderBrowserDialog fbd = new FolderBrowserDialog();

                if (fbd.ShowDialog(this) == DialogResult.OK)
                {
                    System.Diagnostics.Process.Start(fbd.SelectedPath);
                }
            }
        }

        #endregion

        #region Ctrl+Cでクリップボードへコピー、ファイルの存在確認

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.C))
            {
                if (fileLists.Count >= 1)
                {
                    CopyToClipboard(fileLists);
                }
            }
        }

        private void CopyToClipboard(List<string> files)
        {
            if (IsExists(files))
            {
                try
                {
                    System.Collections.Specialized.StringCollection cbfiles = new System.Collections.Specialized.StringCollection();
                    cbfiles.AddRange(files.ToArray());
                    Clipboard.SetFileDropList(cbfiles);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            Application.Exit();
        }

        //ファイルの存在確認
        private bool IsExists(List<string> checkfiles)
        {
            if (checkfiles.Count == 0)
            {
                return false;
            }

            bool check = true;
            foreach (string file in checkfiles)
            {
                if (!File.Exists(file) && !Directory.Exists(file))
                {
                    check = false;
                    break;
                }
            }

            return check;
        }

        #endregion

        #region 起動位置

        private Point GetBootLocation()
        {
            int x, y;
            x = Cursor.Position.X - Width / 2;
            y = Cursor.Position.Y - Height / 2;

            Rectangle workarea = Screen.GetWorkingArea(this);
            Rectangle display = Screen.GetBounds(this);

            //confirm taskbar position
            bool correction = false;
            if (display.Width == workarea.Width)
            {
                if (workarea.Top == 0)
                {
                    //bottom
                    if (y >= workarea.Height - Height)
                    {
                        y = workarea.Height - Height;
                        correction = true;
                    }
                }
                else
                {
                    //up
                    if (y <= display.Height - workarea.Height)
                    {
                        y = display.Height - workarea.Height;
                        correction = true;
                    }
                }
            }
            else
            {
                if (workarea.Left == 0)
                {
                    //right
                    if (x >= workarea.Width - Width)
                    {
                        x = workarea.Width - Width;
                        correction = true;
                    }
                }
                else
                {
                    //left
                    if (x <= display.Width - workarea.Width)
                    {
                        x = display.Width - workarea.Width;
                        correction = true;
                    }
                }
            }

            if (!correction)
            {
                x = Cursor.Position.X + 20;
                y = Cursor.Position.Y + 20;
            }

            if (x <= 0)
            {
                x = 0;
            }

            if (x >= display.Width - Width)
            {
                x = display.Width - Width;
            }

            if (y <= 0)
            {
                y = 0;
            }

            if (y >= display.Height - Height)
            {
                y = display.Height - Height;
            }

            return new Point(x, y);
        }

        #endregion

        #region 設定

        private void ReadFoloderIni()
        {
            folderLists = new List<string>();

            string readall = TextFile.Read(folderIniFile) ?? string.Empty;
            foreach (string str in readall.Replace(System.Environment.NewLine, "\n").Split('\n'))
            {
                string folder = str;
                if (folder.EndsWith(@"\"))
                {
                    folder = folder.Substring(0, folder.Length - 1);
                }

                folder.Trim();

                if (!folderLists.Contains(folder) && folder != "")
                {
                    folderLists.Add(folder);
                }
            }
            TextFile.Write(folderIniFile, string.Join(System.Environment.NewLine, folderLists), false, EncodeLib.UTF8);
        }

        //設定
        private void pictureBoxSetting_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            //メニュー表示
            if (clickAction)
            {
                Point p = pictureBoxSetting.PointToScreen(new Point(0, pictureBoxSetting.Height));
                contextMenuStrip1.Show(p);
            }
        }

        private void タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem.Checked = !タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem.Checked;
            inifile.SetKeyValueBool("Setting", "mousemove", タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem.Checked);
        }

        private void フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem.Checked = !フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem.Checked;
            inifile.SetKeyValueBool("Setting", "fullpath", フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem.Checked);
        }

        private void bootPositionToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            マウスカーソルの位置ToolStripMenuItem.Checked = false;
            マウスカーソルの近くToolStripMenuItem.Checked = false;
            前回終了位置ToolStripMenuItem.Checked = false;
            固定ToolStripMenuItem.Checked = false;

            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.Checked = true;

            int idx = 0;
            if (マウスカーソルの近くToolStripMenuItem.Checked)
            {
                idx = 0;
            }
            if (マウスカーソルの位置ToolStripMenuItem.Checked)
            {
                idx = 1;
            }
            if (前回終了位置ToolStripMenuItem.Checked)
            {
                idx = 2;
            }
            if (固定ToolStripMenuItem.Checked)
            {
                idx = 3;
                inifile.SetKeyValueInt("Setting", "FixTop", Top);
                inifile.SetKeyValueInt("Setting", "FixLeft", Left);
                MessageBox.Show("現在位置を保存しました。");
            }
            inifile.SetKeyValueInt("Setting", "position", idx);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Top >= 0 && Left >= 0)
            {
                inifile.SetKeyValueInt("Setting", "Top", Top);
                inifile.SetKeyValueInt("Setting", "Left", Left);
            }
        }

        private void ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Checked = !ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Checked;
            inifile.SetKeyValueBool("Setting", "tooltip", ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Checked);

            if (ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Checked)
            {
                if (fileLists.Count > 0)
                {
                    toolTip1.SetToolTip(labelRelaypoint, string.Join(System.Environment.NewLine, fileLists.ToArray()));
                }
            }
            else
            {
                toolTip1.SetToolTip(labelRelaypoint, "");
            }
        }
        #endregion
    }
}
