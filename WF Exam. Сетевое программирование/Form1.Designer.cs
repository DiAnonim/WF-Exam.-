namespace WF_Exam.Сетевое_программирование
{
    partial class Checkers
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.rbIsServer = new System.Windows.Forms.RadioButton();
            this.rbIsClient = new System.Windows.Forms.RadioButton();
            this.btnStartGame = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lbJournal = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // rbIsServer
            // 
            this.rbIsServer.AutoSize = true;
            this.rbIsServer.CheckAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.rbIsServer.Location = new System.Drawing.Point(569, 110);
            this.rbIsServer.Name = "rbIsServer";
            this.rbIsServer.Size = new System.Drawing.Size(149, 20);
            this.rbIsServer.TabIndex = 0;
            this.rbIsServer.TabStop = true;
            this.rbIsServer.Text = "Запустить Сервер";
            this.rbIsServer.UseVisualStyleBackColor = true;
            // 
            // rbIsClient
            // 
            this.rbIsClient.AutoSize = true;
            this.rbIsClient.CheckAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.rbIsClient.Location = new System.Drawing.Point(569, 136);
            this.rbIsClient.Name = "rbIsClient";
            this.rbIsClient.Size = new System.Drawing.Size(191, 20);
            this.rbIsClient.TabIndex = 1;
            this.rbIsClient.TabStop = true;
            this.rbIsClient.Text = "Подключиться к серверу";
            this.rbIsClient.UseVisualStyleBackColor = true;
            // 
            // btnStartGame
            // 
            this.btnStartGame.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnStartGame.Location = new System.Drawing.Point(569, 24);
            this.btnStartGame.Name = "btnStartGame";
            this.btnStartGame.Size = new System.Drawing.Size(191, 65);
            this.btnStartGame.TabIndex = 2;
            this.btnStartGame.Text = "Start Game";
            this.btnStartGame.UseVisualStyleBackColor = false;
            this.btnStartGame.Click += new System.EventHandler(this.btnStartGame_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnClose.Location = new System.Drawing.Point(794, 24);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(191, 65);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lbJournal
            // 
            this.lbJournal.FormattingEnabled = true;
            this.lbJournal.HorizontalScrollbar = true;
            this.lbJournal.ItemHeight = 16;
            this.lbJournal.Location = new System.Drawing.Point(569, 192);
            this.lbJournal.Name = "lbJournal";
            this.lbJournal.Size = new System.Drawing.Size(519, 292);
            this.lbJournal.TabIndex = 6;
            // 
            // Checkers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 498);
            this.Controls.Add(this.lbJournal);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnStartGame);
            this.Controls.Add(this.rbIsClient);
            this.Controls.Add(this.rbIsServer);
            this.Name = "Checkers";
            this.Text = "Checkers";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbIsServer;
        private System.Windows.Forms.RadioButton rbIsClient;
        private System.Windows.Forms.Button btnStartGame;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ListBox lbJournal;
    }
}

