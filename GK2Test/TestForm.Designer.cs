namespace GK2Test {
	partial class TestForm {
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
			this.imgDisp = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.imgDisp)).BeginInit();
			this.SuspendLayout();
			// 
			// imgDisp
			// 
			this.imgDisp.Location = new System.Drawing.Point(12, 12);
			this.imgDisp.Name = "imgDisp";
			this.imgDisp.Size = new System.Drawing.Size(776, 426);
			this.imgDisp.TabIndex = 0;
			this.imgDisp.TabStop = false;
			// 
			// TestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.imgDisp);
			this.Name = "TestForm";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.imgDisp)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox imgDisp;
	}
}

