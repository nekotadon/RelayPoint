using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace RelayPoint
{
    public partial class Form1 : Form
    {
        //ファイルリスト
        List<string> files_RelayPoint = new List<string>();

        //フォルダリスト
        List<string> folders = new List<string>();
        string folder_ini;

        //ファイルパス
        string apppath;
        string appfolder;
        string appname;

        //設定ファイル
        TextLib.IniFile pfini = null;

        public Form1()
        {
            InitializeComponent();

            //フォーム
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.Icon = Properties.Resources.app;
            this.BackColor = SystemColors.Window;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = "RelayPoint";

            //フォルダリストの読み込み
            apppath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            appfolder = Path.GetDirectoryName(apppath);
            appname = Path.GetFileNameWithoutExtension(apppath);
            folder_ini = appfolder + @"\folder.ini";
            read_folder_ini();

            //設定ファイルの読み込み
            pfini = new TextLib.IniFile(appfolder + @"\" + appname + ".ini");
            pfini.load();
            this.タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem.Checked = pfini.getvalue("Setting", "mousemove", 1, 0, 1) == 1;
            this.フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem.Checked = pfini.getvalue("Setting", "fullpath", 0, 0, 1) == 1;
            this.ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Checked = pfini.getvalue("Setting", "tooltip", 0, 0, 1) == 1;
            int bootpos = pfini.getvalue("Setting", "position", 0, 0, 3);
            int bootposTop = pfini.getvalue("Setting", "Top", 0);
            int bootposLeft = pfini.getvalue("Setting", "Left", 0);
            int bootposFixTop = pfini.getvalue("Setting", "FixTop", 0);
            int bootposFixLeft = pfini.getvalue("Setting", "FixLeft", 0);
            pfini.WriteIniFile();

            //起動時位置
            this.StartPosition = FormStartPosition.Manual;
            if (bootpos == 0)
            {
                //マウスカーソルの近く
                this.マウスカーソルの近くToolStripMenuItem.Checked = true;
                this.Location = bootLocation();
            }
            else if (bootpos == 1)
            {
                //マウスカーソルの位置
                this.マウスカーソルの位置ToolStripMenuItem.Checked = true;
                this.Location = new Point(Cursor.Position.X - this.Width / 2, Cursor.Position.Y - 14);
            }
            else if (bootpos == 2)
            {
                //前回終了位置
                this.前回終了位置ToolStripMenuItem.Checked = true;
                this.Top = bootposTop;
                this.Left = bootposLeft;
            }
            else if (bootpos == 3)
            {
                //固定
                this.固定ToolStripMenuItem.Checked = true;
                this.Top = bootposFixTop;
                this.Left = bootposFixLeft;
            }

            //マウスで移動させる
            this.MouseDown += new MouseEventHandler(this.control_MouseDown);
            this.MouseMove += new MouseEventHandler(this.control_MouseMove);
            this.MouseUp += new MouseEventHandler(this.control_MouseUp);
            this.label_relaypoint.MouseDown += new MouseEventHandler(this.control_MouseDown);
            this.label_relaypoint.MouseMove += new MouseEventHandler(this.control_MouseMove);
            this.label_relaypoint.MouseUp += new MouseEventHandler(this.control_MouseUp);
            this.label_open.MouseDown += new MouseEventHandler(this.control_MouseDown);
            this.label_open.MouseMove += new MouseEventHandler(this.control_MouseMove);
            this.label_open.MouseUp += new MouseEventHandler(this.control_MouseUp);
            this.pictureBox_setting.MouseDown += new MouseEventHandler(this.control_MouseDown);
            this.pictureBox_setting.MouseMove += new MouseEventHandler(this.control_MouseMove);
            this.pictureBox_setting.MouseUp += new MouseEventHandler(this.control_MouseUp);

            //Ctrl+Cイベント
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(this.form_KeyDown);

            //ラベル
            this.label_relaypoint.Font = new Font(SystemInformation.MenuFont.FontFamily, 10);
            this.label_open.Font = SystemInformation.MenuFont;
            this.label_open.MouseClick += new MouseEventHandler(this.open_MouseClick);
            foreach (var control in this.Controls)
            {
                if (control.GetType() == typeof(Label))
                {
                    ((Label)control).AllowDrop = true;
                    ((Label)control).DragEnter += new DragEventHandler(this.control_DragEnter);
                    ((Label)control).DragDrop += new DragEventHandler(this.control_DragDrop);
                }
            }

            //設定ボタン
            this.pictureBox_setting.BackgroundImage = Properties.Resources.setting.ToBitmap();
            this.pictureBox_setting.BackgroundImageLayout = ImageLayout.Zoom;
            this.pictureBox_setting.MouseClick += new MouseEventHandler(this.pictureBox_setting_MouseClick);
        }

        #region event(MouseDown/Move/Up)

        private Point lastMousePosition;
        private bool mouseCapture;
        private bool click_ok = true;

        private void control_MouseDown(object sender, MouseEventArgs e)
        {
            click_ok = true;

            if (sender is Label && ((Label)sender).Name == this.label_relaypoint.Name && files_RelayPoint.Count >= 1)
            {
                //中継地点でありファイルが存在している場合
                if (isExists(files_RelayPoint))
                {
                    try
                    {
                        DataObject dataObj = new DataObject(DataFormats.FileDrop, files_RelayPoint.ToArray());
                        DragDropEffects effect = DragDropEffects.None | DragDropEffects.Link | DragDropEffects.All;
                        ListView dummy = new ListView();
                        dummy.DoDragDrop(dataObj, effect);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }

                if (!isMouseInWindow())
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
                click_ok = false;
            }

            //差分移動
            if (this.タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem.Checked)
            {
                this.Location = new Point(this.Left + offsetX, this.Top + offsetY);
            }

            lastMousePosition = mp;
        }

        private void control_MouseUp(object sender, MouseEventArgs e)
        {
            click_ok = true;

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            mouseCapture = false;
        }

        //マウスがウィンドウ内にいるか
        private bool isMouseInWindow()
        {
            Point mouse = this.PointToClient(Cursor.Position);
            Point winRectLocation = this.PointToClient(this.Bounds.Location);
            Rectangle winRect = new Rectangle(winRectLocation, this.Bounds.Size);

            if (this.ClientRectangle.Contains(mouse))
            {
                return true;
            }
            else if (this.ClientRectangle.Contains(new Point(mouse.X, 0)) && winRectLocation.Y <= mouse.Y && mouse.Y <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

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
                if (((Label)sender).Name == this.label_relaypoint.Name)
                {
                    //ファイルパスを追加
                    files_RelayPoint.AddRange((string[])e.Data.GetData(DataFormats.FileDrop, false));

                    //重複削除
                    files_RelayPoint = files_RelayPoint.Distinct().ToList();

                    //フォントサイズ変更
                    label_relaypoint.Font = new Font(SystemInformation.MenuFont.FontFamily, 12);

                    //ファイルカウント
                    int count_file = 0;
                    int count_dir = 0;

                    foreach (string file in files_RelayPoint)
                    {
                        if (File.Exists(file))
                        {
                            count_file++;
                        }
                        else if (Directory.Exists(file))
                        {
                            count_dir++;
                        }
                    }

                    label_relaypoint.Text = "";
                    if (count_file >= 1)
                    {
                        label_relaypoint.Text += count_file.ToString() + " file" + (count_file == 1 ? "" : "s");
                    }

                    if (count_dir >= 1)
                    {
                        if (count_file >= 1)
                        {
                            label_relaypoint.Text += System.Environment.NewLine;
                        }

                        label_relaypoint.Text += count_dir.ToString() + " folder" + (count_dir == 1 ? "" : "s");
                    }

                    //ツールチップ変更
                    if (this.ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Checked)
                    {
                        this.toolTip1.SetToolTip(this.label_relaypoint, string.Join(System.Environment.NewLine, files_RelayPoint.ToArray()));
                    }
                }
                else if (((Label)sender).Name == this.label_open.Name)
                {
                    //ファイルパス確保
                    List<string> dropfiles = new List<string>();
                    dropfiles.AddRange((string[])e.Data.GetData(DataFormats.FileDrop, false));

                    //重複削除
                    foreach (string file in dropfiles)
                    {
                        if (Directory.Exists(file) && !folders.Contains(file))
                        {
                            folders.Add(file);
                        }
                    }

                    //フォルダリスト更新
                    TextLib.TextFile.Write(folder_ini, string.Join(System.Environment.NewLine, folders), false, TextLib.TextFile.encoding_utf8);
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

            if (click_ok)
            {
                //メニュー作成
                ContextMenuStrip cmenu = makeContextMenuStrip(this.ToolStripMenuItem_folderopen, true);

                //メニュー表示
                Point p = this.label_open.PointToScreen(new Point(0, this.label_open.Height));
                cmenu.Show(p);
            }
        }

        //メニュー作成
        private ContextMenuStrip makeContextMenuStrip(System.Action<object, System.EventArgs> func, bool edit = false)
        {
            ContextMenuStrip cmenu = new ContextMenuStrip();

            if (edit)
            {
                ToolStripMenuItem item1 = new ToolStripMenuItem();
                item1.Text = "ダイアログで選択する";
                item1.Click += new System.EventHandler(func);
                cmenu.Items.Add(item1);
            }

            ToolStripMenuItem item2 = new ToolStripMenuItem();
            item2.Text = "デスクトップ";
            item2.Click += new System.EventHandler(func);
            cmenu.Items.Add(item2);

            bool full = this.フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem.Checked;
            if (folders.Count > 0)
            {
                ToolStripSeparator sep1 = new ToolStripSeparator();
                cmenu.Items.Add(sep1);

                int idx = -1;
                foreach (string folder in folders)
                {
                    idx++;
                    ToolStripMenuItem item = new ToolStripMenuItem();

                    //メニュー名
                    string text = folder;
                    try
                    {
                        if (!full)
                        {
                            int idx2 = -1;
                            text = Path.GetFileName(folder);
                            foreach (string otherfolder in folders)
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
                    item.Click += new System.EventHandler(func);
                    item.Enabled = Directory.Exists(folder);
                    cmenu.Items.Add(item);
                }
            }

            ToolStripSeparator sep2 = new ToolStripSeparator();
            cmenu.Items.Add(sep2);

            if (edit)
            {
                ToolStripMenuItem item3 = new ToolStripMenuItem();
                item3.Text = "編集";
                item3.Click += new System.EventHandler(func);
                cmenu.Items.Add(item3);
            }

            ToolStripMenuItem item4 = new ToolStripMenuItem();
            item4.Text = "キャンセル";
            item4.Click += new System.EventHandler(func);
            cmenu.Items.Add(item4);

            return cmenu;
        }

        //メニュー選択時
        private void ToolStripMenuItem_folderopen(object sender, System.EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            string folder = "";

            if (item.Text == "キャンセル")
            {
                return;
            }
            else if (item.Text == "編集")
            {
                if (!File.Exists(folder_ini))
                {
                    TextLib.TextFile.Write(folder_ini, "", false, TextLib.TextFile.encoding_utf8);
                }

                if (File.Exists(folder_ini))
                {
                    bool file_ok = true;

                    //更新日時確認
                    System.DateTime dt1 = System.DateTime.Now;
                    System.DateTime dt2 = System.DateTime.Now;
                    try
                    {
                        dt1 = File.GetLastWriteTime(folder_ini);
                    }
                    catch (System.Exception)
                    {
                        file_ok = false;
                    }

                    System.Diagnostics.Process p = System.Diagnostics.Process.Start(folder_ini);
                    p.WaitForExit();

                    //編集後の更新日時確認
                    try
                    {
                        dt2 = File.GetLastWriteTime(folder_ini);
                    }
                    catch (System.Exception)
                    {
                        file_ok = false;
                    }

                    if (file_ok)
                    {
                        System.TimeSpan ts1 = dt1 - dt2;

                        //差分が1.5秒以上の場合
                        if (System.Math.Abs(ts1.TotalSeconds) > 1.5)
                        {
                            try
                            {
                                read_folder_ini();
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
                    if (Directory.Exists(folders[idx]))
                    {
                        folder = folders[idx];
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
                if (files_RelayPoint.Count >= 1)
                {
                    copy_to_clipboard(files_RelayPoint);
                }
            }
        }

        private void copy_to_clipboard(List<string> files)
        {
            if (isExists(files))
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
        private bool isExists(List<string> checkfiles)
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

        private Point bootLocation()
        {
            int x, y;
            x = Cursor.Position.X - this.Width / 2;
            y = Cursor.Position.Y - this.Height / 2;

            Rectangle workarea = Screen.GetWorkingArea(this);
            Rectangle display = Screen.GetBounds(this);

            //confirm taskbar position
            bool correction = false;
            if (display.Width == workarea.Width)
            {
                if (workarea.Top == 0)
                {
                    //bottom
                    if (y >= workarea.Height - this.Height)
                    {
                        y = workarea.Height - this.Height;
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
                    if (x >= workarea.Width - this.Width)
                    {
                        x = workarea.Width - this.Width;
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

            if (x >= display.Width - this.Width)
            {
                x = display.Width - this.Width;
            }

            if (y <= 0)
            {
                y = 0;
            }

            if (y >= display.Height - this.Height)
            {
                y = display.Height - this.Height;
            }

            return new Point(x, y);
        }

        #endregion

        #region 設定

        private void read_folder_ini()
        {
            folders = new List<string>();

            string readall = TextLib.TextFile.Read(folder_ini);
            foreach (string str in readall.Replace(System.Environment.NewLine, "\n").Split('\n'))
            {
                string folder = str;
                if (folder.EndsWith(@"\"))
                {
                    folder = folder.Substring(0, folder.Length - 1);
                }

                folder.Trim();

                if (!folders.Contains(folder) && folder != "")
                {
                    folders.Add(folder);
                }
            }
            TextLib.TextFile.Write(folder_ini, string.Join(System.Environment.NewLine, folders), false, TextLib.TextFile.encoding_utf8);
        }

        //設定
        private void pictureBox_setting_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            //メニュー表示
            if (click_ok)
            {
                Point p = this.pictureBox_setting.PointToScreen(new Point(0, this.pictureBox_setting.Height));
                this.contextMenuStrip1.Show(p);
            }
        }

        private void タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem.Checked = !this.タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem.Checked;
            pfini.setvalue("Setting", "mousemove", this.タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem.Checked ? "1" : "0");
            pfini.WriteIniFile();
        }

        private void フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem.Checked = !this.フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem.Checked;
            pfini.setvalue("Setting", "fullpath", this.フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem.Checked ? "1" : "0");
            pfini.WriteIniFile();
        }

        private void BootPosToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.マウスカーソルの位置ToolStripMenuItem.Checked = false;
            this.マウスカーソルの近くToolStripMenuItem.Checked = false;
            this.前回終了位置ToolStripMenuItem.Checked = false;
            this.固定ToolStripMenuItem.Checked = false;

            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.Checked = true;

            int idx = 0;
            if (this.マウスカーソルの近くToolStripMenuItem.Checked)
            {
                idx = 0;
            }
            if (this.マウスカーソルの位置ToolStripMenuItem.Checked)
            {
                idx = 1;
            }
            if (this.前回終了位置ToolStripMenuItem.Checked)
            {
                idx = 2;
            }
            if (this.固定ToolStripMenuItem.Checked)
            {
                idx = 3;
                pfini.setvalue("Setting", "FixTop", this.Top.ToString());
                pfini.setvalue("Setting", "FixLeft", this.Left.ToString());
                MessageBox.Show("現在位置を保存しました。");
            }
            pfini.setvalue("Setting", "position", idx.ToString());
            pfini.WriteIniFile();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            pfini.setvalue("Setting", "Top", this.Top.ToString());
            pfini.setvalue("Setting", "Left", this.Left.ToString());
            pfini.WriteIniFile();
        }

        private void ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Checked = !this.ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Checked;
            pfini.setvalue("Setting", "tooltip", this.ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Checked ? "1" : "0");
            pfini.WriteIniFile();

            if (this.ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Checked)
            {
                if (files_RelayPoint.Count > 0)
                {
                    this.toolTip1.SetToolTip(this.label_relaypoint, string.Join(System.Environment.NewLine, files_RelayPoint.ToArray()));
                }
            }
            else
            {
                this.toolTip1.SetToolTip(this.label_relaypoint, "");
            }
        }
        #endregion

    }
}
