namespace Raverus.FiskalizacijaDEV.ConnectionTest
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comMjesto = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnStartTest = new System.Windows.Forms.Button();
            this.txtCertifikat = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.comInternetProvideri = new System.Windows.Forms.ComboBox();
            this.comTipVeze = new System.Windows.Forms.ComboBox();
            this.comOs = new System.Windows.Forms.ComboBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDatoteka = new System.Windows.Forms.Button();
            this.txtZaporka = new System.Windows.Forms.TextBox();
            this.txtDatoteka = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnPosalji = new System.Windows.Forms.Button();
            this.btnPogledaj = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comMjesto
            // 
            this.comMjesto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comMjesto.FormattingEnabled = true;
            this.comMjesto.Location = new System.Drawing.Point(105, 12);
            this.comMjesto.Name = "comMjesto";
            this.comMjesto.Size = new System.Drawing.Size(254, 21);
            this.comMjesto.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Mjesto:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Internet provider:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Tip veze:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "OS:";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(12, 292);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(390, 163);
            this.txtLog.TabIndex = 5;
            // 
            // btnStartTest
            // 
            this.btnStartTest.Location = new System.Drawing.Point(12, 490);
            this.btnStartTest.Name = "btnStartTest";
            this.btnStartTest.Size = new System.Drawing.Size(105, 23);
            this.btnStartTest.TabIndex = 6;
            this.btnStartTest.Text = "Start test";
            this.btnStartTest.UseVisualStyleBackColor = true;
            this.btnStartTest.Click += new System.EventHandler(this.btnStartTest_Click);
            // 
            // txtCertifikat
            // 
            this.txtCertifikat.Location = new System.Drawing.Point(29, 42);
            this.txtCertifikat.Name = "txtCertifikat";
            this.txtCertifikat.Size = new System.Drawing.Size(100, 20);
            this.txtCertifikat.TabIndex = 8;
            this.txtCertifikat.Text = "FISKAL 1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Raverus.FiskalizacijaDEV.ConnectionTest.Properties.Resources.ajax_loader;
            this.pictureBox1.Location = new System.Drawing.Point(365, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(37, 23);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 40;
            this.pictureBox1.TabStop = false;
            // 
            // comInternetProvideri
            // 
            this.comInternetProvideri.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comInternetProvideri.FormattingEnabled = true;
            this.comInternetProvideri.Location = new System.Drawing.Point(105, 44);
            this.comInternetProvideri.Name = "comInternetProvideri";
            this.comInternetProvideri.Size = new System.Drawing.Size(254, 21);
            this.comInternetProvideri.TabIndex = 41;
            // 
            // comTipVeze
            // 
            this.comTipVeze.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comTipVeze.FormattingEnabled = true;
            this.comTipVeze.Location = new System.Drawing.Point(105, 75);
            this.comTipVeze.Name = "comTipVeze";
            this.comTipVeze.Size = new System.Drawing.Size(254, 21);
            this.comTipVeze.TabIndex = 42;
            // 
            // comOs
            // 
            this.comOs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comOs.FormattingEnabled = true;
            this.comOs.Location = new System.Drawing.Point(105, 106);
            this.comOs.Name = "comOs";
            this.comOs.Size = new System.Drawing.Size(254, 21);
            this.comOs.TabIndex = 43;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Raverus.FiskalizacijaDEV.ConnectionTest.Properties.Resources.ajax_loader;
            this.pictureBox2.Location = new System.Drawing.Point(365, 42);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(37, 23);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 44;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::Raverus.FiskalizacijaDEV.ConnectionTest.Properties.Resources.ajax_loader;
            this.pictureBox3.Location = new System.Drawing.Point(365, 73);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(37, 23);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox3.TabIndex = 45;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::Raverus.FiskalizacijaDEV.ConnectionTest.Properties.Resources.ajax_loader;
            this.pictureBox4.Location = new System.Drawing.Point(365, 104);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(37, 23);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox4.TabIndex = 46;
            this.pictureBox4.TabStop = false;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(6, 19);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(98, 17);
            this.radioButton1.TabIndex = 47;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Certificate store";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(6, 69);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(69, 17);
            this.radioButton2.TabIndex = 48;
            this.radioButton2.Text = "Datoteka";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnDatoteka);
            this.groupBox1.Controls.Add(this.txtZaporka);
            this.groupBox1.Controls.Add(this.txtDatoteka);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.txtCertifikat);
            this.groupBox1.Location = new System.Drawing.Point(15, 133);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(387, 153);
            this.groupBox1.TabIndex = 49;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Certifikat";
            // 
            // btnDatoteka
            // 
            this.btnDatoteka.Enabled = false;
            this.btnDatoteka.Location = new System.Drawing.Point(354, 89);
            this.btnDatoteka.Name = "btnDatoteka";
            this.btnDatoteka.Size = new System.Drawing.Size(27, 23);
            this.btnDatoteka.TabIndex = 50;
            this.btnDatoteka.Text = "...";
            this.btnDatoteka.UseVisualStyleBackColor = true;
            this.btnDatoteka.Click += new System.EventHandler(this.btnDatoteka_Click);
            // 
            // txtZaporka
            // 
            this.txtZaporka.Enabled = false;
            this.txtZaporka.Location = new System.Drawing.Point(29, 118);
            this.txtZaporka.Name = "txtZaporka";
            this.txtZaporka.PasswordChar = '*';
            this.txtZaporka.Size = new System.Drawing.Size(100, 20);
            this.txtZaporka.TabIndex = 50;
            // 
            // txtDatoteka
            // 
            this.txtDatoteka.Enabled = false;
            this.txtDatoteka.Location = new System.Drawing.Point(29, 92);
            this.txtDatoteka.Name = "txtDatoteka";
            this.txtDatoteka.Size = new System.Drawing.Size(319, 20);
            this.txtDatoteka.TabIndex = 49;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 461);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(390, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 50;
            // 
            // btnPosalji
            // 
            this.btnPosalji.Location = new System.Drawing.Point(154, 490);
            this.btnPosalji.Name = "btnPosalji";
            this.btnPosalji.Size = new System.Drawing.Size(105, 23);
            this.btnPosalji.TabIndex = 51;
            this.btnPosalji.Text = "Pošalji rezultate";
            this.btnPosalji.UseVisualStyleBackColor = true;
            this.btnPosalji.Click += new System.EventHandler(this.btnPosalji_Click);
            // 
            // btnPogledaj
            // 
            this.btnPogledaj.Location = new System.Drawing.Point(296, 490);
            this.btnPogledaj.Name = "btnPogledaj";
            this.btnPogledaj.Size = new System.Drawing.Size(105, 23);
            this.btnPogledaj.TabIndex = 52;
            this.btnPogledaj.Text = "Pogledaj rezultate";
            this.btnPogledaj.UseVisualStyleBackColor = true;
            this.btnPogledaj.Click += new System.EventHandler(this.btnPogledaj_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 523);
            this.Controls.Add(this.btnPogledaj);
            this.Controls.Add(this.btnPosalji);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.comOs);
            this.Controls.Add(this.comTipVeze);
            this.Controls.Add(this.comInternetProvideri);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnStartTest);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comMjesto);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Provjera odziva CIS-a (fiskalizacija)";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comMjesto;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnStartTest;
        private System.Windows.Forms.TextBox txtCertifikat;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox comInternetProvideri;
        private System.Windows.Forms.ComboBox comTipVeze;
        private System.Windows.Forms.ComboBox comOs;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnDatoteka;
        private System.Windows.Forms.TextBox txtZaporka;
        private System.Windows.Forms.TextBox txtDatoteka;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnPosalji;
        private System.Windows.Forms.Button btnPogledaj;
    }
}