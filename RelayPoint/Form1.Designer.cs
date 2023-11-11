namespace RelayPoint
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelRelaypoint = new System.Windows.Forms.Label();
            this.labelFolderOpen = new System.Windows.Forms.Label();
            this.pictureBoxSetting = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.起動時の位置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.マウスカーソルの近くToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.マウスカーソルの位置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.前回終了位置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.固定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSetting)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelRelaypoint
            // 
            this.labelRelaypoint.BackColor = System.Drawing.Color.PaleTurquoise;
            this.labelRelaypoint.Location = new System.Drawing.Point(2, 2);
            this.labelRelaypoint.Name = "labelRelaypoint";
            this.labelRelaypoint.Size = new System.Drawing.Size(120, 120);
            this.labelRelaypoint.TabIndex = 0;
            this.labelRelaypoint.Text = "Drop here\r\n&&\r\nDrag out";
            this.labelRelaypoint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelFolderOpen
            // 
            this.labelFolderOpen.BackColor = System.Drawing.SystemColors.Window;
            this.labelFolderOpen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelFolderOpen.Location = new System.Drawing.Point(2, 124);
            this.labelFolderOpen.Name = "labelFolderOpen";
            this.labelFolderOpen.Size = new System.Drawing.Size(98, 20);
            this.labelFolderOpen.TabIndex = 0;
            this.labelFolderOpen.Text = "フォルダを開く";
            this.labelFolderOpen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip1.SetToolTip(this.labelFolderOpen, "ドラッグ＆ドロップで登録");
            // 
            // pictureBoxSetting
            // 
            this.pictureBoxSetting.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxSetting.Location = new System.Drawing.Point(102, 124);
            this.pictureBoxSetting.Name = "pictureBoxSetting";
            this.pictureBoxSetting.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxSetting.TabIndex = 1;
            this.pictureBoxSetting.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem,
            this.起動時の位置ToolStripMenuItem,
            this.toolStripSeparator1,
            this.ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem,
            this.フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(325, 120);
            // 
            // タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem
            // 
            this.タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem.Name = "タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem";
            this.タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem.Size = new System.Drawing.Size(324, 22);
            this.タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem.Text = "タイトルバー以外でもマウスによるウィンドウ移動を許可";
            this.タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem.Click += new System.EventHandler(this.タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem_Click);
            // 
            // フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem
            // 
            this.フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem.Name = "フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem";
            this.フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem.Size = new System.Drawing.Size(324, 22);
            this.フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem.Text = "フォルダを開くメニューでフォルダのフルパスを表示";
            this.フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem.Click += new System.EventHandler(this.フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem_Click);
            // 
            // 起動時の位置ToolStripMenuItem
            // 
            this.起動時の位置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.マウスカーソルの近くToolStripMenuItem,
            this.マウスカーソルの位置ToolStripMenuItem,
            this.前回終了位置ToolStripMenuItem,
            this.固定ToolStripMenuItem});
            this.起動時の位置ToolStripMenuItem.Name = "起動時の位置ToolStripMenuItem";
            this.起動時の位置ToolStripMenuItem.Size = new System.Drawing.Size(324, 22);
            this.起動時の位置ToolStripMenuItem.Text = "起動時の位置";
            // 
            // マウスカーソルの近くToolStripMenuItem
            // 
            this.マウスカーソルの近くToolStripMenuItem.Name = "マウスカーソルの近くToolStripMenuItem";
            this.マウスカーソルの近くToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.マウスカーソルの近くToolStripMenuItem.Text = "マウスカーソルの近く";
            this.マウスカーソルの近くToolStripMenuItem.Click += new System.EventHandler(this.bootPositionToolStripMenuItem_Click);
            // 
            // マウスカーソルの位置ToolStripMenuItem
            // 
            this.マウスカーソルの位置ToolStripMenuItem.Name = "マウスカーソルの位置ToolStripMenuItem";
            this.マウスカーソルの位置ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.マウスカーソルの位置ToolStripMenuItem.Text = "マウスカーソルの位置";
            this.マウスカーソルの位置ToolStripMenuItem.Click += new System.EventHandler(this.bootPositionToolStripMenuItem_Click);
            // 
            // 前回終了位置ToolStripMenuItem
            // 
            this.前回終了位置ToolStripMenuItem.Name = "前回終了位置ToolStripMenuItem";
            this.前回終了位置ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.前回終了位置ToolStripMenuItem.Text = "前回終了位置";
            this.前回終了位置ToolStripMenuItem.Click += new System.EventHandler(this.bootPositionToolStripMenuItem_Click);
            // 
            // 固定ToolStripMenuItem
            // 
            this.固定ToolStripMenuItem.Name = "固定ToolStripMenuItem";
            this.固定ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.固定ToolStripMenuItem.Text = "固定";
            this.固定ToolStripMenuItem.Click += new System.EventHandler(this.bootPositionToolStripMenuItem_Click);
            // 
            // ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem
            // 
            this.ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Name = "ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem";
            this.ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Size = new System.Drawing.Size(324, 22);
            this.ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Text = "ドロップされたファイルのリストをツールチップで表示";
            this.ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.ToolTipText = "中継点の上にマウスが来た時にドロップされたファイル、フォルダのリストをツールチップで表示します";
            this.ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem.Click += new System.EventHandler(this.ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(321, 6);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(124, 146);
            this.Controls.Add(this.pictureBoxSetting);
            this.Controls.Add(this.labelFolderOpen);
            this.Controls.Add(this.labelRelaypoint);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSetting)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelRelaypoint;
        private System.Windows.Forms.Label labelFolderOpen;
        private System.Windows.Forms.PictureBox pictureBoxSetting;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem タイトルバー以外でもマウスによるウィンドウ移動を許可ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem フォルダを開くメニューでフォルダのフルパスを表示ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 起動時の位置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem マウスカーソルの位置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem マウスカーソルの近くToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 前回終了位置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 固定ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ドロップされたファイルのリストをツールチップで表示ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

