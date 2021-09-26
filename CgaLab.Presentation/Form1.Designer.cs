
namespace CgaLab.Presentation
{
    partial class FormACG
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ModelPictureBox = new System.Windows.Forms.PictureBox();
            this.ModelOpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.DrawTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ModelPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ModelPictureBox
            // 
            this.ModelPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ModelPictureBox.Location = new System.Drawing.Point(-1, -1);
            this.ModelPictureBox.Name = "ModelPictureBox";
            this.ModelPictureBox.Size = new System.Drawing.Size(800, 447);
            this.ModelPictureBox.TabIndex = 0;
            this.ModelPictureBox.TabStop = false;
            this.ModelPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ModelPictureBox_MouseDown);
            this.ModelPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ModelPictureBox_MouseMove);
            this.ModelPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ModelPictureBox_MouseUp);
            // 
            // ModelOpenDialog
            // 
            this.ModelOpenDialog.Filter = "Obj files|*.obj";
            // 
            // DrawTimer
            // 
            this.DrawTimer.Interval = 20;
            this.DrawTimer.Tick += new System.EventHandler(this.DrawTimer_Tick);
            // 
            // FormACG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1089, 490);
            this.Controls.Add(this.ModelPictureBox);
            this.Name = "FormACG";
            this.Text = "ACG";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormACG_KeyDown);
            this.Resize += new System.EventHandler(this.FormACG_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.ModelPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox ModelPictureBox;
        private System.Windows.Forms.OpenFileDialog ModelOpenDialog;
        private System.Windows.Forms.Timer DrawTimer;
    }
}

