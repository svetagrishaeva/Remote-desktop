namespace ScreenViewer.Client {
    partial class Desktop {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Desktop));
            this.btnOperate = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.systemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.systemInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.taskManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.remoteShellToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtLine = new System.Windows.Forms.ToolStripSeparator();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shutdownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.surveillanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.remoteDesktopToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.userSupportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showMessageboxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.connectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOperate
            // 
            this.btnOperate.Location = new System.Drawing.Point(12, 780);
            this.btnOperate.Name = "btnOperate";
            this.btnOperate.Size = new System.Drawing.Size(75, 23);
            this.btnOperate.TabIndex = 1;
            this.btnOperate.Text = "START";
            this.btnOperate.UseVisualStyleBackColor = true;
            this.btnOperate.Click += new System.EventHandler(this.btnOperate_Click_1);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBox2.ContextMenuStrip = this.contextMenuStrip;
            this.pictureBox2.Location = new System.Drawing.Point(12, 31);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(1251, 743);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 8;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            this.pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.systemToolStripMenuItem,
            this.surveillanceToolStripMenuItem,
            this.userSupportToolStripMenuItem,
            this.lineToolStripMenuItem,
            this.connectionToolStripMenuItem});
            this.contextMenuStrip.Name = "ctxtMenu";
            this.contextMenuStrip.Size = new System.Drawing.Size(226, 114);
            // 
            // systemToolStripMenuItem
            // 
            this.systemToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.systemInformationToolStripMenuItem,
            this.fileManagerToolStripMenuItem,
            this.taskManagerToolStripMenuItem,
            this.remoteShellToolStripMenuItem,
            this.ctxtLine,
            this.actionsToolStripMenuItem});
            this.systemToolStripMenuItem.Name = "systemToolStripMenuItem";
            this.systemToolStripMenuItem.Size = new System.Drawing.Size(225, 26);
            this.systemToolStripMenuItem.Text = "Администрация";
            // 
            // systemInformationToolStripMenuItem
            // 
            this.systemInformationToolStripMenuItem.Name = "systemInformationToolStripMenuItem";
            this.systemInformationToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            // 
            // fileManagerToolStripMenuItem
            // 
            this.fileManagerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("fileManagerToolStripMenuItem.Image")));
            this.fileManagerToolStripMenuItem.Name = "fileManagerToolStripMenuItem";
            this.fileManagerToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.fileManagerToolStripMenuItem.Text = "Файловый менеджер";
            // 
            // taskManagerToolStripMenuItem
            // 
            this.taskManagerToolStripMenuItem.Name = "taskManagerToolStripMenuItem";
            this.taskManagerToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.taskManagerToolStripMenuItem.Text = "Диспетчер задач";
            // 
            // remoteShellToolStripMenuItem
            // 
            this.remoteShellToolStripMenuItem.Name = "remoteShellToolStripMenuItem";
            this.remoteShellToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            // 
            // ctxtLine
            // 
            this.ctxtLine.Name = "ctxtLine";
            this.ctxtLine.Size = new System.Drawing.Size(188, 6);
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.shutdownToolStripMenuItem,
            this.restartToolStripMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.actionsToolStripMenuItem.Text = "Действие";
            // 
            // shutdownToolStripMenuItem
            // 
            this.shutdownToolStripMenuItem.Name = "shutdownToolStripMenuItem";
            this.shutdownToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.shutdownToolStripMenuItem.Text = "Выключение";
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.restartToolStripMenuItem.Text = "Перезапуск";
            // 
            // surveillanceToolStripMenuItem
            // 
            this.surveillanceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.remoteDesktopToolStripMenuItem2});
            this.surveillanceToolStripMenuItem.Name = "surveillanceToolStripMenuItem";
            this.surveillanceToolStripMenuItem.Size = new System.Drawing.Size(225, 26);
            this.surveillanceToolStripMenuItem.Text = "Мониторинг";
            // 
            // remoteDesktopToolStripMenuItem2
            // 
            this.remoteDesktopToolStripMenuItem2.Image = ((System.Drawing.Image)(resources.GetObject("remoteDesktopToolStripMenuItem2.Image")));
            this.remoteDesktopToolStripMenuItem2.Name = "remoteDesktopToolStripMenuItem2";
            this.remoteDesktopToolStripMenuItem2.Size = new System.Drawing.Size(215, 22);
            this.remoteDesktopToolStripMenuItem2.Text = "Удаленный рабочий стол";
            // 
            // userSupportToolStripMenuItem
            // 
            this.userSupportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showMessageboxToolStripMenuItem});
            this.userSupportToolStripMenuItem.Name = "userSupportToolStripMenuItem";
            this.userSupportToolStripMenuItem.Size = new System.Drawing.Size(225, 26);
            this.userSupportToolStripMenuItem.Text = "Поддержка пользователей";
            // 
            // showMessageboxToolStripMenuItem
            // 
            this.showMessageboxToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showMessageboxToolStripMenuItem.Image")));
            this.showMessageboxToolStripMenuItem.Name = "showMessageboxToolStripMenuItem";
            this.showMessageboxToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.showMessageboxToolStripMenuItem.Text = "Показать окно сообщения";
            // 
            // lineToolStripMenuItem
            // 
            this.lineToolStripMenuItem.Name = "lineToolStripMenuItem";
            this.lineToolStripMenuItem.Size = new System.Drawing.Size(222, 6);
            // 
            // connectionToolStripMenuItem
            // 
            this.connectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reconnectToolStripMenuItem,
            this.disconnectToolStripMenuItem,
            this.uninstallToolStripMenuItem});
            this.connectionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("connectionToolStripMenuItem.Image")));
            this.connectionToolStripMenuItem.Name = "connectionToolStripMenuItem";
            this.connectionToolStripMenuItem.Size = new System.Drawing.Size(225, 26);
            this.connectionToolStripMenuItem.Text = "Управление клиентами";
            // 
            // reconnectToolStripMenuItem
            // 
            this.reconnectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("reconnectToolStripMenuItem.Image")));
            this.reconnectToolStripMenuItem.Name = "reconnectToolStripMenuItem";
            this.reconnectToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.reconnectToolStripMenuItem.Text = "Восстанавливать соединение";
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("disconnectToolStripMenuItem.Image")));
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.disconnectToolStripMenuItem.Text = "Разъединять соединение";
            // 
            // uninstallToolStripMenuItem
            // 
            this.uninstallToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("uninstallToolStripMenuItem.Image")));
            this.uninstallToolStripMenuItem.Name = "uninstallToolStripMenuItem";
            this.uninstallToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.uninstallToolStripMenuItem.Text = "Удалить";
            // 
            // Desktop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1275, 806);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.btnOperate);
            this.KeyPreview = true;
            this.Name = "Desktop";
            this.Text = "Рабочий стол";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOperate;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem systemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem systemInformationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileManagerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem taskManagerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem remoteShellToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator ctxtLine;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shutdownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem surveillanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem remoteDesktopToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem userSupportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showMessageboxToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator lineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uninstallToolStripMenuItem;
    }
}

