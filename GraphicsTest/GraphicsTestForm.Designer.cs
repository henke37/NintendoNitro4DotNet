namespace GraphicsTest {
	partial class GraphicsTestForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.ImgDisp = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.ImgDisp)).BeginInit();
			this.SuspendLayout();
			// 
			// ImgDisp
			// 
			this.ImgDisp.Location = new System.Drawing.Point(13, 13);
			this.ImgDisp.Name = "ImgDisp";
			this.ImgDisp.Size = new System.Drawing.Size(538, 425);
			this.ImgDisp.TabIndex = 0;
			this.ImgDisp.TabStop = false;
			// 
			// GraphicsTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.ImgDisp);
			this.Name = "GraphicsTestForm";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.ImgDisp)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox ImgDisp;
	}
}

